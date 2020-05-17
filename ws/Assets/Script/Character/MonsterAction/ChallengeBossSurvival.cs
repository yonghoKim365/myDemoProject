using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class ChallengeBossSurvival : CharacterAction
{
	public ChallengeBossSurvival ()
	{
	}

	private CharacterManager _cm;

	sealed public override void init(Monster monster)
	{
		base.init(monster);
		_cm = GameManager.me.characterManager;
		_summonDelay = 5.0f;
		_delay = 0.0f;
	}
	

	sealed public override void startAction ()
	{
		base.startAction ();
	}


//	* 게임목적 : 최대한 오래 살아남기
//		* 맵 세팅
//			→ 스타트 지점으로부터 A미터 이후에 어둠의 마법진이 존재
//			> 어둠의 마법진 : 적 몬스터가 소환되는 위치이자 아군 소환수가 올라서면 즉사 함
//			* 소환 룰
//			→ B(ms) 간격으로, 지정된 몬스터들 중 하나를 랜덤선택하여 (정 중앙위치에) 소환
//			경과시간을 C(sec), 별셋 랭크 기준을 D(sec) 라고 할 때
//			B = (D-C) / D * 3000 + 200
//			* 게임종료 조건
//			- 히어로 사망
//			- 별셋 랭크
//			* 액트별로 룰은 동일하되 등장 몬스터의 레벨이 상승


	string[] units;
	float _summonDelay = 0.0f;
	float _delay = 0.0f;
	float _hitRadius = 0.0f;
	int[] rankData;
	float constValue0 = 0.0f;
	float constValue1 = 0.0f;

	public override void setData (object data)
	{
		ChallengeInfinitySurvival d = (ChallengeInfinitySurvival)data;
		units = d.summonUnits;
		_delay = 0.0f;
		_hitRadius = d.hitRadius;
		rankData = d.rankData;
		_cm.targetingDecal[2].init(TargetingDecal.DecalType.Monster,_hitRadius * 0.01f,true);
		_cm.targetingDecal[2].setPosition(mon.cTransformPosition);
		_cm.targetingDecal[2].mc.start(Color.red,_hitRadius * 0.01f);

		constValue0 = d.constValue[0];
		constValue1 = d.constValue[1];

		setSummonDelay();
	}

	void setSummonDelay()
	{
		_summonDelay = ((float)rankData[2] - GameManager.me.stageManager.playTime) / (float)rankData[2] * constValue0 + constValue1; 
		_summonDelay *= 0.001f;
	}

	sealed public override void doMotion()
	{
		_delay += GameManager.globalDeltaTime;

		hitTest();

		if(_delay < _summonDelay) return;

		createSummon();
		setSummonDelay();
		_delay = 0.0f;
	}	


	void hitTest()
	{
		_v = mon.cTransformPosition;

		for(int i = _cm.playerMonster.Count - 1; i >= 0; --i)
		{
			if(_cm.playerMonster[i].isEnabled)
			{
				if(VectorUtil.DistanceXZ(_v, _cm.playerMonster[i].cTransformPosition) <= _hitRadius + _cm.playerMonster[i].damageRange)
				{
					if(_cm.playerMonster[i].isPlayer)
					{
						_cm.playerMonster[i].characterEffect.addKnockBack(200);
					}
					else
					{
						_cm.playerMonster[i].damageDead();
					}

				}
			}
		}
	}

	void createSummon()
	{
		_v = mon.cTransformPosition;					

		Monster summonMon = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, units[GameManager.inGameRandom.Range(0,units.Length)],_v);

		_v.y = 0.5f;
		GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_ENEMY].getEffect(-1000,_v, null, null, summonMon.summonEffectSize); 
	}

	
	
	public override void clear ()
	{
		base.clear ();
		_cm.targetingDecal[2].isEnabled = false;
		_cm = null;

	}

}

