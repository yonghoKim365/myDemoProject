using System;

sealed public class PVPPlayerSkillSlot : SkillSlot
{
	public PVPPlayerSkillSlot ()
	{
	}

	// 그러니까 결론적으로 이 클래스는 히어로 몬스터가 쓰는 클래스란거지...
	public override bool canTargetingByHeroMonster(int target)
	{
		return skillData.heroMonsterTargetingChecker(mon, target);
	}
	
	sealed public override void canSkillMove()
	{
		skillData.exeData.skillMove(mon);
	}
	
	sealed public override void doSkill(int totalDamageNum)
	{
#if UNITY_EDITOR
		UnityEngine.Debug.LogError("=========== CHECK THIS! ==========");
#endif
		//Log.log(mon.resourceId + "  pvp doSkill");
//		skillData.exeData.monsterShoot(mon, totalDamageNum);		
	}

	public override void doAlternativeSkillAttack (int totalDamageNum)
	{
	}
	
	sealed public override string getBulletPatternId()
	{
		return skillData.baseId;
	}

	// =========== PVP 캐릭터 ============== //



	sealed public override bool canUse()
	{
	 	return (player.mp >= useMp && coolTime <= 0.0f && state != STATE_COOLTIME);// && player.globalSkillCooltime <= 0);
	}
	
	sealed public override void resetCoolTime ()
	{
		if(uiSlot != null)
		{
			uiSlot.resetCoolTime();
		}
		else
		{
//			if(GameManager.info.setupData.canUseSkillAndPetSlotAtStart == false)
			{
				if(maxCoolTime <= 0.1f)
				{
					state = STATE_READY;
					coolTime = 0.0f;
				}
				else
				{
					state = STATE_COOLTIME;
					coolTime = maxCoolTime;
				}
			}
//			else
//			{
//				state = STATE_READY;
//				coolTime = 0.0f;
//			}
		}
	}


	sealed public override void setData(Monster monster, GameIDData heroSkillInfo, UIPlaySkillSlot uiSlot = null)
	{
		base.setData(monster, heroSkillInfo, uiSlot);

		player = (Player)mon;
		_isPress = false;
		player.chargingTime = 0.0f;
		player.timeAfterFullCharging =0.0f;

		if( skillData.coolTime <= 0.1f)
		{
			maxCoolTime = 0.0f;
		}
		else
		{
			maxCoolTime = skillData.coolTime;	
		}

		#if UNITY_EDITOR			
		if(DebugManager.instance.ignoreCoolTime)
		{
			maxCoolTime = 0.3f;
		}
		#endif

		useMp = skillData.mp;
		useMp -= (useMp * player.skillSpDiscount(skillData));

		isB1_3Skill = false;//(skillData.baseId == "SK_B001" || skillData.baseId == "SK_B002" || skillData.baseId == "SK_B003");
		isB7_8Skill = (skillData.baseId == "SK_1202" || skillData.baseId == "SK_3202" || skillData.baseId == "SK_5201");

//		if(player.isSetAi)
//		{
//			runeSelectPoint = MathUtil.levelDifferencePoint(
//															MathUtil.abs(monster.stat.level - skillData.baseLevel), 
//															player.skillRuneSelectCalcValue.attr) 
//							*  player.skillRuneSelectRarePoint.attr[heroSkillInfo.rare];
//		}

		if(uiSlot != null)
		{
			uiSlot.init(this, player);
		}

		resetCoolTime();

	}

	public bool checkSkillTarget(bool checkTargetOnly)
	{
		return skillData.playerTargetingChecker(player, checkTargetOnly, player.targetingDecal);
	}

	bool _isPress = false;

	sealed public override void use()
	{
		onPress(true);
	}





	sealed public override void onPress(bool isPress)
	{
		if(uiSlot != null)
		{
//			UnityEngine.Debug.LogError("isPress : " + isPress);
			GameManager.replayManager.skillButtons[uiSlot.slotIndex] = true;
			return;
		}


		if(_isPress == false && isPress)
		{
			if(player.mp < useMp) return;
			
			// 스킬 동작 중에는 새로운 공격을 할 수 없다.
			if(player.nowPlayingSkillAni) return;
			
			if(checkSkillTarget(true))
			{
				state = STATE_PRESS;
				_isPress = true;
				/*
				 *  pvp 캐릭터는 데칼을 보여주지 않는다...
				 * 
				switch(skillData.targeting)
				{
				case TargetingData.NONE:
					break;
				case TargetingData.FIXED_1:
					// Decal half radius is 95px. targetAttr is radius.
					//player.targetingDecal.init(TargetingDecal.DecalType.Circle, (float)skillData.targetAttr[1] * 0.005f, player.isPlayerSide);
					break;
				case TargetingData.AUTOMATIC_2:
					//player.targetingDecal.init(TargetingDecal.DecalType.Circle, 1.0f,  player.isPlayerSide);
					break;
				case TargetingData.FORWARD_LINEAR_3:
					//player.targetingDecal.init(TargetingDecal.DecalType.Arrow, 1.0f,  player.isPlayerSide);
					break;				
				}
				*/

				player.skillMoveIsNormal = true;
				player.moveType = Player.MoveType.SKILL;
				player.skillModeProgressTime = 0.0f;

				player.startCharging(skillData, chargingTimeLimit, skillInfo);
			}
		}
		else if(_isPress && isPress == false)
		{
			_isPress = false;
			
			resetCoolTime();
			
			if(checkSkillTarget(false))
			{
				if(skillData.exeData.type == AttackData.ATTACH_BULLET_12)
				{
					player.hpWhenAttachSkillStart = player.hp;
					player.moveType = Player.MoveType.ATTACH_SKILL;
					player.skillMoveIsNormal = true;
				}
				else
				{
					player.moveType = Player.MoveType.NORMAL;
					player.skillMoveIsNormal = true;
				}

				player.mp -= useMp;				
				player.nowBulletPatternId = skillData.resource;

				skillData.exeData.playSkill(player, player.nowChargingSkillInfo.reinforceLevel + skillData.baseLevel, player.applyReinforceLevel);
			}
			else
			{
				player.moveType = Player.MoveType.NORMAL;
				player.skillMoveIsNormal = true;
			}

			// 플레이어에게 선택된 스킬 슬롯을 지울까 말까?...
			player.finishCharging();
		}
	}


	public override void cancelChargingAndClearAllCooltime()
	{
		_isPress = false;

		if(uiSlot != null)
		{
			uiSlot.resetCoolTime(true);
		}
		else
		{
			state = STATE_READY;
			coolTime = 0.0f;
		}

		player.finishCharging();

		if(uiSlot != null)
		{
			uiSlot.cancelChargingAndClearAllCooltime();
		}

	}

	

	public override void update ()
	{
		if(uiSlot != null)
		{
			uiSlot.update(true);
		}
		else
		{
			if(state == STATE_COOLTIME)
			{
				coolTime -= GameManager.globalDeltaTime;
				
				if(coolTime < 0.0f)
				{
					coolTime = 0.0f;
					state = STATE_READY;			
				}
			}
			else if(state == STATE_PRESS)
			{
				
				player.onCharging();
				player.targetingDecal.visible = checkSkillTarget(true);
			}
		}
	}

	
}

