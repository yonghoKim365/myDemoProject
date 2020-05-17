using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITutorialRewardPopup : UISystemPopupBase 
{

	public UISprite[] priceContainer;

	public UISprite[] sp;
	public UILabel[] lb;

	public ParticleSystem effect;

	public void open(Dictionary<string, int> rewards)
	{
		lbMsg.text = Util.getUIText("TUTO_COMPLETE_REWARD");

		int i = 0;
		foreach(KeyValuePair<string, int> kv in rewards)
		{
			priceContainer[i].gameObject.SetActive(true);

			switch(kv.Key)
			{
			case WSDefine.RUBY:
				sp[i].spriteName = WSDefine.ICON_RUBY;
				break;
			case WSDefine.GOLD:
				sp[i].spriteName = WSDefine.ICON_GOLD;
				break;
			case WSDefine.ENERGY:
				sp[i].spriteName = WSDefine.ICON_ENERGY;
				break;
			case WSDefine.EXP:
				sp[i].spriteName = WSDefine.ICON_EXP;
				break;
			}

			sp[i].MakePixelPerfect();

			lb[i].text = Util.GetCommaScore(kv.Value) ;
			++i;
			if(i >= 3) break;
		}

		switch(i)
		{
		case 1:
			priceContainer[0].width = 253;

			_v = priceContainer[0].transform.localPosition;
			_v.x = -60.0f;
			priceContainer[0].transform.localPosition = _v;

			_v = sp[0].cachedTransform.localPosition;
			_v.x = 226;
			sp[0].cachedTransform.localPosition = _v;

			priceContainer[1].gameObject.SetActive(false);
			priceContainer[2].gameObject.SetActive(false);

			break;

		case 2:
			priceContainer[0].width = 200;
			priceContainer[1].width = 200;


			_v = priceContainer[0].transform.localPosition;
			_v.x = -128.0f;
			priceContainer[0].transform.localPosition = _v;

			_v.x = 106.0f;
			priceContainer[1].transform.localPosition = _v;


			_v = lb[0].cachedTransform.localPosition;
			_v.x = 181.74f;

			lb[0].transform.localPosition = _v;
			lb[1].transform.localPosition = _v;

			priceContainer[2].gameObject.SetActive(false);
			break;

		case 3:
			priceContainer[0].width = 161;
			priceContainer[1].width = 161;

			_v = priceContainer[0].transform.localPosition;
			_v.x = -161.0f;
			priceContainer[0].transform.localPosition = _v;
			_v.x = 6.0f;
			priceContainer[1].transform.localPosition = _v;
			_v.x = 177.0f;
			priceContainer[2].transform.localPosition = _v;

			_v = lb[0].cachedTransform.localPosition;
			_v.x = 142;
			lb[0].cachedTransform.localPosition = _v;
			lb[1].cachedTransform.localPosition = _v;

			break;
		}


		// -204   -36  135
		// -138   73
		//


		gameObject.SetActive(true);

		if(popupPanel != null && useScaleTween && Time.timeScale >= 1.0f)
		{
			if(ani != null)
			{
				ani.Play();
			}
		}

		effect.Play();

		SoundData.play("uiet_tutorialrwd");
	}
	
	protected override void onClose (GameObject go)
	{
		TutorialManager.waitStartBattle = false;
		base.onClose (go);


		switch(TutorialManager.instance.prevReceivedRewardTutorialId)
		{
		case "T45":
			GameManager.me.uiManager.popupSkillPreview.needToShowTutorialEndCircleEffect = true;
			break;
		}

	}
}
