using UnityEngine;
using System.Collections;
using System;

public class UIMissionListSlotPanel : UIListGridItemPanelBase {

	public UIButton btnReceive;
	public UISprite spReceiveText;

	public UISprite spIcon, spClear, spNew;

	public UILabel lbTitle;

	public UIChallengeItemSlot spRewardItemSlot;

	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnReceive.gameObject).onClick = onClickReceive;
	}
	
	void onClickReceive(GameObject go)
	{
		if(data != null && TutorialManager.instance.clearCheck("T13") == false && data.id.Contains("M_EP_001_01"))
		{
			if(TutorialManager.instance.isTutorialMode == false) return;
		}

		GameManager.me.uiManager.uiMenu.uiMission.onReceiveItem(this);
	}

	public override void setPhotoLoad()
	{
		if(data == null) return;
	}	

	public P_Mission data;

	Vector3 _v;

	public override void setData(object obj)
	{
		isTimeType = false;

		data = GameDataManager.instance.missions[(string)obj];
		lbTitle.text = data.title;

		//HERO	EPIC	CHALL	CHAMP	FRIEND	BUY	RUNE	ETC

		btnReceive.gameObject.name = data.id;

		spRewardItemSlot.gameObject.SetActive(false);

		switch(data.category)
		{
		case "HERO":
			spIcon.spriteName = "icn_quest_hero";
			break;
		case "EPIC":
			spIcon.spriteName = "icn_quest_epic";
			break;
		case "HELL":
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
		default:
			spIcon.spriteName = "icn_quest_etc";
			break;
		}

		spClear.enabled = (data.state == WSDefine.MISSION_CLEAR);

		bool hasKey = UIMission.checkNewDic.ContainsKey(data.id);

		if(!hasKey)
		{
			UIMission.checkNewDic.Add(data.id, false);
		}


		if(UIMission.checkNewDic[data.id] == false)
		{
			spNew.enabled = (data.state != WSDefine.MISSION_CLEAR);
		}
		else
		{
			spNew.enabled = false;
		}


		if(data.state == WSDefine.MISSION_CLEAR)
		{
			btnReceive.isEnabled = true;
			spReceiveText.color = btnReceive.defaultColor;

		}
		else
		{
			btnReceive.isEnabled = false;
			spReceiveText.color = btnReceive.disabledColor;
		}


		if(data.visible == WSDefine.YES) // 진행상황을 보여줌.
		{

		}
		else
		{

		}

/*
		public int state;		// 0:DOING, 1:CLEAR, 2:CLOSE
		public int visible;		// YES:1, NO:0
*/
		bool hasGacha = false;

		int rewardCount = data.rewards.Length;

		int i = 0;
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

			case "RSTONE": 
				iconId = WSDefine.ICON_RUNESTONE;
				break;//	골드	

//			case "RST": 
//				iconId = WSDefine.ICON_RUNESTONE;
//				break;//	룬스톤	
			case "RUBY":
				iconId = WSDefine.ICON_RUBY;
				break;//	루비	
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


			case "GACHA":
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

		spRewardItemSlot.gameObject.SetActive(hasGacha);

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

		if(data.visible == WSDefine.YES)
		{
			lbProgress.gameObject.SetActive(true);

			int vValue = -1000;
			int cValue = -1000;
			int.TryParse(data.clearVariable, out vValue);
			int.TryParse(data.clearConstant, out cValue);

			if(vValue > -1000 && cValue > -1000 && vValue > cValue)
			{
				lbProgress.text = "[71b416]" + data.clearConstant + "[-] / [f7f0a0]" + data.clearConstant + "[-]";
			}
			else
			{
				lbProgress.text = "[71b416]" + data.clearVariable + "[-] / [f7f0a0]" + data.clearConstant + "[-]";
			}
		}
		else
		{
			lbProgress.gameObject.SetActive(false);
		}


		if(data.closeDate > 0)
		{

			lbLeftTime.gameObject.SetActive(true);

			if(lbProgress.cachedGameObject.activeSelf)
			{
				lbProgress.transform.localPosition = new Vector3(165,179,0);

				_v = lbLeftTime.cachedTransform.localPosition;
				_v.y = 164;
				lbLeftTime.cachedTransform.localPosition = _v;

			}
			else
			{
				_v = lbLeftTime.cachedTransform.localPosition;
				_v.y = 179.8f;
				lbLeftTime.cachedTransform.localPosition = _v;
			}

			isTimeType = true;

			TimeSpan ts = (DateTime.Now - GameDataManager.instance.missionCloseTime[data.id]);
			int leftTime = data.closeDate - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = "-";
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime);
			}


			StartCoroutine(updateTime());
		}
		else
		{
			lbProgress.transform.localPosition = new Vector3(165,165,0);
			lbLeftTime.gameObject.SetActive(false);
		}
	}
	
	
	public UISprite[] spRewardIcon;
	public UILabel[] lbRewardCount;

	public UILabel lbProgress;
	public UILabel lbLeftTime;

	bool isTimeType = false;


	IEnumerator updateTime()
	{
		yield return Util.ws1;

		if(isTimeType)
		{
			TimeSpan ts = (DateTime.Now - GameDataManager.instance.missionCloseTime[data.id]);
			int leftTime = data.closeDate - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = "-";
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime);
			}

		}
	}


}
