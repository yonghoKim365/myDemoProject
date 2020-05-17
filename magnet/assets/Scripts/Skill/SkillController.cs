using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillController
{
    //< ��ų �׷� ����
    //public NetData.SkillDataGrop _SkillGroupInfo;
    public NetData._SkillDataGrop __SkillGroupInfo;

    //< ��ų �����
    public Unit Owner
    {
        get { return _Owner; }
        set
        {
            _Owner = value;

            if (_Owner is Pc)
            {
                //Ȳ��ȫ ������Ʈ - ��ų������
                if((_Owner as Pc).syncData.SkillData != null)
                {
                    if(!_Owner.IsPartner)
                    {
                        //�÷��̾�
                        __SkillGroupInfo = _LowDataMgr.GetSkillDataGrop((_Owner as Pc).syncData.NormalAttackData, (_Owner as Pc).syncData.SkillData);
                    }
                    else
                    {
                        //��Ʈ��
                        __SkillGroupInfo = _LowDataMgr.GetSkillDataGrop((_Owner as Pc).syncData.NormalAttackData, (_Owner as Pc).syncData.SkillData);

                    }
                }
                

            }
            else if (_Owner is Npc)
            {
                //������ ��� ��ų�����͸� �����ؼ� ��������Ѵ�.
                //SkillData[] monsterSkillData = new SkillData[4];
                List<SkillData> monsterSkillData = new List<SkillData>();
                SkillData[] monsterNormalAttackData = new SkillData[1];

                {
                    uint skillID = System.Convert.ToUInt32((_Owner as Npc).npcInfo.skill[0]);
                    byte skillLevel = System.Convert.ToByte((_Owner as Npc).npcInfo.SkillLevel[0]);

                    monsterNormalAttackData[0] = new SkillData(skillID, skillLevel);
                }

                int iCount = 0;
                for (int i = 1; i < 5; i++)
                {
                    uint skillID = System.Convert.ToUInt32((_Owner as Npc).npcInfo.skill[i]);
                    byte skillLevel = System.Convert.ToByte((_Owner as Npc).npcInfo.SkillLevel[i]);

                    //monsterSkillData[iCount] = new SkillData(skillID, skillLevel);
                    monsterSkillData.Add(new SkillData(skillID, skillLevel) );
                    iCount++;
                }
                
                __SkillGroupInfo = _LowDataMgr.GetSkillDataGrop(monsterNormalAttackData, monsterSkillData.ToArray() );
                //__SkillGroupInfo = _LowDataMgr.GetSkillDataGrop( monsterSkillData );
            }

            //< ��ų ������ �¾�
            SetSkillData();
        }
    }
    Unit _Owner;
    
    /// <summary> ���̵�� </summary>
    public void AddRaidSkill(uint[] skillIds)
    {
        List<SkillData> totalList = new List<SkillData>();
        List<SkillData> asddList = new List<SkillData>();
        int length = __SkillGroupInfo.skillData.Length;
        for(int i=0; i < length; i++)
        {
            totalList.Add(__SkillGroupInfo.skillData[i]);
        }

        int addLength = skillIds.Length;
        for (int i = 0; i < addLength; i++)
        {
            SkillData data = new SkillData(skillIds[i], 1);
            totalList.Add(data);
            asddList.Add(data);
        }

        __SkillGroupInfo.skillData = totalList.ToArray();

        //dataGroup.ActionDic = GetSkillActionDic(skillData);
        _LowDataMgr.SetSkillActionDic(ref __SkillGroupInfo.ActionDic, asddList.ToArray() );

        for (int i=length; i < __SkillGroupInfo.skillData.Length; i++)
        {
            if (__SkillGroupInfo.skillData[i] != null)
            {
                if (__SkillGroupInfo.skillData[i]._SkillID == 0)
                    continue;

                int skillSlot = i+1;
                if (SkillList[skillSlot] != null)
                    SkillList[skillSlot] = null;

                if (SkillList[skillSlot] == null)
                {
                    Skill NewSkill = new Skill();

                    NewSkill.RaidSkillInit(skillSlot, __SkillGroupInfo.skillData[i]._SkillID, Owner, this);
                    SkillList[skillSlot] = NewSkill;
                }
                //SetSkill(i + 1, __SkillGroupInfo.skillData[i]._SkillID);
            }
        }
    }
    
    //< ��ų ����Ʈ
    public Skill[] SkillList = new Skill[8];    //< ��ų

    public void FixedUpdate()
    {
        for (int i = 0; i < SkillList.Length; i++)
        {
            if (SkillList[i] != null && !GameInfoBase.NotTimeUpdate)
                SkillList[i].SkillUpdate();
        }
    }

    //< ��ų�� �����Ѵ�.
    void SetSkillData()
    {
        //< ��Ÿ ���� ����
        //NormalSkill.Init(0, Owner, this, true);

        // ��Ÿ���� ��罺ų�� ����
        //if(_Owner.UnitType == UnitType.Unit)
        {
            //__SkillGroupInfo.normalAttackData[0]
            SetSkill(0, __SkillGroupInfo.normalAttackData[0]._SkillID);

            for (int i = 0; i < __SkillGroupInfo.skillData.Length; i++)
            {
                if (__SkillGroupInfo.skillData[i] != null)
                {
                    if (__SkillGroupInfo.skillData[i]._SkillID != 0)
                        SetSkill(i + 1, __SkillGroupInfo.skillData[i]._SkillID);
                }
            }

            //for(int i=0; i<__SkillGroupInfo.ActionDic.Count; i++)
            //    SetSkill(i+1, __SkillGroupInfo.skillData[i]._SkillID);
        }
        /*
        else
        {
		  for (int i = 0; i < __SkillGroupInfo.ActionDic.Count; i++)
		  {
			 SetSkill(i);
		  }
        }
        */
    }

    void SetSkill(int slot, uint skillID)
    {
        if (SkillList[slot] != null)
            SkillList[slot] = null;

        if (SkillList[slot] == null)
        {
            Skill NewSkill = new Skill();

            NewSkill.Init(slot, skillID, Owner, this);
            SkillList[slot] = NewSkill;
        }
    }

    /*
    //< ��ų ������ �����Ѵ�.
    void SetSkill(int slot)
    {
        if (SkillList[slot] != null)
            SkillList[slot] = null;

        if (SkillList[slot] == null)
        {
            Skill NewSkill = new Skill();

            NewSkill.Init(slot, Owner, this);
            SkillList[slot] = NewSkill;
        }
	}
    */

    //< �ش罺ų�� ���� ��Ÿ�Ӹ�ŭ �ۼ�Ʈ�� ����( 1 �ƽ� ���� )
    public float IsSkillCoolTimePecent(int slot)
    {
        if (Owner == null || !Owner.Usable)//Owner.Usable True����...
            return 0;

        if (SkillList[slot] != null)
            return SkillList[slot].IsSkillCoolTimePecent();
        else
            return 0;
    }

    //< ����Ҽ� �ִ� ��ų���� �˻�
    public bool IsAbleSkill(int slot)
    {
        //Debug.LogWarning("2JW : SkillController.IsAbleSkill() - " + SkillList[slot] + " : " + SkillList[slot].IsUseAbleSkill());
        if (SkillList[slot] != null && SkillList[slot].IsUseAbleSkill() == 0)
            return true;
        return false;
    }

    /// ��ų Ȱ��ȭ(��ų������Ʈ���� �������ش�)
    public bool ActiveSkill(uint slot)
    {
        if (SkillList[slot] == null)
            return false;

        if (SkillList[slot].bActive)
            return false;

        SkillList[slot].ActiveSkill();
        return true;
    }

    public bool isBuffSkill(uint slot)
    {
        if (SkillList[slot] == null)
            return false;

        if(SkillList[slot]._SkillType == eActiveSkillType.Buff)
        {
            return true;
        }

        return false;
    }

    public ActionInfo GetActionInfo(uint slot)
    {
        if (SkillList[slot] == null)
            return null;

        return SkillList[slot].GetSkillActionInfo();
    }

    /// Ȱ��ȭ �� ��ų�� ������ ���
    public void UseSkill(uint slot, int abilityIdx, bool normal)
    {
        if (SkillList.Length <= slot || SkillList[slot] == null)
        {
            if (GameDefine.TestMode)
                Debug.LogWarning("Not Skill Slot" + slot + " , " + _Owner.name, _Owner.gameObject);
            return;
        }

        SkillList[slot].UseSkill((ushort)abilityIdx, normal);
    }

    public uint GetSkillIndex(int slot)
    {
        return SkillList[slot].GetSkillIndex();
    }

    /// ��ų ��� �ϱ��� ��ų ���� ����
    public SkillActiveCondition GetSkillCondition(int slot, bool AutoModeforceUserUse = false)
    {
        if (SkillList[slot] == null)
            return SkillActiveCondition.eNone;

        return SkillList[slot].GetCondition(AutoModeforceUserUse);
    }

    //< ��ų�� �ʱ���·� �����Ѵ�
    public void ResetSkill()
    {
        for (int i = 0; i < SkillList.Length; i++)
        {
            if (SkillList[i] != null)
                SkillList[i].SetSkillActive();
        }
    }

    public float GetSkillCoolTime(int slot)
    {
        if (SkillList.Length <= slot)
            return 0;

        return SkillList[slot].CoolTime;
    }
}
