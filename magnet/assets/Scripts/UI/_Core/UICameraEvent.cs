using UnityEngine;
using System.Collections;

public class UICameraEvent : MonoBehaviour {

    GameObject NotTouchPanel, Center;
    void Awake()
    {
        NotTouchPanel = Instantiate(Resources.Load("UI/NotTouchPanel")) as GameObject;
        NotTouchPanel.transform.parent = this.gameObject.transform;
        NotTouchPanel.transform.localPosition = Vector3.zero;
        NotTouchPanel.transform.localScale = Vector3.one;

        NotTouchPanel.SetActive(false);
    }

    Vector3 offset;
    Vector3 SetQua;
    public void Setup(GameObject _Center, Vector3 _offset, Vector3 _qua)
    {
        UpdateActive = false;
        Center = _Center;
        offset = _offset;
        SetQua = _qua;
    }

    public bool UpdateActive = false;
    bool StartCheck = false;
    public void StartEvent(System.Action _StartCallBack)
    {
        StartCallBack = _StartCallBack;
        StopCoroutine("_StartEvent");
        StopCoroutine("_EndEvent");

        if (!StartCheck)
            StartCoroutine("_StartEvent");
        else
            SetStart();

        //< 건물 이름은 숨김
        //BuildingBoardPanel.Instance.AllActive(false);
    }

    public void EndEvent(System.Action _EndCallBack)
    {
        EndCallBack = _EndCallBack;
        StopCoroutine("_StartEvent");
        StopCoroutine("_EndEvent");
        StartCoroutine("_EndEvent");
    }

    Vector3 OrgPos;
    Quaternion OrgQua;
    public System.Action StartCallBack;
    IEnumerator _StartEvent()
    {
        //< 이미 시작상태였다면 패스
        //if (StartCheck)
        //    yield break;

        //UpdateActive = true;
        //StartCheck = true;
        //NotTouchPanel.SetActive(true);

        //if (Center != null)
        //    Center.SetActive(false);

        ////< 시작하기전 위치와 각도를 저장한다
        //OrgPos = UIHelper.MainCameraObj.transform.position;
        //OrgQua = UIHelper.MainCameraObj.transform.rotation;

        //CameraMouseZoom.instance.isFreezing = true;

        ////< 해당 카메라 목표위치를 얻는다.
        //Vector3 target = StructureBase.SelectBuild.transform.position + (StructureBase.SelectBuild.transform.right * offset.x) + (StructureBase.SelectBuild.transform.forward * offset.z) + (StructureBase.SelectBuild.transform.up * offset.y);

        ////< 해당 카메라의 목표 각도를 얻는다
        //Vector3 rot = UIHelper.MainCameraObj.transform.rotation.eulerAngles;
        //rot.x = 8 + SetQua.x;
        //rot.y += SetQua.y;
        //rot.z = 2 + SetQua.z;

        ////< 해당 위치로 이동한다
        //float value = 0;
        //while (true)
        //{
        //    value += 0.2f * Time.deltaTime;
        //    UIHelper.MainCameraObj.transform.position = Vector3.Slerp(UIHelper.MainCameraObj.transform.position, target, value);
        //    UIHelper.MainCameraObj.transform.rotation = Quaternion.Slerp(UIHelper.MainCameraObj.transform.rotation, Quaternion.Euler(rot), value);

        //    if (Vector3.Distance(UIHelper.MainCameraObj.transform.position, target) < 0.5f)
        //        break;

        //    yield return null;
        //}

        NotTouchPanel.SetActive(false);

        if (Center != null)
            Center.SetActive(true);

        if (StartCallBack != null)
            StartCallBack();

        UpdateActive = false;

        yield return null;
    }

    System.Action EndCallBack;
    IEnumerator _EndEvent()
    {
        UpdateActive = true;
        StartCheck = false;
        NotTouchPanel.SetActive(true);

        if (Center != null)
            Center.SetActive(false);

        //< 해당 위치로 이동한다
        float value = 0;
        while (true)
        {
            value += 0.2f * Time.deltaTime;
            UIHelper.MainCameraObj.transform.position = Vector3.Slerp(UIHelper.MainCameraObj.transform.position, OrgPos, value);
            UIHelper.MainCameraObj.transform.rotation = Quaternion.Slerp(UIHelper.MainCameraObj.transform.rotation, OrgQua, value);

            if (Vector3.Distance(UIHelper.MainCameraObj.transform.position, OrgPos) < 0.5f)
                break;

            yield return null;
        }

        NotTouchPanel.SetActive(false);
        

        if (EndCallBack != null)
            EndCallBack();

        //BuildingBoardPanel.Instance.AllActive(true);

        UpdateActive = false;
        yield return null;
    }

    //< 강제로 위치 셋
    public void SetStart()
    {
        OrgPos = UIHelper.MainCameraObj.transform.position;
        OrgQua = UIHelper.MainCameraObj.transform.rotation;

        UpdateActive = false;
        //UIHelper.MainCameraObj.transform.position = StructureBase.SelectBuild.transform.position + (StructureBase.SelectBuild.transform.right * offset.x) + (StructureBase.SelectBuild.transform.forward * offset.z) + (StructureBase.SelectBuild.transform.up * offset.y);

        Vector3 rot = UIHelper.MainCameraObj.transform.rotation.eulerAngles;
        rot.x = 8 + SetQua.x;
        rot.y += SetQua.y;
        rot.z = 2 + SetQua.z;
        UIHelper.MainCameraObj.transform.rotation = Quaternion.Euler(rot);
        
        //< 건물 이름을 숨김
        //BuildingBoardPanel.Instance.AllActive(false);
        
        //< UI띄움
        if (Center != null)
            Center.SetActive(true);

        if (StartCallBack != null)
            StartCallBack();
    }

    //< 강제로 종료처리
    public void SetEnd()
    {
        UpdateActive = false;
        UIHelper.MainCameraObj.transform.position = OrgPos;
        UIHelper.MainCameraObj.transform.rotation = OrgQua;
    }
}
