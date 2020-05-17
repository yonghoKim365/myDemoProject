using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpecialPackageManager 
{

	public void init()
	{
		canCheckRunePack = false;

		/*
		P_Package[] testPack = new P_Package[4];
		for(int i=0;i<testPack.Length;i++){

			P_Package pack = new P_Package();

			pack.buyCount = i;

			switch(i){
			case 0: 
				pack.showWeight = 0; break;
			case 1: 
				pack.showWeight = 40; break;
			case 2: 
				pack.showWeight = 10; break;
			case 3: 
				pack.showWeight = 0; break;
			}

			testPack[i] = pack;
		}

		Debug.LogError("==before==");
		for(int i=0;i<testPack.Length;i++){
			Debug.Log("pack2["+i+"].buyCnt:"+testPack[i].buyCount+", weight:"+testPack[i].showWeight);
		}

		testPack = sortPackage(testPack);

		Debug.Log("==after==");
		for(int i=0;i<testPack.Length;i++){
			Debug.Log("pack2["+i+"].buyCnt:"+testPack[i].buyCount+", weight:"+testPack[i].showWeight);
		}

		*/
	}


	P_Package starterPack1, starterPack2;
	P_Package specialPack1, specialPack2;
	P_Package[] specialPacks; //, specialPack2, specialPack3, specialPack4; 
	P_Package runePack;


	public bool canCheckRunePack = false;


	public bool check()
	{
		if(GameDataManager.instance.packages == null) return false;

		if(GameManager.me.introStep != Scene.IntroStep.PlayGame) return false;

		if(GameDataManager.instance.serviceMode == GameDataManager.ServiceMode.IOS_SUMMIT || GameManager.me.isGuest)
		{
			return false;
		}

		starterPack1 = null;
		starterPack2 = null;
		specialPack1 = null;
		specialPack2 = null;
		specialPacks = null;

		int numOfSpecialPack = 0;
		foreach(KeyValuePair<string, P_Package> kv in GameDataManager.instance.packages)
		{
			if (kv.Value.category == SpecialPackage.SPECIAL)
			{
				numOfSpecialPack++;
			}
		}

		// for test
		//numOfSpecialPack = 3;
		//

		specialPacks = new P_Package[numOfSpecialPack];

		runePack = null;

		int specialPackCnt = 0;
		//Debug.Log("  num of packages ="+GameDataManager.instance.packages.Count);

		foreach(KeyValuePair<string, P_Package> kv in GameDataManager.instance.packages)
		{
			switch(kv.Value.category)
			{
			case SpecialPackage.STARTER:
				if(starterPack1 == null) starterPack1 = kv.Value;
				else starterPack2 = kv.Value;
				break;
			case SpecialPackage.SPECIAL:
				if (numOfSpecialPack == 2){
					if(specialPack1 == null) specialPack1 = kv.Value;
					else specialPack2 = kv.Value;
				}
				else{
					specialPacks[specialPackCnt] = kv.Value;
					specialPackCnt++;
					// for test
					//specialPacks[2] = kv.Value;
					//specialPacks[3] = kv.Value;
				}

				break;
			case SpecialPackage.RUNE:
				runePack = kv.Value;
				break;
			}
		}

		if (numOfSpecialPack == 4 && canOpen(specialPacks)){
			return true;
		}
		else if(canOpen(runePack, null))
		{
			return true;
		}

		return false;
	}


	public enum Mode
	{
		Shop, World, Lobby, None
	}


	Mode _mode = Mode.None;

	void setMode()
	{
		if(GameManager.me.uiManager.popupShop.gameObject.activeSelf && GameManager.me.uiManager.popupShop.gameObject.activeInHierarchy)
		{
			_mode = Mode.Shop;
		}
		else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.LOBBY && GameManager.me.uiManager.uiMenu.uiLobby.gameObject.activeSelf && GameManager.me.uiManager.uiMenu.uiLobby.gameObject.activeInHierarchy)
		{
			_mode = Mode.Lobby;
		}
		else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.WORLD_MAP && GameManager.me.uiManager.uiMenu.uiWorldMap.gameObject.activeSelf)
		{
			_mode = Mode.World;
		}
	}


	public bool canOpen(P_Package data, P_Package data2 = null)
	{
		if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode ) return false;

		if(data == null) return false;

		setMode();

		bool stop = true;

		for(int i = data.showViews.Length - 1; i >= 0; --i)
		{
			switch(data.showViews[i])
			{
			case "WORLD":
				if(_mode == Mode.World) stop = false;
				break;
			case "LOBBY":
				if(_mode == Mode.Lobby) stop = false;
				break;
			case "SHOP":
				if(_mode == Mode.Shop) stop = false;
				break;
			}
		}

		if(stop) return false;

		int day = System.DateTime.Now.Day;

		string id = data.category;

		bool checkDate = true;


		switch(data.category)
		{
			/*
		case SpecialPackage.STARTER:
		case SpecialPackage.SPECIAL:

			if(data2 == null) return false;

			id += _mode.ToString();

#if UNITY_EDITOR
			//checkDate = false;
#endif
			if(checkDate && PlayerPrefs.GetInt(id, -1) == day) return false;


			GameManager.me.uiManager.popupSpecialPack.setData(data, data2);
			break;
*/
		case SpecialPackage.RUNE:

#if UNITY_EDITOR
			checkDate = false;
#endif
			id += data.subcategory;

			if(checkDate && PlayerPrefs.GetInt(id, -1) == day) return false;

			if(canCheckRunePack == false) return false;

			canCheckRunePack = false;

			GameManager.me.uiManager.popupSpecialSinglePack.setData(data);//, data2);
			break;
		}

		PlayerPrefs.SetInt(id, day);

		return true;
	}

	public bool canOpen(P_Package[] data)
	{
		if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode ) return false;
		
		if(data == null) return false;
		if (data[0] == null)return false;

		P_Package data1 = data[0];
		
		setMode();
		
		bool stop = true;
		
		for(int i = data1.showViews.Length - 1; i >= 0; --i)
		{
			switch(data1.showViews[i])
			{
			case "WORLD":
				if(_mode == Mode.World) stop = false;
				break;
			case "LOBBY":
				if(_mode == Mode.Lobby) stop = false;
				break;
			case "SHOP":
				if(_mode == Mode.Shop) stop = false;
				break;
			}
		}
		
		if(stop) return false;
		
		int day = System.DateTime.Now.Day;
		
		string id = data1.category;
		
		bool checkDate = true;
		
		
		switch(data1.category)
		{
		//case SpecialPackage.STARTER:
		case SpecialPackage.SPECIAL:
			
			if(data[1] == null) return false;
			
			id += _mode.ToString();
			
			#if UNITY_EDITOR
			// for test
			//checkDate = false;
			#endif
			if(checkDate && PlayerPrefs.GetInt(id, -1) == day) return false;

			data = sortPackage(data);
			GameManager.me.uiManager.popupSpecialPack.setData(data);
			break;
			/*
		case SpecialPackage.RUNE:
		
			#if UNITY_EDITOR
			//			checkDate = false;
			#endif
			id += data.subcategory;
			
			if(checkDate && PlayerPrefs.GetInt(id, -1) == day) return false;
			
			if(canCheckRunePack == false) return false;
			
			canCheckRunePack = false;
			
			GameManager.me.uiManager.popupSpecialSinglePack.setData(data);
			break;
			*/
		}
		
		PlayerPrefs.SetInt(id, day);
		
		return true;
	}


	P_Package[] sortPackage(P_Package[] srcPackages){

		P_Package[] resultPackage = new P_Package[srcPackages.Length];

		for(int k=0;k<resultPackage.Length;k++){
			int maxValue = 0;
			int maxIndex = 0;
			for(int i=0;i<srcPackages.Length;i++){
				if (srcPackages[i] != null){
					if (srcPackages[i].showWeight >= maxValue){
						maxValue = srcPackages[i].showWeight;
						maxIndex = i;
					}
				}
			}

			resultPackage[k] = srcPackages[maxIndex];
			srcPackages[maxIndex] = null;
		}

		return resultPackage;
	}

}



public class SpecialPackage
{
	public const string RUNE = "RUNE";
	public const string SPECIAL = "SPECIAL";
	public const string STARTER = "STARTER";


	public const string UNIT = "UNIT";
	public const string SKILL = "SKILL";
	public const string EQUIP = "EQUIP";

}
