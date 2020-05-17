using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Mob
{
    [Serializable]
    public class MobInfo
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
        byte _AttackType;
        public byte AttackType
        {
            set { _AttackType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AttackType); }
        }
        public string prefab;
        uint _AniId;
        public uint AniId
        {
            set { _AniId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniId); }
        }
        public string PortraitId;
        byte _AiType;
        public byte AiType
        {
            set { _AiType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AiType); }
        }
        public float Scale;
        ushort _Pattenid;
        public ushort Pattenid
        {
            set { _Pattenid = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_Pattenid); }
        }
        byte _Level;
        public byte Level
        {
            set { _Level = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Level); }
        }
        public JsonCustomData skill;
        public JsonCustomData SkillLevel;
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
        uint _LevelupDamageDown;
        public uint LevelupDamageDown
        {
            set { _LevelupDamageDown = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_LevelupDamageDown); }
        }
        public float LevelupDamageDownRate;
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
    }


    [Serializable]
    public class PattenInfo
    {
        public ushort id;
        byte _Typerank;
        public byte Typerank
        {
            set { _Typerank = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Typerank); }
        }
        uint _MainType;
        public uint MainType
        {
            set { _MainType = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MainType); }
        }
        byte _SubType;
        public byte SubType
        {
            set { _SubType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_SubType); }
        }
        ushort _TypeValue;
        public ushort TypeValue
        {
            set { _TypeValue = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_TypeValue); }
        }
        byte _ActionType;
        public byte ActionType
        {
            set { _ActionType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ActionType); }
        }
        uint _Actionvalue;
        public uint Actionvalue
        {
            set { _Actionvalue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Actionvalue); }
        }
    }

    public List<PattenInfo> PattenInfoList = new List<PattenInfo>();
    public Dictionary<uint, MobInfo> MobInfoDic = new Dictionary<uint, MobInfo>();

    [Serializable]
    public class PropInfo
    {
        public uint Id;
        uint _Name;
        public uint Name
        {
            set { _Name = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Name); }
        }
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _DestroyType;
        public byte DestroyType
        {
            set { _DestroyType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DestroyType); }
        }
        byte _AiType;
        public byte AiType
        {
            set { _AiType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AiType); }
        }
        byte _AutoTarget;
        public byte AutoTarget
        {
            set { _AutoTarget = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AutoTarget); }
        }
        uint _RewardItem;
        public uint RewardItem
        {
            set { _RewardItem = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardItem); }
        }
        uint _RewardEXP;
        public uint RewardEXP
        {
            set { _RewardEXP = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardEXP); }
        }
        uint _RewardGold;
        public uint RewardGold
        {
            set { _RewardGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RewardGold); }
        }
        uint _CallSkill;
        public uint CallSkill
        {
            set { _CallSkill = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CallSkill); }
        }
        public string Prefab;
        uint _ResourceUnitId;
        public uint ResourceUnitId
        {
            set { _ResourceUnitId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ResourceUnitId); }
        }
        byte _Speed;
        public byte Speed
        {
            set { _Speed = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Speed); }
        }
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
    }
    public Dictionary<uint, PropInfo> PropInfoDic = new Dictionary<uint, PropInfo>();

    [Serializable]
    public class PropGroupInfo
    {
        public uint PropGroupId;
        public JsonCustomData propIdx;
        public JsonCustomData propRate;
    }
    public Dictionary<uint, PropGroupInfo> PropGroupInfoDic = new Dictionary<uint, PropGroupInfo>();


    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Mob_Mob", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Mob = new JSONObject(strSrc);

            for (int i = 0; i < Mob.list.Count; i++)
            {
                MobInfo tmpInfo = new MobInfo();
                tmpInfo.Id = (uint)Mob[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)Mob[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)Mob[i]["DescriptionId_ui"].n;
                tmpInfo.Class = (byte)Mob[i]["Class_b"].n;
                tmpInfo.AttackType = (byte)Mob[i]["AttackType_b"].n;
                tmpInfo.prefab = Mob[i]["prefab_c"].str;
                tmpInfo.AniId = (uint)Mob[i]["AniId_ui"].n;
                tmpInfo.PortraitId = Mob[i]["PortraitId_c"].str;
                tmpInfo.AiType = (byte)Mob[i]["AiType_b"].n;
                tmpInfo.Scale = (float)Mob[i]["Scale_f"].n;
                tmpInfo.Pattenid = (ushort)Mob[i]["Pattenid_us"].n;
                tmpInfo.Level = (byte)Mob[i]["Level_b"].n;
                tmpInfo.skill = new JsonCustomData(Mob[i]["skill_j"].ToString());
                tmpInfo.SkillLevel = new JsonCustomData(Mob[i]["SkillLevel_j"].ToString());
                tmpInfo.Speed = (byte)Mob[i]["Speed_b"].n;
                tmpInfo.Weight = (float)Mob[i]["Weight_f"].n;
                tmpInfo.BaseHp = (uint)Mob[i]["BaseHp_ui"].n;
                tmpInfo.BaseAtk = (uint)Mob[i]["BaseAtk_ui"].n;
                tmpInfo.BaseHit = (uint)Mob[i]["BaseHit_ui"].n;
                tmpInfo.BaseAvoid = (uint)Mob[i]["BaseAvoid_ui"].n;
                tmpInfo.BaseCriticalRate = (uint)Mob[i]["BaseCriticalRate_ui"].n;
                tmpInfo.BaseCriticalResist = (uint)Mob[i]["BaseCriticalResist_ui"].n;
                tmpInfo.BaseCriticalDamage = (uint)Mob[i]["BaseCriticalDamage_ui"].n;
                tmpInfo.BaseLifeSteal = (uint)Mob[i]["BaseLifeSteal_ui"].n;
                tmpInfo.BaseIgnoreAtk = (uint)Mob[i]["BaseIgnoreAtk_ui"].n;
                tmpInfo.BaseDamageDown = (uint)Mob[i]["BaseDamageDown_ui"].n;
                tmpInfo.BaseDamageDownRate = (uint)Mob[i]["BaseDamageDownRate_ui"].n;
                tmpInfo.LevelUpHp = (uint)Mob[i]["LevelUpHp_ui"].n;
                tmpInfo.LevelUpAtk = (uint)Mob[i]["LevelUpAtk_ui"].n;
                tmpInfo.LevelAvoidRate = (uint)Mob[i]["LevelAvoidRate_ui"].n;
                tmpInfo.LevelHitRate = (uint)Mob[i]["LevelHitRate_ui"].n;
                tmpInfo.LevelupDamageDown = (uint)Mob[i]["LevelupDamageDown_ui"].n;
                tmpInfo.LevelupDamageDownRate = (float)Mob[i]["LevelupDamageDownRate_f"].n;
                tmpInfo.BaseSuperArmor = (uint)Mob[i]["BaseSuperArmor_ui"].n;
                tmpInfo.SuperArmorRecoveryTime = (uint)Mob[i]["SuperArmorRecoveryTime_ui"].n;
                tmpInfo.SuperArmorRecoveryRate = (uint)Mob[i]["SuperArmorRecoveryRate_ui"].n;
                tmpInfo.SuperArmorRecovery = (uint)Mob[i]["SuperArmorRecovery_ui"].n;

                MobInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Mob_Patten", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Patten = new JSONObject(strSrc);

            for (int i = 0; i < Patten.list.Count; i++)
            {
                PattenInfo tmpInfo = new PattenInfo();
                tmpInfo.id = (ushort)Patten[i]["id_us"].n;
                tmpInfo.Typerank = (byte)Patten[i]["Typerank_b"].n;
                tmpInfo.MainType = (uint)Patten[i]["MainType_ui"].n;
                tmpInfo.SubType = (byte)Patten[i]["SubType_b"].n;
                tmpInfo.TypeValue = (ushort)Patten[i]["TypeValue_us"].n;
                tmpInfo.ActionType = (byte)Patten[i]["ActionType_b"].n;
                tmpInfo.Actionvalue = (uint)Patten[i]["Actionvalue_ui"].n;

                PattenInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Mob_Prop", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Prop = new JSONObject(strSrc);

            for (int i = 0; i < Prop.list.Count; i++)
            {
                PropInfo tmpInfo = new PropInfo();
                tmpInfo.Id = (uint)Prop[i]["Id_ui"].n;
                tmpInfo.Name = (uint)Prop[i]["Name_ui"].n;
                tmpInfo.Type = (byte)Prop[i]["Type_b"].n;
                tmpInfo.DestroyType = (byte)Prop[i]["DestroyType_b"].n;
                tmpInfo.AiType = (byte)Prop[i]["AiType_b"].n;
                tmpInfo.AutoTarget = (byte)Prop[i]["AutoTarget_b"].n;
                tmpInfo.RewardItem = (uint)Prop[i]["RewardItem_ui"].n;
                tmpInfo.RewardEXP = (uint)Prop[i]["RewardEXP_ui"].n;
                tmpInfo.RewardGold = (uint)Prop[i]["RewardGold_ui"].n;
                tmpInfo.CallSkill = (uint)Prop[i]["CallSkill_ui"].n;
                tmpInfo.Prefab = Prop[i]["Prefab_c"].str;
                tmpInfo.ResourceUnitId = (uint)Prop[i]["ResourceUnitId_ui"].n;
                tmpInfo.Speed = (byte)Prop[i]["Speed_b"].n;
                tmpInfo.BaseHp = (uint)Prop[i]["BaseHp_ui"].n;
                tmpInfo.BaseAtk = (uint)Prop[i]["BaseAtk_ui"].n;
                tmpInfo.BaseHit = (uint)Prop[i]["BaseHit_ui"].n;
                tmpInfo.BaseAvoid = (uint)Prop[i]["BaseAvoid_ui"].n;
                tmpInfo.BaseCriticalRate = (uint)Prop[i]["BaseCriticalRate_ui"].n;
                tmpInfo.BaseCriticalResist = (uint)Prop[i]["BaseCriticalResist_ui"].n;
                tmpInfo.BaseCriticalDamage = (uint)Prop[i]["BaseCriticalDamage_ui"].n;
                tmpInfo.BaseLifeSteal = (uint)Prop[i]["BaseLifeSteal_ui"].n;
                tmpInfo.BaseIgnoreAtk = (uint)Prop[i]["BaseIgnoreAtk_ui"].n;
                tmpInfo.BaseDamageDown = (uint)Prop[i]["BaseDamageDown_ui"].n;
                tmpInfo.BaseDamageDownRate = (uint)Prop[i]["BaseDamageDownRate_ui"].n;

                PropInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Mob_PropGroup", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject PropGroup = new JSONObject(strSrc);

            for (int i = 0; i < PropGroup.list.Count; i++)
            {
                PropGroupInfo tmpInfo = new PropGroupInfo();
                tmpInfo.PropGroupId = (uint)PropGroup[i]["PropGroupId_ui"].n;
                tmpInfo.propIdx = new JsonCustomData(PropGroup[i]["propIdx_j"].ToString());
                tmpInfo.propRate = new JsonCustomData(PropGroup[i]["propRate_j"].ToString());

                PropGroupInfoDic.Add(tmpInfo.PropGroupId, tmpInfo);
            }
        }



    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/MobInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, MobInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PattenInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PattenInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PropInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PropInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PropGroupInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PropGroupInfoDic);
            fs.Close();
        }

    }

	public void DeserializeData()
	{
		//        MobInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, MobInfo>>("MobInfo");
		//        PattenInfoList = _LowDataMgr.instance.DeserializeData<List<PattenInfo>>("PattenInfo");
		//        PropInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, PropInfo>>("PropInfo");
		
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, MobInfo>>("MobInfo", (data) => { MobInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<PattenInfo>>("PattenInfo", (data) => { PattenInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, PropInfo>>("PropInfo", (data) => { PropInfoDic = data; });
		
	}
}
