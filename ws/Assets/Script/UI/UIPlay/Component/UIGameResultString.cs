using UnityEngine;
using System.Collections;
using System;

public class UIGameResultString : MonoBehaviour 
{
	public bool autoHide = true;

	public Xint testType;



	public void test()
	{
		callback = null;
		init(testType, GameType.Mode.Epic);
	}


	public UISprite spResult;

	public ParticleSystem pcClear;
	public ParticleSystem pcFail;

	GameType.Mode _playMode;


	public UISprite spBgWin;
	public UISprite spBgLose;


	public GameObject targetFSM;
	public string targetFSMName;

	public float fsmDelay = 0.0f;


	public delegate void CallbackAfterResult();
	public CallbackAfterResult callback;

	private float _callbackDelay = -1.0f;
	public void init(Xint type, GameType.Mode playMode, float closeTime = 2.0f, float callbackDelay = -1.0f)
	{
#if !UNITY_EDITOR
		autoHide = true;
#endif

		_callbackDelay = callbackDelay;

		pcClear.gameObject.SetActive(false);
		pcFail.gameObject.SetActive(false);

		gameObject.SetActive(true);

		GameManager.setTimeScale = 1.0f;

		spResult.color = new Color(1,1,1,0.0f);
		_playMode = playMode;

		bool isParticle = false;

		switch(type)
		{
		case Result.Type.GameOver:
			spResult.enabled = true;
			spResult.spriteName = "img_result_gameover";

			pcFail.gameObject.SetActive(true);
			pcFail.Play();

			spBgWin.gameObject.SetActive(false);
			spBgLose.gameObject.SetActive(true);

			isParticle = false;
			break;
		case Result.Type.TimeOver:
			spResult.enabled = true;
			spResult.spriteName = "img_result_timeover";

			spBgWin.gameObject.SetActive(false);
			spBgLose.gameObject.SetActive(true);

			pcFail.gameObject.SetActive(true);
			pcFail.Play();

			isParticle = false;
			break;


		case Result.Type.Finish:
			spResult.enabled = true;
			spResult.spriteName = "img_result_finish";
			
			pcClear.gameObject.SetActive(true);
			pcClear.Play();
			
			spBgWin.gameObject.SetActive(true);
			spBgLose.gameObject.SetActive(false);

			isParticle = false;

			SoundData.play("m_finish");

			break;


		case Result.Type.Clear:
			spResult.enabled = true;

			spResult.spriteName = "img_result_epic_clear";
			
			spBgWin.gameObject.SetActive(true);
			spBgLose.gameObject.SetActive(false);
			
			pcClear.gameObject.SetActive(true);
			pcClear.Play();

			SoundData.play("uiet_clear");

			break;
		case Result.Type.Lose:
			spResult.enabled = true;
			spResult.spriteName = "img_result_lose";
			isParticle = false;

			pcFail.gameObject.SetActive(true);
			pcFail.Play();

			spBgWin.gameObject.SetActive(false);
			spBgLose.gameObject.SetActive(true);

			SoundData.play("f_lose");

			break;
		case Result.Type.Win:
			spResult.enabled = true;
			spResult.spriteName = "img_result_win";
			isParticle = false;

			pcClear.gameObject.SetActive(true);
			pcClear.Play();

			spBgWin.gameObject.SetActive(true);
			spBgLose.gameObject.SetActive(false);

			SoundData.play("f_win");

			break;

		case Result.Type.Perfect:
			spResult.enabled = true;
			spResult.spriteName = "img_result_perpect";
			isParticle = false;
			
			pcClear.gameObject.SetActive(true);
			pcClear.Play();
			
			spBgWin.gameObject.SetActive(true);
			spBgLose.gameObject.SetActive(false);
			
			break;


		case Result.Type.Fail:
			spResult.enabled = true;

			spResult.spriteName = "img_result_epic_fail";
			isParticle = false;
			
			spBgWin.gameObject.SetActive(false);
			spBgLose.gameObject.SetActive(true);

			pcFail.gameObject.SetActive(true);
			pcFail.Play();

			break;
		}

		if(isParticle == false)
		{
			spResult.MakePixelPerfect();

			spResult.width = Mathf.RoundToInt(spResult.width * 1.43f);
			spResult.height = Mathf.RoundToInt(spResult.height * 1.43f);

			_v.x = 0.3f;
			_v.y = 0.3f;
			_v.z = 1.0f;
			spResult.transform.localScale = _v;
			
			_v.x = 1.0f;_v.y = 1.0f;_v.z = 1.0f;

			spResult.color = new Color(1,1,1,0.2f);
			TweenAlpha tp = TweenAlpha.Begin(spResult.gameObject, 0.6f, 1.0f);
			tp.eventReceiver = gameObject;

			TweenScale.Begin(spResult.gameObject, 0.6f, _v).method = UITweener.Method.BounceIn;

			if(autoHide) Invoke ("sequence2", 1.2f);
			Invoke ("onComplete", closeTime);
		}
		else
		{
			Invoke("onComplete", 2.5f);
		}


		if(_callbackDelay >= 0)
		{
			Invoke("onCallback", _callbackDelay);
		}

		if(targetFSM != null)
		{
			Invoke("playFSM", fsmDelay);
		}
	}

	private PlayMakerFSM _targetFSM;
	void playFSM()
	{
		if(_targetFSM == null)
		{
			PlayMakerFSM[] fsms = targetFSM.GetComponents<PlayMakerFSM>();

			if(fsms != null)
			{
				foreach(PlayMakerFSM f in fsms)
				{
					if(f.FsmName == targetFSMName)
					{
						_targetFSM = f;
					}
				}
			}

		}

		if(_targetFSM != null) _targetFSM.enabled = true;
	}





	Vector3 _v = new Vector3();

	public void sequence2()
	{
		TweenAlpha tp = TweenAlpha.Begin(spResult.gameObject, 0.5f, 0.0f);
		tp.eventReceiver = gameObject;
		tp.callWhenFinished = "sequence3";
	}

	void sequence3()
	{

	}


	public void onCallback()
	{
		if(callback != null)
		{
			callback();
		}
	}


	void onComplete()
	{
		if(callback != null)
		{
			if(_callbackDelay < 0) callback();
			callback = null;
			gameObject.SetActive(false);
		}
	}
}
