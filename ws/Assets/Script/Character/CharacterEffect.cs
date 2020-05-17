using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterEffect
{
	public Monster cha;
	private IVector3 _v;
	
	public List<CharacterSkillEffectController> effects = new List<CharacterSkillEffectController>();
	public Dictionary<int, CharacterSkillEffectController> effectsDic = new Dictionary<int, CharacterSkillEffectController>();

	private static readonly int[] dicKeys = new int[]{10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,29,30,31,32,34,35,36};

	public static SkillEffectData knockBackData;
	public static SkillEffectData stunData;

	public void destroy()
	{
		cha = null;
		clear ();
		effects = null;

		for(int i = dicKeys.Length - 1; i >= 0; --i)
		{
			if(effectsDic[dicKeys[i]] != null) effectsDic[dicKeys[i]].clear();
			effectsDic[dicKeys[i]].effect = null;
			effectsDic[dicKeys[i]] = null;
		}

		effectsDic.Clear();
		effectsDic = null;
	}

	public CharacterEffect()
	{

		effectsDic[10] = new CharacterSkillEffectControllerType10();
		effectsDic[10].effect = this;
		
		effectsDic[11] = new CharacterSkillEffectControllerType11();
		effectsDic[11].effect = this;
		
		effectsDic[12] = new CharacterSkillEffectControllerType12();
		effectsDic[12].effect = this;
		
		effectsDic[13] = new CharacterSkillEffectControllerType13();
		effectsDic[13].effect = this;
		
		effectsDic[14] = new CharacterSkillEffectControllerType14();
		effectsDic[14].effect = this;
		
		effectsDic[15] = new CharacterSkillEffectControllerType15();
		effectsDic[15].effect = this;
		
		effectsDic[16] = new CharacterSkillEffectControllerType16();
		effectsDic[16].effect = this;
		
		effectsDic[17] = new CharacterSkillEffectControllerType17();
		effectsDic[17].effect = this;
		
		effectsDic[18] = new CharacterSkillEffectControllerType18();
		effectsDic[18].effect = this;
		
		effectsDic[19] = new CharacterSkillEffectControllerType19();
		effectsDic[19].effect = this;
		
		effectsDic[20] = new CharacterSkillEffectControllerType20();
		effectsDic[20].effect = this;
		
		effectsDic[21] = new CharacterSkillEffectControllerType21();
		effectsDic[21].effect = this;
		
		effectsDic[22] = new CharacterSkillEffectControllerType22();
		effectsDic[22].effect = this;
		
		effectsDic[23] = new CharacterSkillEffectControllerType23();
		effectsDic[23].effect = this;
		
		effectsDic[24] = new CharacterSkillEffectControllerType24();
		effectsDic[24].effect = this;
		
		effectsDic[25] = new CharacterSkillEffectControllerType25();
		effectsDic[25].effect = this;

		effectsDic[29] = new CharacterSkillEffectControllerType29();
		effectsDic[29].effect = this;
		
		effectsDic[30] = new CharacterSkillEffectControllerType30();
		effectsDic[30].effect = this;
		
		effectsDic[31] = new CharacterSkillEffectControllerType31();
		effectsDic[31].effect = this;
		
		effectsDic[32] = new CharacterSkillEffectControllerType32();
		effectsDic[32].effect = this;

		effectsDic[34] = new CharacterSkillEffectControllerType34();
		effectsDic[34].effect = this;

		effectsDic[35] = new CharacterSkillEffectControllerType35();
		effectsDic[35].effect = this;

		effectsDic[36] = new CharacterSkillEffectControllerType36();
		effectsDic[36].effect = this;

	}


	private bool _hasIcon = false;
	private Queue<string> _waitIcon = new Queue<string>();

	public void addIconBuff(string effId)
	{
		if(UIPopupSkillPreview.isOpen) return;
		if(string.IsNullOrEmpty(effId)) return;
		if(_waitIcon.Contains(effId)) return;

		_hasIcon = true;
		_waitTime = 0.0f;
		_waitIcon.Enqueue(effId);
	}

	string iconId;
	float _waitTime = 0.0f;
	void checkBuffIcon()
	{
		if(_hasIcon && GameManager.me.stageManager.playTime >= _waitTime)
		{
			float posX = 0;
			float posY = cha.hitObject.height + 50.0f;
			int num = _waitIcon.Count;
			for(int i = 0; i < 3 && i < num; ++i)
			{
				iconId = _waitIcon.Dequeue();
				ParticleEffect pe = GameManager.me.effectManager.getParticleEffect(iconId, GameManager.info.effectData[iconId].resource);

				if(i == 0)
				{
					if(num > 1) posX = -40.0f;
				}
				else if(i == 1)
				{
					posX = 40.0f;
				}
				else if(i == 2) posY += 40.0f;

				pe.start(cha.cTransformPosition, cha.tf, false, 1, posX, posY);//, cha.shadowSize * customScaleRatio, posX, posY, posZ);
				pe.transform.parent = cha.cTransform;
			}

			_waitTime = GameManager.me.stageManager.playTime + 2.0f;

			if(_waitIcon.Count <= 0) _hasIcon = false;
		}
	}

	
	public bool isPlayingEffect(int type)
	{
		return effectsDic[type].active;
	}


	public void addKnockBack(float value)
	{
		if(effectsDic[25].active == false)
		{
			effects.Add(effectsDic[25]);
		}

		EffectManager.showEfectBySkillEffectType(25, cha, null);
		// 넉백 속도.
		effectsDic[25].init(Monster.TYPE.NONE, -1000, knockBackData, value, value * 0.001f, -0.001f);
	}


	public void addStun(float value)
	{
		if(effectsDic[30].active == false)
		{
			effects.Add(effectsDic[30]);
		}

		effectsDic[30].init(Monster.TYPE.NONE, -1000, stunData, value, value * 0.001f);
	}

	//mon.characterEffect.addStun(GameManager.info.setupData.tagStunTime);


	public void check35(IFloat damage)
	{
		if(effectsDic[35].active)
		{
			damage = damage.AsFloat() * (effectsDic[35].applyValue * 0.01f);

			cha.damage(Monster.TYPE.NONE, null, -1000, false, damage);
		}
	}


	public bool check36(Monster targetMon)
	{
		if( effectsDic[36].active )
		{
			if( Xfloat.lessThan(  VectorUtil.DistanceXZ( cha.cTransformPosition, targetMon.cTransformPosition )  , effectsDic[36].applyValue))
			{
				return true;
			}
		}

		return false;
	}


	
	public void addEffect(Bullet bullet, Monster attacker, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		if(effectsDic[p_skillEffect.type].active == false)
		{
			effects.Add(effectsDic[p_skillEffect.type]);
		}

		if(bullet != null)
		{
			effectsDic[p_skillEffect.type].init(bullet.attackerInfo.stat.monsterType, bullet.attackerInfo.uniqueId, p_skillEffect, p_applyValue, p_duration * 0.001f, p_timeOffset * 0.001f);
		}
		else if(attacker != null)
		{
			effectsDic[p_skillEffect.type].init(attacker.stat.monsterType, attacker.stat.uniqueId, p_skillEffect, p_applyValue, p_duration * 0.001f, p_timeOffset * 0.001f);
		}
		else 
		{
			effectsDic[p_skillEffect.type].init(Monster.TYPE.NONE, -1000, p_skillEffect, p_applyValue, p_duration * 0.001f, p_timeOffset * 0.001f);
		}
	}
	
	
	public void copyEffect(CharacterEffect sourceEffect)
	{
		int len = sourceEffect.effects.Count;
		
		for(int i = 0; i < len; ++i)
		{
			// 공격 혹은 디버프 계열만...
			if(sourceEffect.effects[i].skillEffect.skillData != null && sourceEffect.effects[i].skillEffect.skillData.checkMissChance )
			{
				if(sourceEffect.effects[i].skillEffect.type == 29) continue;

				if(effectsDic[sourceEffect.effects[i].skillEffect.type].active == false)
				{
					effects.Add(effectsDic[sourceEffect.effects[i].skillEffect.type]);
				}				
				
				effectsDic[sourceEffect.effects[i].skillEffect.type].init(sourceEffect.effects[i].effectShooterType, sourceEffect.effects[i].effectAttackerUniqueId, sourceEffect.effects[i].skillEffect, sourceEffect.effects[i].applyValue, sourceEffect.effects[i].duration, sourceEffect.effects[i].timeOffset);
				effectsDic[sourceEffect.effects[i].skillEffect.type].delay = sourceEffect.effects[i].delay;
			}
		}
	}	
	
	
	public void removeEffect(int type, bool doRestore = false)
	{
		effects.Remove(effectsDic[type]);
		if(doRestore) effectsDic[type].restore();
		effectsDic[type].isEnabled = false;
	}
	
	
	public void removeEffectBySkillType(Skill.Type type, int exceptionType = -1)
	{
		int len = effects.Count;
		
		for(int i = len-1; i >= 0; --i)
		{
			if(effects[i].skillEffect.skillData != null && effects[i].skillEffect.skillData.skillType == type)
			{
				if(effects[i].skillEffect.type == exceptionType)
				{
					continue;
				}

				int typeIndex = effects[i].skillEffect.type;								
				effectsDic[typeIndex].restore();
				effectsDic[typeIndex].isEnabled = false;
			}
		}
	}
	
	public bool hasAffectingEffect()
	{
		int len = effects.Count;
		
		for(int i = len-1; i >= 0; --i)
		{
			// 공격 혹은 디버프 계열을 뜻한다.
			if( effects[i].skillEffect.skillData != null && effects[i].skillEffect.skillData.checkMissChance && effects[i].skillEffect.type != 32)
			{
				return true;
			}
		}
	
		return false;
	}
	

	// 죽거나 초기화할때 모두 삭제하는 것.
	// 이때 스탯을 재 입력 시켜서는 안된다.
	public void clear()
	{
		int len = effects.Count;
		int type = 0;
		
		for(int i = len-1; i >= 0; --i)
		{
			effects[i].clear();
			effects[i].isEnabled = false;
		}

		effects.Clear();

		_hasIcon = false;
		_waitIcon.Clear();
		_waitTime = 0.0f;
	}
	
	
	public bool check()
	{
		int len = effects.Count;
		for(int i = len-1; i >= 0; --i)
		{
			if(cha.isEnabled == false) return false;
			
			effects[i].update();	
		}

		checkBuffIcon();

		return true;
	}
}



public class CharacterSkillEffectController
{
	public bool isPlaying = false;
	public bool isMultiple = false;
	public Xfloat duration = 0.0f;
	public Xfloat timeOffset = 0.0f;
	
	public CharacterEffect effect;
	
	public bool active = false;
	public Xfloat delay = 0.0f;
	public Xfloat startDelay = -1.0f;

	public bool checkStartDelay = false;
	
	protected Xfloat _offsetChecker = 0.0f;
	protected Xfloat _tempF;
	protected int _tempI;

	public int effectAttackerUniqueId = -1000;

	public SkillEffectData skillEffect;
		
	public Xfloat applyValue;

	public CharacterSkillEffectController()
	{
	}
	
	protected ParticleEffect pe = null;
	protected ParticleEffect loopPe = null;
	public Monster.TYPE effectShooterType = Monster.TYPE.NONE;
	
	public virtual void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		if(pe != null && pe.particle.loop == false)
		{
			GameManager.me.effectManager.setParticleEffect(pe);
			pe = null;
		}

		effectAttackerUniqueId = attackerUniqueId;
		effectShooterType = shooterType;
		skillEffect = p_skillEffect;
		applyValue = p_applyValue;
		duration = p_duration; // 적용시간
		timeOffset = p_timeOffset; // 간격
		isMultiple = (p_timeOffset >= 0.0f); // 간격 적용되는 이펙트인지. -1 기본값을 가진 녀석들은 1회성이다.
		active = true;
		delay = 0.0f;
		_offsetChecker = 0.0f;
		startDelay = p_skillEffect.startDelay;
		checkStartDelay = (startDelay > 0);
	}

	public virtual void applyFirst()
	{
		
	}

	public virtual void update(){}
	
	public virtual void restore()
	{
	}


	// restore 에서는 수치를 원상복구하고
	// clear에서는 이펙트나 캐릭터 상태를 지운다.

	public virtual void clear()
	{
		if(loopPe != null)
		{
			GameManager.me.effectManager.setParticleEffect(loopPe);
			loopPe = null;
		}

		if(pe != null)
		{
			GameManager.me.effectManager.setParticleEffect(pe);
			pe = null;
		}
	}


	protected bool hasStartDelay()
	{
		if(checkStartDelay)
		{
			if(startDelay > 0)
			{
				startDelay -= GameManager.globalDeltaTime;
			}
			else
			{
				applyFirst();
				checkStartDelay = false;
			}
			
			return true;
		}
		
		return false;
	}

	
	public virtual void applyEffect(bool isFirst = false){}		



	public bool isEnabled
	{
		set
		{
			if(value == false)
			{
				active = false;
				isPlaying = false;
				skillEffect = null;

				_tempF = 0;
				_tempI = 0;
			}
		}
	}
}



// 생명력 최대 비율.
sealed public class CharacterSkillEffectControllerType10 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		if(active)
		{
			effect.cha.maxHp = effect.cha.originalMaxHp;
			effect.cha.hp = effect.cha.hp;					
		}

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);

		_tempI = (int)(effect.cha.maxHp * (float)p_applyValue * 0.01f);

		if(checkStartDelay == false) applyFirst();
	}


	sealed public override void applyFirst()
	{
		effect.cha.maxHp += _tempI;
		effect.cha.hp += _tempI;
		effect.cha.hpEffect(_tempI);
		
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(10,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
		EffectManager.showEfectBySkillEffectTypeWithCharacterSize(10,effect.cha, EffectManager.SkillEffectType.isUp);
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[10].upIcon);
		
		effect.cha.setEffectHpBar(true);
		
		SoundData.playHitSound(10,true);
	}


	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.maxHp = effect.cha.originalMaxHp;
		effect.cha.hp = effect.cha.hp;		
		effect.cha.setEffectHpBar(false);

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
	
}


// 생명력 지속.
sealed public class CharacterSkillEffectControllerType11 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		// 이펙트 효과 적용.
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		applyEffect(true);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(11,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[11].downIcon);		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		else
		{
			if(_offsetChecker >= timeOffset)
			{
				_offsetChecker -= timeOffset;
				applyEffect();
			}
		}
		
		_offsetChecker += GameManager.globalDeltaTime;
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[11].effDown) == false)
		{
			effect.cha.damage(effectShooterType, null, -1, true, applyValue,true, GameManager.info.skillEffectSetupData[11].effDown, GameManager.info.skillEffectSetupData[11].soundDown);	
		}
		else
		{
			effect.cha.damage(effectShooterType, null, -1, true, applyValue,false, null, GameManager.info.skillEffectSetupData[11].soundDown);		
		}
		
		
	}
	
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}


//화상 (↑동일)
sealed public class CharacterSkillEffectControllerType12 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		// 이펙트 효과 적용.
		if(checkStartDelay == false) applyFirst();

	}

	sealed public override void applyFirst()
	{
		applyEffect(true);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(12,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[12].downIcon);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
			return;
		}
		else
		{
			if(_offsetChecker >= timeOffset)
			{
				_offsetChecker -= timeOffset;
				applyEffect();
			}
		}
		
		_offsetChecker += GameManager.globalDeltaTime;
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(isFirst)
		{
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[12].effDown) == false)
			{
				if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[12].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			}
			
			effect.cha.changeColorSet(Monster.ColorSet.BURN);
		}

		effect.cha.damage(effectShooterType, null, -1, true, MathUtil.getDamage(0,effect.cha.stat.defPhysic ,applyValue, effect.cha.stat.defMagic) ,false, null, GameManager.info.skillEffectSetupData[12].soundDown);	
	}
	
	sealed public override void restore()
	{
		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}

	public override void clear ()
	{
		base.clear ();
		if(effect.cha != null) effect.cha.changeColorSet();
		if(pe != null) GameManager.me.effectManager.setParticleEffect(pe);
		pe = null;
	}

}


//부패  (↑동일)
sealed public class CharacterSkillEffectControllerType13 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		// 이펙트 효과 적용.
		if(checkStartDelay == false) applyFirst();
	}


	sealed public override void applyFirst()
	{
		applyEffect(true);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(13,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[13].downIcon);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
			
		}
		else
		{
			if(_offsetChecker >= timeOffset)
			{
				_offsetChecker -= timeOffset;
				applyEffect();
			}
		}
		
		_offsetChecker += GameManager.globalDeltaTime;
		delay += GameManager.globalDeltaTime;		
	}

	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(isFirst)
		{
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[13].effDown) == false)
			{
				if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[13].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			}
			
			effect.cha.changeColorSet(Monster.ColorSet.DECAY);
		}
		
		effect.cha.damage(effectShooterType, null, -1, true, MathUtil.getDamage(0,effect.cha.stat.defPhysic ,applyValue, effect.cha.stat.defMagic), false, null, GameManager.info.skillEffectSetupData[13].soundDown);	
	}
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		

	public override void clear ()
	{
		base.clear ();
		if(effect.cha != null) effect.cha.changeColorSet();
		
		if(pe != null) GameManager.me.effectManager.setParticleEffect(pe);
		pe = null;
	}

}


//========================= 14 ~ 17은 동일한 형식.

//물리공격력
sealed public class CharacterSkillEffectControllerType14 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.atkPhysic -= _tempF;
//			if(effect.cha.originalAtkPhysic < effect.cha.stat.atkPhysic) effect.cha.stat.atkPhysic = effect.cha.originalAtkPhysic;
//		}

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		_tempF = applyValue;
		
		//		if(effect.cha.stat.atkPhysic + _tempF < 0)
		//		{
		//			_tempF -= (effect.cha.stat.atkPhysic + _tempF);
		//		}
		//		effect.cha.stat.atkPhysic += _tempF;
		
		effect.cha.stat.atkPhysic.Set( effect.cha.originalAtkPhysic + applyValue );
		if(effect.cha.stat.atkPhysic < 0) effect.cha.stat.atkPhysic.Set( 0 );
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[14].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(14,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[14].upIcon);
			SoundData.playHitSound(14,true);
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[14].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(14,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[14].downIcon);
			SoundData.playHitSound(14,false);
		}		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.atkPhysic.Set( effect.cha.originalAtkPhysic );

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}

//마법공격력
sealed public class CharacterSkillEffectControllerType15 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.atkMagic -= _tempF;
//			if(effect.cha.originalAtkMagic < effect.cha.stat.atkMagic) effect.cha.stat.atkMagic = effect.cha.originalAtkMagic;
//		}

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);		
		
		if(checkStartDelay == false) applyFirst();

	}

	sealed public override void applyFirst()
	{
		_tempF = applyValue;
		
		//		if(effect.cha.stat.atkMagic + _tempF < 0)
		//		{
		//			_tempF -= (effect.cha.stat.atkMagic + _tempF);
		//		}
		//		effect.cha.stat.atkMagic += _tempF;	
		
		effect.cha.stat.atkMagic.Set( effect.cha.originalAtkMagic + applyValue );
		if(effect.cha.stat.atkMagic < 0) effect.cha.stat.atkMagic.Set( 0 );
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[15].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(15,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[15].upIcon);
			SoundData.playHitSound(15,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[15].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(15,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[15].downIcon);
			SoundData.playHitSound(15,false);
		}			
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.atkMagic.Set( effect.cha.originalAtkMagic );

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}


// 물리방어력
sealed public class CharacterSkillEffectControllerType16 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.defPhysic -= _tempF;
//			if(effect.cha.originalDefPhysic < effect.cha.stat.defPhysic) effect.cha.stat.defPhysic = effect.cha.originalDefPhysic;
//		}

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
	
		if(checkStartDelay == false) applyFirst();

		
	}

	sealed public override void applyFirst()
	{
		_tempF = applyValue;
		
		effect.cha.stat.defPhysic = effect.cha.originalDefPhysic + applyValue;
		if(effect.cha.stat.defPhysic < 0) effect.cha.stat.defPhysic = 0;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[16].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(16,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[16].upIcon);
			SoundData.playHitSound(16,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[16].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(16,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[16].downIcon);
			SoundData.playHitSound(16,false);
		}	
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.defPhysic = effect.cha.originalDefPhysic;

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}

//마법방어력
sealed public class CharacterSkillEffectControllerType17 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
	
		if(checkStartDelay == false) applyFirst();
		
	}

	sealed public override void applyFirst()
	{
		_tempF = applyValue;
		
		//		if(effect.cha.stat.defMagic + _tempF < 1)
		//		{
		//			_tempF -= (effect.cha.stat.defMagic + _tempF - 1);
		//		}
		//		effect.cha.stat.defMagic += _tempF;			
		
		effect.cha.stat.defMagic = effect.cha.originalDefMagic + applyValue;
		if(effect.cha.stat.defMagic < 0) effect.cha.stat.defMagic = 0;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[17].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(17,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[17].upIcon);
			SoundData.playHitSound(17,true);
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[17].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(17,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[17].downIcon);
			SoundData.playHitSound(17,false);
		}	
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.defMagic = effect.cha.originalDefMagic;

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}



//=========================


//물리공격력 비율
sealed public class CharacterSkillEffectControllerType18 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.atkPhysic -= _tempF;
//			if(effect.cha.originalAtkPhysic < effect.cha.stat.atkPhysic) effect.cha.stat.atkPhysic = effect.cha.originalAtkPhysic;
//		}

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();

	}

	sealed public override void applyFirst()
	{
		_tempF = effect.cha.originalAtkPhysic * ((float)applyValue * 0.01f);
		
		//		if(effect.cha.stat.atkPhysic + _tempF < 0)
		//		{
		//			_tempF -= (effect.cha.stat.atkPhysic + _tempF);
		//		}
		//		effect.cha.stat.atkPhysic += _tempF;		
		
		effect.cha.stat.atkPhysic.Set( effect.cha.originalAtkPhysic + _tempF );
		if(effect.cha.stat.atkPhysic < 0) effect.cha.stat.atkPhysic.Set( 0 );
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[18].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(18,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[18].upIcon);
			SoundData.playHitSound(18,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[18].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(18,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[18].downIcon);
			SoundData.playHitSound(18,false);
		}	
	}


	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.atkPhysic.Set( effect.cha.originalAtkPhysic );

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}

// 마법공격력 비율
sealed public class CharacterSkillEffectControllerType19 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();

	}

	sealed public override void applyFirst()
	{
		_tempF = effect.cha.originalAtkMagic * ((float)applyValue * 0.01f);
		
		
		effect.cha.stat.atkMagic.Set( effect.cha.originalAtkMagic + _tempF );
		if(effect.cha.stat.atkMagic < 0) effect.cha.stat.atkMagic.Set( 0 );
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[19].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(19,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[19].upIcon);
			SoundData.playHitSound(19,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[19].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(19,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[19].downIcon);
			SoundData.playHitSound(19,false);
		}	
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.atkMagic.Set( effect.cha.originalAtkMagic );

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}

//물리방어력 비율
sealed public class CharacterSkillEffectControllerType20 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);

		if(checkStartDelay == false) applyFirst();


	}

	sealed public override void applyFirst()
	{
		/*
		_tempF = effect.cha.originalDefPhysic * ((float)applyValue * 0.01f);
		effect.cha.stat.defPhysic = effect.cha.originalDefPhysic + _tempF;
		*/
		
		/*
		N		물리방어력 비율 / 마법방어력 비율 적용 방식				
			
			* 원본 방어력 A, 증가/감소 방어력 비율을 B 라고 할때,				
			▷변경 방어력 = A * (1 + B * 0.01) ^2				
				*/
		
		_tempF =  1 + (float)applyValue * 0.01f;
		_tempF = _tempF * _tempF;
		effect.cha.stat.defPhysic = effect.cha.originalDefPhysic * _tempF;
		
		if(effect.cha.stat.defPhysic < 0) effect.cha.stat.defPhysic = 0;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[20].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(20,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[20].upIcon);
			SoundData.playHitSound(20,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[20].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(20,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[20].downIcon);
			SoundData.playHitSound(20,false);
		}	
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.defPhysic = effect.cha.originalDefPhysic;

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}

// 마법방어력 비율
sealed public class CharacterSkillEffectControllerType21 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.defMagic -= _tempF;
//			if(effect.cha.originalDefMagic < effect.cha.stat.defMagic) effect.cha.stat.defMagic = effect.cha.originalDefMagic;
//		}
//		
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);

		if(checkStartDelay == false) applyFirst();

	}

	sealed public override void applyFirst()
	{
		/*
		_tempF = effect.cha.originalDefMagic * ((float)applyValue * 0.01f);

		effect.cha.stat.defMagic = effect.cha.originalDefMagic + _tempF;
*/
		
		/*
		N		물리방어력 비율 / 마법방어력 비율 적용 방식				
			
			* 원본 방어력 A, 증가/감소 방어력 비율을 B 라고 할때,				
			▷변경 방어력 = A * (1 + B * 0.01) ^2				
				*/
		
		_tempF =  1 + (float)applyValue * 0.01f;
		_tempF = _tempF * _tempF;
		effect.cha.stat.defMagic = effect.cha.originalDefMagic * _tempF;
		
		
		if(effect.cha.stat.defMagic < 0) effect.cha.stat.defMagic = 0;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[21].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(21,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[21].upIcon);
			SoundData.playHitSound(21,true);
			
		}
		else // 하락이펙트.
		{
			GameManager.info.effectData[GameManager.info.skillEffectSetupData[21].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(21,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[21].downIcon);
			SoundData.playHitSound(21,false);
		}	
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.defMagic = effect.cha.originalDefMagic;

		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}



//== 공격 속도.
sealed public class CharacterSkillEffectControllerType22 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();

	}
	

	sealed public override void applyFirst()
	{
		// 공격속도는 수치가 높으면 실제 속도가 느려지는 거다. 그래서 얘에 한해 값을 반전해준다.
		//		if(applyValue >= 0)
		//		{
		//			_tempF = effect.cha.originalAtkSpeed * (1.0f/(1.0f + (float)applyValue * 0.01f));	
		//		}
		//		else
		//		{
		//			_tempF = effect.cha.originalAtkSpeed - (effect.cha.originalAtkSpeed * ((float)applyValue * 0.01f));
		//		}
		
		_tempF = effect.cha.originalAtkSpeed * (1.0f/(1.0f + (float)applyValue * 0.01f));	
		
		
		effect.cha.stat.atkSpeed = _tempF;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[22].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(22,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[22].upIcon);
			SoundData.playHitSound(22,true);
			
		}
		else // 하락이펙트.
		{
			if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[22].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(22,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[22].downIcon);
			SoundData.playHitSound(22,false);
		}	
		
		effect.cha.changeAttackAnimationSpeed(effect.cha.originalAtkSpeed/effect.cha.stat.atkSpeed);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{

		effect.cha.stat.atkSpeed = effect.cha.originalAtkSpeed;
		effect.cha.changeAttackAnimationSpeed(1);
		if(effect.cha.state.Contains(Monster.SHOOT_HEADER))
		{
			effect.cha.clearAnimationMethod();
			effect.cha.onCompleteAttackAni();		
		}

//		if(VersionData.checkCodeVersion(4))
//		{
//
//		}
//		else
//		{
//			effect.cha.action.setAttackDelay();
//		}

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}


//== 이동 속도.
sealed public class CharacterSkillEffectControllerType23 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{

		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		_tempF = effect.cha.originalSpeed * ((float)applyValue * 0.01f);
		
		
		effect.cha.stat.speed = effect.cha.originalSpeed + _tempF;
		
		if(applyValue > 0) // 상승 이펙트.
		{
			if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[23].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(23,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[23].upIcon);
			SoundData.playHitSound(23,true);
			
		}
		else // 하락이펙트.
		{
			if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[23].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(23,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
			effect.addIconBuff(GameManager.info.skillEffectSetupData[23].downIcon);
			SoundData.playHitSound(23,false);
		}
		
		effect.cha.changeWalkAnimationSpeed( effect.cha.stat.speed/effect.cha.originalSpeed);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.speed = effect.cha.originalSpeed;
		effect.cha.changeWalkAnimationSpeed( effect.cha.stat.speed/effect.cha.originalSpeed);

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
}

// 결빙. 이동속도와 공격속도가 동시에 감소.
sealed public class CharacterSkillEffectControllerType24 : CharacterSkillEffectController
{
	float _tempF2;
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
//		if(active)
//		{
//			effect.cha.stat.atkSpeed -= _tempF;
//			effect.cha.stat.speed -= _tempF2;
//			if(effect.cha.stat.speed < 0) effect.cha.stat.speed = 0;				
//		}
		
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		// 1. 공격 속도 
		// 공격속도는 수치가 높으면 실제 속도가 느려지는 거다. 그래서 얘에 한해 값을 반전해준다.
		//공격딜레이 * 1/(1+속성1/100)
		
		// 공격속도는 수치가 높으면 실제 속도가 느려지는 거다. 그래서 얘에 한해 값을 반전해준다.
		if(applyValue >= 0)
		{
			_tempF = effect.cha.originalAtkSpeed * (1.0f/(1.0f + (float)applyValue * 0.01f));	
		}
		else
		{
			_tempF = effect.cha.originalAtkSpeed - (effect.cha.originalAtkSpeed * ((float)applyValue * 0.01f));
		}
		//effect.cha.stat.atkSpeed += _tempF;		
		
		effect.cha.stat.atkSpeed =  _tempF;
		
		// 2. 이동속도 
		_tempF2 = effect.cha.originalSpeed * ((float)applyValue * 0.01f);
		
		//		if(effect.cha.stat.speed + _tempF2 < 0)
		//		{
		//			_tempF2 -= (effect.cha.stat.speed + _tempF2 - 0);
		//		}
		//		effect.cha.stat.speed += _tempF2;	
		
		effect.cha.stat.speed = effect.cha.originalSpeed + _tempF2;
		if(effect.cha.stat.speed < 0) effect.cha.stat.speed = 0.0f;
		
		
		effect.cha.changeAttackAnimationSpeed(effect.cha.originalAtkSpeed/effect.cha.stat.atkSpeed);		
		effect.cha.changeWalkAnimationSpeed( effect.cha.stat.speed/effect.cha.originalSpeed);
		
		if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[24].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(24,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		effect.addIconBuff(GameManager.info.skillEffectSetupData[24].downIcon);
		
		SoundData.playHitSound(24,false);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void restore()
	{
		effect.cha.stat.atkSpeed = effect.cha.originalAtkSpeed;
		effect.cha.stat.speed = effect.cha.originalSpeed;

		effect.cha.changeAttackAnimationSpeed(effect.cha.originalAtkSpeed/effect.cha.stat.atkSpeed);
		effect.cha.changeWalkAnimationSpeed( effect.cha.stat.speed/effect.cha.originalSpeed);

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}		
		
}

sealed public class CharacterSkillEffectControllerType25 : CharacterSkillEffectController
{
	IVector3 _oriPos;
	IVector3 _targetPos;

	private float _p_applyValue;

	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		// 이미 넉백이 걸려있으면 미적용한다.
		if(active == false)
		{
			if(p_duration < 0) p_duration *= -1.0f;

			base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);	

			_p_applyValue = p_applyValue;

			if(checkStartDelay == false) applyFirst();
		}

	}
	

	sealed public override void applyFirst()
	{
		if(effect.cha.state.Contains(Monster.SHOOT_HEADER))
		{
			effect.cha.onCompleteAttackAni();
		}
		
		effect.cha.state = Monster.NORMAL;
		
		_oriPos = effect.cha.cTransformPosition;
		_targetPos = effect.cha.cTransformPosition -((IVector3)(effect.cha.tf.forward) * (float)applyValue);
		
		if(_targetPos.z - effect.cha.damageRange <= MapManager.bottom || _targetPos.z + effect.cha.damageRange >= MapManager.top)
		{
			_targetPos = effect.cha.cTransformPosition;
			_targetPos.x += (float)applyValue * ((effect.cha.isPlayerSide)?-1.0f:1.0f);
		}
		
		if( Xfloat.lessThan( (_targetPos.x - effect.cha.hitObject.width).AsFloat() , StageManager.mapStartPosX.Get() )) 
		{
			_targetPos.x = StageManager.mapStartPosX.Get() + effect.cha.hitObject.width;
		}
		else if(Xfloat.greatEqualThan( (_targetPos.x + effect.cha.hitObject.width).AsFloat() , StageManager.mapEndPosX.Get())) 
		{
			_targetPos.x = StageManager.mapEndPosX.Get() - effect.cha.hitObject.width;
		}
		else if(applyValue < 0)
		{
			if(effect.cha.isPlayerSide)
			{
				if( Xfloat.greaterThan( (_targetPos.x + effect.cha.hitObject.width).AsFloat() , GameManager.me.characterManager.monsterLeftLine.Get() )) 
				{
					_targetPos.x = GameManager.me.characterManager.monsterLeftLine.Get() - effect.cha.hitObject.width;
					
					if(_targetPos.x < effect.cha.cTransformPosition.x)
					{
						_targetPos.x = effect.cha.cTransformPosition.x;
					}
				}
			}
			else
			{
				if( Xfloat.lessThan( (_targetPos.x - effect.cha.hitObject.width).AsFloat() , GameManager.me.characterManager.playerMonsterRightLine.Get() )) 
				{
					_targetPos.x = GameManager.me.characterManager.playerMonsterRightLine.Get() + effect.cha.hitObject.width;
					
					if(_targetPos.x > effect.cha.cTransformPosition.x)
					{
						_targetPos.x = effect.cha.cTransformPosition.x;
					}
				}		
			}
		}
		
		
		effect.cha.removeAttacker();
		effect.cha.setTarget( null );
		effect.cha.setSkillTarget( null );
		
		Quaternion q = new Quaternion();
		
		if(effect.cha.isPlayerSide)
		{
			if(_p_applyValue < 0)
			{
				q.eulerAngles = new Vector3(0, 180, 0);
			}
			else
			{
				q.eulerAngles = new Vector3(0, 0, 0);
			}
		}
		else
		{
			if(_p_applyValue < 0)
			{
				q.eulerAngles = new Vector3(0, 0, 0);
			}
			else
			{
				q.eulerAngles = new Vector3(0, 180, 0);
			}
		}
		
		
		GameManager.info.effectData["E_KNOCKBACK_01"].getEffect(-1000,_oriPos).transform.localRotation = q;
		
		SoundData.playHitSound(25,true);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		delay += GameManager.globalDeltaTime;
		
		if(delay > duration)
		{
			effect.cha.setPosition( _targetPos );
			restore();
		}
		else
		{
			effect.cha.isFreeze.Set( true );

			IFloat step = Easing.EaseOut(delay/duration, EasingType.Quadratic);

			IVector3 v = Vector3.Lerp(_oriPos, _targetPos, step);

			// 끌어당기때는 타겟존을 넘지 못하도록 보정해줘야한다.
			if(applyValue < 0)
			{
				if(effect.cha.isPlayerSide)
				{
					if( Xfloat.greaterThan( (v.x + effect.cha.hitObject.width).AsFloat() , GameManager.me.characterManager.monsterLeftLine.Get() )) 
					{
						restore();
						return;
					}
				}
				else
				{
					if( Xfloat.lessThan( (v.x - effect.cha.hitObject.width).AsFloat() , GameManager.me.characterManager.playerMonsterRightLine.Get() )) 
					{
						restore();
						return;
					}		
				}
			}

			effect.cha.setPosition ( v );	
		}
	}
	
	sealed public override void restore ()
	{
		effect.cha.isFreeze.Set( false );

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);	
	}


	
}







// 아군으로 전환 & HP 감소.
sealed public class CharacterSkillEffectControllerType29 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		// 이펙트 효과 적용.
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		applyEffect(true);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(29,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

//		if(delay >= duration)
//		{
//			// 종료.
//			restore();
//		}
//		else
		{
			if(_offsetChecker >= timeOffset)
			{
				_offsetChecker -= timeOffset;
				applyEffect();
			}
		}
		
		_offsetChecker += GameManager.globalDeltaTime;
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[29].effDown) == false)
		{
			effect.cha.damage(effectShooterType, null, -1, true, applyValue,true, GameManager.info.skillEffectSetupData[29].effDown, (isFirst)?GameManager.info.skillEffectSetupData[29].soundDown:null);	
		}
		else
		{
			effect.cha.damage(effectShooterType, null, -1, true, applyValue,false, null, GameManager.info.skillEffectSetupData[29].soundDown);		
		}
	}
	
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}








// 스턴
sealed public class CharacterSkillEffectControllerType30 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);	
		
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		if(effect.cha.state.Contains(Monster.SHOOT_HEADER)) effect.cha.onCompleteAttackAni();
		
		effect.cha.state = Monster.NORMAL;		
		
		effect.cha.isFreeze.Set( true );
		
		if(pe == null)
		{
			pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[30].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha,null,effect.cha.tf, 10000, 0, 0, 0, 0.7f);//, 10000.0f);//, 0.0f, effect.cha.hitObject.height * 1.2f);
		}
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(30,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		
		SoundData.playHitSound(30,true);		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		delay += GameManager.globalDeltaTime;
		effect.cha.isFreeze.Set( true );
		
		if(delay > duration)
		{
			restore();
		}
	}
	
	sealed public override void restore ()
	{
		effect.cha.isFreeze.Set( false );

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);	
	}
		
}


// 회피
sealed public class CharacterSkillEffectControllerType31 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);	
		
		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[31].effUp].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha,null,effect.cha.tf, 10000.0f, 0.0f, 0.0f, 0.0f, 1.2f);
		
		effect.cha.attackAvoidPercent =  Mathf.CeilToInt(applyValue - 0.4f);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(31,effect.cha, EffectManager.SkillEffectType.upLoop, effectAttackerUniqueId);
		effect.addIconBuff(GameManager.info.skillEffectSetupData[31].upIcon);
		SoundData.playHitSound(31,true);		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		delay += GameManager.globalDeltaTime;
		
		if(delay > duration)
		{
			restore();
		}
	}
	
	sealed public override void restore ()
	{
		effect.cha.attackAvoidPercent = 0;

		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);	
	}	
}


// 전염 

/*
타겟이 지속효과에 적용된 경우에 한하여, 
* 확산속도 딜레이 후, 본인의 현재상태(스킬효과양, 남은지속시간)가 그대로 반경 Ncm 내에 1개 아군에게 전염
  - 전염대상이 없으면 나타날 때까지 계속 찾기
  - 전염시킨 녀석은 더 이상 전염시킬 수 없음
* 전염당한 녀석도 그대로 주변 동료에게 전염
* 전염효과의 지속시간은, 따로 지정되지 않으며 다른 지속효과의 남은지속시간을 그대로 적용
 */
 
sealed public class CharacterSkillEffectControllerType32 : CharacterSkillEffectController
{
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		if(active == false)
		{
			base.init(shooterType, attackerUniqueId, p_skillEffect,p_applyValue,p_duration,p_timeOffset);

			if(checkStartDelay == false) applyFirst();
		}

		if(checkStartDelay && loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(32,effect.cha, EffectManager.SkillEffectType.downLoop, attackerUniqueId);
	}
	
	
	List<Monster> _sortList = new List<Monster>();

	sealed public override void applyFirst()
	{
		if(pe == null)
		{
			pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[32].effDown].getParticleEffectByCharacterSize(effectAttackerUniqueId, effect.cha,null,effect.cha.tf, 10000.0f, 0.0f, 0.0f, 0.0f, 1.2f);
		}	

		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(32,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
	}

	sealed public override void update ()
	{
		if(hasStartDelay()) return;

		if(effect.hasAffectingEffect())
		{
			if(delay >= duration)
			{
				delay -= duration;
				
				// 적 몬스터일 경우. 가장 가까운 곳을 찾음...
				if(effect.cha.isMonster)
				{
					Monster cha = GameManager.me.characterManager.getCloseTeamTarget(effect.cha.isPlayerSide, effect.cha, null, false);

					// 회피하지 않고
					// 전염도 안되있는 녀석에게 전염을 시킬 수 있다.
					if(cha != null && cha.distanceFromHitPoint <= applyValue && cha.canAvoid() == false && cha.characterEffect.effectsDic[32].active == false)
					{
						cha.characterEffect.copyEffect(effect);
						SoundData.playHitSound(32,true);
					}
					
					cha = null;
				}
			}
			
			delay += GameManager.globalDeltaTime;
		}
		else
		{
			restore();
		}
	}
	
	
	sealed public override void restore ()
	{
		clear ();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);	
	}		

}


// 생명력 지속 증가.
sealed public class CharacterSkillEffectControllerType34 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);

		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		// 이펙트 효과 적용.
		applyEffect(true);
		if(loopPe == null) loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(34,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[34].upIcon);		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		else
		{
			if(_offsetChecker >= timeOffset)
			{
				_offsetChecker -= timeOffset;
				applyEffect();
			}
		}
		
		_offsetChecker += GameManager.globalDeltaTime;
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		float hpValue = effect.cha.maxHp * applyValue;
		effect.cha.hp += hpValue;
		effect.cha.hpEffect(hpValue);
	}
	
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}



// 데미지 반사
sealed public class CharacterSkillEffectControllerType35 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);

		if(checkStartDelay == false) applyFirst();
	}

	sealed public override void applyFirst()
	{
		// 이펙트 효과 적용.
		applyEffect(true);

		if(loopPe == null)
		{
			loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(35,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		}
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[35].downIcon);		
	}

	sealed public override void update()
	{
		if(hasStartDelay()) return;

		if(delay >= duration)
		{
			// 종료.
			restore();
		}

		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(isFirst)
		{
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[35].effDown) == false)
			{
				if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[35].effDown].getParticleEffect(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			}

			SoundData.play(GameManager.info.skillEffectSetupData[35].soundDown);
		}

//		float hpValue = effect.cha.maxHp * applyValue;
//		effect.cha.hp += hpValue;
//		effect.cha.hpEffect(hpValue);
	}
	
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}




// 모든 데미지 흡수
sealed public class CharacterSkillEffectControllerType36 : CharacterSkillEffectController
{
	
	sealed public override void init(Monster.TYPE shooterType, int attackerUniqueId, SkillEffectData p_skillEffect, float p_applyValue, float p_duration, float p_timeOffset = -1.0f)
	{
		base.init(shooterType, attackerUniqueId, p_skillEffect, p_applyValue, p_duration, p_timeOffset);
		
		if(checkStartDelay == false) applyFirst();
	}
	
	sealed public override void applyFirst()
	{
		// 이펙트 효과 적용.
		applyEffect(true);
		
		if(loopPe == null)
		{
			loopPe = EffectManager.showEfectBySkillEffectTypeWithCharacterSize(36,effect.cha, EffectManager.SkillEffectType.downLoop, effectAttackerUniqueId);
		}
		
		effect.addIconBuff(GameManager.info.skillEffectSetupData[36].downIcon);		
	}
	
	sealed public override void update()
	{
		if(hasStartDelay()) return;
		
		if(delay >= duration)
		{
			// 종료.
			restore();
		}
		
		delay += GameManager.globalDeltaTime;		
	}
	
	sealed public override void applyEffect(bool isFirst = false)
	{
		if(isFirst)
		{
			if(string.IsNullOrEmpty(GameManager.info.skillEffectSetupData[36].effDown) == false)
			{
				if(pe == null) pe = GameManager.info.effectData[GameManager.info.skillEffectSetupData[36].effDown].getParticleEffect(effectAttackerUniqueId, effect.cha, null, effect.cha.tf);
			}
			
			SoundData.play(GameManager.info.skillEffectSetupData[36].soundDown);
		}
		
		//		float hpValue = effect.cha.maxHp * applyValue;
		//		effect.cha.hp += hpValue;
		//		effect.cha.hpEffect(hpValue);
	}
	
	
	sealed public override void restore()
	{
		clear();
		if(skillEffect != null) effect.removeEffect(skillEffect.type);
	}	
}


