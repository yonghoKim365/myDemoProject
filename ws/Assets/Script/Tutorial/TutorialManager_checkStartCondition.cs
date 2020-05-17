using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class TutorialManager : MonoBehaviour 
{

	public bool isStart = false;

	public bool check(string id, int startSubIndex = 1)
	{
		isTutorialSkipCheckMode = false;

		//return false;

		#if UNITY_EDITOR

		if(string.IsNullOrEmpty(checkTutorialId) == false && id != checkTutorialId )
		{
//			Debug.LogError("튜토리얼 테스트 중 수정해야한다! 여기를!");
			return false;
		}
//

		if(DebugManager.instance.useDebug)
		{
			GameDataManager.instance.completeTutorial = (useTutorialDebugMode == false || BattleSimulator.nowSimulation)?WSDefine.YES:WSDefine.NO;
		}

		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker) return false;

		Debug.Log("isTutorialMode : " + isTutorialMode + "   check Tuto Id : " + id);
		Debug.Log("GameDataManager.instance.completeTutorial : " + GameDataManager.instance.completeTutorial);

		#endif




		if(GameDataManager.instance.lastPlayStatus != WSDefine.LAST_PLAY_STATUS_NONE) return false;

		if(UISystemPopup.instance.popupYesNo.gameObject.activeSelf) return false;

		if(UISystemPopup.instance.popupDefault.gameObject.activeSelf) return false;

		if(GameManager.me.uiManager.popupShop.gameObject.activeSelf) return false;

		if(DebugManager.useTestRound) return false;

		if(isTutorialMode || GameDataManager.instance.completeTutorial  == WSDefine.YES|| (GameDataManager.instance.tutorialHistory.ContainsKey(id)) ) return false;

		if(isStart) return false;

		GameDataManager gm = GameDataManager.instance;
		
		int num = 0;

		subStep = startSubIndex;

		switch(id)
		{
			//	1	탐험모드 플레이 유도	계정 생성 완료 & 메인페이지(로비) 진입 & 보유 에너지 1 이상
		case "T1":
			if(GameDataManager.instance.energy > 0)// && GameDataManager.instance.level == 1) // && GameDataManager.instance.exp == 0 
			{
				isStart = true;
			}
			break;
			
		case "T2": waitStartBattle = true; isStart = true; break;
		case "T3": waitStartBattle = true; isStart = true; break;
		case "T4": waitStartBattle = true; isStart = true; break;
		case "T5": waitStartBattle = true; isStart = true; break;
		case "T6": waitStartBattle = true; isStart = true; break; 
		case "T7": waitStartBattle = true; isStart = true; break;
		case "T8": waitStartBattle = true; isStart = true; break;
		case "T9": waitStartBattle = true; isStart = true; break;
		case "T10": waitStartBattle = true; isStart = true; break;

		case "T13":
			//[액트1 스테이지1 라운드3] 이상 클리어 & 월드맵 페이지 & 스테이지 이동 연출 완료 후
			if( GameDataManager.instance.roundClearStatusCheck(1,1,3)  )
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;

			}
			break;

		case "T15":
			//[액트1 스테이지2 라운드2] 이상 클리어 & 월드맵 라운드 정보 팝업 & 카톡로그인유저
			if(GameDataManager.instance.roundClearStatusCheck(1,1,3) && GameManager.me.isGuest == false)
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;

			}
			break;

//		case "T17":
//			//[액트1 스테이지3 라운드3] 이상 클리어 & 월드맵 페이지 & 스테이지 이동 연출 완료 후 & 보유에너지 1이상
//			if(gm.energy >= 1 && GameDataManager.instance.roundClearStatusCheck(1,3,3))
//			{
//				isStart = true;
//
//			}
//			break;
			
		case "T18": waitStartBattle = true; isStart = true; break;
		case "T19": waitStartBattle = true; isStart = true;break;
		case "T20": waitStartBattle = true; isStart = true; break;

		case "T24":

			if( 
			   GameDataManager.instance.roundClearStatusCheck(1,4,4) && 
			   (gm.championshipStatus == WSDefine.CHAMPIONSHIP_OPEN  ) &&
			   uiManager.uiMenu.currentPanel == UIMenu.WORLD_MAP &&
			   gm.championshipData != null 
			   )
			{
				if(nowTutorialId != "T24")
				{
					isStart = true;

				}
			}
			
			break;

		case "T37":
			//[액트2 스테이지5 라운드5] 이상 클리어 & 히어로페이지
			if(GameDataManager.instance.roundClearStatusCheck(2,5,5))
			{
				if(GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY) == false &&
				   (uiManager.uiMenu.currentPanel == UIMenu.HERO || uiManager.uiMenu.currentPanel == UIMenu.SUMMON || uiManager.uiMenu.currentPanel == UIMenu.SKILL))
				{
					isStart = true;
				}
			}
			break;

		case "T44":

			if(GameDataManager.instance.roundClearStatusCheck(1,1,2))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;
			}

			break;
		case "T45":

			if(GameDataManager.instance.roundClearStatusCheck(1,1,3))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);

				foreach(GameIDData gd in GameDataManager.instance.skillInventoryList)
				{
					if(gd.serverId.Contains("SK_2110_1"))
					{
						isStart = true;
						break;
					}
				}
				
				if(isStart == false)
				{
					EpiServer.instance.sendCompleteTutorial("T45", true);
				}
			}

			break;
		case "T46":

			if(GameDataManager.instance.roundClearStatusCheck(1,2,3))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);

				foreach(GameIDData gd in GameDataManager.instance.partsInventoryList)
				{
					if(gd.serverId.Contains("LEO_RD2_11_1"))
					{
						isStart = true;
						break;
					}
				}

				if(isStart == false)
				{
					EpiServer.instance.sendCompleteTutorial("T46", true);
				}
			}

			break;
//		case "T47":
//
//			if(GameDataManager.instance.roundClearStatusCheck(1,5,5))
//			{
//				isStart = true;
//			}
//
//			break;
			/*
		case "T48":
			//액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
			if(GameDataManager.instance.roundClearStatusCheck(1,2,3))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;
			}
			break;
		case "T49":
			//액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
			//if(GameDataManager.instance.currentAct >= 2 || clearCheck("T49"))
			if(clearCheck("T48"))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;
			}
			break;
		case "T50":
			//액트2 이상인 경우 재도전 선택 완료 후 라운드정보창 팝업상태
			//if(GameDataManager.instance.currentAct >= 2 || clearCheck("T50"))
			if(clearCheck("T49"))
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;
			}
			break;
		*/
		case "T51":
			//1	3	4
			//월드맵 스테이지 이동 연출 완료
			//if(GameDataManager.instance.currentAct >= 2 || clearCheck("T50"))
			if( GameDataManager.instance.roundClearStatusCheck(1,3,4) && GameDataManager.instance.championshipStatus == WSDefine.CHAMPIONSHIP_OPEN && uiManager.uiMenu.currentPanel == UIMenu.WORLD_MAP)
			{
				if(GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
				isStart = true;
				
			}
			break;

		case "T52":

			int composeSourceNumber = 0;
			
			for(int i = GameDataManager.instance.unitInventoryList.Count -1 ; i >= 0 ; --i)
			{
				if(GameDataManager.instance.unitInventoryList[i].rare == RareType.D &&
				   GameDataManager.instance.unitInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					++composeSourceNumber;
					if(composeSourceNumber >= 2) break;
				}
			}

			if(GameDataManager.instance.unitSlots != null)
			{
				foreach(PlayerUnitSlot ps in GameDataManager.instance.unitSlots)
				{
					if(ps.isOpen == false || ps.unitInfo == null) continue;
					
					if(ps.unitInfo.rare == RareType.D &&
					   ps.unitInfo.reinforceLevel == GameIDData.MAX_LEVEL)
					{
						++composeSourceNumber;
						if(composeSourceNumber >= 2) break;
					}
				}
			}


			if(composeSourceNumber >= 2)
			{
				isStart = true;
			}

			break;

		}

		if(isStart)
		{
			isReadyTutorialMode = true;
			readyTutorialId = id;

			if(GameManager.info.tutorialInfoData[id].canSkip)
			{
				EpiServer.instance.sendPrepareTutorial(id);
			}
			else
			{
				isTutorialSkipCheckMode = false;
				EpiServer.instance.sendStartTutorial(readyTutorialId);
			}

			return true;
		}

		return false;
	}
	


	void onConfirmStartTutorial()
	{
		TutorialManager.instance.isTutorialSkipCheckMode = false;
		EpiServer.instance.sendStartTutorial(readyTutorialId);
	}

	void onSkipTutorial()
	{
		isStart = false;
		TutorialManager.instance.isTutorialSkipCheckMode = false;
		uiTutorial.hide();
		EpiServer.instance.sendCompleteTutorial(readyTutorialId, true);
	}



	public void getTutorialPrepareData(ToC_PREPARE_TUTORIAL p)
	{
		isStart = false;

		isTutorialSkipCheckMode = true;
		uiTutorial.openTutorialDialog(186,457, Util.getUIText("CONFIRM_TUTORIAL",GameManager.info.tutorialInfoData[readyTutorialId].title),false);
		uiTutorial.goDialogYesNoButtonContainer.SetActive(true);

		int i = 0;
		if(p.ruby > 0)
		{
			uiTutorial.spPrepareRewardIcon[i].spriteName = WSDefine.ICON_RUBY;
			uiTutorial.spPrepareRewardIcon[i].MakePixelPerfect();
			uiTutorial.lbPrepareRewardLabel[i].text = Util.GetCommaScore(p.ruby);

			++i;
		}

		if(p.gold > 0)
		{
			uiTutorial.spPrepareRewardIcon[i].spriteName = WSDefine.ICON_GOLD;
			uiTutorial.spPrepareRewardIcon[i].MakePixelPerfect();
			uiTutorial.lbPrepareRewardLabel[i].text = Util.GetCommaScore(p.gold);

			++i;
		}
		
		if(p.energy > 0)
		{
			uiTutorial.spPrepareRewardIcon[i].spriteName = WSDefine.ICON_ENERGY;
			uiTutorial.spPrepareRewardIcon[i].MakePixelPerfect();
			uiTutorial.lbPrepareRewardLabel[i].text = Util.GetCommaScore(p.energy);

			++i;
		}
		
		if(p.exp > 0 && i < 3)
		{
			uiTutorial.spPrepareRewardIcon[i].spriteName = WSDefine.ICON_EXP;
			uiTutorial.spPrepareRewardIcon[i].MakePixelPerfect();
			uiTutorial.lbPrepareRewardLabel[i].text = Util.GetCommaScore(p.exp);

			++i;
		}

		switch(i)
		{
		case 1:
			uiTutorial.spPrepareRewardIcon[0].transform.parent.gameObject.SetActive(true);
			uiTutorial.spPrepareRewardIcon[1].transform.parent.gameObject.SetActive(false);
			uiTutorial.spPrepareRewardIcon[2].transform.parent.gameObject.SetActive(false);

			uiTutorial.spPrepareRewardIcon[0].transform.parent.transform.localPosition = new Vector3(-76,84,18);
			break;
		case 2:
			uiTutorial.spPrepareRewardIcon[0].transform.parent.gameObject.SetActive(true);
			uiTutorial.spPrepareRewardIcon[1].transform.parent.gameObject.SetActive(true);
			uiTutorial.spPrepareRewardIcon[2].transform.parent.gameObject.SetActive(false);

			uiTutorial.spPrepareRewardIcon[0].transform.parent.transform.localPosition = new Vector3(-190,84,18);
			uiTutorial.spPrepareRewardIcon[1].transform.parent.transform.localPosition = new Vector3(40,84,18);

			break;
		case 3:
			uiTutorial.spPrepareRewardIcon[0].transform.parent.gameObject.SetActive(true);
			uiTutorial.spPrepareRewardIcon[1].transform.parent.gameObject.SetActive(true);
			uiTutorial.spPrepareRewardIcon[2].transform.parent.gameObject.SetActive(true);

			uiTutorial.spPrepareRewardIcon[0].transform.parent.transform.localPosition = new Vector3(-252,84,18);
			uiTutorial.spPrepareRewardIcon[1].transform.parent.transform.localPosition = new Vector3(-76,84,18);
			uiTutorial.spPrepareRewardIcon[2].transform.parent.transform.localPosition = new Vector3(97,84,18);

			break;
		}



		switch(readyTutorialId)
		{

		case "T15":
			uiTutorial.lbPrepareRewardSpecial.text =  Util.getUIText("TT15_SP");
			uiTutorial.spPrepareRewardSpecialCover.enabled = true;
			uiTutorial.lbPrepareRewardSpecial.transform.localPosition = new Vector3(-192,-636,-19);
			break;

		default:
			uiTutorial.lbPrepareRewardSpecial.transform.localPosition = new Vector3(-192,-654,-19);
			uiTutorial.spPrepareRewardSpecialCover.enabled = false;
			uiTutorial.lbPrepareRewardSpecial.text = "";
			break;
		}
	}
	
	



	public void setArrowAndDimToInventorySlot(Vector3 localPos, bool isUp)
	{
		if(isUp) uiTutorial.setArrowAndDim(localPos.x + 113 + 7 + 126 + 22,localPos.y + 396,false);
		else uiTutorial.setArrowAndDim(localPos.x + 113 + 7 + 126 + 22,localPos.y + 406 + 140,true);
	}


}