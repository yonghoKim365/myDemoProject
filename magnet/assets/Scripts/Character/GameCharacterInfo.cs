using UnityEngine;
using System.Collections.Generic;
using Kpro;

/// 게임 플레이시 필요한 각 캐릭터 상세 정보 및 데이터 저장
public class GameCharacterInfo
{
    public Unit Owner;

    public string UnitName;

    /// 유동적으로 변화하는 스탯들을 가지는 변수
    public Dictionary<AbilityType, Attribute> Stats = new Dictionary<AbilityType, Attribute>();

    /// 접근 편의성용
    public Attribute this[AbilityType stat]    { get { return Stats[stat]; } }

    public int Level
    {
        protected set { level = value; }
        get { return level; }
    }
    public uint Exp     { get { return startExp + AddedExp; } }
    public uint Gold    { get { return AddedGold; } }

    public uint CharIndex;//LowData Index Pc용이다
    public uint EquipHead;
    public uint EquipCostume;
    public uint EquipCloth;
    public uint EquipWeapon;
    public uint SkillSetId;

    //< 버프에 따른 추가 값
    public Dictionary<BuffType, float> BuffValue = new Dictionary<BuffType, float>();

    //< 패시브에 따른 추가값
    public Dictionary<ePassiveType, float> PassiveValue = new Dictionary<ePassiveType, float>();

    /// <summary>
    /// 공격력
    /// </summary>
    public float Atk { 
        get 
        {
            //< 기본스탯 + 버프계산
            float value = Stats[AbilityType.DAMAGE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.DAMAGE_INCREASE) * 0.01f), 0, value);
            
            //< 건물버프 + 계정레벨버프 계산
            //value += Stats[AbilityType.DAMAGE].GetFinalValue(1);
            //value *= 1f + Stats[AbilityType.DAMAGERate].GetFinalValue(1);

            //< 인게임 인스턴스 버프 + 스킬버프 계산
            //value *= 1f + Stats[AbilityType.DAMAGERate].GetFinalValue(2) + GetBuffValue(BuffType.Attack) + GetPassiveValue(ePassiveType.AttackRate);

            //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
            if (value < 0 || value > float.MaxValue * 0.9f)
                value = 10;

            return value; 
        } 
    }

    public float HitRate
    {
        get
        {
            float value = Stats[AbilityType.HIT_RATE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.HITRATE_INCREASE) * 0.01f), 0, value);

            //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
            if (value < 0 || value > float.MaxValue * 0.9f)
                value = 10;

            return value;
        }
    }

    public float CooltimeReduce
    {
        get
        {
            float value = Stats[AbilityType.COOLTIME].FinalValue;
            //value += Mathf.Clamp(value * (GetBuffValue(BuffType.) * 0.01f), 0, value);

            //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
            //if (value < 0 || value > float.MaxValue * 0.9f)
            //    value = 10;

            return value;
        }
    }

    public float DodgeRate
    {
        get
        {
            float value = Stats[AbilityType.DODGE_RATE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.DODGERATE_INCREASE) * 0.01f), 0, value);

            if (value < 0 || value > float.MaxValue * 0.9f)
                value = 10;

            return value;
        }
    }

    /// <summary>
    /// 방어력 무시 공격력
    /// </summary>
    public float DefIgnoreAtk
    {
	   get
	   {
		  float value = Stats[AbilityType.DEFENCE_IGNORE].FinalValue * 0.1f;

		  //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
		  if (value < 0 || value > float.MaxValue * 0.9f)
			 value = 10;

		  return value;
	   }
    }
    
    /// <summary>
    /// 데미지감소
    /// </summary>
    public float Def { 
        get 
        {
            //< 기본스탯 + 아이템능력치계산
            float value = Stats[AbilityType.DAMAGE_DECREASE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.DAMAGEREDUCE_INCRESE) * 0.01f), 0, value);
            //value += GetBuffValue(BuffType.DAMAGEREDUCE_INCRESE);

            ////< 건물버프 + 계정레벨버프 계산
            //value += Stats[AbilityType.DAMAGE_DECREASE].GetFinalValue(1);
            //value *= 1f + Stats[AbilityType.DAMAGE_DECREASERate].GetFinalValue(1);

            ////< 인게임 인스턴스 버프 + 스킬버프 계산
            //value *= 1f + Stats[AbilityType.DAMAGE_DECREASERate].GetFinalValue(2) + GetBuffValue(BuffType.Def) + GetPassiveValue(ePassiveType.DefenceRate);

            //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
            if (value < 0 || value > float.MaxValue * 0.9f)
                value = 10;

            return value; 
        } 
    }

    /// <summary>
    /// 데미지감소율
    /// </summary>
    public float DefRate
    {
	   get
	   {
		    float value = Stats[AbilityType.DAMAGE_DECREASE_RATE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.DAMAGEREDUCEPER_INCREASE) * 0.01f), 0, value);

            //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
            if (value < 0 || value > float.MaxValue * 0.9f)
			 value = 0;

		  return value; 
	   }
    }

    /// <summary>
    /// 최대생명력
    /// </summary>
    private int maxhp = 0;
    public int MaxHp
    {
        get
        {
            //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
            if(SceneManager.instance.IsRTNetwork)
            {
                return maxhp;
            }
            else
            {
                //< 기본스탯 + 아이템능력치계산
                float value = Stats[AbilityType.HP].FinalValue;

                ////< 건물버프 + 계정레벨버프 계산
                //value += Stats[AbilityType.HP].GetFinalValue(1);
                //value *= 1f + Stats[AbilityType.HPRate].GetFinalValue(1);

                ////< 인게임 인스턴스 버프 + 스킬버프 계산
                //value *= 1f + Stats[AbilityType.HPRate].GetFinalValue(2) + GetBuffValue(BuffType.MaxHp) + GetPassiveValue(ePassiveType.MaxHpRate);

                //< 혹시나 수치 연산 문제로 잘못값이 계산된다면 보정
                if (value < 0 || value > int.MaxValue * 0.9f)
                    value = 10;

                return (int)value;
            }
        }
        set {
            maxhp = value;
        }
    }
    
    /// <summary>
    /// 현재생명력
    /// </summary>
    public int Hp
    {
        set { hp = Mathf.Clamp( value, 0, MaxHp ); }
        get { return hp; }
    }

    /// <summary>
    /// 공격속도
    /// </summary>
    public float AtkSpeed
    {
	   get
	   {
			float val = Stats[AbilityType.ATTACK_SPEED].FinalValue;

			// calc buff
			val += Mathf.Clamp(val * (GetBuffValue(BuffType.ATTACKSPEED_INCREASE) * 0.01f), 0, val);
            val = Mathf.Clamp(val, 1f, 1.3f);

            // attackSpeed max value?

            //value += value * pecent;
            if (Owner.Animator.Animation != null)
				AnimationUtil.SetAnimationSpeed(Owner.Animator.Animation, val, GetAttackAnims());
			return val;
	   }
    }                //< 공격 속도
    /// <summary>
    /// 이동속도
    /// </summary>
    public float MoveSpeed{ 
        get 
        {
            return Stats[AbilityType.MOVE_SPEED].FinalValue;
        } 
    }               //< 캐릭터 이동 속도
    /// <summary>
    /// 공격 유효 거리
    /// </summary>
    public float AtkRange
    { 
        get 
        {
            return Stats[AbilityType.ATTACK_RANGE].FinalValue; 
        } 
    }                //< 공격 유효 거리
    /// <summary>
    /// 공격 범위 각도
    /// </summary>
    public float AtkAngle 
    { 
        get 
        { 
            return Stats[AbilityType.ATTACK_ANGLE].FinalValue;
        } 
    }               //< 공격 범위 각도
    /// <summary>
    /// 첫번째 공격시 붙을 거리
    /// </summary>
    public float FirstAtkRange
    { set; get; }           //< 첫번째 공격시 붙을 거리
    float _AtkDelay;                        
    /// <summary>
    /// 공격간 딜레이
    /// </summary>
    public float AtkDelay
    { 
        set 
        { 
            _AtkDelay = value; 
        } 
        get 
        {
            return _AtkDelay - (_AtkDelay * GetBuffValue(BuffType.AttackSpeed)); 
        }
    }                //< 공격간 딜레이
    /// <summary>
    /// 적 인지 범위
    /// </summary>
    public float AtkRecognition
    { set; get; }          //< 적 인지 범위
    /// <summary>
    /// 러쉬 공격 시작 거리
    /// </summary>
    public float RushAtkRange
    { set; get; }            //< 러쉬 공격 시작 거리
    /// <summary>
    /// 치명타 확률
    /// </summary>
    public float CriticalChance
    {
        get
        {
            float value = Stats[AbilityType.CRITICAL_RATE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.CRITICALRATE_INCREASE) * 0.01f), 0, value);
            return value;
        }
    }          //< 크리티컬 확률
    /// <summary>
    /// 치명타 피해
    /// </summary>
    public float CriticalDmgRate
    {
        get
        {
            float value = Stats[AbilityType.CRITICAL_DAMAGE].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.CRITICALDAMAGE_INCREASE) * 0.01f), 0, value);
            return value;
        }
    }         //< 크리티컬 데미지 

    public float CriticalRes
    {
        get
        {
            float value = Stats[AbilityType.CRITICAL_RES].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.CRITICALRES_INCREASE) * 0.01f), 0, value);
            return value;
        }
    }//< 크리티컬 데미지 증가율

    public float DrainHP
    {
        get
        {
            float value = Stats[AbilityType.DRAIN_HP].FinalValue;
            value += Mathf.Clamp(value * (GetBuffValue(BuffType.LIFESTEAL_INCREASE) * 0.01f), 0, value);
            return value;
        }
    }

    /////////////////////////////////////////////////////////
    //슈퍼아머 관련값 시작
    /////////////////////////////////////////////////////////
    uint maxSuperArmor = 0;
    public uint MaxSuperArmor
    {
        get
        {
            if(SceneManager.instance.IsRTNetwork)
            {
                return maxSuperArmor;
            }
            else
            {
                return (uint)Stats[AbilityType.SUPERARMOR].FinalValue;
            }
        }

        set
        {
            maxSuperArmor = value;
        }
    }

    private uint _superArmor = 0;
    public uint SuperArmor
    {
        get
        {
            return _superArmor;
        }

        set
        {
            _superArmor = value;
        }
    }

    public float SuperArmor_Recovery
    {
        get
        {
            return Stats[AbilityType.SUPERARMOR_RECOVERY].FinalValue;
        }
    }

    public float SuperArmor_RecoveryRate
    {
        get
        {
            return Stats[AbilityType.SUPERARMOR_RECOVERY_RATE].FinalValue;
        }
    }

    public float SuperArmor_RecoveryTime
    {
        get
        {
            return Stats[AbilityType.SUPERARMOR_RECOVERY_TIME].FinalValue;
        }
    }
    /////////////////////////////////////////////////////////
    //슈퍼아머 관련값 끝
    /////////////////////////////////////////////////////////

    //무게 추가
    public float Weight
    {
        get
        {
            return Mathf.Clamp(Stats[AbilityType.WEIGHT].FinalValue, 1, 10);
        }
    }


    //public float RegenHp
    //{ get { return Stats[AbilityType.HpRegen].FinalValue; } }                 //< 자동재생력

    public MoveType OrignalMoveType
    { protected set; get; }      //< 데이터상 이동타입
    public MoveType CurMoveType
    { set; get; }          //< 현재 이동타입
    public bool IsDead
    { get { return hp <= 0; } }

    /// <summary>
    /// 대쉬 공격 가능여부. 황비홍에서 대쉬류 공격은 스킬로 빠지기 때문에 평타 대쉬 자체가 없으므로 모두 펄스 처리
    /// </summary>
    public bool CanDashAttack
    { protected set; get; }            //< 대쉬 공격 가능 여부
    public int MaxAnimCombo;                //< 맥스 콤보 카운트(애니메이션에 따라 맥스콤보 숫자(어택애니카운트)

    public eAttackType AttackType;          //< 공격 가능 타입 (접근, 원거리, 모두)
    public float AirHeight;                 //< 지상으로부터 떠 있을 높이.
    public float AirFlyTime;                //< 지상으로부터 떠 있을 시간.
        
    /// 해당 캐릭터의 애니메이션 이름들
    public Resource.AniInfo[]    animDatas = new Resource.AniInfo[(int)eAnimName.Anim_Max];

    // Pure Attribute    
    private int         hp = 0;
    private int         savedMaxHp = 0;
    private int         level = 1;
    public uint         startExp = 0;
    private uint        AddedExp = 0;
    private uint        AddedGold = 0;

    public float GetBuffValue(BuffType type)
    {
        if (BuffValue.ContainsKey(type))
            return BuffValue[type];

        return 0;
    }

    public float GetPassiveValue(ePassiveType type)
    {
        if (PassiveValue.ContainsKey(type))
            return PassiveValue[type];

        return 0;
    }

    public void AddPassiveValue(ePassiveType type, float value)
    {
        if (!PassiveValue.ContainsKey(type))
            PassiveValue.Add(type, 0);

        PassiveValue[type] += value;
    }

    /// 대상 객체에 맞게 정보들을 채우도록 한다.
    public void Init(Unit owner)
    {
        this.Owner = owner;

        switch (owner.UnitType)
        {
            case UnitType.TownUnit:
            case UnitType.TownNpc:
            case UnitType.TownNINPC:
            {
                Pc pc = owner as Pc;

                if (pc != null && pc.IsPartner == true)
                    SetDefaultNoneStat(700, 500, 35000, 0, 7); // 파트너 공방체
                else
                    SetDefaultNoneStat(1000, 500, 35000); // 캐릭터 공방체
            }
            break;
            case UnitType.Unit:
                {
                    Pc pc = owner as Pc;

                    if (pc != null && pc.IsPartner == true)
                        SetStatPC(pc);
                        //SetStatPC(700, 500, 35000, 0, 7); // 파트너 공방체
                    else
                        SetStatPC(pc);
                        //SetDefaultNoneStat(1000, 500, 35000); // 캐릭터 공방체
                }
                break;

            case UnitType.Npc:
            case UnitType.Boss:
            {
                Npc npc = owner as Npc;
                SetMonsterStat(npc.npcInfo.Id, npc.npcInfo.Level);
            }
            break;

            case UnitType.Prop:
            {
                Prop prop = owner as Prop;
                if (prop == null || prop.PropInfo == null)
                {
                    Debug.LogError("not unit cs error " + owner.gameObject.name);
                    return;
                }

                SetStatusProp(prop.PropInfo.Id);
            }
            break;

        case UnitType.Trap:
        {
            //SetStatusTrap();
            SetDefaultNoneStat();
        }
        break;
        }

        //MaxAnimCombo = UnitDataFactory.CountingMaxAnimCombo(animDatas);
        //SetupStatProcess();
    }

    //void SetStatusNoneData(Unit unit)
    //{


    //    SetLevel(1);

	   //CanDashAttack = false;// null != GetAnimData(eAnimName.Anim_dash) && RushAtkRange > 0f;
    //}

    // PC 기본 스탯 정보 대입
    //void SetStatusPC(Pc pc)
    //{   
    //    startExp = pc.syncData.Exp;
    //    Stats = pc.syncData.Stats;

    //    SetDefaultStat( LowDataMgr.GetCompositionDataOfUnit( pc.syncData.LowID, pc.syncData.Grade, pc.syncData.AwakenType ) );

    //    int level = DataManager.GetUnitLevel( pc.syncData.Grade, pc.syncData.Exp );

    //    SetLevel( level );

	   //CanDashAttack = false;// null != GetAnimData(eAnimName.Anim_dash) && RushAtkRange > 0f;   // Dash 애니가 있으면 attack_f
    //}

    // Npc에 맞는 스탯정보 셋팅
    //void SetStatusNpc(Npc npc)
    //{
    //    LowDataMgr.NpcCompositionData comData = LowDataMgr.GetCompositionDataOfNpc( npc.NpcLowID );

    //    startExp = (uint)0;
    //    Stats = GK_StatGenerator.GenerateDefaultStats( comData );
        
    //    SetDefaultStat( comData );
    //    SetLevel( 1 );

	   //CanDashAttack = false;// null != GetAnimData(eAnimName.Anim_dash) && RushAtkRange > 0f;
    //}

    // Prop 기본 스탯 정보 대입
    //void SetStatusProp(Prop prop)
    //{
    //    LowDataMgr.NpcCompositionData comData = LowDataMgr.GetCompositionDataOfNpc(prop.NpcLowID);

    //    Stats = GK_StatGenerator.GenerateDefaultStats(comData);
    //    startExp = 0;
    //    Level = 1;
    //    SetDefaultStat(comData);

    //    CanDashAttack = false;
    //}

    // Trap 기본 스탯 정보 대입
    //void SetStatusTrap()
    //{
    //    Stats = GK_StatGenerator.GenerateDefaultStats();

    //    startExp = 0;
    //    Level = 1;

    //    CanDashAttack = false;
    //}


    //PC일경우 셋팅용
    void SetStatPC(Pc pc)
    {
        FirstAtkRange = 1.04f;
        AtkDelay = 0;
        //AtkRecognition = 20f;
        RushAtkRange = 0;

        CurMoveType = OrignalMoveType = (MoveType)1;
        AttackType = eAttackType.Melee;
        AirHeight = 0;
        AirFlyTime = 0;

        UnitName = pc.syncData._Name;
        CharIndex = pc.IsPartner ? pc.syncData._partnerID : pc.syncData._charIdx;
        
        //장착정보
        EquipCostume = pc.syncData._HideCostume ? 0 : pc.syncData._CostumeItem;
        EquipCloth = pc.syncData._ClothItem;
        EquipWeapon = pc.syncData._WeaponItem;
        EquipHead = pc.syncData._HeadItem;
        SkillSetId = pc.syncData._SkillSetId;

        float atkRange = 2;

        if (pc.IsPartner)
        {
            //파트너일경우
            if (_LowDataMgr.instance.GetPartnerInfo(pc.syncData._partnerID).AiType == 1)
            {
                //밀리
                atkRange = 2;
            }
            else
            {
                //원거리
                atkRange = 5;
            }

        }

        if(G_GameInfo.GameMode == GAME_MODE.SPECIAL_EXP || G_GameInfo.GameMode == GAME_MODE.SPECIAL_GOLD)
        {
            AtkRecognition = 40f;
        }
        else
            AtkRecognition = 20f;

        float value = pc.syncData.GetStats(AbilityType.HP);
        Stats.Add(AbilityType.HP, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DAMAGE);
        Stats.Add(AbilityType.DAMAGE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.HIT_RATE);
        Stats.Add(AbilityType.HIT_RATE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DODGE_RATE);
        Stats.Add(AbilityType.DODGE_RATE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.CRITICAL_RATE);
        Stats.Add(AbilityType.CRITICAL_RATE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.CRITICAL_RES);
        Stats.Add(AbilityType.CRITICAL_RES, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.CRITICAL_DAMAGE);
        Stats.Add(AbilityType.CRITICAL_DAMAGE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DRAIN_HP);
        Stats.Add(AbilityType.DRAIN_HP, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DEFENCE_IGNORE);
        Stats.Add(AbilityType.DEFENCE_IGNORE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DAMAGE_DECREASE);
        Stats.Add(AbilityType.DAMAGE_DECREASE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.DAMAGE_DECREASE_RATE);
        Stats.Add(AbilityType.DAMAGE_DECREASE_RATE, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.COOLTIME);
        Stats.Add(AbilityType.COOLTIME, new Attribute(value));

        value = pc.syncData.GetStats(AbilityType.ATTACK_SPEED);
        Stats.Add(AbilityType.ATTACK_SPEED, new Attribute(value));

        //value = pc.syncData.GetStats(AbilityType.ATTACK_RANGE);
        Stats.Add(AbilityType.ATTACK_RANGE, new Attribute(atkRange));

        //value = pc.syncData.GetStats(AbilityType.ATTACK_ANGLE);
        Stats.Add(AbilityType.ATTACK_ANGLE, new Attribute(140));

        //value = pc.syncData.GetStats(AbilityType.MOVE_SPEED);
        Stats.Add(AbilityType.MOVE_SPEED, new Attribute(7.5f));

        //value = pc.syncData.GetStats(AbilityType.DETECTED_RANGE);
        Stats.Add(AbilityType.DETECTED_RANGE, new Attribute(15f));

        Stats.Add(AbilityType.SUPERARMOR, new Attribute(pc.syncData.GetStats(AbilityType.SUPERARMOR)));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, new Attribute(pc.syncData.GetStats(AbilityType.SUPERARMOR_RECOVERY_TIME)));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, new Attribute(pc.syncData.GetStats(AbilityType.SUPERARMOR_RECOVERY_RATE)));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY, new Attribute(pc.syncData.GetStats(AbilityType.SUPERARMOR_RECOVERY)));

        Stats.Add(AbilityType.WEIGHT, new Attribute(pc.syncData.GetStats(AbilityType.WEIGHT)));

        savedMaxHp = Hp = (int)Stats[AbilityType.HP].FinalValue;
        SuperArmor = (uint)Stats[AbilityType.SUPERARMOR].FinalValue;
    }

    void SetStatusProp(uint propID)
    {
        // enthan Base Stats
        FirstAtkRange = 1.04f;
        AtkDelay = 1f;
        AtkRecognition = 20f;
        RushAtkRange = 0;

        CurMoveType = OrignalMoveType = (MoveType)1;
        AttackType = eAttackType.Melee;
        AirHeight = 0;
        AirFlyTime = 0;

        Mob.PropInfo propData = _LowDataMgr.instance.GetPropInfo(propID);

        Stats.Add(AbilityType.HP, new Attribute(propData.BaseHp));
        Stats.Add(AbilityType.DAMAGE, new Attribute(propData.BaseAtk));
        Stats.Add(AbilityType.HIT_RATE, new Attribute(propData.BaseHit));
        Stats.Add(AbilityType.DODGE_RATE, new Attribute(propData.BaseAvoid));
        Stats.Add(AbilityType.CRITICAL_RATE, new Attribute(propData.BaseCriticalRate));
        Stats.Add(AbilityType.CRITICAL_RES, new Attribute(propData.BaseCriticalResist));
        Stats.Add(AbilityType.CRITICAL_DAMAGE, new Attribute(propData.BaseCriticalDamage));
        Stats.Add(AbilityType.DRAIN_HP, new Attribute(propData.BaseLifeSteal));
        Stats.Add(AbilityType.DEFENCE_IGNORE, new Attribute(propData.BaseIgnoreAtk));
        Stats.Add(AbilityType.DAMAGE_DECREASE, new Attribute(propData.BaseDamageDown));
        Stats.Add(AbilityType.DAMAGE_DECREASE_RATE, new Attribute(propData.BaseDamageDownRate));
        Stats.Add(AbilityType.COOLTIME, new Attribute(0));

        Stats.Add(AbilityType.ATTACK_SPEED, new Attribute(1));
        Stats.Add(AbilityType.ATTACK_RANGE, new Attribute(2));
        Stats.Add(AbilityType.ATTACK_ANGLE, new Attribute(140));

        Stats.Add(AbilityType.MOVE_SPEED, new Attribute(7.5f));
        Stats.Add(AbilityType.DETECTED_RANGE, new Attribute(15f));

        Stats.Add(AbilityType.SUPERARMOR, new Attribute(0));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, new Attribute(0));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, new Attribute(0));
        Stats.Add(AbilityType.SUPERARMOR_RECOVERY, new Attribute(0));

        Stats.Add(AbilityType.WEIGHT, new Attribute(0));

        savedMaxHp = Hp = (int)Stats[AbilityType.HP].FinalValue;
        //프롭은 슈퍼아머 없음
        SuperArmor = 0;
    }

    //마을등의 유닛
    void SetDefaultNoneStat(float atk = 100, float def = 100, float hp = 100, float atkDelay = 0, float atkRange = 4)
    {
        // enthan Base Stats
        FirstAtkRange = 1.04f;
        AtkDelay = 1f;
        AtkRecognition = 20f;
        RushAtkRange = 0;

        CurMoveType = OrignalMoveType = (MoveType)1;
        AttackType = eAttackType.Melee;
        AirHeight = 0;
        AirFlyTime = 0;

        {
            Stats.Add(AbilityType.HP, new Attribute(10000));
            Stats.Add(AbilityType.DAMAGE, new Attribute(100));
            Stats.Add(AbilityType.HIT_RATE, new Attribute(0));
            Stats.Add(AbilityType.DODGE_RATE, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_RATE, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_RES, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_DAMAGE, new Attribute(0));
            Stats.Add(AbilityType.DRAIN_HP, new Attribute(0));
            Stats.Add(AbilityType.DEFENCE_IGNORE, new Attribute(0));
            Stats.Add(AbilityType.DAMAGE_DECREASE, new Attribute(100));
            Stats.Add(AbilityType.DAMAGE_DECREASE_RATE, new Attribute(0));
            Stats.Add(AbilityType.COOLTIME, new Attribute(0));

            Stats.Add(AbilityType.ATTACK_SPEED, new Attribute(1));
            Stats.Add(AbilityType.ATTACK_RANGE, new Attribute(2));
            Stats.Add(AbilityType.ATTACK_ANGLE, new Attribute(140));

            Stats.Add(AbilityType.MOVE_SPEED, new Attribute(7.5f));
            Stats.Add(AbilityType.DETECTED_RANGE, new Attribute(15f));
            Stats.Add(AbilityType.SUPERARMOR, new Attribute(1f));
            

            savedMaxHp = Hp = (int)Stats[AbilityType.HP].FinalValue;
        }
    }

    void SetMonsterStat(uint npcID, uint npcLevel)
    {
        //float atkDelay = 0;
        float atkRange = 2;
        // enthan Base Stats
        FirstAtkRange = 1.04f;
        AtkDelay = 1f;
        AtkRecognition = 20f;
        RushAtkRange = 0;

        CurMoveType = OrignalMoveType = (MoveType)1;
        AttackType = eAttackType.Melee;
        AirHeight = 0;
        AirFlyTime = 0;

        Mob.MobInfo monsterData = _LowDataMgr.instance.GetMonsterInfo(npcID);

        if (monsterData.AiType == 1)
        {
            //밀리
            atkRange = 2;
        }
        else
        {
            //원거리
            atkRange = 8;
        }

        if(monsterData == null)
        {
            //몬스터 데이터가 없다
            Debug.LogWarning(string.Format("Not Found MonsterData:{0}", npcID));

            Stats.Add(AbilityType.HP, new Attribute(100));
            Stats.Add(AbilityType.DAMAGE, new Attribute(100));
            Stats.Add(AbilityType.HIT_RATE, new Attribute(0));
            Stats.Add(AbilityType.DODGE_RATE, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_RATE, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_RES, new Attribute(0));
            Stats.Add(AbilityType.CRITICAL_DAMAGE, new Attribute(0));
            Stats.Add(AbilityType.DRAIN_HP, new Attribute(0));
            Stats.Add(AbilityType.DEFENCE_IGNORE, new Attribute(0));
            Stats.Add(AbilityType.DAMAGE_DECREASE, new Attribute(100));
            Stats.Add(AbilityType.DAMAGE_DECREASE_RATE, new Attribute(0));
            Stats.Add(AbilityType.COOLTIME, new Attribute(0));

            Stats.Add(AbilityType.ATTACK_SPEED, new Attribute(1));
            Stats.Add(AbilityType.ATTACK_RANGE, new Attribute(atkRange));
            Stats.Add(AbilityType.ATTACK_ANGLE, new Attribute(140));

            Stats.Add(AbilityType.MOVE_SPEED, new Attribute(7.5f));
            Stats.Add(AbilityType.DETECTED_RANGE, new Attribute(15f));

            Stats.Add(AbilityType.SUPERARMOR, new Attribute(0));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, new Attribute(0));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, new Attribute(0));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY, new Attribute(0));

            savedMaxHp = Hp = (int)Stats[AbilityType.HP].FinalValue;
            SuperArmor = (uint)Stats[AbilityType.SUPERARMOR].FinalValue;

            Stats.Add(AbilityType.WEIGHT, new Attribute(0));
        }
        else
        {
            Stats.Add(AbilityType.HP, new Attribute( (float)(monsterData.BaseHp + npcLevel * monsterData.LevelUpHp) * 0.1f) );
            Stats.Add(AbilityType.DAMAGE, new Attribute( (float)(monsterData.BaseAtk + npcLevel * monsterData.LevelUpAtk) * 0.1f));
            Stats.Add(AbilityType.HIT_RATE, new Attribute( (float)(monsterData.BaseHit + npcLevel * monsterData.LevelHitRate) * 0.1f));
            Stats.Add(AbilityType.DODGE_RATE, new Attribute( (float)(monsterData.BaseAvoid + npcLevel * monsterData.LevelAvoidRate) * 0.1f));
            Stats.Add(AbilityType.CRITICAL_RATE, new Attribute(monsterData.BaseCriticalRate * 0.1f));
            Stats.Add(AbilityType.CRITICAL_RES, new Attribute(monsterData.BaseCriticalResist * 0.1f));
            Stats.Add(AbilityType.CRITICAL_DAMAGE, new Attribute(monsterData.BaseCriticalDamage * 0.1f));
            Stats.Add(AbilityType.DRAIN_HP, new Attribute(monsterData.BaseLifeSteal * 0.1f));
            Stats.Add(AbilityType.DEFENCE_IGNORE, new Attribute(monsterData.BaseIgnoreAtk));
            Stats.Add(AbilityType.DAMAGE_DECREASE, new Attribute( (monsterData.BaseDamageDown + npcLevel * monsterData.LevelupDamageDown) * 0.1f));
            Stats.Add(AbilityType.DAMAGE_DECREASE_RATE, new Attribute( (monsterData.BaseDamageDownRate + npcLevel * monsterData.LevelupDamageDownRate) * 0.1f));

            Stats.Add(AbilityType.COOLTIME, new Attribute(0));

            Stats.Add(AbilityType.ATTACK_SPEED, new Attribute(1));
            Stats.Add(AbilityType.ATTACK_RANGE, new Attribute(atkRange));
            Stats.Add(AbilityType.ATTACK_ANGLE, new Attribute(140));

            Stats.Add(AbilityType.MOVE_SPEED, new Attribute(7.5f));

            Stats.Add(AbilityType.DETECTED_RANGE, new Attribute(15f));

            Stats.Add(AbilityType.SUPERARMOR, new Attribute(monsterData.BaseSuperArmor));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY_TIME, new Attribute(monsterData.SuperArmorRecoveryTime));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY_RATE, new Attribute(monsterData.SuperArmorRecoveryRate));
            Stats.Add(AbilityType.SUPERARMOR_RECOVERY, new Attribute(monsterData.SuperArmorRecovery));

            Stats.Add(AbilityType.WEIGHT, new Attribute(monsterData.Weight));

            savedMaxHp = Hp = (int)Stats[AbilityType.HP].FinalValue;
            SuperArmor = (uint)Stats[AbilityType.SUPERARMOR].FinalValue;
        }
    }

    //< 퍼센트에 맞게 능력치를 재설정해준다.(1 : Max)
    public void SetStatusPercent(float percent)
    {
        // Unit 데이터에 존재하는 기본값 할당
        Stats[AbilityType.DAMAGE].Value *= percent;
        Stats[AbilityType.DAMAGE_DECREASE].Value *= percent;
        Stats[AbilityType.HP].Value *= percent;
        Stats[AbilityType.CRITICAL_RATE].Value *= percent;
        Stats[AbilityType.CRITICAL_DAMAGE].Value *= percent;
    }

    //< 해당 정보의 퍼센트만큼 값을 대입시켜준다
    public void SetTargetStatus(Dictionary<AbilityType, Attribute> TargetStats, float percent)
    {
        Stats[AbilityType.DAMAGE].Value = TargetStats[AbilityType.DAMAGE].Value * percent;
        Stats[AbilityType.DAMAGE_DECREASE].Value = TargetStats[AbilityType.DAMAGE_DECREASE].Value * percent;
        Stats[AbilityType.HP].Value = TargetStats[AbilityType.HP].Value * percent;

        hp = (int)Stats[AbilityType.HP].Value;
    }

    //< 해당 스탯이 변경될때마다 호출되는 함수
    void SetupStatProcess()
    {
        // 이동 속도 변경에 따른 처리
        Stats[AbilityType.MOVE_SPEED].FinalValueChanged += (attr) => {
            if (null != Owner.Animation)
                AnimationUtil.SetAnimationSpeed( Owner.Animation, ( attr as Attribute ).FinalValue / Stats[AbilityType.MOVE_SPEED].Value, GetAnimData( eAnimName.Anim_move ).aniName );
        };

        // 최대 체력 증/감에 따른 처리
        Stats[AbilityType.HP].FinalValueChanged += (attr) => {
            int addHp = MaxHp - savedMaxHp;
            if (addHp > 0 && !IsDead)
            { 
                Hp += addHp;
            }

            savedMaxHp = MaxHp;
        };

        // 스피드 변경시, 애니메이션 속도 조절되도록 변경
        Stats[AbilityType.ATTACK_SPEED].FinalValueChanged += (attr) => 
        {
            if (null != Owner.Animation)
                AnimationUtil.SetAnimationSpeed( Owner.Animation, ( attr as Attribute ).FinalValue, GetAttackAnims() );
        };
    }

    /// 획득한 자산들 모으기
    public void AddExpAndGold(uint addExp, uint addGold)
    {
        if (addExp > 0)
        {
            AddedExp += addExp;

            //황비홍 프로젝트에 맞게 수정해야함
            //ushort calcLv = (ushort)Level;
            //if (Owner.UnitType == UnitType.Unit)
            //    calcLv = DataManager.GetUnitLevel( (Owner as Pc).syncData.Grade, Exp );

            //if (Level != calcLv)
            //{
            //    Debug.LogWarning( "LevelUP!! : " + Owner + " : " + Level + " >>> " + calcLv );
            //    Level = calcLv;

            //    if (Owner is Pc)
            //    {
            //        Pc pc = Owner as Pc;
            //        SetLevel( Level );
            //        pc.LevelUpEffect();
            //    }
            //    else if (Owner is Npc)
            //    {
            //        SetLevel( Level );
            //    }
            //}
        }

        if (addGold > 0)
        {
            if (GameDefine.TestMode)
                Debug.Log( "총 획득 골드 : " + (addGold + (uint)( (float)addGold )) );
            
            AddedGold += addGold + (uint)( (float)addGold );
        }
    }

    public uint GetAddExp()
    {
        return AddedExp;
    }

    public Resource.AniInfo GetAnimData(eAnimName animEvent)
    {
        return animDatas[(int)animEvent];
    }

    public string[] GetAttackAnims()
    {
        
        // 공격용 애니메이션만 리스트화
        return new string[] { 
            animDatas[(int)eAnimName.Anim_attack1] != null ? animDatas[(int)eAnimName.Anim_attack1].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_attack2] != null ? animDatas[(int)eAnimName.Anim_attack2].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_attack3] != null ? animDatas[(int)eAnimName.Anim_attack3].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_attack4] != null ? animDatas[(int)eAnimName.Anim_attack4].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_skill1] != null ? animDatas[(int)eAnimName.Anim_skill1].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_skill2] != null ? animDatas[(int)eAnimName.Anim_skill2].aniName : string.Empty,
            animDatas[(int)eAnimName.Anim_skill3] != null ? animDatas[(int)eAnimName.Anim_skill3].aniName : string.Empty,
		  animDatas[(int)eAnimName.Anim_skill4] != null ? animDatas[(int)eAnimName.Anim_skill4].aniName : string.Empty,
		  animDatas[(int)eAnimName.Anim_skill5] != null ? animDatas[(int)eAnimName.Anim_skill5].aniName : string.Empty,
		  animDatas[(int)eAnimName.Anim_skill6] != null ? animDatas[(int)eAnimName.Anim_skill6].aniName : string.Empty,
		  animDatas[(int)eAnimName.Anim_skill7] != null ? animDatas[(int)eAnimName.Anim_skill7].aniName : string.Empty,
		  animDatas[(int)eAnimName.Anim_skill8] != null ? animDatas[(int)eAnimName.Anim_skill8].aniName : string.Empty,
	 	  animDatas[(int)eAnimName.Anim_Chain] != null ? animDatas[(int)eAnimName.Anim_Chain].aniName : string.Empty,
       };
    }

    #region PC에서 능력치 보기위함
    
    public override string ToString()
    {
        return "";
        string str = base.ToString() + string.Format( " : Level [ {0} ] | Exp [ {1} ] | HP [ {2}/{3} ] \n", Level, Exp, Hp, MaxHp );

        //str += GK_StatGenerator.PrintStat( ref Stats );

        //< 스킬 정보를 먼저 출력시켜줌
        foreach(KeyValuePair<BuffType, float> value in BuffValue)
        {
            if (value.Value != 0)
                str += string.Format("<b>[" + value.Key.ToString() + "]</b>" + " : TVal {0}, TMul {1}%, FinalVal : {2} \n", 0, value.Value, value.Value);
        }

        //< 순서대로 호출하기위함
        str += GetAbilityStr("Attack", "<color=white>", AbilityType.DAMAGE, AbilityType.NONE, Atk);

        str += GetAbilityStr("Defence", "<color=white>", AbilityType.DAMAGE_DECREASE, AbilityType.DAMAGE_DECREASE_RATE, Def);
        str += GetAbilityStr("MaxHp", "<color=white>", AbilityType.HP, AbilityType.NONE, MaxHp);
        str += GetAbilityStr("CriticalChance", "<color=white>", AbilityType.NONE, AbilityType.CRITICAL_RATE, CriticalChance);
        str += GetAbilityStr("CriticalDmgRate", "<color=white>", AbilityType.NONE, AbilityType.CRITICAL_DAMAGE, CriticalDmgRate);
        
        str += GetAbilityStr("FirstAtkRange", "<color=yellow>", AbilityType.NONE, AbilityType.NONE, FirstAtkRange);
        str += GetAbilityStr("RushAtkRange", "<color=yellow>", AbilityType.NONE, AbilityType.NONE, RushAtkRange);
        str += GetAbilityStr("MoveSpeed", "<color=yellow>", AbilityType.MOVE_SPEED, AbilityType.NONE, MoveSpeed);
        str += GetAbilityStr("AtkSpeed", "<color=yellow>", AbilityType.ATTACK_SPEED, AbilityType.NONE, AtkSpeed);

        return str;
    }

    string[] ValueStr = { "UnitLevelAbility", "Equipment", "EquipmentOption", "BuildBuff", "AccountBuff", "SkillBuff", "TrapBuff", "InstantBuff", "GuildBuff", "MasteryBuff" };
    public string GetAbilityStr(string name, string color, AbilityType type1, AbilityType type2, float FinalVal)
    {
        Attribute[] _Attribute = new Attribute[2];
        foreach (KeyValuePair<AbilityType, Attribute> pair in Stats)
        {
            if (pair.Key == type1)
                _Attribute[0] = pair.Value;

            if (pair.Key == type2)
                _Attribute[1] = pair.Value;
        }

        List<BaseAttribute> value = new List<BaseAttribute>();
        value.Add(null);
        value.Add(null);

        string str = "";

        float TVal = 0, TMul = 0;
        TVal = _Attribute[0] != null ? _Attribute[0].Value : 0;
        TMul = _Attribute[1] != null ? _Attribute[1].Value : 0;

        for (int i = 0; i < ValueStr.Length; i++)
        {
            value[0] = _Attribute[0] != null ? _Attribute[0].GetValue(ValueStr[i]) : null;
            value[1] = _Attribute[1] != null ? _Attribute[1].GetValue(ValueStr[i]) : null;

            TVal += value[0] != null ? value[0].Value : 0;
            TMul += value[1] != null ? value[1].Value : 0;
        }

        //< 최종값 대입
        str += color + "<b>[ " + name + " ]</b> : " + string.Format("TVal {0}, TMul {1}%, FinalVal : {2} \n", TVal, TMul, FinalVal) + "</color>";

        //< 기본값 대입
        if (TVal != 0 || TMul != 0)
            str += color + string.Format(" - UnitAbility" + " : Val {0}, Mul {1}%\n", _Attribute[0].Value, 0) + "</color>";

        //< 그외 값들 대입
        for (int i = 0; i < ValueStr.Length; i++ )
        {
            value[0] = _Attribute[0] != null ? _Attribute[0].GetValue(ValueStr[i]) : null;
            value[1] = _Attribute[1] != null ? _Attribute[1].GetValue(ValueStr[i]) : null;
            if (value[0] != null || value[1] != null)
                str += color + string.Format(" - " + ValueStr[i] + " : Val {0}, Mul {1}%\n", value[0] != null ? value[0].Value : 0, value[1] != null ? value[1].Value : 0) + "</color>";
        }

        return str;
    }

    #endregion
}