using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Character
{
    [Serializable]
    public class CharacterInfo
    {
        public uint Id;
        uint _NameId;
        public uint NameId
        {
            set { _NameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NameId); }
        }
        uint _DescriptionId;
        public uint DescriptionId
        {
            set { _DescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_DescriptionId); }
        }
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        public string PortraitId;
        byte _AiType;
        public byte AiType
        {
            set { _AiType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AiType); }
        }
        byte _Speed;
        public byte Speed
        {
            set { _Speed = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Speed); }
        }
        public float Weight;
        uint _BaseHp;
        public uint BaseHp
        {
            set { _BaseHp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseHp); }
        }
        uint _BaseAtk;
        public uint BaseAtk
        {
            set { _BaseAtk = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseAtk); }
        }
        uint _BaseHit;
        public uint BaseHit
        {
            set { _BaseHit = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseHit); }
        }
        uint _BaseAvoid;
        public uint BaseAvoid
        {
            set { _BaseAvoid = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseAvoid); }
        }
        uint _BaseCriticalRate;
        public uint BaseCriticalRate
        {
            set { _BaseCriticalRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseCriticalRate); }
        }
        uint _BaseCriticalResist;
        public uint BaseCriticalResist
        {
            set { _BaseCriticalResist = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseCriticalResist); }
        }
        uint _BaseCriticalDamage;
        public uint BaseCriticalDamage
        {
            set { _BaseCriticalDamage = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseCriticalDamage); }
        }
        uint _BaseLifeSteal;
        public uint BaseLifeSteal
        {
            set { _BaseLifeSteal = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseLifeSteal); }
        }
        uint _BaseIgnoreAtk;
        public uint BaseIgnoreAtk
        {
            set { _BaseIgnoreAtk = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseIgnoreAtk); }
        }
        uint _BaseDamageDown;
        public uint BaseDamageDown
        {
            set { _BaseDamageDown = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseDamageDown); }
        }
        uint _BaseDamageDownRate;
        public uint BaseDamageDownRate
        {
            set { _BaseDamageDownRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseDamageDownRate); }
        }
        uint _BaseSuperArmor;
        public uint BaseSuperArmor
        {
            set { _BaseSuperArmor = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BaseSuperArmor); }
        }
        uint _SuperArmorRecoveryTime;
        public uint SuperArmorRecoveryTime
        {
            set { _SuperArmorRecoveryTime = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SuperArmorRecoveryTime); }
        }
        uint _SuperArmorRecoveryRate;
        public uint SuperArmorRecoveryRate
        {
            set { _SuperArmorRecoveryRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SuperArmorRecoveryRate); }
        }
        uint _SuperArmorRecovery;
        public uint SuperArmorRecovery
        {
            set { _SuperArmorRecovery = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SuperArmorRecovery); }
        }
        uint _LevelUpHp;
        public uint LevelUpHp
        {
            set { _LevelUpHp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelUpHp); }
        }
        uint _LevelUpAtk;
        public uint LevelUpAtk
        {
            set { _LevelUpAtk = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelUpAtk); }
        }
        uint _LevelAvoidRate;
        public uint LevelAvoidRate
        {
            set { _LevelAvoidRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelAvoidRate); }
        }
        uint _LevelHitRate;
        public uint LevelHitRate
        {
            set { _LevelHitRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelHitRate); }
        }
        uint _LevelUpSuperArmor;
        public uint LevelUpSuperArmor
        {
            set { _LevelUpSuperArmor = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelUpSuperArmor); }
        }
    }

    [Serializable]
    public class StatusInfo
    {
        public uint Id;
        byte _StatusType;
        public byte StatusType
        {
            set { _StatusType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_StatusType); }
        }
        uint _StatusNameId;
        public uint StatusNameId
        {
            set { _StatusNameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_StatusNameId); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _Level;
        public byte Level
        {
            set { _Level = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Level); }
        }
        uint _GoalNameId;
        public uint GoalNameId
        {
            set { _GoalNameId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GoalNameId); }
        }
        uint _GoalDescriptionId;
        public uint GoalDescriptionId
        {
            set { _GoalDescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GoalDescriptionId); }
        }
        byte _ClearType;
        public byte ClearType
        {
            set { _ClearType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearType); }
        }
        uint _Clearvalue;
        public uint Clearvalue
        {
            set { _Clearvalue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Clearvalue); }
        }
        byte _RewardType;
        public byte RewardType
        {
            set { _RewardType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RewardType); }
        }
        uint _RewardId01;
        public uint RewardId01
        {
            set { _RewardId01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId01); }
        }
        uint _RewardId02;
        public uint RewardId02
        {
            set { _RewardId02 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId02); }
        }
        uint _RewardId03;
        public uint RewardId03
        {
            set { _RewardId03 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardId03); }
        }
        uint _NextId;
        public uint NextId
        {
            set { _NextId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextId); }
        }
    }

    public List<StatusInfo> StatusInfoList = new List<StatusInfo>();
    public Dictionary<uint, CharacterInfo> CharacterInfoDic = new Dictionary<uint, CharacterInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Character_Character", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Character = new JSONObject(strSrc);

            for (int i = 0; i < Character.list.Count; i++)
            {
                CharacterInfo tmpInfo = new CharacterInfo();
                tmpInfo.Id = (uint)Character[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)Character[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Character[i]["DescriptionId_ui"].n;
                tmpInfo.Class = (byte)Character[i]["Class_b"].n;
                tmpInfo.PortraitId = Character[i]["PortraitId_c"].str;
                tmpInfo.AiType = (byte)Character[i]["AiType_b"].n;
                tmpInfo.Speed = (byte)Character[i]["Speed_b"].n;
                tmpInfo.Weight = (float)Character[i]["Weight_f"].n;
                tmpInfo.BaseHp = (uint)Character[i]["BaseHp_ui"].n;
                tmpInfo.BaseAtk = (uint)Character[i]["BaseAtk_ui"].n;
                tmpInfo.BaseHit = (uint)Character[i]["BaseHit_ui"].n;
                tmpInfo.BaseAvoid = (uint)Character[i]["BaseAvoid_ui"].n;
                tmpInfo.BaseCriticalRate = (uint)Character[i]["BaseCriticalRate_ui"].n;
                tmpInfo.BaseCriticalResist = (uint)Character[i]["BaseCriticalResist_ui"].n;
                tmpInfo.BaseCriticalDamage = (uint)Character[i]["BaseCriticalDamage_ui"].n;
                tmpInfo.BaseLifeSteal = (uint)Character[i]["BaseLifeSteal_ui"].n;
                tmpInfo.BaseIgnoreAtk = (uint)Character[i]["BaseIgnoreAtk_ui"].n;
                tmpInfo.BaseDamageDown = (uint)Character[i]["BaseDamageDown_ui"].n;
                tmpInfo.BaseDamageDownRate = (uint)Character[i]["BaseDamageDownRate_ui"].n;
                tmpInfo.BaseSuperArmor = (uint)Character[i]["BaseSuperArmor_ui"].n;
                tmpInfo.SuperArmorRecoveryTime = (uint)Character[i]["SuperArmorRecoveryTime_ui"].n;
                tmpInfo.SuperArmorRecoveryRate = (uint)Character[i]["SuperArmorRecoveryRate_ui"].n;
                tmpInfo.SuperArmorRecovery = (uint)Character[i]["SuperArmorRecovery_ui"].n;
                tmpInfo.LevelUpHp = (uint)Character[i]["LevelUpHp_ui"].n;
                tmpInfo.LevelUpAtk = (uint)Character[i]["LevelUpAtk_ui"].n;
                tmpInfo.LevelAvoidRate = (uint)Character[i]["LevelAvoidRate_ui"].n;
                tmpInfo.LevelHitRate = (uint)Character[i]["LevelHitRate_ui"].n;
                tmpInfo.LevelUpSuperArmor = (uint)Character[i]["LevelUpSuperArmor_ui"].n;

                CharacterInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {//캐릭터 신분
            TextAsset data = Resources.Load("TestJson/Character_Status", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Status = new JSONObject(strSrc);

            for (int i = 0; i < Status.list.Count; i++)
            {
                StatusInfo tmpInfo = new StatusInfo();
                tmpInfo.Id = (uint)Status[i]["Id_ui"].n;
                tmpInfo.StatusType = (byte)Status[i]["StatusType_b"].n;
                tmpInfo.StatusNameId = (uint)Status[i]["StatusNameId_ui"].n;
                tmpInfo.Icon = (uint)Status[i]["Icon_ui"].n;
                tmpInfo.Type = (byte)Status[i]["Type_b"].n;
                tmpInfo.Level = (byte)Status[i]["Level_b"].n;
                tmpInfo.GoalNameId = (uint)Status[i]["GoalNameId_ui"].n;
                tmpInfo.GoalDescriptionId = (uint)Status[i]["GoalDescriptionId_ui"].n;
                tmpInfo.ClearType = (byte)Status[i]["ClearType_b"].n;
                tmpInfo.Clearvalue = (uint)Status[i]["Clearvalue_ui"].n;
                tmpInfo.RewardType = (byte)Status[i]["RewardType_b"].n;
                tmpInfo.RewardId01 = (uint)Status[i]["RewardId01_ui"].n;
                tmpInfo.RewardId02 = (uint)Status[i]["RewardId02_ui"].n;
                tmpInfo.RewardId03 = (uint)Status[i]["RewardId03_ui"].n;
                tmpInfo.NextId = (uint)Status[i]["NextId_ui"].n;

                StatusInfoList.Add(tmpInfo);
            }
        }

    }


    public class CharacterInfoChecker
	{

		public class ReferenceTable
		{
			public string name;
			public string field;
		}

		public string 	name;
		public string 	datatype;
		public bool 	isunique;
		public Dictionary<int,ReferenceTable> referenceTable = new Dictionary<int, ReferenceTable>();
	}



	public class CheckerHelper
	{
		public string tableName;
		public Dictionary<string,CharacterInfoChecker> checkerInfoDic = new Dictionary<string, CharacterInfoChecker>();
	}

	CheckerHelper checkHelper;

	public void LoadLowDataForTableCheck()
	{
		parseCheckDic ();

		TextAsset data = Resources.Load("TestJson/"+checkHelper.tableName, typeof(TextAsset)) as TextAsset;
		StringReader sr = new StringReader(data.text);
		string strSrc = sr.ReadToEnd();
		JSONObject Character = new JSONObject(strSrc);
		
		for (int i = 0; i < Character.list.Count; i++) {
			//CharacterInfo tmpInfo = new CharacterInfo ();

			foreach(string s in  checkHelper.checkerInfoDic.Keys){
				if (Character[i].HasField(s)){

					// 자료형에 따른 데이터 유효성체크이나 별 의미가 없어서 주석처리
					//int 형이라고 지정되어 있다고해도 int가 들어오는지 아닌지 알수가 없다.
					/*
					if (checkerInfoDic[s].datatype == "unsigned int"){
						if (!Character[i][s].IsNumber){
							Debug.LogError("["+i+"],"+ s + " is not number");
						}
					}
					else if (checkerInfoDic[s].datatype == "byte"){
						if (!Character[i][s].IsNumber){
							Debug.LogError("["+i+"],"+ s + " is not number");
						}
					}
					else if (checkerInfoDic[s].datatype == "string"){
						if (Character[i][s].IsNull){
							Debug.LogError("["+i+"],"+ s + " is null");
						}
					}
					else if (checkerInfoDic[s].datatype == "float"){
						if (Character[i][s].IsNull){
							Debug.LogError("["+i+"],"+ s + " is null");
						}
					}
					else{
						Debug.Log(" checkerInfoDic[s].datatype :"+checkerInfoDic[s].datatype);
					}
					*/

					//Debug.Log("check field : "+s);

					// 상호참조하는 ID 체크
					for(int k=0; k < checkHelper.checkerInfoDic[s].referenceTable.Count; k++){

						// 이 테이블에서의 ID
						uint checkID1 = (uint)Character[i][s].n;

						CharacterInfoChecker.ReferenceTable refTable = checkHelper.checkerInfoDic[s].referenceTable[k];

						//Debug.Log("check field > "+s+" : "+checkID1+" || ref table: "+refTable.name+" ["+refTable.field+"]");
						// //check field > NameId_ui : 1 || ref table: Local_StringUnit [StringId_ui]

						JSONObject StringUnitSub = _LowDataMgr.instance.getJsonObj(refTable.name);

						// find id
						int idCnt = 0;
						for (int m = 0; m < StringUnitSub.list.Count; m++)
						{
							if (StringUnitSub[m].HasField(refTable.field)){

								uint checkID2 = (uint)StringUnitSub[m][refTable.field].n;

								if ( checkID1 == checkID2){
									idCnt++;
									//Debug.Log(refTable.name+"["+refTable.field+"] has "+checkID2+", OK");
									// Local_StringUnit[StringId_ui] has 1, OK
								}
							}
							else{
								// 참조해야할 테이블에서 해당 필드가 없다. error
								Debug.LogError(refTable.name +" dont have "+refTable.field+" field");
							}
						}

						if (idCnt == 0){
							Debug.LogError(" reference table error, table:"+refTable.name+", field:"+refTable.field+", not found id:"+checkID1);
						}
					}
				}
			}
		}

//		////// ref table check
//		for (int i=0; i<checkerInfoDic.Count; i++) {
//
//			checkerInfoDic[i].name
//			for(int j=0; j < checkerInfoDic[i].referenceTable.Count;j++){
//
//				string refTableField = checkerInfoDic[i].referenceTable[j].field;
//				string refTableName = checkerInfoDic[i].referenceTable[j].name;
//
//				TextAsset data2 = Resources.Load("TestJson/"+refTableName, typeof(TextAsset)) as TextAsset;
//				StringReader sr2 = new StringReader(data2.text);
//				string strSrc2 = sr2.ReadToEnd();
//				JSONObject StringUnit2 = new JSONObject(strSrc2);
//
//				for (int k = 0; k < StringUnit2.list.Count; k++)
//				{
//					if (StringUnit2[k].HasField(refTableField)){
//						StringUnit2[k][refTableField].n
//					}
//					tmpInfo.StringId = (uint)StringUnit2[k]["StringId_ui"].n;
//				}
//
//
//			}
//		}

		Debug.Log (checkHelper.tableName+" check finish");
/*
			TextAsset data = Resources.Load("TestJson/Local_StringUnit", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringUnit = new JSONObject(strSrc);

            for (int i = 0; i < StringUnit.list.Count; i++)
            {
                StringUnitInfo tmpInfo = new StringUnitInfo();
                tmpInfo.StringId = (uint)StringUnit[i]["StringId_ui"].n;
 */




	}

	void parseCheckDic(){

		checkHelper = new CheckerHelper ();

		TextAsset data = Resources.Load ("TestJson_define/def-Character_Character", typeof(TextAsset)) as TextAsset;
		StringReader sr = new StringReader (data.text);
		string strSrc = sr.ReadToEnd ();
		JSONObject Character = new JSONObject (strSrc);
		
		checkHelper.tableName = Character ["tablename"].str;

		Debug.Log (" table check : " + checkHelper.tableName);
		
		List<JSONObject> jsonList = Character ["tablefields"].list;
		
		for (int i = 0; i < jsonList.Count ; i++) {
			
			CharacterInfoChecker checker = new CharacterInfoChecker();
			checker.name = jsonList[i]["name"].str; 
			checker.datatype = jsonList[i]["datatype"].str;
			checker.isunique = jsonList[i]["isunique"].b;
			
			if (jsonList[i].HasField("ref_tables")){
				
				List<JSONObject> refTableList = jsonList[i]["ref_tables"].list;
				
				for (int j = 0; j < refTableList.Count ; j++) {
					
					CharacterInfoChecker.ReferenceTable refTable = new CharacterInfoChecker.ReferenceTable();
					refTable.name = refTableList[j]["tableName"].str;
					refTable.field = refTableList[j]["tableField"].str;
					checker.referenceTable.Add(j, refTable);
				}
			}
			checkHelper.checkerInfoDic.Add(checker.name, checker);
		}
	}

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/CharacterInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, CharacterInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StatusInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StatusInfoList);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
        //CharacterInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, CharacterInfo>>("CharacterInfo");
        //StatusInfoList = _LowDataMgr.instance.DeserializeData<List<StatusInfo>>("StatusInfo");

        _LowDataMgr.instance.DeserializeData<Dictionary<uint, CharacterInfo>>("CharacterInfo", (data) => {
            CharacterInfoDic = data;
        });

        _LowDataMgr.instance.DeserializeData<List<StatusInfo>>("StatusInfo", (data) => {
            StatusInfoList = data;
        });
    }
}
