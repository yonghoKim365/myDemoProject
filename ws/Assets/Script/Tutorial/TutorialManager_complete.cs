using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class TutorialManager : MonoBehaviour 
{


	public void onCompleteTutorial()
	{
		switch(nowTutorialId)
		{
		case "T2":
		case "T3":
		case "T4":
		case "T5":
		case "T6":
		case "T7":
		case "T8":
		case "T9":
		case "T10":
		case "T18":
		case "T19":
		case "T20":
		case "T48":
		case "T49":
		case "T50":
		case "T52":
			uiTutorial.hide();
			waitStartBattle = false;
			closeTutorial();
			break;


		case "T13":
			uiTutorial.hide();
			openDialog(200,100,true,true);
			break;
			/*
		case "T14":
			uiTutorial.hide();
			openDialog(200,450,true,true);
			break;
			*/
		case "T15":
			uiTutorial.hide();
			openDialog(200,400,true,true);
			break;
			/*
		case "T16":
			// 여기서는 포기일때만...
			uiTutorial.hide();
			waitStartBattle = false;
			isTutorialMode = false;
			nowTutorialId = "";
			break;
			
		case "T23":
			Debug.LogError("GameDataManager.instance.tutorialEquip1 : " + GameDataManager.instance.tutorialEquip1 + "   GameDataManager.instance.tutorialEquip2 : " + GameDataManager.instance.tutorialEquip2);
			rareSlot = null;

			rareSourceId =  GameDataManager.instance.tutorialEquip2;

			foreach(UIHeroInventorySlot slot in uiManager.uiMenu.uiHero.inventorySlots)
			{
				if(slot.data != null)
				{
					if(slot.data.id == GameDataManager.instance.tutorialEquip1) rareSlot = slot;
					if(rareSlot != null) break;
				}
			}
			
			if(rareSlot != null)
			{
				uiTutorial.hide();
				
				Vector2 screenPos = Util.screenPositionWithCamViewRect(GameManager.me.uiManager.uiMenu.camera.WorldToScreenPoint(rareSlot.transform.position));
				uiTutorial.setArrowAndDim(screenPos.x + 8, screenPos.y - 115.0f, false);
			}
			
			break;
			*/
//		case "T24":
//			EpiServer.instance.sendRewardTutorial(nowTutorialId);
//			break;

		case "T34":
			subStep = 11;
			break;

		case "T36":
			uiTutorial.hide();
			openDialog(200,400,true,true);
			uiTutorial.showBigSizeCharacter();
			break;


		case "T46":
			uiTutorial.hide();
			openDialog(200,400,true,true);
			uiTutorial.showBigSizeCharacter();
			break;


		default:
			EpiServer.instance.sendRewardTutorial(nowTutorialId);
			break;
		}
	}



}