using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWorldMapRoad : MonoBehaviour {

	public UISprite[] spAct1Roads = new UISprite[5];
	public UISprite[] spAct2Roads = new UISprite[5];
	public UISprite[] spAct3Roads = new UISprite[5];
	public UISprite[] spAct4Roads = new UISprite[5];
	public UISprite[] spAct5Roads = new UISprite[5];

	public UISprite[] spAct6Roads = new UISprite[5];
	public UISprite[] spAct7Roads = new UISprite[5];
	public UISprite[] spAct8Roads = new UISprite[5];
	public UISprite[] spAct9Roads = new UISprite[5];
	public UISprite[] spAct10Roads = new UISprite[5];


	public UIWorldMapCheckPointButton[] cpAct1 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct2 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct3 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct4 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct5 = new UIWorldMapCheckPointButton[5];

	public UIWorldMapCheckPointButton[] cpAct6 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct7 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct8 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct9 = new UIWorldMapCheckPointButton[5];
	public UIWorldMapCheckPointButton[] cpAct10 = new UIWorldMapCheckPointButton[5];



	UISprite[] current = null;
	UIWorldMapCheckPointButton[] currentCp = null;

	public UISprite[] spActLockCover = new UISprite[4];

	public UISprite[] spActLock = new UISprite[4];

	public GameObject[] actParticle = new GameObject[5];

	void Awake()
	{
		for(int i = 0; i < cpAct1.Length; ++i)
		{
			cpAct1[i].act = 1;
			cpAct1[i].stage = i + 1;
		}

		for(int i = 0; i < cpAct2.Length; ++i)
		{
			cpAct2[i].act = 2;
			cpAct2[i].stage = i + 1;
		}

		for(int i = 0; i < cpAct3.Length; ++i)
		{
			cpAct3[i].act = 3;
			cpAct3[i].stage = i + 1;
		}

		for(int i = 0; i < cpAct4.Length; ++i)
		{
			cpAct4[i].act = 4;
			cpAct4[i].stage = i + 1;
		}

		for(int i = 0; i < cpAct5.Length; ++i)
		{
			cpAct5[i].act = 5;
			cpAct5[i].stage = i + 1;
		}

		for(int i = 0; i < cpAct6.Length; ++i)
		{
			cpAct6[i].act = 6;
			cpAct6[i].stage = i + 1;
		}
	}


	public enum RefreshType
	{
		Refresh, StageOpen, ActOpen
	}


	public void refresh(int openAct, int openStage, RefreshType refreshType)
	{

#if UNITY_EDITOR

		if(DebugManager.instance.useDebug)
		{
			GameDataManager.instance.maxAct = 1;
			GameDataManager.instance.maxStage = 1;
		}
#endif

		for(int i = 0; i < GameManager.MAX_ACT - 1; ++i)
		{
			spActLockCover[i].enabled = (openAct < i+2);

			if(openAct > GameManager.MAX_ACT && i >= openAct - 2)
			{
				spActLock[i].gameObject.SetActive( true );
			}
			else
			{
				spActLock[i].gameObject.SetActive( (openAct < i+2) );
			}
		}

		for(int i = 0 ; i < GameManager.MAX_ACT - 1; ++i)
		{
			if(i >= actParticle.Length)
			{
				break;
			}
			else
			{


				actParticle[i].gameObject.SetActive(!spActLockCover[i].enabled);
			}
		}


		int roadAct = openAct;
		int openButtonIndex = openStage;
		int openRoadIndex = openStage;

		switch(refreshType)
		{
		case RefreshType.ActOpen:
			roadAct = GameDataManager.instance.maxActWithCheckingMaxAct;
			openButtonIndex = 1;
			openRoadIndex= 1;
			break;
		case RefreshType.StageOpen:
			roadAct = GameDataManager.instance.maxActWithCheckingMaxAct;
			openRoadIndex = GameDataManager.instance.maxStage;
			break;
		}

		switch(roadAct)
		{
		case 1:
			setLockLoads(spAct2Roads, true);
			setLockLoads(spAct3Roads, true);
			setLockLoads(spAct4Roads, true);
			setLockLoads(spAct5Roads, true);
			setLockLoads(spAct6Roads, true);
			current = spAct1Roads;
			break;
		case 2:
			setLockLoads(spAct1Roads, false);
			setLockLoads(spAct3Roads, true);
			setLockLoads(spAct4Roads, true);
			setLockLoads(spAct5Roads, true);
			setLockLoads(spAct6Roads, true);
			current = spAct2Roads;
			break;
		case 3:
			setLockLoads(spAct1Roads, false);
			setLockLoads(spAct2Roads, false);
			setLockLoads(spAct4Roads, true);
			setLockLoads(spAct5Roads, true);
			setLockLoads(spAct6Roads, true);
			current = spAct3Roads;
			break;
		case 4:
			setLockLoads(spAct1Roads, false);
			setLockLoads(spAct2Roads, false);
			setLockLoads(spAct3Roads, false);
			setLockLoads(spAct5Roads, true);
			setLockLoads(spAct6Roads, true);
			current = spAct4Roads;
			break;
		case 5:
			setLockLoads(spAct1Roads, false);
			setLockLoads(spAct2Roads, false);
			setLockLoads(spAct3Roads, false);
			setLockLoads(spAct4Roads, false);
			setLockLoads(spAct6Roads, true);
			current = spAct5Roads;
			break;
		case 6:
			setLockLoads(spAct1Roads, false);
			setLockLoads(spAct2Roads, false);
			setLockLoads(spAct3Roads, false);
			setLockLoads(spAct4Roads, false);
			setLockLoads(spAct5Roads, false);
			current = spAct6Roads;
			break;
		}


		switch(openAct)
		{
		case 1:
			setMapButtons(cpAct2, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct3, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct4, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct5, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct6, UIWorldMapCheckPointButton.Status.Lock);
			currentCp = cpAct1;
			break;
		case 2:
			setMapButtons(cpAct1, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct3, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct4, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct5, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct6, UIWorldMapCheckPointButton.Status.Lock);
			currentCp = cpAct2;
			break;
		case 3:
			setMapButtons(cpAct1, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct2, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct4, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct5, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct6, UIWorldMapCheckPointButton.Status.Lock);
			currentCp = cpAct3;
			break;
		case 4:
			setMapButtons(cpAct1, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct2, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct3, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct5, UIWorldMapCheckPointButton.Status.Lock);
			setMapButtons(cpAct6, UIWorldMapCheckPointButton.Status.Lock);
			currentCp = cpAct4;
			break;
		case 5:
			setMapButtons(cpAct1, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct2, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct3, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct4, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct6, UIWorldMapCheckPointButton.Status.Lock);
			currentCp = cpAct5;
			break;
		case 6:
			setMapButtons(cpAct1, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct2, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct3, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct4, UIWorldMapCheckPointButton.Status.Clear);
			setMapButtons(cpAct5, UIWorldMapCheckPointButton.Status.Clear);
			currentCp = cpAct6;
			break;
		}

		

		if(openAct <= GameManager.MAX_ACT)
		{
			if(current == null) return;
			
			if(currentCp != null)
			{
				for(int i = 1; i <= 5 ; ++i)
				{
					if(i < openButtonIndex)
					{
						if(currentCp.Length > i-1) setMapButton(currentCp[i-1], UIWorldMapCheckPointButton.Status.Clear);
					}
					else if(i == openButtonIndex)
					{
						if(currentCp.Length > i-1) setMapButton(currentCp[i-1], UIWorldMapCheckPointButton.Status.Current);
					}
					else
					{
						if(currentCp.Length > i-1) setMapButton(currentCp[i-1], UIWorldMapCheckPointButton.Status.Lock);
					}
				}
				
				
				if(refreshType == RefreshType.ActOpen)
				{
					for(int i = 0; i < 4; ++i)
					{
						if(currentCp.Length > i) setMapButton(currentCp[i], UIWorldMapCheckPointButton.Status.Clear);
					}
				}
				
			}

			Debug.Log(" openRoadIndex :"+openRoadIndex);

			for(int i = 1; i <= 5; ++i)
			{
				if(i < openRoadIndex)
				{
					if(current.Length > i-1) setLockLoad(current[i-1],false);
				}
				else if(i == openRoadIndex)
				{
					if(current.Length > i-1) setLockLoad(current[i-1],true); // bug
					//if(current.Length > i-1) setLockLoad(current[i-1], false);
				}
				else
				{
					if(current.Length > i-1) setLockLoad(current[i-1],true);
				}
			}
		}


		GameManager.me.uiManager.uiMenu.uiWorldMap.mapEffectManager.resetPlayMaker();

		GameManager.me.uiManager.uiMenu.uiWorldMap.mapEffectManager.refreshPosition(openAct, openStage);
	}

	Color normalColor = new Color(1,1,1);
	Color lockColor = new Color(0.098f,0.145f,0.784f);

	void setLockLoad(UISprite sp, bool isLock)
	{
		if(sp == null) return;

		if(isLock)
		{
			if(sp.spriteName.Contains("_lock") == false) sp.spriteName = sp.spriteName + "_lock";
		}
		else
		{
			if(sp.spriteName.Contains("_lock")) sp.spriteName = sp.spriteName.Replace("_lock","");
		}
	}

	void setLockLoads(UISprite[] sprites, bool isLock)
	{
		foreach(UISprite sp in sprites)
		{
			setLockLoad(sp,isLock);
		}
	}


	void setMapButton(UIWorldMapCheckPointButton cp, UIWorldMapCheckPointButton.Status status)
	{
		cp.status = status;
	}
	
	void setMapButtons(UIWorldMapCheckPointButton[] cps, UIWorldMapCheckPointButton.Status status)
	{
		foreach(UIWorldMapCheckPointButton cp in cps)
		{
			cp.status = status;
		}
	}




}
