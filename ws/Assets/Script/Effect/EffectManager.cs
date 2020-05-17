using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class EffectManager : MonoBehaviour , IManagerBase
{


	public tk2dAnimatedSprite animatedSprite;

	public ParticleSystem[] particleCharging;

	public GetItemEffect getItemEffect;
	public WordEffect wordEffect;
	public BoxTransitionEffect boxTransitionEffect;
	
	public bool isUseEffect = false;	

	private List<tk2dAnimatedSprite> _tk2dAnimatedSprites = new List<tk2dAnimatedSprite>();	
	private Stack<tk2dAnimatedSprite> _tk2dAnimatedSpritePool = new Stack<tk2dAnimatedSprite>();	
	
	private Stack<AttachedEffect> _attachedEffectPool = new Stack<AttachedEffect>();
	private List<AttachedEffect> _attachedEffectList = new List<AttachedEffect>();
	

	private Dictionary<string,Stack<ParticleEffect>> _particleEffectPool = new Dictionary<string, Stack<ParticleEffect>>(StringComparer.Ordinal);
	private List<ParticleEffect> _particleEffectList = new List<ParticleEffect>();	
	
	private Stack<GetItemEffect> _getItemEffectPool = new Stack<GetItemEffect>();
	private List<GetItemEffect> _getItemEffectList = new List<GetItemEffect>();	
	
	private Stack<WordEffect> _getWordEffectPool = new Stack<WordEffect>();
	private List<WordEffect> _getWordEffectList = new List<WordEffect>();	

	private Stack<UIGetItemEffect> _uiGetItemEffectPool = new Stack<UIGetItemEffect>();
	private List<UIGetItemEffect> _uiGetItemEffectList = new List<UIGetItemEffect>();	


	public const int ELECTRIC_EFFECT_TYPE_WHITE = 0;
	public const int ELECTRIC_EFFECT_TYPE_BLACK = 1;
	public const int ELECTRIC_EFFECT_TYPE_MAGIC_BEAM = 2;
	public const int ELECTRIC_EFFECT_TYPE_WHITE2 = 3;
	public const int ELECTRIC_EFFECT_TYPE_DARK2 = 4;

	private Dictionary<string ,Stack<ElectricEffect>> _electricEffectPool = new Dictionary<string, Stack<ElectricEffect>>();

	private List<ElectricEffect> _electricEffectList = new List<ElectricEffect>();		
	
	
	private GameObject _quakeEffectObject = null;
	private EarthQuakeEffect _eq;		
	
	private Stack<Effect> _cutSceneEffectPool = new Stack<Effect>();

//	public GameObject lampContainer;
//	public LampEffect[] lampLineTop = new LampEffect[6];
//	public LampEffect[] lampLineBottom = new LampEffect[6];
		



	void Awake()
	{
	}

	
	void OnDestroy()
	{
		//Debug.Log("OnDestroy : EffectManager" );
		
		foreach(tk2dAnimatedSprite ani in _tk2dAnimatedSprites)
		{
			Destroy(ani);
		}
		_tk2dAnimatedSprites.Clear();
		_tk2dAnimatedSprites = null;

		while(_tk2dAnimatedSpritePool.Count > 0)
		{
			Destroy(_tk2dAnimatedSpritePool.Pop().gameObject);
		}
		_tk2dAnimatedSpritePool = null;
		
		
		foreach(KeyValuePair<string, Stack<ParticleEffect>> kv in _particleEffectPool)
		{
			while(kv.Value.Count > 0)
			{
				ParticleEffect pe = kv.Value.Pop();
				if(pe != null)
				{
					Destroy(pe.gameObject);	
				}
			}
		}
		
		int count = _particleEffectList.Count;
		for(int i = count - 1; i >= 0; --i)
		{
			if(_particleEffectList[i] != null)
			{
				_particleEffectList[i].destroyAsset();
				if(_particleEffectList[i] != null && _particleEffectList[i].gameObject != null) Destroy(_particleEffectList[i].gameObject);
			}
		}
		
		_particleEffectList.Clear();		
		
	
		while(_attachedEffectPool.Count > 0)
		{
			Destroy(_attachedEffectPool.Pop().gameObject);
		}
		_attachedEffectPool = null;		


		
		while(_getItemEffectPool.Count > 0)
		{
			GetItemEffect ge = _getItemEffectPool.Pop();

			if(ge != null && ge.gameObject != null)
			{
				Destroy(_getItemEffectPool.Pop().gameObject);
			}
		}


		while(_uiGetItemEffectPool.Count > 0)
		{
			try
			{
				Destroy(_uiGetItemEffectPool.Pop().gameObject);
			}
			catch(Exception e)
			{

			}
		}

		_uiGetItemEffectPool.Clear();

		
		while(_getWordEffectPool.Count > 0)
		{
			Destroy(_getWordEffectPool.Pop());
		}
		_getWordEffectPool = null;			
		
		
		foreach(KeyValuePair<string, Stack<ElectricEffect>> kv in _electricEffectPool)
		{
			while(kv.Value.Count > 0)
			{
				ElectricEffect e = kv.Value.Pop();
				if(e != null)
				{
					Destroy(e);	
				}
			}
		}

		_electricEffectPool = null;	


		
		while(_cutSceneEffectPool.Count > 0)
		{
			Destroy(_cutSceneEffectPool.Pop());
		}
		_cutSceneEffectPool = null;				
		
		
		foreach(AttachedEffect ani in _attachedEffectList)
		{
			Destroy(ani.gameObject);
		}
		_attachedEffectList.Clear();
		_attachedEffectList = null;	
		

		
		foreach(GetItemEffect ani in _getItemEffectList)
		{
			Destroy(ani.gameObject);
		}
		_getItemEffectList.Clear();
		_getItemEffectList = null;			
		
		foreach(WordEffect ani in _getWordEffectList)
		{
			if(ani != null) Destroy(ani.gameObject);
		}
		_getWordEffectList.Clear();
		_getWordEffectList = null;	
		
		
		foreach(ElectricEffect ani in _electricEffectList)
		{
			if(ani != null) Destroy(ani.gameObject);
		}
		_electricEffectList.Clear();
		_electricEffectList = null;			
	}




	public void initChargingEffect()
	{
		EffectData ed = GameManager.info.effectData["E_FX_CHARGING_SS"];
		setChargingEffect(ed.resource, true, 4); // ss 급.
		setChargingEffect(ed.resource, false, 10); // ss 급.

	}

	void setChargingEffect(string path, bool isPlayerSide, int slotIndex)
	{
		GameObject go = GameManager.resourceManager.getInstantPrefabs(path);
		string fuck = path.Substring(path.IndexOf("/")+1);
		particleCharging[slotIndex] = go.transform.FindChild(fuck).GetComponent<ParticleSystem>();
		go.SetActive(false);

		if(isPlayerSide)
		{
			go.transform.parent = particleCharging[0].transform.parent.parent;
		}
		else
		{
			go.transform.parent = particleCharging[6].transform.parent.parent;
		}

		go.transform.localPosition = Vector3.zero;

	}




	void destroyAttachedEffect(AttachedEffect ae)
	{
//		ae.destroy();
//		DestroyImmediate(ae.gameObject);
	}



	public void destroyAssets()
	{
		int count = 0;//_getWordEffectList.Count;

//		for(int i = count -1 ; i >= 0; --i)
//		{
//			destroyWordEffect(_getWordEffectList[i]);
//		}
//
//		while(_getWordEffectPool.Count > 0)
//		{
//			destroyWordEffect(_getWordEffectPool.Pop());
//		}
//
//		_getWordEffectList.Clear();

//		while(_attachedEffectPool.Count > 0)
//		{
//			destroyAttachedEffect(_attachedEffectPool.Pop());
//		}


		while(_cutSceneEffectPool.Count > 0)
		{
			destroyEffect(_cutSceneEffectPool.Pop());
		}

		foreach(KeyValuePair<string, Stack<ParticleEffect>> kv in _particleEffectPool)
		{
			while(kv.Value.Count > 0)
			{
				destroyParticleEffect(kv.Value.Pop());	
			}
		}

		count = _particleEffectList.Count;
		
		for(int i = count -1 ; i >= 0; --i)
		{
			destroyParticleEffect(_particleEffectList[i]);
		}		
		
		_particleEffectList.Clear();
	}


	public void clearWordEffect()
	{
		for(int i = _getWordEffectList.Count -1 ; i >= 0; --i)
		{
			setWordEffect(_getWordEffectList[i]);
		}
		
		_getWordEffectList.Clear();
	}

	
	public void clearStage(bool flag = true)
	{
		if(particleCharging != null)
		{
			foreach(ParticleSystem ps in particleCharging)
			{
				if(ps != null)
				{
					ps.transform.parent.gameObject.SetActive(false);
				}
			}
		}

		GameManager.me.mapManager.effectArrive = null;

		int count = _tk2dAnimatedSprites.Count;
		
		for(int i = count -1 ; i >= 0; --i)
		{
			setAnimatedSprite(_tk2dAnimatedSprites[i]);
		}

		count = _getItemEffectList.Count;
		
		for(int i = count -1 ; i >= 0; --i)
		{
			setGetItemEffect(_getItemEffectList[i]);
		}		
		
		count = _getWordEffectList.Count;
		
		for(int i = count -1 ; i >= 0; --i)
		{
			setWordEffect(_getWordEffectList[i]);
			//destroyWordEffect(_getWordEffectList[i]);
		}

		_getWordEffectList.Clear();


		count = _attachedEffectList.Count;


		for(int i = count -1 ; i >= 0; --i)
		{
			setAttachedEffect(_attachedEffectList[i]);
		}

		_attachedEffectList.Clear();


		count = _particleEffectList.Count;

		for(int i = count -1 ; i >= 0; --i)
		{
			setParticleEffect(_particleEffectList[i]);
			//destroyParticleEffect(_particleEffectList[i]);
		}		

		_particleEffectList.Clear();

		/*
		foreach(KeyValuePair<string, Stack<ParticleEffect>> kv in _particleEffectPool)
		{
			while(kv.Value.Count > 0)
			{
				destroyParticleEffect(kv.Value.Pop());	
			}
		}

		while(_cutSceneEffectPool.Count > 0)
		{
			destroyEffect(_cutSceneEffectPool.Pop());
		}
		*/


		if(_eq != null) _eq.isEnabled = false;
	}


	public void destroyParticleInParticleEffectPool(string name)
	{
		if(_particleEffectPool.ContainsKey(name))
		{
			while(_particleEffectPool[name].Count > 0)
			{
				ParticleEffect pe = _particleEffectPool[name].Pop();
				if(pe != null)
				{
					Destroy(pe.gameObject);	
				}
			}
		}
	}


	public void checkParticleEffectPool(EffectData ed)
	{
		if(_particleEffectPool.ContainsKey(ed.id) == false)
		{
			_particleEffectPool.Add(ed.id, new Stack<ParticleEffect>());
		}
	}



	void destroyEffect(Effect e)
	{
		e.isEnabled = false;
		e.destroyAsset();
		Destroy(e.gameObject);
	}



	void destroyParticleEffect(ParticleEffect pe)
	{
		if(pe != null)
		{
			pe.isEnabled = false;
			pe.destroyAsset();
			DestroyImmediate(pe.gameObject,true);
		}
	}


	
	public IEnumerator preLoad()
	{
		int i = 0;
		
		yield return null;

		for(i = 0; i < 15; ++i)
		{
			tk2dAnimatedSprite spr;
			
			if(_tk2dAnimatedSpritePool.Count > 0) spr = _tk2dAnimatedSpritePool.Pop();
			else spr = ((tk2dAnimatedSprite)Instantiate(animatedSprite));			
			spr.StopAndResetFrame();			
			spr.transform.parent = GameManager.me.assetPool;
			spr.gameObject.SetActive( false );
			_tk2dAnimatedSpritePool.Push(spr);
		}
		
//		Debug.Log("tk2dAnimatedSprite Complete");
		
		yield return null;
		
		for(i = 0; i < 25; ++i)
		{
			WordEffect we = (WordEffect)Instantiate(wordEffect);
			we.gameObject.SetActive(false);
			we.isEnabled = false;
			we.transform.parent = GameManager.me.assetPool;
			_getWordEffectPool.Push(we);
		}
		
//		Debug.Log("WordEffect Complete");
		yield return null;
		
		for(i = 0; i < 20; ++i)
		{
			GetItemEffect ge = (GetItemEffect)Instantiate(getItemEffect);
			ge.gameObject.SetActive(false);
			ge.isEnabled = false;
			ge.transform.parent = GameManager.me.assetPool;
			_getItemEffectPool.Push(ge);
		}		
		
//		Debug.Log("GetItemEffect Complete");
		yield return null;
		
	}		
	
	
	
	// -- 애니메이션 
	

	
	public void setAnimatedSprite(tk2dAnimatedSprite spr)
	{
		//spr.enabled = false;
		spr.transform.parent = GameManager.me.assetPool;
		
		_v = spr.scale;
		_v.x = 1.0f;
		_v.y = 1.0f;
		_v.z = 1.0f;
		spr.scale = _v;
		
		spr.gameObject.SetActive(false);
		_tk2dAnimatedSprites.Remove(spr);
		_tk2dAnimatedSpritePool.Push(spr);
	}
	
	public tk2dAnimatedSprite getAnimatedSprite()
	{
		tk2dAnimatedSprite spr;
		
		if(_tk2dAnimatedSpritePool.Count > 0) spr = _tk2dAnimatedSpritePool.Pop();
		else spr = ((tk2dAnimatedSprite)Instantiate(animatedSprite));
		
		_tk2dAnimatedSprites.Add(spr);
		
		return spr;
	}
	
	
	public void init()
	{
	}
	

	
	
	// -- 붙이는 이펙트 -- //
	
	
	public void setAttachedEffect(AttachedEffect eff)
	{
		eff.isEnabled = false;
		eff.transform.parent = GameManager.me.assetPool;
		_attachedEffectList.Remove(eff);
		_attachedEffectPool.Push(eff);	
	}
	
	public AttachedEffect getAttachedEffect()
	{
		AttachedEffect eff;
		
		if(_attachedEffectPool.Count > 0) eff = _attachedEffectPool.Pop();
		else eff = new AttachedEffect();
		
		_attachedEffectList.Add(eff);
		
		return eff;
	}	
	
	
	// -- 컷씬 이펙트 -- //
	
	public void setCutSceneEffect(Effect eff)
	{
//		if(eff == null) return;
		eff.isEnabled = false;
		eff.transform.parent = GameManager.me.assetPool;
		_cutSceneEffectPool.Push(eff);
	}

	public Effect getCutSceneEffect()
	{
		Effect eff;
		
		if(_cutSceneEffectPool.Count > 0)
		{
			eff = _cutSceneEffectPool.Pop();
		}
		else
		{
			eff = new GameObject().AddComponent<Effect>();	
			eff.tf = eff.transform;
		}

		eff.gameObject.SetActive(true);
		return eff;
	}	
	
	
	
	// -- 체인 라이트닝 -- //
	
	public void setElectricEffect(ElectricEffect eff)
	{
		//Debug.Log("======= setParticleEffect " +  eff);
		eff.isEnabled = false;
		eff.transform.parent = GameManager.me.assetPool;
		_electricEffectList.Remove(eff);

		if(_electricEffectPool.ContainsKey(eff.type) == false)  _electricEffectPool.Add(eff.type, new Stack<ElectricEffect>());

		_electricEffectPool[eff.type].Push(eff);
	}
	


	public ElectricEffect getElectricEffect(string type)
	{
		ElectricEffect eff;

		if(_electricEffectPool.ContainsKey(type) &&  _electricEffectPool[type].Count > 0) eff = _electricEffectPool[type].Pop();
		else
		{
			eff = GameManager.info.effectData[type].getElectricEffect();	
		}

		eff.type = type;

		_electricEffectList.Add(eff);
		
		return eff;
	}	
	
	
	// -- 파티클 이펙트 -- //
	
	public void setParticleEffect(ParticleEffect eff)
	{

		if(eff != null && eff.isDestroyed == false)
		{
			eff.isEnabled = false;
			eff.tf.parent = GameManager.me.assetPool;
			_particleEffectList.Remove(eff);

			if(_particleEffectPool[eff.name].Contains(eff) == false)
			{
				_particleEffectPool[eff.name].Push(eff);
			}
		}
	}
	

	public void preloadParticleEffects(string name, string path, int num)
	{
		if(_particleEffectPool.ContainsKey(name) == false)
		{
			_particleEffectPool.Add(name, new Stack<ParticleEffect>());
		}

		for(int j = 0; j < num ; ++j)
		{
			ParticleEffect eff;

			GameObject go = GameManager.resourceManager.getInstantPrefabs(path);
			GameObject container = new GameObject();
			go.transform.parent = container.transform;
			eff = container.AddComponent<ParticleEffect>();
			eff.tf = eff.transform;
			eff.go = eff.gameObject;
			eff.particle = go.particleSystem;
			
			if(eff.particle == null)
			{
				string fuck = path.Substring(path.IndexOf("/")+1);
				Transform tf = go.transform.FindChild(fuck);

				#if UNITY_EDITOR
				if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.gameResourceErrorCheck)
				{
					try
					{
						eff.particle = tf.GetComponent<ParticleSystem>();
					}
					catch(Exception e)
					{
						Debug.LogError("****  EFFECT NAME ERROR : " + name);
					}
				}
				else
				#endif
				{
					eff.particle = tf.GetComponent<ParticleSystem>();
				}
			}

			eff.particles = go.transform.GetComponentsInChildren<ParticleSystem>();
			eff.particleCount = eff.particles.Length;
			eff.startSize = new float[eff.particleCount];
			eff.startSpeed = new float[eff.particleCount];
			for(int i = 0; i < eff.particleCount ; ++i)
			{
				eff.startSize[i] = eff.particles[i].startSize;
				eff.startSpeed[i] = eff.particles[i].startSpeed;
			}
			
			eff.boxCollder = go.transform.GetComponentInChildren<BoxCollider>();
			if(eff.boxCollder != null) eff.boxCollder.enabled = false;
			
			eff.name = name;
			eff.path = path;

			eff.isEnabled = false;
			eff.tf.parent = GameManager.me.assetPool;
			_particleEffectPool[eff.name].Push(eff);
		}
	}


	public ParticleEffect getParticleEffect(string name, string path)
	{
#if UNITY_EDITOR

		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
//			Debug.Log(name);
		}
#endif

		ParticleEffect eff;

		if(_particleEffectPool[name].Count > 0)
		{
			eff = _particleEffectPool[name].Pop();

			if(eff == null)
			{
				return getParticleEffect(name, path);
			}
		}
		else
		{
			GameObject go = GameManager.resourceManager.getInstantPrefabs(path);
			GameObject container = new GameObject();
			go.transform.parent = container.transform;
			_v.x = 0; _v.y = 0; _v.z = 0;
			go.transform.localPosition = _v;
			eff = container.AddComponent<ParticleEffect>();
			eff.tf = eff.transform;
			eff.particle = go.particleSystem;
			
			if(eff.particle == null)
			{
				string fuck = path.Substring(path.IndexOf("/")+1);
				Transform tf = go.transform.FindChild(fuck);
				//Transform tf = go.transform.GetChild(0);
				eff.particle = tf.GetComponent<ParticleSystem>();
			}
			
			eff.particles = go.transform.GetComponentsInChildren<ParticleSystem>();
			
			eff.particleCount = eff.particles.Length;
			eff.startSize = new float[eff.particleCount];
			eff.startSpeed = new float[eff.particleCount];
			for(int i = 0; i < eff.particleCount ; ++i)
			{
				eff.startSize[i] = eff.particles[i].startSize;
				eff.startSpeed[i] = eff.particles[i].startSpeed;
			}
			
			eff.boxCollder = go.transform.GetComponentInChildren<BoxCollider>();
			if(eff.boxCollder != null) eff.boxCollder.enabled = false;
			
			eff.name = name;
			eff.path = path;
		}
		
		_particleEffectList.Add(eff);
		eff.isEnabled = isUseEffect;
		
		return eff;
	}
	

	public void skillCamParticleEffectVisibleIsHide(bool isHide)
	{
		if(isHide)
		{
			for(int i = _particleEffectList.Count - 1; i>= 0; --i)
			{
				_particleEffectList[i].nowOnSkillEffectMode = true;
				_particleEffectList[i].setVisible(false);
			}
		}
		else
		{
			for(int i = _particleEffectList.Count - 1; i>= 0; --i)
			{
				if(_particleEffectList[i].nowOnSkillEffectMode)
				{
					_particleEffectList[i].nowOnSkillEffectMode = false;
					_particleEffectList[i].setVisible(true);
				}
			}
		}
	}

	
	
	// -- 아이템 획득 이펙트 -- //
	

	
	public void setGetItemEffect(GetItemEffect eff)
	{
		eff.isEnabled = false;
		eff.transform.parent = GameManager.me.assetPool;
		_getItemEffectList.Remove(eff);
		_getItemEffectPool.Push(eff);
	}
	
	public GetItemEffect getGetItemEffect()
	{
		GetItemEffect eff;
		
		if(_getItemEffectPool.Count > 0)
		{
			eff = _getItemEffectPool.Pop();
		}
		else
		{
			eff = (GetItemEffect)Instantiate(getItemEffect);	
		}
		
		_getItemEffectList.Add(eff);
		
		eff.gameObject.transform.parent = GameManager.me.mapManager.mapStage;//GameManager.me.player.tf.parent;
		eff.isEnabled = true;
		
		return eff;
	}	


	public UIGetItemEffect uiGetItemEffect;

	public void setAllUIGetItemEffect()
	{
		for(int i = _uiGetItemEffectList.Count - 1; i >= 0; --i)
		{
			_uiGetItemEffectList[i].gameObject.SetActive(false);
			_uiGetItemEffectPool.Push(_uiGetItemEffectList[i]);
		}

		_uiGetItemEffectList.Clear();
	}


	public void setUIGetItemEffect(UIGetItemEffect eff)
	{
		_uiGetItemEffectList.Remove(eff);
		_uiGetItemEffectPool.Push(eff);
	}
	
	public UIGetItemEffect getUIGetItemEffect()
	{
		UIGetItemEffect eff;
		
		if(_uiGetItemEffectPool.Count > 0)
		{
			eff = _uiGetItemEffectPool.Pop();
		}
		else
		{
			eff = (UIGetItemEffect)Instantiate(uiGetItemEffect);	
		}
		
		_uiGetItemEffectList.Add(eff);
		
		eff.gameObject.transform.parent = uiGetItemEffect.transform.parent;

		return eff;
	}	

	public void startGetUIItemEffect(Vector3 position, string id)
	{
		UIGetItemEffect eff = getUIGetItemEffect();

		eff.StartEffect( GameManager.me.uiManager.uiPlay.uiPlayCamera.ScreenToWorldPoint( GameManager.me.uiManager.uiPlay.gameCamera.WorldToScreenPoint(position) ), id);
	}

	
	
	// -- 문자 이펙트 -- //
	
	public void destroyWordEffect(WordEffect eff)
	{
		//Debug.LogError("eff : " + eff);

		eff.isEnabled = false;
		eff.destroyAsset();
		Destroy(eff.gameObject);
	}
	
	public void setWordEffect(WordEffect eff)
	{
		eff.isEnabled = false;
		eff.transform.parent = GameManager.me.assetPool;
		_getWordEffectList.Remove(eff);
		_getWordEffectPool.Push(eff);
	}
	
	public WordEffect getWordEffect()
	{
		WordEffect eff;
		
		if(_getWordEffectPool.Count > 0)
		{
			eff = _getWordEffectPool.Pop();
		}
		else
		{
			eff = (WordEffect)Instantiate(wordEffect);	
		}
		
		_getWordEffectList.Add(eff);
		
		eff.gameObject.transform.parent = GameManager.me.characterManager.inGameGUIContinaer;
		eff.isEnabled = true;
		
		return eff;
	}	
	
	
	
	// 지진 이펙트 //
	
	// 카메라가 위아래로 흔들거리는 이펙트.
	

	
	
	public void quakeEffect(float time, float size, EarthQuakeEffect.Type type = EarthQuakeEffect.Type.Vertical, EarthQuakeEffect.callback callback = null, EarthQuakeEffect.MethodType method = EarthQuakeEffect.MethodType.ByMath)
	{
		if(GameManager.me.cutSceneManager.useCutSceneCamera && UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None) return;

		if(_quakeEffectObject == null)
		{
			_quakeEffectObject = new GameObject("quakeEffect");	
			_eq = _quakeEffectObject.AddComponent<EarthQuakeEffect>();
		}
		
		_eq.init(GameManager.me.gameCamera, time, size, type, callback, method);
		_eq.isEnabled = true;
	}
	
	
	
	public void stopQuakeEffect()
	{
		if(_eq != null && _eq.isEnabled == true) _eq.isEnabled = false;
	}
	
	
	
	
	private int index = 0;
	private Vector3 _v;

	
	
	
	
	public tk2dAnimatedSprite getEffect(string name, Vector3 pos, Transform parent, bool isLoop = true, tk2dAnimatedSprite.AnimationCompleteDelegate completeDelegate = null)
	{
		tk2dAnimatedSprite eff = getAnimatedSprite();
		eff.gameObject.SetActive(true);
		//eff.gameObject.name = name;
		eff.transform.parent = parent;
		eff.transform.position = pos;

		eff.Play(name,0.0f);
		
		if(isLoop)
		{
			eff.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
			eff.animationCompleteDelegate = null;//completeGetPopEffect;
		}
		else
		{
			eff.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
			eff.animationCompleteDelegate = completeDelegate;
		}
		
		
		return eff;
	}	
	


	
	private static Vector3 _vs;
	private static string _effId;
	public static void showEfectBySkillEffectType(int effId, Monster skillTarget, Bullet bullet, bool isUp = true)
	{
		if(bullet != null)
		{
			_vs = bullet.bTransform.position;
			_vs.x += (skillTarget.cTransformPosition.x - _vs.x)*0.5f;
			
			if(isUp) _effId = GameManager.info.skillEffectSetupData[effId].effUp;
			else _effId = GameManager.info.skillEffectSetupData[effId].effDown;

			if(string.IsNullOrEmpty(_effId) == false) GameManager.info.effectData[_effId].getEffect(bullet.attackerInfo.uniqueId,_vs, null, null);
		}
		else
		{
			if(isUp)
			{
				skillTarget.playDamageSoundAndEffect(-1000,true, GameManager.info.skillEffectSetupData[effId].soundUp, GameManager.info.skillEffectSetupData[effId].effUp);
			}
			else
			{
				skillTarget.playDamageSoundAndEffect(-1000,true, GameManager.info.skillEffectSetupData[effId].soundDown, GameManager.info.skillEffectSetupData[effId].effDown); 
			}
		}	
	}
	

	public enum SkillEffectType
	{
		isUp, isDown, upLoop, downLoop, iconUp, iconDown
	}

	public static ParticleEffect showEfectBySkillEffectTypeWithCharacterSize(int effId, Monster target, SkillEffectType type, int attackerUniqueId = -1000)
	{
		switch(type)
		{
		case  SkillEffectType.isUp:
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[effId].effUp)) return null;
			return GameManager.info.effectData[GameManager.info.skillEffectSetupData[effId].effUp].getParticleEffectByCharacterSize(attackerUniqueId, target);
			break;
		case  SkillEffectType.isDown:
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[effId].effDown)) return null;
			return GameManager.info.effectData[GameManager.info.skillEffectSetupData[effId].effDown].getParticleEffectByCharacterSize(attackerUniqueId, target);
			break;
		case  SkillEffectType.upLoop:
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[effId].effUpLoop)) return null;
			return GameManager.info.effectData[GameManager.info.skillEffectSetupData[effId].effUpLoop].getParticleEffectByCharacterSize(attackerUniqueId, target, target.tf, target.tf);
			break;
		case  SkillEffectType.downLoop:
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[effId].effDownLoop)) return null;
			return GameManager.info.effectData[GameManager.info.skillEffectSetupData[effId].effDownLoop].getParticleEffectByCharacterSize(attackerUniqueId, target, target.tf, target.tf);
			break;
		}

		return null;
	}	




	public static void showHitEfectWithCharacterSize(BulletData bd, Monster target, int attackerUniqueId = -1000)
	{
		if(string.IsNullOrEmpty(bd.hitEffect)) return;
		if(target == null) return;

		switch(bd.hitEffectOption)
		{
		case BulletData.HitEffectOptionType.Normal:
			GameManager.info.effectData[bd.hitEffect].getParticleEffectByCharacterSize(attackerUniqueId, target, target.tf, target.tf);
			break;
		case BulletData.HitEffectOptionType.IgnoreAll:
			target.playHitEffect(attackerUniqueId, bd.hitEffect, false, false, 0, 0, 0, true);
			break;
		case BulletData.HitEffectOptionType.IgnoreParent:
			target.playHitEffect(attackerUniqueId, bd.hitEffect, true);
			break;
		case BulletData.HitEffectOptionType.IgnoreSize:
			GameManager.info.effectData[bd.hitEffect].getParticleEffect(attackerUniqueId, target, target.tf, target.tf);
			break;
		}


	}	



}
