using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelSimulation : MonoBehaviour {

    ModelSimulationPanel _ModelSimulationPanel;
    void Start()
    {
        _ModelSimulationPanel = UIMgr.Open("ModelSimulationPanel").GetComponent<ModelSimulationPanel>();
        _ModelSimulationPanel._ModelSimulation = this;
    }

    Vector3 ViewType1 = new Vector3(0, 0, 0);
    Vector3 ViewType2 = new Vector3(20, 0, -30);

    void OnGUI()
    {
        //if (!SimulationGameInfo.IsDone)
        //    return;

        //< 현재 스크린 사이즈를 얻어온다
        float widthSize = Screen.width - 1280;
        float heightSize = Screen.height - 720;

        //< 상단 메뉴 출력
        GUI.Window(10, new Rect(5, (Screen.height - Screen.height) + 5, 1260 + widthSize, 70), _TopMenu, "MENU");

        //< 왼쪽 리스트 출력
        GUI.Window(20, new Rect(5, (Screen.height - Screen.height) + 80, 200, 630 + heightSize), _AniMenu, "ANI");

        GUI.Label(new Rect(210, (Screen.height - Screen.height) + 80, 400, 20), "모델 좌우 회전 : 마우스 왼쪽 클릭후 좌우이동");
        GUI.Label(new Rect(210, (Screen.height - Screen.height) + 100, 400, 20), "모델 이동 : Alt + 마우스 왼쪽 클릭후 이동");
        GUI.Label(new Rect(210, (Screen.height - Screen.height) + 120, 400, 20), "카메라 줌아웃 : 마우스 휠");
        
    }

    string UnitAddInputStr = "";
    bool AuotoIdle = false;
    void _TopMenu(int id)
    {
        UnitAddInputStr = GUI.TextField(new Rect(5, 20, 80, 45), UnitAddInputStr);
        if (GUI.Button(new Rect(85, 20, 52, 45), "LOAD"))
        {
            uint idx;
            if (uint.TryParse(UnitAddInputStr, out idx))
            {
                Camera.main.transform.localPosition = Vector3.zero;
                StopAllCoroutines();
                StartCoroutine(ModelCreate(idx));
            }
        }

        if(GUI.Button(new Rect (200, 20, 100, 45), "AuotoIdle " + (AuotoIdle ? "On" : "Off")))
            AuotoIdle = !AuotoIdle;

        if (GUI.Button(new Rect(400, 20, 100, 45), "카메라 시점 1"))
        {
            if(MainUnit != null)
            {
                ViewType1.y = MainUnit.transform.localRotation.eulerAngles.y;
                MainUnit.transform.localRotation = Quaternion.Euler(ViewType1);
            }
        }

        if (GUI.Button(new Rect(510, 20, 100, 45), "카메라 시점 2"))
        {
            if (MainUnit != null)
            {
                ViewType2.y = MainUnit.transform.localRotation.eulerAngles.y;
                MainUnit.transform.localRotation = Quaternion.Euler(ViewType2);
            }
        }
    }

    void _AniMenu(int id)
    {
        for(int i=0; i<AniPlayList.Count; i++)
        {
            if(GUI.Button(new Rect(5, 20 + (i * 26), 150, 25), AniPlayList[i].ToString()))
            {
                //< 애니메이션 실행
                StopCoroutine("AttackCombo");
                MainUnit.transform.localPosition = center;
                if (MainUnit.PlayAnim(AniPlayList[i]))
                {
                    (MainUnit as Pc).rootMotion.Play(MainUnit.Animator.CurrentAnimState, true);
                    AniActive = true;

                    //< 이펙트도 실행
                    if (MainUnit.GetAniData(AniPlayList[i]) != null)
                    {
                        string StartEffect = MainUnit.GetAniData(AniPlayList[i]).effect;
                        MainUnit.SpawnSkillEffect(StartEffect, MainUnit.Animation[MainUnit.Animator.GetAnimName(AniPlayList[i])].speed, MainUnit.transform, MainUnit.GetAniData(AniPlayList[i]).childEffect == 0 ? null : MainUnit.transform, null);
                    }
                }
            }
        }

        if (GUI.Button(new Rect(5, 20 + ((AniPlayList.Count + 1) * 26), 150, 25), "어택 콤보"))
        {
            AniActive = true;
            StartCoroutine("AttackCombo");
        }
    }

    bool AniActive = false;
    void Update()
    {
        if (AuotoIdle && AniActive && !MainUnit.Animator.Animation.isPlaying)
        {
            AniActive = false;
            MainUnit.transform.localPosition = center;
            MainUnit.PlayAnim(eAnimName.Anim_idle);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (MainUnit == null)
                return;

            Camera.main.transform.localPosition -= (MainUnit.transform.position - Camera.main.transform.position).normalized * 0.5f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (MainUnit == null)
                return;

            Camera.main.transform.localPosition += (MainUnit.transform.position - Camera.main.transform.position).normalized * 0.5f;
        } 
    }

    public Vector3 center = new Vector3(0.5f, -2, 10);

    Unit MainUnit;
    PlayerController syncctr;
    List<eAnimName> AniPlayList = new List<eAnimName>();
    IEnumerator ModelCreate(uint idx)
    {
        yield return null;
        //황비홍 프로젝트께 아님
        ////< 플레이어 생성
        //if (LowDataMgr.GetUnitData(idx) == null)
        //{
        //    Debug.LogWarning("해당 유닛 데이터가 존재하지 않습니다");
        //    //UIMgr.instance.OpenPopup("해당 유닛 데이터가 존재하지 않습니다");
        //    yield break;
        //}

        //if (MainUnit != null)
        //    DestroyImmediate(MainUnit.gameObject);

        ////< 모든 플레이어컨트롤러 삭제
        //if (G_GameInfo.GameInfo != null)
        //    G_GameInfo.GameInfo.AllRemovePlayerController();

        //NetData.UnitData unit = new NetData.UnitData();
        //unit.pc_gsn = idx;
        //unit.pc_id = idx;
        //unit.star_grade = LowDataMgr.GetUnitData(unit.pc_id).defaultGrade;
        //NetData.instance.AddUnitData(unit);

        //PlayerSyncData syncdata = new PlayerSyncData();
        //syncdata.TeamID = 0;
        //syncdata.unitSyncData =new UnitSyncData();
        //syncdata.unitSyncData.LowID = idx;
        //syncdata.unitSyncData.TeamID = 0;
        //syncdata.unitSyncData.Grade = 6;
        //syncdata.unitSyncData.Stats = GK_StatGenerator.GenerateTotalStatsFromNetData(NetData.instance, (uint)syncdata.unitSyncData.LowID, 0);
        //syncdata.unitSyncData.SetSkillLevels(new byte[] { 4, 4, 4, 4 });

        //if (syncctr != null && syncctr.gameObject != null)
        //{
        //    G_GameInfo.GameInfo.RemovePlayerController(syncctr);
        //    DestroyImmediate(syncctr.gameObject);
        //}

        //G_GameInfo.GameInfo.GameLive = false;

        //syncctr = G_GameInfo.GameInfo.CreatePlayerController(syncdata);
        //MainUnit = syncctr.Leader.gameObject.GetComponent<Unit>();
        //syncctr.Leader.IsLeader = true;
        //syncctr.Leader.FSM.Enable(true);
        //syncctr.Leader.SkillCtlr.UnitId = idx;
        //syncctr.Leader.CharInfo.Stats[AbilityType.HP].Value += 100000;

        //MainUnit.transform.localPosition = center;
        //MainUnit.transform.localRotation = Quaternion.Euler(new Vector3(0, -180, 0));

        //_ModelSimulationPanel.ModelGO = MainUnit.gameObject;
        //while(true)
        //{
        //    if (MainUnit.Animator != null && MainUnit.Animator.IsReady)
        //    {
        //        //< 플레이할수있는 리스트를 얻는다
        //        AniPlayList.Clear();
        //        foreach (eAnimName eEnum in eAnimName.GetValues(typeof(eAnimName)))
        //        {
        //            if (MainUnit.Animator.GetAnimName(eEnum) != "1")
        //                AniPlayList.Add(eEnum);
        //        }

        //        Debug.Log("AniPlayList.Count " + AniPlayList.Count);

        //        MainUnit.GetComponent<NavMeshAgent>().enabled = false;
        //        break;
        //    }
        //
        //    yield return null;
        //}

    }

    IEnumerator AttackCombo()
    {
        //황비홍 프로젝트께 아님
        //eAnimName nextani = eAnimName.Anim_attack1;

        //while(true)
        //{
        //    if (MainUnit.PlayAnim(nextani, true))
        //    {
        //        (MainUnit as Pc).rootMotion.Play(MainUnit.Animator.CurrentAnimState, true);
        //        AniActive = true;

        //        //< 이펙트도 실행
        //        if (MainUnit.GetAniData(nextani) != null)
        //        {
        //            string StartEffect = MainUnit.GetAniData(nextani).effect;
        //            MainUnit.SpawnSkillEffect(StartEffect, MainUnit.Animation[MainUnit.Animator.GetAnimName(nextani)].speed, MainUnit.transform, MainUnit.GetAniData(nextani).childEffect == 0 ? null : MainUnit.transform, null);
        //        }
        //    }

        //    while(true)
        //    {
        //        if (!MainUnit.Animator.Animation.isPlaying)
        //            break;

        //        yield return null;
        //    }

        //    nextani = (eAnimName)(nextani + 1);
        //    if (nextani == eAnimName.Anim_skill1)
        //        break;

        //    yield return null;
        //}

        //MainUnit.transform.localPosition = center;
        //MainUnit.PlayAnim(eAnimName.Anim_idle);

        yield return null;
    }
}
