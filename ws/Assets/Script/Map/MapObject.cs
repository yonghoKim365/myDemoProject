using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapObject
{
	public const string ITEM = "ITEM";
	public const string MAGNET = "MAGNET";

	public DropItemEffect dropItemEffect = null;
	
	
	public GameObject gameObject;
	public Transform transform;
	private Vector3 _v;
	private Quaternion _q;
	private BoxCollider _boxCollider;

	private bool _isEnabled = false;

	private IVector3 _boundExtens;
	private IVector3 _boundCenter;
	
	public Animation damageAnimation;
	
	public ICallbackWorker afterLimitCallback = null;
	
	public bool nowUsing = false;
	
	public MapObject (GameObject gobj)
	{
		gameObject = gobj;
		transform = gobj.transform;
		transform.parent = GameManager.me.mapManager.mapStage;	
		dropItemEffect = new DropItemEffect(this);
	}
	
	
	public MapObjectData mapObjectData;
	public int hp = 0;
	
	private float _timeLimit = 0.0f;
	private bool _timeLimitObject = false;
	
	private bool _startDeleteMotion = false;
	
	public float startPosition;

	public delegate void deleteMapObject(MapObject mobj);
	
	public deleteMapObject deleteCompleteCallback = null;
	
	
	private List<AttachedEffect> _effects = new List<AttachedEffect>();
	
	private Dictionary<int, float> _hitBulletTime = new Dictionary<int, float>();
	
	private string _resourceId;
	
	private Vector3 _oriLocalPos;
	
	public void init(MapObjectData mobjData, IVector3 position)
	{
		_hitBulletTime.Clear();
		
		isDeleteObject = false;
		_startDeleteMotion = false;
		
		afterLimitCallback = null;
		
		mapObjectData = mobjData;
		this.hp = mobjData.hp;

#if UNITY_EDITOR		
		gameObject.name = mapObjectData.id;
#endif


		_boundCenter.x = 0;_boundCenter.y = 0; _boundCenter.z = 0;
		_boundExtens.x = 160;_boundExtens.y = 160; _boundExtens.z = 160;

		setHitObject();
		
		transform.parent = GameManager.me.mapManager.mapStage;

		_timeLimit = mapObjectData.timeLimit;
		_timeLimitObject = (_timeLimit > 0.0f);
		
		isDeleteObject = false;
		
		_q = transform.rotation;
		_v = _q.eulerAngles;
		_v.x = mapObjectData.rotateX;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		transform.rotation = _q;		
		
		position.z += ((_boundCenter.y + _boundExtens.y) ) * 0.1f;
		
		setPositionAndTransform(position);

		
		if(mapObjectData.effectData != null)
		{
			foreach(AttachedEffectData aed in mapObjectData.effectData)
			{
				AttachedEffect ae = GameManager.me.effectManager.getAttachedEffect();
				_effects.Add(ae);
				
				_v = _v = transform.position;//transformPosition;
				ae.init(aed, transform, _v, 0.0f);
				ae.isEnabled = true;
				
				if(ae.nowCollisionData != null)
				{
					setHitObject(ae.nowCollisionData);
				}
			}
		}

		_oriLocalPos = transform.localPosition;
		
		isEnabled = true;
	}

	
	public void visibleEffect(bool visible)
	{
		for(int i = _effects.Count - 1; i >= 0; --i)
		{
			_effects[i].gameObject.SetActiveRecursively(visible);
		}
	}
	
	
	public void clearEffect()
	{
		for(int i = _effects.Count - 1; i >= 0; --i)
		{
			GameManager.me.effectManager.setAttachedEffect(_effects[i]);
		}
		
		_effects.Clear();
	}
	
	
	
	
	private void setHitObject(CollisionData cd)
	{
		_boundCenter = cd.center;
		_boundExtens = cd.size * 0.5f;
		_boundExtens.z = 400.0f;
		
		// 기본적으로 총알을 위쪽을 향해 보고 있다.
		// 그런데 오른쪽이나 왼쪽으로 눕게되면 가로와 세로 길이가 서로 바뀌게 된다.
		// 그래서 여기에서는 그 경우에 한해 가로/세로 길이를 교체해준다.
		// 또한 왼쪽으로 누웠을 경우에는 rect 계산시 x에서 width만큼 빼주어 위치 조정도 해야한다.
		
		//if(isSettingAngleToHitObject == false) setAngleToHitObject();
		//setAngleToHitObject();
		
		_hitObject.init(_boundCenter, _boundExtens);
	}	


	HitObject _hitObject = new HitObject();

	private void setHitObject(float zSize = 100.0f)
	{
		// 기본적으로 총알을 위쪽을 향해 보고 있다.
		// 그런데 오른쪽이나 왼쪽으로 눕게되면 가로와 세로 길이가 서로 바뀌게 된다.
		// 그래서 여기에서는 그 경우에 한해 가로/세로 길이를 교체해준다.
		// 또한 왼쪽으로 누웠을 경우에는 rect 계산시 x에서 width만큼 빼주어 위치 조정도 해야한다.
		_boundCenter.z *= zSize * 0.005f;
		_boundExtens.z *= zSize * 0.01f;
		_boundExtens.z = 400.0f;
		
		//if(isSettingAngleToHitObject == false) setAngleToHitObject();
		
		_hitObject.init(_boundCenter, _boundExtens);
		
	}

	
	private int tempAngle;
	private float _tempFloat = 1.0f;
	
	private float dx,dy,dz;
	

	

	float dist = 0;
	Vector3 _playerPos;
	public void playMagnet()
	{
		_v = transformPosition;
		_playerPos = GameManager.me.player.cTransformPosition;
		
		dist = VectorUtil.Distance(_v.x, _playerPos.x);
		
		if(dist > GameDataManager.selectedPlayerData.heroData.itemRange) return;
		
		dx = _v.x - _playerPos.x;
		dy = _v.y - (_playerPos.y + 50.0f);
		dz = _v.z - _playerPos.z;
		
		tempAngle = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) - 180;
		
		if(tempAngle < 0) tempAngle += 360;
		else if(tempAngle >= 360) tempAngle %= 360;			
		
		dist = dist * 250.0f/dist * 1.5f + 200.0f;
		
		_v.x += (dist) * GameManager.globalDeltaTime * GameManager.angleTable[tempAngle].x;
		_v.y += (dist) * 2.0f * GameManager.globalDeltaTime * GameManager.angleTable[tempAngle].y;
		

		tempAngle = (int)(Mathf.Atan2(dz, dx) * Mathf.Rad2Deg) - 180;
		
		if(tempAngle < 0) tempAngle += 360;
		else if(tempAngle >= 360) tempAngle %= 360;			
		_v.z += (dist) * GameManager.globalDeltaTime * GameManager.angleTable[tempAngle].y;

		setPosition(_v);
		//transform.position = _v;
	}


	public bool isEnabled
	{
		set
		{
			_isEnabled = value;
			if(value == false)
			{
				dropItemEffect.isEnabled = false;
			}
			gameObject.SetActiveRecursively(value);
		}
		get
		{
			return _isEnabled;	
		}
	}	
	
	private float _top;
	private float _bottom;
	private float _left;
	private float _right;
	
	public HitObject hitObject
	{
		get
		{
			return _hitObject;
		}
	}
	
	
	public bool isDeleteObject = false;
	
	private Color _tempColor;
	
	
	
	public bool checkBulletHitTime(Bullet b)
	{
		if(_hitBulletTime.ContainsKey(b.uniqueNo))
		{
			if(GameManager.me.stageManager.playTime - _hitBulletTime[b.uniqueNo] <= b.hitTimeOffset)
			{
				return false;
			}
			
			_hitBulletTime[b.uniqueNo] = GameManager.me.stageManager.playTime;
		}
		else
		{
			_hitBulletTime.Add(b.uniqueNo, GameManager.me.stageManager.playTime);
		}
		
		return true;
	}

	public void update()
	{
		_v = transformPosition;
		
		hitObject.setPosition(_v);
		
		if(dropItemEffect.isEnabled)
		{
			dropItemEffect.update();
		}
		else playMagnet();

		isDeleteObject = false;
	}	




////================================== 모션 부드럽게 ======================================/////

	public Vector3 transformPosition = Vector3.zero;
	bool _doRenderSkipFrame = false;
	bool _needPositionRender = false;

	public void LateUpdate()
	{
		if(GameManager.renderSkipFrame == false) _doRenderSkipFrame = _needPositionRender;
		
		if(_needPositionRender)
		{
			transform.position = transformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
		}
		else if(GameManager.renderSkipFrame && _doRenderSkipFrame)
		{
			transform.position = transformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
		}
		_needPositionRender = false;
	}
	

	
	Vector3 prevTransformPosition = Vector3.zero;
	
	public void setPosition(Vector3 pos)
	{
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame)
		{
			setPositionAndTransform(pos);
			return;
		}
		
		if(GameManager.loopIndex == 0) prevTransformPosition = transformPosition;
		transformPosition = pos;
		_needPositionRender = true;
	}
	
	
	public void setPositionAndTransform(Vector3 pos)
	{
		prevTransformPosition = pos;
		transformPosition = pos;
		transform.position = pos;
		_needPositionRender = false;
	}
}

