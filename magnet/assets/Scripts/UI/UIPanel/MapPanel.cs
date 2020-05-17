using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPanel : UIBasePanel {

    public GameObject DetailMap;
    public GameObject SimpleMap;
    public GameObject SumSimpleMap;
    public GameObject DesGo;
    
    public GameObject NpcListObject;

    public Transform SubGroup;//npc리스트클릭시나올 서브그룹

    public UIGrid Grid;

    public UITexture Detail;
    public UITexture Simple;

    public UILabel Channel;
    public UILabel MapName;
    public UILabel MyPos;   //나의좌표값

    public Transform DetailPosTf;
    public Transform SimplePosTf;

    public Transform NpcRoot;
    public Transform NpcDetailRoot;
    public Transform DotRoot;

    public Vector2[] WorldScale;//0 마을, 1 난투장
    public Vector2[] Offset;//0 마을, 1 난투장
    public Vector2[] MapPointOffset;//0 마을, 1 난투장
    public UILabel[] MapText;//0채널, 1 맵이름

    // 난투장에서 보스위치
    public Transform BossDetailPosTf;
    public Transform BossSimplePosTf;

    public Vector3[] DetailTexturePos;  //0마을(-166,-10,0) 1난투장(0,0,0)

    private Vector3 MovePos = new Vector3();    //이동시킬좌표

    private Unit PlayerUnit;

    private bool IsInGame;
    private string PosStr;

    public override void Init()
    {
        PosStr = _LowDataMgr.instance.GetStringCommon(1217);
        if (TownState.TownActive)
        {
            TownState town = SceneManager.instance.GetState<TownState>();
            PlayerUnit = town.MyHero;

            IsInGame = false;
            Detail.mainTexture = Resources.Load("UI/MapTexture/TownMap") as Texture;
            Simple.mainTexture = Resources.Load("UI/MapTexture/TownMap") as Texture;

            for(int i=0;i<20;i++)
            {
                GameObject NpcGo = Instantiate(NpcRoot.GetChild(0).gameObject)as GameObject;
                NpcGo.transform.parent = NpcRoot;
                NpcGo.transform.localPosition = Vector3.zero;
                NpcGo.transform.localScale = Vector3.one;
                NpcGo.SetActive(false);

                GameObject NpcDetailGo = Instantiate(NpcDetailRoot.GetChild(0).gameObject) as GameObject;
                NpcDetailGo.transform.parent = NpcDetailRoot;
                NpcDetailGo.transform.localPosition = Vector3.zero;
                NpcDetailGo.transform.localScale = Vector3.one;
                NpcDetailGo.SetActive(false);
                
            }
           // Destroy(NpcRoot.GetChild(0).gameObject);
           // Destroy(NpcDetailRoot.GetChild(0).gameObject);

            MapName.text = _LowDataMgr.instance.GetStringCommon(532);
            Channel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1200), 1);//추후에 채널 생기면 정상적인 값 넣어줘야함
        }
        else
        {
            G_GameInfo.CharacterMgr.allrUUIDDic.TryGetValue(NetData.instance.GetUserInfo().GetCharUUID(), out PlayerUnit);
            IsInGame = true;

            Detail.mainTexture = Resources.Load("UI/MapTexture/FreeFightMap") as Texture;
            Simple.mainTexture = Resources.Load("UI/MapTexture/FreeFightMap") as Texture;

            MapName.text = _LowDataMgr.instance.GetStringCommon(12);
            Channel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1200), 1);//추후에 채널 생기면 정상적인 값 넣어줘야함
        }


        DetailPosTf.GetComponent<UISprite>().flip = IsInGame ? UIBasicSprite.Flip.Nothing : UIBasicSprite.Flip.Both;
        SimplePosTf.GetComponent<UISprite>().flip = IsInGame ? UIBasicSprite.Flip.Nothing : UIBasicSprite.Flip.Both;

        EventDelegate.Set(SimpleMap.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            DetailMap.SetActive(true);
            SimpleMap.SetActive(false);
            SumSimpleMap.SetActive(false);

            UIBasePanel chatPopup = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
            if(chatPopup != null)
            {
                chatPopup.gameObject.SetActive(false);
            }

            UIBasePanel townPanel = UIMgr.GetTownBasePanel();
            if (townPanel != null)
                townPanel.gameObject.SetActive(false);

            UIBasePanel hudPanel = UIMgr.GetHUDBasePanel();
            if (hudPanel != null)
            {
                hudPanel.gameObject.SetActive(false);
            }
            UIBasePanel joy = UIMgr.GetUIBasePanel("UIObject/Joystick");
            if (joy != null)
            {
                (joy as Joystick).SetJoyActive(false);
            }



        });

        // 맵안을클릭했을시 이동함수
        EventDelegate.Set(Detail.transform.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            for (int kk = 0; kk < DotRoot.childCount; kk++)
            {
                DotRoot.GetChild(kk).gameObject.SetActive(false);
            }
            
           //  //ui상 좌표를 구함
           // Vector3 touchPos = Vector3.zero;
           // if (0 < Input.touchCount)
           // {
           //     Touch touch = Input.GetTouch(0);
           //     touchPos = uiMgr.UICamera.camera.ScreenToWorldPoint(touch.position);
           // }
           // else
           //     touchPos = uiMgr.UICamera.camera.ScreenToWorldPoint(Input.mousePosition);
            
           // DesGo.transform.position = touchPos;

           // int arr = IsInGame ? 1 : 0;
           // Vector3 pos = DesGo.transform.localPosition;//DetailPosTf.localPosition;
           // Vector2 mapPos = Detail.transform.localPosition;
           // Debug.Log(string.Format("MovePosss {0}, {1}", touchPos, Detail.transform.localPosition));
            
           // pos.x = (pos.x + mapPos.x) - Offset[arr].x;
           // pos.z = (pos.y + mapPos.y) - Offset[arr].y;
           // pos.y = (pos.y + mapPos.y) - Offset[arr].y;

           // pos.x = (pos.x / Detail.localSize.x) * WorldScale[arr].x;
           // pos.z = (pos.z / Detail.localSize.y) * WorldScale[arr].y;
           // pos.y = (pos.y / Detail.localSize.y) * WorldScale[arr].y;
            
           //// 좌표값으로 이동
           // if (PlayerUnit != null)
           // {
           //     pos = -pos;
           //     pos.y = PlayerUnit.transform.position.y;
           //     NavMeshHit hit;
           //     if (NavMesh.SamplePosition(pos, out hit, 10f, -1))
           //     {
           //         pos.y = hit.position.y;
           //         PlayerUnit.MovePosition(pos, 1f);
           //     }
           // }
            
        });

        DetailMap.SetActive(false);
        SimpleMap.SetActive(true);
        SumSimpleMap.SetActive(true);

        BossDetailPosTf.gameObject.SetActive(false);
        BossSimplePosTf.gameObject.SetActive(false);
        EventDelegate.Add(onShow, OnShow);

        NpcRoot.gameObject.SetActive(TownState.TownActive);
        NpcDetailRoot.gameObject.SetActive(TownState.TownActive);
        
        for (int i = 0; i < 20; i++)
        {
            Transform tf = Instantiate(SubGroup.GetChild(0)) as Transform;
            tf.parent = SubGroup;
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.gameObject.SetActive(false);
        }

        EventDelegate.Set(Grid.transform.FindChild("Slot_Func").GetComponent<UIButton>().onClick,
            delegate ()
            {
                OnClickNpcList(0);
            });
        EventDelegate.Set(Grid.transform.FindChild("Slot_Story").GetComponent<UIButton>().onClick,
          delegate ()
          {
              OnClickNpcList(1);
          });


    }

    public override void LateInit()
    {
        base.LateInit();

        if (TownState.TownActive)
        {
            SetTownNpc();
            Detail.transform.localPosition = DetailTexturePos[0];
        }
        
    }
    
    void LateUpdate()
    {
        if (PlayerUnit == null)
            return;

        Vector3 pos = PlayerUnit.cachedTransform.position;
        Vector3 angle = PlayerUnit.cachedTransform.eulerAngles;

        MyPos.text = string.Format("{0} : {1},{2}", PosStr, (int)pos.x, (int)pos.z);

        angle.z = transform.forward.z - angle.y;
        angle.y = 0;
        angle.x = 0;

        int arr = IsInGame ? 1 : 0;
        if (!DetailMap.activeSelf)
        {
            pos.x = (pos.x * Simple.localSize.x) / WorldScale[arr].x;
            pos.y = (pos.z * Simple.localSize.y) / WorldScale[arr].y;
            pos.z = 0;

            SimplePosTf.localEulerAngles = angle;
            if (IsInGame)
                Simple.transform.localPosition = new Vector2(Offset[arr].x - pos.x, Offset[arr].y - pos.y);
            else
                Simple.transform.localPosition = new Vector2(pos.x - Offset[arr].x, pos.y - Offset[arr].y);

        }
        else
        {
            pos.x = (pos.x * Detail.localSize.x) / WorldScale[arr].x;
            pos.y = (pos.z * Detail.localSize.y) / WorldScale[arr].y;
            pos.z = 0;

            Vector2 mapPos = Detail.transform.localPosition;
            pos.x = (pos.x - mapPos.x) - Offset[arr].x;
            pos.y = (pos.y - mapPos.y) - Offset[arr].y;

            DetailPosTf.localEulerAngles = angle;
            if (IsInGame)
                DetailPosTf.localPosition = pos;
            else
                DetailPosTf.localPosition = -pos;
        }

    }

    public void BossDrop()
    {
        NpcListObject.SetActive(false);
        bool isActive = false;


        if (G_GameInfo.GameMode != GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
        {
            isActive = false;
        }
        //여기서 해줘
        if ((G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData() != null && (G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData().Daed == 0)
        {
            isActive = true;

            Vector3 bossPos = NaviTileInfo.instance.GetTilePos((G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData().PosX, (G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData().PosY);

            bossPos.x = (bossPos.x * Simple.localSize.x) / WorldScale[1].x;
            bossPos.y = (bossPos.z * Simple.localSize.y) / WorldScale[1].y;
            //bossPos.x = (bossPos.x * Simple.localSize.x / 2) / WorldScale[1].x;
            //bossPos.y = (bossPos.z * Simple.localSize.x / 2) / WorldScale[1].y;
            bossPos.z = 0;

            BossSimplePosTf.localPosition =/* new Vector3(bossPos.y, bossPos.x, 0);*/ /*new Vector2(Offset[1].x - bossPos.x, Offset[1].y - bossPos.y);*/
                bossPos;
            //Simple.transform.localPosition = new Vector2(Offset[arr].x - pos.x, Offset[arr].y - pos.y);

            bossPos = NaviTileInfo.instance.GetTilePos((G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData().PosX, (G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData().PosY);

            bossPos.x = (bossPos.x * Detail.localSize.x) / WorldScale[1].x;
            bossPos.y = (bossPos.z * Detail.localSize.y) / WorldScale[1].y;
            bossPos.z = 0;


            BossDetailPosTf.localPosition = bossPos;

            BossDetailPosTf.localEulerAngles = new Vector3(0, 0, -180);
 
        }

        else
        {
            isActive = false;
        }

        BossDetailPosTf.gameObject.SetActive(isActive);
        BossSimplePosTf.gameObject.SetActive(isActive);
    }

   public void SetResenMonster(int freefightMapType)
    {
        // 몬스터리젠위치
        MonsterArea.gameObject.SetActive(true);


        List<DungeonTable.FreefightTableInfo> list = _LowDataMgr.instance.GetLowDataFreeFightList();
        DungeonTable.FreefightTableInfo freeInfo = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].StageIndex != freefightMapType)
                continue;

            freeInfo = list[i];
        }

        /*//보류
        for (int i = 0; i < MonsterArea.childCount; i++)
        {
            List<string> resenPos = new List<string>();
            switch (i)
            {
                case 0:
                    resenPos = freeInfo.RegenMob1.list;
                    break;
                case 1:
                    resenPos = freeInfo.RegenMob2.list;
                    break;
                case 2:
                    resenPos = freeInfo.RegenMob3.list;
                    break;
                case 3:
                    resenPos = freeInfo.RegenMob4.list;
                    break;

            }
            
            int resenX = 0;
            int resenY = 0;

            int tmpX = Mathf.Abs((int.Parse(resenPos[0]) - int.Parse(resenPos[2])) / 2);
            if ((int.Parse(resenPos[0]) > int.Parse(resenPos[2])))
                resenX = int.Parse(resenPos[2]) + tmpX;
            else
                resenX = int.Parse(resenPos[0]) + tmpX;


            int tmpY = Mathf.Abs((int.Parse(resenPos[1]) - int.Parse(resenPos[3])) / 2);
            if ((int.Parse(resenPos[1]) > int.Parse(resenPos[3])))
                resenY = int.Parse(resenPos[3]) + tmpY;
            else
                resenY = int.Parse(resenPos[1]) + tmpY;

            Vector3 monsterResenPos = NaviTileInfo.instance.GetTilePos(resenX, resenY);

            monsterResenPos.x = (monsterResenPos.x * Detail.localSize.x) / WorldScale[1].x;
            monsterResenPos.y = (monsterResenPos.z * Detail.localSize.y) / WorldScale[1].y;
            monsterResenPos.z = 0;

            Vector2 mapPos = Detail.transform.localPosition;
            monsterResenPos.x = (monsterResenPos.x - mapPos.x) - Offset[1].x;
            monsterResenPos.y = (monsterResenPos.y - mapPos.y) - Offset[1].y;

            MonsterArea.GetChild(i).localPosition = monsterResenPos;

        }
        */
        Detail.transform.localPosition = DetailTexturePos[1];

    }



    public Transform MonsterArea;
    void OnClickNpcList(byte arr)
    {
        if (SubGroup.gameObject.activeSelf && SubGroup.parent == Grid.transform.GetChild(arr))   //다시누른것
        {
            SubGroup.gameObject.SetActive(false);
            Grid.Reposition();
            return;

        }

        List<InputTownModel> npcList = TownNpcMgr.instance.GetNpcList();

        int activeCount = 0;
        for (int i = 0; i < SubGroup.childCount; i++)
        {
            if (i >= npcList.Count)
            {
                SubGroup.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject go = SubGroup.GetChild(i).gameObject;
            Transform tf = go.transform;

            go.SetActive(true);

            if (arr == 0)
            {
                if (npcList[i].NPCType != 0)
                {
                    SubGroup.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
            }

            if (arr == 1)
            {
                if (npcList[i].NPCType == 0)
                {
                    SubGroup.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
            }

            tf.FindChild("label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringUnit(_LowDataMgr.instance.GetNPCInfo((ushort)npcList[i].NpcId).DescriptionId));

            int idx = i;
            UIEventTrigger eTri = go.GetComponent<UIEventTrigger>();
            EventDelegate.Set(eTri.onClick, delegate ()
            {
                int npcID = npcList[idx].NpcId;
                //int npcID = TownNpcMgr.instance.GetTownNPC((NPCTYPE)npcList[idx].NPCType);

                if (npcID != int.MaxValue)
                {
                    (PlayerUnit as MyTownUnit).RunToNPC((uint)npcID, true);
                }

                #region 점찍는부분(주석처리)

                //NavMeshPath meshPath = (PlayerUnit as MyTownUnit).GetMovePath();
                //for (int kk = 0; kk < DotRoot.childCount; kk++)
                //{
                //    DotRoot.GetChild(kk).gameObject.SetActive(false);
                //}

                //int dotCnt = 0;
                //Vector2 mapPos = Detail.transform.localPosition;
                //for (int k = 0; k < meshPath.corners.Length; k++)
                //{
                //    float x = (meshPath.corners[k].x * Detail.localSize.x) / WorldScale[0].x;
                //    float y = (meshPath.corners[k].z * Detail.localSize.y) / WorldScale[0].y;

                //    x = (x - mapPos.x) - MapPointOffset[0].x;
                //    y = (y - mapPos.y) - MapPointOffset[0].y;


                //    Vector3 dotPos = new Vector3(-x, -y, 0);
                //    if (k != 0)
                //    {
                //        // 두점사이에 점찍기.. 10당한개로 기준?잡아줌

                //        float _x = (meshPath.corners[k - 1].x * Detail.localSize.x) / WorldScale[0].x;
                //        float _y = (meshPath.corners[k - 1].z * Detail.localSize.y) / WorldScale[0].y;

                //        _x = (_x - mapPos.x) - MapPointOffset[0].x;
                //        _y = (_y - mapPos.y) - MapPointOffset[0].y;

                //        Vector2 betweenDot = new Vector2(-x, -y) - new Vector2(-_x, -_y);

                //        //meshPath.corner는 코너값이기 떄문에  x,y 중 하나만 값을더해줌

                //        bool isX = false;
                //        int cnt = 0;
                //        //if(Mathf.Abs(betweenDot.x) > Mathf.Abs(betweenDot.y))
                          //차이가더큰값으로 기준..
                //        if (Mathf.Abs(betweenDot.normalized.x) > Mathf.Abs(betweenDot.normalized.y))
                //        {
                //            cnt = (int)Mathf.Abs(betweenDot.x) / 10;
                //            isX = true;
                //        }
                //        else
                //        {
                //            cnt = (int)Mathf.Abs(betweenDot.y) / 10;
                //        }

                          // 차이가 10이상일경우 10당 1개씩 찍어줌
                //        if (cnt >= 1 /*dotCnt*/)//Mathf.Abs(
                //        {
                //            for (int kk = 0/*dotCnt*/; kk < cnt; kk++)
                //            {
                //                DotRoot.GetChild(/*kk*/dotCnt + 1).gameObject.SetActive(true);
                //                if (isX)
                //                {
                //                    DotRoot.GetChild(/*kk*/dotCnt + 1).localPosition = new Vector3((-x + (10 * (kk + 1))), -y);
                //                }
                //                else
                //                {
                //                    DotRoot.GetChild(/*kk*/dotCnt + 1).localPosition = new Vector3(-x, (-y + (10 * (kk + 1))));
                //                }

                //                dotCnt++;
                //            }

                //            //dotCnt = cnt;
                //        }
                //        else
                //        {
                //            if (k < DotRoot.childCount)
                //            {
                //                DotRoot.GetChild(dotCnt + 1).gameObject.SetActive(true);
                //                DotRoot.GetChild(dotCnt + 1).localPosition = dotPos;
                //                dotCnt++;
                //            }
                //        }


                //    }
                //    else
                //    {
                //        if (k < DotRoot.childCount)
                //        {
                //            DotRoot.GetChild(k).gameObject.SetActive(true);
                //            DotRoot.GetChild(k).localPosition = dotPos;
                //            dotCnt = k;
                //        }
                //    }

                //}

                #endregion
            });

            activeCount++;


        }

       

        SubGroup.transform.gameObject.SetActive(true);
        SubGroup.GetComponent<UIGrid>().Reposition();

        Grid.Reposition();

        float cellHeight = SubGroup.GetComponent<UIGrid>().cellHeight;
        float subListHeight = activeCount * cellHeight;

        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            Transform btnTf = Grid.GetChild(i);
            Vector3 pos = btnTf.localPosition;

            if (i < arr)
            {
                pos.y = i * Grid.cellHeight;
            }
            else if (i == arr)
            {
                pos.y = i * Grid.cellHeight;
                SubGroup.parent = btnTf;
                SubGroup.localPosition = new Vector3(0, -96, 0);
            }
            else
                pos.y = subListHeight + (i * Grid.cellHeight) + 66;

            btnTf.localPosition = -pos;
        }

    }




    /// <summary>
    /// 타운에서 NPC표시
    /// </summary>
    public void SetTownNpc()
    {
        NpcListObject.SetActive(true);
        MonsterArea.gameObject.SetActive(false);

        List<InputTownModel> npcList = TownNpcMgr.instance.GetNpcList();

        for (int i = 0; i < NpcRoot.childCount; i++)
        {
            GameObject npc = NpcRoot.GetChild(i).gameObject;
            GameObject dtailnpc = NpcDetailRoot.GetChild(i).gameObject;

            if (i >= npcList.Count)
            {
                npc.SetActive(false);
                dtailnpc.SetActive(false);
                continue;
            }

            string npcIcon = "map_npc";

            if(npcList[i].NPCType == 0) //퀘스트
            {
                npcIcon = "map_npc2";
            }
            npc.GetComponent<UISprite>().spriteName = npcIcon;
            dtailnpc.GetComponent<UISprite>().spriteName = npcIcon;

            //이동
            UIEventTrigger eTri = dtailnpc.GetComponent<UIEventTrigger>();
            int idx = i;
            EventDelegate.Set(eTri.onClick, delegate ()
           {
               int npcID = npcList[idx].NpcId;


               if (npcID != int.MaxValue)
               {
                   SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID, true);
               }
               #region 점찍는부분(주석처리)

               //NavMeshPath meshPath = (PlayerUnit as MyTownUnit).GetMovePath();
               //for (int kk = 0; kk < DotRoot.childCount; kk++)
               //{
               //    DotRoot.GetChild(kk).gameObject.SetActive(false);
               //}


               //int dotCnt = 0;
               //Vector2 mapPos = Detail.transform.localPosition;
               //for (int k = 0; k < meshPath.corners.Length; k++)
               //{
               //    float x = (meshPath.corners[k].x * Detail.localSize.x) / WorldScale[0].x;
               //    float y = (meshPath.corners[k].z * Detail.localSize.y) / WorldScale[0].y;

               //    x = (x - mapPos.x) - MapPointOffset[0].x;
               //    y = (y - mapPos.y) - MapPointOffset[0].y;


               //    Vector3 dotPos = new Vector3(-x, -y, 0);
               //    if (k != 0)
               //    {
               //        // 두점사이에 점찍기.. 10당한개로 기준?잡아줌

               //        float _x = (meshPath.corners[k - 1].x * Detail.localSize.x) / WorldScale[0].x;
               //        float _y = (meshPath.corners[k - 1].z * Detail.localSize.y) / WorldScale[0].y;

               //        _x = (_x - mapPos.x) - MapPointOffset[0].x;
               //        _y = (_y - mapPos.y) - MapPointOffset[0].y;

               //        Vector2 betweenDot = new Vector2(-x, -y) - new Vector2(-_x, -_y);

               //        //meshPath.corner는 코너값이기 떄문에  x,y 중 하나만 값을더해줌

               //        bool isX = false;
               //        int cnt = 0;
               //        //if(Mathf.Abs(betweenDot.x) > Mathf.Abs(betweenDot.y))
               //        if (Mathf.Abs(betweenDot.normalized.x) > Mathf.Abs(betweenDot.normalized.y))
               //        {
               //            cnt = (int)Mathf.Abs(betweenDot.x) / 10;
               //            isX = true;
               //        }
               //        else
               //        {
               //            cnt = (int)Mathf.Abs(betweenDot.y) / 10;
               //        }

               //        if (cnt >= 1 /*dotCnt*/)//Mathf.Abs(
               //        {
               //            for (int kk = 0/*dotCnt*/; kk < cnt; kk++)
               //            {
               //                DotRoot.GetChild(/*kk*/dotCnt + 1).gameObject.SetActive(true);
               //                if (isX)
               //                {
               //                    DotRoot.GetChild(/*kk*/dotCnt + 1).localPosition = new Vector3((-x + (10 * (kk + 1))), -y);
               //                }
               //                else
               //                {
               //                    DotRoot.GetChild(/*kk*/dotCnt + 1).localPosition = new Vector3(-x, (-y + (10 * (kk + 1))));
               //                }

               //                dotCnt++;
               //            }

               //            //dotCnt = cnt;
               //        }
               //        else
               //        {
               //            if (k < DotRoot.childCount)
               //            {
               //                DotRoot.GetChild(dotCnt + 1).gameObject.SetActive(true);
               //                DotRoot.GetChild(dotCnt + 1).localPosition = dotPos;
               //                dotCnt++;
               //            }
               //        }


               //    }
               //    else
               //    {
               //        if (k < DotRoot.childCount)
               //        {
               //            DotRoot.GetChild(k).gameObject.SetActive(true);
               //            DotRoot.GetChild(k).localPosition = dotPos;
               //            dotCnt = k;
               //        }
               //    }

               //}
               #endregion
           });

            Vector3 SimplePos = npcList[i].PathTf.position;
            SimplePos.x = (SimplePos.x * Simple.localSize.x) / WorldScale[0].x;
            SimplePos.y = (SimplePos.z * Simple.localSize.y) / WorldScale[0].y;
            SimplePos.z = 0;

            npc.transform.localPosition = new Vector2(Offset[0].x - SimplePos.x, Offset[0].y - SimplePos.y);
            npc.SetActive(true);


            Vector3 DetailPos = npcList[i].PathTf.position;
            DetailPos.x = (DetailPos.x * Detail.localSize.x) / WorldScale[0].x;
            DetailPos.y = (DetailPos.z * Detail.localSize.y) / WorldScale[0].y;
            DetailPos.z = 0;


            Vector2 text = Detail.transform.localPosition;
            DetailPos.x = (DetailPos.x - text.x) - Offset[0].x;
            DetailPos.y = (DetailPos.y - text.y) - Offset[0].y;

            dtailnpc.transform.localPosition = -DetailPos;
            dtailnpc.SetActive(true);


        }

    }

    public void HideDetail()
    {
        DetailMap.SetActive(false);
        SimpleMap.SetActive(true);
        SumSimpleMap.SetActive(true);

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        if (townPanel != null)
            townPanel.gameObject.SetActive(true);

        UIBasePanel hudPanel = UIMgr.GetHUDBasePanel();
        if (hudPanel != null)
            hudPanel.gameObject.SetActive(true);

        UIBasePanel chatPopup = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
        if (chatPopup != null)
        {
            chatPopup.gameObject.SetActive(true);
        }

        UIBasePanel joy = UIMgr.GetUIBasePanel("UIObject/Joystick", false);
        if (joy != null)
        {
            (joy as Joystick).SetJoyActive(true);
        }

    }

    public void OnShow()
    {
        transform.localPosition = Vector3.zero;
        DetailMap.gameObject.SetActive(false);
        SimpleMap.gameObject.SetActive(true);

        UIBasePanel chatPopup = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
        if (chatPopup != null)
        {
            chatPopup.gameObject.SetActive(true);
        }

        UIBasePanel joy = UIMgr.GetUIBasePanel("UIObject/Joystick", false);
        if (joy != null)
        {
            (joy as Joystick).SetJoyActive(true);
        }

    }

    public override void Hide()
    {
        transform.localPosition = new Vector3(-5000, 0);
        //base.Hide();
    }
}
