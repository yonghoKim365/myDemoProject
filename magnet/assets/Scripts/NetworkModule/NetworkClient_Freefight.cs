using Core.Net;
using UnityEngine;
using System.Net;
using Network = Core.Net.Network;
using Protocol;
using System.Text;
using Sw;
using System.Collections.Generic;

public partial class NetworkClient {

    public bool SendPMsgMessQueryC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMessQueryC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MESS_QUERY_C, sendMsg);
        return true;
    }

//    public bool SendPMsgMessRoomQueryC(int Type)
//    {
//        if (!GetGameConnection())
//            return false;
//
//        var sendMsg = new PMsgMessRoomQueryC();
//        sendMsg.UnType = Type;
//        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MESS_ROOM_QUERY_C, sendMsg);
//        return true;
//    }

    public bool SendPMsgMessRoomEnterC(long roomID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMessRoomEnterC();
		sendMsg.UnStageIndex = (uint)roomID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MESS_ROOM_ENTER_C, sendMsg);
        return true;
    }

    public bool SendPMsgBattleMapMoveCS(List<PathVertex> movePath, float realX, float realY)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBattleMapMoveCS();

        sendMsg.UnMapId = mapserverID;

        //시작점은 빼자
        for (int i = 1; i < movePath.Count; i++)
        {
            var mapPos = new BattleMapPos();
            mapPos.UnPosX = (int)movePath[i].myTilePos.x;
            mapPos.UnPosY = (int)movePath[i].myTilePos.y;

            mapPos.FData1 = realX;
            mapPos.FData2 = realY;

            sendMsg.CMapPos.Add(mapPos);
        }

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BATTLE_MAP_MOVE_CS, sendMsg);
        return true;
    }

    public bool SendPMsgBattleMapMoveCS(int posX, int posY, float realX, float realY)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBattleMapMoveCS();

        sendMsg.UnMapId = mapserverID;

        var mapPos = new BattleMapPos();
        mapPos.UnPosX = posX;
        mapPos.UnPosY = posY;
        mapPos.FData1 = realX;
        mapPos.FData2 = realY;

        sendMsg.CMapPos.Add(mapPos);

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BATTLE_MAP_MOVE_CS, sendMsg);
        return true;
    }

    public bool SendPMsgMessRoomLeaveC(long roomID)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgMessRoomLeaveC();
        sendMsg.UllId = roomID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MESS_ROOM_LEAVE_C, sendMsg);

        return true;
    }

    public bool SendPMsgBattleAttackPrepareC(int unSkillId, int unHeroId, int unDir)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBattleAttackPrepareC();
        sendMsg.UnSkillId = unSkillId;
        sendMsg.UnHeroId = unHeroId;
        sendMsg.UnDir = unDir;

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BATTLE_ATTACK_PREPARE_C, sendMsg);

        return true;
    }

    public bool SendPMsgRoleAttackC(int unSkillId, int unHeroId, int unSkillNotiIdx, int unDir, ref List<TargetInfo> targetList, long UllProjectTileId = 0)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRoleAttackC();
        sendMsg.UnSkillId = unSkillId;
        sendMsg.UnHeroId = unHeroId;
        sendMsg.UnSkillNotiIdx = unSkillNotiIdx;
        sendMsg.UnDir = unDir;

        sendMsg.UllProjectTileId = UllProjectTileId;

        for (int i=0;i< targetList.Count;i++)
        {
            //var target = TargetInfo();
            //target.unTargetType

            sendMsg.CInfo.Add(targetList[i]);
        }

        //Debug.Log("SendPMsgRoleAttackS:" + unSkillId + " noti:"+ unSkillNotiIdx);

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_ATTACK_C, sendMsg);

        return true;
    }

    public bool SendPMsgBattleMapEnterMapReadyC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBattleMapEnterMapReadyC();
        sendMsg.UnMapId = mapserverID;
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BATTLE_MAP_ENTER_MAP_READY_C, sendMsg);

        return true;
    }

//    public bool SendPMsgMessChangeRoomC(long roomId)
//    {
//        if (!GetGameConnection())
//            return false;
//
//        var sendMsg = new PMsgMessChangeRoomC();
//        sendMsg.UllNewRoomId = roomId;
//        mNetworkGame.SendMsg(MSG_DEFINE._MSG_MESS_CHANGE_ROOM_C, sendMsg);
//
//        return true;
//    }

    public bool SendPMsgBattleAddProjectTileC(int unSkillId, int unSkillNotiIdx, int unProjectTileType, int nStartPosX, int nStartPosY, int nEndPosX, int nEndPosY, float fTotalTime, float fSpeed, Vector3 dir)
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgBattleAddProjectTileC();
        sendMsg.UnSkillId = unSkillId;
        sendMsg.UnSkillNotiIdx = unSkillNotiIdx;
        sendMsg.UnProjectTileType = unProjectTileType;
        sendMsg.NStartPosX = nStartPosX;
        sendMsg.NStartPosY = nStartPosY;
        sendMsg.NEndPosX = nEndPosX;
        sendMsg.NEndPosY = nEndPosY;
        sendMsg.FTotalTime = fTotalTime;
        sendMsg.FSpeed = fSpeed;
        sendMsg.FDir.Add(dir.x);
        sendMsg.FDir.Add(dir.y);
        sendMsg.FDir.Add(dir.z);

        mNetworkGame.SendMsg(MSG_DEFINE._MSG_BATTLE_ADD_PROJECTTILE_C, sendMsg);

        return true;
    }


    public bool SendPMsgRoleReliveC()
    {
        if (!GetGameConnection())
            return false;

        var sendMsg = new PMsgRoleReliveC();
        mNetworkGame.SendMsg(MSG_DEFINE._MSG_ROLE_RELIVE_C, sendMsg);

        return true;
    }



    private void PMsgMessQuerySHandler(PMsgMessQueryS pmsgMessQueryS)
    {
		Debug.Log(pmsgMessQueryS); 
		//pmsgMessQueryS.UnDailyFamePoint;

//        int item = pmsgMessQueryS.UnDailyItem;
//        int itemMax = pmsgMessQueryS.UnDailyItemMax;
//        int point = pmsgMessQueryS.UnDailyPoint;
//        int pointMax = pmsgMessQueryS.UnDailyPointMax;
		
        if ( !TownState.TownActive)
        {
            //처음입장할떄
            if ((G_GameInfo.GameInfo as FreeFightGameInfo).GetFreefightDropReward() != null)
            {
                List<NetData.DropItem> itemList = new List<NetData.DropItem>();
//                for(int i=0;i<item;i++)
//                {
//                    itemList.Add(new NetData.DropItem(0, 0, item));
//
//                }
//                (G_GameInfo.GameInfo as FreeFightGameInfo).SetFreefightReward(point, 0, 0, itemList, true);
            }
        }
    }

//    private void PMsgMessRoomQuerySHandler(PMsgMessRoomQueryS pmsgMessRoomQuerySS)
//    {
//        //Debug.Log(pmsgMessRoomQuerySS);
//       
//        List<NetData.MessInfo> Info = new List<NetData.MessInfo>();
//        for (int i = 0; i < pmsgMessRoomQuerySS.CInfo.Count; i++)
//        {
//            Sw.MessInfo info = pmsgMessRoomQuerySS.CInfo[i];
//            NetData.MessInfo room = new NetData.MessInfo(info.UllId, info.UnRoleNum, info.UnRoleNumMax);
//            Info.Add(room);
//        }
//
//        int type = pmsgMessRoomQuerySS.UnType;
//        int cnt = pmsgMessRoomQuerySS.UnCount;
//
//        //UIBasePanel DogFight = UIMgr.GetUIBasePanel("UIPanel/FreefightPanel");
//        //if (DogFight != null)
//        //{
//        //    (DogFight as DogFightPanel).SetRoomInfo(type, cnt, Info);
//
//        //}
//        // (G_GameInfo.GameInfo as FreeFightGameInfo).SelectLastFreeFightType = (uint)type;
//
//        //UIBasePanel DogFight = UIMgr.GetUIBasePanel("UIPanel/FreefightPanel");
//        //if (DogFight != null)
//        //{
//        //    (DogFight as FreefightPanel).SetRoomInfo(type, cnt, Info);
//        //    //UIMgr.OpenDogFight();
//        //}
//
//
//        UIBasePanel InGameHudPanel = UIMgr.GetUIBasePanel("UIPanel/InGameHUDPanel");
//        if (InGameHudPanel != null)
//        {
//            // (DogFight as DogFightPanel).SetRoomList(type, cnt, Info);
//            (G_GameInfo.GameInfo as FreeFightGameInfo).SelectLastFreeFightType = (uint)type;
//            (InGameHudPanel as InGameHUDPanel).FreeFightChannelLsit(Info);
//            return;
//        }
//
//
//
//    }

    private void PMsgMessRoomEnterSHandler(PMsgMessRoomEnterS pmsgMessRoomEnterS)
    {
        Debug.Log(pmsgMessRoomEnterS);

		//pmsgMessRoomEnterS.UnStageIndex;
		//pmsgMessRoomEnterS.UllId;
		if (pmsgMessRoomEnterS.UnErrorCode != (int)Sw.ErrorCode.ER_success) {
			UIMgr.instance.AddErrorPopup((int)pmsgMessRoomEnterS.UnErrorCode);
		}


        FreeFightGameState.selectedRoomNo = (ulong)pmsgMessRoomEnterS.UllId;
    }

    private void PMsgMessRoomLeaveSHandler(PMsgMessRoomLeaveS pmsgMessRoomLeaveS)
    {
        //Debug.Log(pmsgMessRoomLeaveS);
        SceneManager.instance.IsRTNetwork = false;
        (G_GameInfo.GameInfo as FreeFightGameInfo).ResetDropData();

        //난투장나갈때 전투관련업적을보내줌
        G_GameInfo.GameInfo.SendAchieveFightData();

        //Debug.Log(string.Format("MaxCombo:{0}\nMaxDamage:{1}\nSkillCnt:{2}\nKillMob:{3}\nKillBoss:{4}\nPK:{5}:\nDie:{6}\nRevive:{7}",
        //     G_GameInfo.GameInfo._AchieveFightData.MaxCombo,
        //      G_GameInfo.GameInfo._AchieveFightData.MaxDamaga,
        //       G_GameInfo.GameInfo._AchieveFightData.SkillCount,
        //        G_GameInfo.GameInfo._AchieveFightData.KillMonsterCount,
        //         G_GameInfo.GameInfo._AchieveFightData.KillBossCount,
        //          G_GameInfo.GameInfo._AchieveFightData.killPkCount,
        //           G_GameInfo.GameInfo._AchieveFightData.DieCount,
        //            G_GameInfo.GameInfo._AchieveFightData.ReviveCount));

    }

    private void PMsgBattleMapEnterMapSHandler(PMsgBattleMapEnterMapS pmsgBattleMapEnterMapS)
    {
       // Debug.Log(pmsgBattleMapEnterMapS);

        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.FREEFIGHT, null);
        
        mapID = pmsgBattleMapEnterMapS.UnMapType;
        mapserverID = pmsgBattleMapEnterMapS.UnMapId;

        _regenX = pmsgBattleMapEnterMapS.UnPosX;
        _regenY = pmsgBattleMapEnterMapS.UnPosY;

        SceneManager.instance.ActionEvent(_ACTION.PLAY_FREEFIGHT);
        SceneManager.instance.IsRTNetwork = true;

        NetData.instance.MakePlayerFreeFightSyncData(pmsgBattleMapEnterMapS);

        //진행하는 난투장의 아이디를 알 방법이 없다 MapTable의 SceneName으로 구별한다
        Map.MapDataInfo mapLowData = _LowDataMgr.instance.GetMapData((uint)mapID);
        if(mapLowData != null)
        {
            DungeonTable.FreefightTableInfo info = _LowDataMgr.instance.GetLowDataFreeFight(mapLowData.scene);
            if (info != null)
            {
                FreeFightGameState.lastSelectStageId = info.StageIndex;
            }
        }
    }


    //여기나감
    private void PMsgBattleMapLeaveMapSHandler(PMsgBattleMapLeaveMapS pmsgBattleMapLeaveMapS)
    {
        //Debug.Log("pmsgBattleMapLeaveMapS :" + pmsgBattleMapLeaveMapS);
        if (FreeFightGameState.StateActive)
        {
            if(pmsgBattleMapLeaveMapS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                (G_GameInfo.GameInfo as FreeFightGameInfo).FreeFightUserLeave((ulong)pmsgBattleMapLeaveMapS.UllRoleId);
            }
            else if (pmsgBattleMapLeaveMapS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                (G_GameInfo.GameInfo as FreeFightGameInfo).FreeFightNPCLeave((ulong)pmsgBattleMapLeaveMapS.UllRoleId);
            }
        }
        else
        {
            if (pmsgBattleMapLeaveMapS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                for (int i = 0; i< beforeUnitLoadingList.Count; i++)
                {
                    if( beforeUnitLoadingList[i].UllRoleId == pmsgBattleMapLeaveMapS.UllRoleId )
                    {
                        beforeUnitLoadingList.Remove(beforeUnitLoadingList[i]);
                    }
                }
            }
            else if (pmsgBattleMapLeaveMapS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                for (int i = 0; i < beforeNpcLoadingList.Count; i++)
                {
                    if (beforeNpcLoadingList[i].UllNpcId == pmsgBattleMapLeaveMapS.UllRoleId)
                    {
                        beforeNpcLoadingList.Remove(beforeNpcLoadingList[i]);
                    }
                }
            }
        }
    }

    private void PMsgBattleMapRoleInfoSHandler(PMsgBattleMapRoleInfoS pmsgBattleMapRoleInfoS)
    {
        Debug.Log("PMsgBattleMapRoleInfoS - " + pmsgBattleMapRoleInfoS.UllRoleId + ":" + pmsgBattleMapRoleInfoS);

        if (FreeFightGameState.StateActive)
        {
            (G_GameInfo.GameInfo as FreeFightGameInfo).FreeFightUserEnter(pmsgBattleMapRoleInfoS);
        }
        else
        {
            beforeUnitLoadingList.Add(pmsgBattleMapRoleInfoS);
        }
    }

    private void PMsgBattleMapMoveCSHandler(PMsgBattleMapMoveCS pmsgBattleMapMoveCS)
    {
        if (pmsgBattleMapMoveCS.UnErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            //Debug.Log("pmsgBattleMapMoveCS - " + GetErrorString((uint)pmsgBattleMapMoveCS.UnErrorCode));
            Debug.Log(string.Format("pmsgBattleMapMoveCS {0}_{1} Error-{2}", pmsgBattleMapMoveCS.CMapPos[0].UnPosX, pmsgBattleMapMoveCS.CMapPos[0].UnPosY, GetErrorString((uint)pmsgBattleMapMoveCS.UnErrorCode)));
        }
    }

    private void PMsgBattleMapMoveRecvSHandler(PMsgBattleMapMoveRecvS pmsgBattleMapMoveRecvS)
    {
        //Debug.Log(pmsgBattleMapMoveRecvS);

        if (FreeFightGameState.StateActive)
        {
            Unit unit = null;

            if ( pmsgBattleMapMoveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER )
            {
                unit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgBattleMapMoveRecvS.UllRoleId);
            }
            else if (pmsgBattleMapMoveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgBattleMapMoveRecvS.UllRoleId);
            }

            if (unit != null)
            {
                Vector3 pos;
                if (pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].FData1 != 0 && pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].FData2 != 0)
                {
                    pos = new Vector3(pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].FData1, 0f, pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].FData2);
                }
                else
                {
                    pos = NaviTileInfo.instance.GetTilePos(pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].UnPosX, pmsgBattleMapMoveRecvS.CMapPos[pmsgBattleMapMoveRecvS.CMapPos.Count - 1].UnPosY);
                }

                //높이를 현재 높이로 일단 맞춰주자 - 새로운 문제가 발생할수도 있지만 최선책으로 보인다.
                {
                    pos.y = unit.cachedTransform.position.y;
                }

                Vector3 PrevPos = pos;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(pos, out navHit, 20f, -1))
                {
                    pos = navHit.position;
                }

                if (pmsgBattleMapMoveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
                {
                    //Debug.Log("MoveMsg: " + PrevPos + "SamplePos:" + pos);

                    if (unit.MoveableSkill && unit.CurrentState == UnitState.Skill)
                    {
                        //둘중에 무엇이 난지는 선택
                        //unit.transform.position = pos;
                        unit.transform.LookAt(pos);
                    }
                    else
                    {
                        float moveSpeed = 1f;
                        float dist = Vector3.Distance(pos, unit.transform.position);
                        if (dist > 3)
                        {
                            moveSpeed = 1f * (dist / 3);
                        }

                        if ((unit.CurrentState == UnitState.ManualAttack || unit.CurrentState == UnitState.Skill) && unit.SkillBlend)
                        {
                            unit.ChangeState(UnitState.Idle);
                        }

                        if (Vector3.Distance(pos, unit.transform.position) > 5.0)
                        {
                            unit.navAgent.Warp(pos);
                        }
                        else
                        {
                            unit.MovePosition(pos, moveSpeed);
                        }
                    }
                }
                else if (pmsgBattleMapMoveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
                {
                    //Debug.Log("Distance: " + Vector3.Distance(pos, unit.transform.position));

                    if (unit.MoveableSkill && unit.CurrentState == UnitState.Skill)
                    {
                        unit.transform.position = pos;
                    }
                    else
                    {
                        if (Vector3.Distance(pos, unit.transform.position) > 5.0)
                        {
                            unit.navAgent.Warp(pos);
                        }

                        float moveSpeed = 0.6f;
                        float dist = Vector3.Distance(pos, unit.transform.position);
                        if (dist > 3)
                        {
                            moveSpeed = 0.6f * (dist / 3);
                        }

                        //if (!(unit.CurrentState == UnitState.ManualAttack || unit.CurrentState == UnitState.Skill))
                        unit.MovePosition(pos, moveSpeed);
                    }
                }
            }
        }
    }

    private void PMsgBattleNpcInfoSHandler(PMsgBattleNpcInfoS pmsgBattleNpcInfoS)
    {
       // Debug.Log("PMsgBattleNpcInfoS - " + pmsgBattleNpcInfoS.UllNpcId);

        if (FreeFightGameState.StateActive)
        {
            (G_GameInfo.GameInfo as FreeFightGameInfo).FreeFightNPCEnter(pmsgBattleNpcInfoS);
        }
        else
        {
            beforeNpcLoadingList.Add(pmsgBattleNpcInfoS);
        }
    }


    //내가 보낸것
    private void PMsgRoleAttackSHandler(PMsgRoleAttackS pmsgRoleAttackS)
    {
        if (pmsgRoleAttackS.UnErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            Debug.Log("pmsgRoleAttackS - " + GetErrorString((uint)pmsgRoleAttackS.UnErrorCode));
            //Debug.Log(pmsgRoleAttackS);
            return;
        }

        Debug.Log("PMsgRoleAttackS :" + pmsgRoleAttackS);

        if (FreeFightGameState.StateActive)
        {
            for (int i = 0; i < pmsgRoleAttackS.CInfo.Count; i++)
            {
                Unit target = null;
                Unit attacker = null;

                if (pmsgRoleAttackS.CInfo[i].UnTargetType == (int)ROLE_TYPE.ROLE_TYPE_USER)
                {
                    target = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleAttackS.CInfo[i].UllTargetId);
                }
                else if (pmsgRoleAttackS.CInfo[i].UnTargetType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
                {
                    target = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleAttackS.CInfo[i].UllTargetId);
                }

                //내가보낸거만 여기 들어오긴하는데.. 그냥 내 유닛으로 고정 
                attacker = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)NetData.instance._userInfo._charUUID);
                if (attacker != null)
                {
                    attacker.CharInfo.SuperArmor = (uint)pmsgRoleAttackS.UnSuperArmor;
                }

                if (target != null)
                {
                    if (pmsgRoleAttackS.CInfo[i].UnDodge == 1)
                    {
                        //회피
                        if (G_GameInfo.GameInfo.BoardPanel != null)
                            G_GameInfo.GameInfo.BoardPanel.ShowBuff(target.gameObject, target.gameObject, _LowDataMgr.instance.GetStringCommon(21));
                    }
                    else if (pmsgRoleAttackS.CInfo[i].UnCritical == 1)
                    {
                        if (NetData.instance._userInfo._charUUID == (ulong)pmsgRoleAttackS.CInfo[i].UllTargetId)
                        {
                            //해당유닛이 내유닛이다.
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackS.CInfo[i].UnDamage, true, true, false, eDamResistType.None);
                        }
                        else
                        {
                            //아니다
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackS.CInfo[i].UnDamage, false, true, false, eDamResistType.None);
                        }

                        target.StartFlickerFx();
                        target.CharInfo.Hp = pmsgRoleAttackS.CInfo[i].UnTargetHp;
                        target.CharInfo.SuperArmor = (uint)pmsgRoleAttackS.CInfo[i].UnTargetSuperArmor;
                    }
                    else
                    {
                        if (NetData.instance._userInfo._charUUID == (ulong)pmsgRoleAttackS.CInfo[i].UllTargetId)
                        {
                            //해당유닛이 내유닛이다.
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackS.CInfo[i].UnDamage, true, false, false, eDamResistType.None);
                        }
                        else
                        {
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackS.CInfo[i].UnDamage, false, false, false, eDamResistType.None);
                        }

                        target.StartFlickerFx();
                        target.CharInfo.Hp = pmsgRoleAttackS.CInfo[i].UnTargetHp;
                        target.CharInfo.SuperArmor = (uint)pmsgRoleAttackS.CInfo[i].UnTargetSuperArmor;
                    }

                    if (pmsgRoleAttackS.CInfo[i].UnPush == 1)
                    {
                        Vector3 targetpos = NaviTileInfo.instance.GetTilePos(pmsgRoleAttackS.CInfo[i].UnPosX, pmsgRoleAttackS.CInfo[i].UnPosY);
                        target.cachedTransform.position = new Vector3(targetpos.x, target.cachedTransform.position.y, targetpos.z);
                    }
                }
            }
        }
    }

    //나 이외에 남이보낸것
    private void PMsgRoleAttackRecvSHandler(PMsgRoleAttackRecvS pmsgRoleAttackRecvS)
    {
        if (FreeFightGameState.StateActive)
        {
            Unit Attacker = null;

            if (pmsgRoleAttackRecvS.UnAttackerType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                Attacker = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleAttackRecvS.UllAttackerId);

                if(Attacker != null)
                {
                    //몬스터일경우 데미지 큐에 데미지 정보 대입하고 패스
                    Attacker.DamageEnqueue(pmsgRoleAttackRecvS);
                    return;
                }
            }

            Debug.Log("PMsgRoleAttackRecvS :" + pmsgRoleAttackRecvS);

            for (int i = 0; i < pmsgRoleAttackRecvS.CInfo.Count; i++)
            {
                Unit target = null;
                Unit attacker = null;

                if (pmsgRoleAttackRecvS.CInfo[i].UnTargetType == (int)ROLE_TYPE.ROLE_TYPE_USER)
                {
                    target = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleAttackRecvS.CInfo[i].UllTargetId);
                }
                else if (pmsgRoleAttackRecvS.CInfo[i].UnTargetType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
                {
                    target = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleAttackRecvS.CInfo[i].UllTargetId);
                }

                if (pmsgRoleAttackRecvS.UnAttackerType == (int)Sw.ROLE_TYPE.ROLE_TYPE_USER)
                {
                    attacker = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleAttackRecvS.UllAttackerId);
                }
                else if (pmsgRoleAttackRecvS.UnAttackerType == (int)Sw.ROLE_TYPE.ROLE_TYPE_NPC)
                {
                    attacker = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleAttackRecvS.UllAttackerId);
                }

                if (attacker != null)
                {
                    attacker.CharInfo.SuperArmor = (uint)pmsgRoleAttackRecvS.UnSuperArmor;
                }

                if (target != null)
                {
                    if (pmsgRoleAttackRecvS.CInfo[i].UnDodge == 1)
                    {
                        //회피
                        if (G_GameInfo.GameInfo.BoardPanel != null)
                            G_GameInfo.GameInfo.BoardPanel.ShowBuff(target.gameObject, target.gameObject, _LowDataMgr.instance.GetStringCommon(21));
                    }
                    else if (pmsgRoleAttackRecvS.CInfo[i].UnCritical == 1)
                    { 

                        if (NetData.instance._userInfo._charUUID == (ulong)pmsgRoleAttackRecvS.CInfo[i].UllTargetId)
                        {
                            //해당유닛이 내유닛이다.
                            
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackRecvS.CInfo[i].UnDamage, true, true, false, eDamResistType.None);
                        }
                        else
                        {
                            //아니다
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackRecvS.CInfo[i].UnDamage, false, true, false, eDamResistType.None);
                        }

                        target.StartFlickerFx();
                        target.CharInfo.Hp = pmsgRoleAttackRecvS.CInfo[i].UnTargetHp;
                        target.CharInfo.SuperArmor = (uint)pmsgRoleAttackRecvS.CInfo[i].UnTargetSuperArmor;
                    }
                    else
                    {

                        if (NetData.instance._userInfo._charUUID == (ulong)pmsgRoleAttackRecvS.CInfo[i].UllTargetId)
                        {
                            //해당유닛이 내유닛이다.
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackRecvS.CInfo[i].UnDamage, true, false, false, eDamResistType.None);
                        }
                        else
                        {
                            target.ShowDamagedFx(target, (int)pmsgRoleAttackRecvS.CInfo[i].UnDamage, false, false, false, eDamResistType.None);
                        }

                        target.StartFlickerFx();
                        target.CharInfo.Hp = pmsgRoleAttackRecvS.CInfo[i].UnTargetHp;
                        target.CharInfo.SuperArmor = (uint)pmsgRoleAttackRecvS.CInfo[i].UnTargetSuperArmor;
                    }
                }
            }
        }
    }

    private void PMsgBattleAttackPrepareSHandler(PMsgBattleAttackPrepareS pmsgBattleAttackPrepareS)
    {
        //Debug.Log(pmsgBattleAttackPrepareS);
    }

    private void PMsgBattleAttackPrepareRecvSHandler(PMsgBattleAttackPrepareRecvS pmsgBattleAttackPrepareRecvS)
    {
        //Debug.Log("uid:"+ pmsgBattleAttackPrepareRecvS.UllRoleId + " skillID:"+ pmsgBattleAttackPrepareRecvS.UnSkillId);

        if (FreeFightGameState.StateActive)
        {
            Vector3 Rotate = Vector3.zero;

            Unit unit = null;

            if (pmsgBattleAttackPrepareRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgBattleAttackPrepareRecvS.UllRoleId);

                //몬스터의 경우 8방향으로
                Rotate.x = 0.0f;
                Rotate.y = UnitModelHelper.WayToRotate((MAP_DIR)pmsgBattleAttackPrepareRecvS.UnDir);
                Rotate.z = 0.0f;

                //몬스터의 공격일경우 데미지큐를 리셋
                int remainData = unit.DamageQueueReset();

                //if (remainData > 0)
                //    Debug.Log("PMsgBattleAttackPrepareRecvSHandler ReaminData:" + remainData);
            }
            else if (pmsgBattleAttackPrepareRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgBattleAttackPrepareRecvS.UllRoleId);

                //pc의 경우 360도로 
                Rotate.x = 0.0f;
                Rotate.y = (float)pmsgBattleAttackPrepareRecvS.UnDir;
                Rotate.z = 0.0f;
            }

            if (unit != null)
            {
                //어떤 스킬인지 가려내야한다 평타인지 스킬인지
                int slotIdx = 0;
                int NormalAttackIdx = unit.GetNormalAttackComboCount((uint)pmsgBattleAttackPrepareRecvS.UnSkillId);

                Vector3 pos = NaviTileInfo.instance.GetTilePos(pmsgBattleAttackPrepareRecvS.UnPosX, pmsgBattleAttackPrepareRecvS.UnPosY);
                pos.y = unit.transform.position.y;

                if (Vector3.Distance(pos, unit.transform.position) > 5.0)
                {
                    unit.navAgent.Warp(pos);
                }

                if (NormalAttackIdx == -1)
                {
                    //Vector3 pos = NaviTileInfo.instance.GetTilePos(pmsgBattleAttackPrepareRecvS.UnPosX, pmsgBattleAttackPrepareRecvS.UnPosY);
                    //pos.y = unit.transform.position.y;

                    //if (Vector3.Distance(pos, unit.transform.position) > 5.0)
                    //{
                    //    unit.navAgent.Warp(pos);
                    //}

                    //스킬로 인식
                    slotIdx = unit.SkillToSlotID((uint)pmsgBattleAttackPrepareRecvS.UnSkillId);
                    unit.transform.eulerAngles = Rotate;
                    unit.ChangeState(UnitState.Idle); //fsm 구조상 잠깐 idle를 해주는게 좋다
                    unit.UseSkill(slotIdx);
                }
                else
                {
                    //평타인식
                    unit.m_SvComboIdx = NormalAttackIdx;   //애니 동기화를 맞춰주기 위한 부분
                    unit.ChangeState(UnitState.Idle); //fsm 구조상 잠깐 idle를 해주는게 좋다
                    unit.transform.eulerAngles = Rotate;
                    unit.ChangeState(UnitState.ManualAttack);
                }
            }
        }
    }

    private void PMsgBattleMapKickSHandler(PMsgBattleMapKickS pmsgBattleMapKickS)
    {
        //Debug.Log(pmsgBattleMapKickS);
        Unit myUnit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)NetData.instance._userInfo._charUUID);

        Vector3 pos = NaviTileInfo.instance.GetTilePos(pmsgBattleMapKickS.UnNewX, pmsgBattleMapKickS.UnNewY);

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(pos, out navHit, 20f, -1))
        {
            pos = navHit.position;
        }

        PlayerController playerCtrl = G_GameInfo.PlayerController;
        if (playerCtrl != null && playerCtrl.Leader != null)
        {
            playerCtrl.Leader.GetComponent<NavMeshAgent>().Warp(pos);
            //playerCtrl.Leader.Position.x = (float)pmsgBattleMapKickS.UnNewX;
            //playerCtrl.Leader.Position.y = (float)pmsgBattleMapKickS.UnNewY;

            playerCtrl.Leader.BeforePosX = pmsgBattleMapKickS.UnNewX;
            playerCtrl.Leader.BeforePosY = pmsgBattleMapKickS.UnNewY;
            playerCtrl.Leader.MoveNetworkCalibrate(pos);

            //playerCtrl.Leader.inputCtlr.TargetPos = playerCtrl.Leader.transform.position;
        }
    }

    private void PMsgBattleMapFlySHandler(PMsgBattleMapFlyS PMsgBattleMapFlyS)
    {
        Debug.Log("PMsgBattleMapFlyS: " + PMsgBattleMapFlyS);
        if (FreeFightGameState.StateActive)
        {
            Unit unit = null;

            if (PMsgBattleMapFlyS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)PMsgBattleMapFlyS.UllRoleId);
            }
            else if (PMsgBattleMapFlyS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)PMsgBattleMapFlyS.UllRoleId);
            }

            Vector3 pos = NaviTileInfo.instance.GetTilePos(PMsgBattleMapFlyS.UnNewX, PMsgBattleMapFlyS.UnNewY);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 20, -1))
            {
                pos = navHit.position;
            }

            if(unit != null)
            {
                unit.GetComponent<NavMeshAgent>().Warp(pos);
                //unit.Position.x = (float)PMsgBattleMapFlyS.UnNewX;
                //unit.Position.y = (float)PMsgBattleMapFlyS.UnNewY;

                unit.BeforePosX = PMsgBattleMapFlyS.UnNewX;
                unit.BeforePosY = PMsgBattleMapFlyS.UnNewY;
                unit.MoveNetworkCalibrate(pos);
                //if(unit.inputCtlr != null)
                //    unit.inputCtlr.TargetPos = unit.transform.position;
            }
            /*
            PlayerController playerCtrl = G_GameInfo.PlayerController;
            if (playerCtrl != null && playerCtrl.Leader != null)
            {
                playerCtrl.Leader.GetComponent<NavMeshAgent>().Warp(pos);
                playerCtrl.Leader.Position.x = (float)PMsgBattleMapFlyS.UnNewX;
                playerCtrl.Leader.Position.y = (float)PMsgBattleMapFlyS.UnNewY;
                playerCtrl.Leader.inputCtlr.TargetPos = playerCtrl.Leader.transform.position;
            }
            */
        }
    }

    //난투장아이템드랍 
    private void PMsgMessDropSHandler(PMsgMessDropS pmsgMessDropS)
    {
         Debug.Log("PMsgMessDropSHandler :" + pmsgMessDropS);

        /*
         명예, 골드,경험치,드랍아이템
         */

//        int honor = pmsgMessDropS.UnHonor;
//        int coin = pmsgMessDropS.UnCoin;
//        int exp = pmsgMessDropS.UnExp;
//
//        List<NetData.DropItem> itemList = new List<NetData.DropItem>(); 
//        for(int i=0;i< pmsgMessDropS.CInfo.Count;i++)
//        {
//            Sw.MessDropItem mess = pmsgMessDropS.CInfo[i];
//            NetData.DropItem item = new NetData.DropItem(mess.UnType, mess.UnItemId, mess.UnItemNum);
//            itemList.Add(item);
//        }
//
//        //저장해줌
//        (G_GameInfo.GameInfo as FreeFightGameInfo).SetFreefightReward(honor,coin,exp,itemList,false);

    }

	private void PMsgMessSynNotifyBossBeginSHandler(PMsgMessSynNotifyBossBeginS pmsgMessSynNotifyBossBeginS){
		//pmsgMessSynNotifyBossBeginS.UnMinutes;
		Debug.Log("pmsgMessSynNotifyBossBeginS :" + pmsgMessSynNotifyBossBeginS);

	}

	private void PMsgMessSynNotifyBossEndSHandler(PMsgMessSynNotifyBossEndS pmsgMessSynNotifyBossEndS){
		//pmsgMessSynNotifyBossEndS.UnMinutes;
		Debug.Log (" pmsgMessSynNotifyBossEndS :" + pmsgMessSynNotifyBossEndS);

	}

	private void PMsgMessSynNotifyBossBeginOrEndSHandler(PMsgMessSynNotifyBossBeginOrEndS pmsgMessSynNotifyBossBeginOrEndS){
		//pmsgMessSynNotifyBossBeginOrEndS.UnMessBossFightStatus; 1== start, 2 == end
		Debug.Log (" pmsgMessSynNotifyBossBeginOrEndS :" + pmsgMessSynNotifyBossBeginOrEndS);
	}

	//mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossBeginS>(PMsgMessSynNotifyBossBeginSHandler);
	//mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossEndS>(PMsgMessSynNotifyBossEndSHandler);
	//mNetworkGame.AddMsgListener<PMsgMessSynNotifyBossBeginOrEndS>(PMsgMessSynNotifyBossBeginOrEndSHandler);

//    private void PMsgMessChangeRoomSHandler(PMsgMessChangeRoomS pmsgMessChangeRoomS)
//    {
//        Debug.Log("PMsgMessChangeRoomSHandler :" + pmsgMessChangeRoomS);
//
//    }

    private void PMsgRoleDieRecvSHandler(PMsgRoleDieRecvS pmsgRoleDieRecvS)
    {
        //사망처리
        Debug.Log("PMsgRoleDieRecvS: " + pmsgRoleDieRecvS.UllRoleId);
        if (FreeFightGameState.StateActive)
        {
            Unit unit = null;       // 죽은유저
            Unit Attacker = null;   // 죽인유저

            if (pmsgRoleDieRecvS.UnRoleType  == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleDieRecvS.UllRoleId);
            }
            else if (pmsgRoleDieRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleDieRecvS.UllRoleId);
            }

            if (pmsgRoleDieRecvS.UnAttackerType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                Attacker = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleDieRecvS.UllAttackerId);
            }
            else if (pmsgRoleDieRecvS.UnAttackerType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                Attacker = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleDieRecvS.UllAttackerId);
            }

            if ( unit != null )
            {
                unit.Die(Attacker);
                //G_GameInfo.CharacterMgr.RemoveUnit(unit);
            }

            if( pmsgRoleDieRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER )
            {
                //나인지 체크
                if ((ulong)pmsgRoleDieRecvS.UllRoleId == NetData.instance._userInfo._charUUID)
                {
                    //모드에따라 처리
                    if (FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM || FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID)
                    {
                        //콜로세움이나 멀티보스이드의 경우 - 나가기 처리
                        //NetData.instance.ClearRewardData();
                        //G_GameInfo.GameInfo.EndGame(false);
                    }
                    else if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                    {
                        //일단 난투장에선 아무처리 안한다 - 알아서 처리
                       (G_GameInfo.GameInfo as FreeFightGameInfo).SetFreeFightUserKillData(0, "");//죽었으면 PK킬수 0으로초기화

                        //날죽인놈은 체크해둠
                        if(!(G_GameInfo.GameInfo as FreeFightGameInfo).KillAttackerRoleId.Contains(pmsgRoleDieRecvS.UllAttackerId))
                        {
                            (G_GameInfo.GameInfo as FreeFightGameInfo).KillAttackerRoleId.Add(pmsgRoleDieRecvS.UllAttackerId);
                        }

                        //채팅시스템 메시지에 표기해줌
                        string name = "";
                        if (Attacker as Npc)
                        { 
                            name = _LowDataMgr.instance.GetStringUnit(Attacker.GetComponent<Npc>().npcInfo.NameId);
                        }
                        else
                        {
                            name = Attacker.CharInfo.UnitName;
                        }
                        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1035), name);
                        UIMgr.AddLogChat(msg);


                        //여기서 부활팝업
                       //UIMgr.OpenRevivePopup(null, true);
                    }
                }
                else
                {
                    if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                    {
                       

                    }
                }
            }            
        }
    }

    //유료부활인가?
    private void PMsgRoleReliveSHandler(PMsgRoleReliveS pmsgRoleReliveS)
    {
        uint ErrorCode = (uint)pmsgRoleReliveS.UnErrorCode;
        if (ErrorCode != (int)Sw.ErrorCode.ER_success)
        {
            UIMgr.instance.AddErrorPopup((int)ErrorCode);
            Debug.Log(pmsgRoleReliveS.GetType().ToString() + "-" + GetErrorString(ErrorCode));
            return;
        }

        Debug.Log("PMsgRoleReliveS-"+pmsgRoleReliveS);

        if (FreeFightGameState.StateActive)
        {
            Unit player = G_GameInfo.PlayerController.Leader;

            //제거가 안됫을때호출된경우 바로지워줌 
            //if (G_GameInfo.CharacterMgr.FindRoomUnit(NetData.instance.GetUserInfo().GetCharUUID()) != null)
            //    G_GameInfo.CharacterMgr.RemoveUnit(player); //바로지워버림

            (G_GameInfo.GameInfo as FreeFightGameInfo).FreeFightUserLeave(NetData.instance.GetUserInfo().GetCharUUID());

            CameraManager.instance.SetGrayScale(false);
            UIBasePanel revivePanel = UIMgr.GetUIBasePanel("UIPanel/InGameRevivePanel");
            if (revivePanel != null)
                revivePanel.Close();

            UIBasePanel hudPanel = UIMgr.GetHUDBasePanel();
            if (hudPanel != null)
                (hudPanel as InGameHUDPanel).Revive();

            if (player != null)
            {
                SoundManager.instance.PlaySfxSound(eUISfx.UI_revival, false);

                (player as Pc).Revive();

                //player.Revive();
                G_GameInfo.CharacterMgr.AddRoomUnit(player);
            }
        }
    }

    private void PMsgRoleSuperArmorRecoverySHandler(PMsgRoleSuperArmorRecoveryS pmsgRoleSuperArmorRecoveryS)
    {
        Debug.Log("PMsgRoleSuperArmorRecoveryS-" + pmsgRoleSuperArmorRecoveryS);

        if (FreeFightGameState.StateActive)
        {
            Unit unit = null;       
            if (pmsgRoleSuperArmorRecoveryS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgRoleSuperArmorRecoveryS.UllRoleId);
            }
            else if (pmsgRoleSuperArmorRecoveryS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                unit = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgRoleSuperArmorRecoveryS.UllRoleId);
            }

            if(unit != null)
            {
                unit.CharInfo.SuperArmor = (uint)pmsgRoleSuperArmorRecoveryS.UnSuperArmor;
                unit.CharInfo.MaxSuperArmor = (uint)pmsgRoleSuperArmorRecoveryS.UnMaxSuperArmor;
            }
        }
    }

    private void PMsgRoleReliveRecvSHandler(PMsgRoleReliveRecvS pmsgRoleReliveRecvS)
    {
        Debug.Log("PMsgRoleReliveRecvS-"+pmsgRoleReliveRecvS);

        //부활처리
        if (FreeFightGameState.StateActive)
        {
            if (pmsgRoleReliveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                //나일경우
                if ((ulong)pmsgRoleReliveRecvS.UllRoleId == NetData.instance._userInfo._charUUID)
                {
                    Unit player = G_GameInfo.PlayerController.Leader;
                    if (player != null)
                    {
                        SoundManager.instance.PlaySfxSound(eUISfx.UI_revival, false);
                        //SceneManager.instance.CurrentStateBase().PlayMapBGM(Application.loadedLevelName);

                        //player.CharInfo.MaxHp = pmsgRoleReliveRecvS.UnHp;
                        (player as Pc).Revive();

                        //player.Revive();
                        G_GameInfo.CharacterMgr.AddRoomUnit(player);
                        player.CharInfo.Hp = pmsgRoleReliveRecvS.UnHp;
                        player.CharInfo.MaxHp = pmsgRoleReliveRecvS.UnMaxHp;
                        player.CharInfo.SuperArmor = (uint)pmsgRoleReliveRecvS.UnSuperArmor;
                        player.CharInfo.MaxSuperArmor = (uint)pmsgRoleReliveRecvS.UnMaxSuperArmor;

                        CameraManager.instance.SetGrayScale(false);
                        UIBasePanel revivePanel = UIMgr.GetUIBasePanel("UIPanel/InGameRevivePanel");
                        if (revivePanel != null)
                            revivePanel.Close();

                        UIBasePanel hudPanel = UIMgr.GetHUDBasePanel();
                        if (hudPanel != null)
                            (hudPanel as InGameHUDPanel).Revive();
                    }
                }
                else
                {
                    //아닐경우는 어떻게 찾지...?
                    Unit otherPlayer = G_GameInfo.CharacterMgr.GetRoomPlayer((ulong)pmsgRoleReliveRecvS.UllRoleId);
                    if (otherPlayer != null)
                    {
                        otherPlayer.CharInfo.MaxHp = pmsgRoleReliveRecvS.UnHp;
                        (otherPlayer as Pc).Revive();
                        //otherPlayer.Revive();

                        G_GameInfo.CharacterMgr.AddRoomUnit(otherPlayer);
                        otherPlayer.CharInfo.Hp = pmsgRoleReliveRecvS.UnHp;
                        otherPlayer.CharInfo.MaxHp = pmsgRoleReliveRecvS.UnMaxHp;
                        otherPlayer.CharInfo.SuperArmor = (uint)pmsgRoleReliveRecvS.UnSuperArmor;
                        otherPlayer.CharInfo.MaxSuperArmor = (uint)pmsgRoleReliveRecvS.UnMaxSuperArmor;
                    }
                }
            }
            else if (pmsgRoleReliveRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                //몬스터 - 너도 어케찾지...?
            }
        }
    }

    private void PMsgPingCSHandler(PMsgPingCS pmsgPingCS)
    {
        SendPMsgPingCS(pmsgPingCS.Ull64Timer);
    }

    ////난투장 채널변경
    //private void PMsgMessChangeRoomSHandler(PMsgMessChangeRoomS pmsgMessChangeRoomS)
    //{
    //    if (pmsgMessChangeRoomS.UnErrorCode != (int)sw.ErrorCode.ER_success)
    //    {
    //        Debug.Log("pmsgRoleAttackS - " + GetErrorString((uint)pmsgMessChangeRoomS.UnErrorCode));
    //        //Debug.Log(pmsgRoleAttackS);
    //        if (pmsgMessChangeRoomS.UnErrorCode == (int)sw.ErrorCode.ER_MessChangeRoomS_CD_Error)
    //        {
    //            //쿨타임
    //            UIBasePanel InGameHudPanel = UIMgr.GetUIBasePanel("UIPanel/InGameHUDPanel");
    //            if (InGameHudPanel != null)
    //            {
    //                // (DogFight as DogFightPanel).SetRoomList(type, cnt, Info);
    //                (InGameHudPanel as InGameHUDPanel).AlertFreefightCoolTime();
    //                return;
    //            }

    //        }
    //        return;
    //    }
    //    Debug.Log(pmsgMessChangeRoomS);

    //    // NetworkClient.instance.SendPMsgMessRoomLeaveC((long)FreeFightGameState.selectedRoomNo);

   // }
    //난투장 보스등장 
    private void PMsgMessBossInfoSHandler(PMsgMessBossInfoS pmsgMessBossInfoS)
    {
        Debug.Log(pmsgMessBossInfoS);


//        if (pmsgMessBossInfoS.UnBossDead == 1)//주금
//        {
//            (G_GameInfo.GameInfo as FreeFightGameInfo).SetBossData(pmsgMessBossInfoS.UnBossDead, 0, 0, pmsgMessBossInfoS.UnBossReliveTime);
//        }
//        else
//        {
//            (G_GameInfo.GameInfo as FreeFightGameInfo).SetBossData(pmsgMessBossInfoS.UnBossDead, pmsgMessBossInfoS.NPosX, pmsgMessBossInfoS.NPosY);
//        }
    }

    private void PMsgBattleProjectTileInfoSHandler(PMsgBattleProjectTileInfoS pmsgBattleProjectTileInfoS)
    {
        //Debug.Log(pmsgBattleProjectTileInfoS);

        if (FreeFightGameState.StateActive)
        {
            Unit Owner = null;       // 죽은유저

            if (pmsgBattleProjectTileInfoS.UnOwnerRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                Owner = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgBattleProjectTileInfoS.UllOwnerRoleId);
            }
            else if (pmsgBattleProjectTileInfoS.UnOwnerRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                //if((ulong)pmsgBattleProjectTileInfoS.UllOwnerRoleId == NetData.instance._userInfo._charUUID)
                //{
                //    //Owner = null;
                //}
                //else
                {
                    Owner = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgBattleProjectTileInfoS.UllOwnerRoleId);
                }
            }

            if (Owner != null)
            {
                Vector3 dirVec = new Vector3(pmsgBattleProjectTileInfoS.FDir[0], pmsgBattleProjectTileInfoS.FDir[1], pmsgBattleProjectTileInfoS.FDir[2]);
                AbilityData ability = Owner.SkillCtlr.__SkillGroupInfo.GetAbility((uint)pmsgBattleProjectTileInfoS.UnSkillId, (uint)pmsgBattleProjectTileInfoS.UnSkillNotiIdx);
                Owner.SpawnProjectile(_LowDataMgr.GetSkillProjectTileData(ability.callAbilityIdx), (int)10, Owner.TeamID, dirVec, Owner, null, ability, (ulong)pmsgBattleProjectTileInfoS.UllProjectTileId, false);
            }
        }
    }

    private void PMsgBattleAddProjectTileSHandler(PMsgBattleAddProjectTileS pmsgBattleAddProjectTileS)
    {
        Debug.Log(pmsgBattleAddProjectTileS);
    }

    private void PMsgBattleDelProjectTileSHandler(PMsgBattleDelProjectTileS pmsgBattleDelProjectTileS)
    {
        Debug.Log(pmsgBattleDelProjectTileS);
    }

    private void PMsgBuffAttackRecvSHandler(PMsgBuffAttackRecvS pmsgBuffAttackRecvS)
    {
        if (FreeFightGameState.StateActive)
        {
            Unit target = null;

            if (pmsgBuffAttackRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
            {
                target = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgBuffAttackRecvS.UllRoleId);
            }
            else if (pmsgBuffAttackRecvS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
            {
                target = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgBuffAttackRecvS.UllRoleId);
            }

            if (target != null)
            {
                {

                    if (NetData.instance._userInfo._charUUID == (ulong)pmsgBuffAttackRecvS.UllRoleId)
                    {
                        //해당유닛이 내유닛이다.
                        target.ShowDamagedFx(target, (int)pmsgBuffAttackRecvS.UnDamage, true, false, false, eDamResistType.None);
                    }
                    else
                    {
                        target.ShowDamagedFx(target, (int)pmsgBuffAttackRecvS.UnDamage, false, false, false, eDamResistType.None);
                    }
                }

                target.StartFlickerFx();
                target.CharInfo.Hp = pmsgBuffAttackRecvS.UnHp;
            }
        }
    }

    private void PMsgAddBuffSHandler(PMsgAddBuffS pmsgAddBuffS)
    {
        Debug.Log(pmsgAddBuffS);

        //if (FreeFightGameState.StateActive)
        //{
        //    Unit target = null;

        //    if (pmsgAddBuffS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_USER)
        //    {
        //        target = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)pmsgAddBuffS.UllRoleId);
        //    }
        //    else if (pmsgAddBuffS.UnRoleType == (int)ROLE_TYPE.ROLE_TYPE_NPC)
        //    {
        //        target = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)pmsgAddBuffS.UllRoleId);
        //    }

        //    if (target != null)
        //    {
        //        //_LowDataMgr.instance
        //        for(int i=0;i< pmsgAddBuffS.CInfo.Count;i++)
        //        {
        //            SkillTables.BuffInfo buff =  _LowDataMgr.GetBuffData((uint)pmsgAddBuffS.CInfo[i].UnBuffId);

        //            target.BuffCtlr.AttachBuff(null, target, buff, 10000, (uint)pmsgAddBuffS.CInfo[i].FBuffTime);
        //        }
                
        //        //target.BuffCtlr.AttachBuff(null, target, null, 0, 0);
        //    }
        //}
    }
}
