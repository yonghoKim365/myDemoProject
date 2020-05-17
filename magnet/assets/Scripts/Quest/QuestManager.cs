using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sw;

/*
public class QuestData
{
    public uint Id;         //퀘스트의 ID
    public string Title;    //퀘스트의 타이틀
    public string Description;    //퀘스트의 대략적인 내용

    public List<QuestSubData> subQuest; //서브퀘스트의 리스트
    
    public uint nextid;     //이어지는 퀘스트 ID

    //보상리스트 필요
}

public class QuestSubData
{
    public uint Id;         //서브퀘스트의 ID
    public uint MainId;     //이 서브퀘스트의 루트아이디

    public string Description; //게임에 표시될 서브미션의 내용(뭐 어디로 가라던가 그런 요약)

    public QuestSubType QuestType; //퀘스트의 타입
    public uint QuestObject; //퀘스트에 쓰일 실제 내용(NPCTALK경우 NPC의 ID, SINGLEGAMEPLAY일경우 스테이지 번호, SINGLEGAMECLEAR일경우 스테이지 번호)

    public uint QuestTalkSceneID;   //목적을 완료했을시 나올 TalkSceneID
}
*/

public enum QuestSubType
{
    NPCTALK,        //NPC와 대화하기
    SINGLEGAMEPLAY, //특정 싱글게임 스테이지 입장
    SINGLEGAMECLEAR, //특정 싱글게임 스테이지 완료
}

public class QuestInfo
{
    public uint unTaskId;               // 퀘스트 id 任务id
    public uint unTargetNum;            // 퀘스트 목표 수량 任务目标数量
    public uint unComplete;             // 퀘스트 클리어 여부 任务是否完成
    public uint unFetchBonus;			// 퀘스트 보상 수령 여부 0:미수령, 1:수령 任务奖励是否已领取任务奖励,0未领取,1已领取
}

public class QuestManager : Immortal<QuestManager>
{
    public Dictionary<uint, QuestInfo> QuestList;

    //public QuestData currentQuest;
    //public QuestSubData currentSubQuest;

    private TownPanel TownPanelScript;

    //public  bool isPortal = false;
    //public bool isNPC = false;

    protected override void Init()
    {
        //임시로 데이터를 구성해보자 퀘스트 데이터는 항상 현재 currentQuest만 들고있는다.

        base.Init();
        QuestList = new Dictionary<uint, QuestInfo>();
    }

    public void CleanUp()
    {
        //퀘스트 정보 초기화 - 재접등의 상황
        QuestList.Clear();
    }

    public void SetQuestInfo(PMsgTaskQueryInfoS pmsgTaskQueryInfoS)
    {
        for(int i=0;i< pmsgTaskQueryInfoS.CTaskInfos.Count;i++)
        {
            //있나체크
            QuestInfo questInfo = null;
            if(QuestList.TryGetValue(pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId, out questInfo))
            {
                //있다
                //QuestList[(uint)pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId].unTargetNum = pmsgTaskQueryInfoS.CTaskInfos[i].UnTargetNum;
                //QuestList[(uint)pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId].unComplete = pmsgTaskQueryInfoS.CTaskInfos[i].UnComplete;
                //QuestList[(uint)pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId].unFetchBonus = pmsgTaskQueryInfoS.CTaskInfos[i].UnFetchBonus;
                questInfo.unTargetNum = pmsgTaskQueryInfoS.CTaskInfos[i].UnTargetNum;
                questInfo.unComplete = pmsgTaskQueryInfoS.CTaskInfos[i].UnComplete;
                questInfo.unFetchBonus = pmsgTaskQueryInfoS.CTaskInfos[i].UnFetchBonus;

                //UI도 업데이트 해야한다.
            }
            else
            {
                //없다 - 추가
                questInfo = new QuestInfo();
                questInfo.unTaskId = pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId;
                questInfo.unTargetNum = pmsgTaskQueryInfoS.CTaskInfos[i].UnTargetNum;
                questInfo.unComplete = pmsgTaskQueryInfoS.CTaskInfos[i].UnComplete;
                questInfo.unFetchBonus = pmsgTaskQueryInfoS.CTaskInfos[i].UnFetchBonus;
                QuestList.Add(pmsgTaskQueryInfoS.CTaskInfos[i].UnTaskId, questInfo);
            }
        }
    }

    public void QuestComplet(uint QuestID)
    {
        //퀘스트 완료처리
        NetworkClient.instance.SendPMsgTaskCompleteC(QuestID);

    }
    /*
    public Quest.QuestInfo GetCureentQuest()
    {
        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {


            return _LowDataMgr.instance.GetLowDataQuestData(enumerator.Current.Value.unTaskId);
        }
    }
    */

    //퀘스트 완료후 처리해야할것들 - 
    public void QuestCompleteAfterProcess(uint questID, bool success = false)
    {
        Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(questID);
        if(questInfo != null)
        {
            if(questInfo.QuestType == (byte)QuestSubType.NPCTALK)
            {
                //아무일없다
            }
            else if(questInfo.QuestType == (byte)QuestSubType.SINGLEGAMEPLAY)
            {
                //게임 시작 처리를 이어서 한다
				(G_GameInfo._GameInfo as SingleGameInfo).OverrideGameStart(questID);
			}
			else if (questInfo.QuestType == (byte)QuestSubType.SINGLEGAMECLEAR)
            {
                if (TownState.TownActive)//소탕으로 클리어 후
                {   //토크씬이 나왔었다면 챕터페널이 닫혀있을것이다. 다시 열어준다.
                    UIBasePanel chapterPanel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
                    if (chapterPanel != null)
                        UIMgr.OpenChapter(null);
                }
                else
                {
					if(0 < (G_GameInfo.GameInfo as SingleGameInfo).QuestTalkId){
                        UIMgr.Open("UIPanel/ResultRewardStarPanel", true);//무조건 승리겠지?? 아니면 어떻게하지
                    }
                }
            }
        }
        //퀘스트가 없으면 그냥 리턴
    }

    //해당 트리거를 체크하고 맞다면 다음 퀘스트로 이전/토크신을띄움
    public Quest.QuestInfo CheckSubQuest(QuestSubType subtype, uint questObject)
    {
        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(enumerator.Current.Value.unTaskId);

            if(questInfo != null)
            {
                if(enumerator.Current.Value.unComplete != 1)
                {
                    //일단 토크만 처리
                    if (subtype == QuestSubType.NPCTALK)
                    {
                        if (questInfo.QuestType == (byte)QuestSubType.NPCTALK && questObject == questInfo.ParamId)
                        {
                            //체크상태가 맞음 QuestTalkSceneID을 활성화 종료시 SubQuestComplet(); 호출
                            //isNPC = false;
                            if (questInfo.QuestTalkSceneID != 0)
                            {
                                //퀘스트 대화씬 열고 그이후에나 종료
                                //UIMgr.OpenMissionPanel(enumerator.Current.Value.unTaskId);
                            }
                            else
                            {
                                //퀘스트 완료
                                QuestComplet(enumerator.Current.Value.unTaskId);
                            }

                            return questInfo;
                        }
                    }
                    else if (subtype == QuestSubType.SINGLEGAMEPLAY)
                    {
                        if (questInfo.QuestType == (byte)QuestSubType.SINGLEGAMEPLAY && questObject == questInfo.ParamId)
                        {
                            //체크상태가 맞음 QuestTalkSceneID을 활성화 종료시 SubQuestComplet(); 호출
                            if (questInfo.QuestTalkSceneID != 0)
                            {
                                //UIMgr.OpenMissionPanel(enumerator.Current.Value.unTaskId);
                            }
                            else
                            {
                                QuestComplet(enumerator.Current.Value.unTaskId);
                            }
                            return questInfo;
                        }
                    }
                    else if (subtype == QuestSubType.SINGLEGAMECLEAR)
                    {
                        if (questInfo.QuestType == (byte)QuestSubType.SINGLEGAMECLEAR && questObject == questInfo.ParamId)
                        {
                            //체크상태가 맞음 QuestTalkSceneID을 활성화 종료시 SubQuestComplet(); 호출
                            if (questInfo.QuestTalkSceneID != 0)
                            {
                                //UIMgr.OpenMissionPanel(enumerator.Current.Value.unTaskId);

                                if(TownState.TownActive)//소탕으로 클리어일 것이다.
                                {
                                    UIBasePanel chapterPanel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
                                    if (chapterPanel != null)
                                        chapterPanel.Hide();
                                }
                            }
                            else
                            {
                                QuestComplet(enumerator.Current.Value.unTaskId);
                            }
                            return questInfo;
                        }
                    }
                }                
            }            
        }

        return null;
    }

    public Quest.QuestInfo GetCurrentQuest()
    {
        QuestInfo quest = GetCurrentQuestInfo();
        if(quest != null)
        {
            Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(quest.unTaskId);
            if (questInfo != null)
            {
                return questInfo;
            }
        }

        return null;
    }

	// 현재 진행중인 퀘스트만 찾는다. 
	// 다음 퀘스트는 searchNextQuest로 서버에 요청하여 받아야 한다. 2017.10.18.kyh
    public QuestInfo GetCurrentQuestInfo()
    {
        //현재 진행중인 퀘스틀 찾아서 리턴해주는데 어떻게 알지? - 일단 Complet가 0인걸 찾음, 못찾았을경우 Complet는 1인데 Fetch가 0인걸 찾음
        QuestInfo lastQuest = null;
        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            lastQuest = enumerator.Current.Value;//마지막께 들어있을 것이다.
            if (lastQuest.unComplete == 0)
            {
                //진행중인 퀘스트
                return lastQuest;
            }
        }

        //여기까지 왔다 그럼 Complet가 0인걸 찾지못함 이제 Complet는 1이고 Fetch가 0인걸 찾자
        enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.unComplete == 1 && enumerator.Current.Value.unFetchBonus == 0)
            {
                //퀘스트는 완료되었고 보상은 안받은 퀘스트
                return enumerator.Current.Value;
            }
        }

        //진행중인 퀘스트가 없다.
        return null;
    }

    /// <summary> 내가 보유한 마지막 퀘스트의 다음 퀘스트 넘김. </summary>
    public Quest.QuestInfo GetCurNextQuestInfo()
    {
        //내가 보유한 퀘스트중 마지막것을 찾는다.
        QuestInfo lastQuest = null;
        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            lastQuest = enumerator.Current.Value;
            //다 클리어 했어햐함 && 보상까지
            if (lastQuest.unComplete == 0 || (lastQuest.unComplete == 1 && lastQuest.unFetchBonus == 0) )
            {
                return null;
            }
        }

        if (lastQuest != null)
        {
            //진행중인 퀘스트가 없다.
            Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(lastQuest.unTaskId);
            if (questInfo != null)
                return _LowDataMgr.instance.GetLowDataQuestData(questInfo.NextQuestId);
        }

        return null;
    }


    public QuestInfo GetLastQuestInfo()
	{
		QuestInfo lastQuest;
		var enumerator = QuestList.GetEnumerator();
		lastQuest = null;
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value.unComplete == 1)
			{
				lastQuest = enumerator.Current.Value;
			}
		}
		
		return lastQuest;
	}

	public QuestInfo GetLastestUnCompleteQuestInfo()
	{
		QuestInfo nextQuest;
		var enumerator = QuestList.GetEnumerator();
		nextQuest = null;
		while (enumerator.MoveNext())
		{
			uint unComplete = enumerator.Current.Value.unComplete;

			// find last unComplete value;
			if (enumerator.Current.Value.unComplete == 0){
				nextQuest = enumerator.Current.Value;
			}
		}
		
		return nextQuest;
	}
	
	//접속시에 이전 퀘스트가 끊긴게 없는지 확인
	public void CheckMissingQuest()
	{
		//일단 최후 퀘스트를 찾는다 - 이론상 항상 최후의 퀘스트만 할수있다
		if(QuestList.Count != 0)
		{
			uint MaxKey = QuestList.Keys.Max();
            QuestInfo questInfo = null;

            if (QuestList.TryGetValue(MaxKey, out questInfo))
            {
                // 해당퀘스트가 완료상태인가 아니면 패스
                if (questInfo.unComplete == 1)
                {
                    //완료상태면 보상을 받은지 체크
                    if (questInfo.unFetchBonus != 1)
                    {
                        //완료상태고 보상을 받지 않은 상태라면 보상요청
                        Quest.QuestInfo lowInfo = _LowDataMgr.instance.GetLowDataQuestData(questInfo.unTaskId);
                        if (lowInfo.LimitLevel <= NetData.instance.UserLevel)
                        {
                            NetworkClient.instance.SendPMsgTaskFetchBonusC(questInfo.unTaskId);
                        }
                    }
                    else
                    {
                        //완료상태고 보상을 받은상태라면 끊어진 퀘스트를 이어서 요청
                        SearchNextQuest(questInfo.unTaskId);
                    }
                }
            }
        }
    }

    public void SearchNextQuest(uint QuestID)
    {
        //현재퀘스트의 다음퀘스틀 찾아서 신청하자
        Quest.QuestInfo quest = null;

        if (_LowDataMgr.instance.GetQuestData().QuestInfoDic.TryGetValue(QuestID, out quest))
        {
            //퀘스트를 찾았으면 다음퀘스트를 받자
            if (0 < quest.NextQuestId)
            {
                Quest.QuestInfo q = _LowDataMgr.instance.GetLowDataQuestData(quest.NextQuestId);
                if (q != null && q.LimitLevel <= _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MaxLevel))

                {
                    if (q.LimitLevel <= NetData.instance.UserLevel)
                    {
                        QuestInfo questInfo = null;
                        if (QuestList.TryGetValue(quest.ID, out questInfo))
                        {
                            if(questInfo.unComplete == 1)
                                NetworkClient.instance.SendPMsgTaskReceiveTaskC(quest.NextQuestId);
                            else
                                NetworkClient.instance.SendPMsgTaskReceiveTaskC(quest.ID);
                        }
                        else
                            NetworkClient.instance.SendPMsgTaskReceiveTaskC(quest.NextQuestId);
                    }
                    else//레벨이 부족하여 진행을 할 수 없음.
                    {
                        UIBasePanel panel = UIMgr.GetTownBasePanel();
                        if (panel != null)
                            (panel as TownPanel).MissionListSetting();//q != null ? q.LimitLevel : (byte)0
                    }
                }
                else//최대래벨 넘어가는 퀘스트는 더이상 진행 없다.
                {
                    UIBasePanel panel = UIMgr.GetTownBasePanel();
                    if (panel != null)
                        (panel as TownPanel).MissionListSetting();
                }
            }


        }

        /*
        //다음퀘스트를 찾는다-완료안된 퀘스트가 있을경우 찾지않는다 - 보상안받은것도 여기서 처리해아하나
        //bool bUnCompletQuest = false;

        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if(enumerator.Current.Value.unComplete == 0)
            {
                //bUnCompletQuest = true;
                return;
            }
        }

        //보상안받은거 일괄적으로 다 보상받기 - 원래라면 해당 NPC한테 가서 받아야할거같은데
        enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.unComplete == 1 && enumerator.Current.Value.unFetchBonus == 0)
            {
                NetworkClient.instance.SendPMsgTaskFetchBonusC(enumerator.Current.Value.unTaskId);
                return;
            }
        }

        //여기까지 왔으면 모든 퀘스트가 완료된 상태 - 가장 숫자가 큰 퀘스트 ID를 찾고 해당 퀘스트를 수령하자
        uint maxQuestID = 0;
        enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value.unComplete == 1 && enumerator.Current.Value.unFetchBonus == 0)
            {
                if(maxQuestID < enumerator.Current.Value.unTaskId )
                {
                    maxQuestID = enumerator.Current.Value.unTaskId;
                }
            }
        }

        var quest_enumerator = _LowDataMgr.instance.GetQuestData().QuestInfoDic.GetEnumerator();
        //돌면서 하나라도 큰 퀘스트를 받자 - 원래는 렙이나 다음퀘스트 체크를 해야함
        while (quest_enumerator.MoveNext())
        {
            if (quest_enumerator.Current.Value.ID < maxQuestID)
            {
                //큰거에서 바로 멈춤 퀘스트 시작

            }
        }
        */
    }

    public void SetTownPanel(TownPanel panel)
    {
        TownPanelScript = panel;
    }

    /*
    public void SetQuest(uint questID, uint subID)
    {

        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Key == questID)
            {
                currentQuest = enumerator.Current.Value;

                for(int i=0;i< currentQuest.subQuest.Count;i++)
                {
                    if(currentQuest.subQuest[i].Id == subID)
                    {
                        currentSubQuest = currentQuest.subQuest[i];
                        return;
                    }
                }
            }
        }
    }

    public void SetMissionPanel()
    {
        if(currentQuest != null && currentSubQuest != null)
        {
            //미션패널에서 현재퀘스트의 정보를 찍어줌
        }
    }

    public void FindQuest(uint questid, uint subquestid, out QuestData quest, out QuestSubData subquest)
    {
        quest = null;
        subquest = null;

        var enumerator = QuestList.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Key == questid)
            {
                quest = enumerator.Current.Value;

                for (int i = 0; i < quest.subQuest.Count; i++)
                {
                    if (quest.subQuest[i].Id == subquestid)
                    {
                        subquest = quest.subQuest[i];
                        return;
                    }
                }
            }
        }
    }


    //questid, subquestid의 다음퀘스트를 찾는다
    public void FindNextQuest(uint questid, uint subquestid, byte isReward, bool firstLoad )
    {
        uint nextSubQuestID = subquestid + 1;


        QuestData quest;
        QuestSubData subquest;

        FindQuest(questid, nextSubQuestID, out quest, out subquest);

        for (int i = 0; i < quest.subQuest.Count; i++)
        {
            if (nextSubQuestID == quest.subQuest[i].Id)
            {
                //있을경우 해당 서브퀘스트로 변경 - 없을경우 이번퀘스트는 완료를 요청해야함

                QuestSubType type = QuestSubType.NPCTALK;

                if (!firstLoad)
                    type = currentSubQuest.QuestType;

                currentQuest = quest;
                currentSubQuest = quest.subQuest[i];

                if (TownState.TownActive)
                    TownPanelScript.MissionListSetting();

                if(!firstLoad)
                {
                    if (type == QuestSubType.NPCTALK)
                    {

                    }
                    else if (type == QuestSubType.SINGLEGAMEPLAY)
                    {
                        //G_GameInfo.GameInfo.GameStart();
                        (G_GameInfo._GameInfo as SingleGameInfo).OverrideGameStart();
                    }
                    else if (type == QuestSubType.SINGLEGAMECLEAR)
                    {
                        //플래그 자체가 성공일때니 성공만 들어옴
                        (G_GameInfo._GameInfo as SingleGameInfo).OverrideGameEnd(true);
                    }
                }                

                return;
            }
        }

        //퀘스트 정보가 없다
        if( isReward == 0)
        {
            //리워드를 받아야하는 단계 - 이전퀘스트 인데 어떻게?
            if(quest != null && subquest == null)
            {
                currentQuest = quest;
                currentSubQuest = null;

                if (firstLoad)
                {
                    //퀘스트 정보는 있으므로 해당 퀘스트의 완료를 하자
                    if (TownState.TownActive)
                        TownPanelScript.MissionListSetting();
                }
                else
                {
                    //바로 완료 창을 띄워주자
                    HttpSender.instance.ReqQuestReward(() =>
                    {
                        //무조건 완료인가
                        HttpSender.instance.ReqQuestInfo(() =>
                        {

                        });
                    });
                }                
            }             
        }
        else
        {
            //완료한 상태임 다음퀘스트로
            currentQuest = quest;
            SetNextQuest();
        }
            
    }

    //현재의 퀘스트를 완료하고 다음 서브미션으로 진행 서브미션이 완료되었을땐 리워드 팝업호출
    public void SubQuestComplet()
    {
        //서버로 해당 서브미션 완료 메세지를 보낸다

        HttpSender.instance.ReqQuestClear(currentQuest.Id, currentSubQuest.Id, () =>{
            
        });
        //uint nextSubQuestID = currentSubQuest.Id +1;

        //for (int i=0;i< currentQuest.subQuest.Count;i++)
        //{
        //    if(nextSubQuestID == currentQuest.subQuest[i].Id)
        //    {
        //        //찾았으니 다음 서브퀘스트로 변경

        //        QuestSubType type = currentSubQuest.QuestType;

        //        currentSubQuest = currentQuest.subQuest[i];

        //        if(TownState.TownActive)
        //            TownPanelScript.MissionListSetting();


        //        if (type == QuestSubType.NPCTALK)
        //        {
                    
        //        }
        //        else if (type == QuestSubType.SINGLEGAMEPLAY)
        //        {
        //            //G_GameInfo.GameInfo.GameStart();
        //            (G_GameInfo._GameInfo as SingleGameInfo).OverrideGameStart();
        //        }
        //        else if (type == QuestSubType.SINGLEGAMECLEAR)
        //        {
        //            //플래그 자체가 성공일때니 성공만 들어옴
        //            (G_GameInfo._GameInfo as SingleGameInfo).OverrideGameEnd(true);
        //        }

        //        return;
        //    }
        //}
        
        ////못찾았으니 현재의 퀘스트 완료하고 리워드 팝업을 호출하자
        //SetNextQuest();
    }

    public void SetNextQuest()
    {
        //리워드 팝업 호출하고 다음퀘스트 시작

        
        //다음퀘스트 셋팅

        if( QuestList.ContainsKey(currentQuest.nextid) )
        {
            currentQuest = QuestList[currentQuest.nextid];
            currentSubQuest = currentQuest.subQuest[0];
        }
        else
        {
            currentQuest = null;
            currentSubQuest = null;
        }


        if (TownState.TownActive)
            TownPanelScript.MissionListSetting();

    }

    

    /// <summary>
    /// 콜라이더와 충돌이 나면 멈추게 하고 챕터로 이동 시킨다
    /// </summary>
    public void OpenChapter()
    {
        //챕터에서 열리고 나서 초기화함.
        //SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();

        TownPanelScript.OpenChapter();
        TownNpcMgr.instance.ChangeStateIdle();
    }
    */
}