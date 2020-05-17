
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SkillTables
{
    [Serializable]
    public class AbilityInfo
    {
        public uint Idx;
        ushort _notiIdx;
        public ushort notiIdx
        {
            set { _notiIdx = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_notiIdx); }
        }
        public string targetEffect;
        byte _skillType;
        public byte skillType
        {
            set { _skillType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_skillType); }
        }
        byte _applyTarget;
        public byte applyTarget
        {
            set { _applyTarget = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_applyTarget); }
        }
        byte _eCount;
        public byte eCount
        {
            set { _eCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_eCount); }
        }
        byte _targetCount;
        public byte targetCount
        {
            set { _targetCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_targetCount); }
        }
        byte _rangeType;
        public byte rangeType
        {
            set { _rangeType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_rangeType); }
        }
        ushort _radius;
        public ushort radius
        {
            set { _radius = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_radius); }
        }
        ushort _angle;
        public ushort angle
        {
            set { _angle = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_angle); }
        }
        public float rangeCenter;
        public float rangeRotation;
        byte _baseFactor;
        public byte baseFactor
        {
            set { _baseFactor = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_baseFactor); }
        }
        public float eventValue;
        ushort _availableCnt;
        public ushort availableCnt
        {
            set { _availableCnt = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_availableCnt); }
        }
        uint _targetSound;
        public uint targetSound
        {
            set { _targetSound = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_targetSound); }
        }
        byte _cameraShake;
        public byte cameraShake
        {
            set { _cameraShake = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_cameraShake); }
        }
        public float pushTime;
        public float pushpower;
        public float diePower;
		public float stiffenTime;
        public float dieMinDistance;
        public float dieMaxDistance;
    }
    public List<AbilityInfo> AbilityInfoList = new List<AbilityInfo>();

    [Serializable]
    public class ActionInfo
    {
        public uint idx;
        uint _name;
        public uint name
        {
            set { _name = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_name); }
        }
        uint _descrpition;
        public uint descrpition
        {
            set { _descrpition = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_descrpition); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        public float GlobalCooltime;
        ushort _range;
        public ushort range
        {
            set { _range = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_range); }
        }
        ushort _effectCallNotiIdx;
        public ushort effectCallNotiIdx
        {
            set { _effectCallNotiIdx = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_effectCallNotiIdx); }
        }
        byte _needtarget;
        public byte needtarget
        {
            set { _needtarget = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_needtarget); }
        }
        byte _camera;
        public byte camera
        {
            set { _camera = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_camera); }
        }
        byte _skillpass;
        public byte skillpass
        {
            set { _skillpass = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_skillpass); }
        }
        public float casttime;
        public float hittime;
        public float mergetime;
        public float watingtime;
        byte _pabody;
        public byte pabody
        {
            set { _pabody = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_pabody); }
        }
        byte _emit;
        public byte emit
        {
            set { _emit = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_emit); }
        }
    }
    public Dictionary<uint, ActionInfo> ActionInfoDic = new Dictionary<uint, ActionInfo>();

    [Serializable]
    public class BuffInfo
    {
        public uint Indx;
        uint _name;
        public uint name
        {
            set { _name = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_name); }
        }
        uint _descrpition;
        public uint descrpition
        {
            set { _descrpition = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_descrpition); }
        }
        public string icon;
        public string effect;
        public string effectBreak;
        byte _buffcategory;
        public byte buffcategory
        {
            set { _buffcategory = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_buffcategory); }
        }
        byte _buffAbility;
        public byte buffAbility
        {
            set { _buffAbility = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_buffAbility); }
        }
        byte _buffType;
        public byte buffType
        {
            set { _buffType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_buffType); }
        }
        byte _buffGrade;
        public byte buffGrade
        {
            set { _buffGrade = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_buffGrade); }
        }
        byte _startType;
        public byte startType
        {
            set { _startType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_startType); }
        }
        public string CastingEffect;
        byte _baseFactor;
        public byte baseFactor
        {
            set { _baseFactor = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_baseFactor); }
        }
        byte _valueType;
        public byte valueType
        {
            set { _valueType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_valueType); }
        }
        uint _factorRate;
        public uint factorRate
        {
            set { _factorRate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_factorRate); }
        }
        uint _bufflink;
        public uint bufflink
        {
            set { _bufflink = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_bufflink); }
        }
        byte _overLapCount;
        public byte overLapCount
        {
            set { _overLapCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_overLapCount); }
        }
        public float tic;
        uint _buffDisplay;
        public uint buffDisplay
        {
            set { _buffDisplay = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_buffDisplay); }
        }
        byte _addBuffType;
        public byte addBuffType
        {
            set { _addBuffType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_addBuffType); }
        }
    }
    public Dictionary<uint, BuffInfo> BuffInfoDic = new Dictionary<uint, BuffInfo>();

    [Serializable]
    public class BuffGroupInfo
    {
        public uint LogIdx;
        byte _partnerType;
        public byte partnerType
        {
            set { _partnerType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_partnerType); }
        }
        uint _Indx;
        public uint Indx
        {
            set { _Indx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Indx); }
        }
        public float buffRate;
        byte _payType;
        public byte payType
        {
            set { _payType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_payType); }
        }
        uint _cost;
        public uint cost
        {
            set { _cost = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_cost); }
        }
        byte _resetBuff;
        public byte resetBuff
        {
            set { _resetBuff = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_resetBuff); }
        }
    }
    public List<BuffGroupInfo> BuffGroupInfoList = new List<BuffGroupInfo>();

    [Serializable]
    public class ProjectTileInfo
    {
        public uint idx;
        public string prefab;
        uint _colideSound;
        public uint colideSound
        {
            set { _colideSound = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_colideSound); }
        }
        public string colideEffect;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        public float moveSpeed;
        public float maxDistance;
        public float durationTime;
        ushort _radius;
        public ushort radius
        {
            set { _radius = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_radius); }
        }
        byte _penetrate;
        public byte penetrate
        {
            set { _penetrate = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_penetrate); }
        }
        byte _penetrateCount;
        public byte penetrateCount
        {
            set { _penetrateCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_penetrateCount); }
        }
        uint _availableCnt;
        public uint availableCnt
        {
            set { _availableCnt = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_availableCnt); }
        }
        public float damageinterval;
        byte _startType;
        public byte startType
        {
            set { _startType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_startType); }
        }
        public float startLocation;
        byte _yLocation;
        public byte yLocation
        {
            set { _yLocation = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_yLocation); }
        }
        public string castDummy;
        uint _callBuffIdx;
        public uint callBuffIdx
        {
            set { _callBuffIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_callBuffIdx); }
        }
        uint _ProjectSound;
        public uint ProjectSound
        {
            set { _ProjectSound = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ProjectSound); }
        }
    }
    public Dictionary<uint, ProjectTileInfo> ProjectTileInfoDic = new Dictionary<uint, ProjectTileInfo>();

    [Serializable]
    public class SkillLevelInfo
    {
        public uint Idx;
        uint _unitIdx;
        public uint unitIdx
        {
            set { _unitIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_unitIdx); }
        }
        uint _skillIdx;
        public uint skillIdx
        {
            set { _skillIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skillIdx); }
        }
        uint _name;
        public uint name
        {
            set { _name = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_name); }
        }
        uint _skilldesc;
        public uint skilldesc
        {
            set { _skilldesc = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skilldesc); }
        }
        uint _icon;
        public uint icon
        {
            set { _icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_icon); }
        }
        uint _skillLevel;
        public uint skillLevel
        {
            set { _skillLevel = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_skillLevel); }
        }
        byte _LimitLv;
        public byte LimitLv
        {
            set { _LimitLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLv); }
        }
        uint _SkillLevelUpStatusId;
        public uint SkillLevelUpStatusId
        {
            set { _SkillLevelUpStatusId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SkillLevelUpStatusId); }
        }
        uint _SkillLevelUpItem1;
        public uint SkillLevelUpItem1
        {
            set { _SkillLevelUpItem1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SkillLevelUpItem1); }
        }
        uint _SkillLevelUpItem1Count;
        public uint SkillLevelUpItem1Count
        {
            set { _SkillLevelUpItem1Count = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SkillLevelUpItem1Count); }
        }
        uint _SkillLevelUpItem2;
        public uint SkillLevelUpItem2
        {
            set { _SkillLevelUpItem2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SkillLevelUpItem2); }
        }
        uint _SkillLevelUpItem2Count;
        public uint SkillLevelUpItem2Count
        {
            set { _SkillLevelUpItem2Count = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SkillLevelUpItem2Count); }
        }
        uint _CostGold;
        public uint CostGold
        {
            set { _CostGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_CostGold); }
        }
        uint _rate;
        public uint rate
        {
            set { _rate = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rate); }
        }
        public float durationTime;
        public JsonCustomData factorRate;
        uint _factor;
        public uint factor
        {
            set { _factor = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_factor); }
        }
        public JsonCustomData SuperArmorDamage;
        public JsonCustomData SuperArmorRecovery;
        public float cooltime;
        uint _callCastingBuffIdx;
        public uint callCastingBuffIdx
        {
            set { _callCastingBuffIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_callCastingBuffIdx); }
        }
        public float CastingBuffDurationTime;
        public JsonCustomData callBuffIdx;
        uint _callAbilityIdx;
        public uint callAbilityIdx
        {
            set { _callAbilityIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_callAbilityIdx); }
        }
        uint _callChainIdx;
        public uint callChainIdx
        {
            set { _callChainIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_callChainIdx); }
        }
        public float callChainTime;
        public JsonCustomData ignoreDef;
    }

    [Serializable]
    public class SkillSetInfo
    {
        public uint Id;
        byte _Class;
        public byte Class
        {
            set { _Class = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Class); }
        }
        byte _Default;
        public byte Default
        {
            set { _Default = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Default); }
        }
        uint _SetName;
        public uint SetName
        {
            set { _SetName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SetName); }
        }
        uint _SetDescriptionId;
        public uint SetDescriptionId
        {
            set { _SetDescriptionId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SetDescriptionId); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        public string AniPath;
        uint _AniId;
        public uint AniId
        {
            set { _AniId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniId); }
        }
        public JsonCustomData skill0;
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
        public JsonCustomData Chain;
    }

    public List<SkillLevelInfo> SkillLevelInfoList = new List<SkillLevelInfo>();
    public Dictionary<uint, SkillSetInfo> SkillSetInfoDic = new Dictionary<uint, SkillSetInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Skill_Ability", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Ability = new JSONObject(strSrc);

            for (int i = 0; i < Ability.list.Count; i++)
            {
                AbilityInfo tmpInfo = new AbilityInfo();
                tmpInfo.Idx = (uint)Ability[i]["Idx_ui"].n;
                tmpInfo.notiIdx = (ushort)Ability[i]["notiIdx_us"].n;
                tmpInfo.targetEffect = Ability[i]["targetEffect_c"].str;
                tmpInfo.skillType = (byte)Ability[i]["skillType_b"].n;
                tmpInfo.applyTarget = (byte)Ability[i]["applyTarget_b"].n;
                tmpInfo.eCount = (byte)Ability[i]["eCount_b"].n;
                tmpInfo.targetCount = (byte)Ability[i]["targetCount_b"].n;
                tmpInfo.rangeType = (byte)Ability[i]["rangeType_b"].n;
                tmpInfo.radius = (ushort)Ability[i]["radius_us"].n;
                tmpInfo.angle = (ushort)Ability[i]["angle_us"].n;
                tmpInfo.rangeCenter = (float)Ability[i]["rangeCenter_f"].n;
                tmpInfo.rangeRotation = (float)Ability[i]["rangeRotation_f"].n;
                tmpInfo.baseFactor = (byte)Ability[i]["baseFactor_b"].n;
                tmpInfo.eventValue = (float)Ability[i]["eventValue_f"].n;
                tmpInfo.availableCnt = (ushort)Ability[i]["availableCnt_us"].n;
                tmpInfo.targetSound = (uint)Ability[i]["targetSound_ui"].n;
                tmpInfo.cameraShake = (byte)Ability[i]["cameraShake_b"].n;
                tmpInfo.pushTime = (float)Ability[i]["pushTime_f"].n;
                tmpInfo.pushpower = (float)Ability[i]["pushpower_f"].n;
                tmpInfo.diePower = (float)Ability[i]["diePower_f"].n;
				tmpInfo.stiffenTime = (float)Ability[i]["stiffenTime_f"].n;
                tmpInfo.dieMinDistance = (float)Ability[i]["dieMinDistance_f"].n;
                tmpInfo.dieMaxDistance = (float)Ability[i]["dieMaxDistance_f"].n;

                AbilityInfoList.Add(tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Skill_Action", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Action = new JSONObject(strSrc);

            for (int i = 0; i < Action.list.Count; i++)
            {
                ActionInfo tmpInfo = new ActionInfo();
                tmpInfo.idx = (uint)Action[i]["idx_ui"].n;
                tmpInfo.name = (uint)Action[i]["name_ui"].n;
                tmpInfo.descrpition = (uint)Action[i]["descrpition_ui"].n;
                tmpInfo.Icon = (uint)Action[i]["Icon_ui"].n;
                tmpInfo.GlobalCooltime = (float)Action[i]["GlobalCooltime_f"].n;
                tmpInfo.range = (ushort)Action[i]["range_us"].n;
                tmpInfo.effectCallNotiIdx = (ushort)Action[i]["effectCallNotiIdx_us"].n;
                tmpInfo.needtarget = (byte)Action[i]["needtarget_b"].n;
                tmpInfo.camera = (byte)Action[i]["camera_b"].n;
                tmpInfo.skillpass = (byte)Action[i]["skillpass_b"].n;
                tmpInfo.casttime = (float)Action[i]["casttime_f"].n;
                tmpInfo.hittime = (float)Action[i]["hittime_f"].n;
                tmpInfo.mergetime = (float)Action[i]["mergetime_f"].n;
                tmpInfo.watingtime = (float)Action[i]["watingtime_f"].n;
                tmpInfo.pabody = (byte)Action[i]["pabody_b"].n;
                tmpInfo.emit = (byte)Action[i]["emit_b"].n;

                ActionInfoDic.Add(tmpInfo.idx, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Skill_Buff", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Buff = new JSONObject(strSrc);

            for (int i = 0; i < Buff.list.Count; i++)
            {
                BuffInfo tmpInfo = new BuffInfo();
                tmpInfo.Indx = (uint)Buff[i]["Indx_ui"].n;
                tmpInfo.name = (uint)Buff[i]["name_ui"].n;
                tmpInfo.descrpition = (uint)Buff[i]["descrpition_ui"].n;
                tmpInfo.icon = Buff[i]["icon_c"].str;
                tmpInfo.effect = Buff[i]["effect_c"].str;
                tmpInfo.effectBreak = Buff[i]["effectBreak_c"].str;
                tmpInfo.buffcategory = (byte)Buff[i]["buffcategory_b"].n;
                tmpInfo.buffAbility = (byte)Buff[i]["buffAbility_b"].n;
                tmpInfo.buffType = (byte)Buff[i]["buffType_b"].n;
                tmpInfo.buffGrade = (byte)Buff[i]["buffGrade_b"].n;
                tmpInfo.startType = (byte)Buff[i]["startType_b"].n;
                tmpInfo.CastingEffect = Buff[i]["CastingEffect_c"].str;
                tmpInfo.baseFactor = (byte)Buff[i]["baseFactor_b"].n;
                tmpInfo.valueType = (byte)Buff[i]["valueType_b"].n;
                tmpInfo.factorRate = (uint)Buff[i]["factorRate_ui"].n;
                tmpInfo.bufflink = (uint)Buff[i]["bufflink_ui"].n;
                tmpInfo.overLapCount = (byte)Buff[i]["overLapCount_b"].n;
                tmpInfo.tic = (float)Buff[i]["tic_f"].n;
                tmpInfo.buffDisplay = (uint)Buff[i]["buffDisplay_ui"].n;
                tmpInfo.addBuffType = (byte)Buff[i]["addBuffType_b"].n;

                BuffInfoDic.Add(tmpInfo.Indx, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Skill_BuffGroup", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject BuffGroup = new JSONObject(strSrc);

            for (int i = 0; i < BuffGroup.list.Count; i++)
            {
                BuffGroupInfo tmpInfo = new BuffGroupInfo();
                tmpInfo.LogIdx = (uint)BuffGroup[i]["LogIdx_ui"].n;
                tmpInfo.partnerType = (byte)BuffGroup[i]["partnerType_b"].n;
                tmpInfo.Indx = (uint)BuffGroup[i]["Indx_ui"].n;
                tmpInfo.buffRate = (float)BuffGroup[i]["buffRate_f"].n;
                tmpInfo.payType = (byte)BuffGroup[i]["payType_b"].n;
                tmpInfo.cost = (uint)BuffGroup[i]["cost_ui"].n;
                tmpInfo.resetBuff = (byte)BuffGroup[i]["resetBuff_b"].n;

                BuffGroupInfoList.Add(tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Skill_ProjectTile", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ProjectTile = new JSONObject(strSrc);

            for (int i = 0; i < ProjectTile.list.Count; i++)
            {
                ProjectTileInfo tmpInfo = new ProjectTileInfo();
                tmpInfo.idx = (uint)ProjectTile[i]["idx_ui"].n;
                tmpInfo.prefab = ProjectTile[i]["prefab_c"].str;
                tmpInfo.colideSound = (uint)ProjectTile[i]["colideSound_ui"].n;
                tmpInfo.colideEffect = ProjectTile[i]["colideEffect_c"].str;
                tmpInfo.Type = (byte)ProjectTile[i]["Type_b"].n;
                tmpInfo.moveSpeed = (float)ProjectTile[i]["moveSpeed_f"].n;
                tmpInfo.maxDistance = (float)ProjectTile[i]["maxDistance_f"].n;
                tmpInfo.durationTime = (float)ProjectTile[i]["durationTime_f"].n;
                tmpInfo.radius = (ushort)ProjectTile[i]["radius_us"].n;
                tmpInfo.penetrate = (byte)ProjectTile[i]["penetrate_b"].n;
                tmpInfo.penetrateCount = (byte)ProjectTile[i]["penetrateCount_b"].n;
                tmpInfo.availableCnt = (ushort)ProjectTile[i]["availableCnt_us"].n;
                tmpInfo.damageinterval = (float)ProjectTile[i]["dmginterval_f"].n;
                tmpInfo.startType = (byte)ProjectTile[i]["startType_b"].n;
                tmpInfo.startLocation = (float)ProjectTile[i]["startLocation_f"].n;
                tmpInfo.yLocation = (byte)ProjectTile[i]["yLocation_b"].n;
                tmpInfo.castDummy = ProjectTile[i]["castDummy_c"].str;
                tmpInfo.callBuffIdx = (uint)ProjectTile[i]["callBuffIdx_ui"].n;
                tmpInfo.ProjectSound = (uint)ProjectTile[i]["ProjectSound_ui"].n;

                ProjectTileInfoDic.Add(tmpInfo.idx, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Skill_SkillLevel", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject SkillLevel = new JSONObject(strSrc);

            for (int i = 0; i < SkillLevel.list.Count; i++)
            {
                SkillLevelInfo tmpInfo = new SkillLevelInfo();
                tmpInfo.Idx = (uint)SkillLevel[i]["Idx_ui"].n;
                tmpInfo.unitIdx = (uint)SkillLevel[i]["unitIdx_ui"].n;
                tmpInfo.skillIdx = (uint)SkillLevel[i]["skillIdx_ui"].n;
                tmpInfo.name = (uint)SkillLevel[i]["name_ui"].n;
                tmpInfo.skilldesc = (uint)SkillLevel[i]["skilldesc_ui"].n;
                tmpInfo.icon = (uint)SkillLevel[i]["icon_ui"].n;
                tmpInfo.skillLevel = (uint)SkillLevel[i]["skillLevel_ui"].n;
                tmpInfo.LimitLv = (byte)SkillLevel[i]["LimitLv_b"].n;
                tmpInfo.SkillLevelUpStatusId = (uint)SkillLevel[i]["SkillLevelUpStatusId_ui"].n;
                tmpInfo.SkillLevelUpItem1 = (uint)SkillLevel[i]["SkillLevelUpItem1_ui"].n;
                tmpInfo.SkillLevelUpItem1Count = (uint)SkillLevel[i]["SkillLevelUpItem1Count_ui"].n;
                tmpInfo.SkillLevelUpItem2 = (uint)SkillLevel[i]["SkillLevelUpItem2_ui"].n;
                tmpInfo.SkillLevelUpItem2Count = (uint)SkillLevel[i]["SkillLevelUpItem2Count_ui"].n;
                tmpInfo.CostGold = (uint)SkillLevel[i]["CostGold_ui"].n;
                tmpInfo.rate = (uint)SkillLevel[i]["rate_ui"].n;
                tmpInfo.durationTime = (float)SkillLevel[i]["durationTime_f"].n;
                tmpInfo.factorRate = new JsonCustomData(SkillLevel[i]["factorRate_j"].ToString());
                tmpInfo.factor = (uint)SkillLevel[i]["factor_ui"].n;
                tmpInfo.SuperArmorDamage = new JsonCustomData(SkillLevel[i]["SuperArmorDamage_j"].ToString());
                tmpInfo.SuperArmorRecovery = new JsonCustomData(SkillLevel[i]["SuperArmorRecovery_j"].ToString());
                tmpInfo.cooltime = (float)SkillLevel[i]["cooltime_f"].n;
                tmpInfo.callCastingBuffIdx = (uint)SkillLevel[i]["callCastingBuffIdx_ui"].n;
                tmpInfo.CastingBuffDurationTime = (float)SkillLevel[i]["CastingBuffDurationTime_f"].n;
                tmpInfo.callBuffIdx = new JsonCustomData(SkillLevel[i]["callBuffIdx_j"].ToString());
                tmpInfo.callAbilityIdx = (uint)SkillLevel[i]["callAbilityIdx_ui"].n;
                tmpInfo.callChainIdx = (uint)SkillLevel[i]["callChainIdx_ui"].n;
                tmpInfo.callChainTime = (float)SkillLevel[i]["callChainTime_f"].n;
                tmpInfo.ignoreDef = new JsonCustomData(SkillLevel[i]["ignoreDef_j"].ToString());

                SkillLevelInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Skill_SkillSet", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject SkillSet = new JSONObject(strSrc);

            for (int i = 0; i < SkillSet.list.Count; i++)
            {
                SkillSetInfo tmpInfo = new SkillSetInfo();
                tmpInfo.Id = (uint)SkillSet[i]["Id_ui"].n;
                tmpInfo.Class = (byte)SkillSet[i]["Class_b"].n;
                tmpInfo.Default = (byte)SkillSet[i]["Default_b"].n;
                tmpInfo.SetName = (uint)SkillSet[i]["SetName_ui"].n;
                tmpInfo.SetDescriptionId = (uint)SkillSet[i]["SetDescriptionId_ui"].n;
                tmpInfo.Icon = (uint)SkillSet[i]["Icon_ui"].n;
                tmpInfo.AniPath = SkillSet[i]["AniPath_c"].str;
                tmpInfo.AniId = (uint)SkillSet[i]["AniId_ui"].n;
                tmpInfo.skill0 = new JsonCustomData(SkillSet[i]["skill0_j"].ToString());
                tmpInfo.skill1 = (uint)SkillSet[i]["skill1_ui"].n;
                tmpInfo.skill2 = (uint)SkillSet[i]["skill2_ui"].n;
                tmpInfo.skill3 = (uint)SkillSet[i]["skill3_ui"].n;
                tmpInfo.skill4 = (uint)SkillSet[i]["skill4_ui"].n;
                tmpInfo.Chain = new JsonCustomData(SkillSet[i]["Chain_j"].ToString());

                SkillSetInfoDic.Add(tmpInfo.Id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/AbilityInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, AbilityInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ActionInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ActionInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/BuffInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, BuffInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/BuffGroupInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, BuffGroupInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ProjectTileInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ProjectTileInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SkillLevelInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SkillLevelInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SkillSetInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SkillSetInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
		_LowDataMgr.instance.DeserializeData<List<AbilityInfo>>("AbilityInfo", (data) => { AbilityInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, ActionInfo>>("ActionInfo", (data) => {ActionInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, BuffInfo>>("BuffInfo", (data) => { BuffInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<BuffGroupInfo>>("BuffGroupInfo", (data) => { BuffGroupInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, ProjectTileInfo>>("ProjectTileInfo", (data) => { ProjectTileInfoDic = data; });
        _LowDataMgr.instance.DeserializeData<List<SkillLevelInfo>>("SkillLevelInfo", (data) => { SkillLevelInfoList = data; });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, SkillSetInfo>>("SkillSetInfo", (data) => { SkillSetInfoDic = data; });

    }

	// use asset bundle
	public void DeserializeData2()
	{
		_LowDataMgr.instance.DeserializeData<List<SkillLevelInfo>>("SkillLevelInfo", (data) => { SkillLevelInfoList = data; });
	}

	public void DeserializeDataAsync()
	{
		//_LowDataMgr.instance.DeserializeDataAsync<List<SkillLevelInfo>>("SkillLevelInfo", (data) => { SkillLevelInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<SkillLevelInfo>>("SkillLevelInfo", (data) => { SkillLevelInfoList = data; });
	}
}

