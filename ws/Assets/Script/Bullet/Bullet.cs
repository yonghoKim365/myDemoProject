using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
	public bool isCircleColliderType = false;

	public GameObject gobj;
	public Transform bTransform;

	public IVector3 bTransformPosition = new IVector3();

	protected IVector3 _v = new IVector3();
	protected Quaternion _q;
	protected Xbool _isEnabled = false;

	private static List<Monster> _chaByRangeDistance = new List<Monster>();

	public bool attachedToCharacter = false;

	public Xfloat speed = 300.0f;
	
	// 넉 백을 했을때????
	public Xfloat hitTimeOffset = 10.0f;
	
	protected IVector3 _boundExtens = new IVector3();
	protected IVector3 _boundCenter = new IVector3();

	public HitObject hitObject = new HitObject();

	//이 수치가 있으면 타격이 된 후 주변 해당 거리 안에 타겟이 있으면 방향을 튼다...
	public Xint retargetingRange = 0;
	public IFloat retargetingDelayDist = -1000;

	public int prevHp;
	public Xint hp;
	
//	private IFloat dx;
//	private IFloat dy;

	public Monster shooter;
	
	public int uniqueNo = 0;

	public int targetMonsterUniqueId = -1;

	public BulletData bulletData;
	
	public Xfloat timeSinceInit = 0.0f;
	public Xbool isTimeLimitBullet = false;
	public Xbool isDistanceLimitBullet = false;	
	public Xbool useRangeDamagePer = false;
	public Xbool invincible = false;

	private IVector3 _firstPosition = new IVector3();
	
	public Xbool isPlayerBullet = false;
	public Xbool isPlayerListBullet = false;
	
	public bool hasShadow = false;
	

	public AttackInfo attackerInfo = new AttackInfo();

	public Xbool isCollisionCheckAtEndOnly = false;
	
	public int motionType;
	
	public Xfloat limitDistance = 0.0f;
	public Xfloat limitTime = 0.0f;

	public void setTargetNum(int num, bool isRestoreType = false)
	{
		_restoreTargetNum = (isRestoreType)?num:-1;
		_targetNum = num;
		_maxTargetNum = _targetNum;
	}

	private Xint _restoreTargetNum = 0;
	private Xint _targetNum = 0;
	private Xint _maxTargetNum = 0;

	private Xfloat _nextRestoreTargetNumTime = 0.0f;

	public void checkTargetNum()
	{
		if(_restoreTargetNum > 0)
		{
			if(GameManager.me.stageManager.playTime >= _nextRestoreTargetNumTime)
			{
				_targetNum = _restoreTargetNum;
				_nextRestoreTargetNumTime = GameManager.me.stageManager.playTime + hitTimeOffset;
			}
		}
	}


	// 반지름이다!!!!!
	public Xfloat targetRange = 0.0f; 

	public Xfloat type5minDamagerPer = 1.0f;

	public Xfloat minimumDamagePer = 1.0f;
	public Xbool canCollide = true;
	public Xfloat damagePer = 1.0f;


	public BaseSkillData skillData;
	
	public Bullet secondBullet;
	
	public int damageCheckTypeNo = 1;
	
	public bool attachedDamageEffectToCharacter = false;
	
	public Xint totalDamageNum = 1;
	
	public Xfloat delay = 0.0f;
	
	public Xint applyReinforceLevel = 0;

	public Xint heroSkillLevel = 1;

	public int attackEffectType = 1;

	readonly IVector3 zeroVector = IVector3.zero;

	public Xbool isDurationSkill = false;

	// 7번 17번용 딜레이되어 떨어지는 총알 구분하기위해.
	private bool _isDelayEnableBullet = false;

	public IFloat getZ(){

		return _v.z;
	}


	public void init(int damageCheckType,int bulletMotionType = BulletMotionType.ANGLE, int totalDamageCount = 1, BaseSkillData sd = null, float startDelay = 0.0f)
	{
		targetMonsterUniqueId = -1;

		isCircleColliderType = false;
		_nextRestoreTargetNumTime = 0.0f;

		_isDelayEnableBullet = false;

		setTargetNum(1);

		isDeleteObject = false;

		_q.x = 0;
		_q.y = 0;
		_q.z = 0;
		_q.w = 0;
		_q.eulerAngles = zeroVector;

		bTransform.rotation = _q;

		//Log.log("r:",bTransform.rotation, "f:",bTransform.forward);

		attackEffectType = 1;
		
		delay = startDelay;
	
		applyReinforceLevel = RareType.D;

		heroSkillLevel = 1;
		
		totalDamageNum = totalDamageCount;
		
		damageCheckTypeNo = damageCheckType;
		
		switch(damageCheckType)
		{
		case 6:
		case 7:
		case 8:
		case 10:
		case 14:	
		case 16:	
			attachedDamageEffectToCharacter = true;
			break;
		default:
			attachedDamageEffectToCharacter = false;
			break;
		}

		damagePer = 1.0f;
		minimumDamagePer = 1.0f;

		skillData = sd;
		canCollide = true;
		limitTime.Set( 0.0f );
		limitDistance.Set( 0.0f );

		shooter = null;
		isTimeLimitBullet.Set( false );
		isDistanceLimitBullet.Set( false );
		bulletData = null;
		invincible.Set( false );
		timeSinceInit.Set( 0.0f );
		attachedToCharacter = (bulletMotionType == BulletMotionType.ATTACHED)?true:false;
		invincible.Set( false );
		targetRange.Set( 1.0f );
		hp.Set( 0 );
		motionType = bulletMotionType;
		useRangeDamagePer.Set( false );
		minimumDamagePer.Set( 1.0f );
		isCollisionCheckAtEndOnly.Set( false );

		type5minDamagerPer = 1.0f;

		retargetingRange.Set(0);

		// 스킬 타입이고 지속형인 것.
		isDurationSkill.Set( false );

		hitTimeOffset.Set( 10.0f );

		retargetingDelayDist = -1000;
		_lastHitMonId = -1000;
		_lastHitMonPosX = -1000;
		
		if(skillData == null)
		{
			switch(damageCheckType)
			{
			case 3: damageToCharacter = damageTargetOnlyType; 
				break;
			case 4: damageToCharacter = damageRangeWithTargetType; 
				break;
			case 5: damageToCharacter = damageTargetOnlyType5; 
				break;
			case 6: damageToCharacter = damageRangeType; 
				break;
			case 7: damageToCharacter = damageType7; 
				break;
			case 8: damageToCharacter = damageRangeType; 
				break;
			case 9: damageToCharacter = damageTargetOnlyType; 
				break;
			case 10: damageToCharacter = damageTargetOnlyType; 
				hitTimeOffset = 0.5f;
				break;
			case 11: damageToCharacter = damageTargetOnlyType; 
				hitTimeOffset = 0.5f;
				break;
			case 12: damageToCharacter = damageTargetOnlyType; 
				hitTimeOffset = 0.5f;
				break;
			case 14: damageToCharacter = damageRangeType; 
				break;
			case 16: damageToCharacter = damageDistanceType; 
				hitTimeOffset = 0.5f;
				break;
			case 18: damageToCharacter = damageTargetOnlyType; 
				break;
			case -1: damageToCharacter = noDamageType; 
				break;
			}

			attackerInfo.isSkillType = false;
		}
		else
		{
			switch(damageCheckType)
			{
			case 3: damageToCharacter = damageSkillTargetOnlyType; 
				break;
			case 4: damageToCharacter = damageSkillRangeWithTargetType; 
				break;
			case 5: damageToCharacter = damageSkillTargetOnlyType5; 
				break;
			case 6: damageToCharacter = damageSkillRangeType; 
				break;
			case 7: damageToCharacter = damageSkillType7; 
				break;
			case 8: damageToCharacter = damageSkillRangeType; 
				break;
			case 9: damageToCharacter = damageSkillTargetOnlyType; 
				break;
			case 10: damageToCharacter = damageSkillTargetOnlyType; 
				isDurationSkill = true;
				hitTimeOffset = 0.5f;
				break;
			case 11: damageToCharacter = damageSkillTargetOnlyType; 
				isDurationSkill = true;
				hitTimeOffset = 0.5f;
				break;
			case 12: damageToCharacter = damageSkillTargetOnlyType; 
				isDurationSkill = true;
				hitTimeOffset = 0.5f;
				break;
			case 14: damageToCharacter = damageSkillRangeType; 
				break;
			case 16: damageToCharacter = damageSkillDistanceType; 
				hitTimeOffset = 0.5f;
				break;
			case 18: damageToCharacter = damageSkillTargetOnlyType; 
				break;
			case -1: damageToCharacter = noDamageType; 
				break;
			}

			attackerInfo.isSkillType = true;
		}
	}

	
	
	public void init(bool playerBullet, BulletData data, float speed, IVector3 position, Monster attacker = null, float defaultYangle = 0.0f)
	{

		isSettingAngleToHitObject = false;
		
		gobj.SetActive( true );

		if(skillData != null)
		{
			// 원래는 내 총알임. 
			// 근데 우리편에게 쏴야함. 그럼 임시로 적 총알로 바꾸어준다.
			// 그래야지 우리편에게 충돌검사를 시킬 수 있으니까.
			if(playerBullet) 
			{
				// 우리 총알인데 우리편에게 쏘는거면 적 총알.
				// 우리 총알인데 적에게 쏘는거면 우리총알.

				if(skillData.targetType == Skill.TargetType.ME) isPlayerListBullet = false;
				else isPlayerListBullet = true;
			}
			else
			{
				// 적 총알인데 적에게 버프를 쏘는 거면 플레이어 총알.
				// 적 총알인데 플레이어에게 쏘는 거면 적 총알.

				if(skillData.targetType == Skill.TargetType.ME) isPlayerListBullet = true;
				else isPlayerListBullet = false;
			}
		}
		else
		{
			isPlayerListBullet = playerBullet;
		}

		this.isPlayerBullet = playerBullet;
		
		if(isPlayerListBullet) GameManager.me.bulletManager.playerBulletList.Add(this);
		else GameManager.me.bulletManager.monsterBulletList.Add(this);		
		
		shooter = attacker;
		
		bulletData = data;
		
		if(shooter != null)
		{
			attackerInfo.uniqueId = shooter.stat.uniqueId;
			attackEffectType = shooter.atkEffectType;
			MonsterStat.copyTo(ref shooter.stat, ref attackerInfo.stat);
			MonsterStat.copyTo(ref shooter.stat, ref attackerInfo.originalStat);
		}
		else
		{
			attackerInfo.uniqueId = -1;
			attackerInfo.stat.reset();
			attackerInfo.originalStat.reset();
			attackerInfo.stat.monsterType = Monster.TYPE.NONE;
			attackerInfo.originalStat.monsterType = Monster.TYPE.NONE;
		}

		attackerInfo.shooter = shooter;

		if(limitTime > 0.0f) isTimeLimitBullet = true;
		if(limitDistance > 0.0f) isDistanceLimitBullet = true;
		
		gobj.name = data.id;


		// 총알 사이즈 지정 안함 ! //
//		_v = bTransform.localScale;
//		
//		_v.x = bulletData.scale;
//		_v.y = bulletData.scale;
//		
//		bTransform.localScale = _v;			
		
		_speedX = 0;
		_speedY = 0;
		
		this.speed = speed;

		_firstPosition = position;

		setPositionAndTransform(position);

		_retargetingPosition = bTransformPosition;

		_boundCenter.x = 0;_boundCenter.y = 0; _boundCenter.z = 0;
		_boundExtens.x = 20;_boundExtens.y = 20; _boundExtens.z = 20;
		isSettingAngleToHitObject = false;
		setHitObject();

		if(bulletData.effectData != null)
		{
			int edLen = bulletData.effectData.Length;
			for(int i = 0; i < edLen; ++i)
			{
				AttachedEffect ae = GameManager.me.effectManager.getAttachedEffect();
				effects.Add(ae);
				
				_v.x = bTransformPosition.x;
				_v.y = bTransformPosition.y;
				_v.z = bTransformPosition.z;

				ae.init(bulletData.effectData[i], transform, _v, defaultYangle);

				ae.isEnabled = true;

				if(ae.nowCollisionData != null)
				{
					//isSettingAngleToHitObject = !aed.option;
					if(bulletData.ignoreHitObjectRotate ||
						(ae.nowCollisionData.wasLocalRotated == false
					 	&& ae.nowCollisionData.center.x + ae.nowCollisionData.center.y + ae.nowCollisionData.center.z == 0))
					{
						isSettingAngleToHitObject = true;
					}
					else isSettingAngleToHitObject = false;

					setHitObject(ae.nowCollisionData);
				}
			}
		}

		//Log.logError( "bullet setposition at init : " + position , playerBullet,  data,  speed,  position );

		if(attachedToCharacter && shooter != null)
		{
			bTransform.parent = shooter.tf;//
			shooter.attachedBullet.Add(this);
		}
		else bTransform.parent = GameManager.me.mapManager.mapStage;
	}


	public void setEffectLocalPosition(Monster mon)
	{
		if(bulletData.attachEffectToShotPoint)
		{
			_v = Monster.setShootingHand(mon);

			int len = effects.Count;
			for(int i = 0; i < len ; ++i)
			{
				if(effects[i].gameObject != null) effects[i].gameObject.transform.position = _v;
			}
		}
	}
	
	
	
	private float _speedX;
	private float _speedY;
	
	private IVector3 _targetPos;
	
	public void setPositionBullet(IVector3 pos)
	{
		_targetPos = pos;
		
		_speedX = (pos.x - bTransformPosition.x)/( limitTime );
		_speedY = (pos.y - bTransformPosition.y)/( limitTime );

		setAngle(pos);
	}
	
	
	private IVector3 _tempTargetPos;
	public void setTrajeCorty(IVector3 targetPos)
	{
		float t = limitTime;
	
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;
		
		m_vx = (targetPos.x-_v.x)/t;
		m_vy = -0.5f*(G*t) - (targetPos.y-_v.y)/t;
		m_vz = (targetPos.z-_v.z)/t;
		m_t = t;
		bTransform.LookAt(targetPos);
	}	



	int _lastHitMonId = -1000;
	IFloat _lastHitMonPosX = -1000;

	public void startRetargeting(Monster hitMon)
	{
		_lastHitMonId = hitMon.stat.uniqueId;
		retargetingDelayDist = targetRange * 0.5f;
		_lastHitMonPosX = hitMon.cTransformPosition.x;
	}

	// hitmon은 현재 이 총알은 맞은 놈. r은 여기에서 반경 몇 안을 체크할지.
	public void retargeting()
	{
		Monster cha;
		Monster targetCha = null;
		int len = 0;
		int r = retargetingRange.Get();

		if(isPlayerBullet)
		{
			len = GameManager.me.characterManager.monsters.Count;

			// 총알에서 가까운 녀석들부터 처리하기위해... 
			for(int i = 0; i < len ; ++i)
			{	
				cha = GameManager.me.characterManager.monsters[i];
				if(cha.stat.uniqueId == _lastHitMonId || cha.isEnabled == false || cha.cTransformPosition.x < _lastHitMonPosX) //|| cha.canCheckThisBullet(this, false) == false)
				{
					continue; 
				}
				cha.distanceFromHitPoint = MathUtil.abs( bTransformPosition.x, cha.cTransformPosition.x);

				if(cha.distanceFromHitPoint > r)
				{
					continue;
				}

				if(targetCha == null || targetCha.distanceFromHitPoint > cha.distanceFromHitPoint)
				{
					targetCha = cha;
				}
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;
			
			// 총알에서 가까운 녀석들부터 처리하기위해... 
			for(int i = 0; i < len ; ++i)
			{	
				cha = GameManager.me.characterManager.playerMonster[i];
				if(cha.stat.uniqueId == _lastHitMonId || cha.isEnabled == false || _lastHitMonPosX < cha.cTransformPosition.x)// || cha.canCheckThisBullet(this) == false)
				{
					continue;
				}
				cha.distanceFromHitPoint = MathUtil.abs( bTransformPosition.x, cha.cTransformPosition.x);

				if(cha.distanceFromHitPoint > r)
				{
					continue;
				}

				if(targetCha == null || targetCha.distanceFromHitPoint > cha.distanceFromHitPoint)
				{
					targetCha = cha;
				}
			}
			
		}
		
		if(targetCha != null) 
		{
			_v = targetCha.cTransformPosition;
			_v.y = bTransformPosition.y;

			setAngle(_v);

//			speed = speed * 1.2f;
		}

		targetCha = null;
		cha = null;

	}

	IVector3 _retargetingPosition = new IVector3();


//	float angleDY = 0.0f;
//	int angleY = 0;
	
	public void setAngle(IVector3 pos)
	{
//		dx = bTransformPosition.x - pos.x;
//		dy = bTransformPosition.z - pos.z;
//		int ang = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;	 // 180.0f / 3.14	
//		if(ang < 0) ang += 360;
//		else if(ang >= 360) ang %= 360;			
//		
//		angleDY = bTransformPosition.y - pos.y;
//		ang = (int)(Mathf.Atan2(angleDY, dx) * Mathf.Rad2Deg) + 180;	 // 180.0f / 3.14	
//		if(ang < 0) ang += 360;
//		else if(ang >= 360) ang %= 360;			
//		angleY = ang;

		//Log.logError(bulletData.id, bTransform.rotation, pos);
		//To do
		bTransform.LookAt(pos);

		//Log.logError("after 1",bulletData.id, "r:",bTransform.rotation,"tru:", bTransform.rotation.eulerAngles, bTransform.forward, pos, "transform pos: " + bTransformPosition);
	}
	
	public void setAngle(Monster target)
	{
//		dx = bTransformPosition.x - target.cTransformPosition.x;
//		dy = bTransformPosition.z - target.cTransformPosition.z;
//		int ang = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;	 // 180.0f / 3.14	
//		if(ang < 0) ang += 360;
//		else if(ang >= 360) ang %= 360;			

		bTransform.LookAt(target.cTransformPosition);
		//Log.logError("after 2",bulletData.id, bTransform.rotation, target.cTransformPosition);
	}
	
	
	public List<AttachedEffect> effects = new List<AttachedEffect>();
	
	public void clearEffect(bool checkEffectTimeLimit = false)
	{
		if(checkEffectTimeLimit)
		{
			for(int i = effects.Count - 1; i >= 0; --i)
			{
				if(effects[i].particleEffect != null && effects[i].particleEffect.timeLimit > 0)
				{
					effects[i].particleEffect = null;
				}
				
				GameManager.me.effectManager.setAttachedEffect(effects[i]);
			}
		}
		else
		{
			for(int i = effects.Count - 1; i >= 0; --i)
			{
				GameManager.me.effectManager.setAttachedEffect(effects[i]);
			}
		}

		effects.Clear();
		isSettingAngleToHitObject = false;
	}






	public void setReadyAttachedParticleEffect(bool isReady, bool setActive = false, bool isActive = true)
	{
		for(int i = effects.Count - 1; i >= 0; --i)
		{
			if(effects[i].particleEffect != null)
			{
				if(setActive)
				{
					if(isActive)
					{
						effects[i].particleEffect.gameObject.SetActive(true);
				effects[i].particleEffect.ready = isReady;
				effects[i].particleEffect.playParticle();
			}
					else
					{
						effects[i].particleEffect.ready = isReady;
						effects[i].particleEffect.gameObject.SetActive(false);
						//effects[i].particleEffect.playParticle();
					}
				}
				else
				{
					effects[i].particleEffect.ready = isReady;
					effects[i].particleEffect.playParticle();
				}
			}
		}
	}



	
	// -- 포물선용
	public const float G = 1980.0f;
	private IFloat m_vx = 0;
	private IFloat m_vy = 0;
	private IFloat m_vz = 0;
	private IFloat m_t = 0;
	// --
	
	private Xfloat _dist;

	private bool isSettingAngleToHitObject = false;
		
	private void setHitObject(CollisionData cd)
	{
		_boundCenter.x = cd.center.x;
		_boundCenter.y = cd.center.y;
		_boundCenter.z = cd.center.z;

		_boundExtens.x = cd.size.x * 0.5f;
		_boundExtens.y = cd.size.y * 0.5f;
		_boundExtens.z = cd.size.z * 0.5f;
		
		// 기본적으로 총알을 위쪽을 향해 보고 있다.
		// 그런데 오른쪽이나 왼쪽으로 눕게되면 가로와 세로 길이가 서로 바뀌게 된다.
		// 그래서 여기에서는 그 경우에 한해 가로/세로 길이를 교체해준다.
		// 또한 왼쪽으로 누웠을 경우에는 rect 계산시 x에서 width만큼 빼주어 위치 조정도 해야한다.
		
		if(isSettingAngleToHitObject == false) setAngleToHitObject();
		//setAngleToHitObject();

		hitObject.init(_boundCenter, _boundExtens);
	}	
	

	private void setHitObject(float zSize = 100.0f)
	{
		// 기본적으로 총알을 위쪽을 향해 보고 있다.
		// 그런데 오른쪽이나 왼쪽으로 눕게되면 가로와 세로 길이가 서로 바뀌게 된다.
		// 그래서 여기에서는 그 경우에 한해 가로/세로 길이를 교체해준다.
		// 또한 왼쪽으로 누웠을 경우에는 rect 계산시 x에서 width만큼 빼주어 위치 조정도 해야한다.
		_boundCenter.z *= zSize * 0.005f;
		_boundExtens.z *= zSize * 0.01f;
		if(isSettingAngleToHitObject == false) setAngleToHitObject();
		hitObject.init(_boundCenter, _boundExtens);
	}




	public void setHitObject(float zSize, float xSize = -1.0f)
	{
		_boundExtens.z = zSize * 0.5f;
		_boundExtens.y = 1000.0f;
		_boundCenter.z = 0.0f;

		if( Xfloat.greaterThan(  xSize , 0 ) )
		{
			_boundExtens.x = xSize * 0.5f;

			if(damageCheckTypeNo == 12)
			{
				if(isPlayerBullet)
				{
					_boundCenter.x = _boundExtens.x;
				}
				else
				{
					_boundCenter.x = -_boundExtens.x;
				}
			}
		}

		hitObject.init(_boundCenter, _boundExtens);
	}


	
	void setAngleToHitObject()
	{
		float tempF;

		if(isPlayerBullet)
		{
			tempF = _boundCenter.z;
			_boundCenter.z = _boundCenter.x;
			_boundCenter.x = tempF;
		}
		else
		{
			tempF = _boundCenter.z;
			_boundCenter.z = _boundCenter.x;
			_boundCenter.x = -tempF;
		}

		tempF = _boundExtens.z;
		_boundExtens.z = _boundExtens.x;
		_boundExtens.x = tempF;		

		isSettingAngleToHitObject = true;
	}


	
	public delegate void DamageChecker(Monster target);
	public DamageChecker damageToCharacter;
	
	void damageTargetOnlyType(Monster target)
	{
		if(_targetNum <= 0) return;

		if(target != null)
		{
			if(target.damage(this, target, damagePer, minimumDamagePer, true, (string.IsNullOrEmpty(bulletData.hitEffect)?GameManager.info.skillEffectSetupData[attackEffectType].effUp:bulletData.hitEffect), GameManager.info.skillEffectSetupData[attackEffectType].soundDown))
			{
				--_targetNum;
			}
		}
	}


	void damageTargetOnlyType5(Monster target)
	{
		if(_targetNum <= 0) return;
		
		if(target != null)
		{
			float tempDiscountDamageValue = 1.0f;

			if(type5minDamagerPer < 1.0f)
			{
				tempDiscountDamageValue = 1.0f - (_maxTargetNum - _targetNum) * ((1.0f - type5minDamagerPer)/(_maxTargetNum-1));
			}

			if(target.damage(this, 
			                 target, 
			                 damagePer, 
			                 minimumDamagePer, 
			                 true, 
			                 (string.IsNullOrEmpty(bulletData.hitEffect)?GameManager.info.skillEffectSetupData[attackEffectType].effUp:bulletData.hitEffect), 
			                 GameManager.info.skillEffectSetupData[attackEffectType].soundDown ,
			                 tempDiscountDamageValue
			                 ))
			{
				--_targetNum;
			}
		}
	}


	
	void damageRangeType(Monster target)
	{
		checkHitWithRange();
	}
	
	void damageRangeWithTargetType(Monster target)
	{
		checkHitWithRange(target);
	}
	
	void damageType7(Monster target)
	{
		_v.x = bTransformPosition.x;
		_v.y = 1.0f;
		_v.z = bTransformPosition.z;


		bool hasEffect = false;

		if(bulletData.effectData != null)
		{
			for(int i = bulletData.effectData.Length - 1; i >=0; --i)
			{
				if( GameManager.info.effectData.ContainsKey(  bulletData.effectData[i].id + "_HIT") )
				{
					GameManager.info.effectData[bulletData.effectData[i].id + "_HIT"].getEffect(attackerInfo.uniqueId, _v);//,null,null,targetRange*0.02f); 
					hasEffect = true;
				}
			}
		}

		if(hasEffect == false) GameManager.info.effectData["E_METEOR_HIT"].getEffect(attackerInfo.uniqueId, _v);//,null,null,targetRange*0.02f); 


		checkHitWithRange();		
		GameManager.me.effectManager.quakeEffect(0.3f,8.0f, EarthQuakeEffect.Type.Mad);

		if(bulletData.destroySound != null) SoundData.play(bulletData.destroySound);

	}		
	
	void damageDistanceType(Monster target)
	{
//		Debug.//LogError("CHECK THIS == DAMAGE TYPE 16");
//		
//		if(shooter.isPlayerMonster)
//		{
//			
//		}
//		else
//		{
//			
//		}
	}	


	void noDamageType(Monster target)
	{

	}


	
	public void checkSkillHitWithRange(Monster defaultTarget = null)
	{
		_chaByRangeDistance.Clear();

		if(_targetNum <= 0) return;

		damagePer = 1.0f;

		int i,len;
		
		if(defaultTarget != null && defaultTarget.isEnabled)
		{
			if(skillData.applySkillEffect(defaultTarget, heroSkillLevel, this, applyReinforceLevel))
			{
				EffectManager.showHitEfectWithCharacterSize(  bulletData, defaultTarget, attackerInfo.uniqueId);
			}
			--_targetNum;
		}

		Monster mon;

		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;


		if(skillData.targetType == Skill.TargetType.ENEMY) // 
		{
			if(isPlayerBullet) // 내가 쏜거면 적을 상대로 한다.
			{
				len = GameManager.me.characterManager.monsters.Count;
				
				// 총알에서 가까운 녀석들부터 처리하기위해... 이때는 가볍게 계산하기위해 x 좌표만 계산해준다...
				for(i = len -1; i >= 0 ; --i)
				{	
					mon = GameManager.me.characterManager.monsters[i];
					if(mon.isEnabled == false || (defaultTarget != null && defaultTarget == mon)) continue; //mon.skipHitCheck || 
					mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
					_chaByRangeDistance.Add(mon);
				}
			}
			else // 적이 쏜거면 나를 상대로 한다.
			{

				len = GameManager.me.characterManager.playerMonster.Count;
				
				for(i = len -1; i >= 0 ; --i)
				{
					mon = GameManager.me.characterManager.playerMonster[i];
					if(mon.isEnabled == false  || (defaultTarget != null && defaultTarget == mon)) continue; //|| mon.skipHitCheck
					mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
					_chaByRangeDistance.Add(mon);
				}
			}
		}
		else // 버프 스킬이고
		{
			if(isPlayerBullet) // 내가 쏜거면 우리편을 상대로 한다.
			{
				// 버프때 주인공은 미포함함.
				len = GameManager.me.characterManager.playerMonster.Count;
				
				for(i = len -1; i >= 0 ; --i)
				{
					mon = GameManager.me.characterManager.playerMonster[i];
					if( mon.isEnabled == false || (defaultTarget != null && defaultTarget == mon) || mon.isPlayer) continue; //mon.skipHitCheck ||
					mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
					_chaByRangeDistance.Add(mon);
				}
			}
			else // 적이 쏜거면 적을 상대로 한다.
			{
				len = GameManager.me.characterManager.monsters.Count;
				
				// 총알에서 가까운 녀석들부터 처리하기위해... 이때는 가볍게 계산하기위해 x 좌표만 계산해준다...
				for(i = len -1; i >= 0 ; --i)
				{	
					mon = GameManager.me.characterManager.monsters[i];
					if( mon.isEnabled == false || (defaultTarget != null && defaultTarget == mon) || mon.isPlayer) continue; // mon.skipHitCheck || 
					mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
					_chaByRangeDistance.Add(mon);
				}
			}
		}
		
		_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
		
		len = _chaByRangeDistance.Count;
		
		for(i = 0; i < len; ++i)
		{
			if(_targetNum <= 0) break;					
			damageSkillDetail(_chaByRangeDistance[i]);
		}
		
		_chaByRangeDistance.Clear();
	}
	
	
	private void damageSkillDetail(Monster mon)
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		_tempF = VectorUtil.DistanceXZ(_v, mon.cTransformPosition);

		if(Xfloat.lessEqualThan( _tempF , targetRange + mon.damageRange ))
		{
			if(useRangeDamagePer)
			{
				damagePer = 1.0f - (_tempF /  targetRange);
				if(damagePer < 0.0f) damagePer = 0.1f;
				else if(damagePer > 1.0f) damagePer = 1.0f;
			}

			if(skillData.applySkillEffect(mon, heroSkillLevel, this, applyReinforceLevel))
			{
				EffectManager.showHitEfectWithCharacterSize(  bulletData, mon, attackerInfo.uniqueId);
				--_targetNum;
			}
		}		
	}
	
	
	private void damageSkillDetailCharacter(Monster mon)
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		_tempF = VectorUtil.DistanceXZ(_v, mon.cTransformPosition);
		
		if(Xfloat.lessEqualThan( _tempF , targetRange + mon.damageRange))
		{
			if(useRangeDamagePer)
			{
				damagePer = 1.0f - (_tempF /  targetRange);// - ((VectorUtil.DistanceXZ(_v, mon.cTransformPosition)) / ( targetRange * 0.5f));
				if(damagePer < 0.0f) damagePer = 0.1f;
				else if(damagePer > 1.0f) damagePer = 1.0f;
			}

			if(skillData.applySkillEffect(mon, heroSkillLevel, this, applyReinforceLevel))
			{
				EffectManager.showHitEfectWithCharacterSize(  bulletData, mon, attackerInfo.uniqueId);
				--_targetNum;
			}
		}		
	}	
		
	
	
	
	void damageSkillTargetOnlyType(Monster target)
	{
		//Log.log("damageSkillTargetOnlyType : " + target);
		if(_targetNum <= 0) return;

		if(target != null && target.isEnabled && skillData != null)
		{
			//Log.log("damageSkillTargetOnlyType : target.isEnabled" + target.isEnabled);
			if(skillData.applySkillEffect(target, heroSkillLevel, this, applyReinforceLevel))
			{
				EffectManager.showHitEfectWithCharacterSize(  bulletData, target, attackerInfo.uniqueId);
				--_targetNum;
			}
		}
	}


	void damageSkillTargetOnlyType5(Monster target)
	{
		//Log.log("damageSkillTargetOnlyType : " + target);
		if(_targetNum <= 0) return;
		
		if(target != null && target.isEnabled && skillData != null)
		{
			//Log.log("damageSkillTargetOnlyType : target.isEnabled" + target.isEnabled);

			// 최소데미지비율....

			IFloat dp = 1.0f;

			if(type5minDamagerPer < 1.0f)
			{
				dp = 1.0f - (_maxTargetNum - _targetNum) * ((1.0f - type5minDamagerPer)/(_maxTargetNum-1));
			}

			if(skillData.applySkillEffect(target, heroSkillLevel, this, applyReinforceLevel, null, dp ))
			{
				EffectManager.showHitEfectWithCharacterSize(  bulletData, target, attackerInfo.uniqueId);
				--_targetNum;
			}
		}
	}


	
	void damageSkillRangeType(Monster target)
	{
		checkSkillHitWithRange();		
	}
	
	void damageSkillRangeWithTargetType(Monster target)
	{
		if(target != null) checkSkillHitWithRange(target);
	}
	
	void damageSkillType7(Monster target)
	{
		_v.x = bTransformPosition.x;
		_v.y = 1.0f;
		_v.z = bTransformPosition.z;

		bool hasEffect = false;
		
		if(bulletData.effectData != null)
		{
			for(int i = bulletData.effectData.Length - 1; i >=0; --i)
			{
				if( GameManager.info.effectData.ContainsKey(  bulletData.effectData[i].id + "_HIT") )
				{
					GameManager.info.effectData[bulletData.effectData[i].id + "_HIT"].getEffect(attackerInfo.uniqueId,_v);//,null,null,targetRange*0.02f); 
					hasEffect = true;
					break;
				}
			}
		}
		
		if(hasEffect == false) GameManager.info.effectData["E_METEOR_HIT"].getEffect(attackerInfo.uniqueId,_v);//,null,null,targetRange*0.02f); 


		if(bulletData.destroySound != null) SoundData.play(bulletData.destroySound);

		GameManager.me.effectManager.quakeEffect(0.3f,8.0f, EarthQuakeEffect.Type.Mad);
		checkSkillHitWithRange();		
	}		
	
	void damageSkillDistanceType(Monster target)
	{
		if(_targetNum <= 0) return;

		int len = 0;

		if(skillData.targetType == Skill.TargetType.ENEMY) // 공격 스킬이냐..
		{
			if(isPlayerBullet)
			{
				len = GameManager.me.characterManager.monsters.Count;

				for(int i = 0; i < len ; ++i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false) continue; 
					if(Xfloat.lessThan(  VectorUtil.Distance(shooter.cTransformPosition.x, GameManager.me.characterManager.monsters[i].cTransformPosition.x).AsFloat() , targetRange * 2.0f))
					{
						if(skillData.applySkillEffect(GameManager.me.characterManager.monsters[i], heroSkillLevel, this, applyReinforceLevel))
						{
							EffectManager.showHitEfectWithCharacterSize(  bulletData, GameManager.me.characterManager.monsters[i], attackerInfo.uniqueId);
							--_targetNum;
						}
					}

					if(_targetNum <= 0) return;
				}
			}
			else
			{
				len = GameManager.me.characterManager.playerMonster.Count;
				for(int i = 0; i < len ; ++i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false ) continue; 				 //|| mon.skipHitCheck
					if(Xfloat.lessThan(  VectorUtil.Distance(shooter.cTransformPosition.x, GameManager.me.characterManager.playerMonster[i].cTransformPosition.x).AsFloat() , targetRange * 2.0f ))
					{
						if(skillData.applySkillEffect(GameManager.me.characterManager.playerMonster[i], heroSkillLevel, this, applyReinforceLevel))
						{
							EffectManager.showHitEfectWithCharacterSize(  bulletData, GameManager.me.characterManager.playerMonster[i], attackerInfo.uniqueId);
							--_targetNum;
						}
					}

					if(_targetNum <= 0) return;
				}
			}			
			
		}
		else // 버프 스킬이냐..
		{
			if(isPlayerBullet)
			{
				len = GameManager.me.characterManager.playerMonster.Count;
				for(int i = 0; i < len ; ++i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false || GameManager.me.characterManager.playerMonster[i].isPlayer) continue;  // || mon.skipHitCheck
					if(shooter != GameManager.me.characterManager.playerMonster[i] && Xfloat.lessThan(  VectorUtil.Distance(shooter.cTransformPosition.x, GameManager.me.characterManager.playerMonster[i].cTransformPosition.x).AsFloat() , targetRange * 2.0f ))
					{
						if(skillData.applySkillEffect(GameManager.me.characterManager.playerMonster[i], heroSkillLevel, this, applyReinforceLevel))
						{
							EffectManager.showHitEfectWithCharacterSize(  bulletData, GameManager.me.characterManager.playerMonster[i], attackerInfo.uniqueId);
							--_targetNum;
						}
					}

					if(_targetNum <= 0) return;
				}
			}
			else
			{
				len = GameManager.me.characterManager.monsters.Count;
				for(int i = 0; i < len ; ++i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false || GameManager.me.characterManager.monsters[i].isPlayer) continue;  //|| mon.skipHitCheck
					if(Xfloat.lessThan(  VectorUtil.Distance(shooter.cTransformPosition.x, GameManager.me.characterManager.monsters[i].cTransformPosition.x).AsFloat() , targetRange * 2.0f ))
					{
						if(skillData.applySkillEffect(GameManager.me.characterManager.monsters[i], heroSkillLevel, this, applyReinforceLevel))
						{
							EffectManager.showHitEfectWithCharacterSize(  bulletData, GameManager.me.characterManager.monsters[i], attackerInfo.uniqueId);
							--_targetNum;
						}
					}

					if(_targetNum <= 0) return;
				}
			}				
		}
	}		
	
	//===========================

	
	public void destroy(bool checkEffectTimeLimit = false)
	{
		_isEnabled.Set( false );
		
		if(damageCheckTypeNo == 12 && shooter != null)
		{
			shooter.onCompleteLoopSkillAni();
		}

		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		if(secondBullet != null)
		{
			_v.y = 0.0f;
			secondBullet.setPositionAndTransform(_v);
			secondBullet.isEnabled = true;
			secondBullet.setReadyAttachedParticleEffect(true);
			secondBullet = null;
		}

		switch(damageCheckTypeNo)
		{
		case 6: case 7: case 8: case 10: case 11: case 17:
			_v.y = 0.0f;
			break;
		}


#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false || BattleSimulator.instance.skipTime <= 0.0f)
#endif
		{
			if(bulletData.useDestroyEffect)
			{
				GameObject go;

				if(bulletData.destroyEffectOption == BulletData.DestroyEffectOptionType.UseBulletRotation)
				{
					go = GameManager.info.effectData[bulletData.destroyEffectId].getEffect(attackerInfo.uniqueId,_v);
					if(go != null)
					{
						go.transform.rotation = bTransform.rotation;
					}
				}
				else 
				{
					go = GameManager.info.effectData[bulletData.destroyEffectId].getEffect(attackerInfo.uniqueId,_v);

					if(go != null)
					{
						_v.x = 0; _v.y = 0; _v.z = 0;
						_q.eulerAngles = _v;
						go.transform.rotation = _q;
					}
				}
				
			}
		}


		setDelete(checkEffectTimeLimit);

	}


	public void setDelete(bool checkEffectTimeLimit = false)
	{
		isEnabled = false;
		isDeleteObject = true;

		if(attachedToCharacter && shooter != null)
		{
			bTransform.parent = shooter.tf;//
			shooter.attachedBullet.Remove(this);
		}

		clearEffect(checkEffectTimeLimit);
		bTransform.parent = GameManager.me.mapManager.mapStage;
	}


	
	public void update()
	{
		if(_isEnabled == false) return;
		if(delay > 0)
		{
			delay -= GameManager.globalDeltaTime;
			_isDelayEnableBullet = true;
			return;
		}
		
		timeSinceInit += GameManager.globalDeltaTime;// - delay;
		
		updateMotion();
	}
	
	

	
	public void checkHitWithRange(Monster defaultTarget = null)
	{
		if(_targetNum <= 0) return;

		Monster mon;
		damagePer = 1.0f;

		int i, len;

		_v.x = bTransformPosition.x;
		_v.y = 1.0f;
		_v.z = bTransformPosition.z;

		if(defaultTarget != null)
		{
			defaultTarget.damage(this, defaultTarget, damagePer, minimumDamagePer, true, (string.IsNullOrEmpty(bulletData.hitEffect)?GameManager.info.skillEffectSetupData[attackEffectType].effUp:bulletData.hitEffect), GameManager.info.skillEffectSetupData[attackEffectType].soundDown);
			--_targetNum;
		}
		
		if(isPlayerBullet)
		{
			len = GameManager.me.characterManager.monsters.Count;
			
			// 총알에서 가까운 녀석들부터 처리하기위해... 이때는 가볍게 계산하기위해 x 좌표만 계산해준다...
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.monsters[i];
				if(mon.isEnabled == false  || (defaultTarget != null && defaultTarget == mon)) continue;  //|| mon.skipHitCheck
				mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
				_chaByRangeDistance.Add(mon);
			}
			
			_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
			
			len = _chaByRangeDistance.Count;
			
			for(i = 0 ; i < len; ++i)
			{
				if(_targetNum <= 0) break;	
				damageRangeDetail(_chaByRangeDistance[i]);
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;

			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.playerMonster[i];
				if(mon.isEnabled == false || (defaultTarget != null && defaultTarget == mon)) continue;  // || mon.skipHitCheck
				mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
				_chaByRangeDistance.Add(mon);
			}
			
			_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
			
			len = _chaByRangeDistance.Count;
			
			for(i = 0; i < len; ++i)
			{
				if(_targetNum <= 0) break;	
				damageRangeDetail(_chaByRangeDistance[i]);
			}			
		}
		
		_chaByRangeDistance.Clear();
	}


	private Xfloat _tempF;
	private void damageRangeDetail(Monster mon)
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		_tempF = VectorUtil.DistanceXZ(_v, mon.cTransformPosition);
		if( Xfloat.lessEqualThan( _tempF , targetRange + mon.damageRange ))
		{
			if(useRangeDamagePer)
			{
				damagePer = 1.0f - (_tempF /  targetRange);
				if(damagePer < 0.0f) damagePer = 0.1f;
				else if(damagePer > 1.0f) damagePer = 1.0f;
			}

			mon.damage(this, mon, damagePer, minimumDamagePer, true, (string.IsNullOrEmpty(bulletData.hitEffect)?GameManager.info.skillEffectSetupData[attackEffectType].effUp:bulletData.hitEffect), GameManager.info.skillEffectSetupData[attackEffectType].soundDown);
			
			//GameManager.soundManager.playEffect("sounds/critical");						
			--_targetNum;
		}
	}


	IFloat G_X = 1800.0f;
	IFloat G_Y = 1980.0f;//1980.0f;
	IFloat G_Z = 1450.0f;//1980.0f;
	IFloat backwardTime = 0.0f;

	IVector3 btTargetPos;
	bool p2 = false;
	IFloat m_vx2;
	IFloat bt2XTime = 0.0f;
	IFloat bt2StartX = 0.0f;
	IFloat bt2ProgressTime = 0.0f;
	IFloat bt2Zdirection = 1.0f;

	public void setBackwardTrajeCorty(IVector3 targetPos, float GY = 1980.0f, bool bottomSide = true)//, float backwardLimitTime = 0.5f)
	{
//		Debug.LogError("start! "+bTransformPosition);

		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		if(isPlayerBullet)
		{
			targetPos.x -= 30.0f;
			if(Xfloat.greaterThan(  (targetPos.x - _v.x).AsFloat() , limitDistance * 1.1f  )  ) targetPos.x = _v.x + limitDistance * 1.1f;
		}
		else
		{
			targetPos.x += 30.0f;
			if(Xfloat.greaterThan( (_v.x - targetPos.x).AsFloat() , limitDistance * 1.1f )) targetPos.x = _v.x - limitDistance * 1.1f;
		}

		limitTime.Set( limitTime  * (MathUtil.abs(_v.x, targetPos.x) / 900.0f) );

		if(limitTime < 1.0f) limitTime.Set(  1.0f );
		else if(limitTime > 2.0f) limitTime.Set(  2.0f );

		backwardTime = limitTime * 0.4f;

		if(backwardTime < 0.4f)
		{
			limitTime += (0.4f - backwardTime) * 2.0f;
			backwardTime = 0.4f;
		}

		float t = limitTime;

		bt2XTime = limitTime - backwardTime;

		p2 = false;

		G_Y = GY;
		//G_Y = 500 + GameManager.inGameRandom.Range(0,10) * (1980-500) * 0.1f;

		m_vx = -0.5f*(G_X*backwardTime);

		btTargetPos.x = targetPos.x;
		btTargetPos.y = targetPos.y;
		btTargetPos.z = targetPos.z;

		m_vx2 = (targetPos.x-_v.x)/(t-backwardTime);

		m_vy = -0.5f*(G_Y*t) - (targetPos.y-_v.y)/t;

		G_Z = Mathf.Abs(G_Z);
		if(bottomSide) G_Z = -G_Z;

		m_vz = -0.5f*(G_Z*t) - (targetPos.z-_v.z)/t;

		m_t = t;
		bTransform.LookAt(targetPos);
	}

	void updateBackwardTrajecotry ()
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;

		if(Xfloat.lessEqualThan( limitTime - m_t , backwardTime ))
		{
			if(isPlayerBullet)
			{
				_v.x += (m_vx * GameManager.globalDeltaTime);
			}
			else
			{
				_v.x -= (m_vx * GameManager.globalDeltaTime);
			}

			m_vx += G_X * GameManager.globalDeltaTime;
			bt2ProgressTime = 0.0f;
		}
		else
		{
			if(p2 == false)
			{
				p2 = true;
				bt2StartX = _v.x;
			}

			bt2ProgressTime += GameManager.globalDeltaTime;

			float nx = _v.x + m_vx2 * GameManager.globalDeltaTime;
			_v.x = Mathf.Lerp(ITweenEasing.easeInQuad(bt2StartX,btTargetPos.x, bt2ProgressTime/bt2XTime),nx,0.8f);
		}

		_v.y -= m_vy  * GameManager.globalDeltaTime;
		m_vy += G_Y  * GameManager.globalDeltaTime;

		_v.z -= m_vz * GameManager.globalDeltaTime;// * bt2Zdirection;
		m_vz += G_Z  * GameManager.globalDeltaTime;
		
		bTransform.LookAt(_v);
		
		setPosition(_v);
		m_t-= GameManager.globalDeltaTime;

		if(isDistanceLimitBullet)
		{
			_dist.Set( VectorUtil.Distance3D(bTransformPosition, _firstPosition) );
			if(_dist >= limitDistance)
			{
				bulletData.hitSecondActionStart(bTransformPosition, null, this);
				destroy();
			}
		}
	}	
	









	public void setChaser(IVector3 targetPos, float GY = 1980.0f, bool bottomSide = true)//, float backwardLimitTime = 0.5f)
	{
		_v.x = bTransformPosition.x;//.localPosition;
		_v.y = bTransformPosition.y;//.localPosition;
		_v.z = bTransformPosition.z;//.localPosition;

		targetPos.x = _v.x;
		targetPos.y = _v.y;
		targetPos.z = _v.z;

		if(isPlayerBullet)
		{
			targetPos.x -= 50.0f;
		}
		else
		{
			targetPos.x += 50.0f;
		}

		targetPos.y += GameManager.inGameRandom.Range(0,50);

		G_Z = Mathf.Abs(G_Z);
		if(bottomSide)
		{
			G_Z = -G_Z;
			targetPos.z -= 40.0f + GameManager.inGameRandom.Range(0,80);
		}
		else
		{
			targetPos.z += 40.0f + GameManager.inGameRandom.Range(0,80);
		}

		limitTime.Set( VectorUtil.DistanceXZ(_v, targetPos) / speed );

		if(limitTime < 0.5f) limitTime = 0.5f;
		else if(limitTime > 1.0f) limitTime = 1.0f;
		
		float t = limitTime;
		
		G_Y = GY;
		
		m_vx = -0.5f*(G_X*t);// - (targetPos.x-_v.x)/t;
		m_vy = -0.5f*(G_Y*t) - (targetPos.y-_v.y)/t;
		m_vz = (targetPos.z-_v.z)/t;//-0.5f*(G_Z*t) - (targetPos.z-_v.z)/t;
		
		backwardTime = t;
		bTransform.LookAt(targetPos);





		/*
		limitTime = VectorUtil.DistanceXZ(_v, targetPos) / speed;

		if(isPlayerBullet)
		{
			targetPos.x -= 30.0f;
			if(targetPos.x - _v.x > limitDistance * 1.1f) targetPos.x = _v.x + limitDistance * 1.1f;
		}
		else
		{
			targetPos.x += 30.0f;
			if(_v.x - targetPos.x > limitDistance * 1.1f) targetPos.x = _v.x - limitDistance * 1.1f;
		}
		
		limitTime *= MathUtil.abs(_v.x, targetPos.x) / 900.0f;
		
		if(limitTime < 1.0f) limitTime = 1.0f;
		else if(limitTime > 2.0f) limitTime = 2.0f;
		
		backwardTime = limitTime * 0.4f;
		
		if(backwardTime < 0.4f)
		{
			limitTime += (0.4f - backwardTime) * 2.0f;
			backwardTime = 0.4f;
		}

		float t = limitTime;
		
		G_Y = GY;

		m_vx = -0.5f*(G_X*backwardTime);
		m_vy = -0.5f*(G_Y*t) - (targetPos.y-_v.y)/t;
		
		G_Z = Mathf.Abs(G_Z);
		if(bottomSide) G_Z = -G_Z;
		
		m_vz = -0.5f*(G_Z*t) - (targetPos.z-_v.z)/t;
		
		m_t = t;
		bTransform.LookAt(targetPos);
		*/
	}
	
	void updateChaser ()
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;
		
		if(backwardTime > 0)
		{
			if(isPlayerBullet) _v.x += (m_vx * GameManager.globalDeltaTime);
			else _v.x -= (m_vx * GameManager.globalDeltaTime);

			m_vx += G_X * GameManager.globalDeltaTime;

			_v.y -= m_vy  * GameManager.globalDeltaTime;
			m_vy += G_Y  * GameManager.globalDeltaTime;

			_v.z -= m_vz * GameManager.globalDeltaTime;
			m_vz += G_Z  * GameManager.globalDeltaTime;

			_q = Util.getLookRotationQuaternion(_v - bTransformPosition);

			bTransform.rotation = Util.getFixedQuaternionSlerp(bTransform.rotation,_q, 0.5f);

			setPosition(_v);
			backwardTime-= GameManager.globalDeltaTime;

			m_t = 0;

		}
		else
		{
			Monster t;
			
			if(targetMonsterUniqueId >= 0)
			{
				t = GameManager.me.characterManager.getMonsterByUniqueId(!isPlayerListBullet, targetMonsterUniqueId, attackerInfo.stat.uniqueId);
				if(t == null) t = GameManager.me.characterManager.getCloseTargetByPosition(isPlayerListBullet, bTransformPosition, attackerInfo.stat.uniqueId);
			}
			else
			{
				t = GameManager.me.characterManager.getCloseTargetByPosition(isPlayerListBullet, bTransformPosition, attackerInfo.stat.uniqueId);
				
			}
			
			if(t != null)
			{
				targetMonsterUniqueId = t.stat.uniqueId;
				IVector3 cv = t.cTransformPosition;
				cv.y = t.hitObject.height * 0.5f;

				_q = Util.getLookRotationQuaternion(cv - bTransformPosition);

				float qValue = m_t - 0.3f;
				if(qValue > 0) qValue = 0;

				bTransform.rotation = Util.getFixedQuaternionSlerp(bTransform.rotation,_q, 0.6f + qValue);//CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
			}
			
			_v += ( (IVector3)(bTransform.forward) * speed * GameManager.globalDeltaTime);
			setPosition(_v);

			m_t += GameManager.globalDeltaTime;
		}
		
		
		if(isDistanceLimitBullet)
		{
			_dist.Set( VectorUtil.Distance3D(bTransformPosition, _firstPosition) );
			if(_dist >= limitDistance)
			{
				bulletData.hitSecondActionStart(bTransformPosition, null, this);
				destroy();
			}
		}
	}	








	private void updateMotion()
	{
		//Log.log("bullet update motion : ", motionType, bulletData.id, damageCheckTypeNo); 


		if(motionType == BulletMotionType.TRAJECOTRY)//|| motionType == BulletMotionType.TRAJECOTRY_RANDOM)
		{
			if(m_t >= 0.0f)
			{
				_v.x = bTransformPosition.x;//.localPosition;
				_v.y = bTransformPosition.y;//.localPosition;
				_v.z = bTransformPosition.z;//.localPosition;

				_v.x += m_vx * GameManager.globalDeltaTime;
				_v.y -= m_vy  * GameManager.globalDeltaTime;
				_v.z += m_vz * GameManager.globalDeltaTime;
				m_vy += G  * GameManager.globalDeltaTime;
				
				bTransform.LookAt(_v);
				
				setPosition(_v);
				m_t-= GameManager.globalDeltaTime;
			}
			else
			{
				//if(isPlayerBullet) bulletData.hitSecondActionStart(bTransformPosition);
				
				bulletData.hitSecondActionStart(bTransformPosition, null, this);
				
				if(damageToCharacter != null) damageToCharacter(null);
				else checkHitWithRange();
				
				destroy();
			}
			return;
		}
		else if(motionType == BulletMotionType.BACKWARD_TRAJECOTRY)//|| motionType == BulletMotionType.TRAJECOTRY_RANDOM)
		{
			updateBackwardTrajecotry();
			return;
		}
		else if(motionType == BulletMotionType.CHASER)
		{
			updateChaser();
			return;
		}
		else if(isTimeLimitBullet && timeSinceInit >= limitTime)
		{
			bulletData.hitSecondActionStart(bTransformPosition, null, this);

			if(isCollisionCheckAtEndOnly && canCollide)
			{
				if(damageToCharacter != null) damageToCharacter(null);
				else checkHitWithRange();
				// 16번의 경우 무조건 스킬 타격이라고 생각한다.
			}

			destroy();
			return;
		}
		else if(isDistanceLimitBullet)
		{
			_dist = VectorUtil.Distance3D(bTransformPosition, _firstPosition);
			if(_dist >= limitDistance)
			{
				bulletData.hitSecondActionStart(bTransformPosition, null, this);
				destroy();
				return;
			}
		}

		if( motionType == BulletMotionType.ANGLE)
		{
			//Log.logError("motionType == BulletMotionType.ANGLE : " + bulletData.id, _v, bTransform.rotation, bTransform.forward, speed);

			_v.x = bTransformPosition.x;//.localPosition;
			_v.y = bTransformPosition.y;//.localPosition;
			_v.z = bTransformPosition.z;//.localPosition;

			_v += ((IVector3)(bTransform.forward) * speed * GameManager.globalDeltaTime);
			setPosition(_v);

			if(retargetingDelayDist > 0)
			{
				retargetingDelayDist -= speed * GameManager.globalDeltaTime;
			}
			else if(retargetingDelayDist > -500)
			{
				retargetingDelayDist = -1000;
				retargeting();

			}
		}		
		else if( motionType == BulletMotionType.POSITION) // else 였음...
		{
			/// 7번 17번에 한해 떨어지는 타이밍에 이펙트를 다시 실행 시킨다. 
			if(_isDelayEnableBullet)
			{
				_isDelayEnableBullet = false;
				setReadyAttachedParticleEffect(true, true, true);
			}

			_v.x = bTransformPosition.x;//.localPosition;
			_v.y = bTransformPosition.y;//.localPosition;
			_v.z = bTransformPosition.z;//.localPosition;

			_v.x += _speedX * GameManager.globalDeltaTime;
			_v.y += _speedY * GameManager.globalDeltaTime;
			setPosition(_v);
			//bTransform.localPosition = _v;
		}


		//if(damageCheckTypeNo != 12) //Log.log("bullet update pos : " + bTransformPosition + "   sx: " + _speedX + " sy:" + _speedY + "  sp:" + speed + "  dt:" + GameManager.globalDeltaTime);
	}
	
	IVector3 prevTransformPosition = IVector3.zero;

	public void setPosition(IVector3 pos)
	{
//		pos.x = Mathf.Round(pos.x * 100.0f) * 0.01f;
//		pos.y = Mathf.Round(pos.y * 100.0f) * 0.01f;
//		pos.z = Mathf.Round(pos.z * 100.0f) * 0.01f;

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame)
		{
			setPositionAndTransform(pos);
			return;
		}

		if(GameManager.loopIndex == 0) prevTransformPosition = bTransformPosition;
		//Log.logError("bullet : " + bulletData.id + "   " + pos);
		bTransformPosition.x = pos.x;
		bTransformPosition.y = pos.y;
		bTransformPosition.z = pos.z;

		_needPositionRender = true;
	}

	public void setPositionAndTransform(IVector3 pos)
	{
//		pos.x = Mathf.Round(pos.x * 100.0f) * 0.01f;
//		pos.y = Mathf.Round(pos.y * 100.0f) * 0.01f;
//		pos.z = Mathf.Round(pos.z * 100.0f) * 0.01f;

		prevTransformPosition.x = pos.x;
		prevTransformPosition.y = pos.y;
		prevTransformPosition.z = pos.z;

		bTransformPosition.x = pos.x;
		bTransformPosition.y = pos.y;
		bTransformPosition.z = pos.z;

		bTransform.position = pos;
		_needPositionRender = false;
	}

	bool _needPositionRender = false;

	void LateUpdate()
	{
		if(attachedToCharacter == false)
		{
			if(GameManager.renderSkipFrame == false) _doRenderSkipFrame = _needPositionRender;

			if(_needPositionRender)
			{
				bTransform.position = bTransformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			}
			else if(GameManager.renderSkipFrame && _doRenderSkipFrame)
			{
				bTransform.position = bTransformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			}
			_needPositionRender = false;
		}
	}

	bool _doRenderSkipFrame = false;
	private IVector3 _v2;
	
	
	public bool isDeleteObject = false;
	
	void OnEnable ()
	{
		_doRenderSkipFrame = false;
	}

	void OnDisable ()
	{
		_doRenderSkipFrame = false;
	}
	
	public bool isEnabled
	{
		set
		{
			if(value == false)
			{
				gobj.SetActive( false );
			}
			else
			{
				gobj.SetActive( bulletData.visible);

				if(bulletData.startSound != null) SoundData.play(bulletData.startSound);

			}
			
			_isEnabled.Set( value );
		}
		get
		{
			return _isEnabled;	
		}
	}
	
	
	
	
	public HitObject getHitObject()
	{
		if(attachedToCharacter && shooter != null)
		{
			_v.x = shooter.cTransformPosition.x;
			_v.y = shooter.cTransformPosition.y;
			_v.z = shooter.cTransformPosition.z;
		}
		else
		{
			_v.x = bTransformPosition.x;
			_v.y = bTransformPosition.y;
			_v.z = bTransformPosition.z;
		}

		hitObject.x = _v.x + _boundCenter.x - _boundExtens.x;// + ((angle == 180 || angle == 0)?-hitObject.width:0);
		hitObject.y = _v.y + _boundCenter.y - _boundExtens.y;
		hitObject.z = _v.z + _boundCenter.z - _boundExtens.z;
		
		return hitObject;
	}		

	

	public bool isOutSide()
	{
		_v.x = bTransformPosition.x;
		_v.y = bTransformPosition.y;
		_v.z = bTransformPosition.z;
		
		if(_v.x > StageManager.mapEndPosX + 1000 || 
		   _v.x < StageManager.mapStartPosX - 1000 || 
		   _v.y > 2000 || 
		   _v.y < -200 || 
		   _v.z > 1000 || 
		   _v.z < -1000)
		{
			return true;
		}

		return false;
	}



	public void destroyAsset()
	{
		hitObject = null;
		shooter = null;
		bulletData = null;
		skillData = null;
		secondBullet = null;
		bTransform = null;
		damageToCharacter = null;
		gobj = null;
		shooter = null;
		effects = null;
	}
}



public class TrajecotryCalc
{
	public static float value(float startValue, float G, float mValue, float targetTime, bool isMinus)
	{
		float delta = GameManager.globalDeltaTime;//GameManager.LOOP_INTERVAL;
		
		while(0 <= targetTime)
		{
			if(isMinus) startValue -= mValue * delta;//GameManager.LOOP_INTERVAL;
			else startValue += mValue * delta;//GameManager.LOOP_INTERVAL;
			
			mValue += G * delta;
			targetTime -= delta;
		}
		
		return startValue;
	}
}
