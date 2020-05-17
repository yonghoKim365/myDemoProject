using System;
using System.Collections.Generic;
using System.Security.Cryptography;

sealed public class UnitData
{
	public UnitData ()
	{
	}



	public AttackData attackType;

	private string _resource;
	public string resource
	{
		get
		{
			if(isRare)
			{
				return GameManager.info.unitData[baseUnitId].resource;
			}
			else
			{
				return _resource;
			}
		}
#if UNITY_EDITOR
		set
		{
			if(isRare)
			{
				GameManager.info.unitData[baseUnitId].resource = value;
			}
			else
			{
				_resource = value;
			}
		}
#endif
	}
	
	public string id;

	private string _name;

	public string name
	{
		set
		{
			_name = value;
		}
		get
		{
			if(isRare) return GameManager.info.unitData[baseUnitId].name;
			else return _name;
		}
	}

	private string _slotName = "";

	public string slotName
	{
		set
		{
			_slotName = value;
		}
		get
		{
			if(isRare) return GameManager.info.unitData[baseUnitId].slotName;
			else return _slotName;
		}
	}


	public Xint reinforceLevel = 0;
	public int level = 0;
	public int baseLevel = 0;
	public Xfloat cooltime = 0;
	public int maxSummonAtOnce = 0;
	public Xfloat sp = 0;

	public float atk
	{
		get
		{
			return atkMagic + atkPhysic;
		}
	}

	public float def
	{
		get
		{
			return defMagic + defPhysic;
		}
	}


	public Xfloat speed;
	public Xfloat atkRange;
	public Xfloat atkSpeed;
	public Xfloat atkPhysic;
	public Xfloat atkMagic;
	public Xfloat defPhysic;
	public Xfloat defMagic;
	public Xfloat hp;
	
	public string unitType; // 공중 지상

	public string[] skill;

	public int rareSkillNum = 0;
	public string description;

	public bool isBook = false; // 도감용.
	public bool isRare = false;
	public bool isCombineData = false;
	
	public string baseUnitId;
	public string baseIdWithoutRare;
	public int rare = 0;

	public int effectType = 1;


	// 도감용.
	public float rareSp;
	public float rareAtk;
	public float rareDef;
	public float rareHp;

	public float superRareSp;
	public float superRareAtk;
	public float superRareDef;
	public float superRareHp;

	public float legendSp;
	public float legendAtk;
	public float legendDef;
	public float legendHp;

	public Xint grade = 0;



	//============
	const string ID = "ID";
	const string NAME = "NAME";
	const string SNAME = "SNAME";
	const string BASE = "BASE";
	const string LEVEL = "LEVEL";
	const string EFF_TYPE = "EFF_TYPE";
	const string GRADE = "GRADE";
	const string R_SP = "R_SP";
	const string R_ATK = "R_ATK";

	const string R_DEF = "R_DEF";
	const string R_HP = "R_HP";
	const string S_SP = "S_SP";
	const string S_ATK = "S_ATK";
	const string S_DEF = "S_DEF";
	const string S_HP = "S_HP";
	const string L_SP = "L_SP";
	const string L_ATK = "L_ATK";
	const string L_DEF = "L_DEF";
	const string L_HP = "L_HP";

	const string BOOK = "BOOK";

	const string RESOURCE = "RESOURCE";
	public const string COOLTIME = "COOLTIME";
	public const string MAX_SUMMON_AT_ONCE = "MAX_SUMMON_AT_ONCE";
	public const string SP = "SP";
	public const string MOVE_SPEED = "MOVE_SPEED";
	public const string ATK_RANGE = "ATK_RANGE";
	public const string ATK_SPEED = "ATK_SPEED";
	public const string ATK_PHYSIC = "ATK_PHYSIC";
	public const string ATK_MAGIC = "ATK_MAGIC";
	public const string DEF_PHYSIC = "DEF_PHYSIC";
	public const string DEF_MAGIC = "DEF_MAGIC";
	public const string HP = "HP";

	public const string ATK_TYPE = "ATK_TYPE";
	public const string ATK_ATTR1 = "ATK_ATTR1";
	public const string ATK_ATTR2 = "ATK_ATTR2";
	public const string ATK_ATTR3 = "ATK_ATTR3";
	public const string ATK_ATTR4 = "ATK_ATTR4";
	public const string ATK_ATTR5 = "ATK_ATTR5";
	public const string ATK_ATTR6 = "ATK_ATTR6";
	public const string ATK_ATTR7 = "ATK_ATTR7";

	const string UNIT_TYPE = "UNIT_TYPE";
	const string SKILL1 = "SKILL1";
	const string SKILL2 = "SKILL2";
	const string SKILL3 = "SKILL3";

	const string DESCRIPTION = "DESCRIPTION";



	public void setData(List<object> l, Dictionary<string, int> k, bool isRare = false, bool isCombineData = false, bool isPlayer = true)
	{
		this.isRare = isRare;
		
		id = (string)l[k[ID]];

		if(k.ContainsKey(NAME)) name = (string)l[k[NAME]];

		if(k.ContainsKey(SNAME)) slotName = ((string)l[k[SNAME]]).Replace("\\n","\n");

		if(isRare)
		{
			string tempId = String.Empty;
			
			if(l[k[ID]] is int) tempId = ((int)l[k[ID]]).ToString();
			else if(l[k[ID]] is long) tempId = ((long)l[k[ID]]).ToString();
			else if(l[k[ID]] is string) tempId = (string)l[k[ID]];
			
			baseUnitId = (string)l[k[BASE]];
		}
		else
		{
			Util.parseObject(l[k[LEVEL]], out level, true, 1);
			baseLevel = level;

			Util.parseObject(l[k[EFF_TYPE]], out effectType, true, 1);

			baseUnitId = id;

			if(isPlayer)
			{
				int g = 1;
				Util.parseObject(l[k[GRADE]], out g, true, 1);
				grade = g;
				rare = grade - 1;
			}

			if(isPlayer)
			{
				Util.parseObject(l[k[R_SP]], out rareSp, true, 0);
				Util.parseObject(l[k[R_ATK]], out rareAtk, true, 0);
				Util.parseObject(l[k[R_DEF]], out rareDef, true, 0);
				Util.parseObject(l[k[R_HP]], out rareHp, true, 0);
				
				Util.parseObject(l[k[S_SP]], out superRareSp, true, 0);
				Util.parseObject(l[k[S_ATK]], out superRareAtk, true, 0);
				Util.parseObject(l[k[S_DEF]], out superRareDef, true, 0);
				Util.parseObject(l[k[S_HP]], out superRareHp, true, 0);
				
				Util.parseObject(l[k[L_SP]], out legendSp, true, 0);
				Util.parseObject(l[k[L_ATK]], out legendAtk, true, 0);
				Util.parseObject(l[k[L_DEF]], out legendDef, true, 0);
				Util.parseObject(l[k[L_HP]], out legendHp, true, 0);

				isBook = l[k[BOOK]].ToString() == "Y";
			}

			_resource = (string)l[k[RESOURCE]];
		}
		

		Util.parseObject(l[k[COOLTIME]], out cooltime, true, 0.0f);
		Util.parseObject(l[k[MAX_SUMMON_AT_ONCE]], out maxSummonAtOnce, true, 0);
		Util.parseObject(l[k[SP]], out sp, 0);
		
		Util.parseObject(l[k[MOVE_SPEED]], out speed, true, 0.0f);
		Util.parseObject(l[k[ATK_RANGE]], out atkRange, true, 0.0f);
		Util.parseObject(l[k[ATK_SPEED]], out atkSpeed, true, 0.0f);
		atkSpeed *= 0.001f;
		Util.parseObject(l[k[ATK_PHYSIC]], out atkPhysic, true, 0.0f);
		Util.parseObject(l[k[ATK_MAGIC]], out atkMagic, true, 0.0f);
		Util.parseObject(l[k[DEF_PHYSIC]], out defPhysic, true, 0.0f);
		Util.parseObject(l[k[DEF_MAGIC]], out defMagic, true, 0.0f);
		Util.parseObject(l[k[HP]], out hp, true, 0.0f);
		
		int atkType = 0;
		Util.parseObject(l[k[ATK_TYPE]], out atkType,true,1);
		attackType = AttackData.getAttackData(atkType, l[k[ATK_ATTR1]],l[k[ATK_ATTR2]],l[k[ATK_ATTR3]],l[k[ATK_ATTR4]],l[k[ATK_ATTR5]],l[k[ATK_ATTR6]],l[k[ATK_ATTR7]]);
		
		unitType = (string)l[k[UNIT_TYPE]];
		
		skill = objectArrToStringArray(l[k[SKILL1]],l[k[SKILL2]],l[k[SKILL3]]);
		
		description = ((string)l[k[DESCRIPTION]]).Replace("\\n","\n");
		

		
		this.isCombineData = isCombineData;
	}
	
	
	int[] objectArrToArray(params object[] obj)
	{
		int len = obj.Length;
		int arrCount = 0;
		int temp = -1;
		for(int i = 0; i < len; ++i)
		{
			Util.parseObject(obj[i],out temp,true,-1);
			if(temp > -1)
			{
				++arrCount;
			}
		}
		
		int[] returnArr = null;
		
		if(arrCount > 0)
		{
			returnArr = new int[arrCount];
			
			for(int i = 0; i < arrCount; ++i)
			{
				Util.parseObject(obj[i],out returnArr[i],true,-1);
			}
		}
		
		return returnArr;
	}
	
	static List<string> _temp = new List<string>();
	string[] objectArrToStringArray(params object[] obj)
	{
		_temp.Clear();

		int len = obj.Length;
		int arrCount = 0;
		string temp = null;
		for(int i = 0; i < len; ++i)
		{
			if(obj[i] is string)
			{
				temp = ((string)obj[i]).Trim();
				if(temp.Length > 0)
				{
					_temp.Add(temp);			
				}
			}
		}

		if(_temp.Count <= 0) return null;

		string[] returnArr = _temp.ToArray();
		_temp.Clear();

		return returnArr;
	}	
	
	
	
	public UnitData getRareUnitData(UnitData bd, UnitData rd)
	{
		UnitData ud = new UnitData();
		ud.isCombineData = true;
		ud.isRare = true;

		ud.id = rd.id;
		//ud.name = rd.name;

		ud.cooltime = bd.cooltime + rd.cooltime;
		ud.maxSummonAtOnce = bd.maxSummonAtOnce + rd.maxSummonAtOnce;
		ud.sp = bd.sp + rd.sp;
		
		ud.speed = bd.speed + rd.speed;
		ud.atkRange = bd.atkRange + rd.atkRange;
		ud.atkSpeed = bd.atkSpeed + rd.atkSpeed;
		ud.atkPhysic = bd.atkPhysic + rd.atkPhysic;
		ud.atkMagic = bd.atkMagic + rd.atkMagic;
		ud.defPhysic = bd.defPhysic + rd.defPhysic;
		ud.defMagic = bd.defMagic + rd.defMagic;
		ud.hp = bd.hp + rd.hp;
		ud.unitType = bd.unitType; // 공중 지상
		
		ud.baseLevel = bd.baseLevel;

		ud.grade = bd.grade;

		int bs = 0;
		int rs = 0;

		if(bd.skill != null) bs = bd.skill.Length;
		if(rd.skill != null) rs = rd.skill.Length;

		ud.skill =  new string[bs + rs];

		if(bs > 0) bd.skill.CopyTo(ud.skill,0);
		if(rs > 0) rd.skill.CopyTo(ud.skill,bs);


		ud.description = rd.description;
		
		ud.effectType = rd.effectType;
		
		ud.baseUnitId = rd.baseUnitId;

		ud.attackType = new AttackData();

		ud.attackType.isShortType = bd.attackType.isShortType;
		ud.attackType.type = bd.attackType.type;
		ud.attackType.attr = Util.intArrayMerger(bd.attackType.attr, rd.attackType.attr);
		
		ud.reinforceLevel = GameIDData.getInforceLevelFromUnitId(id);

		ud.rare = ud.grade-1;//GameIDData.getRareLevelByReinforceLevel(reinforceLevel);

		ud.level = bd.baseLevel;

		return ud;
	}
	
	
	public UnitData getRareUnitData(UnitData bd)
	{
		isCombineData = true;
		isRare = true;

		//id = rd.id;
		//name = rd.name;
		//ud.level = rd.level;
		cooltime += bd.cooltime;
		maxSummonAtOnce += bd.maxSummonAtOnce;
		sp += bd.sp;
		
		attackType.type = bd.attackType.type;
		attackType.attr = Util.intArrayMerger(bd.attackType.attr, attackType.attr);
		
		speed += bd.speed;// + rd.speed;
		atkRange += bd.atkRange;// + rd.atkRange;
		atkSpeed += bd.atkSpeed;// + rd.atkSpeed;
		atkPhysic += bd.atkPhysic;// + rd.atkPhysic;
		atkMagic += bd.atkMagic;// + rd.atkMagic;
		defPhysic += bd.defPhysic;// + rd.defPhysic;
		defMagic += bd.defMagic;// + rd.defMagic;
		hp += bd.hp;
		unitType = bd.unitType; // 공중 지상
		
		//resource = bd.resource;
		
		skill = Util.concatStringArray(bd.skill, skill);
		
		effectType = bd.effectType;
		
		//rareSkill = rd.rareSkill;
		//description = rd.description;
		
		//baseUnitId = rd.baseUnitId;
		//rareLevel = rd.rareLevel;
		//code = rd.code;
		
		return this;
		
	}	
	
	
	public void setDataToCharacter(Monster cha, TranscendData td, int[] transcendLevel)
	{
		cha.stat.speed.Set ( speed );
		cha.maxHp =  ( hp);
		cha.maxSp.Set ( sp);
			
		cha.stat.atkRange.Set ( atkRange);
		cha.stat.atkSpeed.Set ( atkSpeed);
		cha.stat.atkPhysic.Set ( atkPhysic);
		cha.stat.atkMagic.Set ( atkMagic);
		cha.stat.defPhysic.Set ( defPhysic);
		cha.stat.defMagic.Set ( defMagic);	
		cha.bulletPatternId =   resource;
		
		cha.atkEffectType = effectType;
		
		cha.stat.reinforceLevel.Set ( reinforceLevel);

		cha.stat.baseLevel.Set ( baseLevel);

		//* 유저 아군 (or 적) 유닛 & 몬스터 유닛 : 해당 유닛의, 베이스레벨 + 강화레벨						
		cha.stat.skillTargetLevel.Set ( baseLevel + reinforceLevel );

//		ATK_ATTR1,ATK_ATTR2,ATK_ATTR3,ATK_ATTR4,ATK_ATTR5,ATK_ATTR6,ATK_ATTR7,

		if(td != null)
		{
			cha.stat.originalMoveSpeedRate = cha.stat.speed;
			cha.stat.originalAtkSpeedRate = cha.stat.atkSpeed;

			cha.stat.speed.Set ( td.getValueByATTR( transcendLevel, cha.stat.speed.Get(), WSATTR.MOVE_SPEED_I) );
			cha.stat.atkRange.Set ( td.getValueByATTR( transcendLevel, cha.stat.atkRange.Get(), WSATTR.ATK_RANGE_I) );
			cha.stat.atkSpeed.Set ( td.getValueByATTR( transcendLevel, cha.stat.atkSpeed.Get(), WSATTR.ATK_SPEED_I) );
			cha.stat.atkPhysic.Set ( td.getValueByATTR( transcendLevel, cha.stat.atkPhysic.Get(), WSATTR.ATK_PHYSIC_I) );
			cha.stat.atkMagic.Set ( td.getValueByATTR( transcendLevel, cha.stat.atkMagic.Get(), WSATTR.ATK_MAGIC_I) );
			cha.stat.defPhysic.Set ( td.getValueByATTR( transcendLevel, cha.stat.defPhysic.Get(), WSATTR.DEF_PHYSIC_I) );
			cha.stat.defMagic.Set ( td.getValueByATTR( transcendLevel, cha.stat.defMagic.Get(), WSATTR.DEF_MAGIC_I) );

			cha.stat.maxHp.Set ( td.getValueByATTR( transcendLevel, cha.stat.maxHp.Get(), WSATTR.HP_I) );

			cha.stat.originalMoveSpeedRate = cha.stat.speed / cha.stat.originalMoveSpeedRate;
			cha.stat.originalAtkSpeedRate = cha.stat.atkSpeed / cha.stat.originalAtkSpeedRate;
		}
		else
		{
			cha.stat.originalMoveSpeedRate = 1;
			cha.stat.originalAtkSpeedRate = 1;
		}

		cha.hp = ( cha.maxHp);
		cha.sp = ( cha.sp );
	}



}

