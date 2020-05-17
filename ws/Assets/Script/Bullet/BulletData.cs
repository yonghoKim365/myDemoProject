using UnityEngine;
using System;


[System.Serializable]
public class BulletData
{
	
	public string id;
	
	//public string name;
	
	//public string resourceId;

	public bool ignoreHitObjectRotate = false;

	public float rotateSpeed = 0;
	
	public string hitEffect;

	public enum HitEffectOptionType
	{
		Normal, IgnoreSize, IgnoreParent, IgnoreAll
	}
	
	public HitEffectOptionType hitEffectOption = HitEffectOptionType.Normal;

	public int hp = 0;
	
	public int invincibleValue = 0;
	
	public bool destroy = false;

	public bool attachEffectToShotPoint = false;

	public bool useDestroyEffect = false;
	public string destroyEffectId = null;

	public enum DestroyEffectOptionType
	{
		Normal, UseBulletRotation
	}

	public DestroyEffectOptionType destroyEffectOption = DestroyEffectOptionType.Normal;

	public BulletPatternData bulletPattern = null;
	
	public void setBulletPattern(string patternId)
	{
		if(patternId == null || patternId.Equals("")) return;
		GameManager.info.bulletPatternData.TryGetValue(patternId, out bulletPattern);
	}
	
	
	public int motionType = 0;
	
	// 블랙홀 등에 영향받는지 여부. 기본적으로는 true다.
	//public bool canAttract = true;
	
	public int pivot = -10;

	public float scale = 1.0f;

	public string destroySound = null;
	public string startSound = null;


	public string preloadEffect = null;


	public BulletData ()
	{
	}
	
	
	// 총알을 단순히 쳐맞았을때..
	public Reward hitActionReward;
	
	// 총알을 맞고 죽었을때..
	public Reward deadActionReward;
	
	public void hitDeadActionStart(Vector3 position)
	{
		if(deadActionReward.hasReward) deadActionReward.start(position);
	}
	
	
	public void hitActionStart(Vector3 position, Monster cha, Bullet bullet)
	{
		if(hitActionReward.hasReward) hitActionReward.startBulletHitAction(position,bullet,cha);
	}
	
	public Reward destoryAction;
	
	// 시간이 지나고 터진 후 시작되는 액션...
	public void hitSecondActionStart(Vector3 position, Monster cha = null, Bullet bullet = null)
	{
		if(destoryAction.hasReward) destoryAction.start(position, false, true, bullet, cha);		
	}	
	
	

	
	public bool visible = true;
	
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
				case "IE":
					efd.type = AttachedEffect.TYPE_INDY_EFFECT;
					break;							
				case "S":
					efd.type = AttachedEffect.TYPE_TK2D_ANI_SPRITE;
					break;
				}
				
				string[] effValue = effData[1].Split(',');
				
				int len = effValue.Length;
				
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
						efd.option = (effValue[i].Equals("N") == false);
						break;						
					case 3:
						efd.pos.x = (float)Convert.ToDouble(effValue[i]);
						break;
					case 4:
						efd.pos.y = (float)Convert.ToDouble(effValue[i]);
						break;
					case 5:
						efd.pos.z = (float)Convert.ToDouble(effValue[i]);
						break;
					case 6:
						efd.timeLimit = (float)Convert.ToDouble(effValue[i]);
						break;
					}
				}
				
				effectData[index] = efd;
				++index;
			}
		}
	}
}




