sealed public class PlayerUnitSlot
{
	public Xbool isOpen = new Xbool(false);
	public UnitData unitData;
	public GameIDData unitInfo;

	public void setData(string id)
	{
//		UnityEngine.Debug.Log(id);

		if(string.IsNullOrEmpty(id))
		{
			isOpen.Set(false);
		}
		else
		{
			unitInfo = new GameIDData();
			unitInfo.parse(id, GameIDData.Type.Unit);

			if(GameManager.info.unitData.ContainsKey(unitInfo.resourceId))
			{
				unitData = GameManager.info.unitData[unitInfo.resourceId];
				isOpen.Set(true);
			}
			else
			{
				isOpen.Set(false);
			}

		}
	}
}


sealed public class PlayerSkillSlot
{
	public Xbool isOpen = new Xbool(false);
	public string id;
	public GameIDData infoData;
	
	public void setData(string skillId)
	{
		if(string.IsNullOrEmpty(skillId)) isOpen.Set(false);
		else
		{
			infoData = new GameIDData();
			infoData.parse(skillId, GameIDData.Type.Skill);
			id = infoData.id;
			isOpen.Set(true);
		}
	}
}
