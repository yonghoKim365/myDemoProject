using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectData
{
	public EffectData ()
	{
	}
	
	public string id;
	public string resource;
	public int preloadNum = 0;

	public float characterParticleDefaultYPos = 0.0f;

#if UNITY_EDITOR
	public string path;
#endif

	public enum ResourceType
	{
		SCENE, PREFAB, PARTICLE, SCENE_PARTICLE, BULLET, CHARACTER, CHAIN
	}
	
	public ResourceType type;
	
	const string SCENE = "SCENE";
	const string PREFAB = "PREFAB";
	const string BULLET = "BULLET";
	const string CHARACTER = "CHARACTER";
	const string PARTICLE = "PARTICLE";
	const string CHAIN = "CHAIN";



	public const string E_P_START = "E_P_START";
	public const string E_P_HIT = "E_P_HIT";


	public bool hasCollider = false;
	public Vector3 colliderCenter = new Vector3();
	public Vector3 colliderSize = new Vector3();
	public bool isRotated = false;


	public float scaleFactor = -1000.0f;

	public float maxSizeRatio = 10000f;


	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		
		resource = (string)l[k["RESOURCE"]];


		#if UNITY_EDITOR
		
		try
		{
			path = (string)l[k["PATH"]];
		}
		catch
		{
			path = null;
		}
		#endif



		Util.parseObject(l[k["ATTACHED_TYPE"]], out characterParticleDefaultYPos, -1.0f);

		Util.parseObject(l[k["PRELOAD"]], out preloadNum, true, 0);

#if UNITY_EDITOR
		if(Application.isPlaying && UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.gameResourceErrorCheck)
		{
			preloadNum = 1;
		}
#endif

		string c = l[k["CENTER"]].ToString();
		if(string.IsNullOrEmpty(c) == false && c.Contains(","))
		{
			hasCollider = true;
			colliderCenter = Util.stringToVector3(c,',');
			colliderSize = Util.stringToVector3(l[k["SIZE"]].ToString(),',');

			isRotated = l[k["HASROT"]].ToString().Equals("Y");
		}


		Util.parseObject(l[k["SCALE"]], out scaleFactor, -1000.0f);

		if(k.ContainsKey("MAXSIZE"))
		{
			Util.parseObject(l[k["MAXSIZE"]], out maxSizeRatio, 10000.0f);
		}


		switch((string)l[k["RESOURCE_TYPE"]])
		{
		case SCENE:
			type = ResourceType.SCENE;
			break;
		case PREFAB:
			type = ResourceType.PREFAB;
			break;
		case BULLET:
			type = ResourceType.BULLET;
			break;			
		case CHARACTER:
			type = ResourceType.CHARACTER;
			break;				
		case PARTICLE:
			type = ResourceType.PARTICLE;
			break;			
		case CHAIN:
			type = ResourceType.CHAIN;
			break;
		}



	}
	
	public Monster effectChracter = null;
	
	
	
	public EffectData clone()
	{
		EffectData ed = new EffectData();
		ed.type = type;
		ed.id = id;
		ed.resource = resource;
		return ed;
	}
	


	public ElectricEffect getElectricEffect()
	{
		GameObject go = GameManager.resourceManager.getInstantPrefabs(resource);
		return go.GetComponent<ElectricEffect>();
	}



	public GameObject getEffect(int shooterUniqueId, Vector3 pos, int containerUniqueId, Monster shooter, Transform parent = null, Transform parentContainer = null, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, bool isLocalPosition = false)
	{
		GameObject go = null;
		
		switch(type)
		{
		case ResourceType.PREFAB:
			if(parent == null) parent = GameManager.me.deletePool;
			go= GameManager.resourceManager.getInstantPrefabs(resource);			
			go.transform.parent = parent;
			go.transform.position = pos;
			break;
		case ResourceType.PARTICLE:
			ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(id, resource);
			pe.start(pos, parentContainer, isLocalPosition);
			pe.transform.parent = parent;

			if(isLocalPosition)
			{
				pe.tf.localPosition = pos;
			}

			pe.setTimeLimit( timeLimit );
			pe.containerUniqueId = containerUniqueId;
			pe.shooterUniqueId = shooterUniqueId;
			pe.checkSkillCam();

			shooter.monsterDeadCallback -= pe.monsterDeadCallback;
			shooter.monsterDeadCallback += pe.monsterDeadCallback;

			return pe.gameObject;
			break;			
		case ResourceType.BULLET:
			//GameManager.me.bulletManager.getBullet();
			break;
		}
		return go;
	}


	public GameObject getPrefabEffect()
	{
		return GameManager.resourceManager.getPrefabFromPool(resource);
	}


	// uniqueId 는 쓴 애를 말하는 것임... 몸에 붙은 애가 아니라 이펙트를 유발시킨 놈.
	public GameObject getEffect(int uniqueId, Vector3 pos, Transform parent = null, Transform parentContainer = null, float scale = 1.0f, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f)
	{
		GameObject go = null;
		
		switch(type)
		{
		case ResourceType.PREFAB:
			if(parent == null) parent = GameManager.me.deletePool;
			go= GameManager.resourceManager.getInstantPrefabs(resource);			
			go.transform.parent = parent;
			go.transform.position = pos;
			break;
		case ResourceType.PARTICLE:

			if(scale > maxSizeRatio) scale = maxSizeRatio;

			ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(id, resource);
			pe.start(pos, parentContainer, false, scale, posX, posY, posZ);
			pe.transform.parent = parent;
			pe.setTimeLimit( timeLimit );
			pe.shooterUniqueId = uniqueId;
			pe.checkSkillCam();
			return pe.gameObject;
			break;			
		case ResourceType.BULLET:
			//GameManager.me.bulletManager.getBullet();
			break;
		}
		return go;
	}



	public ParticleEffect getParticleEffect(int uniqueId, Vector3 pos, Transform parent = null, Transform parentContainer = null, float scale = 1.0f, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f)
	{
		if(scale > maxSizeRatio) scale = maxSizeRatio;

		ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(id, resource);
		pe.start(pos, parentContainer, false, scale, posX, posY, posZ);
		pe.transform.parent = parent;
		pe.setTimeLimit( timeLimit );
		pe.shooterUniqueId = uniqueId;
		pe.checkSkillCam();
		return pe;
	}




	public ParticleEffect getParticleEffect(int shooterUniqueId, Monster cha, Transform parent = null, Transform parentContainer = null, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, float customScaleRatio = 1.2f)
	{
		ParticleEffect pe = null;
		
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
		{
			//return GameManager.me.effectManager.dummyParticleEffect;
		}
		#endif

		pe = GameManager.me.effectManager.getParticleEffect(id, resource);
		if(characterParticleDefaultYPos > 0.0f) posY = cha.hitObject.height * characterParticleDefaultYPos;
		pe.transform.parent = parent;
		pe.start(cha.cTransform.position, parentContainer, false, 1, posX, posY, posZ);
		pe.setTimeLimit( timeLimit );
		pe.shooterUniqueId = shooterUniqueId;
		pe.checkSkillCam();
		return pe;
	}



	public ParticleEffect getParticleEffectByCharacterSize(Monster cha, Transform parent = null, Transform parentContainer = null, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, float customScaleRatio = 1.2f)
	{
		return getParticleEffectByCharacterSize(-1000,cha,parent, parentContainer, timeLimit, posX, posY, posZ, customScaleRatio);
	}

	public ParticleEffect getParticleEffectByCharacterSize(int shooterUniqueId, Monster cha, Transform parent = null, Transform parentContainer = null, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, float customScaleRatio = 1.2f)
	{
		ParticleEffect pe = null;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
		{
			//return GameManager.me.effectManager.dummyParticleEffect;
		}
#endif

		switch(type)
		{
		case ResourceType.PARTICLE:
			pe = GameManager.me.effectManager.getParticleEffect(id, resource);
			if(characterParticleDefaultYPos > 0.0f) posY = cha.hitObject.height * characterParticleDefaultYPos;
			pe.transform.parent = parent;

			customScaleRatio = cha.effectSize * customScaleRatio;
			
			if(customScaleRatio > maxSizeRatio) customScaleRatio = maxSizeRatio;

			pe.start(cha.cTransform.position, parentContainer, false, customScaleRatio, posX, posY, posZ);
			pe.setTimeLimit( timeLimit );
			pe.shooterUniqueId = shooterUniqueId;
			pe.checkSkillCam();
			break;
		}
		return pe;
	}	
	


	Vector3 _v;
	Quaternion _q = new Quaternion();

	public ParticleEffect getDamageParticleEffectByCharacterSize(Monster cha, Transform parent = null, Transform parentContainer = null, int maxNum = 10, float timeLimit = ParticleEffect.MAX_TIME_LIMIT, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, float customScaleRatio = 1.2f)
	{
		ParticleEffect pe = null;
		
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
		{
			//return GameManager.me.effectManager.dummyParticleEffect;
		}
		#endif
		
		pe = GameManager.me.effectManager.getParticleEffect(id, resource);
		if(characterParticleDefaultYPos > 0.0f) posY = cha.hitObject.height * characterParticleDefaultYPos;
		pe.transform.parent = parent;

		pe.setMaxParticle(maxNum);

		if(GameManager.info.modelData[cha.resourceId].particleColorLength > 1)
		{
			pe.setStartColor( GameManager.info.modelData[cha.resourceId].particleColors[ UnityEngine.Random.Range(0,GameManager.info.modelData[cha.resourceId].particleColorLength) ] );
		}
		else
		{
			pe.setStartColor( GameManager.info.modelData[cha.resourceId].particleColors[0] );
		}

		customScaleRatio = cha.effectSize * customScaleRatio;

		if(customScaleRatio > maxSizeRatio) customScaleRatio = maxSizeRatio;

		pe.start(cha.cTransform.position, parentContainer, false, customScaleRatio, posX, posY, posZ);


		pe.setTimeLimit( timeLimit );

		_v = parentContainer.rotation.eulerAngles;
		_v.y += 180.0f;
		_q.eulerAngles = _v;
		pe.gameObject.transform.rotation = _q;

		return pe;
	}	

	
	
	public Effect getCutSceneEffect(Transform parent = null, float scale = 1.0f, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f)
	{
		if(parent == null) parent = GameManager.me.deletePool;
	
		Effect eff = GameManager.me.effectManager.getCutSceneEffect();
		
		switch(type)
		{
		case ResourceType.PREFAB:
			eff.effect = GameManager.resourceManager.getInstantPrefabs(resource);	
			eff.effect.transform.parent = eff.tf;
			eff.effect.transform.localPosition = Vector3.zero;
			eff.particleEffect = null;
			break;
		case ResourceType.PARTICLE:

			if(scale > maxSizeRatio) scale = maxSizeRatio;

			ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(id, resource);
			pe.start(eff.tf.position, eff.tf, false, scale, posX, posY, posZ);
			//pe.transform.parent = parent;
			pe.transform.parent = eff.tf;
			eff.particleEffect = pe;
			eff.effect = pe.gameObject;
			eff.tf.parent = parent;
			
			break;			
		case ResourceType.BULLET:
			//GameManager.me.bulletManager.getBullet();
			break;
		}
		
		return eff;
	}	
	
}

