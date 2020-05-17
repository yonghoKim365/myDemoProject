using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class TutorialManager : MonoBehaviour 
{
	public void prepareTutorial(string id)
	{
		isStart = false;
		isTutorialSkipCheckMode = false;

		if(clearCheck(id)) return;

		readyTutorialId = "";
		nowTutorialId = id;
		isTutorialMode = true;
		isReadyTutorialMode = false;
		//subStep = 1;
		uiTutorial.hide();

		SoundManager.instance.clearTutorialVoiceAsset();

		if(id == "T52") // 52번은 사운드가 없음.
		{
			startTutorial();
		}
		else
		{
			SoundManager.instance.loadTutorialSoundAsset(startTutorial);
		}

	}


	public void startTutorial()
	{
		GameManager.me.uiManager.popupOption.hide();

		switch(nowTutorialId)
		{
		case "T1":
			openDialog(200,250,true,true,true);
			uiTutorial.showBigSizeCharacter();
			break;
		case "T2": //		2	탐험 게임모드 (섬멸)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 섬멸(KILLEMALL) 게임모드
			openDialog(200,400,false,true);
			StartCoroutine(minmapPointerVisibleAnimation(1,1));
			break;
		case "T3": //			3	탐험 게임모드 (서바이벌)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 서바이벌 게임모드
			openDialog(200,400,false,true);
			StartCoroutine( uiVisibleAnimation(1,1,uiManager.uiPlay.lbRoundTime.gameObject));
			break;
		case "T4": //				4	탐험 게임모드 (보호)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 보호(PROTECT) 게임모드
			openDialog(200,300,false,true);
			StartCoroutine(minmapPointerVisibleAnimation(1,1,CharacterMinimapPointer.PROTECT));
			break;
		case "T5"://				5	탐험 게임모드 (보스대전)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 보스대전(SNIPING) 게임모드
			openDialog(200,300,false,true);
			StartCoroutine(minmapPointerVisibleAnimation(1,1,CharacterMinimapPointer.BOSS));
			break;
		case "T6"://	6	탐험 게임모드 (몬스터사냥)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 몬스터사냥(KILLCOUNT) 게임모드
			openDialog(200,300,false,true);
			StartCoroutine( uiVisibleAnimation(1,1,uiManager.uiPlay.lbRoundLeftNum.gameObject));
			break;
		case "T7"://	7	탐험 게임모드 (목표지점이동)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 목표지점이동(ARRIVE) 게임모드
			openDialog(200,300,false,true);
			StartCoroutine(minmapPointerVisibleAnimation(1,1,CharacterMinimapPointer.DISTANCE));
			break;
		case "T8"://	8	탐험 게임모드 (목표물파괴)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 목표물파괴(DESTROY) 게임모드
			openDialog(200,300,false,true);
			StartCoroutine(minmapPointerVisibleAnimation(1,1,CharacterMinimapPointer.OBJECT));
			break;
		case "T9"://	9	탐험 게임모드 (아이템획득)	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 아이템획득(GETITEM) 게임모드
			openDialog(200,300,false,true);
			break;
		case "T10"://	10	추격몬스터	탐험모드 & 전투 스타트 직전 (컷씬 완료 후) & 추격몬스터 존재
			openDialog(200,300,false,true);
			uiTutorial.setDimPosition(378,603);
			StartCoroutine(minmapPointerVisibleAnimation(1,2,CharacterMinimapPointer.CHASER));
			break;

		case "T13":
			// 월드맵에서 발동되는 튜토리얼 때는 얘들이 뜨면 안된다...

//			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
//			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
//			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
//			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
//			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			uiManager.uiMenu.uiLobby.spHasTutorialForMission.gameObject.SetActive(false);


			openDialog(200,400,true,true);
			uiTutorial.showBigSizeCharacter();
			break;

		case "T15":
			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

//			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
//			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
//			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
//			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			uiManager.uiMenu.uiLobby.spHasTutorialForFriend.gameObject.SetActive(false);

			break;

//		case "T17": //할것.
//			// 월드맵에서 발동되는 튜토리얼 때는 얘들이 뜨면 안된다...
//			GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
//			if(uiManager.popupChallenge.gameObject.activeSelf) uiManager.popupChallenge.hide();
//			openDialog(200,400,true,true);
//			uiTutorial.showBigSizeCharacter();
//
//			break;
		case "T18"://			18	도전 게임모드 설명 (무한질주)	도전모드 & 전투 스타트 직전 & 무한질주 게임모드
			openDialog(200,300,false,true);
			break;
		case "T19"://			19	도전 게임모드 설명 (무한생존)	도전모드 & 전투 스타트 직전 & 무한생존 게임모드
			openDialog(200,300,false,true);
			break;
		case "T20":
			openDialog(200,300,false,true);
			break;

		case "T24": //할것

			// 월드맵에서 발동되는 튜토리얼 때는 얘들이 뜨면 안된다...

//			if(uiManager.popupChallenge.gameObject.activeSelf) uiManager.popupChallenge.hide();
			uiManager.uiMenu.uiWorldMap.btnChampionship.gameObject.SetActive(true);
			uiManager.uiMenu.uiWorldMap.spChampionship.color = uiManager.uiMenu.uiWorldMap.btnChampionship.defaultColor;

			uiManager.uiMenu.uiWorldMap.spChampionshipExclamation.enabled = true;
			uiManager.uiMenu.uiWorldMap.spChampionshipDormancy.enabled = true;

			/*
			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
*/

			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();

			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);


			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

			uiManager.uiMenu.uiWorldMap.spHasTutorialForChampionship.gameObject.SetActive(false);

			break;

		case "T37":
			openDialog(200,400,true,true,true);
			break;


		case "T44":
			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			break;

		case "T45":
			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			break;

		case "T46":
			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			break;

//		case "T47":
//			openDialog(200,400,true,true,true);
//			uiTutorial.showBigSizeCharacter();
//			break;

		case "T48":
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf == false)
			{
				uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject.SetActive(true);
			}


			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);

			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();
			break;

		case "T49":
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf == false)
			{
				uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject.SetActive(true);
			}

			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);

			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();
			break;

		case "T50":
			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf == false)
			{
				uiManager.uiMenu.uiWorldMap.stageDetail.btnClose.gameObject.SetActive(true);
			}


			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);
			if(uiManager.popupHell.gameObject.activeSelf) uiManager.popupHell.gameObject.SetActive(false);

			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();
			break;

		case "T51":
			openDialog(200,400,true,true,true);
			uiTutorial.showBigSizeCharacter();

			uiManager.uiMenu.uiWorldMap.spHell.color = uiManager.uiMenu.uiWorldMap.btnHell.defaultColor;

			if(uiManager.popupFriendlyPVPAttack.gameObject.activeSelf) GameManager.me.uiManager.popupFriendlyPVPAttack.gameObject.SetActive(false);

			if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
			if(uiManager.popupChampionship.gameObject.activeSelf) uiManager.popupChampionship.gameObject.SetActive(false);
			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			uiManager.uiMenu.uiWorldMap.spHasTutorialForHell.gameObject.SetActive(false);

			break;


		case "T52":
			openDialog(200,400,true,false,true);

			//uiTutorial.btnOKDialog.gameObject.SetActive(true);
			StartCoroutine(delayTuto52Step());

			if(uiManager.popupShop.gameObject.activeSelf) uiManager.popupShop.gameObject.SetActive(false);

			GameManager.me.uiManager.uiMenu.uiSummon.tutorialComposePanel.gameObject.SetActive(true);

			string original = null;
			string source = null;


			for(int i = GameDataManager.instance.unitInventoryList.Count -1 ; i >= 0 ; --i)
			{
				if(GameDataManager.instance.unitInventoryList[i].rare == RareType.D &&
				   GameDataManager.instance.unitInventoryList[i].reinforceLevel == GameIDData.MAX_LEVEL)
				{
					if(original == null)
					{
						original = GameDataManager.instance.unitInventoryList[i].serverId;
					}
					else if(source == null)
					{
						source = GameDataManager.instance.unitInventoryList[i].serverId;
					}
					else break;
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

						if(original == null)
						{
							original = ps.unitInfo.serverId;
						}
						else if(source == null)
						{
							source = ps.unitInfo.serverId;
							break;
						}

					}
				}
			}

			if(original == null) original = "UN10702001_0";
			if(source == null) source = "UN11202001_0";

			if(original != null && source != null) GameManager.me.uiManager.uiMenu.uiSummon.tutorialComposePanel.setData(original, source);


			break;
		}
	}


	IEnumerator delayTuto52Step()
	{
		yield return new WaitForSeconds(3.0f);
		uiTutorial.btnOKDialog.gameObject.SetActive(true);

	}



}