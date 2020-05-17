using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class JackPotDisplayManager : MonoBehaviour 
{
	public UIButton btnUser, btnItem;

	public BoxCollider bcUser, bcItem;

	public UILabel lbUser, lbMiddle, lbItem, lbEnd;

	Vector3 _v, _v2;

	private int _lastDisplayItemDateTime = 0;


	void Awake()
	{
		UIEventListener.Get( btnUser.gameObject ).onClick = onClickUser;
		UIEventListener.Get( btnItem.gameObject ).onClick = onClickItem;
	}


	void onClickUser(GameObject go)
	{
		if(_currentDisplayJackPotData != null)
		{
			if(_currentDisplayJackPotData.userId != PandoraManager.instance.localUser.userID)
			{
				EpiServer.instance.sendGetOtherData(_currentDisplayJackPotData.userId);
			}
		}
	}

	void onClickItem(GameObject go)
	{
		if(_dummy != null)
		{
			_dummy.showInfoPopup(false);
		}
	}


	Dictionary<string, P_Jackpot> _jackpotData = new Dictionary<string, P_Jackpot>();

	List<P_Jackpot> _jackpotList = new List<P_Jackpot>();


	private int _nowDisplayIndex = 0;

	public void parseJackpotServerData(ToC_GET_JACKPOT_USERS data)
	{
		if(data == null)
		{
			return;
		}

		foreach(KeyValuePair<string, P_Jackpot> kv in data.jackpots)
		{
			if(_jackpotData.ContainsKey(kv.Key) == false)
			{
				_jackpotData.Add(kv.Key, kv.Value);
			}
		}

		_jackpotList.Clear();

		string[] tempP;

		int startIndex = 0;

		foreach(KeyValuePair<string, P_Jackpot> kv in _jackpotData)
		{
			P_Jackpot p = kv.Value;

			if(p.userId == PandoraManager.instance.localUser.userID) continue;

			if(startIndex >= 0)
			{
				if(p.date > _lastDisplayItemDateTime)
				{
					_nowDisplayIndex = startIndex;
					startIndex = -1;
				}
				else
				{
					++startIndex;
				}
			}

			if(p.product.Contains("/"))
			{
				tempP = p.product.Split('/');
				p.product = tempP[0] + " " + tempP[1];
			}

			_jackpotList.Add(kv.Value);
		}


		if(_jackpotList.Count <= _nowDisplayIndex)
		{
			_nowDisplayIndex = 0;
		}

		_defaultDelay = 3600f / _jackpotList.Count / 10f;

		_nextDelay = _defaultDelay * (UnityEngine.Random.Range(0.5f, 2.0f));

#if UNITY_EDITOR
		Debug.Log("TEST MODE");
		_nextDelay = 2f;
#endif

	}

	private float _defaultDelay = 0.0f;

	private P_Jackpot _currentDisplayJackPotData;
	private Stack<P_Jackpot> _pool = new Stack<P_Jackpot>();

	P_Jackpot getNewJackPotData()
	{
		if(_pool.Count > 0)
		{
			return _pool.Pop();
		}

		return new P_Jackpot();
	}

	private Queue<P_Jackpot> _myJackPotList = new Queue<P_Jackpot>();




	GameIDData _checkGd = new GameIDData();

	public void addMyJackPot(string itemId, string productName)
	{
		StartCoroutine(addMyJackPotCT(itemId, productName));
	}

	IEnumerator addMyJackPotCT(string itemId, string productName)
	{
		yield return new WaitForSeconds(6.0f);

		_checkGd.parse(itemId);
		if(_checkGd.rare >= RareType.S)
		{
			P_Jackpot p = getNewJackPotData();
			p.itemId = itemId;
			p.userId = PandoraManager.instance.localUser.userID;
			p.nickname = GameDataManager.instance.name;
			p.product = productName;
			
			p.date = Util.DateTimeToTimeStamp(DateTime.UtcNow);
			
			_myJackPotList.Enqueue(p);
			
			if(_nextDelay > 8.0f)
			{
				_nextDelay = 8.0f;
			}
		}
	}



	private float _hideDelay = 0.0f;
	private float _nextDelay = 10.0f;

	void LateUpdate()
	{
		if(_hideDelay > 0)
		{
			_hideDelay -= Time.smoothDeltaTime;
		}
		else if(_hideDelay > -10)
		{
			lbEnd.enabled = false;
			lbMiddle.enabled = false;
			btnItem.gameObject.SetActive(false);
			btnUser.gameObject.SetActive(false);
			_hideDelay = -1000;
		}


		if(_nextDelay > 0)
		{
			_nextDelay -= Time.smoothDeltaTime;
		}
		else
		{

			if(_jackpotList.Count <= _nowDisplayIndex) _nowDisplayIndex = 0;
			
			if(_currentDisplayJackPotData != null && _currentDisplayJackPotData.userId == PandoraManager.instance.localUser.userID)
			{
				_pool.Push(_currentDisplayJackPotData);
				_currentDisplayJackPotData = null;
			}
			
			if(_myJackPotList.Count > 0 && RuneStudioMain.instance.step == RuneStudioMain.Step.Finish)
			{
				_currentDisplayJackPotData = _myJackPotList.Dequeue();
			}
			else
			{
				if(_jackpotList.Count > 0)
				{
					_currentDisplayJackPotData = _jackpotList[_nowDisplayIndex];
					_lastDisplayItemDateTime = _currentDisplayJackPotData.date;
					++_nowDisplayIndex;
				}
				else _currentDisplayJackPotData = null;
			}
			
			if(_currentDisplayJackPotData != null)
			{
				display( _currentDisplayJackPotData );
			}

		}

	}
	
	
	GameIDData _dummy = new GameIDData();

	public void display(P_Jackpot data)
	{
		_hideDelay = 20.0f;
		_nextDelay = _defaultDelay * (UnityEngine.Random.Range(0.5f, 2.0f));


		lbEnd.enabled = true;
		lbMiddle.enabled = true;
		btnItem.gameObject.SetActive(true);
		btnUser.gameObject.SetActive(true);
		
		lbUser.text = data.nickname;

		lbMiddle.text = Util.getUIText("BUYITEM_NOTICE",data.product+" ");

		_dummy.parse(data.itemId);

		lbItem.text = _dummy.itemName;

		lbEnd.text = Util.getHangulJosa(_dummy.itemName.Replace("[-]","")) + " " + Util.getUIText("EARN_ITEM");

		_v = bcUser.extents;
		_v.x = lbUser.printedSize.x * 0.5f;
		bcUser.extents = _v;

		_v = bcUser.center;
		_v.x = lbUser.printedSize.x * 0.5f;
		bcUser.center = _v;


		_v = bcItem.extents;
		_v.x = lbItem.printedSize.x * 0.5f;
		bcItem.extents = _v;

		_v = bcItem.center;
		_v.x = lbItem.printedSize.x * 0.5f;
		bcItem.center = _v;

		_v  = bcUser.transform.localPosition;


		float totalSize = lbUser.printedSize.x + lbMiddle.printedSize.x + lbItem.printedSize.x + lbEnd.printedSize.x;
		
		_v = btnUser.transform.localPosition;
		_v.x = (1000 - totalSize) * 0.5f;
		btnUser.transform.localPosition = _v;


		_v2  = lbMiddle.transform.localPosition;
		_v2.x = _v.x + lbUser.printedSize.x;
		lbMiddle.transform.localPosition = _v2;

		_v = _v2;

		_v2  = bcItem.transform.localPosition;
		_v2.x = _v.x + lbMiddle.printedSize.x;
		bcItem.transform.localPosition = _v2;


		_v = _v2;
		
		_v2  = lbEnd.transform.localPosition;
		_v2.x = _v.x + lbItem.printedSize.x;
		lbEnd.transform.localPosition = _v2;

	}


	void OnDisable()
	{
		lbEnd.enabled = false;
		lbMiddle.enabled = false;
		btnItem.gameObject.SetActive(false);
		btnUser.gameObject.SetActive(false);

		_nextDelay = 5.0f;
	}

	
}
