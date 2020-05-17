using UnityEngine;
using System.Collections;

sealed public class CharacterTooltip : CharacterAttachedUI {
	
	public UISprite spTooltip;
	public UILabel tfText;
	
	Bounds _b;
	
	Bounds _b1;
	Bounds _b2;
	Bounds _b3;
	Bounds _b4;
	
	public void setData(string txt, float delay)
	{
		tfText.text = "\n" + txt + "\n";
		gameObject.SetActive(true);
		
		_b = NGUIMath.CalculateAbsoluteWidgetBounds(tfText.transform);
		spTooltip.width = Mathf.RoundToInt(_b.extents.x * 2.0f + 70.0f);
		spTooltip.height = Mathf.RoundToInt(_b.extents.y * 2.0f + 95.0f);
		_leftTime = delay;

		visible = true;
	}
	
	public float _leftTime = 0.0f;
	
	void LateUpdate()
	{
		if(_visible)
		{
			_leftTime -= CutSceneManager.cutSceneDeltaTime;

			if(_leftTime <= 0.0f)
			{
				visible = false;
			}
		}
	}
}
