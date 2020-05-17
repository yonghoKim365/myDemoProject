using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPopupHellResult : UIPopupBase {

	public UILabel lbDistance, lbKillCount, lbWaveNumber, lbScore;

	public BoxCollider dragScrollViewBoxCollider;

	private bool _isEnabled = false;

	protected override void awakeInit ()
	{
	}

	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		GameManager.me.uiManager.stageClearEffectManager.resetPlayMaker();
	}

	public override void show ()
	{
		base.show ();
	}

	public void show(string[] rewards)
	{
		show ();

		if(calc == null)
		{
			calc = gameObject.AddComponent<TimeProgressCalc>();
		}

		GameManager.me.uiManager.uiPlay.hideMenu();
		btnClose.gameObject.SetActive(false);
		if(dragScrollViewBoxCollider != null) dragScrollViewBoxCollider.enabled = false;

		lbDistance.text = HellModeManager.instance.getTotalDistance() + "m";
		lbKillCount.text = HellModeManager.instance.killUnitCount + "";
		lbWaveNumber.text = "x" + HellModeManager.instance.roundIndex + "";
		
		lbScore.text = "0";

		_isEnabled = true;
		
		StartCoroutine(startDisplay(rewards));
	}




	public RewardItemEffectSlotEventReceiver rewardEffect;

	public float motionSpeed = 80.0f;

	public TimeProgressCalc calc;
	int _step = 0;

	const float xOffset = 35;

	const float CELL_WIDTH = 142;

	IEnumerator startDisplay(string[] rewards)
	{
		while(ani != null && ani.isPlaying)
		{
			yield return null;
		}

		yield return 0.2f;

		init (rewards);

		yield return 0.5f;

		_step = 0;

		_targetScore = HellModeManager.instance.totalScore;

		calc.start( 1 , setScore );

		while(_step == 0)
		{
			yield return null;
		}

		Vector3 defaultPosition = roundRewardInfoSlot.transform.position;

		UIScrollView targetScrollView = rewardScrollView;

		if(_rewardSlots.Count <= 0)
		{
			btnClose.gameObject.SetActive(true);
			if(dragScrollViewBoxCollider != null) dragScrollViewBoxCollider.enabled = true;
		}
		else
		{
			Vector3 tempV = rewardScrollView.transform.localPosition;
			float posX = tempV.x;

			int slotCount = _rewardSlots.Count;
			int maxSlotNum = 4;

			for(int i = 0; i < slotCount; ++i)
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

				defaultPosition = _rewardSlots[i].transform.position;
				rewardEffect.transform.position = defaultPosition;

				rewardEffect.start(RewardItemEffectSlotEventReceiver.Type.HellReward, _rewardSlots[i].inputItemId, i);
				while(_isPlayingEffect)
				{
					yield return null;
				}

				yield return new WaitForSeconds(0.1f);

			}
		}

		yield return new WaitForSeconds(0.2f);

		SoundData.play("uirn_synth_pre");

		btnClose.gameObject.SetActive(true);
		if(dragScrollViewBoxCollider != null) dragScrollViewBoxCollider.enabled = true;
	}

	bool _isPlayingEffect = false;

	public void onCompleteShowAni(int slotIndex)
	{
		SoundData.play("uirf_impact_old");

		_rewardSlots[slotIndex].gameObject.SetActive (true);
		if(_rewardSlots[slotIndex].particle != null)
		{
			_rewardSlots[slotIndex].particle.gameObject.SetActive(true);
			_rewardSlots[slotIndex].particle.Play();
		}
		_isPlayingEffect = false;
	}


	private int _targetScore = 0;

	void setScore( float step, bool isComplete )
	{
		int totalScore = _targetScore;

		if(isComplete == false)
		{
			totalScore = Mathf.RoundToInt(Mathf.Lerp(0, totalScore, step));
		}
		else
		{
			_step = 1;
		}

		lbScore.text = Util.GetCommaScore( totalScore );
	}




	public void init(string[] rewards)
	{


		rewardScrollView.transform.localPosition = Vector3.zero;
		rewardScrollView.panel.clipOffset = Vector2.zero;
		rewardScrollView.ResetPosition();

		if(rewards != null)
		{
			foreach(string str in rewards)
			{
				addRewardSlot(str);
			}
		}
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
		
		_v.x = (_rewardSlots.Count -1) * CELL_WIDTH;
		
		s.transform.localPosition = _v;
	}







	public override void hide (bool isInit = false)
	{
		base.hide(isInit);

		if(isInit) return;

		clearSlots();

		if(_isEnabled)
		{
			_isEnabled = false;

			#if UNITY_EDITOR
			
			if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_MENU) return;
			
			#endif

			GameManager.me.returnToSelectScene();
		}
	}



	void clearSlots()
	{

		foreach(UIChallengeItemSlot s in _rewardSlots)
		{
			_rewardSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_rewardSlots.Clear();
	}




	public int rewardGold = 0;
	public string[] rewardItems = null;

	public void showHellResult()
	{
		if(rewardGold > 0)
		{
			List<string> r = new List<string>();
			r.Add("GOLD_"+rewardGold);
			
			if(rewardItems != null)
			{
				r.AddRange(rewardItems);
				GameManager.me.uiManager.popupHellResult.show(r.ToArray());
			}
		}
		else		
		{
			GameManager.me.uiManager.popupHellResult.show(rewardItems);
		}
	}


}
