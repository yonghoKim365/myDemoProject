using System;
using System.Collections.Generic;
using UnityEngine;


public class CutSceneData
{

	public enum CutSceneType
	{
		CutScene, UnitCam
	}

	public CutSceneType cutSceneType = CutSceneType.CutScene;

	public CutSceneData ()
	{
	}

	const string COMMAND = "COMMAND";
	
	// Condition
	const string ROUND = "ROUND";
	const string PLAYTIME = "PLAYTIME";
	const string HP = "HP";
	
	// Setup.
	const string MAP = "MAP";
	const string OBJECT = "OBJECT";
	const string EFFECT = "EFFECT";
	const string IMAGE = "IMAGE";
	
	// Event
	const string SCREENMODE = "SCREENMODE";
	const string UIACTIVE = "UIACTIVE";
	const string CONTROL_ACTIVE = "CONTROL_ACTIVE";

	const string UIINPUT = "UIINPUT";

	const string SOUND = "SOUND";

	const string FADE = "FADE";
	const string PAUSE = "PAUSE";
	const string PAUSE_TOUCH = "PAUSE_TOUCH";
	const string PAUSE_LONG_TOUCH = "PAUSE_LONG_TOUCH";
	const string PAUSE_TOUCH_OFF = "PAUSE_TOUCH_OFF";


	const string SLOW = "SLOW";
	const string SET_OBJ = "SET_OBJ";
	const string OBJ_PLAY = "OBJ_PLAY";
	const string OBJ_MOVE = "OBJ_MOVE";
	const string OBJ_MOVE2 = "OBJ_MOVE2";
	const string OBJ_P_ANI = "OBJ_P_ANI";
	const string OBJ_ROTATE = "OBJ_ROTATE";
	const string OBJ_DELETE = "OBJ_DELETE";
	const string OBJ_STOP = "OBJ_STOP";
	const string OBJ_VISIBLE = "OBJ_VISIBLE";
	const string OBJ_UPDOWN = "OBJ_UPDOWN";

	const string OBJ_RESIZE = "OBJ_RESIZE";

	const string OBJ_COLOR = "OBJ_COLOR";

	const string OBJ_DMGEFF = "OBJ_DMGEFF";


	const string SET_EFF = "SET_EFF";
	const string EFF_PLAY = "EFF_PLAY";
	const string EFF_MOVE = "EFF_MOVE";
	const string EFF_ROTATE = "EFF_ROTATE";
	const string EFF_DELETE = "EFF_DELETE";
	const string EFF_RESIZE = "EFF_RESIZE";
	
	const string SET_IMG = "SET_IMG";
	const string IMG_DELETE = "IMG_DELETE";
	const string ILLUST_TEXT = "ILLUST_TEXT";
	const string TOOLTIP = "TOOLTIP";

	const string TOOLTIP2 = "TOOLTIP2";

	
	const string R_CONTROL = "R_CONTROL";
	
	const string CAM_SHOT = "CAM_SHOT";
	const string CAM_MOVE = "CAM_MOVE";
	
	const string CAM_TYPE = "CAM_TYPE";

	const string CAM_AROUND = "CAM_AROUND";

	const string RETURN_INGAME = "RETURN_INGAME";

	const string KILL = "KILL";

	const string CLOSE_SCENE = "CLOSE_SCENE";
	const string NEXT_SCENE = "NEXT_SCENE";

	const string SKIP = "SKIP";

	const string CLEAN_ASSET = "CLEAN";

	const string SHOW_MAP = "SHOW_MAP";

	const string SET_TIME = "SET_TIME";


	int act = -1;
	int stage = -1;

	private string _id = "";
	public string id
	{
		set
		{
			_id = value;

			if(value.StartsWith("A"))
			{


				string[] fuck = value.Split('_');

				switch(fuck[0].Length)
				{
				case 6:
					int.TryParse(_id.Substring(1,1),out act);
					int.TryParse(_id.Substring(3,1),out stage);
					break;
				case 7:
					int.TryParse(_id.Substring(1,2),out act);
					int.TryParse(_id.Substring(4,1),out stage);
					break;
				}
			}
		}
		get
		{
			return _id;
		}
	}


	private static List<string> _deleteCutSceneKeys = new List<string>();
	public static void clearCutSceneData()
	{
		_deleteCutSceneKeys.Clear();
		foreach(KeyValuePair<string, CutSceneData> kv in GameManager.info.cutSceneData)
		{
			if(kv.Value.canDelete())
			{
				kv.Value.destroy();
				_deleteCutSceneKeys.Add(kv.Key);
			}
		}

		foreach(string str in _deleteCutSceneKeys)
		{
			GameManager.info.cutSceneData[str] = null;
			GameManager.info.cutSceneData.Remove(str);
		}

		if(_deleteCutSceneKeys.Count > 0) _deleteCutSceneKeys.Clear();
	}


	public bool canDelete()
	{
		if(act < GameDataManager.instance.maxAct ||
		   (act == GameDataManager.instance.maxAct && stage < GameDataManager.instance.maxStage)
		   )
		{
			return true;
		}

		return false;
	}

	public List<CutSceneDataElement> conditions = new List<CutSceneDataElement>();
	public List<CutSceneDataElement> setup = new List<CutSceneDataElement>();
	public List<CutSceneDataElement> events = new List<CutSceneDataElement>();
	public List<CutSceneDataElement> frameEvents = new List<CutSceneDataElement>();
	public List<string> loadMonsterIds = new List<string>();
	public List<int> loadMapIds = new List<int>();
	public List<string> loadSoundIds = new List<string>();
	public List<string> loadEffectIds = new List<string>();
	public CutSceneDataElement roundStateChecker = null;

	string _cmd;

	public void setData(List<object> l, Dictionary<string, int> k, CutSceneType type = CutSceneType.CutScene)
	{
		CutSceneDataElement data = null;
		_cmd = (string)l[k["COMMAND"]];

		cutSceneType = type;

		switch(_cmd)
		{
		case ROUND:
			data = new CutSceneDataRound();	
			roundStateChecker = data;
			break;
		case PLAYTIME:
			data = new CutSceneDataPlayTime();
			conditions.Add(data);
			break;
		case HP:
			data = new CutSceneDataHP();				
			conditions.Add(data);
			break;
		
		//=== setup ===//	
		case MAP:
			data = new CutSceneDataMap();
			setup.Add(data);
			break;
		case OBJECT:
			data = new CutSceneDataObject();
			setup.Add(data);
			break;
		case EFFECT:
			data = new CutSceneDataEffect();
			setup.Add(data);
			break;
		case IMAGE:
			data = new CutSceneDataImage();
			setup.Add(data);			
			break;

		//=== event ===//
		case SCREENMODE:
			data = new CutSceneDataScreenMode();
			break;
		case UIACTIVE:
			data = new CutSceneDataUIActive();
			break;

		case CONTROL_ACTIVE:
			data = new CutSceneDataControlActive();
			break;

		case UIINPUT:
			data = new CutSceneDataUIInput();
			break;

		case SOUND:
			data = new CutSceneDataSound();
			break;

		case FADE:
			data = new CutSceneDataFade();
			break;
		case PAUSE:
			data = new CutSceneDataPause();
			break;
		case PAUSE_TOUCH:
			data = new CutSceneDataPauseTouch();
			break;
		case PAUSE_LONG_TOUCH:
			data = new CutSceneDataPauseLongTouch();
			break;

		case PAUSE_TOUCH_OFF:
			data = new CutSceneDataPauseTouchOff();
			break;


		case SLOW:
			data = new CutSceneDataSlow();
			break;			
		
		case SET_OBJ:
			data = new CutSceneDataSetObject();
			break;
		case OBJ_PLAY:
			data = new CutSceneDataObjectPlay();
			break;
		case OBJ_P_ANI:
			data = new CutSceneDataObjectFaceAnimationPlay();
			break;

		case OBJ_STOP:
			data = new CutSceneDataObjectStop();
			break;

		case OBJ_VISIBLE:
			data = new CutSceneDataObjectVisible();
			break;

		case OBJ_MOVE:
			data = new CutSceneDataObjectMove();
			break;

		case OBJ_MOVE2:
			data = new CutSceneDataObjectMove2();
			break;

		case OBJ_COLOR:
			data = new CutSceneDataObjectColor();
			break;

		case OBJ_DMGEFF:
			data = new CutSceneDataObjectDamageEffect();
			break;

		case OBJ_ROTATE:
			data = new CutSceneDataObjectRotate();
			break;
		case OBJ_DELETE:
			data = new CutSceneDataObjectDelete();
			break;

		case OBJ_UPDOWN:
			data = new CutSceneDataObjectUpDown();
			break;
		case OBJ_RESIZE:
			data = new CutSceneDataObjectResize();
			break;

		case SET_EFF:
			data = new CutSceneDataSetEffect();
			break;
		case EFF_PLAY:
			data = new CutSceneDataEffectPlay();
			break;
		case EFF_MOVE:
			data = new CutSceneDataEffectMove();
			break;
		case EFF_ROTATE:
			data = new CutSceneDataEffectRotate();
			break;
		case EFF_DELETE:
			data = new CutSceneDataEffectDelete();
			break;
		case EFF_RESIZE:
			data = new CutSceneDataEffectResize();
			break;
		case SET_IMG:
			data = new CutSceneDataSetImage();
			break;
		case IMG_DELETE:
			data = new CutSceneDataImageDelete();
			events.Add(data);			
			break;
		case ILLUST_TEXT:
			data = new CutSceneDataIllustText();
			break;
		case TOOLTIP:
			data = new CutSceneDataTooltip();
			break;

		case TOOLTIP2:
			data = new CutSceneDataTooltip2();
			break;

		case R_CONTROL:
			data = new CutSceneDataRoundControl();
			break;
		
		case CAM_TYPE:
			data = new CutSceneDataCameraType();
			break;			
		
		case CAM_SHOT:
			data = new CutSceneDataCamShot();
			break;
		case CAM_MOVE:
			data = new CutSceneDataCamMove();
			break;

		case CAM_AROUND:
			data = new CutSceneDataCamAround();
			break;

		case RETURN_INGAME:
			data = new CutSceneDataReturnInGameCamera();
			break;

		case KILL:
			data = new CutSceneDataKill();
			break;

		case SKIP:
			data = new CutSceneDataSkip();
			break;

		case CLEAN_ASSET:
			data = new CutSceneDataClearAsset();
			break;

		case SHOW_MAP:
			data = new CutSceneDataShowMap();
			break;


		case CLOSE_SCENE:
			data = new CutSceneDataCloseScene();
			break;			
			
		case NEXT_SCENE:
			data = new CutSceneDataNextScene();
			break;		


		case SET_TIME:
			data = new CutSceneSetTime();
			break;	

		}
		
		if(data != null)
		{
			data.setHeader(l[k["HEADER"]]);
			data.setData(l[k["ATTR1"]],l[k["ATTR2"]],l[k["ATTR3"]],l[k["ATTR4"]], l[k["ATTR5"]], l[k["ATTR6"]], l[k["ATTR7"]], l[k["ATTR8"]]);

			if(data.header == CutSceneDataElement.HEADER.EVENT)
			{
				events.Add(data);
			}
			else if(data.header == CutSceneDataElement.HEADER.FRAME_EVENT)
			{
				frameEvents.Add(data);
			}

			if(_cmd == OBJECT)
			{
				string loadData = data.getValue();
				if(loadData != null)
				{
					//Debug.LogError(loadData);
					loadMonsterIds.Add(loadData);
				}

				data.execOption();
			}
			else if(_cmd == MAP)
			{
				int mid = data.getIntValue(0);
				if(mid > 0) loadMapIds.Add(mid);
			}
			else if(_cmd == SOUND)
			{
				string loadData = data.getValue();
				if(string.IsNullOrEmpty(loadData) == false)
				{
					loadSoundIds.Add(loadData);
				}
			}
			else if(_cmd == EFFECT)
			{
				string effectId = data.getValue();
				if(string.IsNullOrEmpty(effectId) == false)
				{
					loadEffectIds.Add(effectId);
				}
			}
		}
	}

	public int checkIndex = 0;
	public int endIndex = 0;
	
	
	public int checkFrameIndex = 0;
	public int endFrameIndex = 0;


	static CutSceneSorterByTime _eventTimeSorter = new CutSceneSorterByTime();
	static CutSceneSorterByFrame _eventFrameSorter = new CutSceneSorterByFrame();


	public void setDataFinallize()
	{
		events.Sort(_eventTimeSorter);
		frameEvents.Sort(_eventFrameSorter);
		endIndex = events.Count;
		endFrameIndex = frameEvents.Count;
	}	




	public void destroy()
	{
		if(conditions != null)
		{
			foreach(CutSceneDataElement cde in conditions){ cde.destroy(); };
			conditions.Clear();
			conditions = null;
		}

		if(setup != null)
		{
			foreach(CutSceneDataElement cde in setup){ cde.destroy(); };
			setup.Clear();
			setup = null;
		}

		if(events != null)
		{
			foreach(CutSceneDataElement cde in events){ cde.destroy(); };
			events.Clear();
			events = null;
		}

		if(frameEvents != null)
		{
			foreach(CutSceneDataElement cde in frameEvents){ cde.destroy(); };
			frameEvents.Clear();
			frameEvents = null;
		}

		if(roundStateChecker != null) roundStateChecker.destroy();
		roundStateChecker = null;

		loadMapIds.Clear();
		loadMapIds = null;

		loadMonsterIds.Clear();
		loadMonsterIds = null;

		loadSoundIds.Clear();
		loadSoundIds = null;
	}

	

//============================================================================================

	
	public void init()
	{
		checkIndex = 0;	
		checkFrameIndex = 0;

		status = STATUS.PREPARE;
	}
	
	public enum STATUS
	{
		PREPARE, PLAY, PAUSE, COMPLETE, PAUSE_TOUCH, PAUSE_LONG_TOUCH, PAUSE_TOUCH_OFF
	}

	public STATUS status;
	
	public void startCutScene()
	{
		status = STATUS.PLAY;
	}
	
	
	public void load(bool isUnitSkillCam = false)
	{
		if(isUnitSkillCam == false)
		{
			// 컷씬이 시작됐을때 인게임 캐릭터가 있다면 언데드 히어로 몬스터들은 세팅이 가능하도록 해준다.
			if(GameManager.me.stageManager.heroMonster != null)
			{
				string tempStr;
				for(int i = 0; i < GameManager.me.stageManager.heroMonster.Length ; ++i)
				{
					tempStr = i.ToString();
					if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(tempStr) == false) GameManager.me.cutSceneManager.cutScenePlayCharacter[tempStr] = new List<Monster>();		
					GameManager.me.cutSceneManager.cutScenePlayCharacter[tempStr].Clear();
					GameManager.me.cutSceneManager.cutScenePlayCharacter[tempStr].Add(GameManager.me.stageManager.heroMonster[i]);
				}
			}
		}

		foreach(CutSceneDataElement cd in setup)
		{
			cd.startAction();
		}
	}
	
	
	
	public void update()
	{
		if(status == STATUS.PREPARE)
		{
			checkOpenCondition();
		}
		else if(status == STATUS.PAUSE)
		{
		}
		else if(status == STATUS.PLAY)
		{
			checkEvent();
		}
	}




	public void checkEvent()
	{
		for(int i = checkIndex; i < endIndex; ++i)
		{

			if(events[i].eventTime <= CutSceneManager.cutScenePlayTime)
			{
				checkIndex = i+1;
				events[i].startAction();
			}
			else break;
		}
		
//		if(checkIndex >= endIndex)
		{
			// 컷씬 종료는 준비된 이벤트가 끝났을때가 아니라 종료 이벤트를 통해 하는 것으로 바꾸었음.
			//status = STATUS.COMPLETE;
			//GameManager.me.cutSceneManager.completeCutScene();
		}
	}


	public void checkFrameEvent()
	{
		//if(status != STATUS.PLAY) return;

		for(int i = checkFrameIndex; i < endFrameIndex; ++i)
		{
			if(frameEvents[i].eventFrame <= GameManager.replayManager.nowFrame)
			{
				checkFrameIndex = i+1;
				frameEvents[i].startAction();
			}
			else break;
		}
	}


	

	// 발동조건 체크...===================================================		
	public void checkOpenCondition()
	{
		#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker) return;
		#endif

	 	foreach(CutSceneDataElement cde in conditions)
		{
			if(cde.check())
			{
				GameManager.me.cutSceneManager.openAndLoadCutScene(this);
				return;
			}
		}
	}
	

}


// 이벤트를 시간대별로 정렬해둔다.
public class CutSceneSorterByTime  : IComparer<CutSceneDataElement>
{
	public int Compare(CutSceneDataElement x, CutSceneDataElement y)
	{
		if(x.eventTime > y.eventTime) return 1;
		else if(x.eventTime < y.eventTime) return -1;
		return 0;
	}	
}


public class CutSceneSorterByFrame  : IComparer<CutSceneDataElement>
{
	public int Compare(CutSceneDataElement x, CutSceneDataElement y)
	{
		if(x.eventFrame > y.eventFrame) return 1;
		else if(x.eventFrame < y.eventFrame) return -1;
		return 0;
	}	
}


//======================================================================
			

public abstract class CutSceneDataElement
{
	public enum HEADER
	{
		CONDITION, SETUP, EVENT, FRAME_EVENT
	}

	public HEADER header;
	public float eventTime = 0.0f;

	public int eventFrame = 0;

	
	public void setHeader(object h)
	{
		if(h is string)
		{
			string str = (string)h;
			if( str.Equals("CON") ) header = HEADER.CONDITION;
			else if( str.Equals("SETUP") ) header = HEADER.SETUP;
			else if( str.StartsWith("F") )
			{
				header = HEADER.FRAME_EVENT;
				int.TryParse(str.Substring(1), out eventFrame);
			}
			else
			{
				header = HEADER.EVENT;
				Util.parseObject(h, out eventTime, true, 0.0f);
			}
		}
		else
		{
			header = HEADER.EVENT;
			Util.parseObject(h, out eventTime, true, 0.0f);
		}
	}
	
	public abstract void setData(params object[] attrs);
	
	public virtual bool check()
	{
		return false;
	}

	public virtual void startAction()
	{
	}

	public virtual string getValue()
	{
		return null;
	}

	public virtual int getIntValue(int option)
	{
		return -1;
	}

	public virtual void execOption()
	{
	}

	public abstract void destroy();

}


// 발동조건 체크...===================================================	

sealed public class CutSceneDataRound : CutSceneDataElement
{
	public int roundStartType;
	//* 라운드 : 스타트 or 클리어 or 실패 (0~2) > 속성1 : 0~2
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out roundStartType, true, 0);
	}
	
	sealed public override bool check()
	{
#if UNITY_EDITOR
		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			if(roundStartType == 2)
			{
				roundStartType = 0;
			}
		}
#endif

		if(
			roundStartType == 0 && GameManager.me.currentScene == Scene.STATE.PLAY_INTRO
			|| roundStartType == 1 && GameManager.me.currentScene == Scene.STATE.PLAY_BATTLE && GameManager.me.stageManager.playTime == 0.0f
			)
		{
			CutSceneManager.nowOpenCutSceneType = CutSceneManager.CutSceneType.Pre;

			return true;
		}
		else if(
			roundStartType == 2 && GameManager.me.currentScene == Scene.STATE.PLAY_CLEAR_SUCCESS
			|| roundStartType == 3 && GameManager.me.currentScene == Scene.STATE.PLAY_CLEAR_FAILED
			|| roundStartType == 4 && GameManager.me.currentScene == Scene.STATE.PLAY_LAST_MONSTER_DIE
			)
		{

			CutSceneManager.nowOpenCutSceneType = CutSceneManager.CutSceneType.After;
			return true;
		}

		return false;	
	}

	public override void destroy ()
	{
	}
	
}


sealed public class CutSceneDataPlayTime : CutSceneDataElement
{
	public float minTime;
	public float maxTime;
	//* 플레이타임: 해당 라운드가 스타트된 시점부터 MIN/MAX 시간(sec) 을 설정 > 속성1 : 00000, 00000
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out minTime, true, 0.0f);
		Util.parseObject(attrs[1], out maxTime, true, 0.0f);
	}
	
	
	sealed public override bool check()
	{
		return (GameManager.me.stageManager.playTime >= minTime && GameManager.me.stageManager.playTime <= maxTime);
	}

	public override void destroy ()
	{
	}
}


sealed public class CutSceneDataHP : CutSceneDataElement
{
	//* 히어로HP: 아군 or 적군 히어로 설정, 해당 캐릭터의 HP MIN/MAX (%) 설정 > 속성1 : 0~N (0 : 아군히어로, 1~N 번째 몬스터히어로) > 속성2 : 000, 000
	
	public int heroNum = 0;
	
	public int minHp = 0;
	public int maxHp = 0;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out heroNum, true, 0);
		
		int[] hp = Util.stringToIntArray((string)attrs[1],',');
		minHp = hp[0];
		maxHp = hp[1];
	}
	

	float hpPer;
	sealed public override bool check()
	{
		if(heroNum == 0)
		{
			hpPer = GameManager.me.player.hp/GameManager.me.player.maxHp * 100.0f;
		}
		else if(GameManager.me.stageManager.heroMonster == null)
		{
			return false;
		}
		else 
		{
			hpPer = GameManager.me.stageManager.heroMonster[heroNum-1].hp/GameManager.me.stageManager.heroMonster[heroNum-1].maxHp * 100.0f;
		}
		
		return (hpPer >= minHp && hpPer <= maxHp);
	}	

	public override void destroy ()
	{
	}
}



//================================================
// SETUP
//===============================================


sealed public class CutSceneDataMap : CutSceneDataElement
{
	public int mapId;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out mapId, true, 0);
	}
	
	
	sealed public override bool check()
	{
		return false;	
	}	
	
	
	sealed public override void startAction ()
	{
		base.startAction ();
		
		if(mapId == 0)
		{
			GameManager.me.cutSceneManager.stopLoopInGame = false;
			GameManager.me.cutSceneManager.status = CutSceneManager.Status.INGAME_CUTSCENE;
			
			// 컷씬을 시작하기 전에 캐릭터 세팅을 안했으면 세팅을 한다.
			GameManager.me.mapManager.setStage(GameManager.me.stageManager.nowRound);
			GameManager.me.mapManager.createBackground(mapId,true);
		}
		else
		{
			GameManager.me.cutSceneManager.stopLoopInGame = true;
			GameManager.me.cutSceneManager.status = CutSceneManager.Status.CUTSCENE;
			GameManager.me.mapManager.createBackground(mapId,false,true);
		}		
	}


	public override int getIntValue (int option)
	{
		return mapId;
	}

	public override void destroy ()
	{
	}

}

public sealed class CutSceneDataObject : CutSceneDataElement
{
	/*
* 해당컷씬에 사용할 모든 3D 오브젝트를 로딩
> 속성1 : 타입(0~6) (아군히어로, 언데드히어로, 아군유닛, 언데드유닛, 컷씬용오브젝트, 유저선택히어로, 유저유닛)
   - 유저선택 히어로 : 해당 라운드를 플레이하기 위해 유저가 선택한 히어로 (유저가 세팅한 파츠로 보임) (속성2 지정 생략)
   - 유저선택 히어로 : 유저가 장착한 소환룬의 소환유닛
> 속성2 : 속성1 에서 선택한 타입의 ID
   - 유저선택히어로일 경우 생략, 유저유닛은 N번째 0~4 지정
> 속성3 : 개체이름 지정
   - 해당 캐릭터의 이름을 텍스트로 지정
   - 이후 스폰/이동/애니메이션 이벤트에 사용
	 */
	
	public int type;
	public string id;
	public string objName;

	public string[] data = null;

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out type, true, 0);
		
		id = attrs[1].ToString();	

		objName = (string)attrs[2];

		string str = (string)attrs[3];

		data  = str.Split(',');
	}
	
	
	sealed public override bool check()
	{
		return false;	
	}	

	
	
	sealed public override void startAction ()
	{
		CutSceneCharacterData cd = new CutSceneCharacterData();
		switch(type)
		{
		case 0: // 아군 히어로
			cd.isPlayerMon = true;
			cd.type = Monster.TYPE.PLAYER;

			switch(id)
			{
			case "0":
				cd.id = Character.LEO;
				break;
			case "1":
				cd.id = Character.KILEY;
				break;
			case "2":
				cd.id = Character.CHLOE;
				break;
			case "3":
				cd.id = Character.LUKE;
				break;
			default:
				cd.id = id;
				break;
			}

			cd.canClone = true;
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;		
			break;
		case 1: // 언데드 히어로
			//cd.cha = GameManager.me.cutSceneManager.addMonsterToStage(false,false,id);
			cd.isPlayerMon = false;
			cd.type = Monster.TYPE.HERO;
			cd.id = id;
			cd.canClone = true;
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;
			break;
		case 2: // 아군유닛 
			//cd.cha = GameManager.me.cutSceneManager.addMonsterToStage(true,true,id);
			cd.isPlayerMon = true;
			cd.type = Monster.TYPE.UNIT;
			cd.id = id;			
			cd.canClone = true;
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;			
			break;
		case 3: // 언데드 유닛
			//cd.cha = GameManager.me.cutSceneManager.addMonsterToStage(false,true, id );	
			cd.isPlayerMon = false;
			cd.type = Monster.TYPE.UNIT;
			cd.id = id;			
			cd.canClone = true;
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;			
			break;
		case 4: // 컷씬용 오브젝트
			
			break;
		case 5: // 유저선택 히어로 

			if(data != null)
			{
				setAnotherPlayer();
			}

			cd.cha = GameManager.me.player;
			cd.cha.state = Monster.NORMAL;
			cd.cha.renderAniRightNow();
			cd.cha.setVisible(true);
			cd.cha.changeShader(false,true);
			cd.cha.setColor(Color.white);
			cd.canClone = false;			
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;		

			break;

		case 6: // 유저유닛
			//cd.cha = GameManager.me.cutSceneManager.addMonsterToStage(true,true, GameDataManager.instance.petSlots[id].unitData.id );
			cd.canClone = true;
			cd.isPlayerMon = true;
			cd.type = Monster.TYPE.UNIT;
			int t = 0;
			int.TryParse(id, out t);
			cd.id = GameDataManager.instance.unitSlots[t].unitData.id;			
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;			
			break;	
		case 7: // NPC
			cd.canClone = true;
			cd.isPlayerMon = true;
			cd.type = Monster.TYPE.NPC;
			cd.id = id;
			GameManager.me.cutSceneManager.cutSceneCharacterDic[objName] = cd;					
			break;					
		}
	}
	
	sealed public override string getValue ()
	{
//		Debug.Log(id + " " + objName + "   type: " + type);
		switch(type)
		{
		case 0: // 아군 히어로
			return null;
		case 1: // 언데드 히어로
			return GameManager.info.heroMonsterData[id].resource;
		case 2: // 아군유닛 
			return GameManager.info.unitData[id].resource;
		case 3: // 언데드 유닛
			return GameManager.info.unitData[id].resource;			
		case 4: // 컷씬용 오브젝트
			break;
		case 5: // 유저선택 히어로 
			break;
		case 6: // 유저유닛
			break;	
		case 7: // NPC
			//Debug.Log(id);
			return GameManager.info.npcData[id].resource;
			break;	
		}
		
		return null;
	}


	public override void execOption ()
	{
		if(data != null && data.Length > 5)
		{
			GamePlayerData gpd = new GamePlayerData(data[0]);
			
			units[0] = data[6];
			units[1] = data[7];
			units[2] = data[8];
			units[3] = data[9];
			units[4] = data[10];
			
			skills[0] = data[11];
			skills[1] = data[12];
			skills[2] = data[13];
			
			DebugManager.instance.setPlayerData(gpd,true,data[0],data[2],data[3],data[4],data[5],units,skills,data[14],(data.Length == 16)?data[15]:null);

			GameManager.me.effectManager.loadEffectFromPlayerData(gpd);
		}
	}


	string[] units = new string[5];
	string[] skills = new string[3];
	void setAnotherPlayer()
	{
		if(data != null && data.Length > 5)
		{
			GamePlayerData gpd = new GamePlayerData(data[0]);

			//"LEO,9,LEO_HD1_1_1_N_0,LEO_BD1_1_1_N_0,LEO_BD1_1_1_N_0,LEO_RD1_1_1_N_2,UN1,UN2,UN3,UN4,UN5,SK_A031_N,SK_A045_R,SK_A040_S,ai_0";
			
			int lv = 1;
			int.TryParse(data[1], out lv);
			
			units[0] = data[6];
			units[1] = data[7];
			units[2] = data[8];
			units[3] = data[9];
			units[4] = data[10];
			
			skills[0] = data[11];
			skills[1] = data[12];
			skills[2] = data[13];
			
			Vector3 prevPos = GameManager.me.player.cTransform.position;
			Vector3 prevPos2 = GameManager.me.player.cTransformPosition;
			
			DebugManager.instance.setPlayerData(gpd,true,data[0],data[2],data[3],data[4],data[5],units,skills,data[14], (data.Length == 16)?data[15]:null);
			GameManager.me.changeMainPlayer(gpd,gpd.id,gpd.partsVehicle.parts.resource.ToUpper());
			GameManager.me.player.isEnabled = true;
			
			GameManager.me.player.cTransform.position = prevPos;
			GameManager.me.player.cTransformPosition = prevPos2;
		}
	}


	sealed public override void destroy ()
	{
		id = null;
		objName = null;
		data = null;
		units = null;
		skills = null;
	}
}


sealed public class CutSceneDataEffect : CutSceneDataElement
{
	/*
* 해당컷씬에 사용할 모든 이펙트 로딩
> 속성1 : 이펙트 아이디
> 속성2 : 이펙트이름 지정
*/
	public string id;
	public string effName;
	
	sealed public override void setData(params object[] attrs)
	{
		id = (string)attrs[0];
		effName = (string)attrs[1];		
	}
	
	
	sealed public override bool check()
	{
		return false;	
	}
	
	
	sealed public override void startAction ()
	{
		CutSceneEffectData cd = new CutSceneEffectData();
		cd.effectData = GameManager.info.effectData[id];
		GameManager.me.cutSceneManager.cutSceneEffectDic[effName] = cd;
	}

	sealed public override void destroy ()
	{
		id = null;
		effName = null;
	}


	sealed public override string getValue ()
	{
		return id;
	}

}


sealed public class CutSceneDataImage : CutSceneDataElement
{
	public string id;
	public string imgName;
	
	sealed public override void setData(params object[] attrs)
	{
		id  = (string)attrs[0];
		imgName = (string)attrs[1];		
	}	

	sealed public override void destroy ()
	{
		id = null;
		imgName = null;
	}

}

//================================================
// EVENT
//===============================================

sealed public class CutSceneDataScreenMode : CutSceneDataElement
{

	/*
* 0 : 게임UI 표시, 1 : 게임UI 날리고 상하부분 검정색 띠 표시, 2 : 게임 UI표시. 단, 조작은 불가, 3: 레터박스/게임UI 모두 비활성화
> 속성1 : 0~2
> 속성2: 모션시간

	*/
	
	public int type = 0;
	public float motionTime = 0.0f;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out type, true, 0);
		Util.parseObject(attrs[1], out motionTime, true, 0);
	}	
	
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case 0:
			GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.GameUI, motionTime);
			break;
		case 1:
			GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.LetterBoxShow, motionTime);
			break;
		case 2:
			GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.GameUI_NO_TOUCH, motionTime);
			break;
		case 3:
			GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.HIDE_ALL, motionTime);
			break;
		}
	}	


	sealed public override void destroy ()
	{

	}

}



sealed public class CutSceneDataControlActive : CutSceneDataElement
{
	public int type = 0;
	
	sealed public override void setData(params object[] attrs)
	{
		// 1이 활성화. 0이 비활성화.
		Util.parseObject(attrs[0], out type, true, 0);
	}	
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.blockControl = (type == 0);
	}	


	sealed public override void destroy ()
	{
		
	}


}




sealed public class CutSceneDataUIActive : CutSceneDataElement
{
	public string[] targetSlots;
	public int type = 0;

	sealed public override void setData(params object[] attrs)
	{
		// 1이 활성화. 0이 비활성화.
		targetSlots = (attrs[0].ToString()).Split(',');
		Util.parseObject(attrs[1], out type, true, 0);
	}	
	
	sealed public override void startAction()
	{
		for(int i = 0; i < targetSlots.Length; ++i)
		{
			switch(targetSlots[i])
			{
			case UnitSlot.U1:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[0].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.UIUnitSlot[0].blockThis = (type != 1);
				break;
			case UnitSlot.U2:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[1].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.UIUnitSlot[1].blockThis = (type != 1);
				break;
			case UnitSlot.U3:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[2].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.UIUnitSlot[2].blockThis = (type != 1);
				break;
			case UnitSlot.U4:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[3].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.UIUnitSlot[3].blockThis = (type != 1);
				break;
			case UnitSlot.U5:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[4].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.UIUnitSlot[4].blockThis = (type != 1);
				break;
			case SkillSlot.S1:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[0].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.uiSkillSlot[0].blockThis = (type != 1);
				break;
			case SkillSlot.S2:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[1].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.uiSkillSlot[1].blockThis = (type != 1);
				break;
			case SkillSlot.S3:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[2].btn.isEnabled = (type == 1);
				GameManager.me.uiManager.uiPlay.uiSkillSlot[2].blockThis = (type != 1);
				break;
			}
		}
	}	


	sealed public override void destroy ()
	{
		targetSlots = null;
	}

}



sealed public class CutSceneDataUIInput : CutSceneDataElement
{
	public string targetSlot;
	public int type = 0;

	sealed public override void setData(params object[] attrs)
	{
		// 1이 누르기. 0이 떼기.
		targetSlot = attrs[0].ToString();
		Util.parseObject(attrs[1], out type, true, 0);
	}	

	sealed public override void startAction()
	{

		#if UNITY_EDITOR
		Debug.LogError(targetSlot + "  " + type);
		#endif



		switch(targetSlot)
		{
		case UnitSlot.U1:
			GameManager.me.uiManager.uiPlay.UIUnitSlot[0].onPress(null,true);
			break;
		case UnitSlot.U2:
			GameManager.me.uiManager.uiPlay.UIUnitSlot[1].onPress(null,true);
			break;
		case UnitSlot.U3:
			GameManager.me.uiManager.uiPlay.UIUnitSlot[2].onPress(null,true);
			break;
		case UnitSlot.U4:
			GameManager.me.uiManager.uiPlay.UIUnitSlot[3].onPress(null,true);
			break;
		case UnitSlot.U5:
			GameManager.me.uiManager.uiPlay.UIUnitSlot[4].onPress(null,true);
			break;
		case SkillSlot.S1:
			GameManager.me.uiManager.uiPlay.uiSkillSlot[0].onIntroPress();//null,type == 1);
			break;
		case SkillSlot.S2:
			GameManager.me.uiManager.uiPlay.uiSkillSlot[1].onIntroPress();//null,type == 1);
			break;
		case SkillSlot.S3:
			GameManager.me.uiManager.uiPlay.uiSkillSlot[2].onIntroPress();//null,type == 1);
			break;
		}
	}	

	sealed public override void destroy ()
	{
		targetSlot = null;
	}

}



sealed public class CutSceneDataSound : CutSceneDataElement
{

	public string soundName;
	public int option; // 0은 멈춤. 1은 재생.
	public int option2; // 0 : music 1: loop effect  -1: none
	public int option3; // 0 : fadein  1: fadeout  2:crossfade  3: queuefade 4: stop -1: none
	public float fadeTime = 0;

	public override string getValue ()
	{
		return soundName;
	}

	sealed public override void setData(params object[] attrs)
	{
		soundName = (string)attrs[0];
		Util.parseObject(attrs[1],out option, true, 1);
		Util.parseObject(attrs[2],out option2, true, -1);
		Util.parseObject(attrs[3],out option3, true, -1);
		Util.parseObject(attrs[4],out fadeTime, true, 0.0f);
	}	
	
	
	sealed public override void startAction()
	{
		if(option2 > -1 && option3 > -1)
		{
			SoundManager.SoundPlayType pType = SoundManager.SoundPlayType.Music;
			if(option2 == 1) pType = SoundManager.SoundPlayType.LoopEffect;

			if(option3 == 4)
			{
				if(pType == SoundManager.SoundPlayType.Music) GameManager.soundManager.stopBG();
				else if(pType == SoundManager.SoundPlayType.LoopEffect) GameManager.soundManager.stopLoopEffect();
			}
			else
			{
				AudioFader.State fadeType = AudioFader.State.FadeIn;
				switch(option3)
				{
				case 0: fadeType = AudioFader.State.FadeIn; break;
				case 1: fadeType = AudioFader.State.FadeOut; break;
				case 2: fadeType = AudioFader.State.CrossFade; break;
				case 3: fadeType = AudioFader.State.QueueFade; break;
				}
				
				GameManager.soundManager.fadePlay(pType, soundName, fadeType, fadeTime);

			}
		}
		else
		{
			if(option == 1)
			{
				if(Time.timeScale >= 5.0f || GameManager.me.cutSceneManager.nowCutSceneSpeed > 1 || GameManager.me.cutSceneManager.isSkipMode) return;
				SoundData.play(soundName);
			}
			else SoundData.stop(soundName);
		}
	}


	sealed public override void destroy ()
	{
		soundName = null;
	}

}



sealed public class CutSceneDataFade : CutSceneDataElement
{

	/*
* 페이드인/아웃 설정
> 속성1 : 0(아웃), 1(인)
> 속성2 : ms (페이드 속도)
	*/
	public int type = 0;
	public float motionTime = 0.0f;
	public int color;
	public int must;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out type, true, 0);
		Util.parseObject(attrs[1], out motionTime, true, 0.0f);
		Util.parseObject(attrs[2], out color, true, 0);
		Util.parseObject(attrs[3], out must, true, 0);
	}	

	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.isSkipMode && must == 0) return;
		if(type == 0) GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_OUT, motionTime, color == 1);
		else if(type == 1) GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_IN, motionTime, color == 1);
	}	

	sealed public override void destroy ()
	{
	
	}

}


sealed public class CutSceneDataPause : CutSceneDataElement
{
	
/*
* 컷씬 진행을 일정시간동안 멈춤, 설정된 시간만큼 컷씬플레이타임 진행이 멈춤
> 속성1 : ms (포즈시간)
> 속성2 : 0 or 1 (화면정지여부)
   - 0 : 화면정지 OFF (진행되던 이벤트는 계속 진행되며 컷씬 플레이타임만 딜레이되는..움직이는 개체 및 애니메이션 계속 플레이)
   - 1 : 화면정지 ON (진행되던 모든 이벤트 멈춤 : 이동 및 애니메이션 멈춤)
*/
	
	public float time = 0.0f;
	public int type = 0;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out time, true, 0.0f);		
		Util.parseObject(attrs[1], out type, true, 0);
	}	
	
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.setPause(time, type);
	}	

	sealed public override void destroy ()
	{
		
	}

}


sealed public class CutSceneDataPauseTouch : CutSceneDataElement
{

	/*
* 컷씬 진행을 멈춘 후 특정영역 터치시 계속 진행
> 속성1 : xxx,yyy,www,hhh (화면좌표 / 너비높이)
> 속성2 : 0 or 1 (화면정지여부)
	*/
	public Rectangle touchRect = new Rectangle();
	public int type;

	int[] dim;
	int[] arrow;
	
	bool showDialog = false;
	float dialogPosX, dialogPosY;
	string dialogTextId;

	string clickUIId = null;




	
	sealed public override void setData(params object[] attrs)
	{
		int[] arr = Util.stringToIntArray((string)attrs[0],',');
		touchRect.x = arr[0];
		touchRect.y = arr[1];
		touchRect.width = arr[2];
		touchRect.height = arr[3];
		
		Util.parseObject(attrs[1],out type, true);

		//		> 속성2 : 0 or 1 (화면정지여부)			
		Util.parseObject(attrs[1],out type, true);
		
		dim = Util.stringToIntArray((string)attrs[2],',');//		> 속성3 : (딤처리) ON/OFF		
		
		arrow = Util.stringToIntArray((string)attrs[3],',');//		> 속성4 : (화살표 표시) ON/OFF , 화살표이미지 좌표			

		if(attrs[4] is string)
		{
			string[] temp = ((string)attrs[4]).Split(',');
			int t = 0;
			int.TryParse(temp[0],out t);
			showDialog = (t == 1);
			
			if(temp.Length > 3)
			{
				float.TryParse(temp[1],out dialogPosX);
				float.TryParse(temp[2],out dialogPosY);
				dialogTextId = temp[3];
			}
		}
		else showDialog = false;

		clickUIId = (attrs[5]).ToString();
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.isSkipMode) return;

		GameManager.me.cutSceneManager.setPauseAndTouch(touchRect, type, clickUIId);

		if(dim[0] == 1)
		{
			GameManager.me.uiManager.uiTutorial.showDim(true);
			GameManager.me.uiManager.uiTutorial.setDimPosition(dim[1],dim[2],dim[3]);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.showDim(false);
		}
		
		
		if(arrow[0] == 1)
		{
			GameManager.me.uiManager.uiTutorial.showArrow(arrow[1],arrow[2]);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.hideArrow();
		}

		
		if(showDialog)
		{
			GameManager.me.uiManager.uiTutorial.openDialog(dialogTextId,dialogPosX, dialogPosY);
			SoundManager.instance.playCutSceneVoice(dialogTextId);


		}
		else
		{
			GameManager.me.uiManager.uiTutorial.hideDialog();
		}
	}	


	sealed public override void destroy ()
	{
		dim = null;
		arrow = null;
		dialogTextId = null;
		clickUIId = null;
	}
	
}




sealed public class CutSceneDataPauseTouchOff : CutSceneDataElement
{
	
	/*
* 컷씬 진행을 멈춘 후 특정영역 터치시 계속 진행
> 속성1 : xxx,yyy,www,hhh (화면좌표 / 너비높이)
> 속성2 : 0 or 1 (화면정지여부)
	*/
	public Rectangle touchRect = new Rectangle();
	public int type;
	
	int[] dim;
	int[] arrow;
	
	bool showDialog = false;
	float dialogPosX, dialogPosY;
	string dialogTextId;


	sealed public override void destroy ()
	{
		dim = null;
		arrow = null;
		dialogTextId = null;
	}

	
	sealed public override void setData(params object[] attrs)
	{
		int[] arr = Util.stringToIntArray((string)attrs[0],',');
		touchRect.x = arr[0];
		touchRect.y = arr[1];
		touchRect.width = arr[2];
		touchRect.height = arr[3];
		
		Util.parseObject(attrs[1],out type, true);
		
		//		> 속성2 : 0 or 1 (화면정지여부)			
		Util.parseObject(attrs[1],out type, true);
		
		dim = Util.stringToIntArray((string)attrs[2],',');//		> 속성3 : (딤처리) ON/OFF		
		
		arrow = Util.stringToIntArray((string)attrs[3],',');//		> 속성4 : (화살표 표시) ON/OFF , 화살표이미지 좌표			
		
		string[] temp = ((string)attrs[4]).Split(',');
		int t = 0;
		int.TryParse(temp[0],out t);
		showDialog = (t == 1);
		
		if(temp.Length > 3)
		{
			float.TryParse(temp[1],out dialogPosX);
			float.TryParse(temp[2],out dialogPosY);
			dialogTextId = temp[3];
		}
		
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.isSkipMode) return;

#if UNITY_EDITOR
		Debug.LogError("== CutSceneDataPauseTouchOff");
#endif

		if(GameManager.me.cutSceneManager.setPauseAndTouchOff(touchRect, type))
		{
			if(dim[0] == 1)
			{
				GameManager.me.uiManager.uiTutorial.showDim(true);
				GameManager.me.uiManager.uiTutorial.setDimPosition(dim[1],dim[2],dim[3]);
			}
			else
			{
				GameManager.me.uiManager.uiTutorial.showDim(false);
			}
			
			
			if(arrow[0] == 1)
			{
				GameManager.me.uiManager.uiTutorial.showArrow(arrow[1],arrow[2]);
			}
			else
			{
				GameManager.me.uiManager.uiTutorial.hideArrow();
			}
			
			
			if(showDialog)
			{
				GameManager.me.uiManager.uiTutorial.openDialog(dialogTextId,dialogPosX, dialogPosY);

				SoundManager.instance.playCutSceneVoice(dialogTextId);

			}
			else
			{
				GameManager.me.uiManager.uiTutorial.hideDialog();
			}

		}
	}	
	
}








sealed public class CutSceneDataPauseLongTouch : CutSceneDataElement
{
	
	/*
* 컷씬 진행을 멈춘 후 특정영역 터치시 계속 진행
> 속성1 : xxx,yyy,www,hhh (화면좌표 / 너비높이)
> 속성2 : 0 or 1 (화면정지여부)
	*/
	public Rectangle touchRect = new Rectangle();
	public int type;
	float duration = 0.0f;

	int[] dim;
	int[] arrow;

	bool showDialog = false;
	float dialogPosX, dialogPosY;
	string dialogTextId;

	int[] hand;


	sealed public override void destroy ()
	{
		dim = null;
		arrow = null;
		dialogTextId = null;
		hand = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		float[] arr = Util.stringToFloatArray((string)attrs[0],',');
		touchRect.x = arr[0];
		touchRect.y = arr[1];
		touchRect.width = arr[2];
		touchRect.height = arr[3];
		duration = arr[4];

//		> 속성2 : 0 or 1 (화면정지여부)			
		Util.parseObject(attrs[1],out type, true, 0);
		dim = Util.stringToIntArray((string)attrs[2],',');//		> 속성3 : (딤처리) ON/OFF		
		arrow = Util.stringToIntArray((string)attrs[3],',');//		> 속성4 : (화살표 표시) ON/OFF , 화살표이미지 좌표			

		string[] temp = ((string)attrs[4]).Split(',');
		int t = 0;
		int.TryParse(temp[0],out t);
		showDialog = (t == 1);

		if(temp.Length > 3)
		{
			float.TryParse(temp[1],out dialogPosX);
			float.TryParse(temp[2],out dialogPosY);
			dialogTextId = temp[3];
		}
		//		> 속성5 : (튜토리얼설명창) ON/OFF , 좌표 , 텍스트			

		hand = Util.stringToIntArray((string)attrs[5],',');
		//		> 속성6 : (롱터치유도손가락이미지) ON/OFF , 좌표	
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.isSkipMode) return;

		GameManager.me.cutSceneManager.cutSceneEventEndTime = CutSceneManager.cutScenePlayTime + duration;
		GameManager.me.cutSceneManager.cutSceneEventFrameEndTime = GameManager.me.stageManager.playTime + duration;

		GameManager.me.cutSceneManager.setPauseAndLongTouch(touchRect, type);

		if(dim[0] == 1)
		{
			GameManager.me.uiManager.uiTutorial.showDim(true);
			GameManager.me.uiManager.uiTutorial.setDimPosition(dim[1],dim[2],dim[3]);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.showDim(false);
		}

		if(arrow[0] == 1)
		{
			GameManager.me.uiManager.uiTutorial.showArrow(arrow[1],arrow[2]);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.hideArrow();
		}

		if(hand[0] == 1)
		{
			GameManager.me.uiManager.uiTutorial.showHand(hand[1],hand[2]);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.hideHand();
		}

		if(showDialog)
		{
			GameManager.me.uiManager.uiTutorial.openDialog(dialogTextId,dialogPosX, dialogPosY);
			SoundManager.instance.playCutSceneVoice(dialogTextId);
		}
		else
		{
			GameManager.me.uiManager.uiTutorial.hideDialog();
		}

	}	
	
}





sealed public class CutSceneDataSlow : CutSceneDataElement
{
/*	
* 이후 모든 움직임(이동,애니메이션 등)이 설정된 비율만큼 느리게 진행됨
* 단, 컷씬타임은 제대로 흐르도록
> 속성1 : 0~100 (%)
* 한번 설정되면 100%로 다시 세팅하기 전까지 지속
*/ 
	
	public float timeScale = 1.0f;
	public int slowType = 0;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out timeScale, true, 1.0f);
		Util.parseObject(attrs[1],out slowType, true);

		timeScale *= 0.01f;
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.isSkipMode) return;

		GameManager.setTimeScale = timeScale * GameManager.me.cutSceneManager.cutScenePlaySpeed;
		
		GameManager.me.cutSceneManager.isSlowMode = (timeScale < 1.0f);
		GameManager.me.cutSceneManager.slowModeTimeProgressType = slowType;
	}	


	sealed public override void destroy ()
	{
	}
	
}




sealed public class CutSceneDataSetObject : CutSceneDataElement
{
/*	
* 로딩된 3D 개체를 1개 배치함, 기본적으로 IDLE 애니메이션 플레이 (있다면..)
> 속성1 : 개체이름
> 속성2 : 3D 좌표 (xxxx,yyyy,zzzz)
> 속성3 : 방향 (2D 각도) or 3D 좌표로 방향을 지정할 수 있는 먼가
		 */
	
	public string name;
	public Vector3 pos;
	public Vector3 rot;
	
	public bool canSetPos = true;
	public bool canSetRot = true;


	sealed public override void destroy ()
	{
		name = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
		
		
		
		
		canSetPos =((string)attrs[1]).Contains(",");
		canSetRot =((string)attrs[2]).Contains(",");
		
		float[] tempF;
		if(canSetPos)
		{
			tempF = Util.stringToFloatArray((string)attrs[1],',');
			pos.x = tempF[0];
			pos.y = tempF[1];
			pos.z = tempF[2];
		}
		
		if(canSetRot)
		{
			tempF = Util.stringToFloatArray((string)attrs[2],',');
			rot.x = tempF[0];
			rot.y = tempF[1];
			rot.z = tempF[2];
		}
	}	
	
	private Vector3 _v;
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(name) == false) GameManager.me.cutSceneManager.cutScenePlayCharacter[name] = new List<Monster>();
		
		if(GameManager.me.cutSceneManager.cutSceneCharacterDic.ContainsKey(name) && GameManager.me.cutSceneManager.cutSceneCharacterDic[name].canClone)
		{
			Monster cha = GameManager.me.cutSceneManager.cutSceneCharacterDic[name].clone();

#if UNITY_EDITOR
			cha.cTransform.name = "CUTSCENE " + cha.cTransform.name;
#endif

			GameManager.me.cutSceneManager.cutScenePlayCharacter[name].Add( cha );
			if(canSetPos) cha.cTransform.position = pos;
			if(canSetRot) cha.rotation = rot;

			cha.setParent( GameManager.me.mapManager.mapStage );

			if(cha.ani != null && cha.ani.clip != null && (cha.ani.clip.name == Monster.NORMAL || cha.ani.clip.name == Monster.DEFAULT_NORMAL))
			{
				cha.playCSAni(cha.getCutSceneNormal(), WrapMode.Loop, true);
			}
		}
		else // 주인공 히어로의 경우...
		{
			GameManager.me.cutSceneManager.cutScenePlayCharacter[name].Clear();
			GameManager.me.cutSceneManager.cutScenePlayCharacter[name].Add(GameManager.me.cutSceneManager.cutSceneCharacterDic[name].cha);
			if(canSetPos) GameManager.me.cutSceneManager.cutSceneCharacterDic[name].cha.cTransform.position = pos;
			if(canSetRot) GameManager.me.cutSceneManager.cutSceneCharacterDic[name].cha.rotation = rot;

			GameManager.me.cutSceneManager.cutSceneCharacterDic[name].cha.setParent( GameManager.me.mapManager.mapStage );
		}
	}	
	
}


sealed public class CutSceneDataObjectPlay : CutSceneDataElement
{

	/*
* 배치된 3D 개체 애니메이션 플레이
* 동일한 개체명을 가진 모든 3D개체에 적용 (이하 동일)
> 속성1 : 개체이름
> 속성2 : 애니메이션 아이디
> 속성3 : 플레이횟수 (0 : 무한)
> 속성4 : 0 or 1 (1: 애니메이션 플레이완료 후, IDLE 애니메이션으로 전환, 0: 애니메이션 마지막 프레임 유지)
	*/
	public string name;
	public string aniId;
	public int playNum;
	public int actionOnComplete;


	sealed public override void destroy ()
	{
		name = null;
		aniId = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
		
		aniId = (string)attrs[1];
		Util.parseObject(attrs[2],out playNum, true, 0);
		Util.parseObject(attrs[3],out actionOnComplete, true, 0);
	}	
	
	
	sealed public override void startAction()
	{
//		Debug.Log(name);

		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(name) == false)
		{
#if UNITY_EDITOR
			Debug.LogError("Fix this~!!!!!!!!");
#endif
			return;
		}

		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[name])
		{
			cha.playCutSceneAni(aniId, playNum, playNum, actionOnComplete);
			if(aniId == Monster.DEAD || aniId == Monster.SCENE_DEAD) cha.csAttr = 0;
		}
	}	
	
}



sealed public class CutSceneDataObjectFaceAnimationPlay : CutSceneDataElement
{
	
	/*
* 배치된 3D 개체 애니메이션 플레이
* 동일한 개체명을 가진 모든 3D개체에 적용 (이하 동일)
> 속성1 : 개체이름
> 속성2 : 애니메이션 아이디
> 속성3 : 플레이횟수 (0 : 무한)
> 속성4 : 0 or 1 (1: 애니메이션 플레이완료 후, IDLE 애니메이션으로 전환, 0: 애니메이션 마지막 프레임 유지)
	*/
	public string name;
	public string aniId;

	public override void destroy ()
	{
		name = null;
		aniId = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
		
		aniId = (string)attrs[1];
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(name) == false)
		{
#if UNITY_EDITOR
			Debug.LogError("Fix this~!!!!!!!!" + name);
#endif
			return;
		}

		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[name])
		{
			cha.playFaceAni(aniId);
		}
	}	
	
}




sealed public class CutSceneDataObjectStop : CutSceneDataElement
{
	/*
	정지 (신규)	
	* 이동중인 3D 개체를 정지시키기
	> 속성1 : 개체이름
	*/
	public string name;


	public override void destroy ()
	{
		name = null;
	
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
	}	
	
	
	sealed public override void startAction()
	{
//		Debug.Log(name);

		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(name) == false)
		{
#if UNITY_EDITOR
			Debug.LogError("Fix this~!!!!!!!!");
#endif
			return;
		}

		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[name])
		{
			cha.cutScenePositionTween = Monster.CutSceneTweenType.None;
			cha.isCutSceneRotationTween = false;
			cha.isCutSceneUpdownMotion = false;
		}
	}	
	
}


sealed public class CutSceneDataObjectVisible : CutSceneDataElement
{
	
	/*
안보이기 (신규)			OBJ_VISIBLE
	* 하이드 ON/OFF 설정		
	* 주로 카메라웍에 사용예정		
	> 속성1 : 개체이름		
	> 속성2 : 0 (안보이기) / 1 (보이기)

	[속성3] (추가)
	 * 0 : 전체* 1,A : 자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)
	 * 2,A : 전방N Cm 지점에서 지름 A범위내에 있는 캐릭터들[속성4] (추가)* 알파값 지정 (0~100)

	*/
	public string name;

	public string parts = null;

	public bool isVisible = false;

	const string US_UNIT = "US_UNIT";
	const string US_HERO  = "US_HERO";
	const string US_ENEMY = "US_ENEMY";
	const string US_TARGET = "US_TARGET";
	const string US_VISIBLE = "US_VISIBLE";

	int[] target;

	float alpha = 1.0f;

	public override void destroy ()
	{
		name = null;
	}

	public enum SkillCamType
	{
		None, MyUnit, MyHero, Enemy, Target, VisibleAll
	}

	private SkillCamType _type = SkillCamType.None;

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];

		if(name.Contains("@"))
		{
			parts = name.Substring(name.IndexOf("@")+1);
			name = name.Substring(0, name.IndexOf("@"));
		}

		switch(name)
		{
		case US_UNIT: _type = SkillCamType.MyUnit; 

			break;
		case US_HERO: _type = SkillCamType.MyHero; break;
		case US_ENEMY: _type = SkillCamType.Enemy; 

			break;
		case US_TARGET: _type = SkillCamType.Target; break;
		case US_VISIBLE: _type = SkillCamType.VisibleAll; break;
		default: _type = SkillCamType.None; break;
		}

		int v = 0;
		Util.parseObject(attrs[1],out v, true, 0);
		isVisible = (v == 1);

		target = Util.stringToIntArray(attrs[2].ToString(),',');

		int a = 100;
		Util.parseObject(attrs[3], out a, true, 100);

		alpha = (float)a * 0.01f;

	}	


	static Monster attacker = null;
	static Monster targetMon = null;
	static Vector3 centerPosition = new Vector3();

	sealed public override void startAction()
	{

#if UNITY_EDITOR
		Debug.Log(name);
#endif
		if(_type == SkillCamType.None)
		{
			if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(name) == false) return;

			if(parts != null)
			{
				foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[name])
				{
					cha.setPartsVisible(parts, isVisible);
				}
			}
			else
			{
				foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[name])
				{
					cha.setVisible(isVisible);
				}
			}

		}
		else
		{
			switch(_type)
			{
			case SkillCamType.VisibleAll:
				if(isVisible)
				{
					GameManager.me.characterManager.restoreMonsterVisibleForSkillCam();
				}
				break;

			case SkillCamType.MyUnit:


				// 타겟몬스터나 기준타겟위치를 세팅한다.
				if(target[0] == 1)  //자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)
				{
					if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
					{
						attacker = GameManager.me.player;
					}
					else
					{
						if(UIPlayUnitSlot.lastActiveSkillUseSlotIndex < 0)
						{
							return;
						}
						else 
						{
							Monster lastActiveSkillUseSlotMonster = UIPlay.getUnitSlot(UIPlayUnitSlot.lastActiveSkillUseSlotIndex).mon;

							if(lastActiveSkillUseSlotMonster == null || 
							   lastActiveSkillUseSlotMonster.isEnabled == false || 
							   lastActiveSkillUseSlotMonster.monsterUISlotIndex != UIPlayUnitSlot.lastActiveSkillUseSlotIndex)
							{
								return;
							}
						}

						attacker = GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon;
					}

					foreach(Monster mon in GameManager.me.characterManager.playerMonster)
					{
						if(mon.hp > 0 && mon.isEnabled && attacker != mon)
						{
							if(targetMon != null) mon.distanceBetweenAttacker = VectorUtil.DistanceXZ(targetMon.cTransformPosition, attacker.cTransformPosition);
							if(targetMon == null || mon.distanceBetweenAttacker < targetMon.distanceBetweenAttacker)
							{
								targetMon = mon;
							}
						}
					}
					
					if(targetMon == null) return;
					centerPosition = targetMon.cTransformPosition;
				}
				else if(target[0] == 2) // /전방A Cm 지점에서 지름 B범위내에 있는 캐릭터들
				{
					if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
					{
						centerPosition = GameManager.me.player.cTransformPosition;
					}
					else
					{
						Monster lastActiveSkillUseMonster = UIPlay.getUnitSlot(UIPlayUnitSlot.lastActiveSkillUseSlotIndex).mon;

						if(lastActiveSkillUseMonster == null || 
						   lastActiveSkillUseMonster.isEnabled == false || 
						   lastActiveSkillUseMonster.monsterUISlotIndex != UIPlayUnitSlot.lastActiveSkillUseSlotIndex)
						{
							return;
						}
						centerPosition = lastActiveSkillUseMonster.cTransformPosition;
					}

					centerPosition.x += target[1];
				}


				// 실제 적용부분.
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.hp > 0 && mon.isEnabled)
					{
						if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
						{
							if(mon.isPlayer) continue;

							switch(target[0])
							{
							case 1: //자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)

								if(targetMon != null)
								{
									if(target[1] == 0)
									{
										if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
									}
									else
									{
										if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
										else if(VectorUtil.DistanceXZ(targetMon.cTransformPosition, mon.cTransformPosition) <= ((float)target[1])*0.5f)
										{
											mon.setVisibleForSkillCam(isVisible, alpha);
										}
									}
								}

								break;
							case 2: //전방A Cm 지점에서 지름 B범위내에 있는 캐릭터들
								//target[1] : a   target[2] : b
								if(VectorUtil.DistanceXZ(centerPosition, mon.cTransformPosition) <= ((float)target[2])*0.5f)
								{
									mon.setVisibleForSkillCam(isVisible, alpha);
								}
								break;
							default:
								mon.setVisibleForSkillCam(isVisible, alpha);
								break;
							}
						}
						else
						{
							if(UIPlayUnitSlot.lastActiveSkillUseSlotIndex != mon.monsterUISlotIndex)
							{
								if(mon.isPlayer == false)
								{
									switch(target[0])
									{
									case 1: //자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)

										if(targetMon != null)
										{
											if(target[1] == 0)
											{
												if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
											}
											else
											{
												if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
												else if(VectorUtil.DistanceXZ(targetMon.cTransformPosition, mon.cTransformPosition) <= ((float)target[1]) * 0.5f)
												{
													mon.setVisibleForSkillCam(isVisible, alpha);
												}
											}
										}

										break;
									case 2: //전방A Cm 지점에서 지름 B범위내에 있는 캐릭터들
										//target[1] : a   target[2] : b
										if(VectorUtil.DistanceXZ(centerPosition, mon.cTransformPosition) <= ((float)target[2]) * 0.5f)
										{
											mon.setVisibleForSkillCam(isVisible, alpha);
										}
										break;
									default:
										mon.setVisibleForSkillCam(isVisible, alpha);
										break;
									}
								}
							}
						}
					}
				}

				break;


			case SkillCamType.Enemy:

				// 타겟몬스터나 기준타겟위치를 세팅한다.
				if(target[0] == 1)  //자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)
				{
					if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
					{
						attacker = GameManager.me.player;
					}
					else
					{
						if(UIPlayUnitSlot.lastActiveSkillUseSlotIndex < 0 ||
						   GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon == null || 
						   GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon.isEnabled == false || 
						   GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon.monsterUISlotIndex != UIPlayUnitSlot.lastActiveSkillUseSlotIndex)
						{
							return;
						}
						
						attacker = GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon;
					}
					
					foreach(Monster mon in GameManager.me.characterManager.monsters)
					{
						if(mon.hp > 0 && mon.isEnabled)
						{
							if(targetMon != null) mon.distanceBetweenAttacker = VectorUtil.DistanceXZ(targetMon.cTransformPosition, attacker.cTransformPosition);
							if(targetMon == null || mon.distanceBetweenAttacker < targetMon.distanceBetweenAttacker)
							{
								targetMon = mon;
							}
						}
					}
					
					if(targetMon == null) return;
					centerPosition = targetMon.cTransformPosition;
				}
				else if(target[0] == 2) // /전방A Cm 지점에서 지름 B범위내에 있는 캐릭터들
				{
					if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
					{
						centerPosition = GameManager.me.player.cTransformPosition;
					}
					else
					{
						if(UIPlay.getUnitSlot(UIPlayUnitSlot.lastActiveSkillUseSlotIndex).mon == null || 
						   UIPlay.getUnitSlot(UIPlayUnitSlot.lastActiveSkillUseSlotIndex).mon.isEnabled == false || 
						   UIPlay.getUnitSlot(UIPlayUnitSlot.lastActiveSkillUseSlotIndex).mon.monsterUISlotIndex != UIPlayUnitSlot.lastActiveSkillUseSlotIndex)
						{
							return;
						}
						centerPosition = GameManager.me.uiManager.uiPlay.UIUnitSlot[UIPlayUnitSlot.lastActiveSkillUseSlotIndex].mon.cTransformPosition;
					}
					
					centerPosition.x += target[1];
				}
				
				
				// 실제 적용부분.
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.hp > 0 && mon.isEnabled)
					{
						switch(target[0])
						{
						case 1: //자신과 가장 가까운 캐릭터 위치에서 지름 A범위내에 있는 캐릭터들 (A를 0으로 세팅시 가장 가까운놈 1마리만)
							
							if(targetMon != null)
							{
								if(target[1] == 0)
								{
									if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
								}
								else
								{
									if(targetMon == mon) mon.setVisibleForSkillCam(isVisible, alpha);
									else if(VectorUtil.DistanceXZ(targetMon.cTransformPosition, mon.cTransformPosition) <= ((float)target[1])*0.5f)
									{
										mon.setVisibleForSkillCam(isVisible, alpha);
									}
								}
							}
							
							break;
						case 2: //전방A Cm 지점에서 지름 B범위내에 있는 캐릭터들
							//target[1] : a   target[2] : b
							if(VectorUtil.DistanceXZ(centerPosition, mon.cTransformPosition) <= ((float)target[2])*0.5f)
							{
								mon.setVisibleForSkillCam(isVisible, alpha);
							}
							break;
						default:
							mon.setVisibleForSkillCam(isVisible, alpha);
							break;
						}
					}
				}
				
				break;

			case SkillCamType.MyHero:

				GameManager.me.player.setVisibleForSkillCam(isVisible, alpha);

				break;





			case SkillCamType.Target:

				if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
				{
					if(CutSceneManager.usingSkillSlotIndex > -1)
					{
						HeroSkillData hsd = GameManager.me.uiManager.uiPlay.uiSkillSlot[CutSceneManager.usingSkillSlotIndex].skillSlot.skillData;

						switch(hsd.targeting)
						{
						case TargetingData.FIXED_1:

							if(hsd.targetType == Skill.TargetType.ME)
							{
								foreach(Monster mon in GameManager.me.characterManager.playerMonster)
								{
									if(mon.isPlayer == false && mon.isEnabled && mon.hp > 0)
									{
										if(VectorUtil.DistanceXZ(mon.cTransformPosition, GameManager.me.player.targetingDecal.targetPosition) <= ((float)hsd.targetAttr[1]) * 0.5f)
										{
											mon.setVisibleForSkillCam(isVisible, alpha);
										}
									}
								}
							}
							else
							{
								foreach(Monster mon in GameManager.me.characterManager.monsters)
								{
									if(mon.isEnabled && mon.hp > 0)
									{
										if(VectorUtil.DistanceXZ(mon.cTransformPosition, GameManager.me.player.targetingDecal.targetPosition) <= hsd.targetAttr[1].Get())
										{
											mon.setVisibleForSkillCam(isVisible, alpha);
										}										
									}
								}
							}

							break;
						case TargetingData.AUTOMATIC_2:

							if(GameManager.me.player.skillTarget != null)
							{
								if(GameManager.me.player.skillTarget.hp > 0 && GameManager.me.player.skillTarget.isEnabled)
								{
									GameManager.me.player.skillTarget.setVisibleForSkillCam(isVisible, alpha);
								}
							}
							break;
						}
					}
				}

				break;


				targetMon = null;
				attacker = null;

			}
		}
	}	
}





sealed public class CutSceneDataObjectMove : CutSceneDataElement
{
/*
	* 지정된 이름의 3D개체를 지정된 좌표로 이동
	* 걷기 애니메이션이 있다면, 해당 좌표로 방향틀어서 이동애니메이션 플레이 (이동완료 후, IDLE 애니메이션 플레이, 방향은 가던방향 유지) << 애니메이션이 있는 경우에만..
	> 속성1 : 개체이름
	> 속성2 : 0 or 1 (절대좌표 or 상대좌표)
	   - 상대좌표 : 해당 캐릭터를 기준 (0,0,0) 으로한 상대좌표
	> 속성3 : 000,000,000 (3D좌표)
	> 속성3 : 이동거리비율 (0~100%) : 100% 일때 해당 좌표로 완전히 이동, 50% 면 중간 지점에서 멈추기
	> 속성5 (추가) :  0 (이동 애니메이션 플레이하지 않음), 1 (이동 애니메이션 플레이)			
*/	
	public string targetName;
	public int posType;
	public Vector3 targetPos;
	public float movePercent;
	public float moveSpeed = 1.0f;
	public int playWalkAni = 0;
	public int useRotate = 1;
	public int normalAfterComplete = 1;


	public override void destroy ()
	{
		targetName = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		Util.parseObject(attrs[1],out posType, true, 0);		
		
		float[] tempF = Util.stringToFloatArray((string)attrs[2],',');
		targetPos.x = tempF[0];
		targetPos.y = tempF[1];
		targetPos.z = tempF[2];	
		
		Util.parseObject(attrs[3],out movePercent, true, 100.0f);	
		
		movePercent *= 0.01f;
		
		Util.parseObject(attrs[4],out moveSpeed, true, 1.0f);	

		Util.parseObject(attrs[5],out playWalkAni, true, 1);	

		Util.parseObject(attrs[6],out useRotate, true, 1);	

		Util.parseObject(attrs[7],out normalAfterComplete, true, normalAfterComplete);	


	}	
	

	private Vector3 _targetPos;
	sealed public override void startAction()
	{


		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName) == false)
		{
//			Debug.Log("targetName : " + targetName);
			return;
		}


		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			cha.cutScenePositionTweenType = 1;
			cha.csStartPos = cha.cTransform.position;
			
			if(posType == 0) // 절대
			{
				_targetPos = targetPos;
			}
			else //상대
			{
				_targetPos = cha.csStartPos + targetPos;
			}

			cha.csTargetPos = _targetPos;
			cha.csTargetDistance = VectorUtil.Distance3D(cha.csStartPos, _targetPos) * movePercent;
			
			// To do... 적을 바라볼때는 괜찮은데... 바닥으로 떨어지거나 할때는 문제가 있을 수 있다.
			//Debug.LogError("startPos : " + cha.csStartPos + "  tPos:" + _targetPos);

			cha.csTweenSpeed = moveSpeed;

			if(playWalkAni == 1) cha.playCSAni(Monster.WALK, WrapMode.Loop);
			//else cha.ani.Stop();

			if(useRotate == 1)
			{
				cha.tf.LookAt(_targetPos); 
				cha.cutScenePositionTween = Monster.CutSceneTweenType.Position;
			}
			else
			{
				cha.cutScenePositionTween = Monster.CutSceneTweenType.PositionWithNoRotation;
			}

			cha.csAttr = normalAfterComplete;

		}
	}	
}






sealed public class CutSceneDataObjectColor : CutSceneDataElement
{
	/*
	 * 
	 * "OBJ_COLOR"
	 * 
	* 지정된 이름의 3D개체의 색상 변경.
	> 속성1 : 개체이름
	> 속성2 : 시작 색상/끝 색상 (시작 색상 생략 가능) 색상 값은 ff00xx 혹은 255,123,123 형식.
	> 속성3 : 시간.
	> 속성4 : easting 속성값.
*/	
	public string targetName;
	public int posType;
	public Vector3 targetPos;
	
	public bool useEasing = false;
	public string easingMethod = null;
	EasingType easingStyle;
	
	public float motionTime = 0.0f;
	
	Color startColor;
	Color targetColor;

	bool hasStartColor = false;

	public override void destroy ()
	{
		targetName = null;
		easingMethod = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		string[] colors = ((string)attrs[1]).Split('/');

		if(colors.Length == 1)
		{
			if(colors[0].Contains(","))
			{
				float[] c1 = Util.stringToFloatArray(colors[0],',');
				targetColor.r = c1[0]/255f;
				targetColor.g = c1[1]/255f;
				targetColor.b = c1[2]/255f;
			}
			else
			{
				targetColor = Util.HexToColor(colors[0]);
			}

			hasStartColor = false;
		}
		else
		{
			if(colors[0].Contains(","))
			{
				float[] c2 = Util.stringToFloatArray(colors[0],',');
				startColor.r = c2[0]/255f;
				startColor.g = c2[1]/255f;
				startColor.b = c2[2]/255f;
			}
			else
			{
				startColor = Util.HexToColor(colors[0]);
			}

			if(colors[1].Contains(","))
			{
				float[] c3 = Util.stringToFloatArray(colors[1],',');
				targetColor.r = c3[0]/255f;
				targetColor.g = c3[1]/255f;
				targetColor.b = c3[2]/255f;
			}
			else
			{
				targetColor = Util.HexToColor(colors[1]);
			}

			hasStartColor = true;


		}


		Util.parseObject(attrs[2],out motionTime, true);	

		string e = (string)attrs[3];
		
		if(string.IsNullOrEmpty(e))
		{
			useEasing = false;
		}
		else
		{
			useEasing = true;
			
			string[] e2 = e.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e2[1]);
			easingMethod = e2[0];
		}
	}	
	
	
	private Vector3 _targetPos;
	sealed public override void startAction()
	{
		
		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName) == false)
		{
			//			Debug.Log("targetName : " + targetName);
			return;
		}
		
		
		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			if(hasStartColor == false)
			{

				cha.csObjectColorTweener.start(motionTime, cha.getColor(), targetColor, useEasing,easingMethod, easingStyle);
			}
			else
			{
				cha.csObjectColorTweener.start(motionTime, startColor, targetColor, useEasing,easingMethod, easingStyle);
			}


		}
	}	
}




sealed public class CutSceneDataObjectMove2 : CutSceneDataElement
{
	/*
	* 지정된 이름의 3D개체를 지정된 좌표로 이동
	* 걷기 애니메이션이 있다면, 해당 좌표로 방향틀어서 이동애니메이션 플레이 (이동완료 후, IDLE 애니메이션 플레이, 방향은 가던방향 유지) << 애니메이션이 있는 경우에만..
	> 속성1 : 개체이름
	> 속성2 : 0 or 1 (절대좌표 or 상대좌표)
	   - 상대좌표 : 해당 캐릭터를 기준 (0,0,0) 으로한 상대좌표
	> 속성3 : 000,000,000 (3D좌표)
	> 속성3 : 이동거리비율 (0~100%) : 100% 일때 해당 좌표로 완전히 이동, 50% 면 중간 지점에서 멈추기
	> 속성5 (추가) :  0 (이동 애니메이션 플레이하지 않음), 1 (이동 애니메이션 플레이)			
*/	
	public string targetName;
	public int posType;
	public Vector3 targetPos;

	public bool useEasing = false;
	public string easingMethod = null;
	EasingType easingStyle;

	public float motionTime = 0.0f;


	public int playWalkAni = 0;
	public int useRotate = 1;
	public int normalAfterComplete = 1;


	public override void destroy ()
	{
		targetName = null;
		easingMethod = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		Util.parseObject(attrs[1],out posType, true, 0);		
		
		float[] tempF = Util.stringToFloatArray((string)attrs[2],',');
		targetPos.x = tempF[0];
		targetPos.y = tempF[1];
		targetPos.z = tempF[2];	
		
		string e = (string)attrs[3];
		
		if(string.IsNullOrEmpty(e))
		{
			useEasing = false;
		}
		else
		{
			useEasing = true;
			
			string[] e2 = e.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e2[1]);
			easingMethod = e2[0];
		}

		Util.parseObject(attrs[4],out motionTime, true);	
		
		Util.parseObject(attrs[5],out playWalkAni, true, 1);	
		
		Util.parseObject(attrs[6],out useRotate, true, 1);	
		
		Util.parseObject(attrs[7],out normalAfterComplete, true, normalAfterComplete);	
		
		
	}	
	
	
	private Vector3 _targetPos;
	sealed public override void startAction()
	{
		
		
		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName) == false)
		{
//			Debug.Log("targetName : " + targetName);
			return;
		}
		
		
		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			cha.cutScenePositionTweenType = 2;

			cha.csStartPos = cha.cTransform.position;
			
			if(posType == 0) // 절대
			{
				_targetPos = targetPos;
			}
			else //상대
			{
				_targetPos = cha.csStartPos + targetPos;
			}
			
			cha.csTargetPos = _targetPos;

			cha.csObjectMove2Tweener.init();
			cha.csObjectMove2Tweener.start(motionTime,cha.csStartPos,_targetPos,useEasing,easingMethod,easingStyle);

			if(playWalkAni == 1) cha.playCSAni(Monster.WALK, WrapMode.Loop);

			if(useRotate == 1)
			{
				cha.tf.LookAt(_targetPos); 
				cha.cutScenePositionTween = Monster.CutSceneTweenType.Position;
			}
			else
			{
				cha.cutScenePositionTween = Monster.CutSceneTweenType.PositionWithNoRotation;
			}
			
			cha.csAttr = normalAfterComplete;
			
		}
	}	
}







sealed public class CutSceneDataObjectUpDown : CutSceneDataElement
{
	/*
둥둥 떠있기	
	* 해당 개체의 Y 위치값을 지정된 범위만큼 지정된 속도로 상하 등속 이동
	* 이동, 애니메이션, 로테이트 등 다른 액션이 진행중일 때에도 계속 상하로 둥둥 유지.. (된다면)
	> 속성1 : 개체이름
	> 속성2 : Y값 범위
	> 속성3 : 속도
	> 속성4 : 1 : 시작 0: 멈춤 // 기본값 1
	> 속성5 : 1 : 애니메이션 그대로 0: 작동중인 애니메이션 멈춤. // 기본값 1

*/	
	public string targetName;
	public float range;
	public float speed;
	public int isPlay = 1;
	public int playAnimation = 0;


	public override void destroy ()
	{
		targetName = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		Util.parseObject(attrs[1],out range, true, 0);		
		Util.parseObject(attrs[2],out speed, true, 0);	

		Util.parseObject(attrs[3],out isPlay, true, 1);	
		Util.parseObject(attrs[4],out playAnimation, true, 1);	

	}	
	
	
	sealed public override void startAction()
	{
		if(isPlay == 1)
		{
			foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
			{
				cha.csUpDownSpeed = speed;
				cha.csUpDownRange = range;
				cha.csUpDownCenter = cha.cTransform.position.y;
				cha.isCutSceneUpdownMotion = true;
				if(playAnimation == 0)
				{
					cha.ani.Stop();
				}
			}
		}
		else
		{
			foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
			{
				cha.isCutSceneUpdownMotion = false;
				if(playAnimation == 0)
				{
					cha.ani.Stop();
				}
			}
		}
	}	
}





sealed public class CutSceneDataObjectRotate : CutSceneDataElement
{

	/*
* 지정된 이름의 3D개체의 방향을 변경
> 속성1 : 개체이름
> 속성2 : 각도
	*/
	
	public string targetName;
	public Vector3 targetRot;
	public float motionTime;


	public override void destroy ()
	{
		targetName = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		float[] tempF = Util.stringToFloatArray((string)attrs[1],',');
		targetRot.x = tempF[0];
		targetRot.y = tempF[1];
		targetRot.z = tempF[2];		
		
		Util.parseObject(attrs[2], out motionTime, true, 0.0f);
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName) == false)
		{
			Debug.LogError("fix this~~~");
			return;
		}

		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			if(motionTime > 0)
			{
				cha.setCSRotation(motionTime, targetRot);
			}
			else
			{
				cha.rotation = targetRot;	
			}
		}
	}	
}


sealed public class CutSceneDataObjectDelete : CutSceneDataElement
{
	/*
	 * 지정된 이름의 모든 3D개체 지우기
	> 속성1 : 개체이름
	*/
	
	public string targetName;
	
	public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
	}	
	
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.removeCutSceneCharacter(targetName);
	}	


	public override void destroy ()
	{
		targetName = null;
	}

}





sealed public class CutSceneDataObjectDamageEffect : CutSceneDataElement
{
	/*
	 * 지정된 이름의 모든 3D개체 데미지 이펙트 주기.
	> 속성1 : 방향. (L,R)
	*/
	
	public string targetName;
	public bool isLeft = true;

	public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];

		if(attrs.Length >= 2)
		{
			isLeft = attrs[1].ToString().Equals("L");
		}
	}	
	
	
	sealed public override void startAction()
	{
		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			cha.cutSceneDamageEffect(isLeft);
		}
	}	
	
	
	public override void destroy ()
	{
		targetName = null;
	}
	
}






sealed public class CutSceneDataObjectResize : CutSceneDataElement
{
	/*
		> 속성1 : 개체이름
		> 속성2 : N%
		> 속성3 : 사이즈변경시간(sec) (0이면 즉시)
		> 속성4 : 사이즈변경 가속도 (생략가능)
	*/

	public string targetName;
	public float size = 100.0f;

	public float motionTime = 0.0f;

	public bool useEasing = false;
	public string easingMethod = null;
	EasingType easingStyle;//MiniTweenEasingType


	public override void destroy ()
	{
		targetName = null;
		easingMethod = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		
		Util.parseObject(attrs[1], out size, true, 100.0f);
		
		size *= 0.01f;

		Util.parseObject(attrs[2], out motionTime, true);

		string e = (string)attrs[3];

		if(string.IsNullOrEmpty(e))
		{
			useEasing = false;
		}
		else
		{
			useEasing = true;

			string[] e2 = e.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e2[1]);
			easingMethod = e2[0];
		}
	}	
	
	
	sealed public override void startAction()
	{
		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			if(motionTime > 0)
			{
				cha.csScaleTweener.start(motionTime, cha.cTransform.localScale.x, cha.cTransform.localScale.x * size, useEasing, easingMethod, easingStyle);
			}
			else
			{
				cha.cTransform.localScale = cha.cTransform.localScale * size;
			}
		}
	}	
}







sealed public class CutSceneDataSetEffect : CutSceneDataElement
{
/*	
* 로딩된 3D 개체를 1개 배치함, 기본적으로 IDLE 애니메이션 플레이 (있다면..)
> 속성1 : 개체이름
> 속성2 : 3D 좌표 (xxxx,yyyy,zzzz)
*/	
	
	public string name;
	public Vector3 pos;
	public Vector3 rot;
	
	public bool canSetPos = true;
	public bool canSetRot = true;


	public override void destroy ()
	{
		name = null;
	}
	
	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
		
		canSetPos =((string)attrs[1]).Contains(",");
		canSetRot =((string)attrs[2]).Contains(",");
		
		float[] tempF;
		if(canSetPos)
		{
			tempF = Util.stringToFloatArray((string)attrs[1],',');
			pos.x = tempF[0];
			pos.y = tempF[1];
			pos.z = tempF[2];
		}
		
		if(canSetRot)
		{
			tempF = Util.stringToFloatArray((string)attrs[2],',');
			rot.x = tempF[0];
			rot.y = tempF[1];
			rot.z = tempF[2];
		}
	}	
	
	private Vector3 _v;
	
	sealed public override void startAction()
	{
		//GameManager.me.cutSceneManager.cutSceneEffectDic[name].effectData.getEffect(pos);
		if(GameManager.me.cutSceneManager.cutScenePlayEffect.ContainsKey(name) == false)
		{
			GameManager.me.cutSceneManager.cutScenePlayEffect[name] = new List<Effect>();
		}
		
		Effect eff = GameManager.me.cutSceneManager.cutSceneEffectDic[name].effectData.getCutSceneEffect();
		GameManager.me.cutSceneManager.cutScenePlayEffect[name].Add(eff);
		
		if(canSetPos) eff.tf.position = pos;
		if(canSetRot) eff.rotation = rot;
	}		
}


sealed public class CutSceneDataEffectPlay : CutSceneDataElement
{
	/*
* 배치된 3D 개체 애니메이션 플레이
* 동일한 개체명을 가진 모든 3D개체에 적용 (이하 동일)
> 속성1 : 개체이름
> 속성2 : 애니메이션 아이디
> 속성3 : 플레이횟수 (0 : 무한)
> 속성4 : 0 or 1 (1: 애니메이션 플레이완료 후, IDLE 애니메이션으로 전환, 0: 애니메이션 마지막 프레임 유지)
	*/
	public string name;

	public override void destroy ()
	{
		name = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			name = attrs[0].ToString();
		}
		else name = (string)attrs[0];
	}
	
	
	sealed public override void startAction()
	{
		
	}	
	
}


sealed public class CutSceneDataEffectMove : CutSceneDataElement
{
/*
	* 지정된 이름의 3D개체를 지정된 좌표로 이동
	* 걷기 애니메이션이 있다면, 해당 좌표로 방향틀어서 이동애니메이션 플레이 (이동완료 후, IDLE 애니메이션 플레이, 방향은 가던방향 유지) << 애니메이션이 있는 경우에만..
	> 속성1 : 개체이름
	> 속성2 : 0 or 1 (절대좌표 or 상대좌표)
	   - 상대좌표 : 해당 캐릭터를 기준 (0,0,0) 으로한 상대좌표
	> 속성3 : 000,000,000 (3D좌표)
	> 속성3 : 이동거리비율 (0~100%) : 100% 일때 해당 좌표로 완전히 이동, 50% 면 중간 지점에서 멈추기
*/	
	public string targetName;
	public int posType;
	public Vector3 targetPos;
	public float movePercent;
	public float moveSpeed = 1.0f;
	
	private Vector3 _targetPos;


	public override void destroy ()
	{
		targetName = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		
		Util.parseObject(attrs[1],out posType, true, 0);		
		
		float[] tempF = Util.stringToFloatArray((string)attrs[2],',');
		targetPos.x = tempF[0];
		targetPos.y = tempF[1];
		targetPos.z = tempF[2];	
		
		Util.parseObject(attrs[3],out movePercent, true, 0.0f);	
		
		movePercent *= 0.01f;
		
		Util.parseObject(attrs[4],out moveSpeed, true, 1.0f);			
		
	}
	
	
	sealed public override void startAction()
	{
//		Debug.LogError(targetName);

		foreach(Effect eff in GameManager.me.cutSceneManager.cutScenePlayEffect[targetName])
		{
			eff.cutSceneInit();
			eff.csStartPos = eff.tf.position;
			
			if(posType == 0) // 절대
			{
				_targetPos = targetPos;
			}
			else //상대
			{
				_targetPos = eff.csStartPos + targetPos;
			}
			
			eff.csTargetPos = _targetPos;
			eff.csTargetDistance = VectorUtil.Distance3D(eff.csStartPos, _targetPos) * movePercent;
			eff.csTweenSpeed = moveSpeed;
			eff.isCutScenePositionTween = true;
		}		
	}	
}


sealed public class CutSceneDataEffectRotate : CutSceneDataElement
{
	/*
* 지정된 이름의 3D개체의 방향을 변경
> 속성1 : 개체이름
> 속성2 : 각도
	*/
	
	public string targetName;
	public Vector3 targetRot;
	public float motionTime;

	public override void destroy ()
	{
		targetName = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		float[] tempF = Util.stringToFloatArray((string)attrs[1],',');
		targetRot.x = tempF[0];
		targetRot.y = tempF[1];
		targetRot.z = tempF[2];		
		
		Util.parseObject(attrs[2], out motionTime, true, 0.0f);
	}	
	
	
	sealed public override void startAction()
	{
		foreach(Effect eff in GameManager.me.cutSceneManager.cutScenePlayEffect[targetName])
		{
			eff.cutSceneInit();
			
			if(motionTime > 0)
			{
				eff.setCSRotation(motionTime, targetRot);
			}
			else
			{
				eff.rotation = targetRot;	
			}
		}			
	}	
}


sealed public class CutSceneDataEffectDelete : CutSceneDataElement
{
	/*
	 * 지정된 이름의 모든 3D개체 지우기
	> 속성1 : 개체이름
	*/
	public string targetName;

	public override void destroy ()
	{
		targetName = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		targetName = (string)attrs[0];
	}	
	
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.removeCutSceneEffect(targetName);
	}	
}


sealed public class CutSceneDataEffectResize : CutSceneDataElement
{
	/*
	크기변경 (신규)		
		* 해당 이펙트의 전체 크기를 변경 	
		> 속성1 : 개체이름	
		> 속성2 : N%	
	*/
	public string targetName;
	public float size = 100.0f;


	public override void destroy ()
	{
		targetName = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		

		Util.parseObject(attrs[1], out size, true, 100.0f);

		size *= 0.01f;
	}	
	
	
	sealed public override void startAction()
	{
		if(GameManager.me.cutSceneManager.cutScenePlayEffect.ContainsKey(targetName))
		{
			foreach(Effect eff in GameManager.me.cutSceneManager.cutScenePlayEffect[targetName])
			{
				eff.resize(size);
			}			
		}
	}	
}





sealed public class CutSceneDataSetImage : CutSceneDataElement
{
/*
* 지정된 이름의 2D 이미지를 특정 화면좌표에 표시
> 속성1 : 이미지이름
> 속성2 : 지속시간 (0:무한)
	 */
	
	public string id;
	public Vector2 pos = new Vector2();
	public float duration;

	public override void destroy ()
	{
		id = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		id = (string)attrs[0];
		float[] tempF = Util.stringToFloatArray((string)attrs[1],',');
		pos.x = tempF[0];
		pos.y = tempF[1];		
	}	
	
	
	sealed public override void startAction()
	{
		
	}	
	
}


sealed public class CutSceneDataImageDelete : CutSceneDataElement
{
/* 지정된 이름의 2D 이미지를 삭제
> 속성1 : 이미지이름
*/	
	
	public string id;


	public override void destroy ()
	{
		id = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		id = (string)attrs[0];
	}	
	
	
	sealed public override void startAction()
	{
		
	}	
	
}


sealed public class CutSceneDataIllustText : CutSceneDataElement
{

	/*
* 특정 캐릭터의 얼굴 일러스트와 함께 화면 한쪽에 표시
> 속성1 : 일러스트ID
> 속성2 : 위치 (0~3) : 좌상단,우상단,좌하단,우하단
> 속성3 : 지속시간 (ms) (0 : 터치하면 넘어감)
> 속성4 : 대사
	*/
	
	public string id;
	public int posType;
	public float duration;
	public string textId;


	public override void destroy ()
	{
		id = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		id = (string)attrs[0];
		Util.parseObject(attrs[1],out posType, true, 0);
		Util.parseObject(attrs[2],out duration, true, 0.0f);
		textId = (string)attrs[3];
	}	
	
	
	sealed public override void startAction()
	{
		
	}	
	
}


sealed public class CutSceneDataTooltip : CutSceneDataElement
{
/*
* 3D 개체 위에 말풍선으로 대사 표시
> 속성1 : 3D 개체이름
> 속성2 : 위치 (캐릭터 기준 2D 화면 상대 좌표) (캐릭터 이동시 따라다님)
> 속성3 : 지속시간 (ms)
> 속성4 : 대사
		 */
	public string targetName;
	public float posX;
	public float posY;
	public float duration;
	public string textId;

	const string PLAYER = "PLAYER";


	public override void destroy ()
	{
		targetName = null;
		textId = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( attrs[0] is int || attrs[0] is long)
		{
			targetName = attrs[0].ToString();
		}
		else targetName = (string)attrs[0];
		
		float[] pos = Util.stringToFloatArray((string)attrs[1],',');
		posX = pos[0];
		posY = pos[1];
		
		Util.parseObject(attrs[2],out duration, true, 0.0f);

//		Debug.LogError(attrs[2] + "   duration : " + duration);

		textId = (string)attrs[3];		
	}
	
	
	sealed public override void startAction()
	{
		//if(GameManager.me.characterManager.inGameGUITooltipContainer.gameObject.activeSelf == false)
		{
			GameManager.me.characterManager.inGameGUITooltipContainer.gameObject.SetActive(true);
		}

		foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
		{
			if(cha.toolTip == null)
			{
				cha.toolTip = GameManager.me.characterManager.getTooltip(cha.cTransform);
			}
			
			cha.toolTip.xPos = posX;
			cha.toolTip.yPos = posY;
			cha.toolTip.setData( Util.getText(textId), duration);
		}		
	}	
	
}



sealed public class CutSceneDataTooltip2 : CutSceneDataElement
{
	/*
* 3D 개체 위에 말풍선으로 대사 표시
> 속성1 : 3D 개체이름
> 속성2 : 위치 (캐릭터 기준 2D 화면 상대 좌표) (캐릭터 이동시 따라다님)
> 속성3 : 지속시간 (ms)
> 속성4 : 대사
		 */
	public string iconId;
	public string position;
	public float duration;
	public string textId;


	public override void destroy ()
	{
		textId = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		iconId = (string)attrs[0];		
		position = (string)attrs[1];		
		Util.parseObject(attrs[2],out duration, true, 0.0f);
		textId = (string)attrs[3];		
	}
	
	
	sealed public override void startAction()
	{
		GameManager.me.characterManager.inGameGUITooltipContainer.gameObject.SetActive(true);

		GameManager.me.cutSceneManager.dialogBox.setData((position == "L"), textId, iconId, duration);

		SoundManager.instance.playCutSceneVoice(textId);
	}	
}




sealed public class CutSceneDataRoundControl : CutSceneDataElement
{
/*	
* 라운드 스타트/클리어/실패 시킴
>속성1 : 0~2 (0:스타트, 1:클리어, 2:실패, 3:인게임 PAUSE, 4:인게임 RESUME)
* 해당 이벤트가 발생되더라도, 컷씬이 종료되지 않고 끝까지 감

*/
	public int type;
	public string[] data;
	public int seed;


	public override void destroy ()
	{
		data = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out type, true, 0);
		data = ((string)attrs[1]).Split(',');
		Util.parseObject(attrs[2],out seed, true, -1);
	}	



	string[] units = new string[5];
	string[] skills = new string[3];
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case 0:// 스타트.

			if(CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.After) return;

			if(data != null && data.Length > 5)
			{
				GamePlayerData gpd = new GamePlayerData(data[0]);

				//"LEO,9,LEO_HD1_1_1_N_0,LEO_BD1_1_1_N_0,LEO_BD1_1_1_N_0,LEO_RD1_1_1_N_2,UN1,UN2,UN3,UN4,UN5,SK_A031_N,SK_A045_R,SK_A040_S,ai_0";

				int lv = 1;
				int.TryParse(data[1], out lv);

				units[0] = data[6];
				units[1] = data[7];
				units[2] = data[8];
				units[3] = data[9];
				units[4] = data[10];

				skills[0] = data[11];
				skills[1] = data[12];
				skills[2] = data[13];

				Vector3 prevPos = GameManager.me.player.cTransform.position;
				Vector3 prevPos2 = GameManager.me.player.cTransformPosition;

				DebugManager.instance.setPlayerData(gpd,true,data[0],data[2],data[3],data[4],data[5],units,skills,data[14],data[15]);
				GameManager.me.changeMainPlayer(gpd,gpd.id,gpd.partsVehicle.parts.resource.ToUpper());
				GameManager.me.player.isEnabled = true;

				GameManager.me.player.cTransform.position = prevPos;
				GameManager.me.player.cTransformPosition = prevPos2;
			}

			CutSceneManager.introCutSceneStartTime = CutSceneManager.cutScenePlayTime;

#if UNITY_EDITOR

			Debug.LogError("introCutSceneStartTime : " + CutSceneManager.introCutSceneStartTime);
#endif

			GameManager.me.uiManager.uiPlay.resetUI();

			GameManager.me.startBattle((seed > -1),seed);

			break;
		case 1: // 클리어
			GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
			break;
		case 2: // 실패.
			GameManager.me.startGameOver();
			break;
		case 3: // 인게임 루프 멈춤. (모든 캐릭터 동작 normal로 변경한다.)
			GameManager.me.cutSceneManager.stopLoopInGame = true;
			
			foreach(Monster mon in GameManager.me.characterManager.monsters)
			{
				// 현재 하던 애니메이션 동작이 있으면 멈추길 기다렸다 해야하나?
				//mon.animation.PlayQueued(Monster.NORMAL);
				// 아님 그냥 바로 멈춰버려야하나?
				mon.state = Monster.NORMAL;
				mon.animation.PlayQueued(Monster.NORMAL);
			}
			
			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				mon.state = Monster.NORMAL;
				mon.animation.PlayQueued(Monster.NORMAL);
			}			
			
			GameManager.me.player.state = Monster.NORMAL;
			
			break;
		case 4: // 인게임 루프 재개.
			GameManager.me.cutSceneManager.stopLoopInGame = true;
			break;
		}
	}	
	
}
	
	

sealed public class CutSceneDataCamShot : CutSceneDataElement
{
/*	
* 설정된 위치에서 대상을 쳐다봄
* 인플레이 중일 때는, 컷씬의 카메라 이벤트를 우선적으로 처리, 컷씬 종료 후 기본 카메라 세팅으로 변경
> 속성1 : (카메라위치) 3D 좌표 (xxxx,yyyy,zzzz)
> 속성2 : (대상) 3D 개체이름
> 속성3 : (화면위치) 2D 좌표 (xxx,yyy)
> 속성4 : (줌) FOV 값
> 속성5 : (대상이동옵션) 0 or 2
* 대상이 이동할 때
   - 0 : 카메라가 따라 움직이거나 회전하지 않고 그냥 그대로 멈춰 있음
   - 1 : 카메라 방향, 줌 정도를 고정한 채 대상이 설정된 화면위치에 유지되도록 대상의 이동과 똑같이 카메라 이동
   - 2 : 카메라 위치, 줌 정도를 고정한 채 대상이 설정된 화면위치에 유지되도록 방향만 이동
 이후 모든 움직임(이동,애니메이션 등)이 설정된 비율만큼 느리게 진행됨
* 단, 컷씬타임은 제대로 흐르도록
> 속성1 : 0~100 (%)
* 한번 설정되면 100%로 다시 세팅하기 전까지 지속
*/
	
	public Vector3 camPos = new Vector3(); // 카메라 회전값.
	public string targetName; 
	public Vector2 camCenter = new Vector2(); // 카메라 중심.
	public float fov;
	public int type;
	
	public Vector3 camRot = new Vector2();
	
	bool setCamRot = false; 
	bool setCamPos = false; 
	bool setTarget = false; // 
	bool setCamCenter = false;
	bool setFov = false;
	bool setMoveType = false;	

	public bool useHandHeld = false;
	public Vector3 handHeldEffect = new Vector3();


	public override void destroy ()
	{
		targetName = null;
		target = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		if( String.IsNullOrEmpty ((string)attrs[0]))
		{
			setCamPos = false;
		}
		else
		{
			float[] _cp = Util.stringToFloatArray((string)attrs[0],',');
			camPos.x = _cp[0];
			camPos.y = _cp[1];
			camPos.z = _cp[2];
			setCamPos = true;
		}
		
		if( attrs[1] is int || attrs[1] is long)
		{
			targetName = attrs[1].ToString();
			setTarget = true;
		}
		else
		{
			if( String.IsNullOrEmpty ((string)attrs[1]))
			{
				setTarget = false;
			}
			else
			{
				targetName = (string)attrs[1];
				setTarget = true;
			}		
		}
		
		
		if( String.IsNullOrEmpty ((string)attrs[2]))
		{
			setCamCenter = false;
		}
		else
		{
			float[] _cc = Util.stringToFloatArray((string)attrs[2],',');
			camCenter.x = _cc[0];
			camCenter.y = _cc[1];
			setCamCenter = true;
		}			
		
		Util.parseObject(attrs[3],out fov, true, -1.0f);
		
		setFov = (fov > 0.0f);
		
		Util.parseObject(attrs[4],out type, true, -1);		
		
		setMoveType = (type > -1);	
		
		
		if( String.IsNullOrEmpty ((string)attrs[5]))
		{
			setCamRot = false;
		}
		else
		{
			float[] _cp = Util.stringToFloatArray((string)attrs[5],',');
			camRot.x = _cp[0];
			camRot.y = _cp[1];
			camRot.z = _cp[2];
			setCamRot = true;
		}		

		string hstr = attrs[6].ToString();
		if( String.IsNullOrEmpty ( hstr ) || hstr.Contains(",") == false)
		{
			useHandHeld = false;
		}
		else
		{
			float[] _hh = Util.stringToFloatArray((string)attrs[6],',');
			handHeldEffect.x = _hh[0]; // x 범위값.
			handHeldEffect.y = _hh[1]; // y 범위값.
			handHeldEffect.z = _hh[2]; // speed.
			useHandHeld = true;
		}	

		//	* 카메라 흔들기
		//	* 정해진 X,Y 범위값 내에서 랜덤하게 카메라 위치만 지정된 속도로 이동
		//	* 바라보는 카메라 각도(카메라회전값) 에는 영향을 미치지 않음
		//	> 속성7 : X범위값, Y범위값, 속도
	}	
	
	Transform target;
	sealed public override void startAction()
	{
		target = null;
		GameManager.me.cutSceneManager.useCutSceneCamera = true;
		
		UIPlay.useUnitSkillCamTargetPosition = false;
		
		if(setTarget)
		{
			if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.None)
			{
				if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName))
				{
					foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
					{
						target = cha.cTransform;
						break;
					}			
				}
			}
			else
			{
				if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play && targetName == "TARGET") 
				{
					UIPlay.useUnitSkillCamTargetPosition = true;
				}
			}
		}


		
		GameManager.me.uiManager.uiPlay.setCutSceneCamera(target, setCamCenter, camCenter, setCamPos, camPos, fov, setCamRot, camRot,  type);

		if(useHandHeld)
		{
			GameManager.me.uiManager.uiPlay.setHandHeldEffect(handHeldEffect);
		}

		target = null;
	}	
	
}




sealed public class CutSceneDataCamMove : CutSceneDataElement
{
/*
* 현재 세팅(위치,방향,줌)에서 설정된 세팅으로 변경
> 속성1 : (카메라위치) 3D 좌표 (xxxx,yyyy,zzzz)
> 속성2 : (대상) 3D 개체이름
> 속성3 : (화면위치) 2D 좌표 (xxx,yyy)
> 속성4 : (줌) FOV 값
> 속성5 : (대상이동옵션) 0 or 2
   - 카메라무브 완료 후 대상 이동 시 옵션
> 속성6 : (변경속도) ms
   - 설정된 시간동안 변경하기
* 카메라 이동 중, 대상이 이동하는 경우… ;
 설정된 위치에서 대상을 쳐다봄
* 인플레이 중일 때는, 컷씬의 카메라 이벤트를 우선적으로 처리, 컷씬 종료 후 기본 카메라 세팅으로 변경
> 속성1 : (카메라위치) 3D 좌표 (xxxx,yyyy,zzzz)
> 속성2 : (대상) 3D 개체이름
> 속성3 : (화면위치) 2D 좌표 (xxx,yyy)
> 속성4 : (줌) FOV 값
> 속성5 : (대상이동옵션) 0 or 2
* 대상이 이동할 때
   - 0 : 카메라가 따라 움직이거나 회전하지 않고 그냥 그대로 멈춰 있음
   - 1 : 카메라 방향, 줌 정도를 고정한 채 대상이 설정된 화면위치에 유지되도록 대상의 이동과 똑같이 카메라 이동
   - 2 : 카메라 위치, 줌 정도를 고정한 채 대상이 설정된 화면위치에 유지되도록 방향만 이동

 */
	
	public string targetName;
	public Vector3 camPos = new Vector3();
	public Vector3 camRot = new Vector3();	
	public Vector2 camCenter = new Vector2();
	public float fov;
	public int typeNext;	
	public int typeImmidetly;
	public float motionTime;
	public string easingType = null;

	public bool useHandHeld = false;
	public Vector3 handHeldEffect = new Vector3();

	
	bool setCamPos = false;
	bool setTarget = false;
	bool setCamCenter = false;
	bool setCamRot = false;
	bool setFov = false;


	public override void destroy ()
	{
		targetName = null;
		easingType = null;
	}

	sealed public override void setData(params object[] attrs)
	{
		if( String.IsNullOrEmpty ((string)attrs[0]))
		{
			setCamPos = false;
		}
		else
		{
			float[] _cp = Util.stringToFloatArray((string)attrs[0],',');
			camPos.x = _cp[0];
			camPos.y = _cp[1];
			camPos.z = _cp[2];
			setCamPos = true;
		}
		
		
		
		if( attrs[1] is int || attrs[1] is long)
		{
			targetName = attrs[1].ToString();
			setTarget = true;
		}
		else
		{
			if( String.IsNullOrEmpty ((string)attrs[1]))
			{
				setTarget = false;
			}
			else
			{
				targetName = (string)attrs[1];
				setTarget = true;
			}		
		}



		
		if( String.IsNullOrEmpty ((string)attrs[2]))
		{
			setCamCenter = false;
			useHandHeld = false;
		}
		else
		{
			string[] at2 = ((string)attrs[2]).Split('/');

			if(String.IsNullOrEmpty (at2[0]))
			{
				setCamCenter = false;
			}
			else
			{
				float[] _cc = Util.stringToFloatArray(at2[0],',');
				camCenter.x = _cc[0];
				camCenter.y = _cc[1];
				setCamCenter = true;
			}

			if(at2.Length == 2)
			{

				if( String.IsNullOrEmpty ( at2[1] ) || at2[1].Contains(",") == false)
				{
					useHandHeld = false;
				}
				else
				{
					float[] _hh = Util.stringToFloatArray(at2[1],',');
					handHeldEffect.x = _hh[0]; // x 범위값.
					handHeldEffect.y = _hh[1]; // y 범위값.
					handHeldEffect.z = _hh[2]; // speed.
					useHandHeld = true;
				}	

			}
		}			



		Util.parseObject(attrs[3],out fov, true, -1.0f);
		
		setFov = (fov > 0.0f);
		
		typeImmidetly = -1;
		
		if(attrs[4] is string && ((string)attrs[4]).Contains(","))
		{
			int[] t = Util.stringToIntArray((string)attrs[4], ',');
			typeImmidetly = t[0];
			typeNext = t[1];			
		}
		else
		{
			Util.parseObject(attrs[4],out typeNext, true, -1);		
		}
		
		setCamRot = false;
		if( String.IsNullOrEmpty ((string)attrs[5]))
		{
			setCamRot = false;
		}
		else
		{
			float[] _cp = Util.stringToFloatArray((string)attrs[5],',');
			camRot.x = _cp[0];
			camRot.y = _cp[1];
			camRot.z = _cp[2];
			setCamRot = true;
		}				
		
		
		Util.parseObject(attrs[6],out motionTime, true, 0);		

		easingType = (string)attrs[7];
	}
	
	Transform target;
	sealed public override void startAction()
	{
		target = null;
		
		GameManager.me.cutSceneManager.useCutSceneCamera = true;

		UIPlay.useUnitSkillCamTargetPosition = false;

		if(setTarget)
		{
			if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.None)
			{
				if(GameManager.me.cutSceneManager.cutScenePlayCharacter.ContainsKey(targetName))
				{
					foreach(Monster cha in GameManager.me.cutSceneManager.cutScenePlayCharacter[targetName])
					{
						target = cha.cTransform;
						break;
					}			
				}
			}
			else
			{
				if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play && targetName == "TARGET") 
				{
					UIPlay.useUnitSkillCamTargetPosition = true;
				}
			}
		}
		
		GameManager.me.uiManager.uiPlay.setCutSceneCameraMove(target, setCamCenter, camCenter, setCamPos, camPos, setCamRot, camRot, fov, motionTime, typeNext, typeImmidetly, easingType);


		if(useHandHeld)
		{
			GameManager.me.uiManager.uiPlay.useHandHeldAfterCameraMoving = useHandHeld;
			GameManager.me.uiManager.uiPlay.handHeldAfterCameraMovingValue = handHeldEffect;
		}

		target = null;
	}	
	
}




sealed public class CutSceneDataCameraType : CutSceneDataElement
{
	
/*
* 컷씬 중 게임 세팅된 카메라를 사용할 것인지 컷씬용 카메라를 사용할 것인지.
> 속성1 : 0-1 (0:컷씬 카메라, 1:게임카메라)
*/
	public int type;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out type, true, 0);
	}
	
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case 0:
			GameManager.me.cutSceneManager.useCutSceneCamera = true;
			break;
		case 1:
			GameManager.me.cutSceneManager.useCutSceneCamera = false;
			break;
		case 2:
			GameManager.me.cutSceneManager.useCutSceneCamera = false;
			GameManager.me.uiManager.uiPlay.resetCamera();
			break;		
		case 3:
			//GameManager.me.cutSceneManager.useCutSceneCamera = false;
			//GameManager.me.uiManager.uiPlay.resetCamera();
			break;	
		}
	}	


	public override void destroy ()
	{

	}

}



sealed public class CutSceneDataCamAround : CutSceneDataElement
{
	Vector3 targetStartPos = new Vector3(); // 속성1> 기준점좌표 [x,y,z(/x,y,z)] : 기준점 시작좌표 / 기준점 끝좌표(생략가능)
	Vector3 targetEndPos = new Vector3();
	bool useTargetEndPos = false;

	Vector3 targetStartScreenPos = new Vector3(); // 속성2> 화면좌표A [x,y(/x,y)] : 기준점을 위치시킬 화면좌표(시작) / 기준점을 위치시킬 화면좌표(끝)
	Vector3 targetEndScreenPos = new Vector3();
	bool useTargetEndScreenPos = false;

	float moveTime = 0.0f; // 속성3> 카메라이동시간 (sec) : 설정된 시간동안 카메라 이동

	int finalCamYpos = -1; //속성4> 최종카메라 높이(y)값 : 현재 높이값에서 설정된 높이값으로 변경 (0 세팅시 높이 변화 없음)

	float distanceTweeningValue = 0.0f; //속성5> 기준점과의 거리변경(%) : 기준점과의 현재거리 대비 N%만큼 거리를 줄이거나 늘림 (y축은 무시, x,z 값만 가지고 기준점과의 거리를 계산)

	int rotValue = 0; //속성6> 회전각도 : 오른쪽 (or 왼쪽) 이동시 양수 / 반대쪽 이동은 음수로 지정 (예 : -540 세팅시 왼쪽으로 한바퀴반 돌기)

	string easing = null; // 속성7> 가속도 : CAM_MOVE 와 동일

	public override void destroy ()
	{
		easing = null;
	}


	sealed public override void setData(params object[] attrs)
	{
		string[] a1 = ((string)attrs[0]).Split('/');
		float[] v1 = Util.stringToFloatArray(a1[0],',');
		targetStartPos.x = v1[0]; targetStartPos.y = v1[1]; targetStartPos.z = v1[2];

		if(a1.Length > 1)
		{
			v1 = Util.stringToFloatArray(a1[1],',');
			targetEndPos.x = v1[0]; targetEndPos.y = v1[1]; targetEndPos.z = v1[2];
			useTargetEndPos = true;
		}
		else useTargetEndPos = false;

		a1 = ((string)attrs[1]).Split('/');

		if(string.IsNullOrEmpty(a1[0]) == false)
		{
			v1 = Util.stringToFloatArray(a1[0],',');
			targetStartScreenPos.x = v1[0]; targetStartScreenPos.y = v1[1];
			targetStartScreenPos.z = 1000.0f;
		}
		else
		{
			targetStartScreenPos.z = -1000.0f;
		}


		if(a1.Length > 1)
		{
			v1 = Util.stringToFloatArray(a1[1],',');
			targetEndScreenPos.x = v1[0]; targetEndScreenPos.y = v1[1];
			useTargetEndScreenPos = true;
		}
		else useTargetEndScreenPos = false;

		Util.parseObject(attrs[2], out moveTime, true, 0);

		Util.parseObject(attrs[3], out finalCamYpos, true, 0);

		float dv = 100.0f;
		Util.parseObject(attrs[4], out dv, true, 100.0f);

		distanceTweeningValue = dv * 0.01f;


		Util.parseObject(attrs[5], out rotValue, true, 0);

		easing = attrs[6].ToString().Trim();

		if(string.IsNullOrEmpty(easing))
		{
			easing = null;
		}
	}
	
	
	sealed public override void startAction()
	{
		UIPlay.usePlayerPositionOffsetWhenSkillEffectCam = true; // true를 해도 아니면 자동으로 false로 처리된다.

		GameManager.me.uiManager.uiPlay.camAround.start(targetStartPos,targetEndPos,useTargetEndPos,targetStartScreenPos,
		                                                targetEndScreenPos,useTargetEndScreenPos,moveTime,finalCamYpos,distanceTweeningValue,rotValue,easing);
	}	
}




sealed public class CutSceneDataReturnInGameCamera : CutSceneDataElement
{
	float moveTime = 0.0f; // 속성 1. 
	string easing = null;  // 속성 2.
	int showMap = WSDefine.NO;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0], out moveTime, true, 0);
		
		easing = attrs[1].ToString().Trim();
		
		if(string.IsNullOrEmpty(easing))
		{
			easing = null;
		}

		Util.parseObject(attrs[2], out showMap, WSDefine.NO);
	}
	
	
	sealed public override void startAction()
	{
		GameManager.me.uiManager.uiPlay.backToGameCamFromSkillCam(moveTime, easing, showMap == WSDefine.YES);
	}	


	
	public override void destroy ()
	{
		easing = null;
	}

}




sealed public class CutSceneSetTime : CutSceneDataElement
{
	
	public float setTime;

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out setTime, true, -1);
	}
	
	
	sealed public override void startAction()
	{
		if(setTime > 0 && GameManager.me.stageManager.playTime.Get() < setTime)
		{
			GameManager.me.stageManager.playTime = setTime;
		}
	}	
	
	
	public override void destroy ()
	{

	}
	
}




sealed public class CutSceneDataNextScene : CutSceneDataElement
{

/*	
* 현 컷씬이 종료된 이후 즉시 다음 컷씬이 발동됨
> 속성1 : 컷씬ID
> 속성2 : 0-1 (0:삭제, 1:유지) 현재 씬에 올라와있는 어셋들을 삭제할것인지. 삭제하지 않으면 다음씬에서도 같은 개체이름을 사용할 수 있음
*/
	public string nextSceneId;
	public int type;
	
	sealed public override void setData(params object[] attrs)
	{
		nextSceneId = (string)attrs[0];
		Util.parseObject(attrs[1],out type, true, 0);
	}
	
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.openNextCutScene(nextSceneId, ((type==0)?true:false));
	}	

	
	public override void destroy ()
	{
		nextSceneId = null;
	}
	
}



//(컷씬세팅) 스킵 버튼 (추가)				
//	* 스킵버튼 표시여부 지정			
//		* 스킵버튼 누르면, N초 동안 페이드아웃			
//		* 컷씬에서 라운드를 제어할 수 있기 때문에, 페이드아웃 후 최고속으로 컷씬 플레이 종료 (이때, 포즈기능 들은 다 터치한 것으로 간주)			
//		> 속성1 : 0 (스킵없음) , 1 (스킵가능)			
//		> 속성2 : 페이드아웃 시간			

sealed public class CutSceneDataSkip : CutSceneDataElement
{
	public int showSkipButton = 0;

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out showSkipButton, true);
		Util.parseObject(attrs[1],out GameManager.me.cutSceneManager.cutSceneSkipFadeOutTime, true, 0);
	}
	
	sealed public override void startAction()
	{
		GameManager.me.cutSceneManager.btnCutSceneSkip.gameObject.SetActive(showSkipButton == 1);
	}	


	public override void destroy ()
	{

	}

}




sealed public class CutSceneDataKill : CutSceneDataElement
{
	public string type;
	public int index = 0;
	
	sealed public override void setData(params object[] attrs)
	{
		type = (string)attrs[0];
		Util.parseObject( attrs[1], out index, true, 0);
	}
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case "MU":
			for(int i = 0; i < GameManager.me.characterManager.monsters.Count; ++i)
			{
				if(GameManager.me.characterManager.monsters[i].unitData == null) continue;
				GameManager.me.characterManager.monsters[i].damageDead();
			}
			break;
		case "PU":
			for(int i = 0; i < GameManager.me.characterManager.playerMonster.Count; ++i)
			{
				if(GameManager.me.characterManager.playerMonster[i].unitData == null) continue;
				GameManager.me.characterManager.playerMonster[i].damageDead();
			}
			break;
		case "UNIT_ALL":

			bool useSound = true;

			if(CutSceneManager.cutScenePlayTime <= 0.1f)
			{
				useSound = false;
			}

			for(int i = 0; i < GameManager.me.characterManager.monsters.Count; ++i)
			{
				if(GameManager.me.characterManager.monsters[i].unitData == null) continue;
				GameManager.me.characterManager.monsters[i].damageDead(useSound);
			}
			for(int i = 0; i < GameManager.me.characterManager.playerMonster.Count; ++i)
			{
				if(GameManager.me.characterManager.playerMonster[i].unitData == null) continue;
				GameManager.me.characterManager.playerMonster[i].damageDead(useSound);
			}
			break;

		case "M":

			if(GameManager.me.characterManager.monsters.Count > index)
			{
				if(GameManager.me.stageManager.isIntro && GameManager.me.characterManager.monsters[index].unitData == null)
				{
#if UNITY_EDITOR
					Debug.LogError("인트로에선 히어로를 죽이지 않음.");
#endif
				}
				else
				{
					GameManager.me.characterManager.monsters[index].damageDead();
				}
			}
			else if(GameManager.me.characterManager.monsters.Count > 0)
			{
				if(GameManager.me.stageManager.isIntro && GameManager.me.characterManager.monsters[index].unitData == null)
				{
#if UNITY_EDITOR
					Debug.LogError("인트로에선 히어로를 죽이지 않음.");
#endif
				}
				else
				{
					GameManager.me.characterManager.monsters[0].damageDead();					
				}

			}
			break;

		case "P":

			if(GameManager.me.characterManager.playerMonster.Count > index)
			{
				if(GameManager.me.characterManager.playerMonster[0].isPlayer == false) GameManager.me.characterManager.playerMonster[index].damageDead();
			}
			else if(GameManager.me.characterManager.playerMonster.Count > 0)
			{
				if(GameManager.me.characterManager.playerMonster[0].isPlayer == false)
				{
					GameManager.me.characterManager.playerMonster[0].damageDead();
				}
			}
			break;
		}
	}	


	public override void destroy ()
	{
		type = null;
	}

}



sealed public class CutSceneDataCloseScene : CutSceneDataElement
{
	public int type;

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out type, true, 0);
	}
	
	sealed public override void startAction()
	{

		GameManager.me.cutSceneManager.closeOpenCutScene((type == 1));
		
	}	


	public override void destroy ()
	{
	
	}

}






sealed public class CutSceneDataShowMap : CutSceneDataElement
{
	public int type;
	
	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out type, true, 0);
	}
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case 0:
			if(GameManager.me.mapManager.inGameMap != null)
			{
				GameManager.me.mapManager.createBackground(GameManager.me.mapManager.inGameMap.id,true);
			}
			break;
		case -1:
			GameManager.me.mapManager.hideMap();
			break;
		default:
			GameManager.me.mapManager.createBackground(type);
			break;
		}
	}	

	public override void destroy ()
	{

	}

}





sealed public class CutSceneDataClearAsset : CutSceneDataElement
{
	public int type;

	// CLEAN 컷씬 재료 지우기 커맨드.
	//속성1
	//0 모두 지우기
	//1 이펙트 지우기
	//2 오브젝트 지우기

	sealed public override void setData(params object[] attrs)
	{
		Util.parseObject(attrs[0],out type, true, 0);
	}
	
	sealed public override void startAction()
	{
		switch(type)
		{
		case 0:
			GameManager.me.cutSceneManager.clearCutSceneAsset(false);
			break;
		case 1:
			GameManager.me.cutSceneManager.clearCutSceneEffectAsset();
			break;
		case 2:
			GameManager.me.cutSceneManager.clearCutSceneObjectAsset(false);
			break;
		}
	}	


	public override void destroy ()
	{
	
	}

}







