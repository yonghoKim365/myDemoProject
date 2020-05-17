using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 한 플레이어가 게임 플레이를 위해 필요한 초기 정보를 가지는 클래스 (InGame에 필요한 데이터)
/// </summary>
public class _PlayerSyncData
{
    //다수유저를 지원하기위한 리스트들
    public List<PlayerUnitData> playerSyncDatas = new List<PlayerUnitData>();

    //내 파트너 포함 다인이 들어오는데서 여러 파트너를 다 한번에 받기위한 리스트들
    //파트너정보 데이터
    public List<PlayerUnitData> partnerSyncDatas = new List<PlayerUnitData>();


    public void Init()
    {
        Reset();
    }


    public void Reset()
    {
        playerSyncDatas.Clear();
        partnerSyncDatas.Clear();
    }

    public PlayerUnitData FindPlayerSyncData(ulong a_Key = 0)
    {
        for (int a_ii = 0; a_ii < playerSyncDatas.Count; a_ii++)
        {
            if (playerSyncDatas[a_ii]._AccountUUID == a_Key)    //UUID
                return playerSyncDatas[a_ii];
        }

        return null;
    }
}

public class SkillData
{
    public uint _SkillID;
    public byte _SkillLevel;

    public SkillData(uint SkillID, byte SkillLevel)
    {
        _SkillID = SkillID;
        _SkillLevel = SkillLevel;
    }
}

public class PlayerUnitData
{
    public byte _TeamID = 0;
    public string _Name = string.Empty;

    //서버 고유번호 - 내유닛구별등에 써야됨
    public ulong _AccountUUID = 0;
    public ulong _TCPUUID = 0;

    //파트너일경우 슬롯번호
    public byte _SlotNo = 0;

    //외향으로 보여줄 현재 장착된 아이템들 - 어차피 유저의 장비품 포함된 스탯 정보 이기떄문에 다른아이템까지 다보내줄 필요가 없다.
    public uint _charIdx = 0; //유저 캐릭터의 종류
    public uint _HeadItem = 0;
    public uint _CostumeItem = 0;
    public uint _ClothItem = 0;
    public uint _WeaponItem = 0;
    public uint _Level = 0;
    public uint _SkillSetId;//스킬 셋트 아이디
    public bool _HideCostume = false;

    public bool _isPartner = false;
    public uint _partnerID = 0;

    public uint _Prefix;
    public uint _Suffix;

    //스킬과 스탯 정보들
    //평타0번과 스킬 1234번
    //public SkillData[] SkillData = new SkillData[5];
    //평타스킬 123
    public SkillData[] NormalAttackData = new SkillData[4];
    //스킬 0123번 유저체인스킬 4,5,6
    public SkillData[] SkillData = new SkillData[7];
    public Dictionary<AbilityType, float> _Stats; //유저의 장비품 포함한 스탯 정보들 - 서버에서 계산해준다

    public Vector3 netPlayerStartPoint;
    public float netPlayerStartRot;

    public virtual void Init( byte Slotno, byte teamId, string name, ulong accountUUID, ulong tcpUUID, uint charIdx, uint headItem, uint costumeItem, uint clothItem, uint weaponItem, uint skillSetId, bool hideCostume, uint level, 
        uint prefix, uint suffix, Dictionary<AbilityType, float> stats)
    {
        _TeamID = teamId;
        _Name = name;
        _AccountUUID = accountUUID;
        _TCPUUID = tcpUUID;
        _charIdx = charIdx;

        _SlotNo = Slotno;

        _HeadItem = headItem;
        _CostumeItem = costumeItem;
        _ClothItem = clothItem;
        _WeaponItem = weaponItem;
        _HideCostume = hideCostume;
        _Level = level;
        _SkillSetId = skillSetId;

        _isPartner = false;

        _Prefix = prefix;
        _Suffix = suffix;

        _Stats = stats;
    }

    public virtual void Init(byte Slotno, byte teamId, string name, ulong accountUUID, ulong tcpUUID, uint partnerID, uint level, Dictionary<AbilityType, float> stats)
    {
        _TeamID = teamId;
        _Name = name;
        _AccountUUID = accountUUID;
        _TCPUUID = tcpUUID;
        _partnerID = partnerID;

        _SlotNo = Slotno;

        _isPartner = true;
        _Level = level;

        _Stats = stats;
    }

    public float GetStats(AbilityType type)
    {
        if(_Stats.ContainsKey(type))
        {
            //가지고 있는 키면
            return _Stats[type];
        }
        else
        {
            //없는 키면
            return 0f;
        }
    }


    public virtual void Reset()
    {
        _SlotNo = 0;
        _TeamID = 0;
        _Name = string.Empty;
        _AccountUUID = 0;
        _TCPUUID = 0;
        _charIdx = 0;
        _HeadItem = 0;
        _CostumeItem = 0;
        _ClothItem = 0;
        _WeaponItem = 0;
        _Prefix = 0;
        _Suffix = 0;
        _SkillSetId = 0;
        _HideCostume = false;
        _Stats = null;
    }
}

