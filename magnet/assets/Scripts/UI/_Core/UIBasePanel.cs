using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PrevReturnType
{
    Not,//���� ó�� ���Ѵٴ� �ǹ�
    Hide,//UI����
    None,//UI����
    Close,//UI�ݱ�(����)
    Quit,//����(����, �α��ο��� ���)
    //HideAndNotShow,//??????������ �����־���.
}

/// ���ӻ� ǥ�õǴ� ��� UIâ�� �� Ŭ������ ��ӹ޾Ƽ� �ۼ��ϵ��� �Ѵ�.
/// UIManager�� ���ؼ� ��� â�� �����ǵ�����.
/// </remarks>

[RequireComponent(typeof(UiOpenEvent))]
public class UIBasePanel : MonoBehaviour
{
    public enum TopMenuT
    {
        None,//TopMenu�� ����
        Default,//Gold, Ruby
        FoodType,//Food, Gold, Ruby
    }
    [Header("-UIBasePanel Link-")]
    public TopMenuT TopMenuType = TopMenuT.None;

    public bool IsPopup = false;//�̰� üũ�Ǿ������� �ش� �����̴� SecondUI���� ã�� ����

    /// <summary>
    /// UIMgr.instance.Prev() �Լ����� ó���� Ÿ��(��Ű�ϰ��)
    /// Prefab���� ������ �ؼ� ����ϰų�, public override PrevReturnType Preve()�� ���� ó���ϸ� �ȴ�.
    /// �� Ÿ�Կ� �°� Close, Hide�Լ� ����Ѵ�.
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

    /// <summary> UIManager�� ���� �����Ǵ��� ����. (ShowingPanel���� ã�� �� �ִ��� ����) </summary>
    public bool Managed = true;

    //< Ŭ���� �����ʴ� �г�(�� �̵��� ���������ʴ��г�)
    public bool NotClear = false;

    /// <summary> �ػ󵵿� ���� �����ϸ� �ǾߵǴ� UI���� Ȯ�� </summary>
    public bool ResolutionScale = true;

    /// <summary> UI�� �Ҽӵ� Ÿ�� </summary>
    public UIMgr.UIType UIType = UIMgr.UIType.Default;

    /// <summary> �����ڿ� �߰��ɶ�, ������ Ÿ�̹� �ɼ� </summary>
    public UIMgr.ShowOption ShowOption = UIMgr.ShowOption.Immediately;

    /// <summary> ���� Panel�� ����ϴ� Z���� �β��� �����Ѵ�. UI�� ���� 3D������Ʈ�� ���̰� ������ �ʿ���. </summary>
    [Range(0, 2000)]
    public int ZThickness = 0;

    /// <summary> Show ȣ��� �߻��� ������ </summary>
    public List<EventDelegate> onShow = new List<EventDelegate>();

    /// <summary> Hide ȣ��� �߻��� ������ </summary>
    public List<EventDelegate> onHide = new List<EventDelegate>();

    /// <summary> â ������ �߻��� ������ </summary>
    public List<EventDelegate> onClose = new List<EventDelegate>();

    public List<GameObject> RotationBoxList;
    public List<GameObject> RotationTargetList;

    public List<TutorialSupport> TutoSupportList=null;

    protected object[] parameters;
    public bool mStarted = false;
    public float RotateValue = 50f;
    public UIMgr uiMgr;//�ڽĿ��� ��� �����ϰ� public���� ����.

    public int BlurCamDepth;// 0�� �ƴϸ� �� ���
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

    //< �г��� �ε尡 �������� true
    public bool LateInitLode = false;
    void Start()
    {
        //if (!UIMgr.instance.atlasSetlist.Contains( gameObject.name ))
        //    AtlasSet();
        // �ػ󵵿� �°� �г� �����ϸ�
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

        ///������ ���Ǿ��� �����Ҷ� ���� �˻��� �����̼� ��� �߰����ش�.
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
    /// �ʿ��� ������Ʈ ���� �� �ʱ�ȭ (Awake)
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Update�� �ʱ�ȭ��, ���� Panel�� �ʿ��� ������ ���� (Start)
    /// -- ���� �̸����� ��õ�ٶ� --
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
    /// �г��� ���������� ������ ������ �ʿ��� ���ڰ����� �޾Ƶд�.
    /// �г��� �������ڸ��� �����ƴٸ�, LateInit()���� ��밡���ϴ�.
    /// </summary>
    /// <param name="args">���� ����Ʈ</param>
    public void SetParams(params object[] args)
    {
        parameters = args;
    }

    public object[] GetParams()
    {
        return parameters;
    }

    /// <summary>
    /// ���߾� �ִ� UI�� ���̵��� �ϰ�, �簻�ſ� �ʿ��� ���� �޾� �ٽ� �����ϵ��� �Ѵ�.
    /// </summary>
    /// <param name="args">���� ����Ʈ</param>
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

        // �Ŵ������� ó���ؾߵ� ��
        if (Managed)
            uiMgr.OnShowPanel(this);//SendMessage("OnShowPanel", this);

        // �߿�! LateInit�� Start()���� ȣ��Ǳ� ������, Start()���� �̸� ȣ��Ǹ� �ȵ�.
        if (mStarted)
        {
            LateInit();
        }

    }

	public virtual void UIOpenEventCallback(){

	}

    /// <summary>
    /// ���� Panel�� ���ߵ��� �Ѵ�. (�ı������� ����)
    /// </summary>
    public virtual void Hide()
    {
        ObjectHide();
    }

    /// <summary> ���� ������Ʈ�� ���� �̺�Ʈ �ܼ� ���̵�.(Hide�� ���� ������ ��ӹ��� ������ ó���ϴ°͵��� ���ϰ� �Ϸ��� �߰���.) </summary>
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
    /// ���� Panel�� �����ϵ��� �Ѵ�.
    /// </summary>
    public virtual void Close()
    {
        _IsHidePanel = true;//�������� üũ�س��´�.
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
    /// ĳ���� �𵨸��� �����ְ� �ϴ� �Լ� �����ϴ� ��
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

    /// <summary> ChapterPopup���� ȣ�����ش�. �������ؼ� ���ӽ��ۿ� �ʿ��Ѱ� ��������. </summary>
    public virtual void GotoInGame()
    {
    }

    /// <summary> ���ſ�.(ReadyPopup���� ���) </summary>
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

    /*//����!
    public virtual void CheckOpenTutorial(OpenTutorialType type)
    {
        if (!uiMgr.GetOpenTuto(type))
            return;

        Quest.TutorialInfo tutoInfo = _LowDataMgr.instance.GetLowDataFirstTutorial((int)type);
        if (tutoInfo == null || NetData.instance.UserLevel < tutoInfo.OpenLevel)//���� ���� �Ұ��� Ʃ�丮��
            return;

        UIMgr.OpenTutorialPopup((int)TutorialType.OPEN_UI_TYPE, (int)type);
    }
    */
    /// <summary> ��Ʈ��ũ ��� ���� ó�� </summary>
    public virtual void NetworkData(params object[] proto)
    {

    }
    
    /// <summary> �� Ÿ��(�κ��丮, ��Ʈ��, �ڽ�Ƭ) </summary>
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

    #region :::: Utils :::: �����ϹǷ� �ּ��Ǿ� ����
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
    /// �ش� Transform ��ü�� ��� TŸ�� ��ü���� ��ȯ���ش�.
    /// </summary>
    public static T[] FindChildren<T> ( Transform transform ) where T : UnityEngine.Component
	{
		return transform.GetComponentsInChildren<T>( true );
	}

    /// <summary>
    /// ��� Transform�� ��ü���� name�� �ش��ϴ� ��ü�� �ڽĵ��� TŸ�� ��θ� ã���ش�.
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
