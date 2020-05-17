using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum eCutSceneEvent
{
    AnimaionPlay,   //< 애니메이션 플레이
    Move,           //< 해당 위치까지 대상(타겟) 이동
    Rotate,         //< 해당 각도까지 대상(타겟) 회전
    Focusing,       //< 포커싱
    PlayBgmSound,   //< 사운드 실행
    SpawnEffect,    //< 이펙트 실행
    ShowBossUI,     //< 일반 보스 UI실행
    Talk,           //< 타겟 머리위에 대사 UI실행
    SetDefaultBoss, //< 기본적인 보스 등장연출 모음
    
    SetStartPos,    //< 시작할때의 위치를 조절함(씬 이벤트연출이 아닌 시작셋팅) 
    SetDefaultPos,  //< 시작할때의 위치를 조절함(씬 이벤트연출이 아닌 시작셋팅)
    
    SetRaidDefaultPos,  //< 레이드에서의 카메라 기본위치 설정
    ShowRaidBossUI, //< 레이드 보스 UI실행
    RaidBossTalk,   //< 레이드 보스 대사 UI 실행
    SetTimeScale,   //< 게임 속도 조절
    CameraShake,    //< 카메라 쉐이크
    ImmediatelyMove,//< 해당 위치로 바로 이동
    SetFadeIn,      //< 페이드인처리
    SetFadeOut,     //< 페이드아웃 처리

    StopUpdate,
    StartUpdate,

    SetRaidStartDefault,    //< 레이드 시작시 기본셋팅

    PlaySound,      //< 일반 사운드 실행
    SetActive,      //< 오브젝트 활성화, 비활성화 처리
}

[System.Serializable]
public class CutSceneEventDataList
{
    //< 호출할 이벤트종류
    public eCutSceneEvent ActionType;

    //< 이벤트에따라 사용할 값
    public string ActionString;     //< 문자열이 필요할때 사용, 애니메이션 이름이라던지, 다른 함수를 호출한다던지..
    public float ActionValue;       //< 수치로 사용, 이동하거나 회전할때에 속도가 될수도있고 그외 등등..
    public bool ActionBool;         //< Bool 수치가 필요할때 사용
    public Vector3 ActionPos;       //< 이동을 하거나 그외 벡터값이 필요할경우 사용

    public float ActionDelay;       //< 이벤트가 종료된후 다음 이벤트를 호출하기 전 딜레이
}

[System.Serializable]
public class CutSceneEventData
{
    //< 호출할 대상
    public CutSceneEventDataList[] EventList;
}

namespace WellFired
{
    [USequencerFriendlyName("Send Message")]
    [USequencerEvent("Signal/CutSceneSendMessage")]
    public class CutSceneSendMessage : USEventBase
    {
        //< 타겟을 대상으로 연출하는것이 아닌, 시스템관련(BGM실행, 포커싱 실행 등..)
        public string SceneInfoStr = "";    //< 이건 그냥 툴에서 보기위해 설명을 적어두는 변수
        public bool EventSystem = false;
        public CutSceneEventData[] _CutSceneEventData = null;
        public override void FireEvent()
        {
            if (!Application.isPlaying)
                return;

            if (_CutSceneEventData == null || G_GameInfo.GameInfo == null)
                return;

            //< 개별적으로 모두 호출해준다.
            for (int i = 0; i < _CutSceneEventData.Length; i++)
                G_GameInfo.GameInfo.StartCoroutine(EventUpdate(_CutSceneEventData[i]));
        }

        public override void ProcessEvent(float deltaTime)
        {

        }

        //<===========================================
        //<         메인 이벤트 업데이트 코루틴
        //<===========================================
        public static CameraFadeEvent _CameraFadeEventObj;
        IEnumerator EventUpdate(CutSceneEventData __CutSceneEventData)
        {
            List<CutSceneEventDataList> EventList = new List<CutSceneEventDataList>();
            for (int i = 0; i < __CutSceneEventData.EventList.Length; i++)
                EventList.Add(__CutSceneEventData.EventList[i]);

            int DefaultIdx = -1;
            for (int i = 0; i < EventList.Count; i++ )
            {
                if(EventList[i].ActionType == eCutSceneEvent.SetRaidStartDefault)
                {
                    DefaultIdx = i;
                    break;
                }
            }

            if(DefaultIdx != -1)
            {
                EventList.Insert(DefaultIdx + 1, CreateCutSceneEventDataList(eCutSceneEvent.StartUpdate, Vector3.zero));
                EventList.Insert(DefaultIdx + 1, CreateCutSceneEventDataList(eCutSceneEvent.SetRaidDefaultPos, Vector3.zero));
                EventList.Insert(DefaultIdx + 1, CreateCutSceneEventDataList(eCutSceneEvent.SetFadeOut, Vector3.zero));
                EventList.Insert(DefaultIdx + 1, CreateCutSceneEventDataList(eCutSceneEvent.StopUpdate, Vector3.zero));
            }

            Unit unit = null;
            for (int i = 0; i < EventList.Count; i++)
            {
                //Debug.Log("EventList ActionType " + EventList[i].ActionType);
                switch (EventList[i].ActionType)
                {
                    #region 이전데이터

                    //< 카메라 포커싱처리
                    case eCutSceneEvent.Focusing:
                        if (EventList[i].ActionBool)
                        {
                            if (G_GameInfo.GameInfo.FocusingCam != null)
                                G_GameInfo.GameInfo.FocusingCam.StartEffect(0, true);
                        }
                        else
                        {
                            if (G_GameInfo.GameInfo.FocusingCam != null && G_GameInfo.GameInfo.FocusingCam.gameObject.activeSelf)
                                G_GameInfo.GameInfo.FocusingCam.EndEffect(true);
                        }
                        break;

                    //< 사운드 실행
                    case eCutSceneEvent.PlayBgmSound:
                        //SoundHelper.PlayBgmSound((uint)EventList[i].ActionValue);
                        break;

                    //< 보스 UI 출력
                    case eCutSceneEvent.ShowBossUI:
                        if (null == UIMgr.GetUI("InGame/BossCutscenePanel"))
                            UIMgr.Open("InGame/BossCutscenePanel", EventList[i].ActionValue);
                        break;

                    //< 기본 셋트 한번에 다해줌
                    case eCutSceneEvent.SetDefaultBoss:

                        //< 포커싱처리
                        G_GameInfo.GameInfo.FocusingCam.StartEffect(0, true);

                        //< 사운드실행
                        //SoundHelper.PlayBgmSound(9008);

                        unit = CutSceneMgr.CutSceneEventDic[0].GetComponent<Unit>();

                        G_GameInfo.GameInfo.FocusingCam.AddObject(unit.gameObject);

                        //< 보스 유아이 실행
                        //if (null == UIMgr.GetUI("InGame/BossCutscenePanel"))
                        //    UIMgr.Open("InGame/BossCutscenePanel", (float)3);

                        //< 해당 애니메이션 플레이 시간을 얻는다.
                        eAnimName aniName2 = GetAniName("Anim_intro");
                        if (aniName2 != eAnimName.Anim_idle)
                            EventList[i].ActionDelay = unit.Animator.GetAnimLength(aniName2) + 1;

                        //EventList[i].ActionDelay += unit.Animator.GetAnimLength(eAnimName.Anim_special);

					Sequence.Duration=EventList[i].ActionDelay+0.5f;
					USTimelineProperty Infolist = Sequence.GetComponentInChildren<USTimelineProperty>();
					//CutSceneMgr.seq.Duration=EventList[i].ActionDelay+0.5f;

					//UI 라이트를 컨트롤 한다. 다시 켜주는 건 CutSceneMgr 에 있음. EndScene 마지막
					Transform Shadow;
					Shadow = GameObject.Find ("UI_ShadowLight").GetComponent<Transform> ();
					USTimelineContainer Container = Sequence.CreateNewTimelineContainer (Shadow);
					GameObject test = new GameObject ("Evnet Timeline");
					test.transform.parent = Container.transform;
					Component Evnt= test.AddComponent ("USTimelineEvent");
					
					GameObject First = new GameObject ("USEnableComponentEvent");
					GameObject Second = new GameObject ("USEnableComponentEvent");
					
					First.transform.parent = Evnt.transform;
					Second.transform.parent = Evnt.transform;
					
					First.AddComponent ("USEnableComponentEvent");
					Second.AddComponent ("USEnableComponentEvent");
					First.GetComponent<USEnableComponentEvent> ().ComponentName = "Light";
					//Second.GetComponent<USEnableComponentEvent> ().ComponentName = "Light";
					//Second.GetComponent<USEnableComponentEvent> ().enableComponent=true;

					First.GetComponent<USEnableComponentEvent> ().FireTime = 0.0f;
					//Second.GetComponent<USEnableComponentEvent> ().FireTime = Sequence.Duration;
					//

					Infolist.Properties [0].curves [0].Keys[1].Time =0.7f;
					Infolist.Properties [0].curves [1].Keys [1].Time = 0.7f;
					Infolist.Properties [0].curves [2].Keys [1].Time =0.7f;
					Infolist.Properties [1].curves [0].Keys[1].Time =0.7f;
					Infolist.Properties [1].curves [1].Keys [1].Time = 0.7f;
					Infolist.Properties [1].curves [2].Keys [1].Time =0.7f;
					Infolist.Properties [1].curves [3].Keys [1].Time =0.7f;

					Infolist.Properties [0].curves [0].Keys[3].Time =Sequence.Duration+10;
					Infolist.Properties [0].curves [1].Keys [3].Time = Sequence.Duration+10;
					Infolist.Properties [0].curves [2].Keys [3].Time =Sequence.Duration+10;
					Infolist.Properties [1].curves [0].Keys[3].Time = Sequence.Duration+10;
					Infolist.Properties [1].curves [1].Keys [3].Time = Sequence.Duration+10;
					Infolist.Properties [1].curves [2].Keys [3].Time = Sequence.Duration+10;
					Infolist.Properties [1].curves [3].Keys [3].Time = Sequence.Duration+10;

					Infolist.Properties [0].curves[0].Keys[2].Time=Sequence.Duration;
					Infolist.Properties [0].curves [1].Keys [2].Time = Sequence.Duration;
					Infolist.Properties [0].curves [2].Keys [2].Time = Sequence.Duration;
					Infolist.Properties [1].curves [0].Keys[2].Time = Sequence.Duration;
					Infolist.Properties [1].curves [1].Keys [2].Time = Sequence.Duration;
					Infolist.Properties [1].curves [2].Keys [2].Time = Sequence.Duration;
					Infolist.Properties [1].curves [3].Keys [2].Time = Sequence.Duration;

                    break;


                    #endregion 이전데이터

                        //< 해당 위치로 강제이동
                    case eCutSceneEvent.ImmediatelyMove:
                        CutSceneMgr.CutSceneEventDic[0].transform.position = EventList[i].ActionPos;
                        break;

                    //< 이펙트 실행
                    case eCutSceneEvent.SpawnEffect:
                        SpawnEffect(EventList[i]);
                        break;

                        //< 카메라 쉐이킹
                    case eCutSceneEvent.CameraShake:
                        CutSceneMgr.Shake((byte)EventList[i].ActionValue);
                        break;

                        //< 보스 UI 출력
                    case eCutSceneEvent.ShowRaidBossUI:
                        if (null == UIMgr.GetUI("InGame/BossCutscenePanel"))
                            UIMgr.Open("InGame/BossCutscenePanel", EventList[i].ActionValue);
                        break;

                        //< 보스 대화 출력
                    case eCutSceneEvent.RaidBossTalk:
                        unit = CutSceneMgr.CutSceneEventDic[0].GetComponent<Unit>();
                        G_GameInfo.GameInfo.StartCoroutine(RaidBossTalkUpdate(unit, EventList[i].ActionString));
                        break;

                    //< 애니메이션 실행
                    case eCutSceneEvent.AnimaionPlay:
                        unit = CutSceneMgr.CutSceneEventDic[(int)EventList[i].ActionValue].GetComponent<Unit>();
                        if (unit != null)
                        {
                            eAnimName aniName = GetAniName(EventList[i].ActionString);
                            unit.PlayAnim(aniName, false, 0.2f, true);

                            //< 해당 애니메이션 플레이 시간을 얻는다.
                            if (aniName != eAnimName.Anim_idle)
                                EventList[i].ActionDelay = unit.Animator.GetAnimLength(aniName) - 0.2f;
                        }
                        else
                        {
                            //< 유닛이 없다면 그냥 바로 실행시켜준다.
                            CutSceneMgr.CutSceneEventDic[(int)EventList[i].ActionValue].animation.CrossFade(EventList[i].ActionString, 0.5f);
                        }
                        break;

                    //< 플레이 자체는 멈춰둔다.
                    case eCutSceneEvent.StopUpdate:
                        CutSceneMgr.seq.Pause();
                        break;

                    case eCutSceneEvent.StartUpdate:
                        CutSceneMgr.seq.Play();
                        break;

                    //< 페이드 인(밝아지는처리)
                    case eCutSceneEvent.SetFadeIn:
                        CutSceneMgr.seq.Pause();
                        yield return G_GameInfo._GameInfo.StartCoroutine(_CameraFadeEventObj.FadeInUpdate());
                        CutSceneMgr.seq.Play();

                        //< 스킵버튼 체크한다.
                        SetSkipPanel();
                        break;

                    //< 페이드 아웃(어두워지는 처리)
                    case eCutSceneEvent.SetFadeOut:
                        if (_CameraFadeEventObj == null)
                        {
                            _CameraFadeEventObj = (Instantiate(Resources.Load("Camera/CameraFadeEvent")) as GameObject).GetComponent<CameraFadeEvent>();
                            _CameraFadeEventObj.transform.parent = CutSceneMgr.startCamObj.transform;
                            _CameraFadeEventObj.transform.localPosition = new Vector3(0, 0, 1);
                            _CameraFadeEventObj.transform.localRotation = Quaternion.identity;
                        }

                        GameObject panel = UIMgr.GetUI("CutSceneSkipPanel");
                        if (panel != null)
                            panel.GetComponent<UIBasePanel>().Close();

                        yield return G_GameInfo._GameInfo.StartCoroutine(_CameraFadeEventObj.FadeOutUpdate());
                        break;

                    //< 레이드를 위한 기본 카메라 위치 셋팅
                    case eCutSceneEvent.SetRaidDefaultPos:
                        CutSceneMgr.startRootObj.transform.position = Vector3.zero;
                        CutSceneMgr.startRootObj.transform.rotation = Quaternion.identity;

                        CutSceneMgr.startCamObj.transform.parent = CutSceneMgr.startRootObj.transform;
                        CutSceneMgr.startCamObj.transform.localPosition = Vector3.zero;
                        CutSceneMgr.startCamObj.transform.localRotation = Quaternion.identity;
                        break;

                    case eCutSceneEvent.SetActive:
                        CutSceneMgr.CutSceneEventDic[(int)EventList[i].ActionValue].SetActive(EventList[i].ActionBool);
                        break;

                    case eCutSceneEvent.SetTimeScale:
                        SetTimeScale(EventList[i]);
                        break;
                }

                //< 딜레이동안 대기
                yield return new WaitForSeconds(EventList[i].ActionDelay);
            }

            yield return null;
        }


        #region 내부 사용 함수
        IEnumerator MoveUpdate(Unit unit, Vector3 targetPos, float Speed)
        {
            //< 해당 위치를 바라봄
            unit.transform.LookAt(targetPos);

            //< 애니메이션은 이동으로
            unit.PlayAnim(eAnimName.Anim_walk, true, 0.2f, true);

            Vector3 Look = (targetPos - unit.transform.position).normalized;

            //< 해당 위치까지 걸어감
            while (true)
            {
                unit.transform.position += Look * Speed * Time.deltaTime;
                if (Vector3.Distance(targetPos, unit.transform.position) < 0.1f)
                    break;

                yield return null;
            }

            //< 애니메이션은 일단 아이들로
            //unit.PlayAnim(eAnimName.Anim_idle, true, 0.2f, true);
        }

        IEnumerator RotateUpdate(Unit unit, Vector3 targetRot, float Speed)
        {
            float value = 0;
            while (true)
            {
                value += Speed * Time.deltaTime;

                if (value > 1)
                {
                    value = 1;
                    unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, Quaternion.Euler(targetRot), value);
                    break;
                }

                unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, Quaternion.Euler(targetRot), value);
                yield return null;
            }
        }

        //< 두번째 인자값으로 값을 나눠서 사용한다.(1 : 스트링인덱스,  2 : 대기시간,  3 : 지속시간)
        IEnumerator TalkUpdate(Unit unit, string info, Vector3 Offset)
        {
            //float id = 0, delay = 0, PlayTime = 0;
			float delay = 0;

            string[] strlist = info.Split(',');
            //id = float.Parse(strlist[0]);
            delay = float.Parse(strlist[1]);
            //PlayTime = float.Parse(strlist[2]);

            //< y는 출력 대기시간
            yield return new WaitForSeconds(delay);

            //< 출력
            //string str = LowDataMgr.GetSpeakLocale((uint)id).title;
            //GameObject sayObj = G_GameInfo.GameInfo.BoardPanel.ShowSay(unit.gameObject, str, PlayTime, false, true);
            //sayObj.GetComponent<SayUI>().offset += Offset;
        }

        //< 두번째 인자값으로 값을 나눠서 사용한다.(1 : 스트링인덱스, 2 : 지속시간)
        IEnumerator RaidBossTalkUpdate(Unit unit, string info)
        {
//            float id = 0, PlayTime = 0;
//
//            string[] strlist = info.Split(',');
//            id = float.Parse(strlist[0]);
//            PlayTime = float.Parse(strlist[1]);

            //< 출력
            //string str = LowDataMgr.GetSpeakLocale((uint)id).title;
            //G_GameInfo.GameInfo.BoardPanel.RaidBossShowSay(unit.gameObject, str, PlayTime);

            yield return null;
        }

        public static void PauseAllUnits(bool pause)
        {
            Unit[] allUnits = new Unit[G_GameInfo.CharacterMgr.allUnitDic.Values.Count];
            G_GameInfo.CharacterMgr.allUnitDic.Values.CopyTo(allUnits, 0);

            foreach (Unit unit in allUnits)
            {
                if (null == unit || (null != unit && unit.CharInfo.IsDead))
                    continue;

                //< 이벤트 몹이라면 패스
                if (unit is EventUnit)
                    continue;

                unit.StaticState(pause);

                //if (!pause)
                //    unit.ChangeState(UnitState.Idle);
            }
        }

        eAnimName GetAniName(string str)
        {
            foreach (eAnimName eEnum in eAnimName.GetValues(typeof (eAnimName)))
            {
                if (str == eEnum.ToString())
                    return eEnum;
            }

            return eAnimName.Anim_idle;
        }

        public CutSceneEventDataList CreateCutSceneEventDataList(eCutSceneEvent type, Vector3 _ActionPos, string _ActionString = "", float _ActionValue = 0, bool _ActionBool = false, float _ActionDelay = 0)
        {
            CutSceneEventDataList data = new CutSceneEventDataList();
            data.ActionType = type;
            data.ActionString = _ActionString;
            data.ActionBool = _ActionBool;
            data.ActionPos = _ActionPos;
            data.ActionDelay = _ActionDelay;
            return data;
        }

        void SpawnEffect(CutSceneEventDataList data)
        {
            if (data.ActionValue != -1)
            {
                G_GameInfo.SpawnEffect(data.ActionString, 1f, CutSceneMgr.CutSceneEventDic[(int)data.ActionValue].transform, CutSceneMgr.CutSceneEventDic[(int)data.ActionValue].transform, Vector3.one, (efftrn) =>
                {
                    efftrn.localPosition = data.ActionPos;
                    CutSceneMgr.EffectList.Add(efftrn.gameObject);
                });
            }
            else
            {
                G_GameInfo.SpawnEffect(data.ActionString, 1f, null, null, Vector3.one, (efftrn) =>
                {
                    efftrn.localPosition = data.ActionPos;
                    CutSceneMgr.EffectList.Add(efftrn.gameObject);
                });
            }
            
        }

        void SetTimeScale(CutSceneEventDataList data)
        {
            float NowTime = Time.timeScale;
            Time.timeScale = 0.1f;

            //< 일정시간동안 대기
            TempCoroutine.instance.FrameDelay(data.ActionValue, () =>
            {
                //< 시간 복구
                Time.timeScale = NowTime;
            });
        }

        void SetSkipPanel()
        {
            //string skipCheck = "RaidCutSceneSkip_" + (G_GameInfo._GameInfo as RaidGameInfo).areaType.ToString();
            //if (PlayerPrefs.GetInt(skipCheck) == 1)
            //{
            //    //< 스킵 패널 오픈
            //    CutSceneMgr.instance.OpenCutSceneSkipPanel();
            //}
            //else
            //{
            //    PlayerPrefs.SetInt(skipCheck, 1);
            //    PlayerPrefs.Save();
            //}
        }

        //public void Shake(int count)
        //{
        //    StopCoroutine("CameraShakeUpdate");
        //    StartCoroutine("CameraShakeUpdate", count);
        //}

        //IEnumerator CameraShakeUpdate(int count)
        //{
        //    int LifeCount = count;

        //    int EvnetIdx = 0;
        //    float MoveSpeed = 0;
        //    Vector3 TargetPos = Vector3.one;
        //    while (true)
        //    {
        //        //< 카메라를 흔들어준당
        //        if (EvnetIdx == 0)
        //        {
        //            CutSceneMgr.startRootObj.transform.position = Vector3.zero;
        //            Vector3 look = Vector3.zero;

        //            //< 가로방향 값을 구한다
        //            float w_value = Random.Range((float)30, (float)100) * 0.01f;

        //            //< 좌로할지 우로할지 정한다
        //            look.x = Random.Range(0, 2) == 0 ? -w_value : w_value;

        //            //< 세로방향 값을 구한다
        //            float h_value = 1 - w_value;

        //            //< 위로할지 아래로할지 정한다
        //            look.y = Random.Range(0, 2) == 0 ? -h_value : h_value;

        //            //< 얼만큼 이동할지
        //            float power = Random.Range((float)0.8f, (float)1.6f);
        //            MoveSpeed = (power * 30) * Time.deltaTime;

        //            //< 최종 이동위치 구함
        //            TargetPos = look * power;
        //            EvnetIdx++;

        //            Debug.Log("look " + look + " , power" + (power) + " , TargetPos " + TargetPos);
        //        }
        //        else if (EvnetIdx == 1)
        //        {
        //            //< 해당위치까지 이동시킨다.
        //            CutSceneMgr.startRootObj.transform.position += (TargetPos - CutSceneMgr.startRootObj.transform.position) * MoveSpeed;
        //            if (Vector3.Distance(CutSceneMgr.startRootObj.transform.position, TargetPos) < MoveSpeed)
        //            {
        //                CutSceneMgr.startRootObj.transform.position = TargetPos;
        //                EvnetIdx++;
        //            }
        //        }
        //        else if (EvnetIdx == 2)
        //        {
        //            //< 다시 정위치로 이동
        //            TargetPos = Vector3.zero;
        //            CutSceneMgr.startRootObj.transform.position = Vector3.zero;
        //            EvnetIdx++;
        //        }
        //        else if (EvnetIdx == 3)
        //        {
        //            //< 해당위치까지 이동시킨다.
        //            CutSceneMgr.startRootObj.transform.position += (TargetPos - CutSceneMgr.startRootObj.transform.position) * (MoveSpeed * 2);
        //            if (Vector3.Distance(CutSceneMgr.startRootObj.transform.position, TargetPos) < (MoveSpeed * 2))
        //            {
        //                CutSceneMgr.startRootObj.transform.position = TargetPos;

        //                EvnetIdx = 0;

        //                LifeCount--;
        //                if (LifeCount <= 0)
        //                    break;
        //            }
        //        }

        //        yield return null;
        //    }

        //    CutSceneMgr.startRootObj.transform.position = Vector3.zero;
        //    yield return null;
        //}
        #endregion

        
    }

    
}
