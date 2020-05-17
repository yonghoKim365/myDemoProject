using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


sealed public partial class Player : Monster
{
	int _dangerPointLen = 0;
	int _readyModeMoveListLen = 0;
	int _battleModeOnLen = 0;
	int _battleModeOffLen = 0;
	int _attachSkillMoveLen = 0;
	int _unitRunePointCheckerLen = 0;
	int _skillRunePointCheckerLen = 0;

	private PlayerAiData[] _dangerPointChecker;
	private PlayerAiData _battleStateChecker;
	private PlayerAiData[] _readyModeMove;
	private PlayerAiData[] _battleModeOnChecker;
	private PlayerAiData[] _battleModeOffChecker;
	private PlayerAiData _safeZoneChecker;

	private PlayerAiData[] _attachSkillMoveChecker;

	public PlayerAiData unitRuneSelectRarePoint;
	public PlayerAiData skillRuneSelectRarePoint;

	public PlayerAiData unitRuneSelectCalcValue;
	public PlayerAiData skillRuneSelectCalcValue;

	public PlayerAiData[] unitRunePointChecker;
	public PlayerAiData[] skillRunePointChecker;

	public PlayerAiData targetingPointHeroChecker;
	public PlayerAiData targetingPointEnemyUnitChecker;
	public PlayerAiData targetingPointMyUnitChecker;

	public PlayerAiData targetingPositionOption;

	private PlayerAiData _nValue;

	public PlayerAiData unitActiveSkillChecker;

	public PlayerAiData changePlayerChecker;



	IVector3 _checkPosition = IVector3.zero;
	AIScore _scoreSlot;

	static List<PlayerAiData> _dangerPointCheckerList = new List<PlayerAiData>();
	static List<PlayerAiData> _readyModeMoveList = new List<PlayerAiData>();
	static List<PlayerAiData> _battleModeOnCheckerList = new List<PlayerAiData>();
	static List<PlayerAiData> _battleModeOffCheckerList = new List<PlayerAiData>();

	static List<PlayerAiData> _unitRunePointCheckerList = new List<PlayerAiData>();

	static List<PlayerAiData> _attachSkillMoveCheckerList = new List<PlayerAiData>();
	static List<PlayerAiData> _skillRunePointCheckerList = new List<PlayerAiData>();

	// AI 세팅.

	public bool isSetAi = false;

	public void setAI(string aiGroupId)
	{
		string aiIds;

		if(aiGroupId != null && GameManager.info.aiGroupData.ContainsKey(aiGroupId))
		{  
			aiIds = GameManager.info.aiGroupData[aiGroupId].data;
		}
		else return;

#if UNITY_EDITOR
		Debug.LogError("aiGroupId : " + aiGroupId + "   isPlayerSide: " + isPlayerSide);
#endif

		string[] ais = aiIds.Split(',');
		int len = ais.Length;

		if(len < 10) return;

		for(int i = 0; i < len; ++i)
		{
			PlayerAiData ai = GameManager.info.playerAiData[ais[i]];

			switch(ai.type)
			{
				case PlayerAiData.Type.dangerPoint: 
				_dangerPointCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.battleState: 
				_battleStateChecker = ai;
				break;

			case PlayerAiData.Type.battleWait: 
				_readyModeMoveList.Add(ai);
				break;

			case PlayerAiData.Type.battleOn: 
				_battleModeOnCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.battleOff: 
				_battleModeOffCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.safeZone: 
				_safeZoneChecker = ai;
				break;

			case PlayerAiData.Type.attachSkillMove: 
				_attachSkillMoveCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.unitRuneSelectPoint: 
				if(ai.type2 == 0) unitRuneSelectRarePoint = ai;
				else unitRuneSelectCalcValue = ai;
				break;

			case PlayerAiData.Type.unitPoint: 
				_unitRunePointCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.skillRuneSelectPoint: 
//				Debug.LogError("ai.type2 : " + ai.type2);
				if(ai.type2 == 0)
				{
					skillRuneSelectRarePoint = ai;
				}
				else
				{
					skillRuneSelectCalcValue = ai;
				}
				break;

			case PlayerAiData.Type.skillPoint: 
				_skillRunePointCheckerList.Add(ai);
				break;

			case PlayerAiData.Type.targetZonePoint: 

				if(ai.type2 == 0) targetingPointHeroChecker = ai;
				else if(ai.type2 == 1) targetingPointEnemyUnitChecker = ai;
				else if(ai.type2 == 2) targetingPointMyUnitChecker= ai;
				break;

			case PlayerAiData.Type.targetZonePoint2: 
				targetingPositionOption = ai;
				break;

			case PlayerAiData.Type.targetZoneNvalue: 
				_nValue = ai;
				break;
			case PlayerAiData.Type.unitActiveSkill:
				unitActiveSkillChecker = ai;
				break;

			case PlayerAiData.Type.changePlayer:
				changePlayerChecker = ai;
				break;
			}
		}

		_dangerPointCheckerList.Sort(PlayerAiData.sort);
		_dangerPointChecker = _dangerPointCheckerList.ToArray();
		_dangerPointLen = _dangerPointChecker.Length;
		_dangerPointCheckerList.Clear();

		_readyModeMoveList.Sort(PlayerAiData.sort);
		_readyModeMove = _readyModeMoveList.ToArray();
		_readyModeMoveListLen = _readyModeMove.Length;
		_readyModeMoveList.Clear();

		_battleModeOnCheckerList.Sort(PlayerAiData.sort);
		_battleModeOnChecker = _battleModeOnCheckerList.ToArray();
		_battleModeOnLen = _battleModeOnChecker.Length;
		_battleModeOnCheckerList.Clear();

		_battleModeOffCheckerList.Sort(PlayerAiData.sort);
		_battleModeOffChecker = _battleModeOffCheckerList.ToArray();
		_battleModeOffLen = _battleModeOffChecker.Length;
		_battleModeOffCheckerList.Clear();

		_attachSkillMoveCheckerList.Sort(PlayerAiData.sort);
		_attachSkillMoveChecker = _attachSkillMoveCheckerList.ToArray();
		_attachSkillMoveLen = _attachSkillMoveChecker.Length;
		_attachSkillMoveCheckerList.Clear();


		_skillRunePointCheckerList.Sort(PlayerAiData.sort);
		skillRunePointChecker = _skillRunePointCheckerList.ToArray();
		_skillRunePointCheckerLen = skillRunePointChecker.Length;
		_skillRunePointCheckerList.Clear();


		_unitRunePointCheckerList.Sort(PlayerAiData.sort);
		unitRunePointChecker = _unitRunePointCheckerList.ToArray();
		_unitRunePointCheckerLen = unitRunePointChecker.Length;
		_unitRunePointCheckerList.Clear();

		isSetAi = true;

	}




	public float getUnitRunePoint(UnitSlot slot)
	{
		float returnValue = 0;
		for(int i = unitRunePointChecker.Length - 1; i >= 0; --i)
		{
			returnValue += unitRunePointChecker[i].getUnitRunePoint(this,slot);
		}
		return returnValue;
	}

	public float getSkillRunePoint(SkillSlot slot)
	{
		float returnValue = 0;
		for(int i = unitRunePointChecker.Length - 1; i >= 0; --i)
		{
			returnValue += skillRunePointChecker[i].getSkillRunePoint(this,slot);
		}
		return returnValue;
	}




















//	public static bool checkFuck = false;

	private IVector3 _tempCheckPosition = new IVector3();
	private float _tempCheckX = 9000.0f;
	public int onOffCheckNextMovingPosIndex = 0;

	// 위험도 체크.
	public void checkDangerZone()
	{
		//player는 0번이 우측이다. // 그럼 끝부터 검사한다.
		//monster는 0번이 좌측이다. // 0부터 검사한다.	

		// 10개 구간으로 나눈다. 0번은 적 캐릭터가 갈 수 있는 가장 좌측이다.
		// 여기에서 부터 100cm 단위로 구간을 나누어 검사를 한다.
		_checkPosition = cTransformPosition;
		_tempCheckPosition = cTransformPosition;
		_tempCheckX = 9000.0f;
		onOffCheckNextMovingPosIndex = 0;
		int i,j;

//		checkFuck = false;

		// 위험도 계산.
		if(isPlayerSide)
		{
			for(i = 0; i < 10; ++i)
			{
				_scoreSlot = dangerPoints[i];
				_scoreSlot.index = i;
				_scoreSlot.score = 0;
				
				_checkPosition.x = cm.monsterLeftLine - (i * 100.0f);// - hitObject.lineRight;

				// 갈곳이 전방...
				if(_checkPosition.x >= cTransformPosition.x && i > onOffCheckNextMovingPosIndex)
				{
					onOffCheckNextMovingPosIndex = i;
				}


				if(_checkPosition.x <= StageManager.mapStartPosX.Get())
				{
					// 위치를 전방에서부터 100cm 간격으로 잡으니까 시작/끝 위치를 넘어가면 걔들은 어차피 아웃이다. 
					// 고로 -10000점을 줘서 계산대상에서 제외해버린다.
					_scoreSlot.score = -10000.0f; 
					continue;
				}

				_scoreSlot.pos = _checkPosition;
				
				for(j = 0; j < _dangerPointLen; ++j)
				{
					_scoreSlot.score += _dangerPointChecker[j].getDangerPoint(this, _checkPosition);
				}

//				Debug.Log(i + "위험 점수계산 : "  + _scoreSlot.score + "    " + _scoreSlot.pos);
			}
		}
		else
		{
			for(i = 0; i < 10; ++i)
			{
				_scoreSlot = dangerPoints[i];
				_scoreSlot.index = i;
				_scoreSlot.score = 0;
				
				_checkPosition.x = cm.playerMonsterRightLine + (i * 100.0f);// - hitObject.lineLeft;


				// 갈곳이 전방...
				if(_checkPosition.x <= cTransformPosition.x && i > onOffCheckNextMovingPosIndex)
				{
					onOffCheckNextMovingPosIndex = i;
				}

				if(_checkPosition.x >= StageManager.mapEndPosX.Get())
				{
					_scoreSlot.score = -10000.0f;
					continue;
				}
				
				_scoreSlot.pos = _checkPosition;
				
				for(j = 0; j < _dangerPointLen; ++j)
				{
					_scoreSlot.score += _dangerPointChecker[j].getDangerPoint(this, _checkPosition);
				}

//				Debug.Log(i + "위험 점수계산 : "  + _scoreSlot.score + "    " + _scoreSlot.pos);

			}
		}
//		checkFuck = true;

		// 현재 위치 위험도.
		dangerPoints[Player.CURRENT_POSITION_INDEX].pos = cTransformPosition;
		dangerPoints[Player.CURRENT_POSITION_INDEX].score = 0;
		for(j = 0; j < _dangerPointLen; ++j)
		{
			dangerPoints[Player.CURRENT_POSITION_INDEX].score += _dangerPointChecker[j].getDangerPoint(this, cTransformPosition);
		}

#if UNITY_EDITOR
//		if(BattleSimulator.nowSimulation == false)
//		{
//			Debug.LogError("현재위치 위험 점수계산 : "  + dangerPoints[Player.CURRENT_POSITION_INDEX].score + "    " + dangerPoints[Player.CURRENT_POSITION_INDEX].pos);
//		}
#endif
	}


	public bool isBattleWaitMode = true;
	public bool isDefaultAttackOn = false;
	public float defaultAttackPositionX = 0.0f;

	public void aiNormalMove()
	{
		// 전투 대기 상태.
		// checkNormalAi에 의해서 전진, 후진, 정지가 정해진다.
		if(isBattleWaitMode == true)
		{
			if(moveState == MoveState.Forward)
			{
				moveForward();
			}
			else if(moveState == MoveState.Backward)
			{
				moveBackward();
			}
			else
			{
				attackData.lookDirection(this,fowardDirectionValue);
			}
		}
		// 전투 진행 상태.
		else
		{
			// 기본 공격 상태는 기존 로직대로 그냥 전진하면 된다.
			// 단 전진 가능한 거리의 제한을 둔다.
			// 이건 line 타입일때만 먹는다.
			if(isDefaultAttackOn) 
			{
				if(isPlayerSide) 
				{
					cm.monsterLimitLine.Set( defaultAttackPositionX );
				}
				else
				{
					cm.playerLimitLine.Set( defaultAttackPositionX);
				}

				action.doMotion();
			}
			else
			{
				// 안전한 위치로 이동.
				if(moveState == MoveState.Forward)
				{
					moveForward();
				}
				else if(moveState == MoveState.Backward)
				{
					moveBackward();
				}
				else
				{
					if(isPlayerSide) 
					{
						cm.monsterLimitLine.Set( -99999) ; 
					}
					else
					{
						cm.playerLimitLine.Set( 99999);
					}

					action.doMotion();
					//attackData.lookDirection(this,fowardDirectionValue);
				}
			}
		}
	}



	public const float NORMAL_MOVE_CHECK_DELAY = 0.3f;
	private float _nowNormalAiCheckDelay = 0.0f;




	public void checkNormalAi()
	{
//		Debug.Log("_nowNormalAiCheckDelay : " + _nowNormalAiCheckDelay);
		if(_nowNormalAiCheckDelay > 0)
		{
			_nowNormalAiCheckDelay -= GameManager.globalDeltaTime;
			return;
		}
		else
		{
			_nowNormalAiCheckDelay = NORMAL_MOVE_CHECK_DELAY;
		}

		isBattleWaitMode = _battleStateChecker.battleStateIsReady(this);



//			if(!isPlayerSide) Debug.LogError("isBattleWaitMode : " + isBattleWaitMode);


		//checkMovingTargetPosition();

		// 전투 대기 상태.
		if(isBattleWaitMode)
		{
			for(i = 0; i < _readyModeMoveListLen; ++i)
			{
				if(_readyModeMove[i].battleWaitMove(this)) // 걸렸으면 이걸 진행한다는 뜻이다...
				{
					break;
				}
			}
		}
		// 전투 진행 상태.
		else
		{
			// 위험도 계산!
			checkDangerZone();


			isDefaultAttackOn = false;
			bool canCheckDefaultAttackOn = true;

			// 기본공격 off 조건.
			for(i = 0; i < _battleModeOffLen ; ++i)
			{
				if(_battleModeOffChecker[i].checkDefaultAttackOnOff(this))
				{
					canCheckDefaultAttackOn = false;
//					if(!isPlayerSide) Debug.LogError("기본공격 off : 안전한 위치로 이동");
					break;
				}
			}
			
			if(canCheckDefaultAttackOn == true)
			{
				// 기본공격 on 조건.
				for(i = 0; i < _battleModeOnLen ; ++i)
				{
					if(_battleModeOnChecker[i].checkDefaultAttackOnOff(this))
					{
						isDefaultAttackOn = true;
//						if(!isPlayerSide) Debug.LogError("기본공격 on");
						break;
					}
				}
			}

			_scoreSlot = null;

			// 2-1) 기본공격 위치.
			//  기본공격 위치 = 타겟존 - 히어로 기본 공격거리
			// 그냥 얘는 기존처럼 전진하면 된다.

			// 기본공격 On인 상태.

			if(isDefaultAttackOn)
			{
				//공격거리 < 210 일때							
				//(기존과 동일) 기본 공격 위치 = 타겟존 - 히어로 기본공격거리 위치로 이동							
				//default 공격이 체크되면 action.doMotion으로 자동으로 움직일거다...
				if(stat.atkRange < 210)
				{
					moveState = MoveState.Forward;
					setMovingDirection(MoveState.Forward);	
				}
				else
				{
												
					float defaultAttackerTargetPostion = 0.0f;

					//근접 아군 유닛 개수가 0마리 이하면, 기본 공격 위치 = 타겟존 - 200 위치로 이동하고 
					if(cm.checkAliveShortUnitNum(isPlayerSide, 0))
					{
						if(isPlayerSide)
						{
							defaultAttackerTargetPostion = GameManager.me.characterManager.targetZoneMonsterLine - 200.0f;
						}
						else
						{
							defaultAttackerTargetPostion = GameManager.me.characterManager.targetZonePlayerLine + 200.0f;
						}
					}
					//아니면, 기본 공격 위치 = 타겟존 - 히어로 기본공격거리					
					else
					{
						if(isPlayerSide)
						{
							defaultAttackerTargetPostion = GameManager.me.characterManager.targetZoneMonsterLine - stat.atkRange;
						}
						else
						{
							defaultAttackerTargetPostion = GameManager.me.characterManager.targetZonePlayerLine + stat.atkRange;
						}
					}


					if(cTransformPosition.x < defaultAttackerTargetPostion)
					{
						if(isPlayerSide)
						{
							defaultAttackPositionX = defaultAttackerTargetPostion;
							moveState = MoveState.Forward;
							setMovingDirection(MoveState.Forward);	
						}
						else
						{
							_v = cTransformPosition;
							_v.x = defaultAttackerTargetPostion;
							setStateAndDirectionByTargetPosition(_v);
							isDefaultAttackOn = false;
							return;
						}
					}
					else
					{
						if(isPlayerSide)
						{
							_v = cTransformPosition;
							_v.x = defaultAttackerTargetPostion;
							setStateAndDirectionByTargetPosition(_v);
							isDefaultAttackOn = false;
							return;
						}
						else
						{
							defaultAttackPositionX = defaultAttackerTargetPostion;
							moveState = MoveState.Forward;
							setMovingDirection(MoveState.Forward);	
						}
					}
				}
			}





			if(isDefaultAttackOn == false) // 기본공격 off일때!
			{
				float targetZoneLine = (isPlayerSide)?cm.targetZoneMonsterLine:cm.targetZonePlayerLine;


				// 3-1 안전한 위치.
				float lowDPoint = 1000;

				for(i = 0; i < 10; ++i)
				{
//					Debug.Log(dangerPoints[i].score + "  " + dangerPoints[i].pos);

					if(dangerPoints[i].score > -9999)
					{
						if(dangerPoints[i].score < lowDPoint) lowDPoint = dangerPoints[i].score;

						if(dangerPoints[i].score <= _safeZoneChecker.value)
						{
							if(_scoreSlot == null)
							{
								_scoreSlot = dangerPoints[i];
							}
							else
							{
								if(VectorUtil.Distance(dangerPoints[i].pos.x,targetZoneLine) < VectorUtil.Distance(_scoreSlot.pos.x,targetZoneLine))
								{
									_scoreSlot = dangerPoints[i];
								}
							}
						}
					}
				}

				if(_scoreSlot == null)
				{
					for(i = 0; i < 10; ++i)
					{
						if(dangerPoints[i].score > -9999 && dangerPoints[i].score <= lowDPoint)
						{
							if(_scoreSlot == null)
							{
								_scoreSlot = dangerPoints[i];
							}
							else
							{
								if(VectorUtil.Distance(dangerPoints[i].pos.x,targetZoneLine) < VectorUtil.Distance(_scoreSlot.pos.x,targetZoneLine))
								{
									_scoreSlot = dangerPoints[i];
								}
							}
						}
					}
				}


				if(_scoreSlot != null)
				{
//					if(!isPlayerSide) Debug.LogError("안전한 위치 : " + _scoreSlot.pos);
					// 안전한 위치는 여기다.
					// 그 곳으로 향하도록 셋팅한다.
					setStateAndDirectionByTargetPosition(_scoreSlot.pos);
				}
				else
				{
					setMovingDirection(MoveState.Stop);	
				}

			}
		}
	}



	void updateSkillMoveAI()
	{
//////////////////////////////// [  1. 스킬 모드 진입. ]  ////////////


		//==- 스킬 모드시 타게팅 위치로 이동한다. ===//
		if(_nowSkillTargetCheckDelay > 0)// && _nowSelectSkillSlot != null)
		{
			_nowSkillTargetCheckDelay -= GameManager.globalDeltaTime; 
			return;
		}
		
		// 스킬 모드 이동 검사.
		_nowSkillTargetCheckDelay = SKILLTARGET_CHECK_DELAY;

#if UNITY_EDITOR

		if(_nowSelectSkillSlot == null)
		{
			Debug.LogError("ERRROR!!!!");
		}
#endif


		//타게팅타입 2,3 번일 때는 즉시 차징하고 타게팅 위치고 나발이고 없다. 완료후 바로 쏜다. 타게팅 위치도 안찾고 그냥 일반모드 이동을 시킨다.
		if(_nowSelectSkillSlot.skillData.targeting > 1 )
		{
			if(nowChargingSkill == null) _nowSelectSkillSlot.use();
			else if(chargingTime >= chargingTimeLimit)
			{
				_nowSelectSkillSlot.onPress(false); 
			}
		}
		else
		{
			//1. 최적위치 찾고.
			//2. 도달시간 계산.
			//3. 남은 풀차징 시간은 항상 계산중.
			//4. 최적위치 max값은 항상 갱신.

			// 0,1번 타케팅 타입의 이동 로직이다.
			checkSkillType_0_1_BestTargetingPosition();

			//	타게팅 최적위치 도달시간(ms) = (현재위치와의 거리 / 이동속도) * 1000							
			_skillMoveLeftTime = VectorUtil.Distance(_pvpTargetPosition.x, cTransformPosition.x) / stat.speed;
//			Debug.Log("_pvpTargetPosition.x : " + _pvpTargetPosition.x + "     cTransformPosition.x : " + cTransformPosition.x);


			checkSkillChargingStart();

			//N =	100 - (스킬모드 진입 후 경과시간 * 5)
			IFloat nValue = 100.0f - (skillModeProgressTime * 7.0f);//((float)_nValue.value - (fullChargingTime * 10.0f));

//			Debug.LogError("nValue : " + nValue + "     스킬모드 진입 후 경과시간:  " + skillModeProgressTime);

			if(skillMoveIsNormal)
			{
				// 스킬이동 판단.
//				Debug.Log("_skillMoveLeftTime : " + _skillMoveLeftTime + "  leftFullChargingTime : " + leftFullChargingTime);
//				Debug.Log("nowSkillMoveTargetZoneBestPoint : " + nowSkillMoveTargetZoneBestPoint + "  skillTargetingPositionMaxPoint : " + skillTargetingPositionMaxPoint + "    calc: " + (skillTargetingPositionMaxPoint * nValue * 0.01f));

				if(_skillMoveLeftTime > leftFullChargingTime && nowSkillMoveTargetZoneBestPoint > skillTargetingPositionMaxPoint * nValue * 0.01f)
				{
					skillMoveIsNormal = false;
					//최적위치로 이동하게 해준다.
					setStateAndDirectionByTargetPosition(_pvpTargetPosition);
				}
			}
			else
			{
//				Debug.Log("nowSkillMoveTargetZoneBestPoint : " + nowSkillMoveTargetZoneBestPoint + "   skillTargetingPositionMaxPoint : "+ skillTargetingPositionMaxPoint);

				// 스킬이동 해제여부 판단.
				if(nowSkillMoveTargetZoneBestPoint < skillTargetingPositionMaxPoint * nValue * 0.01f)
				{
//					Debug.Log("일반모드 전환");
					// 일반 모드로 전환.
					_nowNormalAiCheckDelay = 0.0f;
					skillMoveIsNormal = true;
				}
				else
				{
//					Debug.Log("스킬발사 여부 검사");
					// 스킬 발사 가능한지 검사.
					if(checkSkillShoot() == false)
					{
						setStateAndDirectionByTargetPosition(_pvpTargetPosition);
					}
				}
			}
		}
	}


	void setDirectionToBestSkillTargingPosition()
	{
		if(isPlayerSide == false)
		{
			if(_skillMoveLeftTime >= leftFullChargingTime)
			{
				if(_pvpTargetPosition.x < cTransformPosition.x)
				{
					setMovingDirection(MoveState.Forward);	
				}
				else
				{
					setMovingDirection(MoveState.Backward);	
				}
			}
			else
			{
				if(_pvpTargetPosition.x > cTransformPosition.x)
				{
					// 최적 이동.
					setMovingDirection(MoveState.Backward);	
				}
				else
				{
					setMovingDirection(MoveState.Stop);	
					state = NORMAL;
				}
			}
		}
		else
		{
			
			if(_skillMoveLeftTime >= leftFullChargingTime)
			{
				if(_pvpTargetPosition.x > cTransformPosition.x)
				{
					setMovingDirection(MoveState.Forward);	
				}
				else
				{
					setMovingDirection(MoveState.Backward);	
				}
			}
			else
			{
				if(_pvpTargetPosition.x < cTransformPosition.x)
				{
					// 최적 이동.
					setMovingDirection(MoveState.Backward);				
				}
				else
				{
					setMovingDirection(MoveState.Stop);
					state = NORMAL;
				}
			}
		}

	}


	bool checkSkillShoot()
	{
		if(nowChargingSkill != null)
		{
//			Debug.LogError("checkSkillShoot !");
// 풀차징 하고 7초 지났으면 무조건 발사.

//			Debug.LogError("chargingTime : " + chargingTime + "  chargingTimeLimit: " + chargingTimeLimit  );

			if(chargingTime >= chargingTimeLimit && (VectorUtil.Distance(_pvpTargetPosition.x, cTransformPosition.x) < timeAfterFullCharging * 10.0f || timeAfterFullCharging > 3.0f) )
			{
//				Debug.LogError("  스킬 발사!");
				_nowSelectSkillSlot.onPress(false);
				return true;
			}
		}

		return false;
	}

	IFloat _skillMoveLeftTime = 0.0f;

	void checkSkillChargingStart()
	{
		//		<STEP2 타게팅 최적위치 도달시간 / 풀차징 시간 계산>								

		//				풀차징 시간 (ms) = 풀차징까지의 남은시간							
		//<타게팅 최적위치 도달시간 / 풀차징 시간 계산>

		// 대기중일때.
		if(nowChargingSkill == null)
		{
			// 조건 ok! 스킬 차징!
//			ff Log.log("leftMoveTime:",leftMoveTime,"leftFullChargingTime:",leftFullChargingTime,"MathUtil.abs( leftMoveTime, leftFullChargingTime ):",MathUtil.abs( leftMoveTime, leftFullChargingTime ),"SetupData.skill_ctl_2: ",SetupData.skill_ctl_2*0.001f);

//			Debug.Log("_skillMoveLeftTime : " + _skillMoveLeftTime + "  leftFullChargingTime : " + leftFullChargingTime + "  abs: " + (MathUtil.abs( _skillMoveLeftTime, leftFullChargingTime )) );

			if((_skillMoveLeftTime < leftFullChargingTime || MathUtil.abs( _skillMoveLeftTime, leftFullChargingTime ) < 0.05f))

			{
//				Debug.Log(" USE SKILL SLOT!");
				//ff Log.log(" USE SKILL SLOT!");
				_nowSelectSkillSlot.use();
			}
		}

		//대기중 & 타게팅최적위치도달시간 < 풀차징시간				차징/타게팅		
		//"대기중 & 타게팅최적위치도달시간 ≒ 풀차징시간 (오차 50ms 미만)"				차징/타게팅		
		//"풀차징 & 현재위치 ≒ 타게팅최적위치 (오차 = 풀차징경과시간(초) * 10cm)"				발사	
	}



	public int getUnitRuneSelectPoint(GameIDData selectUnit)
	{
		//N번 소환룬의 룬 선택포인트 = 장비 레어도 점수 - 레벨차 포인트 * 1 + 룬 강화레벨 * 1							
		//int lvPoint = (;
		int equipPoint = getUnitRuneEquipRarePoint();
		int lvPoint = MathUtil.abs(equipPoint, (selectUnit.rare+1)*10);

		return equipPoint - lvPoint * unitRuneSelectCalcValue.attr[0] + selectUnit.reinforceLevel * unitRuneSelectCalcValue.attr[1];
	}


	public int getSkillRuneSelectPoint(GameIDData selectSkill)
	{
		//N번 소환룬의 룬 선택포인트 = 장비 레어도 점수 - 레벨차 포인트 * 1 + 룬 강화레벨 * 1							
		//int lvPoint = (;
		int equipPoint = getSkillRuneEquipRarePoint();
		int lvPoint = MathUtil.abs(equipPoint, (selectSkill.rare+1)*10);
		
		return equipPoint - lvPoint * skillRuneSelectCalcValue.attr[0] + selectSkill.reinforceLevel * skillRuneSelectCalcValue.attr[1];
	}



	public int getUnitRuneEquipRarePoint()
	{
		// * 장비 레어도 점수 = (의상 장비의 레어도 점수 + 타는펫 장비의 레어도 점수) / 2       (스킬 AI 는 모자와 무기)									
		return (unitRuneSelectRarePoint.attr[playerData.partsBody.rare] + unitRuneSelectRarePoint.attr[playerData.partsVehicle.rare]) / 2;
	}


	public int getSkillRuneEquipRarePoint()
	{
		// * 장비 레어도 점수 = (모자 장비의 레어도 점수 + 무기 장비의 레어도 점수) / 2   						
		return (unitRuneSelectRarePoint.attr[playerData.partsHead.rare] + unitRuneSelectRarePoint.attr[playerData.partsWeapon.rare]) / 2;
	}





}
