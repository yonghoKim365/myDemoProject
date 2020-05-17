using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseSkillData
{
	private Player _attacker;

	public static Player enemyHero;


	// ===== PVP && AUTOPLAY ============================================================================================== //

	// 일단 위험도 계산을 위한 녀석들이다.

	// <1 : 히어로 전방 A거리(cm), B지름> 
	///- 타게팅(차징)중 히어로 전방 A거리에 B지름 형태의 (원형 마법진모양의) 타게팅 데칼을 표시함					

	// checktarget은 이 스킬을 쳐맞을 대상이다.
	// targetPosition은 체크를 할 위치가 된다.
	// 라인을 그었다면 그 위치가 되겠지...


	// 1,2,3번의 targetposition은 현재라면 타겟당할 캐릭터의 위치, 위험도(미래 위치 계산)이라면 위치 라인위치를 넣으면 된다.


	public bool canPlayerHeroTargetingType1PVP(Player checkTarget, Vector3 targetPosition)
	{
		// 플레이어 1번의 경우.....
		// 중심점을 기준으로 체크한다. 

		if(targetType == Skill.TargetType.ENEMY) // 상대에게 쏘는것.
		{
			if(checkTarget.isPlayerSide)
			{
				// 적이 나에게 쏘는것.
				_v = enemyHero.cTransformPosition;
				_v.x -= targetAttr[0].Get();
			}
			else
			{
				// 내가 적에게 쏘는것.
				_v = GameManager.me.player.cTransformPosition;
				_v.x += targetAttr[0].Get();
			}

		}
		else // 같은편에게 쏘는 것.
		{
			if(checkTarget.isPlayerSide) // 대상이 나인데 우리편에게 쏘는 거면 시전자는 우리편이다. 
			{
				// 내가 우리편에게 쏘는것.
				_v = GameManager.me.player.cTransformPosition;
				_v.x += targetAttr[0].Get();
			}
			else
			{
				// 적이 적에게 쏘는것.
				_v = enemyHero.cTransformPosition;
				_v.x -= targetAttr[0].Get();
			}
		}

		// 데칼 범위에 타겟 포인트가 들어가있는지 확인한다.
		return (_v.x - targetAttr[1].Get() <= targetPosition.x
		        && _v.x + targetAttr[1].Get() >= targetPosition.x
		        && _v.z - targetAttr[1].Get() <= targetPosition.z
		        && _v.z + targetAttr[1].Get() >= targetPosition.z);
	}
	

	//  - '1' 세팅시, 히어로와 가장 가까운 적(아군)				
	//   - '2' 세팅시, HP가 가장 낮은 적(아군)

	// 리미트 거리가 있는 것.



	public bool canPlayerHeroTargetingType2PVP(Player checkTarget, Vector3 targetPosition)
	{
		//데미지 범위. exeData.attr[1];
		// 타겟이 적이면 공격자를 나로 해야한다.
		// 대상이 우리편인데 적에게 쏘는거다. 그럼 시전자는 적이다. 
		if(targetType == Skill.TargetType.ENEMY) 
		{
			_attacker = (checkTarget.isPlayerSide)?enemyHero:GameManager.me.player;
		}
		else
		{
			_attacker = (checkTarget.isPlayerSide)?GameManager.me.player:enemyHero;
		}

		// 일단 현재 위치들을 기준으로 자동타겟을 가져옴.
		_target = TargetingData.getAutomaticTarget(_attacker, targetType, targetAttr, canUseThisSkillOnThisType);

		// 자동타겟을 가져왔는데 타겟이 아무도 없어 그럼 체크고 나발이고 그냥 아웃.
		if(_target == null) 
		{
			_attacker = null;
			return false;
		}

		// 가장 가까이에 위치한 적.
		if(targetAttr[0].Get() == TargetingData.AUTOMATIC_CLOSE_TARGET_1)
		{
			// 히어로 미포함시.
			if(targetAttr[1].Get() == 1)
			{
				_attacker = null;
				_target = null;
				return false;
			}
			// 공격자에게 가까운 녀석을 가져왔는데 내가 선택한 녀석이 아니네?
			// 그럼 다시 체크해본다.
			// 자동선택해준녀석과 위험도에 있을 적의 위치
			IFloat distBetweenTargetPositionAndAttacker = VectorUtil.DistanceXZ( targetPosition, _attacker.cTransformPosition);

			if(_target != checkTarget)
			{
				// 이동 위치(특정 위치라거나 혹은 이동할 위치)가 현재 타겟된 녀석보다 공격자보다 가까우면 위험도 위치에 타게팅이 됨.
				// 아니라면 끝.
				if(VectorUtil.DistanceXZ( _target.cTransformPosition, _attacker.cTransformPosition) < distBetweenTargetPositionAndAttacker)
				{
					_attacker = null;
					_target = null;
					return false;
				}
			}

			_attacker = null;
			_target = null;

			// 타겟 포지션이 타게팅 위치가 된다는 것을 확인했다. 하지만 타게팅 제한거리보다 거리가 멀면 아웃.
			if(distBetweenTargetPositionAndAttacker < _targetDistanceLimit)
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}
		// 에너지가 낮은 적이니까 이건 획득한 캐릭터가 내가 아니면 무조건 아니다. 거리랑은 상관이 없는 녀석이다.
		else if(_target != checkTarget)
		{
			_attacker = null;
			_target = null;
			return false;
		}

		_attacker = null;
		_target = null;
		return true;
	}		


	//<3 : 전방 직선 발사>	
	public bool canPlayerHeroTargetingType3PVP(Player checkTarget, Vector3 targetPosition)
	{	
		if(targetType == Skill.TargetType.ENEMY) 
		{
			if(checkTarget.isPlayerSide == false)
			{
				// 피격자가 적일때 적의 위험위치가 현재 가장 좌측에 있는 녀석보다 왼쪽에 있으면 쳐맞을 수 있다.
				return (GameManager.me.characterManager.monsters.Count > 0 && GameManager.me.characterManager.monsters[0].lineLeft > targetPosition.x - checkTarget.hitObject.width);
			}
			else
			{
				// 피격자가 나일때 나의 위험위치가 현재 가장 우측에 있는 녀석보다 우측에 있으면 쳐맞을 수 있다.
				return (GameManager.me.characterManager.playerMonster.Count > 0 && GameManager.me.characterManager.playerMonster[0].lineRight < targetPosition.x + checkTarget.hitObject.width);
			}
		}
		else // 같은 편에게 쏠때.
		{
			if(checkTarget.isPlayerSide == false)
			{
				// 피격자가 적일때 타겟 포지션의 우측은 주인공의 위치보다 작아야한다.
				// 그리고 주인공하고 가장 가까워야한다.
				int count = GameManager.me.characterManager.monsters.Count;
				if(count > 1)
				{
					if(targetPosition.x < enemyHero.cTransformPosition.x)
					{
						// 몬스터는 0이 왼쪽에 있는 녀석이다. 
						if(enemyHero.lineIndex == 0) return true;
						else if(GameManager.me.characterManager.monsters[enemyHero.lineIndex+1].cTransformPosition.x > targetPosition.x) return true;
					}
				}
			}
			else
			{
				// 피격자가 나일때 타겟 포지션의 좌측은 주인공의 위치보다 커야한다.
				// 그리고 주인공하고 가장 가까워야한다.
				int count = GameManager.me.characterManager.playerMonster.Count;
				if(count > 1)
				{
					if(targetPosition.x > GameManager.me.player.cTransformPosition.x)
					{
						// 몬스터는 0이 왼쪽에 있는 녀석이다. 
						if(GameManager.me.player.lineIndex == 0) return true;
						else if(GameManager.me.characterManager.playerMonster[GameManager.me.player.lineIndex+1].cTransformPosition.x < targetPosition.x) return true;
					}
				}
			}
		}

		return false;
	}		
	
	
	public bool canPlayerHeroTypeReturnTruePVP(Player checkTarget, Vector3 targetPosition)
	{
		return true;
	}	




	
	
//============== 공격/시전자가 전진을 할 경우 타겟팅이 된다 ============ //


	public bool canPlayerHeroTargetingType1PVP2(Player checkTarget, Vector3 targetPosition)
	{
		if(targetType == Skill.TargetType.ENEMY) // 적에게 쏘는것.
		{
			if(checkTarget.isPlayerSide)
			{
				_v = enemyHero.cTransformPosition;
				_v.x -= targetAttr[0].Get();
				// 적이 주인공에게 쏘는것.
				// 1. 데칼의 우측이 타겟 포지션보다 작으면 안된다.
				if(_v.x - targetAttr[1].Get() > targetPosition.x) return false;
				// 2. 최전방 이동 가능위치의 데칼의 좌측이 타겟 포지션보다 작으면 된다.
				if( (GameManager.me.characterManager.playerMonsterRightLine - targetAttr[0] - targetAttr[1] < targetPosition.x ) == false) return false;
			}
			else
			{
				_v = GameManager.me.player.cTransformPosition;
				_v.x += targetAttr[0].Get();
				// 주인공이 적에게 쏘는것.
				// 1. 데칼의 좌측이 타겟 포지션보다 크면 안된다.
				if(_v.x - targetAttr[1].Get() > targetPosition.x) return false;
				// 2. 최전방 이동 가능위치의 데칼의 우측이 타겟 포지션보다 크면 된다.
				if(  (targetPosition.x < GameManager.me.characterManager.monsterLeftLine + targetAttr[0] + targetAttr[1]) == false) return false;
			}
		}
		else // 같은편에게 쏘는 것.
		{
			if(checkTarget.isPlayerSide) 
			{
				_v = GameManager.me.player.cTransformPosition;
				_v.x += targetAttr[0].Get();
				// 내가 우리편에게 쏘는것.
				// 1. 데칼의 좌측이 타겟 포지션보다 크면 안된다.
				if(_v.x - targetAttr[1].Get() > targetPosition.x) return false;
				// 2. 최전방 이동 가능위치의 데칼의 좌측이 타겟 포지션보다 작으면 된다.
				if( (targetPosition.x < GameManager.me.characterManager.monsterLeftLine + targetAttr[0] + targetAttr[1]) == false) return false;
			}
			else
			{
				_v = enemyHero.cTransformPosition;
				_v.x -= targetAttr[0].Get();
				// 적이 적에게 쏘는것.
				// 1. 데칼의 우측이 타겟 포지션보다 작으면 안된다.
				if(_v.x - targetAttr[1].Get()> targetPosition.x) return false;
				// 2. 최전방 이동 가능위치의 데칼의 좌측이 타겟 포지션보다 작으면 된다.
				if( (GameManager.me.characterManager.playerMonsterRightLine - targetAttr[0] - targetAttr[1] < targetPosition.x ) == false) return false;
			}
		}

		return (_v.z - targetAttr[1].Get() <= targetPosition.z && _v.z + targetAttr[1].Get() >= targetPosition.z);
	}
	


	public bool canPlayerHeroTargetingType2PVP2(Player checkTarget, Vector3 targetPosition)
	{
		// 타겟이 적이면 공격자를 나로 해야한다.
		// 대상이 우리편인데 적에게 쏘는거다. 그럼 시전자는 적이다. 
		if(targetType == Skill.TargetType.ENEMY) 
		{
			_attacker = (checkTarget.isPlayerSide)?enemyHero:GameManager.me.player;
		}
		else
		{
			_attacker = (checkTarget.isPlayerSide)?GameManager.me.player:enemyHero;
		}
		
		// 가장 가까이에 위치한 적.
		if(targetAttr[0] == TargetingData.AUTOMATIC_CLOSE_TARGET_1)
		{
			// 히어로 미포함시.
			if(targetAttr[1] == 1)
			{
				_attacker = null;
				return false;
			}

			if(targetType == Skill.TargetType.ENEMY)
			{
				// 적에게 쏘는 것은 타겟 포지션에 있는 녀석이 최전방에 있으면 됨.
				// 왜냐하면 가장 가까운 녀석에게 쏘는 거니까. 우리한테 가장 가까운 녀석은 최전방에 있는 녀석이라고 보면 됨.
				// 엄밀히 말하면 상하 위치도 봐야겠지만 전진까지 계산해서는 답이 안나온다.
				if(_attacker.isPlayerSide)
				{
					if(GameManager.me.characterManager.monsters[0] != checkTarget && targetPosition.x + checkTarget.hitObject.lineLeft > GameManager.me.characterManager.monsterLeftLine)
					{
						_attacker = null;
						return false;
					}
				}
				else
				{
					if(GameManager.me.characterManager.playerMonster[0] != checkTarget && targetPosition.x + checkTarget.hitObject.lineRight < GameManager.me.characterManager.playerMonsterRightLine)
					{
						_attacker = null;
						return false;
					}
				}
			}
			else
			{
				// 같은 편에게 쏠때는 전방 이동이 가능하므로. 주인공보다 전방에 있으면 무조건 사용 가능이다.
				if(_attacker.isPlayerSide) 
				{
					if(_attacker.cTransformPosition.x > targetPosition.x)
					{
						_attacker = null;
						return false;
					}
				}
				else
				{
					if(_attacker.cTransformPosition.x < targetPosition.x)
					{
						_attacker = null;
						return false;
					}
				}
			}

			// 타겟 포지션이 타게팅 위치가 된다는 것을 확인했다. 하지만 타게팅 제한거리보다 거리가 멀면 아웃.
			if(VectorUtil.DistanceXZ(checkTarget.cTransformPosition, _attacker.cTransformPosition) < _targetDistanceLimit)
			{
				_attacker = null;
				return true;
			}
			else
			{
				_attacker = null;
				return false;
			}
		}
		// 에너지가 낮은 적이니까 이건 획득한 캐릭터가 내가 아니면 무조건 아니다. 거리랑은 상관이 없는 녀석이다.
		else 
		{
			// 일단 현재 위치들을 기준으로 자동타겟을 가져옴.
			_target = TargetingData.getAutomaticTarget(_attacker, targetType, targetAttr, canUseThisSkillOnThisType);

			if(_target == null || _target != checkTarget)
			{
				_attacker = null;
				_target = null;
				return false;
			}
		}
		
		_attacker = null;
		_target = null;
		return true;
	}		
	

	
	public bool canPlayerHeroTargetingType3PVP2(Player checkTarget, Vector3 targetPosition)
	{	
		if(targetType == Skill.TargetType.ENEMY) 
		{
			if(checkTarget.isPlayerSide == false)
			{
				// 피격자가 적일때 적의 위험위치가 현재 가장 좌측에 있는 녀석보다 왼쪽에 있으면 쳐맞을 수 있다.
				return (GameManager.me.characterManager.monsters.Count > 0 && GameManager.me.characterManager.monsters[0].lineLeft > targetPosition.x - checkTarget.hitObject.width);
			}
			else
			{
				// 피격자가 나일때 나의 위험위치가 현재 가장 우측에 있는 녀석보다 우측에 있으면 쳐맞을 수 있다.
				return (GameManager.me.characterManager.playerMonster.Count > 0 && GameManager.me.characterManager.playerMonster[0].lineRight < targetPosition.x + checkTarget.hitObject.width);
			}
		}
		else // 같은 편에게 쏠때.
		{
			if(checkTarget.isPlayerSide == false)
			{
				// 피격자가 적일때 타겟 포지션의 우측은 주인공의 위치보다 작아야한다.
				// 그리고 주인공하고 가장 가까워야한다.
				int count = GameManager.me.characterManager.monsters.Count;
				if(count > 1)
				{
					if(targetPosition.x < enemyHero.cTransformPosition.x)
					{
						return true;
					}
				}
			}
			else
			{
				// 피격자가 나일때 타겟 포지션의 좌측은 주인공의 위치보다 커야한다.
				// 그리고 주인공하고 가장 가까워야한다.
				int count = GameManager.me.characterManager.playerMonster.Count;
				if(count > 1)
				{
					if(targetPosition.x > GameManager.me.player.cTransformPosition.x)
					{
						return true;
					}
				}
			}
		}
		
		return false;

	}		
	
	
	public bool canPlayerHeroTypeReturnTruePVP2(Player checkTarget, Vector3 targetPosition)
	{
		return true;
	}	
	
	
	
	



}