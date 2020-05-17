using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffController {

    public List<Buff> BuffDic = new List<Buff>();    
	public Unit Owner;
    public System.Action AddBuffCallBack;

    public void FixedUpdate()
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i] == null)
            {
                BuffDic.RemoveAt(i);
                i--;
                continue;
            }

            BuffDic[i].BuffUpdate();
        }
    }

    //< 버프 추가
    //public void AttachBuff(Unit caster, Unit target, SkillTables.BuffInfo _buffInfo, AbilityData _ability)
    public void AttachBuff(Unit caster, Unit target, SkillTables.BuffInfo _buffInfo, uint _BuffRate, float _BuffDuration )
    {
        if(_buffInfo == null)
            return;
//	    LOGEX.LogEx("BuffController.AttachBuff()", LOGEX.LogType.NONE, LOGEX.ColorType.PURPLE);
        //< 일단 확률을 먼저 검사한다.
        ushort ran = (ushort)Random.Range(0, 10000);
        if ((ushort)ran > _BuffRate)
        {
            BuffInfoStringShow(caster, target, _buffInfo, "抵抗");
            return;
        }

        //< 적용할 수치를 계산한다.

        //버프의 데미지율은 천분율이라 0.001곱하는데 이건좀...
        float affectValue;

        if (SceneManager.instance.IsRTNetwork)
            affectValue = 1;
        else
            affectValue = SkillAbility.CalcAffectValue(_buffInfo, caster, target, 0.001f);

        //< 같은 타입의 버프가 존재하는지 검사
        bool accrue = false;
        for(int i=0; i<BuffDic.Count; i++)
        {
            //< 같은 버프를 또 쓸경우 중첩됨
            if(BuffDic[i].buffInfo.Indx == _buffInfo.Indx)
            {
                BuffDic[i].OnAttach(this, caster, target, _buffInfo, affectValue, (BuffDurationType)_buffInfo.buffAbility, _BuffDuration);
                accrue = true;
                break;
            }
        }

        //< 중첩되는 버프가 없다면 새로 만들어서 넣어줌
        if(!accrue)
        {
            //< 새버프 생성
            Buff newBuff = new Buff();
            BuffDic.Add(newBuff);
            newBuff.OnAttach(this, caster, target, _buffInfo, affectValue, (BuffDurationType)_buffInfo.buffAbility, _BuffDuration);
        }

        if (AddBuffCallBack != null)
            AddBuffCallBack();

        //< 게임패널에서 업데이트하기 위함
        EventListner.instance.TriggerEvent("BuffUpdate");
    }

    //< 버프가 추가되었을시 머리위에 출력하기위함
    public static void BuffInfoStringShow(Unit caster, Unit target, SkillTables.BuffInfo _buffInfo, string subStr)
    {
        //string str = LowDataMgr.GetLocale(_buffInfo.name).title;

        string str = _LowDataMgr.instance.GetStringSkillName(_buffInfo.buffDisplay);
        
        //< 저항했을시 조립
        str += " " + subStr;
        //< 버프가 붙었을때 스트링을 띄우려면 이곳에서!!

        if (SceneManager.instance.IsRTNetwork)
        {
            
        }
        else
        {
            if (target.gameObject.activeSelf && G_GameInfo.GameInfo.BoardPanel != null)
                G_GameInfo.GameInfo.BoardPanel.ShowBuff(target.gameObject, caster.gameObject, str);
        }
	   
    }

    //< 버프 삭제되었을시 호출(리스트에서 삭제하기위함)
    public void DetachBuff(Buff buff, bool _failed = false)
    {
	   if (BuffDic.Contains(buff))
            BuffDic.Remove(buff);

        //< 게임패널에서 업데이트하기 위함
        EventListner.instance.TriggerEvent("BuffUpdate");

        //< 버프가 끝났을때 스트링을 띄우려면 이곳에서!!
        if(!_failed)
        {

        }
    }

    //< 외부에서 강제로 호출하는것이므로 해당 버프가 강제로 종료되기때문에
    //< 중첩되었을경우 문제가 발생, 즉 왠만하면 부르지말도록 처리...
    public void DetachBuff(uint idx)
    {
	   for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i].buffInfo.Indx == idx)
            {
                BuffDic[i].DetachBuff();
                break;
            }
        }
    }

    //특정타입의 버프삭제
    public void DetachBuff(BuffType type)
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i].buffInfo.buffType == (byte)type)
            {
                BuffDic[i].DetachBuff();
                break;
            }
        }
    }

    public void DetachBuff(BuffDurationType type)
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i].BuffDurType == type)
            {
                BuffDic[i].DetachBuff();
            }
        }
    }

    //해당 타입의 버프가 해당 해제타입인지 체크
    public bool CheckBuffType(uint BuffID, BuffDurationType type)
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i] != null)
                if(BuffDic[i].buffInfo.Indx == BuffID && BuffDic[i].BuffDurType == type )
                    return true;
        }

        return false;
    }

    //< 해당 타입의 버프가 존재하는지 검사
    public bool GetTypeBuffCheck(BuffType type)
    {
        for (int i = 0; i < BuffDic.Count; i++ )
        {
            if (BuffDic[i] != null && BuffDic[i].buffInfo.buffType == (byte)type)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 타입의 버프 캐스터 가져오기.
    /// </summary>
    public Unit GetTypeBuffCasterCheck(BuffType type)
    {
	   for (int i = 0; i < BuffDic.Count; i++)
	   {
		  if (BuffDic[i] != null && BuffDic[i].buffInfo.buffType == (byte)type)
			 return BuffDic[i].Caster;
	   }
	   return null;
    }

    //< 모든 버프를 강제종료시킨다
    public void AllBuffDestroy()
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i] != null)
                BuffDic[i].DetachBuff();
        }

        BuffDic.Clear();

        //< 게임패널에서 업데이트하기 위함
        EventListner.instance.TriggerEvent("BuffUpdate");
    }

    //< 무적 종류의 이펙트가 걸려있을경우 처리해준다.
    public void SetImmuneEffect(bool type)
    {
        for (int i = 0; i < BuffDic.Count; i++)
        {
            if (BuffDic[i] != null)
            {
                switch((BuffType)BuffDic[i].buffInfo.buffType)
                {
                    case BuffType.AllImmune:
                    case BuffType.AttackImmune:
                    case BuffType.SkillImmune:

                        //< 모두 꺼준다
                        if (!type)
                        {
                            BuffDic[i].SetEffect(false);
                        }
                        //< 하나를 켜주고 나간다.
                        else
                        {
                            BuffDic[i].SetEffect(true);
                            return;
                        }
                        break;
                }
            }
        }
    }

    void OnDestroy()
    {
        BuffDic.Clear();
    }

    public int TakeDam(int dam, ref eDamResistType DamResist)
    {
        //< 버프중에 쉴드가 있는지 검사한다.
        for (int i = 0; i < BuffDic.Count; i++ )
        {
            if (BuffDic[i] == null)
                continue;

            if(BuffDic[i].buffInfo.buffType == (byte)BuffType.Shield)
            {
                DamResist = eDamResistType.Shield;
                if (BuffDic[i].affectedValue >= dam)
                {
                    BuffDic[i].affectedValue -= dam;
                    dam = 0;
                }
                else
                {
                    dam -= (int)BuffDic[i].affectedValue;
                    BuffDic[i].affectedValue = 0;
                }

                //< 쉴드가 다 사용되었다면 버프를 종료한다.
                if(BuffDic[i].affectedValue <= 0)
                {
                    BuffDic[i].DetachBuff();
                    i--;
                    continue;
                }

                if (dam <= 0)
                    break;
            }
        }

        //< 최소 1대미지띄우도록
        dam = Mathf.Clamp(dam, 1, dam);

        return dam;
    }
}
