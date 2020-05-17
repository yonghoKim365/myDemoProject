using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheatPopup : UIBasePanel {

    public enum ClearStageState
    {
        None,
        InGame,
        Complete,
    }
    
    public UITextList Desc;
    public UIInput Input;

    public UIButton BtnSendBox;

    public MeshRendererHelper _MeshRendererHelper;
    public ColorPicker _ColorPicker;

    public static ClearStageState CheatStageState;
    public static bool IsStayProtocol;

    public override void Init()
    {
        base.Init();

        EventDelegate.Set(BtnSendBox.onClick, OnClickSend);

        if (UITextList.IsAlready(Desc.gameObject.name))
            return;
        
        Desc.Add(string.Format("코인 : ac 99"));
        Desc.Add(string.Format("원보 : ag 99"));
        Desc.Add(string.Format("우정포인트 : af 99"));
        Desc.Add(string.Format("명예 : ah 99"));
        Desc.Add(string.Format("보석 : aco 99"));
        Desc.Add(string.Format("마계의탑 재화 : ar 99"));
        Desc.Add(string.Format("차관 재화 : alk 99"));
        Desc.Add(string.Format("경험치 : ae 99"));
        Desc.Add(string.Format("아이템 : ai 551001 3 "));
        Desc.Add(string.Format("착 장비 : aq 410001 3"));
        Desc.Add(string.Format("메일 : mal 3"));
        Desc.Add(string.Format("스테미너(원보 필요) : s"));
        Desc.Add(string.Format("소탕권 : sp 3"));
        Desc.Add(string.Format("파트너 조각 : shard n"));

        Desc.Add(string.Format("ex) scene test : single_f001"));
        Desc.Add(string.Format("모험 테스트 : game single_0101  주의) 강제이기때문에 네트워크문제가 생길수있음"));

        Desc.Add(string.Format("무적모드 : qq"));
        Desc.Add(string.Format("나만 무적모드 : q"));
        Desc.Add(string.Format("모든 스테이지 클리어 : allclear\n주의 : 결과 알림 창 뜨기전까지 기다려주세요."));
        Desc.Add(string.Format("모든 의상 장비 생성 : allequip\n주의 : 결과 알림 창 뜨기전까지 기다려주세요."));
        Desc.Add(string.Format("퀘스트 : opentask n"));

        Desc.Add(string.Format("페이스북연동리셋 : cfb"));
        Desc.Add(string.Format("구글연동리셋 : cg"));
    }

    public override void LateInit()
    {
        base.LateInit();
        
        byte rValue = 0, gValue = 0, bValue = 0, aValue = 0;
        float sValue = 0;
        SceneManager.instance.GetUnitLight(ref rValue, ref gValue, ref bValue, ref aValue, ref sValue);
        //SceneManager.instance.GetShadowLight(ref rValue, ref gValue, ref bValue, ref aValue, ref sValue);
        transform.FindChild("LightSetting/unit/r").GetComponent<UIInput>().value = rValue.ToString();
        transform.FindChild("LightSetting/unit/g").GetComponent<UIInput>().value = gValue.ToString();
        transform.FindChild("LightSetting/unit/b").GetComponent<UIInput>().value = bValue.ToString();
        transform.FindChild("LightSetting/unit/a").GetComponent<UIInput>().value = aValue.ToString();

        transform.FindChild("LightSetting/unit/intensity").GetComponent<UIInput>().value = sValue.ToString();
        EventDelegate.Set(transform.FindChild("LightSetting/unit/Btn").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            string r = transform.FindChild("LightSetting/unit/r").GetComponent<UIInput>().value;
            string g = transform.FindChild("LightSetting/unit/g").GetComponent<UIInput>().value;
            string b = transform.FindChild("LightSetting/unit/b").GetComponent<UIInput>().value;
            string a = transform.FindChild("LightSetting/unit/a").GetComponent<UIInput>().value;

            string inten = transform.FindChild("LightSetting/unit/intensity").GetComponent<UIInput>().value;

            byte idxR = 0, idxG = 0, idxB = 0, idxA = 0;
            float idxInten = 0;

            byte.TryParse(r, out idxR);
            byte.TryParse(g, out idxG);
            byte.TryParse(b, out idxB);
            byte.TryParse(a, out idxA);
            float.TryParse(inten, out idxInten);

            SceneManager.instance.SetUnitLight(idxR, idxG, idxB, idxA, idxInten);
            //SceneManager.instance.SetShadowLight(idxR, idxG, idxB, idxA, idxInten);
        });

        float streng = 0;
        SceneManager.instance.GetShadowLight(ref streng);
        transform.FindChild("LightSetting/shadow/strength").GetComponent<UIInput>().value = streng.ToString();

        EventDelegate.Set(transform.FindChild("LightSetting/shadow/Btn").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            string s = transform.FindChild("LightSetting/shadow/strength").GetComponent<UIInput>().value;
            float stength = 0;
            float.TryParse(s, out stength);

            SceneManager.instance.SetShadowLight(stength);
        });

        Light light = uiMgr.UIRoot.transform.FindChild("UI_Light").GetComponent<Light>();
        transform.FindChild("LightSetting/ui_unit/r").GetComponent<UIInput>().value = (light.color.r*255).ToString();
        transform.FindChild("LightSetting/ui_unit/g").GetComponent<UIInput>().value = (light.color.g*255).ToString();
        transform.FindChild("LightSetting/ui_unit/b").GetComponent<UIInput>().value = (light.color.b*255).ToString();
        transform.FindChild("LightSetting/ui_unit/a").GetComponent<UIInput>().value = (light.color.a*255).ToString();

        transform.FindChild("LightSetting/ui_unit/intensity").GetComponent<UIInput>().value = light.intensity.ToString();
        EventDelegate.Set(transform.FindChild("LightSetting/ui_unit/Btn").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            string r = transform.FindChild("LightSetting/ui_unit/r").GetComponent<UIInput>().value;
            string g = transform.FindChild("LightSetting/ui_unit/g").GetComponent<UIInput>().value;
            string b = transform.FindChild("LightSetting/ui_unit/b").GetComponent<UIInput>().value;
            string a = transform.FindChild("LightSetting/ui_unit/a").GetComponent<UIInput>().value;

            string inten = transform.FindChild("LightSetting/ui_unit/intensity").GetComponent<UIInput>().value;

            byte idxR = 0, idxG = 0, idxB = 0, idxA = 0;
            float idxInten = 0;

            byte.TryParse(r, out idxR);
            byte.TryParse(g, out idxG);
            byte.TryParse(b, out idxB);
            byte.TryParse(a, out idxA);
            float.TryParse(inten, out idxInten);

            light.color = new Color32(idxR, idxG, idxB, idxA);
            light.intensity = idxInten;
        });

        Light shadowLight = uiMgr.UIRoot.transform.FindChild("UI_ShadowLight").GetComponent<Light>();
        transform.FindChild("LightSetting/ui_shadow/strength").GetComponent<UIInput>().value = shadowLight.shadowStrength.ToString();

        EventDelegate.Set(transform.FindChild("LightSetting/ui_shadow/Btn").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            string s = transform.FindChild("LightSetting/ui_shadow/strength").GetComponent<UIInput>().value;
            float stength = 0;
            float.TryParse(s, out stength);
            shadowLight.shadowStrength = stength;
        });

        _MeshRendererHelper.OnGetColor(_ColorPicker);
        SetColor(_ColorPicker.GetColor() );
        transform.FindChild("Color/Intensity/l").GetComponent<UIInput>().value = string.Format("{0}", _MeshRendererHelper.GetIntensity() );
        EventDelegate.Set(transform.FindChild("Color/Intensity/btn").GetComponent<UIEventTrigger>().onClick, () => {
            float value = float.Parse(transform.FindChild("Color/Intensity/l").GetComponent<UIInput>().value );
            _MeshRendererHelper.SetIntensity(value);
        });

        Desc.enabled = false;
        Desc.enabled = true;
    }
    
    IEnumerator _ClearAllStage()
    {
        List<DungeonTable.StageInfo> stageList = _LowDataMgr.instance.GetStageInfoList();

        CheatStageState = ClearStageState.InGame;
        IsStayProtocol = false;

        int count = 0;
        while(true)
        {
            yield return new WaitForSeconds(0.1f);

            if (IsStayProtocol)
                continue;

            if(CheatStageState == ClearStageState.InGame)
            {
                IsStayProtocol = true;
                NetworkClient.instance.SendPMsgStageStartC((int)stageList[count].StageId);
            }
            else if (CheatStageState == ClearStageState.Complete)
            {
                //yield return new WaitForSeconds(5);
                IsStayProtocol = true;
                int[] clearStar = new int[] {
                    1,1,1
                };
                NetworkClient.instance.SendPMsgStageCompleteC((int)stageList[count].StageId, 1, clearStar);

                ++count;
            }
            //else if(CheatStageState == ClearStageState.SelectFlop)
            //{
            //    IsStayProtocol = true;
            //    //NetworkClient.instance.SendPMsgStageFlopC((uint)startStageId[count], false, 1);
            //    ++count;
            //}

            if (stageList.Count <= count)
                break;
        }

        UIMgr.instance.AddPopup("알림", "스테이지 클리어 치트키 끝", "확인", null, null, delegate() {
            base.Close();
        });
    }

    IEnumerator QuestClear()
    {
        CheatStageState = ClearStageState.Complete;
        IsStayProtocol = false;

        Quest.QuestInfo curInfo = QuestManager.instance.GetCurrentQuest();
        Dictionary<uint, Quest.QuestInfo> dic = _LowDataMgr.instance.GetQuestData().QuestInfoDic;
        var data = dic.GetEnumerator();
        
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (IsStayProtocol)
                continue;

            if (CheatStageState == ClearStageState.Complete)
            {
                if (!data.MoveNext())
                    break;
                else
                {
                    IsStayProtocol = true;
                    NetworkClient.instance.SendPMsgTaskCompleteC(data.Current.Key);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        UIMgr.instance.AddPopup("알림", "스테이지 클리어 치트키 끝", "확인", null, null, delegate () {
            base.Close();
        });
    }

    IEnumerator CreateEquipment()
    {
        uint charId = NetData.instance.GetUserInfo().GetCharIdx();
        uint[] equipId;
        if (charId == 11000)
            equipId = new uint[] { 410002, 410012, 410022, 410032, 410042, 410102, 410112, 410122, 410132, 410142,
                410202, 410212, 410222, 410232, 410242, 410302, 410312, 410322, 410332, 410342, 410402, 410412,
                410422, 410432, 410442, 410502, 410512, 410522, 410532, 410542, 410602, 410612, 410622, 410632,
                410642, 410702, 410712, 410722, 410732, 410742, 410652, 410752, 410852, 510001
            };
        else if (charId == 12000)
            equipId = new uint[] {
                420002,420012,420022,420032,420042,420102,420112,420122,420132,420142,420202,420212,420222,420232,420242,420302,
                420312,420322,420332,420342,420402,420412,420422,420432,420442,420502,420512,420522,420532,420542,
                420602,420612,420622,420632,420642,420702,420712,420722,420732,420742,420652,420752,420852,520001
            };
        else
            equipId = new uint[] {
                430002,430012,430022,430032,430042,430102,430112,430122,430132,430142,
                430202,430212,430222,430232,430242,430302,430312,430322,430332,430342,430402,430412,430422,430432,430442,430502,430512,
                430522,430532,430542,430602,430612,430622,430632,430642,430702,430712,430722,430732,430742,430652,430752,430852,530001
            };

        int count = 0;
        _LowDataMgr lowMgr = _LowDataMgr.instance;
        while (count < equipId.Length)
        {
            Item.EquipmentInfo item = lowMgr.GetLowDataEquipItemInfo(equipId[count] );
            if (item == null)
                break;

            NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", string.Format("/aq {0} 1", equipId[count++]), 1);
            
            yield return new WaitForSeconds(0.1f);
        }

        UIMgr.instance.AddPopup("알림", "의상 생성 치트키 끝", "확인", null, null, delegate () {
            base.Close();
        });
    }

    void CreateMaterials(string type, int amount)
    {
        int[] ids = null;
        if (type.Equals("asd"))
        {
            ids = new int[] {
                554009,554010,554011,554012,554013,554014,
                560450,560451,560452,560453,560454,560455,560456,
                560461,560462,560463,560464,560465,560466,560467,560468,560469,
                560470,560471,560472,560473,560474,560475,560476,560477,560478,560479,560480,
                560500,560510,560511,560512,560513,
                560530,560531,560532,560533,
                560550,560551,560558,560559,560560,567700,567701,599008
            };
        }
        else if(type.Equals("shard"))
        {
            List<int> idList = new List<int>();
            List<Partner.PartnerDataInfo> parList = _LowDataMgr.instance.GetPartnerDataToList();
            for(int i=0; i < parList.Count; i++)
            {
                if (idList.Contains((int)parList[i].ShardIdx))
                    continue;

                idList.Add((int)parList[i].ShardIdx);
            }

            ids = idList.ToArray();
        }
        else if (type.Equals("costume"))
        {
            uint charIdx = NetData.instance.GetUserInfo()._userCharIndex;
            if(charIdx.Equals(11000))
            {
                //권사 코스튬 조각
                ids = new int[] {
                    560100,
                    560101,
                    560102
                };
            }
            else if (charIdx.Equals(12000))
            {
                //명장 코스튬 조각
                ids = new int[] {
                    560200,
                    560201,
                    560202
                };
            }
            else if (charIdx.Equals(13000))
            {
                //협녀 코스튬 조각
                ids = new int[] {
                    560300,
                    560301,
                    560302
                };
            }
        }

        for (int i = 0; i < ids.Length; i++)
        {
            string key = string.Format("/ai {0} {1}", ids[i], amount);
            NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", key, 1);
        }
    }

    void OnClickSend()
    {
        string str = Input.value;
        if (string.IsNullOrEmpty(str))
            return;

        if( str.Contains(" ") && !str.Contains("game"))//&& (str.Contains(" ") || str.CompareTo("s") == 0 ) )
        { 
            string[] value = str.Split(' ');
            if (value.Length <= 0)
                return;
            switch(value[0])
            {
                case "ag": case "ac": case "af": case "ah":
                case "aco": case "ar": case "alk": case "ae":
                    {
                        uint amount = uint.Parse(value[1]);
                        string send = string.Format("/{0} {1}", value[0], amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;
                case "ai": case "aq" : case "act" :
                    {
                        uint id = uint.Parse(value[1]);
                        uint amount = uint.Parse(value[2]);
                        string send = string.Format("/{0} {1} {2}", value[0], id, amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;
                case "sp":
                    {
                        uint amount = uint.Parse(value[1]);
                        string send = string.Format("/ai 599007 {0}", amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;
                case "mal":
                    {
                        uint amount = uint.Parse(value[1]);
                        string send = string.Format("/{0} {1}", value[0], amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;
                case "s":
                    {
                        //uint amount = uint.Parse(value[1]);
                        //string send = string.Format("/{0} {1}", value[0], amount);
                        //NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                        NetworkClient.instance.SendPMsgBuyPowerC();
                    }
                    break;
                case "ku":
                    {
                        long amount = long.Parse(value[1]);
                        string send = string.Format("/{0} {1}", value[0], amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;
                    
                case "asd":
                case "shard":
                case "costume" :
                    {
                        int amount = int.Parse(value[1]);
                        CreateMaterials(value[0], amount);
                    }
                    break;

                case "preview":
                    {
                        uint amount = uint.Parse(value[1]);
                        SceneManager.instance.SetNoticePanel(NoticeType.GetMailItem, amount);
                    }
                    break;

                case "opentask":
                    {
                        uint amount = uint.Parse(value[1]);
                        string send = string.Format("/{0} {1}", value[0], amount);
                        NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", send, 1);
                    }
                    break;

                case "setskill":
                    uint skillId = uint.Parse(value[1]);

                    NetData.instance.GetUserInfo().GetEquipSKillSet().IsEquip = false;
                    bool isSet = false;
                    List<NetData.SkillSetData> skillData = NetData.instance.GetUserInfo()._SKillSetList;
                    for (int i = 0; i < skillData.Count; i++)
                    {
                        if (!skillData[i].SkillSetId.Equals(skillId))
                            continue;

                        isSet = true;
                        skillData[i].IsEquip = true;
                        break;
                    }

                    if (!isSet)
                    {
                        SkillTables.SkillSetInfo info = _LowDataMgr.instance.GetLowDataSkillSet(skillId);
                        skillData.Add(new NetData.SkillSetData(true, new uint[] { 1, 1, 1, 1 }, new uint[] { info.skill1, info.skill2, info.skill3, info.skill4 }, skillId, 0));
                    }
                    break;
            }
        }
        else
        {
            if(str.Equals("allclear"))
            {
                StartCoroutine("_ClearAllStage");
                return;
            }
            if (str.Equals("questclear"))
            {
                StartCoroutine("QuestClear");
                return;
            }
            else if(str.Equals("allequip"))
            {
                StartCoroutine("CreateEquipment");
                return;
            }
            else if (str.Equals("cg"))
            {
                NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", "/cg", 1);

                string loginType = PlayerPrefs.GetString("logintype", "none");
                string user_id = PlayerPrefs.GetString("user_id", "none");
                string user_token = PlayerPrefs.GetString("user_token", "none");

                if(loginType.Equals("google"))
                {
                    PlayerPrefs.DeleteKey("logintype");
                    PlayerPrefs.DeleteKey("user_id");
                    PlayerPrefs.DeleteKey("user_token");
                    NetData.instance._LoginType = eLoginType.GUEST;
                }

                return;
            }
            else if (str.Equals("cfb"))
            {
                NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, 0, "", "/cfb", 1);

                string loginType = PlayerPrefs.GetString("logintype", "none");
                string user_id = PlayerPrefs.GetString("user_id", "none");
                string user_token = PlayerPrefs.GetString("user_token", "none");

                if (loginType.Equals("facebook"))
                {
                    PlayerPrefs.DeleteKey("logintype");
                    PlayerPrefs.DeleteKey("user_id");
                    PlayerPrefs.DeleteKey("user_token");
                    NetData.instance._LoginType = eLoginType.GUEST;
                }

                return;
            }
            else if (str.Contains("game"))
            {
                string[] split = str.Split(' ');
                if(split[1].Contains("single"))
                {
                    string[] split2 = split[1].Split('_');
                    uint stageNum = 0;
                    if (uint.TryParse(split2[1], out stageNum) )
                    {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.SINGLE, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);

                            SingleGameState.lastSelectStageId = stageNum;
                            SingleGameState.verifyToken = 1023;
                            SingleGameState.IsTest = true;
                            SingleGameState.StageQuestList = new System.Collections.Generic.List<NetData.StageClearData>();
                            SingleGameState.CurStageName = split[1];

                            SceneManager.instance.ActionEvent(_ACTION.PLAY_SINGLE);

                            base.GotoInGame();
                        });
                    }
                }
                else if (split[1].Contains("devildom_tower"))
                {
                    uint stageNum = 0;
                    List<DungeonTable.TowerInfo> list = _LowDataMgr.instance.GetLowDataTowerList();
                    for (int i=0; i < list.Count; i++)
                    {
                        if (!list[i].mapName.Equals(split[1]))
                            continue;

                        stageNum = list[i].StageIndex;
                        break;
                    }

                    if (0 < stageNum)
                    {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.TOWER, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);

                            TowerGameState.lastSelectStageId = stageNum;
                            TowerGameState.IsTest = true;

                            SceneManager.instance.ActionEvent(_ACTION.PLAY_TOWER);
                            base.GotoInGame();
                        });
                    }
                }
                else if (split[1].Contains("Raid_Dungeon"))
                {
                    uint stageNum = 0;
                    List<DungeonTable.SingleBossRaidInfo> list = _LowDataMgr.instance.GetLowDataBossRaidList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!list[i].stageName.Equals(split[1]))
                            continue;

                        stageNum = list[i].raidId;
                        break;
                    }

                    if (0 < stageNum)
                    {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.RAID, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);

                            RaidGameState.lastSelectStageId = stageNum;
                            RaidGameState.IsTest = true;

                            SceneManager.instance.ActionEvent(_ACTION.PLAY_RAID);

                            base.GotoInGame();
                        });
                    }
                }
                else if (split[1].Contains("tower"))
                {
                    string[] split2 = split[1].Split('_');
                    uint stageNum = 0;
                    if (uint.TryParse(split2[1], out stageNum))
                    {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.TOWER, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);

                            TowerGameState.lastSelectStageId = stageNum;
                            TowerGameState.IsTest = true;

                            SceneManager.instance.ActionEvent(_ACTION.PLAY_TOWER);

                            base.GotoInGame();
                        });
                    }
                }
                else if (split[1].Contains("raid"))
                {
                    string[] split2 = split[1].Split('_');
                    uint stageNum = 0;
                    if (uint.TryParse(split2[1], out stageNum))
                    {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.RAID, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);

                            RaidGameState.lastSelectStageId = stageNum;
                            RaidGameState.IsTest = true;

                            SceneManager.instance.ActionEvent(_ACTION.PLAY_RAID);

                            base.GotoInGame();
                        });
                    }
                }
            }
            else if (str.Equals("q") )
            {
                if (!TownState.TownActive)
                    InGameHUDPanel.ZeroDamagePlay_01 = !InGameHUDPanel.ZeroDamagePlay_01;
            }
            else if (str.Equals("qq") )
            {
                if (!TownState.TownActive)
                    InGameHUDPanel.ZeroDamagePlay = !InGameHUDPanel.ZeroDamagePlay;
            }
            else if (str.Equals("cooltime"))
            {
                if (!TownState.TownActive)
                    InGameHUDPanel.ZeroSkillCoolTime = !InGameHUDPanel.ZeroSkillCoolTime;
            }
            else if(str.Equals("tutoreset"))
            {
                string key = string.Format("TutorialData_{0}.json", NetData.instance.GetUserInfo().GetCharUUID());
                if (PlayerPrefs.HasKey(key))
                    PlayerPrefs.DeleteKey(key);
            }
            else
            {
                SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
                {
                    //이상태에서의 데이터를 저장
                    NetData.instance.MakePlayerSyncData(true);

                    TutorialGameState.StageName = str;
                    TutorialGameState.lastSelectStageId = 1;
                    TutorialGameState.verifyToken = 1023;

                    SceneManager.instance.ActionEvent(_ACTION.PLAY_TUTORIAL);

                    base.GotoInGame();
                });
            }

            base.Close();
        }
    }
 
    public void SetColor(Color c)
    {
        transform.FindChild("Color/ccc").GetComponent<UILabel>().text = string.Format("R : {0}\nG : {1}\nB : {2}\nA : {3}", c.r, c.g, c.b, c.a);
    }
}
