using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class Player : Monster
{
	// ======== Attach 스킬 이동 ==============
	public float hpWhenAttachSkillStart = 0.0f;
	
	void aiAttachSkillMove()
	{
		if(moveState == MoveState.Forward)
		{
			moveForward();
		}
		else if(moveState == MoveState.Backward)
		{
			moveBackward();
		}
		else
		{
			attackData.lookDirection(this,fowardDirectionValue);
		}

		for(int i = 0; i < _attachSkillMoveLen; ++i)
		{
			if(_attachSkillMoveChecker[i].checkAttachSkillMove(this,(isPlayerSide)?GameManager.me.player:BaseSkillData.enemyHero) )
			{
				return;
			}
		}

		attackData.lookDirection(this,(isPlayerSide)?100.0f:-100.0f);
		state = NORMAL;
	}
}