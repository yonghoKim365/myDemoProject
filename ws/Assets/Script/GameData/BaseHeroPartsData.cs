using System;
using System.Collections.Generic;

public class BaseHeroPartsData
{
	public BaseHeroPartsData ()
	{
	}
	
	public string id;
	public string name;
	public string character;
	public string resource;
	public string type;
	public string icon;
	public string texture;

	public string baseIdWithoutRare;

	public float[] previewSettingValue = null;

	public bool isBook = false;



	public void setData(List<object> list, Dictionary<string, int> k)
	{
		id = (string)list[k["ID"]];
		name = (string)list[k["NAME"]];
		character = (string)list[k["HERO"]];
		resource = (string)list[k["RESOURCE"]];
		type = (string)list[k["TYPE"]];
		icon = (string)list[k["ICON"]];
		texture = (string)list[k["TEXTURE"]];

		if(k.ContainsKey("PREVIEW"))
		{
			string p = list[k["PREVIEW"]].ToString();
			
			if(string.IsNullOrEmpty(p) == false)
			{
				previewSettingValue = Util.stringToFloatArray(p,',');
				if(previewSettingValue == null || previewSettingValue.Length != 7)
				{
					previewSettingValue = null;
				}
			}
		}

		isBook = list[k["BOOK"]].ToString() == "Y";

		baseIdWithoutRare = id.Substring( 0, id.IndexOf("_") + 3 ) + id.Substring( id.LastIndexOf("_") + 2)  ;

	}

}
