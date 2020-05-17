using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class Player : Monster
{
	public IFloat skillModeProgressTime = 0.0f;

	public bool skillMoveIsNormal = true;
////
//	public bool skillMoveIsNormal
//	{
//		set
//		{
//			_skillMoveIsNormal = value;
//			Debug.LogError("################################# skillMoveIsNormal : " + _skillMoveIsNormal);;
//		}
//		get
//		{
//			return _skillMoveIsNormal;
//		}
//	}




	void aiSkillMove()
	{
		//_nowSelectSkillSlot != null

		// 2~3번 타겟팅 타입일때는 이동을 일반 공격 AI로 돌린다.
		if(_nowSelectSkillSlot.skillData.targeting > 1)
		{
			checkNormalAi();
			aiNormalMove();
		}
		else
		{
			// 스킬이지만 일반 모드 진행중.
			if(skillMoveIsNormal)
			{
				checkNormalAi();
				aiNormalMove();
			}
			else // 스킬이동 모드 진행중.
			{
//				Debug.Log("====== aiSkillMove 스킬 이동" );
				// 0~1번은 스킬 이동 로직에서 나온걸로 이동.
				setPVPSkillMoveToTargetPosition();
			}
		}
	}
	
	
	
	void setPVPSkillMoveToTargetPosition()
	{
//		Debug.Log("setPVPSkillMoveToTargetPosition : " + moveState);
		if(moveState == MoveState.Backward)
		{
			moveBackward();
		}
		else if(UIPopupSkillPreview.isOpen == false)
		{
			action.doMotion();
		}
	}
	
	
	
	
	
	
	
	//======================== PVP TargetPosition ================================//
	public float skillTargetingPositionMaxPoint = 0.0f; // 스킬모드에 들어온 후 가장 높은 최적위치 점수.
	float nowSkillMoveTargetZoneBestPoint = 0.0f; // 현재 최적위치 점수.

	int targetUnitNum = 0;
	int targetHeroNum = 0;

	void setTargetIsMePoint(HeroSkillData skillData, out float heroPoint, out float unitPoint)
	{
		heroPoint = 0;
		unitPoint = 0;
		switch(skillData.skillType)
		{
		case Skill.Type.HEAL:
			unitPoint = targetingPointMyUnitChecker.attr[2];
			break;
		case Skill.Type.BUFF:
			unitPoint = targetingPointMyUnitChecker.attr[3];
			break;
		}
	}

	void setTargetIsEnemyPoint(HeroSkillData skillData, out float heroPoint, out float unitPoint)
	{
		heroPoint = 0;
		unitPoint = 0;
		switch(skillData.skillType)
		{
		case Skill.Type.ATTACK:
			heroPoint = targetingPointHeroChecker.attr[0];
			unitPoint = targetingPointEnemyUnitChecker.attr[0];
			break;
		case Skill.Type.DEBUFF:
			heroPoint = targetingPointHeroChecker.attr[1];
			unitPoint = targetingPointEnemyUnitChecker.attr[1];
			break;
		}		
	}

	SkillSlot _checkSlot;

	// 스킬 타케팅 & 차징 정보 계산.
	public bool checkSkillType_0_1_BestTargetingPosition(SkillSlot slot = null)
	{
		if(slot == null)
		{
			_checkSlot = _nowSelectSkillSlot;
		}
		else
		{
			_checkSlot = slot;
		}
		
		int len = 0;
		int index = 0;
		//player는 0번이 우측이다. // 그럼 끝부터 검사한다.
		
		List<Monster> monsterList;
		
		float tempOp = 1.0f;
		float line = 0.0f;
		
		float heroPoint = 0.0f;
		float unitPoint = 0.0f;
		
		float optionValue = 0.0f;
		
		if(isPlayerSide)
		{
			tempOp = -1.0f;
			line = GameManager.me.characterManager.monsterLeftLine + hitObject.lineRight;
			
			// 내가 나에게 쏘는 것.
			if(_checkSlot.skillData.targetType == Skill.TargetType.ME) 
			{
				monsterList = GameManager.me.characterManager.playerMonster;
				len = monsterList.Count;
				setTargetIsMePoint(_checkSlot.skillData, out heroPoint, out unitPoint);
				
				optionValue = targetingPositionOption.value;
			}
			// 내가 적에게 쏘는 것.
			else
			{
				monsterList = GameManager.me.characterManager.monsters;
				len = monsterList.Count;
				setTargetIsEnemyPoint(_checkSlot.skillData, out heroPoint, out unitPoint);
				index = len-1; // 적에게 쓸때는 맨 끝부터 검사할거다.
				
				optionValue = -targetingPositionOption.value;
			}
		}
		else
		{
			tempOp = 1.0f;
			line = GameManager.me.characterManager.playerMonsterRightLine - hitObject.lineLeft;
			
			// 적이 자기편에게 쏘는 것.
			if(_checkSlot.skillData.targetType == Skill.TargetType.ME)
			{
				monsterList = GameManager.me.characterManager.monsters;
				len = monsterList.Count;
				setTargetIsMePoint(_checkSlot.skillData, out heroPoint, out unitPoint);
				optionValue = -targetingPositionOption.value;
			}
			// 적이 적(플레이어)에게 쏘는것.
			else
			{
				monsterList = GameManager.me.characterManager.playerMonster;
				len = monsterList.Count;
				setTargetIsEnemyPoint(_checkSlot.skillData, out heroPoint, out unitPoint);
				index = len-1; // 적에게 쓸때는 맨 끝부터 검사할거다.
				
				optionValue = targetingPositionOption.value;
			}
		}
		
		
		
		// 10개 구간으로 나눈다. 0번은 pvp 캐릭터가 갈 수 있는 가장 좌측이다.
		// 여기에서 부터 100cm 단위로 구간을 나누어 검사를 한다.
		_v = cTransformPosition;
		
		IFloat targetLine = 0;
		
		for(int i = 0; i < 10; ++i)
		{
			_skillMoveScoreCheckers[i].index = i;
			_skillMoveScoreCheckers[i].score = 0;
			targetUnitNum = 0;
			targetHeroNum = 0;
			
			_v.x = line + (i * 100.0f)*tempOp; // 전방에서 빼던지 더하던지.

			if(isPlayerSide)
			{
				if(_v.x <= StageManager.mapStartPosX.Get())
				{
					_v.x = StageManager.mapStartPosX + 10.0f;
					//_skillMoveScoreCheckers[i].score = -999;
					//continue;
				}
			}
			else
			{
				if(_v.x >= StageManager.mapEndPosX.Get ())
				{
					_v.x = StageManager.mapEndPosX - 10.0f;
					//_skillMoveScoreCheckers[i].score = -999;
					//continue;
				}
			}
			
			_skillMoveScoreCheckers[i].pos = _v;
			
			//타게팅타입 0번(히어로전방 A미터까지) ,1번(히어로전방 A미터, B지름) 스킬을 사용하려는 경우 
			if(_checkSlot.skillData.targeting == 0)
			{
				if(_checkSlot.skillData.exeData.type == 16)
				{
					//player는 0번이 우측이다. // 그럼 끝부터 검사한다.
					//monster는 0번이 좌측이다. // 0부터 검사한다.
					if(_checkSlot.skillData.targetType == Skill.TargetType.ME)
					{
						// monster 검사.
						// 우리편에게 사용 하는 녀석들은 전방에서부터. index 0부터 검색한다.
						// 적에게 사용하는 녀석들은 index = len-1 부터 검색한다.
						// 같은 편에게 쏘는 것. // 0부터 -> 
						
						// 구간 _v는 우리편이면 전방에서부터 줄어드니까. 위치는 500,400,300,200,100 이런식으로 줄어들거다.
						
						// 내가 우리편에게 쓴다. 
						if(isPlayerSide) 
						{
							for(index = 0; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false || _tempChar == this) continue;
								if(_tempChar.cTransformPosition.x < _v.x) // 지정 좌표보다 왼쪽에 있으면 당연히 아웃.
								{
									break;
								}
								else
								{
									// 캐릭터보다 우측에 있고 충돌 영역보다 왼쪽에 있게 된다. 그럼 ok!
									if(_tempChar.cTransformPosition.x  <= _v.x + _checkSlot.skillData.exeData.targetRange)
									{
										// 우리편에게 적용되는 애들은 유닛밖에 없음.
										++targetUnitNum;
									}
								}
							}
						}
						else
						{
							// 적이 자기편에게 쏜다.
							for(index = 0; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false || _tempChar == this) continue;
								// 지정 좌표보다 오른쪽에 있으면 당연히 아웃.
								if(_v.x < _tempChar.cTransformPosition.x )
								{
									break;
								}
								else
								{
									if(_v.x - _checkSlot.skillData.exeData.targetRange <= _tempChar.cTransformPosition.x)
									{
										++targetUnitNum;
									}
								}
							}
						}
					}
					else
					{
						// 다른 편에게 쏘는것. // 맨 끝. (list count -1 부터. 그래서 맨 뒤에서부터 검사하니까)
						if(isPlayerSide)
						{
							for(index = 0; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false) continue;
								if(_tempChar.cTransformPosition.x <= _v.x + _checkSlot.skillData.exeData.targetRange)
								{
									_skillMoveScoreCheckers[i].score += getScoreByMonsterType(_checkSlot.skillData,_tempChar);
								}
								else break;
							}
						}
						else
						{
							// 적이 나에게....
							for(index = 0; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false) continue;
								if(_v.x - _checkSlot.skillData.exeData.targetRange <= _tempChar.cTransformPosition.x)
								{
									_skillMoveScoreCheckers[i].score += getScoreByMonsterType(_checkSlot.skillData,_tempChar);
								}
								else break;
							}
						}
					}
				}
			}
			else if(_checkSlot.skillData.targeting == 1)
			{
				// 현재 위치에서 전방 cm 로 데칼 중심점 위치를 이동시킨다.
				
				if(isPlayerSide)
				{
					_v.x += _checkSlot.skillData.targetAttr[0].Get(); // 데칼 중심점.
					
					if(_checkSlot.skillData.targetType == Skill.TargetType.ME)
					{
						// 내가 우리편에게 
						targetLine = _v.x - _checkSlot.skillData.exeData.targetRange * 0.5f;
						
						if(targetLine < cm.monsterLeftLine) // 타겟이 우리편쪽에 있어야한다.
						{
							for(; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false || _tempChar == this) continue;
								
								// 타겟 데칼을 벗어나면 아웃.
								// 사실 아래 타겟 검사에서 해도 됨. 근데 충돌 검사 횟수를 줄이기위해서 한번 걸러내는 것임.
								if(_tempChar.cTransformPosition.x < targetLine)
								{
									break;
								}
								else
								{
									// 타겟 검사.
									if(_tempChar.hitByDistance(_v, _checkSlot.skillData.exeData.targetRange * 0.5f))
									{
										++targetUnitNum;
									}
								}
							}
						}
					}
					else // 내가 적에게.
					{
						targetLine = _v.x + _checkSlot.skillData.exeData.targetRange * 0.5f;
						
						if(targetLine > cm.monsterLeftLine) // 타겟이 최소한 적편에 걸쳐있기라도 해야한다.
						{
							// 내가 적에게 
							for(; index >= 0; --index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false) continue;
								
								// 타겟 데칼을 벗어나면 아웃.
								// 사실 아래 타겟 검사에서 해도 됨. 근데 충돌 검사 횟수를 줄이기위해서 한번 걸러내는 것임.
								if(_tempChar.cTransformPosition.x < _v.x - _checkSlot.skillData.exeData.targetRange * 0.5f)
								{
									break;
								}
								else
								{
									// 타겟 검사.
									if(_tempChar.hitByDistance(_v, _checkSlot.skillData.exeData.targetRange * 0.5f))
									{
										_skillMoveScoreCheckers[i].score += getScoreByMonsterType(_checkSlot.skillData,_tempChar);
									}
								}
							}
						}
					}
					
				}
				else // 
				{
					_v.x -= _checkSlot.skillData.targetAttr[0].Get(); // 데칼 중심점.
					
					if(_checkSlot.skillData.targetType == Skill.TargetType.ME)
					{
						targetLine = _v.x + _checkSlot.skillData.exeData.targetRange * 0.5f;
						
						if(targetLine > cm.playerMonsterRightLine) // 데칼 우측이 우리편쪽에 최소한 걸쳐야한다.
						{
							// 적이 자기편에게.
							for(; index < len; ++index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false || _tempChar == this) continue;
								
								// 타겟 데칼을 벗어나면 아웃.
								// 사실 아래 타겟 검사에서 해도 됨. 근데 충돌 검사 횟수를 줄이기위해서 한번 걸러내는 것임.
								if(targetLine < _tempChar.cTransformPosition.x)
								{
									break;
								}
								else
								{
									// 타겟 검사.
									if(_tempChar.hitByDistance(_v, _checkSlot.skillData.exeData.targetRange * 0.5f))
									{
										++targetUnitNum;
									}
								}
							}
							
						}
					}
					else // 적이 나에게.
					{
						targetLine = _v.x - _checkSlot.skillData.exeData.targetRange * 0.5f;
						
						if(targetLine <= cm.playerMonsterRightLine) // 데칼 좌측이 최소한 적편에 걸쳐야한다.
						{
							for(; index >= 0; --index)
							{
								_tempChar = monsterList[index];
								if(_tempChar.isEnabled == false) continue;
								
								// 타겟 데칼을 벗어나면 아웃.
								// 사실 아래 타겟 검사에서 해도 됨. 근데 충돌 검사 횟수를 줄이기위해서 한번 걸러내는 것임.
								if(_tempChar.cTransformPosition.x > _v.x + _checkSlot.skillData.exeData.targetRange * 0.5f)
								{
									break;
								}
								else
								{
									// 타겟 검사.
									if(_tempChar.hitByDistance(_v, _checkSlot.skillData.exeData.targetRange * 0.5f))
									{
										_skillMoveScoreCheckers[i].score += getScoreByMonsterType(_checkSlot.skillData,_tempChar);
									}
								}
							}
						}
					}
				}
				
				// 여기는 중심점에다가 상하좌우를 그려놓고
				// 캐릭터들의 충돌 검사를 한다.
				// 단 maxHit 제한이 걸려있으면 걔들은 처리해야지...
				// monster 검사.
			}
			
			_skillMoveScoreCheckers[i].score += targetUnitNum * unitPoint;
			_skillMoveScoreCheckers[i].score += targetHeroNum * heroPoint;
			_skillMoveScoreCheckers[i].score -= dangerPoints[i].score;
			_skillMoveScoreCheckers[i].distance = MathUtil.abs(cTransformPosition.x, _skillMoveScoreCheckers[i].pos.x);
			_skillMoveScoreCheckers[i].temp = targetUnitNum + targetHeroNum;

#if UNITY_EDITOR

//			if(BattleSimulator.nowSimulation == false)
//			{
//				Debug.Log(i + "최적위치점수 : "  + _skillMoveScoreCheckers[i].score + "   dp score: " + dangerPoints[i].score + "    sm pos:" + _skillMoveScoreCheckers[i].pos + "   dp pos: " + dangerPoints[i].pos);
//			}
#endif

		}
		
		_skillMoveScoreCheckers.Sort(_aISkillScoreSorter);
		_checkSlot = null;
		
		if(slot != null)
		{
			return (_skillMoveScoreCheckers[0].temp > 0);
		}
		
		//====================================================================
		// 타게팅 최적 위치를 찾았다!
		_pvpTargetPosition = _skillMoveScoreCheckers[0].pos;
		_pvpTargetPosition.x += (isPlayerSide)?hitObject.lineLeft:hitObject.lineRight;
		
		if(_nowSelectSkillSlot.skillData.exeData.type == 10 || _nowSelectSkillSlot.skillData.exeData.type == 11)
		{
			if(isPlayerSide)
			{
				if(_pvpTargetPosition.x + optionValue < cm.monsterLeftLine)
				{
					_pvpTargetPosition.x += optionValue;
				}
				
			}
			else 
			{
				if(_pvpTargetPosition.x + optionValue > cm.playerMonsterRightLine)
				{
					_pvpTargetPosition.x += optionValue;
				}
			}
		}
		else
		{
			// 최적 위치가 타겟존 근처에 있을때 그 위치까지 가기전에 일반공격을 하기 때문에 실제로는 접근을 제대로 못한다.
			// 그 결과 스킬을 쓰지 못하기 때문에. 타겟존 근처가 이동최전위치라면 보정값 100을 더해준다.

			if(attackData.isShortType)
			{
				if(isPlayerSide)
				{
					if(_pvpTargetPosition.x > cm.monsterLeftLine - stat.atkRange)
					{
						_pvpTargetPosition.x = cm.monsterLeftLine - stat.atkRange;
					}
				}
				else 
				{
					if(_pvpTargetPosition.x < cm.playerMonsterRightLine + stat.atkRange)
					{
						_pvpTargetPosition.x = cm.playerMonsterRightLine + stat.atkRange;
					}
				}
			}
			else
			{
				if(isPlayerSide)
				{
					if(_pvpTargetPosition.x > cm.monsterLeftLine - 150)
					{
						_pvpTargetPosition.x = cm.monsterLeftLine - 150;
					}
				}
				else 
				{
					if(_pvpTargetPosition.x < cm.playerMonsterRightLine + 150)
					{
						_pvpTargetPosition.x = cm.playerMonsterRightLine + 150;
					}
				}
			}

		}

		
//		Debug.LogError("Best Position : " + _skillMoveScoreCheckers[0].pos + "     score: " + _skillMoveScoreCheckers[0].score);
//		Debug.LogError("_pvpTargetPosition : " + _pvpTargetPosition);
		
		
		if(skillTargetingPositionMaxPoint < _skillMoveScoreCheckers[0].score)
		{
			skillTargetingPositionMaxPoint = _skillMoveScoreCheckers[0].score;
		}
		
		nowSkillMoveTargetZoneBestPoint = _skillMoveScoreCheckers[0].score;

		return true;
	}






	bool nowMoveToTargetPosition = false;
//	bool nowMoveToTargetPosition
//	{
//		set
//		{
//
//			_nowMoveToTargetPosition = value;
////			Debug.LogError("## _nowMoveToTargetPosition : " + _nowMoveToTargetPosition);
//		}
//		get
//		{
//			return _nowMoveToTargetPosition;
//		}
//	}


	public void setMovingDirection(MoveState st, bool lookDirection = false)
	{
		prevMoveState = moveState;
		moveState = st;
		nowMoveToTargetPosition = false;

		if(lookDirection)
		{
			if(st == MoveState.Stop)
			{
				attackData.lookDirection(this, (isPlayerSide)?100.0f:-100.0f);
			}
		}
	}


	private IVector3 currentTargetMovingPosition = IVector3.zero;

	// 특정 지점으로 이동하여 위치 유지.
	public void setStateAndDirectionByTargetPosition(IVector3 targetPosition)
	{
//		Debug.LogError("setStateAndDirectionByTargetPosition : "+ targetPosition + "   ct: " + cTransformPosition);

		// 새 위치가 현재 
		if(MathUtil.abs(cTransformPosition.x, targetPosition.x ) < 20.0f)
		{
			moveState = MoveState.Stop;
			state = NORMAL;
			return;
		}

		nowMoveToTargetPosition = true;

		if(isPlayerSide == false)
		{
			if(targetPosition.x < cTransformPosition.x)
			{
				moveState = MoveState.Forward;
			}
			else
			{
				moveState = MoveState.Backward;
			}
		}
		else
		{
			if(targetPosition.x > cTransformPosition.x)
			{
				moveState = MoveState.Forward;
			}
			else
			{
				moveState = MoveState.Backward;
			}
		}

//		Debug.LogError("setStateAndDirectionByTargetPosition moveState: "+ moveState);

		currentTargetMovingPosition = targetPosition;

	}




	float getScoreByMonsterType(HeroSkillData skillData, Monster mon)
	{
		if(mon.isHero)
		{
			if(skillData.skillType == Skill.Type.ATTACK)
			{
				if(mon.hpPer < 0.2f)
				{
					return 100.0f;
				}
				else
				{
					++targetHeroNum;
				}
			}
		}
		else ++targetUnitNum;

		return 0;
	}

}
