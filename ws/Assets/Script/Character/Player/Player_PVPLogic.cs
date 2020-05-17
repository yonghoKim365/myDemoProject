using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


sealed public partial class Player : Monster
{

	public int[] pvpPlayerAttackSkillSlotIndex = new int[3];
	public int pvpPlayerAttackSkillDataLen;

	private static AIScoreSorter _aIScoreSorter = new AIScoreSorter();
	private static AISkillScoreSorter _aISkillScoreSorter = new AISkillScoreSorter();



	private static List<AIScore> _summonScoreCheckers = null;

	private static List<AIScore> _skillScoreCheckers = null;

	private static List<AIScore> _skillMoveScoreCheckers = null;

	public List<AIScore> dangerPoints = null;

	public const int CURRENT_POSITION_INDEX = 10;

	void initPVP()
	{
		if(_summonScoreCheckers == null)
		{
			_summonScoreCheckers = new List<AIScore>(5);

			for(int i = 0; i < 5; ++i) _summonScoreCheckers.Add(new AIScore());

			_skillScoreCheckers = new List<AIScore>(3);
			
			for(int i = 0; i < 3; ++i) _skillScoreCheckers.Add(new AIScore());


			_skillMoveScoreCheckers = new List<AIScore>(10);

			for(int i = 0; i < 10; ++i) _skillMoveScoreCheckers.Add(new AIScore());
		}

		// 11 번째는 현재 위치 계산용이다.
		if(dangerPoints == null)
		{
			dangerPoints = new List<AIScore>(11);
			for(int i = 0; i < 11; ++i) dangerPoints.Add(new AIScore());
		}


		isBattleWaitMode = true;
		isDefaultAttackOn = false;
		_moveTimeChecker = 0.0f;
		prevMoveState = MoveState.Stop;

	}


	void updateAI()
	{
		// 스킬을 차징하거나 스킬을 선택하지 않은 상태에서만
		// 다음 스킬을 선택 할 수 있다.

		if(moveType == MoveType.NORMAL)
		{
			checkNormalAi();

			// 스킬룬 선택 AI
			updateSkillRuneAI();

			skillModeProgressTime = 0.0f;
		}
		else if(moveType == MoveType.SKILL)
		{
			updateSkillMoveAI();
			skillModeProgressTime += GameManager.globalDeltaTime;
		}

		// 소환룬 선택 & 생성 AI. // 얘는 독립적이다.
		updateUnitRuneAI();

		if(changePlayerChecker != null && GameManager.me.battleManager.hasAlivePlayer(isPlayerSide))
		{
			updateTagAi();
		}
	}


// ============ 이동 AI ============= //


	public IFloat getDamagePer(IFloat value)
	{
		value = value / maxHp;

		if(value > 1.0f)
		{
			value = 1.0f;
		}

		return value;
	}



	// ============ Tag AI ============= //


	public static IFloat TAG_DELAY = 2.0f; // 한순간에 모두 뽑지 못하도록 딜레이를 준다.
	private IFloat _nowTagDelay = 0.0f; 
	
	void updateTagAi()
	{
		if(_nowTagDelay > 0)
		{
			_nowTagDelay -= GameManager.globalDeltaTime; return;
		}

		if(GameManager.me.battleManager.getWatingTagSlot(isPlayerSide).state == UIPlayTagSlot.State.Ready)
		{
			if(changePlayerChecker.checkTagAi(this))
			{
				GameManager.me.battleManager.getWatingTagSlot(isPlayerSide).onPressByAi();
				_nowTagDelay = TAG_DELAY;
			}
		}
	}



//============= 소환 AI! ===========//

	public static IFloat SUMMON_DELAY = 0.8f; // 한순간에 모두 뽑지 못하도록 딜레이를 준다.
	private IFloat _nowSummonDelay = 0.0f; 

	void updateUnitRuneAI()
	{
		if(_nowSummonDelay > 0)
		{
			_nowSummonDelay -= GameManager.globalDeltaTime; return;
		}

		bool canSummon = false;

		// 5개 슬롯 중 하나라도 소환이 가능한지 검사한다.
		for(int i = 0; i < _unitSlotsNum; ++i)
		{
			_summonScoreCheckers[i].index = i;
			_summonScoreCheckers[i].canUse = unitSlots[i].canUse();

			if(_summonScoreCheckers[i].canUse)
			{
				canSummon = true;
			}
		}

		// 소환 할 수 있는 녀석이 하나라도 있음.
		if(canSummon)
		{
			// 그럼 모든 슬롯에 대해 점수를 계산함.
			// 단 현재 소환 가능 개수가 1개 이상인 애들이어야함.
			for(i = 0; i < SUMMON_SLOT_NUM; ++i)
			{
				if(i < _unitSlotsNum) _summonScoreCheckers[i].score = unitSlots[i].pvpPoint();
				else
				{
					_summonScoreCheckers[i].canUse = false;
					_summonScoreCheckers[i].score = -1000;
				}
			}

			_summonScoreCheckers.Sort(_aIScoreSorter);

			// 1번 소환수가 즉시 소환가능하면 소환.
			if(_summonScoreCheckers[0].canUse)
			{


//				Log.logError("   1번 소환수 소환 가능! index: " + _summonScoreCheckers[0].index + "   id: " + unitSlots[_summonScoreCheckers[0].index].unitData.id );
				unitSlots[_summonScoreCheckers[0].index].createPVPUnit();
				_nowSummonDelay += SUMMON_DELAY;
			}
			else
			{
				// 소환 가능 개수가 1개 이상인 애들 중 
				// 1등은 현재 소환을 할 수 없어. 쿨타임 & SP에 걸려서.
				// 그럼 후순위들을 계산해봄.
				// 만약 후순위 녀석을 소환한다고 치자. 그런데 얘를 소환해서
				// sp를 써도 1순위 애를 몇초후에 그대로 소환할 수 있으면 소환한다.
				// 아니면 소환못한다.

				for(i = 1; i < SUMMON_SLOT_NUM; ++i)
				{
//					Log.logError("  <<<<  " + i + "   번 소환수 체크!!!"  );

					if(_summonScoreCheckers[i].canUse &&
					   unitSlots[_summonScoreCheckers[0].index].canUseAfterCoolTime(unitSlots[_summonScoreCheckers[i].index].useSp) )
					{


//						Log.logError(i +  " 번 소환수 소환!!!!"  ) ;
						unitSlots[_summonScoreCheckers[i].index].createPVPUnit();
						_nowSummonDelay += SUMMON_DELAY;
						return;
					}
				}
			}
		}
	}








//============= 스킬 AI! ===========//
	private PVPPlayerSkillSlot _nowSelectSkillSlot;

//	private PVPPlayerSkillSlot _nowSelectSkillSlot
//	{
//		get
//		{
//			return __nowSelectSkillSlot;
//		}
//		set
//		{
//				Debug.LogError("_nowSelectSkillSlot : " + value);
//			__nowSelectSkillSlot = value;
//		}
//	}



	private IFloat _updateSkillDelay = 0.0f; 
	const float UPDATE_SKILL_DELAY = 0.5f;

	void updateSkillRuneAI()
	{
		if(_updateSkillDelay > 0.0f)
		{
			_updateSkillDelay -= GameManager.globalDeltaTime;
			return;
		}
		else
		{
			_updateSkillDelay += UPDATE_SKILL_DELAY; 
		}

		if(nowPlayingSkillAni || nowChargingSkill != null) return;

		_nowSelectSkillSlot = null;

		bool canUse = false;
		
		// 사용 가능한지 검사한다.
		for(int i = 0; i < _skillSlotsNum; ++i)
		{
			_skillScoreCheckers[i].index = i;
			_skillScoreCheckers[i].canUse = skillSlots[i].canUse();
			
			if(_skillScoreCheckers[i].canUse)
			{
				canUse = true;

				#if UNITY_EDITOR
//				if(BattleSimulator.nowSimulation == false)
//				{
//									Debug.LogError(" i : " + i + "    " + skillSlots[i].skillData.id + "    는 일단 사용 가능하다!" );
//				}
				#endif


			}
		}
		
		// 사용 할 수 있는 녀석이 하나라도 있음.
		if(canUse)
		{
			// 그럼 모든 슬롯에 대해 점수를 계산함.
			// 단 현재 소환 가능 개수가 1개 이상인 애들이어야함.
			for(i = 0; i < SKILL_SLOT_NUM; ++i)
			{
				if(i < _skillSlotsNum)
				{
					_skillScoreCheckers[i].score = skillSlots[i].pvpPoint();

					if(skillSlots[i].skillData.targeting <= 1)
					{
						if(checkSkillType_0_1_BestTargetingPosition(skillSlots[i]) == false)
						{
							_skillScoreCheckers[i].score = -10000;
						}
					}

#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation == false)
					{
						//Debug.LogError(i + "   " +  skillSlots[i].skillData.id + " : _skillScoreCheckers[i].score : " + _skillScoreCheckers[i].score);
					}
#endif

				}
				else
				{
					_skillScoreCheckers[i].canUse = false;
					_skillScoreCheckers[i].score = -10000;
				}


			}
			
			_skillScoreCheckers.Sort(_aIScoreSorter);
			
			// 1번 스킬이 즉시 소환가능하면 소환.
			if(_skillScoreCheckers[0].canUse)
			{
				if(_skillScoreCheckers[0].score > -999)
				{

					#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation == false)
					{
						//Debug.LogError("   1번 스킬 사용 가능! index: " + _skillScoreCheckers[0].index + "   id: " + skillSlots[_skillScoreCheckers[0].index].skillData.id );
					}
					#endif
					_nowSelectSkillSlot = (PVPPlayerSkillSlot)skillSlots[_skillScoreCheckers[0].index];
					moveType = MoveType.SKILL;
					skillModeProgressTime = 0.0f;
					skillMoveIsNormal = true;
					_pvpTargetPosition = cTransformPosition;
					skillTargetingPositionMaxPoint = 0.0f;
					_updateSkillDelay += UPDATE_SKILL_DELAY;
				}
			}
			else
			{
				// 사용 가능 개수가 1개 이상인 애들 중 
				// 1등은 현재 사용을 할 수 없어. 쿨타임 & MP에 걸려서.
				// 그럼 후순위들을 계산해봄.
				// 만약 후순위 녀석을 소환한다고 치자. 그런데 얘를 소환해서
				// mp를 써도 1순위 애를 몇초후에 그대로 소환할 수 있으면 소환한다.
				// 아니면 소환못한다.
				for(i = 1; i < SKILL_SLOT_NUM; ++i)
				{
//					Debug.Log("  <<<<  " + i + "   번 스킬 체크!!!"  );
					if(_skillScoreCheckers[i].score > -999) 
					{
						if(_skillScoreCheckers[i].canUse &&
						   skillSlots[_skillScoreCheckers[0].index].canUseAfterCoolTime(skillSlots[_skillScoreCheckers[i].index].useMp) )
						{
							_nowSelectSkillSlot = (PVPPlayerSkillSlot)skillSlots[_skillScoreCheckers[i].index];

#if UNITY_EDITOR
							if(BattleSimulator.nowSimulation == false)
							{
//								Debug.LogError(_skillScoreCheckers[i].index +  " 번 스킬 사용!!!!"  + "    " + skillSlots[_skillScoreCheckers[0].index].skillData.id) ;
							}
#endif
							moveType = MoveType.SKILL;
							skillModeProgressTime = 0.0f;
							skillMoveIsNormal = true;
							skillTargetingPositionMaxPoint = 0.0f;
							_pvpTargetPosition = cTransformPosition;
							_updateSkillDelay += UPDATE_SKILL_DELAY;
							return;
						}
					}
				}
			}
		}
	}

}

public class AIScore
{
	public int index = 0;
	public IFloat score = 0;
	public IFloat temp = 0;
	public int type = 0;
	public bool canUse = false;
	public IVector3 pos = IVector3.zero;
	public IFloat distance = 0;

}


public class AIScoreSorter  : IComparer<AIScore>
{
	public int Compare(AIScore x, AIScore y)
	{
		if(x.score < y.score) return 1;
		else if(x.score > y.score) return -1;
		return 0;
	}	
}


public class AISkillScoreSorter  : IComparer<AIScore>
{
	public int Compare(AIScore x, AIScore y)
	{
		// 점수는 높은 순으로.
		int i = y.score.CompareTo(x.score);

		// 거리는 가까운 순으로.
		if(i == 0) i = x.distance.CompareTo(y.distance);

		return i;
	}	
}

