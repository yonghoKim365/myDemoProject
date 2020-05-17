using UnityEngine;
using System.Collections;

public class UIPoupHeroSelectTab : MonoBehaviour {

	public UISprite spCharacter;
	public UISprite spCountBg;
	public UISprite spCharacterName;
	public UILabel lbCount;

	public Character.ID character;
	public bool isBook = false;

	void Awake()
	{
		//changeTab();
	}


	public void changeTab()
	{
		switch(character)
		{
		case Character.ID.LEO:
			spCharacter.spriteName = "img_tab_charac_01";
			spCharacterName.spriteName = "img_text_charac_01";
			break;
		case Character.ID.KILEY:
			spCharacter.spriteName = "img_tab_charac_02";
			spCharacterName.spriteName = "img_text_charac_02";
			break;
		case Character.ID.CHLOE:
			spCharacter.spriteName = "img_tab_charac_03";
			spCharacterName.spriteName = "img_text_charac_03";
			break;
		case Character.ID.LUKE:
			spCharacter.spriteName = "img_tab_charac_04";
			spCharacterName.spriteName = "img_text_charac_04";
			break;
		}
		
		if(isBook)
		{
			lbCount.enabled = true;
			spCountBg.enabled = true;
			spCharacterName.cachedTransform.localPosition = new Vector3(165,26,0);
		}
		else
		{
			lbCount.enabled = false;
			spCountBg.enabled = false;
			spCharacterName.cachedTransform.localPosition = new Vector3(165,13,0);
		}
	}

}
