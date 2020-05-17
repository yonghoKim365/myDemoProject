using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening; //DotTween
//[ExecuteInEditMode]
public class NPCSpawner : MonoBehaviour, ISpawner
{
    public enum SpawnAppearType
    {
        Stand,
        FromTop,
        FromBottom,
    }

    public bool isBoss = false, isMiddleBoss = false;
    public eTeamType teamType = eTeamType.Team2;
    public int UnitNum = 0;
    public int unitId = 0;  //< 스테이지 테이블에서 해당 유닛아이디를 따로 입력한다.
    [HideInInspector]
    public int GroupNo = 1000;
    public SpawnAppearType appearType = SpawnAppearType.Stand;

    public string unitIcon = "";

    //< 컷씬 이벤트에 들어가는 대상인지
    public bool CutSceneEventTarget = false;
    public int CutSceneEventIndex = 0;

    public SpawnGroup Owner { set; get; }
    public Npc spawnedGO;


    public bool RegenLive = false;
    //public float RegenInterval = 0;
    public int OriginRegenCount = 0;
	public int RemainRegenCount = 0;
    //private float _RegenDelay = 999999;

    bool IntroEvent = false;
    bool BossIntroEvent = false;
    string IntroEffect = "";
    //float SpawnDealy = 0;

	public float spawnDelaySec;

	// 기존 npc들을 모두 0 이상으로 수정해줘야하는 불편함이 있어 100-n = 확률이 되도록 함.
	public int reverseProbability;

	public int LinkedNpcUnitNum;

	GameObject spawnerParent;
    
    void Start()
    { 
        // 게임내 필요없는 객체는 삭제 (Dummy 객체 삭제)
        transform.DestroyChildren( false );
        
		spawnerParent = transform.parent.GetComponent<SpawnGroup> ().gameObject;

		SetRegen ();
    }



//    public void SetEventData(StageLowData.NpcPostingInfo data)
//    {
//        IntroEvent = data.intro == 1 ? true : false;
//        BossIntroEvent = data.specialEvent == 1 ? true : false;
//        SpawnDealy = data.firstRegenTime / 1000;
//        IntroEffect = data.introEffect;
//    }

    //public void SetEventData(InfiniteLowData.PostingInfo data)
    //{
    //    IntroEvent = data.intro == 1 ? true : false;
    //    BossIntroEvent = data.specialEvent == 1 ? true : false;
    //    SpawnDealy = data.firstRegenTime / 1000;
    //    IntroEffect = data.introEffect;
    //}

    public bool UnitLive()
    {
        //< 스폰도 아직 안한상태라면 살아있는거라고 침.
        if (!SpawnActive)
            return true;

		//regenCount>0이면 아직 살아있는거라고 가정하고 true리턴함.
		if (RegenLive && RemainRegenCount > 0) {
			return true;
		}

        if (spawnedGO == null || spawnedGO.IsDie) {
			return false;
		}

        return true;
    }



    #region 반복 리젠관련 처리

	public void SetRegen(){
		if (RegenLive && OriginRegenCount > 1) {
			//_RegenDelay = RegenInterval;
			RemainRegenCount = OriginRegenCount;
			// 리젠 시키는 npc들은 소환확률이 100%가 된다.
			reverseProbability = 0;
			StartCoroutine (RegenUpdate ());
		} else {
			RemainRegenCount = OriginRegenCount = 0;
		}
	}


    //public void SetRegenData(InfiniteLowData.PostingInfo data)
    //{
    //    if (data.regenCount > 1)
    //    {
    //        RegenLive = true;
    //        RegenCount = data.regenCount;
    //        RegenTime = data.regenTime;

    //        StartCoroutine(RegenUpdate());
    //    }
    //}

    IEnumerator RegenUpdate()
    {
        while(true)
        {
			while (!SpawnActive){
				yield return null;
			}

			while (spawnedGO != null)
            {
                yield return null;
            }

            //< 젠시킨다.
            Preload();
            Spawn();

            RemainRegenCount--;
			if (RemainRegenCount == 0)
                break;

            yield return null;
        }
    }

    #endregion

    public void Preload()
    {
        GameObject npcGo = G_GameInfo.GameInfo.SpawnNpc((uint)unitId, teamType, GroupNo, transform.position, transform.rotation, isBoss, isMiddleBoss);
        spawnedGO = npcGo.GetComponent<Npc>();
        spawnedGO.spawnner = this as ISpawner;
        spawnedGO.SetNpcPostingInfo((uint)UnitNum);

        //< 숨겨놓음
        spawnedGO.gameObject.SetActive( false );

        //< 컷씬 이벤트에 들어갈 타겟일경우 추가해준다.
        if (isBoss) {
			CutSceneMgr.AddCutSceneEventTarget (npcGo, 0);
			if (CinemaSceneManager.instance != null){
				CinemaSceneManager.instance.setBossUnit(npcGo);
			}
		}
    }

    /// <summary>
    /// ISpawner의 실구현
    /// </summary>
    bool SpawnActive = false;
    public void Spawn()
    {
        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        if(SceneManager.instance.IsRTNetwork)
        {
            return;  //임시로 난투전은 NPC 스폰 시키지 않는다.
        }

        SpawnActive = true;
        StartCoroutine(NpcSpawnUpdate());
        //SendEvent( (int)SpawnGroup.eEvent.Spawned );
    }

    IEnumerator NpcSpawnUpdate()
    {
        //< 일정시간동안 대기
        //yield return new WaitForSeconds(SpawnDealy);

		// 연결된 npc가 죽어야만 소환된다.
		if (LinkedNpcUnitNum > 0) {
			while(Owner.IsUnitLive(LinkedNpcUnitNum)){
				yield return null;
			}
		}

		// 딜레이 이후에 소환된다.
   		yield return new WaitForSeconds(spawnDelaySec);

		if (reverseProbability > 100)
			reverseProbability = 100;

		int probability = 100 - reverseProbability;
		
		#if UNITY_EDITOR
		if (probability == 0) {
			Debug.LogError("probability of NpcSpawner is zero!!. npc will never spawn");
		}
		#endif
		int ratio = Random.Range (1, 100);
  		if (ratio > probability) {
			spawnedGO = null;
			SendEvent( (int)SpawnGroup.eEvent.Dead );
			yield break;
		}

        spawnedGO.gameObject.SetActive(true);
        
        if (G_GameInfo.GameInfo != null && null != spawnedGO)
        {
            switch (appearType)
            {
                case SpawnAppearType.Stand:
                    ActivateSpawnedGO();
					SendEvent( (int)SpawnGroup.eEvent.Spawned );
                    break;
                //DotTween 사용부분이였으나 현재 골나에서는 쓰이지 않다고 함.
                //case SpawnAppearType.FromTop:
                //    {
                //        spawnedGO.transform.DOMove(new Vector3(0, 10, 0), 0.3f).From(true).OnComplete(() => ActivateSpawnedGO());
                //        spawnedGO.GetComponent<Unit>().StaticState(true);
                //    }
                //    break;

                //case SpawnAppearType.FromBottom:
                //    {
                //        spawnedGO.transform.DOMove(new Vector3(0, -5f, 0), 0.3f).From(true).OnComplete(() => ActivateSpawnedGO());
                //        spawnedGO.GetComponent<Unit>().StaticState(true);
                //    }
                //    break;
            }
        }

        yield return null;
    }

    public bool SpawnReady = true;
    void ActivateSpawnedGO()
    {
        SpawnReady = false;
        if (IntroEvent || BossIntroEvent)
        {
            List<string> aniList = new List<string>();

            //if (IntroEvent)
            //    aniList.Add(spawnedGO.GetComponent<Unit>().Animator.GetAnimName(eAnimName.Anim_intro));

            //if (BossIntroEvent)
            //    aniList.Add(spawnedGO.GetComponent<Unit>().Animator.GetAnimName(eAnimName.Anim_special));

            spawnedGO.GetComponent<Unit>().SetIntroEvent(aniList, IntroEffect);
            
        }
        else
        {
            
            string path = "Effect/_UI/_INGAME/Fx_IN_enter_01";
            GameObject effGo = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            effGo.transform.parent = spawnedGO.transform;
            effGo.transform.localScale = spawnedGO.transform.localScale;
            effGo.transform.localPosition = Vector3.zero;
            
            //spawnedGO.GetComponent<Unit>().SpawnEffect("Effect/_UI/_INGAME/Fx_IN_enter_01", 1, spawnedGO.transform);
            spawnedGO.GetComponent<Unit>().StaticState(false);
        }

        //< 스폰될때 AI를 넣어줌
        //BossAI.SetAI(spawnedGO.GetComponent<Unit>(), (uint)unitId);
    }

    public void SendEvent(int evtType)
    {
        if (null == Owner)
            return;

        switch ((SpawnGroup.eEvent)evtType)
        {
            case SpawnGroup.eEvent.Spawned:
                Owner.SendMessage( "OnEvent", new SpawnEventArgs() { sender = Owner, eventType = SpawnGroup.eEvent.Spawned } );
                break;

            case SpawnGroup.eEvent.Dead:
                Owner.SendMessage( "OnEvent", new SpawnEventArgs() { sender = Owner, eventType = SpawnGroup.eEvent.Dead } );
                break;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle labelStyle = new GUIStyle( UnityEditor.EditorStyles.label );
        labelStyle.normal.textColor = Color.red;
        labelStyle.fontSize = 30;
        UnityEditor.Handles.Label( transform.position + Vector3.up, UnitNum.ToString(), labelStyle );

		if (reverseProbability > 0) {
			labelStyle.fontSize = 20;
			labelStyle.normal.textColor = Color.black;
			UnityEditor.Handles.Label( transform.position + Vector3.up*2, (100 - reverseProbability).ToString()+"%", labelStyle );
		}



        UnityEditor.Handles.ArrowCap( 0, transform.position, transform.rotation, 2f );
        UnityEditor.Handles.DrawWireArc( transform.position, transform.up, transform.right, 360, 1f );

        if (0 == unitId)
            return;
         
        string imgName = unitId.ToString();
        int starCnt = 1;
        if (int.TryParse( imgName[imgName.Length - 1].ToString(), out starCnt ))
        {
            Gizmos.DrawIcon( transform.position + new Vector3( 0, 2, 0 ), "Unit/Star" + starCnt.ToString(), true );
        }

        //System.Text.StringBuilder builder = new System.Text.StringBuilder(imgName);
        //builder[builder.Length - 1] = '1';

        if (unitIcon != "")
            Gizmos.DrawIcon(transform.position + new Vector3(0, 1, 0), "Unit/" + unitIcon, true);
          
		if (transform.parent.GetComponent<BoxCollider> () != null) {
			Vector3 colliderPos = transform.parent.transform.TransformPoint (transform.parent.GetComponent<BoxCollider> ().center);
			Debug.DrawLine (transform.position, colliderPos, Color.yellow);
		}

		if (LinkedNpcUnitNum > 0) {

		}

		if (OriginRegenCount > 0) {
			labelStyle.fontSize = 20;
			labelStyle.normal.textColor = Color.blue;
			UnityEditor.Handles.Label( transform.position + Vector3.up*3, "+"+OriginRegenCount.ToString(), labelStyle );
		}

		if (spawnDelaySec > 0) {
			labelStyle.fontSize = 20;
			labelStyle.normal.textColor = Color.blue;
			UnityEditor.Handles.Label( transform.position + Vector3.up*4, "+"+spawnDelaySec.ToString()+"sec", labelStyle );
		}
    }
#endif
}
