using System;
using UnityEngine;
using System.Collections.Generic;

sealed public partial class AttackData
{
	const int CODE_PATCH_NUM = 1;


	private float damagePer = 1.0f;
	private float minimumDamagePer = 1.0f;
	private IVector3 _hitPos;
	// 스플레시 데미지가 작용되는것...
	
	static List<Monster> _chaByRangeDistance = new List<Monster>();



	const string EMPTY_BULLETDATA = "UH1";

	void checkShotEffect(Monster mon, IVector3 pos)
	{
		if(_skillData != null && _skillData.hasShotEffect)
		{
			if(mon.shootingHand != null)
			{
				_skillData.shootShotEffect(mon.shootingHand.position,mon.shootingHand);
			}
			else
			{
				_skillData.shootShotEffect(pos, mon.cTransform);
			}
		}
	}

	
	void shootMonster0(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		if(_skillData.skillDataType == BaseSkillData.SkillDataType.Hero && _skillData.targeting == 1)
		{
			_v = mon.cTransformPosition;
			
			float hitRadius = 0.0f;
			
			if(mon.isPlayerSide) // 주인공 쪽이면.
			{
				_v.x += _skillData.targetAttr[0].Get();
				
				//				lineLeft = _v.x - ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				//				lineRight = _v.x + ((float)_skillData.targetAttr[1])*0.5f;// // 오른쪽
				//				lineFront = _v.z - ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				//				lineDistance = _v.z + ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				
				hitRadius = ((float)_skillData.targetAttr[1])*0.5f;
				
				// 주인공편에게.
				if(_skillData.targetType == Skill.TargetType.ME)
				{
					for(int i = cm.playerMonster.Count - 1; i >= 0; --i)
					{
						if(cm.playerMonster[i].isEnabled == false) continue;
						//if(cm.playerMonster[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.playerMonster[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.playerMonster[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
					
				}
				// 몬스터에게.
				else
				{
					for(int i = cm.monsters.Count - 1; i >= 0; --i)
					{
						if(cm.monsters[i].isEnabled == false) continue;
						//if(cm.monsters[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.monsters[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.monsters[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
			}
			else // pvp 캐릭터 쪽이면.
			{
				_v.x -= _skillData.targetAttr[0].Get();
				
				//				lineLeft = _v.x - ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				//				lineRight = _v.x + ((float)_skillData.targetAttr[1])*0.5f;// // 오른쪽
				//				lineFront = _v.z - ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				//				lineDistance = _v.z + ((float)_skillData.targetAttr[1])*0.5f;// 왼쪽
				
				hitRadius = ((float)_skillData.targetAttr[1])*0.5f;
				
				// 몬스터에게.
				if(_skillData.targetType == Skill.TargetType.ME)
				{
					for(int i = cm.monsters.Count - 1; i >= 0; --i)
					{
						if(cm.monsters[i] == mon) continue;
						if(cm.monsters[i].isEnabled == false) continue;
						//if(cm.monsters[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.monsters[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.monsters[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
				// 주인공들에게.
				else
				{
					for(int i = cm.playerMonster.Count - 1; i >= 0; --i)
					{
						if(cm.playerMonster[i].isEnabled == false) continue;
						//if(cm.playerMonster[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.playerMonster[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.playerMonster[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
			}
		}
		else
		{
			_skillData.applySkillEffect(mon.skillTarget, heroSkillLevel, null, applyReinforceLevel, mon);	
		}
	}	



	
	void shootMonster1(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		if(mon.target == null) mon.target = cm.getShortTargetCharacter(mon);

		//* 공격거리 (cm) : 공격이 가능한 최대 거리, (모든 유닛이 최소공격거리는 없음)
		if(mon.target != null)
		{
			mon.target.attacker = null;
			mon.target.attackerUniqueId = -1;

			if( VectorUtil.DistanceXZ(mon.cTransformPosition, mon.target.cTransformPosition) < (mon.stat.atkRange + mon.target.damageRange + mon.damageRange) * 1.3f)
			{
//				Log.log("shootMonster1 " + mon.resourceId);

				mon.target.attacker = mon;
				mon.target.attackerUniqueId = mon.stat.uniqueId;

				//_skillData 

				if(mon.target.damage(mon.stat.monsterType, mon, mon.stat.uniqueId, (_skillData != null), totalDamageNum, mon.tf, mon.stat.atkPhysic, mon.stat.atkMagic, 1.0f, 1.0f, true, GameManager.info.skillEffectSetupData[mon.atkEffectType].effUp, GameManager.info.skillEffectSetupData[mon.atkEffectType].soundUp ))
				{
				}
			}
			else
			{
//				if(VersionData.codePatchVer >= 3)
				{
					mon.setTarget(null);
				}
			}

		}		
	}
	
	void shootMonster2(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

//		if(VersionData.codePatchVer >= 4)
		{
			minimumDamagePer = ((float)attr[1]) / 100.0f;//* 0.01f;
		}
//		else
//		{
//			minimumDamagePer = ((float)attr[0]) * 0.01f;
//
//		}

		checkHitWithRange(mon, totalDamageNum);
	}


	public void checkHitWithRange(Monster attacker, int totalDamageNum)
	{
		bool hitDefaultTarget = false; // 


#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log(attacker.resourceId +  "  checkHitWithRange : " + totalDamageNum + "    mon len : " + GameManager.me.characterManager.monsters.Count + "    p len : " + GameManager.me.characterManager.playerMonster.Count);
		}
#endif

		damagePer = 1.0f;
		int leftNum = attr[2];
		
		// 기본적으로 최초 쳐맞는 적은 공격 거리를 이용해서 팬다.

		Monster mon;

		_hitPos = attacker.cTransformPosition + (IVector3)(attacker.tf.forward) * (attacker.stat.atkRange + attacker.damageRange); // hitrange
		attacker.effectTargetPosition = _hitPos;

		attacker.setTarget(null);

		//if(attacker.target == null)
		{
			if(attacker.isPlayerSide)
			{
				int len = GameManager.me.characterManager.monsters.Count;
				
				for(int i = len -1; i >= 0 ; --i)
				{	
					mon = GameManager.me.characterManager.monsters[i];
					if(mon.isEnabled == false  || (attacker.target != null && attacker.target == GameManager.me.characterManager.monsters[i])) continue; //|| mon.skipHitCheck
					mon.distanceFromHitPoint = VectorUtil.DistanceXZ(attacker.cTransformPosition, mon.cTransformPosition);
					_chaByRangeDistance.Add(mon);
				}
			}
			else
			{
				int len = GameManager.me.characterManager.playerMonster.Count;
				
				for(int i = len -1; i >= 0 ; --i)
				{	
					mon = GameManager.me.characterManager.playerMonster[i];
					if(mon.isEnabled == false  || (attacker.target != null && attacker.target == mon)) continue; //|| mon.skipHitCheck
					mon.distanceFromHitPoint = VectorUtil.DistanceXZ(attacker.cTransformPosition, mon.cTransformPosition);
					_chaByRangeDistance.Add(mon);
				}
			}

			if(_chaByRangeDistance.Count > 0)
			{
				_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
				attacker.setTarget(_chaByRangeDistance[0]);
			}
		}

		_chaByRangeDistance.Clear();

		if(attacker.target != null)
		{
			float dist =  VectorUtil.DistanceXZ(attacker.cTransformPosition, attacker.target.cTransformPosition);


			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false)
			{
//				Log.log(attacker.resourceId + "  dist : " + dist + "  attacker.cTransformPosition : " + attacker.cTransformPosition + "  attacker.target.cTransformPosition : " + attacker.target.cTransformPosition);
//				Log.log("attacker.stat.atkRange : " + attacker.stat.atkRange + "  attacker.target.damageRange : " + attacker.target.damageRange + "  attacker.damageRange : " + attacker.damageRange);
			}
			#endif


			if(dist < (attacker.stat.atkRange + attacker.target.damageRange + attacker.damageRange) * 1.2f) //hitrange
			{
				hitDefaultTarget = true;
				_hitPos = attacker.target.cTransformPosition;
				attacker.effectTargetPosition = _hitPos;
				attacker.target.attacker = attacker;	
				attacker.target.attackerUniqueId = attacker.stat.uniqueId;					
				attacker.target.damage(attacker.stat.monsterType, attacker, attacker.stat.uniqueId, (_skillData != null), totalDamageNum, attacker.tf, attacker.stat.atkPhysic, attacker.stat.atkMagic, damagePer, minimumDamagePer, true, GameManager.info.skillEffectSetupData[attacker.atkEffectType].effUp, GameManager.info.skillEffectSetupData[attacker.atkEffectType].soundUp);
				--leftNum;
			}
		}

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log("leftNum : " + leftNum + "  _hitDefaultTarget : " + _hitDefaultTarget + "  attacker.target : " + attacker.target);
		}
		#endif



		// 이후에는 그 타겟 지점으로부터 스플래시 데미지를 적용한다.

		if(attacker.isPlayerSide)
		{
			int len = GameManager.me.characterManager.monsters.Count;
			
			// 총알에서 가까운 녀석들부터 처리하기위해... 이때는 가볍게 계산하기위해 x 좌표만 계산해준다...
			for(int i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.monsters[i];
				if(mon.isEnabled == false  || attacker.target != null && (attacker.target == mon && hitDefaultTarget)) continue; //|| mon.skipHitCheck
				mon.distanceFromHitPoint = VectorUtil.Distance(_hitPos.x, mon.cTransformPosition.x);
				_chaByRangeDistance.Add(mon);
			}
			
			_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
			
			len = _chaByRangeDistance.Count;
			
			for(int i = 0; i < len ; ++i)
			{
				if(leftNum <= 0) break;	
				damageDetail(_hitPos, attacker, _chaByRangeDistance[i], ref leftNum, totalDamageNum);
			}			
		}
		else
		{
			int len = GameManager.me.characterManager.playerMonster.Count;

			for(int i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.playerMonster[i];
				if(mon.isEnabled == false  || (attacker.target != null && attacker.target == mon && hitDefaultTarget)) continue; //|| mon.skipHitCheck
				mon.distanceFromHitPoint = VectorUtil.Distance(_v.x, mon.cTransformPosition.x);
				_chaByRangeDistance.Add(mon);
			}
			
			_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);
			
			len = _chaByRangeDistance.Count;
			
			for(int i = 0; i < len; ++i)
			{
				if(leftNum <= 0) break;	
				
				damageDetail(_hitPos, attacker, _chaByRangeDistance[i], ref leftNum, totalDamageNum);
			}
		}
		
		
		_chaByRangeDistance.Clear();
	}
	
	private void damageDetail(IVector3 hitPosition, Monster attacker, Monster target, ref int leftNum, int totalDamageNum)
	{
		//데미지범위	최소데미지비율	최대타겟유닛
		// 특정 상태에서만 타격이 가능.
		//if(target.isMonster == true && target.data.hasHitCondition && target.data.hitCondition != target.state)
		//{
		//}
		//else
		{ 
			IFloat dist =  VectorUtil.DistanceXZ(hitPosition, target.cTransformPosition);
			
//			Log.logError(hitPosition, target.cTransformPosition, dist, attr[0]);
			
			if(dist < (attr[0].Get() + target.damageRange))
			{
				IFloat damagePer = 1.0f - (dist / ( (float)attr[0] * 0.5f));
				
//				Log.logError(damagePer);
				
				if(damagePer < 0.0f) damagePer = 0.1f;
				else if(damagePer > 1.0f) damagePer = 1.0f;
				
//				Log.log("damageDetail : damagePer : " + damagePer);

				//_skillData 

				if(target.damage(attacker.stat.monsterType, attacker, attacker.stat.uniqueId, (_skillData != null), totalDamageNum, attacker.tf, attacker.stat.atkPhysic, attacker.stat.atkMagic, damagePer, minimumDamagePer, true, GameManager.info.skillEffectSetupData[attacker.atkEffectType].effUp))
				{
					if(target != null)
					{
						target.attacker = attacker;
						target.attackerUniqueId = attacker.stat.uniqueId;						
					}
				}
				
				//mon.damage(this, mon, damagePer, ((float)attr[1])*0.01f);
				SoundData.playHitSound(attacker.atkEffectType, false);
				
				--leftNum;
			}
		}
	}	
	
	
	
	
	
	void shootMonsterBullet3(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null)
		{
			_skillData.playSkillSound();
		}

#if UNITY_EDITOR
//		Debug.Log(mon.nowBulletPatternId);
#endif

		// TYPE 3. 직선발사 단일공격  : 최대비행거리	비행속도
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);

		if(attr[2] == 1)
		{
			b.init(3, BulletMotionType.BACKWARD_TRAJECOTRY, totalDamageNum, _skillData);
			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];
			b.limitTime.Set( ((float)attr[3]) / 100.0f);//* 0.01f );

			if(mon.isPlayerSide)
			{
				_v.x -= mon.hitObject.width;
			}
			else
			{
				_v.x += mon.hitObject.width;
			}

			b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);

			if(_skillData != null && _skillData.isTargetingForward) //&& VersionData.codePatchVer >= 2)
			{
				_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
			}
			else
			{
				if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
				{
					_v.x = targetX; 
					_v.y = targetH;
					_v.z = targetZ;
					_v.y = _v.y * 0.5f;
				}
				else
				{
					_v = mon.targetPosition;
					_v.y = mon.targetHeight  * 0.5f;	
				}

			}

			float gy = GameManager.inGameRandom.Range(0,3) * 100.0f + 200;

			b.setBackwardTrajeCorty(_v,gy,(GameManager.inGameRandom.Range(0,10)<5));
		}
		else
		{
			b.init(3, BulletMotionType.ANGLE, totalDamageNum, _skillData);
			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];
			b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);

//			if(VersionData.codePatchVer >= 2)
			{
				if(_skillData != null && _skillData.isTargetingForward) _v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
				else
				{
					if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
					{
						_v.x = targetX;
						_v.y = targetY;
						_v.z = targetZ;
					}
					else
					{
						_v = mon.targetPosition;
					}


					
					if(mon.isPlayerSide)
					{
						if(_v.x < mon.lineRight) _v.x = mon.lineRight.Get();
					}
					else
					{
						if(_v.x > mon.lineLeft) _v.x = mon.lineLeft.Get();
					}
					
					
					_v.y = mon.targetHeight  * 0.5f;	
				}
			}
//			else
//			{
//				_v = mon.targetPosition;
//				_v.y = mon.targetHeight * 0.5f;
//			}

			b.setAngle(_v);
		}

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;				
	}
	
	void shootMonsterBullet4(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 4. 직선발사 범위공격 : 최대비행거리	비행속도	데미지범위	최소데미지비율	최대타겟유닛수			
		#if UNITY_EDITOR
		//		Debug.Log("mon.nowBulletPatternId : " + mon.nowBulletPatternId);
		#endif
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(4, BulletMotionType.ANGLE, totalDamageNum, _skillData);		
		b.isDistanceLimitBullet = true;
		b.limitDistance = attr[0];
		b.useRangeDamagePer.Set( true );
		b.targetRange = ((float)attr[2]) * 0.5f;
		// 최소데미지 비율이란 것은 특정 지점에서 데미지가 떨어졌다.
		// 그럼 반경에 있는 애들한테 스플래시 데미지가 입혀지는데..
		// 최초 데미지가 100이라고 하면 가장 멀리있는 애는 100에서 데미지 비율(50) = 50%의 데미지를 바는다는 뜻이다.
		b.minimumDamagePer = ((float)attr[3]) / 100.0f;//* 0.01f;
		b.setTargetNum(attr[4]);
		
		b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);	

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; 
			_v.y = targetH;
			_v.z = targetZ;
			_v.y = _v.y * 0.5f;
			
		}
		else
		{
			_v = mon.targetPosition;
			_v.y = mon.targetHeight  * 0.5f;
		}

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.setAngle(_v);

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;		
	}


	void shootMonsterBullet5(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 5. 직선발사 관통 이펙트 충돌공격 : 최대비행거리	비행속도	최대타겟유닛수	이펙트타입	이펙트Z/R
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 

		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);
		
		b.init(5, BulletMotionType.ANGLE, totalDamageNum, _skillData);		
		
		b.limitDistance = attr[0];
		b.hp = attr[2]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[2]);

		b.type5minDamagerPer = (float)attr[5] / 100.0f;//* 0.01f;
		if(b.type5minDamagerPer <= 0) b.type5minDamagerPer = 1.0f;

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);	

		if(mon.hasShootingPos)
		{
			if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
			{
				_v.x = targetX;
				_v.z = targetZ;
			}
			else
			{
				_v.x = mon.targetPosition.x;
				_v.z = mon.targetPosition.z;
			}
		}
		else
		{
			if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
			{
				_v.x = targetX; 
				_v.y = targetH;
				_v.z = targetZ;
				_v.y = _v.y * 0.5f;

			}
			else
			{
				_v = mon.targetPosition;
				_v.y = mon.targetHeight  * 0.5f;
			}

		}

		if(_skillData != null && _skillData.isTargetingForward) //&& VersionData.codePatchVer >= 2)
		{
			_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
		}


		b.setAngle(_v);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;		

		b.retargetingRange = attr[6];

		if(attr[3] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[4])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[4]);
		}
	}

	
	void shootMonsterBullet6(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 6. 곡선발사 범위공격 : 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);
		
		b.init(6, BulletMotionType.TRAJECOTRY, totalDamageNum, _skillData);		
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( (float)attr[0]*0.001f );
		b.targetRange = ((float)attr[1]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[3]);
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.y = targetY; _v.z = targetZ;
			b.setTrajeCorty(_v + getUnitSkillTargetingPositionRandomPositionOffset());
		}
		else
		{
			b.setTrajeCorty(mon.targetPosition + getUnitSkillTargetingPositionRandomPositionOffset());
		}

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;	
	}
	
	void shootMonsterBullet7(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		
		
		// TYPE 7. 낙하 범위공격 : 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수		
		b.init(7, BulletMotionType.POSITION, totalDamageNum, _skillData);		

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.z = targetZ;
			_v.y = 1.0f;
			_v = _v + getUnitSkillTargetingPositionRandomPositionOffset() + Util.getPositionByAngleAndDistance(180-attr[4],800.0f);
		}
		else
		{
			_v = mon.targetPosition;
			_v.y = 1.0f;
			mon.targetPosition = _v + getUnitSkillTargetingPositionRandomPositionOffset();
			_v = Util.getPositionByAngleAndDistance(180-attr[4],800.0f) + mon.targetPosition;
		}

		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( (float)attr[0]*0.001f );
		b.targetRange = ((float)attr[1]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[3]);
		b.init(mon.isPlayerSide,bd,5,_v,mon);//false, bd , false, bd.speed, 340, _v, mon);

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.y = 1; _v.z = targetZ;
			b.setPositionBullet(_v + getUnitSkillTargetingPositionRandomPositionOffset());
		}
		else
		{
			b.setPositionBullet(mon.targetPosition);
		}

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;	

		b.isCircleColliderType = true;
		
	}
	
	void shootMonsterBullet8(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 8. 즉시발생 범위공격 : 데미지범위	최소데미지비율	최대타겟유닛수	
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];
#if UNITY_EDITOR
		//Debug.LogError(mon.name + "   " +  mon.resourceId + "  " +  bpd.ids[0]);
#endif

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		
		
		b.init(8, BulletMotionType.POSITION, totalDamageNum, _skillData);		
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( 0.1f );
		b.targetRange = ((float)attr[0]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[1] / 100.0f;//* 0.01f;		
		b.setTargetNum(attr[2]);

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.y = targetY; _v.z = targetZ;
			_v = _v + getUnitSkillTargetingPositionRandomPositionOffset();		
		}
		else
		{
			_v = mon.targetPosition + getUnitSkillTargetingPositionRandomPositionOffset();		
		}

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(mon.isPlayerSide,bd,0.0f, _v, mon, (mon.isPlayerSide)?90.0f:270.0f);//false, bd , false, bd.speed, 340, _v, mon);

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;
	}




	void shootMonsterBullet9(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{

		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 9. 순간 위치고정 이펙트 충돌공격 : 최대타겟유닛수	이펙트타입	이펙트Z/R	이펙트X
		
		// 총알을 날리고... 그 위에 이펙트를 뿌리는 것도 하나의 방법...
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		b.init(9, BulletMotionType.POSITION, totalDamageNum, _skillData);		

		b.limitTime.Set( 0.1f );

		b.hp = attr[0]; // 최대 타겟 유닛수.

		b.setTargetNum(attr[0]);
		// attr[1] // 이펙트 타입
		// attr[2] // 이펙트 z,r

		// 시작 좌표를 제대로 지정하지 않으면 충돌 검사가 제대로 안될거다...

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.y = targetY; _v.z = targetZ;
			_v = _v + getUnitSkillTargetingPositionRandomPositionOffset();		
		}
		else
		{
			_v = mon.targetPosition + getUnitSkillTargetingPositionRandomPositionOffset();		
		}

		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	
		
//		_v.x += (mon.isPlayerSide)?1000.0f:-1000.0f;
//		
//		b.setAngle(_v);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;		

		if(attr[1] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[2])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[2],attr[3]);
		}
	}


	
	void shootMonsterBullet10(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 10. 지속 위치고정 이펙트 충돌공격 : 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		
		// 총알을 날리고... 그 위에 이펙트를 뿌리는 것도 하나의 방법...
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		
		#if UNITY_EDITOR
		//		Debug.Log("shootMonsterBullet10 : mon.nowBulletPatternId : " + mon.nowBulletPatternId);
		//		Debug.Log("bpd.ids[0] : " + bpd.ids[0]);
		#endif
		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		b.init(10, BulletMotionType.ANGLE, totalDamageNum, _skillData);		
		
		//b.hp = attr[0]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[0],true);
		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );
		// attr[2] // 이펙트 타입
		// attr[3] // 이펙트 z,r
		// attr[4] // 이펙트 x		
		
		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.z = targetZ;
			_v.y = 1;
			b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	
			b.setAngle(_v + getUnitSkillTargetingPositionRandomPositionOffset());
		}
		else
		{
			_v = mon.targetPosition;
			_v.y = 1.0f;
			mon.targetPosition = _v + getUnitSkillTargetingPositionRandomPositionOffset();
			b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	
			b.setAngle(mon.targetPosition);
		}

		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = (mon.isPlayerSide)?90.0f:270.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		
		b.bTransform.localRotation = _q;			

		b.setEffectLocalPosition(mon);

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;		

		if(attr[2] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[3])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[3],attr[4]);
		}
	}
	
	void shootMonsterBullet11(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 11. 곡선발사 후 지속 위치고정 이펙트 충돌공격 : 타임리밋	최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);
		
		b.init(11, BulletMotionType.TRAJECOTRY, totalDamageNum, _skillData);		
		b.isCollisionCheckAtEndOnly = true;
		b.canCollide = false;
		b.limitTime.Set( (float)attr[0]*0.001f );
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v.x = targetX; _v.y = targetY; _v.z = targetZ;
			b.setTrajeCorty(_v + getUnitSkillTargetingPositionRandomPositionOffset());
		}
		else
		{
			b.setTrajeCorty(mon.targetPosition + getUnitSkillTargetingPositionRandomPositionOffset());
		}

		
		BulletData bd2 = GameManager.info.bulletData[bpd.ids[1]]; 
		Bullet b2 = GameManager.me.bulletManager.getBullet();		
		b.secondBullet = b2;

		b2.init(10, BulletMotionType.ANGLE);		
		//b2.hp = attr[1]; // 최대 타겟 유닛수.
		b2.setTargetNum(attr[1],true);
		b2.invincible.Set( true );
		b2.limitTime.Set( (float)attr[2]*0.001f ); // 지속시간.
		// attr[3] // 이펙트 타입
		// attr[43] // 이펙트 z,r
		// attr[5] // 이펙트 x		
		// 누운 각도...
		b2.init(mon.isPlayerSide, bd2 , 0.0f, _v, mon);			
		
		_q = b2.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		
		b2.bTransform.localRotation = _q;	
		b2.applyReinforceLevel = applyReinforceLevel;
		b2.heroSkillLevel = heroSkillLevel;

		b2.isEnabled = false;	
		b.isEnabled = true;

		b2.setReadyAttachedParticleEffect(false);

		if(attr[3] > 12) // 일반 원기둥.
		{
			b2.isCircleColliderType = true;
			b2.targetRange = ((float)attr[4])*0.5f;
		}
		else // 사각기둥.
		{
			b2.isCircleColliderType = false;
			b2.setHitObject(attr[4],attr[5]);
		}
	}	
	
	void shootMonsterBullet12(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 12. 지속 캐릭터어태치 이펙트 충돌공격 : 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		b.init(12, BulletMotionType.ATTACHED, totalDamageNum, _skillData, 0.5f);		
		
		//b.hp = attr[0]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[0],true);
		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );
		
		_v = Monster.setShootingHand(mon, mon.hitObject.height * 0.5f);
		
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);			

		_v = mon.tf.rotation.eulerAngles;
		_v.x = 0.0f;
		//_v.y -= (mon.isPlayerSide)?90.0f:-90.0f;	
		//_v.z = (mon.isPlayerSide)?-90.0f:90.0f;	

		if(mon.hasShootingPos && mon.shootingPos.Length == 4)
		{
			_v.x += mon.shootingPos[3];
		}

		_q = b.bTransform.rotation;
		_q.eulerAngles = _v;
		b.bTransform.rotation = _q;
		
		
		_q = b.bTransform.rotation;
		_q.eulerAngles = _v;
		b.bTransform.rotation = _q;		

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;		

		if(attr[2] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[3])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[3],attr[4]);
		}
	}		
	
	void shootMonsterBullet13(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 13. 시한폭탄 : 폭발대기시간	데미지범위	최소데미지비율	최대타겟유닛수
	}		
	
	void shootMonsterBullet14(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 14. 데미지범위	최소데미지비율	최대타겟유닛수		
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		
		
		b.init(14, BulletMotionType.POSITION, totalDamageNum, _skillData);		
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( 0.1f );
		b.targetRange = ((float)attr[0]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[1] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[2]);
		b.init(mon.isPlayerSide,bd,0.0f,mon.cTransformPosition,mon);//false, bd , false, bd.speed, 340, _v, mon);
		
		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 90.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		b.bTransform.localRotation = _q;
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;		
		b.isEnabled = true;			
	}			
	

	const string MAGIC_BEAM = "M";
	const string BLACK = "B";
	const string WHITE_CHAINLIGHTNING_ID = "W";
	const string WHITE_CHAINLIGHTNING2_ID = "W2";
	const string DARK2 = "D2";

	void shootMonsterBullet15(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 15. 최대전체거리	최대연결거리A	  최대연결거리B	최대연결유닛수	연결딜레이
		ChainLightning cl = GameManager.me.bulletManager.getChainLightning(GameManager.info.bulletPatternData[mon.nowBulletPatternId].ids[0],GameManager.info.bulletPatternData[mon.nowBulletPatternId].ids[1]);

		cl.start(mon.isPlayerSide, mon, (float)attr[0],(float)attr[1],(float)attr[2],(int)attr[3],(float)(float)attr[4]*0.001f, attr[5], heroSkillLevel, applyReinforceLevel, _skillData);
		cl.hitEffectId = GameManager.info.bulletPatternData[mon.nowBulletPatternId].ids[1];

		cl.isEnabled = true;
	}	
	
	
	void shootMonsterBullet16(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 16. 최대데미지거리
		
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		
		
		b.init(16, BulletMotionType.POSITION, totalDamageNum, _skillData);

		b.targetRange = ((float)attr[0]) * 0.5f;

		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );
		
		/*
		// 특정 시간 이후에 터트릴거고.
		// 이펙트는 제한 시간까지 재생한다.
		b.isCollisionCheckAtEndOnly = true;
		b.canCollide = true;
		b.limitTime = 0.5f;
		*/

		_v = mon.cTransformPosition;

		if(attr[2] == 0)
		{
			_v.x += (mon.isPlayerSide)?b.targetRange.Get():-b.targetRange.Get();
		}


		_v.z = 0.0f;
		_v.y = 0.0f;
		
		b.init(mon.isPlayerSide,bd,0.0f,_v,mon);//false, bd , false, bd.speed, 340, _v, mon);

		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 90.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		b.bTransform.localRotation = _q;

		b.setTargetNum(9999);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;	

		b.setHitObject(2000,attr[0]);
		
	}	
	
	
	void shootMonsterBullet17(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 17. 타임리밋	  데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격
		_tempF = (float)attr[6];
		_tempI  = (int)(_tempF * 0.01f); // delay
		int totalCount = (int)(_tempF - (_tempI * 100));
		float delay = (_tempF - totalCount);
		delay *= 0.001f;

		if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
		{
			_v2.x = targetX;
			_v2.y = 1.0f;
			_v2.z = targetZ;
		}
		else
		{
			_v2 = mon.targetPosition;
			_v2.y = 1.0f;
		}

		for(int i = 0; i < totalCount; ++i)
		{
			BulletPatternData bpd = null;
			GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
			if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

			BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
			Bullet b = GameManager.me.bulletManager.getBullet();		
			
			// TYPE 7. 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수   사선 각도    
			b.init(7, BulletMotionType.POSITION, totalDamageNum, _skillData, delay * i);		
			_v = _v2 + Util.getPositionByAngleAndDistanceXZ(GameManager.inGameRandom.Range(0,360),(float)attr[5]);
			if(_v.z > MapManager.top) _v.z = MapManager.top;
			else if(_v.z < MapManager.bottom) _v.z = MapManager.bottom;
			
			b.isCollisionCheckAtEndOnly = true;
			b.limitTime.Set( (float)attr[0]*0.001f );
			b.targetRange = ((float)attr[1]) * 0.5f;
			b.useRangeDamagePer.Set( true );
			b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
			b.setTargetNum(attr[3]);

#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.useEffectSkillCamEditor)
			{
				if(mon.isPlayerSide)
				{
					b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(180-attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
				}
				else
				{
					b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
				}
			}
			else
#endif
			{
				b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(180-attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
			}


			b.setPositionBullet(_v);
			b.applyReinforceLevel = applyReinforceLevel;
			b.heroSkillLevel = heroSkillLevel;
			b.isEnabled = true;		
			if(i > 0) b.setReadyAttachedParticleEffect(false, true, false);
		}
	}		
	
	








	void shootMonsterBullet18(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyReinforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(_skillData != null)
		{
			_skillData.playSkillSound();
		}
		
		#if UNITY_EDITOR
		//		Debug.Log(mon.nowBulletPatternId);
		#endif
		
		// TYPE 18. 직선발사 단일공격  : 최대비행거리	 비행속도	   탄환개수
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 


		
		for(int i = 0; i < attr[2]; ++i)
		{
			Bullet b = GameManager.me.bulletManager.getBullet();

			b.init(18, BulletMotionType.CHASER, totalDamageNum * attr[2], _skillData, i * 0.15f - 0.01f);

			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];

			_v = Monster.setShootingHand(mon);
			if(mon.isPlayerSide)
			{
				_v.x -= mon.hitObject.width;
			}
			else
			{
				_v.x += mon.hitObject.width;
			}
			
			b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);

			b.targetMonsterUniqueId = mon.targetUniqueId;

			if(_skillData != null && _skillData.isTargetingForward )//&& VersionData.codePatchVer >= 2)
			{
				_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
			}
			else
			{
				if(targetY > -1000) // VersionData.checkCodeVersion(CODE_PATCH_NUM) && 
				{
					_v.x = targetX;
					_v.y = targetH;
					_v.y = _v.y * 0.5f;
					_v.z = targetZ;
				}
				else
				{
					_v = mon.targetPosition;
					_v.y = mon.targetHeight  * 0.5f;	
				}
			}

			float gy = GameManager.inGameRandom.Range(0,3) * 250.0f + 200;
			
			b.setChaser(_v,gy,(i % 2 == 0));

			b.applyReinforceLevel = applyReinforceLevel;
			b.heroSkillLevel = heroSkillLevel;

			b.isEnabled = false;
			b.setReadyAttachedParticleEffect(false);

			GameManager.me.bulletManager.delayBullets.Add(b);
		}
	}






	
	
	//====================================
	
	float lineLeft = 0;
	float lineRight = 0;
	float lineFront = 0;
	float lineDistance = 0;
	
	void shootPlayer0(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		if(_skillData.skillDataType == BaseSkillData.SkillDataType.Hero && _skillData.targeting == 1)
		{
			_v = mon.cTransformPosition;
			
			float hitRadius = ((float)_skillData.targetAttr[1])*0.5f;
			
			// 프리뷰일때는 거의 광역으로!
			if(UIPopupSkillPreview.isOpen) hitRadius = 5000.0f;
			
			if(mon.isPlayerSide) // 주인공 쪽이면.
			{
				_v.x += _skillData.targetAttr[0].Get();
				
				//				lineLeft = _v.x - hitRadius;// 왼쪽
				//				lineRight = _v.x + hitRadius;// // 오른쪽
				//				lineFront = _v.z - hitRadius;// 왼쪽
				//				lineDistance = _v.z + hitRadius;// 왼쪽
				
				// 주인공편에게.
				if(_skillData.targetType == Skill.TargetType.ME)
				{
					for(int i = cm.playerMonster.Count - 1; i >= 0; --i)
					{
						if(cm.playerMonster[i].isEnabled == false) continue;
						
						//if(cm.playerMonster[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.playerMonster[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.playerMonster[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
					
				}
				// 몬스터에게.
				else
				{
					for(int i = cm.monsters.Count - 1; i >= 0; --i)
					{
						if(cm.monsters[i].isEnabled == false) continue;
						//if(cm.monsters[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.monsters[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.monsters[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
			}
			else // pvp 캐릭터 쪽이면.
			{
				_v.x -= _skillData.targetAttr[0].Get();
				
				//				lineLeft = _v.x - hitRadius;// 왼쪽
				//				lineRight = _v.x + hitRadius;// // 오른쪽
				//				lineFront = _v.z - hitRadius;// 왼쪽
				//				lineDistance = _v.z + hitRadius;// 왼쪽
				
				
				// 몬스터에게.
				if(_skillData.targetType == Skill.TargetType.ME)
				{
					for(int i = cm.monsters.Count - 1; i >= 0; --i)
					{
						if(cm.monsters[i] == mon) continue;
						if(cm.monsters[i].isEnabled == false) continue;
						//if(cm.monsters[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.monsters[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.monsters[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
				// 주인공들에게.
				else
				{
					for(int i = cm.playerMonster.Count - 1; i >= 0; --i)
					{
						if(cm.playerMonster[i].isEnabled == false) continue;
						//if(cm.playerMonster[i].getHitObject().intersects2(lineLeft, lineFront, lineRight, lineDistance))
						if(cm.playerMonster[i].hitByDistance (_v, hitRadius))
						{
							_skillData.applySkillEffect(cm.playerMonster[i], heroSkillLevel, null, applyReinforceLevel, mon);	
						}
					}
				}
			}
		}
		else
		{
			_skillData.applySkillEffect(mon.skillTarget, heroSkillLevel, null, applyReinforceLevel, mon);	
		}
	}
	
	
	void shootPlayer1(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		/*
		//* 공격거리 (cm) : 공격이 가능한 최대 거리, (모든 유닛이 최소공격거리는 없음)
		if(GameManager.me.player.target != null)
		{
			if( VectorUtil.DistanceXZ(GameManager.me.player.cTransformPosition, GameManager.me.player.target.cTransformPosition) < (GameManager.me.player.data.hitRange + GameManager.me.player.target.damageRange) * 1.2f)//mon.hitObject.diagonalWidthDepth + mon.target.hitObject.diagonalWidthDepth) 
			{
				mon.target.damage(mon.stat.atkPhysic, mon.stat.atkMagic);
				GameManager.soundManager.playEffect("eff/demage_bluntness_0");
			}
		}
		*/		
	}
	
	void shootPlayer2(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{

		if(_skillData != null) _skillData.playSkillSound();
		//checkHitWithRange(mon);
	}	
	



	
	void shootPlayerBullet3(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 3. 직선발사 단일공격  : 최대비행거리	비행속도
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 

		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon,mon.hitObject.height * 0.7f);
		
		checkShotEffect(mon,_v);
			
		if(attr[2] == 1)
		{
			b.init(3, BulletMotionType.BACKWARD_TRAJECOTRY, 1, _skillData);
			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];
			b.limitTime.Set( ((float)attr[3]) / 100.0f);//* 0.01f );

			if(mon.isPlayerSide)
			{
				_v.x -= mon.hitObject.width;
			}
			else
			{
				_v.x += mon.hitObject.width;
			}


			b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);

			if(_skillData != null && _skillData.isTargetingForward) _v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
			else
			{
				_v = mon.targetPosition;
				_v.y = mon.targetHeight  * 0.5f;	
			}
			
			float gy = GameManager.inGameRandom.Range(0,3) * 100.0f + 200;
			
			b.setBackwardTrajeCorty(_v,gy,(GameManager.inGameRandom.Range(0,10)<5));
		}
		else
		{
			b.init(3, BulletMotionType.ANGLE, 1, _skillData);
			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];
			b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);
			
			if(_skillData != null && _skillData.isTargetingForward) _v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
			else
			{
				_v = mon.targetPosition;

				if(mon.isPlayerSide)
				{
					if(_v.x < mon.lineRight) _v.x = mon.lineRight.Get();
				}
				else
				{
					if(_v.x > mon.lineLeft) _v.x = mon.lineLeft.Get();
				}


				_v.y = mon.targetHeight  * 0.5f;	
			}
			
			b.setAngle(_v);
		}

		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;

		b.isEnabled = true;	
	}
	
	void shootPlayerBullet4(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 4. 직선발사 범위공격 : 최대비행거리	비행속도	데미지범위	최소데미지비율	최대타겟유닛수			
		
		
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon, mon.hitObject.height * 0.7f);	

		checkShotEffect(mon,_v);

		b.init(4, BulletMotionType.ANGLE, 1, _skillData);		
		b.isDistanceLimitBullet = true;
		b.limitDistance = attr[0];
		b.useRangeDamagePer.Set( true );
		b.targetRange = ((float)attr[2]) * 0.5f;
		// 최소데미지 비율이란 것은 특정 지점에서 데미지가 떨어졌다.
		// 그럼 반경에 있는 애들한테 스플래시 데미지가 입혀지는데..
		// 최초 데미지가 100이라고 하면 가장 멀리있는 애는 100에서 데미지 비율(50) = 50%의 데미지를 바는다는 뜻이다.
		b.minimumDamagePer = ((float)attr[3]) / 100.0f;//* 0.01f;
		b.setTargetNum(attr[4]);

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);	
		
		
		if(_skillData != null && _skillData.isTargetingForward) _v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
		else
		{
			_v = mon.targetPosition;
			_v.y = mon.targetHeight  * 0.5f;	
		}

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.setAngle(_v);		
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;		
	}
	
	void shootPlayerBullet5(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 5. 직선발사 관통 이펙트 충돌공격 : 최대비행거리	비행속도	최대타겟유닛수	이펙트타입	이펙트Z/R
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon, mon.hitObject.height * 0.7f);

		checkShotEffect(mon,_v);

		b.init(5, BulletMotionType.ANGLE, 1, _skillData);
		
		b.limitDistance = attr[0];
		b.hp = attr[2]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[2]);
		b.type5minDamagerPer = (float)attr[5] / 100.0f;//* 0.01f;
		if(b.type5minDamagerPer <= 0) b.type5minDamagerPer = 1.0f;

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);	
		
		if(_skillData != null && _skillData.isTargetingForward)
		{
			_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
		}
		else
		{
			_v = mon.targetPosition;
			_v.y = mon.targetHeight * 0.5f;	
		}
		
		b.setAngle(_v);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;	

		b.retargetingRange = attr[6];

		if(attr[3] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[4])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[4]);
		}

	}
	
	void shootPlayerBullet6(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 6. 곡선발사 범위공격 : 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon, mon.hitObject.height * 0.7f);

		checkShotEffect(mon,_v);

		b.init(6, BulletMotionType.TRAJECOTRY, 1, _skillData);
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( (float)attr[0]*0.001f );
		b.targetRange = ((float)attr[1]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[3]);
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);
		b.setTrajeCorty(mon.targetPosition);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;			
	}
	
	void shootPlayerBullet7(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 7. 낙하 범위공격 : 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수		
		b.init(7, BulletMotionType.POSITION, 1, _skillData);
		_v = mon.targetPosition;
		_v.y = 1.0f;
		mon.targetPosition = _v;
		_v = Util.getPositionByAngleAndDistance(90,800.0f) + mon.targetPosition;
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set((float)attr[0]*0.001f );
		b.targetRange = ((float)attr[1]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[3]);
		b.init(mon.isPlayerSide,bd,5,_v,mon);//false, bd , false, bd.speed, 340, _v, mon);
		b.setPositionBullet(mon.targetPosition);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;
		b.isCircleColliderType = true;
	}
	
	void shootPlayerBullet8(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 8. 즉시발생 범위공격 : 데미지범위	최소데미지비율	최대타겟유닛수	
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		

		b.init(8, BulletMotionType.POSITION, 1, _skillData);
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( 0.1f );
		b.targetRange = ((float)attr[0]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[1] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[2]);


		_v = mon.targetPosition;

		if(bd.pivot > -1) _v.y = bd.pivot;

		b.init(mon.isPlayerSide,bd,0.0f, _v, mon, (mon.isPlayerSide)?90.0f:270.0f);//false, bd , false, bd.speed, 340, _v, mon);
		
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;			
		
	}
	
	void shootPlayerBullet9(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 9. 순간 위치고정 이펙트 충돌공격 : 최대타겟유닛수	이펙트타입	이펙트Z/R	이펙트X
		
		// 총알을 날리고... 그 위에 이펙트를 뿌리는 것도 하나의 방법...
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();

		b.init(9, BulletMotionType.POSITION, 1, _skillData);

		b.limitTime.Set( 0.1f );

		b.hp = attr[0]; // 최대 타겟 유닛수.

		b.setTargetNum(attr[0]);
		// attr[1] // 이펙트 타입
		// attr[2] // 이펙트 z,r

		_v = mon.targetPosition;	

		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	
		
		// 시작 좌표를 제대로 지정하지 않으면 충돌 검사가 제대로 안될거다...
//		_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
//		
//		b.setAngle(_v);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;		


		if(attr[1] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[2])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[2],attr[3]);
		}
	}
	
	void shootPlayerBullet10(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 10. 지속 위치고정 이펙트 충돌공격 : 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// 총알을 날리고... 그 위에 이펙트를 뿌리는 것도 하나의 방법...
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		b.init(10, BulletMotionType.ANGLE, 1, _skillData);
		
		//b.hp = attr[0]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[0],true);
		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );
		// attr[2] // 이펙트 타입
		// attr[3] // 이펙트 z,r
		// attr[4] // 이펙트 x		
		_v = mon.targetPosition;
		_v.y = 1.0f;
		mon.targetPosition = _v;		
		
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	
		
		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = (mon.isPlayerSide)?90.0f:270.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		
		b.bTransform.localRotation = _q;			
		
		b.setAngle(mon.targetPosition);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;		


		if(attr[2] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[3])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[3],attr[4]);
		}



	}
	
	void shootPlayerBullet11(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 11. 곡선발사 후 지속 위치고정 이펙트 충돌공격 : 타임리밋	최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X


		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		_v = Monster.setShootingHand(mon);
		checkShotEffect(mon,_v);
		
		b.init(11, BulletMotionType.TRAJECOTRY, 1, _skillData);
		b.isCollisionCheckAtEndOnly = true;
		b.canCollide = false;
		b.limitTime.Set( (float)attr[0]*0.001f );
		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.setTrajeCorty(mon.targetPosition);
		
		
		BulletData bd2 = GameManager.info.bulletData[bpd.ids[1]]; 
		Bullet b2 = GameManager.me.bulletManager.getBullet();		
		b.secondBullet = b2;
		b2.init(10, BulletMotionType.ANGLE);		
		//b2.hp = attr[1]; // 최대 타겟 유닛수.
		b2.setTargetNum(attr[1],true);
		b2.invincible.Set( true );
		b2.limitTime.Set( (float)attr[2]*0.001f ); // 지속시간.
		// attr[3] // 이펙트 타입
		// attr[43] // 이펙트 z,r
		// attr[5] // 이펙트 x		
		// 누운 각도...
		b2.init(mon.isPlayerSide, bd2 , 0.0f, _v, mon);			
		
		_q = b2.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		
		b2.bTransform.localRotation = _q;		
		b2.applyReinforceLevel = applyReinforceLevel;
		b2.heroSkillLevel = heroSkillLevel;
		b2.isEnabled = false;			
		b.isEnabled = true;

		b2.setReadyAttachedParticleEffect(false);

		if(attr[3] > 12) // 일반 원기둥.
		{
			b2.isCircleColliderType = true;
			b2.targetRange = ((float)attr[4])*0.5f;
		}
		else // 사각기둥.
		{
			b2.isCircleColliderType = false;
			b2.setHitObject(attr[4],attr[5]);
		}

	}	
	
	void shootPlayerBullet12(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 12. 지속 캐릭터어태치 이펙트 충돌공격 : 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();
		
		b.init(12, BulletMotionType.ATTACHED, 1, _skillData, 0.5f);
		
		//b.hp = attr[0]; // 최대 타겟 유닛수.
		b.setTargetNum(attr[0],true);
		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );
		
		_v = Monster.setShootingHand(mon, mon.hitObject.height * 0.6f);
		checkShotEffect(mon, _v);


		b.init(mon.isPlayerSide, bd , 0.0f, _v, mon);	

//		_v.x = (mon.isPlayerSide)?10000.0f:-10000.0f;
//		b.setAngle(_v);

		_v = mon.tf.rotation.eulerAngles;
		_v.x = 0.0f;
		//_v.y -= (mon.isPlayerSide)?90.0f:-90.0f;	탄환의 방향이 바뀌었다..
		_v.z = (mon.isPlayerSide)?-90.0f:90.0f;	

		_q = b.bTransform.rotation;
		_q.eulerAngles = _v;
		b.bTransform.rotation = _q;
		
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;	

		if(attr[2] > 12) // 일반 원기둥.
		{
			b.isCircleColliderType = true;
			b.targetRange = ((float)attr[3])*0.5f;
		}
		else // 사각기둥.
		{
			b.isCircleColliderType = false;
			b.setHitObject(attr[3],attr[4]);
		}
	}		
	
	void shootPlayerBullet13(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		// TYPE 13. 시한폭탄 : 폭발대기시간	데미지범위	최소데미지비율	최대타겟유닛수
	}		
	
	void shootPlayerBullet14(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 14. 데미지범위	최소데미지비율	최대타겟유닛수
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		
		
		b.init(14, BulletMotionType.POSITION, 1, _skillData);
		b.isCollisionCheckAtEndOnly = true;
		b.limitTime.Set( 0.1f );
		b.targetRange = ((float)attr[0]) * 0.5f;
		b.useRangeDamagePer.Set( true );
		b.minimumDamagePer = (float)attr[1] / 100.0f;//* 0.01f;
		b.setTargetNum(attr[2]);
		b.init(mon.isPlayerSide,bd,0.0f,mon.cTransformPosition,mon);//false, bd , false, bd.speed, 340, _v, mon);
		
		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 90.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		b.bTransform.localRotation = _q;
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;			
	}		

	void shootPlayerBullet15(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		ChainLightning cl = GameManager.me.bulletManager.getChainLightning(GameManager.info.bulletPatternData[mon.nowBulletPatternId].ids[0], GameManager.info.bulletPatternData[mon.nowBulletPatternId].ids[1]);

		// TYPE 15. 최대전체거리	최대연결거리A	  최대연결거리B	최대연결유닛수	연결딜레이

		if(UIPopupSkillPreview.isOpen)
		{
			cl.start(mon.isPlayerSide, mon, 3000, 3000, 3000,(int)attr[3],(float)attr[4]*0.001f, attr[5], heroSkillLevel, applyReinforceLevel, _skillData);
		}
		else
		{
			cl.start(mon.isPlayerSide, mon, (float)attr[0],(float)attr[1],(float)attr[2],(int)attr[3],(float)attr[4]*0.001f, attr[5], heroSkillLevel, applyReinforceLevel, _skillData);
		}
		
		cl.isEnabled = true;
	}	
	
	
	void shootPlayerBullet16(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 16. 최대데미지거리
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		Bullet b = GameManager.me.bulletManager.getBullet();		

		b.init(16, BulletMotionType.POSITION, 1, _skillData);

		b.targetRange = ((float)attr[0]) * 0.5f;

		b.invincible.Set( true );
		b.limitTime.Set( (float)attr[1]*0.001f );

		/*
		// 특정 시간 이후에 터트릴거고.
		// 이펙트는 제한 시간까지 재생한다.
		b.isCollisionCheckAtEndOnly = true;
		b.canCollide = true;
		b.limitTime = 0.5f;
		*/

		_v = mon.cTransformPosition;


		if(attr[2] == 0)
		{
			_v.x += (mon.isPlayerSide)?b.targetRange.Get():-b.targetRange.Get();
		}



		_v.z = 0.0f;
		_v.y = 0.0f;
		b.init(mon.isPlayerSide,bd,0.0f,_v,mon);//false, bd , false, bd.speed, 340, _v, mon);


		
		_q = b.bTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 90.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		b.bTransform.localRotation = _q;

		b.setTargetNum(9999);
		b.applyReinforceLevel = applyReinforceLevel;
		b.heroSkillLevel = heroSkillLevel;
		b.isEnabled = true;			

		b.setHitObject(2000,attr[0]);
	}	
	
	
	void shootPlayerBullet17(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();

		checkShotEffect(mon,Monster.setShootingHand(mon, mon.hitObject.height * 0.7f));

		// TYPE 17. 타임리밋	  데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격
		
		_tempF = (float)attr[6];
		_tempI  = (int)(_tempF * 0.01f); // delay
		int totalCount = (int)(_tempF - (_tempI * 100));
		float delay = (_tempF - totalCount)*0.001f;
		_v2 = mon.targetPosition;
		_v2.y = 1.0f;
		
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		
		for(int i = 0; i < totalCount; ++i)
		{
			Bullet b = GameManager.me.bulletManager.getBullet();		
			
			// TYPE 7. 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수   사선 각도    
			b.init(7, BulletMotionType.POSITION, 1, _skillData, delay * i);		
			_v = _v2 + Util.getPositionByAngleAndDistanceXZ(GameManager.inGameRandom.Range(0,360),((float)(attr[5] * GameManager.inGameRandom.Range(0,101)))*0.004f);//GameManager.getRandomNum()))*0.004f);
			if(_v.z > MapManager.top) _v.z = MapManager.top;
			else if(_v.z < MapManager.bottom) _v.z = MapManager.bottom;
			b.isCollisionCheckAtEndOnly = true;
			b.limitTime.Set( (float)attr[0]*0.001f );
			b.targetRange = ((float)attr[1]) * 0.5f;
			b.useRangeDamagePer.Set( true );
			b.minimumDamagePer = (float)attr[2] / 100.0f;//* 0.01f;
			b.setTargetNum(attr[3]);

			

#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.useEffectSkillCamEditor)
			{
				if(mon.isPlayerSide)
				{
					b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(180-attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
				}
				else
				{
					b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
				}
			}
			else
#endif
			{
				b.init(mon.isPlayerSide,bd,5,Util.getPositionByAngleAndDistance(180-attr[4],800.0f) + _v,mon);//false, bd , false, bd.speed, 340, _v, mon);
			}


			b.setPositionBullet(_v);
			b.applyReinforceLevel = applyReinforceLevel;
			b.heroSkillLevel = heroSkillLevel;
			b.isEnabled = true;			
			if(i > 0) b.setReadyAttachedParticleEffect(false, true, false);
		}		
	}	
	



	void shootPlayerBullet18(Monster mon, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_skillData != null) _skillData.playSkillSound();
		
		// TYPE 18. 직선발사 단일공격  : 최대비행거리	 비행속도	   탄환개수
		BulletPatternData bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(mon.nowBulletPatternId, out bpd);
		if(bpd == null) bpd = GameManager.info.bulletPatternData[EMPTY_BULLETDATA];

		BulletData bd = GameManager.info.bulletData[bpd.ids[0]]; 
		
		checkShotEffect(mon,_v);
		
		for(int i = 0; i < attr[2]; ++i)
		{
			Bullet b = GameManager.me.bulletManager.getBullet();
			b.init(18, BulletMotionType.CHASER, attr[2], _skillData, i * 0.15f - 0.01f);

			b.isDistanceLimitBullet = true;
			b.limitDistance = (float)attr[0];

			_v = Monster.setShootingHand(mon,mon.hitObject.height * 0.7f);

			if(mon.isPlayerSide)
			{
				_v.x -= mon.hitObject.width;
			}
			else
			{
				_v.x += mon.hitObject.width;
			}
			
			b.init(mon.isPlayerSide, bd , (float)attr[1], _v, mon);

			b.targetMonsterUniqueId = mon.targetUniqueId;
			
			if(_skillData != null && _skillData.isTargetingForward)
			{
				_v.x += ((mon.isPlayerSide)?1000.0f:-1000.0f);
			}
			else
			{
				_v = mon.targetPosition;
				_v.y = mon.targetHeight  * 0.5f;	
			}
			
			float gy = GameManager.inGameRandom.Range(0,3) * 100.0f + 200;
			
			b.setChaser(_v,gy,(i % 2 == 0));

			b.applyReinforceLevel = applyReinforceLevel;
			b.heroSkillLevel = heroSkillLevel;

			b.isEnabled = false;	
			b.setReadyAttachedParticleEffect(false);
			GameManager.me.bulletManager.delayBullets.Add(b);
		}
	}




	
	
	
	void playSkillRightNow(Player p, int heroSkillLevel, int applyReinforceLevel )
	{
		playerShoot(p, heroSkillLevel, applyReinforceLevel);
	}
	
	void playSkillLinear(Player p, int heroSkillLevel, int applyReinforceLevel )
	{
		p.startSkillAniLinear(playerShoot, heroSkillLevel, applyReinforceLevel);			
	}
	
	void playSkillNormal(Player p, int heroSkillLevel, int applyReinforceLevel )
	{
		p.startSkillAniNormal(playerShoot, heroSkillLevel, applyReinforceLevel);
	}
	
	void playSkillLoop(Player p, int heroSkillLevel, int applyReinforceLevel )
	{
		p.startSkillAniLoop(playerShoot, heroSkillLevel, applyReinforceLevel);
	}
	
	
	//=====================================



	private static IVector3 unitSkillTargetingPositionRandomPositionOffset = IVector3.zero;
	IVector3 getUnitSkillTargetingPositionRandomPositionOffset()
	{
		unitSkillTargetingPositionRandomPositionOffset.x = 0;
		unitSkillTargetingPositionRandomPositionOffset.z = 0;

		if(_skillData != null && _skillData.skillDataType == BaseSkillData.SkillDataType.Unit && _skillData.targeting == 5 && _skillData.targetAttr[1] > 0)
		{
			unitSkillTargetingPositionRandomPositionOffset.x = GameManager.inGameRandom.Range(-_skillData.targetAttr[1],_skillData.targetAttr[1]+1);
			unitSkillTargetingPositionRandomPositionOffset.z = GameManager.inGameRandom.Range(-_skillData.targetAttr[1],_skillData.targetAttr[1]+1);
		}
		
		return unitSkillTargetingPositionRandomPositionOffset;
	}



}