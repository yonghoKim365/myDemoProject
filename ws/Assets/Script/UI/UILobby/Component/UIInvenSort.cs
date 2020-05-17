using UnityEngine;
using System.Collections;

public class UIInvenSort : MonoBehaviour 
{
	public UIToggle cbSortType;
	public UIToggle[] buttons;
	public UIListBase list;

	public UIButton btnClose;


	public enum InvenType
	{
		unit, skill, equip
	}

	public InvenType invenType = InvenType.unit;

	void Awake()
	{
		UIEventListener.Get(cbSortType.gameObject).onClick = onClickSortDirection;
		UIEventListener.Get(btnClose.gameObject).onClick = onClickClose;

//		cbSortType.value = false;
//		list.sortFromHigh = cbSortType.value;
//		LocalDataManager.setInvenSortDirection(invenType, cbSortType.value);

		for(int i = 0; i < buttons.Length; ++i)
		{
			UIEventListener.Get(buttons[i].gameObject).onClick = onClickButton;
		}
	}

	string sortType;

	public void open()
	{

		sortType = LocalDataManager.getInvenSortType(invenType);
		cbSortType.value = !LocalDataManager.isInvenSortDirectionHigh(invenType);

		for(int i = 0; i < buttons.Length; ++i)
		{
			buttons[i].value = (buttons[i].name == sortType);
			buttons[i].collider.enabled = (buttons[i].name != sortType);
		}

		gameObject.SetActive(true);
	}


	void onClickSortDirection(GameObject go)
	{
		list.sortFromHigh = !cbSortType.value;
		LocalDataManager.setInvenSortDirection(invenType, !cbSortType.value);
		refreshList();
	}

	void onClickButton(GameObject go)
	{

		LocalDataManager.setInvenSortType(invenType, go.name);
		list.sortType = go.name;
		refreshList();
	}

	void refreshList()
	{
		SoundData.play("uirn_arrange");

		for(int i = 0; i < buttons.Length; ++i)
		{
			buttons[i].value = (buttons[i].name == list.sortType);
			buttons[i].collider.enabled = (buttons[i].name != list.sortType);
		}

		list.draw();

	}


	void onClickClose(GameObject go)
	{
		hide();
	}

	public void hide()
	{
		gameObject.SetActive(false);
	}

}
