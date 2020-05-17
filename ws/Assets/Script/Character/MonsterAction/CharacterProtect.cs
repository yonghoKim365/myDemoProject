using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class ProtectNpcAction : CharacterAction
{
	public ProtectNpcAction ()
	{
	}

	private Transform _effTransform;
	
	private CharacterManager cm;
	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		cm = GameManager.me.characterManager;
		delay = 0;
	}
	
	private string _prevState;
	
	sealed public override void startAction ()
	{
		base.startAction ();
		mon.state = Monster.NORMAL; //mon.playAni(Monster.WALK);
		_firstDelay = 0.5f;

		//mon.changeAnimationSpeed(Monster.WALK, mon.stat.speed / 300.0f);
	}
	
	private float _firstDelay = 0.3f;

	
	sealed public override void doMotion()
	{
		if(_firstDelay > 0)
		{
			mon.state = Monster.NORMAL;
			_firstDelay -= GameManager.globalDeltaTime;
			return;
		}

		if(delay > 0)
		{
			delay -= GameManager.globalDeltaTime;
			return;
		}

		_v = mon.cTransformPosition;

		if(_v.x < cm.playerMonsterRightLine - targetPos.x)
		{
			if(mon.isPet)
			{
				mon.playAni(Monster.WALK);
			}
			else if(mon.state != Monster.WALK)
			{
				mon.state = Monster.WALK;
			}

			_v.x += mon.stat.speed * GameManager.globalDeltaTime;
			mon.setPosition(_v);
		}
		else
		{
			delay = ((float)GameManager.inGameRandom.Range(5,10))*0.1f;
			mon.state = Monster.NORMAL;
		}
	}
}

