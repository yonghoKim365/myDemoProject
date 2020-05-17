using UnityEngine;
using System.Collections;

sealed public class ParticleEffect : MonoBehaviour 
{
	public const float MAX_TIME_LIMIT = 10000f;

	public int shooterUniqueId = -1000;
	public int containerUniqueId = -1000;

	public ParticleSystem particle;
	public Transform tf;
	public GameObject go;
	
	public ParticleSystem[] particles;
	public float[] startSize;
	public float[] startSpeed;
	public int particleCount;

	public string path = "";

	Vector3 _v;

	void Awake()
	{
		go = gameObject;
	}

	void Start () 
	{
	}

	public BoxCollider boxCollder;
	
	public bool ready = false;
	private Transform parentContainer;
	private Vector3 _dist = new Vector3();

	private bool _isTimeLimitType = false;
	private float _timeLimit = 0.0f;
	public float timeLimit
	{
		set
		{
			_timeLimit = value;
			_isTimeLimitType = true;
		}
		get
		{
			return _timeLimit;
		}
	}
	
	float _time = 0.0f;
	
	int i;

	public void resize(float scale = 1.0f)
	{
		if(particle != null) particle.Stop();

		_v = tf.localScale;
		_v.x = scale;
		_v.y = scale;
		_v.z = scale;
		tf.localScale = _v;
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].startSize = startSize[i] * scale;
			particles[i].startSpeed = startSpeed[i] * scale;
		}

		if(particle != null) particle.Play(false);
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].Play(false);
		}
	}


	public void setMaxParticle(int maxNum)
	{
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].maxParticles = maxNum;
		}
	}


	public void setStartColor(Color color)
	{
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].startColor = color;
		}
	}



	
	public void start(Vector3 pos, Transform parent, bool isLocalPosition = true, float scale = 1.0f, float px = 0.0f, float py = 0.0f, float pz = 0.0f)
	{
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			GameManager.me.effectManager.setParticleEffect(this);
			return;
		}
#endif

		shooterUniqueId = -1000;
		containerUniqueId = -1000;

		isDestroyed = false;
		_v = tf.localScale;
		_v.x = scale;
		_v.y = scale;
		_v.z = scale;
		tf.localScale = _v;
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].startSize = startSize[i] * scale;
			particles[i].startSpeed = startSpeed[i] * scale;
		}
		
		//pos.y += 5.0f;
		//pos.z -= 5.0f;
		
		pos.x += px;
		pos.y += py;
		pos.z += pz;
		
		if(isLocalPosition) tf.localPosition = pos;
		else tf.position = pos;		
		
		
		parentContainer = parent;
		
		if(parent != null) _dist = tf.position - parent.position;
		
		if(particle != null) particle.Play(false);

		for(i = 0; i < particleCount; ++i)
		{
			particles[i].Play(false);
		}

		ready = true;

		_isTimeLimitType = false;
		timeLimit = ParticleEffect.MAX_TIME_LIMIT;
		
		_time = 0.0f;
		
	}		


	public void setTimeLimit(float tlimit)
	{

		if(particle != null && particle.loop == false && tlimit > ParticleEffect.MAX_TIME_LIMIT - 1)
		{
			timeLimit = particle.duration;
		}
		else
		{
			timeLimit = tlimit;
		}
	}



	public void playParticle()
	{
		if(particle != null) particle.Play(false);
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].Play(false);
		}
	}


	public bool nowOnSkillEffectMode = false;

	public static int SKILL_EFFECT_SHOOTER_ID = -1000;

	public void checkSkillCam()
	{
		if(UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None && (UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill || UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill ))
		{
			if(shooterUniqueId < 0 || shooterUniqueId != ParticleEffect.SKILL_EFFECT_SHOOTER_ID)
			{
				nowOnSkillEffectMode = true;
				setVisible(false);
			}
		}
	}


	public void setVisible(bool isShow)
	{
		if(particle != null)
		{
			particle.renderer.enabled = isShow;
		}
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].renderer.enabled = isShow;
		}
	}


	public void monsterDeadCallback(int uniqueId)
	{
		if(containerUniqueId == uniqueId)
		{
			GameManager.me.effectManager.setParticleEffect(this);
		}
	}



	
	void LateUpdate () 
	{

		if(ready && particle != null && particle.isPlaying == false)
		{
			GameManager.me.effectManager.setParticleEffect(this);
		}
		else if(parentContainer != null && parentContainer.gameObject.activeInHierarchy)
		{
			tf.position = parentContainer.position + _dist;
		}
		
		if(ready)
		{
			_time += Time.smoothDeltaTime;

			if(_time >= timeLimit && _isTimeLimitType)
			{
				GameManager.me.effectManager.setParticleEffect(this);
			}
		}
	}



	
	public bool isEnabled
	{
		set
		{
			if(go != null)
			{
				go.SetActive(value);
			}

			if(value == false)
			{
#if UNITY_EDITOR
//				Debug.LogError(gameObject.name);
#endif
				ready = value;

				if(particle != null)
				{
					particle.Stop(true);
					particle.Clear(true);
				}

				shooterUniqueId = -1000;
				containerUniqueId = -1000;

				nowOnSkillEffectMode = false;
				setVisible(true);

				parentContainer = null;

				_time = 0.0f;

				_isTimeLimitType = false;
			}
		}
	}
	
	public bool isDestroyed = false;
	
	public void destroyAsset()
	{
		if(isDestroyed) return;

		//Debug.Log("destroyAsset");

		isDestroyed = true;

		name = null;
		particle = null;
		tf = null;
		go = null;
		particles = null;
		startSize = null;
		startSpeed = null;
		boxCollder = null;
		parentContainer = null;

		if(GameManager.resourceManager != null) GameManager.resourceManager.destroyPrefab(path);
		path = null;
	}

	void OnDestroy()
	{
		if(isDestroyed) return;

		//Debug.Log("OnDestroy");

		isDestroyed = true;

		name = null;
		particle = null;
		tf = null;
		go = null;
		particles = null;
		startSize = null;
		startSpeed = null;
		boxCollder = null;
		parentContainer = null;
		path = null;
	}

}
