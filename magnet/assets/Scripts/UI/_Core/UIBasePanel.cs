using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PrevReturnType
{
    Not,//따로 처리 안한다는 의미
    Hide,//UI숨김
    None,//UI숨김
    Close,//UI닫기(삭제)
    Quit,//종료(마을, 로그인에서 사용)
    //HideAndNotShow,//??????뭔지모름 원래있었다.
}

/// 게임상에 표시되는 모든 UI창은 이 클래스를 상속받아서 작성하도록 한다.
/// UIManager에 의해서 모든 창이 관리되도록함.
/// </remarks>

[RequireComponent(typeof(UiOpenEvent))]
public class UIBasePanel : MonoBehaviour
{
    public enum TopMenuT
    {
        None,//TopMenu가 없음
        Default,//Gold, Ruby
        FoodType,//Food, Gold, Ruby
    }
    [Header("-UIBasePanel Link-")]
    public TopMenuT TopMenuType = TopMenuT.None;

    public bool IsPopup = false;//이거 체크되어있으면 해당 유아이는 SecondUI에서 찾지 못함

    /// <summary>
    /// UIMgr.instance.Prev() 함수에서 처리할 타입(빽키일경우)
    /// Prefab에서 설정만 해서 사용하거나, public override PrevReturnType Preve()로 따로 처리하면 된다.
    /// 이 타입에 맞게 Close, Hide함수 사용한다.
    /// </summary>
    public PrevReturnType ReturnType = PrevReturnType.Close;

    public enum eUIEventType
    {
        All,
        Open,
        End,
        Not
    }
    public static UIBasePanel current;

    public eUIEventType UiEventType = eUIEventType.All;

    /// <summary> UIManager에 의해 관리되는지 여부. (ShowingPanel에서 찾을 수 있는지 여부) </summary>
    public bool Managed = true;

    //< 클리어 되지않는 패널(씬 이동시 삭제되지않는패널)
    public bool NotClear = false;

    /// <summary> 해상도에 따라 스케일링 되야되는 UI인지 확인 </summary>
    public bool ResolutionScale = true;

    /// <summary> UI가 소속될 타입 </summary>
    public UIMgr.UIType UIType = UIMgr.UIType.Default;

    /// <summary> 관리자에 추가될때, 보여질 타이밍 옵션 </summary>
    public UIMgr.ShowOption ShowOption = UIMgr.ShowOption.Immediately;

    /// <summary> 현재 Panel이 사용하는 Z영역 두께를 설정한다. UI상에 띄우는 3D오브젝트의 깊이값 때문에 필요함. </summary>
    [Range(0, 2000)]
    public int ZThickness = 0;

    /// <summary> Show 호출시 발생될 리스너 </summary>
    public List<EventDelegate> onShow = new List<EventDelegate>();

    /// <summary> Hide 호출시 발생될 리스너 </summary>
    public List<EventDelegate> onHide = new List<EventDelegate>();

    /// <summary> 창 닫힐때 발생될 리스너 </summary>
    public List<EventDelegate> onClose = new List<EventDelegate>();

    public List<GameObject> RotationBoxList;
    public List<GameObject> RotationTargetList;

    public List<TutorialSupport> TutoSupportList=null;

    protected object[] parameters;
    public bool mStarted = false;
    public float RotateValue = 50f;
    public UIMgr uiMgr;//자식에서 사용 가능하게 public으로 변경.

    public int BlurCamDepth;// 0이 아니면 블러 사용
    public GameObject BlurObject;

    private bool _IsHidePanel;
    public bool IsHidePanel {
        get {
            return _IsHidePanel;
        }

    }

    #region :::: MonoBehaviour Functions ::::

    [HideInInspector]
    public UiOpenEvent _UiOpenEvent;
    void Awake()
    {
        uiMgr = UIMgr.instance;
        if (Managed)
            uiMgr.AddUI(this);
        
        Init();
        if (UiEventType != eUIEventType.Not && _UiOpenEvent == null)
        {
            _UiOpenEvent = GetComponent<UiOpenEvent>();
            if(_UiOpenEvent == null)
                _UiOpenEvent = gameObject.AddComponent<UiOpenEvent>();
        }
    }

    //< 패널의 로드가 끝났을시 true
    public bool LateInitLode = false;
    void Start()
    {
        //if (!UIMgr.instance.atlasSetlist.Contains( gameObject.name ))
        //    AtlasSet();
        // 해상도에 맞게 패널 스케일링
        if (ResolutionScale)
        {
            int Width = 1280;
            int Height = 720;

            float originalRatio = (float)Width / (float)Height;
            float curRatio = (float)Screen.width / (float)Screen.height;

            transform.localScale = new Vector3((float)curRatio / (float)originalRatio, (float)curRatio / (float)originalRatio, (float)curRatio / (float)originalRatio);
        }
        
        LateInit();
        mStarted = true;

        SetBlur();

        ///별도의 정의없이 생성할때 조건 검사후 로테이션 기능 추가해준다.
        if (0 < RotationBoxList.Count && 0 < RotationTargetList.Count && RotationBoxList.Count == RotationTargetList.Count)
        {
            SettingRotationBox();
        }

        if (_UiOpenEvent != null)
            _UiOpenEvent.InitOpenEvent();

        if (UiEventType == eUIEventType.All || UiEventType == eUIEventType.Open)
        {
            if (_UiOpenEvent != null)// && this.gameObject.activeSelf
            {
				_UiOpenEvent.SetEvent(true, ()=>{
					UIOpenEventCallback();
				});
            }
        }
        else
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (_UiOpenEvent != null)//&& this.gameObject.activeSelf
            {
                _UiOpenEvent.SetReset();
            }

        }
    }
    
    public virtual void OnDestroy()
    {

    }

    #endregion

    /// <summary>
    /// 필요한 컴포넌트 연결 및 초기화 (Awake)
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Update전 초기화로, 실제 Panel에 필요한 데이터 연동 (Start)
    /// -- 좋은 이름으로 추천바람 --
    /// </summary>
    public virtual void LateInit()
    {
        if (TopMenuType != TopMenuT.None)
        {
            uiMgr.OpenTopMenu(this);
        }

        LateInitLode = true;

        //UIDrawCall.ReleaseInactive ();
        //Resources.UnloadUnusedAssets ();
    }

    /// <summary>
    /// 패널이 보여지기전 데이터 연동에 필요한 인자값들을 받아둔다.
    /// 패널이 생성되자마자 설정됐다면, LateInit()에서 사용가능하다.
    /// </summary>
    /// <param name="args">인자 리스트</param>
    public void SetParams(params object[] args)
    {
        parameters = args;
    }

    public object[] GetParams()
    {
        return parameters;
    }

    /// <summary>
    /// 감추어 있는 UI를 보이도록 하고, 재갱신에 필요한 값을 받아 다시 셋팅하도록 한다.
    /// </summary>
    /// <param name="args">인자 리스트</param>
    public void Show(params object[] args)
    {
        SetParams(args);
        _IsHidePanel = false;

        if (UiEventType == eUIEventType.All || UiEventType == eUIEventType.Open)
        {
            if (_UiOpenEvent != null)// && this.gameObject.activeSelf
            {
                _UiOpenEvent.SetEvent(true, ()=>{
					UIOpenEventCallback();
				});
            }
        }
        else
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (_UiOpenEvent != null)//&& this.gameObject.activeSelf
            {
                _UiOpenEvent.SetReset();
            }

        }
        
        if (null == current)
        {
            current = this;
            EventDelegate.Execute(onShow);
            current = null;
        }

        // 매니저에서 처리해야될 일
        if (Managed)
            uiMgr.OnShowPanel(this);//SendMessage("OnShowPanel", this);

        // 중요! LateInit은 Start()에서 호출되기 때문에, Start()전에 미리 호출되면 안됨.
        if (mStarted)
        {
            LateInit();
        }

    }

	public virtual void UIOpenEventCallback(){

	}

    /// <summary>
    /// 현재 Panel을 감추도록 한다. (파괴하지는 않음)
    /// </summary>
    public virtual void Hide()
    {
        ObjectHide();
    }

    /// <summary> 게임 오브젝트를 끄는 이벤트 단순 하이드.(Hide를 쓰면 되지만 상속받은 곳에서 처리하는것들을 못하게 하려고 추가함.) </summary>
    public virtual void ObjectHide()
    {
        _IsHidePanel = true;
        if (TopMenuType != TopMenuT.None)
        {
            if (Managed)
                uiMgr.HideTopMenu();
        }

        if (UiEventType == eUIEventType.All || UiEventType == eUIEventType.End)
        {
            if (_UiOpenEvent != null && this.gameObject.activeSelf)
                _UiOpenEvent.SetEvent(false, () =>
                {
                    HideEvent();
                });
            else
                HideEvent();
        }
        else
            HideEvent();
    }

    public virtual void HideEvent()
    {
        gameObject.SetActive(false);

        if (null == current)
        {
            current = this;
            if (Managed)
                uiMgr.OnHidePanel();

            EventDelegate.Execute(onHide);
            current = null;
        }
        else if (Managed)
            uiMgr.OnHidePanel();
    }

    /// <summary>
    /// 현재 Panel을 제거하도록 한다.
    /// </summary>
    public virtual void Close()
    {
        _IsHidePanel = true;//꺼질떄도 체크해놓는다.
        if (TopMenuType != TopMenuT.None)
        {
            if (Managed)
                uiMgr.HideTopMenu();
        }

        if (UiEventType == eUIEventType.All || UiEventType == eUIEventType.End)
        {
            if (_UiOpenEvent != null && this.gameObject.activeSelf)
                _UiOpenEvent.SetEvent(false, () =>
                {
                    CloseEvent();
                });
            else
                CloseEvent();
        }
        else
            CloseEvent();


    }

    public virtual void CloseEvent()
    {
        if (null == current)
        {
            current = this;
            if (Managed)
                uiMgr.OnClosePanel();

            EventDelegate.Execute(onClose);
            current = null;
        }
        else if (Managed)
            uiMgr.OnClosePanel();

        gameObject.SetActive(false);
        Destroy(gameObject);

    }

    /// <summary>
    /// 캐릭터 모델링을 돌려주게 하는 함수 정의하는 곳
    /// </summary>
    public virtual void SettingRotationBox()
    {
        for (int i = 0; i < RotationBoxList.Count; i++)
        {
            if (RotationBoxList[i] == null)
                continue;

            int array = i;
            UIEventListener.Get(RotationBoxList[array]).onDrag = (go, delta) =>
            {
                for (int j = 0; j < RotationBoxList.Count; j++)
                {
                    if (RotationBoxList[array] != RotationBoxList[j])
                        continue;

                    RotationTargetList[j].transform.Rotate(-(Vector3.up * ((delta.x * Time.deltaTime) * RotateValue)));
                }
            };
        }
    }


    public virtual PrevReturnType Prev()
    {
        return ReturnType;
    }

    /// <summary> ChapterPopup에서 호출해준다. 재정의해서 게임시작에 필요한거 정의하자. </summary>
    public virtual void GotoInGame()
    {
    }

    /// <summary> 갱신용.(ReadyPopup에서 사용) </summary>
    public virtual void OnCloseReadyPopup()
    {
        if (TopMenuType != TopMenuT.None)
            uiMgr.OpenTopMenu(this);
    }

    public virtual void SetBlur()
    {
        if (BlurCamDepth == 0)
            return;

        BlurObject = UIHelper.CreateBlurPanel(transform);
        BlurObject.SetActive(false);
        BlurObject.transform.FindChild("Camera").GetComponent<Camera>().depth = BlurCamDepth;
        BlurObject.SetActive(true);
    }

    public virtual int GetUILayer()
    {
        return gameObject.layer;/* LayerMask.NameToLayer("UILayer");*/
    }

    public virtual bool Quit()
    {
        return true;
    }

    /*//보류!
    public virtual void CheckOpenTutorial(OpenTutorialType type)
    {
        if (!uiMgr.GetOpenTuto(type))
            return;

        Quest.TutorialInfo tutoInfo = _LowDataMgr.instance.GetLowDataFirstTutorial((int)type);
        if (tutoInfo == null || NetData.instance.UserLevel < tutoInfo.OpenLevel)//아직 입장 불가능 튜토리얼
            return;

        UIMgr.OpenTutorialPopup((int)TutorialType.OPEN_UI_TYPE, (int)type);
    }
    */
    /// <summary> 네트워크 통신 응답 처리 </summary>
    public virtual void NetworkData(params object[] proto)
    {

    }
    
    /// <summary> 탭 타입(인벤토리, 파트너, 코스튬) </summary>
    public virtual byte TabType
    {
        get {
            return 0;
        }
    }

    public virtual bool OnSubTutorial() {
        if (SceneManager.instance.CurTutorial == TutorialType.ALL_CLEAR)
            return false;
        
        if (TutoSupportList == null || TutoSupportList.Count <= 0 )
        {
            TutoSupportList = UIHelper.FindComponents<TutorialSupport>(transform);

            for (int i = 0; i < TutoSupportList.Count; i++)
            {
                if (TutoSupportList[i].NextTuto != null)
                    continue;

                for (int j = 0; j < TutoSupportList.Count; j++)
                {
                    if (TutoSupportList[j].TutoType != TutoSupportList[i].TutoType)
                        continue;
                    else if (TutoSupportList[j].SortId != TutoSupportList[i].SortId + 1)
                        continue;

                    TutoSupportList[i].NextTuto = TutoSupportList[j];
                    break;
                }
            }
        }

        if (TutoSupportList.Count <= 0)
            return false;

        int tutoType = 1;
        int arr = 0;
        while(tutoType < (int)TutorialType.MAX)
        {
            if (TutoSupportList.Count <= arr)
            {
                arr = 0;
                ++tutoType;
            }
            TutorialSupport support = TutoSupportList[arr++];

            if (support.TutoType != SceneManager.instance.CurTutorial)
            {
                if( !support.ChangeTutoType() || support.TutoType != SceneManager.instance.CurTutorial)
                    continue;
            }
            Quest.MainTutorialInfo tutoLowData = _LowDataMgr.instance.GetLowDataFirstMainTutorial((uint)support.TutoType, 1);
            if (tutoLowData == null || NetData.instance.UserLevel < tutoLowData.OpenLevel )
                continue;

            if (support.OnTutoSupportStart())
                return true;
        }
        return false;
    }

    #region :::: Utils :::: 사용안하므로 주석되어 있음
    /*
    public T Find<T> ( string name ) where T : UnityEngine.Component
	{
		Transform obj = transform.FindChild ( name );
		if( obj != null )
			return obj.GetComponent<T>();

#if DEBUG
        if (null == obj)
        {
            Debug.LogWarning( "can't found " + name );
        }
#endif
		
		return null;
	}

    public static T Find<T> ( Transform transform, string name ) where T : UnityEngine.Component
	{
		Transform obj = transform.FindChild ( name );
		if( obj != null )
			return obj.GetComponent<T>();

#if DEBUG
        if (null == obj)
        {
            //Debug.LogWarning( "can't found " + name );
        }
#endif
		
		return null;
	}

    public static GameObject Find(Transform transform, string name)
    {
        Transform obj = transform.FindChild(name);
        if (obj != null)
            return obj.gameObject;

#if DEBUG
        if (null == obj)
        {
            //Debug.LogWarning( "can't found " + name );
        }
#endif

        return null;
    }

    /// <summary>
    /// 해당 Transform 객체의 모든 T타입 객체들을 반환해준다.
    /// </summary>
    public static T[] FindChildren<T> ( Transform transform ) where T : UnityEngine.Component
	{
		return transform.GetComponentsInChildren<T>( true );
	}

    /// <summary>
    /// 대상 Transform의 객체에서 name에 해당하는 객체의 자식들중 T타입 모두를 찾아준다.
    /// </summary>
    public static T[] FindChildren<T> ( Transform transform, string name ) where T : UnityEngine.Component
	{
		Transform obj = transform.Find ( name );
		if (obj != null)
			return FindChildren<T> (obj);

#if DEBUG
        if (null == obj)
        {
            Debug.LogWarning( "can't found " + name );
        }
#endif
		
		return null;
    }
    */
    #endregion
}
