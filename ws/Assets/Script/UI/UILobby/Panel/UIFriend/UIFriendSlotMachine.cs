using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendSlotMachine : MonoBehaviour 
{
	const float LINE_HEIGHT = 73.0f;

	public UISlotMachineItem[] items;

	public float[] timeStep = new float[]{1.8f,3f,4f,5f,5.5f};
	public float[] speedStep = new float[]{0.99f, 0.9f, 0.8f, 0.7f};

	public float startSpeed = 200.0f;

	public GameObject goOnEffectBorderContainer;

	List<P_Reward> _rewards = new List<P_Reward>();

	Vector3 _v = new Vector3(0,0,0);

	int _playIndex = 0;


	public bool isReady = false;


	int _rewardTotalCount = 0;

	public void refresh()
	{
		time = 0;
		_speed = startSpeed;
		step = 0;

		isReady = true;

		_rewards.Clear();

		_rewardTotalCount = 0;

		if(GameDataManager.instance.slotMachineRewardList != null)
		{
			foreach(KeyValuePair<string, P_Reward> kv in GameDataManager.instance.slotMachineRewardList)
			{
				_rewards.Add(kv.Value);
			}

			_rewardTotalCount = _rewards.Count;
			
			for(int i = 0; i < 4; ++i)
			{
				_v.y = LINE_HEIGHT * (i-2);
				items[i].tf.localPosition = _v;
				int randomIndex = UnityEngine.Random.Range(0,_rewardTotalCount);
				items[i].setData(_rewards[ randomIndex ].code, _rewards[ randomIndex ].count, _rewards[ randomIndex ].gacha);
			}
		}
	}


	P_Reward receiveReward;
	ToC_PULL_SLOT_MACHINE slotMachinePacketInfo;

	public void start(ToC_PULL_SLOT_MACHINE reward)
	{
		GameManager.me.uiManager.uiMenu.uiFriend.btnBack.isEnabled = false;
		GameManager.me.uiManager.uiMenu.uiFriend.btnInvite.isEnabled = false;

		receiveReward = GameDataManager.instance.slotMachineRewardList[reward.rewardId];
		slotMachinePacketInfo = reward;

		isReady = false;
		time = 0;
		step = 0;
		borderIndex = 0;
		++_playIndex;
		_speed = startSpeed;
		StartCoroutine(progress(_playIndex));

		GameManager.soundManager.playLoopEffect(GameManager.info.soundData["uifr_bonusturn"]);

	}

	public int borderIndexEffectOffset = 3;

	int borderIndex = 0;
	public float time;
	float _timeOffset = 0.05f;

	public float _speed = 100.0f;
	public int step = 0;

	float _finalStepTime = 0.0f;

	const int FINAL_STEP = 4;
	const int FINAL_EFFECT_STEP = 5;

	UISlotMachineItem _rewardResultSlot = null;

	IEnumerator progress(int index)
	{
		while(true)
		{
			if(index != _playIndex)
			{
				break;
			}

			yield return new WaitForSeconds(_timeOffset);

			time += _timeOffset;

			++borderIndex;

			if(borderIndex % borderIndexEffectOffset == 0)
			{
				goOnEffectBorderContainer.SetActive(!goOnEffectBorderContainer.activeSelf);
			}

			if(step == FINAL_STEP)
			{
				_speed *= speedStep[0];
			}
			else if(step != FINAL_EFFECT_STEP)
			{
				if(time > timeStep[step] && step == 0)
				{
					step = 1;
					_speed *= speedStep[1];
				}
				else if(time > timeStep[step] && step == 1)
				{
					step = 2;
					_speed *= speedStep[2];
				}
				else if(time > timeStep[step] && step == 2)
				{
					step = 3;
					_speed *= speedStep[3];
				}
				else if( (time > timeStep[4] || _speed < 120.0f) && step == 3)
				{
					_rewardResultSlot = null;

					for(int i = 0; i < 4; ++i)
					{
						_v = items[i].tf.localPosition;

						if(_v.y > 110)
						{
							_rewardResultSlot = items[i];
							break;
						}
					}

					if(_rewardResultSlot != null)
					{
						step = FINAL_STEP;
						_rewardResultSlot.setData(receiveReward.code, receiveReward.count, receiveReward.gacha);
						continue;
					}
				}
				else if(step >= 3)
				{
					_speed *= speedStep[1];
				}
				else if(step == 1)
				{
					_speed *= speedStep[0];
				}
				else if(step == 2)
				{
					_speed *= speedStep[0];
				}
			}


			float applySpeed = _speed * _timeOffset;

			if(step == FINAL_STEP)
			{
				if(_rewardResultSlot.tf.localPosition.y - applySpeed < -0.1f)
				{
					applySpeed += (_rewardResultSlot.tf.localPosition.y - applySpeed);
				}
			}



			for(int i = 0; i < 4; ++i)
			{
				_v = items[i].tf.localPosition;

				_v.y -= applySpeed;

				if(_v.y <= -LINE_HEIGHT*2.0f)
				{
					_v.y += LINE_HEIGHT*4.0f;

					if(step != FINAL_STEP && step != FINAL_EFFECT_STEP)
					{
						int randomIndex = UnityEngine.Random.Range(0,_rewardTotalCount);
						items[i].setData(_rewards[ randomIndex ].code, _rewards[ randomIndex ].count, _rewards[ randomIndex ].gacha);
					}
				}

				items[i].tf.localPosition = _v;
			}

			if(step == FINAL_STEP)
			{

				if(_rewardResultSlot.tf.localPosition.y <= 0)
				{
					step = FINAL_EFFECT_STEP;
					_finalStepTime = 0.0f;

					float correntYValue = -_rewardResultSlot.tf.localPosition.y;
//
					for(int i = 0; i < 4; ++i)
					{
						_v = items[i].tf.localPosition;
						_v.y += correntYValue;
						//items[i].tf.localPosition = _v;
						items[i].finalPosition = _v;
					}
				}
			}
			else if(step == FINAL_EFFECT_STEP)
			{
				//_finalStepTime += _timeOffset;
				//float easedStep = Easing.EaseInOut(_finalStepTime/0.1f, EasingType.Quadratic);

				for(int i = 0; i < 4; ++i)
				{
					items[i].tf.localPosition = items[i].finalPosition;
					//items[i].tf.localPosition = Vector3.Lerp(items[i].tf.localPosition,items[i].finalPosition,easedStep);
				}

				goOnEffectBorderContainer.SetActive(true);
				
				StartCoroutine(startEndEffect());
				
				break;
			}
		}
	}


	IEnumerator startEndEffect()
	{
		int index = 0;

		while(true)
		{
			goOnEffectBorderContainer.SetActive(index%2 == 0);
			++index;
			if(index > 6) break;
			yield return Util.ws01;
		}

		isReady = true;

		GameManager.soundManager.stopLoopEffect();

		SoundData.play("uifr_bonusstop");

		GameManager.me.uiManager.uiMenu.uiFriend.btnReceiveBonus.isEnabled = (GameDataManager.instance.friendPoint >= GameDataManager.instance.slotMachinePrice);

		if(TutorialManager.instance.isTutorialMode == false)
		{
			UISystemPopup.checkLevelupPopupWithoutCallback();
		}

		GameManager.me.uiManager.uiMenu.uiFriend.btnBack.isEnabled = true;
		GameManager.me.uiManager.uiMenu.uiFriend.btnInvite.isEnabled = true;

		GameManager.me.uiManager.uiMenu.uiFriend.lbSlotMachinePrice.text = Util.GetCommaScore(GameDataManager.instance.slotMachinePrice);

		if(receiveReward.code == WSDefine.REWARD_TYPE_ITEM || receiveReward.code == WSDefine.REWARD_TYPE_GACHA)
		{
			RuneStudioMain.instance.playMakeResult(new string[]{receiveReward.itemId}, false);
		}
		else if(receiveReward.code == WSDefine.REWARD_TYPE_RUNE)
		{
			RuneStudioMain.instance.playMakeResult(new string[]{slotMachinePacketInfo.rewardItem}, false);
		}
		else
		{
			GameManager.me.uiManager.rewardNotice.start(true, receiveReward);
		}

		if(TutorialManager.nowPlayingTutorial("T15",4))
		{
			TutorialManager.instance.subStep = 6;
			EpiServer.instance.sendCompleteTutorial("T15");
		}
	}

}
