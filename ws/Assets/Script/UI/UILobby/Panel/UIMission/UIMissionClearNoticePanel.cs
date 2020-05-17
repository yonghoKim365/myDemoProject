using UnityEngine;
using System.Collections;
using System;

public class UIMissionClearNoticePanel : MonoBehaviour
{
	public UIPanel panel;

	public UISprite spIcon, spClear;

	public UILabel lbTitle;

	public UILabel lbDescription;

	public UISprite[] spRewardIcon;
	public UILabel[] lbRewardCount;

	public GameObject cachedGameobject;

	public TweenAlpha tweenAlpha;

	public UIChallengeItemSlot rewardItemSlot;

	void Awake()
	{
		panel.alpha = 0;
		cachedGameobject = gameObject;
	}

	bool _isFinalNotice = false;

	public bool isScarlet = false;

	public void start(P_MissionNotification notiData, int index, bool isFinalNotice)
	{
		_isFinalNotice = isFinalNotice;

		panel.alpha = 0;

		if(notiData.desc == null)
		{
			isScarlet = false;
			setNormalData(notiData);
		}
		else
		{
			isScarlet = true;
			setNormalData(notiData);

			if(notiData.desc != null && lbDescription != null) lbDescription.text = notiData.desc;
		}

		if(cachedGameobject == null) cachedGameobject = gameObject;

		gameObject.SetActive(true);

		panel.depth = 100 + index;
		_state = State.FadeIn;
	}


	void setNormalData(P_MissionNotification data)
	{
		lbTitle.text = data.title;
		
		switch(data.category)
		{
		case "HERO":
			spIcon.spriteName = "icn_quest_hero";
			break;
		case "EPIC":
			spIcon.spriteName = "icn_quest_epic";
			break;
		case "CHALL":
			spIcon.spriteName = "icn_quest_challenge";
			break;
		case "CHAMP":
			spIcon.spriteName = "icn_quest_champion";
			break;
		case "FRIEND":
			spIcon.spriteName = "icn_quest_friend";
			break;
		case "BUY":
			spIcon.spriteName = "icn_quest_shop";
			break;
		case "RUNE":
			spIcon.spriteName = "icn_quest_rune";
			break;
		case "ETC":
			spIcon.spriteName = "icn_quest_etc";
			break;
		}
		
		int rewardCount = 0;


		if(data.rewards != null) rewardCount = data.rewards.Length;
		
		int i = 0;
		
		bool hasGacha = false;
		
		foreach(P_Reward p in data.rewards)
		{
			if(i > 1) break;
			
			string iconId = null;
			
			switch(p.code)
			{
			case "FRPNT": 
				iconId = WSDefine.ICON_HART;
				break;//	우정포인트	
			case "EXP": 
				iconId = WSDefine.ICON_EXP;
				break;//	경험치	
			case "ENGY": 
			case "ENERGY":
				iconId = WSDefine.ICON_ENERGY;
				break;//	에너지	
			case "GOLD": 
				iconId = WSDefine.ICON_GOLD;
				break;//	골드	
				//			case "RST": 
				//				iconId = WSDefine.ICON_RUNESTONE;
				//				break;//	룬스톤	
			case "RUBY":
				iconId = WSDefine.ICON_RUBY;
				break;//	루비	

			case "RUNESTONE":
			case WSDefine.RUNESTONE:

				iconId = WSDefine.ICON_RUNESTONE;

				break;

			case "SML": break;//	레전드 소환룬	
			case "SMS": break;//	슈퍼레어 소환룬	
			case "SMR": break;//	레어 소환룬	
			case "SMN": break;//	노말 소환룬	
			case "SMALL": break;//	레어도 랜덤 소환룬	
			case "SKL": break;//	레전드 스킬룬	
			case "SKS": break;//	슈퍼레어 스킬룬	
			case "SKR": break;//	레어 스킬룬	
			case "SKM": break;//	노말 스킬룬	
			case "SKALL": break;//	레어도 랜덤 스킬룬	
			case "SMSLT": break;//	소환룬 슬롯 활성화	
			case "SKSLT": break;//	소환룬 슬롯 활성화	
				
			case WSDefine.REWARD_TYPE_GACHA:
				hasGacha = true;
				break;
				
			}
			
			if(iconId != null)
			{
				spRewardIcon[i].spriteName = iconId;
				lbRewardCount[i].text = p.count + "";
				++i;
			}
		}
		
		if(i == 1)
		{
			spRewardIcon[0].enabled = true;
			spRewardIcon[1].enabled = false;
			
			lbRewardCount[0].enabled = true;
			lbRewardCount[1].enabled = false;
			
			spRewardIcon[0].MakePixelPerfect();
		}
		else if(i == 2)
		{
			spRewardIcon[0].enabled = true;
			spRewardIcon[1].enabled = true;
			spRewardIcon[0].MakePixelPerfect();
			spRewardIcon[1].MakePixelPerfect();
			
			lbRewardCount[0].enabled = true;
			lbRewardCount[1].enabled = true;
			
		}
		else
		{
			spRewardIcon[0].enabled = false;
			spRewardIcon[1].enabled = false;
			
			lbRewardCount[0].enabled = false;
			lbRewardCount[1].enabled = false;
		}
		
		rewardItemSlot.gameObject.SetActive(hasGacha);

	}



	enum State
	{
		FadeIn, FadeOut, Close
	}

	State _state = State.Close;

	public void onCompleteTween()
	{
		cachedGameobject.SetActive(false);
		UIMissionClearNotice.setPanelToPool(this);
		if(_isFinalNotice) UIMissionClearNotice.instance.gameObject.SetActive(false);
	}

	IEnumerator setWaitMode()
	{
		yield return new WaitForSeconds(0.7f);
		_state = State.FadeOut;
		onCompleteTween();
	}

}
