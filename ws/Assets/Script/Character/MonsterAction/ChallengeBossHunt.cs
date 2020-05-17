using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class ChallengeBossHunt : CharacterAction
{
	public ChallengeBossHunt ()
	{
	}

	private CharacterManager _cm;

	tk2dSlicedSprite[] zones;

	LampEffect[] lampEffectTop;
	LampEffect[] lampEffectBottom;

	sealed public override void init(Monster monster)
	{
		base.init(monster);

		_cm = GameManager.me.characterManager;

		zones = GameManager.me.mapManager.spHuntZone;
	}
	

	sealed public override void startAction ()
	{
		base.startAction ();
	}


	string[] units;
	float _summonDelay = 0.0f;
	float _delay = 0.0f;

	float[] _checkLevelDistance;
	float[] _summonDelays;

	float _hitRadius = 0.0f;

	int zoneLevel = 0;

	public override void setData (object data)
	{
//		GameManager.me.effectManager.lampContainer.SetActive(true);
//		lampEffectTop = GameManager.me.effectManager.lampLineTop;
//		lampEffectBottom = GameManager.me.effectManager.lampLineBottom;

		ChallengeInfinityHunt d = (ChallengeInfinityHunt)data;
		units = d.summonUnits;
		_delay = 0.0f;
		zoneLevel = -2;
		_summonDelays = d.summonDelays;

		setZoneLevel(-1);
		_checkLevelDistance = d.checkLevelDistance;
		_cm.targetingDecal[2].init(TargetingDecal.DecalType.Monster, 2.0f,true);
		_hitRadius = 200.0f;

		//_cm.targetingDecal[2].setColor(new Color(
		_cm.targetingDecal[2].setPosition(mon.cTransformPosition);

		Vector2 v2 = zones[0].dimensions;

		Vector3 _v3;

		for(int i = 0; i < 5; ++i)
		{
			noneColor[i] = new Color(1,1,1,0.1f + 0.05f * i);

			_v.x = _checkLevelDistance[i];
			zones[i].transform.position = _v;

			if(i < 4) v2.x = _checkLevelDistance[i+1] - _checkLevelDistance[i];
			else v2.x = 500.0f;
			zones[i].dimensions = v2;

			_v3 = lampEffectTop[i].transform.position;
			_v3.x = _checkLevelDistance[i];
			lampEffectTop[i].transform.position = _v3;

			_v3 = lampEffectBottom[i].transform.position;
			_v3.x = _checkLevelDistance[i];
			lampEffectBottom[i].transform.position = _v3;
		}


		zones[0].transform.parent.gameObject.SetActive(true);

		levelColor[0] = new Color(0.2f, 0.5f, 1.0f); // 0단계 색깔이 필요하다!!!
		levelColor[1] = new Color(0.2f, 0.5f, 1.0f);
		levelColor[2] = new Color(0.9f, 0.65f, 0.0f);
		levelColor[3] = new Color(0.14f, 0.84f, 0.78f);
		levelColor[4] = new Color(0.96f, 0.3f, 0.0f);
		levelColor[5] = new Color(0.94f, 0.08f, 0.078f);
	}

	Color selectColor = new Color(1,1,1,0.5f);
	Color[] noneColor = new Color[5];
	Color[] levelColor = new Color[6];

	void setZoneLevel(int newZoneLevel)
	{
		if(zoneLevel != newZoneLevel)
		{
			zoneLevel = newZoneLevel;
			setSummonDelay();

			for(int i = 0; i < 5; ++i)
			{
				if(i == zoneLevel)
				{
					zones[i].color = levelColor[i+1];
					zones[i].gameObject.SetActive(true);
				}
				else
				{
					//zones[i].color = noneColor[i];
					zones[i].gameObject.SetActive(false);
				}

				lampEffectTop[i].turnOn(i <= zoneLevel);
				lampEffectBottom[i].turnOn(i <= zoneLevel);
			}
		}
	}

	void setSummonDelay()
	{
		if(zoneLevel >= 0 && zoneLevel <= 4)
		{
			_summonDelay = _summonDelays[zoneLevel+1] * 0.001f;
			_cm.targetingDecal[2].mc.start(levelColor[zoneLevel+1], 2);
		}
		else
		{
			_summonDelay = _summonDelays[0] * 0.001f;
			_cm.targetingDecal[2].mc.start(levelColor[0], 2);
		}
	}

//	[게임모드 - 무한 사냥]						
//	
//	* 게임목적 : 정해진 시간 (2분) 동안 최대한 많이 몬스터 죽이기					
//		* 맵 세팅					
//			- 스타트 지점에서 1~2미터 앞으로, 일정간격의 1~5단계의 존이 존재					
//			- 5단계 존 끝에는 몬스터들이 등장하는 게이트 or 마법진 배치					
//			* 소환 룰					
//			→ 유저히어로가 N단계존을 밟으면 A ms 간격으로 					
//			지정된 몬스터가 게이트 위치에서 스폰					
//			1단계	2단계	3단계	4단계	5단계
//			소환주기	2000	1500	1000	500	200
//			
//			* 게임종료 조건					
//			- 히어로 사망					
//			- 2분 경과					
//			- 별셋 랭크					
//			* 액트별로 룰은 동일하되 등장 몬스터의 레벨이 상승					
//			
//


	sealed public override void doMotion()
	{
		for(int i = 4; i >= 0; --i)
		{
			if(GameManager.me.player.lineRight > _checkLevelDistance[i])
			{
				setZoneLevel(i);
				break;
			}
		}

		if(GameManager.me.player.lineRight < _checkLevelDistance[0])
		{
			setZoneLevel(-1);
		}


		_delay += GameManager.globalDeltaTime;

		hitTest();

		if(_delay < _summonDelay) return;

		_delay = 0.0f;
		createSummon();
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
		zones[0].transform.parent.gameObject.SetActive(false);
		zones = null;

//		GameManager.me.effectManager.lampContainer.SetActive(false);
//		lampEffectTop = null;
//		lampEffectBottom = null;

		base.clear ();
		_cm.targetingDecal[2].isEnabled = false;
		_cm.targetingDecal[2].mc.isEnabled = false;


		_cm = null;
		
	}


	void hitTest()
	{
		_v = mon.cTransformPosition;
		
		for(int i = _cm.playerMonster.Count - 1; i >= 0; --i)
		{
			if(_cm.playerMonster[i].isEnabled)
			{
				if(VectorUtil.DistanceXZ(_v, _cm.playerMonster[i].cTransformPosition) <= _hitRadius + _cm.playerMonster[i].damageRange )
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

}

