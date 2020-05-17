
﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterCreateState : SceneStateBase
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);
        LoadLevelAsync("CharacterCreateScene");
    }

    public override void OnExit(Action callback)
    {
        base.OnExit(callback);
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName == "CharacterCreateScene")
        {
            SceneSetting();
        }
    }











    //public string tempNickName = string.Empty;
    //public uint SelectedUnitId = 0;
    //public string tempUserId = string.Empty;

    //Dictionary<uint, GameObject> charDic = new Dictionary<uint, GameObject>();
    //Dictionary<uint, GameObject> uicharDic = new Dictionary<uint, GameObject>();

    //GameObject charTargetPoint;
    //GameObject SelectUI;
    //GameObject SelectCharacter;
    //GameObject CharacterSelectRoot;

    //GameObject CharacterCreatePrefab;

    //public GameObject CharacterCreateCamera;
    //public GameObject CharacterBackGroundRoot;

    //Vector3 KnightsPos = new Vector3(-415f, -304f, -300f);
    //Vector3 GunnerPos = new Vector3(410f, -301f, -500f);
    //Vector3 MagicianPos = new Vector3(-10f, -302f, -300f);

    //Vector3 KnightsModelPos = new Vector3(-2.53f, -1.83f, 5.62f);
    //Vector3 MagicianModelPos = new Vector3(-0.06f, -1.7f, 4.99f);
    //Vector3 GunnerModelPos = new Vector3(2.78f, -1.91f, 5.28f);

    //UISprite WhiteEffect;

    ////트윈 예외처리
    //bool bTween = true;

    //public override void OnEnter(System.Action callback)
    //{
    //    base.OnEnter(callback);
    //    LoadLevelAsync("CharacterCreateScene");
    //}

    //public override void OnExit(Action callback)
    //{
    //    if (null != CharacterCreateCamera)
    //        Destroy( CharacterCreateCamera );

    //    base.OnExit( callback );
    //}

    //void OnLevelWasLoaded(int level)
    //{
    //    if (Application.loadedLevelName == "CharacterCreateScene")
    //    {
    //        SceneSetting();

    //        LoadCreate();
    //    }
    //}

    //public void LoadCreate()
    //{
    //    foreach (uint key in charDic.Keys)
    //        DestroyImmediate(charDic[key]);
    //    foreach (uint key in uicharDic.Keys)
    //        DestroyImmediate(uicharDic[key]);

    //    charDic.Clear();
    //    uicharDic.Clear();
    //    bTween = true;
    //    CharacterCreatePrefab = Resources.Load("UI/CharacterSelect/CreateCharacterCamera") as GameObject;
    //    CreateCharacterPanel createcharacterpanel = UIMgr.Open("CharacterSelect/CreateCharacterPanel").GetComponent<CreateCharacterPanel>();
    //    SettingCharacterCreate();

    //    if (createcharacterpanel.Undo != null)
    //        EventDelegate.Remove(createcharacterpanel.Undo, UndoState);

    //    if (createcharacterpanel.CreateCharacterComplete != null)
    //        EventDelegate.Remove(createcharacterpanel.CreateCharacterComplete, GoNextState);

    //    EventDelegate.Set(createcharacterpanel.Undo, UndoState);
    //    EventDelegate.Set(createcharacterpanel.CreateCharacterComplete, GoNextState);
    //}

    //void SettingCharacterCreate()
    //{
    //    Transform uiDefaultRoot = UIMgr.instance.UICamera.transform.Find("Default");

    //    CharacterBackGroundRoot = UIMgr.Open("CharacterSelect/CharacterSelectRoot");
    //    CharacterBackGroundRoot.transform.AttachTo(uiDefaultRoot);

    //    CharacterCreateCamera = Instantiate(CharacterCreatePrefab) as GameObject;
    //    CharacterCreateCamera.transform.AttachTo(CameraManager.instance.transform);
    //    CharacterCreateCamera.transform.localPosition = CharacterCreatePrefab.transform.localPosition;
    //    CharacterCreateCamera.name = "CreateCharacterCamera";

    //    Transform KnightUIRoot = CharacterBackGroundRoot.transform.FindChild("Child1/Placement").transform;
    //    Transform MagicianUIRoot = CharacterBackGroundRoot.transform.FindChild("Child2/Placement").transform;
    //    Transform GunnerUIRoot = CharacterBackGroundRoot.transform.FindChild("Child3/Placement").transform;

    //    Transform KnightRoot = CharacterCreateCamera.transform.FindChild("Knight");
    //    Transform MagicianRoot = CharacterCreateCamera.transform.FindChild("Magician");
    //    Transform GunnerRoot = CharacterCreateCamera.transform.FindChild("Gunner");

    //    WhiteEffect = CharacterBackGroundRoot.transform.FindChild("EffectPanel/WhiteEffect").GetComponent<UISprite>();
    //    WhiteEffect.gameObject.SetActive(false);

    //    KnightUIRoot.transform.localPosition = KnightsPos;
    //    MagicianUIRoot.transform.localPosition = MagicianPos;
    //    GunnerUIRoot.transform.localPosition = GunnerPos;

    //    charTargetPoint = CharacterCreateCamera.transform.FindChild("TargetPoint").gameObject;

    //    CreateAndPlacementUnit( (uint)LowDataMgr.GetConfigValue("NewUnitPickList_1"), KnightUIRoot, KnightRoot);
    //    CreateAndPlacementUnit( (uint)LowDataMgr.GetConfigValue("NewUnitPickList_2"), MagicianUIRoot, MagicianRoot);
    //    CreateAndPlacementUnit( (uint)LowDataMgr.GetConfigValue("NewUnitPickList_3"), GunnerUIRoot, GunnerRoot);
    //}

    //void CreateAndPlacementUnit(uint unitId, Transform parent, Transform characterparent)
    //{
    //    string fileExtension = "_android.assetbundle";

    //    UnitLowData.GradeInfo unitData = LowDataMgr.GetUnitGradeData((uint)unitId, (byte)1, (uint)0);
    //    ResourceLowData.UnitInfo unitRes = LowDataMgr.GetUnitResourceData(unitData.resource);

    //    Debug.LogWarning(GetType() + ".CreateAndPlacementUnit : " + unitRes.modelFile + fileExtension);
    //    //UnitBuilder.CreateGKUnit(unitData.groupId, unitData.grade, (uint)0, characterparent.transform.position, Quaternion.Euler(Vector3.zero));

    //    AssetbundleLoader.AssetLoad(unitRes.modelFile + fileExtension, (GameObject _asset) =>
    //    {
    //        GameObject createdGO = GameObject.Instantiate(_asset) as GameObject;

    //        // 애니메이션 셋팅
    //        ResourceLowData.AniInfo[] animDatas = null;
    //        UnitDataFactory.SetupDefaultAnimForUnit(unitId, unitData.grade, (uint)0, out animDatas);
    //        AssetbundleLoader.AddAnimationClips(createdGO.animation, animDatas, () => 
    //        {
    //            if (null != createdGO.animation)
    //            {
    //                string aniName = LowDataMgr.GetAnimName(unitRes.aniIdle01);
    //                if (null != createdGO.animation.GetClip(aniName))
    //                    createdGO.animation.Blend(aniName);
    //                else
    //                    Debug.LogWarning(createdGO + "는 " + aniName + " 애니메이션이 없습니다.");
    //            }
    //        });

    //        NGUITools.SetLayer(createdGO, LayerMask.NameToLayer("Default"));
    //        createdGO.transform.AttachTo(characterparent, true);
    //        createdGO.transform.localPosition *= unitRes.collectionSize;

    //        UIPanel panel = parent.parent.GetComponent<UIPanel>();
    //        panel.depth = 0;
    //        panel.baseClipRegion = new Vector4(0, 0, 426, Screen.height);

    //        CustomClipAlpha clipalpha = characterparent.GetComponent<CustomClipAlpha>();
    //        clipalpha.SetPanel(panel);
    //        clipalpha.Init("Custom/ClipSahderRimSpec2");
    //        clipalpha.bActive = true;

    //        BoxCollider collider = parent.gameObject.AddComponent<BoxCollider>();
    //        collider.center = new Vector3(-27f, 304f, 0);
    //        collider.size = new Vector3(426f, 720f, 1);

    //        UIEventListener.Get(parent.gameObject).onDrag = (sender, delta) =>
    //        {
    //            createdGO.transform.parent.Rotate(0, -delta.x, 0);
    //        };

    //        UIEventListener.Get(parent.gameObject).onClick = (sender) =>
    //        {
    //            if (bTween == false)
    //                return;

    //            SelectUI = parent.gameObject;
    //            SelectCharacter = createdGO;
    //            createdGO.transform.parent.localRotation = Quaternion.Euler(0, 180, 0);


    //            foreach (uint key in charDic.Keys)
    //            {
    //                if (charDic[key] != createdGO)
    //                {
    //                    charDic[key].transform.localPosition = new Vector3(0, 0, 1000);
    //                    charDic[key].transform.parent.GetComponent<CustomClipAlpha>().bActive = false;
    //                }
    //            }

    //            foreach (uint key in uicharDic.Keys)
    //            {
    //                if (SelectUI == uicharDic[key])
    //                    uicharDic[key].GetComponent<BoxCollider>().enabled = true;
    //                else
    //                    uicharDic[key].GetComponent<BoxCollider>().enabled = false;
    //            }

    //            //ui 처리
    //            panel = parent.parent.GetComponent<UIPanel>();
    //            panel.depth = 10;

    //            float tPos = parent.transform.localPosition.x * -1;

    //            TweenPosition paneltw = panel.GetComponent<TweenPosition>();
    //            if (paneltw == null)
    //                paneltw = panel.gameObject.AddComponent<TweenPosition>();

    //            paneltw.ResetToBeginning();
    //            paneltw.from = new Vector3(0,
    //                              paneltw.transform.localPosition.y,
    //                              paneltw.transform.localPosition.z);

    //            paneltw.to = new Vector3(tPos,
    //                                      paneltw.transform.localPosition.y,
    //                                      paneltw.transform.localPosition.z);
    //            paneltw.duration = 0.2f;
    //            paneltw.PlayForward();

    //            EventDelegate.Callback panelcall = () =>
    //            {
    //                WhiteEffect.gameObject.SetActive(true);
    //                WhiteEffect.parent.GetComponent<UIPanel>().depth = 30;
    //                TweenAlpha alpha = WhiteEffect.GetComponent<TweenAlpha>();
    //                if (alpha == null)
    //                    alpha = WhiteEffect.gameObject.AddComponent<TweenAlpha>();
    //                alpha.onFinished.Clear();
    //                alpha.PlayForward();
    //                alpha.from = 0;
    //                alpha.to = 1f;
    //                alpha.duration = 0.01f;

    //                alpha.AddOnFinished(() =>
    //                {
    //                    alpha.duration = 0.5f;
    //                    alpha.PlayReverse();
    //                    bTween = false;
    //                });

    //            };

    //            UIMgr.Open("CharacterSelect/CreateCharacterPanel", unitId);
    //            SelectedUnitId = unitId;
    //            paneltw.AddOnFinished(panelcall);
    //            StartCoroutine(PanelWiden(panel, true));


    //            //character 처리
    //            float chartargetPosX = charTargetPoint.transform.localPosition.x;

    //            TweenPosition chartw = characterparent.GetComponent<TweenPosition>();
    //            if (chartw == null)
    //                chartw = characterparent.gameObject.AddComponent<TweenPosition>();


    //            chartw.ResetToBeginning();

    //            if (unitId == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_1"))
    //            {
    //                chartw.from = KnightsModelPos;

    //                chartw.to = new Vector3(chartargetPosX,
    //                                      KnightsModelPos.y,
    //                                      KnightsModelPos.z);

    //            }
    //            else if (unitId == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_2"))
    //            {
    //                chartw.from = MagicianModelPos;

    //                chartw.to = new Vector3(chartargetPosX,
    //                                      MagicianModelPos.y,
    //                                      MagicianModelPos.z);

                    
    //            }
    //            else if (unitId == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_3"))
    //            {
    //                chartw.from = GunnerModelPos;

    //                chartw.to = new Vector3(chartargetPosX,
    //                                      GunnerModelPos.y,
    //                                      GunnerModelPos.z);

    //                charDic[unitId].transform.parent.localScale = new Vector3(1.7f, 1.8f, 1.8f);

    //                 //(uint)LowDataMgr.GetConfigValue("NewUnitPickList_3"), GunnerUIRoot, 
    //            }

    //            chartw.duration = 0.2f;
    //            chartw.PlayForward();

    //            StartCoroutine(PanelWiden(panel, true));

    //        };

    //        charDic.Add((uint)unitId, createdGO);
    //        uicharDic.Add((uint)unitId, parent.gameObject);

    //    }, unitId);
    //}

    //void UndoState()
    //{
    //    bTween = true;

    //    foreach (uint key in uicharDic.Keys)
    //    {
    //        uicharDic[key].GetComponent<BoxCollider>().enabled = true;
    //    }

    //    UIPanel panel = SelectUI.transform.parent.GetComponent<UIPanel>();

    //    float tPos = SelectUI.transform.localPosition.x * -1;

    //    TweenPosition paneltw = panel.GetComponent<TweenPosition>();
    //    if (paneltw == null)
    //        paneltw = panel.gameObject.AddComponent<TweenPosition>();

    //    paneltw.onFinished.Clear();
    //    paneltw.ResetToBeginning();
    //    paneltw.from = new Vector3(tPos,
    //                              paneltw.transform.localPosition.y,
    //                              paneltw.transform.localPosition.z);

    //    paneltw.to = new Vector3(0,
    //                             paneltw.transform.localPosition.y,
    //                             paneltw.transform.localPosition.z);


    //    paneltw.duration = 0.2f;
    //    paneltw.PlayForward();



    //    float chartargetpos = SelectCharacter.transform.parent.localPosition.x;
    //    TweenPosition chartw = SelectCharacter.transform.parent.GetComponent<TweenPosition>();
    //    if (chartw == null)
    //        chartw = SelectCharacter.AddComponent<TweenPosition>();


    //    chartw.onFinished.Clear();
    //    chartw.ResetToBeginning();
    //    //chartw.PlayReverse();

    //    foreach (uint key in charDic.Keys)
    //    {
    //        if (charDic[key] == SelectCharacter)
    //        {
    //            if (key == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_1"))
    //            {
    //                chartw.from = new Vector3(chartargetpos,
    //                                          KnightsModelPos.y,
    //                                          KnightsModelPos.z);

    //                chartw.to = KnightsModelPos;

    //                if (GameDefine.TestMode)
    //                    Debug.Log("chartw1  " + chartw.transform.localPosition);

    //            }
    //            else if (key == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_2"))
    //            {
    //                chartw.from = new Vector3(chartargetpos,
    //                                          KnightsModelPos.y,
    //                                          KnightsModelPos.z);

    //                chartw.to = MagicianModelPos;
    //                Debug.Log("chartw2  " + chartw.transform.localPosition);



    //            }
    //            else if (key == (uint)LowDataMgr.GetConfigValue("NewUnitPickList_3"))
    //            {
    //                chartw.from = new Vector3(chartargetpos,
    //                                          KnightsModelPos.y,
    //                                          KnightsModelPos.z);

    //                chartw.to = GunnerModelPos;

    //                if (GameDefine.TestMode)
    //                    Debug.Log("chartw3  " + chartw.transform.localPosition);


    //                SelectCharacter.transform.parent.localScale = new Vector3(1.6f, 1.8f, 1.8f);

                    

    //            }
    //        }
    //    }

    //    chartw.duration = 0.2f;
    //    chartw.PlayForward();

    //    UIMgr.Open("CharacterSelect/CreateCharacterPanel");

    //    StartCoroutine(PanelWiden(panel, false));
    //}

    //IEnumerator PanelWiden(UIPanel _panel, bool _bstate)
    //{
    //    Vector2 size = _panel.GetViewSize();
    //    while (true)
    //    {

    //        if (_bstate == true)
    //        {
    //            size.x += 80;
    //            _panel.baseClipRegion = new Vector4(0, 0, size.x, 750f);

    //            if (Screen.width * 1.3f < _panel.baseClipRegion.z)
    //            {
    //                //_panel.baseClipRegion = new Vector4(0, 0, Screen.width, size.y);
    //                yield break;
    //            }

    //        }
    //        else
    //        {
    //            size.x -= 80;
    //            _panel.baseClipRegion = new Vector4(0, 0, size.x, 750f);

    //            if (426 > _panel.baseClipRegion.z)
    //            {
    //                _panel.baseClipRegion = new Vector4(0, 0, 426, 750f);

    //                foreach (uint key in charDic.Keys)
    //                {
    //                    _panel.depth = 0;

    //                    Transform Tr = _panel.transform.FindChild("Placement");
    //                    if (Tr != SelectUI)
    //                    {
    //                        charDic[key].transform.localPosition = Vector3.zero;
    //                        //  Tr.transform.GetComponent<CustomClipAlpha>().bActive = true;
    //                    }
    //                    else
    //                    {
    //                        Tr.localPosition = Vector3.zero;
    //                    }
    //                }

    //                yield break;
    //            }
    //        }

    //        yield return null;
    //    }
    //}
   
}
