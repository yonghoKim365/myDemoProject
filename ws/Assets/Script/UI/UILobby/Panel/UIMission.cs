using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIMission : UIBase {

	public UILabel lbRuby, lbGold;

	public UIButton tabMain,tabSub,tabEvent,tabPlay;
	public UISprite spTabMain, spTabSub, spTabEvent, spTabPlay;
	public UISprite spTabMainFont, spTabSubFont, spTabEventFont, spTabPlayFont;

	public UIButton btnBuyRuby, btnBuyGold;

	public UIMissionList list;

	public UISprite spMainNew, spSubNew, spEventNew, spPlayNew; 

	public UISprite spMainReward, spSubReward, spEventReward, spPlayReward;


	void Awake()
	{
		setBackButton(UIMenu.LOBBY);

		UIEventListener.Get(tabMain.gameObject).onClick = onClickTabMain;
		UIEventListener.Get(tabSub.gameObject).onClick = onClickTabSub;
		UIEventListener.Get(tabEvent.gameObject).onClick = onClickTabEvent;
		UIEventListener.Get(tabPlay.gameObject).onClick = onClickTabPlay;

		
		UIEventListener.Get(btnBuyGold.gameObject).onClick = onBuyGold;
		UIEventListener.Get(btnBuyRuby.gameObject).onClick = onBuyRuby;
		
		
		GameDataManager.goldDispatcher -= updateGold;
		GameDataManager.goldDispatcher += updateGold;
		
		GameDataManager.rubyDispatcher -= updateRuby;
		GameDataManager.rubyDispatcher += updateRuby;
	}


	public override void hide ()
	{
		base.hide ();
		saveLocalData();
	}


	public enum Type
	{
		Main, Sub, Event, Play, None
	}

	public static Type type = Type.Main;

	public static Type prevType = Type.None;

	void onClickTabMain(GameObject go)
	{
		if(type == Type.Main) return;
		prevType = type;
		type = Type.Main;
		refresh();
	}
	
	void onClickTabSub(GameObject go)
	{
		if(TutorialManager.instance.isTutorialMode) return;

		if(type == Type.Sub) return;
		prevType = type;
		type = Type.Sub;

		refresh();
	}

	void onClickTabEvent(GameObject go)
	{
		if(TutorialManager.instance.isTutorialMode) return;

		if(type == Type.Event) return;
		prevType = type;
		type = Type.Event;
		refresh();
	}

	void onClickTabPlay(GameObject go)
	{
		if(TutorialManager.instance.isTutorialMode) return;
		
		if(type == Type.Play) return;
		prevType = type;
		type = Type.Play;
		refresh();
	}


	public static int mainNewNum = 0;
	public static int subNewNum = 0;
	public static int eventNewNum = 0;
	public static int playMissionNewNum = 0;

	public static int mainRewaradNum = 0;
	public static int subRewardNum = 0;
	public static int eventRewardNum = 0;
	public static int playMissonRewardNum = 0;

	public static Dictionary<string, bool> checkNewDic = new Dictionary<string, bool>();

	void checkNew()
	{
		mainNewNum = 0;
		subNewNum = 0;
		eventNewNum = 0;
		playMissionNewNum = 0;

		mainRewaradNum = 0;
		subRewardNum = 0;
		eventRewardNum = 0;
		playMissonRewardNum = 0;


		if(GameDataManager.instance.missions != null)
		{
			foreach(KeyValuePair<string, P_Mission > kv in GameDataManager.instance.missions)
			{
				if(kv.Value.state == WSDefine.MISSION_CLOSE) continue;

				switch(kv.Value.kind)
				{
				case WSDefine.MISSION_KIND_MAIN:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++mainNewNum;
					else if(prevType == Type.Main) checkNewDic[kv.Value.id] = true;

					if(kv.Value.state == WSDefine.CLEAR) ++mainRewaradNum;

					break;
				case WSDefine.MISSION_KIND_SUB:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++subNewNum;
					else if(prevType == Type.Sub) checkNewDic[kv.Value.id] = true;

					if(kv.Value.state == WSDefine.CLEAR) ++subRewardNum;

					break;
				case WSDefine.MISSION_KIND_EVENT:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++eventNewNum;
					else if(prevType == Type.Event) checkNewDic[kv.Value.id] = true;

					if(kv.Value.state == WSDefine.CLEAR) ++eventRewardNum;

					break;
				case WSDefine.MISSION_KIND_PLAY:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) ++playMissionNewNum;
					else if(prevType == Type.Play) checkNewDic[kv.Value.id] = true;
					
					if(kv.Value.state == WSDefine.CLEAR) ++playMissonRewardNum;
					
					break;
				}
			}
		}

		spMainNew.enabled = (mainNewNum > 0);
		spSubNew.enabled = (subNewNum > 0);
		spEventNew.enabled = (eventNewNum > 0);
		spPlayNew.enabled = (playMissionNewNum > 0);

		spMainReward.enabled = (mainRewaradNum > 0);
		spSubReward.enabled = (subRewardNum > 0 );
		spEventReward.enabled = (eventRewardNum > 0);
		spPlayReward.enabled = (playMissonRewardNum > 0);

	}


	public static bool checkHasNewMission()
	{
		if(GameDataManager.instance.missions != null && checkNewDic != null)
		{
			foreach(KeyValuePair<string, P_Mission > kv in GameDataManager.instance.missions)
			{
				if(kv.Value.state == WSDefine.MISSION_CLOSE) continue;
				
				switch(kv.Value.kind)
				{
				case WSDefine.MISSION_KIND_MAIN:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) return true;
					break;

				case WSDefine.MISSION_KIND_SUB:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) return true;
					break;

				case WSDefine.MISSION_KIND_EVENT:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) return true;

					break;
				case WSDefine.MISSION_KIND_PLAY:
					if(checkNewDic.ContainsKey(kv.Value.id) == false) return true;
					
					break;
				}
			}
		}

		return false;
	}


	private static StringBuilder _sb = new StringBuilder();
	public static void saveLocalData()
	{
		_sb.Length = 0;

		foreach(KeyValuePair<string, bool> kv in checkNewDic)
		{
			if(kv.Value)
			{
				_sb.Append(kv.Key);
				_sb.Append(",");
			}
		}

		PlayerPrefs.SetString("MISSION", _sb.ToString());

		_sb.Length = 0;
	}

	public static void loadLocalData()
	{
		checkNewDic.Clear();

		string str = PlayerPrefs.GetString("MISSION");
		if(str != null)
		{
			string[] s = str.Split(',');

			foreach(string mKey in s)
			{
				if(string.IsNullOrEmpty(mKey) == false)
				{
					checkNewDic.Add(mKey, true);
				}
			}
		}
	}



	public void refresh()
	{
		if(TutorialManager.nowPlayingTutorial("T13") || TutorialManager.instance.clearCheck("T13") == false)
		{
			type = Type.Main;
		}


		switch(type)
		{
		case Type.Main:
			spTabMain.spriteName = "ibtn_tab_onidle";
			spTabSub.spriteName = "ibtn_tab_offidle";
			spTabEvent.spriteName = "ibtn_tab_offidle";
			spTabPlay.spriteName = "ibtn_tab_offidle";

			spTabMainFont.spriteName = "ibtn_tab_mainmisson_onidle";
			spTabSubFont.spriteName = "ibtn_tab_submisson_offidle";
			spTabEventFont.spriteName = "ibtn_tab_event_offidle";
			spTabPlayFont.spriteName = "ibtn_tab_playmisson_offidle";

			break;
		case Type.Sub:
			spTabMain.spriteName = "ibtn_tab_offidle";
			spTabSub.spriteName = "ibtn_tab_onidle";
			spTabEvent.spriteName = "ibtn_tab_offidle";
			spTabPlay.spriteName = "ibtn_tab_offidle";

			spTabMainFont.spriteName = "ibtn_tab_mainmisson_offidle";
			spTabSubFont.spriteName = "ibtn_tab_submisson_onidle";
			spTabEventFont.spriteName = "ibtn_tab_event_offidle";
			spTabPlayFont.spriteName = "ibtn_tab_playmisson_offidle";

			break;
		case Type.Event:
			spTabMain.spriteName = "ibtn_tab_offidle";
			spTabSub.spriteName = "ibtn_tab_offidle";
			spTabEvent.spriteName = "ibtn_tab_onidle";
			spTabPlay.spriteName = "ibtn_tab_offidle";

			spTabMainFont.spriteName = "ibtn_tab_mainmisson_offidle";
			spTabSubFont.spriteName = "ibtn_tab_submisson_offidle";
			spTabEventFont.spriteName = "ibtn_tab_event_onidle";
			spTabPlayFont.spriteName = "ibtn_tab_playmisson_offidle";

			break;
		case Type.Play:
			spTabMain.spriteName = "ibtn_tab_offidle";
			spTabSub.spriteName = "ibtn_tab_offidle";
			spTabEvent.spriteName = "ibtn_tab_offidle";
			spTabPlay.spriteName = "ibtn_tab_onidle";

			spTabMainFont.spriteName = "ibtn_tab_mainmisson_offidle";
			spTabSubFont.spriteName = "ibtn_tab_submisson_offidle";
			spTabEventFont.spriteName = "ibtn_tab_event_offidle";
			spTabPlayFont.spriteName = "ibtn_tab_playmisson_onidle";
			break;
		}

		updateGold();
		updateRuby();

		list.draw();

		checkNew();
		System.GC.Collect();

	}


	public override void show ()
	{
		base.show ();
		prevType = Type.None;

		list.clear();


		if(TutorialManager.instance.clearCheck("T13") == false)
		{
			type = Type.Main;
		}


		EpiServer.instance.sendMissionData();


		// 1.2.3. [액트1 스테이지2 라운드3] 이상 클리어 & 월드맵 페이지 & 스테이지 이동 연출 완료 후.
		TutorialManager.instance.check("T13");
	}



	void onBuyGold(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showGoldShop();
	}
	
	void onBuyRuby(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showRubyShop();
	}

	
	void updateGold()
	{
		lbGold.text = Util.GetCommaScore(GameDataManager.instance.gold);
	}
	
	void updateRuby()
	{
		lbRuby.text = Util.GetCommaScore(GameDataManager.instance.ruby);
	}


	public void onReceiveItem(UIMissionListSlotPanel panel)
	{
		SoundData.play("uiet_missionrwd");
		EpiServer.instance.sendRequestMissionReward(panel.data.id);
	}


	void LateUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(btnBack.isEnabled == false) return;
			if(GameManager.me.uiManager.uiMenu.rayCast(GameManager.me.uiManager.uiMenuCamera.camera, btnBack.gameObject) == false) return;


			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;
			onClickBackToMainMenu(null);
			return;
		}
	}

}
