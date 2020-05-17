using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UIBasePanel 및 UIRoot내에 존재하는 Panel들을 관리하는 클래스
/// </summary>
public class UIMgr : MonoSingleton<UIMgr>//Immortal<UIMgr>
{
    public enum UIType
    {
        System  = 1,
        Default = 2,
        /// <summary>
        /// 특정위치가 아닌 UIRoot/UICamera 에 소속되도록함.
        /// </summary>
        Free = 4,
    };

    public enum ShowOption
    {
        Next = 0,
        Immediately,

        /// <summary>
        /// 보여지는 순서 무시 (Show/Hide 할때도.)
        /// </summary>
        Ignore,
    }

    private GameObject _uiRootGO;
    private GameObject _uiCameraGO;

    public List<string> atlasSetlist = new List<string>();

    public UIRoot UIRoot
    {
        get { return _uiRootGO.GetComponent<UIRoot>(); }
    }

    public UICamera UICamera 
    {
        get { return _uiCameraGO.GetComponent<UICamera>(); }
    }
    
    /// <summary>
    /// 현재 View에 보여지고 있는 UIBasePanel 리스트
    /// </summary>
    [SerializeField]
    private List<UIBasePanel>   ShowingPanels;
    
    GameObject uiDefaultObj;
    GameObject uiSystemObj;
    //GameObject uiFreeObj;
    //GameObject UILight;
    
    private Light ShadowLight;

    public TopMenuObject TopMenu;
    
    public bool IsShop;//가라용
    public bool IsActiveTutorial;
    public bool IsNextUiAchieve;//다음 유아이가 업적인지

    public int PrevAttack;  //전투력 상승 이펙트시 사용할 이전의 전투력수치
    //public bool AchieveMentClearFlag;
    //public uint AchieveType = 1;
    //public uint AchieveSubType = 1;
    //public uint AchieveLv = 1;

    private string _ReturnPanel;
    //public List<int> AlramList = new List<int>();
    //public List<int> BenefitAlram = new List<int>();

    //public uint[] boxFreeTime = new uint[2];
    //public List<NetData._ItemData> NewItemList = new List<NetData._ItemData>();  //새로등록된아이템리스트들
    //public UIBasePanel PrevPanel = null;   //업적팝업 이전의 패널

    public int ComebackDay = 0;

    private SceneManager SceneUIMgr;

    public override void OnInitialize()
    {
        base.OnInitialize();

        SceneUIMgr = SceneManager.instance;

        CheckUIRoot();

        //Transform lightTf = _uiRootGO.transform.FindChild("UI_Light");
        //if (lightTf != null)
        //    UILight = lightTf.gameObject;

        
        ShowingPanels = new List<UIBasePanel>();

        //ReconnectRoot();
        //AchieveMentClearFlag = false;

        if (SceneUIMgr == null)
            return;

        List<UIBasePanel> list = SceneUIMgr.GetSavePanel();
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                UIBasePanel basePanel = list[i];
                if (basePanel == null)
                    continue;

                basePanel.uiMgr = this;
                AddUI(basePanel);
            }

            list.Clear();
        }
    }

    #region :::: Public Functions ::::

    /// <summary>
    /// UIOpen 전용 함수 for UIBasePanel 
    /// 해당UI가 없으면 생성하고, 있으면 Show해주도록 한다.
    /// </summary>
    /// <param name="uiPath">생성하려는 UI의 경로 (UI의 고유이름으로 등록됨) ["UI/" + uiPath]</param>
    static public GameObject Open(string uiPath, params object[] args)
    {
        instance.CheckUIRoot();
     
        UIBasePanel panel = UIMgr.instance.FindInShowing(uiPath);
        if (null != panel)
        {
            // 숨겨져있다면 보여지도록 하고, 보여지는 상태에서 호출되면 재갱신이라고 보면됨.
            panel.Show(args);
        }
        else
        {
            GameObject newPanelGO = ResourceMgr.InstantiateUI(uiPath);
            if (null != newPanelGO)
            {
                panel = newPanelGO.GetComponent<UIBasePanel>();
                if (null == panel)
                    Debug.LogError("UIBasePanel이 존재하지 않는 UI객체입니다.");
                
                panel.gameObject.SetActive(true);
                panel.name = uiPath;
                panel.SetParams(args);
            }
        }

        if (panel is MissionPanel || panel is ResultRewardStarPanel)
            instance.SetShadowLight(false);

        //if (!uiPath.Contains("SystemPopup")  && 0 == instance.SceneUIMgr.CurPopupId)//현재 팝업이 켜져있는것
        //{
        //    UIBasePanel systemPanel = GetUIBasePanel("UIPopup/SystemPopup");
        //    if (systemPanel != null)
        //        (systemPanel as SystemPopup).OnEnd();
        //    else
        //        instance.SceneUIMgr.CurPopupId = -1;
        //}
        
        return panel.gameObject;
    }

    ///<summary>
    ///해당 UI의 오브젝트를 리턴
    ///</summary>>
    static public GameObject GetUI(string uiPath)
    {
        if (UIMgr.instance == null)
            return null;

        UIBasePanel panel = UIMgr.instance.FindInShowing(uiPath);

        if (panel != null)
        {
            return panel.gameObject;
        }
        else
        {
            //Debug.LogWarning(uiPath + "오브젝트를 찾을 수 없습니다.");
            return null;
        }
    }

    /// <summary>
    /// 해당 UI의 UIBasePanel 컴포넌트를 리턴
    /// </summary>
    static public UIBasePanel GetUIBasePanel(string uiPath, bool checkHideObj=true)
    {
	   GameObject go = GetUI(uiPath);
	   if (go == null) return null;

        UIBasePanel basePanel = go.GetComponent<UIBasePanel>();
        if (basePanel == null || (checkHideObj && basePanel.IsHidePanel))//꺼져있는 객체는 Null로 인식시켜버린다.
            return null;

        return basePanel;
    }

    public static void ClosePanel(string uiPath)
    {
        new Task(_ClosePanel(uiPath));
    }

    static IEnumerator _ClosePanel(string uiPath)
    {
        //< 나중에 오픈될수도있기때문에 몇프레임 뒤에 꺼준다.
        yield return null;

        GameObject panel = GetUI(uiPath);
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary> 타운페널을 넘겨줌. </summary>
    static public UIBasePanel GetTownBasePanel()
    {
        UIBasePanel panel = GetUIBasePanel("UIPanel/TownPanel", false);//GetUI("UIPanel/TownPanel");
        return panel;
    }

    /// <summary> 인게임 페널을 넘겨줌. </summary>
    static public UIBasePanel GetHUDBasePanel()
    {
        UIBasePanel panel = GetUIBasePanel("UIPanel/InGameHUDPanel", false);//GetUI("UIPanel/TownPanel");
        return panel;
    }


	static public void setHudPanelVisible(bool b){
		
		UIBasePanel hudPanel = GetHUDBasePanel();
		if (hudPanel != null) {
			if (b){
				hudPanel.Show();
			}
			else{
				hudPanel.Hide();
			}
		}
	}

	static public void setMapPanelVisible(bool b){
		GameObject p = GameObject.Find ("UIPanel/MapPanel");
		if (p != null) {
			if (b){
				p.transform.localPosition = Vector3.zero;
			}
			else{
				p.transform.localPosition = new Vector3(10000,0,0);
			}
		}
	}

	static public void setChatPopupVisible(bool b){
		GameObject p = GameObject.Find ("UIPopup/ChatPopup");
		if (p != null) {
			if (b){
				p.transform.localPosition = Vector3.zero;
			}
			else{
				p.transform.localPosition = new Vector3(10000,0,0);
			}
		}
	}

	// show / hide를 하면 lateInit()이 불러지므로 단순히 보이지 않게 하기 위해 위치를 바꾸어 카메라밖으로 이동시킨다. 아울러 터치도 먹지 않게 한다. 		
	static public void setTownPanelVisible(bool b){
		UIBasePanel townPanel = GetTownBasePanel();
//		if (townPanel != null) {
//			if (b){
//				townPanel.Show();
//			}
//			else{
//				townPanel.Hide();
//			}
//		}

		if (townPanel != null) {
			if (b) {
				townPanel.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
			} else {
				townPanel.gameObject.transform.localPosition = new Vector3 (10000, 0, 0);
			}
		}
	}


	static public void setInGameBoardPanelVisible(bool b){
		GameObject InGameBoardPanelGO = GetUI("UIPanel/InGameBoardPanel");
		if (InGameBoardPanelGO != null) {
			InGameBoardPanelGO.gameObject.SetActive (b);
		}
	}
	
    /*
    /// <summary>
    /// 모든 UI OFF
    /// </summary>
    static public void UIShutDown()
    {
        List<UIBasePanel> Panels = UIMgr.instance.FindAllInShowing<UIBasePanel>();
        foreach (UIBasePanel panel in Panels)
        {
            if (panel.ShowOption == ShowOption.Ignore)
                continue;

            if (panel.Popup)
                continue;

            panel.Hide();
        }
    }
    */
    /// <summary>
    /// 안드로이드, PC에서 뒤로가기 처리함수. 
    /// 아직 좀더 만져줘야함. 주석처리 되어 있는건 기존의 것.(아직 삭제하면 않이됩니다.)
    /// 사용 방식은 PrevReturnType에 맞게 Close, Hide를 호출함. Not일 경우 ShowingPanels등록되어 있는거 삭제만함.
    /// 설정은 코드에서 Prev재정의 하거나, Prefab에서 변경해주면됨.
    /// </summary>
    public void Prev( int depth = 1 )
    {
	   if (SceneManager.instance.IsShowStaticUI )
		  return;
       
        for (int i = 0; i < depth; ++i)//depth는 무조건 1일 것이다.
        {
            int hideIdx = ShowingPanels.FindIndex(i, (basePanel) => basePanel.ShowOption != ShowOption.Ignore);
            if (hideIdx < 0)//-1 값이 나올 경우가 존제함. 예외처리.
                continue;

            UIBasePanel curPanel = ShowingPanels[hideIdx];
            if (!curPanel.gameObject.activeSelf)
            {
                if(depth == 1)
                    depth += 1;

                continue;
            }

            /*
            if (curPanel.name.Contains("TownPanel") || curPanel.name.Contains("LoginPanel") )//(EndPanel != null && EndPanel == ShowingPanels[hideIdx])
            {
                //시스템 종료 팝업			 
                AddPopup(2, Application.Quit, null, null);
	            return;
            }
            */
            //< 뒤로가기가 가능할시에만 처리
            //HideEvent, CloseEvent 함수에서 삭제 함수 호출함. 독특한 예외처리 필요치 않는 이상 신경안써도 됨.
            UIBasePanel hidePanel = ShowingPanels[hideIdx];
            switch (hidePanel.Prev())
            {
			    //< 숨기기만 해줌
                case PrevReturnType.None:
                case PrevReturnType.Hide:
                    hidePanel.Hide();
                    break;
                case PrevReturnType.Close:
                    hidePanel.Close();
                    break;
                case PrevReturnType.Quit:
                    if (hidePanel.Quit())
                    {
                        #if UNITY_EDITOR
                        {
                            AddPopup(75, 74, 117, 76, 0, ()=> {
                                UnityEditor.EditorApplication.isPlaying = false;
                            });
                        }
                        #else
                            AddPopup(75, 74, 117, 76, 0, () => {
                                Application.Quit();
                            });
                        #endif
                    }
                    
                    break;
            }
        }
    }
    /*//사용안함
    //< UI 두개의 우선순위를 변경해준다
    public void ChangeDepth(string owner, string target)
    {
        int owneridx = 0;
        int targetidx = 0;
        for(int i=0; i<ShowingPanels.Count; i++)
        {
            if (ShowingPanels[i].name == target)
                targetidx = i;

            if (ShowingPanels[i].name == owner)
                owneridx = i;
        }

        //< 변경해준다.
        UIBasePanel ownerPanel = ShowingPanels[owneridx];
        UIBasePanel targetPanel = ShowingPanels[targetidx];

        ShowingPanels[owneridx] = targetPanel;
        ShowingPanels[targetidx] = ownerPanel;
    }
    */
    //< 현재 UI중 최상단의 UI를 리턴해준다
    public UIBasePanel GetFirstUI()
    {
        if (ShowingPanels.Count <= 0)
            return null;

        return ShowingPanels[0];
    }

    //< 현재 UI중 2번째를 리턴해준다
    // 업적알림팝업에서 사용 
    public UIBasePanel GetSecondUI()
    {
        if (ShowingPanels.Count <= 0)
            return null;

        for(int i=1; i < ShowingPanels.Count; i++)
        {
            if (ShowingPanels[i].NotClear || !ShowingPanels[i].Managed || ShowingPanels[i].IsPopup)
                continue;

            return ShowingPanels[i];
        }

        return null;
    }

    //해당 타입에 맞는 녀석들 중 최상단에 있는 것을 넘겨준다.
    public UIBasePanel GetFirstUI(UIType type)
    {
        if (ShowingPanels.Count <= 0)
            return null;

        for (int i = 0; i < ShowingPanels.Count; i++)
        {
            if (ShowingPanels[i].UIType == type)
                return ShowingPanels[i];
        }

        return null;
    }

    /// <summary> 열려있는 페널 찾는다 </summary>
    public UIBasePanel GetCurPanel()
    {
        if (ShowingPanels.Count <= 0)
            return null;

        for (int i = 0; i < ShowingPanels.Count; i++)
        {
            if (!ShowingPanels[i].gameObject.activeSelf || ShowingPanels[i].UIType != UIType.Default || !ShowingPanels[i].Managed || ShowingPanels[i].IsPopup || !ShowingPanels[i].name.Contains("Panel") )
                continue;

            return ShowingPanels[i];
        }

        return null;
    }

    /// <summary>
    /// 주어진 객체를 알맞게 파괴한다. (UIBasePanel이 아니라도 파괴는 함. 단, 반환값은 false)
    /// </summary>
    /// <returns>주어진 객체가 UIBasePanel일때만 true, 아니면 false</returns>
    static public bool Close(GameObject panelGO)
    {
        if (null == panelGO)
            return false;

        if (null == panelGO.GetComponent<UIBasePanel>())
        {
            Object.Destroy(panelGO);
            return false;
        }

        panelGO.GetComponent<UIBasePanel>().Close();
        return true;
    }

    /// <summary>
    /// 해당 이름의 UIPanel이 존재하면 파괴되도록한다.
    /// </summary>
    /// <param name="uiPath">생성시 입력한 uiPath명</param>
    static public bool Close(string uiPath)
    {
        UIBasePanel panel = UIMgr.instance.FindInShowing(uiPath);
        if (null == panel)
            return false;

        panel.Close();
        return true;
    }

    /// <summary>
    /// 모든 유아이 삭제.
    /// </summary>
    static public void ClearUI(bool allClear = false)//UIType deleteType = UIType.Default)
    {
        instance.Clear( UIType.Default, allClear);
        instance.Clear( UIType.System, allClear);
	    instance.Clear( UIType.Free, allClear);
    }
    
    /// <summary>
    /// UIBasePanel의 내부 값들을 참조하여 관리되도록 매니저에 추가한다.
    /// </summary>
    public void AddUI(UIBasePanel basePanel)
    {
        AddUI(basePanel, basePanel.UIType, basePanel.ShowOption);
    }

    void AddUI(UIBasePanel basePanel, UIType _uiType, ShowOption _showOpt)
    {
        SetToProperParent(basePanel);

        // 관리되기를 원하지 않으면, 리스트에 추가는 하지 않는다.
        if (!basePanel.Managed)
            return;

        AddShowingList(basePanel);
    }

    /// <summary>
    /// 정해진 UIType에 존재하는 패널을 모두삭제한다.
    /// </summary>
    /// <param name="deleteType">삭제될 UIType들</param>
    public void Clear(UIType deleteType, bool isAll)
    {
        List<UIBasePanel> deleteList = new List<UIBasePanel>();//삭제할 리스트
        int loopCount = ShowingPanels.Count;
        for(int i=0; i < loopCount; i++)
        {
            UIBasePanel panel = ShowingPanels[i];
            if (null == panel || (panel.UIType != deleteType) )
                continue;

            if (panel.NotClear && !isAll)
            {
                //panel.Hide();
                continue;
            }
            
            deleteList.Add(panel);
        }
        
        HideTopMenu();

        if (deleteType == UIType.Free)
        {
            for(int i=0; i < deleteList.Count; i++)
            {
                deleteList[i].Hide();
                SceneManager.instance.AddSavePanel(deleteList[i]);
            }
        }
        else
        {
            while (0 < deleteList.Count)
            {
                UIBasePanel panel = deleteList[deleteList.Count - 1];
                if (ShowingPanels.Contains(panel))// 혹시 모를 예외처리.
                    ShowingPanels.Remove(panel);

                Destroy(panel.gameObject);//위는 실행 안되고 여기만 되면 문제가 생긴걸까?
                deleteList.Remove(panel);
            }

        }

        deleteList.Clear();
    }

    /// <summary>
    /// 현재 보여지고 있는 UI중에 하나를 찾는다.
    /// </summary>
    /// <typeparam name="T">UIBasePanel을 상속받은</typeparam>
    public T FindInShowing<T>() where T : MonoBehaviour
    {
        return ShowingPanels.Find( (basePanel) => basePanel is T ) as T;
    }

    public List<T> FindAllInShowing<T>() where T : MonoBehaviour
    {
        return ShowingPanels.FindAll( (basePanel) => basePanel is T).ConvertAll( (bb) => bb as T);
    }

    public UIBasePanel FindInShowing(string uiPath)
    {
        return ShowingPanels.Find( (basePanel) => basePanel.name == uiPath );
    }

    public UIBasePanel FocusedUI()
    {
        return ShowingPanels.Find( (basePanel) => basePanel.ShowOption == ShowOption.Immediately );
    }
    
#endregion

#region :::: Internal Functions ::::

    /// <summary>
    /// UIRoot가 존재하는지 검사하고, 없으면 생성하도록 한다.
    /// </summary>
    void CheckUIRoot()
    {
        if (_uiRootGO)
            return;

        // 이미 UIRoot가 다른 방식으로 존재한다면, 파괴하도록 한다.
        UIRoot alreadyUIRoot = FindObjectOfType<UIRoot>();
        if (alreadyUIRoot != null && !alreadyUIRoot.name.Contains("StaticRoot") )
            Destroy(alreadyUIRoot.gameObject);

        _uiRootGO = GameObject.Instantiate(Resources.Load("Default/UIRoot") as GameObject) as GameObject;
        _uiCameraGO = _uiRootGO.transform.Find("UICamera").gameObject;
        ShadowLight = _uiRootGO.transform.FindChild("UI_ShadowLight").GetComponent<Light>();

        AsignToRoot(ref uiSystemObj, UIType.System.ToString(), -2000);
        AsignToRoot(ref uiDefaultObj, UIType.Default.ToString(), 1000);
        //AsignToRoot(ref uiFreeObj, UIType.Free.ToString(), -3000);

        //float perx = 1280.0f / Screen.width; 
        //float pery = 720.0f / Screen.height; 
        //float v = (perx > pery) ? perx : pery; 
        //_uiCameraGO.GetComponent<Camera>().orthographicSize = v; 

        //DontDestroyOnLoad(_uiRootGO);
    }
    
    /// <summary>
    /// 각 그룹별 Parent 객체를 UIRoot에 연결하고, 고정 Z위치 설정.
    /// </summary>
    protected void AsignToRoot(ref GameObject obj, string name, float zPos)
    {
        Transform trans = _uiCameraGO.transform.FindChild(name);
        if (trans == null)
        {
            obj = new GameObject(name);
            obj.transform.localScale    = _uiRootGO.transform.lossyScale;/*_uiCameraGO.transform.lossyScale;*/
            obj.transform.parent        = _uiRootGO.transform;/*_uiCameraGO.transform;*/
            obj.transform.position      = _uiRootGO.transform.position;/*_uiCameraGO.transform.position;*/
        }
        else
            obj = trans.gameObject;

        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, zPos);
    }

    /// <summary>
    /// UIType맞는 부모 밑에 위치시켜주도록 한다.
    /// </summary>
    protected void SetToProperParent(UIBasePanel basePanel)
    {
        basePanel.transform.position = _uiRootGO.transform.position;

        switch (basePanel.UIType)
        {
            case UIType.System:
                basePanel.transform.parent = uiSystemObj.transform;
                break;
            case UIType.Default:
                basePanel.transform.parent = uiDefaultObj.transform;
                break;
            case UIType.Free:
                basePanel.transform.parent = _uiRootGO.transform;
                //basePanel.transform.parent = uiFreeObj.transform;
                break;
        }

        basePanel.transform.localPosition = Vector3.zero;
        basePanel.transform.localScale = Vector3.one;
    }

	public void setChildOfUIDefault(Transform t){
		t.parent = uiDefaultObj.transform;
	}

    protected void AddShowingList(UIBasePanel basePanel)
    {
        //Debug.LogWarning("2JW : " + basePanel + " : " + basePanel.gameObject, basePanel);
        if (!basePanel.gameObject.activeSelf)
            basePanel.gameObject.SetActive(true);
        
        ShowingPanels.Insert( 0, basePanel );

        AdjustShowPosition(basePanel);
    }

    /// <summary>
    /// 각 UIBasePanel의 ZTickness를 기반으로 Position만 갱신시키도록 한다.
    /// </summary>
    protected void AdjustShowPosition(UIBasePanel basePanel)
    {
        //float ZPos = 0;
        //UIBasePanel frontPanel = null;

        //ShowingPanels.ForEach( (p) =>
        //{
        //    // 같은 패널이면 무시
        //    if (p == basePanel)
        //        return;

        //    // 현재 UIType과 같아야함.
        //    if (p.UIType != basePanel.UIType)
        //        return;

        //    // ShowOption이 다르면 리턴
        //    if (p.ShowOption != basePanel.ShowOption)
        //        return;

        //    //if (ZPos >= p.transform.localPosition.z)
        //    { 
        //        //ZPos = p.transform.localPosition.z;
        //        frontPanel = p;
        //    }
        //});

        //if (null != basePanel)
        float ZPos = -basePanel.ZThickness;//ZPos 

        basePanel.transform.SetLocalZ(ZPos);
    }

    /// <summary>
    /// UIBasePanel.Show에 의해 호출되는 함수. 리스트에서 가장 앞으로 이동시켜준다.
    /// </summary>protected
    public void OnShowPanel(UIBasePanel basePanel)
    {
        // 보여지는 옵션이 Ignore면, 순서는 무시되도록 한다.
        if (basePanel.ShowOption == ShowOption.Ignore)
            return;
        
        ShowingPanels.Remove(basePanel);
        ShowingPanels.Insert(0, basePanel);
    }

    /// <summary>
    /// UIBasePanel.Hide에 의해 호출되는 함수. 리스트에서 가장 뒤로 이동시켜준다.
    /// </summary>
    public void OnHidePanel()
    {
        ShowingPanels.Remove( UIBasePanel.current );
        ShowingPanels.Add( UIBasePanel.current );
    }

    public void OnClosePanel()
    {
        if(UIBasePanel.current is MissionPanel || UIBasePanel.current is ResultRewardStarPanel)
            SetShadowLight(true);
        
        ShowingPanels.Remove(UIBasePanel.current);
    }


#endregion
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) )//&& !TutorialMgr.TutorialActive && !BuildBuyEventActive)
        {
            //< 통신중이 아닐때만 처리
            //if (SceneManager.instance.IndicatorCount <= 0)
            if( !IsActiveTutorial )//튜토리얼 팝업 열려있을 경우 안됨
                Prev();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F12))
            Debug.Break();
        else if(Input.GetKeyDown(KeyCode.F1))
        {
            string path = "UIPopup/CheatPopup";
            if(GetUIBasePanel(path ) == null)
                Open(path, false);
        }
#endif
    }
    
    public int GetRenderQ()
    {
        int HightRq = 0;
        for(int i=0; i<ShowingPanels.Count; i++)
        {
            if (ShowingPanels[i] == null || !ShowingPanels[i].gameObject.activeSelf)
                continue;

            if (ShowingPanels[i].ShowOption == ShowOption.Ignore)
                continue;

            UIPanel _UIPanel = ShowingPanels[i].GetComponent<UIPanel>();
            if (_UIPanel == null || _UIPanel.renderQueue != UIPanel.RenderQueue.Automatic)
                continue;

            if (HightRq < _UIPanel.startingRenderQueue)
                HightRq = _UIPanel.startingRenderQueue;
        }

        return HightRq + 20;
    }

    /// <summary>
    /// HeroPanel를 열어준다. 추후에 다른 경로를 통하여 아이템, 강화, 승급, 합성, 속성으로 가는 경로가있다면
    /// 함수를 수정해 줘야 한다.
    /// </summary>
    static public UIBasePanel OpenEquipPanel(UIBasePanel reOpenPanel, bool AutoEquip = true)  //자동장착에서 불러왔을때도 구분..
    {
        string invenPath = "UIPanel/EquipmentPanel";
        GameObject go = UIMgr.Open(invenPath, reOpenPanel, AutoEquip);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary>
    /// TownPanel을 열어준다.
    /// </summary>
    static public void OpenTown()
    {
        if (instance.IsNextUiAchieve)
            return;

        string townPath = "UIPanel/TownPanel";
        UIMgr.Open(townPath, true);
    }

    /// <summary>
    /// 챕터 페널 오픈
    /// </summary>
    static public UIBasePanel OpenChapter(UIBasePanel basePanel, uint openStageId=0)
    {
        string chapterPath = "UIPanel/ChapterPanel";
        GameObject go = UIMgr.Open(chapterPath, basePanel, openStageId);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 코스튬 페널 오픈 </summary>
    static public UIBasePanel OpenCostume(UIBasePanel basePanel = null)
    {
        string path = "UIPanel/CostumePanel";
        GameObject go = UIMgr.Open(path, basePanel);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> vip 팝업 열어줌 </summary>
    static public UIBasePanel OpenVipPopup()
    {
        string path = "UIPanel/VipPanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }
    /// <summary>
    /// 파트너 페널 오픈
    /// </summary>
    static public UIBasePanel OpenPartner(bool isReset)
    {
        string path = "UIPanel/PartnerPanel";
        GameObject go = UIMgr.Open(path, isReset);

        return go.GetComponent<UIBasePanel>();
    }
    /*
    /// <summary>
    /// 상점 페널 오픈
    /// </summary>
    static public UIBasePanel OpenShop()
    {
        string path = "UIPanel/ShopPanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }
    */

	static public UIBasePanel OpenPanelTest1Panel()
	{
		string path = "UIPanel/PanelTest1Panel";
		GameObject go = UIMgr.Open(path, true);
		
		return go.GetComponent<UIBasePanel>();
	}

	static public void OpenPanelTest2Panel()
	{
		GameObject test2 = GameObject.Find ("PanelTest2Panel");
		PanelTest2Panel t2Panel = test2.GetComponent<PanelTest2Panel> ();
		UIMgr.instance.setChildOfUIDefault (test2.transform);
		test2.transform.localScale = Vector3.one;
		test2.transform.localPosition = Vector3.zero;
		t2Panel.setShow (true);

		UIMgr.GetTownBasePanel ().Hide ();
	}

    static public UIBasePanel OpenRankPanel()
    {
        string path = "UIPanel/RankingPanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }
    static public UIBasePanel OpenMissionPanel(uint questID, int testMission=0)
    {
        GameObject go = UIMgr.Open("UIPanel/MissionPanel", questID, testMission);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary>
    /// 보스 출현 페널 오픈
    /// </summary>
    static public UIBasePanel OpenAppearBoss()
    {
        string path = "UIPanel/AppearBossPanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary>
    /// 난투장패널
    /// </summary>
    static public UIBasePanel OpenDogFight()
    {
        string path = "UIPanel/FreefightPanel";
        GameObject go = UIMgr.Open(path, true);
        return go.GetComponent<UIBasePanel>();
    }

    /// <summary>
    /// 탑메뉴 생성 or 열기
    /// </summary>
    public void OpenTopMenu(UIBasePanel basePanel)
    {
        if (TopMenu == null)
        {
            string path = "UIObject/TopMenuObject";
            GameObject topMenu = Instantiate(ResourceMgr.LoadUI(path)) as GameObject;

            topMenu.name = "TopMenu";
            TopMenu = topMenu.GetComponent<TopMenuObject>();
            TopMenu.Initialize(uiSystemObj.transform);
        }

        TopMenu.ShowTopMenu(basePanel);
    }

    public void HideTopMenu()
    {
        if(TopMenu != null)
            TopMenu.OnHideTopMenu();
    }

    /// <summary> PVP 페널로 이동 </summary>
    static public UIBasePanel OpenArenaPanel(UIBasePanel basePanel=null)
    {
        string path = "UIPanel/ArenaPanel";
        GameObject go = UIMgr.Open(path, basePanel);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 메일 팝업 </summary>
    static public UIBasePanel OpenMailPopup()
    {
        string path = "UIPopup/MailPanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }
    /*
    /// <summary> 클릭한 아이템의 상세 정보를 보여주는 팝업을 열어준다. </summary>
    static public void OpenClickPopup(uint lowDataId, Vector3 newPos)
    {
        if (instance.IsActiveTutorial)
            return;

        string path = "UIPopup/ClickPopup";
        UIMgr.Open(path, lowDataId, newPos);
    }
    */
    /// <summary> 마계의탑 페널 </summary>
    static public UIBasePanel OpenTowerPanel(uint lowDataId=0)
    {
        string path = "UIPanel/TowerPanel";
        GameObject go = UIMgr.Open(path, lowDataId);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 옵션 페널 </summary>
    static public UIBasePanel OpenOptionPanel(bool isStop, byte type = 0 )
    {
        if (isStop)
            Time.timeScale = 0;

        string path = "UIPanel/OptionPanel";
        GameObject go = UIMgr.Open(path, type);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 업적 페널 </summary>
    static public UIBasePanel OpenAchievePanel(UIBasePanel reOpenPanel, int tabType, int achieveType = 1)
    {
        string path = "UIPanel/AchievePanel";
        GameObject go = UIMgr.Open(path, tabType, reOpenPanel, achieveType);

        return go.GetComponent<UIBasePanel>();
    }
    /// <summary> 혜택 페널 </summary>
    static public UIBasePanel OpenBenefitpanel()
    {
        string path = "UIPanel/Benefitpanel";
        GameObject go = UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }
    /// <summary> 소셜 페널 </summary>
    static public UIBasePanel OpenSocialPanel(int type, UIBasePanel reOpenPanel = null)
    {
        string path = "UIPanel/SocialPanel";
        GameObject go = UIMgr.Open(path, type, reOpenPanel);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 조이스틱 열어줌  </summary>
    static public Joystick OpenJoystick(Vector3 pos)
    {
        string path = "UIObject/Joystick";
        GameObject joystick = UIMgr.Open(path, pos);

        return joystick.GetComponent<Joystick>();
    }

    /// <summary> 채팅 팝업 열어줌 </summary>
    static public ChatPopup OpenChatPopup()
    {
        string path = "UIPopup/ChatPopup";
        GameObject chatPop = UIMgr.Open(path, true);
        
        return chatPop.GetComponent<ChatPopup>();
    }
    
    /// <summary> 퀘스트 팝업 열어줌 </summary>
    static public UIBasePanel OpenQuestPopup()
    {
        string path = "UIPopup/QuestPopup";
        GameObject go =UIMgr.Open(path, true);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 준비 팝업 열어줌 </summary>
    static public UIBasePanel OpenReadyPopup(GAME_MODE mode, UIBasePanel basePanel, int completeCount, int maxCount, uint stageId=0)
    {
        string path = "UIPopup/ReadyPopup";
        GameObject go = UIMgr.Open(path, mode, basePanel, completeCount, maxCount, stageId);

        return go.GetComponent<UIBasePanel>();
    }

    static public UIBasePanel OpenShopPanel(UIBasePanel basePanel=null)
    {
        string path = "UIPanel/ShopPanel";
        GameObject go = UIMgr.Open(path, basePanel);

        return go.GetComponent<UIBasePanel>();
    }


    /// <summary> 가챠 페널 열기 </summary>
    static public void OpenGachaPanel(uint norTime, uint serTime)
    {
        string path = "UIPanel/GachaPanel";
        UIMgr.Open(path, norTime, serTime);
    }

    /// <summary> 튜토리얼 팝업 열기 </summary>
    static public TutorialPopup OpenTutorialPopup(int townTuto=-1, TutorialSupport obj =null)
    {
        string path = "UIPopup/TutorialPopup";
        GameObject go = UIMgr.Open(path, townTuto, obj);
        
        if (go != null)
        {
            return go.GetComponent<TutorialPopup>();
        }

        return null;
    }

    /// <summary> 길드 페널 </summary>
    static public void OpenGuildPanel()
    {
        string path = null;
        if (0 < NetData.instance.GetUserInfo()._GuildId)
            path = "UIPanel/GuildPanel";
        else
            path = "UIPanel/GuildJoinPanel";

        UIMgr.Open(path, true);
    }

    /// <summary> 축원 결과패널 열기 </summary>
    static public void OpenBlessResultPanel(List<uint> id, List<uint> type)
    {
        string path = "UIPanel/PrayerPanel";
        UIMgr.Open(path, id, type);
    }

    static public void OpenSelectTitlePopup()
    {
        string path = "UIPopup/SelectTitlePopup";
        UIMgr.Open(path, true);
    }

    /// <summary> 맵 페널 연다 </summary>
    static public MapPanel OpenMapPanel()
    {
        string path = "UIPanel/MapPanel";
        GameObject go = UIMgr.Open(path, true);
        return go.GetComponent<MapPanel>();
    }

    /// <summary> 콜로세움 </summary>
    static public void OpenColosseumPanel(uint dungeonId)
    {
        string path = "UIPanel/ColosseumPanel";
        UIMgr.Open(path, dungeonId);
    }

    /// <summary> 초대 팝업 </summary>
    static public void OpenInvitePopup(GAME_MODE mode)
    {
        string path = "UIPopup/InvitePopup";
        UIMgr.Open(path, mode);
    }
    
    /// <summary> 복귀팝업 </summary>
    static public void OpenComebackPopup(int info)
    {
        string path = "UIPopup/ComebackPopup";
        UIMgr.Open(path, info);
    }
    
    /// <summary> 실명인증 팝업 </summary>
    static public void OpenNameCertifyPopup()
    {
        string path = "UIPopup/NameCertifyPopup";
        UIMgr.Open(path, true);
    }

    /// <summary> 실명인증 팝업 </summary>
    static public void OpenDungeonPanel(bool isInvite=false, uint dungeonId=0, GAME_MODE mode=GAME_MODE.NONE, int subType=0)
    {
        string path = "UIPanel/DungeonPanel";
        UIMgr.Open(path, isInvite, dungeonId, mode, subType);
    }

    /// <summary> 부활 팝업 </summary>
    static public void OpenRevivePopup(System.Action callBack, bool IsFreefight=false)
    {
        string path = "UIPanel/InGameRevivePanel";
        UIMgr.Open(path, callBack, IsFreefight);
    }

    /// <summary> 유저정보 팝업 </summary>
    static public void OpenUserInfoPopup(long charUID, string userName, int charIdx, int vipLv, int lv=0, bool isCharInfo=false)
    {
        string path = "UIPopup/UserInfoPopup";
        UIMgr.Open(path, charUID, userName, charIdx, vipLv, lv, isCharInfo);
    }

    /// <summary> 알림 페널 </summary>
    static public UIBasePanel OpenNoticePanel(NoticeType type, uint condition=0, string str=null, object obj=null)
    {
        string path = "UIPanel/NoticePanel";
        GameObject go = Open(path, type, condition, str, obj);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 카테고리페널 열어 </summary>
    static public UIBasePanel OpenCategoryPanel(UIBasePanel prevPanel)
    {
        string path = "UIPanel/CategoryPanel";
        GameObject go = Open(path, prevPanel);

        return go.GetComponent<UIBasePanel>();
    }

    /// <summary> 아이템 상세 팝업 </summary>
    static public ItemDetailPopup OpenDetailPopup(UIBasePanel basePanel, NetData._ItemData itemData=null, int depth = 3, bool isAutoQ = false)
    {
        string path = "UIPopup/DetailPopup";
        GameObject go = Open(path, isAutoQ, depth, basePanel, itemData);

        return go.GetComponent<ItemDetailPopup>();
    }

    /// <summary> 아이템 상세 팝업 </summary>
    static public ItemDetailPopup OpenDetailPopup(UIBasePanel basePanel, uint itemLowData, int depth = 6, bool isAutoQ = false)
    {
        NetData._ItemData itemData = new NetData._ItemData(itemLowData, itemLowData, 0, 0, false);
        string path = "UIPopup/DetailPopup";
        GameObject go = Open(path, isAutoQ, depth, basePanel, itemData);

        return go.GetComponent<ItemDetailPopup>();
    }

    public void AddErrorPopup(int errorPopLowId, uint titleId=141, uint btn_0=117)
    {
        if (errorPopLowId == 0)
        {
            Debug.Log(string.Format("popup <color=red>LowDataId={0} error</color>", errorPopLowId));
            return;
        }

        SceneUIMgr.AddErrorPopup(errorPopLowId, titleId, btn_0);
    }

    /// <summary> 텍스트로만 강제로 넣는 함수. 테이블 추가에 용이하지 않는 것은 이걸 사용.</summary>
    public void AddPopup(string title, string msg, string btn_0, string btn_1=null, string btn_2=null, System.Action callBack_1=null, System.Action callBack_2=null, System.Action callBack_3=null)
    {
        SceneUIMgr.AddPopup(0, msg, title, btn_0, btn_1, btn_2, callBack_1, callBack_2, callBack_3);
    }

    /// <summary> StringCommon이용해서 텍스빼오는 거 </summary>
    public void AddPopup(uint title, uint msg, uint btn_0, uint btn_1=0, uint btn_2=0, System.Action callBack_1 = null, System.Action callBack_2 = null, System.Action callBack_3 = null)
    {
        SceneUIMgr.AddPopup(0, _LowDataMgr.instance.GetStringCommon(msg), _LowDataMgr.instance.GetStringCommon(title == 0 ? 141 : title)
            , _LowDataMgr.instance.GetStringCommon(btn_0 == 0 ? 117 : btn_0), btn_1 == 0 ? null : _LowDataMgr.instance.GetStringCommon(btn_1), btn_2 == 0 ? null : _LowDataMgr.instance.GetStringCommon(btn_2)
            , callBack_1, callBack_2, callBack_3 );
    }
    
    /// <summary>
    /// UI형식에 맞게 능력치를 string으로 준다.
    /// </summary>
    /// <param name="type">AbilityType 값</param>
    /// <param name="value">바꾸고자 하는 값</param>
    /// <returns></returns>
    public string GetAbilityStrValue(AbilityType type, float value)
    {
        string valueStr = null;
        if (value == 0)
            return "0";

		//valueStr = value.ToString("0"); // ToString("#,##");
		int intVal = (int)value;
		valueStr = intVal.ToString("0"); // ToString("#,##");

        return valueStr;
    }

	public string GetAbilityStrValue(float value)
	{
		string valueStr = null;
		if (value == 0)
			return "0";

		int intVal = (int)value;
		valueStr = intVal.ToString("0"); // ToString("#,##");
		
		return valueStr;
	}
    
    /// <summary>
    /// Ability의 글자를 빼온다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetAbilityLocName(AbilityType type)
    {
        uint id = 0;
        switch (type)
        {
            case AbilityType.HP:
                id = 18;
                break;//체력
            case AbilityType.DAMAGE:
                id = 19;
                break;//공격력
            case AbilityType.HIT_RATE:
                id = 20;
                break;//명중률
            case AbilityType.DODGE_RATE:
                id = 21;
                break;//회피율
            case AbilityType.CRITICAL_RATE:
                id = 22;
                break;//치명타 확률
            case AbilityType.CRITICAL_RES:
                id = 23;
                break;//치명타 저항
            case AbilityType.CRITICAL_DAMAGE:
                id = 24;
                break;//치명타 피해
            case AbilityType.DRAIN_HP:
                id = 25;
                break;//생명력 흡수
            case AbilityType.DEFENCE_IGNORE:
                id = 26;
                break;//방어력 무시
            case AbilityType.DAMAGE_DECREASE:
                id = 27;
                break;//데미지 감소
            case AbilityType.DAMAGE_DECREASE_RATE:
                id = 28;
                break;//데미지 감소율
            case AbilityType.COOLTIME:
                id = 29;
                break;//쿨타임
            case AbilityType.ATTACK_SPEED:
                id = 96;
                break;//공격속도 증가
            case AbilityType.EXP_UP:
                id = 95;
                break;//경험치 증가
            case AbilityType.ALLSTAT_RATE:
                id = 97;
                break;
            case AbilityType.ATTACK_RANGE:
                id = 919;
                break;
            case AbilityType.ATTACK_ANGLE:
                id = 920;
                break;
            case AbilityType.MOVE_SPEED:
                id = 921;
                break;
            case AbilityType.DETECTED_RANGE:
                id = 922;
                break;
        }

        string name = _LowDataMgr.instance.GetStringCommon(id);
        if (string.IsNullOrEmpty(name))
            name = type.ToString();

        return name;
    }
    
    /// <summary> 탑메뉴 재화 재갱신 </summary>
    public void RefreshTopMenuCash(AssetType type)
    {
        if (TopMenu == null)
            return;

        TopMenu.RefreshCash(type);
    }

    public void CloseReadyPopup()
    {
        UIBasePanel ready = GetUIBasePanel("UIPopup/ReadyPopup");
        if (ready != null)
            (ready as ReadyPopup).OnClose();
    }
    
    public static void AddLogChat(string msg)
    {
        UIBasePanel chatPopup = GetUIBasePanel("UIPopup/ChatPopup", false);
        if (chatPopup == null)
            return;

        (chatPopup as ChatPopup).AddLogChat(msg);
    }

    /// <summary> 멀티, 콜로에서 사용할 유아이삭제 함수 </summary>
    public void OnlyDefaultUI()
    {
        int i = 0;
        while(i < ShowingPanels.Count)
        {
            UIBasePanel panel = ShowingPanels[i];
            if (panel is TownPanel || !panel.Managed || panel.NotClear || panel.IsPopup)
            {
                i++;
                continue;
            }

            panel.CloseEvent();
            ShowingPanels.Remove(panel);
            
        }
    }

    /// <summary> 탑메뉴에 들어갈 이름 </summary>
    public void SetTopMenuTitleName(uint commonLowDataId)
    {
        if (TopMenu == null)
        {
            string path = "UIObject/TopMenuObject";
            GameObject topMenu = Instantiate(ResourceMgr.LoadUI(path)) as GameObject;

            topMenu.name = "TopMenu";
            TopMenu = topMenu.GetComponent<TopMenuObject>();
            TopMenu.Initialize(uiSystemObj.transform);
        }

        TopMenu.SetTitleName(commonLowDataId);
    }

    /// <summary> 리젠 타임 </summary>
    public void SetRegenPower()
    {
        if (TopMenu == null)
        {
            //string path = "UIObject/TopMenuObject";
            //GameObject topMenu = Instantiate(ResourceMgr.LoadUI(path)) as GameObject;

            //topMenu.name = "TopMenu";
            //TopMenu = topMenu.GetComponent<TopMenuObject>();
            //TopMenu.Initialize(uiSystemObj.transform);
            return;
        }
        
        TopMenu.SetPowerTime(SceneManager.instance.RegenPowerTime);
    }

    /// <summary> 바닥없는 곳은 이거 꺼놓고 다시 켜놓는 작업해야함 </summary>
    public void SetShadowLight(bool isUse)
    {
        ShadowLight.shadows = isUse ? LightShadows.Hard : LightShadows.None;
    }

	public void SetShadowLightActive(bool b)
	{
		ShadowLight.gameObject.SetActive (b);
	}
    
    public string ReturnPanel
    {
        set {
            _ReturnPanel = value;
        }
        get {

            if (string.IsNullOrEmpty(_ReturnPanel))
                return "";

            string name = _ReturnPanel;
            _ReturnPanel = null;
            return name;
        }
    }
}


