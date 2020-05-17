using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillViewScene : MonoBehaviour {

    //List<UnitGroupInfo> UnitGroupDic = new List<UnitGroupInfo>();
    //UnitGroupInfo SelectUnitGroup;
    //Dictionary<int, string> UnitNameDic = new Dictionary<int, string>();

    //bool IsDone = false;
    //void Update()
    //{
    //    if(!IsDone && SimulationGameInfo.IsDone)
    //    {
    //        IsDone = true;
    //        UnitGroupDic = UnitSkillLowData.SkillGroupDic;

    //        int choiceIdx = PlayerPrefs.GetInt("SkillViewScene");
    //        if (choiceIdx == 0)
    //        {
    //            SelectUnitGroup = UnitGroupDic[0];
    //        }
    //        else
    //        {
    //            for (int i = 0; i < UnitGroupDic.Count; i++)
    //            {
    //                if (UnitGroupDic[i].unitIdx == choiceIdx)
    //                {
    //                    SelectUnitGroup = UnitGroupDic[i];
    //                    break;
    //                }
    //            }
    //        }
            
    //        //< 맵 로드
    //        InGameMapMgr.instance.LoadMap("Quarry001", 0, "[\"Quarry001_Lightmap\"]", (obj) =>
    //        {
    //            obj.transform.localPosition = new Vector3(0, 0, 23);
    //        });
    //    }
    //}

    //bool IsGame = false;
    //void OnGUI()
    //{
    //    if (!IsDone)
    //        return;

    //    float height = 10;

    //    if (IsGame)
    //    {
    //        if (GUI.Button(new Rect(10, height, 200, 100), "시뮬레이션 종료"))
    //        {
    //            IsGame = false;

    //            GameObject _camera = GameObject.Find("CameraManager");
    //            _camera.transform.position = new Vector3(0, 1000, 0);
    //            Camera.main.transform.localPosition = Vector3.zero;

    //            if (MainUnit != null)
    //                DestroyImmediate(MainUnit);
    //        }

    //        for (int i = 0; i < SelectUnitGroup.skillGroups.Count; i++)
    //        {
    //            if (GUI.Button(new Rect(5, 150 + (i * 80), 130, 80), (i + 1) + "번 스킬 사용"))
    //            {
    //                Unit unit = MainUnit.GetComponent<Unit>();

    //                //< 스킬 정보 대입
    //                unit.SkillCtlr.UnitId = (uint)SelectUnitGroup.unitIdx;
    //                unit.SkillCtlr.SetSkill((uint)i, i, i);

    //                unit.SkillCtlr.SkillList[i].coolTime = 0;

    //                //< 스킬 실행
    //                unit.UseSkill(i);
    //                break;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (GUI.Button(new Rect(10, height, 200, 100), "시뮬레이션 실행"))
    //        {
    //            if (SelectUnitGroup == null)
    //            {
    //                return;
    //            }

    //            GameStart();
    //        }

    //        if (SelectUnitGroup == null)
    //            return;

    //        if (!UnitNameDic.ContainsKey(SelectUnitGroup.unitIdx))
    //        {
    //            UnitLowData.GradeInfo pcdata = LowDataMgr.GetUnitGradeInfo((uint)SelectUnitGroup.unitIdx);
    //            if (pcdata != null)
    //                UnitNameDic.Add(SelectUnitGroup.unitIdx, LowDataMgr.GetLocale(pcdata.stringId).title);
    //            else
    //                UnitNameDic.Add(SelectUnitGroup.unitIdx, "없음");
    //        }

    //        string info = string.Format("{0}({1})", SelectUnitGroup.unitIdx, UnitNameDic[SelectUnitGroup.unitIdx]);
    //        GUI.TextArea(new Rect(230, height, 200, 30), info);

    //        height += 30;
    //        if(GUI.Button(new Rect(230, height, 100, 70), "PREV"))
    //        {
    //            for (int i = 0; i < UnitGroupDic.Count; i++)
    //            {
    //                if (UnitGroupDic[i].unitIdx == SelectUnitGroup.unitIdx)
    //                {
    //                    int num = i;
    //                    num--;
    //                    if (num >= 0)
    //                        SelectUnitGroup = UnitGroupDic[num];

    //                    break;
    //                }
    //            }

    //            PlayerPrefs.SetInt("SkillViewScene", SelectUnitGroup.unitIdx);
    //            PlayerPrefs.Save();

    //        }
    //        if (GUI.Button(new Rect(330, height, 100, 70), "NEXT"))
    //        {
    //            for (int i = 0; i < UnitGroupDic.Count; i++)
    //            {
    //                if (UnitGroupDic[i].unitIdx == SelectUnitGroup.unitIdx)
    //                {
    //                    int num = i;
    //                    num++;
    //                    if (UnitGroupDic.Count > num)
    //                        SelectUnitGroup = UnitGroupDic[num];

    //                    break;
    //                }
    //            }

    //            PlayerPrefs.SetInt("SkillViewScene", SelectUnitGroup.unitIdx);
    //            PlayerPrefs.Save();
    //        }
    //    }
    //}

    //string SimulationUnitID;
    //GameObject MainUnit;
    //void GameStart()
    //{
    //    //< 플레이어 생성
    //    SimulationUnitID = SelectUnitGroup.unitIdx.ToString();

    //    if (LowDataMgr.GetUnitData(uint.Parse(SimulationUnitID)) == null)
    //    {
    //        return;
    //    }

    //    if (MainUnit != null)
    //        DestroyImmediate(MainUnit);

    //    //< 모든 플레이어컨트롤러 삭제
    //    if (G_GameInfo.GameInfo != null)
    //        G_GameInfo.GameInfo.AllRemovePlayerController();

    //    //GameObject Team1Start = GameObject.Find("Team1Start");

    //    NetData.UnitData unit = new NetData.UnitData();
    //    unit.pc_gsn = uint.Parse(SimulationUnitID);
    //    unit.pc_id = uint.Parse(SimulationUnitID);
    //    unit.star_grade = LowDataMgr.GetUnitData(unit.pc_id).defaultGrade;
    //    NetData.instance.AddUnitData(unit);

    //    PlayerSyncData syncdata = new PlayerSyncData();
    //    syncdata.TeamID = 0;
    //    syncdata.unitSyncDatas = new List<UnitSyncData>();
    //    syncdata.unitSyncDatas.Add(new UnitSyncData());
    //    syncdata.unitSyncDatas[0].LowID = uint.Parse(SimulationUnitID);
    //    syncdata.unitSyncDatas[0].TeamID = 0;
    //    syncdata.unitSyncDatas[0].Grade = unit.star_grade;
    //    syncdata.unitSyncDatas[0].Stats = GK_StatGenerator.GenerateTotalStatsFromNetData(NetData.instance, (uint)syncdata.unitSyncDatas[0].LowID, 0);

    //    PlayerController sync = G_GameInfo.GameInfo.CreatePlayerController(syncdata);
    //    MainUnit = sync.Leader.gameObject;
    //    sync.Leader.FSM.Enable(true);
    //    sync.Leader.SkillCtlr.UnitId = (uint)SelectUnitGroup.unitIdx;

    //    G_GameInfo.GameInfo.AutoMode = true;

    //    GameObject _camera = GameObject.Find("CameraManager");
    //    _camera.transform.position = new Vector3(0, 0, 0);
    //    Camera.main.transform.localPosition = Vector3.zero;

    //    //< 몬스터 위치를 다시잡는다.
    //    G_GameInfo.GameInfo.characterMgr.allTeamDic[1][0].transform.position = new Vector3(-5, 0, 27);
    //    G_GameInfo.GameInfo.characterMgr.allTeamDic[1][0].FSM.Enable(UnitState.Idle);
    //    IsGame = true;
    //}
}
