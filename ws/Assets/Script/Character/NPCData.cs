using System;
using System.Collections.Generic;

sealed public class NPCData
{
	public NPCData ()
	{
	}
	
	public enum Type
	{
		PLAYER, MONSTER
	}
	
	public string id;
	
	public Type type;
	
	public string resource;

	public string faceTexture = null;

	public string head = "";
	public string body = "";
	public string weapon = "";
	public string vehicle = "";

	public string name = "";
	
	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		
		if(((string)l[k["TYPE"]]).Equals("P"))
		{
			type = Type.PLAYER;
		}
		else if(((string)l[k["TYPE"]]).Equals("M"))
		{
			type = Type.MONSTER;
		}

		name = (string)l[k["NAME"]];

		resource = (string)l[k["RESOURCE"]];

//		UnityEngine.Debug.LogError(l[k["FACE"]]);
		faceTexture = (string)l[k["FACE"]];

		if(string.IsNullOrEmpty(faceTexture))
		{
			faceTexture = null;
		}
		else if(faceTexture.Trim().Length < 3) faceTexture = null;

		head = (string)l[k["HEAD"]];
		body = (string)l[k["BODY"]];
		weapon = (string)l[k["WEAPON"]];
		vehicle = (string)l[k["VEHICLE"]];

		Util.parseObject(l[k["MOVE_SPEED"]], out speed, true, 0.0f);
		Util.parseObject(l[k["ATK_PHYSIC"]], out atkPhysic, true, 0.0f);
		Util.parseObject(l[k["ATK_MAGIC"]], out atkMagic, true, 0.0f);
		Util.parseObject(l[k["DEF_PHYSIC"]], out defPhysic, true, 0.0f);
		Util.parseObject(l[k["DEF_MAGIC"]], out defMagic, true, 0.0f);
		Util.parseObject(l[k["HP"]], out hp, true, 0.0f);		
		
	}
	
	
	public float speed;
	public float atkRange;
	public float atkSpeed;
	public float atkPhysic;
	public float atkMagic;
	public float defPhysic;
	public float defMagic;
	public float hp;
	
	
	public void setDataToCharacter(Monster cha)
	{
		cha.stat.speed = speed;
		cha.maxHp = hp;
		cha.hp = hp;
			
		cha.stat.atkPhysic  = atkPhysic;
		cha.stat.atkMagic  = atkMagic;
		cha.stat.defPhysic  = defPhysic;
		cha.stat.defMagic = defMagic;	
	}	
	
	
}

