using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SelectHeroPanel : UIBasePanel
{
    enum ViewType { Select, Create };
    
    public TweenPosition[] TweenSelectBtn;//0~2
    public TweenPosition[] TweenCreateBtn;//0~2
    public TweenPosition[] CreateInfoTp;    //생성뷰 직업설명
    public TweenPosition[] SelectInfoTp;    //선택뷰 직업설명 
    public TweenAlpha SelectInfoBg; //선택뷰 직업설명뒤의 bg

    public UIButton BtnBack;//뒤로가기
    public UIButton BtnGameStart;//게임시작
    public UIButton BtnCreate;//캐릭 생성
    public UIButton BtnRandomNick;//랜덤 닉네임
    public UIInput InputNickName;

    public Transform Explanation;//캐릭터의 설명
    
    public UIEventTrigger[] BtnCreateChars;//슬롯 번호대로
    public UIEventTrigger[] BtnSelectChars;//슬롯 번호대로

    public GameObject[] SelectIcon; //선택/비선택
    public GameObject[] SelectCreateIcon; //생서뷰에서 선택/비선택

    public UISprite[] Faces;//생성한 캐릭터의 얼굴
    public UILabel[] NickNames;//생성한 캐릭터의 이름
    public UILabel[] Levels;//생성한 캐릭터의 레벨

    public Transform SelectInfo;    //선택뷰에서 직업설명
    public UISprite[] classInfo;     //생성뷰에서 직업설명


    //public Transform[] SelectCharSlots;//생성 or 선택하는 캐릭터 슬롯들 0~2
    public GameObject[] ViewObj;//생성, 선택에 관한 뷰. [0]선택, [1]생성
    private NetData.CharacterInfo[] SlotInfo = new NetData.CharacterInfo[3];
    private ViewType CurViewType;

    private Vector3 ExplStartPos;//캐릭터 설명의 시작 값
    private Vector3 CharOriginRot;    

    private float[] BtnToX;
    private float BtnFromX;

    //트윈,딜레이좌표들
    private float[] SelectInfoFromTo;
    private float[] SelectBtnFrom;
    private float[] SelectBtnTo;
    private float CreateBtnFrom;
    private float CreateBtnTo;
    private float[] CreateInfoFrom;
    private float[] CreateInfoTo;
    private float[] StartBtnFromTo;
    private float[] CreateBtnFromTo;
    private float[] CreateInfoFromTo;
    private float[] SelectInfoDelay;
    private float[] CretaeInfoDelay;

    private short SelectSlotArr;//선택한 슬롯은 몇번째인가?
	private short prevSelectedSlot;
    private short SelectJobArr;//선택한 캐릭터 직업 몇번째인가?

    public GameObject[] SelectHeroEffRoot;  //캐릭터선택이펙트 루트
    public GameObject[] SelectJobEffRoot;  //캐릭터생성이펙트 루트

    private bool IsUseDeletBtn = true;//삭제 버튼 사용
    
    private Dictionary<uint, List<uint>> CharModelDic = new Dictionary<uint, List<uint>>();//캐릭터 기본 모델링 생성 정보
    
    private GameObject pcModelBase;
    private GameObject pcModel;
	private GameObject pcModelForIntroOut;

	
	private bool bTouchBlockWhileModelChanging;
	private bool bTouchBlockWhileCutScenePlaying;

	private bool bInstantStart;

	CreateCharCutSceneManager cutSceneManager;

	private byte defaultCharIdx = 1;

    public override void Init()
    {
        base.Init();

        for (int i = 0; i < SlotInfo.Length; i++)
            SlotInfo[i] = new NetData.CharacterInfo();

        CharModelDic.Add(11000, new List<uint>() { 102, 510001, 510002 });//권사
        CharModelDic.Add(12000, new List<uint>() { 110, 520001, 520002 });//창
        CharModelDic.Add(13000, new List<uint>() { 120, 530001, 530002 });//의

        for (int i = 0; i < BtnSelectChars.Length; i++)
        {
            BtnSelectChars[i].transform.FindChild("Empty").gameObject.SetActive(true);
            BtnSelectChars[i].transform.FindChild("Char").gameObject.SetActive(false);
        }

        EventDelegate.Set(BtnBack.onClick, Close);//뒤로가기 버튼
        EventDelegate.Set(BtnSelectChars[0].onClick, delegate () { OnClickSelectSlot(0); });//캐릭터 슬롯 선택
        EventDelegate.Set(BtnSelectChars[1].onClick, delegate () { OnClickSelectSlot(1); });//캐릭터 슬롯 선택
        EventDelegate.Set(BtnSelectChars[2].onClick, delegate () { OnClickSelectSlot(2); });//캐릭터 슬롯 선택

        EventDelegate.Set(BtnCreateChars[0].onClick, delegate () { OnClickJobSlot2(0, true); });//캐릭터 직업(생성) 슬롯 선택
        EventDelegate.Set(BtnCreateChars[1].onClick, delegate () { OnClickJobSlot2(1, true); });//캐릭터 직업(생성) 슬롯 선택
        EventDelegate.Set(BtnCreateChars[2].onClick, delegate () { OnClickJobSlot2(2, true); });//캐릭터 직업(생성) 슬롯 선택

        EventDelegate.Set(BtnCreate.onClick, OnClickCreateChar);//캐릭터 생성 버튼
        EventDelegate.Set(BtnRandomNick.onClick, OnClickRandomName);//랜덤 이름 버튼
        //EventDelegate.Set(BtnDelete.onClick, OnDeleteChar);//캐릭터 삭제 버튼
        EventDelegate.Set(BtnGameStart.onClick, OnClickGameStart);//게임 시작 버튼

        Transform btnDelete = transform.FindChild("SelectView/BtnDelete");
        if (Debug.isDebugBuild)
        {
            IsUseDeletBtn = true;
            UIEventTrigger uiTri = btnDelete.GetComponent<UIEventTrigger>();
            EventDelegate.Set(uiTri.onClick, OnDeleteChar);
        }
        else
            IsUseDeletBtn = false;

        btnDelete.gameObject.SetActive(IsUseDeletBtn);

        pcModelBase = GameObject.Find("ModelPos");
        RotationTargetList.Add(pcModelBase);

        CharOriginRot = pcModelBase.transform.eulerAngles;
        //ExplStartPos = Explanation.transform.localPosition;

        BtnToX = new float[] {
            BtnSelectChars[0].transform.localPosition.x
            , BtnSelectChars[1].transform.localPosition.x
            , BtnSelectChars[2].transform.localPosition.x
        };

        BtnFromX = BtnCreateChars[1].transform.localPosition.x;

        SaveTransform();
        //for (int i = 0; i < Explanation.transform.childCount; i++)
        //{
        //    CreateInfoFrom[i] = Explanation.transform.get
        //}


        for (int i = 0; i < SelectIcon.Length; i++)
        {
            SelectIcon[i].SetActive(false);
        }

        UIHelper.CreateEffectInGame(ViewObj[0].transform.FindChild("bg_line/eff"), "Fx_UI_SelectCharacter_02");

        for (int i = 0; i < 3; i++)
        {
            UIHelper.CreateEffectInGame(SelectHeroEffRoot[i].transform, "Fx_UI_SelectCharacter_01");
            SelectHeroEffRoot[i].SetActive(false);
            //SelectHeroEffRoot[i].transform.GetChild(0).FindChild("selectHero_01").gameObject.SetActive(false);
            //SelectHeroEffRoot[i].transform.GetChild(0).FindChild("selectHero_03").gameObject.SetActive(false);

            UIHelper.CreateEffectInGame(SelectJobEffRoot[i].transform.FindChild("sel").transform, "Fx_UI_CreateCharacter_02");
            UIHelper.CreateEffectInGame(SelectJobEffRoot[i].transform.FindChild("none").transform, "Fx_UI_CreateCharacter_01");
            //SelectJobEffRoot[i].SetActive(false);
            //SelectJobEffRoot[i].transform.GetChild(0).FindChild("selectHero_02").gameObject.SetActive(false);
        }

        InputNickName.label.text = _LowDataMgr.instance.GetStringCommon(902);//닉네임을 입력하시오

		if (GameObject.Find ("2_SelectScene") != null) {
			cutSceneManager = GameObject.Find ("2_SelectScene").GetComponent<CreateCharCutSceneManager> ();
		}

    }
    void SaveTransform()
    {
        //트윈관련 좌표들 저장
        SelectBtnFrom = new float[] {
            BtnSelectChars[0].transform.GetComponent<TweenPosition>().from.x,
            BtnSelectChars[1].transform.GetComponent<TweenPosition>().from.x,
            BtnSelectChars[2].transform.GetComponent<TweenPosition>().from.x
        };
        SelectBtnTo = new float[] {
            BtnSelectChars[0].transform.GetComponent<TweenPosition>().to.x,
            BtnSelectChars[1].transform.GetComponent<TweenPosition>().to.x,
            BtnSelectChars[2].transform.GetComponent<TweenPosition>().to.x
        };
        CreateBtnFrom = BtnCreateChars[0].transform.GetComponent<TweenPosition>().from.x; ;
        CreateBtnTo = BtnCreateChars[0].transform.GetComponent<TweenPosition>().to.x; ;
        SelectInfoFromTo = new float[]
        {
            SelectInfo.gameObject.GetComponent<TweenPosition>().from.x,
            SelectInfo.gameObject.GetComponent<TweenPosition>().to.x
        };
        CreateInfoFromTo = new float[]
        {
            Explanation.GetComponent<TweenPosition>().from.x,
            Explanation.GetComponent<TweenPosition>().to.x
        };
        //CreateInfoFrom = new float[Explanation.transform.childCount];
        //CreateInfoTo = new float[Explanation.transform.childCount];

        //선택뷰설명의 딜레이값
        SelectInfoDelay = new float[]
        {
            SelectInfoTp[0].delay,
            SelectInfoTp[1].delay,
            SelectInfoTp[2].delay,
            BtnGameStart.GetComponent<TweenPosition>().delay,
            SelectInfo.GetComponent<TweenPosition>().delay
        };
        CretaeInfoDelay = new float[]
        {
            CreateInfoTp[0].delay,
            CreateInfoTp[1].delay,
            CreateInfoTp[2].delay,
            BtnGameStart.GetComponent<TweenPosition>().delay
        };

        StartBtnFromTo = new float[]
        {
             BtnGameStart.GetComponent<TweenPosition>().from.y,
              BtnGameStart.GetComponent<TweenPosition>().to.y
        };
        CreateBtnFromTo = new float[]
        {
            BtnCreate.GetComponent<TweenPosition>().from.y,
            BtnCreate.GetComponent<TweenPosition>().to.y
        };
    }

    public override void LateInit()
    {
        base.LateInit();
        ViewObj[(byte)ViewType.Create].SetActive(false);
        ViewObj[(byte)ViewType.Select].SetActive(false);
        if (NetData.instance.CharacterCount == 0)//생성해둔 캐릭터가 없다.
        {
            OnClickRandomName();
			OnClickJobSlot2(defaultCharIdx, false);
            ChangeSlots(ViewType.Create);
            return;
        }
        else
        {
            // 선택뷰순서
            // 왼쪽아이콘 -> 설명 -> 설명뒤bg -> 시작버튼순으로
            TempCoroutine.instance.FrameDelay(0.1f, () =>
            {
                float delay = 0f;
                int loopCount2 = TweenSelectBtn.Length;
                //아이콘
                for (int i = 0; i < loopCount2; i++)
                {
                    TweenPosition tp = TweenSelectBtn[i];
                    tp.to.x = SelectBtnTo[i];
                    tp.from.x = SelectBtnFrom[i];
                    tp.ResetToBeginning();
                    tp.PlayForward();

                    delay += /*tp.duration +*/ tp.delay;
                }

                //설명
                for (int i = 0; i < SelectInfoTp.Length; i++)
                {
                    TweenPosition tp = SelectInfoTp[i];
                    tp.delay += delay;
                    tp.ResetToBeginning();
                    tp.PlayForward();
                }
                //설명bg
                SelectInfoBg.ResetToBeginning();
                SelectInfoBg.delay += delay;
                SelectInfoBg.PlayForward();

                delay += SelectInfoBg.delay;

                //시작버튼
                TweenPosition btnTp = BtnGameStart.GetComponent<TweenPosition>();
                btnTp.from.y = StartBtnFromTo[0];
                btnTp.to.y = StartBtnFromTo[1];

                btnTp.ResetToBeginning();
                btnTp.delay += delay;
                btnTp.PlayForward();

                ViewObj[(byte)ViewType.Select].SetActive(true);

            });

#if UNITY_EDITOR

//                        bInstantStart = true;
//                        // quick start.
//                        TempCoroutine.instance.FrameDelay(5f, () =>
//                        {
//                            if (bInstantStart)
//                            {
//                                bInstantStart = false;
//                                OnClickGameStart();
//                            }
//                        });
#endif
        }

        SelectSlotArr = (short)PlayerPrefs.GetInt("SelectHeroSlot", 0);

		//FixSelectedSlotIndex ();

        Dictionary<ulong, NetData.CharacterInfo>.ValueCollection characters = NetData.instance.GetCharacters().Values;

        for (int i = 0; i < characters.Count; i++)
        {
            NetData.CharacterInfo charInfo = characters.ElementAt(i);

            uint slot = (uint)charInfo.slot_no - 1;

            //Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData((uint)charInfo.character_id);
            Faces[slot].spriteName = PortName(charInfo.character_id.ToString());/*charLowData.PortraitId;*/
            Levels[slot].text = string.Format(_LowDataMgr.instance.GetStringCommon(453), charInfo.level);//레벨값 받아와야함
            NickNames[slot].text = charInfo.nickname;
            
            BtnSelectChars[slot].transform.FindChild("Empty").gameObject.SetActive(false);
            BtnSelectChars[slot].transform.FindChild("Char").gameObject.SetActive(true);
            
            SlotInfo[slot].Set(charInfo);

            if (slot == SelectSlotArr)
            {
                OnClickSelectIcon(charInfo.character_id);
                SelectIcon[SelectSlotArr].SetActive(true);
                SelectHeroEffRoot[SelectSlotArr].SetActive(true);
				CreatePcAndEffect(SlotInfo[slot], eAnimName.Anim_idle, eAnimName.Anim_none);
            }
        }
    }
    void ResetDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            SelectInfoTp[i].delay = SelectInfoDelay[i];
        }
        BtnGameStart.GetComponent<TweenPosition>().delay = SelectInfoDelay[3];
        SelectInfo.GetComponent<TweenPosition>().delay = SelectInfoDelay[4];
    }
    void ResetCreateDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateInfoTp[i].delay = CretaeInfoDelay[i];
        }
        BtnCreate.GetComponent<TweenPosition>().delay = CretaeInfoDelay[3];
    }
    /// <summary> 슬롯들 움직이게 하는 함수. 무조건 캐릭터 생성으로 갔을 때부터 시작임. </summary>
    void ChangeSlots(ViewType type)
    {
		if (CurViewType == type)
			return;

        CurViewType = type;
        
        if (type == ViewType.Create)//지금 선택뷰 였다는 얘기
        {
			uiMgr.UICamera.camera.enabled = true;

            ResetDelay();
            
			for(int i=0;i< TweenSelectBtn.Length;i++){
				TweenSelectBtn[i].gameObject.SetActive(false);
			}

            SelectInfo.gameObject.SetActive(false);
            BtnGameStart.gameObject.SetActive(false);

            if (IsUseDeletBtn)
                transform.FindChild("SelectView/BtnDelete").gameObject.SetActive(false);

			OnCallbackCreateView();

			Dictionary<ulong, NetData.CharacterInfo>.ValueCollection characters = NetData.instance.GetCharacters().Values;
			if (characters.Count == 0){
				BtnBack.gameObject.SetActive(false);
			}
			else{
				BtnBack.gameObject.SetActive(true);
			}

        }
        else//지금 생성뷰 였다는 얘기
        {

			cutSceneManager.stopAll();
			cutSceneManager.setActive(false);

			CameraManager.instance.mainCamera.enabled = true;

			uiMgr.UICamera.enabled = true;
			uiMgr.UICamera.camera.enabled = true;

            ResetCreateDelay();
            float delay = TweenCreateBtn[0].duration;
            int loopCount = TweenCreateBtn.Length;
            for (int i = 0; i < loopCount; i++)
            {
                TweenPosition tp = TweenCreateBtn[i];
                tp.to.x = CreateBtnFrom;
                tp.from.x = CreateBtnTo;
                tp.ResetToBeginning();
                tp.PlayForward();

                delay += tp.delay;
            }

            Explanation.GetComponent<TweenPosition>().to.x = CreateInfoFromTo[0];
            Explanation.GetComponent<TweenPosition>().from.x = CreateInfoFromTo[1];
            Explanation.GetComponent<TweenPosition>().ResetToBeginning();
            Explanation.GetComponent<TweenPosition>().delay = 0;
            Explanation.GetComponent<TweenPosition>().PlayForward();

            BtnCreate.GetComponent<TweenPosition>().from.y = CreateBtnFromTo[1];
            BtnCreate.GetComponent<TweenPosition>().to.y = CreateBtnFromTo[0];
            BtnCreate.GetComponent<TweenPosition>().ResetToBeginning();
            BtnCreate.GetComponent<TweenPosition>().delay = 0;
            BtnCreate.GetComponent<TweenPosition>().PlayForward();

            ViewObj[(byte)ViewType.Create].transform.FindChild("CreateNickName").gameObject.SetActive(false);

			OnCallbackSelectView();

			SelectInfo.gameObject.SetActive(true);
			BtnGameStart.gameObject.SetActive(true);

            SelectSlotArr = prevSelectedSlot;
            //FixSelectedSlotIndex();
        }
    }

    /// <summary> 생성뷰로 전환된 후 호출 </summary>
    void OnCallbackCreateView()
    {
        ResetCreateDelay();
        float delay = 0f;
        //생성뷰 
        //직업아이콘 -> 직업설명 -> 생성버튼
        ViewObj[(byte)ViewType.Select].SetActive(false);

        int loopCount2 = TweenCreateBtn.Length;
        for (int i = 0; i < loopCount2; i++)
        {
            TweenPosition tp = TweenCreateBtn[i];
            tp.to.x = CreateBtnTo;
            tp.from.x = CreateBtnFrom;
            tp.ResetToBeginning();
            tp.PlayForward();

            if (i < 2)
                delay += tp.delay;
        }

        for (int i = 0; i < CreateInfoTp.Length; i++)
        {
            CreateInfoTp[i].delay += delay;
            CreateInfoTp[i].ResetToBeginning();
            CreateInfoTp[i].ResetToBeginning();
            CreateInfoTp[i].PlayForward();

            if (i < 2)
                delay += CreateInfoTp[i].delay;
        }

        TweenPosition btnTp = BtnCreate.GetComponent<TweenPosition>();
        btnTp.from.y = CreateBtnFromTo[0];
        btnTp.to.y = CreateBtnFromTo[1];

        btnTp.ResetToBeginning();
        btnTp.delay += delay;
        btnTp.PlayForward();


        ViewObj[(byte)ViewType.Create].transform.FindChild("CreateNickName").gameObject.SetActive(true);
        ViewObj[(byte)CurViewType].SetActive(true);
    }

    /// <summary> 생성뷰로 전환된 후 호출 </summary>
    void OnCallbackSelectView()
    {
        ViewObj[(byte)CurViewType].SetActive(true);
        float delay = TweenCreateBtn[0].duration;

        ResetDelay();
        float flowDelay = 0f;

        ViewObj[(byte)ViewType.Create].SetActive(false);

        int loopCount2 = TweenSelectBtn.Length;
        for (int i = 0; i < loopCount2; i++)
        {
			TweenSelectBtn[i].gameObject.SetActive(true);
            TweenPosition tp = TweenSelectBtn[i];
            tp.to.x = SelectBtnTo[i];
            tp.from.x = SelectBtnFrom[i];
            tp.ResetToBeginning();
            tp.PlayForward();

            flowDelay += tp.delay;
        }

        for (int i = 0; i < SelectInfoTp.Length; i++)
        {
            TweenPosition tp = SelectInfoTp[i];
            tp.delay += delay;
            tp.ResetToBeginning();
            tp.PlayForward();
        }

        SelectInfoBg.ResetToBeginning();
        SelectInfoBg.delay += delay;
        SelectInfoBg.PlayForward();

        TweenPosition btnTp = BtnGameStart.GetComponent<TweenPosition>();
        btnTp.from.y = StartBtnFromTo[0];
        btnTp.to.y = StartBtnFromTo[1];

        btnTp.delay += delay;
        btnTp.ResetToBeginning();
        btnTp.PlayForward();


        if (IsUseDeletBtn)
            transform.FindChild("SelectView/BtnDelete").gameObject.SetActive(true);

        SelectInfo.gameObject.transform.localPosition = SelectInfo.GetComponent<TweenPosition>().from;
        SelectInfo.gameObject.SetActive(true);
		SelectInfo.GetComponent<TweenPosition> ().PlayForward ();
    }
    
    /// <summary> 외부에서 강제 종료 </summary>
    public void OnCloseCompulsion()
    {
        if (pcModel != null)
            Destroy(pcModel);

		pcModelBase.transform.eulerAngles = CharOriginRot;

        LoginState state = SceneManager.instance.GetState<LoginState>();
        if (state != null)
            state.LoginSceneProcess();

        base.Close();
    }
    
#region 이벤트 버튼 함수
    public override void Close()
    {
		if (bTouchBlockWhileModelChanging)return;
		if (bTouchBlockWhileCutScenePlaying)
			return;

        if (ViewObj[(byte)ViewType.Select].activeSelf)//서버 선택으로 넘어감.
        {
            OnCloseCompulsion();
        }
        else {
            if (CurViewType == ViewType.Select)
                return;
                        
            RotationBoxList[0].SetActive(true);
            ChangeSlots(ViewType.Select);
        }
    }

    /// <summary> 캐릭터 슬롯 선택. 0 ~ 2슬롯</summary>
    void OnClickSelectSlot(byte arr)
    {
		if (CurViewType == ViewType.Create)
			return;

		if (bTouchBlockWhileModelChanging)return;

		bool bRefreshPc = false;
		if (pcModel == null) {
			bRefreshPc = true;
		}
		if (pcModel != null && SelectSlotArr != arr) {
			bRefreshPc = true;
		}

		bInstantStart = false;
        
        if (0 < SlotInfo[arr].c_usn)//있다.
        {

			if (SelectSlotArr == arr)return; // do nothing when same slot clicked. 

			SelectSlotArr = arr;
			PlayerPrefs.SetInt("SelectHeroSlot", SelectSlotArr);

			if (bRefreshPc){
				CreatePcAndEffect(SlotInfo[arr], eAnimName.Anim_intro, eAnimName.Anim_idle, 1);
            }

            for (int i = 0; i < SelectIcon.Length; i++)
            {
                SelectIcon[i].SetActive(i == arr);
            }

            float delay = 0f;
            //선택시 설명 연출한번더
            ResetDelay();
            TweenPosition infoTp = SelectInfo.GetComponent<TweenPosition>();

            infoTp.to.x = SelectInfoFromTo[1];
            infoTp.from.x = SelectInfoFromTo[0];
            infoTp.ResetToBeginning();
            infoTp.PlayForward();
            delay += infoTp.delay + infoTp.duration;

            for (int i = 0; i < SelectInfoTp.Length; i++)
            {
                TweenPosition tp = SelectInfoTp[i];
                tp.delay += delay;
                tp.ResetToBeginning();
                tp.PlayForward();
            }

            SelectInfoBg.ResetToBeginning();
            SelectInfoBg.PlayForward();

            OnClickSelectIcon(SlotInfo[arr].character_id);
            for (int i = 0; i < SelectHeroEffRoot.Length; i++)
            {
                SelectHeroEffRoot[i].SetActive(i==arr);
            }
        }
        else//없다. 생성뷰로 전환
        {
            OnClickRandomName();
			SelectJobArr = -1;//0;
			prevSelectedSlot = SelectSlotArr;
			SelectSlotArr = arr;

            //OnClickJobSlot(0, false);
            //ChangeSlots(ViewType.Create);

			OnClickJobSlot2(defaultCharIdx, false); // 1 is default character value.

        }
    }
    
    void ChangeShader(Material[] ms, float shin, float rim, float ambi)
    {
        for (int j = 0; j < ms.Length; j++)
        {
            Material m = ms[j];
            m.SetFloat("_Shininess", shin);
            m.SetFloat("_RimPower", rim);
            m.SetFloat("_Ambient", ambi);
        }
    }

	public void CreatePcAndEffect(NetData.CharacterInfo charInfo, eAnimName firstAni, eAnimName secondAni, byte introType = 0)
	{
		uint charIdx = charInfo.character_id;
		uint HeadItemIdx = charInfo.Head;
		uint CostumeItemIdx = charInfo.CostumeId;
		uint ClothItemIdx = charInfo.Cloth;
		uint WeaponItemIdx = charInfo.Weapon;
		bool HideCostume = charInfo.IsHideCostume;

        StopAllCoroutines();

		StartCoroutine (destroyAndReloadPcAndAttachEffect (charIdx, HeadItemIdx, CostumeItemIdx, ClothItemIdx, WeaponItemIdx, charInfo.SkillSetId, HideCostume, firstAni, secondAni, introType));
    }
    
	IEnumerator destroyAndReloadPcAndAttachEffect(uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, uint skillSetId, bool HideCostume, eAnimName firstAni, eAnimName secondAni, byte introType)
	{
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();

		//SceneManager.instance.showStopWatchTimer ("destroyAndReloadPcAndAttachEffect start");

		bTouchBlockWhileModelChanging = true;

        RotationBoxList[0].SetActive(false);
        if (pcModel != null) {

			if (introType != 0) {
				clonePcAndIntroOutAni();
			} else {
				Destroy (pcModel);
			}
		}

		//SceneManager.instance.showStopWatchTimer ("clonePcAndIntroOutAni");

		GameObject[] effect = new GameObject[2];
		GameObject _unit = UnitModelHelper.PCModelLoadRimSpec (charIdx, HeadItemIdx, CostumeItemIdx, ClothItemIdx, WeaponItemIdx, HideCostume, ref effect);
		//Item.CostumeInfo lowData = _LowDataMgr.instance.GetLowDataCostumeInfo (CostumeItemIdx);
		pcModel = _unit;
		Resource.AniInfo[] ads = null;

		//SceneManager.instance.showStopWatchTimer ("PCModelLoadRimSpec");
        pcModel.transform.parent = pcModelBase.transform;
		pcModelBase.transform.eulerAngles = CharOriginRot;
		setPcModelScale ((int)charIdx);
		pcModel.transform.localPosition = new Vector3 (0, 0, -100);// set position outside screen.

		//SceneManager.instance.showStopWatchTimer ("setPcModelScale");

        if (effect != null)
        {
            ResettingParticle(_unit.transform.localScale.x, UIHelper.FindComponents<ParticleSystem>(effect[0].transform), 0.5f);
            if (effect[1] != null)
                ResettingParticle(_unit.transform.localScale.x, UIHelper.FindComponents<ParticleSystem>(effect[1].transform), 0.5f);
        }
        
            //ads = _LowDataMgr.instance.UIAniInfoSetting(lowData.Bodyprefab, _unit, lowData.AniId, 1);
            // UIAniInfoSetting, 실제로 여기서 가장 오래 걸림.
            SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        if(setInfo != null)
        { 
			yield return StartCoroutine(_LowDataMgr.instance.UIAniInfoSettingAsync(setInfo.AniPath, _unit, setInfo.AniId, 1, (retVal)=>{
				ads = retVal;
			}));
		}

		UnitAnimator unitAnim = new UnitAnimator();
		unitAnim.Init(_unit, _unit.animation, ads);
		unitAnim.Animation.playAutomatically = false;


//#if UNITY_EDITOR
//		float _d = GameObject.Find ("SelectCharDebugHelper").GetComponent<SelectCharDebugHelper> ().charChangeDelay;
//#endif

		float delayTime_for_intro = 0f;
		if (firstAni == eAnimName.Anim_intro) {
//			#if UNITY_EDITOR
//			yield return new WaitForSeconds (_d);
//			#else
			yield return new WaitForSeconds (1.0f);
//			#endif

			unitAnim.PlayAnim (eAnimName.Anim_intro_start);
			delayTime_for_intro = unitAnim.GetAnimLength(eAnimName.Anim_intro_start);

			////////////////////////////// attach ani's effect
			string path =  getIntroStartAniEffectPath(charIdx);
			string effName = getIntroStartAniEffectName(charIdx);
			GameObject effGo = GameObject.Instantiate(Resources.Load(string.Format("Effect/_PC/{0}/{1}", path, effName) )) as GameObject;
			effGo.transform.parent = pcModel.transform;// _unit.transform;
			effGo.transform.localScale = Vector3.one;// _unit.transform.localScale;
			effGo.transform.localPosition = Vector3.zero;

			if (charIdx == 13000){
				if (effect[0] != null)effect[0].SetActive(false);
				if (effect[1] != null)effect[1].SetActive(false);	

				TempCoroutine.instance.FrameDelay(4.8f, () => {

					if (effect[0] != null)effect[0].SetActive(true);
					if (effect[1] != null)effect[1].SetActive(true);
				});
			}
			////////////////////////////// attach ani's effect

			unitAnim.PlayAnim (firstAni, true, 0.1f, false, true, false);

		} else {
			unitAnim.PlayAnim(firstAni);
			delayTime_for_intro = 0f;
		}

		// unit effect
		StartCoroutine (playUnitEffect (delayTime_for_intro, charIdx, unitAnim, _unit, firstAni, secondAni));

		unitAnim.PlayAnim(secondAni, true, 0.1f, true, true, false);

		pcModel.transform.localPosition = Vector3.zero;
		pcModelBase.transform.SetChildLayer(14);
        
		yield return null;
	}
	
	IEnumerator playUnitEffect(float _delay, uint charIdx, UnitAnimator unitAnim, GameObject _unit, eAnimName firstAni, eAnimName secondAni){
		
		yield return new WaitForSeconds(_delay);
		
		if (_unit == null) {
			yield break;
		}
		
		//if(secondAni != eAnimName.Anim_none) 
		if (firstAni == eAnimName.Anim_intro)
		{ 
			unitAnim.PlayAnimationSound(firstAni);
			string effName = unitAnim.GetAnimationEffect(firstAni);// FX_pc_f_intro
			if ( !string.IsNullOrEmpty(effName))
			{
				string path = getIntroStartAniEffectPath(charIdx);
				GameObject effGo = GameObject.Instantiate(Resources.Load(string.Format("Effect/_PC/{0}/{1}", path, effName) )) as GameObject;
				effGo.transform.parent = pcModelBase.transform;
				effGo.transform.localScale = Vector3.one;
				effGo.transform.localPosition = Vector3.zero;
			}
			
			float runTime = unitAnim.GetAnimLength(firstAni);
			yield return new WaitForSeconds(runTime);
			RotationBoxList[0].SetActive(true);
		}
		else
			RotationBoxList[0].SetActive(true);
		
		bTouchBlockWhileModelChanging = false;
	}

	private string getIntroStartAniEffectName(uint charIdx){
		// intro start ani's effect 

		//string path = "pojol";
		string effName = "Fx_pc_p_intro_start";

		if (charIdx == 11000) {
			//path = "fighter";
			effName = "Fx_pc_f_intro_start";
		} else if (charIdx == 13000) {
			//path = "doctor";
			effName = "Fx_pc_d_intro_start";
		}

		return effName;

	}

	private string getIntroStartAniEffectPath(uint charIdx){
		// intro start ani's effect 
		
		string path = "pojol";
		
		if (charIdx == 11000) {
			path = "fighter";
		} else if (charIdx == 13000) {
			path = "doctor";
		}
		
		return path;
		
	}

	void setPcModelScale(int charIdx){
		if (charIdx == 11000)//권
		{
			//pcModel.transform.localScale = new Vector3(2.25f, 2.25f, 2.25f);
			//pcModel.transform.localEulerAngles = new Vector3(0, 0, 0);
			pcModel.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
			pcModel.transform.localEulerAngles = new Vector3(0, 6.87f, 0);
		}
		else if (charIdx == 12000)//창
		{
			//pcModel.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
			//pcModel.transform.localEulerAngles = new Vector3(0, 2.22f, 0);
			pcModel.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
			pcModel.transform.localEulerAngles = new Vector3(0, 4.929999f, 0);
		}
		else if (charIdx == 13000)//의
		{
			//pcModel.transform.localScale = new Vector3(2.4f, 2.4f, 2.4f);
			//pcModel.transform.localEulerAngles = new Vector3(0, 1.53f, 0);
			pcModel.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);
			pcModel.transform.localEulerAngles = new Vector3(0, 4.62f, 0);

		}

	}

	void clonePcAndIntroOutAni(){
		if (pcModelForIntroOut != null) {
			StopCoroutine ("playUnitEffect");
			Destroy (pcModelForIntroOut);
		}

        pcModelForIntroOut = (GameObject)Instantiate (pcModel, pcModel.transform.position, pcModel.transform.rotation);
		pcModelForIntroOut.transform.parent = pcModelBase.transform;

		Destroy (pcModel);
		
		string aniName = "pc_f_intro_end_01";
		string basicEffectForRemove = "Fx_pc_f_intro(Clone)";
        if (pcModelForIntroOut.name.Contains("pc_d"))
        {
            aniName = "pc_d_intro_end_01";
            basicEffectForRemove = "Fx_pc_d_intro(Clone)";
        }
        else if (pcModelForIntroOut.name.Contains("pc_p"))
        {
            aniName = "pc_p_intro_end_01";
            basicEffectForRemove = "Fx_pc_p_intro(Clone)";
        }

        // 게임오브젝트를 Instantiate할때 케릭터에 붙어있던 이펙트가 다시 생겨나서 이를 지워준다. intro out시에는 필요없는 이펙트임.
        if (pcModelForIntroOut.transform.FindChild (basicEffectForRemove) != null) {
			pcModelForIntroOut.transform.FindChild (basicEffectForRemove).gameObject.SetActive(false);
		}
		
		pcModelForIntroOut.GetComponent<Animation> ().Play (aniName);
		float _delay = pcModelForIntroOut.GetComponent<Animation> () [aniName].length;
		
		TempCoroutine.instance.FrameDelay (_delay, () => {
			Destroy (pcModelForIntroOut);
		});

		// intro out effect
		string effName = null;
		string path = "pojol";
		if (pcModelForIntroOut.name.Contains("pc_p")){
			effName = "Fx_pc_p_intro_end";
		}
		else if (pcModelForIntroOut.name.Contains("pc_f")){
			path = "fighter";
			effName = "Fx_pc_f_intro_end";
		}
		else if (pcModelForIntroOut.name.Contains("pc_d")){
			path = "doctor";
			effName = "Fx_pc_d_intro_end";
		}

		if (effName != null) {
			GameObject effGo = GameObject.Instantiate(Resources.Load(string.Format("Effect/_PC/{0}/{1}", path, effName) )) as GameObject;
			effGo.transform.parent = pcModelForIntroOut.transform;// _unit.transform;
			effGo.transform.localScale = Vector3.one;// _unit.transform.localScale;
			effGo.transform.localPosition = Vector3.zero;
		}
	}

	IEnumerator waitAndDestroy(float _delay, GameObject obj){

		yield return new WaitForSeconds(_delay);

		Destroy (obj);

		yield return null;

	}
	
	


	/// <summary> 생성 유아이에서 캐릭터를 선택함. 생성한다. 0권술사, 1포졸, 2의사</summary>
	void OnClickJobSlot2(byte arr, bool isSelect)
	{

		List<Character.CharacterInfo> charList = _LowDataMgr.instance.GetCharacterDataToList();
		if (charList.Count <= arr)//오류
			return;

		if (SelectJobArr == arr) {
			return;
		}
/*
		if (cutSceneManager.isSeqPlaying ())
			return;

		SelectJobArr = arr;
	
		cutSceneManager.seqPlay(SelectJobArr);

		//StartCoroutine (OnClickJobSlotStep2 ());
		*/
		StartCoroutine (OnClickJobSub (arr, isSelect));
	}

	IEnumerator OnClickJobSub(byte arr, bool isSelect){

		bTouchBlockWhileCutScenePlaying = true;
		InputNickName.gameObject.SetActive (false);

		if (cutSceneManager == null) {

			Application.backgroundLoadingPriority = ThreadPriority.High;

			SceneManager.instance.ShowNetProcess("LoginScene_Addtive");

			AsyncOperation async = Application.LoadLevelAdditiveAsync("LoginScene_Addtive");

			async.allowSceneActivation = false;

			while (async.progress < 0.9f)
			{
				yield return null;
			}

			async.allowSceneActivation = true;

			while (!async.isDone)
				yield return null;

			yield return new WaitForEndOfFrame();

			yield return new WaitForSeconds(0.1f);

			if (GameObject.Find ("2_SelectScene") != null) {
				cutSceneManager = GameObject.Find ("2_SelectScene").GetComponent<CreateCharCutSceneManager> ();
			}

			SceneManager.instance.EndNetProcess("LoginScene_Addtive");
			Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		}
		
		if (cutSceneManager.isSeqPlaying ())
			yield break;
		
		SelectJobArr = arr;
		
		cutSceneManager.seqPlay (SelectJobArr, () => {
			bTouchBlockWhileCutScenePlaying = false;
			InputNickName.gameObject.SetActive (true);
		});
	}

	public void OnClickJobSlotStep2(){

		for(int i=0;i< SelectCreateIcon.Length;i++)
		{
			SelectCreateIcon[i].SetActive(i == SelectJobArr);
		}
		
		//선택시 설명 연출한번더
		ResetCreateDelay();
		float delay = 0f;
		
		TweenPosition infoTp = Explanation.GetComponent<TweenPosition>();
		infoTp.to.x = CreateInfoFromTo[1];
		infoTp.from.x = CreateInfoFromTo[0];
		infoTp.ResetToBeginning();
		infoTp.PlayForward();
		delay += infoTp.delay + infoTp.duration;
		
		for (int i = 0; i < CreateInfoTp.Length; i++)
		{
			CreateInfoTp[i].delay += delay;
			CreateInfoTp[i].ResetToBeginning();
			CreateInfoTp[i].PlayForward();
			
			if (i < 2)
				delay += CreateInfoTp[i].delay;
		}
		
		OnClickCreateSelectIcon(SelectJobArr);

        for (int i = 0; i < SelectJobEffRoot.Length; i++)
        {
            SelectJobEffRoot[i].transform.FindChild("sel").gameObject.SetActive(i == SelectJobArr);
            SelectJobEffRoot[i].transform.FindChild("none").gameObject.SetActive(i != SelectJobArr);
        }

        ChangeSlots(ViewType.Create);

	}


	/*
    /// <summary> 생성 유아이에서 캐릭터를 선택함. 생성한다. 0권술사, 1포졸, 2의사</summary>
    void OnClickJobSlot(byte arr, bool isSelect)
    {
        List<Character.CharacterInfo> charList = _LowDataMgr.instance.GetCharacterDataToList();
        if (charList.Count <= arr)//오류
            return;

		//Debug.Log (" ---------- OnClickJobSlot, SelectJobArr :" + SelectJobArr+", arr:"+arr);
		//Debug.Log (" bBlockForChangingModel :" + bBlockForChangingModel);

		//bool refreshPc = false;
		if (pcModel == null) {
			//refreshPc = true;

			// 인트로 아웃 애니되는 오브젝트 destroy & 새로운 케릭터의 인트로 인 오브젝트 생성사이에 클릭되어 들어오는것을 막는다. 
			//if (bBlockForChangingModel)return;
		}
		else if( isSelect) {
            //if (SelectJobArr != arr) {
            //	refreshPc = true;
            //}
            if (arr == 0 && pcModel.name.Contains("pc_f") ){
                return;
            }
            else if (arr == 1 && pcModel.name.Contains("pc_p") ){
                return;
            }
            else if (arr == 2 && pcModel.name.Contains("pc_d") ){
                return;
            }
        }

		// if intro start ani playing, cannot select other character.
		if (pcModel != null) {

            string aniName = "pc_f_intro_start_01";
            string aniName_2 = "pc_f_intro_end_01";
            if (pcModel.name.Contains("pc_d")) {
                aniName = "pc_d_intro_start_01";
                aniName_2 = "pc_d_intro_end_01";
            }
            else if (pcModel.name.Contains("pc_p")) {
                aniName = "pc_p_intro_start_01";
                aniName_2 = "pc_p_intro_end_01";
            }

            if (pcModel.animation.IsPlaying(aniName) || pcModel.animation.IsPlaying(aniName_2) || pcModelForIntroOut != null) {
                return;
            }
		}
        
        SelectJobArr = arr;

		//if (refreshPc) {
			TempCoroutine.instance.StopCoroutine ("FrameDelay");
			//TempCoroutine.instance.FrameDelay (0.5f, () =>
			//{
			//	JobInfo.ResetToBeginning (SceneManager.instance.IsPlaySoundFx, _LowDataMgr.instance.GetStringUnit (charList [SelectJobArr].DescriptionId));
			//});
        //}

        //임시로하드코딩 
        //if (refreshPc) {
        List<uint> info = null;
        if (CharModelDic.TryGetValue (charList [arr].Id, out info)) {
			CreatePlayerModel (charList [arr].Id, 0, info [0], info [1], info [2], false, eAnimName.Anim_intro, eAnimName.Anim_idle, 1);
		}
        //}
        
        for(int i=0;i< SelectCreateIcon.Length;i++)
        {
            SelectCreateIcon[i].SetActive(i == arr);
        }

        //선택시 설명 연출한번더
        ResetCreateDelay();
        float delay = 0f;

        TweenPosition infoTp = Explanation.GetComponent<TweenPosition>();
        infoTp.to.x = CreateInfoFromTo[1];
        infoTp.from.x = CreateInfoFromTo[0];
        infoTp.ResetToBeginning();
        infoTp.PlayForward();
        delay += infoTp.delay + infoTp.duration;

        for (int i = 0; i < CreateInfoTp.Length; i++)
        {
            CreateInfoTp[i].delay += delay;
            CreateInfoTp[i].ResetToBeginning();
            CreateInfoTp[i].PlayForward();

            if (i < 2)
                delay += CreateInfoTp[i].delay;
        }

        OnClickCreateSelectIcon(SelectJobArr);
    }
*/    
    /// <summary> 랜덤 닉네임 클릭함. </summary>
    void OnClickRandomName()
    {
        List<Character.CharacterInfo> charList = _LowDataMgr.instance.GetCharacterDataToList();
        //Explanation.text = _LowDataMgr.instance.GetStringUnit(charList[SelectJobArr].DescriptionId);

        uint charLowDataID = charList[SelectJobArr].Id;

        string rndNickName = null;
        while (true)
        {
            rndNickName = _LowDataMgr.instance.GetStringRndNickName(charLowDataID);
            if ( !_LowDataMgr.instance.IsBanString(rndNickName))
                break;
        }
        
        InputNickName.value = rndNickName;
    }

    /// <summary> 캐릭터 생성 버튼 </summary>
    void OnClickCreateChar()
    {
        string nick = InputNickName.value;
        nick = nick.Replace(" ", "");//띄어쓰기 삭제
        nick = nick.Replace("\n", "");//개행 삭제
        nick = nick.Replace("\"", "");//큰 따옴표 삭제

        //string touchStr = _LowDataMgr.instance.GetStringCommon(64);//터치
        if (string.IsNullOrEmpty(nick) || nick.Equals(_LowDataMgr.instance.GetStringCommon(902) ))//기본 닉네임으로 함. 안됨.
        {
            //UIMgr.instance.AddPopup(13, null, null, null);
            uiMgr.AddPopup(141, 121, 117);
            return;
        }

        if(_LowDataMgr.instance.IsBanString(nick))
        {
            uiMgr.AddPopup(141, 898, 117);
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        NativeHelper.instance.DisableNavUI();
#endif

        //선택 슬롯 갱신 및 무슨 캐릭인지 알아야함
        List<Character.CharacterInfo> charList = _LowDataMgr.instance.GetCharacterDataToList();

        uint cLowDataIdx = charList[SelectJobArr].Id;
        //HttpSender.instance.SendCreateChar(cLowDataIdx, nick, (SelectSlotArr + 1), OnCharListInfo);

		if (SelectSlotArr == -1) {
			SelectSlotArr = 0;
		}

        NetworkClient.instance.SendPMsgRoleCreateNewC(nick, (int)cLowDataIdx, SelectSlotArr + 1);

		//cutSceneManager.setUILight (true);

    }
    /*
    /// <summary> 캐릭터 생성후 리스트를 다시 받아온다.</summary>
    public void OnCharListInfo()//2016.12.05 부터 생성후 바로 시작하게 변경 (현재 미사용)
    {
        Dictionary<ulong, NetData.CharacterInfo>.ValueCollection characters = NetData.instance.GetCharacters().Values;
        for (int i = 0; i < characters.Count; i++)
        {
            NetData.CharacterInfo charInfo = characters.ElementAt(i);

            uint slot = (uint)charInfo.slot_no - 1;

            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData((uint)charInfo.character_id);
            Faces[slot].spriteName = PortName(charInfo.character_id.ToString());/*charLowData.PortraitId;*//*
            Levels[slot].text = string.Format(_LowDataMgr.instance.GetStringCommon(453), charInfo.level);//레벨값 받아와야함
            NickNames[slot].text = charInfo.nickname;

            //SetClassBackGround(charLowData.Class, slot);

            BtnSelectChars[slot].transform.FindChild("Empty").gameObject.SetActive(false);
            BtnSelectChars[slot].transform.FindChild("Char").gameObject.SetActive(true);

            SlotInfo[slot].nickName = charInfo.nickname;
            SlotInfo[slot].charIdx = charInfo.c_usn;
            SlotInfo[slot].charId = charInfo.character_id;
            SlotInfo[slot].level = charInfo.level;
            if (SelectSlotArr == slot)
            {
                //Explanation.text = _LowDataMgr.instance.GetStringUnit(charLowData.DescriptionId);

                uint costume = (uint)NetData.instance.GetCostumeByChar(SlotInfo[slot].charIdx).costume_id;
                CreatePlayerModel(SlotInfo[slot].charId, 0, costume, 0, 0, false, eAnimName.Anim_idle, eAnimName.Anim_none);
            }
        }

        ChangeSlots(ViewType.Select);
    }
    */
    string PortName(string pot)
    {
        string name = "";

        switch (pot)
        {
            case "11000":
                name = "pc_f_port_02";
                break;
            case "12000":
                name = "pc_p_port_02";
                break;
            case "13000":
                name = "pc_d_port_02";
                break;


            default:
                break;
        }

        return name;
    }

    //선택뷰에서 보여줄 직업에관한 설명들
    void OnClickSelectIcon(uint charType)
    {
        string charName = "";
        string charInfo = "";

        switch (charType)
        {
            case 11000:
                charName = "fighter_name_txt";
                charInfo = "fighter_info_txt";
                break;
            case 12000:
                charName = "pozol_name_txt";
                charInfo = "pozol_info_txt";
                break;
            case 13000:
                charName = "doctor_name_txt";
                charInfo = "doctor_info_txt";
                break;
            default:
                break;
        }

        SelectInfo.FindChild("img_txt_cha").GetComponent<UISprite>().spriteName = charName;
        SelectInfo.FindChild("img_txt_info").GetComponent<UISprite>().spriteName = charInfo;
    }

    //생성뷰에서 보여줄 직업에관한 설명들
    void OnClickCreateSelectIcon(short charType)
    {
        string charName = "";
        string charInfo = "";

        switch (charType)
        {
            case 0:
                charName = "fighter_name_txt";
                charInfo = "fighter_message_txt_";
                break;
            case 1:
                charName = "pozol_name_txt";
                charInfo = "pozol_message_txt_";
                break;
            case 2:
                charName = "doctor_name_txt";
                charInfo = "doctor_message_txt_";
                break;
            default:
                break;
        }

        classInfo[0].spriteName = charName;

        for (int i = 1; i < 5; i++)
        {
            string path = string.Format("message_txt_0{0}", i);
            classInfo[i].spriteName = string.Format("{0}0{1}", charInfo, i);
        }
    }


    /// <summary> 캐릭터 삭제 </summary>
    public void OnDeleteChar()
    {
        if (SelectSlotArr < 0 || SlotInfo[SelectSlotArr].c_usn <= 0)
            return;

		bInstantStart = false;
        ulong charIdx = SlotInfo[SelectSlotArr].c_usn;
        NetworkClient.instance.SendPMsgRoleDeleteC((long)charIdx);
    }

    /// <summary> 캐릭터 삭제 응답 함수 </summary>
    public void OnAnswerDeleteChar()
    {
		if (SelectSlotArr == -1)
			return;
	    
        if (!NetData.instance.DeleteCharacter(SlotInfo[SelectSlotArr].c_usn))
            return;

        BtnSelectChars[SelectSlotArr].transform.FindChild("Empty").gameObject.SetActive(true);
        BtnSelectChars[SelectSlotArr].transform.FindChild("Char").gameObject.SetActive(false);
        SlotInfo[SelectSlotArr].c_usn = 0;
		SlotInfo[SelectSlotArr].character_id = 0;

        if (pcModel != null) {
			Destroy (pcModel);
		}

        SelectSlotArr = -1;
		
        int loopCount = SlotInfo.Length;
		int remainCharCnt = 0;
        for (int i = 0; i < loopCount; i++)
        {
            if (SlotInfo[i].character_id <= 0)
                continue;


            SelectSlotArr = (byte)i;

			CreatePcAndEffect(SlotInfo[i], eAnimName.Anim_idle, eAnimName.Anim_none);

			remainCharCnt++;
            break;
        }

		PlayerPrefs.SetInt("SelectHeroSlot", SelectSlotArr);

        for (int i = 0; i < SelectIcon.Length; i++)
        {
            SelectIcon[i].SetActive(i == SelectSlotArr);
        }
        for (int i = 0; i < SelectHeroEffRoot.Length; i++)
        {
            SelectHeroEffRoot[i].SetActive(i == SelectSlotArr);
           
        }

        if (remainCharCnt == 0) {
			// goto create charactor directly.
			TempCoroutine.instance.FrameDelay(0.5f, () =>
		    {
				OnClickJobSlot2(defaultCharIdx, false);
			});
		}
    }

    /// <summary> 캐릭터 생성후 바로 시작 </summary>
    public void OnCharCreateAndGameStart(ulong charIdx)
    {
        if (charIdx <= 0)
            return;


		cutSceneManager.stopAll ();
        NetworkClient.instance.SendPMsgRoleSelectC((long)charIdx);
        PlayerPrefs.SetInt("SelectHeroSlot", SelectSlotArr);
    }

    /// <summary> 게임 스타트 </summary>
    void OnClickGameStart()
    {
		if (CurViewType == ViewType.Create)
			return;

        if (SelectSlotArr < 0 || SlotInfo[SelectSlotArr].c_usn <= 0)
            return;

		if (bTouchBlockWhileCutScenePlaying)
			return;


        NetworkClient.instance.SendPMsgRoleSelectC((long)SlotInfo[SelectSlotArr].c_usn);
        PlayerPrefs.SetInt("SelectHeroSlot", SelectSlotArr);

    }

    /// <summary> 마을 가면 종료 함수 호출됨 </summary>
    public void OnClosePanel()
    {
        base.Close();
    }

#endregion 이벤트 버튼 함수 끗 
    /*
	void FixSelectedSlotIndex(){
		Dictionary<ulong, NetData.CharacterInfo>.ValueCollection characters = NetData.instance.GetCharacters().Values;
		
		////////////////////////////////////////////////////////////////////////
		// 선택된 슬롯 인덱스 SelectSlotArr 와 보유중인 캐릭터의 슬롯이 일치하지 않으면 찾아서 수정해준다. 혹시 모를 버그 방지.
		// ex) 캐릭터는 1,2 슬롯에 있는데 SelectSlotArr 은 0인 경우.
		{
			int charCnt = 0;
			short firstSlot = 0;
			bool bMatchSlot = false;
			for (int i = 0; i < characters.Count; i++) {
				NetData.CharacterInfo charInfo = characters.ElementAt (i);

				if (charInfo.slot_no == 0){ // prevent error.
					// 1,2,3 중 하나의 값을 갖는다. 0이면 안됨.
					charInfo.slot_no = 1;
				}
				
				uint slot = (uint)charInfo.slot_no - 1;
				
				//Debug.Log (" slot :" + slot);
				if (charCnt == 0) {
					firstSlot = (short)slot;
				}
				if (slot == SelectSlotArr) {
					bMatchSlot = true;
				}
				charCnt++;
			}
			
			if (characters.Count > 0 && bMatchSlot == false) {
				SelectSlotArr = firstSlot;
				PlayerPrefs.SetInt ("SelectHeroSlot", SelectSlotArr);
			}
		}
		// SelectSlotArr fix 여기까지.
		///////////////////////////////////////////////////////////////
	}
    */
    /// <summary>
    /// 파티클 사이즈 재조정
    /// </summary>
    /// <param name="go">원하는 객체</param>
    void ResettingParticle(float scale, List<ParticleSystem> parList, float offScale)
    {
        float scaleX = 0;
        //if (parent == null)
        //    scaleX = offScale;
        //else
        {
            float x = scale;
            scaleX = x;// * 0.0027f;
        }

        if (scaleX < 0)//설마 0 이하가 될까...
            scaleX = 1f;

        int listCount = parList.Count;
        for (int i = 0; i < listCount; i++)
        {
            ParticleSystem ps = parList[i];
            if (ps == null)
                continue;

            float startSize = ps.startSize * scaleX;
            float startSpeed = ps.startSpeed * scaleX;
            float startLifeTime = ps.startLifetime * (scaleX + (scaleX * 0.1f));

            ps.startSize = startSize;
            ps.startSpeed = startSpeed;
            ps.startLifetime = startLifeTime;
        }
    }
}

