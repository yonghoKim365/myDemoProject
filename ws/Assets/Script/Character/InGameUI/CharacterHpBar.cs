using UnityEngine;
using System.Collections;

sealed public class CharacterHpBar : CharacterAttachedUI {

	//public UISlider energyBar;
	public const string PLAYER_HP_NAME = "hp";
	public const string ENEMY_HP_NAME = "ehp";
	public const string HP_MAX_EFFECT_HP_NAME = "shp";

	public tk2dClippedSprite spEnergy;

	sealed public override bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			_visible = value;
			if(value == true) setPosition();
			gameObject.SetActive(value);
		}
	}	

	Rect _rect;

	public void setData(float hpPer)
	{
		if(_isEnabled)
		{
			_rect = spEnergy.ClipRect;
			_rect.x = 1.0f - hpPer;
			spEnergy.ClipRect = _rect;
			//energyBar.value = hpPer;
		}		
	}
	
	public void setData(float hp, float maxHp)
	{
		if(_isEnabled)
		{
			//energyBar.value = hp/maxHp;
			_rect = spEnergy.ClipRect;
			spEnergy.ClipRect = _rect;
		}
	}
}
