using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Sw;


public partial class NetData{

	//플레이어용
	public Dictionary<AbilityType, float> CalcPlayerStats(uint charIdx=0, uint iLevel=0, List<_ItemData> equipList=null, _CostumeData equipCostume=null)
	{
		if (charIdx <= 0)
		{
			charIdx = GetUserInfo()._userCharIndex;
			iLevel = GetUserInfo()._Level;
			equipList = GetUserInfo().GetEquipItemList();
			equipCostume = GetUserInfo().GetEquipCostume();
		}

		bool bDebugStat = false;
		
		
		Dictionary<AbilityType, float> playerStats = new Dictionary<AbilityType, float>();
		
		Dictionary<AbilityType, float> itemStats = new Dictionary<AbilityType, float>();
		
		//기본 스탯 넣어주자
		//uint iLevel = GetUserInfo()._Level;
		
		//일단 기본캐릭만
		Character.CharacterInfo charInfo = _LowDataMgr.instance.GetCharcterData(charIdx);//GetUserInfo().GetCharIdx()
		
		//float t1 = (float)(iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1));
		//float t2 = (float)(t1 * charInfo.LevelUpHp);
		//float t3 = (float)(charInfo.BaseHp + t2);
		float powValue = Mathf.Pow (_LowDataMgr.instance.GetFormula (FormulaType.LEVELUP_CALIBRATION, 1), iLevel);

		playerStats.Add(AbilityType.HP, 					(float)(charInfo.BaseHp  + 			(iLevel * powValue * charInfo.LevelUpHp)) 		* 0.1f);
		playerStats.Add(AbilityType.DAMAGE, 				(float)(charInfo.BaseAtk + 			(iLevel * powValue * charInfo.LevelUpAtk)) 		* 0.1f);
		playerStats.Add(AbilityType.HIT_RATE, 				(float)(charInfo.BaseHit + 			(iLevel * powValue * charInfo.LevelHitRate)) 	* 0.1f);
		playerStats.Add(AbilityType.DODGE_RATE, 			(float)(charInfo.BaseAvoid + 		(iLevel * powValue * charInfo.LevelAvoidRate)) 	* 0.1f);
		playerStats.Add(AbilityType.CRITICAL_RATE, 			(float)(charInfo.BaseCriticalRate 	* 0.1f));
		playerStats.Add(AbilityType.CRITICAL_RES, 			(float)(charInfo.BaseCriticalResist * 0.1f));
		playerStats.Add(AbilityType.CRITICAL_DAMAGE,		(float)(charInfo.BaseCriticalDamage * 0.1f));
		playerStats.Add(AbilityType.DRAIN_HP, 				(float)(charInfo.BaseLifeSteal 		* 0.1f));
		playerStats.Add(AbilityType.DEFENCE_IGNORE, 		(float)(charInfo.BaseIgnoreAtk 		* 0.1f));
		playerStats.Add(AbilityType.DAMAGE_DECREASE,		(float)(charInfo.BaseDamageDown 	* 0.1f));
		playerStats.Add(AbilityType.DAMAGE_DECREASE_RATE, 	(float)(charInfo.BaseDamageDownRate * 0.1f));
		playerStats.Add(AbilityType.COOLTIME, 0f);
		playerStats.Add(AbilityType.ATTACK_SPEED, 			_LowDataMgr.instance.GetAttackSpeed(1f));
		
		playerStats.Add(AbilityType.SUPERARMOR, 			(float)(charInfo.BaseSuperArmor + (iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1) * charInfo.LevelUpSuperArmor)));
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY, 	(float)(charInfo.SuperArmorRecovery));
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, (float)(charInfo.SuperArmorRecoveryRate));
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, (float)(charInfo.SuperArmorRecoveryTime));

        playerStats.Add(AbilityType.WEIGHT,                 charInfo.Weight);
		
		
		playerStats.Add(AbilityType.EXP_UP, 0f);
		playerStats.Add(AbilityType.ALLSTAT_RATE, 0f);
		
		#if UNITY_EDITOR
		if (bDebugStat){
		Debug.Log (" CalcPlayerStats , lv:"+iLevel+" =======================================");
		Debug.Log (" === player Stats ===");
		var playerStatsEnum = playerStats.GetEnumerator();
		while (playerStatsEnum.MoveNext())
		{
			Debug.Log(" ability:"+playerStatsEnum.Current.Key+", v:"+playerStatsEnum.Current.Value);
		}
		
		Debug.Log (" === equipment Stats ===");
		}
		#endif
		
		
		for (int i = 0; i < equipList.Count; i++)// (int)ePartType.PART_MAX
		{
			_ItemData equipItem = equipList[i];
			if (equipItem != null)
			{
				List<ItemAbilityData> statList = equipItem.StatList;
				int count = statList.Count;

//				if (bDebugStat){
//					Debug.Log (" = equipItem, id:"+equipItem._equipitemDataIndex+", _enchant:"+equipItem._enchant+", num of stat:"+count);
//				}
				for (int j=0; j < count; j++)
				{
					if (itemStats.ContainsKey(statList[j].Ability)==false){
						itemStats.Add(statList[j].Ability, 0f);
					}

					itemStats[statList[j].Ability] += _LowDataMgr.instance.GetItemAbilityValue(statList[j].Value, equipItem._enchant);

					if (bDebugStat){
						Debug.Log (" = equipItem, id:"+equipItem._equipitemDataIndex+", _enchant:"+equipItem._enchant+", ability:"+statList[j].Ability+",v:"+itemStats[statList[j].Ability]);
					}
				}
			}
		}
		

		// 일단 주석처리. 코스튬은 추후 다시 붙일 예정. 20171103 kyh
		/*
		//Debug.Log (" === jewel Stats     ===");
		//현재 가지고 들어가는 코스튬에 박힌 보석정보 - 소켓이 4개
		//_CostumeData equipCostume = GetUserInfo().GetEquipCostume();
		if (equipCostume != null)
		{
			int length = equipCostume._EquipJewelLowId.Length;
			for (ushort i = 0; i < length; i++)
			{
				uint lowDataId = equipCostume._EquipJewelLowId[i];
				if (lowDataId <= 0)//미장착 이란 소리
					continue;
				
				//Debug.Log (" = jewel id:"+lowDataId);
				
				Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(lowDataId);
				if (0 < useLowData.OptionType)
				{
					ItemAbilityData data = new ItemAbilityData();
					data.Ability = (AbilityType)useLowData.OptionType;
					data.Value = useLowData.value;
					
					if (itemStats.ContainsKey(data.Ability) == false){
						itemStats.Add(data.Ability, 0);
					}
					
					//Debug.Log (" ability:"+ data.Ability+", v:"+data.Value+", abilityValue:"+RealValue(data.Ability, data.Value));
					
					//있는거면 더해주기
					itemStats[data.Ability] += RealValue(data.Ability, data.Value);
				}
			}
		}
		*/

		// // 일단 주석처리. 코스튬은 추후 다시 붙일 예정. 20171103 kyh
		///////////////////////////////// costume ability add
		/* 
		if (NetData.instance.GetUserInfo ().GetEquipCostume() != null){
			NetData.ItemAbilityData CostumeAbility = NetData.instance.GetUserInfo ().GetEquipCostume ().AbilityData;

			// new 
			// (캐릭터 스탯 + 코스튬 스탯) * formula = 캐릭터 최종스탯 이였으나 캐릭터 스탯 + (코스튬 스탯 * formula) = 캐릭터 최종스탯 으로 변경한다.
			// 따라서 코스튬 스탯을 itemStats에 add하지않고 바로 playerStats에 추가한다.2017.8.18 kyh
			if (CostumeAbility != null) {
				if (playerStats.ContainsKey (CostumeAbility.Ability) == false) {
					playerStats.Add (CostumeAbility.Ability, 0f);
				}
				#if UNITY_EDITOR
				if (bDebugStat){
				//Debug.Log (" === costume Stats   ===");
				//Debug.Log (" costume ability : " + CostumeAbility.Ability + ", v:" + _LowDataMgr.instance.GetCostumeAbilityValue (GetUserInfo ().GetEquipCostume ()._Grade, CostumeAbility.Value));
				}
				#endif
				playerStats [CostumeAbility.Ability] += _LowDataMgr.instance.GetCostumeAbilityValue (GetUserInfo ().GetEquipCostume ()._Grade, GetUserInfo ().GetEquipCostume ()._MinorGrade, CostumeAbility.Value);
			}
			///////////////////////////////// costume ability add
			/// 
		}
		*/
		

		if (bDebugStat) {
			Debug.Log (" === player + item   ===");
		}
		//최종스탯계산

		float subCharUpRate = 1f;//_LowDataMgr.instance.GetSubCharUpValue ();
		var enumerator = itemStats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (playerStats.ContainsKey(enumerator.Current.Key))
			{
				
				//float t = (1 + (_LowDataMgr.instance.CostumeGradeCalc( GetUserInfo().GetEquipCostume()._Grade )));// 1.02
				
				//				Debug.Log(" calc player stat===");
				//				Debug.Log(" player stat name:"+enumerator.Current.Key);
				//				Debug.Log(" playerStats:"+playerStats[enumerator.Current.Key]);
				//				Debug.Log(" ItemStat:"+enumerator.Current.Value);
				//Debug.Log(" formula:"+(1 + (_LowDataMgr.instance.CostumeGradeCalc( GetUserInfo().GetEquipCostume()._Grade ))) );
				//float val = (playerStats[enumerator.Current.Key] + enumerator.Current.Value) * (1 + (_LowDataMgr.instance.CostumeGradeCalc( GetUserInfo().GetEquipCostume()._Grade )));
				//Debug.Log(" val : "+val);
				//playerStats[enumerator.Current.Key] = (playerStats[enumerator.Current.Key] + enumerator.Current.Value) * (1 + (_LowDataMgr.instance.CostumeGradeCalc( GetUserInfo().GetEquipCostume()._Grade )));

				float prevVal = 0f;
				if (bDebugStat){
					prevVal = playerStats[enumerator.Current.Key];
				}
				playerStats[enumerator.Current.Key] = myRoundToInt2((playerStats[enumerator.Current.Key] + enumerator.Current.Value) * subCharUpRate);

				if (bDebugStat){
					Debug.Log (" = ability:"+enumerator.Current.Key+", pc ["+prevVal+"]+ item ["+enumerator.Current.Value+"]="+playerStats[enumerator.Current.Key]);
				}
			}
		}

		#if UNITY_EDITOR
		if (bDebugStat){
			Debug.Log (" === final  Stats ===");
			var finalEnum = playerStats.GetEnumerator();
			while (finalEnum.MoveNext())
			{
				Debug.Log(" ability:"+finalEnum.Current.Key+", v:"+finalEnum.Current.Value);
			}
		}
		#endif
		
		return playerStats;
	}
	
	//파트너 스탯정보 만들기
	public Dictionary<AbilityType, float> CalcPartnerStats(uint partnerLevel, Partner.PartnerDataInfo partnerInfo, uint quality/*, uint minorGrade, uint Enchant*/)
	{
		//일단 헬퍼에있는거랑 따로 계산
		Dictionary<AbilityType, float> partnerStats1 = new Dictionary<AbilityType, float>();
		Dictionary<AbilityType, float> partnerStats2 = new Dictionary<AbilityType, float>();
		Dictionary<AbilityType, float> partnerStatsComplet = new Dictionary<AbilityType, float>();


		float rate = 0.1f;
		partnerStats1.Add(AbilityType.HP, 					(float)((partnerInfo.BaseHp + (partnerLevel * partnerInfo.LevelUpHp)) * rate) );
		partnerStats1.Add(AbilityType.DAMAGE, 				(float)((partnerInfo.BaseAtk + (partnerLevel * partnerInfo.LevelUpAtk)) * rate) );
		partnerStats1.Add(AbilityType.HIT_RATE, 			(float)((partnerInfo.BaseHit + (partnerLevel * partnerInfo.LevelHitRate)) * rate) );
		partnerStats1.Add(AbilityType.DODGE_RATE, 			(float)((partnerInfo.BaseAvoid + (partnerLevel * partnerInfo.LevelAvoidRate)) * rate) );
		partnerStats1.Add(AbilityType.CRITICAL_RATE, 		(float)(partnerInfo.BaseCriticalRate * rate));
		partnerStats1.Add(AbilityType.CRITICAL_RES, 		(float)(partnerInfo.BaseCriticalResist * rate));
		partnerStats1.Add(AbilityType.CRITICAL_DAMAGE, 		(float)(partnerInfo.BaseCriticalDamage * rate));
		partnerStats1.Add(AbilityType.DRAIN_HP, 			(float)(partnerInfo.BaseLifeSteal * rate));
		partnerStats1.Add(AbilityType.DEFENCE_IGNORE, 		(float)(partnerInfo.BaseIgnoreAtk * rate));
		partnerStats1.Add(AbilityType.DAMAGE_DECREASE, 		(float)(partnerInfo.BaseDamageDown * rate));
		partnerStats1.Add(AbilityType.DAMAGE_DECREASE_RATE, (float)(partnerInfo.BaseDamageDownRate * rate));
		
		partnerStats2.Add(AbilityType.SUPERARMOR, 			(float)(partnerInfo.BaseSuperArmor + (partnerLevel * partnerInfo.LevelUpSuperArmor)));
		partnerStats2.Add(AbilityType.SUPERARMOR_RECOVERY, 	(float)(partnerInfo.SuperArmorRecovery));
		partnerStats2.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, (float)(partnerInfo.SuperArmorRecoveryRate));
		partnerStats2.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, (float)(partnerInfo.SuperArmorRecoveryTime));


        //안곱해도됨
        partnerStatsComplet.Add(AbilityType.WEIGHT, (float)partnerInfo.Weight);

        // 0.1 곱해야 하는 stat들.

        // 0.1 곱해야 하는 stat들.
        var enumerator1 = partnerStats1.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            // old
            //float chantVal = _LowDataMgr.instance.PartnerRefineCalc(Enchant);
            //float fValue = (enumerator.Current.Value * (1 + _LowDataMgr.instance.PartnerRefineCalc(Enchant) +
            //                                                _LowDataMgr.instance.PartnerGradeCalc((uint)((partnerInfo.Quality - 1) * 5) + minorGrade) ));

            // fValue =         value               *  1.015^파트너 강화 수치 
			float fValue = myRoundToInt2(enumerator1.Current.Value); /** _LowDataMgr.instance.PartnerRefineCalc(0)*/;
            partnerStatsComplet.Add(enumerator1.Current.Key, fValue);
        }

        var enumerator2 = partnerStats2.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            // fValue =         value               *  1.015^파트너 강화 수치 
            float fValue = Mathf.RoundToInt( enumerator2.Current.Value); // * _LowDataMgr.instance.PartnerRefineCalc(0);
            partnerStatsComplet.Add(enumerator2.Current.Key, fValue);
        }

        return partnerStatsComplet;

	}

	int myRoundToInt2(float v){

		return Mathf.FloorToInt (v += 0.51f);

//		if (v - Mathf.Floor(v) > 0.499999f) {
//			return Mathf.FloorToInt(v) + 1;
//		}
//		
//		return Mathf.FloorToInt (v);
	}


	#region :: 유저 싱크 데이터 ::
	public float RealValue(AbilityType type, float value)
	{
		//float _value = value;
		if (type == AbilityType.HP) return value * 0.1f;
		if (type == AbilityType.DAMAGE) return value * 0.1f;
		if (type == AbilityType.HIT_RATE) return value * 0.1f;
		if (type == AbilityType.DODGE_RATE) return value * 0.1f;
		if (type == AbilityType.CRITICAL_RATE) return value * 0.1f;
		if (type == AbilityType.CRITICAL_RES) return value * 0.1f;
		if (type == AbilityType.CRITICAL_DAMAGE) return value * 0.1f;
		if (type == AbilityType.DRAIN_HP) return value * 0.1f;
		if (type == AbilityType.DEFENCE_IGNORE) return value * 0.1f;
		if (type == AbilityType.DAMAGE_DECREASE) return value * 0.1f;
		if (type == AbilityType.DAMAGE_DECREASE_RATE) return value * 0.1f;
		if (type == AbilityType.SUPERARMOR) return value;
		if (type == AbilityType.SUPERARMOR_RECOVERY) return value;
		if (type == AbilityType.SUPERARMOR_RECOVERY_RATE) return value;
		if (type == AbilityType.SUPERARMOR_RECOVERY_TIME) return value;
		
		return 0;
	}
	
	public float CalcSkillCoolTime(float skillCoolTime, float coolTimeReduce){
		
		float value1 = _LowDataMgr.instance.GetFormula (FormulaType.COOLTIME_REDUCE, 1);
		float value2 = _LowDataMgr.instance.GetFormula (FormulaType.COOLTIME_REDUCE, 2);
		float value3 = _LowDataMgr.instance.GetFormula (FormulaType.COOLTIME_REDUCE, 3);
		float coolTimeCap = _LowDataMgr.instance.GetFormula (FormulaType.COOLTIME_REDUCE_CAP, 1);

		float coolTimeReduceFinal = coolTimeReduce * value1 * value2 * value3;
		if (coolTimeReduceFinal > coolTimeCap) {
			coolTimeReduceFinal =  coolTimeCap;
		}
		float coolTime = skillCoolTime - coolTimeReduceFinal;
		
		return coolTime;
		//return skillCoolTime - coolTimeReduce;
	}
	
	
	
	/// <summary> 파트너 전투력 계산(승급 or 강화 용) </summary>
	public float GetPartnerBattlePoint(Partner.PartnerDataInfo lowData, uint level, uint quality/*, uint minorGrade, uint enchant*/)
	{
		Dictionary<AbilityType, float> stats = CalcPartnerStats(level, lowData, quality/*, minorGrade, enchant*/);
		
		float total = 0;
		for(int i=1; i < (int)BattlePointType.MAX; i++)
		{
			BattlePointType battleType = (BattlePointType)i;
			AbilityType aType = (AbilityType)Enum.Parse(typeof(AbilityType), battleType.ToString() );

			if (!stats.ContainsKey(aType))
				continue;

			//Debug.Log(string.Format("{0}, {1}, {2}", battleType, aType, stats[aType]) );
			//float v = _LowDataMgr.instance.GetLowDataBattlePoint(battleType, stats[aType]); 
			//Debug.Log(string.Format("{0}, {1}, {2}", battleType, aType, v) );
			total += _LowDataMgr.instance.GetLowDataBattlePoint(battleType, stats[aType]);
		}

		//Debug.Log (" PartnerBattlePt:" + total+", level:"+level+", quality:"+quality+", minor:"+minorGrade+",enchant:"+enchant);
		//return (float)Mathf.RoundToInt (total); // round from 0.1~0.5 cutted. so add 0.5 and calc round.
		return (float)myRoundToInt2 (total);
	}

	public _PlayerSyncData _playerSyncData = null;
	
	public void MakePlayerFreeFightSyncData(PMsgBattleMapEnterMapS netData )
	{
		//내 플레이어용 싱크데이터
		_playerSyncData = null;
		_playerSyncData = new _PlayerSyncData();
		
		PlayerUnitData myData = new PlayerUnitData();
		_playerSyncData.Init();
		
		Dictionary<AbilityType, float> playerStats = CalcPlayerStats();
		
		playerStats[AbilityType.HP] = netData.UnHp;
        playerStats[AbilityType.SUPERARMOR] = netData.UnMaxSuperArmor;

        uint HELMETID = 0;
		uint CLOTHID = 0;
		uint WEAPONID = 0;
		
		Item.EquipmentInfo tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.HELMET);
		if (tempInfo == null)
			HELMETID = 0;
		else
			HELMETID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.CLOTH);
		if (tempInfo == null)
			CLOTHID = 0;
		else
			CLOTHID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.WEAPON);
		if (tempInfo == null)
			WEAPONID = 0;
		else
			WEAPONID = tempInfo.Id;
		
		//ushort[] skillLevel = null;
		//uint equipCostumeId = 0;
		//if (GetUserInfo().GetEquipCostume() == null)
		//{
		//    Newbie.NewbieInfo newbieinfo = _LowDataMgr.instance.GetNewbieCharacterData(GetUserInfo()._userCharIndex);
		//    equipCostumeId = newbieinfo.CostumIdx;
		
		//    skillLevel = new ushort[] {
		//        1, 1, 1, 1, 1
		//    };
		//}
		//else
		//{
		//    equipCostumeId = GetUserInfo().GetEquipCostume()._costmeDataIndex;
		//    skillLevel = GetUserInfo().GetEquipCostume()._skillLevel;
		//}
		
		ushort[] skillLevel = null;
		uint equipCostumeId = 0;
		
		equipCostumeId = (uint)netData.UnCostumeId;
		skillLevel = new ushort[] { 1, 1, 1, 1, 1 };
		
		//Item.CostumeInfo costumeData = _LowDataMgr.instance.GetLowDataCostumeInfo(equipCostumeId);
        SkillSetData skillSetData = GetUserInfo().GetEquipSKillSet();
        myData.Init(0,
		            0,
		            Nickname,
		            NetData.instance.AccountUUID,
		            0,
		            GetUserInfo().GetCharIdx(),
		            HELMETID,
		            equipCostumeId,
		            CLOTHID,
		            WEAPONID,
                    skillSetData.SkillSetId,
		            GetUserInfo().isHideCostum,
		            GetUserInfo()._Level,
		            GetUserInfo()._LeftTitle,
		            GetUserInfo()._RightTitle,
		            playerStats);

        uint skillIndex = 0;
        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetData.SkillSetId);
        for (int i = 0; i < setInfo.skill0.Count; i++)
        {
            uint.TryParse(setInfo.skill0[i], out skillIndex);
            myData.NormalAttackData[i] = new SkillData(skillIndex, (byte)skillLevel[0]);
        }

        myData.SkillData[0] = new SkillData(skillSetData.SkillId[0], (byte)skillSetData.SkillLevel[0]);
        myData.SkillData[1] = new SkillData(skillSetData.SkillId[1], (byte)skillSetData.SkillLevel[1]);
        myData.SkillData[2] = new SkillData(skillSetData.SkillId[2], (byte)skillSetData.SkillLevel[2]);
        myData.SkillData[3] = new SkillData(skillSetData.SkillId[3], (byte)skillSetData.SkillLevel[3]);

        for (int i = 0; i < setInfo.Chain.Count; i++)
        {
            uint.TryParse(setInfo.Chain[i], out skillIndex);
            myData.SkillData[4 + i] = new SkillData(skillIndex, (byte)skillLevel[1]);
        }
        _playerSyncData.playerSyncDatas.Add(myData);
		
	}
	
	public void MakePlayerSyncData(bool bMakePartnerData)
	{
		//내 플레이어용 싱크데이터
		_playerSyncData = null;
		_playerSyncData = new _PlayerSyncData();
		
		PlayerUnitData myData = new PlayerUnitData();
		_playerSyncData.Init();
		
		Dictionary<AbilityType, float> playerStats = CalcPlayerStats();
		
		uint HELMETID = 0;
		uint CLOTHID = 0;
		uint WEAPONID = 0;
		
		Item.EquipmentInfo tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.HELMET);
		if (tempInfo == null)
			HELMETID = 0;
		else
			HELMETID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.CLOTH);
		if (tempInfo == null)
			CLOTHID = 0;
		else
			CLOTHID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.WEAPON);
		if (tempInfo == null)
			WEAPONID = 0;
		else
			WEAPONID = tempInfo.Id;
		
		ushort[] skillLevel = null;
		uint equipCostumeId = 0;
		if (GetUserInfo().GetEquipCostume() == null)
		{
			Newbie.NewbieInfo newbieinfo = _LowDataMgr.instance.GetNewbieCharacterData(GetUserInfo()._userCharIndex);
			equipCostumeId = newbieinfo.CostumIdx;
			
			skillLevel = new ushort[] {
				1, 1, 1, 1, 1
			};
		}
		else
		{
			equipCostumeId = GetUserInfo().GetEquipCostume()._costmeDataIndex;
			skillLevel = GetUserInfo().GetEquipCostume()._skillLevel;
		}
		
        SkillSetData skillSetData = GetUserInfo().GetEquipSKillSet();
        uint skillSetId=0;
        uint[] skillIds = new uint[4];
        uint[] skillLv = new uint[4];

        if (skillSetData == null)
        {
            switch (GetUserInfo().GetCharIdx())
            {
                case 11000:
                    skillSetId = 100;
                    break;
                case 12000:
                    skillSetId = 200;
                    break;
                case 13000:
                    skillSetId = 300;
                    break;
            }

            SkillTables.SkillSetInfo info = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
            skillIds[0] = info.skill1;
            skillIds[1] = info.skill2;
            skillIds[2] = info.skill3;
            skillIds[3] = info.skill4;
            for (int i = 0; i < skillLv.Length; i++)
                skillLv[i] = 1;
        }
        else
        {
            skillSetId = skillSetData.SkillSetId;
            for(int i=0; i < skillIds.Length; i++)
            {
                skillIds[i] = skillSetData.SkillId[i];
                skillLv[i] = skillSetData.SkillLevel[i];
            }
        }

        myData.Init(0,
		            (int)eTeamType.Team1,
		            Nickname,
		            AccountUUID,                
		            0,
		            GetUserInfo().GetCharIdx(),
		            HELMETID,
		            equipCostumeId,
		            CLOTHID,
		            WEAPONID,
                    skillSetId,
                    GetUserInfo().isHideCostum,
		            GetUserInfo()._Level,
		            GetUserInfo()._LeftTitle,
		            GetUserInfo()._RightTitle,
		            playerStats);
		
        uint skillIndex = 0;
        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        for (int i = 0; i < setInfo.skill0.Count; i++)
        {
            uint.TryParse(setInfo.skill0[i], out skillIndex);
            myData.NormalAttackData[i] = new SkillData(skillIndex, (byte)skillLevel[0]);
        }

        myData.SkillData[0] = new SkillData(skillIds[0], (byte)skillLv[0]);
        myData.SkillData[1] = new SkillData(skillIds[1], (byte)skillLv[1]);
        myData.SkillData[2] = new SkillData(skillIds[2], (byte)skillLv[2]);
        myData.SkillData[3] = new SkillData(skillIds[3], (byte)skillLv[3]);

        for (int i = 0; i < setInfo.Chain.Count; i++)
        {
            uint.TryParse(setInfo.Chain[i], out skillIndex);
            myData.SkillData[4 + i] = new SkillData(skillIndex, (byte)skillLevel[1]);
        }

        _playerSyncData.playerSyncDatas.Add(myData);

        if (!bMakePartnerData)
			return;
		
		for (int i = 0; i < 2; i++)//GetUserInfo().GetPartnerList().Count
        {
			_PartnerData partner = GetUserInfo().GetEquipPartner(i+1);//슬롯 시작값이 1부터이다. i를 1부터 시작시키게 했다.

            if (partner != null)
			{
				Partner.PartnerDataInfo partnerInfo = partner.GetLowData();
				Dictionary<AbilityType, float> partnerStats = CalcPartnerStats(partner._NowLevel, partnerInfo, partnerInfo.Quality/*, partner._MinorGrade, partner._Enchant*/);
				
				PlayerUnitData partnerData = new PlayerUnitData();
				partnerData.Init((byte)(i+1),
				                 (byte)eTeamType.Team1,
				                 partner.GetLocName(),//"Partner0",
				                 2 + (ulong)i,
                                 0,
				                 partner._partnerDataIndex,
				                 partner._NowLevel,
				                 partnerStats);
				
				partnerData.NormalAttackData[0] = new SkillData(partnerInfo.skill0, 1);
				partnerData.SkillData[0] = new SkillData(partnerInfo.skill1, 1);
				partnerData.SkillData[1] = new SkillData(partnerInfo.skill2, 1);
				partnerData.SkillData[2] = new SkillData(partnerInfo.skill3, 1);
				partnerData.SkillData[3] = new SkillData(partnerInfo.skill4, 1);
				
                //파트너 버프 삭제
				//if (partner.GetBuffSkillToSlot(0) != null)
				//	if (partner.GetBuffSkillToSlot(0)._skillIndex != 0)
				//		partnerData.PassiveBuff[0] = new SkillData(partner.GetBuffSkillToSlot(0)._skillIndex, (byte)partner.GetBuffSkillToSlot(0)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(1) != null)
				//	if (partner.GetBuffSkillToSlot(1)._skillIndex != 0)
				//		partnerData.PassiveBuff[1] = new SkillData(partner.GetBuffSkillToSlot(1)._skillIndex, (byte)partner.GetBuffSkillToSlot(1)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(2) != null)
				//	if (partner.GetBuffSkillToSlot(2)._skillIndex != 0)
				//		partnerData.PassiveBuff[2] = new SkillData(partner.GetBuffSkillToSlot(2)._skillIndex, (byte)partner.GetBuffSkillToSlot(2)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(3) != null)
				//	if (partner.GetBuffSkillToSlot(3)._skillIndex != 0)
				//		partnerData.PassiveBuff[3] = new SkillData(partner.GetBuffSkillToSlot(3)._skillIndex, (byte)partner.GetBuffSkillToSlot(3)._skillLevel);
				
				_playerSyncData.partnerSyncDatas.Add(partnerData);
			}
		}
	}
	
	public _PlayerSyncData OtherPcSyncData(ulong AccountUUID, ulong a_rUUID, string a_Nick, uint a_CharIdx, uint a_userCostumeIdx, uint a_userLevel, uint prefix, uint suffix, uint maxSuperArmor, uint skillSetId)
	{
		_PlayerSyncData otherPcSyncData = new _PlayerSyncData();
		//if (_playerSyncData == null)
		//    return;
		
		PlayerUnitData myData = new PlayerUnitData();
		
		Dictionary<AbilityType, float> playerStats = new Dictionary<AbilityType, float>();
		
		//기본 스탯 넣어주자
		uint iLevel = 1;
		
		//일단 기본캐릭만
		Character.CharacterInfo charInfo = _LowDataMgr.instance.GetCharcterData(11000);
		playerStats.Add(AbilityType.HP, (float)(charInfo.BaseHp + iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1) * charInfo.LevelUpHp));
		playerStats.Add(AbilityType.DAMAGE, (float)(charInfo.BaseAtk + iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1) * charInfo.LevelUpAtk));
		playerStats.Add(AbilityType.HIT_RATE, (float)(charInfo.BaseHit * 0.1f + iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1) *charInfo.LevelHitRate));
		playerStats.Add(AbilityType.DODGE_RATE, (float)(charInfo.BaseAvoid * 0.1f + iLevel * _LowDataMgr.instance.GetFormula(FormulaType.LEVELUP_CALIBRATION, 1) *charInfo.LevelAvoidRate));
		playerStats.Add(AbilityType.CRITICAL_RATE, (float)(charInfo.BaseCriticalRate * 0.1f));
		playerStats.Add(AbilityType.CRITICAL_RES, (float)(charInfo.BaseCriticalResist * 0.1f));
		playerStats.Add(AbilityType.CRITICAL_DAMAGE, (float)(charInfo.BaseCriticalDamage * 0.1f));
		playerStats.Add(AbilityType.DRAIN_HP, (float)(charInfo.BaseLifeSteal * 0.1f));
		playerStats.Add(AbilityType.DEFENCE_IGNORE, (float)(charInfo.BaseIgnoreAtk));
		playerStats.Add(AbilityType.DAMAGE_DECREASE, (float)(charInfo.BaseDamageDown));
		playerStats.Add(AbilityType.DAMAGE_DECREASE_RATE, (float)(charInfo.BaseDamageDownRate * 0.1f));
		playerStats.Add(AbilityType.EXP_UP, 0f);
		playerStats.Add(AbilityType.ALLSTAT_RATE, 0f);
		playerStats.Add(AbilityType.SUPERARMOR, maxSuperArmor);
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY, (float)(charInfo.SuperArmorRecovery));
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, (float)(charInfo.SuperArmorRecoveryRate));
		playerStats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, (float)(charInfo.SuperArmorRecoveryTime));
		
		/*
        for (int i = 0; i < (int)ePartType.PART_MAX; i++)
            _ItemData equipItem = GetUserInfo().GetEquipParts((ePartType)i);

            if (equipItem != null)
            {
                var enumerator = equipItem.Stats.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (playerStats.ContainsKey(enumerator.Current.Key))
                    {
                        //있는거면 더해주기
                        playerStats[enumerator.Current.Key] += RealValue(enumerator.Current.Key, enumerator.Current.Value);
                    }
                    else
                    {
                        //없는거면 추가
                        playerStats.Add(enumerator.Current.Key, RealValue(enumerator.Current.Key, enumerator.Current.Value));
                    }

                }
            }
        }
        */
		
		uint HELMETID = 0;
		uint CLOTHID = 0;
		uint WEAPONID = 0;
		
		Item.EquipmentInfo tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.HELMET);
		if (tempInfo == null)
			HELMETID = 0;
		else
			HELMETID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.CLOTH);
		if (tempInfo == null)
			CLOTHID = 0;
		else
			CLOTHID = tempInfo.Id;
		
		tempInfo = GetUserInfo().GetEquipPartsLowData(ePartType.WEAPON);
		if (tempInfo == null)
			WEAPONID = 0;
		else
			WEAPONID = tempInfo.Id;
		
        //서버에서 정상적인 데이터 오기전까지는 일렇게.
        if(skillSetId <= 0 )
        {
            switch(a_CharIdx)
            {
                case 11000:
                    skillSetId = 100;
                    break;
                case 12000:
                    skillSetId = 200;
                    break;
                case 13000:
                    skillSetId = 300;
                    break;
            }
        }

		myData.Init(0,
		            0,
		            a_Nick,
		            AccountUUID,
		            a_rUUID,
		            a_CharIdx,
		            0,
		            a_userCostumeIdx,
		            0,
		            0,
                    skillSetId,
                    false,
		            1,
		            prefix,
		            suffix, 
		            playerStats);
		
		//Item.CostumeInfo costumeData = _LowDataMgr.instance.GetLowDataCostumeInfo(a_userCostumeIdx);
		
        uint skillIndex = 0;
        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        for (int i = 0; i < setInfo.skill0.Count; i++)
        {
            uint.TryParse(setInfo.skill0[i], out skillIndex);
            myData.NormalAttackData[i] = new SkillData(skillIndex, (byte)1);
        }

        myData.SkillData[0] = new SkillData(setInfo.skill1, (byte)1);
        myData.SkillData[1] = new SkillData(setInfo.skill2, (byte)1);
        myData.SkillData[2] = new SkillData(setInfo.skill3, (byte)1);
        myData.SkillData[3] = new SkillData(setInfo.skill4, (byte)1);

        for (int i = 0; i < setInfo.Chain.Count; i++)
        {
            uint.TryParse(setInfo.Chain[i], out skillIndex);
            myData.SkillData[4 + i] = new SkillData(skillIndex, (byte)1);
        }
        
		otherPcSyncData.playerSyncDatas.Add(myData);
		
		return otherPcSyncData;
	}
	
	public PlayerUnitData MakeEnemySyncData(ulong uuID, string nick, uint charIdx, uint level, uint vipLv, int att, uint costumeIdx
	                                        , bool isHideCostume, List<int> equipIdxList, List<_PartnerData> parList, uint prefix, uint suffix, uint skillSetId)
	{
		PlayerUnitData enemyData = new PlayerUnitData();
		List<_ItemData> equipList = new List<_ItemData>();
		uint HELMETID = 0;
		uint CLOTHID = 0;
		uint WEAPONID = 0;
		for (int i = 0; i < equipIdxList.Count; i++)
		{
			Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)equipIdxList[i]);
			if (equipInfo == null)
				continue;
			
			switch ((ePartType)equipInfo.Type)
			{
			case ePartType.CLOTH:
				CLOTHID = equipInfo.Id;
				break;
			case ePartType.HELMET:
				HELMETID = equipInfo.Id;
				break;
			case ePartType.WEAPON:
				WEAPONID = equipInfo.Id;
				break;
			}
			
			equipList.Add(new _ItemData((ulong)equipIdxList[i], (uint)equipIdxList[i], 0, equipInfo.Grade, 0, 0));
		}
		
		ushort[] skillLevel = new ushort[] {//스킬의 레벨을 현재는 알 수 없다. 1로 강제지정한다.
			1, 1, 1, 1, 1
		};
		Dictionary<AbilityType, float> playerStats = CalcPlayerStats(charIdx, level, equipList, new _CostumeData((ulong)costumeIdx, (uint)costumeIdx, 1, 0, skillLevel, true, true) );
		
		if (isHideCostume)
		{
		}
		else
		{
		}
		
		//Item.CostumeInfo costumeData = _LowDataMgr.instance.GetLowDataCostumeInfo(costumeIdx);
        //서버에서 정상적인 데이터 오기전까지는 일렇게.
        if (skillSetId <= 0)
        {
            switch (charIdx)
            {
                case 11000:
                    skillSetId = 100;
                    break;
                case 12000:
                    skillSetId = 200;
                    break;
                case 13000:
                    skillSetId = 300;
                    break;
            }
        }
        enemyData.Init(0,
		               (int)eTeamType.Team2,
		               nick,
		               uuID,
		               uuID,
		               charIdx,
		               HELMETID,
		               costumeIdx,
		               CLOTHID,
		               WEAPONID,
                       skillSetId,
                       isHideCostume,
		               level,
		               prefix,
		               suffix,
		               playerStats);

        uint skillIndex = 0;
        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        for (int i = 0; i < setInfo.skill0.Count; i++)
        {
            uint.TryParse(setInfo.skill0[i], out skillIndex);
            enemyData.NormalAttackData[i] = new SkillData(skillIndex, (byte)skillLevel[0]);
        }

        enemyData.SkillData[0] = new SkillData(setInfo.skill1, (byte)1);
        enemyData.SkillData[1] = new SkillData(setInfo.skill2, (byte)1);
        enemyData.SkillData[2] = new SkillData(setInfo.skill3, (byte)1);
        enemyData.SkillData[3] = new SkillData(setInfo.skill4, (byte)1);

        for (int i = 0; i < setInfo.Chain.Count; i++)
        {
            uint.TryParse(setInfo.Chain[i], out skillIndex);
            enemyData.SkillData[4 + i] = new SkillData(skillIndex, (byte)skillLevel[1]);
        }
        _playerSyncData.playerSyncDatas.Add(enemyData);
		
		for (int i = 0; i < parList.Count; i++)
		{
			_PartnerData partner = parList[i];
			
			if (partner != null)
			{
				Partner.PartnerDataInfo partnerInfo = partner.GetLowData();
				if (partnerInfo == null)//테이블 변경으로 서버에서 가지고 있는 데이터와 다를 수 있다.
				{
					Debug.LogWarning("not found partner lowData error " + partner._partnerDataIndex);
					continue;
				}
				
				Dictionary<AbilityType, float> partnerStats = CalcPartnerStats(partner._NowLevel, partnerInfo, partnerInfo.Quality/*, partner._MinorGrade, partner._Enchant*/);
				
				PlayerUnitData partnerData = new PlayerUnitData();
				partnerData.Init((byte)(i+1),
				                 (byte)eTeamType.Team2,
				                 partner.GetLocName(),//"Partner0",
				                 2 + (ulong)i,
                                 2 + (ulong)i,
                                 partner._partnerDataIndex,
				                 partner._NowLevel,
				                 partnerStats);
				
				partnerData.NormalAttackData[0] = new SkillData(partnerInfo.skill0, 1);
				partnerData.SkillData[0] = new SkillData(partnerInfo.skill1, 1);
				partnerData.SkillData[1] = new SkillData(partnerInfo.skill2, 1);
				partnerData.SkillData[2] = new SkillData(partnerInfo.skill3, 1);
				partnerData.SkillData[3] = new SkillData(partnerInfo.skill4, 1);
				
				//파트너 버프 삭제
				//if (partner.GetBuffSkillToSlot(0) != null)
				//	if (partner.GetBuffSkillToSlot(0)._skillIndex != 0)
				//		partnerData.PassiveBuff[0] = new SkillData(partner.GetBuffSkillToSlot(0)._skillIndex, (byte)partner.GetBuffSkillToSlot(0)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(1) != null)
				//	if (partner.GetBuffSkillToSlot(1)._skillIndex != 0)
				//		partnerData.PassiveBuff[1] = new SkillData(partner.GetBuffSkillToSlot(1)._skillIndex, (byte)partner.GetBuffSkillToSlot(1)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(2) != null)
				//	if (partner.GetBuffSkillToSlot(2)._skillIndex != 0)
				//		partnerData.PassiveBuff[2] = new SkillData(partner.GetBuffSkillToSlot(2)._skillIndex, (byte)partner.GetBuffSkillToSlot(2)._skillLevel);
				
				//if (partner.GetBuffSkillToSlot(3) != null)
				//	if (partner.GetBuffSkillToSlot(3)._skillIndex != 0)
				//		partnerData.PassiveBuff[3] = new SkillData(partner.GetBuffSkillToSlot(3)._skillIndex, (byte)partner.GetBuffSkillToSlot(3)._skillLevel);
				
				_playerSyncData.partnerSyncDatas.Add(partnerData);
			}
		}
		
		return enemyData;
	}
	#endregion
}
