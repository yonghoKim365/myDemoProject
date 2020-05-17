using UnityEngine;
using System.Collections;

public class UIInstantDungeonSelectHeroSlot : MonoBehaviour {

	public UIButton btn;

	public UISprite spMainSubIcon;

	public UISprite spBackground;
	public UISprite spCharacter;

	public string iconName;

	public UISprite spSelect;

	public string characterName;

	private bool _isSelect = false;

	private bool _isEnable = false;
	public bool isEnable
	{
		set
		{
			if(value)
			{
				spCharacter.spriteName = iconName;
				spBackground.spriteName = "img_selecthero_bg_on";
			}
			else
			{
				spCharacter.spriteName = iconName + "grey";
				spBackground.spriteName = "img_selecthero_bg_off";
			}

			btn.enabled = value;
			_isEnable = value;
		}
		get
		{
			return _isEnable;
		}
	}

	public bool isSelect
	{
		set
		{
			_isSelect = value;
			spSelect.enabled = _isSelect;

			if(_isEnable)
			{
				btn.enabled = !value;
			}
			else
			{
				btn.enabled = false;
			}
		}
		get
		{
			return _isSelect;
		}
	}


}
