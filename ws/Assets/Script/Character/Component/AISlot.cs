using System;

sealed public class AISlot : BaseSlot
{
	public AISlot ()
	{
	}
	
	public int excuteCount = 0;
	public MonsterHeroAI ai;
	
	public void setData(Monster mon, MonsterHeroAI ai)
	{
		//Log.logError(ai.id, ai.coolTime);
		
		_heroMon = mon;	
		coolTime = ai.coolTime + ai.coolTimeStartDelay;
		maxCoolTime = ai.coolTime;
		//_coolTimeStartDelay = ai.coolTimeStartDelay;
		excuteCount = 0;
		this.ai = ai;
	}

	sealed public override void update ()
	{
		if(ai.firstCheck(_heroMon, this) == false) return;
		base.update ();
		//Log.log(ai.id, _coolTime);
	}
	
	
	sealed public override bool canUse()
	{
		return (coolTime <= 0.0f);
	}

	public override void destroy ()
	{
		base.destroy ();
		ai = null;
	}
	
}

