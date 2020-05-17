using System;
using UnityEngine;

sealed public class Pet : Monster
{
	public Pet ()
	{
	}

	Player hero = null;

	public Transform tfRidePoint;


	void OnDestroy()
	{
		shadow = null;
		smrNumber = 0;
		mrNumber = 0;
		smrs = null;
		mrs = null;

		hero = null;
		tfRidePoint = null;
	}

	public void setParts(HeroPartsData hpd, int rare)
	{
		string partsName = hpd.vehicleResource;

		foreach(SkinnedMeshRenderer smr in smrs)
		{
			if(smr.gameObject.name.Contains("parts"))
			{
				if(smr.name.Equals(partsName))
				{
					smr.gameObject.SetActive(true);
					CharacterUtil.setPartsTexture(smr,hpd);
				}
				else
				{
					smr.gameObject.SetActive(false);
				}
			}
		}
	}


	public bool removeHeroFromPet()
	{
		if(hero != null && hero.container.transform != null)
		{
			if(hero.playerData.characterId == Character.ID.CHLOE ||
			   hero.playerData.characterId == Character.ID.LEO ||
			   hero.playerData.characterId == Character.ID.KILEY )
			{
				hero.container.transform.parent = container.transform.parent;
				_v = hero.container.transform.localPosition;
				_v.y = 0;
				hero.container.transform.localPosition = _v;
				_q = hero.container.transform.localRotation;
				_v = _q.eulerAngles;
				_v.x = 0; _v.z = 0;
				_q.eulerAngles = _v;
				hero.container.transform.localRotation = _q;

				return true;
			}
		}

		return false;
	}


	public void init(Player parent = null)
	{
		hero = parent;
		tf = transform;
		setHitObject();
		resetAnimations();
		isHero = false;
		isPet = true;

		if(parent != null)
		{
			if(hero.pet == null)
			{
				hero.container.transform.localPosition = new Vector3(0,0,0);
			}


			setPositionCtransform(hero.cTransform.position);
			tf.rotation = hero.tf.rotation;

			hero.cTransform = cTransform;
			hero.tf = tf;
			hero.cTransformPosition = cTransformPosition;
			hero.container.gameObject.SetActive(true);

			setParent(  hero.container.gameObject.transform.parent );

			hero.container.transform.parent = tfRidePoint;
			//hero.container.transform.parent = tf;
			if(hero.shadow != null)
			{
				hero.shadow.gameObject.SetActive(false);
			}

			_q = tf.rotation;
			_v = _q.eulerAngles;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.y = (hero.isPlayerSide)?90.0f:270.0f;
			_v.z = 0.0f;
			_q.eulerAngles = _v;
			tf.rotation = _q;	

			if(hero.chargingGauge != null) hero.chargingGauge.pointer = cTransform;

			if(hero.miniMapPointer != null) hero.miniMapPointer.cha = cTransform;

			//hero.transform.rotation = _q;	
			hero.container.transform.localPosition = Vector3.zero;

			if(hero.playerData != null) setParts(hero.playerData.partsVehicle.parts, hero.playerData.partsVehicle.itemInfo.rare);

			hero.initAni(Monster.WIN, WrapMode.Once);
		}
		else
		{
			_currentDamageTime = -1.0f;
			isDamageFrame = false;
		}
		//		_v.y = (isPlayerSide)?90.0f:270.0f;

		initShadowAndEffectSize();
		isEnabled = true;
	}
	

	sealed public override void setHitObject()
	{
		if(hero == null)
		{
			_v = cTransform.localScale;
		}
		else
		{
			_v = hero.cTransform.localScale;
		}

		
		_boundCenter = normalCollider.center;
		_boundCenter.x *= _v.z;//_v.x;
		_boundCenter.y *= _v.y;
		_boundCenter.z *= _v.x;//_v.z;
		
		_boundExtens = normalCollider.size;
		_boundExtens.x *= 0.5f * _v.z;//_v.x;
		_boundExtens.y *= 0.5f * _v.y;
		_boundExtens.z *= 0.5f * _v.x;//_v.z;

		float t = _boundCenter.z;
		_boundCenter.z = _boundCenter.x;
		_boundCenter.x = t;

		t = _boundExtens.z;
		_boundExtens.z = _boundExtens.x;
		_boundExtens.x = t;

//		if(hero.isFlipX == false)
//		{
//			float tempF = _boundExtens.x;
//			_boundExtens.x = _boundExtens.z;
//			_boundExtens.z = tempF;
//			
//			tempF = _boundCenter.x;
//			_boundCenter.x = _boundCenter.z;
//			_boundCenter.z = tempF;			
//		}

		hitObject.init(_boundCenter, _boundExtens);		
		hitObject.setPosition(_v);
		
		bodyYCenter = hitObject.height * 0.55f;

		damageMotionValue = hitObject.width * _damageMotionValueRatio;
		damageMotionStep2Value = damageMotionValue * _damageMotionStep2ValueRatio;
		if(hero != null)
		{
			hero.damageMotionValue = damageMotionValue;
			hero.damageMotionStep2Value = damageMotionStep2Value;
		}
	}		


	public override void update ()
	{
		if(hero == null) 
		{
			_v = cTransformPosition;
		}
		else 
		{
			_v = hero.cTransformPosition;
		}

		hitObject.setPosition(_v);
		lineRight = hitObject.right;
		lineLeft = hitObject.x;

		if(hero != null)
		{
			hero.lineRight = lineRight;
			hero.lineLeft = lineLeft;
		}
		else if(action != null)
		{
			baseUpdate();
			action.doMotion();
		}
	}

	sealed public override void dead (bool useSound)
	{
		if(hero == null) base.dead();
	}

	sealed public override string state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			if(hero == null) playAni(_state);
		}
	}
	
}

