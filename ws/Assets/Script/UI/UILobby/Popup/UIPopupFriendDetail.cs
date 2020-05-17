using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupFriendDetail : UIPopupBase {

	public UIButton btnMainToSub, btnSubToMain;

	public UILabel lbName;
	public UISprite spGradeIcon, spSelectedSlotSelectBorder = null;


	
	public UIButton btnVisit;

	public Transform sampleContainer;
	public Player sample;


	public UIHeroInventorySlot[] partsSlots = new UIHeroInventorySlot[4];
	public UISummonInvenSlot[] summonSlots = new UISummonInvenSlot[5];
	public UISkillInvenSlot[] skillSlots = new UISkillInvenSlot[3];



	protected override void awakeInit ()
	{
		UIEventListener.Get(btnVisit.gameObject).onClick = onClickVisit;
		UIEventListener.Get(btnMainToSub.gameObject).onClick = onClickMainToSub;
		UIEventListener.Get(btnSubToMain.gameObject).onClick = onClickSubToMain;
	}



	public override void show ()
	{
		gameObject.SetActive(true);

		if(spSelectedSlotSelectBorder != null) spSelectedSlotSelectBorder.gameObject.SetActive(false);
	}

	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		tooltip.hide();
	}


	void onClickVisit(GameObject go)
	{
		if(_subHero != null)
		{
			GameManager.me.uiManager.uiVisitingLobby.show(  _name, _currentAct, _playerMainData, _playerSubData, _selectUnitRunes, _selectSkillRunes, _selectSubUnitRunes, _selectSubSkillRunes, _showPhoto, _photoUrl);
		}
		else
		{
			GameManager.me.uiManager.uiVisitingLobby.show(_name, _currentAct, _playerMainData, null, _selectUnitRunes, _selectSkillRunes, null, null, _showPhoto, _photoUrl);
		}
	}


	public UIIconTooltip tooltip;


	public void showSlotTooltip(UISprite selectBorder, string description, Vector3 pos)
	{
		if(spSelectedSlotSelectBorder != null) spSelectedSlotSelectBorder.gameObject.SetActive(false);

		spSelectedSlotSelectBorder = selectBorder;
//		selectBorder.gameObject.SetActive(true);
//		selectBorder.enabled = true;

		if(description != null)
		{
//			lbSlotTooltip.gameObject.SetActive(true);
//			lbSlotTooltip.text = description;

			pos.y += 100.0f;
			pos.x -= 214.0f;

//			lbSlotTooltip.transform.localPosition = pos;

			if(pos.x > -23)
			{
				pos.x = -23;
			}

			tooltip.start(description, pos.x, pos.y, 2, true, true, 80.0f);
		}
		else
		{
			tooltip.hide();
			//lbSlotTooltip.gameObject.SetActive(false);
		}

	}


	void setLeague(int league)
	{
		spGradeIcon.enabled = true;

		switch(league)
		{
		case WSDefine.LEAGUE_BRONZE:
			spGradeIcon.spriteName = "icn_levelmedal_bronz";
			break;
		case WSDefine.LEAGUE_SILVER:
			spGradeIcon.spriteName = "icn_levelmedal_silver";
			break;
		case WSDefine.LEAGUE_GOLD:
			spGradeIcon.spriteName = "icn_levelmedal_gold";
			break;
		case WSDefine.LEAGUE_MASTER:
			spGradeIcon.spriteName = "icn_levelmedal_master";
			break;
		case WSDefine.LEAGUE_PLATINUM:
			spGradeIcon.spriteName = "icn_levelmedal_platinum";
			break;
		case WSDefine.LEAGUE_LEGEND:
			spGradeIcon.spriteName = "icn_levelmedal_legend";
			break;
		default:
			spGradeIcon.enabled = false;
			break;
		}
	}


	public void setData(ToC_ENEMY_DATA p, P_Champion c)
	{
		show ();
		setData(p.eHero, p.eSubHero, p.eSelectUnitRunes, p.eSelectSkillRunes, p.eSubSelectUnitRunes, p.eSubSelectSkillRunes);
		draw(p.act, c.nickname, GameDataManager.instance.champLeague, c.showPhoto, c.imageUrl);
		btnVisit.gameObject.SetActive(true);
	}
	
	
	
	public void setData(ToC_GET_HELL_USER_INFO p, P_UserRank userData)
	{
		show ();
		
		setData(p.hero, p.subHero, p.selectUnitRunes, p.selectSkillRunes, p.selectSubUnitRunes, p.selectSubSkillRunes);
		draw(p.act, UIPopupHellRankingListSlotPanel.checkingHellUserName, p.league, WSDefine.YES , userData.imageUrl);
		btnVisit.gameObject.SetActive(true);
	}
	
	
	public void setData(ToC_GET_OTHER_DATA p)
	{
		#if UNITY_EDITOR
		p.act = 1;
		#endif
		
		show ();
		setData(p.hero, p.subHero, p.selectUnitRunes, p.selectSkillRunes, p.selectSubUnitRunes, p.selectSubSkillRunes);
		draw(p.act, p.nickname, p.league, WSDefine.YES, p.imageUrl);
		btnVisit.gameObject.SetActive(true);
	}
	
	




	void setData(P_Hero mainHero, P_Hero subHero, 
	             Dictionary<string, string> mainUnit, Dictionary<string, string> mainSkill, 
	             Dictionary<string, string> subUnit, Dictionary<string, string> subSkill)
	{
		_mainHero = mainHero;
		_subHero = subHero;

		_leoUnits = null;
		_leoSkills = null;
		_chloeUnits = null;
		_chloeSkills = null;
		_kileyUnits = null;
		_kileySkills = null;

		switch(mainHero.name)
		{
		case Character.LEO:
			_leoUnits = mainUnit;
			_leoSkills = mainSkill;
			break;
		case Character.CHLOE:
			_chloeUnits = mainUnit;
			_chloeSkills = mainSkill;
			break;
		case Character.KILEY:
			_kileyUnits = mainUnit;
			_kileySkills = mainSkill;
			break;
		}
		
		
		if(subHero != null)
		{
			switch(subHero.name)
			{
			case Character.LEO:
				_leoUnits = subUnit;
				_leoSkills = subSkill;
				break;
			case Character.CHLOE:
				_chloeUnits = subUnit;
				_chloeSkills = subSkill;
				break;
			case Character.KILEY:
				_kileyUnits = subUnit;
				_kileySkills = subSkill;
				break;
			}
		}
	}





	void setData(P_Hero mainHero, P_Hero subHero, 
	             Dictionary<string, string> leoUnit, Dictionary<string, string> leoSkill, 
	             Dictionary<string, string> kileyUnit, Dictionary<string, string> kileySkill,
	             Dictionary<string, string> chloeUnit, Dictionary<string, string> chloeSkill)
	{
		_mainHero = mainHero;
		_subHero = subHero;

		_leoUnits = leoUnit;
		_leoSkills = leoSkill;
		_kileyUnits = kileyUnit;
		_kileySkills = kileySkill;
		_chloeUnits = chloeUnit;
		_chloeSkills = chloeSkill;
	}



	private Quaternion _q;

	private string _name;

	private GamePlayerData _playerMainData;
	private GamePlayerData _playerSubData;

	private GamePlayerData _playerData
	{
		get
		{
			if(_isMain) return _playerMainData;
			else return _playerSubData;
		}
	}


	private GameIDData[] _selectUnitRunes = new GameIDData[5];
	private GameIDData[] _selectSkillRunes = new GameIDData[3];

	private GameIDData[] _selectSubUnitRunes = new GameIDData[5];
	private GameIDData[] _selectSubSkillRunes = new GameIDData[3];

	private int _currentAct = 1;
	private string _photoUrl = "";
	private int _showPhoto = WSDefine.YES;

	private bool _isMain = true;

	private Dictionary<string, string> _leoUnits = null;
	private Dictionary<string, string> _leoSkills = null;

	private Dictionary<string, string> _kileyUnits = null;
	private Dictionary<string, string> _kileySkills = null;

	private Dictionary<string, string> _chloeUnits = null;
	private Dictionary<string, string> _chloeSkills = null;

	private P_Hero _mainHero = null;
	private P_Hero _subHero = null;


	private Dictionary<string, string> _emptyUnits = new Dictionary<string, string>();
	private Dictionary<string, string> _emptySkills = new Dictionary<string, string>();

	private Dictionary<string, string> selectUnitRunes
	{
		get
		{
			switch(_mainHero.name)
			{
			case Character.LEO:
				if(_leoUnits == null) return _emptyUnits;
				else return _leoUnits;
			case Character.KILEY:
				if(_kileyUnits == null) return _emptyUnits;
				else return _kileyUnits;
			case Character.CHLOE:
				if(_chloeUnits == null) return _emptyUnits;
				else return _chloeUnits;
			}

			return _emptyUnits;
		}
	}

	private Dictionary<string, string> selectSkillRunes
	{
		get
		{
			switch(_mainHero.name)
			{
			case Character.LEO:
				if(_leoSkills == null) return _emptySkills;
				return _leoSkills;
			case Character.KILEY:
				if(_kileySkills == null) return _emptySkills;
				return _kileySkills;
			case Character.CHLOE:
				if(_chloeSkills == null) return _emptySkills;
				return _chloeSkills;
			}
			return _emptySkills;
		}
	}




	private Dictionary<string, string> selectSubUnitRunes
	{
		get
		{
			if(_subHero != null)
			{
				switch(_subHero.name)
				{
				case Character.LEO:
					if(_leoUnits == null) return _emptyUnits;
					else return _leoUnits;
				case Character.KILEY:
					if(_kileyUnits == null) return _emptyUnits;
					else return _kileyUnits;
				case Character.CHLOE:
					if(_chloeUnits == null) return _emptyUnits;
					else return _chloeUnits;
				}
			}

			return _emptyUnits;
		}
	}
	
	private Dictionary<string, string> selectSubSkillRunes
	{
		get
		{
			if(_subHero != null)
			{
				switch(_subHero.name)
				{
				case Character.LEO:
					if(_leoSkills == null) return _emptySkills;
					return _leoSkills;
				case Character.KILEY:
					if(_kileySkills == null) return _emptySkills;
					return _kileySkills;
				case Character.CHLOE:
					if(_chloeSkills == null) return _emptySkills;
					return _chloeSkills;
				}
			}

			return _emptySkills;
		}
	}









	private P_Hero selectHero
	{
		get
		{
			if(_isMain == false)
			{
				return _subHero;
			}

			return _mainHero;
		}
	}

	void draw(int act, string name, int league, int showPhoto = WSDefine.TRUE, string photoUrl = "")
	{
		_isMain = true;
		_currentAct = act;
		_name = name;
		lbName.text = name;
		_showPhoto = showPhoto;
		_photoUrl = photoUrl;
		setLeague(league);

		_playerMainData = new GamePlayerData(_mainHero.name);
		DebugManager.instance.setPlayerData(_playerMainData, false, _mainHero.name, 
		                                    _mainHero.selEqts[HeroParts.HEAD],
		                                    _mainHero.selEqts[HeroParts.BODY],
		                                    _mainHero.selEqts[HeroParts.WEAPON],
		                                    _mainHero.selEqts[HeroParts.VEHICLE]);


		int i = 0;

		if(selectUnitRunes == null)
		{
			_selectUnitRunes[0] = null;
			_selectUnitRunes[1] = null;
			_selectUnitRunes[2] = null;
			_selectUnitRunes[3] = null;
			_selectUnitRunes[4] = null;
		}
		else
		{
			foreach(KeyValuePair<string, string> kv in selectUnitRunes)
			{
				if(string.IsNullOrEmpty(kv.Value))
				{
					_selectUnitRunes[i] = null;
				}
				else
				{
					GameIDData gd = new GameIDData();
					gd.parse(kv.Value, GameIDData.Type.Unit);
					_selectUnitRunes[i] = gd;
				}
				
				++i;
			}
		}

		i = 0;


		if(selectSkillRunes == null)
		{
			_selectSkillRunes[0] = null;
			_selectSkillRunes[1] = null;
			_selectSkillRunes[2] = null;
		}
		else
		{
			foreach(KeyValuePair<string, string> kv in selectSkillRunes)
			{
				if(string.IsNullOrEmpty(kv.Value))
				{
					_selectSkillRunes[i] = null;
				}
				else
				{
					GameIDData gd = new GameIDData();
					gd.parse(kv.Value, GameIDData.Type.Skill);
					_selectSkillRunes[i] = gd;
				}
				
				++i;
			}

		}





		if(_subHero != null)
		{
			_playerSubData = new GamePlayerData(_subHero.name);
			DebugManager.instance.setPlayerData(_playerSubData, false, _subHero.name, 
			                                    _subHero.selEqts[HeroParts.HEAD],
			                                    _subHero.selEqts[HeroParts.BODY],
			                                    _subHero.selEqts[HeroParts.WEAPON],
			                                    _subHero.selEqts[HeroParts.VEHICLE]);


			i = 0;
			foreach(KeyValuePair<string, string> kv in selectSubUnitRunes)
			{
				if(string.IsNullOrEmpty(kv.Value))
				{
					_selectSubUnitRunes[i] = null;
				}
				else
				{
					if(_selectSubUnitRunes[i] == null) _selectSubUnitRunes[i] = new GameIDData();
					_selectSubUnitRunes[i].parse(kv.Value, GameIDData.Type.Unit);
				}
				
				++i;
			}
			
			i = 0;
			foreach(KeyValuePair<string, string> kv in selectSubSkillRunes)
			{
				if(string.IsNullOrEmpty(kv.Value))
				{
					_selectSubSkillRunes[i] = null;
				}
				else
				{
					if(_selectSubSkillRunes[i] == null) _selectSubSkillRunes[i] = new GameIDData();
					_selectSubSkillRunes[i].parse(kv.Value, GameIDData.Type.Skill);
				}
				
				++i;
			}
		}

		draw();
	}



	void draw()
	{
		if(_subHero == null)
		{
			btnMainToSub.gameObject.SetActive(false);
			btnSubToMain.gameObject.SetActive(false);
		}
		else
		{
			if(_isMain)
			{
				btnSubToMain.gameObject.SetActive(false);
				btnMainToSub.gameObject.SetActive(true);
			}
			else
			{
				btnMainToSub.gameObject.SetActive(false);
				btnSubToMain.gameObject.SetActive(true);
			}
		}

		partsSlots[0].setStringData(selectHero.selEqts[HeroParts.HEAD], false);
		partsSlots[1].setStringData(selectHero.selEqts[HeroParts.BODY], false);
		partsSlots[2].setStringData(selectHero.selEqts[HeroParts.WEAPON], false);
		partsSlots[3].setStringData(selectHero.selEqts[HeroParts.VEHICLE], false);
		
		int i = 0;


		foreach(KeyValuePair<string, string> kv in selectUnitRunes)
		{
			if(string.IsNullOrEmpty(kv.Value))
			{
				summonSlots[i].setData( null );
			}
			else
			{
				if(_isMain) summonSlots[i].setData( _selectUnitRunes[i] );
				else summonSlots[i].setData( _selectSubUnitRunes[i] );
			}
			
			++i;
		}
		
		i = 0;
		foreach(KeyValuePair<string, string> kv in selectSkillRunes)
		{
			if(string.IsNullOrEmpty(kv.Value))
			{
				skillSlots[i].setData( null );
			}
			else
			{
				if(_isMain) skillSlots[i].setData( _selectSkillRunes[i] );
				else skillSlots[i].setData( _selectSubSkillRunes[i] );
			}
			
			++i;
		}

		if(sample != null)
		{
			GameManager.me.characterManager.cleanMonster(sample);
			sample = null;
		}
		
		sample = (Player)GameManager.me.characterManager.getMonster(true,true,_playerData.id,false);
		sample.init(_playerData,true,false);
		sample.pet = (Pet)GameManager.me.characterManager.getMonster(true,false,_playerData.partsVehicle.parts.resource.ToUpper(),false);
		sample.pet.init(sample);
		
		sample.container.SetActive(true);
		sample.animation["idle"].speed = 1.0f;
		sample.animation.Play("idle");
		
		sample.setParent( sampleContainer );
		_v = sample.cTransform.localPosition;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		sample.cTransform.localPosition = _v;
		
		_q = sampleContainer.transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		sampleContainer.transform.localRotation = _q;
		
		_q = sample.cTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 190.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		sample.cTransform.localRotation = _q;
		
		_v.x = 0; _v.y =0; _v.z = 0;
		sample.tf.localPosition = _v;
		
		_q = sample.tf.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		sample.tf.localRotation = _q;
		
		_v = sample.cTransform.position;
		sample.shadow.transform.position = _v;
		
		
		for(i = 0; i < 4; ++i)
		{
			partsSlots[i].slotType = BaseSlot.InventorySlotType.FriendDetailSlot;
		}
		
		for(i = 0; i < 5; ++i)
		{
			summonSlots[i].slotType = BaseSlot.InventorySlotType.FriendDetailSlot;
		}
		
		for(i = 0; i < 3; ++i)
		{
			skillSlots[i].slotType = BaseSlot.InventorySlotType.FriendDetailSlot;
		}

	}


	void onClickMainToSub(GameObject go)
	{
		_isMain = false;
		draw();
	}
	
	void onClickSubToMain(GameObject go)
	{
		_isMain = true;
		draw();
	}




	public void setData(ToC_GET_FRIEND_DETAIL p, string friendId)
	{
		show ();
		
		int i = 0;
		
		Util.stringBuilder.Length = 0;
		
		string imgUrl = "";
		
		if(epi.GAME_DATA.appFriendDic.ContainsKey(friendId))
		{
			Util.stringBuilder.Append(epi.GAME_DATA.appFriendDic[friendId].f_Nick);
			imgUrl = epi.GAME_DATA.appFriendDic[friendId].image_url;
		}
		else if(GameDataManager.instance.friendDatas.ContainsKey(friendId))
		{
			Util.stringBuilder.Append(GameDataManager.instance.friendDatas[friendId].nickname);
			
			if(epi.GAME_DATA.friendDic.ContainsKey(friendId))
			{
				imgUrl = epi.GAME_DATA.friendDic[friendId].image_url;
			}
		}
		
		
		if(friendId != p.nickname )
		{
			Util.stringBuilder.Append(" (");
			Util.stringBuilder.Append(p.nickname);
			Util.stringBuilder.Append(")");
		}
		
		int league = -1;
		
		if(GameDataManager.instance.friendDatas.ContainsKey(friendId))
		{
			league = GameDataManager.instance.friendDatas[friendId].league;
		}
		
		setData(p.selectHero, p.selectSubHero, p.selectUnitRunes, p.selectSkillRunes, p.selectSubUnitRunes, p.selectSubSkillRunes);
		
		draw(p.act, Util.stringBuilder.ToString(), league, WSDefine.YES, imgUrl);
		
		Util.stringBuilder.Length = 0;
		
		btnVisit.gameObject.SetActive(true);
		
	}
	
	
	public void myData()
	{
		show ();
		
		_mainHero = GameDataManager.instance.serverHeroData[GameDataManager.instance.selectHeroId];
		
		_leoUnits = GameDataManager.instance.selectUnitRunes[Character.LEO];
		_leoSkills = GameDataManager.instance.selectSkillRunes[Character.LEO];
		
		if(GameDataManager.instance.selectUnitRunes.TryGetValue(Character.KILEY, out _kileyUnits) == false)
		{
			_kileyUnits = null;
		}
		
		if(GameDataManager.instance.selectSkillRunes.TryGetValue(Character.KILEY, out _kileySkills) == false)
		{
			_kileySkills = null;
		}
		
		if(GameDataManager.instance.selectUnitRunes.TryGetValue(Character.CHLOE, out _chloeUnits) == false)
		{
			_chloeUnits = null;
		}
		
		if(GameDataManager.instance.selectSkillRunes.TryGetValue(Character.CHLOE, out _chloeSkills) == false)
		{
			_chloeSkills = null;
		}
		
		if(GameDataManager.instance.selectSubHeroId != null)
		{
			if(GameDataManager.instance.serverHeroData.TryGetValue(GameDataManager.instance.selectSubHeroId, out _subHero) == false)
			{
				_subHero = null;
			}
		}
		else
		{
			_subHero = null;
		}



		draw(GameDataManager.instance.maxAct, 
		     GameDataManager.instance.name, 
		     GameDataManager.instance.champLeague);
		
		btnVisit.gameObject.SetActive(false);
	}




}
