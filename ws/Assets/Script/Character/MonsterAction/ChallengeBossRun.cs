using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class ChallengeBossRun : CharacterAction
{
	public ChallengeBossRun ()
	{
	}

	private CharacterManager _cm;

	sealed public override void init(Monster monster)
	{
		base.init(monster);

		_cm = GameManager.me.characterManager;
	}
	

	sealed public override void startAction ()
	{
		base.startAction ();
	}


	string[] units;
	float _summonDelay = 0.0f;
	float _delay = 0.0f;
	float _checkDistance = 0.0f;

	public override void setData (object data)
	{
		ChallengeInfinityRun d = (ChallengeInfinityRun)data;
		units = d.summonUnits;
		_summonDelay = d.summonDelay;
		_delay = 0.0f;
		_checkDistance = d.checkDistance;
	}

	
//	[게임모드 - 무한 질주]		
//	
//	* 게임목적 : 정해진 시간 (2분) 내에 최대한 멀리 이동하기	
//		* 맵 세팅	
//			→ 스타트 지점으로부터 A미터 이후부터 B미터 간격으로 총 N개의 토템이 박혀있음	
//			(무한맵 아님)	
//			* 토템(히어로) 소환 AI	
//			→ 적이 C(cm) 이내로 접근하면,	
//			D(ms) 간격으로, 지정된 몬스터들 중 하나를 랜덤선택하여 소환	
//			토템의 순서를 E번째 라고 할 때,	
//			D = (N-E) / N * 3000 + 200	
//			* 게임종료 조건	
//			- 히어로 사망	
//			- 2분 경과	
//			- 별셋 랭크	
//			* 액트별로 룰은 동일하되 등장 몬스터의 레벨이 상승	
//			


	sealed public override void doMotion()
	{
		_delay += GameManager.globalDeltaTime;

		if(_delay < _summonDelay) return;

		if(_cm.playerMonsterRightLine + _checkDistance >= mon.cTransformPosition.x)
		{
			_delay = 0.0f;
			createSummon();
		}
	}	

	int _createNum = 0;

	void createSummon()
	{
		_v = mon.cTransformPosition;					
		_v.x = mon.lineLeft;
		
		_v.z = (float)GameManager.inGameRandom.Range(MapManager.summonBottom, MapManager.summonTop);

		if(_v.z >= 0.0f && _v.z < 80) _v.z = 80;
		else if(_v.z <= 0.0f && _v.z > -80) _v.z = -80;

		if(_v.x < GameManager.me.characterManager.targetZonePlayerLine)
		{
			_v.x = GameManager.me.characterManager.targetZonePlayerLine + 200.0f;
		}
		
		if(_v.x >= mon.lineLeft) _v.x = mon.lineLeft;
		
		Monster summonMon = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, units[GameManager.inGameRandom.Range(0,units.Length)],_v);
		_v.y = 0.5f;
		GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_ENEMY].getEffect(-1000,_v, null, null, summonMon.summonEffectSize); 
	}

	
	
	
	
	public override void clear ()
	{
		base.clear ();
		_cm = null;
	}
	
	
	
	
	
	
	
	
}

