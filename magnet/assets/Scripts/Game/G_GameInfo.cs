using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

/// <summary>
/// 인게임에서만 필요한 매니져들은 모두 게임 인포에서 생성 된다.
/// 게임인포의 접근을 용이 하게 만들기 위한 클래스이며 매니져 접근 방법은 코드를 참고 해라.
/// </summary>
public class G_GameInfo
{
    public static GameInfoBase _GameInfo = null;

    static PlayerController _MyController = null;

    public static GAME_MODE GameMode
    {
        get { return GameInfo == null ? GAME_MODE.NONE : GameInfo.GameMode; }
        set { GameMode = value; }
    }

    public static Vector3 G_SafetyZone = Vector3.zero;

    /// <summary>
    /// 게임인포 찾아서 리턴
    /// </summary>
    public static GameInfoBase GameInfo
    {
        get
        {
            if (_GameInfo == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("GameInfo");
                if (go == null)
                    return null;
                else
                    _GameInfo = go.GetComponent<GameInfoBase>();
            }

            return _GameInfo;
        }
    }

    public static void ResetGameInfo()
    {
        _GameInfo = null;
        _MyController = null;
    }

    /// 케릭터 매니져를 리턴
    public static CharacterMgr CharacterMgr
    {
        get
        {
            if (_GameInfo == null)
                return null;

            return _GameInfo.characterMgr;
        }
    }

    public static PlayerController PlayerController
    {
        get
        {
            if (GameInfo == null)
                return null;

            if (_MyController == null)
            {
                _MyController = GameInfo.playerCtrl;
            }

            return _MyController;
        }
    }

    public static SpawnPool EffectPool
    {
        get
        {
            if (_GameInfo == null)
                return null;

            return _GameInfo.effectPool;
        }
    }

    public static SpawnPool ProjectilePool
    {
        get
        {
            if (_GameInfo == null)
                return null;

            return _GameInfo.projectilePool;
        }
    }

    public static SpawnPool InGameObjPool
    {
        get
        {
            if (_GameInfo == null)
                return null;

            return _GameInfo.InGameObjPool;
        }
    }

    /// 이펙트 폴더에 존재하는 프리팹을 스폰시켜준다.
    public static Transform SpawnEffect(string prefabName, float speed = 1f, Transform posAndRot = null, Transform parent = null, Vector3 scale = default(Vector3), System.Action<Transform> call = null)
    {
        if (_GameInfo == null || _GameInfo.effectPool == null)
        {
            if (TempOriEffectsDic.ContainsKey(prefabName))
            //AssetbundleLoader.GetEffect(prefabName, (trn) =>
            {
                //< 인게임이 아닌곳에서 호출한것이기때문에 생성해서 붙여주고 리턴해줌
                //GameObject obj = TaskManager.Instantiate(trn) as GameObject;
                GameObject obj = TaskManager.Instantiate(TempOriEffectsDic[prefabName]) as GameObject;

                if (parent != null)
                    obj.transform.parent = parent;

                if (posAndRot != null)
                {
                    obj.transform.position = posAndRot.transform.position;
                    obj.transform.rotation = posAndRot.transform.rotation;
                }

                if (call != null)
                    call(obj.transform);
            }//, false);
            return null;
        }
        
        if (!PoolManager.Pools["Effect"].prefabs.ContainsKey(prefabName))
        {
            CreateNewEffectPoolItem(prefabName, () => { SpawnEffect(prefabName, speed, posAndRot, parent, scale, call); });
            return null;
        }

        Transform spawned;

        if (null != posAndRot)
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName, posAndRot.position, posAndRot.rotation, parent);
        else
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName, parent);

        spawned.localScale = scale;

        // 최대 이펙트 스피드는 0.5f배로. 4배이상은 안나오는게 많음.
        speed = Mathf.Clamp(speed, 0.5f, 4f);
        NsEffectManager.AdjustSpeedRuntime(spawned.gameObject, speed);

        if (call != null)
            call(spawned);

        return spawned;
    }

    public static Transform SpawnEffect(string prefabName, Vector3 posAndRot, Quaternion spawnRot, float speed = 1)
    {
//	    Debug.LogError("2JW : G_GameInfo.SpawnEffect() In - " + prefabName);
	    if (!PoolManager.Pools["Effect"].prefabs.ContainsKey(prefabName))
        {
            CreateNewEffectPoolItem(prefabName, () => { SpawnEffect(prefabName, posAndRot, spawnRot, speed); });
            return null;
        }

        Transform spawned;

        if (posAndRot != Vector3.zero)
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName, posAndRot, spawnRot);
        else
            spawned = PoolManager.Pools["Effect"].Spawn(prefabName);

        // 최대 이펙트 스피드는 0.5f배로. 4배이상은 안나오는게 많음.
        speed = Mathf.Clamp(speed, 0.5f, 4f);
        NsEffectManager.AdjustSpeedRuntime(spawned.gameObject, speed);

        return spawned;
    }

    static Dictionary<string, GameObject> TempOriEffectsDic = new Dictionary<string, GameObject>();
    //< 만약 풀에 로드가 안되어있는 이펙트라면 찾아서 넣어준다.
    static Dictionary<string, List<System.Action>> EffectLoads = new Dictionary<string, List<System.Action>>();
    static bool CreateNewEffectPoolItem(string prefabName, System.Action call)
    {
	    //Debug.LogWarning("2JW :  <color=red>Commmons Effect</color> - " + prefabName);
	    if (EffectLoads.ContainsKey(prefabName))
	    {
		    EffectLoads[prefabName].Add(call);
            return false;
        }

        EffectLoads.Add(prefabName, new List<System.Action>());
        EffectLoads[prefabName].Add(call);
	    GameObject oriEff = null;
	    if (TempOriEffectsDic.ContainsKey(prefabName))
	    {
		    oriEff = TempOriEffectsDic[prefabName];
	    }
	    else
	    {
            if (prefabName.Contains("pc_f"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PC/fighter/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("pc_d"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PC/doctor/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("pc_p"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PC/pojol/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_ameymona"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/ameymona/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_chesaman"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/chesaman/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_gyehyangdong"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/gyehyangdong/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_haejeo"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/haejeo/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_jaejee"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/jaejee/{0}", prefabName)) as GameObject;
            }            
            else if (prefabName.Contains("par_jamhyehye"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/jamhyehye/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_jeoyukyeong"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/jeoyukyeong/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_jopunghwa"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/jopunghwa/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_kweegakchil"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/kweegakchil/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_limso"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/limso/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_luyeongbok"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/luyeongbok/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_mak"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/mak_gyeran/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_ryangkwan"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/ryangkwan/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_ryuhwasang"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/ryuhwasang/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_sobukwee"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/sobukwee/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_wongihong"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/wongihong/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_wongkeiying"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/wongkeiying/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("par_yoodamuk"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_PARTNER/yoodamuk/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_fightboss"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_fightboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_raid"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_RAID/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_dongkwan_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_dongkwan_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_firefght_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_b_firefght_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_bully_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_bully_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_fightboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_b_fightboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_ninjaboss"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_ninjaboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_chesaman_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_chesaman_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_toga_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_toga_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_dakeda_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_dakeda_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_staggered_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_staggered_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_staggered_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_staggered_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_henchmanmaster_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_henchmanmaster/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_henchman_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_henchman_01/{0}", prefabName)) as GameObject;
            }           
            else if (prefabName.Contains("m_b_jpsoldier_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_jpsoldier_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_hyehye_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_hyehye_01/{0}", prefabName)) as GameObject;
            }
            //else if (prefabName.Contains("m_b_dakeda_01"))//위에 동일한거 있음 주석
            //{
            //    oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_dakeda_01/{0}", prefabName)) as GameObject;
            //}
            else if (prefabName.Contains("m_b_goblinboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_goblinboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_ohwa_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_ohwa_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_sin_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_sin_01/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("m_hunter_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_hunter_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_fairy_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_fairy_01/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("m_b_merchant_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_merchant_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_admiral_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_admiral_07/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("m_faulty_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_faulty_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_faulty_07/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_france_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_france_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_07/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_08"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_08/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_henchman_09"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_henchman_09/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpsoldier_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpsoldier_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpsoldier_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpsoldier_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpsoldier_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpsoldier_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpsoldier_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpsoldier_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_07/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_08"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_08/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_jpspy_09"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_jpspy_09/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_military_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_military_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_military_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_military_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_military_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_military_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_military_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_military_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_military_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_military_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_ohwa_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_ohwa_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_ohwa_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_ohwa_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_ohwa_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_ohwa_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_ohwa_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_ohwa_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pirate_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pirate_07/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_staggered_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_staggered_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_staggered_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_staggered_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_staggered_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_staggered_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_staggered_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_staggered_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_staggered_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_staggered_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_merchant_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/merchant_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_merchant_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/merchant_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_merchant_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/merchant_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_merchant_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/merchant_04/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("m_fighter_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_fighter_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_guard_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_guard_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_wanderer_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_wanderer_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_wanderer_0"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_wanderer_0/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_rioter_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_rioter_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_robber_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_robber_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_thug_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_thug_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_devil_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_devil_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_actor_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_actor_01/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("m_toga_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_05"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_05/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_06"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_06/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_toga_07"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_toga_07/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_thiefboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_thiefboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_escortboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_escortboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_terroristboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_terroristboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_b_policeboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_policeboss_01/{0}", prefabName)) as GameObject;
            }
            else if(prefabName.Contains("b_guardboss_01") )
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_guardboss_01/{0}", prefabName)) as GameObject;
            }

            else if (prefabName.Contains("b_firefight_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_firefight_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_pojolboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_pojolboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_rioterboss_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_rioterboss_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_devilman_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_devilman_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_devilman_02"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_devilman_02/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_devilman_03"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_devilman_03/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_devilman_04"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_devilman_04/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_goblin_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_goblin_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_dealer_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_dealer_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_pojol_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_pojol_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_fishman_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_fishman_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("m_escort_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/m_escort_01/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_wongkeiying_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/wongkeiying/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_ryuhwasang_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/ryuhwasang/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_jopunghwa_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/jopunghwa/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_jamhyehye_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/jamhyehye/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_jeoyukyeong_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/jeoyukyeong/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_jaejee_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/jaejee/{0}", prefabName)) as GameObject;
            }
            else if (prefabName.Contains("b_jpsoldier_01"))
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_NPC/b_jpsoldier_01/{0}", prefabName)) as GameObject;
            }
            else
            {
                oriEff = ResourceMgr.Load(string.Format("Effect/_Common/{0}", prefabName)) as GameObject;
            }

			if (oriEff != null){
				GameObjectExtension.SetLayer(oriEff, LayerMask.NameToLayer("Focus"), true);
				//oriEff.layer = LayerMask.NameToLayer("Focus");
			}

		    if (oriEff == null)
		    {
			    Debug.LogWarning("2JW : not effect <color=red>Commmons Effect</color> - " + prefabName + " : " + oriEff, oriEff);
			    EffectLoads[prefabName].Clear();
			    EffectLoads.Remove(prefabName);
			    return false;
		    }
		    //Debug.LogWarning("2JW : " + oriEff);
		    TempOriEffectsDic.Add(prefabName, oriEff);
	    }
	    //AssetbundleLoader.GetEffect(prefabName, (obj) =>
	    {
		    //Debug.LogWarning("2JW : " + _GameInfo);
		    //Debug.LogWarning("2JW : " + _GameInfo.effectPool);
		    CreatePoolItem(_GameInfo.effectPool, oriEff);

            for (int i = 0; i < EffectLoads[prefabName].Count; i++)
                EffectLoads[prefabName][i]();

            EffectLoads[prefabName].Clear();
            EffectLoads.Remove(prefabName);
        }//, false);

	    return false;
    }

    static void CreatePoolItem(SpawnPool targetPool, object obj)
    {
	    //Debug.LogWarning("2JW : CreatePoolItem In TargetPool - " + targetPool, targetPool);
	    if (obj == null) { Debug.LogWarning("2JW : CreatePoolItem In obj - " + obj); return; }
	    PrefabPool prefabPool = new PrefabPool(((GameObject)obj).transform);
        prefabPool.preloadAmount = 1;
        prefabPool.AddSpawnCount = 3;
        targetPool.CreatePrefabPool(prefabPool, true);

        //#if DEBUG
        //Debug.Log("CreatePoolItem " + ((GameObject)obj).name);
        //#endif
    }

    static Dictionary<string, List<System.Action<GameObject>>> ProjectTileLoads = new Dictionary<string, List<System.Action<GameObject>>>();
    public static void SpawnProjectTile(string prefabName, Transform parent, System.Action<GameObject> call)
    {
        if (!G_GameInfo.ProjectilePool.prefabs.ContainsKey(prefabName))
        {
            if (ProjectTileLoads.ContainsKey(prefabName))
            {
                ProjectTileLoads[prefabName].Add(call);
                return;
            }

            ProjectTileLoads.Add(prefabName, new List<System.Action<GameObject>>());
            ProjectTileLoads[prefabName].Add(call);

            //< 프로젝트타일 가져오기
            AssetbundleLoader.GetProjectTile(prefabName, (tile) =>
            {
                CreatePoolItem(_GameInfo.projectilePool, tile);
                //SpawnProjectTile(prefabName, parent, call);

                GameObject projectobj = G_GameInfo.ProjectilePool.Spawn(prefabName, parent.position, parent.rotation, parent).gameObject;

                //< 레이어 설정
                NGUITools.SetChildLayer(projectobj.transform, parent.gameObject.layer);

                for (int i = 0; i < ProjectTileLoads[prefabName].Count; i++)
                    ProjectTileLoads[prefabName][i](projectobj);

                ProjectTileLoads[prefabName].Clear();
                ProjectTileLoads.Remove(prefabName);
            });
        }
        else
        {
            GameObject projectobj = G_GameInfo.ProjectilePool.Spawn(prefabName, parent.position, parent.rotation, parent).gameObject;

            //< 레이어 설정
            NGUITools.SetChildLayer(projectobj.transform, parent.gameObject.layer);

            call(projectobj);
        }
    }

    public static GameObject ProtoTypeProjectileGenerator(string prefabName)
    {
	   //이펙트 경로 예시 Effect/_projectile/_partner/mok_kwailan/Fx_par_mok_kwailan_attack_P
	   //prefabName 예시 Fx_par_mok_kwailan_attack_P (파트너 mok_kwailan만 지금 _가 하나더 들어가서 예외가 생김)
	   //프리팹 네임에서 경로를 뽑아와야 한다.
	   //나중에는 어셋번들로 가게되면 이부분 필요없어질 것임.



	    string[] strs = prefabName.Split('_');

	    string nameDir = "";
        string rootDir = "Effect";///_projectile";
	    string[] childDirs = { "_pc", "_partner", "_npc",}; //임시
	    int idx = 0;
	    switch (strs[1])
	    {
            case "pc":
                //idx = 0;
                //nameDir = "fighter";//아직 없으므로 임시
                {
                    switch (strs[2])
                    {
                        case "d":
                            nameDir = "doctor";
                            break;

                        case "f":
                            nameDir = "fighter";
                            break;

                        case "p":
                            nameDir = "pojol";
                            break;

                    }
                }

                break;
            case "par":
                idx = 1;
                //nameDir = string.Format("{0}_{1}", strs[2], strs[3]);
                nameDir = string.Format("{0}", strs[2]);
                break;
            case "m":
                idx = 2;
                //nameDir = "monster";//아직 없으므로 임시
                nameDir = string.Format("{0}_{1}_{2}", strs[1], strs[2], strs[3]);
                break;
	    }

	    string fullPath = string.Format("{0}/{1}/{2}/{3}", rootDir, childDirs[idx], nameDir, prefabName);
	    Object obj = Resources.Load(fullPath);
	    //GameObject instGo = GameObject.Instantiate(obj) as GameObject;
	    //instGo.name = instGo.name.Replace("(Clone)", "");

        return obj as GameObject;
    }

    //< 인게임 오브젝트풀에 있는 프리팹을 스폰시킨다.
    public static GameObject SpawnInGameObj(string prefabName, Vector3 pos, Quaternion qua)
    {
        if (!G_GameInfo.InGameObjPool.prefabs.ContainsKey(prefabName))
        {
            return null;
        }
        else
        {
            GameObject projectobj = G_GameInfo.InGameObjPool.Spawn(prefabName, pos, qua).gameObject;
            return projectobj;
        }
    }
}