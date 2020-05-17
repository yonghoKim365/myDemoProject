using System;
using UnityEngine;

public class CharacterAction
{

	public const int STATE_FREEZE = -1;
	public const int STATE_PREPARE = 0;
	public const int STATE_MOVE = 1;	
	public const int STATE_ACTION = 2;
	public const int STATE_SUMMON = 3;
	public const int STATE_WAIT = 4;	
	public const int STATE_ANIMATION = 5;


	
	public Monster mon;
	
	public bool isDamageFrame = false;
	public bool isAfterAttackFrame = false;

	public bool canUsePassiveSkill = true;

	public UnitSkillData nowUnitSkillData = null;

	public CharacterAction ()
	{
	}

	public IVector3 targetPos = Vector3.zero;
	public IFloat dx = 0.0f;
	public IFloat dy = 0.0f;
	
	public static IFloat rotationSpeed = 10.0f;
	public static IFloat combatZone = 200.0f;
	public static IFloat playerCombatZone = 50.0f;

	public Xfloat freezeTime = 0.0f;

	protected Quaternion _q; 		
	
	protected int _state;
	
	public virtual int state
	{
		set { _state =  value; }
		get { return _state; }
	}

	protected IVector3 _v;
	protected IVector3 _v2;
	protected HitObject _tempHitObject;
	protected HitObject _targetHitObject;
	protected HitObject _shooterHitObject;
	protected IFloat _tempX;
	protected IFloat _tempF;


	public virtual void clear()
	{
		mon = null;
		nowUnitSkillData = null;
		canUsePassiveSkill = true;
	}


	public virtual void init(Monster monster)
	{
		this.mon = monster;
		dx = 0.0f;
		dy = 0.0f;
		state = 0;
		targetPos.x = 0.0f;
		targetPos.y = 0.0f;
		targetPos.z = 0.0f;
		state = 0;
		if(mon != null)
		{
			mon.target = null;
			mon.targetAngle = 0;
		}
		isDamageFrame = false;
		isAfterAttackFrame = false;
		canUsePassiveSkill = true;
		delay = 0.0f;
		freezeTime = 0.0f;
	}
	
	public virtual void setFirstPosition(Vector3 pos)
	{
	}

	public virtual void setData(object data)
	{

	}

	
	public virtual void startAction()
	{
		_state = STATE_PREPARE;
		//Debug.Log("action : startAction");
	}
	

	public virtual void doMotion()
	{
	}

	
	public virtual void damage(int damageValue)
	{
	}

	
	public virtual void dead()
	{
		mon.target = null;
		mon.targetAngle = 0;
	}
	
	public Xfloat delay = 0.0f;
	public void setAttackDelay()
	{
		delay = mon.stat.atkSpeed;		
	}

	public void setAttackDelay(float offset)
	{
		delay = mon.stat.atkSpeed * offset;
	}
	
	
	public virtual void onAttack(int totalDamageNum, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
	}
	
	public virtual void onCompleteAttackAni(bool isClearActionType = false)
	{
		//Debug.LogError("action onCompleteAttackAni");
	}
	
	public virtual void clearSkillData()
	{
	}
	
	
}

