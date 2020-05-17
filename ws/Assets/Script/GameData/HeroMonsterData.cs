using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class HeroMonsterData : BaseHeroData
{
	public string id;
	
	public string partsSkill1;
	public string partsSkill2;
	public string partsSkill3;
	public float atkRange;
	public float atkSpeed;
	
	public AttackData attackType;
	
	public string bulletPatternId;

	public bool isMiddleBoss = false;

	public bool isPlayerResourceType = false;
	public string faceTexture = null;
	public string head = null;
	public string body = null;
	public string weapon = null;
	public string vehicle = null;

	public HeroMonsterData ()
	{
	}
	
	sealed public override void setData(List<object> list, Dictionary<string, int> k)
	{
		name = (string)list[k["NAME"]];
		slotName = ((string)list[k["SNAME"]]).Replace("\\n","\n");;
		id = (string)list[k["ID"]];
		
		base.setData(list, k);
		
		resource = (string)list[k["RESOURCE"]];

		if(resource.Contains(","))
		{
			string[] r = resource.Split(',');
			resource = r[0];
			faceTexture = r[1];
			head = r[2];
			body = r[3];
			weapon = r[4];
			vehicle = r[5];
			isPlayerResourceType = true;
		}
		else isPlayerResourceType = false;

		bulletPatternId = (string)list[k["BULLET_PATTERN"]];
		if(string.IsNullOrEmpty(bulletPatternId)) bulletPatternId = id;

//		partsSkill1 = (string)list[k["PARTS_SKILL1"]];
//		partsSkill2 = (string)list[k["PARTS_SKILL2"]];
//		partsSkill3 = (string)list[k["PARTS_SKILL3"]];
		
		Util.parseObject(list[k["ATK_RANGE"]],out atkRange,true, 0);
		Util.parseObject(list[k["ATK_SPEED"]],out atkSpeed,true, 0);	
		atkSpeed *= 0.001f;
		
		int atkType = 0;
		Util.parseObject(list[k["ATK_TYPE"]], out atkType, true, 1);
		attackType = AttackData.getAttackData(atkType, list[k["ATTR1"]],list[k["ATTR2"]],list[k["ATTR3"]],list[k["ATTR4"]],list[k["ATTR5"]],list[k["ATTR6"]],list[k["ATTR7"]]);
		
		Util.parseObject(list[k["EFF_TYPE"]], out atkEffectType, true, 1);

		isMiddleBoss = (list[k["MIDDLE_BOSS"]].ToString().Length > 0);

	}
	
	sealed public override void setDataToCharacter(Monster cha)
	{
		base.setDataToCharacter(cha);
		
		cha.stat.atkPhysic = atkPhysic;
		cha.stat.atkMagic = atkMagic;
		
		cha.stat.atkRange = atkRange;
		cha.stat.atkSpeed = atkSpeed;
		cha.bulletPatternId = bulletPatternId;
		
		cha.atkEffectType = atkEffectType;
		
		cha.stat.baseLevel = level;

		cha.stat.maxHp = MathUtil.RoundToInt(hpMax);

		//* 몬스터 히어로 : 데이타시트에 설정된 레벨				
		cha.stat.skillTargetLevel = level;
	}
			
	
}
