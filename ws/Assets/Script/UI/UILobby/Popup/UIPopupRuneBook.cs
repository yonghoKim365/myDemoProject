using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupRuneBook : UIPopupBase 
{


	public UILabel lbTitle, lbCount;

	public Transform[] tfRankTitle;
	int[] rareSlotNum = new int[RareType.RUNE_BOOK_SLOT_NUM];

	public UIButton btnS, btnSS;
	public UISprite spGradeS, spGradeSS;

	protected override void awakeInit ()
	{

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

	public enum Type
	{
		SkillBook, UnitBook
	}

	Type _type;

	public void init(Type type)
	{
		show ();

		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			rareSlotNum[i] = 0;
		}

		_type = type;

		scrollView.transform.localPosition = Vector3.zero;
		scrollView.panel.clipOffset = Vector2.zero;
		scrollView.ResetPosition();


		if(_type == Type.SkillBook)
		{
			lbTitle.text = Util.getUIText("SKILL_RUNE") + " " + Util.getUIText("BOOK");
			EpiServer.instance.sendSkillRuneHistory();
			//refreshSkill();
		}
		else
		{
			lbTitle.text = Util.getUIText("UNIT_RUNE") + " " + Util.getUIText("BOOK");
			EpiServer.instance.sendUnitRuneHistory();
			//refreshUnit();
		}

		changeTab(RareType.S);
	}

	public void onCompleteReceiveUnitRuneHistory()
	{
		if(gameObject.activeInHierarchy == false) return;
		refreshUnit();
	}

	public void onCompleteRecieveSkillRuneHistory()
	{
		if(gameObject.activeInHierarchy == false) return;
		refreshSkill();
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

		if(_type == Type.SkillBook)
		{
			s.setData(slotData.baseId + "_20_0");	
		}
		else
		{
			s.setData(slotData.baseId + "02001_0");		
		}

		if(_type == Type.SkillBook)
		{
			s.setBookType(UIChallengeItemSlot.Type.BookSkillType, slotData);
		}
		else
		{
			s.setBookType(UIChallengeItemSlot.Type.BookUnitType, slotData);
		}



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


		if(_type == Type.UnitBook)
		{
			GameManager.me.characterManager.clearUnusedResource(true);
			GameManager.me.clearMemory();
		}

	}


	int _hasCount = 0;
	int _totalCount = 0;

	Dictionary<string, RuneBookData>[] _book = new Dictionary<string, RuneBookData>[RareType.RUNE_BOOK_SLOT_NUM];
	List<RuneBookData> _bookList = new List<RuneBookData>();

	public void refreshUnit()
	{
		_hasCount = 0;
		_totalCount = 0;

		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			if(_book[i] == null) _book[i] = new Dictionary<string, RuneBookData>();
			else _book[i].Clear();
		}

		foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseUnitData)
		{
			if(kv.Value.isBook == false) continue;

			RuneBookData bs = new RuneBookData(kv.Key);
			bs.baseLevel = kv.Value.baseLevel;
			bs.rare = kv.Value.rare;
			_book[bs.rare].Add(kv.Key, bs);
			++_totalCount;
		}
		
		RuneBookData tempRb;
		
		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.unitInventory)
		{
			if(_book[kv.Value.rare].ContainsKey(kv.Value.baseId) == false) continue;
			tempRb = _book[kv.Value.rare][ kv.Value.baseId ];
			++tempRb.nowCount;
			_book[kv.Value.rare][ kv.Value.baseId ] = tempRb;
		}

		if(GameDataManager.instance.playerUnitSlots != null)
		{
			foreach(KeyValuePair<string, PlayerUnitSlot[]> kv in GameDataManager.instance.playerUnitSlots)
			{
				if(GameDataManager.instance.serverHeroData.ContainsKey(kv.Key))
				{
					foreach(PlayerUnitSlot ps in kv.Value)
					{
						if(ps.isOpen == false || ps.unitInfo == null) continue;
						if(_book[ps.unitInfo.rare].ContainsKey(ps.unitInfo.baseId) == false) continue;
						tempRb = _book[ps.unitInfo.rare][ ps.unitInfo.baseId ];
						++tempRb.nowCount;
						_book[ps.unitInfo.rare][ ps.unitInfo.baseId ] = tempRb;
					}
				}
			}
		}


		
		string[] s = new string[2];
		if(GameDataManager.instance.historyUnitRunes != null)
		{
			foreach(KeyValuePair<string, int> kv in GameDataManager.instance.historyUnitRunes)
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
	}



	public void refreshSkill()
	{
		_hasCount = 0;
		_totalCount = 0;
		
		for(int i = 0; i < RareType.RUNE_BOOK_SLOT_NUM; ++i)
		{
			if(_book[i] == null) _book[i] = new Dictionary<string, RuneBookData>();
			else _book[i].Clear();
		}
		
		foreach(KeyValuePair<string, HeroSkillData> kv in GameManager.info.heroBaseSkillData)
		{
			if(kv.Value.isBook == false) continue;
			if(kv.Value.isMonsterSkill) continue;
			
			RuneBookData bs = new RuneBookData(kv.Key);
			bs.baseLevel = kv.Value.baseLevel;

			bs.rare = kv.Value.grade - 1;
			_book[bs.rare].Add(kv.Key, bs);
			++_totalCount;
		}
		
		RuneBookData tempRb;
		
		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.skillInventory)
		{
			if(_book[kv.Value.rare].ContainsKey(kv.Value.baseId) == false) continue;
			tempRb = _book[kv.Value.rare][ kv.Value.baseId ];
			++tempRb.nowCount;
			_book[kv.Value.rare][ kv.Value.baseId ] = tempRb;
		}

		if(GameDataManager.instance.playerSkillSlots != null)
		{
			foreach(KeyValuePair<string, PlayerSkillSlot[]> kv in GameDataManager.instance.playerSkillSlots)
			{
				if( GameDataManager.instance.serverHeroData.ContainsKey( kv.Key ) )
				{
					foreach(PlayerSkillSlot ps in kv.Value)
					{
						if(ps.isOpen == false || ps.infoData == null) continue;
						if(_book[ps.infoData.rare].ContainsKey(ps.infoData.baseId) == false) continue;
						tempRb = _book[ps.infoData.rare][ ps.infoData.baseId ];
						++tempRb.nowCount;
						_book[ps.infoData.rare][ ps.infoData.baseId ] = tempRb;
					}
				}
			}
		}

		
		string[] s = new string[2];

		if(GameDataManager.instance.historySkillRunes != null)
		{
			foreach(KeyValuePair<string, int> kv in GameDataManager.instance.historySkillRunes)
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
	}


	void refreshProcess2()
	{
		_bookList.Clear();

		for(int i = RareType.MAX_RARE; i >= 0; --i)
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
			
			if(_type == Type.SkillBook)
			{
				_bookList.Sort(RuneBookData.skillSort);
			}
			else
			{
				_bookList.Sort(RuneBookData.unitSort);
			}
			
			
			int len = _bookList.Count;
			
			for(int j = 0; j < len; ++j)
			{
				addItemSlot( _bookList[j] );
			}
			
			_bookList.Clear();
			// 그린다...
		}
		
		
		lbCount.text = "[fced52]"+_hasCount + "[-][c9a072]/" + _totalCount + "[-]";
	}











}



public struct RuneBookData
{
	public string baseId;

	public int nowCount;
	public int hasCount;

	public int baseLevel;
	public int rare;

	public RuneBookData(string bId)
	{
		baseId = bId;
		baseLevel = 1;
		rare = 0;
		nowCount = 0;
		hasCount = 0;
	}

	public static int unitSort(RuneBookData x, RuneBookData y)
	{
		int i = GameManager.info.baseUnitData[x.baseId].name.CompareTo(GameManager.info.baseUnitData[y.baseId].name);
		return i;
	}

	public static int skillSort(RuneBookData x, RuneBookData y)
	{
		int i = GameManager.info.heroSkillData[x.baseId].name.CompareTo(GameManager.info.heroSkillData[y.baseId].name);
		return i;
	}

	public static int equipSort(RuneBookData x, RuneBookData y)
	{
		int i = HeroPartsData.sortValueByPartsType(GameManager.info.heroBasePartsData[x.baseId].type).CompareTo(HeroPartsData.sortValueByPartsType(GameManager.info.heroBasePartsData[y.baseId].type));

		if(i == 0) GameManager.info.heroBasePartsData[x.baseId].id.CompareTo(GameManager.info.heroBasePartsData[y.baseId].id);

		return i;
	}

}


