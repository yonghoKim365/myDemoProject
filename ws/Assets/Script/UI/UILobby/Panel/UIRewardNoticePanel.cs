using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRewardNoticePanel : MonoBehaviour 
{
	private Queue<P_Reward> _items = new Queue<P_Reward>();
	public Animation ani;

	private Vector3 _startPosition = new Vector3(0,400,0);

	public UISprite spIcon;
	public UILabel lbText;
	public bool isPlaying = false;


	public static bool checkReward(string code, bool startNow)
	{
		if(code.StartsWith("EN"))
		{
			P_Reward boughtShopItem = new P_Reward();
			boughtShopItem.count = 0;
			int.TryParse(code.Split('_')[1], out boughtShopItem.count);
			boughtShopItem.code = WSDefine.ENERGY;
			GameManager.me.uiManager.rewardNotice.start(startNow, boughtShopItem);
			return true;
		}
		else if(code.StartsWith("GO"))
		{
			P_Reward boughtShopItem = new P_Reward();
			boughtShopItem.count = 0;
			int.TryParse(code.Split('_')[1], out boughtShopItem.count);
			boughtShopItem.code = WSDefine.GOLD;
			GameManager.me.uiManager.rewardNotice.start(startNow, boughtShopItem);
			return true;
		}
		else if(code.StartsWith("RU"))
		{
			P_Reward boughtShopItem = new P_Reward();
			boughtShopItem.count = 0;
			int.TryParse(code.Split('_')[1], out boughtShopItem.count);
			boughtShopItem.code = WSDefine.RUBY;
			GameManager.me.uiManager.rewardNotice.start(startNow, boughtShopItem);
			return true;
		}
		else if(code.StartsWith("RS"))
		{
			P_Reward boughtShopItem = new P_Reward();
			boughtShopItem.count = 0;
			int.TryParse(code.Split('_')[1], out boughtShopItem.count);
			boughtShopItem.code = WSDefine.RUNESTONE;
			GameManager.me.uiManager.rewardNotice.start(startNow, boughtShopItem);
			return true;
		}

		return false;
	}



	public static void addGold(int gold)
	{
		P_Reward boughtShopItem = new P_Reward();
		boughtShopItem.count = gold;
		boughtShopItem.code = WSDefine.GOLD;
		GameManager.me.uiManager.rewardNotice.start(true, boughtShopItem);
	}


	public void start(bool playRightNow, params P_Reward[] data )
	{
		if(data != null)
		{
			for(int i =0; i < data.Length; ++i)
			{
				if((data[i].code != WSDefine.REWARD_TYPE_ITEM && data[i].code != WSDefine.REWARD_TYPE_GACHA) )
				{
					_items.Enqueue(data[i]);
				}
			}
		}

		if(_items.Count > 0 && isPlaying == false)
		{
			transform.localPosition = _startPosition;
			gameObject.SetActive(true);
			if(playRightNow) next ();
			else isPlaying = false;
		}
	}


	public void next()
	{
		if(_items.Count > 0)
		{
			P_Reward temp = _items.Dequeue();

			spIcon.spriteName = WSDefine.getItemIconByRewardCode(temp.code);
			spIcon.MakePixelPerfect();

//			Debug.LogError(temp.code + " spIcon.spriteName : "  +  spIcon.spriteName);

			switch(temp.code)
			{
			case WSDefine.REWARD_TYPE_FRIENDPOINT:
			case WSDefine.REWARD_TYPE_EXP:
				lbText.text = Util.getUIText("GET_REWARD_COUNT",temp.count.ToString());
				break;
			case WSDefine.REWARD_TYPE_GOLD:
				lbText.text = Util.getUIText("GET_REWARD_GOLD_COUNT", Util.GetCommaScore( temp.count ));
				break;
			default:
				lbText.text = Util.getUIText("GET_REWARD_COUNT1",temp.count.ToString());
				break;
			}

			isPlaying = true;
			ani.Play("NoticeGetReward");
		}
		else
		{
			hide ();
		}
	}

	void Update()
	{
		if(isPlaying && ani.isPlaying == false)
		{
			next();
		}
	}


	public void hide()
	{
		gameObject.SetActive(false);
		transform.localPosition = _startPosition;
		isPlaying = false;
	}

}
