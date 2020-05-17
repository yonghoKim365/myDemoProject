using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class MonsterDeadZone : CharacterAction
{
	public MonsterDeadZone ()
	{
	}
	
	private CharacterManager _cm;
	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		_cm = GameManager.me.characterManager;
	}
	
	
	sealed public override void startAction ()
	{
		base.startAction ();
	}

	float _hitRadius = 0.0f;

	public override void setData (object data)
	{
		ChallengeInfinitySurvival d = (ChallengeInfinitySurvival)data;

		_hitRadius = d.hitRadius;

		_cm.targetingDecal[2].init(TargetingDecal.DecalType.DeadZone, _hitRadius * 0.01f,true);
		_cm.targetingDecal[2].mc.start(Color.red, _hitRadius * 0.01f, false);
		_cm.targetingDecal[2].setPosition(mon.cTransformPosition);
	}
	

	sealed public override void doMotion()
	{
		hitTest();
	}	
	
	
	void hitTest()
	{
		_v = mon.cTransformPosition;
		
		for(int i = _cm.playerMonster.Count - 1; i >= 0; --i)
		{
			if(_cm.playerMonster[i].isEnabled)
			{
				if(VectorUtil.DistanceXZ(_v, _cm.playerMonster[i].cTransformPosition) <= _hitRadius + _cm.playerMonster[i].damageRange)
				{
					if(_cm.playerMonster[i].isPlayer)
					{
						_cm.playerMonster[i].characterEffect.addKnockBack(200);
					}
					else
					{
						_cm.playerMonster[i].damageDead();
					}
				}
			}
		}
	}

	
	
	
	public override void clear ()
	{
		base.clear ();
		_cm.targetingDecal[2].isEnabled = false;
		_cm = null;
		
	}
	
}

