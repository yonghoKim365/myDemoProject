using UnityEngine;
using System.Collections;
using System;

public class UIHellModeInfo : MonoBehaviour 
{
	public UILabel lbLeftTime, lbDistance, lbKillCount, lbCurrentWave, lbTotalScore;


	private float _tempDist,_prevTime,_prevDist;

	public void init()
	{
		lbLeftTime.text = "";
		lbDistance.text = "";
		lbKillCount.text = "0";
		_prevTime = -1;
		lbLeftTime.color = Color.white;
	}


	public void updateDistance(int dist)
	{
		lbDistance.text = dist + "m";
	}

	public int updateValue = 0;

	public void setLeftTime(int time)
	{
		if(_prevTime != time)
		{
			lbLeftTime.text = Util.secToHourMinuteSecondString(time);
			_prevTime = time;

			if(time <= 10)
			{
				if(time % 2 == 0) lbLeftTime.color = Color.white;
				else lbLeftTime.color = Color.red;
			}
			
			if(time <= 20)
			{
				if(GameManager.me.uiManager.uiPlay.didShowHurryUp == false)
				{
					GameManager.me.uiManager.uiPlay.spHurryup.gameObject.SetActive(true);
					GameManager.me.uiManager.uiPlay.didShowHurryUp = true;
				}
			}
		}
	}


	public void setTotalScore(int score)
	{
		lbTotalScore.text = Util.GetCommaScore(score);
	}


}
