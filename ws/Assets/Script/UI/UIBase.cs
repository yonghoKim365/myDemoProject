using UnityEngine;
using System.Collections;

public class UIBase : MonoBehaviour {

	public virtual void show()
	{
		gameObject.SetActive(true);
	}
	
	public virtual void hide()
	{
		gameObject.SetActive(false);
	}

	public UIButton btnBack;

	protected virtual void setBackButton(int nextPanel)
	{
		nextPanelIndex = nextPanel;
		UIEventListener.Get(btnBack.gameObject).onClick = onClickBackToMainMenu;
		UIEventListener.Get(btnBack.gameObject).onPress = onPressBackToMainMenu;
	}
	

	int nextPanelIndex;
	public virtual void onClickBackToMainMenu(GameObject go)
	{
		UIMenu.instance.changePanel(nextPanelIndex);
	}


	public virtual void onPressBackToMainMenu(GameObject go, bool state)
	{
		if(state)
		{
			SoundData.play("uibt_back");
		}
	}

}
