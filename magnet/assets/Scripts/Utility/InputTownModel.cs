using UnityEngine;
using System.Collections;

/*
public class NPCDATA
{
    public uint NpcId;
    public string NpcName;
}
*/

public class InputTownModel : MonoBehaviour//, IInputObject
{
    /// <summary>
    /// 충돌체크해야할 대상(타운유닛이 고정으로 들어올 것이다)
    /// </summary>
    

    public int NpcId;

    public byte NPCType;
    public string NPCName;
    //얘가 뭘할수 있는지. 퀘스트 npc = 0
    //100번 1:1 pvp
    public Transform PathTf;//npc 위치 길찾기용

    Collider TargetCollider;
    ModelType modelType = ModelType.MODEL_NONE;
    public enum ModelType { MODEL_NONE, MODEL_NPC };
    /// <summary>
    /// 타운모델이 NPC일때 세팅
    /// 추후에 인자로 NPC데이터를 넘기고 클릭시에 해당 NPC가 취할수 있는 행동에 대한 판별부가 있어야함.
    /// 주로 튜토리얼과 임무와 관련이 깊음.
    /// 예) 황기영 클릭시 대화창이 뜨고 임무에 관한 간단한 스토리 전개 후 임무 진행
    /// </summary>
    public void InitNpc(Collider _target, byte _Type, string _NPCName)
    {
	    TargetCollider = _target;
        NPCType = _Type;

        NPCName = _NPCName;

        if (PathTf == null)
        {
            PathTf = transform.FindChild("PathTf");
            if(PathTf == null)//그래도 없으면 자신.
                PathTf = transform;
        }
        
        //임시 NpcData
        //npcData = new NPCDATA();
        //npcData.NpcId = npcId;
        //npcData.NpcName = "黄麒英";

        //일단 모델타입이 NPC일때와 포탈일 경우로 구분하고
        //펑션 타입을 정의해서 아래 클릭이벤트 들어오면 이벤트에 맞는 펑션 호출.
        //NPC일때는 NPC가 현재 가능한 액션정보를 찾아서 액션정보에 맞는 대화를 띄우거나
        //포탈일때는 포탈의 종류에 맞는 UI를 호출하거
        modelType = ModelType.MODEL_NPC;

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        if(townPanel != null)
            (townPanel as TownPanel).CreateHeadObjet(gameObject, NPCName, 0, 0, false);
    }

    public void InputEvent(POINTER_INFO ptr)
    {
        /*
        //조이스틱 끄고 키기
        if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
        {
            //Debug.Log("OnPress");
            if(EasyTouch.instance.enable)
                EasyTouch.SetEnabled(false);
        }
	   else if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
	   {
		  //Debug.Log("OnPress");
		  if ( !EasyTouch.instance.enable)
			 EasyTouch.SetEnabled(true);
	   }
	   else if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG)
        {
            //Debug.Log("OnDrag");
            if(!EasyTouch.instance.enable)
                EasyTouch.SetEnabled(true);
        }
        */
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.CLICK)
        {
            switch (modelType)
            {
                case ModelType.MODEL_NPC:
                    
                    NpcClickEvent();
                    break;
            }
        }
    }

    public void NpcClickEvent()
    {
        TownPanel panel = UIMgr.GetTownBasePanel() as TownPanel;
        if (panel == null || panel.IsHideTown)//무시
            return;

        MyTownUnit myHero = SceneManager.instance.GetState<TownState>().MyHero;
        if (myHero == null || myHero.IsMiniMapMove)//미니맵으로 온것의 경우 그냥 무시한다.
            return;

        //퀘스트 NPC
        if ((NPCTYPE)NPCType == NPCTYPE.QUEST_NPC)
        {
            Quest.QuestInfo info = QuestManager.instance.CheckSubQuest(QuestSubType.NPCTALK, (uint)NpcId);
            if (info != null && 0 < info.QuestTalkSceneID)
                UIMgr.OpenMissionPanel(info.ID);
        }
        //else if ((NPCTYPE)NPCType == NPCTYPE.ARENA_NPC)
        //{
        //    GameObject townpanel = UIMgr.GetUI("UIPanel/TownPanel");
        //    townpanel.GetComponent<TownPanel>().Hide();
        //    UIMgr.OpenArenaPanel();
        //}
        //else if ((NPCTYPE)NPCType == NPCTYPE.SPECIAL_NPC)
        //{
        //    panel.Hide();
        //    UIMgr.OpenSpecial();
        //}
        //else if ((NPCTYPE)NPCType == NPCTYPE.FREEFIGHT_NPC)
        //{
        //    panel.Hide();
        //    UIMgr.OpenDogFight();
        //}
        //else if ((NPCTYPE)NPCType == NPCTYPE.TOWER_NPC)
        //{
        //    panel.Hide();
        //    UIMgr.OpenTowerPanel();
        //}
        //else if ((NPCTYPE)NPCType == NPCTYPE.COSTUME_NPC)
        //{
        //    panel.Hide();
        //    UIMgr.OpenCostume();
        //}
        else if((NPCTYPE)NPCType == NPCTYPE.SINGLE_NPC)
        {
            //모험모드 NPC
            //UIMgr.instance.UiOpenType = UI_OPEN_TYPE.NONE;
            SceneManager.instance.UiOpenType = UI_OPEN_TYPE.NONE;
            GameObject townpanel = UIMgr.GetUI("UIPanel/TownPanel");
            townpanel.GetComponent<TownPanel>().OpenChapter();
        }

        //진행중인 퀘스트가 있는지 체크해야됨
        myHero.ResetMoveTarget();
    }

    void OnTriggerEnter(Collider _co)
    {
        if(_co.collider == TargetCollider && SceneManager.instance.GetState<TownState>().MyHero.CheckNpc() == NpcId )
            NpcClickEvent();
    }

    void OnTriggerStay(Collider col)
    {
        if (col.collider == TargetCollider && SceneManager.instance.GetState<TownState>().MyHero.CheckNpc() == NpcId )
            NpcClickEvent();
    }

    /// <summary>
    /// 플레이어 캐릭터의 코스튬, 외형이 바뀌면 호출된다.
    /// 삭제했다가 재생성하므로 작업이 필요하기에 추가함.
    /// </summary>
    /// <param name="_target"></param>
    public void SetNewTargetCollider(Collider _target)
    {
        TargetCollider = _target;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle labelStyle = new GUIStyle(UnityEditor.EditorStyles.label);
        labelStyle.normal.textColor = Color.red;
        labelStyle.fontSize = 30;
        UnityEditor.Handles.Label(transform.position + Vector3.up, NpcId.ToString(), labelStyle);
        UnityEditor.Handles.ArrowCap(0, transform.position, transform.rotation, 2f);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.right, 360, 1f);

        if (0 == NpcId)
            return;

        /*
        string imgName = NpcId.ToString();
        int starCnt = 1;
        if (int.TryParse(imgName[imgName.Length - 1].ToString(), out starCnt))
        {
            Gizmos.DrawIcon(transform.position + new Vector3(0, 2, 0), "Unit/Star" + starCnt.ToString(), true);
        }

        //System.Text.StringBuilder builder = new System.Text.StringBuilder(imgName);
        //builder[builder.Length - 1] = '1';

        if (NpcId != "")
            Gizmos.DrawIcon(transform.position + new Vector3(0, 1, 0), "Unit/" + NpcId, true);

        */
    }
#endif
}
