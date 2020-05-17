using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RuneStudioMain : MonoBehaviour 
{
	
	public string transcendResultId;
	public GameIDData.Type transcendType;
	public GameIDData transcendResultData = new GameIDData();
	public GameIDData transcendOriginalData = new GameIDData();

	public static bool transcendFromTabSlot = false;

	public void playTranscendResult(string newId, string sourceId, GameIDData.Type cType)
	{
		GameManager.me.uiManager.uiMenu.uiHero.selectedSlot = null;
		GameManager.me.uiManager.uiMenu.uiSkill.selectSlot = null;
		GameManager.me.uiManager.uiMenu.uiSummon.selectSlot = null;
		
		StartCoroutine( playTranscendResultCT(newId, sourceId, cType) );
	}
	
	IEnumerator playTranscendResultCT(string newId, string sourceId, GameIDData.Type cType)
	{
		reinforceSlotContainer.SetActive(false);
		RuneStudioMain.instance.rootTranscend.gameObject.SetActive(true);

		transcendMovablePanelParent.parent.transform.localScale = new Vector3(1,1,1);
		transcendMovablePanelParent.transform.localScale = new Vector3(1,1,1);
		
		type = Type.Transcend;
		
		transcendType = cType;

		transcendResultId = newId;
		transcendResultData.parse(newId, cType);

		transcendOriginalData.parse(sourceId);


		reinforceSlotContainer.SetActive(true);
		
		for(int i = 0; i < 5; ++i)
		{
			Renderer ren;
			
			if(i < 2)
			{
				GameIDData temp = new GameIDData();

				if(i == 0) temp.parse(sourceId, cType);
				else if(i == 1)
				{
					if(string.IsNullOrEmpty(UIRuneReforegePopup.currentIngredientId) == false)
					{
						temp.parse(UIRuneReforegePopup.currentIngredientId, cType);
					}
					else
					{
						temp.parse(sourceId, cType);
					}
				}

				//if(i == 0) 
				{
					switch(temp.type)
					{
					case GameIDData.Type.Equip:
						reinforceSlots[i].setData(UIChallengeItemSlot.Type.Equip,temp);
						break;
					case GameIDData.Type.Unit:
						reinforceSlots[i].setData(UIChallengeItemSlot.Type.Unit,temp);
						break;
					case GameIDData.Type.Skill:
						reinforceSlots[i].setData(UIChallengeItemSlot.Type.Skill,temp);
						break;
					}
				}



//				else if(i == 1)
//				{
//					reinforceSlots[i].setData(WSDefine.RUNESTONE+"_1");
//					reinforceSlots[i].showLevelBar = false;
//				}

				reinforceRenderingSlot[i].enabled = true;
			}
			else
			{
				ren = reinforceRenderingSlot[i].GetComponent<Renderer>();
				reinforceRenderingSlot[i].enabled = false;
			}
		}

		if(cType == GameIDData.Type.Unit)
		{
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;
			
			GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[transcendResultData.unitData.resource]);
			GameDataManager.instance.startModelLoad();
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;
			
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}
		else if(cType == GameIDData.Type.Equip)
		{
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
		}

		cam256.gameObject.SetActive(true);
		
		sendEvent(transcendStarter);
		
		if(TutorialManager.instance.isTutorialMode == false)
		{
			GameManager.me.uiManager.activeRuneStudioSkipButton();
			step = Step.Start;
		}
	}
	
	
	
	
	public void showTranscendResultCard(bool isSkipMode = false)
	{
		transcendStartDoor.gameObject.SetActive(false);
		GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);
		reinforceSlotContainer.SetActive(false);
		GameManager.me.uiManager.popupReforege.step = 2;

		if(isSkipMode)
		{
			refreshInventory(transcendResultData.type);
			
			endProcess();
			
			switch(transcendResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( transcendResultData, RuneInfoPopup.Type.Normal, UIRuneReforegePopup.fromTabSlot == false, false, null, transcendOriginalData);
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.spSkipModeBackground.gameObject.SetActive(true);
				break;
			case GameIDData.Type.Skill:
				GameManager.me.uiManager.popupSkillPreview.show( transcendResultData, RuneInfoPopup.Type.Normal, UIRuneReforegePopup.fromTabSlot == false, false, null, transcendOriginalData);
				GameManager.me.uiManager.popupSkillPreview.spSkipModeBackground.gameObject.SetActive(true);
				break;
				
			case GameIDData.Type.Unit:				
				GameManager.me.uiManager.popupSummonDetail.show( transcendResultData, RuneInfoPopup.Type.Normal, UIRuneReforegePopup.fromTabSlot == false, false, transcendOriginalData);
				GameManager.me.uiManager.popupSummonDetail.spSkipModeBackground.gameObject.SetActive(true);
				break;
			}
		}
		else
		{
			switch(transcendResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( transcendResultData, RuneInfoPopup.Type.Transcend, UIRuneReforegePopup.fromTabSlot == false, false, null, transcendOriginalData);
				break;
			case GameIDData.Type.Skill:
				RuneStudioMain.instance.spSkillIcon.cachedGameObject.transform.parent.gameObject.SetActive(true);
				GameManager.me.uiManager.popupSkillPreview.show( transcendResultData, RuneInfoPopup.Type.Transcend, UIRuneReforegePopup.fromTabSlot == false, false, null, transcendOriginalData);
				break;
				
			case GameIDData.Type.Unit:
				GameManager.me.uiManager.popupSummonDetail.show( transcendResultData, RuneInfoPopup.Type.Transcend, UIRuneReforegePopup.fromTabSlot == false, false, transcendOriginalData);
				break;
			}
			
			transcendMovablePanelParent.transform.localScale = new Vector3(1,0,1);
			
			iTween.ScaleTo(transcendMovablePanelParent.gameObject, iTween.Hash(
				"scale", new Vector3(1,1,1),
				"delay", 0.0f,
				"time",0.3f,
				"easetype", iTween.EaseType.easeInSine,
				"looptype", iTween.LoopType.none,
				"oncomplete", "iTweenOnCompletetranscend",
				"islocal",true,
				"onCompleteTarget",gameObject
				)); 
			
		}
	}
	
	
	
	
	
	
	public void iTweenOnCompletetranscend()
	{
		//		Debug.LogError("iTweenOnCompletetranscend!");
		
		switch(transcendResultData.type)
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
		
		StartCoroutine(onCompleteTranscendProcess());
	}
	
	
	
	IEnumerator onCompleteTranscendProcess()
	{
		yield return new WaitForSeconds(0.5f);
		
		RuneStudioMain.instance.refreshInventory(RuneStudioMain.instance.transcendType);
		
		switch(transcendType)
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