using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void PointerNonUIHitInfo(POINTER_INFO ptr);

public struct POINTER_INFO
{
    public enum INPUT_EVENT
    {
        NO_CHANGE,
        PRESS,			// 장치에 의해 눌려졌을때.
        RELEASE,
        CLICK,
        DOUBLECLICK,
        DRAG
    }

    public UICamera.MouseOrTouch mouseOrTouch;

    public INPUT_EVENT evt;

    public Ray ray;

    public RaycastHit hitInfo;
}

public struct ClickUnitData
{
    public Unit target;
    public float dis;
}

/// <summary>
/// NGUI와 연동을 위한 InputManager
/// </summary>
/// <remarks>최대한 간단하게 제작</remarks>
public class InputManager : MonoSingleton<InputManager>//Immortal<InputManager> 
{
    /// <summary>
    /// 현재 게임에서의 메인 카메라를 연결해주도록 한다.
    /// </summary>
    [SerializeField]
    Camera gameCamera;

    public Camera GameCamera
    {
        get { return gameCamera; }
        set { gameCamera = value; }
    }

    public POINTER_INFO pointer;
    public GameObject ClickEffect;
    public GameObject InstanceClickEffect = null;
    public GameObject InstanceDragEffect = null;
    
    public PointerNonUIHitInfo informNonUIHit = (POINTER_INFO ptr) => { };

    public LayerMask InputCullingMask, InputCullingMask_InGame;

    // temporary variables
    RaycastHit hit;
    IInputObject tempObj;

    int prevTerrainLayer = -1;
    int terrainLayer;
    int terrainOutsideLayer;

    //protected override void Init()
    public override void OnInitialize()
    {
        //base.Init();
        base.OnInitialize();

        pointer = new POINTER_INFO();
        
        // Input이 UI가 아닐시, 이벤트를 받을 객체를 지정해준다.
        UICamera.fallThrough = this.gameObject;

        if (gameCamera == null)
            gameCamera = Camera.main;

        CullingMaskSetting();
    }

    void CullingMaskSetting()
    {
        InputCullingMask = -1;  // -1 : Everything, 0 : Nothing
        InputCullingMask.value ^= 1 << LayerMask.NameToLayer( "UILayer" );
        InputCullingMask.value ^= 1 << LayerMask.NameToLayer( "Ignore Raycast" );
        //InputCullingMask |= 1 << LayerMask.NameToLayer("Unit"); // Default
        //InputCullingMask |= 1 << LayerMask.NameToLayer("Terrain"); // Terrain

        terrainLayer = LayerMask.NameToLayer( "Terrain" );
        terrainOutsideLayer = LayerMask.NameToLayer( "Terrain_Outside" );


        InputCullingMask_InGame = -1;  // -1 : Everything, 0 : Nothing
        //InputCullingMask_InGame |= 1 << LayerMask.NameToLayer("Unit"); // Default
    }

    void OnLevelWasLoaded(int level)
    {
        if (gameCamera == null)
            gameCamera = Camera.main;
    }

    void ClickEvent()
    {
        //< 인게임 내에서의 터치
        if (!TownState.TownActive && G_GameInfo._GameInfo != null && G_GameInfo.PlayerController != null && G_GameInfo._GameInfo.AutoMode)
        {
            List<ClickUnitData> Clicks = new List<ClickUnitData>();

            RaycastHit[] hits = Physics.RaycastAll(pointer.ray, Mathf.Infinity, InputCullingMask_InGame);
            for (int i = 0; i < hits.Length; i++)
            {
                Unit unit = hits[i].collider.gameObject.GetComponent<Unit>();
                if (unit == null || unit.TeamID == 0 || unit.IsDie || !unit.Usable)
                    continue;

                ClickUnitData nData = new ClickUnitData();
                nData.target = unit;
                nData.dis = hits[i].distance;
                Clicks.Add(nData);
            }

            if(Clicks.Count > 0)
            {
                Clicks.Sort(delegate(ClickUnitData tmp1, ClickUnitData tmp2) { return tmp1.dis.CompareTo(tmp2.dis); });
                G_GameInfo.PlayerController.SetTargetUnit(Clicks[0].target);
            }
        }
    }

    void ProcessEvent()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        if (gameCamera.gameObject == null)
            return;

        pointer.mouseOrTouch = UICamera.currentTouch;
        pointer.ray = gameCamera.ScreenPointToRay(UICamera.currentTouch.pos);

        if (TownState.TownActive || G_GameInfo._GameInfo == null)
        {
            RaycastHit[] hits = Physics.RaycastAll(pointer.ray, Mathf.Infinity, InputCullingMask);
            for (int i = 0; i < hits.Length; i++)
            {
                // Terrain은 가장 가까운 객체 하나만 작동하도록 한다.
                int hitLayer = hits[i].collider.gameObject.layer;
                if (hitLayer == terrainLayer ||
                    hitLayer == terrainOutsideLayer)
                {
                    if (prevTerrainLayer == terrainLayer)
                        continue;
                    prevTerrainLayer = hitLayer;
                }

                pointer.hitInfo = hits[i];
                tempObj = (IInputObject)hits[i].collider.gameObject.GetComponent("IInputObject");

                if (tempObj != null)
                {
                    tempObj.InputEvent(pointer);
                    //ppzz2
                    if (pointer.evt == POINTER_INFO.INPUT_EVENT.CLICK)
                        break;
                }


                informNonUIHit(pointer);
            }
        }

        

        prevTerrainLayer = -1;
    }    
    
    #region Event Functions ----------------------------------

    void OnPress(bool isDown)
    {
        pointer.evt = isDown ? POINTER_INFO.INPUT_EVENT.PRESS : POINTER_INFO.INPUT_EVENT.RELEASE;

        ProcessEvent();
    }

    void OnDrag(Vector2 delta)
    {
        pointer.evt = POINTER_INFO.INPUT_EVENT.DRAG;

        ProcessEvent();
    }

    void OnClick()
    {
        // 드래그 했다가 Press off되면 NonUIHit 보내지 않기.
        if (pointer.mouseOrTouch.dragStarted)
            return;

        pointer.evt = POINTER_INFO.INPUT_EVENT.CLICK;

        ClickEvent();
        ProcessEvent();
    }

    void OnDoubleClick()
    {
        pointer.evt = POINTER_INFO.INPUT_EVENT.DOUBLECLICK;

        ProcessEvent();
    }

    void OnHover(bool isOver)
    {
        //Debug.Log("OnHover : " + isOver);
    }

    void OnSelect(bool selected)
    {
        //Debug.Log("OnSelect : " + selected);
    }

    void OnDrop(GameObject drag)
    {
        //Debug.Log("OnDrop : " + drag);
    }

    #endregion

    public void SetNonUIHitDelegate(PointerNonUIHitInfo del)
    {
        informNonUIHit = del;
    }

    public void AddNonUIHitDelegate(PointerNonUIHitInfo del)
    {
        informNonUIHit += del;
    }

    public void RemoveNonUIHitDelegate(PointerNonUIHitInfo del)
    {
        informNonUIHit -= del;
    }
}
