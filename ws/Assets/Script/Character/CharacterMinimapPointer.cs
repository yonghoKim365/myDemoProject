using UnityEngine;
using System.Collections;

public class CharacterMinimapPointer : MonoBehaviour
{
	public UISprite pointer;

	public Transform cha;

	public Transform cachedTransform;
	
	public static float startX = -100188f;
	public static float endX = -99771f;
	public static float width = 416.7f;
	public static float yPos = 265.0f;

	public static float fingerStartX = -100188f;
	public static float fingerEndX = -99771f;

	
	void Awake()
	{
		cachedTransform = transform;
	}


	public const string DISTANCE = "icn_minimap_simbol_distance"; //16
	public const string CHASER = "icn_minimap_simbol_chasemonster"; //15

	public const string DESTROY = "icn_minimap_simbol_destroy"; //14
	 
	public const string OBJECT = "icn_minimap_simbol_object"; //13
	public const string PROTECT = "icn_minimap_simbol_protectnpc"; //12

	public const string BOSS = "icn_minimap_simbol_enemy_boss"; //11

	public const string KILLCOUNT = "icn_minimap_simbol_enemy_animal_killcount"; //10
	public const string ITEM = "icn_minimap_simbol_item"; //9

	public const string PLAYER = "icn_minimap_simbol_user_hero"; //8
	public const string E_HERO = "icn_minimap_simbol_enemy_hero"; //7

	public const string P_UNIT = "icn_minimap_simbol_user_animal"; // 6
	public const string E_UNIT = "icn_minimap_simbol_enemy_animal"; // 5

	private bool _positionType = false;

	public string type;

	// UI가 초기화될때 기본적으로 모든 미니맵 심볼은 활성화가 된다. 하지만 태그 캐릭터의 경우 이때 활성화가 되면 안된다. 
	public bool canInitVisibleAtStartTime = true;

	public void init(string minimapType, Transform owner, int depth, bool positionType = false)
	{
		cha = owner;
		type = minimapType;
		_positionType = positionType;
		visible = true;
		pointer.spriteName = type;
		pointer.depth = depth;
		canInitVisibleAtStartTime = true;
	}

	private float _setXpos = 0.0f;
	public void setPosition(float xPos)
	{
		_setXpos = xPos;
		if(xPos < StageManager.mapStartPosX) xPos = StageManager.mapStartPosX;

		_v = cachedTransform.localPosition;
		_v.x = startX + width * ((xPos - StageManager.mapStartPosX)/(StageManager.mapEndPosX - StageManager.mapStartPosX));
		_v.y = yPos;
		_v.z = 0.0f;
		cachedTransform.localPosition = _v;
		visible = true;
	}

	public void changeSide(bool isPlayer)
	{
		if(isPlayer)
		{
			pointer.spriteName = P_UNIT;
			pointer.depth = 6;
		}
		else
		{
			pointer.spriteName = E_UNIT;
			pointer.depth = 5;
		}
	}

	
	protected bool _visible = true;
	
	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			_visible = value;

			if(value == true) refreshPosition();

			gameObject.SetActive(value);
		}
	}
	
	protected Vector2 _v2 = Vector2.one;
	
	
	protected bool _isEnabled = false;
	
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;	
			gameObject.SetActive(value);
			if(value == false)
			{
				cha = null;
				_positionType = false;
			}

			_visible = value;
		}
		get
		{
			return _isEnabled;	
		}
	}
	
	protected Vector3 _v;
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(GameManager.me.currentScene != Scene.STATE.PLAY_BATTLE || _visible == false) return;

		if(_positionType)
		{
			_v = cachedTransform.localPosition;
			_v.x = -189 + width * ((_setXpos - StageManager.mapStartPosX)/(StageManager.mapEndPosX - StageManager.mapStartPosX));
			_v.y = yPos;
			cachedTransform.localPosition = _v;
		}
		else
		{
			if(cha == null) return;
			
			_v = cha.localPosition;
			if(_v.x < StageManager.mapStartPosX) _v.x = StageManager.mapStartPosX;

			_v.x = -189 + width * ((_v.x - StageManager.mapStartPosX)/(StageManager.mapEndPosX - StageManager.mapStartPosX));
			_v.y = yPos;
			cachedTransform.localPosition = _v;
		}
	}


	public void refreshPosition()
	{
		if(_positionType)
		{
			_v = cachedTransform.localPosition;
			_v.x = -189 + width * ((_setXpos - StageManager.mapStartPosX)/(StageManager.mapEndPosX - StageManager.mapStartPosX));
			_v.y = yPos;
			cachedTransform.localPosition = _v;
		}
		else
		{
			if(cha == null) return;
			
			_v = cha.localPosition;
			if(_v.x < StageManager.mapStartPosX) _v.x = StageManager.mapStartPosX;

			_v.x = -189 + width * ((_v.x - StageManager.mapStartPosX)/(StageManager.mapEndPosX - StageManager.mapStartPosX));
			_v.y = yPos;
			cachedTransform.localPosition = _v;
		}

	}

}
