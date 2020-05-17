using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class MonsterHeroMonster : CharacterAction
{
	public MonsterHeroMonster ()
	{
	}

	private Transform _effTransform;

	TargetingDecal decal = null;


	private string _nextCheckAI = null;
	private bool _hasNextCheckAI = false;

	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		_delay = 0.0f;

		mon.cTransform.gameObject.name = "HERO_MONSTER!!!";
		mon.heroMonsterData.attackType.init(AttackData.AttackerType.Hero,AttackData.AttackType.Attack);

		decal = GameManager.me.characterManager.getMonsterHeroTargetingDecal();

		_nextCheckAI = null;
		_hasNextCheckAI = false;
	}


	private string _prevState;

	sealed public override void startAction ()
	{
		base.startAction ();
		_delay = 0.0f;
		_delay = 1000.0f;
	}
	


	bool checkAI()
	{
		AISlot slot;
		int len = mon.aiSlots.Length;
		for(int i = 0; i < len; ++i)
		{
			if(_hasNextCheckAI)
			{
				if(mon.aiSlots[i].ai.id != _nextCheckAI)
				{
					continue;
				}
			}

			slot = mon.aiSlots[i];

			if(slot.ai.check(mon, slot))
			{
				#if UNITY_EDITOR					
				Debug.Log("=== "+slot.ai.id + " : EXCUTE!!!" + Time.timeSinceLevelLoad + "     : " + slot.ai.moveType);
				#endif					
				_nowSelectedAISlot = slot;
				
				if(_nowSelectedAISlot.ai.moveType == MonsterHeroAI.MoveType.NONE)
				{
					mon.setPlayAniRightNow(Monster.NORMAL);
					_state = STATE_ACTION;

					if(_nowSelectedAISlot.ai.actionType == MonsterHeroAI.ActionType.SKILL2)
					{
						_delay = 0.0f;
						initTargetingDecal();
					}
				}
				else if(_nowSelectedAISlot.ai.moveType == MonsterHeroAI.MoveType.TARGETZONE)
				{
					_delay = 0.0f;
					_state = STATE_MOVE;
				}
				else
				{
					_state = STATE_MOVE;
				}
				return true;
			}
			else
			{
				_hasNextCheckAI = false;
			}
		}

		return false;
	}




	bool checkAISkill2Mode()
	{
		foreach(AISlot slot in mon.aiSlots)
		{
			if(slot.ai.check(mon, slot, true))
			{
				#if UNITY_EDITOR					
				Debug.Log("=== SKILL2 MODE: "+slot.ai.id + " : EXCUTE!!!" + Time.timeSinceLevelLoad + "     : " + slot.ai.moveType);
				#endif					

				if(slot.ai.moveType == MonsterHeroAI.MoveType.NONE)
				{
					if(slot.ai.actionType == MonsterHeroAI.ActionType.SUMMON)
					{
						createSummon(slot.ai);
					}
				}
				return true;
			}
		}
		
		return false;
	}



	private IFloat _delay = 10.0f;

	private AISlot _nowSelectedAISlot = null;

	private HeroSkillData _nowSelectedSkillData;
	
	sealed public override void doMotion()
	{

		#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker) return;
		#endif

		switch(_state)
		{
		case STATE_PREPARE:
			
			_nowSelectedAISlot = null;
			
			// ai 를 돌린다. 액션이 나왔으면 그냥 액션을 하면 된다 애니 동작이 끝나면 ai 재검사를 하면 됨.
			// 그런데 이동 동작이 걸렸다. 그럼 이동중엔 해당 이동조건만 계속 검사한다. 다른 조건은 검사하지 않는다.
			// 그러다가 이동조건이 깨지면 다시 ai 조건을 처음부터 돌려본다.
			checkAI();
			
			break;
			
		case STATE_MOVE:
			
			if(_nowSelectedAISlot.ai.moveType == MonsterHeroAI.MoveType.TARGETZONE)
			{
				_delay += GameManager.globalDeltaTime;
				
				if(_delay >= 2.0f)
				{
					if(checkAI()) return;
				}
				
				
				_v = mon.cTransformPosition;
				_tempX = GameManager.me.characterManager.targetZonePlayerLine + _nowSelectedAISlot.ai.moveDistanceFromTargetZone;
				
				if(_tempX > _v.x) _tempF = MathUtil.abs(_tempX, _v.x);
				else _tempF = MathUtil.abs(_v.x, _tempX);

#if UNITY_EDITOR				
				//Debug.Log(mon.lineRight + "  endLine: " +  StageManager.mapEndPosX);
#endif				
				if(_tempF > 3.0f && mon.lineRight < StageManager.mapEndPosX)
				{
					_v = mon.cTransformPosition;
					_v.x -= 20.0f;
					mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, Util.getLookRotationQuaternion(_v - mon.cTransformPosition), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
					_v2 = (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime;
					_v2.y = 0;

					if(_tempX > _v.x)
					{
						mon.setPosition(mon.cTransformPosition - _v2);
						
						_v = mon.cTransformPosition;
						if(_v.x > _tempX)
						{
							_nowSelectedAISlot = null;					
							mon.setPlayAniRightNow(Monster.NORMAL);
							_state = STATE_PREPARE;							
						}
						else
						{
							mon.setPlayAniRightNow(Monster.WALK);
						}
					}
					else
					{
						mon.setPosition(mon.cTransformPosition + _v2);
						_v = mon.cTransformPosition;
						if(_v.x < _tempX)
						{
							_nowSelectedAISlot = null;					
							mon.setPlayAniRightNow(Monster.NORMAL);
							_state = STATE_PREPARE;							
						}
						else
						{
							mon.setPlayAniRightNow(Monster.WALK);
						}
					}
				}
				else
				{
					_nowSelectedAISlot = null;					
					mon.setPlayAniRightNow(Monster.NORMAL);
					_state = STATE_PREPARE;
				}
			}
			else if(_nowSelectedAISlot.ai.moveType == MonsterHeroAI.MoveType.ACTION_POSITION)
			{
				// 최전방에 가거나 x 번째 애를 때릴 수 있는 곳에 가면 정지.
				// 도착했으면 공격을 할 수 있으면 공격을 한다.
				// 공격을 할 수 없다면 ai는 리셋.
				if(checkAI()) return;

				mon.heroMonsterData.attackType.chracterMove(mon);

			}
			else if(_nowSelectedAISlot.ai.moveType == MonsterHeroAI.MoveType.SKILL_POSITION)
			{
				// 스킬 사용 가능 위치로 이동.
				// 최전방에 갔거나 스킬 사용 위치까지 이동했다면 멈추고 스킬을 사용한다.
				// 그리고 ai는 리셋.
				mon.skillSlots[_nowSelectedAISlot.ai.skillNum].canSkillMove();
			}
			else
			{
				if(_nowSelectedAISlot.ai.check(mon, _nowSelectedAISlot) == false)
				{
					_nowSelectedAISlot = null;
					_state = STATE_PREPARE;
					mon.setPlayAniRightNow(Monster.NORMAL);
				}
			}
			break;
		
		case STATE_ANIMATION:

			_delay -= GameManager.globalDeltaTime;
			
			if(_delay <= 0)
			{
				_nowSelectedAISlot = null;
				_state = STATE_PREPARE;
			}

			break;

		case STATE_ACTION:

			bool setAniIgnore = _nowSelectedAISlot.ai.setAniIgnore;
			bool ignoreIdleAni = _nowSelectedAISlot.ai.ignoreIdleAni;

			switch(_nowSelectedAISlot.ai.actionType)
			{
			case MonsterHeroAI.ActionType.ATTACK:
				setAttackDelay();
				mon.state = Monster.ATK_IDS[mon.heroMonsterData.attackType.type];
				_state = STATE_WAIT;				
				break;
			case MonsterHeroAI.ActionType.SKILL:

				_nowSelectedAISlot.ai.checkCanTargetingSkillAtAction(mon);

				mon.state = Monster.ATK_IDS[mon.skillSlots[_nowSelectedAISlot.ai.actionSkill[0]].skillData.exeData.type];
				_state = STATE_WAIT;	

				if(mon.skillTarget != null)
				{
					mon.targetPosition = mon.skillTarget.cTransformPosition;
					mon.targetHeight = mon.skillTarget.hitObject.height;
					mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
				}

				break;

			case MonsterHeroAI.ActionType.SKILL2:
				
				//_nowSelectedAISlot.ai.checkCanTargetingSkillAtAction(mon);
				_delay += GameManager.globalDeltaTime;
				if(_delay < _nowSelectedAISlot.ai.actionSkillDelay)
				{
					updateTargetingDecal();
					checkAISkill2Mode();
				}
				else
				{
					// 데칼 삭제.

					bool canShoot = false;

					switch(mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData.targeting)
					{
					case TargetingData.AUTOMATIC_2:
					case TargetingData.FORWARD_LINEAR_3:
						canShoot = checkSkillTarget(true);
						break;
					default:
						canShoot = true;
						break;
					}

					if(canShoot)
					{
						mon.state = Monster.ATK_IDS[mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData.exeData.type];
						_state = STATE_WAIT;	
					}
					else
					{
						decal.hide();
						onCompleteAttackAni();
					}
				}
				break;


			case MonsterHeroAI.ActionType.SUMMON:
				//mon.state = Monster.SHOOT; ////mon.playAni(Monster.SHOOT);			
				_state = STATE_PREPARE;		
				createSummon(_nowSelectedAISlot.ai);
				break;

			case MonsterHeroAI.ActionType.PLAYANI:

				_state = STATE_ANIMATION;

				//mon.nowAniId = _nowSelectedAISlot.ai.actionAni;
				//mon.setAniData(mon.nowAniId);
				mon.playAni(_nowSelectedAISlot.ai.actionAni);
				mon.renderAni();

				_delay = mon.ani[_nowSelectedAISlot.ai.actionAni].length;

				//playani일때 강제로 애니메이션이 동작하게 하면서 그 다음에 walk를 무시하게 할 것인지를 정한다.
				// 만약 walk를 무시하게만들면 이동시 walk 애니는 동작하지 않는다.
				// 샌드웜이 땅에 숨을때 써먹을거다.
				// 숨은뒤 이동하게 만들면 된다. 그리고 다시 나타나는 애니메이션을 동작시키거나 혹은 다른 공격 동작이 시전될때는
				// 이동 애니를 다시 켜게 만들면 된다.
				// 문제는 이동 위치를 특정 위치로 정하기 애매하는 것이다. 현재 위치를 기준으로 상대위치로 옮기는 기능을 추가해야하나?

				//mon.setPlayAniRightNow("");

				//mon.animation[_nowSelectedAISlot.ai.aniName].length

				break;
			}

			if(setAniIgnore)
			{
				mon.ignoreIdleAni = ignoreIdleAni;
			}

			break;
		}
	}	


	private bool _setTarget = false;
	void initTargetingDecal()
	{
		_nowSelectedSkillData = mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData;

		if(_nowSelectedSkillData.targetType == Skill.TargetType.ENEMY)
		{

			switch(_nowSelectedSkillData.targeting)
			{
			case TargetingData.NONE:
				break;
			case TargetingData.FIXED_1:
				decal.init(TargetingDecal.DecalType.Circle, (float)_nowSelectedSkillData.targetAttr[1] * 0.005f, false, true, _nowSelectedAISlot.ai.actionSkillDelay);//checkSkillTarget(true));
				decal.setColor(Color.red);
				decal.skillExeType = _nowSelectedSkillData.exeData.type;
				break;
			case TargetingData.AUTOMATIC_2:
				decal.init(TargetingDecal.DecalType.Circle, 1.0f, false, checkSkillTarget(false), _nowSelectedAISlot.ai.actionSkillDelay);
				decal.setColor(Color.red);
				decal.skillExeType = _nowSelectedSkillData.exeData.type;
				break;
			case TargetingData.FORWARD_LINEAR_3:

				if(mon.isVisible )
				{
					decal.init(TargetingDecal.DecalType.Arrow, 1.0f, false, checkSkillTarget(false), -1.0f);
					decal.setColor(Color.red);
					decal.skillExeType = _nowSelectedSkillData.exeData.type;
				}
				else
				{
					decal.visible = false;
				}

				break;				
			}	
		}
		else
		{
			decal.visible = false;
		}

		_setTarget = false;
		_decalVisible = false;
	}


	bool _decalVisible = false;
	void updateTargetingDecal()
	{
		if(_nowSelectedSkillData == null) return ;

		if(_nowSelectedSkillData.targeting != TargetingData.NONE) //checkSkillTarget(true))
		{
			if(_setTarget == false)
			{
				switch(_nowSelectedSkillData.targeting)
				{
				case TargetingData.FIXED_1:
					
					//				public int actionSkillTarget = 0;
					//				public float actionSkillDist = 0.0f;
					
					switch(_nowSelectedAISlot.ai.actionSkillTarget)
					{
					case -1: // 본인.
						mon.setSkillTarget(mon);
						mon.targetHeight = mon.hitObject.height;
						mon.targetPosition = mon.cTransformPosition;		
						mon.targetPosition.x += _nowSelectedAISlot.ai.actionSkillDist;
						mon.targetUniqueId = mon.stat.uniqueId;
						decal.setPosition(mon.targetPosition);
						_decalVisible = true;
						_setTarget = true;
						
						break;
					case 0: // 플레이어 히어로.
						mon.setSkillTarget(GameManager.me.player);
						mon.targetHeight = GameManager.me.player.hitObject.height;
						mon.targetPosition = GameManager.me.player.cTransformPosition;		
						mon.targetPosition.x += _nowSelectedAISlot.ai.actionSkillDist;
						mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
						decal.setPosition(mon.targetPosition);
						_decalVisible = true;
						_setTarget = true;
						break;
					default:
						
						if(_nowSelectedSkillData.targetType == Skill.TargetType.ME)
						{
							mon.setSkillTarget(GameManager.me.characterManager.getCloseMonsterTeamTargetByIndex(_nowSelectedAISlot.ai.actionSkillTarget, mon));
						}
						else
						{
							mon.setSkillTarget(GameManager.me.characterManager.getCloseEnemyTargetByIndex(false,_nowSelectedAISlot.ai.actionSkillTarget));
						}
						
						if(mon.skillTarget != null)
						{
							mon.targetHeight = mon.skillTarget.hitObject.height;
							mon.targetPosition = mon.skillTarget.cTransformPosition;		
							mon.targetPosition.x += _nowSelectedAISlot.ai.actionSkillDist;
							mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
							_decalVisible = true;
							decal.setPosition(mon.targetPosition);
							_setTarget = true;
						}
						
						break;
					}
					
					break;
				case TargetingData.AUTOMATIC_2:
					_decalVisible = checkSkillTarget(true);
					_setTarget = _decalVisible;
					break;
				case TargetingData.FORWARD_LINEAR_3:

					if(mon.isVisible)
					{
						_decalVisible = checkSkillTarget(true);
						_setTarget = _decalVisible;
					}
					break;				
				}	
			}

			if(_decalVisible && _nowSelectedSkillData.targetType == Skill.TargetType.ENEMY)
			{
				if(decal.didStartEffect == false)
				{
					decal.startDecalEffect();
				}

				decal.visible = true;
			}
			else decal.visible = false;
		}
		else
		{
			decal.visible = false;
		}
	}



	private bool checkSkillTarget(bool checkTargetOnly)
	{
		if(_nowSelectedSkillData == null) return false;

		if(_nowSelectedSkillData.targeting >= 2)
		{
			return _nowSelectedSkillData.playerTargetingChecker(mon, checkTargetOnly, decal);
		}
		else
		{
			return true;
		}
	}


	sealed public override void clearSkillData ()
	{
		base.clearSkillData ();
		_nowSelectedAISlot = null;
		_state = STATE_PREPARE;
		mon.setPlayAniRightNow(Monster.NORMAL);
	}
	
	
	int _createNum = 0;
	sealed public override void onAttack(int totalDamageNum, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
#if UNITY_EDITOR
		if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			if(_isDebugAttack)
			{
				mon.nowBulletPatternId = mon.bulletPatternId;
				mon.heroMonsterData.attackType.monsterShoot(mon, totalDamageNum);
			}
			else
			{
				mon.nowBulletPatternId = mon.skillSlots[_debugSkillSlot].getBulletPatternId();
				if(mon.skillTarget != null)
				{
					mon.targetPosition = mon.skillTarget.cTransformPosition;
					mon.targetHeight = mon.skillTarget.hitObject.height;
					mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
				}
				mon.skillSlots[_debugSkillSlot].doSkill(totalDamageNum);
			}

			return;
		}
#endif


		if(_nowSelectedAISlot == null) return;

		if(_nowSelectedAISlot.ai.actionType == MonsterHeroAI.ActionType.ATTACK)
		{
			mon.nowBulletPatternId = mon.bulletPatternId;
			mon.heroMonsterData.attackType.monsterShoot(mon, totalDamageNum);
		}
		else if(_nowSelectedAISlot.ai.actionType == MonsterHeroAI.ActionType.SKILL)
		{
//			Debug.LogError("MonsterHeroAI.ActionType.SKILL");

			mon.nowBulletPatternId = mon.skillSlots[_nowSelectedAISlot.ai.actionSkill[0]].getBulletPatternId();
			if(mon.skillTarget != null)
			{
				mon.targetPosition = mon.skillTarget.cTransformPosition;
				mon.targetHeight = mon.skillTarget.hitObject.height;
				mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
			}
			mon.skillSlots[_nowSelectedAISlot.ai.actionSkill[0]].doSkill(totalDamageNum);
		}
		else if(_nowSelectedAISlot.ai.actionType == MonsterHeroAI.ActionType.SKILL2)
		{
			if(decal.didStartEffect)
			{
				decal.hide();
			}
//			Debug.LogError("MonsterHeroAI.ActionType.SKILL2 : " + mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData.name);

			mon.nowBulletPatternId = mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].getBulletPatternId();

			if(mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData.targeting == TargetingData.FORWARD_LINEAR_3)
			{
				mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].skillData.canPlayerHeroTargetingType3(mon,false,null);
			}

			mon.skillSlots[_nowSelectedAISlot.ai.actionSkill2].doSkill(totalDamageNum);
		}
	}


	static List<int> shuffleList = new List<int>();
	static List<int> shuffleList2 = new List<int>();

	void createSummon(MonsterHeroAI ai)
	{

		_createNum = ai.actionSummonSlotNum.Length;
		
		int i = 0;
		
		shuffleList.Clear();
		shuffleList2.Clear();
		
		for(i = 0; i < _createNum; ++i)
		{
			shuffleList.Add(i);
		}
		
		int targetIndex = 0;
		for(i = 0; i < _createNum; ++i)
		{
			targetIndex = GameManager.inGameRandom.Range(0,shuffleList.Count);
			shuffleList2.Add(shuffleList[targetIndex]);
			shuffleList.RemoveAt(targetIndex);
		}
		
		
		for(i = 0; i < _createNum; ++i)
		{
			targetIndex = shuffleList2[i];

			mon.unitSlots[ai.actionSummonSlotNum[i]].create(ai.actionSummonSlotPosX, targetIndex, _createNum);	
		}
	}

	
	sealed public override void onCompleteAttackAni(bool isClearActionType = false)
	{
		if(_nowSelectedAISlot != null )
		{
			_nextCheckAI = _nowSelectedAISlot.ai.nextAi;

			if(_nowSelectedAISlot.ai.actionType == MonsterHeroAI.ActionType.SKILL2)
			{
				mon.setSkillTarget(null);
			}

			if(string.IsNullOrEmpty(_nextCheckAI ) == false)
			{
				_hasNextCheckAI = true;
			}
		}

		_nowSelectedAISlot = null;
		_state = STATE_PREPARE;
		mon.setPlayAniRightNow(Monster.NORMAL);
	}	


	public override void dead ()
	{
		base.dead ();
		if(decal != null)
		{
			decal.hide();
		}
	}


	public override void clear ()
	{
		base.clear ();

		_nowSelectedSkillData = null;

		if(decal != null)
		{
			GameManager.me.characterManager.setMonsterHeroTargetingDecal(decal);
		}

		decal = null;
	}
	
	
	
	
	
	bool _isDebugAttack = false;
	int _debugSkillSlot = 0;

	// 디버깅용.
	public override void setData (object data)
	{
		string fuck = (string)data;
		string[] f = fuck.Split(',');

		if(f[0] == "ANI")
		{
			_isDebugAttack = true;
			mon.playAni(f[1]);
			return;
		}

		mon.setTarget(GameManager.me.characterManager.playerMonster[0]);
		mon.setSkillTarget(GameManager.me.characterManager.playerMonster[0]);

		// 0이면 일반공격, 1이면 스킬이다.
		if(Convert.ToInt32(f[0]) == 0)
		{
			_isDebugAttack = true;

			setAttackDelay();
			mon.state = Monster.ATK_IDS[mon.heroMonsterData.attackType.type];
			_state = STATE_WAIT;				
		}
		else
		{
			_isDebugAttack = false;

			_debugSkillSlot = Convert.ToInt32(f[1]);

			if(mon.skillSlots.Length > _debugSkillSlot)
			{
				mon.state = Monster.ATK_IDS[mon.skillSlots[_debugSkillSlot].skillData.exeData.type];
				
				_state = STATE_WAIT;	
				
				if(mon.skillTarget != null)
				{
					mon.targetPosition = mon.skillTarget.cTransformPosition;
					mon.targetHeight = mon.skillTarget.hitObject.height;
					mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
				}
			}
		}
	}

	
	
	
	
	
	
}

