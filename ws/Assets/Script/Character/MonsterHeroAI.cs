using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class MonsterHeroAI
{
	const string IGNORE = "IGNORE";

	IFloat tempF;
	int tempI;
	
	public string id;
	public int[] hpMinMax;
	public int[] playerHpMinMax;
	public int[] spMinMax;
	public int[] mpMinMax;
	public int[] totalMinMax;
	
	public int[] unitNumMinMax;
	public int[] eUnitNumMinMax;
	public int[] unitNumConMinMax; //아군대비상대소환유닛개수
	public int[] unitTHPMinMax;
	
	public int[] zoneUnitNumMinMax;
	public int[] zonePlayerUnitNumMinMax;
	public int[] zoneUnitNumConMinMax;
	public int[] zoneTHPMinMax;
	
	public int[] distTZoneMinMax;
	public int[] maxTzoneMinMax;
	
	public int[] distStartMinMax;
	public int[] distEndLineMinMax;

	public int[] distFromHeroMinMax;

	public int targetInDefaultTargetZone;
	
	public string didSkill; // 상대스킬발동????
	public int[] canTargeting;
	
	public int summonSlotNum;
	public int skillNum;
	
	public int[] playTimeMinMaxSinceStart;
	
	public IFloat coolTime;
	public IFloat coolTimeStartDelay;
	
	public int executeChange;
	
	public int maxExcuteNum;
	
	//이동
	public IFloat moveDistanceFromTargetZone;
	public int actionPosition;
	
	public int[] skillPosition;

	
	// 액션
	public int actionDefaultAttack;
	public int[] actionSummonSlotNum;
	public IFloat actionSummonSlotPosX = 0.0f;
	
	public int[] actionSkill;
	

	public int actionSkill2;
	public IFloat actionSkillDelay = 0.0f;
	public int actionSkillTarget = 0;
	public IFloat actionSkillDist = 0.0f;

	public string nextAi;

	public string actionAni = null;

	public bool setAniIgnore = false;
	public bool ignoreIdleAni = false;

	public MonsterHeroAI ()
	{
		
	}
	
	string _tstr;
	private void parser(object obj, out int[] arr, Checker callback)
	{

		#if UNITY_EDITOR
		try
		{
			#endif

		_tstr = (string)obj;
		if(string.IsNullOrEmpty(_tstr))
		{
			arr = null;
			return;
		}
		string[] ta = _tstr.Split(',');
		arr = new int[ta.Length];

		int.TryParse(ta[0], out arr[0]);
		int.TryParse(ta[1], out arr[1]);

		tempCheckers.Add(callback);

			#if UNITY_EDITOR
		}
		catch(Exception e)
		{
			Debug.LogError(id + "  "  + obj + "   " +  e.Message);
			arr = null;

		}
				#endif

	}
	
	private void parser(object obj, out int intVar, Checker callback)
	{
		if(obj is int)
		{
			intVar = (int)obj;
			tempCheckers.Add(callback);
		}
		else if(obj is  long)
		{
			intVar = (int)(long)obj;
			tempCheckers.Add(callback);
		}
		else
		{
			intVar = -1;
		}
	}
	

	private void parser(object obj, out int intVar, Checker callback, int defaultValue)
	{
		if(obj is int)
		{
			intVar = (int)obj;
			tempCheckers.Add(callback);
		}
		else if(obj is  long)
		{
			intVar = (int)(long)obj;
			tempCheckers.Add(callback);
		}
		else
		{
			intVar = defaultValue;
			if(intVar > -1)
			{
				tempCheckers.Add(callback);
			}
		}
	}	

	
	private void parser(object obj, out float fVar, Checker callback)
	{
		Util.parseObject(obj, out fVar, true, -1.0f);
		if(fVar > -1)
		{
			tempCheckers.Add(callback);
		}
	}		



	private void parser(object obj, out IFloat fVar, Checker callback)
	{
		Util.parseObject(obj, out fVar, true, -1.0f);
		if(fVar > -1)
		{
			tempCheckers.Add(callback);
		}
	}	

/// <summary>
/// S ==/// </summary>
/// <param name="l">L.</param>
/// <param name="k">K.</param>


	private void firstParser(object obj, out int[] arr, Checker callback)
	{
//		Debug.Log(obj);
		_tstr = (string)obj;
		if(string.IsNullOrEmpty(_tstr))
		{
			arr = null;
			return;
		}
		string[] ta = _tstr.Split(',');
		arr = new int[ta.Length];
		int.TryParse(ta[0], out arr[0]);
		int.TryParse(ta[1], out arr[1]);
		
		tempFirstCheckers.Add(callback);
	}
	
	private void firstParser(object obj, out int intVar, Checker callback)
	{
		if(obj is int)
		{
			intVar = (int)obj;
			tempFirstCheckers.Add(callback);
		}
		else if(obj is  long)
		{
			intVar = (int)(long)obj;
			tempFirstCheckers.Add(callback);
		}
		else
		{
			intVar = -1;
		}
	}
	
	
	private void firstParser(object obj, out int intVar, Checker callback, int defaultValue)
	{
		if(obj is int)
		{
			intVar = (int)obj;
			tempFirstCheckers.Add(callback);
		}
		else if(obj is  long)
		{
			intVar = (int)(long)obj;
			tempFirstCheckers.Add(callback);
		}
		else
		{
			intVar = defaultValue;
			if(intVar > -1)
			{
				tempFirstCheckers.Add(callback);
			}
		}
	}	
	
	
	private void firstParser(object obj, out float fVar, Checker callback)
	{
		Util.parseObject(obj, out fVar, -1.0f);
		if(fVar > -1.0f)
		{
			tempFirstCheckers.Add(callback);
		}
	}		





	
	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		
		tempCheckers.Clear();
		tempFirstCheckers.Clear();
		
		parser(l[k["HP"]], out hpMinMax, checkHP);
		parser(l[k["E_HP"]], out playerHpMinMax, checkEHP);
		parser(l[k["SP"]], out spMinMax, checkSP);
		parser(l[k["MP"]], out mpMinMax, checkMP);
		parser(l[k["TOTAL_HP"]], out totalMinMax, checkTotalUnitHP);
		parser(l[k["UNIT_NUM"]], out unitNumMinMax, checkAliveUnitNum);
		parser(l[k["E_UNIT_NUM"]], out eUnitNumMinMax, checkPlayerAliveUnitNum);
		parser(l[k["UNIT_NUM_CON"]], out unitNumConMinMax , checkUnitNumCondition);
		parser(l[k["UNIT_THP"]], out unitTHPMinMax, checkUnitNumTHP);
		parser(l[k["ZONE_UNIT_NUM"]], out zoneUnitNumMinMax, checkPlayerUnitNumInTargetZone);
		parser(l[k["ZONE_E_UNIT_NUM"]], out zonePlayerUnitNumMinMax, checkMonsterUnitNumInPlayerTargetZone);
		parser(l[k["ZONE_UNIT_NUM_CON"]], out zoneUnitNumConMinMax, checkZoneUnitNumCondition);
		parser(l[k["ZONE_THP"]], out zoneTHPMinMax, checkTargetZoneTHP);
	

		// 거리는 최상위 조건 되겠다!
		if(l[k["DIST_TZONE"]] is string)
		{
			string[] dZone = ((string)l[k["DIST_TZONE"]]).Split('/');
			firstParser(dZone[0], out distTZoneMinMax,checkDistFromTargetZone );
			
			if(dZone.Length == 2)
			{
				firstParser(dZone[1], out maxTzoneMinMax,checkDistRecordedTargetZone );	
			}
		}
		
		parser(l[k["DIST_HERO"]], out distFromHeroMinMax, checkDistFromHero);

		parser(l[k["DIST_START"]], out distStartMinMax, checkDistFromStartPoint);
		parser(l[k["DIST_ENDLINE"]], out distEndLineMinMax, checkDistFromEndLine);
		parser(l[k["TARGET_IN_CZONE"]], out targetInDefaultTargetZone, checkEnemyInDefaultActionZone);
		
		
		// 미정 
		//parser(l[k["DID_SKILL"]], out didSkill , checkPlayerDidSkillToMe);
		
		parser(l[k["CAN_TARGETING"]], out canTargeting, checkCanTargetingSkill);
		
		
		parser(l[k["SUMMON_SLOT_NUM"]], out summonSlotNum, checkCanSummon);		
		parser(l[k["SKILL_NUM"]], out skillNum, checkCanUseSkill);
		
		playTimeMinMaxSinceStart = Util.stringToIntArray((string)l[k["PLAYTIME"]],',');
		if(playTimeMinMaxSinceStart.Length == 2)
		{
			tempCheckers.Add(checkPlayerTime);
		}


		if( l[k["COOLTIME"]] is string)
		{
			string[] ct = ((string)l[k["COOLTIME"]]).Split(',');



			if(ct.Length == 2)
			{
				IFloat.tryParse(ct[0], out coolTime);
				if(coolTime > -1.0f) tempCheckers.Add(checkCoolTime);
				IFloat.tryParse(ct[1],out coolTimeStartDelay);			
			}
			else
			{
				IFloat.tryParse(ct[0], out coolTime);
				if(coolTime > -1.0f) tempCheckers.Add(checkCoolTime);
				coolTimeStartDelay = 0.0f;
			}
		}
		else
		{
			parser(l[k["COOLTIME"]], out coolTime, checkCoolTime);
			coolTimeStartDelay = 0.0f;
		}



		parser(l[k["EXE_CHANCE"]], out executeChange, checkExeChange, 100);
		
		checkers = tempCheckers.ToArray();
		tempCheckers.Clear();		

		_firstCheckerNum = tempFirstCheckers.Count;
		firstCheckers = tempFirstCheckers.ToArray();
		tempFirstCheckers.Clear();

		Util.parseObject(l[k["EXE_MAX"]], out maxExcuteNum, true, -1);
		
		_checkerNum = checkers.Length;

		
		// 실행조건.
		Util.parseObject(l[k["MOVE_DIST_TZONE"]], out moveDistanceFromTargetZone, true, -1);
		
		Util.parseObject(l[k["ACT_POSITION"]], out actionPosition, true, -1);
		
		_tstr = (string)l[k["SKILL_POSITION"]];
		if(_tstr.Length > 0) Util.stringToIntArray(out skillPosition, (string)l[k["SKILL_POSITION"]], ',');
		else skillPosition = null;
		
		if(moveDistanceFromTargetZone > -1) moveType = MoveType.TARGETZONE;
		else if(actionPosition > -1) moveType = MoveType.ACTION_POSITION;
		else if(skillPosition != null) moveType = MoveType.SKILL_POSITION;
		else moveType = MoveType.NONE;
		
		// 액션...
		Util.parseObject(l[k["ACT"]], out actionDefaultAttack,  true, 0);
		
		actionSummonSlotNum = null;
		actionSummonSlotPosX = 0.0f;
		if(l[k["ACT_SUMMON"]] is string)
		{
			_tstr = (string)l[k["ACT_SUMMON"]];
			if(string.IsNullOrEmpty(_tstr) == false)
			{
				string[] temp = _tstr.Split('/');
				actionSummonSlotNum = Util.stringToIntArray(temp[0],',');
				IFloat.tryParse(temp[1], out actionSummonSlotPosX);
			}
		}
		
		_tstr = (string)l[k["ACT_SKILL"]];
		if(_tstr.Length > 0)
		{
			Util.stringToIntArray(out actionSkill, (string)l[k["ACT_SKILL"]], ',');

			if(actionSkill.Length > 1)
			{
				parser(l[k["CAN_TARGETING"]], out canTargeting, checkCanTargetingSkill);
			}

		}
		else actionSkill = null;		


		_tstr = (string)l[k["ACT_SKILL2"]];
		if(_tstr.Length > 0)
		{
			string[] as2 = _tstr.Split(',');
			Util.parseObject(as2[0],out actionSkill2, true, -1);
			Util.parseObject(as2[1],out actionSkillDelay, true, -1.0f);
			if(as2.Length > 2)
			{
				Util.parseObject(as2[2],out actionSkillTarget, true, -1);
				Util.parseObject(as2[3],out actionSkillDist, true, 0.0f);
			}

		}
		else
		{
			actionSkill2 = -1;
		}

		ignoreIdleAni = false;

		if(k.ContainsKey("PLAYANI"))
		{
			_tstr = (string)l[k["PLAYANI"]];
			
			if(_tstr.Length > 0)
			{
				string[] pani = _tstr.Split(',');
				
				switch(pani.Length)
				{
				case 1:
					if(string.IsNullOrEmpty(pani[0]) == false)
					{
						actionAni = pani[0];
					}

					break;
				case 2:
					if(string.IsNullOrEmpty(pani[0]) == false)
					{
						actionAni = pani[0];
					}

					ignoreIdleAni = (pani[1] == "Y");

					setAniIgnore = true;

					break;
				}

//				if(actionAni != null)
//				{
//					setAniIgnore = true;
//
//					if(string.IsNullOrEmpty(actionAni))
//					{
//						actionAni = null;
//					}
//				}
			}
		}



		if(actionDefaultAttack > 0)
		{
			actionType = ActionType.ATTACK;
		}
		else if(actionSummonSlotNum != null)
		{
			actionType = ActionType.SUMMON;
		}
		else if(actionSkill != null)
		{
			actionType = ActionType.SKILL;
		}
		else if(actionSkill2 > -1)
		{
			actionType = ActionType.SKILL2;
		}
		else if( string.IsNullOrEmpty( actionAni ) == false)
		{
			actionType = ActionType.PLAYANI;
		}


		if(actionType == ActionType.SKILL || actionType == ActionType.SKILL2)
		{
			_tstr = (string)l[k["NEXT_AI"]];

			if(string.IsNullOrEmpty(_tstr) == false)
			{
				nextAi = _tstr;
			}
			else
			{
				nextAi = null;
			}
		}
	}

	public MoveType moveType;
	public ActionType actionType = ActionType.NONE;
	
	public enum MoveType
	{
		NONE, TARGETZONE, ACTION_POSITION, SKILL_POSITION
	}
	
	public enum ActionType
	{
		ATTACK, SUMMON, SKILL, SKILL2, PLAYANI, NONE
	}
	
	
	
	
	delegate bool Checker(Monster mon, AISlot ai);
	
	static List<Checker> tempCheckers = new List<Checker>();
	static List<Checker> tempFirstCheckers = new List<Checker>();

	Checker[] firstCheckers;
	Checker[] checkers;
	int _checkerNum = 0;
	int _firstCheckerNum = 0;


	public bool firstCheck(Monster mon, AISlot ai)
	{
		for(int i = 0; i < _firstCheckerNum; ++i)
		{
			if(firstCheckers[i](mon, ai) == false)
			{
				return false;
			}
		}

		return true;
	}

	public bool check(Monster mon, AISlot ai, bool isSkill2Mode = false)
	{
		if(maxExcuteNum > 0 && ai.excuteCount >= maxExcuteNum) return false;

		if(isSkill2Mode)
		{
			if(actionType != ActionType.SUMMON) return false;
		}

		for(int i = 0; i < _firstCheckerNum; ++i)
		{
			if(firstCheckers[i](mon, ai) == false)
			{
				return false;
			}
		}

		for(int i = 0; i < _checkerNum; ++i)
		{
			if(checkers[i](mon, ai) == false)
			{
#if UNITY_EDITOR					
			//if(DebugManager.instance.useDebug)	 Debug.Log("ai Id: " + id + "   : false");
#endif
				return false;
			}
		}
#if UNITY_EDITOR			
//		Debug.Log("ai Id: " + id + "   : true");
#endif		
		return true;
	}
	
	
// 검사 조건들! ==========================================================	
	
	int _per;
	bool checkHP(Monster mon, AISlot ai)
	{
		_per = (int)(( (float)mon.hp / mon.maxHp ) * 100.0f);
		return (_per >= hpMinMax[0] && _per <= hpMinMax[1]);
	}
	
	bool checkEHP(Monster mon, AISlot ai)
	{
		_per = (int)(( (float)GameManager.me.player.hp / GameManager.me.player.maxHp ) * 100.0f);
		return (_per >= playerHpMinMax[0] && _per <= playerHpMinMax[1]);
	}
	
	bool checkSP(Monster mon, AISlot ai)
	{
		_per = (int)(( (float)mon.sp / mon.maxSp ) * 100.0f);
		return (_per >= spMinMax[0] && _per <= spMinMax[1]);
	}
	
	bool checkMP(Monster mon, AISlot ai)
	{
		_per = (int)(( (float)mon.mp / mon.maxMp ) * 100.0f);
		return (_per >= mpMinMax[0] && _per <= mpMinMax[1]);
	}	
			
	bool checkTotalUnitHP(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.totalMonsterUnitHp >= totalMinMax[0] && GameManager.me.characterManager.totalMonsterUnitHp <= totalMinMax[1]);
	}
	
	
	bool checkAliveUnitNum(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.totalAliveMonsterUnitNum >= unitNumMinMax[0] && GameManager.me.characterManager.totalAliveMonsterUnitNum <= unitNumMinMax[1]);
	}	
	
	
	bool checkPlayerAliveUnitNum(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum >= eUnitNumMinMax[0] && GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum <= eUnitNumMinMax[1]);
	}		
	
	//* 아군대비상대소환유닛개수 : 상대개수/아군개수*100
	bool checkUnitNumCondition(Monster mon, AISlot ai)
	{
		_per = (int)((float)GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum / (float)GameManager.me.characterManager.totalAliveMonsterUnitNum * 100.0f);
		return (_per >= unitNumConMinMax[0] && _per <= unitNumConMinMax[1]);
	}		

	//* 아군대비상대소환유닛HP : 상대유닛HP총합/아군유닛HP총합*100	
	bool checkUnitNumTHP(Monster mon, AISlot ai)
	{
		_per = (int)((float)GameManager.me.characterManager.totalPlayerUnitHp / (float)GameManager.me.characterManager.totalMonsterUnitHp * 100.0f);
		return (_per >= unitNumConMinMax[0] && _per <= unitNumConMinMax[1]);
		
	}		
	
	bool checkPlayerUnitNumInTargetZone(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.playerUnitInMonsterTargetZone >= zoneUnitNumMinMax[0] && GameManager.me.characterManager.playerUnitInMonsterTargetZone <= zoneUnitNumMinMax[1]);
	}		
	

	bool checkMonsterUnitNumInPlayerTargetZone(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.monterUnitInPlayerTargetZone >= zonePlayerUnitNumMinMax[0] && GameManager.me.characterManager.monterUnitInPlayerTargetZone <= zonePlayerUnitNumMinMax[1]);
	}	
	
	//아군대비상대소환유닛개수 ZONE_UNIT_NUM_CON
	bool checkZoneUnitNumCondition(Monster mon, AISlot ai)
	{
		_per = (int)((float)GameManager.me.characterManager.playerUnitInMonsterTargetZone / (float)GameManager.me.characterManager.monterUnitInPlayerTargetZone* 100.0f);
		return (_per >= zoneUnitNumConMinMax[0] && _per <= zoneUnitNumConMinMax[1]);
	}	
	
	//아군대비상대소환유닛총HP  ZONE_THP
	bool checkTargetZoneTHP(Monster mon, AISlot ai)
	{
		_per = (int)((float)GameManager.me.characterManager.playerUnitHPInMonsterTargetZone / (float)GameManager.me.characterManager.monterUnitHPInPlayerTargetZone * 100.0f);
		return (_per >= zoneTHPMinMax[0] && _per <= zoneTHPMinMax[1]);
	}	
	
	//타겟존과의거리 DIST_TZONE
	bool checkDistFromTargetZone(Monster mon, AISlot ai)
	{
		tempF = MathUtil.abs(mon.cTransformPosition.x , GameManager.me.characterManager.targetZonePlayerLine);
		return (tempF >= distTZoneMinMax[0]  && tempF <= distTZoneMinMax[1]);
	}
	
	//최대 넘어가본 타겟존.
	bool checkDistRecordedTargetZone(Monster mon, AISlot ai)
	{
		return (GameManager.me.characterManager.longestWalkedTargetZonePlayerLine >= maxTzoneMinMax[0]  && GameManager.me.characterManager.longestWalkedTargetZonePlayerLine <= maxTzoneMinMax[1]);
	}


	//상대히어로와의거리 DIST_HERO
	bool checkDistFromHero(Monster mon, AISlot ai)
	{
		tempF = mon.cTransformPosition.x - GameManager.me.player.cTransformPosition.x;
		return (tempF >= distFromHeroMinMax[0]  && tempF <= distFromHeroMinMax[1]);
	}


	//스타트지점과의거리 DIST_START
	bool checkDistFromStartPoint(Monster mon, AISlot ai)
	{
		tempF = mon.cTransformPosition.x - mon.stageMonsterData.posX;
		return (tempF >= distStartMinMax[0]  && tempF <= distStartMinMax[1]);
		
	}
	
	//끝지점과의거리 DIST_ENDLINE
	bool checkDistFromEndLine(Monster mon, AISlot ai)
	{
		tempF = MathUtil.abs(mon.cTransformPosition.x , StageManager.mapEndPosX);
		return (tempF >= distEndLineMinMax[0]  && tempF <= distEndLineMinMax[1]);
	}	

	private Monster _tempMon;
	//기본공격거리내 적존재 TARGET_IN_CZONE
	// 원래는 캐릭터 공격 거리내에 적 존재였는데 지금은 지정된 거리 안에 적이 있는지만 본다.


	bool checkEnemyInDefaultActionZone(Monster mon, AISlot ai)
	{
		int len = GameManager.me.characterManager.playerMonster.Count;

		if(len <= 0) return false;

		if(VectorUtil.DistanceXZ(mon.cTransformPosition, GameManager.me.characterManager.playerMonster[0].cTransformPosition) < targetInDefaultTargetZone)
		{
			mon.targetPosition = GameManager.me.characterManager.playerMonster[0].cTransformPosition;
			mon.targetHeight = GameManager.me.characterManager.playerMonster[0].hitObject.height;
			mon.targetUniqueId = GameManager.me.characterManager.playerMonster[0].stat.uniqueId;
			return true;
		}
		/*
		if(GameManager.me.player.isEnabled)
		{
			if(len >= targetInDefaultTargetZone)
			{
				_tempMon = GameManager.me.characterManager.playerMonster[targetInDefaultTargetZone-1];
			}
			else if(len > 0 && len + 1 >= targetInDefaultTargetZone)
			{
				_tempMon = GameManager.me.characterManager.playerMonster[targetInDefaultTargetZone-1];
			}
			else if(targetInDefaultTargetZone == 1)
			{
				return (GameManager.me.player.cTransformPosition.x + GameManager.me.player.damageRange + mon.stat.atkRange >= GameManager.me.characterManager.playerMonsterRightLine); // hitrange
			}


			bool returnValue = false;

			if(_tempMon != null)
			{
				if(_tempMon.lineRight > GameManager.me.player.lineRight)
				{
					returnValue = (_tempMon.cTransformPosition.x + _tempMon.damageRange + mon.stat.atkRange >= GameManager.me.characterManager.playerMonsterRightLine); // hitrange
				}
				else
				{
					returnValue = (GameManager.me.player.cTransformPosition.x + GameManager.me.player.damageRange + mon.stat.atkRange >= GameManager.me.characterManager.playerMonsterRightLine); // hitrange
				}
				
				_tempMon = null;
			}

			return returnValue;
		}
		else
		{
			if(len >= targetInDefaultTargetZone)
			{
				return (GameManager.me.characterManager.playerMonster[targetInDefaultTargetZone-1].damageRange + mon.stat.atkRange >= GameManager.me.characterManager.playerMonsterRightLine); // hitrange
			}
		}
		*/

		return false;


		//if(VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition) <= mon.hitRange  + mon.target.damageRange)
		//mon.stat.atkRange

		//targetInDefaultTargetZone
		//return (GameManager.me.characterManager.playerMonsterRightLine + targetInDefaultTargetZone >= mon.lineLeft);
		//return (GameManager.me.characterManager.getCloseEnemyTargetByPosX(false).lineLeft > GameManager.me.characterManager.targetZonePlayerLine - mon.stat.atkRange);
	}
	
	//상대스킬발동 DID_SKILL
	bool checkPlayerDidSkillToMe(Monster mon, AISlot ai)
	{
		// To do
		return false;
	}
	
	
	//스킬타게팅가능 CAN_TARGETING
	bool checkCanTargetingSkill(Monster mon, AISlot ai)
	{
		return mon.skillSlots[canTargeting[0]].canTargetingByHeroMonster(canTargeting[1]);
	}


	//스킬타게팅가능 실행조건에서 검사 CAN_
	public bool checkCanTargetingSkillAtAction(Monster mon)
	{
		return mon.skillSlots[actionSkill[0]].canTargetingByHeroMonster(actionSkill[1]);
	}

	
	//[조건] 소환&스킬
	//* N번 : 해당 언데드히어로가 보유한 소환유닛/스킬 슬롯의 
	//* 현재 보유 SP/MP 및 쿨타임, 최대소환개수 등을 따져서
	//   소환/스킬 가능한지 판단
	
	// N번소환유닛소환가능? SUMMON_SLOT_NUM
	bool checkCanSummon(Monster mon, AISlot ai)
	{
		// To do!!
		//mon.action.
		//mon.stageMonsterData.units[summonSlotNum]
		return mon.unitSlots[summonSlotNum].canUse();
	}
	
	// N번스킬사용가능? SKILL_NUM
	bool checkCanUseSkill(Monster mon, AISlot ai)
	{
		// To do!!
		
		//Debug.Log(mon.skillSlots + "   " + skillNum);
		
		return mon.skillSlots[skillNum].canUse();
	}
	
	// 플레이타임 PLAYTIME
	bool checkPlayerTime(Monster mon, AISlot ai)
	{
		return (GameManager.me.stageManager.playTime >= playTimeMinMaxSinceStart[0] && GameManager.me.stageManager.playTime <= playTimeMinMaxSinceStart[1]);
	}	

	
	// 쿨타임 COOLTIME저
	bool checkCoolTime(Monster mon, AISlot ai)
	{
		return ai.canUse();
	}
			
	
	
	
// 실행조건들! ==========================================================
	
	bool _tb;
	// N%확률로 EXE_CHANCE
	bool checkExeChange(Monster mon, AISlot ai)
	{
		tempI = GameManager.inGameRandom.Range(0,100);//GameManager.getRandomNum();
		_tb = (tempI < executeChange);
		ai.resetCoolTime();
		if(_tb) ++ai.excuteCount;
		return _tb;
	}
	

// 실제 행동할 것들 : 이동 ==========================================================	
	
	// 타겟존과의거리 MOVE_DIST_TZONE
	
	// 기본공격가능위치	ACT_POSITION	
	
	//스킬사용가능위치 SKILL_POSITION
	
	
// 실제 행동할 것들 : 액션 ==========================================================	
	
	//기본공격 ACT	
	
	// 소환수 소환 ACT_SUMMON	
	
	//스킬 사용 ACT_SKILL
	
}

