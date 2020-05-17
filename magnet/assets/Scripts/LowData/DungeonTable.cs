using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class DungeonTable
{
    [Serializable]
    public class StageInfo
    {
        public uint StageId;
        public JsonCustomData RequireStageId;
        public string StageName;
        byte _ChapId;
        public byte ChapId
        {
            set { _ChapId = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ChapId); }
        }
        uint _ChapName;
        public uint ChapName
        {
            set { _ChapName = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ChapName); }
        }
        uint _String;
        public uint String
        {
            set { _String = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_String); }
        }
        byte _type;
        public byte type
        {
            set { _type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_type); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        byte _DailyEntercount;
        public byte DailyEntercount
        {
            set { _DailyEntercount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DailyEntercount); }
        }
        uint _property;
        public uint property
        {
            set { _property = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_property); }
        }
        uint _AreaExplain;
        public uint AreaExplain
        {
            set { _AreaExplain = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AreaExplain); }
        }
        ushort _LimitTime;
        public ushort LimitTime
        {
            set { _LimitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_LimitTime); }
        }
        byte _ClearType1;
        public byte ClearType1
        {
            set { _ClearType1 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearType1); }
        }
        ushort _ClearValue1;
        public ushort ClearValue1
        {
            set { _ClearValue1 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ClearValue1); }
        }
        byte _ClearType2;
        public byte ClearType2
        {
            set { _ClearType2 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearType2); }
        }
        ushort _ClearValue2;
        public ushort ClearValue2
        {
            set { _ClearValue2 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ClearValue2); }
        }
        byte _ClearType3;
        public byte ClearType3
        {
            set { _ClearType3 = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearType3); }
        }
        ushort _ClearValue3;
        public ushort ClearValue3
        {
            set { _ClearValue3 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ClearValue3); }
        }
        byte _useValue;
        public byte useValue
        {
            set { _useValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_useValue); }
        }
        uint _BossIdx;
        public uint BossIdx
        {
            set { _BossIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossIdx); }
        }
        uint _BossRewardItemId;
        public uint BossRewardItemId
        {
            set { _BossRewardItemId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossRewardItemId); }
        }
        byte _BossItemMinCount;
        public byte BossItemMinCount
        {
            set { _BossItemMinCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BossItemMinCount); }
        }
        byte _BossItemMaxCount;
        public byte BossItemMaxCount
        {
            set { _BossItemMaxCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BossItemMaxCount); }
        }
        public JsonCustomData rewardItemId;
        public JsonCustomData regenMonster;
        uint _clearRewardGold;
        public uint clearRewardGold
        {
            set { _clearRewardGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearRewardGold); }
        }
        uint _clearRewardExp;
        public uint clearRewardExp
        {
            set { _clearRewardExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearRewardExp); }
        }
        byte _LightmapType;
        public byte LightmapType
        {
            set { _LightmapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LightmapType); }
        }
        uint _FixedReward;
        public uint FixedReward
        {
            set { _FixedReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FixedReward); }
        }
        byte _ClearStageCount;
        public byte ClearStageCount
        {
            set { _ClearStageCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ClearStageCount); }
        }
        uint _FirstReward;
        public uint FirstReward
        {
            set { _FirstReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FirstReward); }
        }
    }

    [Serializable]
    public class SingleBossRaidInfo
    {
        public uint raidId;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        ushort _level;
        public ushort level
        {
            set { _level = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_level); }
        }
        uint _stageString;
        public uint stageString
        {
            set { _stageString = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_stageString); }
        }
        uint _stageDesc;
        public uint stageDesc
        {
            set { _stageDesc = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_stageDesc); }
        }
        public string stageName;
        byte _levelLimit;
        public byte levelLimit
        {
            set { _levelLimit = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_levelLimit); }
        }
        byte _useValue;
        public byte useValue
        {
            set { _useValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_useValue); }
        }
        ushort _limitTime;
        public ushort limitTime
        {
            set { _limitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_limitTime); }
        }
        uint _clearExp;
        public uint clearExp
        {
            set { _clearExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearExp); }
        }
        uint _clearGold;
        public uint clearGold
        {
            set { _clearGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearGold); }
        }
        uint _BossIdx;
        public uint BossIdx
        {
            set { _BossIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossIdx); }
        }
        public JsonCustomData RewardId;
        public JsonCustomData RewardProb;
        uint _FightingPower;
        public uint FightingPower
        {
            set { _FightingPower = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FightingPower); }
        }
        public JsonCustomData ItemId;
        byte _cameraType;
        public byte cameraType
        {
            set { _cameraType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_cameraType); }
        }
        uint _FixedReward;
        public uint FixedReward
        {
            set { _FixedReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FixedReward); }
        }
    }

    [Serializable]
    public class SkillInfo
    {
        public byte Index;
        public string StageName;
        byte _MinenterLv;
        public byte MinenterLv
        {
            set { _MinenterLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MinenterLv); }
        }
        public JsonCustomData RewardItemId;
        byte _UseEnergy;
        public byte UseEnergy
        {
            set { _UseEnergy = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_UseEnergy); }
        }
        public JsonCustomData RegenCharacter;
        public JsonCustomData RegenMob;
        public JsonCustomData MonsterIndex;
        public JsonCustomData MonsterTime;
        public JsonCustomData regenCount;
        public JsonCustomData DropPoint;
        public JsonCustomData DropGold;
        byte _lghtmapType;
        public byte lghtmapType
        {
            set { _lghtmapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_lghtmapType); }
        }
        ushort _LimitTime;
        public ushort LimitTime
        {
            set { _LimitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_LimitTime); }
        }
        uint _ReviveCost;
        public uint ReviveCost
        {
            set { _ReviveCost = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ReviveCost); }
        }
    }
    
    [Serializable]
    public class FreefightTableInfo
    {
        public uint StageIndex;
        uint _MapIndex;
        public uint MapIndex
        {
            set { _MapIndex = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MapIndex); }
        }
        public string StageName;
        uint _GroupDescription;
        public uint GroupDescription
        {
            set { _GroupDescription = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_GroupDescription); }
        }
        byte _StageGroup;
        public byte StageGroup
        {
            set { _StageGroup = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_StageGroup); }
        }
        byte _MinenterLv;
        public byte MinenterLv
        {
            set { _MinenterLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MinenterLv); }
        }
        byte _MaxenterLv;
        public byte MaxenterLv
        {
            set { _MaxenterLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MaxenterLv); }
        }
        public JsonCustomData ARegenCharacter;
        public JsonCustomData BRegenCharacter;
        public JsonCustomData CRegenCharacter;
        public JsonCustomData ARegenMob;
        byte _ARegenNormalCount;
        public byte ARegenNormalCount
        {
            set { _ARegenNormalCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ARegenNormalCount); }
        }
        byte _ARegenEliteCount;
        public byte ARegenEliteCount
        {
            set { _ARegenEliteCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ARegenEliteCount); }
        }
        public JsonCustomData ARegenNormalIdx;
        public JsonCustomData ARegenEliteIdx;
        public JsonCustomData BRegenMob;
        byte _BRegenNormalCount;
        public byte BRegenNormalCount
        {
            set { _BRegenNormalCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BRegenNormalCount); }
        }
        byte _BRegenEliteCount;
        public byte BRegenEliteCount
        {
            set { _BRegenEliteCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BRegenEliteCount); }
        }
        public JsonCustomData BRegenNormalIdx;
        public JsonCustomData BRegenEliteIdx;
        public JsonCustomData CRegenMob;
        byte _CRegenNormalCount;
        public byte CRegenNormalCount
        {
            set { _CRegenNormalCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_CRegenNormalCount); }
        }
        byte _CRegenEliteCount;
        public byte CRegenEliteCount
        {
            set { _CRegenEliteCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_CRegenEliteCount); }
        }
        public JsonCustomData CRegenNormalIdx;
        public JsonCustomData CRegenEliteIdx;
        public JsonCustomData DRegenMob;
        byte _DRegenNormalCount;
        public byte DRegenNormalCount
        {
            set { _DRegenNormalCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DRegenNormalCount); }
        }
        byte _DRegenEliteCount;
        public byte DRegenEliteCount
        {
            set { _DRegenEliteCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_DRegenEliteCount); }
        }
        public JsonCustomData DRegenNormalIdx;
        public JsonCustomData DRegenEliteIdx;
        public JsonCustomData ARegenMBoss;
        public JsonCustomData ARegenMBossIdx;
        public JsonCustomData BRegenMBoss;
        public JsonCustomData BRegenMBossIdx;
        public JsonCustomData CRegenMBoss;
        public JsonCustomData CRegenMBossIdx;
        public JsonCustomData RegenCBoss;
        public JsonCustomData RegenCBossIdx;
        ushort _ScoreNormal;
        public ushort ScoreNormal
        {
            set { _ScoreNormal = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreNormal); }
        }
        ushort _ScoreElite;
        public ushort ScoreElite
        {
            set { _ScoreElite = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreElite); }
        }
        ushort _ScoreMBoss;
        public ushort ScoreMBoss
        {
            set { _ScoreMBoss = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreMBoss); }
        }
        ushort _ScoreCBoss01;
        public ushort ScoreCBoss01
        {
            set { _ScoreCBoss01 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreCBoss01); }
        }
        ushort _ScoreCBoss02;
        public ushort ScoreCBoss02
        {
            set { _ScoreCBoss02 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreCBoss02); }
        }
        ushort _ScoreCBoss03;
        public ushort ScoreCBoss03
        {
            set { _ScoreCBoss03 = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreCBoss03); }
        }
        ushort _ScoreKill;
        public ushort ScoreKill
        {
            set { _ScoreKill = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreKill); }
        }
        ushort _ScoreDeath;
        public ushort ScoreDeath
        {
            set { _ScoreDeath = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_ScoreDeath); }
        }
        byte _InstantReviveCash;
        public byte InstantReviveCash
        {
            set { _InstantReviveCash = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_InstantReviveCash); }
        }
        public float FogEnd;
        byte _lghtmapType;
        public byte lghtmapType
        {
            set { _lghtmapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_lghtmapType); }
        }
    }

    [Serializable]
    public class EquipInfo
    {
        public byte Index;
        public string StageName;
        byte _MinenterLv;
        public byte MinenterLv
        {
            set { _MinenterLv = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_MinenterLv); }
        }
        public JsonCustomData RewardItemId;
        byte _UseEnergy;
        public byte UseEnergy
        {
            set { _UseEnergy = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_UseEnergy); }
        }
        public JsonCustomData RegenCharacter;
        public JsonCustomData RegenMob;
        public JsonCustomData MonsterIndex;
        public JsonCustomData RegenMobCount;
        public JsonCustomData RegenMobTime;
        public JsonCustomData GetMobPoint;
        public JsonCustomData RegenDummy;
        public JsonCustomData DummyMonster;
        public JsonCustomData DummyMonsterTime;
        byte _RegenDummyCount;
        public byte RegenDummyCount
        {
            set { _RegenDummyCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RegenDummyCount); }
        }
        byte _RegenDummyCountMax;
        public byte RegenDummyCountMax
        {
            set { _RegenDummyCountMax = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RegenDummyCountMax); }
        }
        public JsonCustomData DropGold;
        byte _lghtmapType;
        public byte lghtmapType
        {
            set { _lghtmapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_lghtmapType); }
        }
        ushort _LimitTime;
        public ushort LimitTime
        {
            set { _LimitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_LimitTime); }
        }
        uint _ReviveCost;
        public uint ReviveCost
        {
            set { _ReviveCost = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ReviveCost); }
        }
    }

    [Serializable]
    public class TowerInfo
    {
        public uint StageIndex;
        uint _level;
        public uint level
        {
            set { _level = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_level); }
        }
        uint _maxLevel;
        public uint maxLevel
        {
            set { _maxLevel = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_maxLevel); }
        }
        uint _NextStage;
        public uint NextStage
        {
            set { _NextStage = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextStage); }
        }
        public string mapName;
        ushort _limitTime;
        public ushort limitTime
        {
            set { _limitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_limitTime); }
        }
        uint _mingold;
        public uint mingold
        {
            set { _mingold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_mingold); }
        }
        uint _maxgold;
        public uint maxgold
        {
            set { _maxgold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_maxgold); }
        }
        uint _exp;
        public uint exp
        {
            set { _exp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_exp); }
        }
        uint _FirstReward;
        public uint FirstReward
        {
            set { _FirstReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FirstReward); }
        }
        public JsonCustomData rewarditemId;
        uint _gachaIdx;
        public uint gachaIdx
        {
            set { _gachaIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_gachaIdx); }
        }
        uint _gachaValue;
        public uint gachaValue
        {
            set { _gachaValue = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_gachaValue); }
        }
        uint _getPoint;
        public uint getPoint
        {
            set { _getPoint = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_getPoint); }
        }
        uint _MobId1;
        public uint MobId1
        {
            set { _MobId1 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MobId1); }
        }
        uint _MobId2;
        public uint MobId2
        {
            set { _MobId2 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MobId2); }
        }
        uint _MobId3;
        public uint MobId3
        {
            set { _MobId3 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MobId3); }
        }
        uint _MobId4;
        public uint MobId4
        {
            set { _MobId4 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MobId4); }
        }
        uint _MobId5;
        public uint MobId5
        {
            set { _MobId5 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MobId5); }
        }
		uint _MobId6;
		public uint MobId6
		{
			set { _MobId6 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId6); }
		}
		uint _MobId7;
		public uint MobId7
		{
			set { _MobId7 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId7); }
		}
		uint _MobId8;
		public uint MobId8
		{
			set { _MobId8 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId8); }
		}
		uint _MobId9;
		public uint MobId9
		{
			set { _MobId9 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId9); }
		}
		uint _MobId10;
		public uint MobId10
		{
			set { _MobId10 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId10); }
		}
		uint _MobId11;
		public uint MobId11
		{
			set { _MobId11 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_MobId11); }
		}
        uint _FixedReward;
        public uint FixedReward
        {
            set { _FixedReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FixedReward); }
        }
    }

    [Serializable]
    public class ChapterRewardInfo
    {
        public uint ID;
        byte _ChapId;
        public byte ChapId
        {
            set { _ChapId = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ChapId); }
        }
        byte _ChapType;
        public byte ChapType
        {
            set { _ChapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ChapType); }
        }
        byte _BoxId;
        public byte BoxId
        {
            set { _BoxId = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BoxId); }
        }
        byte _NeedStar;
        public byte NeedStar
        {
            set { _NeedStar = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_NeedStar); }
        }
        uint _Reward;
        public uint Reward
        {
            set { _Reward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Reward); }
        }
    }

    [Serializable]
    public class ColosseumInfo
    {
        public uint StageId;
        uint _RequireStageId;
        public uint RequireStageId
        {
            set { _RequireStageId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RequireStageId); }
        }
        uint _MapIndex;
        public uint MapIndex
        {
            set { _MapIndex = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MapIndex); }
        }
        public string StageName;
        uint _String;
        public uint String
        {
            set { _String = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_String); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        uint _property;
        public uint property
        {
            set { _property = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_property); }
        }
        uint _AreaExplain;
        public uint AreaExplain
        {
            set { _AreaExplain = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AreaExplain); }
        }
        uint _FightingPower;
        public uint FightingPower
        {
            set { _FightingPower = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FightingPower); }
        }
        ushort _LimitTime;
        public ushort LimitTime
        {
            set { _LimitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_LimitTime); }
        }
        byte _usevalue;
        public byte usevalue
        {
            set { _usevalue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_usevalue); }
        }
        public JsonCustomData MobGroup;
        uint _BossIdx;
        public uint BossIdx
        {
            set { _BossIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossIdx); }
        }
        uint _BossRewardItemId;
        public uint BossRewardItemId
        {
            set { _BossRewardItemId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossRewardItemId); }
        }
        byte _BossItemMinCount;
        public byte BossItemMinCount
        {
            set { _BossItemMinCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BossItemMinCount); }
        }
        byte _BossItemMaxCount;
        public byte BossItemMaxCount
        {
            set { _BossItemMaxCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_BossItemMaxCount); }
        }
        public JsonCustomData rewardItemId;
        uint _clearRewardGold;
        public uint clearRewardGold
        {
            set { _clearRewardGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearRewardGold); }
        }
        uint _clearRewardExp;
        public uint clearRewardExp
        {
            set { _clearRewardExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearRewardExp); }
        }
        byte _LightmapType;
        public byte LightmapType
        {
            set { _LightmapType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LightmapType); }
        }
        uint _FixedReward;
        public uint FixedReward
        {
            set { _FixedReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FixedReward); }
        }
    }

    [Serializable]
    public class MultyBossRaidInfo
    {
        public uint raidId;
        byte _Type;
        public byte Type
        {
            set { _Type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Type); }
        }
        ushort _level;
        public ushort level
        {
            set { _level = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_level); }
        }
        uint _stageString;
        public uint stageString
        {
            set { _stageString = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_stageString); }
        }
        uint _stageDesc;
        public uint stageDesc
        {
            set { _stageDesc = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_stageDesc); }
        }
        uint _MapIndex;
        public uint MapIndex
        {
            set { _MapIndex = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_MapIndex); }
        }
        byte _levelLimit;
        public byte levelLimit
        {
            set { _levelLimit = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_levelLimit); }
        }
        byte _useValue;
        public byte useValue
        {
            set { _useValue = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_useValue); }
        }
        ushort _limitTime;
        public ushort limitTime
        {
            set { _limitTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_limitTime); }
        }
        uint _clearExp;
        public uint clearExp
        {
            set { _clearExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearExp); }
        }
        uint _clearGold;
        public uint clearGold
        {
            set { _clearGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_clearGold); }
        }
        uint _BossIdx;
        public uint BossIdx
        {
            set { _BossIdx = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BossIdx); }
        }
        public JsonCustomData RegenCharacter;
        public JsonCustomData BossSpawnpos;
        public JsonCustomData RewardId;
        public JsonCustomData RewardProb;
        uint _FightingPower;
        public uint FightingPower
        {
            set { _FightingPower = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FightingPower); }
        }
        public JsonCustomData ItemId;
        byte _cameraType;
        public byte cameraType
        {
            set { _cameraType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_cameraType); }
        }
        uint _EnterPoint;
        public uint EnterPoint
        {
            set { _EnterPoint = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_EnterPoint); }
        }
        byte _EnterLevel;
        public byte EnterLevel
        {
            set { _EnterLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_EnterLevel); }
        }
        uint _FixedReward;
        public uint FixedReward
        {
            set { _FixedReward = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_FixedReward); }
        }
    }
    [Serializable]
	public class ContentsOpenInfo
	{
		public uint LogID;
		byte _ContentsType;
		public byte ContentsType {
			set { _ContentsType = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_ContentsType); }
		}
		uint _ContentsIdx;
		public uint ContentsIdx {
			set { _ContentsIdx = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_ContentsIdx); }
		}
		byte _ConditionType1;
		public byte ConditionType1 {
			set { _ConditionType1 = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_ConditionType1); }
		}
		uint _ConditionValue1;
		public uint ConditionValue1 {
			set { _ConditionValue1 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_ConditionValue1); }
		}
		byte _ConditionType2;
		public byte ConditionType2 {
			set { _ConditionType2 = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_ConditionType2); }
		}
		uint _ConditionValue2;
		public uint ConditionValue2 {
			set { _ConditionValue2 = EncryptHelper.SSecureUINT(value); }
			get { return EncryptHelper.GSecureUINT(_ConditionValue2); }
		}
		byte _OpenType;
		public byte OpenType {
			set { _OpenType = EncryptHelper.SSecureBYTE(value); }
			get { return EncryptHelper.GSecureBYTE(_OpenType); }
		}
	}



    [Serializable]
    public class FreeFightPointInfo
    {
        public uint LogID;
        public float MinBattlePoint;
        public float MaxBattlePoint;
        uint _Point;
        public uint Point
        {
            set { _Point = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Point); }
        }
    }

    public Dictionary<uint, StageInfo> StageInfoDic = new Dictionary<uint, StageInfo>();
    public Dictionary<byte, EquipInfo> EquipInfoDic = new Dictionary<byte, EquipInfo>();
    public Dictionary<byte, SkillInfo> SkillInfoDic = new Dictionary<byte, SkillInfo>();
    public Dictionary<uint, SingleBossRaidInfo> SingleBossRaidInfoDic = new Dictionary<uint, SingleBossRaidInfo>();
    public Dictionary<uint, FreefightTableInfo> FreefightTableInfoDic = new Dictionary<uint, FreefightTableInfo>();
    public Dictionary<uint, ColosseumInfo> ColosseumInfoDic = new Dictionary<uint, ColosseumInfo>();
    public Dictionary<uint, MultyBossRaidInfo> MultyBossRaidInfoDic = new Dictionary<uint, MultyBossRaidInfo>();
    public List<TowerInfo> TowerInfoList = new List<TowerInfo>();
    public List<ChapterRewardInfo> ChapterRewardInfoList = new List<ChapterRewardInfo>();
    public List<ContentsOpenInfo> ContentsOpenInfoList = new List<ContentsOpenInfo>();
    public Dictionary<uint, FreeFightPointInfo> FreeFightPointInfoDic = new Dictionary<uint, FreeFightPointInfo>();

    public void LoadLowData()
    {
        //일반 스테이지
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Stage", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Stage = new JSONObject(strSrc);

            for (int i = 0; i < Stage.list.Count; i++)
            {
                StageInfo tmpInfo = new StageInfo();
                tmpInfo.StageId = (uint)Stage[i]["StageId_ui"].n;
                tmpInfo.RequireStageId = new JsonCustomData(Stage[i]["RequireStageId_j"].ToString());
                tmpInfo.StageName = Stage[i]["StageName_c"].str;
                tmpInfo.ChapId = (byte)Stage[i]["ChapId_b"].n;
                tmpInfo.ChapName = (uint)Stage[i]["ChapName_ui"].n;
                tmpInfo.String = (uint)Stage[i]["String_ui"].n;
                tmpInfo.type = (byte)Stage[i]["type_b"].n;
                tmpInfo.LimitLevel = (byte)Stage[i]["LimitLevel_b"].n;
                tmpInfo.DailyEntercount = (byte)Stage[i]["DailyEntercount_b"].n;
                tmpInfo.property = (uint)Stage[i]["property_ui"].n;
                tmpInfo.AreaExplain = (uint)Stage[i]["AreaExplain_ui"].n;
                tmpInfo.LimitTime = (ushort)Stage[i]["LimitTime_us"].n;
                tmpInfo.ClearType1 = (byte)Stage[i]["ClearType1_b"].n;
                tmpInfo.ClearValue1 = (ushort)Stage[i]["ClearValue1_us"].n;
                tmpInfo.ClearType2 = (byte)Stage[i]["ClearType2_b"].n;
                tmpInfo.ClearValue2 = (ushort)Stage[i]["ClearValue2_us"].n;
                tmpInfo.ClearType3 = (byte)Stage[i]["ClearType3_b"].n;
                tmpInfo.ClearValue3 = (ushort)Stage[i]["ClearValue3_us"].n;
                tmpInfo.useValue = (byte)Stage[i]["useValue_b"].n;
                tmpInfo.BossIdx = (uint)Stage[i]["BossIdx_ui"].n;
                tmpInfo.BossRewardItemId = (uint)Stage[i]["BossRewardItemId_ui"].n;
                tmpInfo.BossItemMinCount = (byte)Stage[i]["BossItemMinCount_b"].n;
                tmpInfo.BossItemMaxCount = (byte)Stage[i]["BossItemMaxCount_b"].n;
                tmpInfo.rewardItemId = new JsonCustomData(Stage[i]["rewardItemId_j"].ToString());
                tmpInfo.regenMonster = new JsonCustomData(Stage[i]["regenMonster_j"].ToString());
                tmpInfo.clearRewardGold = (uint)Stage[i]["clearRewardGold_ui"].n;
                tmpInfo.clearRewardExp = (uint)Stage[i]["clearRewardExp_ui"].n;
                tmpInfo.LightmapType = (byte)Stage[i]["LightmapType_b"].n;
                tmpInfo.FixedReward = (uint)Stage[i]["FixedReward_ui"].n;
                tmpInfo.ClearStageCount = (byte)Stage[i]["ClearStageCount_b"].n;
                tmpInfo.FirstReward = (uint)Stage[i]["FirstReward_ui"].n;

                StageInfoDic.Add(tmpInfo.StageId, tmpInfo);
            }
        }

        //보스 레이드
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_SingleBossRaid", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject SingleBossRaid = new JSONObject(strSrc);

            for (int i = 0; i < SingleBossRaid.list.Count; i++)
            {
                SingleBossRaidInfo tmpInfo = new SingleBossRaidInfo();
                tmpInfo.raidId = (uint)SingleBossRaid[i]["raidId_ui"].n;
                tmpInfo.Type = (byte)SingleBossRaid[i]["Type_b"].n;
                tmpInfo.level = (ushort)SingleBossRaid[i]["level_us"].n;
                tmpInfo.stageString = (uint)SingleBossRaid[i]["stageString_ui"].n;
                tmpInfo.stageDesc = (uint)SingleBossRaid[i]["stageDesc_ui"].n;
                tmpInfo.stageName = SingleBossRaid[i]["stageName_c"].str;
                tmpInfo.levelLimit = (byte)SingleBossRaid[i]["levelLimit_b"].n;
                tmpInfo.useValue = (byte)SingleBossRaid[i]["useValue_b"].n;
                tmpInfo.limitTime = (ushort)SingleBossRaid[i]["limitTime_us"].n;
                tmpInfo.clearExp = (uint)SingleBossRaid[i]["clearExp_ui"].n;
                tmpInfo.clearGold = (uint)SingleBossRaid[i]["clearGold_ui"].n;
                tmpInfo.BossIdx = (uint)SingleBossRaid[i]["BossIdx_ui"].n;
                tmpInfo.RewardId = new JsonCustomData(SingleBossRaid[i]["RewardId_j"].ToString());
                tmpInfo.RewardProb = new JsonCustomData(SingleBossRaid[i]["RewardProb_j"].ToString());
                tmpInfo.FightingPower = (uint)SingleBossRaid[i]["FightingPower_ui"].n;
                tmpInfo.ItemId = new JsonCustomData(SingleBossRaid[i]["ItemId_j"].ToString());
                tmpInfo.cameraType = (byte)SingleBossRaid[i]["cameraType_b"].n;
                tmpInfo.FixedReward = (uint)SingleBossRaid[i]["FixedReward_ui"].n;

                SingleBossRaidInfoDic.Add(tmpInfo.raidId, tmpInfo);
            }
        }

        //경험치
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Skill", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Skill = new JSONObject(strSrc);

            for (int i = 0; i < Skill.list.Count; i++)
            {
                SkillInfo tmpInfo = new SkillInfo();
                tmpInfo.Index = (byte)Skill[i]["Index_b"].n;
                tmpInfo.StageName = Skill[i]["StageName_c"].str;
                tmpInfo.MinenterLv = (byte)Skill[i]["MinenterLv_b"].n;
                tmpInfo.RewardItemId = new JsonCustomData(Skill[i]["RewardItemId_j"].ToString());
                tmpInfo.UseEnergy = (byte)Skill[i]["UseEnergy_b"].n;
                tmpInfo.RegenCharacter = new JsonCustomData(Skill[i]["RegenCharacter_j"].ToString());
                tmpInfo.RegenMob = new JsonCustomData(Skill[i]["RegenMob_j"].ToString());
                tmpInfo.MonsterIndex = new JsonCustomData(Skill[i]["MonsterIndex_j"].ToString());
                tmpInfo.MonsterTime = new JsonCustomData(Skill[i]["MonsterTime_j"].ToString());
                tmpInfo.regenCount = new JsonCustomData(Skill[i]["regenCount_j"].ToString());
                tmpInfo.DropPoint = new JsonCustomData(Skill[i]["DropPoint_j"].ToString());
                tmpInfo.DropGold = new JsonCustomData(Skill[i]["DropGold_j"].ToString());
                tmpInfo.lghtmapType = (byte)Skill[i]["lghtmapType_b"].n;
                tmpInfo.LimitTime = (ushort)Skill[i]["LimitTime_us"].n;
                tmpInfo.ReviveCost = (uint)Skill[i]["ReviveCost_ui"].n;

                SkillInfoDic.Add(tmpInfo.Index, tmpInfo);
            }
        }

        //난투장
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Freefight", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Freefight = new JSONObject(strSrc);

            for (int i = 0; i < Freefight.list.Count; i++)
            {
                FreefightTableInfo tmpInfo = new FreefightTableInfo();
                tmpInfo.StageIndex = (uint)Freefight[i]["StageIndex_ui"].n;
                tmpInfo.MapIndex = (uint)Freefight[i]["MapIndex_ui"].n;
                tmpInfo.StageName = Freefight[i]["StageName_c"].str;
                tmpInfo.GroupDescription = (uint)Freefight[i]["GroupDescription_ui"].n;
                tmpInfo.StageGroup = (byte)Freefight[i]["StageGroup_b"].n;
                tmpInfo.MinenterLv = (byte)Freefight[i]["MinenterLv_b"].n;
                tmpInfo.MaxenterLv = (byte)Freefight[i]["MaxenterLv_b"].n;
                tmpInfo.ARegenCharacter = new JsonCustomData(Freefight[i]["ARegenCharacter_j"].ToString());
                tmpInfo.BRegenCharacter = new JsonCustomData(Freefight[i]["BRegenCharacter_j"].ToString());
                tmpInfo.CRegenCharacter = new JsonCustomData(Freefight[i]["CRegenCharacter_j"].ToString());
                tmpInfo.ARegenMob = new JsonCustomData(Freefight[i]["ARegenMob_j"].ToString());
                tmpInfo.ARegenNormalCount = (byte)Freefight[i]["ARegenNormalCount_b"].n;
                tmpInfo.ARegenEliteCount = (byte)Freefight[i]["ARegenEliteCount_b"].n;
                tmpInfo.ARegenNormalIdx = new JsonCustomData(Freefight[i]["ARegenNormalIdx_j"].ToString());
                tmpInfo.ARegenEliteIdx = new JsonCustomData(Freefight[i]["ARegenEliteIdx_j"].ToString());
                tmpInfo.BRegenMob = new JsonCustomData(Freefight[i]["BRegenMob_j"].ToString());
                tmpInfo.BRegenNormalCount = (byte)Freefight[i]["BRegenNormalCount_b"].n;
                tmpInfo.BRegenEliteCount = (byte)Freefight[i]["BRegenEliteCount_b"].n;
                tmpInfo.BRegenNormalIdx = new JsonCustomData(Freefight[i]["BRegenNormalIdx_j"].ToString());
                tmpInfo.BRegenEliteIdx = new JsonCustomData(Freefight[i]["BRegenEliteIdx_j"].ToString());
                tmpInfo.CRegenMob = new JsonCustomData(Freefight[i]["CRegenMob_j"].ToString());
                tmpInfo.CRegenNormalCount = (byte)Freefight[i]["CRegenNormalCount_b"].n;
                tmpInfo.CRegenEliteCount = (byte)Freefight[i]["CRegenEliteCount_b"].n;
                tmpInfo.CRegenNormalIdx = new JsonCustomData(Freefight[i]["CRegenNormalIdx_j"].ToString());
                tmpInfo.CRegenEliteIdx = new JsonCustomData(Freefight[i]["CRegenEliteIdx_j"].ToString());
                tmpInfo.DRegenMob = new JsonCustomData(Freefight[i]["DRegenMob_j"].ToString());
                tmpInfo.DRegenNormalCount = (byte)Freefight[i]["DRegenNormalCount_b"].n;
                tmpInfo.DRegenEliteCount = (byte)Freefight[i]["DRegenEliteCount_b"].n;
                tmpInfo.DRegenNormalIdx = new JsonCustomData(Freefight[i]["DRegenNormalIdx_j"].ToString());
                tmpInfo.DRegenEliteIdx = new JsonCustomData(Freefight[i]["DRegenEliteIdx_j"].ToString());
                tmpInfo.ARegenMBoss = new JsonCustomData(Freefight[i]["ARegenMBoss_j"].ToString());
                tmpInfo.ARegenMBossIdx = new JsonCustomData(Freefight[i]["ARegenMBossIdx_j"].ToString());
                tmpInfo.BRegenMBoss = new JsonCustomData(Freefight[i]["BRegenMBoss_j"].ToString());
                tmpInfo.BRegenMBossIdx = new JsonCustomData(Freefight[i]["BRegenMBossIdx_j"].ToString());
                tmpInfo.CRegenMBoss = new JsonCustomData(Freefight[i]["CRegenMBoss_j"].ToString());
                tmpInfo.CRegenMBossIdx = new JsonCustomData(Freefight[i]["CRegenMBossIdx_j"].ToString());
                tmpInfo.RegenCBoss = new JsonCustomData(Freefight[i]["RegenCBoss_j"].ToString());
                tmpInfo.RegenCBossIdx = new JsonCustomData(Freefight[i]["RegenCBossIdx_j"].ToString());
                tmpInfo.ScoreNormal = (ushort)Freefight[i]["ScoreNormal_us"].n;
                tmpInfo.ScoreElite = (ushort)Freefight[i]["ScoreElite_us"].n;
                tmpInfo.ScoreMBoss = (ushort)Freefight[i]["ScoreMBoss_us"].n;
                tmpInfo.ScoreCBoss01 = (ushort)Freefight[i]["ScoreCBoss01_us"].n;
                tmpInfo.ScoreCBoss02 = (ushort)Freefight[i]["ScoreCBoss02_us"].n;
                tmpInfo.ScoreCBoss03 = (ushort)Freefight[i]["ScoreCBoss03_us"].n;
                tmpInfo.ScoreKill = (ushort)Freefight[i]["ScoreKill_us"].n;
                tmpInfo.ScoreDeath = (ushort)Freefight[i]["ScoreDeath_us"].n;
                tmpInfo.InstantReviveCash = (byte)Freefight[i]["InstantReviveCash_b"].n;
                tmpInfo.FogEnd = (float)Freefight[i]["FogEnd_f"].n;
                tmpInfo.lghtmapType = (byte)Freefight[i]["lghtmapType_b"].n;

                FreefightTableInfoDic.Add(tmpInfo.StageIndex, tmpInfo);
            }
        }

        //장비 재료던전(구 : 골드)
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Equip", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Equip = new JSONObject(strSrc);

            for (int i = 0; i < Equip.list.Count; i++)
            {
                EquipInfo tmpInfo = new EquipInfo();
                tmpInfo.Index = (byte)Equip[i]["Index_b"].n;
                tmpInfo.StageName = Equip[i]["StageName_c"].str;
                tmpInfo.MinenterLv = (byte)Equip[i]["MinenterLv_b"].n;
                tmpInfo.RewardItemId = new JsonCustomData(Equip[i]["RewardItemId_j"].ToString());
                tmpInfo.UseEnergy = (byte)Equip[i]["UseEnergy_b"].n;
                tmpInfo.RegenCharacter = new JsonCustomData(Equip[i]["RegenCharacter_j"].ToString());
                tmpInfo.RegenMob = new JsonCustomData(Equip[i]["RegenMob_j"].ToString());
                tmpInfo.MonsterIndex = new JsonCustomData(Equip[i]["MonsterIndex_j"].ToString());
                tmpInfo.RegenMobCount = new JsonCustomData(Equip[i]["RegenMobCount_j"].ToString());
                tmpInfo.RegenMobTime = new JsonCustomData(Equip[i]["RegenMobTime_j"].ToString());
                tmpInfo.GetMobPoint = new JsonCustomData(Equip[i]["GetMobPoint_j"].ToString());
                tmpInfo.RegenDummy = new JsonCustomData(Equip[i]["RegenDummy_j"].ToString());
                tmpInfo.DummyMonster = new JsonCustomData(Equip[i]["DummyMonster_j"].ToString());
                tmpInfo.DummyMonsterTime = new JsonCustomData(Equip[i]["DummyMonsterTime_j"].ToString());
                tmpInfo.RegenDummyCount = (byte)Equip[i]["RegenDummyCount_b"].n;
                tmpInfo.RegenDummyCountMax = (byte)Equip[i]["RegenDummyCountMax_b"].n;
                tmpInfo.DropGold = new JsonCustomData(Equip[i]["DropGold_j"].ToString());
                tmpInfo.lghtmapType = (byte)Equip[i]["lghtmapType_b"].n;
                tmpInfo.LimitTime = (ushort)Equip[i]["LimitTime_us"].n;
                tmpInfo.ReviveCost = (uint)Equip[i]["ReviveCost_ui"].n;

                EquipInfoDic.Add(tmpInfo.Index, tmpInfo);
            }
        }

        //마계의 탑
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Tower", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Tower = new JSONObject(strSrc);

            for (int i = 0; i < Tower.list.Count; i++)
            {
                TowerInfo tmpInfo = new TowerInfo();
                tmpInfo.StageIndex = (uint)Tower[i]["StageIndex_ui"].n;
                tmpInfo.level = (uint)Tower[i]["level_ui"].n;
                tmpInfo.maxLevel = (uint)Tower[i]["maxLevel_ui"].n;
                tmpInfo.NextStage = (uint)Tower[i]["NextStage_ui"].n;
                tmpInfo.mapName = Tower[i]["mapName_c"].str;
                tmpInfo.limitTime = (ushort)Tower[i]["limitTime_us"].n;
                tmpInfo.mingold = (uint)Tower[i]["mingold_ui"].n;
                tmpInfo.maxgold = (uint)Tower[i]["maxgold_ui"].n;
                tmpInfo.exp = (uint)Tower[i]["exp_ui"].n;
                tmpInfo.FirstReward = (uint)Tower[i]["FirstReward_ui"].n;
                tmpInfo.rewarditemId = new JsonCustomData(Tower[i]["rewarditemId_j"].ToString());
                tmpInfo.gachaIdx = (uint)Tower[i]["gachaIdx_ui"].n;
                tmpInfo.gachaValue = (uint)Tower[i]["gachaValue_ui"].n;
                tmpInfo.getPoint = (uint)Tower[i]["getPoint_ui"].n;
                tmpInfo.MobId1 = (uint)Tower[i]["MobId1_ui"].n;
                tmpInfo.MobId2 = (uint)Tower[i]["MobId2_ui"].n;
                tmpInfo.MobId3 = (uint)Tower[i]["MobId3_ui"].n;
                tmpInfo.MobId4 = (uint)Tower[i]["MobId4_ui"].n;
                tmpInfo.MobId5 = (uint)Tower[i]["MobId5_ui"].n;
				tmpInfo.MobId6 = (uint)Tower[i]["MobId6_ui"].n;
				tmpInfo.MobId7 = (uint)Tower[i]["MobId7_ui"].n;
				tmpInfo.MobId8 = (uint)Tower[i]["MobId8_ui"].n;
				tmpInfo.MobId9 = (uint)Tower[i]["MobId9_ui"].n;
				tmpInfo.MobId10 = (uint)Tower[i]["MobId10_ui"].n;
				tmpInfo.MobId11 = (uint)Tower[i]["MobId11_ui"].n;
                tmpInfo.FixedReward = (uint)Tower[i]["FixedReward_ui"].n;

                TowerInfoList.Add(tmpInfo);
            }
        }

        //일반 스테이지 챕터 보상
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_ChapterReward", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ChapterReward = new JSONObject(strSrc);

            for (int i = 0; i < ChapterReward.list.Count; i++)
            {
                ChapterRewardInfo tmpInfo = new ChapterRewardInfo();
                tmpInfo.ID = (uint)ChapterReward[i]["ID_ui"].n;
                tmpInfo.ChapId = (byte)ChapterReward[i]["ChapId_b"].n;
                tmpInfo.ChapType = (byte)ChapterReward[i]["ChapType_b"].n;
                tmpInfo.BoxId = (byte)ChapterReward[i]["BoxId_b"].n;
                tmpInfo.NeedStar = (byte)ChapterReward[i]["NeedStar_b"].n;
                tmpInfo.Reward = (uint)ChapterReward[i]["Reward_ui"].n;

                ChapterRewardInfoList.Add(tmpInfo);
            }
        }

        //콜로세움
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_Colosseum", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Colosseum = new JSONObject(strSrc);

            for (int i = 0; i < Colosseum.list.Count; i++)
            {
                ColosseumInfo tmpInfo = new ColosseumInfo();
                tmpInfo.StageId = (uint)Colosseum[i]["StageId_ui"].n;
                tmpInfo.RequireStageId = (uint)Colosseum[i]["RequireStageId_ui"].n;
                tmpInfo.MapIndex = (uint)Colosseum[i]["MapIndex_ui"].n;
                tmpInfo.StageName = Colosseum[i]["StageName_c"].str;
                tmpInfo.String = (uint)Colosseum[i]["String_ui"].n;
                tmpInfo.LimitLevel = (byte)Colosseum[i]["LimitLevel_b"].n;
                tmpInfo.property = (uint)Colosseum[i]["property_ui"].n;
                tmpInfo.AreaExplain = (uint)Colosseum[i]["AreaExplain_ui"].n;
                tmpInfo.FightingPower = (uint)Colosseum[i]["FightingPower_ui"].n;
                tmpInfo.LimitTime = (ushort)Colosseum[i]["LimitTime_us"].n;
                tmpInfo.usevalue = (byte)Colosseum[i]["usevalue_b"].n;
                tmpInfo.MobGroup = new JsonCustomData(Colosseum[i]["MobGroup_j"].ToString());
                tmpInfo.BossIdx = (uint)Colosseum[i]["BossIdx_ui"].n;
                tmpInfo.BossRewardItemId = (uint)Colosseum[i]["BossRewardItemId_ui"].n;
                tmpInfo.BossItemMinCount = (byte)Colosseum[i]["BossItemMinCount_b"].n;
                tmpInfo.BossItemMaxCount = (byte)Colosseum[i]["BossItemMaxCount_b"].n;
                tmpInfo.rewardItemId = new JsonCustomData(Colosseum[i]["rewardItemId_j"].ToString());
                tmpInfo.clearRewardGold = (uint)Colosseum[i]["clearRewardGold_ui"].n;
                tmpInfo.clearRewardExp = (uint)Colosseum[i]["clearRewardExp_ui"].n;
                tmpInfo.LightmapType = (byte)Colosseum[i]["LightmapType_b"].n;
                tmpInfo.FixedReward = (uint)Colosseum[i]["FixedReward_ui"].n;

                ColosseumInfoDic.Add(tmpInfo.StageId, tmpInfo);
            }
        }

        //멀티 보스 레이드
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_MultyBossRaid", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject MultyBossRaid = new JSONObject(strSrc);

            for (int i = 0; i < MultyBossRaid.list.Count; i++)
            {
                MultyBossRaidInfo tmpInfo = new MultyBossRaidInfo();
                tmpInfo.raidId = (uint)MultyBossRaid[i]["raidId_ui"].n;
                tmpInfo.Type = (byte)MultyBossRaid[i]["Type_b"].n;
                tmpInfo.level = (ushort)MultyBossRaid[i]["level_us"].n;
                tmpInfo.stageString = (uint)MultyBossRaid[i]["stageString_ui"].n;
                tmpInfo.stageDesc = (uint)MultyBossRaid[i]["stageDesc_ui"].n;
                tmpInfo.MapIndex = (uint)MultyBossRaid[i]["MapIndex_ui"].n;
                tmpInfo.levelLimit = (byte)MultyBossRaid[i]["levelLimit_b"].n;
                tmpInfo.useValue = (byte)MultyBossRaid[i]["useValue_b"].n;
                tmpInfo.limitTime = (ushort)MultyBossRaid[i]["limitTime_us"].n;
                tmpInfo.clearExp = (uint)MultyBossRaid[i]["clearExp_ui"].n;
                tmpInfo.clearGold = (uint)MultyBossRaid[i]["clearGold_ui"].n;
                tmpInfo.BossIdx = (uint)MultyBossRaid[i]["BossIdx_ui"].n;
                tmpInfo.RegenCharacter = new JsonCustomData(MultyBossRaid[i]["RegenCharacter_j"].ToString());
                tmpInfo.BossSpawnpos = new JsonCustomData(MultyBossRaid[i]["BossSpawnpos_j"].ToString());
                tmpInfo.RewardId = new JsonCustomData(MultyBossRaid[i]["RewardId_j"].ToString());
                tmpInfo.RewardProb = new JsonCustomData(MultyBossRaid[i]["RewardProb_j"].ToString());
                tmpInfo.FightingPower = (uint)MultyBossRaid[i]["FightingPower_ui"].n;
                tmpInfo.ItemId = new JsonCustomData(MultyBossRaid[i]["ItemId_j"].ToString());
                tmpInfo.cameraType = (byte)MultyBossRaid[i]["cameraType_b"].n;
                tmpInfo.EnterPoint = (uint)MultyBossRaid[i]["EnterPoint_ui"].n;
                tmpInfo.EnterLevel = (byte)MultyBossRaid[i]["EnterLevel_b"].n;
                tmpInfo.FixedReward = (uint)MultyBossRaid[i]["FixedReward_ui"].n;

                MultyBossRaidInfoDic.Add(tmpInfo.raidId, tmpInfo);
            }
        }

        //컨텐츠 오픈조건
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_ContentsOpen", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ContentsOpen = new JSONObject(strSrc);

            for (int i = 0; i < ContentsOpen.list.Count; i++)
            {
                ContentsOpenInfo tmpInfo = new ContentsOpenInfo();
                tmpInfo.LogID = (uint)ContentsOpen[i]["LogID_ui"].n;
                tmpInfo.ContentsType = (byte)ContentsOpen[i]["ContentsType_b"].n;
                tmpInfo.ContentsIdx = (uint)ContentsOpen[i]["ContentsIdx_ui"].n;
                tmpInfo.ConditionType1 = (byte)ContentsOpen[i]["ConditionType1_b"].n;
                tmpInfo.ConditionValue1 = (uint)ContentsOpen[i]["ConditionValue1_ui"].n;
                tmpInfo.ConditionType2 = (byte)ContentsOpen[i]["ConditionType2_b"].n;
                tmpInfo.ConditionValue2 = (uint)ContentsOpen[i]["ConditionValue2_ui"].n;
                tmpInfo.OpenType = (byte)ContentsOpen[i]["OpenType_b"].n;

                ContentsOpenInfoList.Add(tmpInfo);
            }
        }

        //난투장 포인트
        {
            TextAsset data = Resources.Load("TestJson/Dungeon_FreeFightPoint", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject FreeFightPoint = new JSONObject(strSrc);

            for (int i = 0; i < FreeFightPoint.list.Count; i++)
            {
                FreeFightPointInfo tmpInfo = new FreeFightPointInfo();
                tmpInfo.LogID = (uint)FreeFightPoint[i]["LogID_ui"].n;
                tmpInfo.MinBattlePoint = (float)FreeFightPoint[i]["MinBattlePoint_f"].n;
                tmpInfo.MaxBattlePoint = (float)FreeFightPoint[i]["MaxBattlePoint_f"].n;
                tmpInfo.Point = (uint)FreeFightPoint[i]["Point_ui"].n;

                FreeFightPointInfoDic.Add(tmpInfo.LogID, tmpInfo);
            }
        }

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StageInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StageInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SingleBossRaidInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SingleBossRaidInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SkillInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SkillInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/FreefightInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, FreefightTableInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/EquipInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, EquipInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/TowerInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, TowerInfoList);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ChapterRewardInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ChapterRewardInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ColosseumInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ColosseumInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/MultyBossRaidInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, MultyBossRaidInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ContentsOpenInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ContentsOpenInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/FreeFightPointInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, FreeFightPointInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StageInfo>>("StageInfo", (data) => { StageInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, SingleBossRaidInfo>>("SingleBossRaidInfo", (data) => { SingleBossRaidInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<byte, SkillInfo>>("SkillInfo", (data) => { SkillInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, FreefightTableInfo>>("FreefightInfo", (data) => { FreefightTableInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<byte, EquipInfo>>("EquipInfo", (data) => { EquipInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<TowerInfo>>("TowerInfo", (data) => { TowerInfoList = data; });
		_LowDataMgr.instance.DeserializeData<List<ChapterRewardInfo>>("ChapterRewardInfo", (data) => { ChapterRewardInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, ColosseumInfo>>("ColosseumInfo", (data) => { ColosseumInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, MultyBossRaidInfo>>("MultyBossRaidInfo", (data) => { MultyBossRaidInfoDic = data; });
        _LowDataMgr.instance.DeserializeData<List<ContentsOpenInfo>>("ContentsOpenInfo", (data) => { ContentsOpenInfoList = data; });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, FreeFightPointInfo>>("FreeFightPointInfo", (data) => { FreeFightPointInfoDic = data; });
    }
}
