using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIHelper
{
    public static readonly Color OrangeColor = new Vector4(166f / 255f, 45f / 255f, 36f / 255f, 1f);

    //< 카메라를 켜고 꺼준다
    static GameObject _MainCameraObj;
    public static GameObject MainCameraObj
    {
        get { return _MainCameraObj; }
    }
    public static void SetMainCameraActive(bool type)
    {
        if (_MainCameraObj == null && Camera.main != null)
            _MainCameraObj = Camera.main.gameObject;

        if (_MainCameraObj != null)
            _MainCameraObj.SetActive(type);

        //황비홍 프로젝트에선 친구관련 컨텐츠가 없음
        //< 친구 방문시에는 그냥 밑에 패스
        //if (NetData.instance.friendVisitInfo != null && NetData.instance.friendVisitInfo.Active)
        //    return;

        //< 마을일경우 맵도 껏다켜준다
        if (SceneManager.instance != null && SceneManager.instance.CurrState() == _STATE.TOWN)
        {
            //< 건물도 숨기고 켜주고 한다
            //if (StructureMgr.instance != null)
            //    StructureMgr.instance.SetBuildActive(type);

            //GameObject MapObjects = SceneManager.instance.GetState<TownState>().MapObjects;

            //if (MapObjects != null)
            //    MapObjects.SetActive(type);
        }
    }

    public static void SetMainCameraDepth(int _depth)
    {
        if (_MainCameraObj == null && Camera.main != null)
            _MainCameraObj = Camera.main.gameObject;

        if (_MainCameraObj != null)
            _MainCameraObj.GetComponent<Camera>().depth = _depth;
    }

    #region :: UIWidget ::

    /// <summary>
    /// 스프라이트 변경 및 사이즈 조절
    /// </summary>
    /// <param name="spriteName">변경하고자 하는 이름</param>
    /// <param name="scale">변경한 이미지의 비율 (원본 크기에 비례)</param>
    public static void ChangeSprite(UISprite sprite, string spriteName, float scale = 1f)
    {
        sprite.spriteName = spriteName;
        ResizeSprite(sprite, scale);
    }

    /// <summary>
    /// 등급표시(별) 스프라이트 전용 함수
    /// </summary>
    /// <param name="grade">변경하고자 하는 등급 (1 ~ 7)</param>
    /// <param name="scale">스케일 비율</param>
    public static void ChangeGradeSprite(UISprite sprite, int grade, float scale = 1f, uint awaken = 0)
    {
        grade = Mathf.Clamp(grade, 1, 7);

        sprite.spriteName = awaken == 0 ? "n_star_" + grade : "n_star_" + grade + "_g";
        sprite.MakePixelPerfect();
        ResizeSprite(sprite, scale);
    }

    /// <summary>
    /// 스프라이트를 원본사이즈로 변경.
    /// </summary>
    /// <param name="sprite">대상 UISprite</param>
    /// <param name="scale">스케일 비율</param>
    public static void ResizeSprite(UISprite sprite, float scale = 1f)
    {
        if (null == sprite.GetAtlasSprite())
            return;
        sprite.SetDimensions(Mathf.RoundToInt(sprite.GetAtlasSprite().width * scale),
                              Mathf.RoundToInt(sprite.GetAtlasSprite().height * scale));
    }


    #endregion

    #region :: 공용팝업 ::
    /// <summary>
    /// Confirm이 null 이면 1버튼 Confirm이 notnull 이면 2버튼 팝업 생성
    /// {0} == changemsg1, {1} == changemsg2
    /// </summary>
    public static void OpenPopup(ushort msg, string changemsg1 = null, string changemsg2 = null, System.Action<GameObject> Confirm = null)
    {

        UIMgr.Open("PopupBase", msg, changemsg1, changemsg2, Confirm);
    }

    #endregion

    #region :: UI를 위한 3D모델 ::
    ////////////////////////여기부터 황비홍 프로젝트 신규 함수
    /// <summary>
    /// 플레이어 생성
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="charIdx"></param>
    /// <param name="HeadItemIdx"></param>
    /// <param name="CostumeItemIdx"></param>
    /// <param name="ClothItemIdx"></param>
    /// <param name="WeaponItemIdx"></param>
    /// <param name="HideCostume"></param>
    /// <returns></returns>
    /// bool isLighting, bool isShadow, bool isRim=true, bool isEff=true
    public static GameObject CreatePcUIModel(string panelName, Transform parent, uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, uint skillSetId , byte aniState, bool HideCostume, bool isShadow, bool isWeapon = true)
    {
        if (0 < parent.childCount)
        {
            int count = parent.childCount;
            while (0 < count)
            {
                --count;
                if (parent.GetChild(count).name.Contains("pc"))//혹시모를 예외처리
                    GameObject.Destroy(parent.GetChild(count).gameObject);
            }
        }

        GameObject[] effect = isWeapon ? new GameObject[2] : null;
        UnitAnimator unitAnim = new UnitAnimator();

        //뻑날때를 대비해 예외처리
        GameObject _unit = UnitModelHelper.PCModelLoadRimSpec(charIdx, HeadItemIdx, CostumeItemIdx, ClothItemIdx, WeaponItemIdx, HideCostume, ref effect, isWeapon);

        //Item.CostumeInfo lowData = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx);

        Resource.AniInfo[] ads = null;

        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        if (setInfo != null)
            ads = _LowDataMgr.instance.UIAniInfoSetting(setInfo.AniPath, _unit, setInfo.AniId, aniState);

        if (effect != null)//이펙트의 사이즈를 조정해야 하는데 미리 켜져있을 경우 활활 거리는게 있어서 추가함.
        {
            int loopCount = effect.Length;
            for (int i = 0; i < loopCount; i++)
            {
                if (effect[i] != null)
                    effect[i].SetActive(false);
            }
        }

        unitAnim.Init(_unit, _unit.animation, ads);
        unitAnim.Animation.playAutomatically = false;
        unitAnim.PlayAnim(eAnimName.Anim_idle);

        UIModel model = _unit.AddComponent<UIModel>();
        model.Init(parent, unitAnim, effect, charIdx == 13000, isShadow, panelName);

        return _unit;
    }

    ////////////////////////여기부터 황비홍 프로젝트 신규 함수
    /// <summary> 파트너 생성 </summary>
    public static GameObject CreatePartnerUIModel(Transform parent, uint partnerIdx, byte aniState, bool isLight, bool isShadow, string panelName)
    {
        if (0 < parent.childCount)
        {
            int count = parent.childCount;
            while (0 < count)
            {
                --count;
                if (parent.GetChild(count).name.Contains("par"))//혹시모를 예외처리
                    GameObject.Destroy(parent.GetChild(count).gameObject);
            }
        }

        GameObject[] effect = isLight ? new GameObject[2] : null;//강제로 isLight가 false라면 이펙트 생성 하지않는다.
        UnitAnimator unitAnim = new UnitAnimator();

        Partner.PartnerDataInfo lowData = _LowDataMgr.instance.GetPartnerInfo(partnerIdx);
        string leftWeaDummy = lowData.LeftWeaDummy;
        if (!string.IsNullOrEmpty(leftWeaDummy) && leftWeaDummy.Contains("Fx_par_jaejee_weapon"))
            leftWeaDummy = ""; //string.Format("{0}_UI", leftWeaDummy);//재지의 경우 UI용으로 별도 가지고있다.

		//GameObject _unit = UnitModelHelper.PartnerModelLoad(partnerIdx, ref effect, isLight, lowData.RightWeaDummy, leftWeaDummy, QualityManager.instance.GetModelQuality());
		// UI용 파트너는 퀄리티 설정에 상관없이 항상 _s 프리팹 사용 // kyh.
		GameObject _unit = UnitModelHelper.PartnerModelLoad (partnerIdx, ref effect, isLight, lowData.RightWeaDummy, leftWeaDummy, ModelQuality.UI);

        int childCount = _unit.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (_unit.transform.GetChild(i).renderer == null)
                continue;

            Material[] m = _unit.transform.GetChild(i).renderer.materials;
            if (m == null)
                continue;

            int mCount = m.Length;
            for (int j = 0; j < mCount; j++)
            {
                m[j].SetFloat("_IsRim", 0);
            }
        }

        //졌을경우 
        Resource.AniInfo[] ads = _LowDataMgr.instance.UIAniInfoSetting(
            lowData.prefab,
            _unit, lowData.AniId,
            aniState);

        if (effect != null)//이펙트의 사이즈를 조정해야 하는데 미리 켜져있을 경우 활활 거리는게 있어서 추가함.
        {
            int loopCount = effect.Length;
            for (int i = 0; i < loopCount; i++)
            {
                if (effect[i] != null)
                    effect[i].SetActive(false);
            }
        }

        unitAnim.Init(_unit, _unit.animation, ads);
        unitAnim.Animation.playAutomatically = false;
        unitAnim.PlayAnim(eAnimName.Anim_idle);

        UIModel model = _unit.AddComponent<UIModel>();
        model.Init(parent, unitAnim, effect, true, isShadow, panelName);

        return _unit;
    }

    public static GameObject CreateTalkNPC(string PanelName, Transform parent, string NPCPrefName, bool isRim, bool UseUICamera = true)
    {
        if (0 < parent.childCount)
            GameObject.DestroyImmediate(parent.GetChild(0).gameObject);


        //GameObject[] effect = isCreateEffect ? new GameObject[2] : null;
        //UnitAnimator unitAnim = new UnitAnimator();

        bool isLight = false;
        string lightModelName = NPCPrefName;
        if (ResourceMgr.Load(string.Format("Character/Prefab/{0}_s", lightModelName)) != null)
        {
            lightModelName = string.Format("{0}_s", NPCPrefName);
            isLight = true;
        }

        GameObject _unit = UnitModelHelper.ModelLoadtoString(lightModelName);
        if (isLight)
        {
            int childCount = _unit.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (_unit.transform.GetChild(i).renderer == null)
                    continue;

                Material[] m = _unit.transform.GetChild(i).renderer.materials;
                if (m == null)
                    continue;

                int mCount = m.Length;
                for (int j = 0; j < mCount; j++)
                {
                    m[j].SetFloat("_IsRim", isRim ? 1 : 0);
                }
            }
        }
        //PartnerTableLowData.PartnerInfo lowData = _LowDataMgr.instance.GetPartnerInfo(partnerIdx);
        //_ResourceLowData.AniTableInfo[] ads = _LowDataMgr.instance.TownAniInfoSetting(lowData.prefab, _unit, lowData.AniId);

        /*
        if (effect != null)//이펙트의 사이즈를 조정해야 하는데 미리 켜져있을 경우 활활 거리는게 있어서 추가함.
        {
            int loopCount = effect.Length;
            for (int i = 0; i < loopCount; i++)
            {
                if (effect[i] != null)
                    effect[i].SetActive(false);
            }
        }

        unitAnim.Init(_unit, _unit.animation, ads);
        unitAnim.Animation.playAutomatically = false;
        unitAnim.PlayAnim(isBattleIdle ? eAnimName.Anim_battle_idle : eAnimName.Anim_idle);
        */

        UIModel model = _unit.AddComponent<UIModel>();
        model.Init(PanelName, parent, NPCPrefName.Contains("pc_d_"), UseUICamera);

        return _unit;
    }



    /// <summary> 몬스터 생성 </summary>
    public static GameObject CreateMonsterUIModel(Transform parent, uint mobId, bool isBattleIdle, bool isRim, bool isShadow, string panelName)
    {
        bool isLight = false;
        Mob.MobInfo mobLowData = _LowDataMgr.instance.GetMonsterInfo(mobId);
        string prefabName = mobLowData.prefab;
        if (0 < parent.childCount)
        {
            if (parent.GetChild(0).name.Contains(prefabName))
                return null;

            GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
        }

        if (ResourceMgr.Load(string.Format("Character/Prefab/{0}_s", prefabName)) != null)
        {
            isLight = true;
            prefabName = string.Format("{0}_s", prefabName);
        }

        GameObject _unit = GameObject.Instantiate(Resources.Load(string.Format("Character/Prefab/{0}", prefabName))) as GameObject;
        if (isLight)
        {
            int childCount = _unit.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (_unit.transform.GetChild(i).renderer == null)
                    continue;

                Material[] m = _unit.transform.GetChild(i).renderer.materials;
                if (m == null)
                    continue;

                int mCount = m.Length;
                for (int j = 0; j < mCount; j++)
                {
                    m[j].SetFloat("_IsRim", isRim ? 1 : 0);
                }
            }
        }

        Resource.AniInfo[] ads = _LowDataMgr.instance.TownAniInfoSetting(mobLowData.prefab, _unit, mobLowData.AniId, true);

        UnitAnimator unitAnim = new UnitAnimator();
        unitAnim.Init(_unit, _unit.animation, ads);
        unitAnim.Animation.playAutomatically = false;
        unitAnim.PlayAnim(isBattleIdle ? eAnimName.Anim_battle_idle : eAnimName.Anim_idle);

        UIModel model = _unit.AddComponent<UIModel>();
        model.Init(parent, unitAnim, null, true, isShadow, panelName);

        return _unit;
    }

    #endregion

    #region :: 친구방문 ::

    #endregion

    #region :: 길드 ::

    #endregion

    #region DevHelper
    public enum ConsolLogType { Log, Warning, Error };
    static public void ConsolLog(ConsolLogType _clt, string Title = "", string Disc = "", Object obj = null)
    {
#if UNITY_EDITOR
        switch (_clt)
        {
            case ConsolLogType.Log: Debug.Log(Disc.Equals("") ? Title : string.Format("{0} : {1}", Title, Disc), obj); break;
            case ConsolLogType.Warning: Debug.LogWarning(Disc.Equals("") ? Title : string.Format("{0} : {1}", Title, Disc), obj); break;
            case ConsolLogType.Error: Debug.LogError(Disc.Equals("") ? Title : string.Format("{0} : {1}", Title, Disc), obj); break;
        }
#endif
    }
    #endregion

    /// <summary>
    /// 챕터를 생성한다. 지금은 가라 데이터가 들어가 있다. 추후 수정해야함.
    /// </summary>
    /// <param name="chapterNumber"> 챕터 아이디</param>
    /// <returns></returns>
    public static GameObject CreateUIChapter(int chapterNumber, Transform parent, bool isHard)
    {
        string chapObj = isHard ? "HardChapter_0" : "Chapter_0";
        string path = string.Format("UI/UIObject/{0}{1}", chapObj, chapterNumber);

        GameObject obj = ResourceMgr.Load(path) as GameObject;
        if (obj == null)
            return null;

        GameObject chapterGo = GameObject.Instantiate(obj) as GameObject;
        chapterGo.transform.parent = parent;
        chapterGo.transform.localPosition = Vector3.zero;
        chapterGo.transform.localScale = Vector3.one;
        chapterGo.SetActive(false);

        return chapterGo;
    }

    public static void CreateSlotItem(bool setTarget, int createCount, Transform targetSlotTf, Transform parent, System.Action<Transform, int> callBackFuc)
    {
        for (int i = setTarget ? 1 : 0; i < createCount; i++)
        {
            Transform newSlotTf = Transform.Instantiate(targetSlotTf) as Transform;
            newSlotTf.parent = parent;
            newSlotTf.localPosition = Vector3.zero;
            newSlotTf.localScale = Vector3.one;

            newSlotTf.name = string.Format("{0}", targetSlotTf.name, i);

            if (callBackFuc != null)
                callBackFuc(newSlotTf, i);
        }

        if (setTarget && callBackFuc != null)
            callBackFuc(targetSlotTf, 0);
    }

    /// <summary>
    /// 획득한 아이템에 셋팅하려고 정의해놓은 가라 데이터 추후에 꼭 삭제하자.
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="itemIcon"></param>
    /// <param name="itemName"></param>
    /// <param name="nowStack"></param>
    /// <param name="maxStack"></param>
    public static void GetFakeItemData(uint itemIndex, ref string itemIcon, ref string itemName, ref int nowStack)
    {
        ItemType type;
        switch (itemIndex)
        {
            case 1:
                type = ItemType.Weapon;
                itemIcon = "icon_weapon_1110";
                break;
            case 2:
                type = ItemType.Dress;
                itemIcon = "icon_armor_01";
                break;
            case 3:
                type = ItemType.Cap;
                itemIcon = "icon_helmet_01";
                break;
            case 4:
                type = ItemType.NeckLace;
                itemIcon = "icon_neck_lace";
                break;
            case 5:
                type = ItemType.Ring;
                itemIcon = "icon_ring";
                break;
            default:
                type = ItemType.Weapon;
                itemIcon = "icon_weapon_1110";
                break;
        }

        nowStack = 1;
        itemName = type.ToString();
    }

    /// <summary>
    /// 성공, 실패 이펙트 생성
    /// </summary>
    /// <param name="isSuccess">성공? 실패?</param>
    /// <param name="parent">어미가 될 객체</param>
    public static void CreateResultEffect(bool isSuccess, Transform parent)
    {
        string effName = isSuccess ? "success" : "failure";
        Object obj = Resources.Load(string.Format("Effect/_UI/_INGAME/Fx_IN_{0}_01", effName));
        if (obj == null)
        {
            Debug.LogError("Not find Effect effect name = Fx_IN_" + effName);
            return;
        }

        GameObject eff = GameObject.Instantiate(obj) as GameObject;
        eff.transform.parent = parent;
        eff.transform.localPosition = Vector3.zero;//new Vector3(0, 0, -100);
        eff.transform.localScale = Vector3.one;
    }

    /// <summary> _UI/INGAME 폴더에 있는 이펙트 생성 </summary>
    public static GameObject CreateEffectInGame(Transform parent, string name, bool isChangeLayer = true, int setRenderQ = -1)
    {
        string path = string.Format("Effect/_UI/_INGAME/{0}", name);
        GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
        effGo.transform.parent = parent;
        effGo.transform.localScale = Vector3.one;
        effGo.transform.localPosition = Vector3.zero;

        if (isChangeLayer)
        {
            effGo.layer = parent.gameObject.layer;
            effGo.transform.SetChildLayer(parent.gameObject.layer);
        }

        if (0 < setRenderQ)//하위 객체들 다 랜더큐 값으로 넣어줌
        {
            if (0 < effGo.transform.childCount)
            {
                List<MeshRenderer> list = FindComponents<MeshRenderer>(effGo.transform);
                for (int i = 0; i < list.Count; i++)
                {
                    SetRenderQueue q = list[i].transform.GetComponent<SetRenderQueue>();
                    if (q == null)
                        q = list[i].gameObject.AddComponent<SetRenderQueue>();

                    q.ResetRenderQ(setRenderQ);
                }
            }
        }

        return effGo;
    }

    /// <summary> 아이템 상세 팝업 생성 함수. </summary>
    //public static ItemDetailPopup CreateItemDetailPopup(UIBasePanel basePanel, byte depth = 3, bool isAutoQ = false)
    //{
    //    GameObject popup = GameObject.Instantiate(Resources.Load("UI/UIPopup/DetailPopup")) as GameObject;
    //    popup.transform.parent = basePanel.transform;
    //    popup.transform.localPosition = new Vector3(0, -30, -1500);
    //    popup.transform.localScale = Vector3.one;

    //    ItemDetailPopup popupSc = popup.GetComponent<ItemDetailPopup>();
    //    popupSc.Initailize(basePanel, depth, isAutoQ);

    //    return popupSc;
    //}

    public static GameObject CreateBlurPanel(Transform parent)
    {
        GameObject popup = GameObject.Instantiate(Resources.Load("UI/UIObject/BlurPanel")) as GameObject;
        popup.transform.parent = parent;
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;

        return popup;
    }

    /// <summary> 인벤 아이템 슬롯 생성 </summary>
    public static GameObject CreateInvenSlot(Transform parent)
    {
        GameObject popup = GameObject.Instantiate(Resources.Load("UI/UIObject/InvenItemSlotObject")) as GameObject;
        popup.transform.parent = parent;
        popup.transform.localPosition = Vector3.zero;
        popup.transform.localScale = Vector3.one;
        popup.transform.localEulerAngles = Vector3.zero;

        return popup;
    }

    public static T FindComponent<T>(Transform target) where T : Component
    {
        T mainT = target.GetComponent<T>();
        if (mainT != null)
            return mainT;

        int childCount = target.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTf = target.GetChild(i);
            T t = childTf.GetComponent<T>();
            if (t != null)
                return t;
            if (0 < childTf.childCount)
            {
                t = FindComponent<T>(childTf);
                if (t != null)
                    return t;
            }
        }

        return null;
    }

    public static List<T> FindComponents<T>(Transform target) where T : Component
    {
        List<T> list = new List<T>();
        T[] mainT = target.GetComponents<T>();
        if (mainT != null)
            list.AddRange(mainT);

        int childCount = target.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTf = target.GetChild(i);
            if (0 < childTf.childCount)
            {
                List<T> newList = FindComponents<T>(childTf);
                if (newList != null)
                    list.AddRange(newList);
            }
            else
            {
                T[] t = childTf.GetComponents<T>();
                if (t != null)
                    list.AddRange(t);
            }
        }

        return list;
    }

    public static string GetClassIcon(uint charIdx)
    {
        switch (charIdx)
        {
            case 11000:
                return "Img_fighter";
            case 12000:
                return "Img_pozol";
            case 13000:
                return "Img_doctor";
        }

        return null;
    }

    public static int GetClassType(uint charIdx)
    {
        switch (charIdx)
        {
            case 11000:
                return 1;
            case 12000:
                return 2;
            case 13000:
                return 3;
        }

        return 0;
    }

    public static string GetClassPortIcon(uint charIdx, int num = 1)
    {
        switch (charIdx)
        {
            case 11000:
                return string.Format("pc_f_port_0{0}", num);
            case 12000:
                return string.Format("pc_p_port_0{0}", num);
            case 13000:
                return string.Format("pc_d_port_0{0}", num);
        }

        return null;
    }

    public static string GetItemGradeColor(int grade)
    {
        switch (grade)
        {
            case 0:
                return "[FFFFFF]";
            case 1:
                return "[5BAB37]";
            case 2:
                return "[237EB3]";
            case 3:
                return "[7842A8]";
            case 4:
                return "[A2712C]";
            case 5:
                return "[B19E24]";
            case 6:
                return "[9E3234]";
        }

        return null;
    }

    public static string GetDefaultEquipIcon(ePartType type)
    {
        string defaultIcon = null;
        switch (type)
        {
            case ePartType.CLOTH:
                defaultIcon = "Icon_default_03";
                break;
            case ePartType.HELMET:
                defaultIcon = "Icon_default_01";
                break;
            case ePartType.NECKLACE:
                defaultIcon = "Icon_default_02";
                break;
            case ePartType.RING:
                defaultIcon = "Icon_default_04";
                break;
            case ePartType.SHOES:
                defaultIcon = "Icon_default_05";
                break;
            case ePartType.WEAPON:
                defaultIcon = "Icon_default_06";
                break;

        }

        return defaultIcon;
    }

    /// <summary> Item_EquipmentSet에 있는 Type에 따른 아이콘 </summary>
    public static string GetEquipSetIcon(byte setType)
    {
        //switch (setType)
        //{
        //    case 1 :
        //        break;//균형셋
        //    case 2 :
        //        break;//공격셋
        //    case 3 :
        //        break;//치명셋
        //    case 4 :
        //        break;//방어셋
        //    case 5 :
        //        break;//생명셋

        //    default:
        //        Debug.LogError("unDefined type error " + setType);
        //        break;
        //}

        return string.Format("Img_Set0{0}", setType);
    }
}


