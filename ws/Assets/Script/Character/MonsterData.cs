using UnityEngine;
using System;
using System.Collections.Generic;

sealed public class MonsterData
{
	public bool canChangeShader = true;

	public string[] deleteParts = null;

	public PartsUVAnimation[] partsUV = null;

	public bool hasFaceAni = false;

	public bool hasParticle = false;
	
	public string id;
	//public string type;
	public string name;
	
	public string damageSound;

	public MonsterCategory.Category category;
	
	//public string motionType;
	public IFloat speed = 0.0f;
	//public string bullet;
	//public string bulletType;
	public int hp = 0;
	//public bool invincible = false;
	public int rangeDistance = -1;
	public string resource;
	public string icon;


	public string defaultTexture = null;

	public IFloat damageRange = 0;
	
	public bool deleteBulletAfterDead = false;
	//public bool hasHitCondition = false;
	public string hitCondition = "";

	public bool hasShootCondition = false;
//	public BulletPatternData[] bulletPatterns;	
	//public bool useFlip = false;
	//public Reward hitReward;
	//public Reward reward;
	public int hitDamage = 1;
	public bool isBlockMonster = false;
	public string createMotionType = null;
	public float createMotionTime = 0.0f;	
	public void setCreateMotionType(string str)
	{
		if(str != null && str != "")
		{
			string[] codes = str.Split(':');
			if(codes.Length != 2) return;
			
			createMotionType = codes[0];
			createMotionTime = (float)System.Convert.ToDouble(codes[1]);
		}
	}	
	
	public string deleteMotionType = null;
	public string deleteMotionValue = null;
	public int deleteMotionIntValue = 0;
	public float deleteMotionTime = 0.0f;		
	
	public void setDeleteMotionType(string str)
	{
		if(str != null && str != "")
		{
			string[] codes = str.Split(':');
			
			switch(codes.Length)
			{
			case 2:
				deleteMotionType = codes[0];
				deleteMotionValue = codes[1];
				int.TryParse(codes[1],out deleteMotionIntValue);
				break;
			case 3:
				deleteMotionType = codes[0];
				deleteMotionValue = codes[1];
				int.TryParse(codes[1],out deleteMotionIntValue);
				deleteMotionTime = (float)System.Convert.ToDouble(codes[2]);
				break;
			}
			
			
			
		}
	}		
	
		
	public string explosionEffect = "";
		
	public bool visible = true;
	
	
	public IFloat scale = 1.0f;
	
	public AttachedEffectData[] effectData = null;
	
	public void setEffect(string str)
	{
		if(str != null && str != "")
		{
			string[] effects = str.Split('&');
			
			effectData = new AttachedEffectData[effects.Length];
			int index = 0;
			
			foreach(string eff in effects)
			{
				AttachedEffectData efd = new AttachedEffectData();
				
				string[] effData = eff.Split(':');
				
				switch(effData[0])
				{
				case "P":
					efd.type = AttachedEffect.TYPE_PREFAB;
					break;
				case "E":
					efd.type = AttachedEffect.TYPE_EFFECT;
					break;					
				case "S":
					efd.type = AttachedEffect.TYPE_TK2D_ANI_SPRITE;
					break;
				}

				string[] effValue = effData[1].Split('@');

				int len = effValue.Length;

				if(len > 1)
				{
					efd.parentName = effValue[1];
				}

				effValue = effValue[0].Split(',');

				len = effValue.Length;
				
				for(int i = 0; i < len; ++i)
				{
					switch(i)
					{
					case 0:
						efd.id = effValue[i];
						break;
					case 1:
						efd.attachToParent = (effValue[i].Equals("N") == false);
						break;
					case 2:
						efd.pos.x = (float)Convert.ToDouble(effValue[i]);
						break;
					case 3:
						efd.pos.y = (float)Convert.ToDouble(effValue[i]);
						break;
					case 4:
						efd.pos.z = (float)Convert.ToDouble(effValue[i]);
						break;						
					}
				}
				
				effectData[index] = efd;
				++index;
			}
		}
	}	
	
	
	public string summonSound = null;
	
	
	public MonsterData ()
	{
	}


	public static void setUnitIcon(MonsterData md, UISprite targetSprite, int defaultDepth, bool useIconResourceName = true)
	{

		IconIndexData uid;

		if(string.IsNullOrEmpty(md.icon) == false && useIconResourceName)
		{
			GameManager.info.unitIconIndexData.TryGetValue(md.icon, out uid);
		}
		else
		{
			GameManager.info.unitIconIndexData.TryGetValue(md.resource, out uid);
		}

		if(uid == null)
		{
			Debug.LogError("ICON ERROR : " + md.name);
			return;
		}


		targetSprite.atlas = ResourceManager.instance.unitIconAtlas[uid.collectionNumber];
		targetSprite.spriteName = uid.id;
		targetSprite.depth = defaultDepth + uid.collectionNumber;
	}

	public static void setUnitIcon(string monsterId, UISprite targetSprite, int defaultDepth, bool useIconResourceName = true)
	{
		if(GameManager.info.monsterData.ContainsKey(monsterId))
		{
			setUnitIcon(GameManager.info.monsterData[monsterId], targetSprite, defaultDepth, useIconResourceName);
		}
	}

}
