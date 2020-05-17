using System;
using UnityEngine;
using System.Collections.Generic;

sealed public partial class AttackData
{
	public const int NONE_0 = 0;
	public const int SHORT_1 = 1;
	public const int SHORT_RANGE_2 = 2;
	public const int LINE_3 = 3;
	public const int LINE_RANGE_4 = 4;
	public const int LINE_PASS_5 = 5;
	public const int CURVE_RANGE_6 = 6;
	public const int DROP_RANGE_7 = 7;
	public const int CIRCLE_RANGE_8 = 8;
	public const int FIXED_EFFECT_9 = 9;
	public const int CONTINUE_FIXED_EFFECT_10 = 10;
	public const int CURVE_AFTER_CONTINUE_FIXED_EFFECT_11 = 11;
	public const int ATTACH_BULLET_12 = 12;
	public const int TIME_BOMB_13 = 13;
	public const int LAND_MINE_14 = 14;
	
	public const int CHAIN_LIGHTING_15 = 15;
	public const int SCREEN_ATTACK_16 = 16;
	public const int METHEO_17 = 17;

	public const int CHASER_18 = 18;

	
	
	public Xint[] attrOriginal;
	public Xint[] attr;
	
	public int type;
	


	public bool isShortType = false;


	public AttackData ()
	{
	}
	
	public static AttackData getAttackData(int type, params object[] args)
	{
		// 6~11번은 최대 타겟 거리가 추가됐다. 
		// 최대 타겟 거리는 타겟팅 타입 2번에 사용된다.
		// 타겟팅 타입 2번은 가장 가까운 적 혹은 에너지가 작은 적을 뜻하는데
		// 얘가 나보다 타겟 거리보다 멀면 타겟팅을 할 수 없다.
		
		AttackData ad = new AttackData();
		
		ad.type = type;
		
		int len = 0;
		switch(type)
		{
		case SHORT_1: //1
			len = 0;		
			ad.isShortType = true;
			break; 
		case SHORT_RANGE_2:  //2
			len = 3; //데미지범위	최소데미지비율	최대타겟유닛수
			ad.isShortType = true;
			break;
		case LINE_3:  // 3
			len = 4; //최대비행거리	비행속도   타입 1이면 그쪽 방향으로...
			break; 
		case LINE_RANGE_4: // 4
			len = 5; //최대비행거리	비행속도	데미지범위	최소데미지비율	최대타겟유닛수
			break;
		case LINE_PASS_5:  //5
			len = 7; //최대비행거리	비행속도	최대타겟유닛수	이펙트타입	이펙트Z/R  // 방향틀기 거리 추가.
			break;
		case CURVE_RANGE_6:  //6
			len = 5; //타임리밋	데미지범위	최소데미지비율	최대타겟유닛수  // 최대 타겟거리 추가.
			break;
		case DROP_RANGE_7: // 7
			len = 6; //타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
			break;
		case CIRCLE_RANGE_8: // 8
			len = 4; //데미지범위	최소데미지비율	최대타겟유닛수
			break;
		case FIXED_EFFECT_9: // 9
			len = 5; //최대타겟유닛수	이펙트타입	이펙트Z/R	이펙트X
			break;
			// 10
		case CONTINUE_FIXED_EFFECT_10: //최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			len = 6;
			break;
			//11
		case CURVE_AFTER_CONTINUE_FIXED_EFFECT_11: //타임리밋	최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			len = 7;
			break;
			//
		case ATTACH_BULLET_12: // 12
			len = 5; // 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			break;
		case TIME_BOMB_13: // 13
			len = 4; //폭발대기시간	  데미지범위	최소데미지비율	최대타겟유닛수
			break;
		case LAND_MINE_14: //14
			len = 4; //데미지범위	  최소데미지비율	최대타겟유닛수  자연폭발시간
			break;
			
			//15 
		case CHAIN_LIGHTING_15:  //최대전체거리	최대연결거리A	최대연결거리B	최대연결유닛수	연결딜레이
			len = 6;			
			break;
			
		case SCREEN_ATTACK_16: // 최대데미지거리
			len = 3;
			break;
			
		case METHEO_17: // 타임리밋	데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격
			len = 7;
			break;

		case CHASER_18: // 최대비행거리	비행속도	탄환개수
			len = 3;
			break;
		}
		
		ad.attrOriginal = new Xint[len];
		ad.attr = new Xint[len];
		for(int i = 0; i < len; ++i)
		{
			int v = 0;
			Util.parseObject(args[i],out v, true, 0);	
			ad.attrOriginal[i] = v;
			ad.attr[i] = v;
		}
		
		return ad;
	}


	public AttackData clone()
	{
		AttackData ad = new AttackData();

		ad.type = this.type;

		ad.attrOriginal = new Xint[this.attrOriginal.Length];
		ad.attr = new Xint[ad.attrOriginal.Length];

		for(int i = this.attrOriginal.Length - 1; i >= 0; --i)
		{
			ad.attrOriginal[i] = this.attrOriginal[i];
			ad.attr[i] = this.attrOriginal[i];
		}

		return ad;
	}



	
	public enum AttackerType
	{
		Hero, Unit
	}
	
	public enum AttackType
	{
		Skill, Attack
	}


	public delegate void MonsterShoot(Monster mon, int totalDamageNum, int heroSkillLevel = 1, int applyInforceLevel = 20, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000);
	public MonsterShoot monsterShoot;
	
	public delegate void PlayerShoot(Player p, int heroSkillLevel, int applyInforceLevel);
	public PlayerShoot playerShoot;	
	
	public delegate void PlayPlayerSkill(Player p, int heroSkillLevel, int applyInforceLevel);
	public PlayPlayerSkill playSkill;		
	
	public delegate void SetBulletData(Monster mon);
	public SetBulletData setBulletData;
	
	public delegate float PreDamageCalc(int heroSkillLevel, int applyInforceLevel, Monster attacker, Monster target);
	public PreDamageCalc preDamageCalc;

	BaseSkillData _skillData = null;
	TranscendData _transData = null;


	public Xint maxHit = 99999;
	public Xfloat targetRange = 0.0f;

	private CharacterManager cm;

	private static int[] ATTR_STR = new int[]{ WSATTR.ATK_ATTR1_I,WSATTR.ATK_ATTR2_I,WSATTR.ATK_ATTR3_I,WSATTR.ATK_ATTR4_I,WSATTR.ATK_ATTR5_I,WSATTR.ATK_ATTR6_I,WSATTR.ATK_ATTR7_I};
	private static int[] E_ATTR_STR = new int[]{ WSATTR.E_ATTR1_I,WSATTR.E_ATTR2_I,WSATTR.E_ATTR3_I,WSATTR.E_ATTR4_I,WSATTR.E_ATTR5_I,WSATTR.E_ATTR6_I,WSATTR.E_ATTR7_I};

	public void init(AttackerType attackerType, AttackType aType, BaseSkillData hsd = null, TranscendData td = null, int[] transcendLevel = null, bool isFromHeroSkill = false)
	{
		//Log.log("init attackdata!!!");
		if(td != null)
		{
			for(int i = attrOriginal.Length - 1; i >= 0; --i)
			{
				if( isFromHeroSkill )
				{
					attr[i] = td.getValueByATTR(transcendLevel, attrOriginal[i].Get(), E_ATTR_STR[i]);

				}
				else
				{
					attr[i] = td.getValueByATTR(transcendLevel, attrOriginal[i].Get(), ATTR_STR[i]);
				}
			}
		}

		cm = GameManager.me.characterManager;
		
		_skillData = hsd;
		
		switch(type)
		{
			
		case NONE_0:
			chracterMove = moveStay;					
			
			monsterShoot = shootMonster0;
			playerShoot = shootPlayer0;
			playSkill = playSkillRightNow;	
			preDamageCalc = damageCalc0;


			if(_skillData.skillDataType == BaseSkillData.SkillDataType.Hero && _skillData.targeting == 1)
			{
				targetRange = _skillData.targetAttr[1];
			}

			break;
			
		case SHORT_1: //1
			//len = 1;	
			chracterMove = moveShortAttackType;	
			
			if(aType == AttackType.Attack)
			{
				monsterShoot = shootMonster1;
				playerShoot = shootPlayer1;
				preDamageCalc = damageCalc0;

			}			
			
			playSkill = playSkillNormal;
			
			break;
		case SHORT_RANGE_2:  //2
			//len = 3; //데미지범위	최소데미지비율	최대타겟유닛수

			chracterMove = moveShortAttackType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonster2;
				playerShoot = shootPlayer2;
				preDamageCalc = damageCalc0;
			}			
			
			playSkill = playSkillNormal;
			
			break;
		case LINE_3: //3
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet3;
				playerShoot = shootPlayerBullet3;
				preDamageCalc = damageCalc0;
			}
			
			playSkill = playSkillLinear;
			
			break;
			
		case LINE_RANGE_4: //4
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet4;
				playerShoot = shootPlayerBullet4;
				preDamageCalc = damageCalc0;
			}
			
			playSkill = playSkillLinear;
			
			break;
			
			
		case LINE_PASS_5: //5
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				targetRange = attr[4];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet5;
				playerShoot = shootPlayerBullet5;
				preDamageCalc = damageCalc0;
			}
			
			playSkill = playSkillLinear;
			
			break;
		case CURVE_RANGE_6: //6
			//len = 4; //타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				targetRange = attr[1];
				maxHit = attr[3];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet6;
				playerShoot = shootPlayerBullet6;
				preDamageCalc = damageCalc0;
			}			
			
			playSkill = playSkillNormal;

			break;
		case DROP_RANGE_7: //7
			//len = 4; //타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;

				targetRange = attr[1];
				maxHit = attr[3];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet7;
				playerShoot = shootPlayerBullet7;
				preDamageCalc = damageCalc0;
			}				
			
			playSkill = playSkillNormal;

			break;
		case CIRCLE_RANGE_8: 
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				targetRange = attr[0];
				maxHit = attr[2];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet8;
				playerShoot = shootPlayerBullet8;
				preDamageCalc = damageCalc0;
			}				
			
			playSkill = playSkillNormal;

			//len = 3; //데미지범위	최소데미지비율	최대타겟유닛수
			break;
		case FIXED_EFFECT_9: 
			//len = 4; //최대타겟유닛수	이펙트타입	이펙트Z/R	이펙트X
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;

				// 11~12 사각기둥. 21~22. 원기둥.
				if(attr[1] > 12) targetRange = attr[2];
				else targetRange = attr[3];

				maxHit = attr[0];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet9;
				playerShoot = shootPlayerBullet9;
				preDamageCalc = damageCalc0;
			}		
			
			playSkill = playSkillNormal;

			
			break;
		case CONTINUE_FIXED_EFFECT_10: //최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			//len = 5;
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				
				// 11~12 사각기둥. 21~22. 원기둥.
				if(attr[2] > 12) targetRange = attr[3];
				else targetRange = attr[4];


				maxHit = attr[0];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet10;
				playerShoot = shootPlayerBullet10;
				preDamageCalc = damageCalc0;
			}		
			
			playSkill = playSkillNormal;



			
			break;
		case CURVE_AFTER_CONTINUE_FIXED_EFFECT_11: //타임리밋	최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			//len = 6;
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				
				// 11~12 사각기둥. 21~22. 원기둥.
				if(attr[3] > 12) targetRange = attr[4];
				else targetRange = attr[5];

				maxHit = attr[1];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet11;
				playerShoot = shootPlayerBullet11;
				preDamageCalc = damageCalc0;
			}		
			
			playSkill = playSkillNormal;



			break;
		case ATTACH_BULLET_12: 
			//len = 5; // 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet12;
				playerShoot = shootPlayerBullet12;
				preDamageCalc = damageCalc0;
			}		
			
			playSkill = playSkillLoop;
			
			break;
		case TIME_BOMB_13: //13

#if UNITY_EDITOR
			Debug.Log("==== 안쓰는 것!!!! ====!!!!");
#endif

			//len = 4; //폭발대기시간	  데미지범위	최소데미지비율	최대타겟유닛수		
			if(attackerType == AttackerType.Hero)
			{
				chracterMove = moveLineType;	
				skillMove = moveSkillHeroMonsterLineType;
			}
			else if(attackerType == AttackerType.Unit)
			{
				chracterMove = moveLineType;	
			}		
			
			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet12;
				playerShoot = shootPlayerBullet12;
			}		
			
			playSkill = playSkillNormal;
			
			break;
		case LAND_MINE_14: //14
			
			//len = 4; //데미지범위	최소데미지비율	최대타겟유닛수  자연폭발시간
			if(attackerType == AttackerType.Hero)
			{
				chracterMove = moveLandMineType;//moveLandMineAttackType;	
				skillMove = moveLandMineType;//moveLandMineAttackType;
			}
			else if(attackerType == AttackerType.Unit)
			{
				chracterMove = moveLandMineType;//moveLandMineAttackType;
			}			
			
			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet14;
				playerShoot = shootPlayerBullet14;
				preDamageCalc = damageCalc0;
			}			
			
			playSkill = playSkillNormal;
			
			break;
			
		case CHAIN_LIGHTING_15 :  //15
			
//			Debug.Log("==== 수정해야함 ====!!!!");
			
			//len = 3; //최대전체거리	최대연결거리A	최대연결거리B	최대연결유닛수	연결딜레이
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet15;
				playerShoot = shootPlayerBullet15;
				preDamageCalc = damageCalc0;
			}		
			
			playSkill = playSkillLoop;
			
			break;
			
		
		case SCREEN_ATTACK_16:
			
			// 이동은 수정해야함. 그런데 굳이 할 필요는 없을듯???
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				targetRange = attr[0];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet16;
				playerShoot = shootPlayerBullet16;
				preDamageCalc = damageCalc0;
			}				
			
			playSkill = playSkillNormal;



			break;
		case METHEO_17: //17
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
				targetRange = attr[5];
				maxHit = attr[3];
			}

			chracterMove = moveLineType;	

			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet17;
				playerShoot = shootPlayerBullet17;
				preDamageCalc = damageCalc0;
			}				
			
			playSkill = playSkillNormal;

			break;	


		case CHASER_18: //18
			
			if(attackerType == AttackerType.Hero)
			{
				skillMove = moveSkillHeroMonsterLineType;
			}
			
			chracterMove = moveLineType;	
			
			if(aType == AttackType.Attack || aType == AttackType.Skill)
			{
				monsterShoot = shootMonsterBullet18;
				playerShoot = shootPlayerBullet18;
				preDamageCalc = damageCalc0;
			}
			
			playSkill = playSkillLinear;
			
			break;




		}	
	}
	
	

	
	



	public bool checkingDefaultAttackRange(Monster attacker, Monster target, float offset = 0.0f)
	{
		if(attacker.isPlayerSide == false)
		{
			if(isShortType)
			{
				return ( Xfloat.lessEqualThan( VectorUtil.DistanceXZ(target.cTransformPosition , attacker.cTransformPosition) , attacker.stat.atkRange  + target.damageRange + offset + attacker.damageRange) ) ; // hitrange
			}
			else
			{
				return ( Xfloat.lessEqualThan( attacker.cTransformPosition.x - attacker.stat.atkRange - offset - attacker.damageRange  , target.lineRight ));
			}

		}
		else
		{
			if(isShortType)
			{
				return ( Xfloat.lessEqualThan( VectorUtil.DistanceXZ(target.cTransformPosition , attacker.cTransformPosition) , attacker.stat.atkRange  + target.damageRange + offset + attacker.damageRange )); // hitrange
			}
			else
			{
				return ( Xfloat.greatEqualThan( attacker.cTransformPosition.x + attacker.stat.atkRange + offset + attacker.damageRange ,  target.lineLeft ) );
			}

		}
	}


	public bool checkingDefaultAttackRange(Monster attacker, Monster target, IVector3 targetPosition, float offset = 0.0f)
	{
			if(attacker.isPlayerSide == false)
			{
				if(isShortType)
				{
					return ( VectorUtil.DistanceXZ(targetPosition , attacker.cTransformPosition) <= (IFloat)( attacker.stat.atkRange  + target.damageRange + offset + attacker.damageRange ) ); // hitrange
				}
				else
				{
					return ( Xfloat.lessEqualThan( attacker.cTransformPosition.x - attacker.stat.atkRange - offset - attacker.damageRange , targetPosition.x + target.hitObject.lineRight  ));
				}
				
			}
			else
			{
				if(isShortType)
				{
					return ( Xfloat.lessEqualThan(  VectorUtil.DistanceXZ(targetPosition , attacker.cTransformPosition) , attacker.stat.atkRange  + target.damageRange + offset + attacker.damageRange )); // hitrange
				}
				else
				{
					return ( Xfloat.greatEqualThan(  attacker.cTransformPosition.x + attacker.stat.atkRange + offset + attacker.damageRange , target.hitObject.lineLeft + targetPosition.x ) );
				}
			}
	}

}





