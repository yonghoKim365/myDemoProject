using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning
{
	public int uniqueNo = 0;

	public bool isDeleteObject = false;

	public string type;

	public string hitEffectId = "";

	public ChainLightning (string chainType, string hit)
	{
		type = chainType;

		hitEffectId = hit;
		if(hitEffectId == null) hitEffectId = "E_CHAINLIGHTNING_HIT";
	}
	
	Xfloat _totalDistanceLimit = 0;
	Xfloat _distanceLimitA = 0 ;
	Xfloat _distanceLimitB = 0;
	int _maxConnection;
	Xfloat _connectionDelay = 0;
	Xfloat _delay = 0;
	Xfloat _timeSinceBeginning = 0;

	List<ElectricEffect> _effects = new List<ElectricEffect>();
	List<ParticleEffect> _particleEffects = new List<ParticleEffect>();
	List<Monster>  _targets = new List<Monster>();

	static List<Monster>  _chaByRangeDistance = new List<Monster>();
	
	int _nowConnectionNum = 0;
	
	bool _restoreCharacterAni = false;
	bool _canMore = false;
	bool _firstRemove = true;
	bool _isPlayerBullet = false;
	
	IVector3 _startPosition;
	IVector3 _v;
	int len;
	
	Monster _attacker;
	

	int _applyInforceLevel = 0;
	int _skillLevel = 1;
	
	private BaseSkillData _skillData = null;


	IFloat _damageOffsetPerConnectionLevel = 1.0f;


	public void start(bool isPlayerBullet, Monster attacker, float totalDistanceLimit, float distanceLimitA, float distanceLimitB, int maxConnection, float connectionDelay, int damagePer, int skillLevel, int applyInforceLevel, BaseSkillData sd = null)
	{
		if(damagePer <= 0)
		{
			damagePer = 100;
		}

		if(damagePer < 100)
		{
			_damageOffsetPerConnectionLevel = ((float)damagePer)/100.0f/(float)maxConnection;
		}
		else
		{
			_damageOffsetPerConnectionLevel = 0.0f;
		}


		isDeleteObject = false;
		_skillData = sd;
		_attacker = attacker;
		_delay = 0.0f;
		_totalDistanceLimit.Set( totalDistanceLimit );
		_distanceLimitA.Set( distanceLimitA );
	 	_distanceLimitB.Set( distanceLimitB );
		_maxConnection = maxConnection;
		_connectionDelay = connectionDelay;
		_nowConnectionNum = 0;
		_canMore = true;
		_restoreCharacterAni = false;
		_timeSinceBeginning = 0.0f;
		_isPlayerBullet = isPlayerBullet;
		_firstRemove = true;
		_nextDelay = 0.3f;
		_applyInforceLevel = applyInforceLevel;
		_skillLevel = skillLevel;

		getNextTarget(_isPlayerBullet);
	}
	
	
	private Xfloat _nextDelay = 0.0f;
/*	
속성	최대전체거리	최대연결거리A	최대연결거리B	최대연결유닛수	연결딜레이	
						
▷ 시전자와 가장 가까이 있는 적 유닛에게 라이트닝을 연결한 후, 그 유닛과 가장 가까운 유닛들을 계속 연결하여 공격하는 방식						
						
* 최대전체거리 : 체인라이트닝을 시전한 시전자와 다음 연결할 대상의 거리가 [최대거리] 보다 클 경우 연결 중지						
* 최대연결거리A : 시전자와 최초타겟이 연결될 수 있는 최대거리, 최대연결거리보다 클 경우 연결 중지						
* 최대연결거리B : 최초타겟 이후의 유닛과 다음 타겟과의 최대거리						
* 최대연결유닛수 : 최대연결유닛수에 도달하면 더 이상 연결 중지						
* 연결딜레이 : (ms) 최초 연결 이후 다음 연결까지의 딜레이 시간
*/
	public void update()
	{
		if(_enabled == false) return;
		
		_delay += GameManager.globalDeltaTime;

		if(_canMore == false)
		{
			if(_restoreCharacterAni == false)
			{
				_restoreCharacterAni = true;
				_attacker.onCompleteLoopSkillAni();
			}
			
			if(_delay >= _nextDelay)
			{
				i = _effects.Count;
				
				if(i > 0)
				{
					GameManager.me.effectManager.setElectricEffect(_effects[0]);
					_effects.RemoveAt(0);
				
					GameManager.me.effectManager.setParticleEffect(_particleEffects[0]);
					_particleEffects.RemoveAt(0);
				
					if(_targets.Count > 0)
					{
						_targets[0].chainLighting.Remove(this);
						_targets.RemoveAt(0);
					}
					
					_delay = 0.0f;
					_nextDelay = 0.2f;					
				}
				else
				{
					isDeleteObject = true;
				}
			}
			return;
		}
		
		if(_attacker.isEnabled == false)
		{
			_canMore = false;
			return;
		}
		
		if(_delay >= _connectionDelay)
		{
			_delay -= _connectionDelay;
			
			getNextTarget(_isPlayerBullet);
		}
		
		if(_nowConnectionNum >= _maxConnection)
		{
			_canMore = false;
			_delay = 0.0f;
		}
	}
	
	
	

	int removeIndex = 0;
	int targetLen = 0;
	public void removeCharacter(Monster target)
	{
		removeIndex = _targets.IndexOf(target);
		targetLen = _targets.Count;

		if(removeIndex > -1)
		{
			_targets.RemoveAt(removeIndex);

			if(_effects.Count == targetLen)
			{
				GameManager.me.effectManager.setElectricEffect(_effects[removeIndex]);
				_effects.RemoveAt(removeIndex);
			}
			if(_particleEffects.Count == targetLen)
			{
				GameManager.me.effectManager.setParticleEffect(_particleEffects[removeIndex]);
				_particleEffects.RemoveAt(removeIndex);
			}
		}

	}
	

	float _nowTotalDistance = 0.0f;
	float _tempF;
	public void getNextTarget(bool isPlayerSide)
	{
		_chaByRangeDistance.Clear();

		int targetCount = _targets.Count;

		if(targetCount == 0)
		{
			_v = _attacker.cTransformPosition;
		}
		else
		{
			_v =  _targets[_targets.Count-1].cTransformPosition;
		}

		Monster cha;

		if(isPlayerSide)
		{
			len = GameManager.me.characterManager.monsters.Count;
			
			
			// 총알에서 가까운 녀석들부터 처리하기위해... 
			for(i = len -1; i >= 0 ; --i)
			{	
				cha = GameManager.me.characterManager.monsters[i];
				if(cha.isEnabled == false  || cha.chainLighting.Contains(this))
				{
					continue; 
				}
				cha.distanceFromHitPoint = VectorUtil.DistanceXZ(_v, cha.cTransformPosition);
				_chaByRangeDistance.Add(cha);
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;
			
			// 총알에서 가까운 녀석들부터 처리하기위해... 
			for(i = len -1; i >= 0 ; --i)
			{	
				cha = GameManager.me.characterManager.playerMonster[i];
				if(cha.isEnabled == false  || cha.chainLighting.Contains(this)) continue; 
				cha.distanceFromHitPoint = VectorUtil.DistanceXZ(_v, cha.cTransformPosition);
				_chaByRangeDistance.Add(cha);
			}

		}

		_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPointCharacter);
		
		len = _chaByRangeDistance.Count;
		
		if(len <= 0) 
		{
			_canMore = false;
			_delay = 0.0f;	
			_chaByRangeDistance.Clear();
			return;
		}
		
		i = len - 1;
		
		cha = _chaByRangeDistance[i];
		_chaByRangeDistance.Clear();
		
		if(_nowConnectionNum == 0) // 최대연결거리 A만 계산.
		{

//			if(VectorUtil.DistanceXZ(_attacker.cTransformPosition, cha.cTransformPosition).AsFloat() > _distanceLimitA - _attacker.damageRange - cha.damageRange)

			if(VectorUtil.DistanceXZ(_attacker.cTransformPosition, cha.cTransformPosition).AsFloat() > _distanceLimitA)
			{
				_canMore = false;
				_delay = 0.0f;		
				return;
			}
		}
		else // 최대연결거리B와 최대 전체거리를 다 계산해야함.
		{
			if(targetCount == 0)
			{
				_canMore = false;
				_delay = 0.0f;		
				return;
			}

			_tempF = VectorUtil.DistanceXZ(_targets[targetCount-1].cTransformPosition, cha.cTransformPosition);

			if(_tempF > _distanceLimitB)
			{
				_canMore = false;
				_delay = 0.0f;					
				return;
			}
			else
			{
				_nowTotalDistance = 0.0f;
				for(i = _targets.Count - 1; i > 0; --i)
				{
					_nowTotalDistance += VectorUtil.DistanceXZ(_targets[i].cTransformPosition, _targets[i-1].cTransformPosition);
				}
				
				if(_nowTotalDistance + _tempF > _totalDistanceLimit )
				{
					_canMore = false;
					_delay = 0.0f;					
					return;						
				}
			}
		}
		
		_startPosition = cha.cTransformPosition;
		
		ElectricEffect eff = GameManager.me.effectManager.getElectricEffect(type);
		eff.mode = ElectricEffect.Mode.CHARACTER;
		_effects.Add(eff);
		
		ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(hitEffectId,GameManager.info.effectData[hitEffectId].resource);
		_v = cha.cTransformPosition;
		_v.y = cha.hitObject.top * 0.5f;

		if(GameManager.info.effectData[hitEffectId].maxSizeRatio < cha.effectSize)
		{
			pe.start(_v,cha.tf,true, GameManager.info.effectData[hitEffectId].maxSizeRatio);
		}
		else
		{
			pe.start(_v,cha.tf,true, cha.effectSize);
		}


		pe.shooterUniqueId = _attacker.stat.uniqueId;
		pe.checkSkillCam();

		_particleEffects.Add(pe);
		
		if(_nowConnectionNum == 0)
		{
			eff.cFrom = _attacker;	
			if(_attacker.shootingHand != null) eff.shootingPoint = _attacker.shootingHand;
		}
		else
		{
			eff.cFrom = _targets[targetCount-1];
		}
		
		eff.cTo = cha;
		eff.isEnabled = true;
		
		cha.chainLighting.Add(this);
		_targets.Add(cha);
		++_nowConnectionNum;	


		IFloat dp = 1.0f;

		if(_damageOffsetPerConnectionLevel > 0.0f && _nowConnectionNum > 1)
		{
			dp = 1.0f - (_nowConnectionNum - 1) * _damageOffsetPerConnectionLevel;
		}

		if(_skillData != null)
		{
			_skillData.applySkillEffect(cha, _skillLevel, null, _applyInforceLevel, _attacker, dp);
		}
		else
		{
			cha.damage(_attacker.stat.monsterType,
			           _attacker,
			           _attacker.stat.uniqueId,false, 
			           1, 
			           _attacker.tf, 
			           _attacker.stat.atkPhysic, 
			           _attacker.stat.atkMagic, 
			           1.0f, 
			           1.0f, 
			           true, 
			           GameManager.info.skillEffectSetupData[_attacker.atkEffectType].effUp, 
			           GameManager.info.skillEffectSetupData[_attacker.atkEffectType].soundDown, 
			           dp
			           );
		}
		

	}
	
	
	
	


	
	
	int i = 0;
	
	private bool _enabled = false;
	
	public bool isEnabled
	{
		set
		{
			_enabled = value;
			
			if(value == false)
			{
				for(i = _effects.Count - 1; i >= 0; --i)
				{
					GameManager.me.effectManager.setElectricEffect(_effects[i]);
				}
				
				for(i = _particleEffects.Count - 1; i >= 0; --i)
				{
					GameManager.me.effectManager.setParticleEffect(_particleEffects[i]);
				}				
				
				for(i = _targets.Count - 1; i >= 0; --i)
				{
					_targets[i].chainLighting.Remove(this);
				}
				
				_targets.Clear();				
				_effects.Clear();
				_particleEffects.Clear();
				_skillData = null;
				
				if(_restoreCharacterAni == false)
				{
					_restoreCharacterAni = true;
					if(_attacker != null && _attacker.isEnabled) _attacker.onCompleteLoopSkillAni();
				}

				_attacker = null;
			}
		}
	}
	
}

