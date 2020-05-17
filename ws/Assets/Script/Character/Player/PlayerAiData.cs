using System;
using UnityEngine;
using System.Collections.Generic;

sealed public partial class PlayerAiData : BaseData
{
	public string id;

	public int value;
	public int[] attr;

	public enum Type
	{
		dangerPoint,
		battleState,
		battleWait,
		battleOn,
		battleOff,
		safeZone,
		attachSkillMove,
		unitRuneSelectPoint,
		unitPoint,
		skillRuneSelectPoint,
		skillPoint,
		targetZonePoint,
		targetZonePoint2,
		targetZoneNvalue,
		unitActiveSkill,
		changePlayer
	}

	public Type type;
	public int type2;

	const string DP = "DP";
	const string STATE = "STATE";
	const string WAIT = "WAIT";
	const string BATTLE_ON = "BATTLE_ON";
	const string BATTLE_OFF = "BATTLE_OFF";
	const string SAFEZONE = "SAFEZONE";
	const string AS_MOVE = "AS_MOVE";
	const string U_RUNE_POINT = "U_RUNE_POINT";
	const string UNIT_POINT = "UNIT_POINT";
	const string S_RUNE_POINT = "S_RUNE_POINT";
	const string SKILL_POINT = "SKILL_POINT";
	const string T_POINT = "T_POINT";
	const string T_POINT2 = "T_POINT2";
	const string T_N_POINT = "T_N_POINT";

	const string USKILL = "USKILL";

	const string CHANGE_PLAYER = "CHANGE_PLAYER";

	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];

		object obj = l[k["DATA"]];

		if(obj is string)
		{
			string str = (string)obj;
			if(str.Contains(","))
			{
				attr = Util.stringToIntArray(str,',');
			}
			else
			{
				int.TryParse(str, out value);
			}
		}
		else 
		{
			Util.parseObject(obj, out value);
		}

		Util.parseObject(l[k["TYPE2"]],out type2, true, 0);

//		Debug.LogError(id + "   " + (string)l[k["TYPE1"]] + "  " + type2 + "   " + obj + "   " + attr + "    " +value);

		switch((string)l[k["TYPE1"]])
		{
		case DP:
			type = Type.dangerPoint;
			switch(type2)
			{
			case 0: getDangerPoint = getDp0; break;
			case 1: getDangerPoint = getDp1; break;
			case 2: getDangerPoint = getDp2; break;
			case 3: getDangerPoint = getDp3; break;
			case 4: getDangerPoint = getDp4; break;
			case 5: getDangerPoint = getDp5; break;
			case 6: getDangerPoint = getDp6; break;
			case 7: getDangerPoint = getDp7; break;
			case 8: getDangerPoint = getDp8; break;
			}
			break;
		case STATE:
			type = Type.battleState;
			break;
		case WAIT:
			type = Type.battleWait;

			switch(type2)
			{
			case 0: battleWaitMove = readyModeMove0; break;
			case 1: battleWaitMove = readyModeMove1; break;
			case 2: battleWaitMove = readyModeMove2; break;
			}

			break;
		case BATTLE_ON:
			type = Type.battleOn;
			switch(type2)
			{
			case 0: checkDefaultAttackOnOff = defaultAttackOnChecker0; break;
			case 1: checkDefaultAttackOnOff = defaultAttackOnChecker1; break;
			case 2: checkDefaultAttackOnOff = defaultAttackOnChecker2; break;
			case 3: checkDefaultAttackOnOff = defaultAttackOnChecker3; break;

			}
			break;
		case BATTLE_OFF:
			type = Type.battleOff;
			switch(type2)
			{
			case 0: checkDefaultAttackOnOff = defaultAttackOffChecker0; break;
			case 1: checkDefaultAttackOnOff = defaultAttackOffChecker1; break;
			case 2: checkDefaultAttackOnOff = defaultAttackOffChecker2; break;
			case 3: checkDefaultAttackOnOff = defaultAttackOffChecker3; break;
			}
			break;
		case SAFEZONE:
			type = Type.safeZone;
			break;
		case AS_MOVE:
			type = Type.attachSkillMove;

			switch(type2)
			{
			case 0:checkAttachSkillMove = attachSkillMove0;break;
			case 1:checkAttachSkillMove = attachSkillMove1;break;
			case 2:checkAttachSkillMove = attachSkillMove2;break;
			case 3:checkAttachSkillMove = attachSkillMove3;break;
			}

			break;
		case U_RUNE_POINT:
			type = Type.unitRuneSelectPoint;
			break;
		case UNIT_POINT:
			type = Type.unitPoint;
			switch(type2)
			{
			case 0:getUnitRunePoint = getUnitRunePoint0;break;
			case 1:getUnitRunePoint = getUnitRunePoint1;break;
			case 2:getUnitRunePoint = getUnitRunePoint2;break;
			case 3:getUnitRunePoint = getUnitRunePoint3;break;
			}
			break;
		case S_RUNE_POINT:
			type = Type.skillRuneSelectPoint;
			break;
		case SKILL_POINT:
			type = Type.skillPoint;
			switch(type2)
			{
			case 0:getSkillRunePoint = getSkillRunePoint0;break;
			case 1:getSkillRunePoint = getSkillRunePoint1;break;
			case 2:getSkillRunePoint = getSkillRunePoint2;break;
			case 3:getSkillRunePoint = getSkillRunePoint3;break;
			case 4:getSkillRunePoint = getSkillRunePoint4;break;
			case 5:getSkillRunePoint = getSkillRunePoint5;break;
			case 6:getSkillRunePoint = getSkillRunePoint6;break;
			case 7:getSkillRunePoint = getSkillRunePoint7;break;
			}
			break;
		case T_POINT:
			type = Type.targetZonePoint;
			break;
		case T_POINT2:
			type = Type.targetZonePoint2;
			break;
		case T_N_POINT:
			type = Type.targetZoneNvalue;
			break;
		case USKILL:
			type = Type.unitActiveSkill;
			break;
		case CHANGE_PLAYER:
			type = Type.changePlayer;
			break;
		}
	}

	public delegate IFloat DangerPointChecker(Player player, IVector3 checkPosition);
	public DangerPointChecker getDangerPoint;

	public delegate bool CheckWithPlayer(Player player);
	public CheckWithPlayer checkDefaultAttackOnOff;
	public CheckWithPlayer battleWaitMove;


	public delegate IFloat UnitRunePointChecker(Player player, UnitSlot slot);
	public UnitRunePointChecker getUnitRunePoint;

	public delegate IFloat SkillRunePointChecker(Player player, SkillSlot slot);
	public SkillRunePointChecker getSkillRunePoint;

	public delegate bool CheckAttachedSkillMove(Player player, Player enemy);
	public CheckAttachedSkillMove checkAttachSkillMove;

	bool attachSkillMove0(Player player, Player enemy)
	{
		// "HP 10%미만 & 적 히어로가 공격스킬 차징중 & 타게팅타입 1번" 타게팅 벗어날 때까지 후진		
		if(player.hpPer * 100.0f < value
		   && enemy.nowChargingSkill != null 
		   && enemy.nowChargingSkill.skillType == Skill.Type.ATTACK  
		   && enemy.nowChargingSkill.targeting == 1)
		{
			if(enemy.nowChargingSkill.playerHeroTargetingPVPModeChecker(player,  player.cTransformPosition))
			{
				player.moveBackward();
			}
			else
			{
				player.state = Monster.NORMAL;
				player.setMovingDirection(Monster.MoveState.Stop, true);	
			}

			return true;
		}

		return false;
	}

	// 좀 더 수정 필요.
	bool attachSkillMove1(Player player, Player enemy)
	{
		//"HP 20%미만 & 적유닛에게 데미지를 1회라도 입은경우"				타겟존 -1m 지점으로 이동	
		if(player.hpPer * 100.0f < attr[0] && player.hp < player.hpWhenAttachSkillStart)
		{
			_v = player.cTransformPosition;

			if(player.isPlayerSide)
			{
				_v.x = GameManager.me.characterManager.monsterLeftLine - attr[1] - player.damageRange;
			}
			else
			{
				_v.x = GameManager.me.characterManager.playerMonsterRightLine + attr[1] + player.damageRange;
			}

			player.setStateAndDirectionByTargetPosition(_v);

			/*
			if(player.isPlayerSide && player.lineRight >= GameManager.me.characterManager.monsterLeftLine - attr[1])
			{
				player.moveBackward();
			}
			else if(player.isPlayerSide == false && player.lineLeft <= GameManager.me.characterManager.playerMonsterRightLine + attr[1])
			{
				player.moveBackward();
			}
			else
			{
				player.state = Monster.NORMAL;
				player.setMovingDirection(Monster.MoveState.Stop, true);	
			}
			*/


			return true;
		}
		return false;
	}

	bool attachSkillMove2(Player player, Player enemy)
	{
		//그외 (아군 유닛이 1개이상 존재)				최전방위치(타겟존 - 50 cm) 유지		

		_v = player.cTransformPosition;
//		_v.z = 0.0f;
		if(player.isPlayerSide)
		{
			if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum > 0)
			{
				_v.x = GameManager.me.characterManager.monsterLeftLine - value - player.damageRange;
				player.setStateAndDirectionByTargetPosition(_v);

//				if(player.lineRight >= GameManager.me.characterManager.monsterLeftLine - value)
//				{
//					player.moveBackward();
//				}
//				else
//				{
//					player.state = Monster.NORMAL;
//					player.setMovingDirection(Monster.MoveState.Stop, true);	
//				}

				return true;
			}
		}
		else
		{
			if(GameManager.me.characterManager.totalAliveMonsterUnitNum > 0)
			{
				_v.x = GameManager.me.characterManager.playerMonsterRightLine + value + player.damageRange;
				player.setStateAndDirectionByTargetPosition(_v);
//				if(player.lineLeft <= GameManager.me.characterManager.playerMonsterRightLine + value)
//				{
//					player.moveBackward();
//				}
//				else
//				{
//					player.state = Monster.NORMAL;
//					player.setMovingDirection(Monster.MoveState.Stop, true);	
//				}


				return true;
			}
		}

		return false;

	}

	bool attachSkillMove3(Player player, Player enemy)
	{
		//그외 (아군 유닛이 없을 때)				적 타겟존 지점 - 100 cm 유지		
		_v = player.cTransformPosition;
//		_v.z = 0.0f;
		if(player.isPlayerSide)
		{
			if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum == 0)
			{
				_v.x = GameManager.me.characterManager.monsterLeftLine - value - player.damageRange;
				player.setStateAndDirectionByTargetPosition(_v);

//				if(player.lineRight >= GameManager.me.characterManager.monsterLeftLine - value)
//				{
//					player.moveBackward();
//				}
//				else
//				{
//					player.state = Monster.NORMAL;
//					player.setMovingDirection(Monster.MoveState.Stop, true);	
//				}

				return true;
			}
		}
		else
		{
			if(GameManager.me.characterManager.totalAliveMonsterUnitNum == 0)
			{
				_v.x = GameManager.me.characterManager.playerMonsterRightLine + value + player.damageRange;
				player.setStateAndDirectionByTargetPosition(_v);

//				if(player.lineLeft <= GameManager.me.characterManager.playerMonsterRightLine + value)
//				{
//					player.moveBackward();
//				}
//				else
//				{
//					player.state = Monster.NORMAL;
//					player.setMovingDirection(Monster.MoveState.Stop, true);	
//				}

				return true;
			}
		}


		return false;
	}



	int index = 0;
	IFloat _result;
	IVector3 _v;
	IVector3 _originalCtransformPosition;
	IFloat _tempDamage;
	IFloat _selectDamage;

	Player _targetPlayer = null;
	List<Monster> _monsters = null;
	HeroSkillData _readySkill = null;

//=========== [위치별 위험도 점수 계산] ==============================
	//히어로의 현재위치 및 최전방부터 적정지점 (타겟존에서 최대 10미터 떨어진 곳)까지 50센치 간격으로 각각 점수를 매김
	//특정 위치 에서의 상황별 위험도 점수

	IFloat getDp0(Player player, IVector3 checkPosition)
	{
		//해당 위치가 적 유닛들의 기본공격 데미지 범위 (공격거리+데미지범위) 안에 들어오는지
		// 기본 공격 위치를 offset으로 체크할지 아예 존 위치로 체크할지는 확인이 필요하다.
		_v = checkPosition;
		_result = 0;

//		int unitCount = 0;

		if(player.isPlayerSide)
		{
			_monsters = GameManager.me.characterManager.monsters;
//			unitCount = GameManager.me.characterManager.totalAliveMonsterUnitNum;
		}
		else
		{
			_monsters = GameManager.me.characterManager.playerMonster;
		}

		int len = _monsters.Count;

		int count = 0;

		for(int i = 0; i < len; ++i)
		{
			if(_monsters[i].unitData != null) 
			{
				if(_monsters[i].unitData.attackType.checkingDefaultAttackRange(_monsters[i], player, _v))
				{
					++count;
					break;
				}
			}
			else if(_monsters[i].isPlayer)
			{
				if(_monsters[i].attackData.checkingDefaultAttackRange(_monsters[i], player, _v))
				{
					++count;
					break;
				}
			}
			else //if(VersionData.codePatchVer >= 4)
			{
				if(_monsters[i].heroMonsterData != null && _monsters[i].heroMonsterData.attackType.checkingDefaultAttackRange(_monsters[i], player, _v, _monsters[i].stat.atkRange.Get() * 0.5f))
				{
					++count;
					break;
				}
			}
		}


		_result = count * value;

//		Debug.Log("dl0 : " + _result);

		_monsters = null;
		return _result;
	}


	IFloat getDp1(Player player, IVector3 checkPosition)
	{
		//해당 위치가 상대히어로의 공격스킬 사정권에 들어오는지

		_v = checkPosition;

		_tempDamage = 0.0f;
		_selectDamage = 0.0f;
		_result = 0;

		if(player.isPlayerSide) _targetPlayer = GameManager.me.pvpPlayer;
		else _targetPlayer = GameManager.me.player;

		if(_targetPlayer == null) return 0;

//		상대가 즉시 공격가능한 모든 스킬 에 대하여,
//		해당 위치에서 맞았을 때의 차감 데미지 구하기,
//		(0단계 차징 적용, 전투공식 및 최소데미지비율적용)
//		그중 가장 높은 데미지를 주는 값을 내 전체 HP로 나누어
//		0~1 사이의 비율값을 구함
		for(int i = 0; i < _targetPlayer.pvpPlayerAttackSkillDataLen; ++i)
		{
			index = _targetPlayer.pvpPlayerAttackSkillSlotIndex[i];
			
			// 즉시 쓸 수 있고.
			if(_targetPlayer.skillSlots[index].canUse())
			{
				// 타게팅도 가능하면 데미지를 구해본다.
				if(_targetPlayer.skillSlots[index].skillData.playerHeroTargetingPVPModeChecker(player, checkPosition))
				{
					// 0 단계 차징이다...
					_tempDamage = _targetPlayer.skillSlots[index].skillData.preDamageCalc(player,_targetPlayer.skillSlots[index].skillInfo.reinforceLevel + _targetPlayer.skillSlots[index].skillData.baseLevel ,0,_targetPlayer);
					if(_tempDamage > _selectDamage)
					{
						_selectDamage = _tempDamage;
						_readySkill = _targetPlayer.skillSlots[index].skillData;
					}
				}
			}
		}
		
		if(_targetPlayer.nowChargingSkill == null || _targetPlayer.nowChargingSkill == _readySkill)
		{
			if(_selectDamage > 0)
			{
				//Debug.Log("B 그 스킬이 데미지도 주네? : " + (_selectDamage / (float)_hp) * (float)SetupData.bMovePoint2) ;
				_result = player.getDamagePer(_selectDamage) * (float)value;
			}
		}

		_targetPlayer = null;
		_readySkill = null;

//		Debug.Log("dp1 : " + _result);
		return _result;		
	}


	//상대가 공격스킬을 차징중이고 해당위치가 사정권에 든 상태 
	
	//		[예상 데미지 비율]
	//		
	//		상대가 현재 시전하고 있는 스킬 에 대하여,
	//		현재 위치에서 맞았을 때의 차감 데미지 구하기,
	//		(풀 차징 적용, 전투공식 및 최소데미지비율적용)
	//			데미지 값을 내 전체 HP로 나누어
	//				0~1 사이의 비율값을 구함
	//				
	//				[N% 확률로]
	//				실제로는 상대방이 무슨스킬을 사용할지 모르기 때문에, 
	//				가끔씩은 맞도록 하기 위해 확률을 적용


	IFloat getDp2(Player player, IVector3 checkPosition)
	{
		if(player.isPlayerSide) _targetPlayer = GameManager.me.pvpPlayer;
		else _targetPlayer = GameManager.me.player;

		_result = 0;

		if(_targetPlayer == null) return 0;

		if(_targetPlayer.nowChargingSkill != null)
		{
			if(_targetPlayer.nowChargingSkill.skillType == Skill.Type.ATTACK)
			{
				if(_targetPlayer.nowChargingSkill.playerHeroTargetingPVPModeChecker(player,checkPosition))
				{
					// 풀차징 적용.
					_tempDamage = _targetPlayer.nowChargingSkill.preDamageCalc(player, _targetPlayer.nowChargingSkillInfo.reinforceLevel + _targetPlayer.nowChargingSkill.baseLevel, 20, _targetPlayer);
//											Debug.LogError("== 4 차징:사정권!: " + GameManager.me.player.nowChargingSkill.id + "    _tempDamage: " + _tempDamage);

					if(attr[0] > GameManager.inGameRandom.Range(0,100))
					{
						_result = player.getDamagePer(_tempDamage) * (float)attr[1];
					}
				}
			}
		}

//		Debug.Log("dp2 : " + _result);
		_targetPlayer = null;
		return _result;		
	}

	IFloat getDp3(Player player, IVector3 checkPosition)
	{
		//상대가 공격스킬을 차징중이고 (지금은 아니지만) 상대방이 전진하면 자신이 사정권에 들게되는 상태 
		if(player.isPlayerSide) _targetPlayer = GameManager.me.pvpPlayer;
		else _targetPlayer = GameManager.me.player;
		_result = 0;

		if(_targetPlayer == null) return 0;

		if(_targetPlayer.nowChargingSkill != null)
		{
			if(_targetPlayer.nowChargingSkill.skillType == Skill.Type.ATTACK)
			{
				// 상대가 공격스킬을 차징중이고 (지금은 아니지만) 상대방이 전진하면 자신이 사정권에 들게되는 상태 
				if(_targetPlayer.nowChargingSkill.playerHeroTargetingPVPModeChecker(player,checkPosition) == false &&
				   _targetPlayer.nowChargingSkill.playerHeroTargetingPVPModeMoveForwardChecker(player,checkPosition))
				{
					// 풀차징 적용
					_tempDamage = _targetPlayer.nowChargingSkill.preDamageCalc(player, _targetPlayer.nowChargingSkillInfo.reinforceLevel + _targetPlayer.nowChargingSkill.baseLevel, 20,  _targetPlayer);
					//						Debug.LogError("== 6 차징:앞으로 사정권!: " + GameManager.me.player.nowChargingSkill.id + "    _tempDamage: " + _tempDamage);
					if(attr[0] > GameManager.inGameRandom.Range(0,100))//GameManager.getRandomNum())
					{
						_result = player.getDamagePer(_tempDamage) * (float)attr[1];
					}
				}
			}
		}

//		Debug.Log("dp3 : " + _result);
		_targetPlayer = null;
		return _result;		
	}

	IFloat cpLeft;
	IFloat cpRight;
	IFloat bLeft;
	IFloat bRight;

	IFloat getDp4(Player player, IVector3 checkPosition)
	{
		//해당 위치에 공격스킬 지속 이펙트 발생
		// 현재 위치를 기준으로 앞뒤로 50cm 를 대상 지점으로 삼자.
		// 간격을 100cm 잡았으니까.
		List<Bullet> bullets;
		_result = 0;

		if(player.isPlayerSide) bullets = GameManager.me.bulletManager.monsterBulletList;
		else bullets = GameManager.me.bulletManager.playerBulletList;
		
		int len = bullets.Count;
		
		for(int i = 0; i < len ; ++i)
		{
			if(bullets[i].isDurationSkill)
			{
				cpLeft = checkPosition.x - 50.0f;
				cpRight = checkPosition.x + 50.0f;


				if(bullets[i].isCircleColliderType)
				{
					bLeft = bullets[i].bTransformPosition.x - bullets[i].targetRange;
					bRight = bullets[i].bTransformPosition.x + bullets[i].targetRange;
				}
				else
				{
					bLeft = bullets[i].hitObject.x;
					bRight = bullets[i].hitObject.right;
				}

				if(cpLeft <= bLeft && cpRight >= bLeft)
				{
					_result = value;
					break;
				}
				else if(bLeft <= cpLeft && bRight >= cpLeft)
				{
					_result = value;
					break;
				}
			}
		}

//		Debug.Log("dp4 : " + _result);
		bullets = null;
		return _result;		
	}

	IFloat getDp5(Player player, IVector3 checkPosition)
	{
		//해당 위치와 히어로 위치 사이에 공격스킬 지속 이펙트가 발생
		//총알의 반경 따위는 필요없고 그냥 그 사이에만 있으면 된다고 판단.
		//필요하면 총알반경도 포함하자.
		List<Bullet> bullets;

		_result = 0;

		int len;

		if(player.isPlayerSide)
		{
			bullets = GameManager.me.bulletManager.monsterBulletList;
			len = bullets.Count;
			for(int i = 0; i < len ; ++i)
			{
				if(bullets[i].isDurationSkill)
				{
//					if(checkPosition.x <= bullets[i].bTransformPosition.x && bullets[i].bTransformPosition.x <= player.cTransformPosition.x)
//					{
//						_result = value;
//						break;
//					}
					cpLeft = checkPosition.x - 50.0f;
					cpRight = player.cTransformPosition.x;

					if(cpRight < cpLeft )
					{
						cpLeft = cpRight;
						cpRight = player.cTransformPosition.x;
					}


					if(bullets[i].isCircleColliderType)
					{
						bLeft = bullets[i].bTransformPosition.x - bullets[i].targetRange;
						bRight = bullets[i].bTransformPosition.x + bullets[i].targetRange;
					}
					else
					{
						bLeft = bullets[i].hitObject.x;
						bRight = bullets[i].hitObject.right;
					}


					if(cpLeft <= bLeft && cpRight >= bLeft)
					{
						_result = value;
						break;
					}
					else if(bLeft <= cpLeft && bRight >= cpLeft)
					{
						_result = value;
						break;
					}

				}
			}
		}
		else
		{
			bullets = GameManager.me.bulletManager.playerBulletList;
			len = bullets.Count;
			for(int i = 0; i < len ; ++i)
			{
				if(bullets[i].isDurationSkill)
				{
//					if(player.cTransformPosition.x <= bullets[i].bTransformPosition.x &&  bullets[i].bTransformPosition.x <= checkPosition.x )
//					{
//						_result = value;
//						break;
//					}

					cpLeft = player.cTransformPosition.x;
					cpRight = checkPosition.x + 50.0f;

					if(cpLeft > cpRight)
					{
						cpLeft = cpRight;
						cpRight = player.cTransformPosition.x;
					}


					if(bullets[i].isCircleColliderType)
					{
						bLeft = bullets[i].bTransformPosition.x - bullets[i].targetRange;
						bRight = bullets[i].bTransformPosition.x + bullets[i].targetRange;
					}
					else
					{
						bLeft = bullets[i].hitObject.x;
						bRight = bullets[i].hitObject.right;
					}

					if(cpLeft <= bLeft && cpRight >= bLeft)
					{
						_result = value;
						break;
					}
					else if(bLeft <= cpLeft && bRight >= cpLeft)
					{
						_result = value;
						break;
					}

				}
			}
		}

//		Debug.Log("dp5 : " + _result);
		bullets = null;
		return _result;		
	}



	// 해당 위치에 몬스터히어로의 1/2번 타게팅 데칼이 표시됨
	// PvE 전용.

	IFloat getDp6(Player player, IVector3 checkPosition)
	{
		//해당 위치에 공격스킬 지속 이펙트 발생
		// 현재 위치를 기준으로 앞뒤로 50cm 를 대상 지점으로 삼자.
		// 간격을 100cm 잡았으니까.
		_result = 0;
		
		int len = GameManager.me.characterManager.activeMonsterTargetingDecal.Count;
		
		for(int i = 0; i < len ; ++i)
		{
			if(GameManager.me.characterManager.activeMonsterTargetingDecal[i].visible && GameManager.me.characterManager.activeMonsterTargetingDecal[i].state == TargetingDecal.State.Start)
			{
				cpLeft = checkPosition.x - 50.0f;
				cpRight = checkPosition.x + 50.0f;
				bLeft = GameManager.me.characterManager.activeMonsterTargetingDecal[i].lineLeft;
				bRight = GameManager.me.characterManager.activeMonsterTargetingDecal[i].lineRight;
				
				if(cpLeft <= bLeft && cpRight >= bLeft)
				{
					_result = value;
					break;
				}
				else if(bLeft <= cpLeft && bRight >= cpLeft)
				{
					_result = value;
					break;
				}
			}
		}

//		Debug.Log("dp6 : " + _result);
		return _result;		
	}



	IFloat getDp7(Player player, IVector3 checkPosition)
	{
		//상대 히어로가 5번/15번 공격타입의 스킬을 차징(PvP) or 데칼표시(PvE)

		if(player.isPlayerSide) _targetPlayer = GameManager.me.pvpPlayer;
		else _targetPlayer = GameManager.me.player;
		_result = 0;
		int unitNum = 0;
		int len = 0;

		if(_targetPlayer != null) // PVP
		{
			if(_targetPlayer.nowChargingSkill != null)
			{
				if(_targetPlayer.nowChargingSkill.skillType == Skill.Type.ATTACK && (_targetPlayer.nowChargingSkill.exeData.type == 5 || _targetPlayer.nowChargingSkill.exeData.type == 15) )
				{
					//(5 - 해당위치 앞 아군유닛개수) * 10
					if(player.isPlayerSide)
					{
						len = GameManager.me.characterManager.playerMonster.Count;

						for(int j = 0; j < len; ++j)
						{
							if(GameManager.me.characterManager.playerMonster[j].cTransformPosition.x > checkPosition.x)
							{
								if(GameManager.me.characterManager.playerMonster[j].isEnabled && GameManager.me.characterManager.playerMonster[j].unitData != null)
								{
									++unitNum;
								}
							}
							else break;
						}
					}
					else
					{
						len = GameManager.me.characterManager.monsters.Count;

						for(int j = 0; j < len; ++j)
						{
							if(GameManager.me.characterManager.monsters[j].cTransformPosition.x < checkPosition.x)
							{
								if(GameManager.me.characterManager.monsters[j].isEnabled && GameManager.me.characterManager.monsters[j].unitData != null)
								{
									++unitNum;
								}
							}
							else break;
						}
					}

					_result = ( 5 - unitNum) * value;

				}
			}

			_targetPlayer = null;
		}
		else // PVE
		{
			int decalLen = GameManager.me.characterManager.activeMonsterTargetingDecal.Count;
			
			for(int i = 0; i < decalLen ; ++i)
			{
				if(GameManager.me.characterManager.activeMonsterTargetingDecal[i].visible && 
				   GameManager.me.characterManager.activeMonsterTargetingDecal[i].state == TargetingDecal.State.Start &&
				   ( GameManager.me.characterManager.activeMonsterTargetingDecal[i].skillExeType == 5 || GameManager.me.characterManager.activeMonsterTargetingDecal[i].skillExeType == 15 ) )
				{
					len = GameManager.me.characterManager.playerMonster.Count;
					
					for(int j = 0; j < len; ++j)
					{
						if(GameManager.me.characterManager.playerMonster[j].cTransformPosition.x > checkPosition.x)
						{
							if(GameManager.me.characterManager.playerMonster[j].isEnabled && GameManager.me.characterManager.playerMonster[j].unitData != null)
							{
								++unitNum;
							}
						}
						else break;
					}

					_result = ( 5 - unitNum) * value;

					break;
				}
			}

		}

//		Debug.Log("dp7 : " + _result);
		return _result;		
	}






	IFloat getDp8(Player player, IVector3 checkPosition)
	{
		//(탐험모드 서바이벌, 프로텍트모드 only) 전방 300 cm 이내에 해골마법진이 있는 경우

		_result = 0;

		if(player.isPlayerSide)
		{
			if(StageManager.instance.nowRound.mode == RoundData.MODE.SURVIVAL ||
			   StageManager.instance.nowRound.mode == RoundData.MODE.PROTECT)
			{
				int len = GameManager.me.characterManager.decoMonsters.Count;
				
				for(int j = 0; j < len; ++j)
				{
					if(GameManager.me.characterManager.decoMonsters[j].cTransformPosition.x - 300 < checkPosition.x)
					{
						if(GameManager.me.characterManager.decoMonsters[j].category == MonsterCategory.Category.MONSTER_DEAD_ZONE &&
						   GameManager.me.characterManager.decoMonsters[j].isEnabled)
						{
							return value;
						}
					}
				}
			}
		}

		return _result;		
	}




//====================================================================================================
//	[이동 AI - 일반모드]		

	//==========================


	public bool battleStateIsReady(Player player)
	{
		return (MathUtil.abs( GameManager.me.characterManager.targetZoneMonsterLine , GameManager.me.characterManager.targetZonePlayerLine ) > value);

	}


	//========================

	//특정 위치 고정		
	//최후방 지점 + -1 cm 지점으로 이동 후 정지
	// → (테스트용) 해당 위치로 이동 후 움직이지 않음, (-1 세팅시 위치고정 해제)
	bool readyModeMove0(Player player)
	{
		if(value == -1)
		{
			return false;
		}
		else
		{
			_v = player.cTransformPosition;

			if(player.isPlayerSide) _v.x = StageManager.mapStartPosX + value;
			else _v.x = StageManager.mapEndPosX - value;

			player.setStateAndDirectionByTargetPosition(_v);

//			if(player.isPlayerSide)
//			{
//				Debug.LogError("위치이동 : " + _v + "     : " + player.moveState);
//			}

			return true;

		}
		//최후방 지점 + -1 cm 지점으로 이동 후 정지
		return false;
	}

	//소환된 소환유닛이 없을 경우		
	// 1 (0 : 정지, 1 : 전진)			
	bool readyModeMove1(Player player)
	{
		// 1 (0 : 정지, 1 : 전진)
		if(player.isPlayerSide)
		{
			if(GameManager.me.characterManager.playerMonster.Count <= 1)
			{
				if(value == 1) // 전진
				{
					player.setMovingDirection(Monster.MoveState.Forward);	
				}
				else if(value > 1)
				{
					_v = player.cTransformPosition;
					_v.x = GameManager.me.characterManager.targetZoneMonsterLine - value;
					player.setStateAndDirectionByTargetPosition(_v);
				}
				else // 정지.
				{
					player.setMovingDirection(Monster.MoveState.Stop);	
					player.state = Monster.NORMAL;
				}

//				if(player.isPlayerSide)
//				{
//					Debug.LogError("소환유닛없음 : " + player.moveState);
//				}



				return true;
			}
		}
		else
		{
			if(GameManager.me.characterManager.monsters.Count <= 1)
			{
				if(value == 1) // 전진
				{
					player.setMovingDirection(Monster.MoveState.Forward);	
				}
				else if(value > 1)
				{
					_v = player.cTransformPosition;
					_v.x = GameManager.me.characterManager.targetZonePlayerLine + value;
					player.setStateAndDirectionByTargetPosition(_v);
				}
				else // 정지.
				{
					player.setMovingDirection(Monster.MoveState.Stop);	
					player.state = Monster.NORMAL;
				}
				return true;
			}
		}
		return false;
	}

	//소환된 소환유닛이 있는 경우
	bool readyModeMove2(Player player)
	{
		//타겟존 - 100 cm 지점 유지하며 이동
		if(player.isPlayerSide)
		{
			if(GameManager.me.characterManager.playerMonster.Count > 1)
			{
				_v = player.cTransformPosition;
				_v.x = GameManager.me.characterManager.targetZonePlayerLine - value;
				player.setStateAndDirectionByTargetPosition(_v);

//				Debug.LogError("소환유닛있음  : 이동 : " + _v + "   : "  + player.moveState);

				return true;
			}
		}
		else
		{
			if(GameManager.me.characterManager.monsters.Count > 1)
			{
				_v = player.cTransformPosition;
				_v.x = GameManager.me.characterManager.targetZoneMonsterLine + value;
				player.setStateAndDirectionByTargetPosition(_v);
				return true;
			}
		}
		return false;
	}



	//========================

	bool defaultAttackOnChecker0(Player player)
	{
		//히어로 HP가 70% 이상 & 아군 생존 근접유닛 개수가 2개
		if(player.hpPer * 100.0f >= value)
		{
			int len = 0;
			if(player.isPlayerSide)
			{
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len > 2) return false;
					}
				}
			}
			else
			{
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len > 2) return false;
					}
				}
			}
			
			return len == 2;
		}
		
		return false;
	}

	bool defaultAttackOnChecker1(Player player)
	{
		//히어로 HP가 50% 이상 & 아군 생존 근접유닛 개수가 1개
		if(player.hpPer * 100.0f >= value)
		{
			int len = 0;
			if(player.isPlayerSide)
			{
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len > 1) return false;
					}
				}
			}
			else
			{
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len > 1) return false;
					}
				}
			}
			
			return len == 1;
		}

		return false;
	}

	bool defaultAttackOnChecker2(Player player)
	{
		//히어로 HP가 30% 이상 & 아군 생존 근접유닛 개수가 0개
		if(player.hpPer * 100.0f >= value)
		{
			if(player.isPlayerSide)
			{
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						return false;
					}
				}
			}
			else
			{
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						return false;
					}
				}
			}
			
			return true;
		}

		return false;
	}



	bool defaultAttackOnChecker3(Player player)
	{
		//히어로 HP가 30% 이상 & 아군 생존 근접유닛 개수가 3개
		if(player.hpPer * 100.0f >= value)
		{
			int len = 0;
			if(player.isPlayerSide)
			{
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len >= 3) return true;
					}
				}
			}
			else
			{
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
					{
						++len;
						if(len >= 3) return true;
					}
				}
			}
			
			if(len >= 3) return true;
		}
		
		return false;
	}



	bool defaultAttackOffChecker0(Player player)
	{
		//히어로 HP가 50% 미만일 때
		return (player.hpPer * 100.0f < value);
	}
	
	bool defaultAttackOffChecker1(Player player)
	{
		//현재위치의 위험도 점수 10점 이상
		return (player.dangerPoints[Player.CURRENT_POSITION_INDEX].score >= value);
	}
	
	bool defaultAttackOffChecker2(Player player)
	{
		//현재위치의 위험도 점수 5점 이상 & 아군 생존 소환유닛 개수 0개
		if(player.dangerPoints[Player.CURRENT_POSITION_INDEX].score >= value)
		{
			if(player.isPlayerSide)
			{
				return (GameManager.me.characterManager.playerMonster.Count < 1);
			}
			else
			{
				return (GameManager.me.characterManager.monsters.Count < 1);
			}
		}

		return false;
	}


	bool defaultAttackOffChecker3(Player player)
	{
		//기본공격 가능 위치의 위험도 점수가 7점 이상이면 공격 못함.
		// 앞쪽 이동할 거리의 위험도 점수로 대체!

		int i = player.onOffCheckNextMovingPosIndex - 1;
		if(i < 0) i = 0;
		for(; i <= player.onOffCheckNextMovingPosIndex; ++i)
		{
			if(player.dangerPoints[i].score >= value)
			{
				return true;
			}
		}

		return false;
	}



//====== tag ai...

	public bool checkTagAi(Player player)
	{
		Player waitPlayer = GameManager.me.battleManager.getWatingPlayer(player.isPlayerSide);

		IFloat point = (player.hpPer.Get() + (float)attr[0]);  //현재 히어로의 남은 HP 비율(0~100) x 1
		point += player.hp / waitPlayer.hp * (float)attr[1]; //현재 히어로의 남은 HP / 대기 회어로의 남은 HP  * 100				
		point += (player.sp/player.maxSp)/(waitPlayer.sp/waitPlayer.maxSp) * (float)attr[2]; //현재 히어로 남은 SP 비율 / 대기 회어로 남은 SP 비율 * 100				
		point += (player.mp/player.maxMp)/(waitPlayer.mp/waitPlayer.maxMp) * (float)attr[3]; //현재 히어로 남은 MP 비율 / 대기 회어로 남은 MP 비율 * 50			

		if(player.isPlayerSide)
		{
			point += GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum * attr[4]; //생존 소환룬 마리수 x 50				
		}
		else
		{
			point += GameManager.me.characterManager.totalAliveMonsterUnitNum * attr[4]; //생존 소환룬 마리수 x 50				
		}

//#if UNITY_EDITOR
//		attr[5] = 1000;
//
//#endif

		return Xfloat.lessEqualThan(point, attr[5]);
	}


//====== 유닛룬....

	// 1. 쿨타임이 남아있는 경우.
	IFloat getUnitRunePoint0(Player player, UnitSlot slot)
	{
		if(slot.coolTime > 0)
		{
			return -(attr[0] + MathUtil.RoundToInt(slot.coolTime * (float)attr[1]));
		}
		return 0;
	}
	
	// 2. sp가 모자란 경우.
	IFloat getUnitRunePoint1(Player player, UnitSlot slot)
	{
		
		if(player.sp < slot.useSp)
		{
			return -(attr[0] + ( (slot.useSp-player.sp) * (float)attr[1] ));
		}
		return 0;
	}
	
	
	//3.공격스킬 & 상대 적 유닛개수 3마리 이상 & 공격타입이 0,1,3번이 아님
	IFloat getUnitRunePoint2(Player player, UnitSlot slot)
	{
		if(player.isPlayerSide)
		{
			//// 3.
			if(GameManager.me.characterManager.monterUnitInPlayerTargetZone >= attr[0]
			   && (slot.unitData.attackType.type == 0 || slot.unitData.attackType.type == 1 || slot.unitData.attackType.type == 3))
			{
				return -attr[1];
			}
		}
		else
		{
			if(GameManager.me.characterManager.playerUnitInMonsterTargetZone >= attr[0]
			   && (slot.unitData.attackType.type == 0 || slot.unitData.attackType.type == 1 || slot.unitData.attackType.type == 3))
			{
				return -attr[1];
			}
		}
		
		return 0;
	}
	
	IFloat getUnitRunePoint3(Player player, UnitSlot slot)
	{
		if(player.isPlayerSide)
		{
			if(GameManager.me.characterManager.totalAliveMonsterUnitNum - GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum > attr[0] 
			   && slot.coolTime <= 0)
			{
				return attr[1];
			}
		}
		else
		{
			if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum - GameManager.me.characterManager.totalAliveMonsterUnitNum > attr[0] 
			   && slot.coolTime <= 0)
			{
				return attr[1];
			}
		}
		
		return 0;
	}





//=== 스킬 룬.

	//쿨타임이 남아있는 경우				- (10 + (남은쿨타임(초) * 2))		

	IFloat getSkillRunePoint0(Player player, SkillSlot slot)
	{
		// 쿨타임이 남아있는 경우. 차감.
		if(slot.coolTime > 0)
		{
			return - (attr[0] + (slot.coolTime * (float)attr[1]));
		}
		return 0;
	}

	//MP가 모자란 경우				 - (10 + (모자란MP * 1))		
	IFloat getSkillRunePoint1(Player player, SkillSlot slot)
	{
		// mp가 모자란 경우. 차감. 
		if(player.mp < slot.useMp)
		{
			//			Debug.Log( "mp가 모자란 경우. 차감" + (skillData.mp-mon.mp));
			return - (attr[0] +  ( (slot.useMp-player.mp) * (float)attr[1] ));
		}
		return 0;
	}


	//공격스킬 & 상대 적 유닛개수 3마리 이상 & 공격타입이 0,1,3번이 아님
	IFloat getSkillRunePoint2(Player player, SkillSlot slot)
	{
		if(player.isPlayerSide)
		{
			//공격스킬 & 타겟존 내 적유닛개수 7마리 이상 & 공격타입이 0,1,3번이 아님 증가.
			if(slot.skillData.skillType == Skill.Type.ATTACK &&  GameManager.me.characterManager.totalAliveMonsterUnitNum >= attr[0]
			   && slot.skillData.exeData.type != 0 && slot.skillData.exeData.type != 1 && slot.skillData.exeData.type != 3)
			{
				//			Debug.Log( "공격스킬 & 타겟존 내 적유닛개수 7마리 이상 & 공격타입이 0,1,3번이 아님 증가");
				return attr[1];
			}
		}
		else
		{
			if(slot.skillData.skillType == Skill.Type.ATTACK &&  GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum >= attr[0]
			   && slot.skillData.exeData.type != 0 && slot.skillData.exeData.type != 1 && slot.skillData.exeData.type != 3)
			{
				//			Debug.Log( "공격스킬 & 타겟존 내 적유닛개수 7마리 이상 & 공격타입이 0,1,3번이 아님 증가");
				return attr[1];
			}
		}
		return 0;
	}

	//(힐 or 버프스킬) & 생존 아군유닛 개수 0 마리
	IFloat getSkillRunePoint3(Player player, SkillSlot slot)
	{
		if(player.isPlayerSide)
		{
			//(힐 or 버프스킬) & 생존 아군유닛 개수 0 마리
			if(slot.skillData.targetType == Skill.TargetType.ME &&  GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum == 0)			  
			{
				return value;
			}
		}
		else
		{
			if(slot.skillData.targetType == Skill.TargetType.ME &&  GameManager.me.characterManager.totalAliveMonsterUnitNum == 0)
			{
				//			Debug.Log( "공격스킬 & 타겟존 내 적유닛개수 7마리 이상 & 공격타입이 0,1,3번이 아님 증가");
				return value;
			}
		}
		return 0;
	}


	//(디버프스킬) & 생존 적유닛 개수 0 마리
	IFloat getSkillRunePoint4(Player player, SkillSlot slot)
	{
		if(player.isPlayerSide)
		{
			if(slot.skillData.skillType == Skill.Type.DEBUFF &&  GameManager.me.characterManager.totalAliveMonsterUnitNum == 0)			  
			{
				return value;
			}
		}
		else
		{
			if(slot.skillData.skillType == Skill.Type.DEBUFF &&  GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum == 0)
			{
				return value;
			}
		}
		return 0;
	}


	//SK_B001/SK_B002/SK_B003 스킬 &HP가장 낮은 유닛의 HP 비율 < 50 %
	IFloat getSkillRunePoint5(Player player, SkillSlot slot)
	{
		if(slot.isB1_3Skill)
		{
			if(player.isPlayerSide)
			{
				if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum > 0 && GameManager.me.characterManager.lowestPlayerUnitHpPer < attr[0])
				{
					return attr[1];
				}
			}
			else
			{
				if(GameManager.me.characterManager.totalAliveMonsterUnitNum > 0 && GameManager.me.characterManager.lowestMonsterUnitHpPer  < attr[0])
				{
					return attr[1];
				}				
			}

		}
		return 0;
	}

	//SK_B001/SK_B002/SK_B003 스킬 & HP가장 낮은 유닛의 HP 비율 > 80 %
	IFloat getSkillRunePoint6(Player player, SkillSlot slot)
	{
		if(slot.isB1_3Skill)
		{
			if(player.isPlayerSide)
			{
				if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum > 0 && GameManager.me.characterManager.lowestPlayerUnitHpPer > attr[0])
				{
					return attr[1];
				}
			}
			else
			{
				if(GameManager.me.characterManager.totalAliveMonsterUnitNum > 0 && GameManager.me.characterManager.lowestMonsterUnitHpPer > attr[0])
				{
					return attr[1];
				}				
			}
			
		}
		return 0;
	}

	//SK_B007/SK_B008 스킬 & 생존 아군유닛의 평균 HP 비율 > 90 %
	IFloat getSkillRunePoint7(Player player, SkillSlot slot)
	{
		if(slot.isB7_8Skill)
		{
			if(player.isPlayerSide)
			{
				if(GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum > 0 && GameManager.me.characterManager.lowestPlayerUnitHpPer > attr[0])
				{
					return attr[1];
				}
			}
			else
			{
				if(GameManager.me.characterManager.totalAliveMonsterUnitNum > 0 && GameManager.me.characterManager.lowestMonsterUnitHpPer > attr[0])
				{
					return attr[1];
				}				
			}
			
		}
		return 0;
	}


	public bool checkActiveSkill(Monster attacker, UnitSkillData skillData)
	{
		int len = 0;
		int aliveUnitWithoutMe = 0;
		IFloat distanceFromTargetzone = 0;

		if(attacker.isPlayerSide)
		{
			distanceFromTargetzone = (MathUtil.abs( attacker.cTransformPosition.x , GameManager.me.characterManager.targetZoneMonsterLine ));

			len = GameManager.me.characterManager.playerMonster.Count;
			for(int i = len - 1; i >= 0; --i)
			{
				if(GameManager.me.characterManager.playerMonster[i].enabled && 
				   GameManager.me.characterManager.playerMonster[i].isPlayer == false &&
				   GameManager.me.characterManager.playerMonster[i] != attacker)
				{
					++aliveUnitWithoutMe;
				}
			}

		}
		else
		{
			distanceFromTargetzone = (MathUtil.abs( attacker.cTransformPosition.x , GameManager.me.characterManager.targetZonePlayerLine ));

			len = GameManager.me.characterManager.monsters.Count;

			for(int i = len - 1; i >= 0; --i)
			{
				if(GameManager.me.characterManager.monsters[i].enabled && 
				   GameManager.me.characterManager.monsters[i].isPlayer == false &&
				   GameManager.me.characterManager.monsters[i] != attacker)
				{
					++aliveUnitWithoutMe;
				}
			}
		}

		switch(skillData.targeting)
		{
		case 0: //		0	본인					
			//타겟존과의 거리 700 이하					attr0
			return (distanceFromTargetzone < attr[0]);

			break;
		case 1: //		1	자신을 공격한 근접유닛					
			//타겟존과의 거리 400 이하					attr1
			return (distanceFromTargetzone < attr[1]);

			break;
			
		case 2: //		2	자신이 공격한 상대					
			//타겟존과의 거리 < 본인의 공격거리 * 1.2					attr2

			IFloat ar = attacker.stat.atkRange * (float)attr[2]/100.0f ; //IFloat ar = attacker.stat.atkRange * (float)attr[2]*0.01f ;
			return distanceFromTargetzone < ar ;
			break;
			
		case 3: //		3	자신과 가장 가까운 적					
			//타겟존과의 거리 800 이하					attr3
			return (distanceFromTargetzone < attr[3]);

			break;
			
		case 4: //		4	자신과 가장 가까운 아군유닛					
			//(본인 제외) 아군유닛 1마리 이상 생존					attr4
			return aliveUnitWithoutMe >= attr[4];

			break;
			
		case 5:

			switch(skillData.skillType)
			{
			case Skill.Type.ATTACK://		5	전방 N거리 지점			공격 스킬		
				//전방N, [공격타입속성] 범위내 적히어로, 적유닛 1마리 이상					attr5
				initSkillTargetingChecker(skillData, attacker.cTransformPosition, (attacker.isPlayerSide)?skillData.targetAttr[0].Get():-skillData.targetAttr[0].Get());

				if(_isCircleType) return (targetingChecker(attacker, Skill.TargetType.ENEMY, skillData.targetAttr[0], _radius, true, attr[5]) );
				else return (targetingChecker(attacker, Skill.TargetType.ENEMY, _hitBox, true, attr[5]));

				break;
			case Skill.Type.DEBUFF://		5	전방 N거리 지점			디버프		
				//전방N, [공격타입속성] 범위내에 적유닛 1마리 이상					attr6
				initSkillTargetingChecker(skillData, attacker.cTransformPosition, (attacker.isPlayerSide)?skillData.targetAttr[0].Get():-skillData.targetAttr[0].Get());

				if(_isCircleType) return (targetingChecker(attacker, Skill.TargetType.ENEMY, skillData.targetAttr[0], _radius, true, attr[6]) );
				else return (targetingChecker(attacker, Skill.TargetType.ENEMY, _hitBox, true, attr[6]));

				break;
			case Skill.Type.HEAL://		5	전방 N거리 지점			힐		
				//전방N, [공격타입속성] 범위내에 아군유닛 1마리 이상					attr7
				initSkillTargetingChecker(skillData, attacker.cTransformPosition, (attacker.isPlayerSide)?skillData.targetAttr[0].Get():-skillData.targetAttr[0].Get());

				if(_isCircleType) return (targetingChecker(attacker, Skill.TargetType.ME, skillData.targetAttr[0], _radius, true, attr[7]) );
				else return (targetingChecker(attacker, Skill.TargetType.ME, _hitBox, true, attr[7]));

				break;

			case Skill.Type.BUFF://		5	전방 N거리 지점			버프		
				//"전방N, 지름 300 범위내에 적히어로, 적유닛 1마리 이상 & 타겟존과의 거리 400 이하"					attr8,attr9
				return (targetingChecker(attacker, Skill.TargetType.ENEMY, skillData.targetAttr[0], attr[8], true, attr[9]) && distanceFromTargetzone <= 400.0f);
				break;
			}
			break;
			
		case 6: //		6	HP가 가장 낮은 아군 유닛					
			//본인 제외 아군유닛 1마리 이상					attr10
			return aliveUnitWithoutMe >= attr[10];

			break;
		case 7:
			if(skillData.exeData.type == 5) //		7	정면방향			발동타입 5		
			{
				//타겟존과의 거리 600 이하					attr11
				return distanceFromTargetzone <= attr[11];
			}
			else if(skillData.exeData.type == 12) //		7	정면방향			발동타입 12		
			{
				_hitBox.height = skillData.exeData.attr[3];
				_hitBox.depth = skillData.exeData.attr[3];
				_hitBox.width = skillData.exeData.attr[4];
				_hitBox.setPosition(attacker.cTransformPosition);
				//속성 4,5 (z,x) 범위내 적 1마리 이상 존재					attr12
				return (targetingChecker(attacker, Skill.TargetType.ENEMY, _hitBox, true, attr[12]));
				//skillData.exeData.attr[3]   
				//skillData.exeData.attr[4]   
				//attr[15] 1마리 
				//return (targetingChecker(attacker, Skill.TargetType.ME, skillData.exeData.attr[3], attr[9], false) >= attr[10]);
			}
			else //		7	정면방향			기타		
			{
				//타겟존과의 거리 600 이하					attr13
				return distanceFromTargetzone <= attr[13];
			}
			break;
		}


		return false;
	}



	private static bool _isCircleType = false;
	private static IFloat _radius = 0.0f;
	private void initSkillTargetingChecker(UnitSkillData sd, IVector3 attackerPosition, IFloat xOffset)
	{
		switch(sd.exeData.type)
		{
		case 6:
			_radius = ((IFloat)sd.exeData.attr[1]);
			_isCircleType = true;
			break;
		case 7:
			_radius = ((IFloat)sd.exeData.attr[1]);
			_isCircleType = true;
			break;
		case 8:
			_radius = ((IFloat)sd.exeData.attr[0]);
			_isCircleType = true;
			break;
		case 9:
			if(sd.exeData.attr[1] > 12) // 일반 원기둥.
			{
				_isCircleType =  true;
				_radius = ((IFloat)sd.exeData.attr[2]);
			}
			else // 사각기둥.
			{
				_isCircleType =  false;

				_hitBox.height = 1000;
				_hitBox.depth = sd.exeData.attr[2];
				_hitBox.width = sd.exeData.attr[3];
				attackerPosition.x += xOffset;
				_hitBox.setPosition(attackerPosition);
			}
			break;
		case 10:
			if(sd.exeData.attr[2] > 12) // 일반 원기둥.
			{
				_isCircleType =  true;
				_radius =  ((IFloat)sd.exeData.attr[3]);
			}
			else // 사각기둥.
			{
				_isCircleType = false;
				_hitBox.height = 1000;
				_hitBox.depth = sd.exeData.attr[3];
				_hitBox.width = sd.exeData.attr[4];
				attackerPosition.x += xOffset;
				_hitBox.setPosition(attackerPosition);

			}
			break;
		case 11:
			if(sd.exeData.attr[3] > 12) // 일반 원기둥.
			{
				_isCircleType = true;
				_radius = (IFloat)sd.exeData.attr[4];
			}
			else // 사각기둥.
			{
				_isCircleType =  false;
				_hitBox.height = 1000;
				_hitBox.depth = sd.exeData.attr[4];
				_hitBox.width = sd.exeData.attr[5];
				attackerPosition.x += xOffset;
				_hitBox.setPosition(attackerPosition);
			}
			break;

		case 17:
			_isCircleType = true;
			_radius = (IFloat)sd.exeData.attr[5];
			break;
		}
	}




	private static HitObject _hitBox = new HitObject();

	public bool targetingChecker(Monster mon, Skill.TargetType targetType, IFloat xOffset, IFloat hitRadius, bool includeHero, int checkNum)
	{
		_v = mon.cTransformPosition;

		int hitCount = 0;

		hitRadius = hitRadius*0.5f; // 지름을 반지름으로 바꿔준다.

		if(mon.isPlayerSide) // 주인공 쪽이면.
		{
			_v.x += xOffset;

			// 주인공편에게.
			if(targetType == Skill.TargetType.ME)
			{
				for(int i =GameManager.me.characterManager.playerMonster.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.playerMonster[i].isHero) continue;
					if(GameManager.me.characterManager.playerMonster[i] == mon) continue;

					if(GameManager.me.characterManager.playerMonster[i].hitByDistance (_v, hitRadius))
					{
						++hitCount;	
						if(hitCount >= checkNum) return true;
					}
				}
				
			}
			// 몬스터에게.
			else
			{
				for(int i = GameManager.me.characterManager.monsters.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.monsters[i].isHero) continue;
					if(GameManager.me.characterManager.monsters[i].hitByDistance (_v, hitRadius))
					{
						++hitCount;	
						if(hitCount >= checkNum) return true;
					}
				}
			}
		}
		else // pvp 캐릭터 쪽이면.
		{
			_v.x -= xOffset;

			// 몬스터에게.
			if(targetType == Skill.TargetType.ME)
			{
				for(int i = GameManager.me.characterManager.monsters.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.monsters[i].isHero) continue;
					if(GameManager.me.characterManager.monsters[i] == mon) continue;
					if(GameManager.me.characterManager.monsters[i].hitByDistance (_v, hitRadius))
					{
						++hitCount;
						if(hitCount >= checkNum) return true;
					}
				}
			}
			// 주인공들에게.
			else
			{
				for(int i = GameManager.me.characterManager.playerMonster.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.playerMonster[i].isHero) continue;
					if(GameManager.me.characterManager.playerMonster[i].hitByDistance (_v, hitRadius))
					{
						++hitCount;
						if(hitCount >= checkNum) return true;
					}
				}
			}
		}

		return false;
	}





	public bool targetingChecker(Monster mon, Skill.TargetType targetType, HitObject hitBox, bool includeHero, int checkNum)
	{
		int hitCount = 0;
		
		if(mon.isPlayerSide) // 주인공 쪽이면.
		{
			// 주인공편에게.
			if(targetType == Skill.TargetType.ME)
			{
				for(int i =GameManager.me.characterManager.playerMonster.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.playerMonster[i].isPlayer) continue;
					if(GameManager.me.characterManager.playerMonster[i] == mon) continue;
					
					if(GameManager.me.characterManager.playerMonster[i].getHitObject().intersectsBullet(hitBox))
					{
						++hitCount;	
						if(hitCount >= checkNum) return true;
					}
				}
				
			}
			// 몬스터에게.
			else
			{
				for(int i = GameManager.me.characterManager.monsters.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.monsters[i].isPlayer) continue;
					if(GameManager.me.characterManager.monsters[i].getHitObject().intersectsBullet(hitBox))
					{
						++hitCount;	
						if(hitCount >= checkNum) return true;
					}
				}
			}
		}
		else // pvp 캐릭터 쪽이면.
		{
			// 몬스터에게.
			if(targetType == Skill.TargetType.ME)
			{
				for(int i = GameManager.me.characterManager.monsters.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.monsters[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.monsters[i].isPlayer) continue;
					if(GameManager.me.characterManager.monsters[i] == mon) continue;
					if(GameManager.me.characterManager.monsters[i].getHitObject().intersectsBullet(hitBox))
					{
						++hitCount;
						if(hitCount >= checkNum) return true;
					}
				}
			}
			// 주인공들에게.
			else
			{
				for(int i = GameManager.me.characterManager.playerMonster.Count - 1; i >= 0; --i)
				{
					if(GameManager.me.characterManager.playerMonster[i].isEnabled == false) continue;
					if(includeHero == false && GameManager.me.characterManager.playerMonster[i].isPlayer) continue;
					if(GameManager.me.characterManager.playerMonster[i].getHitObject().intersectsBullet(hitBox))
					{
						++hitCount;
						if(hitCount >= checkNum) return true;
					}
				}
			}
		}
		
		return false;
	}










	public static int sort(PlayerAiData x, PlayerAiData y)
	{
		return x.type2.CompareTo(y.type2);
	}


}