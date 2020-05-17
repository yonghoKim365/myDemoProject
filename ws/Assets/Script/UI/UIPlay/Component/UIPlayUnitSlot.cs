using UnityEngine;
using System.Collections;

public class UIPlayUnitSlot : MonoBehaviour {

	public static readonly float[] summonPos = new float[12]{5,11,0,7,3,6,10,8,1,4,2,9};
	public static int summonPosIndex = 0;

	const string SP_NUMBER_OFF = "img_sp_number_off";
	const string SP_NUMBER_ON = "img_sp_number_on";

	public int iconDepth = 0;

	public static int useTouchTutorial = WSDefine.NO;



	public Monster mon
	{
		set
		{
			if(unitSlot != null) unitSlot.mon = value;
		}
		get
		{
			if(unitSlot != null)
			{
				return unitSlot.mon;
			}

			return null;

		}
	}

	public static int lastActiveSkillUseSlotIndex = -1;

	public bool isClicked = false;
	public int slotIndex = 0;

	public UILabel tfUseSp;
	public UILabel tfUnitName;

	const int MAX_SHOW_LEFT_SUMMON_NUM = 5;

	public const string NORMAL_GAUGE_COLOR = "pgsb_hp_headup_foreground_playerside";
	public const string SPECIAL_GAUGE_COLOR = "pgsb_hp_headup_specialtime";

	public UISprite spPetIcon;
	public UISprite spLockImage;
	public UISprite spBackground;
	public UISprite spFrameBorder;
	public UISprite spDamageEffect;
	public UISprite spFeverBackground, spFeverGauge, spFeverTouch;
	public UIButton btn;

	public UISprite spFeverPoint;


	public UISprite spHpBar;


	private UnitData unitData;
	private MonsterData monsterData;
	private TranscendData transcendData;
	private int[] transcendLevel = null;

	private Vector3 _v;
	private Quaternion _q = new Quaternion();
	
	public bool isLocked = false;
	


	public GameObject goActiveSkillContainer = null;

	public ParticleSystem touchEffect;


	public UnitSlot unitSlot = null;


	void Awake () 
	{
		UIEventListener.Get(btn.gameObject).onPress = onPress;
	}

	void OnDestroy ()
	{
		clear();
	}

	Player _player;

	public void clear()
	{
		_player = null;
		mon = null;
		if(unitSlot != null) unitSlot.activeSkillData = null;
		unitData = null;
		monsterData = null;
		unitSlot = null;
		transcendData = null;
		transcendLevel = null;
	}

	public void init(UnitData md, TranscendData td, int[] tLevel, Player player)
	{
		_player = player;

		spDamageEffect.cachedGameObject.SetActive(false);

		_state = 0;

		leftNum = 0;

		lastActiveSkillUseSlotIndex = -1;
//		unitSlot.activeSkillState = STATE_ACTIVE_SKILL_COOLTIME;

		mon = null;

		blockThis = false;

		isClicked = false;

		isLocked = false;
		unitData = md;

		transcendData = td;
		transcendLevel = tLevel;

		tfUnitName.text = md.name;

#if UNITY_EDITOR
//		Debug.Log(unitData.resource);
#endif

		monsterData = GameManager.info.monsterData[unitData.resource];
		
//		unitSlot.useSp = unitData.sp;
//		unitSlot.useSp -= (unitSlot.useSp * _player.summonSpPercent(unitData));

		if(player != null)
		{
			tfUseSp.text = Mathf.RoundToInt(unitSlot.useSp / player.maxSp * 100.0f) + "";  //Mathf.CeilToInt(unitSlot.useSp - 0.4f) + "";	
		}
		else
		{
			tfUseSp.text = Mathf.RoundToInt(unitSlot.useSp / _player.maxSp * 100.0f) + "";  //Mathf.CeilToInt(unitSlot.useSp - 0.4f) + "";	
		}


#if UNITY_EDITOR
		Debug.LogError("***** " + slotIndex + "  - icon: " + monsterData.resource);
#endif

		MonsterData.setUnitIcon(monsterData, spPetIcon, iconDepth);

		spPetIcon.MakePixelPerfect();
		spPetIcon.width = 102;

//		if( unitData.cooltime <= 0.1f) unitSlot.maxCoolTime = 0.0f;
//		else unitSlot.maxCoolTime = unitData.cooltime;	

		spBackground.spriteName = RareType.getRareBgSprite(md.rare);
		spFrameBorder.spriteName = RareType.getRareLineSprite(md.rare);

		updateSummonNum();

		goActiveSkillContainer.SetActive(false);


		spHpBar.cachedGameObject.SetActive(false);

		tfUseSp.enabled = true;

		resetCoolTime(true);

		resetActiveSkillCoolTime();
	}



	public void createUnit()
	{
		#if UNITY_EDITOR
//		if(BattleSimulator.nowSimulation == false) Log.log("createUnit : " + slotIndex);
		#endif

		spHpBar.cachedGameObject.SetActive(true);
		spHpBar.fillAmount = 1.0f;
		spHpBar.spriteName = NORMAL_GAUGE_COLOR;
		tfUseSp.enabled = false;

		if(unitSlot.activeSkillData != null)
		{
			goActiveSkillContainer.SetActive(true);
			spFeverBackground.enabled = false;
			spFeverTouch.enabled = false;
		}
		else goActiveSkillContainer.SetActive(false);

		unitSlot.coolTime = unitSlot.maxCoolTime;
		_state = STATE_COOLTIME;
		unitSlot.activeSkillState = STATE_ACTIVE_SKILL_COOLTIME;

		//_player.state = Character.SHOOT;
		
		_v = _player.cTransformPosition;
		_v.x += 100.0f;
		_v.y = 1.0f;
		
		++summonPosIndex;
		if(summonPosIndex >= 12) summonPosIndex = 0;
		
		_v.z = MapManager.summonBottom + MapManager.mapSummonHeight * summonPos[summonPosIndex]/11.0f;
		
		if(_v.x + 10.0f > GameManager.me.characterManager.monsterLeftLine) _v.x = GameManager.me.characterManager.monsterLeftLine - 100.0f;
		if(_v.x <= StageManager.mapStartPosX) _v.x = StageManager.mapStartPosX + 50.0f;

#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			_v.x = 0.0f; _v.y = 1.0f; _v.z = 0.0f;
		}
#endif


		if(slotIndex >= 10)
		{
			switch(slotIndex % 10)
			{
			case 0:
				++GameDataManager.instance.playSubData[UnitSlot.U1];
				break;
			case 1:
				++GameDataManager.instance.playSubData[UnitSlot.U2];
				break;
			case 2:
				++GameDataManager.instance.playSubData[UnitSlot.U3];
				break;
			case 3:
				++GameDataManager.instance.playSubData[UnitSlot.U4];
				break;
			case 4:
				++GameDataManager.instance.playSubData[UnitSlot.U5];
				break;
			}

		}
		else
		{
			switch(slotIndex % 10)
			{
			case 0:
				++GameDataManager.instance.playData[UnitSlot.U1];
				break;
			case 1:
				++GameDataManager.instance.playData[UnitSlot.U2];
				break;
			case 2:
				++GameDataManager.instance.playData[UnitSlot.U3];
				break;
			case 3:
				++GameDataManager.instance.playData[UnitSlot.U4];
				break;
			case 4:
				++GameDataManager.instance.playData[UnitSlot.U5];
				break;
			}

		}

		mon = GameManager.me.mapManager.addMonsterToStage( transcendData, transcendLevel, true, null, unitData.id ,_v);
		mon.monsterUISlotIndex = slotIndex;
		// 소환 방어력 증가
		
		_v.y = 0.5f;

		btn.isEnabled = false;

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		SoundData.playSummonSound( mon.monsterData.resource );

		SoundData.play(UnitSlot.getSummonSoundByRare(unitData.rare));

		string unitSoundId = unitData.baseUnitId;
		if(unitSoundId.StartsWith("UN6"))
		{
			unitSoundId = "UN5"+unitSoundId.Substring(3);
		}

		switch(_player.playerData.characterId)
		{
		case Character.ID.LEO:
			SoundData.playVoice("L"+unitSoundId);
			break;
		case Character.ID.KILEY:
			SoundData.playVoice("K"+unitSoundId);
			break;
		case Character.ID.CHLOE:
			SoundData.playVoice("C"+unitSoundId);
			break;
		case Character.ID.LUKE:
			SoundData.playVoice("LK"+unitSoundId);
			break;
		}

		GameManager.info.effectData[UnitSlot.getSummonEffectByRare(unitData.rare)].getEffect(-1000, _v, null, null, mon.summonEffectSize); //GameManager.resourceManager.getInstantPrefabs("Effect/virtical 14");
	}


	//=== 현재 소환 소환 가능한 몬스터 갯수.
	int leftNum = 0;




	public void updateSummonNum()
	{
		if(GameManager.me.characterManager.alivePlayerUnit.ContainsKey(unitData.id))
		{
			leftNum = (unitData.maxSummonAtOnce - GameManager.me.characterManager.alivePlayerUnit[unitData.id]);

			if(GameManager.me.characterManager.hasSideChangePlayerUnit)
			{
				leftNum += GameManager.me.characterManager.getAliveSideChangeUnitNum(true,unitData.id);
			}
		}
		else
		{
			leftNum = unitData.maxSummonAtOnce;
		}
	}
	

	public void onPress(GameObject go, bool state)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(GameManager.me.battleManager.waitingForPlayerChange) return;

		if(GameManager.me.isAutoPlay == false)
		{
			if(GameManager.me.recordMode != GameManager.RecordMode.record) return;

			if(state)
			{
				GameManager.replayManager.unitButtons[slotIndex % 10] = true;
			}
		}
		else
		{
			if(unitSlot.activeSkillData != null && leftNum == 0 && unitSlot.activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_READY && mon != null)
			{
				if(GameManager.me.recordMode != GameManager.RecordMode.record) return;
				
				if(state)
				{
					GameManager.replayManager.unitButtons[slotIndex % 10] = true;
				}
			}
		}
	}


	void onClickProcess()
	{
		if(GameManager.me.battleManager.waitingForPlayerChange) return;

//		Log.log("ounitslot nClickProcess : " + GameManager.replayManager.nowFrame);
//		Debug.LogError("onClickProcess" + slotIndex + "   " + GameManager.me.stageManager.playTime + "  frame: " + GameManager.replayManager.nowFrame);

		if(GameManager.me.currentScene == Scene.STATE.PLAY_BATTLE)// && _player.globalUnitCooltime <= 0)	
		{
			if(unitSlot.activeSkillState == STATE_ACTIVE_SKILL_READY && mon != null)
			{
				useActiveSkill();

#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation) return;
#endif

				SoundData.play("btl_unitskill");
				touchEffect.Play();
				return;
			}

			if(_state == STATE_READY && maxNumCheck())
			{
				if(_player.sp >= unitSlot.useSp)
				{
					_player.sp -= unitSlot.useSp;	
					createUnit();

					#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation) return;
					#endif

					touchEffect.Play();
				}
			}
		}
	}


	bool _setTouchTurorial = false;

	void useActiveSkill()
	{
		// 액티브 스킬을 사용한 후 버튼을 누를 수 있는지를 판단하여 버튼 활성화를 시켜준다.
		if(_state == STATE_READY)
		{
			if(CutSceneManager.nowOpenCutScene && blockThis)
			{
			}
			else btn.isEnabled = true; 
		}
		else
		{
			btn.isEnabled = false; 
		}

		#if UNITY_EDITOR
//		if(BattleSimulator.nowSimulation == false) Log.log("useActiveSkill " + slotIndex);
		#endif

		playSkillEffectCam(unitSlot.activeSkillData.activeSkillCamId);

		if(unitSlot.activeSkillData.activeSkillDuration <= 0 && mon != null)
		{
			unitSlot.canUseOneShotUnitSkill = true;
			unitSlot.activeSkillData.doActiveSkill(mon);
		}

		unitSlot.canUseOneShotUnitSkill = false;
		resetActiveSkillCoolTime(true);

		if(_setTouchTurorial == false)
		{
			PlayerPrefs.SetInt("TOUCHTUTORIAL", WSDefine.NO);
			_setTouchTurorial = true;
		}
	}


	public void useDebugActiveSkill()
	{
#if UNITY_EDITOR
		playSkillEffectCam(unitSlot.activeSkillData.activeSkillCamId);

		if(mon != null)
		{
			unitSlot.canUseOneShotUnitSkill = true;
			unitSlot.activeSkillData.doActiveSkill(mon);
		}
		
		unitSlot.canUseOneShotUnitSkill = false;
		resetActiveSkillCoolTime(true);
#endif
	}



	public static string nowReadySkillCamId = "";

	public void playSkillEffectCam(string camId = null)
	{
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

		lastActiveSkillUseSlotIndex = mon.monsterUISlotIndex;

		nowReadySkillCamId = camId;
	}



	public bool checkActiveSkillDuration()
	{
		return (unitSlot.activeSkillState == STATE_ACTIVE_SKILL_DURATION && unitSlot.nowSkillCoolTime > 0);
	}




	bool maxNumCheck()
	{
		updateSummonNum();
		return (leftNum > 0);
	}

	public const int STATE_LOCKED = -1;
	public const int STATE_READY = 0;
	public const int STATE_COOLTIME = 1;

	public const int STATE_ACTIVE_SKILL_READY = 2;
	public const int STATE_ACTIVE_SKILL_COOLTIME = 3;
	public const int STATE_ACTIVE_SKILL_DURATION = 4;

	public const string DEFAULT_UNIT_SKILL_EFFECT = "E_SUM_UNITSKILL_START";

	private Xint _state = 0;

//	public int activeSkillState = STATE_ACTIVE_SKILL_COOLTIME;

	public bool blockThis = false;



	public void resetActiveSkillCoolTime(bool useSkill = false)
	{
		if(unitSlot == null || unitSlot.activeSkillData == null) return;

		unitSlot.maxSkillCoolTime = unitSlot.activeSkillData.activeSkillCooltime;
		unitSlot.nowSkillCoolTime = 0.0f;
		spFeverTouch.enabled = false;
		spFeverBackground.enabled = false;
		//spFeverGauge.enabled = false;
		spFeverPoint.enabled = false;
		spFeverGauge.fillAmount = 0.0f;

		if(useSkill && unitSlot.activeSkillData.activeSkillDuration > 0)
		{
			unitSlot.nowSkillCoolTime = unitSlot.activeSkillData.activeSkillDuration;

			unitSlot.activeSkillState = STATE_ACTIVE_SKILL_DURATION;

			if(mon  != null)
			{
				GameManager.info.effectData[DEFAULT_UNIT_SKILL_EFFECT].getParticleEffectByCharacterSize(mon.stat.uniqueId, mon, null, mon.tf);

				if(string.IsNullOrEmpty(unitSlot.activeSkillData.activeSkillEffect) == false)
				{
#if UNITY_EDITOR
					if(UnitSkillCamMaker.instance.useUnitSkillCamMaker) mon.deleteSkillAttachedEffect();
#endif
					ParticleEffect.SKILL_EFFECT_SHOOTER_ID = mon.stat.uniqueId;
					GameManager.me.cutSceneManager.startUnitSkillCamScene(nowReadySkillCamId, mon.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill);
					mon.skillAttachedEffect = GameManager.info.effectData[unitSlot.activeSkillData.activeSkillEffect].getParticleEffect(mon.stat.uniqueId, mon, null, mon.tf);

				}
			}
		}
		else
		{
			unitSlot.activeSkillState = STATE_ACTIVE_SKILL_COOLTIME;

			if(useSkill && mon  != null)
			{
				GameManager.info.effectData[DEFAULT_UNIT_SKILL_EFFECT].getParticleEffectByCharacterSize(mon.stat.uniqueId, mon, null, mon.tf);

				if(string.IsNullOrEmpty(unitSlot.activeSkillData.activeSkillEffect) == false)
				{
					GameManager.info.effectData[unitSlot.activeSkillData.activeSkillEffect].getParticleEffect(mon.stat.uniqueId, mon, null, mon.tf);
				}
			}
			else if(!useSkill & mon != null) mon.deleteSkillAttachedEffect();
		}
	}


	static readonly string[] activeFeverIds = new string[]{"img_fever_effect1","img_fever_effect2","img_fever_effect3","img_fever_effect4","img_fever_effect5","img_fever_effect6","img_fever_effect7","img_fever_effect8"};

	IEnumerator activeSkillMaxEffect()
	{
		int index = 0;
		spFeverBackground.spriteName = activeFeverIds[index];
		spFeverBackground.enabled = true;
		if(useTouchTutorial == WSDefine.YES) spFeverTouch.enabled = true;

		while(true)
		{
			if(unitSlot.activeSkillState != STATE_ACTIVE_SKILL_READY)
			{
				spFeverBackground.enabled = false;
				spFeverTouch.enabled = false;
				break;
			}
			else if(unitSlot.nowSkillCoolTime < unitSlot.maxSkillCoolTime)
			{
				spFeverBackground.enabled = false;
				spFeverTouch.enabled = false;
				break;
			}
			else if(mon == null)
			{
				spFeverBackground.enabled = false;
				spFeverTouch.enabled = false;
				break;
			}

			yield return Util.ws008;
			++index;

			if(useTouchTutorial == WSDefine.YES)
			{
				switch(index)
				{
				case 0:
					spFeverTouch.alpha = 0.9f;
					_v.x = 1; _v.y = 1;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 1:
					spFeverTouch.alpha = 1.0f;
					_v.x = 1.08f; _v.y = 1.08f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 2:
					_v.x = 1.15f; _v.y = 1.08f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 3:
					_v.x = 1.09f; _v.y = 1.08f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 4:
					_v.x = 1; _v.y = 1;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 5:
					_v.x = 1.0972f; _v.y = 1.08f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 6:
					_v.x = 1.1528f; _v.y = 1.12f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 7:
					_v.x = 1.0972f; _v.y = 1.08f;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 8:
					_v.x = 1; _v.y = 1;
					spFeverTouch.cachedTransform.localScale = _v;
					break;
				case 9:
					spFeverTouch.alpha = 0.5f;
					break;
				case 10:
					spFeverTouch.alpha = 0;
					break;
				}
			}



			if(index > 12)
			{
				index = 0;
				spFeverBackground.spriteName = activeFeverIds[index];	
				spFeverBackground.enabled = true;
			}
			else if(index > 7)
			{
				if(index == 8) spFeverBackground.enabled = false;
			}
			else
			{
				spFeverBackground.spriteName = activeFeverIds[index];	
			}
		}
	}

	public void deadCreatedMonster()
	{
		if(mon != null)
		{
			mon = null;
			spFeverBackground.enabled = false;
			spFeverTouch.enabled = false;

			if(unitSlot.activeSkillState == STATE_ACTIVE_SKILL_DURATION)
			{
				resetActiveSkillCoolTime();
				
				btn.isEnabled = _state == STATE_READY;
			}
		}

		if(unitSlot.activeSkillData != null) goActiveSkillContainer.gameObject.SetActive(false);

		spDamageEffect.cachedGameObject.SetActive(false);

		spHpBar.cachedGameObject.SetActive(false);

		tfUseSp.enabled = true;
	}


	public void resetCoolTime(bool isStart = false, bool useRightNow = false)
	{
//		if(isStart == false)
		{
			if(unitSlot.maxCoolTime <= 0.1f || useRightNow)
			{
				_state = STATE_READY;
				unitSlot.coolTime = 0.0f;

				if(CutSceneManager.nowOpenCutScene && blockThis)
				{
				}
				else btn.isEnabled = true; 

				spLockImage.fillAmount = 0.0f;
			}
			else
			{
				_state = STATE_COOLTIME;

				if(isStart)
				{
					unitSlot.coolTime = 2.0f;
				}
				else
				{
					unitSlot.coolTime = unitSlot.maxCoolTime;
				}


				btn.isEnabled = false;
				spLockImage.fillAmount = unitSlot.coolTime/unitSlot.maxCoolTime;
			}
		}
//		else
//		{
//				_state = STATE_READY;
//				unitSlot.coolTime = 2.0f;
//				if(CutSceneManager.nowOpenCutScene && blockThis)
//				{
//				}
//				else btn.isEnabled = true; 
//
//				spLockImage.fillAmount = 0.0f;
//		}
	}	
	


	new public void update(bool fromAiSlot = false)
	{
		if(isLocked || unitData == null) return;

		updateSummonNum();

		if(_state == STATE_COOLTIME)
		{
			if(leftNum > 0)
			{
				unitSlot.coolTime.Set ( unitSlot.coolTime - GameManager.globalDeltaTime);
			}
			
			if(unitSlot.coolTime < 0.0f)
			{
				unitSlot.coolTime = 0.0f;
				_state = STATE_READY;			

				if(CutSceneManager.nowOpenCutScene && blockThis)
				{
				}
				else btn.isEnabled = true; 
			}

			_tempF = unitSlot.coolTime/unitSlot.maxCoolTime;

#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
			{
			}
			else
			{
				if(MathUtil.abs(spLockImage.fillAmount,_tempF) > 0.02f || (_tempF <= 0.0f)) spLockImage.fillAmount = unitSlot.coolTime/unitSlot.maxCoolTime;
			}
#else
			if(MathUtil.abs(spLockImage.fillAmount,_tempF) > 0.02f || (_tempF <= 0.0f)) spLockImage.fillAmount = unitSlot.coolTime/unitSlot.maxCoolTime;
#endif
		}

		spBackground.color = ((leftNum > 0 && _player.sp >= unitSlot.useSp && _state == STATE_READY))?UISlot.NORMAL_COLOR:UISlot.LOCK_COLOR;
		spPetIcon.color = spBackground.color;
		spFrameBorder.color = spBackground.color;


		if(unitSlot.activeSkillData != null)
		{
			// 소환중이면...
			if(leftNum == 0)
			{
				if(unitSlot.activeSkillState == STATE_ACTIVE_SKILL_COOLTIME)
				{
					unitSlot.nowSkillCoolTime += GameManager.globalDeltaTime;

					if(unitSlot.nowSkillCoolTime >= unitSlot.maxSkillCoolTime)
					{
						unitSlot.activeSkillState = STATE_ACTIVE_SKILL_READY;

						spFeverGauge.fillAmount = 1.0f;
						spFeverPoint.enabled = false;
						btn.isEnabled = true;
						StartCoroutine(activeSkillMaxEffect());
					}
					else
					{
						unitSlot.activeSkillState = STATE_ACTIVE_SKILL_COOLTIME;

						spFeverGauge.fillAmount = (unitSlot.nowSkillCoolTime / unitSlot.maxSkillCoolTime);
					}

					updateFeverUI();

				}
				else if(unitSlot.activeSkillState == STATE_ACTIVE_SKILL_DURATION)
				{
					unitSlot.nowSkillCoolTime -= GameManager.globalDeltaTime;

					if(unitSlot.nowSkillCoolTime > 0)
					{
						spFeverGauge.fillAmount = (unitSlot.nowSkillCoolTime / unitSlot.activeSkillData.activeSkillDuration);
					}
					else
					{
						resetActiveSkillCoolTime();
					}
				}
#if UNITY_EDITOR
				// 시뮬레이터일때만 ai가 유닛스킬을 쓴다.
				else if(BattleSimulator.nowSimulation &&  fromAiSlot && unitSlot.activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_READY && mon != null)
				{
					if(isClicked == false)
					{
						if(_player.unitActiveSkillChecker == null || 
						   _player.unitActiveSkillChecker.checkActiveSkill(mon,unitSlot.activeSkillData) == false)
						{
							return;
						}
						GameManager.replayManager.unitButtons[slotIndex % 10] = true;
						//useActiveSkill();
						//isClicked = true;
					}
				}
#endif
			}
			else
			{
				if(unitSlot.activeSkillState == STATE_ACTIVE_SKILL_DURATION)
				{
					resetActiveSkillCoolTime();

					btn.isEnabled = _state == STATE_READY;
				}
			}
		}



		if(isClicked)
		{
			isClicked = false;
			onClickProcess();
		}
	}

	float _tempF;



	void updateFeverUI()
	{
		if(spFeverGauge.fillAmount > 0 && spFeverGauge.fillAmount < 1.0f)
		{
			if(spFeverGauge.fillAmount >= 0.875f)
			{
				_v.x = -40.0f + ( (spFeverGauge.fillAmount - 0.875f)/0.125f) * 40.0f;
				_v.y = 43.0f;
				_v.z = 0;
				spFeverPoint.cachedTransform.localPosition = _v;
				
				_v.x = 0; _v.y = 0; 
				_v.z = 90;
			}
			else if(spFeverGauge.fillAmount >= 0.625f)
			{
				_v.x = -41.5f;
				_v.y = -40.0f + ( (spFeverGauge.fillAmount - 0.625f)/0.25f) * 80.0f;
				_v.z = 0;
				spFeverPoint.cachedTransform.localPosition = _v;
				
				_v.x = 0; _v.y = 0; 
				_v.z = 180;
			}
			else if(spFeverGauge.fillAmount > 0.375f)
			{
				_v.x = 40.0f - ( (spFeverGauge.fillAmount - 0.375f)/0.25f) * 80.0f;
				_v.y = -39.0f;
				_v.z = 0;
				spFeverPoint.cachedTransform.localPosition = _v;
				
				_v.x = 0; _v.y = 0; 
				_v.z = 270;
			}
			else if(spFeverGauge.fillAmount > 0.125f)
			{
				_v.x = 41.5f;
				_v.y = 40.0f - ((spFeverGauge.fillAmount - 0.125f)/0.25f) * 80.0f;
				_v.z = 0;
				spFeverPoint.cachedTransform.localPosition = _v;
				
				_v.x = 0; _v.y = 0; 
				_v.z = 0;
			}
			else
			{
				_v.x = ( (spFeverGauge.fillAmount)/0.125f) * 40.0f;
				_v.y = 43.0f;
				_v.z = 0;
				spFeverPoint.cachedTransform.localPosition = _v;
				
				_v.x = 0; _v.y = 0; 
				_v.z = 90;
			}
			
			_q.eulerAngles = _v;
			spFeverPoint.cachedTransform.localRotation = _q;
			spFeverPoint.enabled = true;
		}
		else
		{
			spFeverPoint.enabled = false;
		}
	}



	public void clearAllCooltime()
	{
		spDamageEffect.cachedGameObject.SetActive(false);
		
		_state = STATE_READY;
		
		lastActiveSkillUseSlotIndex = -1;

		blockThis = false;
		
		isClicked = false;
		
		updateSummonNum();
		
		goActiveSkillContainer.SetActive(false);

		spHpBar.cachedGameObject.SetActive(false);
		
		tfUseSp.enabled = true;
		
		resetCoolTime(true, true);
		
		resetActiveSkillCoolTime();
	}




}
