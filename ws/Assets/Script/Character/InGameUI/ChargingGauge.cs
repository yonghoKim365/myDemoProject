using UnityEngine;
using System.Collections;

sealed public class ChargingGauge : CharacterAttachedUI {
	
	public UISprite spCircle;
	public ParticleSystem psFull;
	public ParticleSystem psFullAura;
	public bool isFull = false;

	const string FULL_SPRITE = "pgsb_circle_foreground2";
	const string PROGRESS_SPRITE = "pgsb_circle_foreground";

	sealed public override void init(Transform pointer, float posX = 0.0f, float posY = 100.0f, bool isVisible = false)
	{
		base.init(pointer, posX, posY, isVisible);

		psFullAura.Stop();

		spCircle.fillDirection = UISprite.FillDirection.Radial360;

		setData(0);
	}

	public void setData(float per)
	{
		if(_isEnabled)
		{
			if(per >= 1.0f)
			{
				per = 1.0f;
				setFull();
			}
			else
			{
				if(per <= 0) spCircle.spriteName = PROGRESS_SPRITE;

				isFull = false;
				spCircle.fillAmount = per;
			}
		}		
	}


	public void setFull()
	{
		if(_isEnabled && isFull == false)
		{
			spCircle.spriteName = FULL_SPRITE;
			spCircle.fillAmount = 1.0f;
			psFull.Play();
			isFull = true;
			psFullAura.Play();
		}		
	}


}
