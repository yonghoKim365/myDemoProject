using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RuneStudioMain : MonoBehaviour 
{

	public string composeResultId;
	public GameIDData.Type composeType;
	public GameIDData composeResultData = new GameIDData();

	public void playComposeResult(string newId, string[] sourceIds, GameIDData.Type cType)
	{
		GameManager.me.uiManager.uiMenu.uiHero.selectedSlot = null;
		GameManager.me.uiManager.uiMenu.uiSkill.selectSlot = null;
		GameManager.me.uiManager.uiMenu.uiSummon.selectSlot = null;

		StartCoroutine( playComposeResultCT(newId, sourceIds, cType) );
	}

	IEnumerator playComposeResultCT(string newId, string[] sourceIds, GameIDData.Type cType)
	{
		reinforceSlotContainer.SetActive(false);
		RuneStudioMain.instance.rootCompose.gameObject.SetActive(true);

		composeMovablePanelParent.transform.localScale = new Vector3(1,1,1);
		
		type = Type.Compose;

		composeType = cType;



		composeResultId = newId;
		composeResultData.parse(newId, cType);

		composeCardFrame1.setData(sourceIds[0], cType);
		composeCardFrame2.setData(sourceIds[1], cType);

		showCard512Studio(100);

		if(cType == GameIDData.Type.Unit)
		{
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

			GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[composeCardFrame1.data.unitData.resource]);
			GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[composeCardFrame2.data.unitData.resource]);
			GameDataManager.instance.startModelLoad();
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}
		else if(cType == GameIDData.Type.Equip)
		{
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}

		card2Container.gameObject.SetActive(true);

		card2BgSlot[0].customSizeRatio = 1.85f;
		card2BgSlot[1].customSizeRatio = 1.85f;

		card2BgSlot[0].setData(composeCardFrame1.data);
		card2BgSlot[1].setData(composeCardFrame2.data);

		composeResultData.parse(newId, cType);


		cam512.gameObject.SetActive(true);

		sendEvent(composeController[composeCardFrame1.data.rare]);

		if(TutorialManager.instance.isTutorialMode == false)
		{
			yield return new WaitForSeconds(0.4f);
			
			step = Step.Start;
			GameManager.me.uiManager.activeRuneStudioSkipButton();
		}
	}




	public void showComposeResultCard(bool isSkipMode = false)
	{

		GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);

		if(isSkipMode)
		{
			refreshInventory(composeResultData.type);

//			GameManager.me.uiManager.uiMenu.uiHero.refreshHeroModel();

			endProcess();

			switch(composeResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( composeResultData, RuneInfoPopup.Type.Normal, UIComposePanel.isTabSlot == false, false);
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.spSkipModeBackground.gameObject.SetActive(true);
				break;
			case GameIDData.Type.Skill:
				GameManager.me.uiManager.popupSkillPreview.show( composeResultData, RuneInfoPopup.Type.Normal, UIComposePanel.isTabSlot == false, false);
				GameManager.me.uiManager.popupSkillPreview.spSkipModeBackground.gameObject.SetActive(true);
				break;
				
			case GameIDData.Type.Unit:				
				GameManager.me.uiManager.popupSummonDetail.show( composeResultData, RuneInfoPopup.Type.Normal, UIComposePanel.isTabSlot == false, false);
				GameManager.me.uiManager.popupSummonDetail.spSkipModeBackground.gameObject.SetActive(true);
				break;
			}
		}
		else
		{
			switch(composeResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( composeResultData, RuneInfoPopup.Type.Compose, UIComposePanel.isTabSlot == false, false);
				
				break;
			case GameIDData.Type.Skill:
				RuneStudioMain.instance.spSkillIcon.cachedGameObject.transform.parent.gameObject.SetActive(true);
				GameManager.me.uiManager.popupSkillPreview.show( composeResultData, RuneInfoPopup.Type.Compose, UIComposePanel.isTabSlot == false, false);
				
				break;
				
			case GameIDData.Type.Unit:
				
				GameManager.me.uiManager.popupSummonDetail.show( composeResultData, RuneInfoPopup.Type.Compose, UIComposePanel.isTabSlot == false, false);
				
				break;
			}
			
			composeMovablePanelParent.transform.localScale = new Vector3(1,0,1);
			
			iTween.ScaleTo(composeMovablePanelParent.gameObject, iTween.Hash(
				"scale", new Vector3(1,1,1),
				"delay", 0.0f,
				"time",0.3f,
				"easetype", iTween.EaseType.easeInSine,
				"looptype", iTween.LoopType.none,
				"oncomplete", "iTweenOnCompleteCompose",
				"islocal",true,
				"onCompleteTarget",gameObject
				)); 

		}
	}



	
	
	
	public void iTweenOnCompleteCompose()
	{
//		Debug.LogError("iTweenOnCompleteCompose!");
		
		switch(composeResultData.type)
		{
		case GameIDData.Type.Equip:
			
			foreach(UIPanelRefresher p in GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.panelRefresher)
			{
				p.draw();
			}
			
			break;
		case GameIDData.Type.Skill:
			
			foreach(UIPanelRefresher p in GameManager.me.uiManager.popupSkillPreview.panelRefresher)
			{
				p.draw();
			}
			
			break;
			
		case GameIDData.Type.Unit:
			
			foreach(UIPanelRefresher p in GameManager.me.uiManager.popupSummonDetail.panelRefresher)
			{
				p.draw();
			}
			
			break;
		}
		
		StartCoroutine(onCompleteComposeProcess());
	}
	


	IEnumerator onCompleteComposeProcess()
	{
		yield return new WaitForSeconds(0.5f);
		
		RuneStudioMain.instance.refreshInventory(RuneStudioMain.instance.composeType);
		
		switch(composeType)
		{
		case GameIDData.Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnClose.gameObject.SetActive(true);
//			GameManager.me.uiManager.uiMenu.uiHero.refreshHeroModel();

			if(TutorialManager.nowPlayingTutorial("T46"))
			{
				TutorialManager.uiTutorial.setArrowAndDim(995,544,false);
			}
			break;

		case GameIDData.Type.Skill:
			GameManager.me.uiManager.popupSkillPreview.btnClose.gameObject.SetActive(true);
			GameManager.me.uiManager.popupSkillPreview.reinforcePlayScene();
			
			break;
		case GameIDData.Type.Unit:
			GameManager.me.uiManager.popupSummonDetail.btnClose.gameObject.SetActive(true);
			break;


		}


		step = Step.Finish;
	}


}