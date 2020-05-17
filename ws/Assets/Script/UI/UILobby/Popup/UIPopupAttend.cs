using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

public class UIPopupAttend : UIPopupBase 
{
	public UIAttendSlot[] slots = new UIAttendSlot[28];

	UIAttendSlot nowSlot;

	public UILabel lbTitle, lbDescription;

	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public bool isPlaying = false;

	int _today = 0;

	public override void show ()
	{
		if(GameDataManager.instance.attendData == null)
		{
			return;
		}

		lbTitle.text = GameDataManager.instance.attendData.title;
		lbDescription.text = GameDataManager.instance.attendData.desc;

		isPlaying = true;

		Dictionary<string, string> attendTable = GameDataManager.instance.attendData.rewardTable;
		int day = GameDataManager.instance.attendData.today;

		Vector3 v = new Vector3();
		
		for(int i = 0 ; i < 28; ++i)
		{
			if(slots[i] == null)
			{
				slots[i] = (UIAttendSlot)Instantiate(slots[0]);
				slots[i].transform.parent = slots[0].transform.parent;
			}
			
			v.x = 137 *  (i % 7) + 2;
			v.y = -117 * ( (int) i / 7 );
			
			slots[i].transform.localPosition = v;
			
			slots[i].lbDay.text = (i+1).ToString();
		}

		UIAttendSlot s;

		foreach(KeyValuePair<string, string> kv in attendTable)
		{
			int slotDay = 1;
			int.TryParse(kv.Key, out slotDay);

			s = slots[slotDay-1];
			s.spCheck.enabled = (slotDay < day); 

			if(slotDay == day)
			{
				nowSlot = s;
			}

			s.setData(kv.Value);

		}

		btnClose.gameObject.SetActive(false);

		base.show();

		StartCoroutine(play () );

		_today = GameDataManager.instance.attendData.today;

		GameDataManager.instance.attendData = null;
	}




	public Animation nowSlotAnimation;

	IEnumerator play()
	{
		while(ani.isPlaying)
		{
			yield return null;
		}

		yield return Util.ws02;



		GameManager.me.uiManager.popupAttendReward.slot.setData(nowSlot.data);
		GameManager.me.uiManager.popupAttendReward.slot.lbDay.enabled = false;
		GameManager.me.uiManager.popupAttendReward.slot.spCheck.enabled = false;

		GameManager.me.uiManager.popupAttendReward.lbMsg.text = Util.getUIText("GET_ATTENDGIFT",_today.ToString());

		nowSlotAnimation.transform.parent.transform.localPosition = nowSlot.transform.localPosition;
		nowSlotAnimation.gameObject.SetActive(true);

		nowSlotAnimation.Play();

		while(nowSlotAnimation.isPlaying)
		{
			yield return null;
		}

		SoundData.play("uirf_impact_old");

		nowSlot.spCheck.enabled = true;

		yield return Util.ws1;

		GameManager.me.uiManager.popupAttendReward.show();

		nowSlotAnimation.gameObject.SetActive(false);

		yield return null;

		btnClose.gameObject.SetActive(true);

	}


	protected override void onClickClose (GameObject go)
	{
		base.onClickClose(go);

		isPlaying = false;
	}


}
