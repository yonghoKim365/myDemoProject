using System;
using System.Collections.Generic;
using System.Security.Cryptography;

sealed public class IconIndexData
{
	public IconIndexData ()
	{
	}

	public string id;
	public int collectionNumber = 0;

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];

		Util.parseObject(l[k["FILE"]], out collectionNumber, true, 0);
	}
}


sealed public class Icon
{
	private static IconIndexData _temp;
	public static void setSkillIcon(string spriteId, UISprite targetSprite, int defaultDepth = -1000)
	{
		if(GameManager.info.skillIconIndexData.TryGetValue(spriteId, out _temp))
		{
			targetSprite.atlas = ResourceManager.instance.skillIconAtlas[_temp.collectionNumber];
			targetSprite.spriteName = spriteId;

			if(defaultDepth > - 1000)
			{
				targetSprite.depth = defaultDepth + _temp.collectionNumber;
			}
		}
	}
	
	
	public static void setEquipIcon(string spriteId, UISprite targetSprite, int defaultDepth = -1000)
	{
		if(GameManager.info.equipIconIndexData.TryGetValue(spriteId, out _temp))
		{
			targetSprite.atlas = ResourceManager.instance.equipIconAtlas[_temp.collectionNumber];
			targetSprite.spriteName = spriteId;

			if(defaultDepth > - 1000)
			{
				targetSprite.depth = defaultDepth + _temp.collectionNumber;
			}
		}
	}
}

