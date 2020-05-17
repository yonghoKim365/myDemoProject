using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class PlayerAction : CharacterAction
{
	public PlayerAction ()
	{
	}
	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		//setAttackDelay();
		player = (Player)monster;
	}

	Player player = null;

	sealed public override void clear()
	{
		mon = null;
		nowUnitSkillData = null;
		canUsePassiveSkill = true;
		player = null;
	}

	sealed public override void startAction ()
	{
		base.startAction ();
	}
	
	sealed public override void doMotion()
	{

		switch(_state)
		{
		case STATE_PREPARE:

			// 전진 중이면 그냥 이동시키면 된다.
			// 라인 타입이면 그냥 앞으로 이동시키면 된다.
			// 근거리 공격이면 전투 영역에 들어가면 타겟팅을 해서 움직인다.
			// 정지중이고 기본공격 및 스킬을 사용중이지 않으면 기본공격을 쓸 수 있다.
			// 공격이 가능한 상황이라면.
			// 근접 공격일때는 상대에게 자동으로 접근도 하는데 아주 단거리만 이동하게 만든다.
			// 원거리 공격일때는 제자리에서 방향만 틀어서 공격을 하게 한다.
	
			if(player.moveState == Monster.MoveState.Forward)
			{
				if(player.nowPlayingSkillAni)
				{
					player.attackData.moveLineType(mon);
					return;
				}

				player.attackData.chracterMove(mon);
			}
			else // 후진은 없다. 어차피 얘가 작동될때는 그 전에 전부 전진 혹은 정지로 세팅이 된다.
			{
				if(player.isDefaultAttacking == false && player.nowPlayingSkillAni == false)
				{

					if(player.isPlayerSide)
					{
						// 공격 거리 이내에 접근이면 공격 이동을.
						if((GameManager.me.stageManager.isIntro == false) && 
						   player.lineRight + player.stat.atkRange >= GameManager.me.characterManager.monsterLeftLine)
						{
							player.attackData.chracterMove(mon);
						}
						// 아니면 제자리 정지다.
						else
						{
							player.attackData.lookDirection(mon, 100.0f);
							player.setPlayAniRightNow(Monster.NORMAL);
						}
					}
					else
					{
						// 공격 거리 이내에 접근이면 공격 이동을.
						if(player.lineLeft - player.stat.atkRange <= GameManager.me.characterManager.playerMonsterRightLine)
						{
							player.attackData.chracterMove(mon);
						}
						// 아니면 제자리 정지다.
						else
						{
							player.attackData.lookDirection(mon, -100.0f);
							player.setPlayAniRightNow(Monster.NORMAL);
						}
					}

				}
				// 일반 공격을 하거나 스킬 동작중인데 타겟은 없네?
				else if(player.target == null)
				{
					player.attackData.lookDirection(player, player.fowardDirectionValue);
					if(player.state != Monster.NORMAL && player.nowAniId.Contains(Monster.SKILL_HEAD) == false)
					{
						player.setPlayAniRightNow(Monster.NORMAL);
					}
				}
				else if(player.state != Monster.NORMAL && player.nowAniId.Contains(Monster.SKILL_HEAD) == false)
				{
					player.setPlayAniRightNow(Monster.NORMAL);
				}
			}

			break;
		case STATE_ACTION:

			if(player.nowPlayingSkillAni || player.isDefaultAttacking)
			{
				_state = STATE_PREPARE;
				return;
			}

#if UNITY_EDITOR

			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				if(UnitSkillCamMaker.instance.disablePlayerAttack)
				{
					setAttackDelay();
					_state = STATE_PREPARE;
					return;
				}
			}
#endif

			setAttackDelay();
			player.isDefaultAttacking.Set( true );
			player.state = Monster.ATK_IDS[ player.attackData.type]; //Monster.SHOOT;


			_state = STATE_WAIT;				
			break;
		case STATE_WAIT:
			if(player.moveState == Monster.MoveState.Forward) player.attackData.chracterMove(mon);
			else
			{

			}
			break;
		}
	}


	bool canPlayerUseDefaultAttack(Monster mon)
	{
		if(player.nowChargingSkill != null || player.nowPlayingSkillAni)
		{
			return false;
		}
		
		return true;
	}


	// 일반 공격.
	sealed public override void onAttack(int totalDamageNum, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		if(player.nowPlayingSkillAni) return;

#if UNITY_EDITOR

		if(player.nowChargingSkill != null)
		{
			int a = 0;
		}
#endif

		player.nowBulletPatternId = player.playerData.partsWeapon.parts.baseId;
		player.attackData.monsterShoot(mon, totalDamageNum);				
	}
	
	sealed public override void onCompleteAttackAni(bool isClearActionType = false)
	{
		_state = STATE_PREPARE;
		player.setPlayAniRightNow(Monster.NORMAL);
	}

	
	sealed public override void dead ()
	{
		base.dead ();
	}
	
}

