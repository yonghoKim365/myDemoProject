using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NGUISpriteChanger : MonoBehaviour 
{
	public GameObject root;
	public UIAtlas sourceAtlas;

	public string spriteName;

	public string checkString;

	public void checkSprite(UISprite[] sprites)
	{
		if(root == null) return;
		if(sprites == null) return;
		
		foreach(UISprite sp in sprites)//Object.FindObjectsOfType(typeof(UISprite)))
		{
			if(sp.spriteName == spriteName)
			{
				Debug.LogError("spriteName: " + sp.spriteName + "   atlas: " + sp.atlas.name);

				if(string.IsNullOrEmpty(checkString) == false)
				{
					sp.name = sp.name + checkString;
				}

			}
		}
	}


	public void changeAll(UISprite[] sprites)
	{
		Debug.LogError("change All");

		if(root == null) return;
		if(sourceAtlas == null) return;
		if(sprites == null) return;

		foreach(UISprite sp in sprites)//Object.FindObjectsOfType(typeof(UISprite)))
		{
			UISpriteData sd = sourceAtlas.GetSprite(sp.spriteName);
			if(sp.atlas != sourceAtlas && sd != null)
			{
				Debug.LogError(sd.name + "   " + sp.spriteName);
				sp.atlas = sourceAtlas;
				sp.spriteName = sd.name;

				string n = getAtlasName(sourceAtlas);

				if(n == null)
				{
					sp.gameObject.name += " CHECKTHIS";
				}
				else
				{
					sp.gameObject.name = sp.depth + " " + n + " " + sd.name + " CHECKTHIS";
				}
			}
		}
	}


	public static string getAtlasName(UIAtlas at)
	{
		if(at == null) return null;

		switch(at.name)
		{
		case "NGUI_ANIMAL":
			return "a";
		case "NGUI_CHAMPIONSHIP":
			return "cp";
		case "NGUI_CHAMPIONSHIP_TABLE":
			return "ct";
		case "NGUI_COMMON":
			return "c";
		case "NGUI_COMMON_POPUP_FRAME":
			return "cp";
		case "NGUI_COMMON_POPUP_FRAME2":
			return "cp2";
		case "NGUI_EQUIP_ICON":
			return "ei";
		case "NGUI_GACHA":
			return "g";
		case "NGUI_HERO_HUD":
			return "hh";
		case "NGUI_HUD_FONT":
			return "hf";
		case "NGUI_LOBBY_MESSAGEBOX_MISSION_OPTION_POSTBOX_SKILL_SOCIAL":
			return "lm";
		case "NGUI_SHOP":
			return "s";
		case "NGUI_SHOP2":
			return "s2";
		case "NGUI_SKILL_ICON":
			return "si";
		case "NGUI_START_CHARACTER":
			return "sc";
		case "NGUI_START_LOADING_WORLDMAP":
			return "sl";
		case "NGUI_TUTORIAL_SKILL_GUIDE":
			return "ts";
		case "NGUI_UNIT_ICON":
			return "ui";
		case "NGUI_UNIT_ICON2":
			return "ui2";
		case "NGUI_WORLDMAP_BG":
			return "wb";
		case "NGUI_WORLDMAP_LAND":
			return "wl";
		case "NGUI_WORLDMAP_LOCK":
			return "wk";
		case "NGUI_WORLDMAP_ROAD":
			return "wr";

		case "NGUI_LANGUAGE_A_M":
			return "am";
		case "NGUI_LANGUAGE_O_W":
			return "ow";

		}

		return null;
	}

}