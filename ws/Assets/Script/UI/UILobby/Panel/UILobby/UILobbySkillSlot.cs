using UnityEngine;
using System.Collections;

public class UILobbySkillSlot : MonoBehaviour {

	public UIButton btn;
	public UISprite spIcon, spSkillRare;

	void Awake()
	{
		UIEventListener.Get(btn.gameObject).onClick = onClickSkillSlot;
	}

	GameIDData data = null;

	GamePlayerData gamePlayerData = null;

	public void setData(GameIDData infoData, GamePlayerData gpd = null)
	{
		if(infoData == null)
		{
			gamePlayerData = null;
			data = null;
			spIcon.gameObject.SetActive(false);
			spSkillRare.gameObject.SetActive(false);
		}
		else
		{
			gamePlayerData = gpd;
			data = infoData;
			spIcon.gameObject.SetActive(true);

			Icon.setSkillIcon(data.getSkillIcon(), spIcon);

			spSkillRare.gameObject.SetActive(true);
			spSkillRare.spriteName = RareType.getRareLineSprite(infoData.rare);
		}
	}

	void onClickSkillSlot(GameObject go)
	{
		if(data == null) return;
		GameManager.me.uiManager.popupSkillPreview.show(data, RuneInfoPopup.Type.PreviewOnly, false, true, gamePlayerData);
	}
}
