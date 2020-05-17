using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UIPlayStageClear : MonoBehaviour 
{
	public Transform effectRoot;
	
	public Transform[] effectTfs;
	public ParticleSystem[] effectParticles;
	public Animation[] effectAnis;
	
	public PlayMakerFSM[] effectFSMs;
	
	Dictionary<Transform, ObjectDefaultInformation> _objectDefaultInfo = new Dictionary<Transform, ObjectDefaultInformation>();
	Dictionary<Transform, bool> _fsmActive = new Dictionary<Transform, bool>();
	Dictionary<AnimationState, float> _aniSpeed = new Dictionary<AnimationState, float>();

	public PlayMakerFSM startFsm;


	void Awake()
	{
		saveDefaultTransformData();
	}


	void saveDefaultTransformData()
	{
		if(effectTfs != null)
		{
			foreach(Transform tf in effectTfs)
			{
				ObjectDefaultInformation info = new ObjectDefaultInformation();
				info.isActive = tf.gameObject.activeSelf;
				info.localPosition = tf.localPosition;
				info.localScale = tf.localScale;
				info.localRotation = tf.localRotation;
				_objectDefaultInfo[tf] = info;
			}
		}
		
		if(effectFSMs != null)
		{
			foreach(PlayMakerFSM f in effectFSMs)
			{
				_fsmActive[f.transform] = f.enabled;
			}
		}
		
		if(effectAnis != null)
		{
			foreach(Animation f in effectAnis)
			{
				foreach(AnimationState s in f)
				{
					_aniSpeed[s] = s.speed;
				}
			}
		}
	}



	public void collectUI()
	{
		effectTfs = effectRoot.GetComponentsInChildren<Transform>(true);
		effectParticles = effectRoot.GetComponentsInChildren<ParticleSystem>(true);
		effectFSMs = effectRoot.GetComponentsInChildren<PlayMakerFSM>(true);
		effectAnis = effectRoot.GetComponentsInChildren<Animation>(true);
	}

	public void resetPlayMaker()
	{
		if(_isPlay)
		{
			reset(effectTfs, effectParticles, effectFSMs, effectAnis);
			_isPlay = false;
		}

		gameObject.SetActive(false);
	}
	
	void reset(Transform[] tfs, ParticleSystem[] ps, PlayMakerFSM[] fsms, Animation[] anis)
	{
		if(tfs != null)
		{
			ObjectDefaultInformation info;
			foreach(Transform tf in tfs)
			{
				info = _objectDefaultInfo[tf];
				tf.gameObject.SetActive(info.isActive);
				tf.localPosition = info.localPosition;
				tf.localScale = info.localScale;
				tf.localRotation = info.localRotation;
			}
		}
		
		if(ps != null)
		{
			foreach(ParticleSystem p in ps)
			{
				p.Stop();
				p.Clear();
				p.Play();
			}
		}
		
		if(fsms != null)
		{
			foreach(PlayMakerFSM f in fsms)
			{
				f.enabled = _fsmActive[f.transform];
			}
		}
		
		if(anis != null)
		{
			foreach(Animation f in anis)
			{
				foreach(AnimationState s in f)
				{
					s.speed = _aniSpeed[s];
				}
			}
		}
	}



	private bool _isPlay = false;

	public void play()
	{
		gameObject.SetActive(true);
		_isPlay = true;
		startFsm.enabled = true;

		UIPlaySkillSlot.hideAllSlotEffect();
	}


	public void onOpenRoundClearPopup()
	{
		Debug.Log("onOpenRoundClearPopup");

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic || GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong)
		{
			GameManager.me.checkMissionNoticeAndOpenRoundClearPopup();
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Hell)
		{
			GameManager.me.uiManager.popupHellResult.showHellResult();
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Friendly)
		{
			GameManager.me.uiManager.popupChampionshipResult.open(true);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Championship)
		{
			GameManager.me.uiManager.popupChampionshipResult.open(false);
		}

	}



}
