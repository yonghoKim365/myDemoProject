using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillEventMgr : Immortal<SkillEventMgr>
{
    enum eEventType
    {
        Skill,
        Spawn,
    }

    eEventType _eEventType = eEventType.Skill;

    public static bool ActiveEvent = false; //< 마지막으로 실행한 이벤트 체크
    static bool _EventUpdate = false;//< 이벤트가 실행중인지 여부
    public static bool EventUpdate
    {
        get { return _EventUpdate; }
        set
        {
            if(value)
                EventListner.instance.TriggerEvent("UseSkill_Start", G_GameInfo.PlayerController.Leader);
            else
                EventListner.instance.TriggerEvent("UseSkill_End", G_GameInfo.PlayerController.Leader);

            _EventUpdate = value;
        }
    }

    public FocusingCamera FocusingCam;
    RtsCamera rtsCam;
    public static bool Live = false;
    public void Setup(RtsCamera _rtsCam, FocusingCamera _FocusingCam)
    {
        rtsCam = _rtsCam;
        FocusingCam = _FocusingCam;

        CameraManager.instance.SetSkillEventCamera(FocusingCam.gameObject);

        Live = true;
    }

    public bool IsFocusEnable
    {
        get {
            if ( !Live || FocusingCam == null)
                return false;

            return FocusingCam.gameObject.activeSelf;
        }
    }

    void OnDestroy()
    {
        Live = false;
    }

    public void SpawnEvent(bool type, Unit target)
    {
        if (!Live)
            return;

        //< 스킬이벤트가 활성화중일떈 나감
        if (EventUpdate && _eEventType == eEventType.Skill)
            return;

        if (ActiveEvent == type)
            return;

        ActiveEvent = type;
        StopAllCoroutines();

        //< 타겟 리스트를 구한다.
        if (type)
        {
            units.Clear();
            units.Add(target);
            SetLayer(target, 14);
        }

        _eEventType = eEventType.Spawn;
        StartCoroutine("CameraUpdate_Spawn", ActiveEvent);
    }

    List<Unit> units = new List<Unit>();
    Unit UseUnit;
    public bool SetEvent(bool type, Unit parentUnit, ActionInfo actionData = null)
    {
        if (!Live)
            return false;

        //< 스폰 이벤트가 진행중일시엔 나감
        if (EventUpdate && _eEventType == eEventType.Spawn)
            return false;

        //< 실행하는거라면 조건을 검사함
        if(type)
        {
            if (parentUnit.UnitType != UnitType.Unit || parentUnit.TeamID != 0 || !parentUnit.IsLeader)
                return false;
        }

        if (ActiveEvent == type)
            return false;

        //< 종료할때에는 처음 시전한 유닛만 되도록 처리
        if (!type && UseUnit != parentUnit)
            return false;

        ActiveEvent = type;
        _eEventType = eEventType.Skill;

        //< 혹 실행하고있을수도있기에 종료
        StopAllCoroutines();

        //actionData.camera = 4;

        if(ActiveEvent)
        {
            UseUnit = parentUnit;
            //<==========================================
            //<     이벤트 타입에따라서 카메라 처리
            //<==========================================

            //< 기본 스킬 줌(살짝 줌되었다 빠짐)
            if (actionData.camera == 1)
                StartCoroutine(CameraUpdate_1(parentUnit, true));
            //< 2 : 필살스킬(깊게 줌되며 검은색 배경처리)
            //< 3 : 버프스킬(뒤로 살짝 빠지며 검은색 배경처리)
            //< 4 : 점프스킬(위로 쭉 빠지면서 검색은 배경처리)
            else if (actionData.camera == 2 || actionData.camera == 3 || actionData.camera == 4)
            {
                //< 현재 살아있는 모든 유닛에 레이어를 변경해줌
                for(int j=0; j<G_GameInfo.CharacterMgr.allUnitList.Count; j++)
                {
                    Unit unit = G_GameInfo.CharacterMgr.allUnitList[j];
                    if (unit == null || unit.UnitType == UnitType.Prop || unit.UnitType == UnitType.Trap || unit.CharInfo.IsDead)
                        continue;

                    SetLayer(unit, 11);
                    if (unit.EffectDic.ContainsKey(true))
                    {
                        for (int i = 0; i < unit.EffectDic[true].Count; i++)
                        {
                            Transform effect = unit.EffectDic[true][i];
                            if (effect != null && effect.gameObject != null && effect.gameObject.layer != 11)
                                SetLayer(effect.gameObject, 11);
                        }
                    }
                    if (unit.EffectDic.ContainsKey(false))
                    {
                        for (int i = 0; i < unit.EffectDic[false].Count; i++)
                        {
                            Transform effect = unit.EffectDic[false][i];
                            if (effect != null && effect.gameObject != null && effect.gameObject.layer != 11)
                                SetLayer(effect.gameObject, 11);
                        }
                    }

                    units.Add(unit);
                }

                //< 실행
                if (actionData.camera == 2)
                    StartCoroutine(CameraUpdate(parentUnit, true));
                else if (actionData.camera == 3)
                    StartCoroutine(CameraUpdate_3(parentUnit, true));
                else if (actionData.camera == 4)
                    StartCoroutine(CameraUpdate_4(parentUnit, true, (byte)(actionData.camera - 3)));
            }
            else if (actionData.camera == 5)
                StartCoroutine(CameraUpdate_5(parentUnit, true));
        }
        else
        {
            //< 기본 스킬 줌(살짝 줌되었다 빠짐)
            //if (actionData.camera == 1)
            //    StartCoroutine(CameraUpdate_1(parentUnit, false));
            //< 필살스킬(깊게 줌되며 검은색 배경처리)
            //else if (actionData.camera == 2)
            //    StartCoroutine(CameraUpdate(parentUnit, false));
            //else if (actionData.camera == 3)
            //    StartCoroutine(CameraUpdate_3(parentUnit, false));
            //else if (actionData.camera == 4)
            //    StartCoroutine(CameraUpdate_4(parentUnit, false, (byte)(actionData.camera - 3)));
            //else if (actionData.camera == 5)
            //    StartCoroutine(CameraUpdate_5(parentUnit, false));


			if (Camera.main==null){
				Debug.LogError(" Camera.main is null");
			}


			if (Camera.main != null){
	            Camera.main.transform.parent = rtsCam.transform;
	            Camera.main.transform.localPosition = Vector3.zero;
	            Camera.main.transform.localRotation = Quaternion.identity;
			}

            this.transform.parent = null;

            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        return true;
    }

    void LayerReset()
    {
        //< 레이어를 돌려줌
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
            {
                SetLayer(units[i], 14);
                if (units[i].EffectDic.ContainsKey(true))
                {
                    for (int j = 0; j < units[i].EffectDic[true].Count; j++)
                    {
                        Transform effect = units[i].EffectDic[true][j];
                        if (effect != null && effect.gameObject != null && effect.gameObject.layer != 0)
                            SetLayer(effect.gameObject, 11);
                    }
                }
                if (units[i].EffectDic.ContainsKey(false))
                {
                    for (int j = 0; j < units[i].EffectDic[false].Count; j++)
                    {
                        Transform effect = units[i].EffectDic[false][j];
                        if (effect != null && effect.gameObject != null && effect.gameObject.layer != 0)
                            SetLayer(effect.gameObject, 11);
                    }
                }
            }
        }
        units.Clear();
    }

    //< 1번 타입의 카메라 업데이트
    IEnumerator CameraUpdate_1(Unit parentUnit, bool _ActiveEvent)
    {
        EventUpdate = true;
        Vector3 target = Vector3.zero;
        if (_ActiveEvent)
        {
            //< 메인카메라도 같이 움직이도록 처리
            this.transform.position = Camera.main.transform.position;
            this.transform.rotation = Camera.main.transform.rotation;

            Camera.main.transform.parent = this.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            //target = Camera.main.transform.position + (Camera.main.transform.forward * 4);
            
        }
        else
            this.transform.transform.parent = rtsCam.transform;

        //< 줌 업데이트 처리
        float value = 0;
        while (true)
        {
            value += 0.4f * Time.deltaTime;
            if (_ActiveEvent)
            {
                if (Vector3.Distance(Camera.main.transform.position, target) > 0)
                {
                    target = rtsCam.transform.position + (rtsCam.transform.forward * 4);
                    this.transform.transform.position = Vector3.Slerp(this.transform.transform.position, target, value);
                }
                
            }
            else if (!_ActiveEvent)
            {
                this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, target, value);
                if (Vector3.Distance(this.transform.localPosition, target) < 0.2f)
                    break;
            }

            yield return null;
        }

        //< 다시 원상복귀 시킨다.
        if (!_ActiveEvent)
        {
            Camera.main.transform.parent = rtsCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            
            this.transform.parent = null;
        }

        EventUpdate = false;
        yield return null;
    }

    //< 2번 타입의 카메라 업데이트
    IEnumerator CameraUpdate(Unit parentUnit, bool _ActiveEvent)
    {
        EventUpdate = true;
        Vector3 target = Vector3.zero;
        Quaternion targetRot = Quaternion.identity;

        if (_ActiveEvent)
        {
            this.transform.position = FocusingCam.transform.position;
            this.transform.rotation = FocusingCam.transform.rotation;

            //< 포커스카메라 실행
            FocusingCam.SetSkillEvent(true);
            FocusingCam.gameObject.SetActive(true);
            FocusingCam.transform.parent = null;

            Vector3 unitForward = parentUnit.transform.localEulerAngles;
            if ((unitForward.y >= 30 && unitForward.y < 200))
            {
                Vector3 targetPos = parentUnit.transform.position - (parentUnit.transform.forward * 3);
                targetPos -= parentUnit.transform.right * 5;
                targetPos += parentUnit.transform.up * 3;
                FocusingCam.transform.position = targetPos;
                FocusingCam.transform.LookAt(parentUnit.transform.position + (parentUnit.transform.up * 2));

                //< 조금더 보정
                FocusingCam.transform.position -= parentUnit.transform.right * 2;
            }
            else
            {
                Vector3 targetPos = parentUnit.transform.position - (parentUnit.transform.forward * 3);
                targetPos += parentUnit.transform.right * 5;
                targetPos += parentUnit.transform.up * 3;
                FocusingCam.transform.position = targetPos;
                FocusingCam.transform.LookAt(parentUnit.transform.position + (parentUnit.transform.up * 2));

                //< 조금더 보정
                FocusingCam.transform.position += parentUnit.transform.right * 2;
            }

            //< 위치 저장
            target = FocusingCam.transform.position;
            targetRot = FocusingCam.transform.rotation;

            FocusingCam.transform.parent = this.transform;
            FocusingCam.transform.localPosition = Vector3.zero;
            FocusingCam.transform.localRotation = Quaternion.identity;

            //< 메인카메라도 같이 움직이도록 처리
            Camera.main.transform.parent = FocusingCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        }
        else
        {
            FocusingCam.transform.parent = rtsCam.transform;
        }

        //< 해당 위치로 이동한다
        float value = 0;
        Vector3 Targetalpha = ActiveEvent ? new Vector3(255,255,255) : Vector3.zero;
        Vector3 alpha = ActiveEvent ? Vector3.zero : new Vector3(255, 255, 255);
        while (true)
        {
            if (_ActiveEvent)
            {
                value += 0.6f * Time.deltaTime;
                this.transform.position = Vector3.Slerp(this.transform.position, target, value);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, value);

                alpha = Vector3.Slerp(alpha, Targetalpha, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                if (Vector3.Distance(FocusingCam.transform.position, target) < 0.2f)
                    break;
            }
            else
            {
                value += 1 * Time.deltaTime;
                FocusingCam.transform.localPosition = Vector3.Slerp(FocusingCam.transform.localPosition, target, value);
                FocusingCam.transform.localRotation = Quaternion.Slerp(FocusingCam.transform.localRotation, targetRot, value);

                alpha = Vector3.Slerp(alpha, Vector3.zero, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                if (Vector3.Distance(FocusingCam.transform.localPosition, target) < 0.2f)
                    break;
            }

            yield return null;
        }

        if (!_ActiveEvent)
        {
            Camera.main.transform.parent = rtsCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            FocusingCam.transform.parent = Camera.main.transform;
            FocusingCam.transform.localPosition = Vector3.zero;
            FocusingCam.transform.localRotation = Quaternion.identity;

            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        EventUpdate = false;
    }

    //< 3번 타입의 카메라 업데이트
    IEnumerator CameraUpdate_3(Unit parentUnit, bool _ActiveEvent)
    {
        EventUpdate = true;
        Vector3 target = Vector3.zero;
        if (_ActiveEvent)
        {
            //< 메인카메라도 같이 움직이도록 처리
            this.transform.position = Camera.main.transform.position;
            this.transform.rotation = Camera.main.transform.rotation;

            FocusingCam.SetSkillEvent(true);
            FocusingCam.gameObject.SetActive(true);

            Camera.main.transform.parent = this.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            //target = Camera.main.transform.position - (Camera.main.transform.forward * 5);
        }
        else
            this.transform.transform.parent = rtsCam.transform;

        //< 줌 업데이트 처리
        float value = 0;
        Vector3 Targetalpha = ActiveEvent ? new Vector3(255, 255, 255) : Vector3.zero;
        Vector3 alpha = ActiveEvent ? Vector3.zero : new Vector3(255, 255, 255);
        while (true)
        {
            if (_ActiveEvent)
            {
                target = rtsCam.transform.position - (rtsCam.transform.forward * 5);
                value += 0.6f * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Targetalpha, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                this.transform.transform.position = Vector3.Slerp(this.transform.transform.position, target, value);
                if (Vector3.Distance(Camera.main.transform.position, target) < 0.2f)
                    break;
            }
            else if (!_ActiveEvent)
            {
                value += 1 * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Vector3.zero, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, target, value);
                if (Vector3.Distance(this.transform.localPosition, target) < 0.2f)
                    break;
            }

            yield return null;
        }

        //< 다시 원상복귀 시킨다.
        if (!_ActiveEvent)
        {
            Camera.main.transform.parent = rtsCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            this.transform.parent = null;

            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        EventUpdate = false;
        yield return null;
    }

    //< 4번 타입의 카메라 업데이트
    IEnumerator CameraUpdate_4(Unit parentUnit, bool _ActiveEvent, byte Delay)
    {
        EventUpdate = true;
        Vector3 target = Vector3.zero;
        if (_ActiveEvent)
        {
            //< 메인카메라도 같이 움직이도록 처리
            this.transform.position = Camera.main.transform.position;
            this.transform.rotation = Camera.main.transform.rotation;

            FocusingCam.SetSkillEvent(true);
            FocusingCam.gameObject.SetActive(true);

            Camera.main.transform.parent = this.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            //target = Camera.main.transform.position - (Camera.main.transform.forward * 5);
        }
        else
            this.transform.transform.parent = rtsCam.transform;

        //< 아웃 업데이트 처리
        float value = 0;
        Vector3 Targetalpha = ActiveEvent ? new Vector3(255, 255, 255) : Vector3.zero;
        Vector3 alpha = ActiveEvent ? Vector3.zero : new Vector3(255, 255, 255);
        while (true)
        {
            if (_ActiveEvent)
            {
                target = rtsCam.transform.position + (Vector3.up * 3);
                value += 0.6f * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Targetalpha, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                this.transform.transform.position = Vector3.Slerp(this.transform.transform.position, target, value);
                if (Vector3.Distance(Camera.main.transform.position, target) < 0.2f)
                    break;
            }
            else if (!_ActiveEvent)
            {
                value += 1 * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Vector3.zero, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, target, value);
                if (Vector3.Distance(this.transform.localPosition, target) < 0.2f)
                    break;
            }

            yield return null;
        }

        if(_ActiveEvent)
        {
            yield return new WaitForSeconds(Delay);
            StartCoroutine(CameraUpdate_4(parentUnit, false, 0));

            yield break;
        }

        //< 다시 원상복귀 시킨다.
        if (!_ActiveEvent)
        {
            Camera.main.transform.parent = rtsCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            this.transform.parent = null;

            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        EventUpdate = false;
        yield return null;
    }

    //< 5번 타입의 카메라 업데이트
    IEnumerator CameraUpdate_5(Unit parentUnit, bool _ActiveEvent)
    {
        EventUpdate = true;
        Vector3 target = Vector3.zero;
        if (_ActiveEvent)
        {
            //< 메인카메라도 같이 움직이도록 처리
            this.transform.position = Camera.main.transform.position;
            this.transform.rotation = Camera.main.transform.rotation;

            Camera.main.transform.parent = this.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

        }
        else
            this.transform.transform.parent = rtsCam.transform;

        //< 아웃 업데이트 처리
        float value = 0;
        while (true)
        {
            if (_ActiveEvent)
            {
                value += 0.4f * Time.deltaTime;
                if (Vector3.Distance(Camera.main.transform.position, target) > 0.1f)
                {
                    target = rtsCam.transform.position + (Vector3.up * 4);
                    this.transform.transform.position = Vector3.Slerp(this.transform.transform.position, target, value);
                }
                else
                    break;

            }
            else if (!_ActiveEvent)
            {
                value += 0.8f * Time.deltaTime;
                this.transform.localPosition = Vector3.Slerp(this.transform.localPosition, target, value);
                if (Vector3.Distance(this.transform.localPosition, target) < 0.2f)
                    break;
            }

            yield return null;
        }

        if (_ActiveEvent)
        {
            yield return new WaitForSeconds(0.8f);
            StartCoroutine(CameraUpdate_4(parentUnit, false, 0));
        }

        //< 다시 원상복귀 시킨다.
        if (!_ActiveEvent)
        {
            Camera.main.transform.parent = rtsCam.transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            this.transform.parent = null;

            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        EventUpdate = false;
        yield return null;
    }

    //< 유닛소환, 체인지등에 들어가는 연출 이벤트
    IEnumerator CameraUpdate_Spawn(bool _ActiveEvent)
    {
        EventUpdate = true;
        if (_ActiveEvent)
        {
            //< 포커스카메라 실행
            FocusingCam.SetSkillEvent(true);
            FocusingCam.gameObject.SetActive(true);
        }

        float value = 0;
        Vector3 Targetalpha = _ActiveEvent ? Vector3.one * 200 : Vector3.zero;
        Vector3 alpha = _ActiveEvent ? Vector3.zero : Vector3.one * 200;
        while (true)
        {
            if (_ActiveEvent)
            {
                value += 2 * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Targetalpha, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                if (alpha.z >= 200)
                    break;
            }
            else
            {
                value += 2 * Time.deltaTime;
                alpha = Vector3.Slerp(alpha, Vector3.zero, value);
                FocusingCam.SkillCoverMaterial.color = new Color32(255, 255, 255, (byte)alpha.z);

                if (alpha.z <= 0)
                    break;
            }

            yield return null;
        }

        //< 시작이었다면 일정시간 대기후 다시 되돌림
        if (_ActiveEvent)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnEvent(false, null);
            yield break;
        }

        //< 종료처리
        else
        {
            FocusingCam.SetSkillEvent(false);
            FocusingCam.gameObject.SetActive(false);

            //< 레이어를 돌려줌
            LayerReset();
        }

        EventUpdate = false;
        yield return null;
    }
    
    //< 강제로 종료하기위해 처리
    public void SetEnd()
    {
        EventUpdate = false;
        StopCoroutine("CameraUpdate");

        if (FocusingCam == null || Camera.main == null || rtsCam == null)
            return;

        FocusingCam.transform.parent = Camera.main.transform;
        FocusingCam.transform.localPosition = Vector3.zero;
        FocusingCam.transform.localRotation = Quaternion.identity;

        Camera.main.transform.parent = rtsCam.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
        FocusingCam.SetSkillEvent(false);
        FocusingCam.gameObject.SetActive(false);

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
                SetLayer(units[i], 0);
        }
    }

    void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    void SetLayer(Unit unit, int layer)
    {
        unit.gameObject.layer = layer;
        for (int i = 0; i < unit.Transforms.Length; i++)
        {
            if (unit.Transforms[i] != null)
                unit.Transforms[i].gameObject.layer = layer;
        }
    }
}
