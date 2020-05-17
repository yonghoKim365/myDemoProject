using UnityEngine;
using System.Collections;

public class HellModeManager : MonoBehaviour
{

	public Xint hellClearType;



	public Xint killPoint = 0;

	public Xint distancePoint = 0;


	public Xint leftTime = 0;

	public Xint timePoint = 0;

	public Xint totalPlayTime = 0;

	public Xint killUnitCount = 0;
	public Xint prevDistance = 0;
	public Xint currentDistance = 0;

	public RoundData.MODE_TYPE waveType;

	public static HellModeManager instance = null;

	public Xint continueHellWave = -1;

//	public Xint lastPlayingHellModePlayingTime = 0;


	// 25wave 지나면 남은 시간을 초당 100점으로 환산한다.


	void Awake()
	{
		if(instance != null)
		{
			instance = null;
		}

		instance = this;
	}


	private void init()
	{
		_prevDist = -1000;
		_tempDist = 0;

		roundIndex = 0;

		killPoint = 0;
		distancePoint = 0;

		leftTime = GameManager.info.roundData["HELL"].settingTime;

		killUnitCount = 0;
		prevDistance = 0;
		currentDistance = 0;

		totalPlayTime = 0;



		timePoint = 0;

//		lastPlayingHellModePlayingTime = 0;

		GameManager.me.uiManager.uiPlay.hellModeInfo.lbKillCount.text = "0";
		GameManager.me.uiManager.uiPlay.hellModeInfo.updateDistance( 0 );
		GameManager.me.uiManager.uiPlay.hellModeInfo.setTotalScore( 0 );


	}

	string[] _roundDataSet;
	int[] _roundClearBonusTime;

	public Xint roundIndex = 0;

	public bool isOpen = false;

	public void setStage(RoundData rd)
	{
		init();

		if(rd != null)
		{
			isOpen = true;
			parseRoundAndTimeInfo(rd.settingAttr);
			leftTime = rd.settingTime;
			roundIndex = 0;
			nextRound();
		}
		else
		{
			isOpen = false;
		}
	}


	void parseRoundAndTimeInfo(string settingAttr)
	{
		string[] temp = settingAttr.Split(',');

		int len = temp.Length;

		_roundDataSet = new string[len];
		_roundClearBonusTime = new int[len];

		string[] t;

		for(int i = 0; i < len; ++i)
		{
			t = temp[i].Split(':');
			_roundDataSet[i] = t[0];

			if(t.Length == 2) int.TryParse(t[1], out _roundClearBonusTime[i]);
			else _roundClearBonusTime[i] = 0;
		}

	}


	public bool checkNextRound()
	{
		updateTimePoint();

		prevDistance += currentDistance;

		currentDistance = 0;

		if(roundIndex >= _roundDataSet.Length)
		{
			HellModeManager.instance.hellClearType = Result.Type.Finish;
			GameManager.me.stageManager.nowPlayingGameResult = Result.Type.Finish;

			return false;
		}
		else
		{
			int timeIndex = roundIndex - 1;
			if(timeIndex < 0) timeIndex = 0;
			
			if(timeIndex >= _roundDataSet.Length)
			{
				leftTime += _roundClearBonusTime[_roundDataSet.Length-1];
			}
			else
			{
				leftTime += _roundClearBonusTime[timeIndex];
			}
			
			nextRound();
			GameManager.setTimeScale = 1.0f;
			
			StartCoroutine(GameManager.me.nextHellMode());

			return true;
		}
	}



	public void loadContinueHellMode()
	{
//		Debug.LogError("loadContinueHellMode");

		init();

		isOpen = true;

		parseRoundAndTimeInfo(GameManager.info.roundData["HELL"].settingAttr);

//		Debug.LogError("HellModeManager.instance.continueHellWave : " + HellModeManager.instance.continueHellWave);

		if(HellModeManager.instance.continueHellWave > 1)
		{
			if(GameDataManager.instance.lastHellWaveInfo != null)
			{
				roundIndex = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[0]) - 1;
				prevDistance = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[1]);
				killUnitCount = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[2]);

				int k = killUnitCount.Get();
				GameManager.me.uiManager.uiPlay.hellModeInfo.lbKillCount.text = k.ToString();

				leftTime = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[3]);
				
				GameManager.me.player.hp = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[4]);
				GameManager.me.player.sp = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[5]);
				GameManager.me.player.mp = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[6]);


				GameDataManager.instance.playData[UnitSlot.U1] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[8]);
				GameDataManager.instance.playData[UnitSlot.U2] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[9]);
				GameDataManager.instance.playData[UnitSlot.U3] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[10]);
				GameDataManager.instance.playData[UnitSlot.U4] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[11]);
				GameDataManager.instance.playData[UnitSlot.U5] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[12]);

				GameDataManager.instance.playData[SkillSlot.S1] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[13]);
				GameDataManager.instance.playData[SkillSlot.S2] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[14]);
				GameDataManager.instance.playData[SkillSlot.S3] = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[15]);

				totalPlayTime = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[16]);

				timePoint = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[17]);

				killPoint = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[18]);
				
//				GameManager.me.stageManager.playTime = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[7]);

//				lastPlayingHellModePlayingTime = System.Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[7]);

				GameManager.me.player.hpBar.visible = false;

				GameManager.me.uiManager.uiPlay.lbRoundInfo.text = "WAVE "+roundIndex;

//				Debug.LogError("roundIndex : " + roundIndex);
//				Debug.LogError("prevDistance : " + prevDistance);
//				Debug.LogError("killUnitCount : " + killUnitCount);
//				Debug.LogError("totalTime : " + leftTime);
//				Debug.LogError("hp : " + GameDataManager.instance.lastHellWaveInfo[4]);
//				Debug.LogError("sp : " + GameDataManager.instance.lastHellWaveInfo[5]);
//				Debug.LogError("mp : " + GameDataManager.instance.lastHellWaveInfo[6]);
//				Debug.LogError("playTime : " + GameDataManager.instance.lastHellWaveInfo[7]);

			}
		}
		else
		{
			leftTime = GameManager.info.roundData["HELL"].settingTime;
		}

		nextRound();
	}






	public void nextRound()
	{
		if(isOpen == false) return;	

		_prevDist = -1000;
		_tempDist = 0;

		#if UNITY_EDITOR
		Debug.LogError("====== nextRound ! ========= roundIndex : " + roundIndex);
#endif

		if(roundIndex >= _roundDataSet.Length)
		{
			setNowRound(GameManager.info.roundData[_roundDataSet[_roundDataSet.Length-1]]);
		}
		else
		{
			setNowRound(GameManager.info.roundData[_roundDataSet[roundIndex]]);
		}

		++roundIndex;
		GameManager.me.uiManager.uiPlay.lbRoundInfo.text = "WAVE "+roundIndex;

		GameManager.me.uiManager.uiPlay.hellModeInfo.lbLeftTime.color = Color.white;

	}

	public RoundData nowRound;

	void setNowRound(RoundData rd)
	{
		nowRound = rd;

		StageManager.mapStartPosX = nowRound.mapStartEndPosX[0];
		StageManager.mapEndPosX = nowRound.mapStartEndPosX[1];
		StageManager.mapPlayerEndPosX = StageManager.mapEndPosX - 100;

		switch(nowRound.mode)
		{
		case RoundData.MODE.SNIPING:
			waveType = RoundData.MODE_TYPE.SNIPING;
			break;
		case RoundData.MODE.ARRIVE:
			waveType = RoundData.MODE_TYPE.ARRIVE;
			break;
		case RoundData.MODE.KILLEMALL:
			waveType = RoundData.MODE_TYPE.KILLEMALL;
			break;
		}
	}


	public bool checkClear(int attr)
	{
		updateDistance();

		if(GameManager.me.isPlaying == false) return false;

		switch(waveType)
		{
		case RoundData.MODE_TYPE.SNIPING:

			if(nowRound.targetHpPer >= 0)
			{
				if(StageManager.instance.heroMonster[nowRound.targetIndex].hpPer * 100.0f <= nowRound.targetHpPer)
				{
					GameManager.me.mapManager.clearRound();
					return true;
				}
			}
			else if(StageManager.instance.heroMonster[nowRound.targetIndex].hp <= 0)
			{
				GameManager.me.mapManager.clearRound();
				return false;
			}
			
			if(attr == ClearChecker.CHECK_AFTER_DELETE)
			{
				if(StageManager.instance.heroMonster[nowRound.targetIndex].isEnabled == false)
				{
					GameManager.me.mapManager.clearRound();
				}
			}

			break;
		case RoundData.MODE_TYPE.ARRIVE:

			if(attr == ClearChecker.CHECK_TIME)
			{
				//updateDistance();
				if(Xfloat.greatEqualThan( (GameManager.me.player.cTransformPosition.x + StageManager.ARRIVE_DISTANCE_BUFFER).AsFloat() , nowRound.targetPos.Get() ))
				{
					GameManager.me.mapManager.clearRound();
					GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = "0m";
					return true;	
				}
			}


			break;
		case RoundData.MODE_TYPE.KILLEMALL:

			if(attr == ClearChecker.CHECK_AFTER_DELETE)
			{
				if((GameManager.me.mapManager.bossNum + GameManager.me.mapManager.monUnitNum) == 0)
				{
					GameManager.me.mapManager.clearRound();
				}
			}

			break;
		}

		return false;
	}


	public bool timeFailCheck()
	{
		return (leftTime < GameManager.me.stageManager.playTime);
	}


	public void setKillUnit(UnitData ud)
	{
		killPoint += GameManager.info.hellSetupData[roundIndex.Get()].killPoint.Get();

		int k = killUnitCount.Get() + 1;
		killUnitCount = k;
		GameManager.me.uiManager.uiPlay.hellModeInfo.lbKillCount.text = k.ToString();
	}



	private int _tempDist, _prevDist;
	void updateDistance()
	{
		_tempDist = MathUtil.RoundToInt(GameManager.me.player.cTransformPosition.x) - MathUtil.RoundToInt(GameManager.me.stageManager.nowRound.playerStartPosX);
		_tempDist = MathUtil.FloorToInt((float)_tempDist * 0.01f);

		if(_tempDist > _prevDist)
		{
			_prevDist = _tempDist;
			currentDistance = _tempDist;
			GameManager.me.uiManager.uiPlay.hellModeInfo.updateDistance( currentDistance + prevDistance );
		}
	}

	public int getTotalDistance()
	{
		return currentDistance + prevDistance;
	}


	public void updateTime()
	{
		GameManager.me.uiManager.uiPlay.hellModeInfo.setLeftTime((int)( leftTime - GameManager.me.stageManager.playTime ));
	}


	private void updateTimePoint()
	{
		int t =  leftTime.Get() - MathUtil.RoundToInt( GameManager.me.stageManager.playTime);
		timePoint = timePoint.Get() + t * (GameManager.info.hellSetupData[roundIndex.Get()].timeBonus);

#if UNITY_EDITOR
		Debug.LogError("updateTimePoint - roundIndex : " + roundIndex + "   t: " + t + "     timePoint : " + GameManager.info.hellSetupData[roundIndex].timeBonus);
#endif
	}


	public int totalScore
	{
		get
		{
//			Debug.LogError("roundIndex : " + roundIndex);

			int ts = killPoint.Get() + getClearPoint() + timePoint;
			GameManager.me.uiManager.uiPlay.hellModeInfo.setTotalScore(ts);
			return ts;
		}
	}


	int getClearPoint()
	{
		int cp = 0;

		for(int i = 1; i < roundIndex && i < 26; ++i)
		{
			cp += GameManager.info.hellSetupData[i].clearBonus.Get();
		}

		return cp;
	}



	public bool hellModeCheatCheck()
	{
		int currentRoundIndex = roundIndex;

		RoundData rd;

		int requireDistance = 0;
		int requireKillCount = 0;

		for(int i = 0; i < currentRoundIndex; ++i)
		{
			if(currentRoundIndex >= _roundDataSet.Length)
			{
				rd = GameManager.info.roundData[_roundDataSet[_roundDataSet.Length-1]];
			}
			else
			{
				rd = GameManager.info.roundData[_roundDataSet[currentRoundIndex]];
			}


			switch(rd.mode)
			{
			case RoundData.MODE.KILLCOUNT:
				requireKillCount += rd.killMonsterCount;
				break;
			case RoundData.MODE.KILLEMALL:

				if(rd.unitMonsters != null)
				{
					foreach(StageMonsterData data in rd.unitMonsters)
					{
						if(data.type == StageMonsterData.Type.UNIT)
						{
							requireKillCount += rd.killMonsterCount;
						}
					}		
				}
				break;
			case RoundData.MODE.ARRIVE:
				requireDistance += rd.targetPos;
				break;
			}
		}

		if(killUnitCount < requireKillCount - 1) return false;

		if((currentDistance + prevDistance) < requireDistance - 5 ) return false;

		return true;

	}





	void onDestroy()
	{
		instance = null;
	}


}
