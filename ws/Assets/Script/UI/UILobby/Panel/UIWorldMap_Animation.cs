using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class UIWorldMap : UIBase 
{


	//=== 드래그 애니메이션 ===//

	float _lastTime = 0.0f;
	bool _canScrollBg = true;
	bool _isScrollEffect = false;
	float _startTouchX = 0;
	
	public void OnPress(GameObject go, bool isPress)
//	void OnPress(bool isPress)
	{
		if(TutorialManager.instance.isTutorialMode) return;
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;

		if(isPress)
		{
			_lastTime = RealTime.time;
			_isScrollEffect = false;
			_lastScrollDelta = 0.0f;
			_startTouchX = Input.mousePosition.x;

			friendList.gameObject.SetActive(false);
			friendDetailButton.gameObject.SetActive(false);

		}
		else
		{
//			Debug.Log("_lastScrollDelta : " + _lastScrollDelta);

			if(MathUtil.abs(_startTouchX , Input.mousePosition.x) < 30.0f) return;

			_timeDelta = RealTime.time - _lastTime;
			
			float timeValue = 1.0f/_timeDelta;
			
			if(timeValue > 100.0f) timeValue = 100.0f;
			else if(timeValue < 1.0f) timeValue = 1.0f;

			if(RealTime.time - _lastDragTime >= 0.5f) _lastScrollDelta = 0.0f;

			if(Mathf.Abs(_lastScrollDelta + ((_lastScrollDelta > 0)?timeValue:-timeValue) ) > 10.0f)
			{
				timeValue *= timeValue;
				
				if(timeValue > 100.0f) timeValue = 100.0f;
				
				_lastScrollDelta += (_lastScrollDelta > 0)?timeValue:-timeValue * 0.475f;
				
				_isScrollEffect = true;
			}
			else
			{
				_isScrollEffect = false;
			}

			_lastTime = RealTime.time;
		}
	}
	
	private float _lastScrollDelta = 0.0f;
	
	private float _timeDelta = -1.0f;

	private float _lastDragTime = 0.0f;

	public void onDragStage(GameObject go, Vector2 delta)
//	void onDragStage(Vector2 delta)
	{
		if(TutorialManager.instance.isTutorialMode) return;
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;


		_isScrollEffect = false;

		_canScrollBg = true;
		
		_v = camWorldCamera.transform.localPosition;
		
		_v.x = _v.x - delta.x;//Mathf.Lerp(_v.x, _v.x - delta.x, 0.8f);
		
		if(_v.x < MAP_CAMERA_START_X)
		{
			_canScrollBg = false;
			_v.x = MAP_CAMERA_START_X;
		}
		else if(_v.x > MAP_CAMERA_END_X)
		{
			_canScrollBg = false;
			_v.x = MAP_CAMERA_END_X;
		}
		
		_lastScrollDelta = delta.x;
		camWorldCamera.transform.localPosition = _v;

		_lastDragTime = RealTime.time;
	}
	
	
	
	int act = 1;
	int stage = 1;


	public Transform tfDragTransform;

	private Vector2 _lastMousePosition = new Vector2();
	private bool _isMouseDown = false;
	private RaycastHit _uiCheckHitInfo;

	void LateUpdate()
	{

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(btnBack.isEnabled == false) return;
			if(GameManager.me.uiManager.uiMenu.rayCast(GameManager.me.uiManager.uiMenuCamera.camera, btnBack.gameObject) == false) return;

			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;
			onClickBackToMainMenu(null);
			return;
		}



		#if UNITY_EDITOR
		
		if(Input.GetMouseButtonUp(1))
		{
			StartCoroutine( startWalk(act, stage) );

			GameDataManager.instance.maxAct = act;
			GameDataManager.instance.maxStage = stage;

			++stage;
			if(stage > 5)
			{
				stage = 1;
				++act;
			}
		}
		else if(Input.GetMouseButtonUp(2))
		{
			hideHeroMonster();
		}

		#endif
		if(TutorialManager.instance.isTutorialMode) return;



		if(nowPlayingWalkAnimation)
		{
			if(Input.GetMouseButtonUp(0))
			{
				if(Time.timeScale > 1.0f)
				{
					GameManager.setTimeScale = 1.0f;
				}
			}
			else if(Input.GetMouseButton(0))
			{
				if(Time.timeScale < 4.0f)
				{
					GameManager.setTimeScale = 4.0f;
				}
			}
		}



		if(_isScrollEffect)
		{
			_v = camWorldCamera.transform.localPosition;
			
			_lastScrollDelta = Mathf.Lerp(_lastScrollDelta, _lastScrollDelta * 0.85f, 0.8f);
			
			if(Mathf.Abs(_lastScrollDelta) < 2.0f) _isScrollEffect = false;
			
			_v.x = Mathf.Lerp(_v.x, _v.x - _lastScrollDelta, 0.8f);
			
			if(_v.x < MAP_CAMERA_START_X)
			{
				_canScrollBg = false;
				_v.x = MAP_CAMERA_START_X;
				_isScrollEffect = false;
			}
			else if(_v.x > MAP_CAMERA_END_X)
			{
				_canScrollBg = false;
				_v.x = MAP_CAMERA_END_X;
				_isScrollEffect = false;
			}
			
			camWorldCamera.transform.localPosition = _v;
		}



		if(mapPlayer != null && nowPlayingWalkAnimation == false &&  mapPlayer.ani.IsPlaying("idle"))
		{
			if(_playerRandomAniDelay > 0)
			{
				_playerRandomAniDelay -= Time.smoothDeltaTime;
			}
			else
			{
				_playerRandomAniDelay = UnityEngine.Random.Range(5.0f, 10.0f);
				
				string aniName = "memo0"+UnityEngine.Random.Range(1,3);
				mapPlayer.ani.GetClip(aniName).wrapMode = WrapMode.Once;
				mapPlayer.ani.CrossFade("memo0"+UnityEngine.Random.Range(1,3));
				
				mapPlayer.ani.CrossFadeQueued("idle");
			}
		}


		if(mapHeroMonster != null && nowPlayingWalkAnimation == false &&  mapHeroMonster.ani.isPlaying == false)
		{
			if(mapHeroMonster.ani.GetClip("memo") != null) mapHeroMonster.ani.GetClip("memo").wrapMode = WrapMode.Loop;
			mapHeroMonster.ani.CrossFade("memo", 0.2f);
		}
	}
	

	float _playerRandomAniDelay = 5.0f;
	float _mapHeroRandomAniDelay = 5.0f;

}