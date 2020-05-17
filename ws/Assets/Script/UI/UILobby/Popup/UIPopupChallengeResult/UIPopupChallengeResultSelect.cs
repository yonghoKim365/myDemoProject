using UnityEngine;
using System.Collections;

public class UIPopupChallengeResultSelect : MonoBehaviour 
{
	private int _selectCount;

	public UIChallengeItemSlot[] slots;

	public UISprite[] spStars;

	public string[] rewardItems;

	public Animation ani;

	void Awake()
	{

	}

	bool _close = false;

	void setStarSprite(int inputRank)
	{
		_close = false;

		for(int i = 0; i < 3; ++i)
		{
			if(i < inputRank)
			{
				spStars[i].spriteName = "img_result_rank_star_on_star";
			}
			else
			{
				spStars[i].spriteName = "img_result_rank_star_off_star";
			}
			
		}
	}

	private int _totalStar;

	public void start(int star)
	{
		/*
		for(int i = slots.Length - 1; i >= 0; --i)
		{
			slots[i].spCloseCover.gameObject.SetActive(true);
			slots[i].index = i;
			slots[i].useButton = true;
			slots[i].type = UIChallengeItemSlot.Type.SelectChampionshipResultSlot;
		}

		_totalStar = star;
		_selectCount = 0;

		setStarSprite(_totalStar);

		RuneStudioMain.instance.type = RuneStudioMain.Type.ChallengeResult;
		RuneStudioMain.instance.challengeResult1Container.gameObject.SetActive(false);

		transform.localPosition = new Vector3(0,700,0);
		gameObject.SetActive(true);

		TweenPosition tw0 = TweenPosition.Begin(GameManager.me.uiManager.popupChallengeResult.mainPanel.gameObject, 0.6f, new Vector3(0,-1000,0));

		TweenPosition tw = TweenPosition.Begin(gameObject, 0.6f, Vector3.zero);
		tw.eventReceiver = gameObject;
		tw.callWhenFinished = "onFinishedTweening";
		tw.method = UITweener.Method.EaseIn;
		*/

	}

	void onFinishedTweening()
	{
		/*
		RuneStudioMain.instance.challengeResult2Container.gameObject.SetActive(true);
		
		for(int i = 0; i < 3; ++i)
		{
			RuneStudioMain.instance.challengeResult2StarGameObject[i].gameObject.SetActive( i < _totalStar );
		}

		setStarSprite(0);
		*/
	}


	public void close()
	{
		/*
		TweenPosition tw = TweenPosition.Begin(gameObject, 0.5f, new Vector3(0,-1000,0));
		GameManager.me.uiManager.popupChallengeResult.onCompleteSelectScene();
		*/
	}


	public void hide()
	{
		gameObject.SetActive(false);
	}


	public void onClick(UIChallengeItemSlot clickItem)
	{
		/*
		if(_selectCount >= _totalStar) return;

		RuneStudioMain.instance.challengeResult2Star[_totalStar - _selectCount - 1].enabled = true;
		clickItem.setData(rewardItems[_selectCount]);
		++_selectCount;
		StartCoroutine(delayShowSlot(clickItem.index, clickItem));
		*/
	}

	IEnumerator delayShowSlot(int slotIndex, UIChallengeItemSlot clickItem)
	{
		/*
		RuneStudioMain.instance.challengeResultSlotFSM[slotIndex].enabled = true;
		clickItem.spCloseCover.gameObject.SetActive(false);

		if(_selectCount >= _totalStar && _close == false)
		{
			_close = true;
			yield return new WaitForSeconds(4.0f);
			close();
		}
		*/

		yield return null;
	}
}
