using UnityEngine;
using System.Collections;

public class HeadObject : MonoBehaviour
{

    public UILabel NickName;
    public UILabel TitleName;
    public UISlider HpSlider;
    public UISlider ArmorSlider;
    public Unit Owner;

    public bool IsMy;//자기 자신
    public bool IsActivate;//활성화 중인지 오브젝트 풀에서 확인용으로 사용.
    public float LerpTime = 5;
    public float Distance = 50;
    public float HpDownValue = 0.01f;

    private Transform TargetTf;
    private float Height;
    private bool IsNameOnly;
    //private bool IsArmor;

    string origin_name;

    // 화면비율에 따른 위치 보정용
    private float ScreenWScale;

    /// <summary> TownPanel에서 사용 이름만을 표기함. </summary>
    public void ShowOnlyNickName(GameObject target, string name, uint prefix, uint suffix, bool isMy)
    {
        ScreenWScale = (float)((float)1280 / Screen.width);
        Height = 3.47f;

        NickName.gameObject.SetActive(true);
        NickName.text = name;
        origin_name = name;

        HpSlider.gameObject.SetActive(false);
        ArmorSlider.gameObject.SetActive(false);

        RefreshTitle(prefix, suffix);

        //HeadObject의 움직임을 Left로 해놨기 때문에 미리 자리를 잡아놓는다.
        Owner = target.GetComponent<Unit>();

        // 여기서 이름표시 제한을..?
        bool active = true;
        char[] showArr = SceneManager.instance.optionData.ShowName.ToCharArray();

        if (Owner != null && Owner.m_rUUID == NetData.instance.GetUserInfo()._charUUID)
        {
            //나의이름
            active = showArr[1].Equals('1') ? false : true;
        }
        else
        {
            //나머지들
            active = showArr[2].Equals('1') ? false : true;
        }

        NickName.transform.parent.gameObject.SetActive(active);

        TargetTf = target.transform;
        PosUpdate();

        IsMy = isMy;

        IsNameOnly = true;

        SetActivate(true);
    }

    public void Show(GameObject target, string name, uint prefix, uint suffix, bool isMy)
    {
        if (target == null || target.GetComponent<Unit>() == null)
            return;

        ScreenWScale = (float)((float)1280 / Screen.width);
        Owner = target.GetComponent<Unit>();
        Height = Owner.Height + 1.2f;

        if (string.IsNullOrEmpty(name))
            NickName.gameObject.SetActive(false);
        else
        {
            NickName.gameObject.SetActive(true);
            NickName.text = name;
        }

        RefreshTitle(prefix, suffix);

        HeadUpdate();
        IsMy = isMy;

        IsNameOnly = false;
        SetActivate(true);

        ChangeHead(target);
    }

    public void ChangeHead(GameObject target)
    {

        if (target == null || target.GetComponent<Unit>() == null)
            return;

        Owner = target.GetComponent<Unit>();

        // 여기서 이름,체력바 제한을..?
        bool nameActive = true;
        bool barActive = true;
        char[] showNameArr = SceneManager.instance.optionData.ShowName.ToCharArray();
        char[] showBarArr = SceneManager.instance.optionData.ShowHpBar.ToCharArray();

        //bar
        if (IsMy)//|| Owner.IsPartner
        {
            //나
            barActive = showBarArr[1].Equals('1') ? false : true;
            nameActive = showNameArr[1].Equals('1') ? false : true;
        }
        else
        {
            byte teamID = 0;
            int teamCount = NetData.instance._playerSyncData.playerSyncDatas.Count;

            for (int i = 0; i < teamCount; i++)
            {
                PlayerUnitData teamData = NetData.instance._playerSyncData.playerSyncDatas[i];
                if (teamData._TCPUUID != 0)
                    continue;

                teamID = teamData._TeamID;
                break;
            }


            if (Owner.TeamID == teamID)//파티원??
                barActive = showBarArr[2].Equals('1') ? false : true;
            else
                barActive = showBarArr[3].Equals('1') ? false : true;

            nameActive = showNameArr[2].Equals('1') ? false : true;
        }

        HpSlider.transform.gameObject.SetActive(barActive);
        ArmorSlider.transform.gameObject.SetActive(barActive);
        NickName.transform.parent.gameObject.SetActive(nameActive);


    }

    public void ShowNickInTown(bool flag)
    {
        NickName.transform.parent.gameObject.SetActive(flag);
    }

    public void ChangeNameColor(bool IsAttacker)
    {
        NickName.color = IsAttacker ? Color.red : Color.white;
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (IsNameOnly)
        {
            PosUpdate();
        }
        else
        {
            if (Owner == null || Owner.IsDie)
            {
                SetActivate(false);
                return;
            }

            HeadUpdate();
        }
    }

    void HeadUpdate()
    {
        float value = ((float)Owner.CharInfo.Hp / (float)Owner.CharInfo.MaxHp);
        if (value < HpSlider.value)
            HpSlider.value -= HpDownValue;
        else
            HpSlider.value = value;

        if (Owner.CharInfo.MaxSuperArmor > 0)
        {
            float armorValue = (float)Owner.CharInfo.SuperArmor / (float)Owner.CharInfo.MaxSuperArmor;
            ArmorSlider.value = armorValue;
        }

        Vector3 newPos = Owner.cachedTransform.position;
        newPos.y += Height;// 인게임에서 헤드위치 조정

        Vector2 to = MathHelper.WorldToUIPosition(newPos) * ScreenWScale;
        Vector2 pos = transform.localPosition;
        float dis = Vector3.Distance(to, pos);
        if (Distance < dis)
            transform.localPosition = to;
        else
        {
            if (IsMy)
                transform.localPosition = Vector3.Lerp(transform.localPosition, to, Time.deltaTime * LerpTime);//
            else
                transform.localPosition = to;
        }
    }

    void PosUpdate()
    {
        if (TargetTf == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 newPos = TargetTf.position;
        newPos.y += Height;
        Vector2 to = MathHelper.WorldToUIPosition(newPos) * ScreenWScale;
        Vector2 pos = transform.localPosition;

        if (Distance < Vector3.Distance(to, pos))
        {
            transform.localPosition = to;
        }
        else
        {
            if (IsMy)
                transform.localPosition = Vector2.Lerp(pos, to, Time.deltaTime * LerpTime);
            else
                transform.localPosition = to;
        }

        Vector3 sp = CameraManager.instance.mainCamera.WorldToScreenPoint(TargetTf.position);
        if (sp.x < 0 || sp.y < 0 || sp.z < 0)
        {
            NickName.text = "";
        }
        else if (CameraManager.instance.mainCamera.pixelWidth < sp.x || CameraManager.instance.mainCamera.pixelHeight < sp.y)
        {
            NickName.text = "";
        }
        else
        {
            NickName.text = origin_name;
        }
    }


    void SetActivate(bool active)
    {
        IsActivate = active;
        gameObject.SetActive(active);
    }

    public void RefreshTitle(uint prefix, uint suffix)
    {
        if (prefix == 0 && suffix == 0)
        {
            TitleName.gameObject.SetActive(false);
            return;
        }

        string pre = null, suf = null;
        Title.TitleInfo preInfo = _LowDataMgr.instance.GetLowDataTitle(prefix);
        Title.TitleInfo sufInfo = _LowDataMgr.instance.GetLowDataTitle(suffix);
        if (preInfo != null)
            pre = _LowDataMgr.instance.GetLowDataTitleName(preInfo.TitleName);

        if (sufInfo != null)
            suf = _LowDataMgr.instance.GetLowDataTitleName(sufInfo.TitleName);

        TitleName.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(suf))
            TitleName.text = string.Format("{0}", pre);
        else if (string.IsNullOrEmpty(pre))
            TitleName.text = string.Format("{0}", suf);
        else
            TitleName.text = string.Format("{0} {1}", pre, suf);
    }

    public ulong GetOwnerUID()
    {
        if (TownState.TownActive)
        {
            if (Owner is TownUnit)
                return (Owner as TownUnit).GetUID();
            else if (Owner is MyTownUnit)
            {
                return NetData.instance.GetUserInfo()._charUUID;
            }
            else//유저가 아님
                return 0;
        }
        else
            return 0;
    }
}
