using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/RtsCamera-Touch")]
public class RtsCameraTouch : MonoBehaviour 
{
    public enum TOUCH_STATE
    {
        NONE,
        DRAG,
    }

    private float       StartDist = 0;
    private bool        FirstZoom = false;
    public TOUCH_STATE  DragState = TOUCH_STATE.NONE;
    float               ZoomValue = 0;

    Vector3             ClickWorldPos;
    Vector3             DragWorldPos;
    Vector3             DragMovement;
    bool                ReversAction = false;
    public float        DragDamping = 10f;

    public bool AllowPan;
    public bool PanBreaksFollow;
    public float PanSpeed;

    public bool AllowRotate;
    public float RotateSpeed;

    public bool AllowTilt;
    public float TiltSpeed;

    public bool AllowZoom;
    public float ZoomSpeed;

    private RtsCamera _rtsCamera;

    void Reset()
    {
        AllowPan = true;
        PanBreaksFollow = true;
        PanSpeed = 50f;

        AllowRotate = true;
        RotateSpeed = 360f;

        AllowTilt = true;
        TiltSpeed = 200f;

        AllowZoom = true;
        ZoomSpeed = 500f;
    }

    void Start()
    {
        _rtsCamera = gameObject.GetComponent<RtsCamera>();

        InputManager.instance.AddNonUIHitDelegate(UpdateInput);
    }

    void Update()
    {
        if (_rtsCamera == null)
            return;

        if (AllowZoom)
        {
            if (Input.touchCount > 1)
            {
                TouchZoom();
            }

            _rtsCamera.Distance -= ZoomValue * ZoomSpeed * Time.deltaTime;
        }
    }

    void TouchZoom()
    {
        if (TOUCH_STATE.NONE == DragState)
        {
            StartDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            DragState = TOUCH_STATE.DRAG;
            FirstZoom = true;
        }
        else
        {
            if (FirstZoom)
            {
                FirstZoom = false;
                return;
            }
			
            float dist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
			
            float offset = dist - StartDist;
            if (Mathf.Abs(offset) > 100)
                offset *= 0.01f;
            StartDist = dist;
            ZoomValue = offset * 0.1f;
        }
    }

    public void UpdateInput(POINTER_INFO ptr)
    {
        if (ptr.evt != POINTER_INFO.INPUT_EVENT.DRAG && ptr.evt != POINTER_INFO.INPUT_EVENT.PRESS)
            return;
		
        DragCamera(ptr);
    }

    bool DragCamera(POINTER_INFO ptr)
    {
        if (Input.touchCount > 1)
            return false;

        Vector3 worldPos = Vector3.zero;

        if (!CalcluateGroundPos(ptr.ray, ref worldPos, "Water"))
        {
            return false;
        }
		
        switch (ptr.evt)
        {
        case POINTER_INFO.INPUT_EVENT.PRESS:
            ClickWorldPos = worldPos;
            break;
        case POINTER_INFO.INPUT_EVENT.DRAG:
            if (DragState == TOUCH_STATE.DRAG)
            {
                ClickWorldPos = worldPos;
                DragState = TOUCH_STATE.NONE;

                break;
            }
            worldPos.y = ClickWorldPos.y;
            DragWorldPos = worldPos;

            if (!ReversAction)
            {
                DragMovement = ClickWorldPos - DragWorldPos;
            }
            else
                ClickWorldPos += (DragWorldPos - ClickWorldPos) * 2f;

            break;
        }
        return true;
		
    }

    bool CalcluateGroundPos(Ray ray, ref Vector3 planpos, string maskname)
    {
        RaycastHit hitdata;
        int mask = 1 << LayerMask.NameToLayer(maskname);
        if (Physics.Raycast(ray, out hitdata, 100000, mask))
        {
            planpos = hitdata.point;
            return true;
        }
        else return false;
    }

    Vector3 UpdateDrag()
    {
        Vector3 camPos = transform.position;

        if (DragMovement != Vector3.zero)
        {
            float t = DragDamping * Time.smoothDeltaTime;
            Vector3 delta = DragMovement * t;
            camPos = Vector3.Lerp(transform.position, transform.position + DragMovement, t);
            DragMovement -= delta;
        }
        return camPos;
    }

    ///// <summary>
    ///// 카메라 이동 가능 영역
    ///// </summary>
    //public Rect         moveableRect;
    //public float        DragDamping = 10f;
    
    //Vector3             DragWorldPos;
    //Vector3             DragMovement;
    //Vector3             ClickWorldPos;
    //Vector3             NewPos;

    //public bool         isFreezing = false;
    
    //[SerializeField]
    //bool ReversAction = false;

    //void Start()
    //{
    //    InputManager.instance.AddNonUIHitDelegate(UpdateInput);
    //}

    //void OnDestroy()
    //{
    //}

    //void Update()
    //{	
    //    if (isFreezing)
    //        return;
		
    //    NewPos = UpdateDrag();
		
    //    #if !UNITY_EDITOR || (UNITY_IPHONE || UNITY_ANDROID)
    //    DragZoom();
    //    #endif
		
    //    CheckBoundary(NewPos);
    //}

    //public void UpdateInput(POINTER_INFO ptr)
    //{
    //    if (ptr.evt != POINTER_INFO.INPUT_EVENT.DRAG && ptr.evt != POINTER_INFO.INPUT_EVENT.PRESS)
    //        return;
		
    //    DragCamera(ptr);
    //}

    //Vector3 UpdateDrag()
    //{
    //    Vector3 camPos = transform.position;

    //    if (DragMovement != Vector3.zero)
    //    {
    //        float t = DragDamping * Time.smoothDeltaTime;
    //        Vector3 delta = DragMovement * t;
    //        camPos = Vector3.Lerp(transform.position, transform.position + DragMovement, t);
    //        DragMovement -= delta;
    //    }
    //    return camPos;
    //}

    //#region ::	Camera Drag
	
    //bool DragCamera(POINTER_INFO ptr)
    //{

    //    if (Input.touchCount > 1)
    //        return false;
    //    Vector3 worldPos = Vector3.zero;

    //    if (!CalcluateGroundPos(ptr.ray, ref worldPos, "Water"))
    //    {
    //        return false;
    //    }
		
    //    switch (ptr.evt)
    //    {
    //    case POINTER_INFO.INPUT_EVENT.PRESS:
    //        ClickWorldPos = worldPos;
    //        break;
    //    case POINTER_INFO.INPUT_EVENT.DRAG:
    //        if (DragState == TOUCH_STATE.DRAG)
    //        {
    //            ClickWorldPos = worldPos;
    //            DragState = TOUCH_STATE.NONE;

    //            break;
    //        }
    //        worldPos.y = ClickWorldPos.y;
    //        DragWorldPos = worldPos;

    //        if (!ReversAction)
    //        {
    //            DragMovement = ClickWorldPos - DragWorldPos;
    //        }
    //        else
    //            ClickWorldPos += (DragWorldPos - ClickWorldPos) * 2f;

    //        break;
    //    }
    //    return true;
		
    //}
	
    //#if !UNITY_EDITOR || (UNITY_IPHONE || UNITY_ANDROID)
    //bool DragZoom()
    //{
    //    if (Input.touchCount <= 1)
    //        return false;
		
    //    TouchZoom();
    //    return true;
    //}
    //#endif

    //void TouchZoom()
    //{
    //    if (TOUCH_STATE.NONE == DragState)
    //    {
    //        StartDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
    //        DragState = TOUCH_STATE.DRAG;
    //        FirstZoom = true;
    //    }
    //    else
    //    {
    //        if (FirstZoom)
    //        {
    //            FirstZoom = false;
    //            return;
    //        }
			
    //        float dist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
			
    //        float offset = dist - StartDist;
    //        if (Mathf.Abs(offset) > 100)
    //            offset *= 0.01f;
    //        StartDist = dist;
    //        ZoomValue = offset * 0.1f;
    //    }
		
    //    Zoom();
    //}

    //bool CalcluateGroundPos(Ray ray, ref Vector3 planpos, string maskname)
    //{
    //    RaycastHit hitdata;
    //    int mask = 1 << LayerMask.NameToLayer(maskname);
    //    if (Physics.Raycast(ray, out hitdata, 100000, mask))
    //    {
    //        planpos = hitdata.point;
    //        return true;
    //    }
    //    else return false;
    //}

    //void CheckBoundary(Vector3 movingPos)
    //{

    //    if (movingPos.x < moveableRect.xMin)
    //        movingPos.x = moveableRect.xMin;
    //    if (movingPos.z < moveableRect.yMin)
    //        movingPos.z = moveableRect.yMin;
    //    if (movingPos.x > moveableRect.xMax)
    //        movingPos.x = moveableRect.xMax;
    //    if (movingPos.z > moveableRect.yMax)
    //        movingPos.z = moveableRect.yMax;

    //    transform.position = movingPos;
    //}

    //public void ReversActionFlag(bool flag)
    //{
    //    ReversAction = flag;
    //}

    //#region ::	Camera Zoom
	
    //[SerializeField]
    //float ZoomValue = 0;
	
    //[SerializeField]
    //float ZoomOffset = 10;

    //public float MaxHeight;
    //public float ResetMaxHeight;
    //public float InitMaxHeight;

    //[SerializeField]
    //float MinHeight = 15;
	
    //#if !UNITY_EDITOR || (UNITY_IPHONE || UNITY_ANDROID)    
    //void TouchZoom()
    //{
    //    if (TOUCH_STATE.NONE == DragState)
    //    {
    //        StartDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
    //        DragState = TOUCH_STATE.DRAG;
    //        FirstZoom = true;
    //    }
    //    else
    //    {
    //        if (FirstZoom)
    //        {
    //            FirstZoom = false;
    //            return;
    //        }
			
    //        float dist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
			
    //        float offset = dist - StartDist;
    //        if (Mathf.Abs(offset) > 100)
    //            offset *= 0.01f;
    //        StartDist = dist;
    //        ZoomValue = offset * 0.1f;
    //    }
		
    //    Zoom();
    //}
    //#endif
	
    //void MouseZoom()
    //{
    //    ZoomValue = Input.GetAxis("Mouse ScrollWheel") * 10;

    //    if (ZoomValue != 0)
    //    {
    //        Zoom();
    //    }
    //}
	
    //void Zoom()
    //{
    //    //camera.fieldOfView -= ZoomValue;
    //    ZoomValue = ZoomValue / 10f;
    //    Vector3 TransPos = NewPos + (transform.forward * (ZoomValue * ZoomOffset));
		
    //    if (TransPos.y < MinHeight || TransPos.y > MaxHeight)
    //    {
    //        TransPos.y = Mathf.Clamp(TransPos.y, MinHeight, MaxHeight);
    //        TransPos.x = NewPos.x;
    //        TransPos.z = NewPos.z;
    //    }
		
    //    NewPos = TransPos;

    //    ZoomValue = 0;
    //}
    //#endregion
}