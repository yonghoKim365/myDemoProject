using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class MonsterUnitAction : CharacterAction
{
	public MonsterUnitAction ()
	{
	}

	private Transform _effTransform;
	
	private CharacterManager cm;

	private bool _lastAttackIsNormalAttack = false;
	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		//setAttackDelay();
		nowUnitSkillData = null;
		canUsePassiveSkill = true;

		cm = GameManager.me.characterManager;

		//ff Log.log("mon action init : "+  "    " + mon.resourceId + "     mon.isPlayerSide : " + mon.isPlayerSide);

		_lastAttackIsNormalAttack = false;

		mon.unitData.attackType.init(AttackData.AttackerType.Unit,AttackData.AttackType.Attack);
	}

	private string _prevState;
	
	sealed public override void startAction ()
	{
		base.startAction ();
		mon.setPlayAniRightNow(Monster.NORMAL);
		
		// 처음 소환하고 대기시간.
		_firstDelay = 0.5f;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log("mon action startAction : " + mon.resourceId); //ff
		}
#endif
	}
	
	private float _firstDelay = 0.0f;

	bool _didSkill = false;

	sealed public override void doMotion()
	{

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log("domotion : "+  mon.resourceId); //ff
//			Log.log(_state + "    delay:" + _firstDelay + "     mon.state:"+mon.state + "   state:"+ _state); //ff
		}
#endif



		switch(_state)
		{
		case STATE_PREPARE:
			
			if(_firstDelay > 0)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
				_firstDelay -= GameManager.globalDeltaTime;
				return;
			}


			if(freezeTime > 0)
			{
				freezeTime -= GameManager.globalDeltaTime;
				return;
			}


			_didSkill = false;

			// 스킬이 있다면.. 스킬 검사...
			if(mon.skillSlots != null)
			{
				foreach(SkillSlot ss in mon.skillSlots)
				{
					if(ss.canUse())
					{
						// 스킬을 사용할 수 있다면...
						ss.doSkill(1);
						_didSkill = true;
					}
				}
			}

			
			if(_didSkill) return;


			#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker) return;
			#endif

			mon.unitData.attackType.chracterMove(mon);	
			break;
		
		case STATE_ACTION:

			// 스킬이 있다면.. 
			// 이 부분에서는 애니메이션이 있는 제 2의 스킬 동작을 사용한다.

			_didSkill = false;

			if(mon.skillSlots != null)
			{
				foreach(SkillSlot ss in mon.skillSlots)
				{
					if(ss.canUse())
					{
						// 스킬을 사용할 수 있다면...
						ss.doAlternativeSkillAttack(1);
						_didSkill = true;
					}
				}
			}			

			if(_didSkill) return;

#if UNITY_EDITOR			
			if(BattleSimulator.nowSimulation == false)
			{
//				Log.log("## STATE_ACTION!! : " + Monster.ATK_IDS[ mon.unitData.attackType.type-1 ] , mon.resourceId);// ff
			}
#endif			
			//일반 공격.
			setAttackDelay();
			nowUnitSkillData = null;

#if UNITY_EDITOR
			if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useEffectSkillCamEditor)
			{
				if(UnitSkillCamMaker.CURRENT_ANI_NAME != null && UnitSkillCamMaker.CURRENT_ANI_NAME.Length > 3)
				{
					mon.state = UnitSkillCamMaker.CURRENT_ANI_NAME; //Monster.SHOOT;
				}
				else
				{
					mon.state = Monster.ATK_IDS[ mon.unitData.attackType.type]; //Monster.SHOOT;
				}
			}
			else
			{
				mon.state = Monster.ATK_IDS[ mon.unitData.attackType.type]; //Monster.SHOOT;
			}
#else
			mon.state = Monster.ATK_IDS[ mon.unitData.attackType.type]; //Monster.SHOOT;
#endif

			_lastAttackIsNormalAttack = true;
			canUsePassiveSkill = true;
			_state = STATE_WAIT;				
			break;


		case STATE_WAIT:

			_didSkill = false;
			
			// 스킬이 있다면.. 스킬 검사...
			if(mon.skillSlots != null)
			{
				foreach(SkillSlot ss in mon.skillSlots)
				{
					if(ss.canUse() && ss.hasSkillAni == false)
					{
						// 스킬을 사용할 수 있다면...
						ss.doSkill(1);
						_didSkill = true;
					}
				}
			}

			if(_didSkill) return;

			break;
		}
	}


	void checkSkill()
	{

	}


	sealed public override void onAttack(int totalDamageNum, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log("unit on attack : " + nowUnitSkillData); //ff 
		}
#endif
		// 일반 공격일때.
		if(nowUnitSkillData == null)
		{
			mon.nowBulletPatternId = mon.bulletPatternId;
			mon.unitData.attackType.monsterShoot(mon, totalDamageNum, 1, 20, targetX, targetY, targetZ, targetH);
		}
		else
		{
			mon.nowBulletPatternId = nowUnitSkillData.getBulletPatternId();
			nowUnitSkillData.exeData.monsterShoot(mon, totalDamageNum, 1, 20, targetX, targetY, targetZ, targetH);		
		}

		canUsePassiveSkill = true;

		if(mon.skillSlots != null)
		{
			foreach(SkillSlot ss in mon.skillSlots)
			{
				if(ss.canUse())
				{
					// 스킬을 사용할 수 있다면...
					ss.doSkill(1);
					//break;
				}
			}
		}			
	}
	
	sealed public override void onCompleteAttackAni(bool isClearActionType = false)
	{
		//Debug.//LogError("### UNIT onCompleteAttackAni");
		_state = STATE_PREPARE;
		//Debug.//Log("Delay : " + delay);
//		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}


//		if(VersionData.codePatchVer >= 1)
		{
			// Attacked로 인해 기본 공격에 이어 발동되는 패시브 스킬이 있다.
			// 그런데 동작이 진행되는 도중 액티브 스킬을 쓰면 기본 공격이 끊기고 여기가 호출되는데
			// 순서) 기본공격 -> 액티브 스킬 -> oncompleteattack ani -> 액티브 스킬 발동.
			// 버그로 아래 순서가 씹히게 된다. 그래서 액티브 스킬이 발동되면 complete ani때는
			// attacked 패시브 스킬은 체크하면 안된다.

			if(isClearActionType)
			{
				_lastAttackIsNormalAttack = false;
				isAfterAttackFrame = false;
				return;
			}
		}


		if(_lastAttackIsNormalAttack)
		{
			_lastAttackIsNormalAttack = false;

			isAfterAttackFrame = true;
			
			if(mon.skillSlots != null)
			{
				foreach(SkillSlot ss in mon.skillSlots)
				{
					if(ss.canUse())
					{
						// 스킬을 사용할 수 있다면...
						ss.doSkill(1);
						//break;
					}
				}
			}
			
			isAfterAttackFrame = false;

		}
	}
	
	
	sealed public override void damage (int damageValue)
	{

		base.damage (damageValue);
		
		isDamageFrame = true;
		
		if(mon.skillSlots != null)
		{
			foreach(SkillSlot ss in mon.skillSlots)
			{
				if(ss.canUse())
				{
					// 스킬을 사용할 수 있다면...
					ss.doSkill(1);
					isDamageFrame = false;
					
					mon.attacker = null;
					mon.attackerUniqueId = -1;	
					//break;
				}
			}
		}		
		
		isDamageFrame = false;
		
	}
	
	
	sealed public override void dead ()
	{
		base.dead ();
		
		if(mon.skillSlots != null)
		{
			foreach(SkillSlot ss in mon.skillSlots)
			{
				if(ss.canUse())
				{
					// 스킬을 사용할 수 있다면...
					ss.doSkill(1);
					//break;
				}
			}
		}


		if(mon.unitData.attackType.type == AttackData.LAND_MINE_14)
		{
			mon.nowBulletPatternId = mon.bulletPatternId;
			//ff Log.log("AttackData.LAND_MINE_14 ");
			mon.unitData.attackType.monsterShoot(mon, 1);

			GameManager.info.effectData["E_METEOR_HIT"].getEffect(-1000,mon.cTransformPosition); 

		}

	}
	
}

