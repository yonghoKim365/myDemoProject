using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class MonsterChaser : CharacterAction
{
	public MonsterChaser ()
	{
	}

	private Transform _effTransform;
	
	private CharacterManager cm;

	private float _firstDelay = 0.3f;
	private Monster _target;
	
	private float _setColorTime = -1000.0f;
	private bool _setRed = false;
	private float _infoShowTime = 0;
	private int _infoValue = 0;
	private int _m = 0;

	
	sealed public override void init(Monster monster)
	{
		base.init(monster);
		cm = GameManager.me.characterManager;

		_infoValue = -1000;
		_setColorTime = 0.0f;
		_setRed = false;
		_infoShowTime = -1;
		_m = 0;
	}
	
	private string _prevState;
	
	sealed public override void startAction ()
	{
		base.startAction ();
		mon.state = Monster.NORMAL; //mon.playAni(Monster.WALK);
		_firstDelay = 0.5f;
	}

	sealed public override void doMotion()
	{
		switch(_state)
		{
		case STATE_PREPARE:
			
			if(_firstDelay > 0)
			{
				_firstDelay -= GameManager.globalDeltaTime;
				return;
			}
			
			if(GameManager.me.stageManager.protectNPC == null)
			{
				move(GameManager.me.player);
			}
			else
			{
				if(GameManager.me.stageManager.protectNPC.lineLeft < GameManager.me.player.lineLeft)
				{
					move (GameManager.me.stageManager.protectNPC);
				}
				else
				{
					move(GameManager.me.player);
				}
			}
			
			break;
		
		case STATE_ACTION:

			GameManager.me.cutSceneManager.startUnitSkillCamScene("CHASER", mon.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.ChaserAttack);

			if(mon.ani.GetClip(Monster.ATK) == false)
			{
				_target.damageDead();
				setAttackDelay();
				onCompleteAttackAni();
			}
			else
			{
				mon.state = Monster.SHOOT;
				setAttackDelay();
				_state = STATE_WAIT;				
			}
			break;
		}

		if(_infoValue <= 7)
		{
			if(GameManager.me.stageManager.playTime - _setColorTime >= 0.5f)
			{
				_setColorTime = GameManager.me.stageManager.playTime;
			}
			else return;

			if(_setRed)
			{
//				GameManager.me.uiManager.uiPlay.lbChaser.enabled = true;
//				GameManager.me.uiManager.uiPlay.lbChaser.color = Color.white;
//				_setRed = false;
			}
			else
			{
//				GameManager.me.uiManager.uiPlay.lbChaser.enabled = false;
				GameManager.me.uiManager.uiPlay.lbChaser.color = Color.red;
				_setRed = true;
			}
		}
		else
		{
			if(_setRed)
			{
//				GameManager.me.uiManager.uiPlay.lbChaser.enabled = true;
				GameManager.me.uiManager.uiPlay.lbChaser.color = Color.white;
				_setRed = false;
			}
		}
	}
	


	void move(Monster target)
	{
		if(GameManager.me.stageManager.playTime - _infoShowTime > 0.5f)
		{
			_m = (int)(VectorUtil.Distance(GameManager.me.player.cTransformPosition.x , mon.cTransformPosition.x) * 0.01f );

			if(_m != _infoValue)
			{
				GameManager.me.uiManager.uiPlay.lbChaser.text = "-" + _m + "m";
				_infoValue = _m;
				_infoShowTime = GameManager.me.stageManager.playTime;
			}
		}


		_target = target;

		mon.attackPosition = Util.getPositionByAngleAndDistanceXZ(0, mon.stat.atkRange  + _target.damageRange + mon.damageRange); // hitrange
		//mon.action.delay = 0.3f;
	
		_v = _target.cTransformPosition + mon.attackPosition;

		if(VectorUtil.DistanceXZ(_target.cTransformPosition , mon.cTransformPosition) <= mon.stat.atkRange  + _target.damageRange + mon.damageRange) // hitrange
		{
			_v = _target.cTransformPosition - mon.cTransformPosition;
			
			_q = Util.getLookRotationQuaternion(_v);
			// 자리를 다 잡았으면 공격...
			mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
			
			if((mon.action.delay > 0 || Xfloat.greaterThan( Quaternion.Angle(_q, mon.tf.rotation) , 5)) || _target.isEnabled == false)
			{
				mon.state = Monster.NORMAL; 
				mon.action.delay -= GameManager.globalDeltaTime;
			}
			else
			{
				mon.action.state = CharacterAction.STATE_ACTION;
			}
			
		}						
		else if(mon.cTransformPosition.x + mon.attackPosition.x >= _target.cTransformPosition.x && VectorUtil.Distance(_v.z, mon.cTransformPosition.z) < 5)
		{
			//Debug.Log("1");
			
			_v = _target.cTransformPosition - mon.cTransformPosition;
			
			_q = Util.getLookRotationQuaternion(_v);
			// 자리를 다 잡았으면 공격...
			mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
			
			if((mon.action.delay > 0 || Xfloat.greaterThan( Quaternion.Angle(_q, mon.tf.rotation) , 5)) || _target.isEnabled == false)
			{
				mon.state = Monster.NORMAL; 
				mon.action.delay -= GameManager.globalDeltaTime;
			}
			else
			{
				mon.action.state = CharacterAction.STATE_ACTION;
			}
		}						
		else
		{
			_v = _target.cTransformPosition + mon.attackPosition;
			_q = Util.getLookRotationQuaternion(_v - mon.cTransformPosition);
			mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
			
			_v = mon.cTransformPosition + mon.tf.forward * mon.stat.speed * GameManager.globalDeltaTime;
			_v.y = 0;

			mon.setPlayAniRightNow(Monster.WALK);
			mon.animation[Monster.WALK].speed = 0.3f;
			mon.setPosition(_v);
		}
		
	}
	
	
	
	
	
	sealed public override void onAttack(int totalDamageNum, int targetX = -1000, int targetY = -1000, int targetZ = -1000, int targetH = -1000)
	{
		//Debug.LogError("=======   Chaser Attack!");
		_target.damageDead();

	}
	
	sealed public override void onCompleteAttackAni(bool isClearActionType = false)
	{
		_state = STATE_PREPARE;
		//Debug.Log("Delay : " + delay);
		mon.state = Monster.NORMAL; //mon.playAni(Monster.NORMAL);
		//mon.setHitObject();		
	}
	
	
}

