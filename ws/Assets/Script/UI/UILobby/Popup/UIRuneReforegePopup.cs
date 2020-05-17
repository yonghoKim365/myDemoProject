using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRuneReforegePopup : UIPopupBase {

	public int[] currentEffectValue = new int[4];
	public int currentLevel = 0;
	private bool _showDiff = false;

	private int _step = 0;
	public int step
	{
		set
		{
			if(_step == 1 && value == 2)
			{
				_showDiff = true;
			}

			_step = value;
		}
		get
		{
			return _step;
		}

	}


	public UIButton btnReforge;

	public UILabel lbReforgePrice, lbNeedMaterial;

	public UIRuneReforegePopupATTRSlot[] attSlots;

	public UIChallengeItemSlot slotBefore, slotAfter;
	public UILabel lbSlotBefore, lbSlotAfter;

	public UIScrollView materialScrollView;

	public UIChallengeItemSlot prefab;

	public Transform tfMaterialContainer;

	public static bool fromTabSlot = false;

	private GameIDData _data = new GameIDData();
	private GameIDData _prevData = new GameIDData();

	private List<UIChallengeItemSlot> _materialSlots = new List<UIChallengeItemSlot>();
	private Stack<UIChallengeItemSlot> _materialSlotPool = new Stack<UIChallengeItemSlot>();

	public static string currentIngredientId = null;

	private List<GameIDData> _materialData = new List<GameIDData>();

	public UILabel lbMaterialRemoveNotice;


	protected override void awakeInit ()
	{
		UIEventListener.Get(btnReforge.gameObject).onClick = onClickReforge;
	}

	public override void hide (bool isInit)
	{
		base.hide (isInit);

		if(isInit == false)
		{
			clearSlots();
		}

		_showDiff = false;
		_step = 0;
	}

	void onClickReforge(GameObject go)
	{
		if(GameDataManager.instance.gold < GameDataManager.instance.reforgePrices[WSDefine.GOLD])
		{
			GameManager.me.uiManager.popupGoldForRuby.show (GameDataManager.instance.evolvePrices["GOLD"], onConfirmReforge);
			return;
		}

		onConfirmReforge();
	}

	void onConfirmReforge()
	{
		currentLevel = _data.totalPLevel;
		EpiServer.instance.sendReforgeRune(_data.serverId, currentIngredientId, (fromTabSlot?WSDefine.YES:WSDefine.NO), _data.type);

//		GameManager.me.uiManager.popupReforege.hide(false);
//		RuneStudioMain.instance.playTranscendResult(_data.serverId + "03120825", _data.serverId, _data.type);

	}


	private string _prevReforgeId = null;

	public void show(string serverId, bool didLoad, string prevReforgeId = null)
	{
		base.show();

		step = 2;

		fromTabSlot = didLoad;

		_prevReforgeId = prevReforgeId;

		_data.parse(serverId);
		_prevData.parse(prevReforgeId);

		materialScrollView.transform.localPosition = Vector3.zero;
		materialScrollView.panel.clipOffset = Vector2.zero;
		materialScrollView.ResetPosition();

		_materialData.Clear();

		draw();

		refreshInfo(true);

		_step = 0;

		switch(_data.type)
		{
		case GameIDData.Type.Unit:
			lbMaterialRemoveNotice.text = Util.getUIText("REFOREGE_UNIT");
			break;			
		case GameIDData.Type.Skill:
			lbMaterialRemoveNotice.text = Util.getUIText("REFOREGE_SKILL");
			break;
		case GameIDData.Type.Equip:
			lbMaterialRemoveNotice.text = Util.getUIText("REFOREGE_EQUIP");
			break;
		}



	}

//		1> 99레벨 미만 SS등급일때, 상세정보 UI에서 [제련] 버튼 표시									

//		3> 조건에 맞는 재료아이템들을 모두 표시 후, 유저가 직접 변경 가능									
//		- 디펄트 선택 재료는, 레어등급 낮은 순 > 제련레벨 낮은 순 > 강화레벨 낮은순									
//		4> 재료가 없으면, 제련 버튼 비활성 & 재료 선택 UI에 메시지 표시									
//		- 필요한 재료룬 이름 알려줌									
//		5> 골드 부족시, 골드 부족 & 루비 사용 메시지 팝업									
//		6> 제련후, 상승한 제련레벨 및 상승 속성 수치 표시									
//		7> 재료가 남아있다면 계속 제련 가능, 									
//		- 재료 없으면 재료 없음 메시지 표시 & 제련 버튼 비활성									



	void refreshInfo(bool setStat = false)
	{
		slotBefore.setReforgeData(_data, false);
		slotAfter.setReforgeData(_data, false);
		slotAfter.useButton = false;
		slotBefore.useButton = false;

		slotBefore.lbTranscendLevel.enabled = false;
		slotAfter.lbTranscendLevel.enabled = false;

		if(_showDiff)
		{
			lbSlotBefore.text = _data.totalPLevel + " [ffe400](↑" +  (_data.totalPLevel - currentLevel )  + ")[-]";
		}
		else
		{
			lbSlotBefore.text = _data.totalPLevel.ToString();
		}

		int minLevel = _data.totalPLevel;
		int maxLevel = _data.totalPLevel;
		int plusValue = minLevel;

		if(_prevSlot != null)
		{
			int[] logic = GameDataManager.instance.reforgeLogic;

			if(_prevSlot.infoData.rare >= RareType.SS)
			{
				minLevel = minLevel + logic[1]; // 5
			}
			else
			{
				minLevel = minLevel + logic[0]; // 1
			}

			maxLevel = minLevel;

			// 강화 레벨 보너스.

			int pl = (_prevSlot.infoData.reinforceLevel.Get());



			if(pl >= logic[12] && pl <= logic[13])
			{
				maxLevel += logic[14];
			}
			if(pl >= logic[9] && pl <= logic[10])
			{
				maxLevel += logic[11];
			}
			if(pl >= logic[6] && pl <= logic[7])
			{
				maxLevel += logic[8];
			}
			if(pl >= logic[3] && pl <= logic[4])
			{
				maxLevel += logic[5];
			}

			// 제련 레벨 보너스.
			if(_prevSlot.infoData.totalPLevel > 0)
			{
				maxLevel += Mathf.FloorToInt((float)_prevSlot.infoData.totalPLevel / (float)(logic[2]));
			}
		}

		if(maxLevel > GameIDData.MAX_P_LEVEL) maxLevel = GameIDData.MAX_P_LEVEL;
		if(minLevel > GameIDData.MAX_P_LEVEL) minLevel = GameIDData.MAX_P_LEVEL;

		if(maxLevel > minLevel)
		{
			lbSlotAfter.text = minLevel + "~" + maxLevel;
		}
		else
		{
			//if(minLevel == 0) lbSlotAfter.text = "-";
			//else 
				lbSlotAfter.text = minLevel.ToString();
		}

		btnReforge.isEnabled = (_materialData.Count > 0);
		//		2> 제련 후 '최소 제련레벨 ~ 최대 제련레벨' 표시									


		if(setStat)
		{
			TranscendData td = _data.transcendData;
			
			for(int i = 0; i < 4; ++i)
			{
				attSlots[i].lbName.text = td.description[i];
				
				
				int newValue = td.getApplyRateValue(_data.transcendLevel[i],  i);
				
				if(_showDiff)
				{
					int diff = newValue - currentEffectValue[i];
					string resultType = td.getApplyRateTypeString(i);
					
					string result = td.getApplyRateValueString(_data.transcendLevel[i],  i);
					
					if(diff > 0)
					{
						result += " [ffe400](↑" + diff  + resultType +")[-]";
					}
					else if(diff < 0)
					{
						result += " [be1010](↓" + diff  + resultType +")[-]";
					}
					
					attSlots[i].lbValue.text = result;
					
				}
				else
				{
					attSlots[i].lbValue.text = td.getApplyRateValueString(_data.transcendLevel[i],  i);
				}
				
				currentEffectValue[i] = newValue;
			}

		}


		lbReforgePrice.text = Util.GetCommaScore( GameDataManager.instance.reforgePrices[WSDefine.GOLD] );
	}




	void draw()
	{
		collectMaterial();

		foreach(GameIDData gd in _materialData)
		{
			addSlot(gd);
		}

		float startOffset = 0;
		float slotOffset = 120;

		switch(_materialSlots.Count)
		{
		case 1:
			startOffset = 318;
			break;
		case 2:
			startOffset = 182;
			slotOffset = 448 - 182;
			break;
		case 3:
			startOffset = 120;
			slotOffset = 318-120;
			break;
		case 4:
			startOffset = 23;
			slotOffset = 221 - 23;
			break;
		}

		int matCount = _materialSlots.Count;

		for(int i = 0; i < matCount; ++i)
		{
			_v.x = startOffset + i * slotOffset;
			_v.y = 0; _v.z = 0;
			_materialSlots[i].transform.localPosition = _v;
		}

		// 자동 선택.
		if(matCount > 0)
		{
			onClickSlot(_materialSlots[0]);
			lbNeedMaterial.enabled = false;
		}
		else
		{
			lbNeedMaterial.enabled = true;


			string sRune = null;
			string ssRune = null;

			int rare;

			switch(_data.type)
			{
			case GameIDData.Type.Equip:



				foreach(KeyValuePair<string, HeroPartsData> kv in GameManager.info.heroPartsDic)
				{
					if( kv.Value.baseIdWithoutRare == _data.baseIdWithoutRare )
					{
						rare = kv.Value.grade.Get() - 1;

						if(sRune == null && rare == RareType.S)
						{
							sRune = kv.Value.name;
						}
						else if(ssRune == null && rare == RareType.SS)
						{
							ssRune = kv.Value.name;
						}
					}

					if(sRune != null && ssRune != null) break;
				}

				break;
			case GameIDData.Type.Unit:


				foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseUnitData)
				{
					if( kv.Value.baseIdWithoutRare == _data.baseIdWithoutRare )
					{
						rare = kv.Value.rare;
						
						if(sRune == null && rare == RareType.S)
						{
							sRune = kv.Value.name;
						}
						else if(ssRune == null && rare == RareType.SS)
						{
							ssRune = kv.Value.name;
						}
					}
					
					if(sRune != null && ssRune != null) break;
				}


				break;
			case GameIDData.Type.Skill:


				foreach(KeyValuePair<string, HeroSkillData> kv in GameManager.info.heroBaseSkillData)
				{
					if( kv.Value.baseIdWithoutRare == _data.baseIdWithoutRare )
					{
						rare = kv.Value.rare;
						
						if(sRune == null && rare == RareType.S)
						{
							sRune = kv.Value.name;
						}
						else if(ssRune == null && rare == RareType.SS)
						{
							ssRune = kv.Value.name;
						}
					}
					
					if(sRune != null && ssRune != null) break;
				}

				break;
			}

			if(sRune != null && ssRune != null)
			{
				lbNeedMaterial.text = Util.getUIText("NEED_REFOREGE_MATERIAL_BOTH", sRune, ssRune);
			}
			else if(sRune != null)
			{
				lbNeedMaterial.text = Util.getUIText("NEED_REFOREGE_MATERIAL_S", sRune);
			}
			else if(ssRune != null)
			{
				lbNeedMaterial.text = Util.getUIText("NEED_REFOREGE_MATERIAL_SS", ssRune);
			}
		}
	}



//	* 제련 재료 룬 : (등급부분을 제외하고) 원본 룬과 동일한 베이스 아이디를 가진 SS등급 or S등급												
//	예> UN613 의 제련 재료 : UN613, UN513 // SK_6110 : SK6110, SK5110 // LEO_BD6_22 : LEO_BD6_22, LEO_BD5_22												

	void collectMaterial()
	{
		List<GameIDData> targetList;

		switch(_data.type)
		{
		case GameIDData.Type.Unit:
			collectProcess(GameDataManager.instance.unitInventoryList);
			break;
			
		case GameIDData.Type.Skill:
			collectProcess(GameDataManager.instance.skillInventoryList);
			break;
			
		case GameIDData.Type.Equip:
			collectProcess(GameDataManager.instance.partsInventoryList);
			break;
		}
	}


	void collectProcess(List<GameIDData> targetList)
	{
		_materialData.Clear();

		bool _skip = fromTabSlot;

		foreach(GameIDData gd in targetList)
		{
			if(gd.rare < RareType.S) continue;
			
			if(gd.baseIdWithoutRare == _data.baseIdWithoutRare)
			{
				if(_skip == false && gd.serverId == _data.serverId)
				{
					_skip = true;
					continue;
				}
				_materialData.Add(gd);
			}
		}

		_materialData.Sort(GameIDData.sortReforgeMaterial);
	}


	void clearSlots()
	{
		if(_prevSlot != null)
		{
			_prevSlot.select = false;
			_prevSlot = null;
		}

		foreach(UIChallengeItemSlot s in _materialSlots)
		{
			_materialSlotPool.Push(s);
			s.gameObject.SetActive(false);
		}
		_materialSlots.Clear();
	}


	void addSlot(GameIDData gd)
	{
		UIChallengeItemSlot s;
		
		if(_materialSlotPool.Count > 0 )
		{
			s = _materialSlotPool.Pop();
		}
		else
		{
			s = Instantiate(prefab) as UIChallengeItemSlot;
		}
		
		_materialSlots.Add(s);
		
		s.gameObject.SetActive(true);
		
		s.setReforgeData(gd, true);
		s.useButton = true;
		
		s.transform.parent = tfMaterialContainer;
	}

	private UIChallengeItemSlot _prevSlot = null;

	public void onClickSlot(UIChallengeItemSlot selectSlot)
	{
		if(_prevSlot != null) _prevSlot.select = false;

		currentIngredientId = selectSlot.inputItemId;
		selectSlot.select = true;
		_prevSlot = selectSlot;

		if(_prevSlot != null)
		{
			refreshInfo();
		}
	}






}
