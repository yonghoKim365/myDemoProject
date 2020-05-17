using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RuneStudioMain : MonoBehaviour 
{
	public string evolveResultId;
	public GameIDData.Type evolveType;
	public GameIDData evolveResultData = new GameIDData();

	public StudioCardBgSlot evolveSourceBgSlot;

	public void playEvolveResult(string newId, string sourceId, GameIDData.Type cType)
	{
		GameManager.me.uiManager.uiMenu.uiHero.selectedSlot = null;
		GameManager.me.uiManager.uiMenu.uiSkill.selectSlot = null;
		GameManager.me.uiManager.uiMenu.uiSummon.selectSlot = null;
		
		StartCoroutine( playEvolveResultCT(newId, sourceId, cType) );
	}
	
	IEnumerator playEvolveResultCT(string newId, string sourceId, GameIDData.Type cType)
	{
		evolutionRuneRenderer.sharedMaterial.color = new Color (0.5f, 0.5f, 0.5f);

		reinforceSlotContainer.SetActive(false);

		type = Type.Evolve;
		
		evolveType = cType;

		evolveResultId = newId;
		evolveResultData.parse(newId, cType);

		switch(evolveResultData.rare)
		{
		case RareType.SS:
			rootEvolve[4].gameObject.SetActive(true);
			evolutionCardFrame[4].setData(sourceId, cType);
			evolveMovablePanelParent[4].transform.localScale = new Vector3(1,1,1);
			showCard512Studio(RareType.S);
			break;
		case RareType.S:
			rootEvolve[3].gameObject.SetActive(true);
			evolutionCardFrame[3].setData(sourceId, cType);
			evolveMovablePanelParent[3].transform.localScale = new Vector3(1,1,1);
			showCard512Studio(RareType.A);
			break;
		case RareType.A:
			rootEvolve[2].gameObject.SetActive(true);
			evolutionCardFrame[2].setData(sourceId, cType);
			evolveMovablePanelParent[2].transform.localScale = new Vector3(1,1,1);
			showCard512Studio(RareType.B);
			break;
		case RareType.B:
			rootEvolve[1].gameObject.SetActive(true);
			evolutionCardFrame[1].setData(sourceId, cType);
			evolveMovablePanelParent[1].transform.localScale = new Vector3(1,1,1);
			showCard512Studio(RareType.C);
			break;
		default:
			rootEvolve[0].gameObject.SetActive(true);
			evolutionCardFrame[0].setData(sourceId, cType);
			evolveMovablePanelParent[0].transform.localScale = new Vector3(1,1,1);
			showCard512Studio(RareType.D);
			break;
		}

		if(cType == GameIDData.Type.Unit)
		{
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;
			
			switch(evolveResultData.rare)
			{
			case RareType.SS:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[evolutionCardFrame[4].data.unitData.resource]);
				break;
			case RareType.S:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[evolutionCardFrame[3].data.unitData.resource]);
				break;
			case RareType.A:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[evolutionCardFrame[2].data.unitData.resource]);
				break;
			case RareType.B:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[evolutionCardFrame[1].data.unitData.resource]);
				break;
			default:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[evolutionCardFrame[0].data.unitData.resource]);
				break;
			}





			GameDataManager.instance.startModelLoad();
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;
			
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}
		else if(cType == GameIDData.Type.Equip)
		{
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}

		evolveSourceBgSlot.gameObject.SetActive(true);
		evolveSourceBgSlot.customSizeRatio = 3.2f;




		cam512.gameObject.SetActive(true);

		switch(evolveResultData.rare)
		{
		case RareType.SS:
			evolveSourceBgSlot.setData(evolutionCardFrame[4].data);
			sendEvent(evolveController[4]);
			break;
		case RareType.S:
			evolveSourceBgSlot.setData(evolutionCardFrame[3].data);
			sendEvent(evolveController[3]);
			break;
		case RareType.A:
			evolveSourceBgSlot.setData(evolutionCardFrame[2].data);
			sendEvent(evolveController[2]);
			break;
		case RareType.B:
			evolveSourceBgSlot.setData(evolutionCardFrame[1].data);
			sendEvent(evolveController[1]);
			break;
		default:
			evolveSourceBgSlot.setData(evolutionCardFrame[0].data);
			sendEvent(evolveController[0]);
			break;
		}



		if(TutorialManager.instance.isTutorialMode == false)
		{
			yield return new WaitForSeconds(0.4f);
			
			step = Step.Start;
			GameManager.me.uiManager.activeRuneStudioSkipButton();
		}
	}
	
	
	public static bool evolutionRuneFromInventory = false;

	
	public void showEvolveResultCard(bool isSkipMode = false)
	{
		GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);

		evolveSourceBgSlot.hide();
		evolveSourceBgSlot.gameObject.SetActive(false);

		if(isSkipMode)
		{
			refreshInventory(evolveResultData.type);
			
//			GameManager.me.uiManager.uiMenu.uiHero.refreshHeroModel();
			
			endProcess();
			
			switch(evolveResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( evolveResultData, RuneInfoPopup.Type.Normal, evolutionRuneFromInventory, false);
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.spSkipModeBackground.gameObject.SetActive(true);
				break;
			case GameIDData.Type.Skill:
				GameManager.me.uiManager.popupSkillPreview.show( evolveResultData, RuneInfoPopup.Type.Normal, evolutionRuneFromInventory, false);
				GameManager.me.uiManager.popupSkillPreview.spSkipModeBackground.gameObject.SetActive(true);
				break;
				
			case GameIDData.Type.Unit:				
				GameManager.me.uiManager.popupSummonDetail.show( evolveResultData, RuneInfoPopup.Type.Normal, evolutionRuneFromInventory, false);
				GameManager.me.uiManager.popupSummonDetail.spSkipModeBackground.gameObject.SetActive(true);
				break;
			}
		}
		else
		{
			switch(evolveResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( evolveResultData, RuneInfoPopup.Type.Evolve, evolutionRuneFromInventory, false);
				
				break;
			case GameIDData.Type.Skill:
				RuneStudioMain.instance.spSkillIcon.cachedGameObject.transform.parent.gameObject.SetActive(true);
				GameManager.me.uiManager.popupSkillPreview.show( evolveResultData, RuneInfoPopup.Type.Evolve, evolutionRuneFromInventory, false);
				
				break;
				
			case GameIDData.Type.Unit:
				
				GameManager.me.uiManager.popupSummonDetail.show( evolveResultData, RuneInfoPopup.Type.Evolve, evolutionRuneFromInventory, false);
				
				break;
			}
			
//			evolveMovablePanelParent.transform.localScale = new Vector3(1,0,1);

			/*
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
				*/
			
		}
	}
	
	
	
	
	
	
	public void iTweenOnCompleteEvolve()
	{
#if UNITY_EDITOR
		Debug.LogError("iTweenOnCompleteEvolve!");
#endif
		
		switch(evolveResultData.type)
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
		
		StartCoroutine(onCompleteEvolveProcess());
	}

	
	IEnumerator onCompleteEvolveProcess()
	{
		yield return new WaitForSeconds(0.5f);

		refreshInventory(evolveType);

		switch(evolveType)
		{
		case GameIDData.Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnClose.gameObject.SetActive(true);
			
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