using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRoundClearPopup : UIPopupBase 
{

	protected override void onClickClose (GameObject go)
	{
		GameManager.setTimeScale = 1.0f;

		clearSlots();

		base.onClickClose (go);

		#if UNITY_EDITOR
		
		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_MENU) return;
		
		#endif

		GameManager.me.uiManager.stageClearEffectManager.resetPlayMaker();

		GameManager.me.returnToSelectScene();
	}


	protected override void awakeInit ()
	{
	
	}


	List<string> _rewards = new List<string>();


	public BoxCollider[] dragScollViewBoxColliders;


	public override void show ()
	{
		roundLevelupInfoSlot.gameObject.SetActive(false);
		roundRewardInfoSlot.gameObject.SetActive(false);

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false && (TutorialManager.instance.isTutorialMode || GameManager.me.uiManager.currentUI != UIManager.Status.UI_PLAY))
		{
			return;
		}
#else
		if(TutorialManager.instance.isTutorialMode || GameManager.me.uiManager.currentUI != UIManager.Status.UI_PLAY)
		{
			return;
		}
#endif

		base.show ();

		btnClose.gameObject.SetActive(false);

		if(dragScollViewBoxColliders != null)
		{
			foreach(BoxCollider bc in dragScollViewBoxColliders)
			{
				bc.enabled = false;
			}
		}
			


		rewardScrollView.transform.localPosition = Vector3.zero;
		levelUpScrollView.transform.localPosition = Vector3.zero;

		rewardScrollView.panel.clipOffset = Vector2.zero;
		levelUpScrollView.panel.clipOffset = Vector2.zero;

		rewardScrollView.ResetPosition();
		levelUpScrollView.ResetPosition();

		GameManager.setTimeScale = 1.0f;

		StartCoroutine(  startDisplay()  );
	}


	void createSlots()
	{
		_rewards.Clear();

		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			GameManager.me.stageManager.nowRound = GameManager.info.roundData[DebugManager.instance.debugRoundId];
		}
		#endif

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic)
		{
			if(GameManager.me.stageManager.nowRound.rewards != null)
			{
				foreach(string str in GameManager.me.stageManager.nowRound.rewards)
				{
					if(str.StartsWith("GOLD") || str.StartsWith("EXP") )
					{
						_rewards.Add(str);
					}
				}
			}
		}

		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			if(GameManager.me.stageManager.nowRound.rewards != null)
			{
				foreach(string str in GameManager.me.stageManager.nowRound.rewards)
				{
					if(str.StartsWith("GOLD") == false && str.StartsWith("EXP") == false)
					{
						_rewards.Add(str);
					}
				}
			}
		}
		#endif
		
		if(GameDataManager.instance.roundItems != null)
		{
			_rewards.AddRange(GameDataManager.instance.roundItems);
			GameDataManager.instance.roundItems = null;
		}
		
		int len = _rewards.Count;
		
		for(int i = 0; i < len; ++i)
		{
			addRewardSlot(_rewards[i]);
		}
		
		_rewards.Clear();
		
		
		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			try
			{
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(0, 20, 0.0f, 0.1f, DebugManager.instance.debugUnitId[0], RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(0, 20, 0.0f, 0.1f, DebugManager.instance.debugUnitId[0], RareType.S));

				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(5, 1, 0.2f, 0.1f, DebugManager.instance.equipBody, RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(1, 1, 0.2f, 0.1f, DebugManager.instance.equipHead, RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(5, 1, 0.2f, 0.1f, DebugManager.instance.equipVehicle, RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(0, 20, 0.0f, 0.1f, DebugManager.instance.debugUnitId[0], RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(5, 1, 0.2f, 0.1f, DebugManager.instance.debugSkillId[0], RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(5, 1, 0.2f, 0.1f, DebugManager.instance.debugUnitId[0], RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(1, 19, 12.0f, 0.1f, DebugManager.instance.equipWeapon, RareType.S));
				GameDataManager.instance.roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(5, 1, 0.2f, 0.1f, DebugManager.instance.debugUnitId[0], RareType.S));
			}
			catch(System.Exception e)
			{
				
			}
		}
		#endif
		
		GameDataManager.instance.roundClearLevelUpInvenItems.Sort(RoundClearLevelupItemData.sort);
		
		len = GameDataManager.instance.roundClearLevelUpInvenItems.Count;
		
		for(int i = 0; i < len; ++i)
		{
			addLevelupSlot(GameDataManager.instance.roundClearLevelUpInvenItems[i]);
		}
		
		GameDataManager.instance.roundClearLevelUpInvenItems.Clear();
	}


	public UIScrollView rewardScrollView;
	public Transform tfRewardContainer;
	public UIChallengeItemSlot roundRewardInfoSlot;
	List<UIChallengeItemSlot> _rewardSlots = new List<UIChallengeItemSlot>();
	Stack<UIChallengeItemSlot> _rewardSlotPool = new Stack<UIChallengeItemSlot>();
	
	void addRewardSlot(string rewardId)
	{
		UIChallengeItemSlot s;
		
		if(_rewardSlotPool.Count > 0 )
		{
			s = _rewardSlotPool.Pop();
		}
		else
		{
			s = Instantiate(roundRewardInfoSlot) as UIChallengeItemSlot;
		}
		
		_rewardSlots.Add(s);
		
		s.gameObject.SetActive(false);
		
		s.setData(rewardId);
		
		s.type = UIChallengeItemSlot.Type.StageRewardPreviewItem;
		s.useButton = true;
		
		s.transform.parent = tfRewardContainer;
		
		_v.x = (_rewardSlots.Count -1) * 142;

		s.transform.localPosition = _v;
	}


	public UIScrollView levelUpScrollView;
	public Transform tfLevelupContainer;
	public UIRoundClearLevelUpItemSlot roundLevelupInfoSlot;
	List<UIRoundClearLevelUpItemSlot> _levelupSlots = new List<UIRoundClearLevelUpItemSlot>();
	Stack<UIRoundClearLevelUpItemSlot> _levelupSlotPool = new Stack<UIRoundClearLevelUpItemSlot>();
	
	void addLevelupSlot(RoundClearLevelupItemData settingData)
	{

		UIRoundClearLevelUpItemSlot s;
		
		if(_levelupSlotPool.Count > 0 )
		{
			s = _levelupSlotPool.Pop();
		}
		else
		{
			s = Instantiate(roundLevelupInfoSlot) as UIRoundClearLevelUpItemSlot;
		}
		
		_levelupSlots.Add(s);
		
		s.gameObject.SetActive(false);
		
		s.setData(settingData);

		if(s.itemSlot != null && s.itemSlot.infoData != null)
		{
			s.itemSlot.setTransendLevel();
		}

		s.itemSlot.useButton = false;
		
		s.transform.parent = tfLevelupContainer;
		
		_v.x = (_levelupSlots.Count -1) * 142;
		
		s.transform.localPosition = _v;
	}




	void clearSlots()
	{

		foreach(UIRoundClearLevelUpItemSlot s in _levelupSlots)
		{
			_levelupSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_levelupSlots.Clear();


		foreach(UIChallengeItemSlot s in _rewardSlots)
		{
			_rewardSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_rewardSlots.Clear();
	}






	public RewardItemEffectSlotEventReceiver rewardEffect;
	
	public float motionSpeed = 80.0f;
	
	public TimeProgressCalc calc;
	int _step = 0;
	
	const float CELL_WIDTH = 142;

	const float xOffset = 35;


	bool _isRewardMotion = false;

	IEnumerator startDisplay()
	{
		while(ani != null && ani.isPlaying)
		{
			yield return null;
		}

		yield return 0.3f;

		_isRewardMotion = true;

		createSlots();

		yield return new WaitForSeconds(0.1f);

		Vector3 defaultPosition = roundRewardInfoSlot.transform.position;

		UIScrollView targetScrollView = rewardScrollView;


		Vector3 tempV = targetScrollView.transform.localPosition;
		float posX = tempV.x;
		
		int maxSlotNum = 4;

		int slotCount = _rewardSlots.Count;

		int slotEffectStartIndex = 2;

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic)
		{
			// gold & exp는 고정.
			switch(slotCount)
			{
			case 2:
				_rewardSlots[0].transform.localPosition = new Vector3(133,0,0);
				_rewardSlots[1].transform.localPosition = new Vector3(326,0,0);
				break;
			case 3:
				_rewardSlots[0].transform.localPosition = new Vector3(230-158,0,0);
				_rewardSlots[1].transform.localPosition = new Vector3(230,0,0);
				break;
			}
			
			_rewardSlots[0].gameObject.SetActive(true);
			_rewardSlots[1].gameObject.SetActive(true);
		}
		else
		{
			slotEffectStartIndex = 0;
		}

		for(int i = slotEffectStartIndex; i < slotCount; ++i)
		{
			_isPlayingEffect = true;
			
			if(i > (maxSlotNum-1))
			{
				tempV.x = 0; tempV.y = 0; tempV.z = 0;

				float p = posX - (  (i-(maxSlotNum-1)) * CELL_WIDTH ) + xOffset;

				while(true)
				{
					if(targetScrollView.transform.localPosition.x > p)
					{
						tempV.x = -motionSpeed;

						targetScrollView.MoveRelative(tempV);
					}
					else
					{
						tempV.x = p - targetScrollView.transform.localPosition.x;
						targetScrollView.MoveRelative(tempV);
						break;
					}

					yield return new WaitForSeconds(0.02f);
				}
			}
			else
			{
				switch(slotCount)
				{
				case 1:
					_rewardSlots[i].transform.localPosition = new Vector3(230,0,0);
					break;
				case 2:
					if(i == 0) _rewardSlots[i].transform.localPosition = new Vector3(133,0,0);
					else _rewardSlots[i].transform.localPosition = new Vector3(326,0,0);
					break;
				case 3:
					if(i == 0) _rewardSlots[i].transform.localPosition = new Vector3(230-158,0,0);
					else if(i == 1) _rewardSlots[i].transform.localPosition = new Vector3(230,0,0);
					else _rewardSlots[i].transform.localPosition = new Vector3(230+158,0,0);
					break;
				}
			}

			if( i > slotEffectStartIndex - 1)
			{
				defaultPosition = _rewardSlots[i].transform.position;
				rewardEffect.transform.position = defaultPosition;
				
				if(_rewardSlots[i].inputItemId != null)
				{
					rewardEffect.start(RewardItemEffectSlotEventReceiver.Type.RoundClearReward, _rewardSlots[i].inputItemId, i);
				}
				else
				{
					rewardEffect.start(RewardItemEffectSlotEventReceiver.Type.RoundClearReward, _rewardSlots[i].infoData.serverId, i);
				}
				
				while(_isPlayingEffect)
				{
					yield return null;
				}
				
				yield return new WaitForSeconds(0.1f);
			}
		}





		GameManager.setTimeScale = 2.0f;


		// 레벨업 슬롯.

		_isRewardMotion = false;

		targetScrollView = levelUpScrollView;

		tempV = targetScrollView.transform.localPosition;
		posX = tempV.x;

		slotCount = _levelupSlots.Count;

		for(int i = 0; i < slotCount; ++i)
		{
			_isPlayingEffect = true;
			
			if(i > (maxSlotNum-1))
			{
				tempV.x = 0; tempV.y = 0; tempV.z = 0;

				float p = posX - (  (i-(maxSlotNum-1)) * CELL_WIDTH ) + xOffset;

				if(_levelupSlots[i].itemData.plusLevel > 0)
				{
					while(true)
					{
						if(targetScrollView.transform.localPosition.x > p)
						{
							float mSpeed = motionSpeed * ((_levelupSlots[i].itemData.plusLevel > 0)?2f:1f);
							
							tempV.x = -mSpeed;
							targetScrollView.MoveRelative(tempV);
						}
						else
						{
							tempV.x = p - targetScrollView.transform.localPosition.x;
							targetScrollView.MoveRelative(tempV);
							break;
						}
						
						yield return new WaitForSeconds(0.02f);
					}
				}
			}

			if(_levelupSlots[i].itemData.plusLevel > 0)
			{
				defaultPosition = _levelupSlots[i].transform.position;
				rewardEffect.transform.position = defaultPosition;
				
				if(_levelupSlots[i].itemSlot.inputItemId != null)
				{
					rewardEffect.start(RewardItemEffectSlotEventReceiver.Type.RoundClearReward, _levelupSlots[i].itemSlot.inputItemId, i);
				}
				else
				{
					rewardEffect.start(RewardItemEffectSlotEventReceiver.Type.RoundClearReward, _levelupSlots[i].itemSlot.infoData.serverId, i);
				}
				
				while(_isPlayingEffect)
				{
					yield return null;
				}

				SoundData.play("uiet_levelup");

				_levelupSlots[i].particleLevelup.gameObject.SetActive(true);
				_levelupSlots[i].particleLevelup.Play();

				yield return new WaitForSeconds(0.1f);

			}
			else
			{
				//SoundData.play("uirn_runeattach");

				_levelupSlots[i].gameObject.SetActive (true);
				_levelupSlots[i].particleLevelup.gameObject.SetActive(false);
			}

			yield return new WaitForSeconds(0.1f);
		}


		if(slotCount > 4 && targetScrollView.transform.localPosition.x < 0)
		{
			tempV.x = 0; tempV.y = 0; tempV.z = 0;


			for(int i = _levelupSlots.Count - 1 ; i >= 0; --i)
			{
				if(_levelupSlots[i].particleLevelup != null) _levelupSlots[i].particleLevelup.gameObject.SetActive(false);
			}


			while(true)
			{
				if(targetScrollView.transform.localPosition.x < 0)
				{
					float mSpeed = motionSpeed * 0.2f;
					tempV.x += mSpeed;


					if( targetScrollView.transform.localPosition.x + tempV.x == 0)
					{
						targetScrollView.MoveRelative(tempV);
						break;
					}
					else if( targetScrollView.transform.localPosition.x + tempV.x > 0)
					{
						tempV.x = -targetScrollView.transform.localPosition.x;
						targetScrollView.MoveRelative(tempV);
						break;
					}
					else
					{
						targetScrollView.MoveRelative(tempV);
					}

				}
				else
				{
					tempV.x = -targetScrollView.transform.localPosition.x;
					targetScrollView.MoveRelative(tempV);
					break;
				}
				
				yield return new WaitForSeconds(0.01f);
			}
		}

		GameManager.setTimeScale = 1.0f;

		yield return new WaitForSeconds(0.2f);

		SoundData.play("uirn_synth_pre");

		btnClose.gameObject.SetActive(true);

		if(dragScollViewBoxColliders != null)
		{
			foreach(BoxCollider bc in dragScollViewBoxColliders)
			{
				bc.enabled = true;
			}
		}
	}
	
	bool _isPlayingEffect = false;
	
	public void onCompleteShowAni(int slotIndex)
	{
		if(_isRewardMotion)
		{
			SoundData.play("uirf_impact_old");

			_rewardSlots[slotIndex].gameObject.SetActive (true);
			if(_rewardSlots[slotIndex].particle != null)
			{
				_rewardSlots[slotIndex].particle.gameObject.SetActive(true);
				_rewardSlots[slotIndex].particle.Play();
			}
		}
		else
		{
			_levelupSlots[slotIndex].gameObject.SetActive (true);
			if(_levelupSlots[slotIndex].itemSlot.particle != null)
			{
				_levelupSlots[slotIndex].itemSlot.particle.gameObject.SetActive(true);
				_levelupSlots[slotIndex].itemSlot.particle.Play();
			}
		}


		_isPlayingEffect = false;
	}








}



public struct RoundClearLevelupItemData
{
	public string id;
	public int oldLevel;
	public float oldPercent;
	public float newPercent;
	public int plusLevel;
	public int rare;

	public float totalPercentDiff;


	public RoundClearLevelupItemData(int pLevel, int oLevel, float newPer, float oPer, string inputId, int inputRare)
	{
		plusLevel = pLevel;
		newPercent = newPer;
		oldLevel = oLevel;
		oldPercent = oPer;
		id = inputId;
		rare = inputRare;

		if(plusLevel == 0)
		{
			totalPercentDiff = (newPercent - oldPercent);
		}
		else
		{
			totalPercentDiff = plusLevel + (1-oldPercent) + newPercent;
		}
	}


	public static int sort(RoundClearLevelupItemData x, RoundClearLevelupItemData y)
	{
		int i = y.plusLevel.CompareTo(x.plusLevel);
		if(i == 0) return y.totalPercentDiff.CompareTo(x.totalPercentDiff);
		if(i == 0) return y.rare.CompareTo(x.rare);
		if(i == 0) return y.oldLevel.CompareTo(x.oldLevel);
		return i;
	}
}
