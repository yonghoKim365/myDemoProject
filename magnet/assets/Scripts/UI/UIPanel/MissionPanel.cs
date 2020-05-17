using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionPanel : UIBasePanel {
    // 0 : my , 1 : target
    public UITexture[] ModelImgs;

    /// <summary>
    /// Labels의 인덱스
    /// </summary>
    enum labelT { MyName, TargetName, MTitle, TalkerName, Talk, Btn, }
    /// <summary>
    /// 패널에서 쓰이는 라벨들
    /// </summary>
    public UILabel[] Labels;
    /// <summary>
    /// Btns의 인덱스
    /// </summary>
    enum btnT { Close, Next }
    /// <summary>
    /// 패널의 버튼들
    /// </summary>
    public UIButton[] Btns;

    public GameObject LeftModelPos;
    public GameObject RightModelPos;

    public string[] beforeModel;
    public GameObject[] beforeModelObject;

    UIBasePanel beforePanel = null;
    public TypewriterEffect typewriterEffect = null;

    public enum PortraitPosition
    {
        LEFT,
        RIGHT,
    }
    /*
    public class QuestTalkScene
    {
        public uint SceneID;
        public uint Sequence;
        public PortraitPosition Position;
        public string npcPref;
        public string TalkString;
    }
    */

    //private bool IsTutorial;

    public override void Init()
    {
	    base.Init();

        /*
        if(TownState.TownActive)
        {
            GameObject town = UIMgr.GetUI("UIPanel/TownPanel");
            if (town != null)
            {
                beforePanel = town.GetComponent<TownPanel>();
                beforePanel.Hide();
            }
        }
        else
        {
            GameObject ingame = UIMgr.GetUI("UIPanel/InGameHUDPanel");
            if (ingame != null)
            {
                beforePanel = ingame.GetComponent<InGameHUDPanel>();
                beforePanel.Hide();
            }
        }
        */

        beforeModel = new string[2];
        beforeModel[0] = "";
        beforeModel[1] = "";
        beforeModelObject = new GameObject[2];
        
        UIEventListener.Get(Btns[(int)btnT.Close].gameObject).onClick = CloseClkEvent;
	    UIEventListener.Get(Btns[(int)btnT.Next].gameObject).onClick = NextClkEvent;
    }

    //MissionData curMdata { get { return MissionMgr.instance.CurMission; } }
    //MSubData curMsub { get { return MissionMgr.instance.CurMSub; } }
    //List<MSubData> curMsubList { get { return MissionMgr.instance.CurMSubList; } }

    uint currentID = 0;
    List<Quest.QuestTalkSceneInfo> talkScene;
    //NPCDATA npcdata;
    uint sceneID;
    uint questID;
    Quest.QuestInfo questInfo;

    private int IsTestQuest = 0;

    public override void LateInit()
    {
	    base.LateInit();

        //uiMgr.SetOverCamMask(true, LayerMask.NameToLayer("UI"));

        questID = (uint)parameters[0];
        //IsTutorial = (bool)parameters[1];
        IsTestQuest = (int)parameters[1];

        //uint talkSceneId = 0;
        //if (!IsTutorial)
        //{
        Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(questID);

        if (questInfo.QuestType == (byte)QuestSubType.NPCTALK)
        {
            GameObject town = UIMgr.GetUI("UIPanel/TownPanel");
            if (town != null)
            {
                beforePanel = town.GetComponent<TownPanel>();
                beforePanel.Hide();
            }
        }
            
        //}
        //else
        //{
        //    Quest.TutorialInfo tutorial = _LowDataMgr.instance.GetLowDataTutorial(questID);
        //    talkSceneId = tutorial.QuestTalkSceneID;
        //}

        talkScene = _LowDataMgr.instance.GetQuestTalk(questInfo.QuestTalkSceneID);
        currentID = 0;

        //完成		     //완료
        //延续	 	     //계속
        //快捷键		//바로가기
        if (talkScene == null || talkScene.Count == 0)
            NextClkEvent(null);
        else
            MissionUpdate();
    }

    void MissionUpdate()
    {
        
        Labels[(int)labelT.MTitle].text = "";
        string Name = talkScene[(int)currentID].NPCNameRIGHT;

        //string talkername = "";

        if(Name.Equals("0"))
        {
            Name = "";
        }
        
        if (Name.Contains("PLAYER"))
        {
            Name = Name.Replace("PLAYER", NetData.instance.Nickname);
        }
        //오른쪽
        if (!string.IsNullOrEmpty(Name))
        {
            Labels[(int)labelT.MyName].gameObject.SetActive(true);
            Labels[(int)labelT.MyName].text = Name;
        }
        else
            Labels[(int)labelT.MyName].gameObject.SetActive(false);
            
        Name = talkScene[(int)currentID].NPCNameLEFT;

        if (Name.Equals("0"))
        {
            Name = "";
        }

        {
            if (Name.Contains("PLAYER"))
            {
                Name = Name.Replace("PLAYER", NetData.instance.Nickname);
            }
            //왼쪽

            if (!string.IsNullOrEmpty(Name))
            {
                Labels[(int)labelT.TargetName].gameObject.SetActive(true);
                Labels[(int)labelT.TargetName].text = Name;
            }
            else
                Labels[(int)labelT.TargetName].gameObject.SetActive(false);
        }
        
        string ModelName = talkScene[(int)currentID].NPCModelRIGHT;

        GameObject talker;

        if (!ModelName.Equals("0") && !string.IsNullOrEmpty(ModelName))
        {
            if (!beforeModel[0].Equals(ModelName))
            {
                if (ModelName.Contains("PLAYER"))
                {
                    uint charIdx = NetData.instance.GetUserInfo().GetCharIdx();
                    uint costumeIdx = 0;
                    NetData._CostumeData equipCostumeData = NetData.instance.GetUserInfo().GetEquipCostume();
                    if (equipCostumeData == null)
                    {
                        Newbie.NewbieInfo newbieinfo = _LowDataMgr.instance.GetNewbieCharacterData(NetData.instance.GetUserInfo()._userCharIndex);
                        costumeIdx = newbieinfo.CostumIdx;
                    }
                    else
                        costumeIdx = equipCostumeData._costmeDataIndex;

                    talker = UIHelper.CreatePcUIModel("MissionPanel",LeftModelPos.transform
                        , charIdx, 0, costumeIdx, 0, 0, NetData.instance.GetUserInfo().GetEquipSKillSet().SkillSetId, 3, NetData.instance.GetUserInfo().isHideCostum, false);
                }
                else
                {
                    talker = UIHelper.CreateTalkNPC("MissionPanel",LeftModelPos.transform, ModelName, false, false);
                    ChangeNpcModelSize(talker, true);
                }

                beforeModelObject[0] = talker;
                beforeModel[0] = ModelName;
            }

            if (talkScene[(int)currentID].TalkPosition == 0)
            {
                SkinnedMeshRenderer[] ren = beforeModelObject[0].GetComponentsInChildren<SkinnedMeshRenderer>(true);

                ChangeShader(true, ren);

                Vector3 pos = LeftModelPos.transform.localPosition;
                pos.z = 500;
                LeftModelPos.transform.localPosition = pos;

                Labels[(int)labelT.MyName].depth = -5;
            }
            else
            {
                SkinnedMeshRenderer[] ren = beforeModelObject[0].GetComponentsInChildren<SkinnedMeshRenderer>(true);

                ChangeShader(false, ren);

                Vector3 pos = LeftModelPos.transform.localPosition;
                pos.z = -900;
                LeftModelPos.transform.localPosition = pos;

                Labels[(int)labelT.MyName].depth = 5;
            }
        }

        ModelName = talkScene[(int)currentID].NPCModelLEFT;
        if (!ModelName.Equals("0") && !string.IsNullOrEmpty(ModelName))
        {
            if (!beforeModel[1].Equals(ModelName))
            {
                if (ModelName.Contains("PLAYER"))
                {
                    uint costumeIdx = 0;
                    NetData._CostumeData equipCostumeData = NetData.instance.GetUserInfo().GetEquipCostume();
                    if (equipCostumeData == null)
                    {
                        Newbie.NewbieInfo newbieinfo = _LowDataMgr.instance.GetNewbieCharacterData(NetData.instance.GetUserInfo()._userCharIndex);
                        costumeIdx = newbieinfo.CostumIdx;
                    }
                    else
                        costumeIdx = equipCostumeData._costmeDataIndex;

                    talker = UIHelper.CreatePcUIModel("MissionPanel",LeftModelPos.transform
                        , NetData.instance.GetUserInfo().GetCharIdx(), 0, equipCostumeData._costmeDataIndex, 0, 0, NetData.instance.GetUserInfo().GetEquipSKillSet().SkillSetId, 3, NetData.instance.GetUserInfo().isHideCostum, false);

                   // ChangePcModelSize(talker);
                }
                else
                {
                    talker = UIHelper.CreateTalkNPC("MissionPanel",RightModelPos.transform, ModelName, false, false);
                    ChangeNpcModelSize(talker, false);
                }

                beforeModelObject[1] = talker;
                beforeModel[1] = ModelName;
            }

            if (talkScene[(int)currentID].TalkPosition == 1)
            {
                GetComponent<UIPanel>().renderQueue = UIPanel.RenderQueue.Automatic;
                SkinnedMeshRenderer[] ren = beforeModelObject[1].GetComponentsInChildren<SkinnedMeshRenderer>(true);
                ChangeShader(true, ren);

                Vector3 pos = RightModelPos.transform.localPosition;
                pos.z = 500;
                RightModelPos.transform.localPosition = pos;

                Labels[(int)labelT.TargetName].depth = -5;
            }
            else
            {
                GetComponent<UIPanel>().renderQueue = UIPanel.RenderQueue.StartAt;
                GetComponent<UIPanel>().startingRenderQueue = 3050;

                SkinnedMeshRenderer[] ren = beforeModelObject[1].GetComponentsInChildren<SkinnedMeshRenderer>(true);
                ChangeShader(false, ren);

                Vector3 pos = RightModelPos.transform.localPosition;
                pos.z = -900;
                RightModelPos.transform.localPosition = pos;

                Labels[(int)labelT.TargetName].depth = 5;
            }
        }

        string talk = talkScene[(int)currentID].TalkString;
        if (talk.Contains("\\\\\n"))
            talk = talk.Replace("\\\\\n", "\n");
        else if (talk.Contains("\\\\n"))
            talk = talk.Replace("\\\\n", "\n");
        else if (talk.Contains("\\\n"))
            talk = talk.Replace("\\\n", "\n");
        else if (talk.Contains("\\n"))
            talk = talk.Replace("\\n", "\n");

        //if(IsTutorial)
        //{
        //    if (talk.Contains("{0}"))
        //    {
        //        uint id = 0;
        //        uint charId = NetData.instance.GetUserInfo()._userCharIndex;
        //        if (charId == 13000)
        //            id = 672;
        //        else
        //            id = 671;

        //        talk = string.Format(talk, _LowDataMgr.instance.GetStringCommon(id));
        //    }
        //}
        
        typewriterEffect.ResetToBeginning(SceneManager.instance.IsPlaySoundFx, string.Format("{0}", talk));
		if (SceneManager.instance.testData.bQuestTestStart) {
			TempCoroutine.instance.FrameDelay(1.0f, ()=>{
				NextClkEvent(this.gameObject);
			});
		}

    }
    
    void ChangeShader(bool type, SkinnedMeshRenderer[] ren)//, int a_Type)
    {
        Material a_MM = null;
        float amount = 0;
        
        if (type)
        {
            amount = 0.5f;
        }
        else
        {
            amount = 0f;
        }

        for (int i = 0; i < ren.Length; i++)
        {
            if (0 < ren[i].materials.Length)
            {
                a_MM = ren[i].materials[(ren[i].materials.Length - 1)];
                a_MM.SetColor("_FlashColor", Color.black);
                a_MM.SetFloat("_FlashAmount", amount);
            }
        }
    }

    void NextClkEvent(GameObject go)
    {
        currentID++;
        if (talkScene.Count-1 >= currentID)
        {
            MissionUpdate();
        }
        else
        {
            //QuestManager.instance.QuestComplet(questID);
            if (0 != IsTestQuest)
            {
                Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(questID);
                uint nextQuestId = 0;
                while (true)
                {
                    if (0 < IsTestQuest)
                    {
                        if (questInfo.NextQuestId <= 0)
                            break;

                        questInfo = _LowDataMgr.instance.GetLowDataQuestData(questInfo.NextQuestId);
                    }
                    else
                    {
                        if (questInfo.BeforeQuestId <= 0)
                            break;

                        questInfo = _LowDataMgr.instance.GetLowDataQuestData(questInfo.BeforeQuestId);
                    }

                    if (0 < questInfo.QuestTalkSceneID)
                    {
                        nextQuestId = questInfo.ID;
                        break;
                    }
                }

                if (nextQuestId == 0)
                {
                    Close();
                    return;
                }
                
                Close();
                TempCoroutine.instance.FrameDelay(0.5f, ()=> {
                    UIMgr.OpenMissionPanel(nextQuestId, IsTestQuest);
                });
            }
            else
            {
                Close();
            }
        }
        //끝
	   
    }
    
    void CloseClkEvent(GameObject go)
    {
        //QuestManager.instance.QuestComplet(questID);
        Close();
    }

    //안드로이드 뒤로가기 키가 눌리면 꺼지게 해줘야해서 추가함
    public override void Close()
    {
        if (IsTestQuest == 0)//!IsTutorial && 
        {
            QuestManager.instance.QuestComplet(questID);
        }

        if (SceneManager.instance.CurTutorial == TutorialType.QUEST)//패널이 닫히면 변경해줌.
            SceneManager.instance.CurTutorial = TutorialType.QUEST;

        base.Close();
    }

    public override void OnDestroy()
    {
	    base.OnDestroy();
        
        if(beforePanel != null)
        {
            beforePanel.Show();
        }
    }

    public override PrevReturnType Prev()
    {
	   return base.Prev();
    }

    void ChangeNpcModelSize(GameObject go, bool isRight)
    {

        // 왼쪽은 여기서 재졍의해준다... (left_ + 붙여줌)
        string prefabName = go.name;
        if (!isRight)
        {
            prefabName = string.Format("{0}{1}", "left_", go.name);
        }

        Partner.PartnerScaleInfo info = _LowDataMgr.instance.GetPartnerScaleInfo(prefabName, "MissionPanel");
        if (info == null)
        {
            Debug.Log(string.Format("{0} is null", gameObject.name));

            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = new Vector3(0, 180, 0);
            go.transform.localScale = new Vector3(180, 180, 180);
        }
        else
        {
            go.transform.localPosition = new Vector3(info._x, info._y, 0);
            go.transform.localEulerAngles = new Vector3(info.rotate_x, info.rotate_y, 0);
            go.transform.localScale = new Vector3(info.scale, info.scale, info.scale);

        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        //if (IsTutorial) 
        //    return;

        if (GUI.Button(new Rect(200, 100, 100, 50), "다음 대화"))
        {
            IsTestQuest = 1;
        }

        if (GUI.Button(new Rect(100, 100, 100, 50), "이전 대화"))
        {
            IsTestQuest = -1;
        }
        
    }
#endif
}
