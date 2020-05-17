using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//public enum eSkillLevelData
//{
//    //< 공용
//    targetLimit = 0,
//    radius,
//    factorRate,
//    basicValue,
//    ignoreDef,

//    //< 버프관련
//    overlapcount = 20,
//    durationTime,

//    //< 발사체관련
//    moveSpeed = 40,
//    maxDistance,
//    penetrate,
//    multi,
//    startAngle,
//    hit,
//    div,
//}

//public class SaveValue
//{
//    public MainSkillTool.ValueType type;
//    public object data;
//    public string[] SelectComboItem;

//    public SaveValue(object obj, MainSkillTool.ValueType _type = MainSkillTool.ValueType.Common, string[] _SelectComboItem = null)
//    {
//        data = obj;
//        type = _type;
//        SelectComboItem = _SelectComboItem;
//    }
//}

//public class SystemData
//{
//    public enum eSystemDataType
//    {
//        UnitGroupInfo,
//        NormalGroupInfo,
//        SkillGroupInfo,
//        AbilityGroup,
//    }

//    public eSystemDataType type;
//    public MainSkillTool parent;

//    public UnitGroupInfo _UnitGroupInfo;
//    public SkillGroupInfo _NormalGroupInfo;
//    public SkillGroupInfo _SkillGroupInfo;
//    public AbilityGroup _AbilityGroup;

//    public UnitGroupInfo _CopyUnitGroupInfo;
//    public SkillGroupInfo _CopyNormalGroupInfo;
//    public SkillGroupInfo _CopySkillGroupInfo;
//    public AbilityGroup _CopyAbilityGroup;

//    public void DataCopy()
//    {
//        if (_UnitGroupInfo != null)
//            _CopyUnitGroupInfo = _UnitGroupInfo.GetClone();
//        if (_NormalGroupInfo != null)
//            _CopyNormalGroupInfo = _NormalGroupInfo.GetClone();
//        if (_SkillGroupInfo != null)
//            _CopySkillGroupInfo = _SkillGroupInfo.GetClone();
//        if (_AbilityGroup != null)
//            _CopyAbilityGroup = _AbilityGroup.GetClone();
//    }

//    public void AddData()
//    {
//        //< 해당 데이터를 찾아서 추가한다.
//        if (type == eSystemDataType.UnitGroupInfo)
//        {
//            if (_CopyUnitGroupInfo == null)
//                return;

//            int lastIdx = 0;
//            for (int i = 0; i < parent.UnitGroupDic.Count; i++)
//                lastIdx = lastIdx < parent.UnitGroupDic[i].unitIdx ? parent.UnitGroupDic[i].unitIdx : lastIdx;

//            _CopyUnitGroupInfo.unitIdx = ++lastIdx;
//            parent.UnitGroupDic.Add(_CopyUnitGroupInfo);

//            parent.SelectUnitGroup = _CopyUnitGroupInfo;
//        }
//        else if (type == eSystemDataType.NormalGroupInfo)
//        {
//            if (_CopyNormalGroupInfo == null)
//                return;

//            if (parent.SelectUnitGroup.normalGroups.Count >= 4)
//                parent.ShowPopup("노말 그룹 추가 실패", MainSkillTool.eErrorCode.NotAddSkillGroup_MaxCount);
//            else
//            {
//                //< 선택한 녀석 위에 추가해준다.
//                if (_NormalGroupInfo == null)
//                {
//                    parent.SelectUnitGroup.normalGroups.Add(_CopyNormalGroupInfo);
//                }
//                else
//                {
//                    for (int i = 0; i < parent.SelectUnitGroup.normalGroups.Count; i++)
//                    {
//                        if (parent.SelectUnitGroup.normalGroups[i] == _NormalGroupInfo)
//                        {
//                            parent.SelectUnitGroup.normalGroups.Insert(i, _CopyNormalGroupInfo);
//                            break;
//                        }
//                    }
//                }

//                //< 카운트를 다시 대입
//                for (int i = 0; i < parent.SelectUnitGroup.normalGroups.Count; i++)
//                    parent.SelectUnitGroup.normalGroups[i].skillIdx = (i + 1);

//                parent.SelectSkillGroup = _CopyNormalGroupInfo;
//                parent.SelectNormalGroup = _CopyNormalGroupInfo;
//            }
//        }
//        else if (type == eSystemDataType.SkillGroupInfo)
//        {
//            if (_CopySkillGroupInfo == null)
//                return;

//            if (parent.SelectUnitGroup.skillGroups.Count >= 4)
//                parent.ShowPopup("스킬 그룹 추가 실패", MainSkillTool.eErrorCode.NotAddSkillGroup_MaxCount);
//            else
//            {
//                //< 선택한 녀석 위에 추가해준다.
//                if (_SkillGroupInfo == null)
//                {
//                    parent.SelectUnitGroup.skillGroups.Add(_CopySkillGroupInfo);
//                }
//                else
//                {
//                    for (int i = 0; i < parent.SelectUnitGroup.skillGroups.Count; i++)
//                    {
//                        if (parent.SelectUnitGroup.skillGroups[i] == _SkillGroupInfo)
//                        {
//                            parent.SelectUnitGroup.skillGroups.Insert(i, _CopySkillGroupInfo);
//                            break;
//                        }
//                    }
//                }

//                //< 카운트를 다시 대입
//                for (int i = 0; i < parent.SelectUnitGroup.skillGroups.Count; i++)
//                    parent.SelectUnitGroup.skillGroups[i].skillIdx = (i + 1);

//                parent.SelectSkillGroup = _CopySkillGroupInfo;
//                parent.SelectNormalGroup = null;
//            }
//        }
//        else if (type == eSystemDataType.AbilityGroup)
//        {
//            if (_CopyAbilityGroup == null)
//                return;

//            if (parent.SelectSkillGroup.abilityGroups.Count > 8)
//            {
//                parent.ShowPopup("어빌리티 추가에 실패하였습니다.", MainSkillTool.eErrorCode.NotAddAbility_MaxCount);
//            }
//            else
//            {
//                //< 선택한 녀석 위에 추가해준다.
//                if (_AbilityGroup == null)
//                {
//                    parent.SelectSkillGroup.abilityGroups.Add(_CopyAbilityGroup);
//                }
//                else
//                {
//                    for (int i = 0; i < parent.SelectSkillGroup.abilityGroups.Count; i++)
//                    {
//                        if (parent.SelectSkillGroup.abilityGroups[i] == _AbilityGroup)
//                        {
//                            parent.SelectSkillGroup.abilityGroups.Insert(i, _CopyAbilityGroup);
//                            break;
//                        }
//                    }
//                }
//            }

//            parent.SelectAbilityGroup = _CopyAbilityGroup;
//        }

//        //_CopyUnitGroupInfo = null;
//        //_CopyNormalGroupInfo = null;
//        //_CopySkillGroupInfo = null;
//        //_CopyAbilityGroup = null;
//    }

//    public void Delete()
//    {
//        if (type == eSystemDataType.UnitGroupInfo)
//            parent.DeleteUnitGroup();
//        else if (type == eSystemDataType.SkillGroupInfo)
//            parent.DeleteSkillGroupData();
//        else if (type == eSystemDataType.NormalGroupInfo)
//            parent.DeleteNormalGroupData();
//        else if (type == eSystemDataType.AbilityGroup)
//            parent.DeleteAbilityGroupData();
//    }
//}

//public class MainSkillTool : MonoBehaviour {

//    static string FilePath = Application.dataPath + "/SkillData.json";

//    //< 유닛 그룹
//    public List<UnitGroupInfo> UnitGroupDic = new List<UnitGroupInfo>();

//    //< 현재 선택한 유닛 그룹
//    public UnitGroupInfo SelectUnitGroup;

//    //< 현재 선택한 스킬그룹
//    public SkillGroupInfo SelectSkillGroup, SelectNormalGroup;

//    //< 현재 선택한 스킬 데이터
//    public AbilityGroup SelectAbilityGroup;

//    //< 현재 선택한 어빌리티
//    public SkillAbilityInfo SelectAbilityInfo;

//    // Use this for initialization
//    void Awake()
//    {
//        if (File.Exists(FilePath))
//        {
//            string str = File.ReadAllText(FilePath);
//            var response = JSON.Load(str);
//            UnitGroupDic = response.Make<List<UnitGroupInfo>>();

//            //< 정렬을 시켜준다.
//            UnitGroupDic.Sort(delegate(UnitGroupInfo tmp1, UnitGroupInfo tmp2) { return tmp1.unitIdx.CompareTo(tmp2.unitIdx); });
//        }
//        else
//        {
//            AddUnitGroupList(1);
//        }

//        ServerPath = PlayerPrefs.GetString("SERVER_PATH");
//        if (ServerPath == "")
//            ServerPath = "URL";

//        SimulationUnitID = PlayerPrefs.GetString("SimulationUnitID");


//        //< 멤버변수 string로 접근하여 값 대입 테스트
//        //SkillAbilityInfo test = new SkillAbilityInfo();
//        //test.radius = 55;

//        //System.Type type = typeof(SkillAbilityInfo);
//        //System.Reflection.FieldInfo[] f = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
//        //for (int i = 0; i < f.Length; i++)
//        //{
//        //    Debug.Log(f[i].Name);

//        //    if (f[i].Name == "radius")
//        //        test.SetData("radius", 9992);
//        //}
//    }

//    bool FirstCheck = false;
//    void Update()
//    {
//        if (!FirstCheck && SimulationGameInfo.IsDone)
//        {
//            FirstCheck = true;

//            //< 맵을 불러온다
//            InGameMapMgr.instance.LoadMap("Quarry001", 0, "[\"Quarry_Lightmap\"]", (obj) =>
//            {
//                obj.transform.position = new Vector3(0, 0, 23);
//            });
//        }
//    }

//    float MainMenuHeight = 80;
//    float MainMenuHeightSize = 620;
//    Rect[] rect = new Rect[7];
//    void OnGUI()
//    {
//        if (!SimulationGameInfo.IsDone)
//            return;

//        if (ShowPopupCheck)
//        {
//            GUI.Window(100, new Rect(440, 160, 400, 400), ErrorPopup, "에러");
//            GUI.UnfocusWindow();
//            GUI.BringWindowToFront(100);
//            GUI.FocusWindow(100);
//        }
//        else if (ShowComboBox)
//        {
//            GUI.Window(20, new Rect(rect[3].max.x + 10, MainMenuHeight, 300, MainMenuHeightSize), ComboBoxView, "Select");
//            GUI.UnfocusWindow();
//            GUI.BringWindowToFront(20);
//            GUI.FocusWindow(20);
//        }
//        else if (SkillComboView)
//        {
//            GUI.Window(50, new Rect(rect[4].max.x + 10, MainMenuHeight, 300, MainMenuHeightSize), _SkillComboView, "Select");
//            GUI.UnfocusWindow();
//            GUI.BringWindowToFront(50);
//            GUI.FocusWindow(50);
//        }
//        else if (FileUploadCheck)
//        {
//            GUI.Window(30, new Rect(440, 160, 400, 210), UploadPopup, "파일 업로드");
//            GUI.UnfocusWindow();
//            GUI.BringWindowToFront(30);
//            GUI.FocusWindow(30);
//        }
//        else if (SystemMemuPopupCheck)
//        {
//            GUI.Window(40, SystemMemuPopupPos, SystemMemuPopup, "시스템 메뉴");
//            GUI.UnfocusWindow();
//            GUI.BringWindowToFront(40);
//            GUI.FocusWindow(40);
//        }


//        //else
//        {
//            //< 상단 메뉴
//            TopMenu();

//            if (!SimulationLive)
//            {
//                //< 왼쪽 유닛 그룹 리스트
//                UnitGroupList();

//                //< 스킬 그룹
//                SkillGroupData();

//                //< 어빌리티 그룹
//                AbilityGroupData();

//                //< 중앙 어빌리티
//                AbilityEdit();

//                //< 우측 상세정보
//                DataEdit();

//                //< 레벨정보
//                SkillLevelData();
//            }
//            else
//            {
//                for (int i = 0; i < SelectUnitGroup.skillGroups.Count; i++)
//                {
//                    if (GUI.Button(new Rect(5, 100 + (i * 60), 100, 60), (i + 1) + "번 스킬 사용"))
//                    {
//                        Unit unit = MainUnit.GetComponent<Unit>();

//                        //< 스킬 정보 대입
//                        unit.SkillCtlr.UnitId = (uint)SelectUnitGroup.unitIdx;
//                        //unit.SkillCtlr.SetSkill((uint)i, i, i);

//                        unit.SkillCtlr.SkillList[i].coolTime = 0;

//                        //< 스킬 실행
//                        unit.UseSkill(i);
//                        break;
//                    }
//                }
//            }
//        }
//    }

//    #region 상단 메뉴
//    string GroupAddInputStr = "", GroupSeachInputStr = "", ServerPath = "", SimulationUnitID = "";
//    bool FileUploadCheck, FileUploadSuccess = false, SimulationLive = false;
//    GameObject MainUnit;
//    PlayerController syncctr;
//    void TopMenu()
//    {
//        GUI.Window(10, new Rect(5, 5, 1380, 70), _TopMenu, "MENU");
//    }

//    void _TopMenu(int id)
//    {
//        //<=======================================================
//        //<                 어빌리티 리스트 처리
//        //<=======================================================
//        GroupAddInputStr = GUI.TextField(new Rect(5, 20, 80, 22), GroupAddInputStr);
//        if (GUI.Button(new Rect(85, 20, 52, 22), "ADD"))
//        {
//            int idx;
//            if (int.TryParse(GroupAddInputStr, out idx))
//                AddUnitGroupList(idx);
//            else
//                ShowPopup("그룹 추가에 실패하였습니다.", eErrorCode.NotNumberStr);
//        }

//        GroupSeachInputStr = GUI.TextField(new Rect(5, 44, 80, 22), GroupSeachInputStr);
//        if (GUI.Button(new Rect(85, 44, 52, 22), "Seach"))
//        {
//            int idx;
//            if (int.TryParse(GroupSeachInputStr, out idx))
//                SeachUnitGroup(idx);
//            else
//                ShowPopup("그룹 검색에 실패하였습니다.", eErrorCode.NotNumberStr);
//        }

//        //<=======================================================
//        //<                 서버 데이터 동기화
//        //<=======================================================
//        if (GUI.Button(new Rect(150, 20, 150, 45), "서버데이터로 동기화"))
//        {
//            UnitGroupDic = UnitSkillLowData.SkillGroupDic;
//        }

//        if (GUI.Button(new Rect(320, 20, 100, 45), "파일 정리"))
//        {
//            FileClean();
//        }

//        //<=======================================================
//        //<                 시뮬레이션 실행
//        //<=======================================================
//        if (GUI.Button(new Rect(500, 20, 200, 45), SimulationLive ? "시뮬레이션 종료" : "시뮬레이션 실행"))
//        {
//            //< 선 체크 진행
//            if (!SimulationLive)
//            {
//                if (SelectUnitGroup == null)
//                {
//                    ShowPopup("유닛그룹을 선택해주세요", eErrorCode.None);
//                    return;
//                }

//                SimulationUnitID = SelectUnitGroup.unitIdx.ToString();
//                //if (SimulationUnitID == "")
//                //{
//                //    ShowPopup("사용할 유닛 인덱스를 채워주세요", eErrorCode.None);
//                //    return;
//                //}

//                //< 플레이어 생성
//                if (LowDataMgr.GetUnitData(uint.Parse(SimulationUnitID)) == null)
//                {
//                    ShowPopup("해당 유닛이 테이블에 존재하지않습니다.", eErrorCode.None);
//                    return;
//                }

//                //< 파일부터 저장
//                DataSave();

//                UnitSkillLowData.SkillGroupDic = UnitGroupDic;

//                PlayerPrefs.SetString("SimulationUnitID", SimulationUnitID);

//                if (MainUnit != null)
//                    DestroyImmediate(MainUnit);

//                //< 모든 플레이어컨트롤러 삭제
//                if (G_GameInfo.GameInfo != null)
//                    G_GameInfo.GameInfo.AllRemovePlayerController();

//                //GameObject Team1Start = GameObject.Find("Team1Start");

//                NetData.UnitData unit = new NetData.UnitData();
//                unit.pc_gsn = uint.Parse(SimulationUnitID);
//                unit.pc_id = uint.Parse(SimulationUnitID);
//                unit.star_grade = LowDataMgr.GetUnitData(unit.pc_id).defaultGrade;
//                NetData.instance.AddUnitData(unit);

//                PlayerSyncData syncdata = new PlayerSyncData();
//                syncdata.TeamID = 0;
//                syncdata.unitSyncDatas = new List<UnitSyncData>();
//                syncdata.unitSyncDatas.Add(new UnitSyncData());
//                syncdata.unitSyncDatas[0].LowID = uint.Parse(SimulationUnitID);
//                syncdata.unitSyncDatas[0].TeamID = 0;
//                syncdata.unitSyncDatas[0].Grade = unit.star_grade;
//                syncdata.unitSyncDatas[0].Stats = GK_StatGenerator.GenerateTotalStatsFromNetData(NetData.instance, (uint)syncdata.unitSyncDatas[0].LowID, 0);

//                if (syncctr != null && syncctr.gameObject != null)
//                    DestroyImmediate(syncctr.gameObject);

//                syncctr = G_GameInfo.GameInfo.CreatePlayerController(syncdata);
//                MainUnit = syncctr.Leader.gameObject;
//                syncctr.Leader.IsLeader = true;
//                syncctr.Leader.FSM.Enable(true);
//                syncctr.Leader.SkillCtlr.UnitId = (uint)SelectUnitGroup.unitIdx;
//                syncctr.Leader.CharInfo.Stats[AbilityType.HP].Value += 9999999;

//                G_GameInfo.GameInfo.AutoMode = true;
//                Invoke("UnitChangeState", 0.5f);

//                GameObject _camera = GameObject.Find("CameraManager");
//                _camera.transform.position = new Vector3(0, 0, 0);
//                Camera.main.transform.localPosition = Vector3.zero;

//                SimulationLive = !SimulationLive;

//                //< 몬스터 위치를 다시잡는다.
//                G_GameInfo.GameInfo.characterMgr.allTeamDic[1][0].transform.position = new Vector3(0, 0, 24);
//                G_GameInfo.GameInfo.characterMgr.allTeamDic[1][0].FSM.Enable(UnitState.Idle);
//                G_GameInfo.GameInfo.characterMgr.allTeamDic[1][0].ChangeState(UnitState.Idle);
//            }
//            else
//            {
//                GameObject _camera = GameObject.Find("CameraManager");
//                _camera.transform.position = new Vector3(0, 1000, 0);
//                Camera.main.transform.localPosition = Vector3.zero;

//                SimulationLive = !SimulationLive;

//                if (MainUnit != null)
//                    DestroyImmediate(MainUnit);
//            }
//        }
//        //SimulationUnitID = GUI.TextField(new Rect(500, 45, 200, 22), SimulationUnitID);

//        //<=======================================================
//        //<                 에러체크 실행
//        //<=======================================================
//        if (GUI.Button(new Rect(705, 20, 100, 45), "Error Check"))
//        {
//            ErrorCheck();
//        }

//        //<=======================================================
//        //<                     저장 처리
//        //<=======================================================
//        if (GUI.Button(new Rect(809, 20, 150, 45), "Json 저장하기"))
//            SaveJson();
//        if (GUI.Button(new Rect(970, 20, 300, 25), "서버 업로드"))
//        {
//            if (ServerPath.Contains("URL") || ServerPath.Length < 10)
//                ShowPopup("업로드 PHP 주소를 제대로 입력해주세요.", eErrorCode.None);
//            else
//            {
//                PlayerPrefs.SetString("SERVER_PATH", ServerPath);
//                PlayerPrefs.Save();

//                FileUploadCheck = true;
//                FileUploadSuccess = false;
//                StartCoroutine(UploadFileCo(FilePath, "", ServerPath));
//            }
//        }
//        ServerPath = GUI.TextField(new Rect(970, 45, 300, 22), ServerPath);
//    }

//    void FileClean()
//    {
//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            for (int j = 0; j < UnitGroupDic[i].normalGroups.Count; j++)
//            {
//                for (int a = 0; a < UnitGroupDic[i].normalGroups[j].abilityGroups.Count; a++)
//                {
//                    for (int b = 0; b < UnitGroupDic[i].normalGroups[j].abilityGroups[a].skillAbilitys.Count; b++)
//                        UnitGroupDic[i].normalGroups[j].abilityGroups[a].skillAbilitys[b].AddType();
//                }
//            }
//        }

//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            FileClean(UnitGroupDic[i].normalGroups);
//            FileClean(UnitGroupDic[i].skillGroups);
//        }
//    }

//    void FileClean(List<SkillGroupInfo> list)
//    {
//        for (int j = 0; j < list.Count; j++)
//        {
//            for (int a = 0; a < list[j].abilityGroups.Count; a++)
//            {
//                for (int b = 0; b < list[j].abilityGroups[a].skillAbilitys.Count; b++)
//                {
//                    list[j].abilityGroups[a].skillAbilitys[b].AddType();

//                    FileClean(list[j].abilityGroups[a].skillAbilitys[b].subAbilitys);
//                }
//            }
//        }
//    }

//    void FileClean(List<SkillAbilityInfo> list)
//    {
//        for (int b = 0; b < list.Count; b++)
//        {
//            list[b].AddType();
//            FileClean(list[b].subAbilitys);
//        }
//    }

//    void UnitChangeState()
//    {
//        MainUnit.GetComponent<Unit>().ChangeState(UnitState.Attack);
//    }

//    #endregion

//    #region 유닛 그룹 리스트
//    void UnitGroupList()
//    {
//        rect[0] = new Rect(5, MainMenuHeight, 140, MainMenuHeightSize);
//        GUI.Window(0, rect[0], _UnitGroupList, "UnitGroup");
//    }

//    Vector2 UnitGroupScrollPosition;
//    void _UnitGroupList(int id)
//    {
//        float height = 25;
//        bool TouchCheck = false;

//        UnitGroupScrollPosition = GUI.BeginScrollView(new Rect(0, height, 140, MainMenuHeightSize), UnitGroupScrollPosition, new Rect(0, 0, 0, (UnitGroupDic.Count + 1) * 35));

//        height = 0;
//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            UnitGroupInfo Value = UnitGroupDic[i];
//            if (GUI.Button(new Rect(5, height, 115, 30), Value.unitIdx + "(" + Value.skillGroups.Count.ToString() + ")",
//                GetStyle(SelectUnitGroup == Value)))
//            {
//                TouchCheck = true;

//                //< 혹시 수정해놓고 다른거 누를수도있으므로 저장해줌..
//                DataSave();

//                if (!ShowPopupCheck)
//                {
//                    SelectUnitGroup = Value;

//                    //< 없을수가없음... 없으면 문제임
//                    SelectSkillGroup = SelectUnitGroup.skillGroups[0];

//                    //< 초기화및 값 대입
//                    ResetUnitGroup(SelectUnitGroup);

//                    //< 선택했을수도있기때문에 해제
//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;

//                    //< 시스템 메뉴 처리
//                    SystemMemuPopup(Value, null, null, null);
//                }
//            }
//            height += 35;
//        }
//        GUI.EndScrollView();

//        //< 마우스 오른쪽을 눌렀는지 검사
//        if (!TouchCheck)
//        {
//            if (Input.GetMouseButtonUp(1) && rect[0].Contains(new Vector2(Input.mousePosition.x, 720 - Input.mousePosition.y)))
//                SystemMemuPopup(SystemData.eSystemDataType.UnitGroupInfo);
//        }
//    }

//    void AddUnitGroupList(int idx)
//    {
//        UnitGroupInfo groupdata = null;
//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            if (UnitGroupDic[i].unitIdx == idx)
//            {
//                groupdata = UnitGroupDic[i];
//                break;
//            }
//        }

//        if (groupdata != null)
//        {
//            ShowPopup("유닛그룹 추가에 실패하였습니다.", eErrorCode.NotAddUnitGroup);
//        }
//        else
//        {
//            UnitGroupInfo data = new UnitGroupInfo();
//            data.unitIdx = idx;
//            UnitGroupDic.Add(data);
//            SelectUnitGroup = data;

//            //< 스킬 그룹을 하나 추가해줌
//            AddSkillGroup();
//            AddNormalGroup();
//        }
//    }

//    void SaveUnitGroup()
//    {
//        //SaveSkillGroup();
//    }

//    void ResetUnitGroup(UnitGroupInfo info)
//    {
//        if (info == null)
//            return;

//        ResetSkillGroup(SelectSkillGroup);
//    }

//    void SeachUnitGroup(int idx)
//    {
//        UnitGroupInfo groupdata = null;
//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            if (UnitGroupDic[i].unitIdx == idx)
//            {
//                groupdata = UnitGroupDic[i];
//                break;
//            }
//        }

//        if (groupdata == null)
//            ShowPopup("해당 키의 유닛그룹이 없습니다.", eErrorCode.None);
//        else
//        {
//            DataSave();

//            SelectUnitGroup = UnitGroupDic[idx];
//            SelectSkillGroup = SelectUnitGroup.skillGroups[0];

//            //< 초기화및 값 대입
//            ResetUnitGroup(SelectUnitGroup);

//            //< 선택했을수도있기때문에 해제
//            SelectAbilityGroup = null;
//            SelectAbilityInfo = null;
//        }
//    }

//    public void DeleteUnitGroup()
//    {
//        if (SelectUnitGroup == null)
//            return;

//        for (int i = 0; i < UnitGroupDic.Count; i++)
//        {
//            if (UnitGroupDic[i] == SelectUnitGroup)
//            {
//                UnitGroupDic.RemoveAt(i);

//                SelectUnitGroup = null;
//                SelectSkillGroup = null;
//                SelectNormalGroup = null;
//                SelectAbilityGroup = null;
//                SelectAbilityInfo = null;
//                break;
//            }
//        }
//    }
//    #endregion

//    #region 스킬 그룹
//    Dictionary<string, SaveValue> SkillGroupDataInputStr = new Dictionary<string, SaveValue>();
//    void SkillGroupData()
//    {
//        rect[1] = new Rect(rect[0].max.x + 10, MainMenuHeight, 100, 210);
//        GUI.Window(8, rect[1], _NormalGroupData, "Normal Group");

//        rect[5] = new Rect(rect[1].max.x, MainMenuHeight, 100, 210);
//        GUI.Window(1, rect[5], _SkillGroupData, "Skill Group");
//    }

//    void _NormalGroupData(int id)
//    {
//        if (SelectUnitGroup == null)
//            return;

//        float height = 25;

//        //<=============================================
//        //<         좌측 평타 그룹 리스트
//        //<=============================================
//        //< 리스트 출력
//        bool TouchCheck = false;
//        for (int i = 0; i < SelectUnitGroup.normalGroups.Count; i++)
//        {
//            string str = string.Format("{0}({1})", SelectUnitGroup.normalGroups[i].skillIdx, SelectUnitGroup.normalGroups[i].abilityGroups.Count.ToString());
//            if (GUI.Button(new Rect(5, height, 90, 30), str,
//                GetStyle(SelectNormalGroup == SelectUnitGroup.normalGroups[i])))
//            {
//                TouchCheck = true;

//                //< 이전거는 친절하게 저장해줌
//                DataSave();

//                if (!ShowPopupCheck)
//                {
//                    SelectSkillGroup = SelectUnitGroup.normalGroups[i];
//                    SelectNormalGroup = SelectSkillGroup;

//                    //< 기존 데이터 삭제및 대입
//                    ResetSkillGroup(SelectSkillGroup, true);

//                    //< 선택했을수도있기때문에 해제
//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;

//                    //< 시스템 메뉴 처리
//                    SystemMemuPopup(null, SelectUnitGroup.normalGroups[i], null, null);
//                }
//            }
//            height += 35;
//        }

//        //< 마우스 오른쪽을 눌렀는지 검사
//        if (!TouchCheck)
//        {
//            if (Input.GetMouseButtonUp(1) && rect[1].Contains(new Vector2(Input.mousePosition.x, 720 - Input.mousePosition.y)))
//                SystemMemuPopup(SystemData.eSystemDataType.NormalGroupInfo);
//        }

//        //< 그룹 추가 버튼
//        if (GUI.Button(new Rect(5, 175, 90, 30), "추가"))
//            AddNormalGroup();
//    }

//    void _SkillGroupData(int id)
//    {
//        if (SelectUnitGroup == null)
//            return;

//        //<=============================================
//        //<         우측 스킬 그룹 리스트
//        //<=============================================
//        float height = 25;

//        //< 리스트 출력
//        bool TouchCheck = false;
//        for (int i = 0; i < SelectUnitGroup.skillGroups.Count; i++)
//        {
//            string str = string.Format("{0}({1},{2}){3}", SelectUnitGroup.skillGroups[i].skillIdx, SelectUnitGroup.skillGroups[i].abilityGroups.Count.ToString(), SelectUnitGroup.skillGroups[i].maxSkillLevel, SelectUnitGroup.skillGroups[i].passive ? "_P" : "");
//            if (GUI.Button(new Rect(5, height, 90, 30), str,
//                GetStyle(SelectSkillGroup == SelectUnitGroup.skillGroups[i])))
//            {
//                TouchCheck = true;

//                //< 이전거는 친절하게 저장해줌
//                DataSave();

//                if (!ShowPopupCheck)
//                {
//                    SelectSkillGroup = SelectUnitGroup.skillGroups[i];
//                    SelectNormalGroup = null;

//                    //< 기존 데이터 삭제및 대입
//                    ResetSkillGroup(SelectSkillGroup);

//                    //< 선택했을수도있기때문에 해제
//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;

//                    //< 시스템 메뉴 처리
//                    SystemMemuPopup(null, null, SelectUnitGroup.skillGroups[i], null);
//                }
//            }
//            height += 35;
//        }

//        //< 마우스 오른쪽을 눌렀는지 검사
//        if (!TouchCheck)
//        {
//            if (Input.GetMouseButtonUp(1) && rect[5].Contains(new Vector2(Input.mousePosition.x, 720 - Input.mousePosition.y)))
//                SystemMemuPopup(SystemData.eSystemDataType.SkillGroupInfo);
//        }


//        //< 그룹 추가 버튼
//        if (GUI.Button(new Rect(5, 175, 90, 30), "추가"))
//            AddSkillGroup();
//    }

//    void AddSkillGroup()
//    {
//        if (SelectUnitGroup == null)
//            return;

//        if (SelectUnitGroup.skillGroups.Count >= 4)
//            ShowPopup("스킬 그룹 추가 실패", eErrorCode.NotAddSkillGroup_MaxCount);
//        else
//        {
//            //< 기존거 일단 저장해줌
//            DataSave();

//            SkillGroupInfo info = new SkillGroupInfo();
//            info.skillIdx = SelectUnitGroup.skillGroups.Count + 1;
//            SelectUnitGroup.skillGroups.Add(info);
//            SelectSkillGroup = info;

//            //< 데이터 대입
//            ResetSkillGroup(SelectSkillGroup);

//            //< 어빌리티 그룹도 추가해줌
//            AddAbilityGroup(true);

//            //< 개수 갱신
//            SelectUnitGroup.skillCount = SelectUnitGroup.skillGroups.Count;
//        }
//    }

//    void SaveSkillGroup()
//    {
//        if (SelectSkillGroup == null)
//            return;

//        if (SkillGroupDataInputStr.Count == 0)
//            ResetSkillGroup(new SkillGroupInfo());

//        //< 스킬 정보 저장
//        if (SkillGroupDataInputStr.ContainsKey("스킬이름(인덱스)"))
//            SelectSkillGroup.skillName = TryParse_ui(SkillGroupDataInputStr, "스킬이름(인덱스)");
//        if (SkillGroupDataInputStr.ContainsKey("스킬설명(인덱스)"))
//            SelectSkillGroup.descrpition = TryParse_ui(SkillGroupDataInputStr, "스킬설명(인덱스)");
//        if (SkillGroupDataInputStr.ContainsKey("스킬아이콘명"))
//            SelectSkillGroup.icon = (string)SkillGroupDataInputStr["스킬아이콘명"].data;
//        if (SkillGroupDataInputStr.ContainsKey("스킬 쿨다운"))
//            SelectSkillGroup.cooldown = TryParse_us(SkillGroupDataInputStr, "스킬 쿨다운");
//        if (SkillGroupDataInputStr.ContainsKey("시전 최소 사거리"))
//            SelectSkillGroup.range = TryParse_f(SkillGroupDataInputStr, "시전 최소 사거리");
//        if (SkillGroupDataInputStr.ContainsKey("애니메이션명"))
//            SelectSkillGroup.aniName = (string)SkillGroupDataInputStr["애니메이션명"].data;
//        if (SkillGroupDataInputStr.ContainsKey("시전 이펙트명"))
//            SelectSkillGroup.effect = (string)SkillGroupDataInputStr["시전 이펙트명"].data;
//        if (SkillGroupDataInputStr.ContainsKey("이펙트 어빌리티"))
//            SelectSkillGroup.EffectAbilityIdx = TryParse_i(SkillGroupDataInputStr, "이펙트 어빌리티");

//        if (SkillGroupDataInputStr.ContainsKey("타겟팅 유무"))
//            SelectSkillGroup.needTarget = (bool)SkillGroupDataInputStr["타겟팅 유무"].data;
//        if (SkillGroupDataInputStr.ContainsKey("패시브 유무"))
//            SelectSkillGroup.passive = (bool)SkillGroupDataInputStr["패시브 유무"].data;

//        if (SkillLevelDataStr.Count == 0)
//            ResetSkillGroup(SelectSkillGroup);

//        //< 스킬 레벨 정보 저장
//        SelectSkillGroup.skillLevels.Clear();
//        for (int i = 0; i < SkillLevelDataStr.Count; i++)
//        {
//            SelectSkillGroup.skillLevels.Add(new List<SkillLevelInfo>());
//            for (int j = 0; j < SkillLevelDataStr[i].Count; j++)
//            {
//                SkillLevelInfo data = new SkillLevelInfo();
//                data.level = (byte)(i + 1);
//                data.skillType = TryParse_b(SkillLevelDataStr[i][j], "skillType");
//                data.infoIdx = TryParse_ui(SkillLevelDataStr[i][j], "infoIdx");
//                data.levelDataType = TryParse_b(SkillLevelDataStr[i][j], "levelDataType");
//                data.dataValue = TryParse_f(SkillLevelDataStr[i][j], "dataValue");

//                SelectSkillGroup.skillLevels[i].Add(data);
//            }

//        }
//        SelectSkillGroup.maxSkillLevel = (byte)(SelectSkillGroup.skillLevels.Count);
//    }

//    void AddNormalGroup()
//    {
//        if (SelectUnitGroup == null)
//            return;

//        if (SelectUnitGroup.normalGroups.Count >= 4)
//            ShowPopup("노말 그룹 추가 실패", eErrorCode.NotAddSkillGroup_MaxCount);
//        else
//        {
//            //< 기존거 일단 저장해줌
//            DataSave();

//            SkillGroupInfo info = new SkillGroupInfo();
//            info.skillIdx = SelectUnitGroup.normalGroups.Count + 1;
//            SelectUnitGroup.normalGroups.Add(info);
//            SelectSkillGroup = info;

//            //< 데이터 대입
//            ResetSkillGroup(SelectSkillGroup, true);

//            //< 어빌리티 그룹도 추가해줌
//            AddAbilityGroup(true);
//        }
//    }

//    void ResetSkillGroup(SkillGroupInfo info, bool normal = false)
//    {
//        if (info == null)
//            return;

//        //< 스킬 정보 대입
//        SkillGroupDataInputStr.Clear();

//        if (!normal)
//        {
//            SkillGroupDataInputStr.Add("스킬이름(인덱스)", new SaveValue(info.skillName.ToString()));
//            SkillGroupDataInputStr.Add("스킬설명(인덱스)", new SaveValue(info.descrpition.ToString()));
//            SkillGroupDataInputStr.Add("스킬아이콘명", new SaveValue(info.icon));
//            SkillGroupDataInputStr.Add("스킬 쿨다운", new SaveValue(info.cooldown.ToString()));
//            SkillGroupDataInputStr.Add("시전 최소 사거리", new SaveValue(info.range.ToString()));
//            SkillGroupDataInputStr.Add("애니메이션명", new SaveValue(info.aniName));
//            SkillGroupDataInputStr.Add("시전 이펙트명", new SaveValue(info.effect));
//            SkillGroupDataInputStr.Add("이펙트 어빌리티", new SaveValue(info.EffectAbilityIdx.ToString()));

//            SkillGroupDataInputStr.Add("타겟팅 유무", new SaveValue(info.needTarget));
//            SkillGroupDataInputStr.Add("패시브 유무", new SaveValue(info.passive));
//        }
//        else
//        {
//            SkillGroupDataInputStr.Add("스킬아이콘명", new SaveValue(info.icon));
//            SkillGroupDataInputStr.Add("애니메이션명", new SaveValue(info.aniName));
//            SkillGroupDataInputStr.Add("시전 이펙트명", new SaveValue(info.effect));
//        }

//        //< 스킬 레벨 정보 대입
//        SkillLevelDataStr.Clear();

//        for (int i = 0; i < info.skillLevels.Count; i++)
//        {
//            SkillLevelDataStr.Add(new List<Dictionary<string, SaveValue>>());
//            for (int j = 0; j < info.skillLevels[i].Count; j++)
//            {
//                SkillLevelDataStr[i].Add(new Dictionary<string, SaveValue>());
//                SkillLevelDataStr[i][j].Add("level", new SaveValue(info.skillLevels[i][j].level.ToString()));
//                SkillLevelDataStr[i][j].Add("infoIdx", new SaveValue(info.skillLevels[i][j].infoIdx.ToString()));
//                SkillLevelDataStr[i][j].Add("skillType", new SaveValue(info.skillLevels[i][j].skillType.ToString()));
//                SkillLevelDataStr[i][j].Add("levelDataType", new SaveValue(info.skillLevels[i][j].levelDataType.ToString()));
//                SkillLevelDataStr[i][j].Add("dataValue", new SaveValue(info.skillLevels[i][j].dataValue.ToString()));
//            }
//        }

//        //< 어빌리티 데이터 정보도 초기화
//        //ResetDataValue();
//    }

//    public void DeleteSkillGroupData()
//    {
//        if (SelectUnitGroup.skillGroups.Count == 1)
//        {
//            ShowPopup("스킬그룹 삭제에 실패하였습니다.", eErrorCode.NotDeleteSkillGroup_MinCount);
//        }
//        else
//        {
//            for (int i = 0; i < SelectUnitGroup.skillGroups.Count; i++)
//            {
//                if (SelectUnitGroup.skillGroups[i] == SelectSkillGroup)
//                {
//                    SelectUnitGroup.skillGroups.RemoveAt(i);

//                    SelectSkillGroup = null;
//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;
//                    break;
//                }
//            }

//            //< 기존 스킬 그룹 인덱스 변경
//            for (int i = 0; i < SelectUnitGroup.skillGroups.Count; i++)
//                SelectUnitGroup.skillGroups[i].skillIdx = (int)(i + 1);

//            //< 개수 갱신
//            SelectUnitGroup.skillCount = SelectUnitGroup.skillGroups.Count;
//        }
//    }

//    public void DeleteNormalGroupData()
//    {
//        if (SelectUnitGroup.normalGroups.Count == 1)
//        {
//            ShowPopup("그룹 삭제에 실패하였습니다.", eErrorCode.NotDeleteSkillGroup_MinCount);
//        }
//        else
//        {
//            for (int i = 0; i < SelectUnitGroup.normalGroups.Count; i++)
//            {
//                if (SelectUnitGroup.normalGroups[i] == SelectSkillGroup)
//                {
//                    SelectUnitGroup.normalGroups.RemoveAt(i);

//                    SelectSkillGroup = null;
//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;
//                    break;
//                }
//            }

//            //< 기존그룹 인덱스 변경
//            for (int i = 0; i < SelectUnitGroup.normalGroups.Count; i++)
//                SelectUnitGroup.normalGroups[i].skillIdx = (int)(i + 1);
//        }
//    }
//    #endregion

//    #region 어빌리티 그룹
//    Dictionary<string, SaveValue> AbilityDataInputStr = new Dictionary<string, SaveValue>();
//    public GUIStyle NormalGuiStyle, ChoiceGuiStyle;
//    void AbilityGroupData()
//    {
//        rect[2] = new Rect(rect[0].max.x + 10, 300, 200, 400);
//        GUI.Window(2, rect[2], _AbilityGroupData, "Ability Group");
//    }

//    void _AbilityGroupData(int id)
//    {
//        if (SelectSkillGroup == null)
//            return;

//        float height = 25;
//        bool TouchCheck = false;
//        for (int i = 0; i < SelectSkillGroup.abilityGroups.Count; i++)
//        {
//            if (GUI.Button(new Rect(5, height, 190, 30), SelectSkillGroup.abilityGroups[i].abilityIdx.ToString(), GetStyle(SelectAbilityGroup == SelectSkillGroup.abilityGroups[i])))
//            {
//                TouchCheck = true;

//                //< 이전거는 친절하게 저장해줌
//                DataSave();

//                if (!ShowPopupCheck)
//                {
//                    //< 새로 클릭한 값으로 대입
//                    SelectAbilityGroup = SelectSkillGroup.abilityGroups[i];
//                    SelectAbilityInfo = null;

//                    //< 데이터 초기화
//                    ResetAbilityDataValue(SelectAbilityGroup);

//                    //< 시스템 메뉴 처리
//                    SystemMemuPopup(null, null, null, SelectSkillGroup.abilityGroups[i]);
//                }
//            }

//            height += 35;
//        }

//        //< 마우스 오른쪽을 눌렀는지 검사
//        if (!TouchCheck)
//        {
//            if (Input.GetMouseButtonUp(1) && rect[2].Contains(new Vector2(Input.mousePosition.x, 720 - Input.mousePosition.y)))
//                SystemMemuPopup(SystemData.eSystemDataType.AbilityGroup);
//        }

//        //< 어빌리티 그룹 추가 버튼
//        if (GUI.Button(new Rect(5, 355, 190, 40), "어빌리티그룹 추가"))
//            AddAbilityGroup();
//    }

//    void AddAbilityGroup(bool NotSelect = false)
//    {
//        if (SelectSkillGroup == null)
//            return;

//        if (SelectSkillGroup.abilityGroups.Count > 8)
//        {
//            ShowPopup("어빌리티 추가에 실패하였습니다.", eErrorCode.NotAddAbility_MaxCount);
//        }
//        else
//        {
//            //< 이전거는 친절하게 저장해줌
//            SaveAbilityGroupData();

//            if (!ShowPopupCheck)
//            {
//                SelectSkillGroup.abilityGroups.Add(new AbilityGroup());

//                SelectAbilityGroup = SelectSkillGroup.abilityGroups[SelectSkillGroup.abilityGroups.Count - 1];
//                ResetAbilityDataValue(SelectAbilityGroup);

//                //< 어빌리티도 추가해줌
//                AddAbility();

//                if (NotSelect)
//                    SelectAbilityGroup = null;
//            }
//        }
//    }

//    void ResetAbilityDataValue(AbilityGroup info)
//    {
//        if (info == null)
//            return;

//        AbilityDataInputStr.Clear();
//        AbilityDataInputStr.Add("노티 인덱스", new SaveValue(info.abilityIdx.ToString()));
//        AbilityDataInputStr.Add("시전 이펙트명", new SaveValue(info.effect.ToString()));
//        AbilityDataInputStr.Add("부모에게 귀속", new SaveValue(info.child));
//    }

//    void SaveAbilityGroupData()
//    {
//        if (SelectAbilityGroup == null)
//            return;

//        if (AbilityDataInputStr.Count == 0)
//            ResetAbilityDataValue(new AbilityGroup());

//        //< 스킬 정보 저장
//        SelectAbilityGroup.abilityIdx = TryParse_ui(AbilityDataInputStr, "노티 인덱스");
//        SelectAbilityGroup.effect = (string)AbilityDataInputStr["시전 이펙트명"].data;
//        SelectAbilityGroup.child = (bool)AbilityDataInputStr["부모에게 귀속"].data;
//    }

//    public void DeleteAbilityGroupData()
//    {
//        if (SelectSkillGroup.abilityGroups.Count == 1)
//        {
//            ShowPopup("어빌리티그룹 삭제에 실패하였습니다.", eErrorCode.NotDeleteAbilityGroup_MinCount);
//        }
//        else
//        {
//            for (int i = 0; i < SelectSkillGroup.abilityGroups.Count; i++)
//            {
//                if (SelectSkillGroup.abilityGroups[i] == SelectAbilityGroup)
//                {
//                    SelectSkillGroup.abilityGroups.RemoveAt(i);

//                    SelectAbilityGroup = null;
//                    SelectAbilityInfo = null;
//                    break;
//                }
//            }

//        }
//    }
//    #endregion

//    #region 어빌리티 정보
//    void AbilityEdit()
//    {
//        rect[3] = new Rect(rect[2].max.x + 10, MainMenuHeight, 400, MainMenuHeightSize);
//        GUI.Window(3, rect[3], _AbilityEdit, "Ability");
//    }

//    void _AbilityEdit(int id)
//    {
//        if (SelectSkillGroup == null || SelectAbilityGroup == null)
//            return;

//        float Height = 25;
//        for (int i = 0; i < SelectAbilityGroup.skillAbilitys.Count; i++)
//        {
//            //Height = NextHeight;
//            if (GUI.Button(new Rect(5, Height, 80, 30), SkillType[SelectAbilityGroup.skillAbilitys[i].skillType], GetStyle(SelectAbilityInfo == SelectAbilityGroup.skillAbilitys[i], SelectAbilityGroup.skillAbilitys[i].skillType)))
//            {
//                //< 기존값을 저장시켜줌
//                DataSave();

//                if (!ShowPopupCheck)
//                {
//                    //< 대입
//                    SelectAbilityInfo = SelectAbilityGroup.skillAbilitys[i];

//                    //< 저장되있는 어빌리티 데이터를 리셋
//                    ResetDataValue();
//                }
//            }

//            if (!subAbilitysView(SelectAbilityGroup.skillAbilitys[i], ref Height, 1))
//                Height += 35;
//        }

//        //< 어빌리티 추가 버튼
//        if (GUI.Button(new Rect(5, MainMenuHeightSize - 45, 390, 40), "신규 어빌리티 기능 추가"))
//            AddAbility();
//    }

//    //< 아래로 뿌리내려지는 어빌리티 랜더
//    bool subAbilitysView(SkillAbilityInfo info, ref float Height, int depth)
//    {
//        if (info == null)
//            return false;

//        bool AddCheck = false;
//        for (int j = 0; j < info.subAbilitys.Count; j++)
//        {
//            if (GUI.Button(new Rect(5 + depth * 100, Height, 80, 30), SkillType[info.subAbilitys[j].skillType], GetStyle(SelectAbilityInfo == info.subAbilitys[j], info.subAbilitys[j].skillType)))
//            {
//                //< 기존값을 저장시켜줌
//                DataSave();

//                SelectAbilityInfo = info.subAbilitys[j];

//                //< 저장되있는 어빌리티 데이터를 리셋
//                ResetDataValue();

//            }

//            AddCheck = true;

//            if (!subAbilitysView(info.subAbilitys[j], ref Height, depth + 1))
//                Height += 35;
//        }

//        return AddCheck;
//    }

//    void AddAbility()
//    {
//        if (SelectAbilityGroup == null)
//            return;

//        SkillAbilityInfo data = new SkillAbilityInfo();
//        SelectAbilityGroup.skillAbilitys.Add(data);
//    }

//    void DeleteAbility()
//    {
//        if (SelectAbilityInfo == null)
//            return;

//        DeleteSubAbility(SelectAbilityGroup.skillAbilitys);
//    }

//    bool DeleteSubAbility(List<SkillAbilityInfo> list)
//    {
//        for (int i = 0; i < list.Count; i++)
//        {
//            if (list[i] == SelectAbilityInfo)
//            {
//                list.RemoveAt(i);
//                SelectAbilityInfo = null;
//                return true;
//            }
//            else if (list[i].subAbilitys.Count > 0)
//            {
//                if (DeleteSubAbility(list[i].subAbilitys))
//                    break;
//            }
//        }

//        return false;
//    }

//    //< 어빌리티를 추가해준다(서브어빌리티)
//    void AddsubAbilitys(int count, SkillAbilityInfo ability)
//    {
//        if (ability.subAbilitys == null)
//            ability.subAbilitys = new List<SkillAbilityInfo>();

//        if (ability.subAbilitys.Count < count)
//        {
//            for (int i = ability.subAbilitys.Count; i < count; i++)
//            {
//                SkillAbilityInfo data = new SkillAbilityInfo();
//                ability.subAbilitys.Add(data);
//            }
//        }
//        else if (ability.subAbilitys.Count > count)
//        {
//            for (int i = ability.subAbilitys.Count; i > count; i--)
//                ability.subAbilitys.RemoveAt(ability.subAbilitys.Count - 1);
//        }
//    }

//    void ResetDataValue()
//    {
//        //< 혹시 데이터가있을경우 대입
//        if (SelectAbilityInfo != null)
//        {
//            CopyDataViewStr(SelectAbilityInfo);
//        }
//        else
//        {
//            CopyDataViewStr(new SkillAbilityInfo());
//        }
//    }

//    void CopyDataViewStr(SkillAbilityInfo info)
//    {
//        DataViewStr.Clear();

//        info.AddType();

//        DataViewStr.Add("스킬 종류", new SaveValue(info.skillType.ToString(), ValueType.All, SkillType));
//        DataViewStr.Add("타겟 타입", new SaveValue(info.applyTarget.ToString(), ValueType.Common, ApplyTarget));
//        DataViewStr.Add("타겟 타입(세부)", new SaveValue(info.applyTargetType.ToString(), ValueType.Common, ApplyTargetType));
//        DataViewStr.Add("타겟 인원수", new SaveValue(info.targetLimit.ToString(), ValueType.Common));

//        //Debug.Log((eSkillType)info.skillType + " , " + GetBuffType(info.skillType));

//        //< 발사체
//        if (info.skillType == 2)
//        {
//            DataViewStr.Add("범위", new SaveValue(info.radius.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("프리팹명", new SaveValue(info.skillProjectTile.prefab, ValueType.ProjectTile));
//            DataViewStr.Add("충돌시 이펙트명", new SaveValue(info.skillProjectTile.colideEffect, ValueType.ProjectTile));
//            DataViewStr.Add("이펙트 귀속", new SaveValue(info.skillProjectTile.effectchild, ValueType.ProjectTile));
//            DataViewStr.Add("충돌시 사운드명", new SaveValue(info.skillProjectTile.colideSound, ValueType.ProjectTile));
//            DataViewStr.Add("이동 속도", new SaveValue(info.skillProjectTile.moveSpeed.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("최대이동거리", new SaveValue(info.skillProjectTile.maxDistance.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("관통 유무", new SaveValue(info.skillProjectTile.penetrate, ValueType.ProjectTile));
//            DataViewStr.Add("처음발사개수(멀티)", new SaveValue(info.skillProjectTile.multi.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("발사 각도", new SaveValue(info.skillProjectTile.startAngle.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("체인 카운트", new SaveValue(info.skillProjectTile.hit.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("충돌시 나뉠개수", new SaveValue(info.skillProjectTile.div.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("발사 초기 위치", new SaveValue(info.skillProjectTile.startPosType.ToString(), ValueType.ProjectTile));
//            DataViewStr.Add("발사 위치 파일명", new SaveValue(info.skillProjectTile.castDummyName, ValueType.ProjectTile));
//            DataViewStr.Add("호출 어빌인덱스", new SaveValue(info.subAbilitys.Count.ToString(), ValueType.ProjectTile));
//        }

//        //< 버프
//        else if (GetBuffType(info.skillType))
//        {
//            DataViewStr.Add("범위", new SaveValue(info.radius.ToString(), ValueType.Normal));

//            DataViewStr.Add("baseFactor", new SaveValue(info.baseFactor.ToString(), ValueType.Buff, BaseFactor));
//            DataViewStr.Add("factorRate", new SaveValue(info.factorRate.ToString(), ValueType.Buff));
//            DataViewStr.Add("basicValue", new SaveValue(info.basicValue.ToString(), ValueType.Buff));

//            DataViewStr.Add("아이콘명", new SaveValue(info.skillBuff.iconSprite, ValueType.Buff));
//            DataViewStr.Add("이펙트명", new SaveValue(info.skillBuff.effect, ValueType.Buff));
//            DataViewStr.Add("이펙트 출력위치", new SaveValue(info.skillBuff.effectpos.ToString(), ValueType.Buff));
//            DataViewStr.Add("버프 종류", new SaveValue(info.skillBuff.buffType.ToString(), ValueType.Buff));
//            DataViewStr.Add("최대 중첩", new SaveValue(info.skillBuff.overlapcount.ToString(), ValueType.Buff));
//            DataViewStr.Add("유지 시간", new SaveValue(info.skillBuff.durationTime.ToString(), ValueType.Buff));
//            DataViewStr.Add("반복 시간", new SaveValue(info.skillBuff.tic.ToString(), ValueType.Buff));
//        }

//        //< 기본어빌리티
//        else if (info.skillType != 0)
//        {
//            DataViewStr.Add("범위", new SaveValue(info.radius.ToString(), ValueType.Normal));
//            DataViewStr.Add("각도", new SaveValue(info.skillNormal.angle.ToString(), ValueType.Normal));

//            DataViewStr.Add("baseFactor", new SaveValue(info.baseFactor.ToString(), ValueType.Normal, BaseFactor));
//            DataViewStr.Add("factorRate", new SaveValue(info.factorRate.ToString(), ValueType.Normal));
//            DataViewStr.Add("basicValue", new SaveValue(info.basicValue.ToString(), ValueType.Normal));

//            DataViewStr.Add("파워", new SaveValue(info.skillNormal.power.ToString(), ValueType.Normal));

//            DataViewStr.Add("방어 무시 여부", new SaveValue(info.skillNormal.ignoreDef, ValueType.Normal));
//            DataViewStr.Add("시전시 무적 여부", new SaveValue(info.skillNormal.immune, ValueType.Normal));

//            DataViewStr.Add("이벤트종류", new SaveValue(info.skillNormal.eventType.ToString(), ValueType.Normal));
//            DataViewStr.Add("이벤트 수치", new SaveValue(info.skillNormal.eventSub.ToString(), ValueType.Normal));
//            DataViewStr.Add("스킬오브젝트명", new SaveValue(info.skillNormal.skillObject, ValueType.Normal));
//            DataViewStr.Add("스킬오브젝트 수치", new SaveValue(info.skillNormal.skillObjectValue, ValueType.Normal));
//            DataViewStr.Add("호출 어빌인덱스", new SaveValue(info.subAbilitys.Count.ToString(), ValueType.Normal));
//        }
//        else if (info.skillType == 0)
//            DataViewStr.Add("호출 어빌인덱스", new SaveValue(info.subAbilitys.Count.ToString(), ValueType.MultiTasking));
//    }

//    //< 데이터를 저장해준다
//    void SaveDataValue()
//    {
//        if (SelectAbilityInfo == null)
//            return;

//        if (DataViewStr.Count == 0)
//            ResetDataValue();

//        SelectAbilityInfo.skillType = TryParse_b(DataViewStr, "스킬 종류");
//        SelectAbilityInfo.applyTarget = TryParse_b(DataViewStr, "타겟 타입");
//        SelectAbilityInfo.targetLimit = TryParse_b(DataViewStr, "타겟 인원수");
//        SelectAbilityInfo.applyTargetType = TryParse_b(DataViewStr, "타겟 타입(세부)");

//        SelectAbilityInfo.AddType();

//        //< 발사체
//        if (SelectAbilityInfo.skillType == 2)
//        {
//            SelectAbilityInfo.radius = TryParse_f(DataViewStr, "범위");
//            (SelectAbilityInfo.skillProjectTile).prefab = (string)DataViewStr["프리팹명"].data;
//            (SelectAbilityInfo.skillProjectTile).colideEffect = (string)DataViewStr["충돌시 이펙트명"].data;
//            (SelectAbilityInfo.skillProjectTile).effectchild = (bool)DataViewStr["이펙트 귀속"].data;
//            (SelectAbilityInfo.skillProjectTile).colideSound = (string)DataViewStr["충돌시 사운드명"].data;
//            (SelectAbilityInfo.skillProjectTile).moveSpeed = TryParse_f(DataViewStr, "이동 속도");
//            (SelectAbilityInfo.skillProjectTile).maxDistance = TryParse_f(DataViewStr, "최대이동거리");
//            (SelectAbilityInfo.skillProjectTile).penetrate = (bool)DataViewStr["관통 유무"].data;
//            (SelectAbilityInfo.skillProjectTile).multi = TryParse_b(DataViewStr, "처음발사개수(멀티)");
//            (SelectAbilityInfo.skillProjectTile).startAngle = TryParse_f(DataViewStr, "발사 각도");
//            (SelectAbilityInfo.skillProjectTile).hit = TryParse_b(DataViewStr, "체인 카운트");
//            (SelectAbilityInfo.skillProjectTile).div = TryParse_b(DataViewStr, "충돌시 나뉠개수");
//            (SelectAbilityInfo.skillProjectTile).startPosType = TryParse_b(DataViewStr, "발사 초기 위치");
//            (SelectAbilityInfo.skillProjectTile).castDummyName = (string)DataViewStr["발사 위치 파일명"].data;
//        }
//        //< 버프
//        else if (GetBuffType(SelectAbilityInfo.skillType))
//        {
//            SelectAbilityInfo.radius = TryParse_f(DataViewStr, "범위");

//            SelectAbilityInfo.baseFactor = TryParse_b(DataViewStr, "baseFactor");
//            SelectAbilityInfo.factorRate = TryParse_f(DataViewStr, "factorRate");
//            SelectAbilityInfo.basicValue = TryParse_i(DataViewStr, "basicValue");


//            (SelectAbilityInfo.skillBuff).iconSprite = (string)DataViewStr["아이콘명"].data;
//            (SelectAbilityInfo.skillBuff).effect = (string)DataViewStr["이펙트명"].data;
//            (SelectAbilityInfo.skillBuff).effectpos = TryParse_b(DataViewStr, "이펙트 출력위치");
//            (SelectAbilityInfo.skillBuff).buffType = TryParse_i(DataViewStr, "버프 종류");
//            (SelectAbilityInfo.skillBuff).overlapcount = TryParse_b(DataViewStr, "최대 중첩");
//            (SelectAbilityInfo.skillBuff).durationTime = TryParse_f(DataViewStr, "유지 시간");
//            (SelectAbilityInfo.skillBuff).tic = TryParse_f(DataViewStr, "반복 시간");
//        }

//        //< 노말
//        else if (SelectAbilityInfo.skillType != 0)
//        {
//            SelectAbilityInfo.radius = TryParse_f(DataViewStr, "범위");
//            (SelectAbilityInfo.skillNormal).angle = TryParse_f(DataViewStr, "각도");

//            SelectAbilityInfo.baseFactor = TryParse_b(DataViewStr, "baseFactor");
//            SelectAbilityInfo.factorRate = TryParse_f(DataViewStr, "factorRate");
//            SelectAbilityInfo.basicValue = TryParse_i(DataViewStr, "basicValue");


//            (SelectAbilityInfo.skillNormal).eventType = TryParse_b(DataViewStr, "이벤트종류");
//            (SelectAbilityInfo.skillNormal).eventSub = TryParse_f(DataViewStr, "이벤트 수치");
//            (SelectAbilityInfo.skillNormal).skillObject = (string)DataViewStr["스킬오브젝트명"].data;
//            (SelectAbilityInfo.skillNormal).skillObjectValue = (string)DataViewStr["스킬오브젝트 수치"].data;
//            (SelectAbilityInfo.skillNormal).power = TryParse_f(DataViewStr, "파워");
//            (SelectAbilityInfo.skillNormal).ignoreDef = (bool)DataViewStr["방어 무시 여부"].data;
//            (SelectAbilityInfo.skillNormal).immune = (bool)DataViewStr["시전시 무적 여부"].data;
//        }
//    }
//    #endregion

//    #region 우측 상세정보
//    Dictionary<string, SaveValue> DataViewStr = new Dictionary<string, SaveValue>();
//    SaveValue ComboSelectObj;
//    string ComboSelectKey;
//    bool ShowComboBox = false;

//    void DataEdit()
//    {
//        rect[4] = new Rect(rect[3].max.x + 10, MainMenuHeight, 300, MainMenuHeightSize);
//        GUI.Window(4, rect[4], _DataEdit, "Data");
//    }

//    void _DataEdit(int id)
//    {
//        //< 어빌리티를 선택했다면 어빌리티 정보를 표시
//        if (SelectAbilityInfo != null)
//        {
//            //< 타입을 먼저 표시
//            byte idx = TryParse_b(DataViewStr, "스킬 종류");

//            //< 타입에따라 호출되는 함수가 다름

//            if (idx == 0)
//                NormalDataView(new ValueType[] { ValueType.All, ValueType.MultiTasking });
//            else if (idx == 2)
//                NormalDataView(new ValueType[] { ValueType.All, ValueType.Common, ValueType.ProjectTile, ValueType.MultiTasking });
//            else if (GetBuffType(idx))
//            {
//                ////< 카운트다운 버프만 멀티테스킹 추가
//                //if (idx == (byte)eSkillType.Skill_BuffCountDown)
//                //    NormalDataView(new ValueType[] { ValueType.All, ValueType.Common, ValueType.Buff, ValueType.MultiTasking });
//                //else
//                //    NormalDataView(new ValueType[] { ValueType.All, ValueType.Common, ValueType.Buff});
//            }
//            else
//                NormalDataView(new ValueType[] { ValueType.All, ValueType.Common, ValueType.Normal });
//        }
//        else if (SelectAbilityGroup != null)
//            AbilityGroupView();
//        else if (SelectSkillGroup != null)
//            SkillGroupView();
//        else if (SelectUnitGroup != null)
//            UnitGroupView();
//    }

//    //< 유닛그룹 정보 뷰어
//    void UnitGroupView()
//    {

//    }

//    //< 스킬 그룹 정보 뷰어
//    void SkillGroupView()
//    {
//        //<================================
//        //<         스킬 정보 입력 
//        //<================================
//        float height = 25;
//        foreach (KeyValuePair<string, SaveValue> list in SkillGroupDataInputStr)
//        {
//            SaveValue data = list.Value;
//            GUI.Label(new Rect(5, height, 130, 30), list.Key);
//            if (data.data is string)
//                data.data = GUI.TextField(new Rect(120, height, 170, 20), data.data.ToString());
//            else if (data.data is bool)
//                data.data = GUI.Toggle(new Rect(120, height, 170, 20), (bool)data.data, "Check");

//            height += 24;
//        }

//        //< 스킬그룹 삭제 버튼
//        if (GUI.Button(new Rect(5, MainMenuHeightSize - 45, 290, 40), "선택한 그룹 삭제"))
//        {
//            if (SelectNormalGroup != null)
//                DeleteNormalGroupData();
//            else
//                DeleteSkillGroupData();
//        }
//    }

//    //< 어빌리티 그룹 정보 뷰어
//    void AbilityGroupView()
//    {
//        //<================================
//        //<         그룹 정보 입력 
//        //<================================
//        float height = 25;
//        foreach (KeyValuePair<string, SaveValue> list in AbilityDataInputStr)
//        {
//            SaveValue data = list.Value;
//            GUI.Label(new Rect(5, height, 130, 30), list.Key);
//            if (data.data is string)
//                data.data = GUI.TextField(new Rect(120, height, 170, 20), data.data.ToString());
//            else if (data.data is bool)
//                data.data = GUI.Toggle(new Rect(120, height, 170, 20), (bool)data.data, "Check");

//            height += 26;
//        }

//        //< 어빌리티 데이터 삭제 버튼
//        if (GUI.Button(new Rect(5, MainMenuHeightSize - 45, 290, 40), "선택한 어빌리티그룹 삭제"))
//            DeleteAbilityGroupData();
//    }

//    //< 일반 어빌리티 정보 뷰어
//    void NormalDataView(ValueType[] Successlist)
//    {
//        float height = 25;
//        foreach (KeyValuePair<string, SaveValue> list in DataViewStr)
//        {
//            for (int i = 0; i < Successlist.Length; i++)
//            {
//                if (Successlist[i] == list.Value.type)
//                {
//                    SaveValue data = list.Value;
//                    GUI.Label(new Rect(5, height, 130, 30), list.Key);
//                    if (data.data is string)
//                    {
//                        if (data.SelectComboItem != null)
//                        {
//                            int idx = int.Parse(data.data.ToString());
//                            if (GUI.Button(new Rect(120, height, 170, 20), data.SelectComboItem[idx].ToString()))
//                            {
//                                ChoiceIdx = idx;
//                                ShowComboBox = true;
//                                ComboSelectObj = list.Value;
//                                ComboSelectKey = list.Key;
//                            }
//                        }
//                        else
//                            data.data = GUI.TextField(new Rect(120, height, 170, 20), data.data.ToString());

//                        //< 예외처리를 해줘야함
//                        if (list.Key == "호출 어빌인덱스")
//                        {
//                            int value = 0;
//                            if (int.TryParse((string)data.data, out value))
//                            {
//                                if (value > 3)
//                                    data.data = "3";
//                                else if (value < 0)
//                                    data.data = "0";

//                                //< 값을 추가해줌
//                                if (value >= 0)
//                                    AddsubAbilitys(value, SelectAbilityInfo);
//                            }
//                            else
//                                DataViewStr["호출 어빌인덱스"].data = "0";
//                        }
//                    }
//                    else if (data.data is bool)
//                        data.data = GUI.Toggle(new Rect(120, height, 170, 20), (bool)data.data, "Check");

//                    height += 25;
//                }
//            }
//        }


//        //< 어빌리티 삭제
//        if (GUI.Button(new Rect(5, MainMenuHeightSize - 45, 290, 40), "선택한 어빌리티 삭제"))
//            DeleteAbility();
//    }

//    //< 콤보 박스(종류를 선택할수있는 콤보박스처리)
//    int ChoiceIdx = 0;
//    Vector2 scrollPosition;
//    void ComboBoxView(int id)
//    {
//        bool ExitCheck = false;

//        //< 리스트대로 뿌려준다.
//        float Height = 15;
//        int _ChoiceIdx = -1;
//        scrollPosition = GUI.BeginScrollView(new Rect(0, Height, 290, 500), scrollPosition, new Rect(0, 0, 0, (ComboSelectObj.SelectComboItem.Length + 1) * 35));

//        for (int i = 0; i < ComboSelectObj.SelectComboItem.Length; i++)
//        {
//            if (GUI.Button(new Rect(5, Height, 270, 30), ComboSelectObj.SelectComboItem[i], GetStyle(i == ChoiceIdx)))
//            {
//                _ChoiceIdx = i;

//                //< 같은걸 두번클릭하면 종료
//                if (_ChoiceIdx == ChoiceIdx)
//                    ExitCheck = true;
//            }

//            Height += 35;
//        }
//        GUI.EndScrollView();

//        if (_ChoiceIdx != -1)
//            ChoiceIdx = _ChoiceIdx;

//        //< 종료처리
//        if (ExitCheck || GUI.Button(new Rect(5, MainMenuHeightSize - 100, 290, 90), "확인"))
//        {
//            if (ComboSelectObj != null)
//            {
//                ComboSelectObj.data = ChoiceIdx.ToString();
//                ComboSelectObj = null;
//            }

//            //< 스킬타입이 변경되었다면 객체타입을 변경해줌
//            if (ComboSelectKey == "스킬 종류")
//            {
//                SelectAbilityInfo.skillType = (byte)ChoiceIdx;
//                ResetDataValue();
//            }

//            ShowComboBox = false;
//        }
//    }

//    #endregion

//    #region 스킬 레벨 정보
//    List<List<Dictionary<string, SaveValue>>> SkillLevelDataStr = new List<List<Dictionary<string, SaveValue>>>();
//    void SkillLevelData()
//    {
//        rect[6] = new Rect(rect[4].max.x + 10, MainMenuHeight, 300, MainMenuHeightSize);
//        GUI.Window(6, rect[6], _SkillLevelData, "SkillLevelData");
//    }

//    Vector2 SkillLevelScrollPosition;
//    float ScrollHeight = 0;
//    void _SkillLevelData(int id)
//    {
//        if (SelectNormalGroup == null && SelectSkillGroup != null && SelectAbilityGroup == null)
//        {
//            //<================================
//            //<     스킬 레벨 정보 입력 
//            //<================================
//            SkillLevelScrollPosition = GUI.BeginScrollView(new Rect(0, 25, 300, MainMenuHeightSize - 80), SkillLevelScrollPosition, new Rect(0, 0, 0, ScrollHeight));
//            float height = 0;
//            for (int i = 0; i < SkillLevelDataStr.Count; i++)
//            {
//                for (int j = 0; j < SkillLevelDataStr[i].Count; j++)
//                {
//                    if (j == 0)
//                    {
//                        //< 처음에만 레벨을 띄워줌
//                        GUI.Label(new Rect(10, height, 120, 30), "Lv." + SkillLevelDataStr[i][j]["level"].data.ToString() + "     Skill Info :");

//                        //< 처음에만 스킬 인포를 적도록 처리
//                        SkillLevelDataStr[i][j]["infoIdx"].data = GUI.TextField(new Rect(120, height, 120, 20), SkillLevelDataStr[i][j]["infoIdx"].data.ToString());

//                        height += 25;
//                    }

//                    //< 레벨 삭제
//                    if (GUI.Button(new Rect(245, height, 30, 25), "-"))
//                    {
//                        if (SkillLevelDataStr[i].Count == 1)
//                            SkillLevelDataStr.RemoveAt(i);
//                        else
//                            SkillLevelDataStr[i].RemoveAt(j);

//                        SaveSkillGroup();
//                        ResetSkillGroup(SelectSkillGroup);
//                        break;
//                    }

//                    //< 레벨 추가(레벨추가는 맨 아래에만 가능)
//                    if (SkillLevelDataStr[i].Count == j + 1)
//                    {
//                        if (GUI.Button(new Rect(245, height + 25, 30, 25), "+"))
//                        {
//                            SkillLevelInfo info = new SkillLevelInfo();
//                            SkillLevelDataStr[i].Add(new Dictionary<string, SaveValue>());
//                            SkillLevelDataStr[i][SkillLevelDataStr[i].Count - 1].Add("level", new SaveValue(SkillLevelDataStr[i].Count.ToString()));
//                            SkillLevelDataStr[i][SkillLevelDataStr[i].Count - 1].Add("infoIdx", new SaveValue(info.infoIdx.ToString()));
//                            SkillLevelDataStr[i][SkillLevelDataStr[i].Count - 1].Add("skillType", new SaveValue(info.skillType.ToString()));
//                            SkillLevelDataStr[i][SkillLevelDataStr[i].Count - 1].Add("levelDataType", new SaveValue(info.levelDataType.ToString()));
//                            SkillLevelDataStr[i][SkillLevelDataStr[i].Count - 1].Add("dataValue", new SaveValue(info.dataValue.ToString()));

//                            SaveSkillGroup();
//                            ResetSkillGroup(SelectSkillGroup);
//                            break;
//                        }
//                    }


//                    byte skillType = byte.Parse(SkillLevelDataStr[i][j]["skillType"].data.ToString());
//                    byte levelDataType = byte.Parse(SkillLevelDataStr[i][j]["levelDataType"].data.ToString());

//                    string str = string.Format("{0} : {1}", SkillType[skillType], (eSkillLevelData)levelDataType);
//                    if (GUI.Button(new Rect(50, height, 190, 30), str))
//                    {
//                        //< 현재 갖고있는 어빌리티들의 타입을 저장한다.
//                        SetSkillComboItem();

//                        ChoiceSkillComboIdx = Vector2.zero;
//                        SelectSkillLevelData = SkillLevelDataStr[i][j];
//                        SkillComboView = true;
//                    }

//                    height += 32;
//                    SkillLevelDataStr[i][j]["dataValue"].data = GUI.TextField(new Rect(50, height, 190, 20), SkillLevelDataStr[i][j]["dataValue"].data.ToString());

//                    height += 32;
//                }
//            }
//            ScrollHeight = height;
//            GUI.EndScrollView();

//            //<====================================
//            //<          스킬 레벨 추가
//            //<====================================
//            if (GUI.Button(new Rect(5, MainMenuHeightSize - 45, 290, 40), "스킬 레벨 추가"))
//            {
//                if (SkillLevelDataStr.Count >= 9)
//                    ShowPopup("스킬레벨 추가 실패", eErrorCode.NotAddSkillLevel_MaxCount);
//                else
//                {
//                    SkillLevelInfo info = new SkillLevelInfo();
//                    SkillLevelDataStr.Add(new List<Dictionary<string, SaveValue>>());
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1].Add(new Dictionary<string, SaveValue>());
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1][0].Add("level", new SaveValue(SkillLevelDataStr.Count.ToString()));
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1][0].Add("infoIdx", new SaveValue(info.infoIdx.ToString()));
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1][0].Add("skillType", new SaveValue(info.skillType.ToString()));
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1][0].Add("levelDataType", new SaveValue(info.levelDataType.ToString()));
//                    SkillLevelDataStr[SkillLevelDataStr.Count - 1][0].Add("dataValue", new SaveValue(info.dataValue.ToString()));

//                    SaveSkillGroup();
//                    ResetSkillGroup(SelectSkillGroup);
//                }
//            }
//        }
//    }

//    void SetSkillComboItem()
//    {
//        SkillComboDic.Clear();
//        for (int i = 0; i < SelectSkillGroup.abilityGroups.Count; i++)
//        {
//            for (int j = 0; j < SelectSkillGroup.abilityGroups[i].skillAbilitys.Count; j++)
//            {
//                if (!SkillComboDic.ContainsKey(SelectSkillGroup.abilityGroups[i].skillAbilitys[j].skillType))
//                {
//                    //< 타입에 따라서 안에 변수리스트를 넣어줌
//                    byte type = SelectSkillGroup.abilityGroups[i].skillAbilitys[j].skillType;
//                    SetSkillComboData(type);

//                    //< 서브값도 있는지 체크
//                    SetSubSkillComboItem(SelectSkillGroup.abilityGroups[i].skillAbilitys[j].subAbilitys);
//                }
//            }
//        }
//    }

//    void SetSubSkillComboItem(List<SkillAbilityInfo> subAbilitys)
//    {
//        for (int i = 0; i < subAbilitys.Count; i++)
//        {
//            if (!SkillComboDic.ContainsKey(subAbilitys[i].skillType))
//                SetSkillComboData(subAbilitys[i].skillType);

//            //< 서브값도 있는지 체크
//            SetSubSkillComboItem(subAbilitys[i].subAbilitys);
//        }
//    }

//    void SetSkillComboData(byte type)
//    {
//        SkillComboDic.Add(type, new List<eSkillLevelData>());

//        //< 발사체
//        if (type == 2)
//        {
//            SkillComboDic[type].Add(eSkillLevelData.moveSpeed);
//            SkillComboDic[type].Add(eSkillLevelData.maxDistance);
//            SkillComboDic[type].Add(eSkillLevelData.penetrate);
//            SkillComboDic[type].Add(eSkillLevelData.startAngle);
//            SkillComboDic[type].Add(eSkillLevelData.multi);
//        }
//        //< 버프
//        else if (GetBuffType(type))
//        {
//            SkillComboDic[type].Add(eSkillLevelData.factorRate);
//            SkillComboDic[type].Add(eSkillLevelData.basicValue);
//            SkillComboDic[type].Add(eSkillLevelData.overlapcount);
//            SkillComboDic[type].Add(eSkillLevelData.durationTime);
//        }
//        //< 기본
//        else if (type != 0)
//        {
//            SkillComboDic[type].Add(eSkillLevelData.factorRate);
//            SkillComboDic[type].Add(eSkillLevelData.basicValue);
//            SkillComboDic[type].Add(eSkillLevelData.ignoreDef);
//        }
//    }

//    bool SkillComboView = false;
//    Dictionary<byte, List<eSkillLevelData>> SkillComboDic = new Dictionary<byte, List<eSkillLevelData>>();
//    Vector3 SkillComboViewScrollPos;
//    int SkillComboItemValue = 0;
//    Vector2 ChoiceSkillComboIdx = new Vector2();
//    Dictionary<string, SaveValue> SelectSkillLevelData;
//    void _SkillComboView(int id)
//    {
//        bool ExitCheck = false;

//        //< 리스트대로 뿌려준다.
//        float Height = 15;
//        SkillComboViewScrollPos = GUI.BeginScrollView(new Rect(0, Height, 290, 500), SkillComboViewScrollPos, new Rect(0, 0, 0, (SkillComboItemValue) * 35));
//        Height = 0;

//        SkillComboItemValue = 0;
//        foreach (KeyValuePair<byte, List<eSkillLevelData>> dic in SkillComboDic)
//        {
//            for (int i = 0; i < dic.Value.Count; i++)
//            {
//                string str = string.Format("{0} : {1}", SkillType[dic.Key], (eSkillLevelData)dic.Value[i]);
//                if (GUI.Button(new Rect(5, Height, 270, 30), str, GetStyle((dic.Key == (byte)ChoiceSkillComboIdx.x) && ((int)dic.Value[i] == (byte)ChoiceSkillComboIdx.y))))
//                {
//                    //< 같은걸 클릭했다면 종료
//                    if (((byte)ChoiceSkillComboIdx.x == dic.Key) && ((int)ChoiceSkillComboIdx.y == (int)dic.Value[i]))
//                        ExitCheck = true;
//                    else
//                    {
//                        //< 클릭을 할경우 저장
//                        ChoiceSkillComboIdx.x = dic.Key;
//                        ChoiceSkillComboIdx.y = (int)dic.Value[i];
//                    }
//                }
//                Height += 35;
//                SkillComboItemValue++;
//            }
//        }
//        GUI.EndScrollView();

//        //< 종료처리
//        if (ExitCheck || GUI.Button(new Rect(5, MainMenuHeightSize - 100, 290, 90), "확인"))
//        {
//            SkillComboView = false;

//            SelectSkillLevelData["skillType"].data = ((byte)ChoiceSkillComboIdx.x).ToString();
//            SelectSkillLevelData["levelDataType"].data = ((byte)ChoiceSkillComboIdx.y).ToString();
//        }
//    }
//    #endregion

//    #region 공통함수
//    float TryParse_f(Dictionary<string, SaveValue> dic, string key)
//    {
//        float value_f = 0;
//        if (float.TryParse((string)dic[key].data, out value_f))
//            return value_f;
//        else
//            ShowPopup(key + " 값이 잘못 되었습니다", eErrorCode.NotNumberStr);

//        return value_f;
//    }
//    byte TryParse_b(Dictionary<string, SaveValue> dic, string key)
//    {
//        byte value_f = 0;
//        if (byte.TryParse((string)dic[key].data, out value_f))
//            return value_f;
//        else
//            ShowPopup(key + " 값이 잘못 되었습니다", eErrorCode.NotNumberStr);

//        return value_f;
//    }
//    int TryParse_i(Dictionary<string, SaveValue> dic, string key)
//    {
//        int value_f = 0;
//        if (int.TryParse((string)dic[key].data, out value_f))
//            return value_f;
//        else
//            ShowPopup(key + " 값이 잘못 되었습니다", eErrorCode.NotNumberStr);

//        return value_f;
//    }
//    uint TryParse_ui(Dictionary<string, SaveValue> dic, string key)
//    {
//        uint value_f = 0;
//        if (uint.TryParse((string)dic[key].data, out value_f))
//            return value_f;
//        else
//            ShowPopup(key + " 값이 잘못 되었습니다", eErrorCode.NotNumberStr);

//        return value_f;
//    }
//    ushort TryParse_us(Dictionary<string, SaveValue> dic, string key)
//    {
//        ushort value_f = 0;
//        if (ushort.TryParse((string)dic[key].data, out value_f))
//            return value_f;
//        else
//            ShowPopup(key + " 값이 잘못 되었습니다", eErrorCode.NotNumberStr);

//        return value_f;
//    }

//    static List<int> BuffTypeList;
//    public static bool GetBuffType(int skilltype)
//    {
//        //if (BuffTypeList == null)
//        //{
//        //    BuffTypeList = new List<int>();
//        //    foreach (eSkillType eEnum in eSkillType.GetValues(typeof(eSkillType)))
//        //    {
//        //        if (eEnum.ToString().Contains("_Buff"))
//        //            BuffTypeList.Add((int)eEnum);
//        //    }
//        //}

//        if (BuffTypeList.Contains(skilltype))
//            return true;

//        if (GetDotType(skilltype))
//            return true;

//        return false;
//    }

//    static List<int> DotTypeList;
//    public static bool GetDotType(int skilltype)
//    {
//        //if (DotTypeList == null)
//        //{
//        //    DotTypeList = new List<int>();
//        //    foreach (eSkillType eEnum in eSkillType.GetValues(typeof(eSkillType)))
//        //    {
//        //        if (eEnum.ToString().Contains("_Dot"))
//        //            DotTypeList.Add((int)eEnum);
//        //    }
//        //}

//        if (DotTypeList.Contains(skilltype))
//            return true;

//        return false;
//    }

//    bool ShowPopupCheck = false;
//    string ErrorMsg, HelpMsg;
//    public void ShowPopup(string str, eErrorCode code)
//    {
//        ShowPopupCheck = true;
//        ErrorMsg = str;

//        if (code == eErrorCode.NotNumberStr)
//            HelpMsg = "잘못된 값이 들어있습니다.\n숫자로만 값을 채워주세요.";
//        else if (code == eErrorCode.NotAddUnitGroup)
//            HelpMsg = "해당 키값이 이미 등록되어있습니다. \n다른 키값으로 등록해주세요";
//        else if (code == eErrorCode.NotAddAbility_MaxCount)
//            HelpMsg = "어빌리티 개수가 가득 찼습니다.\n기존 어빌리티를 삭제하고\n새로 등록해주세요.";
//        else if (code == eErrorCode.NotDeleteAbility_MinCount)
//            HelpMsg = "최소 어빌리티는 1개이상 있어야합니다.\n최소개수 이므로 삭제가 불가능합니다.";
//        else if (code == eErrorCode.NotAddSkillGroup_MaxCount)
//            HelpMsg = "스킬그룹 개수가 가득 찼습니다.\n기존 스킬 그룹을 삭제하고 새로 등록해주세요";
//        else if (code == eErrorCode.NotDeleteSkillGroup_MinCount)
//            HelpMsg = "최소 스킬그룹은 1개이상 있어야합니다.\n최소개수 이므로 삭제가 불가능합니다.";
//        else if (code == eErrorCode.NotAddSkillLevel_MaxCount)
//            HelpMsg = "스킬레벨 개수가 가득 찼습니다.\n기존 스킬 레벨을 삭제하고 새로 등록해주세요";
//        else if (code == eErrorCode.NotDeleteAbilityGroup_MinCount)
//            HelpMsg = "최소 어빌리티 그룹은 1개이상 있어야합니다.\n최소개수 이므로 삭제가 불가능합니다.";
//    }

//    public void ErrorPopup(int id)
//    {
//        GUI.TextArea(new Rect(5, 25, 390, 60), "에러 내역 : \n" + ErrorMsg);

//        GUI.TextArea(new Rect(5, 90, 390, 100), "해결 방법 : \n" + HelpMsg);
//        if (GUI.Button(new Rect(5, 290, 390, 100), "확인"))
//        {
//            ShowPopupCheck = false;
//        }
//    }

//    bool SystemMemuPopupCheck = false;
//    Rect SystemMemuPopupPos;
//    SystemData _SystemData = new SystemData();
//    void SystemMemuPopup(UnitGroupInfo data1, SkillGroupInfo data2, SkillGroupInfo data3, AbilityGroup data4)
//    {
//        _SystemData.parent = this;
//        _SystemData._UnitGroupInfo = data1;
//        _SystemData._NormalGroupInfo = data2;
//        _SystemData._SkillGroupInfo = data3;
//        _SystemData._AbilityGroup = data4;

//        if (_SystemData._UnitGroupInfo != null)
//            _SystemData.type = SystemData.eSystemDataType.UnitGroupInfo;
//        else if (_SystemData._NormalGroupInfo != null)
//            _SystemData.type = SystemData.eSystemDataType.NormalGroupInfo;
//        else if (_SystemData._SkillGroupInfo != null)
//            _SystemData.type = SystemData.eSystemDataType.SkillGroupInfo;
//        else if (_SystemData._AbilityGroup != null)
//            _SystemData.type = SystemData.eSystemDataType.AbilityGroup;

//        if (Input.GetMouseButtonUp(1))
//        {
//            SystemMemuPopupCheck = true;
//            SystemMemuPopupPos = new Rect(Input.mousePosition.x, 720 - Input.mousePosition.y, 150, 200);
//        }
//    }
//    void SystemMemuPopup(SystemData.eSystemDataType type)
//    {
//        if (SystemMemuPopupCheck == true)
//            return;

//        _SystemData.type = type;

//        _SystemData.parent = this;
//        _SystemData._UnitGroupInfo = null;
//        _SystemData._NormalGroupInfo = null;
//        _SystemData._SkillGroupInfo = null;
//        _SystemData._AbilityGroup = null;

//        SystemMemuPopupCheck = true;
//        SystemMemuPopupPos = new Rect(Input.mousePosition.x, 720 - Input.mousePosition.y, 150, 150);
//    }
//    string[] SystemMemuStr = { "복사하기", "붙여넣기", "삭제하기" };
//    void SystemMemuPopup(int id)
//    {
//        //< 종류에따라 처리
//        for (int i = 0; i < 3; i++)
//        {
//            if (GUI.Button(new Rect(5, 25 + (i * 32), 140, 30), SystemMemuStr[i]))
//            {
//                if (i == 0)
//                    _SystemData.DataCopy();
//                else if (i == 1)
//                    _SystemData.AddData();
//                else if (i == 2)
//                    _SystemData.Delete();

//                SystemMemuPopupCheck = false;
//            }
//        }

//        //< 종료팝업
//        if (GUI.Button(new Rect(5, SystemMemuPopupPos.height - 35, 140, 30), "닫기"))
//            SystemMemuPopupCheck = false;

//        //< 아무곳이나 클릭하면 종료처리
//        if (SystemMemuPopupCheck)
//        {
//            if (Input.GetMouseButtonUp(0))
//                SystemMemuPopupCheck = false;
//        }
//    }

//    GUIStyle GetStyle(bool type)
//    {
//        if (type)
//            return ChoiceGuiStyle;

//        return NormalGuiStyle;
//    }

//    GUIStyle GetStyle(bool type, byte skilltype)
//    {
//        GUIStyle style;
//        if (type)
//            style = new GUIStyle(ChoiceGuiStyle);
//        else
//            style = new GUIStyle(NormalGuiStyle);

//        //< 색상 보정
//        if (skilltype == 2)
//            style.normal.textColor = style.hover.textColor = Color.yellow;
//        else if (skilltype == 3 || skilltype == 4)
//            style.normal.textColor = style.hover.textColor = Color.green;
//        else
//            style.normal.textColor = style.hover.textColor = Color.magenta;

//        return style;
//    }

//    void DataSave()
//    {
//        //< 아래서부터 순서대로 저장시켜줌
//        if (SelectAbilityInfo != null)
//            SaveDataValue();

//        if (SelectAbilityGroup != null)
//            SaveAbilityGroupData();

//        if (SelectSkillGroup != null)
//            SaveSkillGroup();

//        if (SelectUnitGroup != null)
//            SaveUnitGroup();
//    }

//    void ErrorCheck()
//    {
//        //< 혹시 꼭 넣어야되는데 비어있는 값이 있는지 체크
//        bool NullDataCheck = false;
//        for (int g = 0; g < UnitGroupDic.Count; g++)
//        {
//            UnitGroupInfo Value = UnitGroupDic[g];

//            if (!NullDataCheck && DataSaveErrorCheck(Value.normalGroups.Count == 0, "[유닛그룹 : " + Value.unitIdx + " ]" + "\n => 노말그룹이 비어있습니다"))
//                NullDataCheck = true;

//            //< 노말 그룹 체크
//            for (int i = 0; i < Value.normalGroups.Count; i++)
//            {
//                if (!NullDataCheck && DataSaveErrorCheck(Value.normalGroups[i].icon == "", "[유닛그룹 : " + Value.unitIdx + " ]" + " [노말그룹 : " + (i + 1).ToString() + " ] \n=> 아이콘명이 비어있습니다."))
//                    NullDataCheck = true;

//                if (!NullDataCheck && DataSaveErrorCheck(Value.normalGroups[i].aniName == "", "[유닛그룹 : " + Value.unitIdx + " ]" + " [노말그룹 : " + (i + 1).ToString() + " ] \n=> 애니메이션 이름이 비어있습니다."))
//                    NullDataCheck = true;
//            }

//            //< 스킬 그룹 체크
//            for (int i = 0; i < Value.skillGroups.Count; i++)
//            {
//                if (!NullDataCheck && DataSaveErrorCheck(Value.skillGroups[i].skillName == 0, "[유닛그룹 : " + Value.unitIdx + " ]" + " [스킬그룹 : " + (i + 1).ToString() + " ] \n=> 스킬 이름 인덱스가 비어있습니다"))
//                    NullDataCheck = true;

//                if (!NullDataCheck && DataSaveErrorCheck(Value.skillGroups[i].descrpition == 0, "[유닛그룹 : " + Value.unitIdx + " ]" + " [스킬그룹 : " + (i + 1).ToString() + " ] \n=> 스킬 설명 인덱스가 비어있습니다"))
//                    NullDataCheck = true;

//                if (!NullDataCheck && DataSaveErrorCheck(Value.skillGroups[i].icon == "", "[유닛그룹 : " + Value.unitIdx + " ]" + " [스킬그룹 : " + (i + 1).ToString() + " ] \n=> 스킬 아이콘명이 비어있습니다."))
//                    NullDataCheck = true;

//                if (!NullDataCheck && DataSaveErrorCheck(Value.skillGroups[i].aniName == "", "[유닛그룹 : " + Value.unitIdx + " ]" + " [스킬그룹 : " + (i + 1).ToString() + " ] \n=> 애니메이션 이름이 비어있습니다."))
//                    NullDataCheck = true;

//                //< 어빌리티 그룹 체크
//                for (int j = 0; j < Value.skillGroups[i].abilityGroups.Count; j++)
//                {
//                    //dic.Value.skillGroups[i].abilityGroups[j].skillAbilitys
//                }
//            }
//        }
//    }

//    //< 파일 저장
//    void SaveJson()
//    {
//        //< 저장부터 하고 만듬
//        DataSave();

//        //< 에러체크
//        //ErrorCheck();

//        File.WriteAllText(FilePath, TinyJSON.JSON.Dump(UnitGroupDic));
//    }

//    bool DataSaveErrorCheck(bool type, string errorMsg)
//    {
//        ShowPopup(errorMsg, eErrorCode.None);
//        return type;
//    }

//    //< 업로드
//    IEnumerator UploadFileCo(string path, string localFileName, string uploadURL)
//    {
//        WWWForm postForm = new WWWForm();
//        byte[] bytes = System.IO.File.ReadAllBytes(path + localFileName);

//        //< 사이즈 , 이름, 경로
//        postForm.AddBinaryData("files", bytes, path + localFileName, "text/plain");
//        postForm.AddField("dir", "json");
//        postForm.AddField("os", "android");

//        //< 업로드 시킴
//        WWW upload = new WWW(uploadURL, postForm);
//        yield return upload;

//        //< 대기타다가 에러가 아니라면
//        if (upload.error == null)
//        {
//            FileUploadSuccess = true;
//        }
//        else
//        {
//            ShowPopup(upload.error, eErrorCode.None);
//        }

//        upload.Dispose();
//    }

//    void UploadPopup(int id)
//    {
//        if (GUI.Button(new Rect(5, 5, 390, 200), !FileUploadSuccess ? "파일 업로드 중.." : "업로드 완료"))
//        {
//            if (FileUploadSuccess)
//                FileUploadCheck = false;
//        }
//    }
//    #endregion

//    #region 공통 데이터
//    public enum ValueType
//    {
//        All,
//        Common,
//        Normal,
//        Buff,
//        ProjectTile,
//        MultiTasking
//    }

//    public enum eErrorCode
//    {
//        None,
//        NotNumberStr,               //< 스트링에 숫자가 아닌값이 들어있음
//        NotAddSkillGroup_MaxCount,  //< 스킬 그룹 추가 실패, 맥스값 도달
//        NotDeleteSkillGroup_MinCount,   //< 스킬 그룹 삭제 실패, 최소값 도달
//        NotAddUnitGroup,            //< 그룹 추가 실패, 동일한 키값이 들어있음
//        NotAddAbility_MaxCount,     //< 어빌리티 추가 실패, 맥스값 도달
//        NotDeleteAbility_MinCount,  //< 어빌리티 삭제 실패, 최소값 도달
//        NotAddSkillLevel_MaxCount,  //< 스킬 레벨 추가 실패, 맥스값 도달
//        NotDeleteAbilityGroup_MinCount, //< 어빌리티 그룹 삭제 실패, 최소값 도달
//    }


//    static string[] SkillType = { "멀티캐스팅", "일반공격", "발사체", "버프", "카운트다운 버프", "공격력(버프)", "방어력(버프)", "최대HP(버프)", "공격력(%)(버프)", "방어력(%)(버프)", "최대HP(%)(버프)", 
//                                    "크리티컬확률(%)(버프)","크리티컬대미지(%)(버프)","공격속도(%)(버프)","물속성 공격력(버프)","불속성 공격력(버프)","금속속성 공격력(버프)","독속성 공격력(버프)","자연속성 공격력(버프)",
//                                    "완전 무적(버프)","일반공격 무적(버프)","스킬공격 무적(버프)","회복(즉시)","회복(지속)","방어무시(버프)","출혈(도트)","화상(도트)","독(도트)","동상(도트)","방어막(버프)","띄우기","스킬오브젝트","흡혈(버프)","치명타시 회복(버프)",
//                                    "부활(버프)","얼림(버프)","석화(버프)","혼란(버프)","기절(버프)","다운(버프)","분신(버프)", "빛속성 공격력(버프)", "암흑속성 공격력(버프)", "사이즈변경(버프)", "띄우기(버프)", "넉백(버프)", "유닛소환(버프)"};
//    static string[] ApplyTarget = { "자신", "대상(적)", "아군만", "대상(논타겟)", "미사용" };
//    static string[] ApplyTargetType = { "없음", "체력이 가장 낮은대상", "체력이 가장 높은대상", "지능이 가장 낮은대상", "지능이 가장 높은대상" };
//    static string[] BaseFactor = { "없음", "공격력", "방어력", "전체체력", "남은체력", "크리확률", "크리공격력", "공격속도", "자연공격력", "독공격력", "물공격력", "금속공격력", "불공격력", "빛공격력", "암흑공격력" };
//    //static string[] LevelDataType = { "없음", "어빌리티", "수치증가(value)", "수치증가(%)" };

//    #endregion
//}
