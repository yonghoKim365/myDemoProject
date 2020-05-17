using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using JsonFx.Json;

[ExecuteInEditMode]
public class Weme : MonoBehaviour {

	public UILabel lbText;

	public UISprite spProgressBar;

	private bool _isMuteSound = false;

	public static Weme instance;

	public GameObject goProgressBarContainer;

	public PlayMakerFSM loadingEndFsm;


	void Awake()
	{
		UISystemPopup.init();
		_isMuteSound = (PlayerPrefs.GetInt("SFX") == 0)?false:true;

		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			gameObject.transform.parent = null;
		}
		else
		{
			Destroy(gameObject);
		}
	}



	public void editorInit()
	{
		PlayMakerFSM[] fsms = gameObject.GetComponentsInChildren<PlayMakerFSM>(true);

		foreach(PlayMakerFSM fsm in fsms)
		{
			if(fsm.enabled == false && fsm.FsmName.Contains("loading_complete"))
			{
				loadingEndFsm = fsm;
				break;
			}
		}
	}


	void Start () 
	{	
		setIntroText();
	}



	public void startLoadingScene()
	{
		StartCoroutine(NextScene());
	}


	public bool stopSceneLoadingBar = false;

	float _time = 0;

	void Update () 
	{
		if(stopSceneLoadingBar) return;

		_time += Time.smoothDeltaTime;
		
		float value = _time / 8.0f;
		if(value >= 1.0f) value = 1.0f;
		value *= 0.1f;
		
		spProgressBar.fillAmount = value;
	}


	public static AsyncOperation mainSceneLoadingAsync = null;

	
	IEnumerator NextScene()
	{
		yield return new WaitForSeconds(0.2f);
		mainSceneLoadingAsync = Application.LoadLevelAsync("scene_main");
	}




	void setIntroText()
	{

		string source = (Resources.Load(ClientDataLoader.CLIENT_DATA_PATH + "data_introtext_client") as TextAsset).ToString();

		List<IntroTextData> normal = new List<IntroTextData>();
		List<IntroTextData> days = new List<IntroTextData>();
		List<IntroTextData> dates = new List<IntroTextData>();
		List<IntroTextData> times = new List<IntroTextData>();

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = ClientDataLoader.getKeyIndexDic((List<object>)jd[ClientDataLoader.NAME]);
		// -- 여기까지.

		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != ClientDataLoader.NAME)
			{
				List<object> list = kv.Value as List<object>;
				IntroTextData data = new IntroTextData();
				data.setData(list, k);

				switch(data.type)
				{
				case IntroTextData.Type.Date:
					dates.Add(data);
					break;
				case IntroTextData.Type.Day:
					days.Add(data);
					break;
				case IntroTextData.Type.Time:
					times.Add(data);
					break;
				default:
					normal.Add(data);
					break;
				}

				list = null;
			}
		}

		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

		DateTime now = System.DateTime.Now;

		DateTime checkTime = System.DateTime.Today;

		DateTime start = new DateTime();;
		DateTime end = new DateTime();

		for(int i = times.Count - 1; i >= 0; --i)
		{
			if(times[i].startHour == 24) times[i].startHour = 0;
			if(times[i].endHour == 24) times[i].endHour = 0;

			if(times[i].startHour <= times[i].endHour)
			{
				start = ChangeTime(checkTime, times[i].startHour,times[i].startMin);
				end = ChangeTime(checkTime, times[i].endHour,times[i].endMin);
			}
			else if(times[i].startHour > times[i].endHour)
			{
				start = ChangeTime(checkTime, times[i].startHour,times[i].startMin);
				end = ChangeTime(checkTime.AddDays(1), times[i].endHour,times[i].endMin);
			}

			if(now >= start && now <= end)
			{
				normal.Add(times[i]);
			}
		}

		for(int i = days.Count - 1; i >= 0; --i)
		{
			foreach(int d in days[i].day)
			{
				if(d == (int)now.DayOfWeek)
				{
					normal.Add(days[i]);
				}
			}
		}


		for(int i = dates.Count - 1; i >= 0; --i)
		{
			if(dates[i].startMonth <= dates[i].endMonth)
			{
				start = ChangeDate(checkTime, dates[i].startMonth,dates[i].startDay);
				end = ChangeDate(checkTime, dates[i].endMonth,dates[i].endDay);
			}
			else if(dates[i].startMonth > dates[i].endMonth)
			{
				start = ChangeDate(checkTime, dates[i].startMonth,dates[i].startDay);
				end = ChangeDate(checkTime.AddYears(1), dates[i].endMonth,dates[i].endDay);
			}
			
			if(now >= start && now <= end)
			{
				normal.Add(dates[i]);
			}
		}

		if(normal.Count > 0)
		{
			normal.Sort(IntroTextData.sort);

			int totalWeight = 0;

			for(int i = normal.Count - 1; i >= 0; --i)
			{
				totalWeight += normal[i].weight;
			}

			int randomValue = UnityEngine.Random.Range(0,totalWeight);

			totalWeight = 0;

			for(int i = normal.Count - 1; i >= 0; --i)
			{
				totalWeight += normal[i].weight;

				if(totalWeight > randomValue)
				{
					lbText.text = normal[i].text;
					return;
				}
			}


			lbText.text = normal[0].text;

			return;
		}


		lbText.text = "지금은 소울 타임! 모험할 준비는 되셨나요?";
	}


	void OnDestroy()
	{
		instance = null;
	}


	public static DateTime ChangeDate(DateTime dateTime, int month, int day)
	{
		return new DateTime(
			dateTime.Year,
			month,
			day);

	}

	public static DateTime ChangeTime(DateTime dateTime, int hours, int minutes)
	{
		return (new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, 0));
	}
}
