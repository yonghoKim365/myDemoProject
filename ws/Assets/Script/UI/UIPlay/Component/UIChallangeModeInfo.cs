using UnityEngine;
using System.Collections;
using System;

public class UIChallangeModeInfo : MonoBehaviour 
{
	public UILabel lbTimer, lbProgress;
	public UISprite[] spRank;

	public UISprite spProgressbar;

	Xint _rank = -1;

	const string STAR_1_ON = "img_challgauge_star1_on";
	const string STAR_2_ON = "img_challgauge_star2_on";
	const string STAR_3_ON = "img_challgauge_star3_on";

	const string STAR_1_OFF = "img_challgauge_star1_off";
	const string STAR_2_OFF = "img_challgauge_star2_off";
	const string STAR_3_OFF = "img_challgauge_star3_off";

	public Transform tfProgressStart;
	public Transform tfProgressEnd;

	public GameObject timeContainer;

	private int _rank1Data;
	private int _rank2Data;
	private int _rank3Data;

	private int _prevTime = -999;

	private float _prevDist = -999;

	private string _endWord = "";

	int roundModeType = 0;
	public void init(string roundMode, int rank1, int rank2, int rank3)
	{
		_prevTime = -999;
		_prevDist = -999;

		switch(roundMode)
		{
		case RoundData.MODE.C_HUNT:
			//거리(사냥수),제한시간 표시.
			update(0);
			timeContainer.SetActive(true);
			lbProgress.transform.localPosition = new Vector3(922,-5,0);
			_endWord = "M";
			roundModeType = 0;

			lbProgress.text = "0" + _endWord;// 마리
			break;
		case RoundData.MODE.C_RUN:
			// 무한질주. 제한시간. 거리표시.
			update(0);
			timeContainer.SetActive(true);
			lbProgress.transform.localPosition = new Vector3(923,-5,0);
			_endWord = "m";
			roundModeType = 1;
			lbProgress.text = "0" + _endWord;// 미터
			break;
		case RoundData.MODE.C_SURVIVAL:
			update(0);
			lbProgress.transform.localPosition = new Vector3(962,-5,0);
			timeContainer.SetActive(false);
			_endWord = "";
			roundModeType = 2;
			lbProgress.text = Util.secToHourMinuteSecondString(0);// 시간.
			break;

		case RoundData.MODE.B_TEST:
			//거리(사냥수),제한시간 표시.
			update(0);
			timeContainer.SetActive(false);
			lbProgress.transform.localPosition = new Vector3(922,-5,0);
			_endWord = "M";
			roundModeType = 3;
			
			lbProgress.text = "0" + _endWord;// 마리
			break;

		}

		_rank1Data = rank1;
		_rank2Data = rank2;
		_rank3Data = rank3;


		float w = (tfProgressEnd.position.x - tfProgressStart.position.x);
		Vector3 pv = spRank[0].transform.position;
		pv.x = tfProgressStart.position.x + (float)rank1 / (float)rank3 * w;
		spRank[0].transform.position = pv;

		pv.x = tfProgressStart.position.x + (float)rank2 / (float)rank3 * w;
		spRank[1].transform.position = pv;

		pv.x = tfProgressStart.position.x + w;
		spRank[2].transform.position = pv;

		rank = 0;
	}

	private float _tempDist;
	public void updateDistance()
	{
		_tempDist = GameManager.me.player.cTransformPosition.x - GameManager.me.stageManager.nowRound.playerStartPosX.Get();

		if(_tempDist > _prevDist && (int)_tempDist != (int)_prevDist)
		{
			_prevDist = _tempDist;
			update((int)(_prevDist), (int)(_prevDist * 0.01f) + "m");
		}
	}

	public int updateValue = 0;

	public void setLeftTime(int time)
	{
		if(_prevTime != time)
		{
			lbTimer.text = Util.secToHourMinuteSecondString(time);
			_prevTime = time;
		}
	}

	public int rank
	{
		set
		{
			if(_rank != value)
			{
				_rank = value;

				switch(_rank)
				{
				case 1:
					spRank[0].spriteName = STAR_1_ON;
					spRank[1].spriteName = STAR_2_OFF;
					spRank[2].spriteName = STAR_3_OFF;

					if(UnityEngine.Random.Range(0,10) > 5) SoundData.play("bgm_battle_c");
					else SoundData.play("bgm_battle_b");

					break;
				case 2:
					spRank[0].spriteName = STAR_1_ON;
					spRank[1].spriteName = STAR_2_ON;
					spRank[2].spriteName = STAR_3_OFF;

					SoundData.play("bgm_battle_a");

					break;
				case 3:
					spRank[0].spriteName = STAR_1_ON;
					spRank[1].spriteName = STAR_2_ON;
					spRank[2].spriteName = STAR_3_ON;	

					SoundData.play("bgm_battle_d");

					break;
				default:
					spRank[0].spriteName = STAR_1_OFF;
					spRank[1].spriteName = STAR_2_OFF;
					spRank[2].spriteName = STAR_3_OFF;
					break;
				}
			}
		}
		get
		{
			return _rank;
		}
	}

	public void update(int nowValue, string progressStr = null)
	{
		if(nowValue >= _rank3Data)
		{
			rank = 3;
		}
		else if(nowValue >= _rank2Data)
		{
			rank = 2;
		}
		else if(nowValue >= _rank1Data)
		{
			rank = 1;
		}
		
		progress = (float)nowValue / (float)_rank3Data;
		
		if(progressStr != null) lbProgress.text = progressStr;
		else
		{
			if(roundModeType == 0) lbProgress.text = nowValue + _endWord;// 마리
			else if(roundModeType == 1) lbProgress.text = Mathf.CeilToInt((float)nowValue * 0.01f) + _endWord;// 미터
			else if(roundModeType == 2) lbProgress.text = Util.secToHourMinuteSecondString(nowValue);// 시간.
			else if(roundModeType == 3) lbProgress.text = nowValue + _endWord;// 마리
		}
		
		updateValue = nowValue;
	}
	
	
	private float _progress = 0.0f;
	public float progress
	{
		set
		{
			_progress = value;
			spProgressbar.fillAmount = value;
		}
		get
		{
			return _progress;
		}
	}

}
