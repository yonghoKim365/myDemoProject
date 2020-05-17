public class HeroPartsItem
{
	public string partsId = string.Empty;
	public string characterId = string.Empty;

	public GameIDData itemInfo;

	public HeroPartsData parts;

	public int rare
	{
		get
		{
			return itemInfo.rare;
		}
	}

	public int reinforceLevel
	{
		get
		{
			return itemInfo.reinforceLevel;
		}
	}


	public HeroPartsItem(string character, string id)
	{
		if(string.IsNullOrEmpty(id)) return;
		characterId = character;
		partsId = id;

		itemInfo = new GameIDData();
		itemInfo.parse(id, GameIDData.Type.Equip);
		parts = itemInfo.partsData;
	}
}
