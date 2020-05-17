using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NPCTYPE
{
    QUEST_NPC = 0,
    SHOP_NPC = 1,
    COLOSSEUM_NPC = 2,
    SPECIAL_NPC = 3,
    FREEFIGHT_NPC = 4,
    ARENA_NPC = 5,
    TOWER_NPC = 6,
    RANKING_NPC = 7,
    GUILD_NPC = 8,
    COSTUME_NPC = 9,
    SINGLE_NPC = 10, 
    FIVE_WAVE_NPC=11,
    ETC_NPC = 99,
}

public class TownNpcMgr : MonoSingleton<TownNpcMgr>
{
    //public GameObject[] Npcs;
    List<InputTownModel> NpcModels = new List<InputTownModel>();
    public GameObject[] Potals;
    List<Collider> PotalBoxs = new List<Collider>();

    MyTownUnit TownUnit = null;

    public void Init()
    {
        TownState townstate = SceneManager.instance.CurrentStateBase() as TownState;
        if (townstate != null)
        {
            TownUnit = townstate.MyHero;
        }
        
		StartCoroutine (CreateTownNpcs ());
        CreatePotals();
		StartCoroutine (CreateCreatures ());
        SetNewMyTownHero(TownUnit);

    }


    //강아지나 고양이와 같은 크리쳐들
    IEnumerator CreateCreatures()
    {
		int n = _LowDataMgr.instance.getNumOfNonInteractiveNpc ();
		for (int i=1; i< n+1 ; i++) {

			yield return new WaitForSeconds(2f);

			NonInteractiveNpcData.NonInteractiveNpcDataInfo npcData = _LowDataMgr.instance.getNonInteractiveNpc((ushort)i);

			InteractionNPCInfo testData = new InteractionNPCInfo();
			testData.NPCPref = npcData.prefab;
			testData.MoveSpeed = npcData.moveSpeed;
			testData.scale = npcData.scale;
			testData.moveRange = npcData.moveRange;

			if (testData.NPCPref == "npc_cat_01"){
				testData.AniID = 90001;
			}
			else if (testData.NPCPref == "npc_cat_02"){
				testData.AniID = 90002;
			}
			else if (testData.NPCPref == "npc_sibainue_01"){
				testData.AniID = 90003;
			}

            object[] initialData = new object[4] { 0, 0, (int)UnitType.TownNINPC, testData };

            bool findRegenPos = false;

            NavMeshHit hit;
            Vector3 RandomPos = new Vector3();
            while (!findRegenPos)
            {
				RandomPos.x = Random.Range((float)npcData.firstPosMin, (float)npcData.firstPosMax);
                RandomPos.y = -5f;
                RandomPos.z = Random.Range(-110f, 105f);

                if (NavMesh.SamplePosition(RandomPos, out hit, 10, -1))
                {
                    RandomPos = hit.position;
                    findRegenPos = true;
                }
            }

			InteractionNPC tu = null;

			yield return StartCoroutine(InteractionNPC.CreateTownUnitAsync (RandomPos, (retVal)=>{ tu = retVal; }, initialData));

            tu.gameObject.GetComponentInChildren<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;

            //tu.transform.position = pos;
            //tu.GetComponent<NavMeshAgent>().Warp(pos);

            tu.ChangeState(UnitState.Wander);
        }
    }

    /// <summary>
    /// 마을에 있는 NPC를 생성한. - 이건 테이블을 정의해서 목록 만들어서 구현이 필요하다.
    /// </summary>
    IEnumerator CreateTownNpcs()
    {
        foreach (Transform trans in GetComponentsInChildren<Transform>())
        {
            InputTownModel model = (InputTownModel)trans.GetComponent("InputTownModel");
            if (null != model)
            {
                NpcData.NpcInfo npcData = _LowDataMgr.instance.GetNPCInfo((ushort)model.NpcId);

                if (npcData != null)
                {
     
					//GameObject oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", npcData.prefab)) as GameObject;

					GameObject oriUnit = null;
					yield return StartCoroutine( ResourceMgr.LoadAsync (string.Format("Character/Prefab/{0}", npcData.prefab), (retVal) => {
						oriUnit = retVal as GameObject;
					}));

//					ResourceRequest resReq = Resources.LoadAsync (string.Format("Character/Prefab/{0}", npcData.prefab), typeof(GameObject));
//					while (!resReq.isDone) { 
//						yield return null; 
//					}
//					GameObject oriUnit = resReq.asset as GameObject;

                    if (oriUnit == null)
                    {
                        Debug.LogWarning("not found npc model error! path = Character/Prefab/" + npcData.prefab);
                        oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;
                    }

                    GameObject Model = Instantiate(oriUnit) as GameObject;
                    Model.transform.parent = model.transform;
                    Model.transform.localScale = Vector3.one;
                    Model.transform.localPosition = Vector3.zero;
                    Model.transform.localEulerAngles = Vector3.zero;

                    Model.GetComponent<Animation>().cullingType = AnimationCullingType.BasedOnRenderers;

                    if ( QualityManager.instance.GetQuality() != QUALITY.QUALITY_HIGH )
                    {
                        GameObject shadowGO = ResourceMgr.Instantiate<GameObject>("Etc/Shadow");
                        shadowGO.transform.parent = Model.transform;
                        shadowGO.transform.localPosition = new Vector3(0f, 0.06f, 0f);
                        shadowGO.transform.localRotation = Quaternion.Euler(new Vector3(270, 0, 0));
                        shadowGO.transform.localScale = Vector3.one * 1.8f;
                    }

                    BoxCollider bc = model.gameObject.AddComponent<BoxCollider>();
                    bc.isTrigger = true;
                    bc.size = new Vector3(2f, 2f, 2f);
                    bc.center = new Vector3(0, 1, 0);
                    string npcName = _LowDataMgr.instance.GetStringUnit(npcData.DescriptionId);

                    if (npcName == null)
                    {
                        npcName = "cannot find name.";
                    }

                    model.InitNpc(TownUnit.GetComponent<Collider>(), npcData.Type, npcName);

                    NpcModels.Add(model);
                    NGUITools.SetLayer(Model, 14);
                }
            }
        }


        UIBasePanel Map = UIMgr.GetUIBasePanel("UIPanel/MapPanel");
        if (Map != null)
        {
            (Map as MapPanel).SetTownNpc();
        }


        yield return null;
    }

    /// <summary>
    /// 마을 내에서 포탈 역할(UI오픈)을 할 구역에 오브젝트 세팅
    /// </summary>
    public void CreatePotals()
    {
        //
        for (int i = 0; i < Potals.Length; i++)
        {
            Collider bc = Potals[i].GetComponentInChildren<Collider>();
            //메쉬는 나중에 지워주고 box콜리더만 쓰도록 하자
            //bc.GetComponent<MeshRenderer>().enabled = false;

            CollisionTownModel ctm = bc.GetComponent<CollisionTownModel>();
            if (ctm == null)
                ctm = bc.gameObject.AddComponent<CollisionTownModel>();

            ctm.CollisionModelInit(TownUnit.GetComponent<Collider>(), CollisionTownModel.FUNC_TYPE.STAGE_OPEN );

            PotalBoxs.Add(bc);
        }
    }

    public InputTownModel GetNpcObject(ushort npcID)
    {
        for (int i = 0; i < NpcModels.Count; i++)
        {
            if (NpcModels[i].NpcId == npcID)
                return NpcModels[i];
        }

        return null;
    }

    GameObject GetPotalObject()
    {
        //if(Potals[0].transform.parent != null)
        //    return Potals[0].transform.parent.gameObject;

        return Potals[0];
    }

    public void RunTownUnit(TownRunTargetType type, uint id = 0)
    {
        Vector3 pos = Vector3.zero;
        if (type == TownRunTargetType.Npc)
        {
            InputTownModel twm = GetNpcObject((ushort)id);
            if (twm == null)
                return;

		  Vector3 offset = TownUnit.transform.position - twm.transform.position;

		  pos = twm.transform.position + (offset.normalized);
        }
        else if (type == TownRunTargetType.Potal)
        {
            GameObject go = GetPotalObject();
            if (go == null)
                return;

            pos = go.transform.position;
        }

        TownUnit.townUnitHelper.MovePosition(pos);
        //TownUnit.MovePosition(pos, 1f);
        /*
        if (TownUnit.CalculatePath(pos))
        {
            TownUnit.ChangeState(UnitState.Move);
            //townUnit.PlayAnim(eAnimName.Anim_move);
            //IsRun = true;
        }
        */
    }

    public Vector3 GetNpcPosition(TownRunTargetType type, uint id)
    {
        if (type == TownRunTargetType.Potal)
            return GetPotalObject().transform.position;
        else
        {
            InputTownModel model = GetNpcObject((ushort)id);
            if (model != null)
            {
                return model.transform.GetChild(0).position;
            }
        }

        return Vector3.zero;
    }

    public int GetTownNPC(NPCTYPE type)
    {
        for(int i=0;i< NpcModels.Count;i++)
        {
            if(NpcModels[i] != null)
            {
                if((NPCTYPE)NpcModels[i].NPCType == type)
                {
                    return NpcModels[i].NpcId;
                }
            }
        }

        return int.MaxValue;
    }

    /*헷갈릴거 같으니 주석.
    //움직임 정지 콜라이더와 충돌이 실행시킨다
    public void ChangeStateIdle()
    {
        TownUnit.moveTargetPotal = false;
        TownUnit.ChangeState(UnitState.Idle);
    }
    */

    /// <summary>
    /// 플레이어 캐릭터가 삭제되고 생성될때 호출됨.
    /// </summary>
    /// <param name="newUnit"></param>
    public void SetNewMyTownHero(MyTownUnit newUnit)
    {
        TownUnit = newUnit;
        for (int i = 0; i < Potals.Length; i++)
        {
            Collider bc = Potals[i].GetComponentInChildren<Collider>();
            CollisionTownModel ctm = bc.GetComponent<CollisionTownModel>();
            ctm.SetNewTargetCollider(TownUnit.GetComponent<Collider>() );
        }

        for (int i = 0; i < NpcModels.Count; i++)
        {
            //BoxCollider bc = NpcModels[i].GetComponentInChildren<BoxCollider>();
            NpcModels[i].SetNewTargetCollider(TownUnit.GetComponent<Collider>() );
        }
    }

    /// <summary> TownNPC들 넘겨준다 </summary>
    public List<InputTownModel> GetNpcList()
    {
        return NpcModels;
    }
}

