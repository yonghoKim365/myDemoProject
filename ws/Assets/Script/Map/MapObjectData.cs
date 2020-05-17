using System;

public class MapObjectData
{
	public string id;
	public string type;
	

	public string name;

	public int hp;
	
	public string attr = "";
	public int attrValue;
	
	public Reward reward;
	
	public Reward afterLimitAction;
	
	public float scale = 1.0f;
	
	public float timeLimit = -1.0f;
	
	public string createMotionType = null;
	public float createMotionTime = 0.0f;
	
	public string deleteMotionType = null;
	public float deleteMotionTime = 0.0f;
	
	public float rotateX = 0.0f;
	
	
	public MapObjectData ()
	{
	}
	
	
	public string playMotionType = null;
	public float playMotionValue = 0.0f;

	public void setPlayMotionType(string str)
	{
		if(str != null && str != "")
		{
			string[] codes = str.Split(':');
			if(codes.Length != 2) return;
			
			playMotionType = codes[0];
			playMotionValue = (float)System.Convert.ToDouble(codes[1]);
		}
	}
	
	
	
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
	
	public void setDeleteMotionType(string str)
	{
		if(str != null && str != "")
		{
			string[] codes = str.Split(':');
			if(codes.Length != 2) return;
			
			deleteMotionType = codes[0];
			deleteMotionTime = (float)System.Convert.ToDouble(codes[1]);
		}		
	}
	
	
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
	
}

