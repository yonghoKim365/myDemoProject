using System;

public abstract class BaseSlot
{
	public const string LOCK = "L";
	public const string LOCK2 = "L2";
	public const string CHECK = "C";

	public const int STATE_READY = 0;
	public const int STATE_COOLTIME = 1;
	public const int STATE_PRESS = 2;

	public enum InventorySlotType
	{
		Normal, FriendDetailSlot, HeroInfoSlot
	}


	public BaseSlot ()
	{
	}
	
	protected Monster _heroMon;
	
	public Xfloat maxCoolTime = 0.0f;
	public Xfloat coolTime = 0.0f;
	
	public virtual void update()
	{
		coolTime.Set ( coolTime - GameManager.globalDeltaTime );
	}
	
	public abstract bool canUse();
	
	public virtual void resetCoolTime()
	{
		coolTime.Set( maxCoolTime );
	}	
		
	
	public virtual bool checkCooltime()
	{
		return (coolTime <= 0.0f);
	}		

	public virtual void destroy()
	{
		_heroMon = null;
	}

}

