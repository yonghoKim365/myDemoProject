using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIVisitingLobby : MonoBehaviour {

	public UIButton btnChangeToMain, btnChangeToSub;

	public UIButton[] btnEquips;
	public UIButton[] btnSkills;

	public UIButton btnBack;

	public float nowZoomSize = 0.0f;

	public float lobbyCharacterZoomMin = 0.5f;
	public float lobbyCharacterZoomDefault = 0.7f;
	public float lobbyCharacterZoomMax = 1.2f;
	
	public float characterStageCenterPosX = 0;

	public UILabel lbName;

	public Transform heroParent;
	public Transform[] unitParent;
	
	public Player hero;
	public Monster[] units = new Monster[5];

	public PhotoDownLoader playerPhoto;

	public UISprite spEmptyFace;

	public LobbyCharacterPosition lobbyCharacterPositionSetter= new LobbyCharacterPosition();

	public Camera uiCamera;
	public Camera chracterCamera;
	
	private RaycastHit uiCheckHitInfo;

	const string HERO = "HERO";
	const string PET = "PET";
	const string UNIT0 = "UNIT0";
	const string UNIT1 = "UNIT1";
	const string UNIT2 = "UNIT2";
	const string UNIT3 = "UNIT3";
	const string UNIT4 = "UNIT4";

	private GameObject[] _lobbyBackground = new GameObject[UILobby.LOBBY_MAP_NAMES.Length];


	public Queue<GameObject> activePanelAfterClose = new Queue<GameObject>();


	public UIHeroInventoryTab[] equips = new UIHeroInventoryTab[4];
	public UISkillInvenSlot[] skills = new UISkillInvenSlot[3];




	bool _fromShop = false;

	void Awake()
	{
		UIEventListener.Get(btnBack.gameObject).onClick = onClickBack;

		characterStageCenterPosX = characterCameraTransform.transform.localPosition.x;
		
		lobbyTreeContainer.transform.localScale = new Vector3(lobbyCharacterZoomDefault,lobbyCharacterZoomDefault,lobbyCharacterZoomDefault);
		nowZoomSize = lobbyCharacterZoomDefault;
		UIEventListener.Get(btnChangeToMain.gameObject).onClick = onClickChangeToMain;
		UIEventListener.Get(btnChangeToSub.gameObject).onClick = onClickChangeToSub;
	}


	bool _isEnabled = false;

	void onClickBack(GameObject go)
	{
		UINetworkLock.instance.show();

		StartCoroutine(backToMyWorld());
	}

	IEnumerator backToMyWorld()
	{
		_isEnabled = false;

		cleanLobbyCharacter();
		cleanMapAsset();

		_playerMainData = null;
		_playerSubData = null;

		_selectMainUnitRunes = null;
		_selectMainSkillRunes = null;
		_selectSubUnitRunes = null;
		_selectSubSkillRunes = null;

		while(activePanelAfterClose.Count > 0)
		{
			activePanelAfterClose.Dequeue().SetActive(true);
		}

		if(_fromShop)
		{
			GameManager.me.uiManager.uiMenuCamera.camera.enabled = false;
			GameManager.me.uiManager.uiMenuCamera.enabled = false;
		}

		yield return new WaitForSeconds(0.3f);

		GameManager.me.characterManager.clearUnusedResource(true);
		GameManager.me.clearMemory();

		yield return Util.ws05;

		UINetworkLock.instance.hide();

		gameObject.SetActive(false);

	}


	void cleanMapAsset()
	{
		int checkActIndex = GameDataManager.instance.maxActWithCheckingMaxAct;

		for(int i = 0; i < _lobbyBackground.Length; ++i)
		{
			// 현재 내가 실제 써야할 맵이 아니면 메모리에서도 지운다.
			if( (i+1) != checkActIndex)
			{
				if(_lobbyBackground[i] != null)
				{
					GameObject.Destroy(_lobbyBackground[i].gameObject);
				}

				if(GameDataManager.instance.lobbyMapResource.ContainsKey(UILobby.LOBBY_MAP_NAMES[i]))
				{
					if(GameDataManager.instance.lobbyMapResource[UILobby.LOBBY_MAP_NAMES[i]] != null)
					{
						GameObject.Destroy(GameDataManager.instance.lobbyMapResource[UILobby.LOBBY_MAP_NAMES[i]].gameObject);
					}
					
					GameDataManager.instance.lobbyMapResource[name] = null;
					GameManager.me.clearMemory();
				}
			}
			else
			{
				if(_lobbyBackground[i] != null) _lobbyBackground[i].gameObject.SetActive(false);
			}

			_lobbyBackground[i] = null;
		}
	}




//	public Vector3 defaultCameraTransformLocalPosition = new Vector3(37.5f, 662.0f, 1020.0f);
//	public Vector3 defaultCameraTransformLocalRotation = new Vector3(29.5f, 181.0f, 0.0f);

	public Vector3 defaultCameraTransformLocalPosition = new Vector3(0, 662.0f, 1020.0f);
	public Vector3 defaultCameraTransformLocalRotation = new Vector3(29.5f, 179.0f, 0.0f);//-6.7f);

	Quaternion _q;



	private GamePlayerData _playerData
	{
		get
		{
			if(_isMain) return _playerMainData;
			else return _playerSubData;
		}
	}

	private GameIDData[] _selectUnitRunes
	{
		get
		{
			if(_isMain) return _selectMainUnitRunes;
			else return _selectSubUnitRunes;
		}
	}

	private GameIDData[] _selectSkillRunes
	{
		get
		{
			if(_isMain) return _selectMainSkillRunes;
			else return _selectSubSkillRunes;
		}
	}

	private GamePlayerData _playerMainData = null;
	private GamePlayerData _playerSubData = null;

	private GameIDData[] _selectMainUnitRunes = null;
	private GameIDData[] _selectMainSkillRunes = null;

	private GameIDData[] _selectSubUnitRunes = null;
	private GameIDData[] _selectSubSkillRunes = null;


	private int _currentAct;
	private bool _isMain = false;

	public void show (string name, int act, GamePlayerData playerData, GamePlayerData subData, GameIDData[] selectUnitRunes, GameIDData[] selectSkillRunes, GameIDData[] selectSubUnitRunes, GameIDData[] selectSubSkillRunes, int showPhoto, string photoUrl)
	{
		_isMain = true;

		if(act >  GameManager.MAX_ACT)
		{
			act =  GameManager.MAX_ACT;
		}

		UINetworkLock.instance.show();

		_fromShop = false;

		if(GameManager.me.uiManager.uiMenu.uiWorldMap.gameObject.activeSelf)
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.uiMenu.uiWorldMap.gameObject);
		}

		if(GameManager.me.uiManager.popupChampionship.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupChampionship.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.popupChampionship.gameObject);
		}

		if(GameManager.me.uiManager.popupHell.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupHell.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.popupHell.gameObject);
		}

		if(GameManager.me.uiManager.uiMenu.uiFriend.gameObject.activeSelf)
		{
			GameManager.me.uiManager.uiMenu.uiFriend.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.uiMenu.uiFriend.gameObject);
		}

		if(GameManager.me.uiManager.popupFriendDetail.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupFriendDetail.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.popupFriendDetail.gameObject);
		}

		if(GameManager.me.uiManager.popupShop.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupShop.gameObject.SetActive(false);
			activePanelAfterClose.Enqueue(GameManager.me.uiManager.popupShop.gameObject);
			_fromShop = true;
			GameManager.me.uiManager.uiMenuCamera.camera.enabled = true;
			GameManager.me.uiManager.uiMenuCamera.enabled = true;
		}

		lbName.text = name;

		gameObject.SetActive(true);

		_currentAct = act;

		_playerMainData = playerData;
		_playerSubData = subData;

		_selectMainUnitRunes = selectUnitRunes;
		_selectMainSkillRunes = selectSkillRunes;
		_selectSubUnitRunes = selectSubUnitRunes;
		_selectSubSkillRunes = selectSubSkillRunes;

		if(subData == null)
		{
			btnChangeToMain.gameObject.SetActive(false);
			btnChangeToSub.gameObject.SetActive(false);
		}
		else
		{
			btnChangeToSub.gameObject.SetActive(true);
			btnChangeToMain.gameObject.SetActive(false);
		}

		draw();

		_q.eulerAngles = defaultCameraTransformLocalRotation;
		chracterCamera.transform.localRotation = _q;

		// 방문 로비는 무조건 최대치 확대.
		nowZoomSize = 1.2f;

		chracterCamera.fieldOfView = 15.0f * nowZoomSize;

		UIManager.setPlayerPhoto(showPhoto, photoUrl, spEmptyFace, playerPhoto, true, 70);

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) 
		{
			//GameDataManager.instance.maxAct = 3;
			GameDataManager.instance.maxStage = 1;
			GameDataManager.instance.maxRound = 1;

			++GameDataManager.instance.maxAct;

			if(GameDataManager.instance.maxAct > 5) GameDataManager.instance.maxAct = 1;
		}
#endif

		StartCoroutine(updateLobbyMap());

		_isEnabled = true;
	}



	void draw()
	{
		equips[0].setData(_playerData.partsHead.itemInfo);
		equips[1].setData(_playerData.partsBody.itemInfo);
		equips[2].setData(_playerData.partsWeapon.itemInfo);
		equips[3].setData(_playerData.partsVehicle.itemInfo);
		
		for(int i = 0; i < 4; ++i)
		{
			equips[i].isMyInven = false;
		}
		
		for(int i = 0; i < 3; ++i)
		{
			skills[i].setData(_selectSkillRunes[i], i);
			skills[i].isMyInven = false;
		}
		
		StartCoroutine( updateCharacter() );

	}




	IEnumerator updateLobbyMap()
	{
		int checkActIndex = _currentAct;
		
		for(int i = 0; i < _lobbyBackground.Length; ++i)
		{
			if(i == checkActIndex-1)
			{
				if(i < GameManager.MAX_ACT)
				{
					if(_lobbyBackground[i] != null)
					{
						_lobbyBackground[i].gameObject.SetActive(true);
						continue;
					}
					
					GameDataManager.instance.loadLobbyMap(UILobby.LOBBY_MAP_NAMES[i]);
					while(GameDataManager.instance.isCompleteLoadLobbyMap == false) { yield return null; }

					GameDataManager.instance.lobbyMapResource.TryGetValue( UILobby.LOBBY_MAP_NAMES[i] , out _lobbyBackground[i] );
					
					if(_lobbyBackground[i] != null)
					{
						_lobbyBackground[i].transform.parent = lobbyTreeContainer.transform;
						_v.x = 0; _v.y = 0; _v.z = 0; _lobbyBackground[i].transform.localPosition = _v; 
						
						switch(i)
						{
						case 1: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v;  break;
						case 2: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v; break;
						default: _v.x = 1; _v.y = 1; _v.z = 1; _lobbyBackground[i].transform.localScale = _v; break;
						}

						_lobbyBackground[i].gameObject.SetActive(true);
					}
				}
			}
		}
	}










	public void onClickEquip(GameIDData data)
	{
		GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show(data, RuneInfoPopup.Type.PreviewOnly, false, true, _playerData);
	}


	public void onClickSkill(GameIDData data)
	{
		GameManager.me.uiManager.popupSkillPreview.show(data, RuneInfoPopup.Type.PreviewOnly, false, true, _playerData);
		GameManager.me.uiManager.popupSkillPreview.setOtherUserDescription(hero);
	}


	void cleanLobbyCharacter()
	{
		if(hero != null) GameManager.me.characterManager.cleanMonster(hero);
		hero = null;

		for(int i = 0; i < 5; ++i)
		{
			if(units[i] != null)
			{
				GameManager.me.effectManager.removeBodyEffect(units[i]);
				GameManager.me.characterManager.cleanMonster(units[i]);
			}
			units[i] = null;
		}
	}

	IEnumerator updateCharacter()
	{
		if(hero != null) GameManager.me.characterManager.cleanMonster(hero);
		for(int i = 0; i < 5; ++i)
		{
			if(units[i] != null) GameManager.me.characterManager.cleanMonster(units[i]);
			units[i] = null;
		}

		for(int i = 0; i < 5; ++i)
		{
			if(_selectUnitRunes[i] != null)
			{
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[_selectUnitRunes[i].unitData.resource]);
			}
		}

		GameDataManager.instance.loadDefaultModelData();

		while(GameDataManager.instance.isCompleteLoadModel == false)
		{ 
			yield return null;
		}


		while(GameDataManager.instance.isCompleteLoadLobbyMap == false)
		{ 
			yield return null;
		}


		hero = (Player)GameManager.me.characterManager.getMonster(true,true,_playerData.id,false);//  (Player)Instantiate(GameManager.me.player);
		hero.init(_playerData,true,false);

		hero.pet = (Pet)GameManager.me.characterManager.getMonster(false,true,_playerData.partsVehicle.parts.resource.ToUpper(),false);
		hero.pet.init(hero);

		hero.setParent(heroParent);
		hero.cTransform.localScale = new Vector3(1,1,1);		
		hero.cTransform.localPosition = new Vector3(0,0,0);	

		hero.setPositionCtransform(heroParent.position);

		Quaternion q = hero.tf.localRotation;
		Vector3 v = q.eulerAngles;
		v.x = 0; v.y = 0; v.z = 0;
		q.eulerAngles = v;
		hero.state = Monster.NORMAL;

		hero.pet.collider.enabled = true;
		hero.pet.collider.name = "PET";

		hero.pet.tf.localRotation = q;	
		hero.pet.cTransform.localRotation = q;



		for(int i = 0; i < 5; ++i)
		{
			if(_selectUnitRunes[i] == null)
			{
				continue;
			}
			
			units[i] = GameManager.me.characterManager.getMonster(false, true,_selectUnitRunes[i].unitData.resource, false);
			units[i].isEnabled = true;
			CharacterUtil.setRare(_selectUnitRunes[i].unitData.rare, units[i]);
			units[i].setParent( unitParent[i] );

			units[i].cTransform.localPosition = new Vector3(0,0,0);	

			units[i].init(_selectUnitRunes[i].transcendData, _selectUnitRunes[i].transcendLevel, _selectUnitRunes[i].unitData.id,true,Monster.TYPE.UNIT);



			units[i].normalCollider.enabled = true;
			units[i].name = "UNIT"+i;
			units[i].shadow.gameObject.SetActive( true );
			units[i].initShadowAndEffectSize();
			units[i].startAction();
			units[i].setIdleAndFreeze(true);
			
			if(units[i].ani.GetClip(Monster.NORMAL_LOBBY) != null)
			{
				units[i].state = Monster.NORMAL_LOBBY;
			}
			else
			{
				units[i].state = Monster.NORMAL;
			}
			
			units[i].renderAniRightNow();

			 q = units[i].transform.localRotation;
			 v = q.eulerAngles;
			v.x = 0; v.y = 0; v.z = 0;
			q.eulerAngles = v;
			units[i].transform.localRotation = q;		
			units[i].cTransform.localRotation = q;

			GameManager.me.effectManager.checkUnitBodyEffect(units[i], true);
		}

		lobbyCharacterPositionSetter.changePosition(units, false);

//		Debug.Log("updateCharacter complete ");

		UINetworkLock.instance.hide();
	}
	
	




	
	
	public Transform characterCameraTransform;
	
	
	// --------------------------- 캐릭터 줌 관련 --------------------------- //
	
	float zoomSize = 0.0f;
	void LateUpdate()
	{
		if(GameManager.me.uiManager.currentUI != UIManager.Status.UI_MENU || 
		   _isEnabled == false ||
		   GameManager.me.uiManager.uiVisitingLobby.gameObject.activeSelf == false) return;


		updateCharacterLoop();

		_v = characterCameraTransform.transform.localPosition;
		_v.x = characterStageCenterPosX - transform.position.x;

		//zoomSize = lobbyTreeContainer.localScale.x;
		zoomSize = nowZoomSize;

		if(zoomSize <= 0.0f) zoomSize = 0.01f;

		float ratio = (zoomSize - lobbyCharacterZoomMin) / (lobbyCharacterZoomMax - lobbyCharacterZoomMin);

		ratio = 1 - ratio;

		// ratio가 1일때 y는 419. rx는 18.
		// ratio가 0일때 y는 662, rx는 29.5

		float targetY = defaultCameraTransformLocalPosition.y - (ratio)*(641.0f-340.0f);//(662.0f-419.0f);

		//_v.y = targetY;
		_v.y = Mathf.Lerp(_v.y, targetY, 0.9f + ratio);

		characterCameraTransform.transform.localPosition = _v; // 화면 고정을 위해 주석처리.

		_v = defaultCameraTransformLocalRotation;
		//_v.x -= (ratio)*(29.5f-18.0f);
		_v.x -= (ratio)*(29.5f-17.0f);

		_v.y = 179.0f; _v.z = 0.0f;

		_q.eulerAngles = _v;
		chracterCamera.transform.localRotation = _q; // 화면 고정을 위해 주석처리.

		// 오브젝트를 클릭하지 않았으면.
		if(finchAndSelectObject() == false && currentSelectObject == null)
		{
		}		
	}



	float currentTime = 0.0f;
	float _updateLoopLeftTime = 0.0f; 
	void updateCharacterLoop()
	{
		float newTime = currentTime + Time.smoothDeltaTime;
		float frameTime = newTime - currentTime; 
		currentTime = newTime;
		_updateLoopLeftTime += frameTime; 

		while ( _updateLoopLeftTime >= GameManager.LOOP_INTERVAL)      
		{           
			if(hero != null) hero.updateAnimationMethod();
			for(int i = 0; i < 5; ++i)
			{
				if(units[i] != null)
				{
					units[i].updateAnimationMethod();
				}
			}
			_updateLoopLeftTime -= GameManager.globalDeltaTime;
		} 

		if(hero != null) hero.renderAni();
		for(int i = 0; i < 5; ++i)
		{
			if(units[i] != null)
			{
				units[i].renderAni();
			}
		}

	}



	
	// 화면 특정 좌표를 눌렀을때 반응 하는 것.	
	// ngui ui를 눌렀다면 반응을 안하고 아니라면 캐릭터는 그 위치로 이동한다.
	private float _draggingDistance = 0.0f;
	private bool _isTouchDragging = false;

	
	private float _dragginDistance2 = 0.0f;
	private bool _isTouchDragging2 = false;
	private bool _checkTouchStart = false;	
	
	private float touchDistance;	
	private Vector2 prevMousePosition = Vector2.zero;	
	
	public Transform characterStage;
	public Transform lobbyTreeContainer;
	
	private Vector3 _v;
	
	private GameObject currentSelectObject;
	
	private bool _isMouseDown = false;
	private bool _isMouseDownStart = false;


	private void zoomStage()
	{
		if(UICamera.hoveredObject != UICamera.fallThrough) return;
		//lobbyTreeContainer.localScale = _v;
		nowZoomSize = _v.x;
		chracterCamera.fieldOfView = 15.0f * nowZoomSize;
	}

	// Update is called once per frame
	private bool finchAndSelectObject () {
		
		Vector2 mousePos = Vector2.zero;
		
		bool isMouseUp = false;
		bool isDrag = false;
		
#if UNITY_EDITOR
		
			if(_isMouseDown == false) _isMouseDown = Input.GetMouseButtonDown(0);
			isMouseUp = Input.GetMouseButtonUp(0);
			if(isMouseUp) _isMouseDown = false;
		
			mousePos = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
			if(Input.touchCount >= 1)
			{
				Touch t = Input.GetTouch(0);
				_isMouseDown = (t.phase == TouchPhase.Began);
				isMouseUp = (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled);
				mousePos = t.position;
				isDrag = (t.phase == TouchPhase.Moved);
			}
#endif		
		
#if UNITY_EDITOR



		// 확대 축소. 핀치 줌.
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//_v = lobbyTreeContainer.localScale;
			_v.x = nowZoomSize;
			_v.x = Mathf.Min(_v.x*1.2f, lobbyCharacterZoomMax);
			_v.y = _v.z = _v.x;
			zoomStage();

			//lobbyTreeContainer.localScale = _v;
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//_v = lobbyTreeContainer.localScale;
			_v.x = nowZoomSize;
			_v.x = Mathf.Max(_v.x/1.2f, lobbyCharacterZoomMin);			
			_v.y = _v.z =_v.x;			
			zoomStage();
			//lobbyTreeContainer.localScale = _v;
		}

		/*

		// 터치가 시작한 순간.
		if(Input.GetMouseButtonDown(0))
		{
//			Debug.Log("터치가 시작.");
			
			prevMousePosition = mousePos;
			_draggingDistance = 0.0f;
			_isTouchDragging = false;
			_isMouseDown = true;
			_isMouseDownStart = true;
		}
		
		// 화면 이동. 마우스가 눌린 상태.
		if(Input.GetMouseButton(0) && _isMouseDownStart)
		{

			characterStage.Rotate(0,((prevMousePosition.x)-Input.mousePosition.x)*10.0f*Time.smoothDeltaTime, 0);
			
			float deltaX = prevMousePosition.x - mousePos.x;
			float deltaY = prevMousePosition.y - mousePos.y;
			_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
			if(_draggingDistance > 20.0f) _isTouchDragging = true;
			
			prevMousePosition = mousePos;
		}
*/

		
		if(Input.GetMouseButtonDown(0))
		{
			// 캐릭터 개체 하나씩 회전시키기위한 값들...
			GameObject sgo = getCharacterByScreenPosition(mousePos);

			prevMousePosition = mousePos;
			_draggingDistance = 0.0f;
			_isTouchDragging = false;
			_isMouseDown = true;
			_isMouseDownStart = true;

			if(sgo!=null)
			{
				currentSelectObject = sgo;			
			}
			//==================================//
		}
		else if(Input.GetMouseButtonUp(0))
		{
			_isMouseDownStart = false;

			getCharacterByScreenPosition(mousePos);

			if(currentSelectObject != null)
			{
				// 화면 이동이 이루어진 상태가 아니라면....
				// 해당 타일에 맞는 작업을 시작한다.
				if(_isTouchDragging == false)
				{
					playDefaultAnimation();
				}
				
				currentSelectObject = null;
		
				_draggingDistance = 0.0f;
				_isTouchDragging = false;			
			}
		}


		// 화면 이동. 마우스가 눌린 상태.
		if(Input.GetMouseButton(0) && _isMouseDownStart && currentSelectObject != null)
		{
			currentSelectObject.transform.Rotate(0,((prevMousePosition.x)-Input.mousePosition.x)*30.0f*Time.smoothDeltaTime, 0);
			
			float deltaX = prevMousePosition.x - mousePos.x;
			float deltaY = prevMousePosition.y - mousePos.y;
			_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
			if(_draggingDistance > 20.0f) _isTouchDragging = true;
			
			prevMousePosition = mousePos;
		}



#elif UNITY_IPHONE || UNITY_ANDROID

		if(Input.touchCount > 0)mousePos = Input.GetTouch(0).position;
		
		if(Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			
			if(touch.phase == TouchPhase.Began)
			{
				prevMousePosition = (Vector3)touch.position;
				
				GameObject sgo = getCharacterByScreenPosition(mousePos);
				
				if(sgo!=null)
				{
				   _draggingDistance = 0.0f;
				  _isTouchDragging = false;	
					
					currentSelectObject = sgo;
					return true;
				}
			}
			else if(touch.phase == TouchPhase.Moved)
			{
				float deltaX = prevMousePosition.x - mousePos.x;
				float deltaY = prevMousePosition.y - mousePos.y;
				_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
				if(_draggingDistance > 20.0f)
				{
					_isTouchDragging = true;
				}					
				
				//characterStage.Rotate(0,((prevMousePosition.x)-touch.position.x)*10.0f*Time.smoothDeltaTime,0);

				if(currentSelectObject != null) currentSelectObject.transform.Rotate(0,((prevMousePosition.x)-touch.position.x)*30.0f*Time.smoothDeltaTime,0);

				prevMousePosition = (Vector3)touch.position;
			}
			else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				if(currentSelectObject != null)
				{
					// 화면 이동이 이루어진 상태가 아니라면....
					// 해당 타일에 맞는 작업을 시작한다.
					if(_isTouchDragging == false)
					{
//						Debug.LogError("클릭 오브젝트 : " + currentSelectObject.name);
						playDefaultAnimation();
						//currentSelectObject.animation.Play("attack");	
					}
					
					currentSelectObject = null;
					_draggingDistance = 0.0f;
					_isTouchDragging = false;	
					
					return true;
				}
				
				_draggingDistance = 0.0f;
				_isTouchDragging = false;				
			}
		}


		else if(Input.touchCount > 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
			{
				touchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			}
			else if((Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) //!uiManager.isUIOver 
			{
				float dis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			
				if(touchDistance>dis)
				{
					//_v = lobbyTreeContainer.localScale;
					_v.x = nowZoomSize;
					_v.x = Mathf.Min(_v.x*1.02f, lobbyCharacterZoomMax);
					_v.y = _v.z = _v.x;
					zoomStage();
					//lobbyTreeContainer.localScale = _v;
				}
				else if(touchDistance<dis)
				{
					//_v = lobbyTreeContainer.localScale;
					_v.x = nowZoomSize;
					_v.x = Mathf.Max(_v.x/1.02f, lobbyCharacterZoomMin);
					_v.y = _v.z =_v.x;			
					zoomStage();
					//lobbyTreeContainer.localScale = _v;
				}
				touchDistance = dis;
			}
			
			if(Input.GetTouch(1).phase == TouchPhase.Ended)
			{
				prevMousePosition = (Vector3)Input.GetTouch(0).position;
			}else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				prevMousePosition = (Vector3)Input.GetTouch(1).position;
			}
			
			currentSelectObject = null;
		}


#endif

		return false;
	}	

	public GameObject getCharacterByScreenPosition(Vector3 pos)
	{
		if(UICamera.hoveredObject != UICamera.fallThrough) return null;

		if(Physics.Raycast(chracterCamera.ScreenPointToRay(Input.mousePosition), out uiCheckHitInfo))
		{
//			Debug.Log("getCharacterByScreenPosition : " + uiCheckHitInfo.collider.gameObject.name);
			switch( uiCheckHitInfo.collider.gameObject.name )
			{
			case HERO:
			case PET:
				return hero.cTransform.gameObject;
				break;
			case UNIT0:
			case UNIT1:
			case UNIT2:
			case UNIT3:
			case UNIT4:
				return uiCheckHitInfo.collider.gameObject;
				break;
			}
		}
		return null;
	}





	void playDefaultAnimation()
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		if(UICamera.hoveredObject != UICamera.fallThrough || TutorialManager.instance.isTutorialMode ) return;

		if(uiCheckHitInfo.collider.gameObject == null) return;

		switch( uiCheckHitInfo.collider.gameObject.name )
		{
		case HERO:
		case PET:

			hero.state = Monster.SHOOT;
			hero.render();

			break;
		case UNIT0:
			GameManager.me.uiManager.popupSummonDetail.show(_selectUnitRunes[0], RuneInfoPopup.Type.PreviewOnly, false);
			GameManager.me.uiManager.popupSummonDetail.setOtherUserDescription(hero);
			break;
		case UNIT1:
			GameManager.me.uiManager.popupSummonDetail.show(_selectUnitRunes[1], RuneInfoPopup.Type.PreviewOnly, false);
			GameManager.me.uiManager.popupSummonDetail.setOtherUserDescription(hero);
			break;
		case UNIT2:
			GameManager.me.uiManager.popupSummonDetail.show(_selectUnitRunes[2], RuneInfoPopup.Type.PreviewOnly, false);
			GameManager.me.uiManager.popupSummonDetail.setOtherUserDescription(hero);
			break;
		case UNIT3:
			GameManager.me.uiManager.popupSummonDetail.show(_selectUnitRunes[3], RuneInfoPopup.Type.PreviewOnly, false);
			GameManager.me.uiManager.popupSummonDetail.setOtherUserDescription(hero);
			break;
		case UNIT4:
			GameManager.me.uiManager.popupSummonDetail.show(_selectUnitRunes[4], RuneInfoPopup.Type.PreviewOnly, false);
			GameManager.me.uiManager.popupSummonDetail.setOtherUserDescription(hero);
			break;
		}

	}
	


	void onClickChangeToMain(GameObject go)
	{
		_isMain = true;

		cleanLobbyCharacter();

		btnChangeToMain.gameObject.SetActive(false);
		btnChangeToSub.gameObject.SetActive(true);

		draw();
	}
	
	
	void onClickChangeToSub(GameObject go)
	{
		_isMain = false;

		cleanLobbyCharacter();
		btnChangeToMain.gameObject.SetActive(true);
		btnChangeToSub.gameObject.SetActive(false);

		draw();

	}

}
