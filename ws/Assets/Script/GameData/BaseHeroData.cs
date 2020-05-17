using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeroData
{
	//public string id = "";
	public string name = "";
	public string slotName = "";


	public int level;

	public float hpMax;
	public float hpRecovery;
	public float spMax;
	public float spRecovery;
	public float mpMax;
	public float mpRecovery;
	public float atkPhysic;
	public float atkMagic;
	public float defPhysic;
	public float defMagic;
	public float speed;
	public float summonSpPercent;
	public float unitHpUp;
	public float unitDefUp;
	public float skillSpDiscount;

	public float skillAtkUp;
	public float skillUp;
	public float skillTimeUp;
	
	public string resource;
	
	public int atkEffectType = 1;
	
	
	public BaseHeroData ()
	{
	}
	
	public virtual void setData(List<object> list, Dictionary<string, int> k)
	{
		
		resource = (string)list[k["RESOURCE"]];
		
		Util.parseObject(list[k["LEVEL"]],out level ,true, 1);

		Util.parseObject(list[k["HPMAX"]],out hpMax,true,0);
		Util.parseObject(list[k["HP_RECOVERY"]],out hpRecovery,true,0);
		Util.parseObject(list[k["SPMAX"]],out spMax,true,0);
		Util.parseObject(list[k["SP_RECOVERY"]],out spRecovery,true,0);
		Util.parseObject(list[k["MPMAX"]],out mpMax,true,0);
		Util.parseObject(list[k["MP_RECOVERY"]],out mpRecovery,true,0);
		Util.parseObject(list[k["ATK_PHYSIC"]],out atkPhysic,true,0);
		Util.parseObject(list[k["ATK_MAGIC"]],out atkMagic,true,0);
		Util.parseObject(list[k["DEF_PHYSIC"]],out defPhysic,true,0);
		Util.parseObject(list[k["DEF_MAGIC"]],out defMagic,true,0);
		Util.parseObject(list[k["SPEED"]],out speed,true,0);
		Util.parseObject(list[k["SUMMON_SP_PER"]],out summonSpPercent,true,0);
		Util.parseObject(list[k["UNIT_HP_UP"]],out unitHpUp,true,0);
		Util.parseObject(list[k["UNIT_DEF_UP"]],out unitDefUp,true,0);
		Util.parseObject(list[k["SKILL_SP_DISCOUNT"]],out skillSpDiscount,true,0);



		Util.parseObject(list[k["1-2_SKILL_ATK_UP"]],out skillAtkUp,true,0);
		Util.parseObject(list[k["3_SKILL_UP"]],out skillUp,true,0);
		Util.parseObject(list[k["4-9_SKILL_TIME_UP"]],out skillTimeUp,true,0);
	}	

	
	public virtual void setDataToCharacter(Monster cha)
	{
		cha.maxHp = MathUtil.RoundToInt(hpMax);
		cha.hp = MathUtil.RoundToInt(hpMax);
		cha.maxSp = spMax;
		cha.sp = spMax;
		cha.stat.spRecovery = spRecovery;
		cha.maxMp = mpMax;
		cha.stat.mpRecovery = mpRecovery;
		cha.mp = mpMax;
		
		cha.stat.atkPhysic = 0.0f;
		cha.stat.atkMagic = 0.0f;
		
		cha.stat.defPhysic = defPhysic;
		cha.stat.defMagic = defMagic;
		cha.stat.speed = speed;

		cha.stat.summonSpPercent = summonSpPercent * 0.01f;
		cha.stat.unitHpUp = unitHpUp * 0.01f;
		cha.stat.unitDefUp = unitDefUp * 0.01f;
		cha.stat.skillMpDiscount = skillSpDiscount * 0.01f;

		cha.stat.skillAtkUp = skillAtkUp* 0.01f;
		cha.stat.skillUp = skillUp* 0.01f;
		cha.stat.skillTimeUp = skillTimeUp* 0.01f;
		
		cha.stat.baseLevel = level;
	}
	
	
}

