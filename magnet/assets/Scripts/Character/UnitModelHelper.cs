using UnityEngine;
using System.Collections;
using Sw;

public class UnitModelHelper : MonoBehaviour
{
    /// <summary>
    /// 플레이어 캐릭터의 모델로딩
    /// </summary>
    /// <param name="charIdx">캐릭터의 고유 아이디 현재로선 권사 11000, 의사 13000</param>
    /// <param name="HeadItemIdx">헬멧에 낀 아이템의 인덱스</param>
    /// <param name="CostumeItemIdx"></param>
    /// <param name="ClothItemIdx"></param>
    /// <param name="WeaponItemIdx"></param>
    /// <param name="HideCostume"></param>
    /// <returns></returns>
	/// 
    public static GameObject PCModelLoad( uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, bool HideCostume, ref GameObject[] WeaponEffect, ModelQuality mq = ModelQuality.HIGH)
    {


        //추후 다 어셋번들에서 읽어오게 해야함
        GameObject _unit = null;

        string headPref = "";
        string weaponPref = "";
        string clothPref = "";
        //uint aniIdx = 0;
        string rightWeaponDummy = "";
        string leftWeaponDummy = "";



        if (HideCostume)
        {
            //착용한 옷아이템이 없을경우
            if (ClothItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 옷외형을 가져옴
                clothPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.CLOTH).prefab;
            }
            else
            {
                //현재 장착중인 옷을 가져옴
                clothPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(ClothItemIdx).prefab;
            }

            //코스튬을 숨기든 말던 애니메이션 인덱스는 코스튬의 것을 가져와야함
            //aniIdx = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).AniId;

            //착용한 무기아이템의 정보
            if (WeaponItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 무기외형을 가져옴
                weaponPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).prefab;
            }
            else
            {
                //현재 장착중인 무기 가져옴
                weaponPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(WeaponItemIdx).prefab;
            }

            //착용한 머리아이템의 정보
            if (HeadItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 머리외형을 가져옴
                headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;
            }
            else
            {
                //현재 장착중인 머리 가져옴
                headPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(HeadItemIdx).prefab;
            }

            rightWeaponDummy = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).RightWeaDummy;
            leftWeaponDummy = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).LeftWeaDummy;

        }
        else
        {
            //착용한 머리아이템의 정보만 체크
            if (HeadItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 머리외형을 가져옴
                headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;
            }
            else
            {
                //현재 장착중인 머리 가져옴
                headPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(HeadItemIdx).prefab;
            }

            clothPref = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Bodyprefab;
            weaponPref = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Weaprefab;


            rightWeaponDummy = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).RightWeaDummy;
            leftWeaponDummy = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).LeftWeaDummy;
        }

        if (!headPref.Contains("face") || Resources.Load(string.Format("Character/Prefab/{0}", headPref)) == null)
        {
            Debug.LogError("not found head prefab " + headPref);
            if (charIdx == 12000)
                headPref = "pc_p_face_Lv01";
            else if (charIdx == 11000)
                headPref = "pc_f_face_Lv01";
            else if (charIdx == 13000)
                headPref = "pc_d_face_Lv01";
        }

		clothPref = GetPrefabSuffix(clothPref, mq);
		weaponPref = GetPrefabSuffix(weaponPref, mq);
		headPref = GetPrefabSuffix(headPref, mq);

        _unit = CreatePlayerDefaultModel(clothPref,
                                         weaponPref,
                                         headPref);

        if (_unit != null && WeaponEffect != null)
        {
            WeaponEffect = AttachWeaponEffect(_unit, false,
                                              rightWeaponDummy,
                                              leftWeaponDummy);
        }

        return _unit;
    }






	public static IEnumerator PCModelLoadAsync( uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, 
	                                           bool HideCostume , System.Action<GameObject, GameObject[]> callback, ModelQuality mq = ModelQuality.HIGH)
	{
		//추후 다 어셋번들에서 읽어오게 해야함
		GameObject _unit = null;

		GameObject[] WeaponEffect = {null, null};

        string headPref = "";
        string weaponPref = "";
        string clothPref = "";
        //uint aniIdx = 0;
        string rightWeaponDummy = "";
        string leftWeaponDummy = "";

        if (HideCostume)
        {
            //착용한 옷아이템이 없을경우
            if (ClothItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 옷외형을 가져옴
                clothPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.CLOTH).prefab;
            }
            else
            {
                //현재 장착중인 옷을 가져옴
                clothPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(ClothItemIdx).prefab;
            }

            //코스튬을 숨기든 말던 애니메이션 인덱스는 코스튬의 것을 가져와야함
            //aniIdx = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).AniId;

            //착용한 무기아이템의 정보
            if (WeaponItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 무기외형을 가져옴
                weaponPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).prefab;
            }
            else
            {
                //현재 장착중인 무기 가져옴
                weaponPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(WeaponItemIdx).prefab;
            }

            //착용한 머리아이템의 정보
            if (HeadItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 머리외형을 가져옴
                headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;
            }
            else
            {
                //현재 장착중인 머리 가져옴
                headPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(HeadItemIdx).prefab;
            }

            rightWeaponDummy = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).RightWeaDummy;
            leftWeaponDummy = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).LeftWeaDummy;

        }
        else
        {
            //착용한 머리아이템의 정보만 체크
            if (HeadItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 머리외형을 가져옴
                headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;
            }
            else
            {
                //현재 장착중인 머리 가져옴
                headPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(HeadItemIdx).prefab;
            }

            clothPref = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Bodyprefab;
            weaponPref = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Weaprefab;


            rightWeaponDummy = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).RightWeaDummy;
            leftWeaponDummy = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).LeftWeaDummy;
        }

        if (!headPref.Contains("face") || Resources.Load(string.Format("Character/Prefab/{0}", headPref)) == null)
        {
            Debug.LogError("not found head prefab " + headPref);
            if (charIdx == 12000)
                headPref = "pc_p_face_Lv01";
            else if (charIdx == 11000)
                headPref = "pc_f_face_Lv01";
            else if (charIdx == 13000)
                headPref = "pc_d_face_Lv01";
        }

        clothPref = GetPrefabSuffix(clothPref, mq);
        weaponPref = GetPrefabSuffix(weaponPref, mq);
        headPref = GetPrefabSuffix(headPref, mq);

        yield return SceneManager.instance.StartCoroutine(CreatePlayerDefaultModelAsync(    clothPref,
                                                                                            weaponPref,
                                                                                            headPref,
                                                                                            (retval) => { _unit = retval; }));


        if (_unit != null)
        {
            yield return SceneManager.instance.StartCoroutine(AttachWeaponEffectAsync(_unit, false,
                                                                                 rightWeaponDummy,
                                                                                 leftWeaponDummy,
                                                                                  (retval) => { WeaponEffect = retval; }));
        }

        callback(_unit, WeaponEffect);
    }

	static string GetPrefabSuffix(string prefabName, ModelQuality mq){
		if (mq == ModelQuality.HIGH) {
			return string.Format("{0}_ss", prefabName);
		}
		else if (mq == ModelQuality.UI) {
			return string.Format("{0}_s", prefabName);
		}

		return string.Format("{0}", prefabName);
	}
	
	static string GetPrefabSuffix(string prefabPath, string prefabName, ModelQuality mq){
		
		//string.Format("Character/Prefab/{0}_s", clothPref)
		if (mq == ModelQuality.HIGH) {
			return string.Format("{0}{1}_ss", prefabPath, prefabName);
		}
		else if (mq == ModelQuality.UI) {
			return string.Format("{0}{1}_s", prefabPath, prefabName);
		}

		return string.Format("{0}{1}", prefabPath, prefabName);
	}

    public static GameObject PCModelLoadRimSpec(uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, bool HideCostume, ref GameObject[] WeaponEffect, bool isWeapon=true)
    {
        //추후 다 어셋번들에서 읽어오게 해야함
        GameObject _unit = null;
        string headPref = "";
        string weaponPref = "";
        string clothPref = "";

        //착용한 머리아이템의 정보
        if (HeadItemIdx == 0)
        {
            //캐릭터의 디폴트 아이템의 머리외형을 가져옴
            headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;
        }
        else
        {
            //현재 장착중인 머리 가져옴
            headPref = _LowDataMgr.instance.GetLowDataEquipItemInfo(HeadItemIdx).prefab;
        }

        if (HideCostume)
        {
            //uint aniIdx = 0;

            //착용한 옷아이템이 없을경우
            if (ClothItemIdx == 0)
            {
                //캐릭터의 디폴트 아이템의 옷외형을 가져옴
                clothPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.CLOTH).prefab;
            }
            else
            {
                //현재 장착중인 옷을 가져옴
                Item.EquipmentInfo cloth = _LowDataMgr.instance.GetLowDataEquipItemInfo(ClothItemIdx);
                if(cloth != null)
                    clothPref = cloth.prefab;
            }

            //코스튬을 숨기든 말던 애니메이션 인덱스는 코스튬의 것을 가져와야함
            //uint aniIdx = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).AniId;

            if (isWeapon)
            {
                //착용한 무기아이템의 정보
                if (WeaponItemIdx == 0)
                {
                    //캐릭터의 디폴트 아이템의 무기외형을 가져옴
                    weaponPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).prefab;
                }
                else
                {
                    //현재 장착중인 무기 가져옴
                    Item.EquipmentInfo weapon = _LowDataMgr.instance.GetLowDataEquipItemInfo(WeaponItemIdx);
                    if (weapon != null)
                        weaponPref = weapon.prefab;
                }
            }
        }
        else
        {
            clothPref = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Bodyprefab;
            weaponPref = isWeapon ? _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).Weaprefab : "";
        }

		//if (Resources.Load(GetPrefabSuffix("Character/Prefab/",clothPref)) != null) //  
		if (Resources.Load(string.Format("Character/Prefab/{0}_s", clothPref)) != null)
			clothPref = string.Format("{0}_s", clothPref); //GetPrefabSuffix(clothPref);
        else
            Debug.LogWarning(string.Format("not found {0}_s prefab", clothPref));

        if (!string.IsNullOrEmpty(weaponPref))
        {
			//if (Resources.Load(GetPrefabSuffix("Character/Prefab/",weaponPref)) != null) 
			if (Resources.Load(string.Format("Character/Prefab/{0}_s", weaponPref)) != null)
				weaponPref = string.Format("{0}_s", weaponPref); //GetPrefabSuffix( weaponPref);
            else
                Debug.LogWarning(string.Format("not found {0}_s prefab", weaponPref));
        }

		//if (headPref.Contains("face") && Resources.Load(GetPrefabSuffix("Character/Prefab/",headPref)) != null) 
		if (headPref.Contains("face") && Resources.Load(string.Format("Character/Prefab/{0}_s", headPref)) != null)
			headPref = string.Format("{0}_s", headPref); //GetPrefabSuffix( headPref);
        else//대가리 못찾음 기본으로 셋팅해준다
        {
            Debug.LogWarning(string.Format("not found headPrefab, headItemIdx={0}, charIdx={1}", HeadItemIdx, charIdx));
            headPref = _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.HELMET).prefab;

			//if (Resources.Load(GetPrefabSuffix("Character/Prefab/",headPref)) != null) 
			if (Resources.Load(string.Format("Character/Prefab/{0}_s", headPref)) != null)
				headPref = string.Format("{0}_s", headPref); //GetPrefabSuffix( headPref);
        }
		
        //_unit = HideCostume ? CreatePlayerDefaultModel(clothPref, weaponPref, headPref) : CreatePlayerDefaultModel(clothPref, weaponPref, headPref);
        _unit = CreatePlayerDefaultModel(clothPref, weaponPref, headPref);

        if (_unit != null && WeaponEffect != null)
        {
            WeaponEffect = HideCostume ? 
                AttachWeaponEffect(_unit, false, _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).RightWeaDummy, _LowDataMgr.instance.GetDefaultItemInfo(charIdx, ePartType.WEAPON).LeftWeaDummy)
                : AttachWeaponEffect(_unit, false, _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).RightWeaDummy, _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeItemIdx).LeftWeaDummy);
        }
		        
        return _unit;

    }

    public static GameObject CreatePlayerDefaultModel(string basePrefName, string weaponPrefName, string headPrefName)
    {
        GameObject oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", basePrefName)) as GameObject;
        if (oriUnit == null)
        {
            Debug.LogWarning("not found player model error! path = Character/Prefab/" + basePrefName);
            oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;
            GameObject _ErrorUnit = GameObject.Instantiate(oriUnit) as GameObject;
            return _ErrorUnit;
        }

        GameObject _myUnit = GameObject.Instantiate(oriUnit) as GameObject;

        ModelModifier Modifier = _myUnit.AddComponent<ModelModifier>();

        // 여기까지 베이스모델 로딩완료 머리/무기를 붙이자
        if (!headPrefName.Equals(""))
        {
            Modifier.ModelApply(string.Format("Character/Prefab/{0}", headPrefName));
        }

        if (!weaponPrefName.Equals(""))
        {
            Modifier.ModelApply(string.Format("Character/Prefab/{0}", weaponPrefName));
        }

        return _myUnit;
    }

	public static IEnumerator CreatePlayerDefaultModelAsync(string basePrefName, string weaponPrefName, string headPrefName, System.Action<GameObject> callback )
	{
		GameObject oriUnit = null; 
		yield return SceneManager.instance.StartCoroutine (ResourceMgr.LoadAsync (string.Format ("Character/Prefab/{0}", basePrefName),(retval)=> { oriUnit = retval as GameObject;}));

		if (oriUnit == null)
		{
			Debug.LogWarning("not found player model error! path = Character/Prefab/" + basePrefName);
			oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;
			GameObject _ErrorUnit = GameObject.Instantiate(oriUnit) as GameObject;
			callback (_ErrorUnit);
		}
		
		GameObject _myUnit = GameObject.Instantiate(oriUnit) as GameObject;
		
		ModelModifier Modifier = _myUnit.AddComponent<ModelModifier>();
		
		// 여기까지 베이스모델 로딩완료 머리/무기를 붙이자
		if (!headPrefName.Equals(""))
		{
			Modifier.ModelApply(string.Format("Character/Prefab/{0}", headPrefName));
		}
		
		if (!weaponPrefName.Equals(""))
		{
			Modifier.ModelApply(string.Format("Character/Prefab/{0}", weaponPrefName));
		}


		callback (_myUnit);
	}

    public static void SetupStandardComponent(Transform incompleteUnit)
    {
        GameObject go = incompleteUnit.gameObject;
        Rigidbody rigidBody = incompleteUnit.GetComponent<Rigidbody>();
        BoxCollider collider = incompleteUnit.GetComponent<BoxCollider>();
        NavMeshAgent navAgent = incompleteUnit.GetComponent<NavMeshAgent>();
        AudioSource Audio = incompleteUnit.GetComponent<AudioSource>();
        
        // Rigidbody Setting
        if (null == rigidBody)
        {
            rigidBody = go.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
        }

        // Search Box Setting
        if (null == collider)
        {
            collider = go.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f, 0.5f, 0f);
            collider.size = new Vector3(1f, 1f, 1f);
        }

        if (!TownState.TownActive)//마을이 아닐 경우에만 생성
        {
            // 거리체크용 캡슐콜리더
            if (GameDefine.skillPushTest)
            {
                CapsuleCollider collider2 = go.AddComponent<CapsuleCollider>();
                collider2.center = new Vector3(0f, 0.81f, 0f);
                collider2.radius = 1.2f;
                collider2.height = 8;
            }
        }
        // NavMeshAgent Setting
        if (null == navAgent)
        {
            navAgent = go.AddComponent<NavMeshAgent>();
            navAgent.radius = 0.7f;
            navAgent.height = collider.size.y;
            navAgent.baseOffset = 0f;
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            navAgent.avoidancePriority = 50;
            navAgent.acceleration = 10000;
            navAgent.angularSpeed = 1080;
            navAgent.stoppingDistance = 1f;
            navAgent.autoBraking = false;
        }

        //UnitAudio Setting
        if (null == Audio)
        {
            //일단 임시
            Audio = go.AddComponent<AudioSource>();
            Audio.volume = 1f;
        }

        if (!TownState.TownActive)//마을이 아닐 경우에만 생성
        {
            go.AddComponent<Unit_AI>();
        }
    }

    public static GameObject PartnerModelLoad(uint partnerID, ref GameObject[] WeaponEffect, bool isLight, string rightWea, string leftWea, ModelQuality mq = ModelQuality.HIGH)
    {
        Partner.PartnerDataInfo partner = _LowDataMgr.instance.GetPartnerInfo(partnerID);

        string partnerPref = "";
        GameObject oriUnit = null;

        if (partner != null)
        {
            partnerPref = partner.prefab;

            if (isLight)
            {
                //if (Resources.Load(string.Format("Character/Prefab/{0}_s", partnerPref)) != null)
				//	partnerPref = string.Format("{0}_s", partnerPref);
				if (Resources.Load( GetPrefabSuffix("Character/Prefab/",partnerPref, mq)) != null)
					partnerPref = GetPrefabSuffix( partnerPref, mq);
                else
					Debug.LogWarning(GetPrefabSuffix("not found prefab ", partnerPref, mq));
            }

            oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", partnerPref)) as GameObject;

            if (oriUnit == null)
                oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model", partnerPref)) as GameObject;
        }
        else
        {
            oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model", partnerPref)) as GameObject;
        }

        GameObject _unit = GameObject.Instantiate(oriUnit) as GameObject;

        if (_unit != null && WeaponEffect != null)
        {
            WeaponEffect = AttachWeaponEffect(_unit, true, rightWea, leftWea);
        }

        return _unit;
    }

    public static GameObject NPCModelLoad(uint monsterID, ModelQuality mq = ModelQuality.HIGH)
    {
        Mob.MobInfo mosterInfo = _LowDataMgr.instance.GetMonsterInfo(monsterID);

        string monsterPref = "";
        GameObject oriUnit = null;

        if (mosterInfo != null)
        {
            monsterPref = mosterInfo.prefab;
            //oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", monsterPref)) as GameObject;
			oriUnit = ResourceMgr.Load( GetPrefabSuffix( "Character/Prefab/", monsterPref, mq)) as GameObject;

            if (oriUnit == null)
                oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model", monsterPref)) as GameObject;
        }
        else
        {
            oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model", monsterPref)) as GameObject;
        }

        //GameObject _unit = GameObject.Instantiate(oriUnit) as GameObject;
       
        return oriUnit;
    }

    public static GameObject ModelLoadtoString(string prefName)
    {
        GameObject oriUnit = null;

        
        oriUnit = ResourceMgr.Load(string.Format("Character/Prefab/{0}", prefName)) as GameObject;

        if (oriUnit == null)
            oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;

        GameObject _unit = GameObject.Instantiate(oriUnit) as GameObject;

        return _unit;
    }

	public static IEnumerator ModelLoadAsync(string prefName, System.Action<GameObject> callback)
	{
		GameObject oriUnit = null;

		// static 이라서 sceneManager 갖다씀
		yield return SceneManager.instance.StartCoroutine(ResourceMgr.LoadAsync(string.Format("Character/Prefab/{0}", prefName), (retVal)=>{
			oriUnit = retVal as GameObject;
		}));
		
		if (oriUnit == null)
			oriUnit = ResourceMgr.Load(string.Format("Etc/Missing_Model")) as GameObject;
		
		GameObject _unit = GameObject.Instantiate(oriUnit) as GameObject;

		callback (_unit);
	}

    public static GameObject[] AttachWeaponEffect( GameObject model, bool isPartner, string RightWeaDummy, string LeftWeaDummy )
    {
        string EffectRootName = "";

        GameObject[] WeaponEffects = new GameObject[2];
        WeaponEffects[0] = null;
        WeaponEffects[1] = null;

        if (isPartner)
        {
            if (!RightWeaDummy.Equals("none"))
            {
                EffectRootName = string.Format("Effect/_PARTNER/{0}", RightWeaDummy);
                WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", model);
            }

            if (!LeftWeaDummy.Equals("none"))
            {
                EffectRootName = string.Format("Effect/_PARTNER/{0}", LeftWeaDummy);
                WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", model);
            }

            return WeaponEffects;
        }
        else
        {
            if (!RightWeaDummy.Equals("none"))
            {
                EffectRootName = string.Format("Effect/_PC/{0}", RightWeaDummy);
                WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", model);
            }

            if (!LeftWeaDummy.Equals("none"))
            {
                EffectRootName = string.Format("Effect/_PC/{0}", LeftWeaDummy);
                WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", model);
            }

            return WeaponEffects;
        }
    }

	public static IEnumerator AttachWeaponEffectAsync( GameObject model, bool isPartner, string RightWeaDummy, string LeftWeaDummy, System.Action<GameObject[]> callback )
	{
		string EffectRootName = "";
		
		GameObject[] WeaponEffects = new GameObject[2];
		WeaponEffects[0] = null;
		WeaponEffects[1] = null;
		
		if (isPartner)
		{
			if (!RightWeaDummy.Equals("none"))
			{
				EffectRootName = string.Format("Effect/_PARTNER/{0}", RightWeaDummy);
				//WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", model);
				yield return SceneManager.instance.StartCoroutine( AttachedWeaponEffectAsync(EffectRootName, "wp_dummy_01", model, false, (retval) =>{ WeaponEffects[0] = retval; }));
			}
			
			if (!LeftWeaDummy.Equals("none"))
			{
				EffectRootName = string.Format("Effect/_PARTNER/{0}", LeftWeaDummy);
				//WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", model);
				yield return SceneManager.instance.StartCoroutine( AttachedWeaponEffectAsync(EffectRootName, "wp_dummy_02", model, false, (retval) =>{ WeaponEffects[1] = retval; }));
			}
			
			callback(WeaponEffects);
		}
		else
		{
			if (!RightWeaDummy.Equals("none"))
			{
				EffectRootName = string.Format("Effect/_PC/{0}", RightWeaDummy);
				//WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", model);
				yield return SceneManager.instance.StartCoroutine( AttachedWeaponEffectAsync(EffectRootName, "wp_dummy_01", model, false, (retval) =>{ WeaponEffects[0] = retval; }));
			}
			
			if (!LeftWeaDummy.Equals("none"))
			{
				EffectRootName = string.Format("Effect/_PC/{0}", LeftWeaDummy);
				//WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", model);
				yield return SceneManager.instance.StartCoroutine( AttachedWeaponEffectAsync(EffectRootName, "wp_dummy_02", model, false, (retval) =>{ WeaponEffects[1] = retval; }));
			}

			callback(WeaponEffects);
		}
	}

    public static GameObject AttachedWeaponEffect(string EffectRootName, string dummyname, GameObject modelGo, bool LayerChange = false)
    {
        Transform dummyTrs = null;
        GameObject oriEffect = Resources.Load(EffectRootName) as GameObject;
        if (oriEffect == null)
        {
            Debug.LogWarning("AttachedWeaponEffect:Effect Not Find - " + EffectRootName);
            return null;
        }

        Transform[] trs = modelGo.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < trs.Length; i++)
        {
            if (trs[i].name == dummyname)
            {
                dummyTrs = trs[i];
            }
        }
        GameObject go = null;

        if (dummyTrs != null)
        {
            go = Instantiate(oriEffect) as GameObject;
            go.transform.parent = dummyTrs;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            if (LayerChange)
                NGUITools.SetLayer(go, modelGo.layer);
        }
        else
        {
            Debug.LogWarning("AttachedWeaponEffect:NotFound - " + dummyname);
        }
        return go;
    }

	public static IEnumerator AttachedWeaponEffectAsync(string EffectRootName, string dummyname, GameObject modelGo, bool LayerChange = false, System.Action<GameObject> callback = null)
	{
		Transform dummyTrs = null;
		//GameObject oriEffect = Resources.Load(EffectRootName) as GameObject;

		ResourceRequest resReq = Resources.LoadAsync(EffectRootName, typeof(GameObject));
		
		while (!resReq.isDone) { 
			yield return null; 
		}
		GameObject oriEffect = resReq.asset as GameObject;

		if (oriEffect == null)
		{
			Debug.LogWarning("AttachedWeaponEffect:Effect Not Find - " + EffectRootName);
			callback(null);
			yield break;
		}
		
		Transform[] trs = modelGo.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < trs.Length; i++)
		{
			if (trs[i].name == dummyname)
			{
				dummyTrs = trs[i];
			}
		}
		GameObject go = null;
		
		if (dummyTrs != null)
		{
			go = Instantiate(oriEffect) as GameObject;
			go.transform.parent = dummyTrs;
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			if (LayerChange)
				NGUITools.SetLayer(go, modelGo.layer);
		}
		else
		{
			Debug.LogWarning("AttachedWeaponEffect:NotFound - " + dummyname);
		}

		callback(go);
	}

    public static MAP_DIR RotateTo8Way(float Rotate)
    {
        if (Rotate >= 337.5f && Rotate <= 22.5 )
            return MAP_DIR.MAP_DIR_NORTH;
        else if (Rotate >= 22.5f && Rotate <= 67.5f)
            return MAP_DIR.MAP_DIR_NORTHEAST;
        else if (Rotate >= 67.5f && Rotate <= 112.5f)
            return MAP_DIR.MAP_DIR_EAST;
        else if (Rotate >= 112.5f && Rotate <= 157.5f)
            return MAP_DIR.MAP_DIR_SOUTHEAST;
        else if (Rotate >= 157.5f && Rotate <= 202.5f)
            return MAP_DIR.MAP_DIR_SOUTH;
        else if (Rotate >= 202.5f && Rotate <= 247.5f)
            return MAP_DIR.MAP_DIR_SOUTHWEST;
        else if (Rotate >= 247.5f && Rotate <= 292.5f)
            return MAP_DIR.MAP_DIR_WEST;
        else if (Rotate >= 292.5f && Rotate <= 337.5f)
            return MAP_DIR.MAP_DIR_NORTHWEST;

        return MAP_DIR.MAP_DIR_NULL;
    }

    public static float WayToRotate(MAP_DIR dir)
    {
        if( dir ==  MAP_DIR.MAP_DIR_NORTH )
        {
            return 0f;
        }
        else if (dir == MAP_DIR.MAP_DIR_NORTHEAST)
        {
            return 45f;
        }
        else if (dir == MAP_DIR.MAP_DIR_EAST)
        {
            return 90f;
        }
        else if (dir == MAP_DIR.MAP_DIR_SOUTHEAST)
        {
            return 135f;
        }
        else if (dir == MAP_DIR.MAP_DIR_SOUTH)
        {
            return 180f;
        }
        else if (dir == MAP_DIR.MAP_DIR_SOUTHWEST)
        {
            return 225f;
        }
        else if (dir == MAP_DIR.MAP_DIR_WEST)
        {
            return 270f;
        }
        else if (dir == MAP_DIR.MAP_DIR_NORTHWEST)
        {
            return 315f;
        }

        return 0f;
    }
}
