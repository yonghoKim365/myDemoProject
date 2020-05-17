using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Partner
{
    [Serializable]
    public class PartnerDataInfo
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
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        uint _AniId;
        public uint AniId
        {
            set { _AniId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniId); }
        }
        public string prefab;
        public string LeftWeaDummy;
        public string RightWeaDummy;
        public string PortraitId;
        byte _AiType;
        public byte AiType
        {
            set { _AiType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_AiType); }
        }
        byte _Quality;
        public byte Quality
        {
            set { _Quality = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Quality); }
        }
        uint _ShardIdx;
        public uint ShardIdx
        {
            set { _ShardIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ShardIdx); }
        }
        ushort _NeedShardValue;
        public ushort NeedShardValue
        {
            set { _NeedShardValue = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_NeedShardValue); }
        }
        ushort _PaybackValue;
        public ushort PaybackValue
        {
            set { _PaybackValue = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_PaybackValue); }
        }
        uint _QualityUpItem;
        public uint QualityUpItem
        {
            set { _QualityUpItem = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QualityUpItem); }
        }
        uint _QualityUpItemCount;
        public uint QualityUpItemCount
        {
            set { _QualityUpItemCount = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QualityUpItemCount); }
        }
        uint _QualityUpNeedGold;
        public uint QualityUpNeedGold
        {
            set { _QualityUpNeedGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QualityUpNeedGold); }
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
        uint _skill0;
        public uint skill0
        {
            set { _skill0 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skill0); }
        }
        uint _skill1;
        public uint skill1
        {
            set { _skill1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skill1); }
        }
        uint _skill2;
        public uint skill2
        {
            set { _skill2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skill2); }
        }
        uint _skill3;
        public uint skill3
        {
            set { _skill3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skill3); }
        }
        uint _skill4;
        public uint skill4
        {
            set { _skill4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skill4); }
        }
        uint _QualityUpId;
        public uint QualityUpId
        {
            set { _QualityUpId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QualityUpId); }
        }
    }


    public Dictionary<uint, PartnerDataInfo> PartnerDataInfoDic = new Dictionary<uint, PartnerDataInfo>();

    [Serializable]
    public class PartnerScaleInfo
    {
        public uint Id;
        public string prefab;
        public float _x;
        public float _y;
        public float rotate_x;
        public float rotate_y;
        public string panel_name;
        public float scale;
    }
    public List<PartnerScaleInfo> PartnerScaleInfoList = new List<PartnerScaleInfo>();
    
    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Partner_PartnerData", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject PartnerData = new JSONObject(strSrc);

            for (int i = 0; i < PartnerData.list.Count; i++)
            {
                PartnerDataInfo tmpInfo = new PartnerDataInfo();
                tmpInfo.Id = (uint)PartnerData[i]["Id_ui"].n;
                tmpInfo.NameId = (uint)PartnerData[i]["NameId_ui"].n;
                tmpInfo.DescriptionId = (uint)PartnerData[i]["DescriptionId_ui"].n;
                tmpInfo.Type = (byte)PartnerData[i]["Type_b"].n;
                tmpInfo.Class = (byte)PartnerData[i]["Class_b"].n;
                tmpInfo.AniId = (uint)PartnerData[i]["AniId_ui"].n;
                tmpInfo.prefab = PartnerData[i]["prefab_c"].str;
                tmpInfo.LeftWeaDummy = PartnerData[i]["LeftWeaDummy_c"].str;
                tmpInfo.RightWeaDummy = PartnerData[i]["RightWeaDummy_c"].str;
                tmpInfo.PortraitId = PartnerData[i]["PortraitId_c"].str;
                tmpInfo.AiType = (byte)PartnerData[i]["AiType_b"].n;
                tmpInfo.Quality = (byte)PartnerData[i]["Quality_b"].n;
                tmpInfo.ShardIdx = (uint)PartnerData[i]["ShardIdx_ui"].n;
                tmpInfo.NeedShardValue = (ushort)PartnerData[i]["NeedShardValue_us"].n;
                tmpInfo.PaybackValue = (ushort)PartnerData[i]["PaybackValue_us"].n;
                tmpInfo.QualityUpItem = (uint)PartnerData[i]["QualityUpItem_ui"].n;
                tmpInfo.QualityUpItemCount = (uint)PartnerData[i]["QualityUpItemCount_ui"].n;
                tmpInfo.QualityUpNeedGold = (uint)PartnerData[i]["QualityUpNeedGold_ui"].n;
                tmpInfo.Speed = (byte)PartnerData[i]["Speed_b"].n;
                tmpInfo.Weight = (float)PartnerData[i]["Weight_f"].n;
                tmpInfo.BaseHp = (uint)PartnerData[i]["BaseHp_ui"].n;
                tmpInfo.BaseAtk = (uint)PartnerData[i]["BaseAtk_ui"].n;
                tmpInfo.BaseHit = (uint)PartnerData[i]["BaseHit_ui"].n;
                tmpInfo.BaseAvoid = (uint)PartnerData[i]["BaseAvoid_ui"].n;
                tmpInfo.BaseCriticalRate = (uint)PartnerData[i]["BaseCriticalRate_ui"].n;
                tmpInfo.BaseCriticalResist = (uint)PartnerData[i]["BaseCriticalResist_ui"].n;
                tmpInfo.BaseCriticalDamage = (uint)PartnerData[i]["BaseCriticalDamage_ui"].n;
                tmpInfo.BaseLifeSteal = (uint)PartnerData[i]["BaseLifeSteal_ui"].n;
                tmpInfo.BaseIgnoreAtk = (uint)PartnerData[i]["BaseIgnoreAtk_ui"].n;
                tmpInfo.BaseDamageDown = (uint)PartnerData[i]["BaseDamageDown_ui"].n;
                tmpInfo.BaseDamageDownRate = (uint)PartnerData[i]["BaseDamageDownRate_ui"].n;
                tmpInfo.BaseSuperArmor = (uint)PartnerData[i]["BaseSuperArmor_ui"].n;
                tmpInfo.SuperArmorRecoveryTime = (uint)PartnerData[i]["SuperArmorRecoveryTime_ui"].n;
                tmpInfo.SuperArmorRecoveryRate = (uint)PartnerData[i]["SuperArmorRecoveryRate_ui"].n;
                tmpInfo.SuperArmorRecovery = (uint)PartnerData[i]["SuperArmorRecovery_ui"].n;
                tmpInfo.LevelUpHp = (uint)PartnerData[i]["LevelUpHp_ui"].n;
                tmpInfo.LevelUpAtk = (uint)PartnerData[i]["LevelUpAtk_ui"].n;
                tmpInfo.LevelAvoidRate = (uint)PartnerData[i]["LevelAvoidRate_ui"].n;
                tmpInfo.LevelHitRate = (uint)PartnerData[i]["LevelHitRate_ui"].n;
                tmpInfo.LevelUpSuperArmor = (uint)PartnerData[i]["LevelUpSuperArmor_ui"].n;
                tmpInfo.skill0 = (uint)PartnerData[i]["skill0_ui"].n;
                tmpInfo.skill1 = (uint)PartnerData[i]["skill1_ui"].n;
                tmpInfo.skill2 = (uint)PartnerData[i]["skill2_ui"].n;
                tmpInfo.skill3 = (uint)PartnerData[i]["skill3_ui"].n;
                tmpInfo.skill4 = (uint)PartnerData[i]["skill4_ui"].n;
                tmpInfo.QualityUpId = (uint)PartnerData[i]["QualityUpId_ui"].n;

                PartnerDataInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }



        {
            TextAsset data = Resources.Load("TestJson/Partner_PartnerScale", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject PartnerScale = new JSONObject(strSrc);

            for (int i = 0; i < PartnerScale.list.Count; i++)
            {
                PartnerScaleInfo tmpInfo = new PartnerScaleInfo();
                tmpInfo.Id = (uint)PartnerScale[i]["Id_ui"].n;
                tmpInfo.prefab = PartnerScale[i]["prefab_c"].str;
                tmpInfo._x = (float)PartnerScale[i]["x_pos_f"].n;
                tmpInfo._y = (float)PartnerScale[i]["y_pos_f"].n;
                tmpInfo.rotate_x = (float)PartnerScale[i]["x_rotate_pos_f"].n;
                tmpInfo.rotate_y = (float)PartnerScale[i]["y_rotate_pos_f"].n;
                tmpInfo.scale = (float)PartnerScale[i]["scale_f"].n;
                tmpInfo.panel_name = PartnerScale[i]["panel_name"].str;

                PartnerScaleInfoList.Add(tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PartnerDataInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PartnerDataInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/PartnerScaleInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, PartnerScaleInfoList);
            fs.Close();
        }


    }

    public void DeserializeData()
    {
//        PartnerDataInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, PartnerDataInfo>>("PartnerDataInfo");
//        PartnerScaleInfoList = _LowDataMgr.instance.DeserializeData<List<PartnerScaleInfo>>("PartnerScaleInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, PartnerDataInfo>>("PartnerDataInfo", (data) => { PartnerDataInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<PartnerScaleInfo>>("PartnerScaleInfo", (data) => { PartnerScaleInfoList = data; });

    }
}
