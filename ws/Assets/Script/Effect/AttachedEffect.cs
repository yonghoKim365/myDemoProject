using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class AttachedEffect 
{
	public const int TYPE_PREFAB = 0;	
	public const int TYPE_TK2D_ANI_SPRITE = 1;
	public const int TYPE_EFFECT = 2;
	public const int TYPE_INDY_EFFECT = 3; 
	
	public int type;
	
	public AttachedEffect ()
	{
	}

	public GameObject gameObject;
	public Transform transform;
	
	private Vector3 _v;
	private Quaternion _q;
	
	public AttachedEffectData data;
	
	public tk2dAnimatedSprite sprite;
	public ParticleEffect particleEffect = null;
	
	public static Dictionary<string, CollisionData> collisionData = new Dictionary<string, CollisionData>(StringComparer.Ordinal);
	
	public CollisionData nowCollisionData = null;
	
	public bool option = true;
	
	bool removeParent = false;

	Animation _ani = null;
	
	public void init(AttachedEffectData effectData, Transform parent, Vector3 position, float yAngle = 0.0f)
	{
		data = effectData;
		nowCollisionData = null;
		position += data.pos;
		removeParent = false;
		_ani = null;

		type = data.type;
		
		if(data.type == TYPE_PREFAB)
		{
			gameObject = GameManager.resourceManager.getPrefabFromPool(GameManager.info.effectData[data.id].resource);
			_ani = gameObject.animation;

			if(collisionData.TryGetValue(data.id, out nowCollisionData) == false)
			{
				BoxCollider collider = gameObject.GetComponentInChildren<BoxCollider>();
				if(collider == null) collider = gameObject.GetComponent<BoxCollider>();

				if(collider != null)
				{
#if UNITY_EDITOR
//					if(BattleSimulator.nowSimulation == false) Debug.Log("충돌박스 없음!!!!");					
#endif

					collider.enabled = false;
				}

				if(GameManager.info.effectData[data.id].hasCollider == false)
				{
					nowCollisionData = null;
				}
				else
				{
					nowCollisionData = new CollisionData(GameManager.info.effectData[data.id]);
					collisionData.Add(data.id, nowCollisionData);
				}

				
			}
			
			if(data.option == false)
			{
				removeParent = true;
				
				GameManager.resourceManager.setPrefabToPoolDelay(gameObject.animation.clip.length,data.id,gameObject);
			}
		}
		else if(data.type == TYPE_EFFECT)
		{
			particleEffect = GameManager.me.effectManager.getParticleEffect(data.id, GameManager.info.effectData[data.id].resource);	
			
			particleEffect.start(position, null, data.attachToParent);

			gameObject = particleEffect.gameObject;
			
			if(collisionData.TryGetValue(data.id, out nowCollisionData) == false)
			{
				if(particleEffect.boxCollder == null)
				{
					particleEffect.boxCollder = gameObject.GetComponentInChildren<BoxCollider>();	
					if(particleEffect.boxCollder != null) particleEffect.boxCollder.enabled = false;
				}
				
				if(GameManager.info.effectData[data.id].hasCollider == false)
				{
					nowCollisionData = null;
				}
				else
				{
					nowCollisionData = new CollisionData(GameManager.info.effectData[data.id]);
					collisionData.Add(data.id, nowCollisionData);
				}
			}
		}
		else if(data.type == TYPE_INDY_EFFECT)
		{
			particleEffect = GameManager.me.effectManager.getParticleEffect(data.id, GameManager.info.effectData[data.id].resource);
			gameObject = particleEffect.gameObject;
			particleEffect.start(position, null, data.attachToParent);
			particleEffect.setTimeLimit( data.timeLimit );

			if(collisionData.TryGetValue(data.id, out nowCollisionData) == false)
			{
				if(particleEffect.boxCollder == null)
				{
					particleEffect.boxCollder = gameObject.GetComponentInChildren<BoxCollider>();	
					if(particleEffect.boxCollder != null) particleEffect.boxCollder.enabled = false;
				}

				if(GameManager.info.effectData[data.id].hasCollider == false)
				{
					nowCollisionData = null;
				}
				else
				{
					nowCollisionData = new CollisionData(GameManager.info.effectData[data.id]);
					collisionData.Add(data.id, nowCollisionData);

				}
			}			
		}			
//		else if(data.type == TYPE_TK2D_ANI_SPRITE)
//		{
//			sprite = GameManager.me.effectManager.getAnimatedSprite();
//			
//			gameObject = sprite.gameObject;
//			transform = sprite.transform;
//			
//			gameObject.SetActive(true);
//			
//			sprite.Play(sprite.GetClipIdByName(data.id),0.0f);
//			
//			sprite.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
//		}
		
		transform = gameObject.transform;
		
		if(data.attachToParent)
		{
			if(removeParent)
			{
				position = parent.position;
				position += data.pos;
				transform.position = position;				
			}
			else
			{
				transform.parent = parent;
				position.x = 0;position.y = 0;position.z=0;
				position += data.pos;
				transform.localPosition = position;
			}
		}
		else
		{
			transform.parent = GameManager.me.mapManager.mapStage;
			position.z = data.pos.z;	
			transform.position = position;
		}
		
		_q = transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = yAngle;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		transform.localRotation = _q;
	}
	
	
	public void addParticle(string id, Transform parent, Vector3 position, bool useCharacterSize = false, Monster target = null)
	{
		type = TYPE_EFFECT;
		
		if(useCharacterSize)
		{
			particleEffect = GameManager.info.effectData[id].getParticleEffectByCharacterSize(target, parent, target.tf);
		}
		else
		{
			particleEffect = GameManager.me.effectManager.getParticleEffect(id, GameManager.info.effectData[id].resource);	
			particleEffect.start(position, parent, true);
		}

		if(particleEffect == null) return;

		gameObject = particleEffect.gameObject;
		transform = gameObject.transform;

		_q = transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		transform.localRotation = _q;	

		/*
		transform.parent = parent;
		position.x = 0;position.y = 0;position.z=0;
		transform.localPosition = position;
		

		*/
	}	
	
	
	
	
	private bool _isEnabled = false;
	
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;
			
			if(_isEnabled == false)
			{
				transform.parent = null;
				
				if(type == TYPE_PREFAB)
				{
					if(data.option)
					{
						if(data.attachToParent)
						{
							gameObject.SetActive(value);
						}
						
						GameManager.resourceManager.setPrefabToPool(GameManager.info.effectData[data.id].resource,gameObject);
					}
				}
				else if(type == TYPE_EFFECT)
				{
					if(particleEffect != null) GameManager.me.effectManager.setParticleEffect(particleEffect);
				}
				else if(type == TYPE_TK2D_ANI_SPRITE)
				{
					gameObject.SetActive(value);
					GameManager.me.effectManager.setAnimatedSprite(sprite);
				}

				gameObject = null;
				_ani = null;
				particleEffect = null;
				nowCollisionData = null;
			}
			else
			{
				gameObject.SetActive(value);
				if(_ani != null) _ani.Play();
			}
		}
		get
		{
			return _isEnabled;
		}
	}
}



public class CollisionData
{
	public Vector3 center;
	public Vector3 size;
	public bool wasLocalRotated = false;
	
	public CollisionData(EffectData ed)
	{
		center = ed.colliderCenter;
		size = ed.colliderSize;
		wasLocalRotated = ed.isRotated;
	}
}
