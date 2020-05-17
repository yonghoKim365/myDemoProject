using UnityEngine;
using System.Collections;

public class TargetingDecal : MonoBehaviour {
	
	
	public enum DecalType
	{
		Circle, Arrow, Monster, DeadZone, None
	}

	public enum State
	{
		Start, Hide
	}

	public State state = State.Hide;

	
	public ParticleMagicCircle mc;
	
	public tk2dSprite sprite;
	
	const string CIRCLE_NAME = "decal_01";
	const string ARROW_NAME = "decal_arrow_01";
	const string MONSTER_NAME = "magiccircle_custom02";
	
	public Transform tf;
	
	private DecalType _type = DecalType.None;
	private float _alpha = 0.0f;
	
	private float _targetSize = 1.0f;
	
	private int _decalIndex = 0;
	
	public SimpleRotater rotater;
	
	public bool didStartEffect = false;
	
	Vector3 _v;
	Quaternion _q;
	
	public bool ignoreAngle = false;
	
	public Color colorLv0 = Color.white; // white
	public Color colorLv1 = new Color(5.0f/255.0f, 128.0f/255.0f, 243.0f/255.0f); // blue
	public Color colorLv2 = Color.yellow; // yellow
	public Color colorLv3 = new Color(93.0f/255.0f,45.0f/255.0f,145.0f/255.0f); // violet
	
	public void setColorByChargingLevel(int lv)
	{
		if(sprite == null) return;
		switch(lv)
		{
		case 0:
			_c = sprite.color;
			_c.r = colorLv0.r;
			_c.g = colorLv0.g;
			_c.b = colorLv0.b;
			sprite.color = _c;
			if(rotater.enabled) rotater.speed = 50.0f;
			break;
		case 1:
			_c = sprite.color;
			_c.r = colorLv1.r;
			_c.g = colorLv1.g;
			_c.b = colorLv1.b;
			sprite.color = _c;
			if(rotater.enabled) rotater.speed = 50.0f;//100.0f;
			break;
		case 2:
			_c = sprite.color;
			_c.r = colorLv2.r;
			_c.g = colorLv2.g;
			_c.b = colorLv2.b;
			sprite.color = _c;
			if(rotater.enabled) rotater.speed = 50.0f;//150.0f;
			break;
		case 3:
			_c = sprite.color;
			_c.r = colorLv3.r;
			_c.g = colorLv3.g;
			_c.b = colorLv3.b;
			sprite.color = _c;
			if(rotater.enabled) rotater.speed = 50.0f;//200.0f;
			break;
		}
	}


	public void setColor(Color c)
	{
		sprite.color = c;
	}


	bool _isPlayer = true;

	bool _isDecreaseSpinningSpeedType = false;
	float _decreaseSpinningDuration = 0.0f;
	float _progressTime = 0.0f;

	public int skillExeType = -1;

	public void init(DecalType type, float targetSize = 1.0f, bool isPlayerSide = true, bool startNow = true, float timer = -1.0f)
	{
		_isPlayer = isPlayerSide;

		state = State.Start;

		skillExeType = -1;

		didStartEffect = false;
		_type = type;
		_alpha = 0.1f;
		
		if(type == DecalType.Circle)
		{
			sprite.SetSprite(CIRCLE_NAME);
			_vSize = sprite.scale;
			_vSize.x = 0.1f;
			_vSize.y = 0.1f;
			sprite.scale = _vSize;
			rotater.enabled = true;//(targetSize <= 2.0f);

			setColorByChargingLevel(0);
			_c = sprite.color;
		}
		else if(type == DecalType.Arrow)
		{
			sprite.SetSprite(ARROW_NAME);
			_vSize = sprite.scale;
			_vSize.x = 1.0f;
			_vSize.y = 1.0f;
			sprite.scale = _vSize;
			rotater.enabled = false;

			setColorByChargingLevel(0);
			_c = sprite.color;
		}
		else if(type == DecalType.Monster)
		{
			sprite.SetSprite(MONSTER_NAME);
			_vSize = sprite.scale;
			_vSize.x = 1.0f;
			_vSize.y = 1.0f;
			sprite.scale = _vSize;
			rotater.enabled = true;
			setColor(Color.red);
			_c = sprite.color;
		}
		if(type == DecalType.DeadZone)
		{
			/*
			sprite.SetSprite(CIRCLE_NAME);
			_vSize = sprite.scale;
			_vSize.x = _targetSize;
			_vSize.y = _targetSize;
			sprite.scale = _vSize;
			rotater.enabled = true;//(targetSize <= 2.0f);
			setColor(Color.red);
			_c = sprite.color;
			*/
		}

		if(isPlayerSide)
		{
			_c.a = _alpha;
			
			_v.x = 90.0f;
			_v.y = 0.0f;
			_v.z = 0.0f;
		}
		else
		{
			_c.a = _alpha;
			
			_v.x = 90.0f;
			_v.y = 180.0f;
			_v.z = 0.0f;
		}
		
		
		_q.eulerAngles = _v;
		if(ignoreAngle == false) transform.localRotation = _q;
		
		if(sprite != null) sprite.color = _c;
		_targetSize = targetSize;
		++_decalIndex;
		
		_alphaOffset = 0.9f / (TOTAL_EFFECT_TIME / EFFECT_TIME_OFFSET);
		_fadeAlphaOffset = _alphaOffset * 1.5f;
		_targetOffset = (_targetSize - 0.1f) / (TOTAL_EFFECT_TIME / EFFECT_TIME_OFFSET);
		
		if(startNow) startDecalEffect();


		_isDecreaseSpinningSpeedType = (timer > 0);
		_decreaseSpinningDuration = timer;
		_progressTime = 0.0f;
	}



	public float lineLeft
	{
		get
		{
			return targetPosition.x - _targetSize * 100.0f;
		}
	}

	public float lineRight
	{
		get
		{
			return targetPosition.x + _targetSize * 100.0f;
		}
	}



	
	public void startDecalEffect()
	{
		didStartEffect = true;
		if(_isEnabled == false) isEnabled = true;
		StartCoroutine(decalEffect(_decalIndex));
	}
	
	float _targetOffset = 0.0f;
	float _alphaOffset = 0.0f;
	float _fadeAlphaOffset = 0.0f;
	Vector3 _vSize;
	Color _c;
	
	
	const float TOTAL_EFFECT_TIME = 0.2f;
	const float EFFECT_TIME_OFFSET = 0.03f;

	WaitForSeconds effectTimeOffset = new WaitForSeconds(EFFECT_TIME_OFFSET);

	IEnumerator decalEffect(int nowCallIndex)
	{
		while(true)
		{
			if(nowCallIndex != _decalIndex) break;
			if(_visible == false) break;
			
			if(_alpha < 1.0f)
			{
				_alpha += _alphaOffset;				
				if(_alpha > 1.0f) _alpha = 1.0f;

				if(sprite != null)
				{
					_c = sprite.color;
					_c.a = _alpha;
					sprite.color = _c;
				}
			}
			
			if(_type == DecalType.Circle )
			{
				_vSize = sprite.scale;
				
				if(_vSize.x < _targetSize)
				{
					_vSize.x += _targetOffset;
					if(_vSize.x > _targetSize) _vSize.x = _targetSize;
					_vSize.y = _vSize.x;
					//if(_vSize.y > 2.0f) _vSize.y = 2.0f;
					if(_vSize.y > _targetSize) _vSize.y = _targetSize;
					sprite.scale = _vSize;
				}
				
				if(_alpha >= 1.0f && _vSize.x >= _targetSize) break;
			}
			else
			{
				if(_alpha >= 1.0f) break;
			}
			
			yield return effectTimeOffset;
		}
	}
	
	
	public void hide()
	{
		state = State.Hide;
		didStartEffect = false;
		if(gameObject.activeInHierarchy) StartCoroutine(hideDecal(_decalIndex));
	}
	
	
	IEnumerator hideDecal(int nowCallIndex)
	{
		while(true)
		{
			if(nowCallIndex != _decalIndex) break;
			if(_visible == false) break;
			
			if(_alpha > 0.0f)
			{
				_alpha -= _fadeAlphaOffset;				
				if(_alpha <= 0.0f) _alpha = 0.0f;

				if(sprite != null) 
				{
					_c = sprite.color;
					_c.a = _alpha;
					sprite.color = _c;
				}

				if(_alpha <= 0.0f)
				{
					isEnabled = false;
					break;
				}
			}
			
			yield return effectTimeOffset;
		}
	}
	
	public void changeSize(Vector3 targetScale)
	{
		if(sprite != null) sprite.scale = targetScale;
	}
	
	
	bool _needPositionRender = false;
	public Vector3 targetPosition;
	Vector3 prevTransformPosition = Vector3.zero;
	
	public void setPosition(Vector3 pos)
	{
		if(_firstSetting || GameManager.me.recordMode == GameManager.RecordMode.continueGame)
		{
			prevTransformPosition = transform.position;
			transform.position = pos;
			targetPosition = pos;
			_needPositionRender = false;
			_firstSetting = false;
		}
		else
		{
			if(GameManager.loopIndex == 0) prevTransformPosition = targetPosition;
			targetPosition = pos;
			_needPositionRender = true;
		}
	}
	
	bool _doRenderSkipFrame = false;
	void OnEnable ()
	{
		_doRenderSkipFrame = false;
	}
	
	void OnDisable ()
	{
		_doRenderSkipFrame = false;
	}
	
	Vector3 prevPos;
	void LateUpdate()
	{
		if(GameManager.renderSkipFrame == false) _doRenderSkipFrame = _needPositionRender;
		
		if(_needPositionRender)
		{
			tf.position = targetPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			//prevPos = tf.position;
			//tf.position = VectorUtil.lerp(tf.position, targetPosition, GameManager.renderDeltaTime);
		}
		else if(GameManager.renderSkipFrame && _doRenderSkipFrame)
		{
			tf.position = targetPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			//tf.position = VectorUtil.lerp(prevPos, targetPosition, GameManager.renderDeltaTime);
		}
		_needPositionRender = false;


		if(_isDecreaseSpinningSpeedType)
		{
			_progressTime += Time.smoothDeltaTime;

			if(rotater == null) return;

			float s = (200.0f - (200.0f * MiniTweenEasingType.getEasingValue(_progressTime / _decreaseSpinningDuration, MiniTweenEasingType.EaseIn, EasingType.Cubic))) * 0.95f;
			if(s < 1.0f)
			{
				rotater.speed = 0.0f;
			}
			else
			{
				rotater.speed = s;
			}
		}
	}
	
	private bool _firstSetting = false;
	
	private bool _isEnabled = false;
	public bool isEnabled
	{
		set
		{
			visible = value;
			_isEnabled = value;
			gameObject.SetActive(value);
			if(value == false)
			{
				didStartEffect = false;
				_isDecreaseSpinningSpeedType = false;
				state = State.Hide;
			}
			_firstSetting = true;
		}
		get
		{
			return _isEnabled;
		}
	}
	
	
	private bool _visible = false;
	public bool visible
	{
		set
		{
			if(_visible != value)
			{
				_visible = value;
				if(sprite != null) sprite.renderer.enabled = value;
				if(value == false) _firstSetting = true;
			}
		}
		get
		{
			return _visible;
		}
	}
	
	
}
