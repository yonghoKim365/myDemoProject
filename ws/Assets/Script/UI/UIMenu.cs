using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMenu : UIBase {
	

	public const int LOBBY = 0;
	public const int HERO = 1;
	public const int SUMMON = 2;
	public const int SKILL = 3;
	public const int WORLD_MAP = 4;
	public const int FRIEND = 5;
	public const int MISSION = 6;

	public int currentPanel = LOBBY;

	public UIBase[] uiPanels;

	public GameObject mainPanel;
	public GameObject nextPanel;

	public const int ANI_TO_LEFT = 0;
	public const int ANI_TO_RIGHT = 1;
	
	private Vector3 _oldPanelPos = new Vector3();
	private Vector3 _newPanelPos = new Vector3();
	private Vector3 _v;

	public static UIMenu instance;
	
	public UIWorldMap uiWorldMap;
	public UILobby uiLobby;
	public UISummon uiSummon;
	public UISkill uiSkill;
	public UIHero uiHero;

	public UIFriend uiFriend;
	public UIMission uiMission;

	public GameObject secondMenuLayer;


	public enum UIPosition
	{
		Lobby, Hero, Summon, Skill, Mission, Friend, Shop, WorldMap, Message, Option, None
	}

	// 특정 상황에서 강제 이동시킬 위치.
	public UIPosition directGoIndex = UIPosition.None;

	void Awake()
	{
		instance = this;
	}
	
	void OnDestroy()
	{
		instance = null;
	}

	public override void show ()
	{
		secondMenuLayer.SetActive(true);
		gameObject.SetActive( true );
		mainPanel.SetActive(true);
		showPanel(currentPanel);
	}


	public override void hide ()
	{
		base.hide ();
		secondMenuLayer.SetActive(false);
	}


	public void showPanel(int index)
	{
		for(int i = 0; i < uiPanels.Length; ++i)
		{
			if(uiPanels[i] == null) continue;
			
			if(index == i)
			{
//				SoundData.play("uicm_page");
				uiPanels[i].show();
				currentPanel = i;
			}
			else
			{
				uiPanels[i].hide();
			}
		}
	}




	public void changePanel(int nextPanelIndex)
	{
//		SoundData.play("uicm_page");
		uiPanels[currentPanel].hide();
		currentPanel = nextPanelIndex;
		uiPanels[nextPanelIndex].show();
	}



	
	public void panelAnimation(int direction, int nextPanelIndex, float duration = 0.2f)
	{
		if(direction == ANI_TO_LEFT)
		{
			_oldPanelPos.z = uiPanels[currentPanel].transform.position.z;
			_newPanelPos.z = uiPanels[nextPanelIndex].transform.position.z;
			
			_oldPanelPos.x = -1196;							
			_newPanelPos.x = 0;
			_v = uiPanels[nextPanelIndex].transform.localPosition;
			_v.x = 1196;
			uiPanels[nextPanelIndex].transform.localPosition = _v;			
		}
		else
		{
			_oldPanelPos.z = uiPanels[currentPanel].transform.localPosition.z;
			_newPanelPos.z = uiPanels[nextPanelIndex].transform.localPosition.z;
			
			_oldPanelPos.x = 1196;	
			_newPanelPos.x = 0;
			_v = uiPanels[nextPanelIndex].transform.localPosition;
			_v.x = -1196;
			uiPanels[nextPanelIndex].transform.localPosition = _v;			
		}
		
		TweenPosition tp = TweenPosition.Begin(uiPanels[currentPanel].gameObject, duration, _oldPanelPos);
		tp.eventReceiver = uiPanels[currentPanel].gameObject;
		tp.callWhenFinished = "hide";
		tp.method = UITweener.Method.EaseIn;		
		TweenPosition.Begin(uiPanels[nextPanelIndex].gameObject, duration, _newPanelPos).method = UITweener.Method.EaseIn;

		currentPanel = nextPanelIndex;
	}




	public bool rayCast(Camera cam, GameObject go)
	{
		if(cam == null) return false;

		if(UICamera.Raycast(cam.WorldToScreenPoint(go.transform.position)) == false) return false;

		if(UICamera.hoveredObject == null) return false; 

#if UNITY_EDITOR
		Debug.Log(UICamera.hoveredObject);
#endif

		return (UICamera.hoveredObject == go);

	}


}
