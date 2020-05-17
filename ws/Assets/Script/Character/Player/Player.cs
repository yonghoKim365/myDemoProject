using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class Player : Monster
{
	public const int SUMMON_SLOT_NUM = 5;
	public const int SKILL_SLOT_NUM = 3;

	//public float globalSkillCooltime = 0.0f;



//	[HideInInspector]
//	public float deltaPlayerPosY = 0.0f;
	
	void Start()
	{
		container = transform.parent.gameObject;
	}

// ===================================== 차징 관련 ===========================================//

	private int _chargingLevel = 0;

	// 현재 차징 중인 스킬.
	public HeroSkillData nowChargingSkill = null;
//	public HeroSkillData nowChargingSkill
//	{
//		set
//		{
//			_nowChargingSkill = value;
//			Debug.LogError("==== nowChargingSkill : " + value);
//		}
//		get
//		{
//			return _nowChargingSkill;
//		}
//	}



	// 풀차징 이후 지난 시간.
	public Xfloat timeAfterFullCharging = 0.0f;

	// 현재 차징 타임.
	public Xfloat chargingTime = 0.0f;

	// 현재 스킬의 최대 차징시간.
	public Xfloat chargingTimeLimit = 0.0f;

	public GameIDData nowChargingSkillInfo;

	private Xint _isFullCharging = 0;
	public void startCharging(HeroSkillData sd, Xfloat nowChargingLimitTime, GameIDData ei)
	{
		chargingLevel = 0;
		chargingTime.Set( 0.0f );
		timeAfterFullCharging.Set( 0.0f );
		nowChargingSkill = sd;
		chargingTimeLimit.Set( nowChargingLimitTime );
		nowChargingSkillInfo = ei;
		_isFullCharging.Set(0);

		int effectIndex = 0;

		switch(ei.rare)
		{
		case RareType.SS:
			effectIndex = 4;
			break;
		case RareType.S:
			effectIndex = 3;
			break;
		case RareType.A:
			effectIndex = 2;
			break;
		case RareType.B:
			effectIndex = 1;
			break;
		}

		for(int i = 0; i < 6; ++i)
		{
			if(i == effectIndex)
			{
				_chargingEffectsTf[i].gameObject.SetActive(true);
				_chargingEffect = _chargingEffects[i];
			}
			else
			{
				_chargingEffectsTf[i].gameObject.SetActive(false);
			}
		}

		_chargingEffect.Stop();
		_chargingEffect.Play();	
		
		GameManager.soundManager.playChargingEffect( GameManager.info.soundData[ "btl_skcharging1" ]);
		
		if(chargingGauge != null)
		{
			chargingGauge.visible = true;
			chargingGauge.setData(0);
		}
	}

	public void finishCharging()
	{
		chargingTime.Set( 0.0f );
		chargingLevel = 0;			
		targetingDecal.hide();
		if(chargingGauge != null) chargingGauge.visible = false;
	}


	public bool onCharging()
	{
		chargingTime.Set( chargingTime + GameManager.globalDeltaTime);
		
		if(_isFullCharging < WSDefine.YES)
		{
			IFloat checkTime = chargingTime;

			if(chargingTime >= chargingTimeLimit)
			{
				_isFullCharging.Set( WSDefine.YES );
				timeAfterFullCharging.Set(  timeAfterFullCharging.Get() + (chargingTime - chargingTimeLimit) );
				chargingGauge.setFull();
				checkTime = chargingTimeLimit.Get();

				_chargingEffectsTf[5].gameObject.SetActive(true);
				_chargingEffects[5].Play();

				SoundData.play("btl_shcharged1");

			}
			else
			{
				chargingGauge.setData((chargingTimeLimit <= 0)?1f: (float)(chargingTime/chargingTimeLimit) );
			}

			if(checkTime >= nowChargingSkill.maxChargingTime / 4.0f * 3.0f )
			{
				chargingLevel = RareType.A;
			}
			else if(checkTime >= nowChargingSkill.maxChargingTime / 4.0f * 2.0f)
			{
				chargingLevel = RareType.B;
			}
			else if(checkTime >= nowChargingSkill.maxChargingTime / 4.0f)
			{
				chargingLevel = RareType.C;
			}
			return false;
		}
		else
		{
			timeAfterFullCharging.Set( timeAfterFullCharging.Get() + GameManager.globalDeltaTime.Get() );
			return true;
		}
	}



	public int applyReinforceLevel
	{
		get
		{
//			* 효과 수치 지정 방식, [최소,최대] 형식으로 입력								.
//				- N 강화레벨의 수치 = 최소값 + (최대값 - 최소값) / 19 * (N - 1)			.					
//				- 차징중 발사시, 차징시간에 따라 N값을 줄여서 적용							.	
//				N = (현차징시간 - 최소차징시간) / (최대차징시간 - 최소차징시간) * 19 + 1		.						
//			* 수치가 하나로 지정돼 있으면 강화레벨과 상관없이 고정								.
			if(nowChargingSkill.isChargingTimeSingle)
			{
				return 1;
			}
			else
			{
				if(chargingTime < nowChargingSkill.minChargingTime)
				{
					return - (  Mathf.FloorToInt((chargingTime / nowChargingSkill.minChargingTime) * 100.0f) );
				}
				else
				{
					int returnValue = 0;

					if(chargingTime > chargingTimeLimit)
					{
						returnValue = Mathf.FloorToInt(   ( chargingTimeLimit - nowChargingSkill.minChargingTime ) / ( nowChargingSkill.maxChargingTime - nowChargingSkill.minChargingTime) * 19 + 1);
					}
					else
					{
						returnValue = Mathf.FloorToInt(   ( chargingTime - nowChargingSkill.minChargingTime ) / ( nowChargingSkill.maxChargingTime - nowChargingSkill.minChargingTime) * 19 + 1);
					}

					if(returnValue > 20) returnValue = 20;
					return returnValue;
				}
			}
		}
	}






	public TargetingDecal targetingDecal;
	public ChargingGauge chargingGauge;
	
	ParticleSystem _chargingEffect = null;
	ParticleSystem[] _chargingEffects = new ParticleSystem[6];
	Transform[] _chargingEffectsTf = new Transform[6];



	// 차징 이펙트를 자연스럽게 전환시키기 위해.
	IEnumerator changeChargingEffectDelay(int index)
	{
//		yield return wait008;

		yield return null;

		_chargingEffectsTf[index].gameObject.SetActive(false);
		//GameManager.me.effectManager.particleCharging[index].transform.parent.gameObject.SetActive(false);
	}


	public int chargingLevel
	{
		set
		{
			_chargingLevel = value;

			if(_chargingLevel == HeroSkillData.CHARING_NORMAL)
			{
				nowChargingSkill = null;

				for(int i = 0; i < 6; ++i)
				{
					_chargingEffectsTf[i].gameObject.SetActive(false);
				}

				_chargingEffect = null;

				GameManager.soundManager.stopLoopChargingEffect();
				GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Charging,"", AudioFader.State.FadeOut, 0.3f);
			}
			/*
//				StartCoroutine(changeChargingEffectDelay(0));				
//				_chargingEffectsTf[1].gameObject.SetActive(true);
//				_chargingEffectsTf[2].gameObject.SetActive(false);
//				_chargingEffectsTf[3].gameObject.SetActive(false);				
//				_chargingEffect = _chargingEffects[1];
//				_chargingEffect.Stop();
//				_chargingEffect.Play();



//				GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Charging,"btl_skcharging2", AudioFader.State.CrossFade, 0.3f);

//				targetingDecal.setColorByChargingLevel(value);
			}
			else if(_chargingLevel == HeroSkillData.CHARING_LV2)				
			{
//				_chargingEffectsTf[0].gameObject.SetActive(false);
//				StartCoroutine(changeChargingEffectDelay(1));
//				_chargingEffectsTf[2].gameObject.SetActive(true);
//				_chargingEffectsTf[3].gameObject.SetActive(false);				
//				_chargingEffect = _chargingEffects[2];
//				_chargingEffect.Stop();
//				_chargingEffect.Play();

//				SoundData.play("btl_shcharged2");
//				GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Charging,"btl_skcharging3", AudioFader.State.CrossFade, 0.3f);

//				targetingDecal.setColorByChargingLevel(value);
			}
			else if(_chargingLevel == HeroSkillData.CHARING_LV3)
			{
//				_chargingEffectsTf[0].gameObject.SetActive(false);
//				_chargingEffectsTf[1].gameObject.SetActive(false);
//				StartCoroutine(changeChargingEffectDelay(2));
//				_chargingEffectsTf[3].gameObject.SetActive(true);				
//				_chargingEffect = _chargingEffects[3];
//				_chargingEffect.Stop();
//				_chargingEffect.Play();

//				SoundData.play("btl_shcharged3");

//				targetingDecal.setColorByChargingLevel(value);
			}
			*/
		}
		get
		{
			return _chargingLevel;
		}
	}

	// ===================================== 차징 관련 끝. ===========================================//


	// 변수 초기화.
	void initVals()
	{

		nowChargingSkill = null;
		
		timeAfterFullCharging.Set( 0.0f );
		chargingTime = 0.0f;

		nowSkillMoveTargetZoneBestPoint = 0.0f;
		skillTargetingPositionMaxPoint = 0.0f;
		leftFullChargingTime = 0.0f;
		_skillMoveLeftTime = 0.0f;

		isDefaultAttacking.Set( false );
		moveType = MoveType.NORMAL;
		skillMoveIsNormal = true;
		skillModeProgressTime = 0.0f;
		setMovingDirection(MoveState.Stop);

		currentTargetMovingPosition.x = 0.0f;
		currentTargetMovingPosition.y = 0.0f;
		currentTargetMovingPosition.z = 0.0f;

		_nowNormalAiCheckDelay = 0.0f;

		_nowTagDelay = 0.0f;
		_nowSummonDelay = 0.0f;
		_updateSkillDelay = 0.0f;
		_nowSkillTargetCheckDelay = 0.0f;

		_energyBarShowTime = -99.0f;
	}

	private CharacterManager cm;
	private Monster _tempChar;
	private Monster _tempChar2;


	public void init(GamePlayerData gpd, bool playerSide = true, bool isIngame = true, int tagIndex = -1)
	{
		//globalSkillCooltime = 0.0f;
		initVals();
		resetDefaultVals();

		npcData = null;

		if(gpd == null) playerData = null;
		else playerData = gpd.clone();

		_state = Monster.NORMAL;
		isMonster = false;
		isDeleteObject = false;
		
		cm = GameManager.me.characterManager;

		isPlayer = true;
		isPlayerSide = playerSide;
		isPlayersPlayer = isPlayerSide;

		stat.monsterType = TYPE.HERO;
		stat.playerType = (isPlayerSide)?MonsterStat.PlayerType.Player:MonsterStat.PlayerType.PVPPlayer;

		fowardDirectionValue = (isPlayerSide?1000.0f:-1000.0f);
		isHero = true;		

		baseInit();

		isPlayer = true;
		isPlayersPlayer = isPlayerSide;
		isPlayerSide = playerSide;
		fowardDirectionValue = (isPlayerSide?1000.0f:-1000.0f);
		isHero = true;		

		playerTagIndex = tagIndex;

		//setAI("AI_DP_0,AI_DP_1,AI_DP_2,AI_DP_3,AI_DP_4,AI_DP_5,AI_ST_0,AI_WT_1,AI_WT_2,AI_BO_0,AI_BO_1,AI_BO_2,AI_BX_0,AI_BX_1,AI_BX_2,AI_SZ_0,AI_AM_0,AI_AM_1,AI_AM_2,AI_AM_3,AI_UR_0,AI_UR_1,AI_UP_0,AI_UP_1,AI_UP_2,AI_UP_3,AI_SR_0,AI_SR_1,AI_SP_0,AI_SP_1,AI_SP_2,AI_SP_3,AI_SP_4,AI_SP_5,AI_SP_6,AI_SP_7,AI_TP_0,AI_TP_1,AI_TP_2,AI_T2_0,AI_TN_0");

		// 실제 플레이어의 데이터를 입력.
		// pvp 모드는 어찌할 것인가?
		initHeroData();
		initParts();

		action = GameManager.me.characterManager.getCharacterAction(category);
		action.init(this);

//		Debug.Log("== PLAYER INIT!!!! : " + isPlayerSide + "   gpd : " + gpd.heroData.level);

		// 임시코드. 펫 크기 감안...
		damageRange = 80.0f;
		
		_q = tf.rotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.y = (isPlayerSide)?90.0f:270.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		tf.rotation = _q;	
		
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		tf.localPosition = _v;

		_v = cTransformPosition;
		//_v.x = 0.0f;
		_v.y = 1.0f;
		//_v.z = 0.0f;
		setPositionCtransform(_v);	
		
		if(isIngame)
		{
			if(miniMapPointer == null)
			{
				miniMapPointer = GameManager.me.characterManager.getMinimapPointer(true, cTransform);
				miniMapPointer.isEnabled = true;
			}

			if(hpBar == null)
			{
				hpBar = GameManager.me.characterManager.getHpBar(cTransform);

				hpBar.isEnabled = true;	
				hpBar.visible = false;

				hpBar.yPos = 100.0f;

				if(isPlayerSide) hpBar.spEnergy.SetSprite(CharacterHpBar.PLAYER_HP_NAME);
				else hpBar.spEnergy.SetSprite(CharacterHpBar.ENEMY_HP_NAME);
			}

			if(isPlayerSide)
			{
				for(int i = 0; i < 6; ++i)
				{
					_chargingEffects[i] = GameManager.me.effectManager.particleCharging[i];
					_chargingEffectsTf[i] = _chargingEffects[i].transform.parent;
				}

				targetingDecal = GameManager.me.characterManager.targetingDecal[0];
				chargingGauge = GameManager.me.characterManager.chargingGauge[0];

			}
			else
			{
				for(int i = 0; i < 6; ++i)
				{
					_chargingEffects[i] = GameManager.me.effectManager.particleCharging[i+6];
					_chargingEffectsTf[i] = _chargingEffects[i].transform.parent;
				}

				targetingDecal = GameManager.me.characterManager.targetingDecal[1];
				chargingGauge = GameManager.me.characterManager.chargingGauge[1];
//				if(hpBar != null) hpBar.isEnabled = false;
				// 플레이어 적으로 나오는 경우는 pvp 밖에 없다.
				//initHpBar();
			}

			initChargingGauge();

			initPVP();

			if(miniMapPointer != null)
			{
				if(isPlayerSide)
				{
					miniMapPointer.init(CharacterMinimapPointer.PLAYER,cTransform, 8);
				}
				else
				{
					miniMapPointer.init(CharacterMinimapPointer.E_HERO,cTransform, 7);
				}

				miniMapPointer.isEnabled = true;
			}

			chargingLevel = 0;

			saveCharacterOriginalValue();
		}
		
		initShadowAndEffectSize();

		if(faceAniEye != null) playFaceAni("EYE_LOOP");
		if(faceAniMouth != null) playFaceAni("MOUTH_CLOSE");

		ani.cullingType = AnimationCullingType.BasedOnUserBounds;
		ani.localBounds = new Bounds(Vector3.zero,new Vector3(1000,1000,1000));

		foreach(SkinnedMeshRenderer smr in smrs)
		{
			smr.sharedMesh.bounds = new Bounds(Vector3.zero,new Vector3(1000,1000,1000));
			smr.updateWhenOffscreen = true;
		}


		if(GameManager.me.stageManager.isIntro)
		{
			if(GameManager.me.stageManager.nowRound != null && GameManager.me.stageManager.nowRound.id == "INTRO")
			{
				maxHp = 10000;
				hp = 10000;
			}
		}
	}


	public void changeGamePlayerData(GamePlayerData gpd, bool isGame = false)
	{
		if(gpd == null) playerData = null;
		else playerData = gpd.clone();

		if(isGame)
		{
			initHeroData();
			initParts();
		}
		else
		{
			playerData.setAllDataToPlayer(this);
		}

		if(hpBar != null)
		{
			hpBar.visible = false;
			_energyBarShowTime = -100.0f;
		}
	}



	// 히어로 정보 세팅.
	private void initHeroData()
	{
		pvpPlayerAttackSkillDataLen = 0;

		//monsterData = GameManager.info.monsterData[playerData.heroData.resource];
		playerData.setAllDataToPlayer(this);

		if(hpBar != null)
		{
			hpBar.visible = false;
			_energyBarShowTime = -100.0f;
		}

		attackData = playerData.partsWeapon.parts.attackType;
		attackData.init(AttackData.AttackerType.Hero, AttackData.AttackType.Attack, null, playerData.partsWeapon.itemInfo.transcendData, playerData.partsWeapon.itemInfo.transcendLevel);

		setAniData(Monster.ATK_IDS[attackData.type]);
		setDefaultHitRange(_tempAniData, false);//(attackData.type == 1 || attackData.type == 2));


		if(isPlayerSide)
		{
			int slotIndex = 0;
			List<int> openSlots = new List<int>();

			if(playerData.units == null) playerData.units = new string[SUMMON_SLOT_NUM];
			if(playerData.skills == null) playerData.skills = new string[SKILL_SLOT_NUM];

			//============== unit setting =============== //

			UIPlayUnitSlot[] uiUnitSlots = (playerTagIndex == 0)?GameManager.me.uiManager.uiPlay.UIUnitSlot1:GameManager.me.uiManager.uiPlay.UIUnitSlot2;
			UIPlaySkillSlot[] uiSkillSlots = (playerTagIndex == 0)?GameManager.me.uiManager.uiPlay.uiSkillSlot1:GameManager.me.uiManager.uiPlay.uiSkillSlot2;

			UISprite[] uiUnitEmptySlot = (playerTagIndex == 0)?GameManager.me.uiManager.uiPlay.uiUnitEmptySlot1:GameManager.me.uiManager.uiPlay.uiUnitEmptySlot2;
			UISprite[] uiSkillEmptySlot = (playerTagIndex == 0)?GameManager.me.uiManager.uiPlay.uiSkillEmptySlot1:GameManager.me.uiManager.uiPlay.uiSkillEmptySlot2;


			for(i = 0; i < SUMMON_SLOT_NUM; ++i)
			{
				if(GameDataManager.instance.playerUnitSlots.ContainsKey( playerData.id ) == false)
				{
					playerData.units[i] = null;
				}
				else if(GameDataManager.instance.playerUnitSlots[playerData.id][i].isOpen)
				{
					uiUnitSlots[i].slotIndex = i + (playerTagIndex!=0?10:0);
					uiUnitSlots[i].gameObject.SetActive(true);
					uiUnitEmptySlot[i].gameObject.SetActive(false);
					openSlots.Add(i);
					++slotIndex;
					playerData.units[i] = GameDataManager.instance.playerUnitSlots[playerData.id][i].unitInfo.serverId;
				}
				else
				{
					uiUnitSlots[i].isLocked = true;
					uiUnitSlots[i].gameObject.SetActive(false);
					uiUnitEmptySlot[i].gameObject.SetActive(true);
					playerData.units[i] = null;
				}
			}

			playerData.units = playerData.units;

			unitSlots = new UnitSlot[slotIndex];
			_unitSlotsNum = slotIndex;

			for(i = 0; i < slotIndex; ++i)
			{
				if(unitSlots[i] == null)  unitSlots[i] = new UnitSlot();

				if(GameManager.me.player == this && playerTagIndex == 0)
				{
					unitSlots[i].setData(this, GameDataManager.instance.playerUnitSlots[playerData.id][openSlots[i]].unitInfo, GameManager.me.uiManager.uiPlay.UIUnitSlot1[openSlots[i]] );
				}
				else if(playerTagIndex == 1)
				{
					unitSlots[i].setData(this, GameDataManager.instance.playerUnitSlots[playerData.id][openSlots[i]].unitInfo, GameManager.me.uiManager.uiPlay.UIUnitSlot2[openSlots[i]] );
				}
				else
				{
					unitSlots[i].setData(this, GameDataManager.instance.playerUnitSlots[playerData.id][openSlots[i]].unitInfo );
				}

			}

			//============== skill setting =============== //

			slotIndex = 0;
			openSlots.Clear();
			
			for(i = 0; i < SKILL_SLOT_NUM; ++i)
			{
				if(GameDataManager.instance.playerSkillSlots.ContainsKey(playerData.id) == false)
				{
					playerData.skills[i] = null;
				}
				else if(GameDataManager.instance.playerSkillSlots[playerData.id][i].isOpen)
				{
					
					uiSkillSlots[i].slotIndex = i;
					uiSkillSlots[i].gameObject.SetActive(true);
					uiSkillEmptySlot[i].gameObject.SetActive(false);
					++slotIndex;
					openSlots.Add(i);
					playerData.skills[i] = GameDataManager.instance.playerSkillSlots[playerData.id][i].infoData.serverId;
					
					if(GameDataManager.instance.playerSkillSlots[playerData.id][i].infoData.skillData.skillType == Skill.Type.ATTACK)
					{
						pvpPlayerAttackSkillSlotIndex[pvpPlayerAttackSkillDataLen] = i;
						++pvpPlayerAttackSkillDataLen;
					}
				}
				else
				{
					uiSkillSlots[i].gameObject.SetActive(false);
					uiSkillEmptySlot[i].gameObject.SetActive(true);
					uiSkillSlots[i].isLocked = true;
					playerData.skills[i] = null;
				}
			}


			skillSlots = new PVPPlayerSkillSlot[slotIndex];
			_skillSlotsNum = slotIndex;

			for(i = 0; i < slotIndex; ++i)
			{
				skillSlots[i] = new PVPPlayerSkillSlot();

				if(GameManager.me.player == this && playerTagIndex == 0)
				{
					skillSlots[i].setData(this, GameDataManager.instance.playerSkillSlots[playerData.id][openSlots[i]].infoData, GameManager.me.uiManager.uiPlay.uiSkillSlot1[openSlots[i]] );
				}
				else if(playerTagIndex == 1)
				{
					skillSlots[i].setData(this, GameDataManager.instance.playerSkillSlots[playerData.id][openSlots[i]].infoData, GameManager.me.uiManager.uiPlay.uiSkillSlot2[openSlots[i]] );
				}
				else
				{
					skillSlots[i].setData(this, GameDataManager.instance.playerSkillSlots[playerData.id][openSlots[i]].infoData );
				}


				pvpPlayerAttackSkillSlotIndex[i] = i;
			}

		}
		else
		{
			if(playerData.units == null) _unitSlotsNum = 0;
			else
			{
				_unitSlotsNum = 0;

				for( i = 0; i < playerData.units.Length; ++i)
				{
					if(string.IsNullOrEmpty(  playerData.units[i] ) == false)
					{
						++_unitSlotsNum;
					}
				}
			}


			unitSlots = new UnitSlot[_unitSlotsNum];
			int ui = 0;

			if(_unitSlotsNum > 0)
			{
				for( i = 0; i < playerData.units.Length; ++i)
				{
					if(string.IsNullOrEmpty(  playerData.units[i] ) == false)
					{
						GameIDData pvpUnitInfoData = new GameIDData();
						pvpUnitInfoData.parse(playerData.units[i], GameIDData.Type.Unit);
						
						unitSlots[ui] = new UnitSlot();
						unitSlots[ui].setData(this, pvpUnitInfoData);
						++ui;
					}
				}
			}

			if( playerData.skills == null) _skillSlotsNum = 0;
			else
			{
				_skillSlotsNum = 0;
				
				for( i = 0; i < playerData.skills.Length; ++i)
				{
					if(string.IsNullOrEmpty(  playerData.skills[i] ) == false)
					{
						++_skillSlotsNum;
					}
				}
			}

			skillSlots = new PVPPlayerSkillSlot[_skillSlotsNum];

			pvpPlayerAttackSkillDataLen = 0;
			int si = 0;

			if(_skillSlotsNum > 0)
			{
				for( i = 0; i < playerData.skills.Length; ++i)
				{
					if(string.IsNullOrEmpty(  playerData.skills[i] ) == false)
					{
						GameIDData skillInfo = new GameIDData();
						skillInfo.parse(playerData.skills[i], GameIDData.Type.Skill);
						
						skillSlots[si] = new PVPPlayerSkillSlot();
						skillSlots[si].setData(this, skillInfo);
						
						if(skillInfo.skillData.skillType == Skill.Type.ATTACK)
						{
							pvpPlayerAttackSkillSlotIndex[pvpPlayerAttackSkillDataLen] = si;
							++pvpPlayerAttackSkillDataLen;
						}
						++si;
					}
				}
			}
		}


		changeAnimationSpeed(Monster.BWALK, stat.speed/200.0f);
		if(pet != null) pet.changeAnimationSpeed(Monster.BWALK, stat.speed/200.0f);

	}


	// 파츠 세팅.
	private void initParts()
	{
		foreach(KeyValuePair<string, GameObject> kv in heads)
		{
			heads[kv.Key].SetActive( playerData.partsHead.parts.resource.Equals(kv.Key));
		}

		foreach(KeyValuePair<string, GameObject> kv in bodies)
		{
			bodies[kv.Key].SetActive ( playerData.partsBody.parts.resource.Equals(kv.Key));
		}
		
		foreach(KeyValuePair<string, GameObject> kv in weapons)
		{
			weapons[kv.Key].SetActive ( playerData.partsWeapon.parts.resource.Equals(kv.Key) || (playerData.partsWeapon.parts.resource + "_arrow").Equals(kv.Key));
		}

		string partsResourceId = "";

		//if(playerData.id.Contains(Character.CHLOE)) return;

		foreach(SkinnedMeshRenderer smr in smrs)
		{
			if(smr.name.Equals(playerData.partsHead.parts.resource))
			{
				CharacterUtil.setPartsTexture(smr,playerData.partsHead.parts);
			}
			else if(smr.name.Equals(playerData.partsBody.parts.resource))
			{
				CharacterUtil.setPartsTexture(smr,playerData.partsBody.parts);
			}
			else if(smr.name.Contains(playerData.partsWeapon.parts.resource))
			{
				if(smr.name.Equals(playerData.partsWeapon.parts.resource) || smr.name.Equals(playerData.partsWeapon.parts.resource + "_arrow"))
				{
					CharacterUtil.setPartsTexture(smr,playerData.partsWeapon.parts);
				}
			}
			else if(playerData.faceTexture != null)
			{
				if(smr.name.Contains(playerData.faceTexture))
				{
					smr.gameObject.SetActive(true);
					CharacterUtil.setTexture(smr,playerData.faceTexture);
				}
				else if(smr.name.Contains("_face"))
				{
					smr.gameObject.SetActive(false);
				}
			}
		}

		if(pet != null) pet.setParts(playerData.partsVehicle.parts, playerData.partsVehicle.rare);


	}










	public override Xfloat hp
	{
		get
		{
			return _hp;
		}
		set
		{
//			Log.log(value);

			if(value > maxHp) _hp.Set(maxHp);
			else if(value <= 0)
			{
				#if UNITY_EDITOR

				if(DebugManager.instance.isAutoCombat && BattleSimulator.nowSimulation == false)
				{
					_hp.Set(maxHp);
				}
				else
				#endif
				{
					_hp.Set(0);

				}
			}
			else
			{
				_hp.Set(value);
			}
			
			hpPer.Set( _hp/maxHp );


			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1) return;
			#endif

			if(isPlayer && npcData == null)
			{
				if(isPlayerSide)
				{
#if UNITY_EDITOR
//					Log.logError("set hp : " + _hp);
#endif
					if(GameManager.me.player != null && GameManager.me.player == this)
					{
						GameManager.me.uiManager.uiPlay.updateHP(hpPer,_hp,maxHp);
					}
					else
					{
						if(playerTagIndex > -1) GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateHP(hpPer,_hp,maxHp);
					}
				}
				else
				{
					if(GameManager.me.pvpPlayer != null && GameManager.me.pvpPlayer == this)
					{
						GameManager.me.uiManager.uiPlay.playerPVPEnemyHpBar.updateHP(hpPer,_hp,maxHp);
					}
					else
					{
						if(playerTagIndex > -1) GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateHP(hpPer,_hp,maxHp);
					}
				}
			}

			if(CutSceneManager.nowOpenCutScene == false && UIPopupSkillPreview.isOpen == false && isVisible)
			{
				setHpBar();
			}

		}
	}



	public void updateAllGauge()
	{
		if(isPlayer && npcData == null)
		{
			if(isPlayerSide)
			{
				if(GameManager.me.player != null && GameManager.me.player == this)
				{
					GameManager.me.uiManager.uiPlay.updateHP(hpPer,_hp,maxHp, false);
				}
				else
				{
					if(playerTagIndex > -1) GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateHP(hpPer,_hp,maxHp);
				}
			}
			else
			{
				if(GameManager.me.pvpPlayer != null && GameManager.me.pvpPlayer == this)
				{
					GameManager.me.uiManager.uiPlay.playerPVPEnemyHpBar.updateHP(hpPer,_hp,maxHp, false);
				}
				else
				{
					if(playerTagIndex > -1) GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateHP(hpPer,_hp,maxHp);
				}

			}

			//=========

			if(isPlayersPlayer)
			{
				if(GameManager.me.player != null && GameManager.me.player == this)
				{
					GameManager.me.uiManager.uiPlay.updateMP(_mp,maxMp);
					GameManager.me.uiManager.uiPlay.updateSP(_sp,maxSp);
				}
			}
			
			if(playerTagIndex > -1)
			{
				if(isPlayerSide)
				{
					GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateMP(_mp,maxMp);
					GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateSP(_sp,maxSp);
				}
				else
				{
					GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateMP(_mp,maxMp);
					GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateSP(_sp,maxSp);	
				}
			}
		}
	}







	public void recoveryWaitPlayer()
	{
		_recoveryDelaySp.Set(_recoveryDelaySp + GameManager.globalDeltaTime);
		if(_recoveryDelaySp >= GameManager.info.setupData.waitPlayerRecoveryDelaySp)
		{
			_recoveryDelaySp.Set( 0 ) ; //_recoveryDelaySp - GameManager.info.setupData.waitPlayerRecoveryDelaySp);

			sp += stat.spRecovery;
		}


		_recoveryDelayMp.Set(_recoveryDelayMp + GameManager.globalDeltaTime);
		if(_recoveryDelayMp >= GameManager.info.setupData.waitPlayerRecoveryDelayMp)
		{
			_recoveryDelayMp.Set( 0 ) ;//_recoveryDelayMp - GameManager.info.setupData.waitPlayerRecoveryDelayMp);
			
			mp += stat.mpRecovery;
		}


		_hpRecoveryDelay.Set(_hpRecoveryDelay + GameManager.globalDeltaTime );

		if( _hpRecoveryDelay >= 1 && hp < maxHp)
		{
			_hpRecoveryDelay.Set( 0 );//_hpRecoveryDelay - 1.0f );

			hp += maxHp * GameManager.info.setupData.waitPlayerRecoveryHpPer;
		}

	}	








	public override Xfloat mp
	{
		get
		{
			return _mp;
		}
		set
		{
			_mp.Set( value );
			if(_mp > maxMp) _mp.Set( maxMp );
			else if(_mp < 0) _mp.Set( 0 );
			
			if(isPlayer && npcData == null)
			{
				if(isPlayersPlayer)
				{
					if(GameManager.me.player != null && GameManager.me.player == this)
					{
						GameManager.me.uiManager.uiPlay.updateMP(_mp,maxMp);	
					}
				}
				
				if(playerTagIndex > -1)
				{
					if(isPlayerSide)
					{
						GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateMP(_mp,maxMp);
					}
					else
					{
						GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateMP(_mp,maxMp);
					}
				}
			}
		}
	}
	

	public override Xfloat sp
	{
		get
		{
			return _sp;
		}
		set
		{
			_sp.Set( value );
			if(_sp > maxSp) _sp.Set( maxSp );
			else if(_sp < 0) _sp = 0;

			if(isPlayer && npcData == null)
			{
				if(isPlayersPlayer)
				{
					if(GameManager.me.player != null && GameManager.me.player == this)
					{
						GameManager.me.uiManager.uiPlay.updateSP(_sp,maxSp);	
					}
				}


				if(playerTagIndex > -1)
				{
					if(isPlayerSide)
					{
						GameManager.me.uiManager.uiPlay.playerTagSlot[playerTagIndex].updateSP(_sp,maxSp);	
					}
					else
					{
						GameManager.me.uiManager.uiPlay.pvpTagSlot[playerTagIndex].updateSP(_sp,maxSp);	
					}
				}

			}
		}
	}














	
	sealed public override void update()
	{
		if(isReady == false) return;

		if(heroMonsterData != null && isPlayer == false)
		{
			base.update();
			
			hitObject.setPosition(cTransformPosition);
			
			if(pet != null)
			{
				pet.update();
			}
			else
			{
				lineLeft = hitObject.x;
				lineRight = hitObject.right;
			}
			
			return;
		}

		updateAnimationMethod();

		if(_isEnabled == false)
		{
			if(npcData != null)
			{
				baseUpdate();
			}
			return;
		}
//		Log.log("==================================================");
//		Log.log(isPlayerSide);

		baseUpdate();

		//globalSkillCooltime -= GameManager.globalDeltaTime;


		action.delay.Set( action.delay - GameManager.globalDeltaTime);

		if(characterEffect.check() == false) return;


		if(UIPopupSkillPreview.isOpen == false)
		{
			// 기본적으로 이동을 담당.
			// 일반/스킬/어태치는 다른 곳에서 판단한다.
			// 얘는 ai에서 택해준 이동 로직에 의해 그저 움직일 뿐.
			movePlayer(); 
		}

		hitObject.setPosition(cTransformPosition);

		if(pet != null)
		{
			pet.update();
		}
		else
		{
			lineLeft = hitObject.x;
			lineRight = hitObject.right;
		}

		//updateChargingEffect();

		if(UIPopupSkillPreview.isOpen) return;

		if(isPlayerSide)
		{
			if(GameManager.me.player == null || GameManager.me.player != this || GameManager.me.battleManager.waitingForPlayerChange) return;

			if(unitSlots != null) for(int i = 0 ; i < _unitSlotsNum; ++i) unitSlots[i].update();
			if(skillSlots != null) for(int i = 0 ; i < _skillSlotsNum; ++i) skillSlots[i].update();

			if(GameManager.me.isAutoPlay || BattleSimulator.nowSimulation)
			{
				// 자동전투일때 ai 업데이트.
				updateAI();
			}
		}
		else
		{
			if(GameManager.me.pvpPlayer == null || GameManager.me.pvpPlayer != this || GameManager.me.battleManager.waitingForPVPChange) return;

			if(unitSlots != null) for(int i = 0 ; i < _unitSlotsNum; ++i) unitSlots[i].update();
			if(skillSlots != null) for(int i = 0 ; i < _skillSlotsNum; ++i) skillSlots[i].update();

			// 자동전투일때 ai 업데이트.
			updateAI();
		}
	}


	public void updateChargingEffect()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(_chargingEffectsTf[i] != null)
			{
				_chargingEffectsTf[i].position = cTransform.position;
			}
		}
	}


	
	sealed public override void damageDead(bool useSound = true)
	{
		if(_isEnabled == false) return;

#if UNITY_EDITOR

		if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			return;
		}
#endif

		hp -= _hp;
		dead(useSound);
	}			



	sealed public override void dead(bool useSound = true)
	{
#if UNITY_EDITOR
		Debug.Log("PLAYER DEAD!!!");

		if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			return;
		}

#endif

		_isEnabled.Set( false );
		isFreeze.Set( true );

		removeAttacker();
		removeTarget();

		clearPlayerEffect();

		if(pet != null)
		{
			pet.removeHeroFromPet();
		}

		state = Monster.DEAD;

		SoundManager.instance.stopLoopEffect(2);

		if(useSound) SoundData.playDieSound(  resourceId , playerData.characterId);

		if((GameManager.me.stageManager.nowRound != null && GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PROTECT) || CutSceneManager.nowOpenCutScene)
		{
			GameManager.setTimeScale = 1.0f;
		}
		else
		{

			if(GameManager.me.stageManager.nowRound != null && GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PVP)
			{
				if(GameManager.me.playMode == GameManager.PlayMode.replay)
				{

				}
				else
				{
					if(GameManager.me.battleManager.hasAlivePlayer(isPlayerSide) == false)
					{
						GameManager.setTimeScale = 0.4f;
					}
				}
			}
			else
			{
				GameManager.setTimeScale = 0.4f;
			}
		}
	}




	public void setChange(bool isVisible)
	{
		_isEnabled.Set( isVisible );
		isFreeze.Set( !isVisible );

		miniMapPointer.visible = isVisible;

		if(skillSlots != null)
		{
			for(int i = 0 ; i < _skillSlotsNum; ++i)
			{
				skillSlots[i].cancelChargingAndClearAllCooltime();
			}
		}

		if(unitSlots != null)
		{
			for(int i = 0; i < _unitSlotsNum; ++i)
			{
				if(unitSlots[i] != null)
				{
					unitSlots[i].clearAllCooltime();

				}
			}
		}


		moveState = MoveState.Stop;

		onCompleteAttackAni(true);

		chargingLevel = HeroSkillData.CHARING_NORMAL;

		state = Monster.NORMAL;

		onCompleteSkillAni(null);

		removeAttacker();
		removeTarget();
		
		clearPlayerEffect(false);

		clearAnimationMethod(true);

		changeShader(false,true);

		setColor(_normalColor);

		if(hpBar != null) hpBar.visible = false;
		_energyBarShowTime = -100.0f;
		
		_currentDamageTime = -1.0f;
		isDamageFrame = false;

		if(isVisible)
		{
			setVisible(true);

			initChargingGauge();
		}
		else
		{
			GameManager.info.effectData["E_CHANGE_PLAYER"].getEffect(-1000, cTransformPosition, null, null);//, shadowSize);
		}
	}


	public void initChargingGauge()
	{
		if(chargingGauge != null)
		{
			chargingGauge.init(cTransform, 0, hitObject.height + 30.0f);
			chargingGauge.isEnabled = true;	
			chargingGauge.visible = false;

		}
	}



	public override void destroy(bool isOriginal)
	{


		_dangerPointChecker = null;
		_battleStateChecker = null;
		_readyModeMove = null;
		_battleModeOnChecker = null;
		_battleModeOffChecker = null;
		_safeZoneChecker = null;
		
		_attachSkillMoveChecker = null;
		
		unitRuneSelectRarePoint = null;
		skillRuneSelectRarePoint = null;
		
		unitRuneSelectCalcValue = null;
		skillRuneSelectCalcValue = null;
		
		unitRunePointChecker = null;
		skillRunePointChecker = null;
		
		targetingPointHeroChecker = null;
		targetingPointEnemyUnitChecker = null;
		targetingPointMyUnitChecker = null;
		
		targetingPositionOption = null;

		changePlayerChecker = null;

		_nValue = null;
		
		unitActiveSkillChecker = null;

		_nowSelectSkillSlot = null;

		_chargingEffect = null;

		cm = null;

		_tempChar = null;

		_tempChar2 = null;

		for(int i = 0; i < 4; ++i)
		{
			if(_chargingEffects != null) _chargingEffects[i] = null;
			if(_chargingEffectsTf != null && _chargingEffectsTf[i] != null)
			{
				_v = _chargingEffectsTf[i].position;
				_v.x = -9999.0f;
				_chargingEffectsTf[i].position = _v;
				_chargingEffectsTf[i] = null;
			}
		}

		if(targetingDecal != null)
		{
			targetingDecal.isEnabled = false;
		}

		initChargingGauge();

		_chargingEffectsTf = null;
		targetingDecal = null;
		chargingGauge = null;

		_scoreSlot = null;
			
		_checkSlot = null;
				
		if(dangerPoints != null) dangerPoints.Clear();
		dangerPoints = null;
				
		_nowSelectSkillSlot = null;
				
		nowChargingSkill = null;

		nowChargingSkillInfo = null;
				
		pvpPlayerAttackSkillSlotIndex = null;

		base.destroy(isOriginal);
	}


	public void clearPlayerEffect(bool resetUniqueId = true)
	{
		if(hpBar != null) hpBar.visible = false;

		if(chargingLevel > 0) chargingLevel = 0;

		if(targetingDecal != null)
		{
			targetingDecal.isEnabled = false;
		}

		if(chargingGauge != null)
		{
			chargingGauge.transform.position = new Vector3(-99999,0,0);
			chargingGauge.isEnabled = false;	
			chargingGauge.visible = false;
		}

		if(resetUniqueId)
		{
			stat.uniqueId = -1;
			prevUniqueId = -1;
		}

		changeShader(false);

		setColor(_normalColor);

		clearEffect();
		clearAnimationMethod();
		
		while(chainLighting.Count > 0)
		{
			chainLighting[0].removeCharacter(this);
			chainLighting.RemoveAt(0);
		}

	}
	
	
	
	sealed public override bool damage(TYPE shooterType, Monster shooter, int checkUnituqeId, bool isSkillType, int damageIndex, Transform hitter, float attackersAtkPhysic, float attackersAtkMagic, float damagePer = 1.0f, float minimumDamagePer = 1.0f, bool useHitEffect = true, string effectId = "E_HIT_STRIKE", string soundId = null, float tempDiscountDamageValue = 1.0f)
	{
//		Log.logError("hitter: " + hitter.name + "   " + GameManager.me.stageManager.playTime);
//		Log.logError("damage: " + GameManager.me.stageManager.playTime);

		_damage = MathUtil.getDamage(attackersAtkPhysic, stat.defPhysic, attackersAtkMagic, stat.defMagic, damagePer, minimumDamagePer, 1.0f/(float)damageIndex, tempDiscountDamageValue);		

		if(_isEnabled == false) return false;
		
		playDamageSoundAndEffect((shooter != null)?shooter.stat.uniqueId:-1000, useHitEffect, soundId, effectId);		
		
		//if(_damage < 1 || _invincibleTime > 0.0f ) return false;
		if(_damage < 1 ) return false;

		if(GameManager.me.isPlaying == false) return false;

		if(invincible.Get() == false)
		{
			// 회피 확률.
			if(canAvoid()) return false;
			
			checkDamageReflection(_damage,shooter,checkUnituqeId);

			if(checkDamageTanker(_damage))
			{
				return false;
			}

			hp -= _damage;
			hpEffect(-_damage);

			if(_hp <= 0)
			{
				//Debug.LogError("플레이어 사망!! :  " + GameManager.me.stageManager.playTime);

				dead();
				return true;			
			}
//			else
//			{
//				if(_damage > 0 && _damageMotionDuration > 100.0f) damageMotionEffect(tf);
//			}

			if(isSkillType == false) addDamageCharacter(shooter, checkUnituqeId);
//			_invincibleTime.Set( GameManager.info.setupData.playerHitCooltime );

			setDamageFrame();
			return true;
		}
		
		return false;
	}
	
	
	sealed public override bool damage(TYPE shooterType, Monster shooter, int checkUnituqeId, bool isSkillType, IFloat damageValue, bool useHitEffect = true, string effectId = "E_HIT_STRIKE", string soundId = null)
	{
		if(_isEnabled == false) return false;

		playDamageSoundAndEffect((shooter != null)?shooter.stat.uniqueId:-1000,useHitEffect, soundId, effectId);				
		
		//if(damageValue < 1 || _invincibleTime > 0.0f ) return false;
		if(damageValue < 1 ) return false;

		if(GameManager.me.isPlaying == false) return false;

		if(invincible.Get() == false)
		{
			// 회피 확률.
			if(canAvoid()) return false; // shooterType != TYPE.UNIT			

			checkDamageReflection(damageValue,shooter);

			if(checkDamageTanker(damageValue) )
			{
				return false;
			}

			hp -= damageValue;
			hpEffect(-damageValue);


			if(_hp <= 0)
			{
				dead();
				return true;			
			}
//			else
//			{
//				if(damageValue > 0 && _damageMotionDuration > 100.0f) damageMotionEffect(tf);
//			}

			if(isSkillType == false) addDamageCharacter(shooter, checkUnituqeId);

//			_invincibleTime.Set( GameManager.info.setupData.playerHitCooltime );

			setDamageFrame();
			return true;

		}
		setDamageFrame();
		return false;
	}		
	
	
//	sealed protected override void damageMotionEffect(Transform cha)
//	{
//	}	



	sealed public override void resetPosition(bool isHide = true)
	{
//		Log.log("resetPosition player : " + name, this);


		_v = cTransformPosition;

		if(isHide) _v.x = -99999.0f;
		else _v.x = 0.0f;


		_v.y = 0;
		_v.z = 0;
		setPositionCtransform(_v);
	}	
	


	sealed public override string state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			
			if(_state == Monster.DEAD)
			{
				_tempChar = null;
				_tempChar2 = null;

				stat.uniqueId = -1;
				prevUniqueId = -1;
				clearEffect();
				invincible = true;
			}

			//Debug.Log("player state : " + _state + "   " + GameManager.me.stageManager.playTime);
//			Log.log("player state : " + _state);
			playAni(_state);
		}
	}	
	
	

	public override void setParent(Transform parent)
	{
		//container.gameObject.transform.parent = parent;
		//if(pet != null) pet.container.transform.parent = parent;
		if(pet != null) pet.cTransform.parent = parent;
		else cTransform.parent = parent;
	}


	//	public float summonSpPercent;
	public Xfloat summonSpPercent(UnitData ud)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowSummonSpPercent;
		value += playerData.partsBody.parts.nowSummonSpPercent;
		value += playerData.partsWeapon.parts.nowSummonSpPercent;
		value += playerData.partsVehicle.parts.nowSummonSpPercent;

		return value;
	}


	//	public float unitHpUp;
	public Xfloat unitHpUp(UnitData ud)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowUnitHpUp;
		value += playerData.partsBody.parts.nowUnitHpUp;
		value += playerData.partsWeapon.parts.nowUnitHpUp;
		value += playerData.partsVehicle.parts.nowUnitHpUp;
		
		return value;
	}

	//	public float unitDefUp;
	public Xfloat unitDefUp(UnitData ud)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowUnitDefUp;
		value += playerData.partsBody.parts.nowUnitDefUp;
		value += playerData.partsWeapon.parts.nowUnitDefUp;
		value += playerData.partsVehicle.parts.nowUnitDefUp;
		
		return value;
	}

	//	public float skillSpDiscount;
	public Xfloat skillSpDiscount(HeroSkillData sd)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowSkillSpDiscount;
		value += playerData.partsBody.parts.nowSkillSpDiscount;
		value += playerData.partsWeapon.parts.nowSkillSpDiscount;
		value += playerData.partsVehicle.parts.nowSkillSpDiscount;
		
		return value;
	}

	//	public float skillAtkUp; 
	public Xfloat skillAtkUp(BaseSkillData sd)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowSkillAtkUp;
		value += playerData.partsBody.parts.nowSkillAtkUp;
		value += playerData.partsWeapon.parts.nowSkillAtkUp;
		value += playerData.partsVehicle.parts.nowSkillAtkUp;
		
		return value ;
	}

	//	public float skillUp;
	public Xfloat skillUp(BaseSkillData sd)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowSkillUp;
		value += playerData.partsBody.parts.nowSkillUp;
		value += playerData.partsWeapon.parts.nowSkillUp;
		value += playerData.partsVehicle.parts.nowSkillUp;
		
		return value ;

	}

	//	public float skillTimeUp;
	public Xfloat skillTimeUp(BaseSkillData sd)
	{
		float value = 0.0f;
		value += playerData.partsHead.parts.nowSkillTimeUp;
		value += playerData.partsBody.parts.nowSkillTimeUp;
		value += playerData.partsWeapon.parts.nowSkillTimeUp;
		value += playerData.partsVehicle.parts.nowSkillTimeUp;

		return value ;
	}
}


