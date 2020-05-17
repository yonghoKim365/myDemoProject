using UnityEngine;
using System.Collections;

public class Joystick : UIBasePanel {
    
    public GameObject JoyObj;
    public Transform joyBol;

    public Vector2 BolPosition;
    
    public Camera CurrentCam;
    public bool IsPress;
    public bool IsActive = true;
    public float MoveDistance = 20;
    public float LerpSpeed = 5f;
    public float Radius = 0.3f;
    
    private Vector3 ResetJoyPos;

    private CallBack OnMoveJoy;
    private CallBack OnStartJoy;
    private CallBack OnEndJoy;

    private Transform DummyTf;
    private Vector2 BoxSize;
    private Touch CurTouch;
    private bool IsEditor;

    public delegate void CallBack(Joystick joy);

    /// <summary> 외부에서 쓸 변수 </summary>
    public Touch CurrentTouch
    {
        get {
            return CurTouch;
        }
    }

    //public void Awake()
    //{
    //    JoyObj.SetActive(true);
        
    //    DummyTf = new GameObject("Dummy").transform;
    //    DummyTf.parent = transform;
    //    ResetJoyPos = JoyObj.transform.localPosition;
    //    BoxSize = collider.GetComponent<BoxCollider>().size;
    //}

    public override void Init()
    {
        base.Init();
#if UNITY_EDITOR
        IsEditor = true;
#else
        IsEditor = false;
#endif
    }

    public override void LateInit()
    {
        base.LateInit();
        if (mStarted)
            return;

        JoyObj.SetActive(true);

        if (parameters != null && parameters.Length > 0)
            this.transform.localPosition = (Vector3)parameters[0];

        DummyTf = new GameObject("Dummy").transform;
        DummyTf.parent = transform;
        ResetJoyPos = JoyObj.transform.localPosition;
        BoxSize = collider.GetComponent<BoxCollider>().size;
    }

    //void Start()
    //{
    //    //if (CurrentCam == null)
    //    //    CurrentCam = UIMgr.instance.UICamera.camera;
    //}

    void OnPress(bool isPress)
    {
        if (!IsActive)
            return;

        if (IsPress && isPress)
            return;

        if (CurrentCam == null)
            CurrentCam = UIMgr.instance.UICamera.camera;

        if( TownState.TownActive && isPress && uiMgr.IsActiveTutorial)//튜토리얼중 이상 행위를 했음
        {
            UIBasePanel tutoPop = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
            if (tutoPop != null && (tutoPop as TutorialPopup).TutoSupport.IsOnClick)
            {
                //TutorialType nextType = SceneManager.instance.CurTutorial + 1;
                //if (nextType == TutorialType.COSTUME)
                //    nextType += 1;

                TutorialType nextType = SceneManager.instance.NextTutorial();//다음으로 넘김

                //if (tutoPop != null)// && (tutoPop as TutorialPopup).TutoSupport.IsStayPanel
                (tutoPop as TutorialPopup).StartNextTutorial(nextType);
            }
        }

        if (!IsEditor)
        {
            if (1 < Input.touchCount && IsPress && !isPress)//조이스틱을 움직이던 손가락이 나간건지 확인
            {
                int length = Input.touches.Length;
                for (int i = 0; i < length; i++)
                {
                    Touch t = Input.touches[i];
                    if (CurTouch.fingerId != t.fingerId)
                        continue;

                    if (t.phase != TouchPhase.Canceled)//터치를 한 손가락을 땐거라면 그대로 진행
                        return;//아니라면 무시.
                    break;
                }
            }
        }

        IsPress = isPress;
        //JoyObj.SetActive(IsPress);

        if (isPress)//시작
        {
            if (!IsEditor)
            {
                if (0 == CurTouch.tapCount)
                {
                    int touchCount = -1;
                    //if (1 < Input.touchCount)//2개 이상이다. 터치 영역확인해서 잡는다
                    {
                        //600, 350
                        float boxX = BoxSize.x / 2;
                        float boxY = BoxSize.y / 2;
                        int length = Input.touches.Length;
                        for (int i = 0; i < length; i++)
                        {
                            Touch t = Input.touches[i];

                            Ray r = CurrentCam.ScreenPointToRay(t.position);//UICamera.lastTouchPosition);
                            Vector3 touchPos = r.GetPoint(0);

                            DummyTf.position = touchPos;
                            Vector3 pos = DummyTf.localPosition;

                            if (pos.x < boxX && -boxX < pos.x
                              && pos.y < boxY && -boxY < pos.y)//영역안에 있는 손가락 찾음
                            {
                                touchCount = i;
                                break;
                            }
                        }
                    }

                    if (touchCount == -1)
                    {
                        IsPress = false;
                        return;
                    }

                    CurTouch = Input.GetTouch(touchCount);
                }
                else//이미 손가락 있다면 무시.
                    return;
            }
            else
            {

            }

            if (OnStartJoy != null)
                OnStartJoy(this);
        }
        else
        {
            CurTouch = new Touch();

            BolPosition = Vector3.zero;
            joyBol.localPosition = Vector3.zero;
            
            JoyObj.transform.localPosition = ResetJoyPos;// StartJoyPos;
            if (OnEndJoy != null)
                OnEndJoy(this);
        }
    }
    /*//기획변경으로 주석.(his, 2017.08.29)
    void OnClick()
    {
        //클릭일 경우에만 검사한다.
        if ( !TownState.TownActive)//마을일 경우 Npc클릭이 있으므로 한번 검사한다// isPress && 
            return;
        
        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(rayPoint, out hit))
        {
            InputTownModel inputModel = hit.collider.gameObject.GetComponent<InputTownModel>();
            if (inputModel != null)
            {
                inputModel.NpcClickEvent();
            }
            else
            {
                CollisionTownModel townModel = hit.collider.gameObject.GetComponent<CollisionTownModel>();
                if (townModel != null)
                    townModel.OnTriggerEvent();
            }
        }

        //return false;
    }
    */
    public void SetCallback(CallBack start, CallBack move, CallBack end)
    {
        OnStartJoy = start;
        OnMoveJoy = move;
        OnEndJoy = end;
    }

    public float GetAngle()
    {
        float angle = Mathf.Atan2(BolPosition.x, BolPosition.y) * (180 / Mathf.PI);
        return angle;// * Mathf.Rad2Deg;
    }

    public Vector2 Normalize
    {
        get {
            Vector2 nor = BolPosition.normalized;
            return nor;
        }

    }

    public void SetJoyActive(bool isActive)
    {
        BolPosition = Vector3.zero;
        joyBol.localPosition = Vector3.zero;

        if (isActive)
            Show();
        else
            Hide();

        //gameObject.SetActive(isActive);
        IsActive = isActive;
        if( !IsActive)
        {
            //OnPress(false);
            if (OnEndJoy != null)
                OnEndJoy(this);

            JoyObj.transform.localPosition = ResetJoyPos;
            //JoyObj.SetActive(false);
            IsPress = false;
        }
        else
        {
        }
    }
    
    public bool IsMovingJoy()
    {
        float dis = Vector3.Distance(Vector3.zero, BolPosition);
        if (MoveDistance < dis)
            return true;

        return false;
    }

    void Update()
    {
        if (!IsPress || !IsActive)//시작이 맞는지
        {
            if (!IsPress)
            {

            }

            return;
        }

        Vector2 touchPos = Vector2.zero;

        if (!IsEditor)
        { 
            if (0 == Input.touchCount)
            {
                OnPress(false);
                return;
            }

            int touchCount = -1;
            int length = Input.touches.Length;
            for (int i = 0; i < length; i++)
            {
                Touch t = Input.touches[i];
                if (CurTouch.fingerId == t.fingerId)
                {
                    touchCount = i;
                    break;
                }
            }

            if (touchCount == -1)
            {
                OnPress(false);
                return;
            }

            CurTouch = Input.GetTouch(touchCount);
            touchPos = CurrentCam.ScreenToWorldPoint(CurTouch.position);
        }
        else
            touchPos = CurrentCam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 padPos = JoyObj.transform.position;
        Vector2 trPos = transform.position, movePos;
        float dis = Vector2.Distance(touchPos, trPos);
        if (PadRadius < dis)
            movePos = trPos + (touchPos - trPos).normalized * PadRadius;//pos + 
        else
            movePos = trPos + (touchPos - trPos).normalized * dis;

        if (Radius < Vector2.Distance(touchPos, padPos))
        {
            Vector2 pos = (Vector2)joyBol.position - (touchPos - padPos).normalized * Radius;
            JoyObj.transform.position = Vector2.Lerp(padPos, pos, Time.deltaTime * LerpSpeed);
        }

        joyBol.position = movePos;
        BolPosition = joyBol.localPosition;

        if (IsMovingJoy() )
        {
            if (OnMoveJoy != null)
                OnMoveJoy(this);
        }
        else
        {
            if (OnEndJoy != null)
                OnEndJoy(this);
        }
    }


    public float PadRadius = 1;


    public override void Close()
    {
        base.Close();
    }
}
