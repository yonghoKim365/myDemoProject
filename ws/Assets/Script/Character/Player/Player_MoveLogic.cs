using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class Player : Monster
{
	private float _moveTimeChecker = 0.0f;

	private IVector3 _pvpTargetPosition;
	private IFloat _nowSkillTargetCheckDelay = 0.0f;
	const float SKILLTARGET_CHECK_DELAY = 0.2f;
	public int changeAutoPlayDelay = 0;


	private void movePlayer()
	{
		if(isFreeze == false)
		{
			if(isPlayerSide && (GameManager.me.isAutoPlay == false && BattleSimulator.nowSimulation == false) )
			{
#if UNITY_EDITOR
//				if(BattleSimulator.nowSimulation == false) Log.log("move normal");
#endif

				normalPlayerMove();
			}
			else
			{
				#if UNITY_EDITOR
//				if(BattleSimulator.nowSimulation == false) Log.log("move ai");
#endif
				checkMovingTargetPosition();

				// 일반 이동모드일때.
				if(moveType == MoveType.NORMAL)
				{
//					if(isPlayersPlayer == false) Debug.Log("moveType == MoveType.NORMAL");
					aiNormalMove();
				}
				else
				{
					if(isPlayerSide && _nowSelectSkillSlot == null)
					{
						moveType = Player.MoveType.NORMAL;
						skillMoveIsNormal = true;
						return;
					}

					// 스킬 이동모드일때.
					if(moveType == MoveType.SKILL)
					{
						//					if(isPlayersPlayer == false)Debug.Log("moveType == MoveType.SKILL");
						checkSkillChargingTime();
						aiSkillMove();
					}
					// 어태치 스킬이동 모드일때.
					else //if(moveType == MoveType.ATTACH_SKILL)
					{
						//					if(isPlayersPlayer == false)Debug.Log("moveType == MoveType.ATTACH_SKILL");
						aiAttachSkillMove();
					}


				}
			}
		}
	}	

	public IFloat leftFullChargingTime = 0.0f;
	void checkSkillChargingTime()
	{
		//풀차징 시간 (ms) = 풀차징까지의 남은시간
//		Debug.Log("_nowSelectSkillSlot.chargingTimeLimit : " + _nowSelectSkillSlot.chargingTimeLimit);
//		Debug.Log("chargingTime : " + chargingTime);
		leftFullChargingTime = _nowSelectSkillSlot.chargingTimeLimit - chargingTime;
	}


	// pvp 캐릭터 후진.
	public bool moveBackward()
	{
		_v = cTransformPosition;
		
		_v2 = _v;
		_v2.x += (isPlayerSide)?-1000.0f:1000.0f;
		_v2.z = 0.0f;
		
		_q = Util.getLookRotationQuaternion(_v2 - cTransformPosition);
		
		_v2 = _q.eulerAngles;
		_v2.y += 180.0f;
		_q.eulerAngles = _v2;

		if(Xfloat.greaterThan( Quaternion.Angle(_q, tf.rotation) , 5)) tf.rotation = Util.getFixedQuaternionSlerp( tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);	

		_v -= (IVector3)(tf.forward) * stat.speed * GameManager.globalDeltaTime;
		_v.y = 0;

		if(isPlayerSide)
		{
			if(Xfloat.greaterThan(  _v.x , StageManager.mapStartPosX.Get() ) && cm.checkPlayerMonsterBlockLine(this)) 
			{
				setPosition(_v); 
			}
			else
			{
				state = NORMAL;
				return false;
			}
		}
		else
		{
			if(Xfloat.lessThan(  _v.x , StageManager.mapEndPosX.Get()) && cm.checkMonsterBlockLine(this)) setPosition(_v); 
			else
			{
				state = NORMAL;
				return false;
			}
		}

		//if(_state != WALK) 
		state = BWALK;
		return true;
	}



	public bool moveForward()
	{
		_v = cTransformPosition;
		_v.x += (isPlayerSide)?1000.0f:-1000.0f;
		tf.rotation = Util.getFixedQuaternionSlerp(tf.rotation, Util.getLookRotationQuaternion(_v - cTransformPosition), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);	

		_v = cTransformPosition;
		_v += (IVector3)(tf.forward) * stat.speed * GameManager.globalDeltaTime;
		_v.y = 0;

		if(isPlayerSide == false)
		{
			if(cm.playerMonsterRightLine <= _v.x && cm.checkMonsterBlockLine(this)) 
			{
				setPosition(_v); 
			}
			else
			{
				state = NORMAL;
				return false;
			}
		}
		else
		{
			if(_v.x <= cm.monsterLeftLine && cm.checkPlayerMonsterBlockLine(this))
			{
				setPosition(_v); 
			}
			else
			{
				state = NORMAL;
				return false;
			}
		}
		
		if(_state != WALK) state = WALK;
		return true;
	}


	/*
	 * 특정 위치를 지정하여 이동시킬때 그 위치로 충분히 이동했으면 멈추게 한다.
	 */
	void checkMovingTargetPosition()
	{
		if(nowMoveToTargetPosition == false) return;

//		if(isPlayerSide == false)
//		{
//			Debug.Log("nowMoveToTargetPosition : " + nowMoveToTargetPosition + "     currentTargetMovingPosition: " + currentTargetMovingPosition + "   ct: " + cTransformPosition);
//		}

		if(moveState == MoveState.Backward)
		{
			if(isPlayerSide)
			{
				if(cTransformPosition.x < currentTargetMovingPosition.x)
				{
					setMovingDirection(MoveState.Stop);
					state = NORMAL;
				}
			}
			else
			{
				if(cTransformPosition.x > currentTargetMovingPosition.x)
				{
					setMovingDirection(MoveState.Stop);
					state = NORMAL;				
				}
			}
		}
		else if(moveState == MoveState.Forward)
		{
			if(isPlayerSide)
			{
				if(cTransformPosition.x > currentTargetMovingPosition.x)
				{
					setMovingDirection(MoveState.Stop);
					state = NORMAL;
				}
			}
			else
			{
				if(cTransformPosition.x < currentTargetMovingPosition.x)
				{
					setMovingDirection(MoveState.Stop);
					state = NORMAL;
				}
			}
		}
	}




// ========= 일반 이동 ==============
	void normalPlayerMove()
	{
		if(moveState == MoveState.Backward) // 뒤로 후진...
		{
			removeTarget(!nowPlayingSkillAni);
			moveBackward(); // 끝까지 뒤로 이동했으면 더이상 이동하지 못한다.
		}
		else if(UIPopupSkillPreview.isOpen == false)
		{
			action.doMotion();
		}
	}



}