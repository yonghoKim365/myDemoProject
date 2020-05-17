using UnityEngine;
using System.Collections;

public class UIPlaySkillSlot : MonoBehaviour {

	public bool isClicked = false;
	public int slotIndex = 0;

	public UILabel lbLeftItemNum;

	public UISprite spLockImage;
	public UISprite spSkillIcon;

	public UISprite spBackground, spFrameBorder;

	public UIButton btn;

	public ParticleSystem chargingEffect, chargingFullNoticeEffect, chargingFullEffect;

	// Use this for initialization
	void Start () 
	{
		UIEventListener.Get(btn.gameObject).onPress = onPress;//.onClick = onClick; //onPress;//Click;
	}

	// 현재 스킬의 강화도에 따른 최대 차징 시간.
	public Xfloat chargingTimeLimit = 1.0f;

//	public HeroSkillData skillSlot.skillData;
	private Vector3 _v;
	
	public UILabel tfItemName;
	
	public bool isLocked = false;

	bool _isPress = false;

	public bool isWaitingPress = false; // 스킬을 쓸 수 없는 상태인데 버튼을 누르고 있으면 이 녀석이 true가 된다. 그래서 클릭한것과 마찬가지의 효과를 주게된다.

	public static int lastClickedSlotIndex = -1;

	bool _isButtonEnabled = false;

	bool _lastClickState = false;




	public PVPPlayerSkillSlot skillSlot;

	int _playerTagIndex = -1;

	private Player _player;

	public void init(PVPPlayerSkillSlot aiSkillSlot, Player player)
	{	
		_player = player;

		_playerTagIndex = player.playerTagIndex;

		chargingEffect.Stop();
		chargingFullNoticeEffect.Stop();
		chargingFullEffect.Stop();
		_isFullCharging = false;

		skillSlot = aiSkillSlot;

		_lastClickState = false;

		lastClickedSlotIndex = -1;
		blockThis = false;
		isClicked = false;
		_isPress = false;
		isWaitingPress = false;

		isLocked = false;
		//Debug.Log(skillId);

		skillSlot.skillData.exeData.init(AttackData.AttackerType.Hero,AttackData.AttackType.Skill, skillSlot.skillData, skillSlot.skillInfo.transcendData, skillSlot.skillInfo.transcendLevel);

		chargingTimeLimit = skillSlot.skillData.getChargingTime(skillSlot.skillInfo.reinforceLevel).AsFloat();

		lbLeftItemNum.text = Mathf.RoundToInt(skillSlot.useMp / player.maxMp * 100.0f).ToString();//Mathf.CeilToInt(useMp).ToString();

		tfItemName.text = skillSlot.skillData.name;

		spBackground.spriteName = RareType.getRareBgSprite(skillSlot.skillInfo.rare);
		spFrameBorder.spriteName = RareType.getRareLineSprite(skillSlot.skillInfo.rare);

		Icon.setSkillIcon(skillSlot.skillInfo.getSkillIcon(), spSkillIcon);
		spSkillIcon.MakePixelPerfect();

		resetCoolTime();

		btn.isEnabled = true;
	}


	public void onPress(GameObject go, bool isPress)
	{
		if(GameManager.me.battleManager.waitingForPlayerChange) return;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
#endif
		if(GameManager.me.isAutoPlay || GameManager.me.stageManager.isIntro) return;
		//recordMode == RecordMode.continueGame

		if(GameManager.me.recordMode != GameManager.RecordMode.record) return;

		//if(GameManager.me.recordMode == GameManager.PlayMode.replay) return;

		GameManager.replayManager.skillButtons[slotIndex] = true;
	}


	public void onIntroPress()
	{
		if(GameManager.me.isAutoPlay) return;
		if(GameManager.me.recordMode != GameManager.RecordMode.record)
		{
			return;
		}

#if UNITY_EDITOR
		Debug.LogError("==  onIntroPress : " + slotIndex);
#endif

		GameManager.replayManager.skillButtons[slotIndex] = true;
	}




	void LateUpdate()
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(_isPress == false ) return;

		if( GameManager.me.isAutoPlay ) return;

		if( GameManager.me.playMode != GameManager.PlayMode.normal ) return;

		if(GameManager.me.recordMode != GameManager.RecordMode.record ) return;

		if(GameManager.me.stageManager.isIntro) return;



#if UNITY_EDITOR

		if(Input.GetMouseButton(0) == false)
		{
			GameManager.replayManager.skillButtons[slotIndex] = true;
		}

#else
		if(Input.touchCount == 0)
		{
			GameManager.replayManager.skillButtons[slotIndex] = true;
		}
		else
		{

		}
#endif
	}


	





//=======================================================================================	


	public void cancelChargingAndClearAllCooltime()
	{
		isWaitingPress = false;
		chargingEffect.Stop();
		chargingFullEffect.Stop();
		chargingFullNoticeEffect.Stop();
		_isFullCharging = false;
		_isPress = false;
	}

	
	
	private Vector3 _v2;

	public void resetCoolTime(bool clearCooltime = false)
	{
//		if(GameManager.info.setupData.canUseSkillAndPetSlotAtStart == false)
		{
			if(skillSlot.maxCoolTime <= 0.1f || clearCooltime)
			{
				skillSlot.state = SkillSlot.STATE_READY;
				skillSlot.coolTime.Set( 0.0f );
				if(CutSceneManager.nowOpenCutScene && blockThis)
				{
				}
				else _isButtonEnabled = true;

				spLockImage.fillAmount = 0.0f;
			}
			else
			{
				skillSlot.state = SkillSlot.STATE_COOLTIME;
				skillSlot.coolTime = skillSlot.maxCoolTime;
				_isButtonEnabled = false;
				spLockImage.fillAmount = skillSlot.coolTime/skillSlot.maxCoolTime;
			}
		}
//		else
//		{
//				skillSlot.state = SkillSlot.STATE_READY;
//				skillSlot.coolTime = 0.0f;
//			if(CutSceneManager.nowOpenCutScene && blockThis)
//			{
//			}
//
//			else _isButtonEnabled = true;
//
//				spLockImage.fillAmount = 0.0f;
//		}
	}	


	public bool releasePressByTurnOffAutoPlay()
	{
		if(_player != null && _player.nowChargingSkill != null && skillSlot.state == SkillSlot.STATE_PRESS)
		{
			GameManager.replayManager.skillButtons[slotIndex] = true;
			return true;
		}

		return false;
	}


	public  bool blockThis = false;
	private bool _isFullCharging = false;

	new public void update(bool updateByAI = false)
	{
		if(isLocked) return; // 스킬이 장착되지 않은 슬롯은 아예 방지.

		Player player = _player;

		if(skillSlot.state == SkillSlot.STATE_COOLTIME)
		{
			skillSlot.coolTime.Set( skillSlot.coolTime - GameManager.globalDeltaTime );
			
			if(skillSlot.coolTime < 0.0f)
			{
				skillSlot.coolTime = 0.0f;
				skillSlot.state = SkillSlot.STATE_READY;			
				if(CutSceneManager.nowOpenCutScene && blockThis)
				{
				}
//				else btn.isEnabled = true; 
				else _isButtonEnabled = true;
			}


			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
			{
			}
			else
			{
				spLockImage.fillAmount = skillSlot.coolTime/skillSlot.maxCoolTime;
			}
			#else
			spLockImage.fillAmount = skillSlot.coolTime/skillSlot.maxCoolTime;
			#endif
		}
		else if(skillSlot.state == SkillSlot.STATE_PRESS)
		{
			player.onCharging();
			if(player.chargingGauge.isFull && _isFullCharging == false)
			{
				_isFullCharging = true;
				//chargingEffect.Stop();
				chargingFullNoticeEffect.Play();
				chargingFullEffect.Play();

#if UNITY_EDITOR
//				Debug.LogError(slotIndex + " FULL : " + GameManager.replayManager.nowFrame);
#endif

			}

			if(skillSlot.checkSkillTarget(true) && skillSlot.skillData.targeting != TargetingData.NONE)
			{
				if(player.targetingDecal.didStartEffect == false)
				{
					player.targetingDecal.startDecalEffect();
				}
				player.targetingDecal.visible = true;
			}
			else
			{
				player.targetingDecal.visible = false;
			}
		}

		// 스킬 동작 중에는 새로운 공격을 할 수 없다.
		bool canUseThisSlotSkill = (player.mp >= skillSlot.useMp && skillSlot.state == SkillSlot.STATE_READY && player.nowPlayingSkillAni == false && player.nowChargingSkill == null);

//		Log.log("SLOTINDEX: " + slotIndex);
//		Log.log("canUseThisSlotSkill : " + canUseThisSlotSkill);
//		Log.log("player.mp : " + player.mp + "  skillSlot.useMp: " + skillSlot.useMp +  "  skillSlot.state : " + skillSlot.state + "   player.nowPlayingSkillAni : " +  player.nowPlayingSkillAni + "  player.nowChargingSkill : " + player.nowChargingSkill);

		spBackground.color = (canUseThisSlotSkill)?UISlot.NORMAL_COLOR:UISlot.LOCK_COLOR;
		spSkillIcon.color = spBackground.color;

		spFrameBorder.color = spBackground.color;

//		Log.log("isClicked : " + isClicked + "   isWaitingPress : " + isWaitingPress + "  _lastClickState : " + _lastClickState + "   updateByAI : " + updateByAI + "  GameManager.me.isAutoPlay : " + GameManager.me.isAutoPlay    );

		if(updateByAI == false)
		{
			if(GameManager.me.isAutoPlay || BattleSimulator.nowSimulation) return;
		}

		if(isClicked || isWaitingPress)
		{
			if(isClicked)
			{
				_lastClickState = !_lastClickState;
//				Debug.LogError(slotIndex + " _lastClickState : " + _lastClickState);
			}

			if(_isPress == false) // 터치를 시작...
			{
				if(updateByAI)
				{
					if(GameManager.me.isAutoPlay)
					{
						if(skillSlot.checkSkillTarget(true) == false)
						{
							canUseThisSlotSkill = false;
						}
					}
#if UNITY_EDITOR
					else if(BattleSimulator.nowSimulation)
					{
						if(skillSlot.checkSkillTarget(true) == false)
						{
							canUseThisSlotSkill = false;
						}
					}
#endif
				}

//				Log.log("canUseThisSlotSkill : " + canUseThisSlotSkill);

				if(canUseThisSlotSkill)
				{
					skillSlot.state = SkillSlot.STATE_PRESS;
					_isPress = true;
					_isFullCharging = false;
					chargingEffect.Play();
					chargingFullEffect.Stop();
					chargingFullNoticeEffect.Stop();

					
					switch(skillSlot.skillData.targeting)
					{
					case TargetingData.NONE:
						//GameManager.me.effectManager.tfTargetingCircle.gameObject.SetActive(false);
						//GameManager.me.effectManager.spTargetingArrow.gameObject.SetActive(false);
						break;
					case TargetingData.FIXED_1:
						_player.targetingDecal.init(TargetingDecal.DecalType.Circle, (float)skillSlot.skillData.targetAttr[1] * 0.005f, true, skillSlot.checkSkillTarget(true));
						break;
					case TargetingData.AUTOMATIC_2:
						_player.targetingDecal.init(TargetingDecal.DecalType.Circle, 1.0f, true, skillSlot.checkSkillTarget(true));
						break;
					case TargetingData.FORWARD_LINEAR_3:
						_player.targetingDecal.init(TargetingDecal.DecalType.Arrow, 1.0f, true, skillSlot.checkSkillTarget(true));
						break;				
					}	

					#if UNITY_EDITOR
//					Log.log("player.startCharging");
					#endif

					player.skillMoveIsNormal = true;
					player.moveType = Player.MoveType.SKILL;
					player.skillModeProgressTime = 0.0f;

					player.startCharging(skillSlot.skillData,chargingTimeLimit,skillSlot.skillInfo);

					isWaitingPress = false;
				}
				else
				{
					isWaitingPress = true;
				}
			}
			else if(_isPress) // 터치를 종료...
			{
				isWaitingPress = false;

				chargingEffect.Stop();
				chargingFullEffect.Stop();
				chargingFullNoticeEffect.Stop();
				_isFullCharging = false;


				_isPress = false;
				
				resetCoolTime();

				if(updateByAI == false)
				{
					if(skillSlot.checkSkillTarget(false))
					{
						player.mp -= skillSlot.useMp;				
						player.nowBulletPatternId = skillSlot.skillData.resource;
						
						skillSlot.skillData.exeData.playSkill(player, skillSlot.skillInfo.reinforceLevel + skillSlot.skillData.baseLevel, player.applyReinforceLevel);

						playSkillVoice();
					}
				}
				else
				{
					if(skillSlot.checkSkillTarget(false))
					{
						if(skillSlot.skillData.exeData.type == AttackData.ATTACH_BULLET_12)
						{
							player.hpWhenAttachSkillStart = player.hp;
							player.moveType = Player.MoveType.ATTACH_SKILL;
							player.skillMoveIsNormal = true;
						}
						else
						{
							player.moveType = Player.MoveType.NORMAL;
							player.skillMoveIsNormal = true;
						}
						
						player.mp -= skillSlot.useMp;				
						player.nowBulletPatternId = skillSlot.skillData.resource;
						
						skillSlot.skillData.exeData.playSkill(player, player.nowChargingSkillInfo.reinforceLevel + skillSlot.skillData.baseLevel, player.applyReinforceLevel);

						playSkillVoice();
					}
					else
					{
						player.moveType = Player.MoveType.NORMAL;
						player.skillMoveIsNormal = true;
					}

				}

				#if UNITY_EDITOR
//				Log.log("player.finishCharging");
				#endif

//				Debug.LogError("=== player.finishCharging() ===");
				player.finishCharging();

				
				lastClickedSlotIndex = slotIndex;
			}


			if(_lastClickState == false)
			{
				isWaitingPress = false;
			}

			isClicked = false;

		}
	}


	void playSkillVoice()
	{

		if(_playerTagIndex == 0)
		{
			switch(slotIndex)
			{
			case 0:
				++GameDataManager.instance.playData[SkillSlot.S1];
				break;
			case 1:
				++GameDataManager.instance.playData[SkillSlot.S2];
				break;
			case 2:
				++GameDataManager.instance.playData[SkillSlot.S3];
				break;
			}
		}
		else if(_playerTagIndex == 1)
		{
			switch(slotIndex)
			{
			case 0:
				++GameDataManager.instance.playSubData[SkillSlot.S1];
				break;
			case 1:
				++GameDataManager.instance.playSubData[SkillSlot.S2];
				break;
			case 2:
				++GameDataManager.instance.playSubData[SkillSlot.S3];
				break;
			}
		}

		switch(_player.playerData.characterId)
		{
		case Character.ID.LEO:
			SoundData.playVoice("L"+skillSlot.skillData.baseId);
			break;
		case Character.ID.KILEY:
			SoundData.playVoice("K"+skillSlot.skillData.baseId);
			break;
		case Character.ID.CHLOE:
			SoundData.playVoice("C"+skillSlot.skillData.baseId);
			break;
		case Character.ID.LUKE:
			SoundData.playVoice("LK"+skillSlot.skillData.baseId);
			break;
		}
	}


	public void clear()
	{
		_player = null;
		skillSlot = null;
	}


	public void hideEffect()
	{
		if(chargingEffect != null) chargingEffect.Stop();
		if(chargingFullNoticeEffect != null) chargingFullNoticeEffect.Stop();
		if(chargingFullEffect != null) chargingFullEffect.Stop();
	}



	public static void hideAllSlotEffect()
	{
		if(GameManager.me.uiManager.uiPlay.uiSkillSlot != null)
		{
			for(int i = GameManager.me.uiManager.uiPlay.uiSkillSlot.Length - 1; i >=0; --i)
			{
				GameManager.me.uiManager.uiPlay.uiSkillSlot[i].hideEffect();
			}
		}
	}


}
