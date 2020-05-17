using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupEquipRuneBook : UIPopupBase 
{


	public Transform[] tfRankTitle;
	int[] rareSlotNum = new int[RareType.RUNE_BOOK_SLOT_NUM];


	public UIButton btnLeo, btnKiley, btnChloe, btnLuke;

	public UIPoupHeroSelectTab goLeoOn, goKileyOn, goChloeOn;

	public UIButton btnS, btnSS;
	public UISprite spGradeS, spGradeSS;
	

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnLeo.gameObject).onClick = onClickLeo;
		UIEventListener.Get(btnKiley.gameObject).onClick = onClickKiley;
		UIEventListener.Get(btnChloe.gameObject).onClick = onClickChloe;

		UIEventListener.Get( btnS.gameObject ).onClick = onClickSGrade;
		UIEventListener.Get( btnSS.gameObject ).onClick = onClickSSGrade;
	}


	void onClickSGrade(GameObject go)
	{
		changeTab(RareType.S);
	}
	
	void onClickSSGrade(GameObject go)
	{
		changeTab(RareType.SS);
	}
	
	
	void changeTab(int rare)
	{
		if(rare == RareType.SS)
		{
			tfRankTitle[RareType.SS].gameObject.SetActive(true);
			tfRankTitle[RareType.S].gameObject.SetActive(false);
			
			spGradeS.spriteName = "img_list_titlebg_s_off";
			spGradeSS.spriteName = "img_list_titlebg_ss_on";
			
		}
		else if(rare == RareType.S)
		{
			tfRankTitle[RareType.SS].gameObject.SetActive(false);
			tfRankTitle[RareType.S].gameObject.SetActive(true);
			
			spGradeS.spriteName = "img_list_titlebg_s_on";
			spGradeSS.spriteName = "img_list_titlebg_ss_off";
		}

		tfRankTitle[RareType.D].gameObject.SetActive(_nowCharacter != Character.CHLOE);
		tfRankTitle[RareType.C].gameObject.SetActive(_nowCharacter != Character.CHLOE);
	}




	public override void show ()
	{
		base.show ();
	}

	public override void hide (bool isInit = false)
	{
		base.hide ();

		if(isInit) return;
		clearSlots();
	}

	string _nowCharacter = "";



	void onClickLeo(GameObject go)
	{
		btnLeo.gameObject.SetActive(false);
		goLeoOn.gameObject.SetActive(true);

		btnKiley.gameObject.SetActive(true);
		goKileyOn.gameObject.SetActive(false);

		btnChloe.gameObject.SetActive(true);
		goChloeOn.gameObject.SetActive(false);

		_v = btnKiley.transform.localPosition;
		_v.x = 425;
		btnKiley.transform.localPosition = _v;

		_nowCharacter = Character.LEO;
		EpiServer.instance.sendEquipmentHistory(_nowCharacter);
	}

	void onClickKiley(GameObject go)
	{
		btnLeo.gameObject.SetActive(true);
		goLeoOn.gameObject.SetActive(false);
		
		btnKiley.gameObject.SetActive(false);
		goKileyOn.gameObject.SetActive(true);		

		btnChloe.gameObject.SetActive(true);
		goChloeOn.gameObject.SetActive(false);

		_nowCharacter = Character.KILEY;
		EpiServer.instance.sendEquipmentHistory(_nowCharacter);
	}

	void onClickChloe(GameObject go)
	{
		btnLeo.gameObject.SetActive(true);
		goLeoOn.gameObject.SetActive(false);
		
		btnKiley.gameObject.SetActive(true);
		goKileyOn.gameObject.SetActive(false);		

		_v = btnKiley.transform.localPosition;
		_v.x = 354;
		btnKiley.transform.localPosition = _v;


		btnChloe.gameObject.SetActive(false);
		goChloeOn.gameObject.SetActive(true);		

		_nowCharacter = Character.CHLOE;
		EpiServer.instance.sendEquipmentHistory(_nowCharacter);		
	}



	public void init()
	{
		show ();

		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			rareSlotNum[i] = 0;
		}

		scrollView.transform.localPosition = Vector3.zero;
		scrollView.panel.clipOffset = Vector2.zero;
		scrollView.ResetPosition();

		switch(UIHero.nowHero)
		{
		case Character.KILEY:
			onClickKiley(null);
			break;
		case Character.CHLOE:
			onClickChloe(null);
			break;
		default:
			onClickLeo(null);
			break;
		}

		changeTab(RareType.S);
	}


	public void onCompleteRecieveEquipRuneHistory(Dictionary<string, int> historyEquipments)
	{
		refresh(historyEquipments);
	}


	public UIScrollView scrollView;

	public UIChallengeItemSlot itemInfoSlot;
	List<UIChallengeItemSlot> _slots = new List<UIChallengeItemSlot>();
	Stack<UIChallengeItemSlot> _slotPool = new Stack<UIChallengeItemSlot>();
	
	void addItemSlot( RuneBookData slotData ) //  string itemId, int rare, bool isSkill)
	{
		UIChallengeItemSlot s;
		
		if(_slotPool.Count > 0 )
		{
			s = _slotPool.Pop();
		}
		else
		{
			s = Instantiate(itemInfoSlot) as UIChallengeItemSlot;
		}
		
		_slots.Add(s);
		
		s.gameObject.SetActive(true);

		string id = slotData.baseId;

		s.setData(id.Substring(0,id.LastIndexOf("_"))+id.Substring(id.Length-2,1)+"_"+id.Substring(id.Length-1)+"1_20");		

		s.setBookType(UIChallengeItemSlot.Type.BookEquipType, slotData);

		s.transform.parent = tfRankTitle[slotData.rare];
#if UNITY_EDITOR
		s.name = slotData.rare+"_"+rareSlotNum[slotData.rare];
#endif

		_v.x = 74  + ((rareSlotNum[slotData.rare]) % 8) * 126;
		_v.y = -128 - ((rareSlotNum[slotData.rare]) / 8) * 128;
		_v.z = 0;

		s.transform.localPosition = _v;

		++rareSlotNum[slotData.rare];
	}


	void clearSlots()
	{
		
		foreach(UIChallengeItemSlot s in _slots)
		{
			_slotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_slots.Clear();
	}


	int _hasCount = 0;
	int _totalCount = 0;

	Dictionary<string, RuneBookData>[] _book = new Dictionary<string, RuneBookData>[RareType.RUNE_BOOK_SLOT_NUM];
	List<RuneBookData> _bookList = new List<RuneBookData>();


	public void refresh(Dictionary<string, int> historyEquipments)
	{
		_hasCount = 0;
		_totalCount = 0;

		clearSlots();


		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			rareSlotNum[i] = 0;
		}
		
		scrollView.transform.localPosition = Vector3.zero;
		scrollView.panel.clipOffset = Vector2.zero;
		scrollView.ResetPosition();



		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			if(_book[i] == null) _book[i] = new Dictionary<string, RuneBookData>();
			else _book[i].Clear();
		}

		foreach(KeyValuePair<string, BaseHeroPartsData > kv in GameManager.info.heroBasePartsData)
		{
			if(kv.Value.isBook == false) continue;
			if(kv.Value.character != _nowCharacter) continue;

			RuneBookData bs = new RuneBookData(kv.Key);
			bs.baseLevel = 20;

			switch(kv.Key.Substring(kv.Key.Length-2,1))
			{
			case "1":
				if(_nowCharacter == Character.CHLOE) continue;
				bs.rare = RareType.D;
				break;
			case "2":
				if(_nowCharacter == Character.CHLOE) continue;
				bs.rare = RareType.C;
				break;
			case "3":
				bs.rare = RareType.B;
				break;
			case "4":
				bs.rare = RareType.A;
				break;
			case "5":
				bs.rare = RareType.S;
				break;
			case "6":
				bs.rare = RareType.SS;
				break;
			}

			_book[bs.rare].Add(kv.Key, bs);
			++_totalCount;
		}
		
		RuneBookData tempRb;
		
		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.partsInventory)
		{
			if(_book[kv.Value.rare].ContainsKey(kv.Value.baseId) == false) continue;
			if(kv.Value.partsData.character != _nowCharacter) continue;

			tempRb = _book[kv.Value.rare][ kv.Value.baseId ];
			++tempRb.nowCount;
			_book[kv.Value.rare][ kv.Value.baseId ] = tempRb;
		}

		GameIDData gd;
		if(GameDataManager.instance.heroes.ContainsKey(_nowCharacter))
		{
			gd = GameDataManager.instance.heroes[_nowCharacter].partsHead.itemInfo;

			if(_book[gd.rare].ContainsKey(gd.baseId))
			{
				tempRb = _book[gd.rare][ gd.baseId ];
				++tempRb.nowCount;
				_book[gd.rare][ gd.baseId ] = tempRb;
			}

			gd = GameDataManager.instance.heroes[_nowCharacter].partsWeapon.itemInfo;
			
			if(_book[gd.rare].ContainsKey(gd.baseId))
			{
				tempRb = _book[gd.rare][ gd.baseId ];
				++tempRb.nowCount;
				_book[gd.rare][ gd.baseId ] = tempRb;
			}

			gd = GameDataManager.instance.heroes[_nowCharacter].partsBody.itemInfo;
			
			if(_book[gd.rare].ContainsKey(gd.baseId))
			{
				tempRb = _book[gd.rare][ gd.baseId ];
				++tempRb.nowCount;
				_book[gd.rare][ gd.baseId ] = tempRb;
			}

			gd = GameDataManager.instance.heroes[_nowCharacter].partsVehicle.itemInfo;
			
			if(_book[gd.rare].ContainsKey(gd.baseId))
			{
				tempRb = _book[gd.rare][ gd.baseId ];
				++tempRb.nowCount;
				_book[gd.rare][ gd.baseId ] = tempRb;
			}
		}

		
		string[] s = new string[2];

		if(historyEquipments != null)
		{
			foreach(KeyValuePair<string, int> kv in historyEquipments)
			{
				for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
				{
					if(_book[i].ContainsKey(kv.Key))
					{
						tempRb = _book[i][kv.Key];
						++tempRb.hasCount;
						_book[i][kv.Key] = tempRb;
						continue;
					}
				}
			}

		}

		refreshProcess2();

		changeTab(RareType.S);
	}


	void refreshProcess2()
	{
		_bookList.Clear();

		for(int i = RareType.SS; i >= 0; --i)
		{
			if(i < RareType.S)
			{
				_v = tfRankTitle[i].transform.position;
				_v.y = _slots[_slots.Count-1].transform.position.y - 100.0f;
				tfRankTitle[i].transform.position = _v;
			}
			
			foreach(KeyValuePair<string, RuneBookData> kv in _book[i])
			{
				if(kv.Value.hasCount > 0 || kv.Value.nowCount > 0) ++_hasCount;
				_bookList.Add(kv.Value);
			}
			
			_book[i].Clear();
			
			_bookList.Sort(RuneBookData.equipSort);

			int len = _bookList.Count;
			
			for(int j = 0; j < len; ++j)
			{
				addItemSlot( _bookList[j] );
			}
			
			_bookList.Clear();
			// 그린다...
		}
		
		switch(_nowCharacter)
		{
		case Character.LEO:
			goLeoOn.lbCount.text = "[fed627]"+_hasCount + "[-]/" + _totalCount;
			break;
		case Character.KILEY:
			goKileyOn.lbCount.text = "[fed627]"+_hasCount + "[-]/" + _totalCount;
			break;
		case Character.CHLOE:
			goChloeOn.lbCount.text = "[fed627]"+_hasCount + "[-]/" + _totalCount;
			break;
		}
	}
}



