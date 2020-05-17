using UnityEngine;
using System.Collections;

public class RewardItemEffectSlotEventReceiver : MonoBehaviour 
{
	public UIChallengeItemSlot slot;

	public Animation ani;

	private int _targetIndex = 0;
	private Type _type;

	public enum Type
	{
		HellReward, RoundClearReward
	}

	public void start(Type type, string id, int targetIndex)
	{
//		Debug.LogError("start : " + targetIndex);
		_type = type;
		_targetIndex = targetIndex;
		slot.setData(id);
		ani.Rewind();
		ani.Play();
	}


	public void onCompleteTween()
	{
//		Debug.LogError("complete!");

		if(_type == Type.HellReward)
		{
			GameManager.me.uiManager.popupHellResult.onCompleteShowAni(_targetIndex);
		}
		else if(_type == Type.RoundClearReward)
		{
			GameManager.me.uiManager.popupRoundClear.onCompleteShowAni(_targetIndex);
		}

		slot.gameObject.SetActive(false);
	}
}
