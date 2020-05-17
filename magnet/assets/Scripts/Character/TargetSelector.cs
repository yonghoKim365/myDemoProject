using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// GoldenKnights를 위한 적절한 타겟을 찾아주기 위한 클래스
/// </summary>
/// <remarks>
/// 종속성 없애는건...다음에~
/// </remarks>
public class TargetSelector
{
    public class AggroData : IComparable
    {
        public float LastAttackT;
        public float Point;
        public Unit Attacker;

        int IComparable.CompareTo(object o)
        {
            AggroData temp = (AggroData)o;
            if (this.Point == temp.Point)
                return 0;
            else if (this.Point < temp.Point)
                return 1;
            else
                return -1;
        }

        public override string ToString()
        {
            return "AggroData : " + Attacker + " : Aggro [ " + Point + " ] : LastAtkT [ " + LastAttackT + " ]\n";
        }
    }

    Unit            Owner;

    bool            bInit = false;
    float           updateTime = 1f;    // 주기마다 어그로 정보 갱신
    //float           addingDefRate;      // 방어력에 따른 추가 어그로 포인트를 주기 위한 값
    //Coroutine       updateRoutine;

    List<AggroData> aggroList = new List<AggroData>();    

    protected TargetSelector() {}

    public TargetSelector(Unit owner)
    {
        Init( owner );
    }

    /// <summary>
    /// Coroutine을 복구를 위해서 사용.
    /// </summary>
    public void OnEnable()
    {
        Owner.StartCoroutine( UpdateRoutine() );
    }

    protected void Init(Unit owner)
    {
        if (bInit)
            return;

        Owner = owner;

        Owner.StartCoroutine( UpdateRoutine() );

        updateTime = 5f; // LowDataMgr.GetConfigValue( "Aggro_UpdateTime" );
        //addingDefRate = (float)LowDataMgr.GetConfigValue("Aggro_DefRate") / GameDefine.ConvertValueToRate;

        bInit = true;
    }

    IEnumerator UpdateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds( updateTime );

            UpdateAggro();
        }
    }

    public void UpdateAggro()
    {
        Validate();

        aggroList.Sort();

        AggroData topData = aggroList.Count > 0 ? aggroList[0] : null;
        if (null == topData)
            return;

        // 유저 캐릭터들은 정해진 타겟 안바뀌게 하기.
        bool canChangeTarget = !(Owner.UnitType == UnitType.Unit);

        if (Owner.TargetID != topData.Attacker.GetInstanceID() && canChangeTarget)
        {
            Owner.SetTarget( topData.Attacker.GetInstanceID() );
        }
    }

    public AggroData AddAggro(Unit attacker, float damage, float percent = 0)
    {
        // 어그로 포인트 계산
        int point = (int)damage;

        //point = (int)(damage + (damage * (attacker.CharInfo.Def * addingDefRate)));
        point = (int)( damage + ( damage ) ); // * attacker.CharInfo.Stats[AbilityType.AddAggro].FinalValue ) );

        AggroData attackerData = FindData( attacker );

        if (attackerData == null)
        {
            // 처음 생긴 어그로라면
            AggroData NewData = new AggroData();
            NewData.Attacker = attacker;
            NewData.LastAttackT = Time.time;
            NewData.Point = Mathf.Max( 0, point + (point * percent) );
            aggroList.Add( NewData );
        }
        else
        {
            //기존 어그로가 있다면
            attackerData.Point = attackerData.Point + (attackerData.Point * percent);
            attackerData.Point += point;
            attackerData.Point = Mathf.Max( 0, attackerData.Point );

            attackerData.LastAttackT = Time.time;
        }

        return aggroList[0];
    }

    /// <summary>
    /// 가장 최상위 타겟을 찾아준다. (주의. Update문에서 사용하지 말것!)
    /// </summary>
    public Unit TopMostTarget()
    {
        Validate();

        eAttackType attackType = Owner.CharInfo.AttackType;

        AggroData selTargetData = aggroList.Find( (aggro) => { return G_GameInfo.CharacterMgr.CanTarget( attackType, aggro.Attacker ); } );

        return null != selTargetData ? selTargetData.Attacker : null;
        //return aggroList.Count > 0 ? aggroList[0].Attacker : null;
    }

    public void ClearData()
    {
        aggroList.Clear();
    }

    public AggroData FindData(Unit target)
    {
        return aggroList.Find( AD => AD.Attacker == target );
    }

    /// <summary>
    /// 유효하지 않은 데이터를 삭제하도록 한다.
    /// </summary>
    void Validate()
    { 
        aggroList.RemoveAll( AD => null == AD.Attacker || !AD.Attacker.Usable );
    }
    
    public override string ToString()
    {
        string info = string.Empty;

        foreach (AggroData data in aggroList)
            info += data.ToString();

        return info;
    }
}
