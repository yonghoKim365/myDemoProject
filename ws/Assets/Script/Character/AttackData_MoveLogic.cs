using System;
using UnityEngine;
using System.Collections.Generic;

sealed public partial class AttackData
{
	
	private IVector3 _v;
	private IVector3 _v2;


	private Xfloat _tempX;
	private Xfloat _tempX2;

	private Xfloat _tempF;		
	private Xfloat _tempF2;

	private Xfloat _tempI;
	private Quaternion _q;
	
	public delegate void CharacterMove(Monster mon);
	public CharacterMove chracterMove;
	public CharacterMove skillMove;
	
	
	private HitObject _hitObject;

	public const float FAST_SPEED_VALUE = 2.0f;

	//=================================== 근접 공격용. ======================================================//
	Monster target;
	
	void moveShortAttackType(Monster mon)
	{
		if(mon.isPlayerSide)
		{
			if(mon.isPlayer)
			{
				movePlayerShortAttackType(mon);
			}
			else
			{
				movePlayerMonsterShortAttackType(mon);
			}
		}
		else
		{
			if(mon.isPlayer)
			{
				movePVPShortAttackType(mon);
			}
			else
			{
				moveMonsterShortAttackType (mon);
			}
		}
	}



	// 근접 공격 타입인 적 유닛, 히어로 몬스터가 사용하는 이동 로직.
	void moveMonsterShortAttackType(Monster mon)
	{
//		Log.log(mon.resourceId + " moveMonsterShortAttackType : " + mon.cTransformPosition + "  mon.stat.atkRange : " + mon.stat.atkRange);  //ff 
		//_tempF = mon.cTransformPosition.x - mon.stat.atkRange;
		_tempF = mon.lineLeft - mon.stat.atkRange;
		_tempF2 = cm.playerMonsterRightLine + CharacterAction.combatZone ;

		// 1. 나의 공격 거리가 전투존 밖이면 앞으로 그냥 전진한다. 단 block monster를 체크해야한다.
		if(_tempF > _tempF2 && cm.checkMonsterBlockLine(mon))
		{
//			Log.log("_tempF:",_tempF ,"playerMonsterRightLine:", cm.playerMonsterRightLine , " CharacterAction.combatZone:", CharacterAction.combatZone , "cm.checkMonsterBlockLine(mon):", cm.checkMonsterBlockLine(mon)); //ff  

			bool isFastType = (GameManager.me.stageManager.isPVPMode && mon.lineLeft > cm.pvpPlayerPosX);

			_tempF = _tempF - mon.stat.speed * GameManager.globalDeltaTime * (isFastType?AttackData.FAST_SPEED_VALUE:1.0f);

			// 전진은 하지만 맵 시작 위치 보다 많이 가면 그대로 정지해야한다.
			if(_tempF <= StageManager.mapStartPosX)
			{
//				Log.log(" 01 speed:",mon.stat.speed," mpox:"+StageManager.mapStartPosX); //ff 
				mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("47");
			}
			else
			{
				moveForwardAndRotation(mon,true, isFastType);
			}
		}
		// 2. 전투존안에 들어왔는데 맵 시작위치에 걸리면 움직이지 못한다. 이 경우면 어차피 상대할 적은 없다. 고로 공격도 못할거다.
		else if(Xfloat.lessEqualThan(  _tempF - mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapStartPosX ))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("54");
		}		
		// 3. block 몬스터에 걸리면 움직이지 못한다. 근접 공격 타입은 앞에 오브젝트가 있으면 적을 때릴 방법이 없다고 보는게 맞다.
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("59");
		}
		else 
		{
//			Log.log("   mon.target 1 : " + mon.target); //ff 
			
			// 일단 전투존 안에 들어왔다.
			// 1. 타겟을 찾아본다.
			// 타겟을 찾아서 때릴 수 있는 최적위치를 체크한다.
			if(mon.target == null)
			{
				mon.checkWalkTime = 0.0f;
				mon.target = cm.getShortTargetCharacter(mon);
				if(mon.target != null)
				{
					mon.target.setTargetAngle(mon);
					mon.attackPosition = Util.getPositionByAngleAndDistanceXZ(mon.targetAngle, mon.target.damageRange + mon.stat.atkRange + mon.damageRange);
					
					if(mon.attackPosition.x < 0) mon.target = null;
					else
					{
						mon.target.attackers.Add(mon);	

						_hitObject = mon.getHitObject();
						
						if( Xfloat.lessEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z - (_hitObject.depth * 0.5f)).AsFloat() , MapManager.bottom * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.bottom * 0.8f) - mon.target.cTransformPosition.z + (_hitObject.depth * 0.5f);
						}
						else if( Xfloat.greatEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z + (_hitObject.depth * 0.5f)).AsFloat() , MapManager.top * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.top * 0.8f) - mon.target.cTransformPosition.z - (_hitObject.depth * 0.5f);
						}


					}
				}
			}

			//2. 타겟을 찾았으면.
			if(mon.target != null)
			{
				if(mon.target.isEnabled == false)
				{
					mon.removeTarget();
					return;
				}
				
				// 타겟을 향해 걸어가던지. 다 걸어갔으면 걔를 때려야한다.
				_v = mon.target.cTransformPosition + mon.attackPosition;

				// 공격 범위안에 있다! 그럼 회전/공격 딜레이 판단후 공격한다.
				if(Xfloat.lessEqualThan( VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition) , mon.target.damageRange + mon.stat.atkRange + mon.damageRange ))
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				// 최적 공격 위치 중 x 좌표에 접근했고 둘간의 z 값의 차이가 5 이하다. 
				// 그럼 공격 할 수 있다고 판단한다. 
				else if(Xfloat.greatEqualThan( mon.target.cTransformPosition.x + mon.attackPosition.x , mon.cTransformPosition.x) && 
				        VectorUtil.Distance(_v.z, mon.cTransformPosition.z) < (IFloat)(mon.hitObject.depth * 1.2f ))//5)
						
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				else
				{
					// 자리를 못잡았으면 적의 위치를 계산해서 가져와야한다.
					// 적의 위치는 적을 때리고 있는 애들의 위치도 계산해서 가져온다.
					
					// 적이 최전방에서 공격 가능 범위보다 멀리 있는 녀석이다. 그럼 타겟이 될 수 없다.
					if((Xfloat.lessThan( (mon.target.lineRight + cm.targetZoneDistance) ,  cm.playerMonsterRightLine ))) 
					{
						mon.removeTarget();
					}
					else
					{

						// 타겟과 근접해 있음에도 계속 움직인 경우 약간의 딜레이 후엔 새로운 적을 찾아보게 한다.
						if(mon.target.lineRight > mon.lineLeft)
						{
							mon.checkWalkTime += GameManager.globalDeltaTime;
							if(mon.checkWalkTime > 0.3f)
							{
								mon.removeTarget();
								return;
							}
						}
						else mon.checkWalkTime = 0.0f;
						
						_hitObject = mon.getHitObject();
						
						if(_hitObject.z <= MapManager.bottom)
						{
							_v = mon.cTransformPosition;
							_v.z += mon.stat.speed * GameManager.globalDeltaTime;
							
							mon.setPosition(_v);							
							mon.removeTarget();									
							return;
						}
						else if(_hitObject.distance >= MapManager.top)
						{
							_v = mon.cTransformPosition;
							_v.z -= mon.stat.speed * GameManager.globalDeltaTime;
							
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}							
						
						_v = mon.target.cTransformPosition + mon.attackPosition;
						_q = Util.getLookRotationQuaternion(_v - mon.cTransformPosition);
						mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
						
						_v = mon.cTransformPosition + (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime;
						_v.y = 0.0f;

						if(_v.x > mon.cTransformPosition.x)
						{
							_v.x = mon.cTransformPosition.x;
							//mon.removeTarget();
							//return;
						}
						else if( Xfloat.lessThan( _v.x , mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f ))
						{
							_v.x = mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f;
						}

						mon.setPlayAniRightNow(Monster.WALK);   //Log.log("154");  //ff 
						mon.setPosition(_v);
					}
				}
			}	
		}		
	}
	

	// 근접 공격 타입인 우리 편 몬스터들이 사용.
	void movePlayerMonsterShortAttackType(Monster mon)
	{
		//_tempF = mon.cTransformPosition.x + mon.stat.atkRange;//mon.hitRange;
		_tempF.Set( mon.lineRight + mon.stat.atkRange );
		_tempF2.Set( cm.monsterLeftLine - CharacterAction.combatZone );

		// 1. 전투 존 밖이면 직진한다.
		if(_tempF < _tempF2 && cm.checkPlayerMonsterBlockLine(mon) )
		{
			bool isFastType = (mon.lineRight < cm.playerPosX );

			_tempF.Set( _tempF + mon.stat.speed * GameManager.globalDeltaTime * (isFastType?FAST_SPEED_VALUE:1.0f) );

//			Debug.LogError(_tempF + "  s: " + mon.stat.speed +"   dt:" +  GameManager.globalDeltaTime + "  value: " +( mon.stat.speed * GameManager.globalDeltaTime ) );
			if(_tempF >= StageManager.mapPlayerEndPosX)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("172");
			}
			else
			{
				moveForwardAndRotation(mon,false,isFastType);
			}
		}
		else if(Xfloat.greatEqualThan( _tempF + mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapPlayerEndPosX ) )
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("178");
		}
		else if(cm.checkPlayerMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("182");
		}
		else 
		{
			// 일단 전투존 안에 들어왔다.
			// 1. 타겟을 찾아본다.
			if(mon.target == null)
			{
				mon.checkWalkTime = 0.0f;
				mon.target = cm.getShortTargetCharacter(mon);
				if(mon.target != null)
				{
					mon.target.setTargetAngle(mon);	
					mon.attackPosition = Util.getPositionByAngleAndDistanceXZ(mon.targetAngle, mon.target.damageRange + mon.stat.atkRange + mon.damageRange);//mon.hitRange 
//					Log.logError("mon.targetAngle : " + mon.targetAngle + "mon.stat.atkRange: " ,mon.stat.atkRange, "mon.target.damageRange : " + mon.target.damageRange + " mon.attackPosition : "+ mon.attackPosition);
					if(mon.attackPosition.x > 0) mon.target = null;
					else
					{
						mon.target.attackers.Add(mon);		

						_hitObject = mon.getHitObject();


						if(Xfloat.lessEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z - (_hitObject.depth * 0.5f)).AsFloat() , MapManager.bottom * 0.8f ) )
						{
							mon.attackPosition.z = (MapManager.bottom * 0.8f) - mon.target.cTransformPosition.z + (_hitObject.depth * 0.5f);
						}
						else if(Xfloat.greatEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z + (_hitObject.depth * 0.5f)).AsFloat() , MapManager.top * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.top * 0.8f) - mon.target.cTransformPosition.z - (_hitObject.depth * 0.5f);
						}


					}
				}
			}
			
			//2. 타겟을 찾았으면.
			if(mon.target != null)
			{
				if(mon.target.isEnabled == false)
				{
					mon.removeTarget();
					return;
				}
				
				// 타겟을 향해 걸어가던지. 다 걸어갔으면 걔를 때려야한다.
				
				_v = mon.target.cTransformPosition + mon.attackPosition;
				
//				Log.log(this,VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition), mon.stat.atkRange, mon.target.damageRange, mon.target.cTransformPosition , mon.cTransformPosition );  //ff 
				
				if(Xfloat.lessEqualThan( VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition) , mon.target.damageRange + mon.stat.atkRange + mon.damageRange ))// mon.hitRange
				{
					lookTargetAndAttack(mon, mon.target,true);
				}
				else if(_v.x <= mon.cTransformPosition.x && 
				        VectorUtil.Distance(_v.z, mon.cTransformPosition.z) < (IFloat)(mon.hitObject.depth * 1.2f )) // 5가 좋을까???
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				else
				{
					// 자리를 못잡았으면 적의 위치를 계산해서 가져와야한다.
					// 적의 위치는 적을 때리고 있는 애들의 위치도 계산해서 가져온다.
					if(Xfloat.lessThan( cm.monsterLeftLine + cm.targetZoneDistance , mon.target.lineLeft )) mon.removeTarget();
					else
					{

						// 주인공쪽....
						if(mon.target.lineLeft < mon.lineRight)
						{
							mon.checkWalkTime += GameManager.globalDeltaTime;
							if(mon.checkWalkTime > 0.3f)
							{
								mon.removeTarget();
								return;
							}
						}
						else mon.checkWalkTime = 0.0f;
						
						_hitObject = mon.getHitObject();
						
						if(_hitObject.z <= MapManager.bottom)
						{
							_v = mon.cTransformPosition;
							_v.z += mon.stat.speed * GameManager.globalDeltaTime;
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}
						else if(_hitObject.distance >= MapManager.top)
						{
							_v = mon.cTransformPosition;
							_v.z -= mon.stat.speed * GameManager.globalDeltaTime;
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}
						
						_v = mon.target.cTransformPosition + mon.attackPosition;

						_q = Util.getLookRotationQuaternion(_v - mon.cTransformPosition);
						mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
						
						_v = mon.cTransformPosition + (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime;
						_v.y = 0.0f;

						if(_v.x < mon.cTransformPosition.x)
						{
							_v.x = mon.cTransformPosition.x;
//							mon.removeTarget();
//							return;
						}
						else if(Xfloat.greaterThan( _v.x , mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f ) )
						{
							_v.x = mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f;
						}

						mon.setPlayAniRightNow(Monster.WALK);   //ff Log.log("271");
						mon.setPosition(_v);
						
					}
				}
			}	
		}
	}


	//==============================


	// 근접 공격 타입인 적 유닛, 히어로 몬스터가 사용하는 이동 로직.
	void movePVPShortAttackType(Monster mon)
	{
//		Log.log(mon.name + " moveMonsterShortAttackType : " + mon.cTransformPosition + "  mon.hitRange : " + mon.hitRange); //ff 
		//_tempF = mon.cTransformPosition.x - mon.stat.atkRange;// hitRange;
		_tempF.Set( mon.lineLeft - mon.stat.atkRange );

		// 1. 나의 공격 거리가 전투존 밖이면 앞으로 그냥 전진한다. 단 block monster를 체크해야한다.
		if(mon.moveState == Monster.MoveState.Forward && mon.lineLeft > cm.playerMonsterRightLine && cm.checkMonsterBlockLine(mon))
		{
//			Log.log("_tempF:",_tempF ,"playerMonsterRightLine:", cm.playerMonsterRightLine , " CharacterAction.combatZone:", CharacterAction.combatZone , "cm.checkMonsterBlockLine(mon):", cm.checkMonsterBlockLine(mon)); //ff 

			bool isFastType = (GameManager.me.stageManager.isPVPMode && mon.lineLeft > cm.pvpPlayerPosX);
			_tempF.Set( mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime * (isFastType?AttackData.FAST_SPEED_VALUE:1.0f) );

			// 전진은 하지만 맵 시작 위치 보다 많이 가면 그대로 정지해야한다.
			if(_tempF <= StageManager.mapStartPosX)
			{
//				Log.log(" 01 speed:",mon.stat.speed," mpox:"+StageManager.mapStartPosX); //ff 
				mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("47");
			}
			else
			{
				moveForwardAndRotation(mon,true,isFastType);
			}
		}
		// 2. 전투존안에 들어왔는데 맵 시작위치에 걸리면 움직이지 못한다. 이 경우면 어차피 상대할 적은 없다. 고로 공격도 못할거다.
		else if(Xfloat.lessEqualThan( _tempF - mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapStartPosX))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("54");
		}		
		// 3. block 몬스터에 걸리면 움직이지 못한다. 근접 공격 타입은 앞에 오브젝트가 있으면 적을 때릴 방법이 없다고 보는게 맞다.
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("59");
		}
		else 
		{
//			Log.log("   mon.target 1 : " + mon.target); //ff 
			
			// 일단 전투존 안에 들어왔다.
			// 1. 타겟을 찾아본다.
			// 타겟을 찾아서 때릴 수 있는 최적위치를 체크한다.
			if(mon.target == null)
			{
				mon.checkWalkTime = 0.0f;
				mon.target = cm.getShortTargetCharacter(mon);
				if(mon.target != null)
				{
					mon.target.setTargetAngle(mon);
					mon.attackPosition = Util.getPositionByAngleAndDistanceXZ(mon.targetAngle, mon.target.damageRange + mon.stat.atkRange + mon.damageRange);
					
					if(mon.attackPosition.x < 0) mon.target = null;
					else
					{
						mon.target.attackers.Add(mon);	

						_hitObject = mon.getHitObject();
						
						if(Xfloat.lessEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z - (_hitObject.depth * 0.5f)).AsFloat() , MapManager.bottom * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.bottom * 0.8f) - mon.target.cTransformPosition.z + (_hitObject.depth * 0.5f);
						}
						else if( Xfloat.greatEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z + (_hitObject.depth * 0.5f)).AsFloat() , MapManager.top * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.top * 0.8f) - mon.target.cTransformPosition.z - (_hitObject.depth * 0.5f);
						}


					}
				}
			}
			
			//2. 타겟을 찾았으면.
			if(mon.target != null)
			{
				if(mon.target.isEnabled == false)
				{
					mon.removeTarget();
					return;
				}
				
				// 타겟을 향해 걸어가던지. 다 걸어갔으면 걔를 때려야한다.
				_v = mon.target.cTransformPosition + mon.attackPosition;
				
				// 공격 범위안에 있다! 그럼 회전/공격 딜레이 판단후 공격한다.
				if(Xfloat.lessEqualThan( VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition) , mon.target.damageRange + mon.stat.atkRange + mon.damageRange ))
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				// 최적 공격 위치 중 x 좌표에 접근했고 둘간의 z 값의 차이가 5 이하다. 
				// 그럼 공격 할 수 있다고 판단한다. 
				else if(Xfloat.greatEqualThan( mon.target.cTransformPosition.x + mon.attackPosition.x , mon.cTransformPosition.x) && 
				        VectorUtil.Distance(_v.z, mon.cTransformPosition.z) < (IFloat)(mon.hitObject.depth * 1.2f ) )//5)
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				else
				{
					// 자리를 못잡았으면 적의 위치를 계산해서 가져와야한다.
					// 적의 위치는 적을 때리고 있는 애들의 위치도 계산해서 가져온다.
					
					// 적이 최전방에서 공격 가능 범위보다 멀리 있는 녀석이다. 그럼 타겟이 될 수 없다.
					if((Xfloat.lessThan( mon.target.lineRight + cm.targetZoneDistance , cm.playerMonsterRightLine ) ))
					{
						mon.removeTarget();
					}
					else
					{
						

						
						// 타겟과 근접해 있음에도 계속 움직인 경우 약간의 딜레이 후엔 새로운 적을 찾아보게 한다.
						if(mon.target.lineRight > mon.lineLeft)
						{
							mon.checkWalkTime += GameManager.globalDeltaTime;
							if(mon.checkWalkTime > 0.3f)
							{
								mon.removeTarget();
								return;
							}
						}
						else mon.checkWalkTime = 0.0f;
						
						_hitObject = mon.getHitObject();
						
						if(_hitObject.z <= MapManager.bottom)
						{
							_v = mon.cTransformPosition;
							_v.z += mon.stat.speed * GameManager.globalDeltaTime;
							
							mon.setPosition(_v);							
							mon.removeTarget();									
							return;
						}
						else if(_hitObject.distance >= MapManager.top)
						{
							_v = mon.cTransformPosition;
							_v.z -= mon.stat.speed * GameManager.globalDeltaTime;
							
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}							
						
						_v = mon.target.cTransformPosition + mon.attackPosition;
						_q = Util.getLookRotationQuaternion(_v - mon.cTransformPosition);
						mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
						
						_v = mon.cTransformPosition + mon.tf.forward * mon.stat.speed * GameManager.globalDeltaTime;
						_v.y = 0.0f;
						
						if(_v.x > mon.cTransformPosition.x)
						{
							_v.x = mon.cTransformPosition.x;
							//							mon.removeTarget();
							//							return;
						}
						else if(Xfloat.lessThan( (_v.x).AsFloat() , (mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f).AsFloat() ))
						{
							_v.x = mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f;
						}
						
						mon.setPlayAniRightNow(Monster.WALK);  //ff Log.log("154");
						mon.setPosition(_v);
					}
				}
			}	
		}		
	}
	

	// 플레이어. 레오같은 놈.
	void movePlayerShortAttackType(Monster mon)
	{
		//_tempF = mon.cTransformPosition.x + mon.stat.atkRange;//mon.hitRange;
		_tempF = mon.lineRight + mon.stat.atkRange;

		
		// 1. 전투 존 밖이면 직진한다.
		if(mon.moveState == Monster.MoveState.Forward && mon.lineRight < cm.monsterLeftLine && cm.checkPlayerMonsterBlockLine(mon) )
		{
			_tempF = mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime;

			if(_tempF >= StageManager.mapPlayerEndPosX)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("172");
			}
			else
			{
				moveForwardAndRotation(mon,false);
			}
		}
		else if(Xfloat.greatEqualThan( mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapPlayerEndPosX ))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("178");
		}
		else if(cm.checkPlayerMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);   ////ff Log.log("182");
		}
		else 
		{
			// 일단 전투존 안에 들어왔다.
			// 1. 타겟을 찾아본다.
			if(mon.target == null)
			{
				mon.checkWalkTime = 0.0f;
				mon.target = cm.getShortTargetCharacter(mon);
				if(mon.target != null)
				{
					mon.target.setTargetAngle(mon);	
					mon.attackPosition = Util.getPositionByAngleAndDistanceXZ(mon.targetAngle, mon.target.damageRange + mon.stat.atkRange + mon.damageRange);//mon.hitRange 
//										Log.logError("mon.targetAngle : " + mon.targetAngle + "mon.hitRange: " ,mon.hitRange, "mon.target.damageRange : " + mon.target.damageRange + " mon.attackPosition : "+ mon.attackPosition);
					if(mon.attackPosition.x > 0) mon.target = null;
					else
					{
						mon.target.attackers.Add(mon);								

						_hitObject = mon.getHitObject();
						
						if(Xfloat.lessEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z - (_hitObject.depth * 0.5f)).AsFloat() , MapManager.bottom * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.bottom * 0.8f) - mon.target.cTransformPosition.z + (_hitObject.depth * 0.5f);
						}
						else if(Xfloat.greatEqualThan( (mon.target.cTransformPosition.z + mon.attackPosition.z + (_hitObject.depth * 0.5f)).AsFloat() , MapManager.top * 0.8f ))
						{
							mon.attackPosition.z = (MapManager.top * 0.8f) - mon.target.cTransformPosition.z - (_hitObject.depth * 0.5f);
						}

					}
				}
			}
			
			//2. 타겟을 찾았으면.
			if(mon.target != null)
			{
				if(mon.target.isEnabled == false)
				{
					mon.removeTarget();
					return;
				}
				
				// 타겟을 향해 걸어가던지. 다 걸어갔으면 걔를 때려야한다.
				
				_v = mon.target.cTransformPosition + mon.attackPosition;
				
//				Log.log(this,VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition), mon.hitRange,mon.target.damageRange, mon.target.cTransformPosition , mon.cTransformPosition ); //ff
				
				if(Xfloat.lessEqualThan( VectorUtil.DistanceXZ(mon.target.cTransformPosition , mon.cTransformPosition) , mon.target.damageRange + mon.stat.atkRange + mon.damageRange ))// mon.hitRange
				{
					lookTargetAndAttack(mon, mon.target,true);
				}
				else if( _v.x <= mon.cTransformPosition.x && VectorUtil.Distance(_v.z, mon.cTransformPosition.z) < (IFloat)(mon.hitObject.depth * 1.2f ) ) // 5가 좋을까???
				{
					lookTargetAndAttack(mon, mon.target,true);
				}						
				else
				{
					// 자리를 못잡았으면 적의 위치를 계산해서 가져와야한다.
					// 적의 위치는 적을 때리고 있는 애들의 위치도 계산해서 가져온다.
					if(Xfloat.lessThan( cm.monsterLeftLine + cm.targetZoneDistance , mon.target.lineLeft )) mon.removeTarget();
					else
					{
						if(mon.moveState != Monster.MoveState.Forward)
						{
							if(mon.prevMoveState != Monster.MoveState.Forward)
							{
								mon.removeTarget();
								mon.setPlayAniRightNow(Monster.NORMAL);
								return;
							}
						}

						// 주인공쪽....
						if(mon.target.lineLeft < mon.lineRight)
						{
							mon.checkWalkTime += GameManager.globalDeltaTime;
							if(mon.checkWalkTime > 0.3f)
							{
								mon.removeTarget();
								return;
							}
						}
						else mon.checkWalkTime = 0.0f;
						
						_hitObject = mon.getHitObject();
						
						if(_hitObject.z <= MapManager.bottom)
						{
							_v = mon.cTransformPosition;
							_v.z += mon.stat.speed * GameManager.globalDeltaTime;
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}
						else if(_hitObject.distance >= MapManager.top)
						{
							_v = mon.cTransformPosition;
							_v.z -= mon.stat.speed * GameManager.globalDeltaTime;
							mon.setPosition(_v);
							mon.removeTarget();									
							return;
						}
						
						_v = mon.target.cTransformPosition + mon.attackPosition;
						
						_q = Util.getLookRotationQuaternion(_v - mon.cTransformPosition);
						mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
						
						_v = mon.cTransformPosition + (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime;
						_v.y = 0.0f;
						
						if(_v.x < mon.cTransformPosition.x)
						{
							_v.x = mon.cTransformPosition.x;
							//							mon.removeTarget();
							//							return;
						}
						else if(Xfloat.greaterThan( _v.x , mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f))
						{
							_v.x = mon.target.cTransformPosition.x + mon.attackPosition.x * 0.8f;
						}
						
						mon.setPlayAniRightNow(Monster.WALK);   //ff Log.log("271");
						mon.setPosition(_v);
						
					}
				}
			}	
		}
	}




	//==============================







	/*
	// 지뢰 타입.
	void moveLandMineAttackType(Monster mon)
	{
		//Debug.LogError("CHECK THIS!!!");
		// SHOULD CHECK THIS!!
		mon.action.delay += GameManager.globalDeltaTime;
		
		if(mon.action.delay > attr[3])
		{
			mon.dead();
		}
	}
	*/

	public void moveLandMineType(Monster mon)
	{
		if(mon.isPlayerSide) movePlayerSideLandMineType(mon, mon.isPlayer);
		else moveEnemyLandMineType(mon, mon.isPlayer);
	}



	// 이동을 한다... 기본 공격 거리내 적이 나올 때까지...
	void moveEnemyLandMineType(Monster mon, bool isPlayer = false)
	{
		// 전진 가능하다.
		if(mon.lineLeft > GameManager.me.characterManager.playerMonsterRightLine && cm.checkMonsterBlockLine(mon))
		{
			// 맵에 걸린다. 그럼 제자리.
			_tempF = mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime;

			bool isFastType = GameManager.me.stageManager.isPVPMode && mon.lineLeft > cm.pvpPlayerPosX ;


			if(_tempF < StageManager.mapStartPosX || _tempF < GameManager.me.characterManager.playerMonsterRightLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			// 전진할 수 있으면 전방을 바라보면서 이동한다.
			else
			{
				moveLineAndRotation(mon,true,mon.isPlayer,isFastType);
			}
		}
		// 3. 맵에 걸리면 제자리.
		else if(Xfloat.lessThan( mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapStartPosX ))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데.. 원거리 공격이 가능한 녀석들이라면 
		// 상대를 공격할 수 있으면 공격도 해야한다. 
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
	}
	
	
	
	void movePlayerSideLandMineType(Monster mon, bool isPlayer = false)
	{
		if(mon.lineRight < GameManager.me.characterManager.monsterLeftLine && cm.checkPlayerMonsterBlockLine(mon))
		{
			bool isFastType = ( mon.lineRight < cm.playerPosX  );

			_tempF = mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime * (isFastType ?2.0f:1.0f);
			
			if(_tempF > StageManager.mapPlayerEndPosX || _tempF > GameManager.me.characterManager.monsterLeftLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			else
			{
				moveLineAndRotation(mon,false,mon.isPlayer, isFastType);
			}
		}
		else if(Xfloat.greaterThan( mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapPlayerEndPosX ))
		{ 
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
	}	














	
	void moveStay(Monster mon)
	{

	}	
	
		
	
	// =========== 라인 공격 타입 ==========//
	
	public void moveLineType(Monster mon)
	{
		if(mon.isPlayerSide)
		{
			if(mon.isPlayer)
			{
				movePlayerPlayerSideLineType(mon);
			}
			else
			{
				movePlayerSideLineType(mon);
			}
		}
		else
		{
			if(mon.isPlayer)
			{
				movePlayerEnemyLineType(mon);
			}
			else
			{
				moveEnemyLineType(mon);
			}
			
		}
	}
	
	
	
	public void lookTargetAndAttack(Monster mon, Monster target, bool isShortType = false)
	{
		_q = Util.getLookRotationQuaternion(target.cTransformPosition - mon.cTransformPosition);
		// 자리를 다 잡았으면 공격...
		mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);

#if UNITY_EDITOR
//		 Log.log(mon.stat.uniqueId + "   " + mon.resourceId + "  mon.action.delay : " + mon.action.delay, "q: " + _q + "  tfq: " + mon.tf.rotation, Quaternion.Angle(_q, mon.tf.rotation) );
#endif

		// 공격 딜레이에 걸리거나 상대방을 덜 바라봤으면 일단 대기.
		if((mon.action.delay > 0 || Xfloat.greaterThan( Quaternion.Angle(_q, mon.tf.rotation) , 5) ))
		{
			if(mon.isDefaultAttacking == false)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);    //ff Log.log("325");
			}
		}
		// 모든게 완벽하면 공격 준비를 한다.
		else
		{
			if(isShortType == false)
			{
				mon.targetPosition = target.cTransformPosition;
				mon.targetHeight = target.hitObject.height;
				mon.targetUniqueId = target.stat.uniqueId;
			}
			
			mon.action.state = CharacterAction.STATE_ACTION;
		}
	}


	public void lookDirection(Monster mon, float direction = 100.0f)
	{
		_v = mon.cTransformPosition;
		_v.x += direction;
		mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, Util.getLookRotationQuaternion(_v - mon.cTransformPosition), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);	
	}
	
	public void moveLineAndRotation(Monster mon, bool isLeft, bool checkSkillMove = false, bool isFastType = false)
	{
		if(checkSkillMove)
		{
			if(string.IsNullOrEmpty(mon.nowAniId)) mon.state = Monster.WALK;
		}
		else mon.state = Monster.WALK;
		
		//		if(mon.prevPlayingAniName != Monster.WALK) mon.state = Monster.WALK;
		
		_v = mon.cTransformPosition;
		
		if(isLeft)
		{
			if(isFastType)
			{
				_v.x -= mon.stat.speed * GameManager.globalDeltaTime * AttackData.FAST_SPEED_VALUE;
			}
			else
			{
				_v.x -= mon.stat.speed * GameManager.globalDeltaTime;
			}


			mon.setPosition(_v);						
			_v.x -= 50.0f; 
		}
		else
		{
			if(isFastType)
			{
				_v.x += mon.stat.speed * GameManager.globalDeltaTime * AttackData.FAST_SPEED_VALUE;
			}
			else
			{
				_v.x += mon.stat.speed * GameManager.globalDeltaTime;
			}



			mon.setPosition(_v);						
			_v.x += 50.0f; 
		}
		
		mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, Util.getLookRotationQuaternion(_v - mon.cTransformPosition), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);	
	}
	
	
	public void moveForwardAndRotation(Monster mon, bool isLeft, bool isFastType = false)
	{
		//mon.playAni(Monster.WALK);
		mon.setPlayAniRightNow(Monster.WALK);   //ff Log.log("380");
		
		_v = mon.cTransformPosition;
		
		if(isLeft) _v.x -= 50.0f; 
		else _v.x += 50.0f; 
		
		mon.setPlayAniRightNow(Monster.WALK);   //ff Log.log("387");
		mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, Util.getLookRotationQuaternion(_v - mon.cTransformPosition), CharacterAction.rotationSpeed * GameManager.globalDeltaTime);

		if(isFastType)
		{
			_v = mon.cTransformPosition + (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime * 2f;
		}
		else
		{
			_v = mon.cTransformPosition + (IVector3)(mon.tf.forward) * mon.stat.speed * GameManager.globalDeltaTime;
		}

		_v.y = 0;
		mon.setPosition(_v);
	}



	// pvp 직선 캐릭터.
	void movePlayerEnemyLineType(Monster mon)
	{
		// 1. 타게팅 할 수 있는 녀석이 있나 살펴본다.
		//if(GameManager.me.player.nowChargingSkill != null || mon.nowPlayingSkillAni || mon.moveState == Monster.MoveState.Forward) target = null;
		if( mon.nowPlayingSkillAni ) target = null;
		else target = getLineTargetPlayerCharacter(mon);
		
//		Log.log(mon,mon.lineLeft,GameManager.me.characterManager.playerMonsterRightLine,"mon.cTransformPosition:",mon.cTransformPosition,"target:"+target); // ff 
		// 2. 타게팅 할 수 있는 녀석이 없다. 그리고 전진도 가능하다.
		if((target == null || mon.moveState == Monster.MoveState.Forward ) && mon.lineLeft  > GameManager.me.characterManager.playerMonsterRightLine && cm.checkMonsterBlockLine(mon) && mon.lineLeft > cm.playerLimitLine)
		{
			// 맵에 걸린다. 그럼 제자리.
			_tempF = mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime ;
			
			if(_tempF < StageManager.mapStartPosX || _tempF < GameManager.me.characterManager.playerMonsterRightLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			// 전진할 수 있으면 전방을 바라보면서 이동한다.
			else moveLineAndRotation(mon,true,mon.isPlayer);
		}
		// 3. 맵에 걸리면 제자리.
		else if(Xfloat.lessThan( mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapStartPosX ))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데.. 원거리 공격이 가능한 녀석들이라면 
		// 상대를 공격할 수 있으면 공격도 해야한다. 
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		
		target = null;
	}


	
	// 이동을 한다... 기본 공격 거리내 적이 나올 때까지...
	void moveEnemyLineType(Monster mon)
	{
		// 1. 타게팅 할 수 있는 녀석이 있나 살펴본다.
		target = getLineTargetPlayerCharacter(mon);

//		Log.log(mon,mon.lineLeft,GameManager.me.characterManager.playerMonsterRightLine,"mon.cTransformPosition:",mon.cTransformPosition,"target:"+target); //ff
		// 2. 타게팅 할 수 있는 녀석이 없다. 그리고 전진도 가능하다.
		if(target == null && mon.lineLeft > GameManager.me.characterManager.playerMonsterRightLine && cm.checkMonsterBlockLine(mon))
		{
			// 맵에 걸린다. 그럼 제자리.

			bool isFastType = (GameManager.me.stageManager.isPVPMode && mon.lineLeft > cm.pvpPlayerPosX);

			_tempF = mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime * (isFastType?FAST_SPEED_VALUE:1.0f);

			if(_tempF < StageManager.mapStartPosX || _tempF < GameManager.me.characterManager.playerMonsterRightLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			// 전진할 수 있으면 전방을 바라보면서 이동한다.
			else moveLineAndRotation(mon,true,mon.isPlayer,isFastType);
		}
		// 3. 맵에 걸리면 제자리.
		else if(Xfloat.lessThan( mon.lineLeft - mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapStartPosX ))
		{
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데.. 원거리 공격이 가능한 녀석들이라면 
		// 상대를 공격할 수 있으면 공격도 해야한다. 
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		
		target = null;
	}
	

	// 주인공. 카일리가 되겠다.
	void movePlayerPlayerSideLineType(Monster mon)
	{
		//if(GameManager.me.player.nowChargingSkill != null || mon.nowPlayingSkillAni || mon.moveState == Monster.MoveState.Forward) target = null;
		if(mon.nowPlayingSkillAni) target = null;
		else target = getLineTargetMonsterCharacter(mon);

		if((target == null || mon.moveState == Monster.MoveState.Forward ) &&  mon.lineRight  < GameManager.me.characterManager.monsterLeftLine && cm.checkPlayerMonsterBlockLine(mon) && mon.lineRight < cm.monsterLimitLine)
		{
			_tempF = mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime;
			
			if(_tempF > StageManager.mapEndPosX || _tempF > GameManager.me.characterManager.monsterLeftLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			else
			{
				moveLineAndRotation(mon,false,mon.isPlayer);
			}
		}
		else if(Xfloat.greaterThan( mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapEndPosX ))
		{ 
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데.. 원거리 공격이 가능한 녀석들이라면 
		// 상대를 공격할 수 있으면 공격도 해야한다. 
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		
		target = null;
	}	


	// 주인공의 직선 유닛들.

	void movePlayerSideLineType(Monster mon)
	{
		target = getLineTargetMonsterCharacter(mon);

		if(target == null && mon.lineRight < GameManager.me.characterManager.monsterLeftLine && cm.checkPlayerMonsterBlockLine(mon))
		{
			bool isFastType = ( mon.lineRight < cm.playerPosX  );

			_tempF = mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime * (isFastType ?AttackData.FAST_SPEED_VALUE:1.0f);

			if(_tempF > StageManager.mapPlayerEndPosX || _tempF > GameManager.me.characterManager.monsterLeftLine)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			else
			{
				moveLineAndRotation(mon,false,mon.isPlayer, isFastType);
			}
		}
		else if(Xfloat.greaterThan( mon.lineRight + mon.stat.speed * GameManager.globalDeltaTime , StageManager.mapPlayerEndPosX ))
		{ 
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 4. 오브젝트에 걸리면 제자리인데.. 원거리 공격이 가능한 녀석들이라면 
		// 상대를 공격할 수 있으면 공격도 해야한다. 
		else if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		
		target = null;
	}	
	












// 라인 이동타입인 카일리의 경우 제자리에 서있을때도 공격을 할 수 있는지 체크한다.============
	void checkStandingAttackForPlayerPlayerSideLineType(Monster mon)
	{
		if(mon.nowPlayingSkillAni) target = null;
		else target = getLineTargetMonsterCharacter(mon);
		
		if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		target = null;
	}	
	
	
	
	void checkStandingAttackForPlayerEnermyLineType(Monster mon)
	{
		if( mon.nowPlayingSkillAni ) target = null;
		else target = getLineTargetPlayerCharacter(mon);
		
		if(cm.checkMonsterBlockLine(mon) == false)
		{
			if(target != null) lookTargetAndAttack(mon,target);
			else mon.setPlayAniRightNow(Monster.NORMAL);
		}
		// 이동도 다 했고. 적도 있어. 그럼 그 녀석을 바라보고 공격을 한다.
		else if(target != null) lookTargetAndAttack(mon, target);
		else
		{
			mon.action.state = CharacterAction.STATE_PREPARE;
			mon.setPlayAniRightNow(Monster.NORMAL);
		}
		
		target = null;
	}
//=======================================================================














	
	
	
	
	
	
	Monster _cha;
	
	List<Monster> lineTargetList;
	Monster getLineTargetPlayerCharacter(Monster mon)
	{
		_cha = null;
		_tempX = 100000;
		
		lineTargetList = GameManager.me.characterManager.playerMonster;
		int len = lineTargetList.Count;
		
		for(int i = 0; i < len; ++i)
		{
			if(lineTargetList[i].isEnabled == false) continue;
			
			if(Xfloat.lessThan(  mon.lineLeft - mon.stat.atkRange - CharacterManager.lineOffset , lineTargetList[i].lineRight ))
			{
				_v = lineTargetList[i].cTransformPosition;
				_v.x = lineTargetList[i].lineRight.Get();
				_tempX2 = VectorUtil.DistanceXZ(_v, mon.cTransformPosition);
				
				if(_tempX2 < _tempX)
				{
					_tempX = _tempX2;
					_cha = lineTargetList[i];
				}
			}
		}
		
		lineTargetList = null;
		
		return _cha;
	}



	Monster getLineTargetMonsterCharacter(Monster mon)
	{
		_cha = null;
		_tempX = 100000;
		
		lineTargetList = GameManager.me.characterManager.monsters;
		int len = lineTargetList.Count;
		
		for(int i = 0; i < len; ++i)
		{
			if(lineTargetList[i].isEnabled == false) continue;
			
			if( Xfloat.greaterThan( mon.lineRight + mon.stat.atkRange + CharacterManager.lineOffset ,  lineTargetList[i].lineLeft ))
			{
				_v = lineTargetList[i].cTransformPosition;
				_v.x = lineTargetList[i].lineLeft.Get();
				_tempX2 = VectorUtil.DistanceXZ(_v, mon.cTransformPosition);
				
				if(_tempX2 < _tempX)
				{
					_tempX = _tempX2;
					_cha = lineTargetList[i];
				}
			}
		}			
		
		lineTargetList = null;
		return _cha;
	}	
	
	
	
	
	
	
	
	
	
	
	
	
	
	//=-=====================================================
	// 히어로 몬스터 스킬.
	//========================================================
	
	
	//=== 스킬을 사용하기 위해 움직이는.....
	
	// 수정이 필요한 부분이다.
	// 어차피 이걸 쓰는 애는 히어로 몬스터 밖에 없다.
	
	void moveSkillHeroMonsterLineType(Monster mon)
	{
		// 스킬 타게팅 가능한지 여부를 본다.
		// 애가 움직이는 영역까지 계산해서 타게팅이 가능하다고 봤으면...
		// 실제 걔한테 쓸 수 있는 곳 까지 움직인다.
		
		// 타게팅 검사에서 타게팅 대상을 찾았다면...
		// 실제 지금 위치에서 타게팅이 가능한지 본다.
		// 가능하지 않으면 가능하도록 움직인다.
		// 상대가 적이면 전방으로 이동해야한다.
		// 상대가 우리편이면 앞 뒤로 이동 할 수 있다.
		// 그런데 걷는 도중 타게팅 대상을 잃었으면 나가리다.
		
		// 스킬 상대가 없으면 취소다.
		if(mon.skillTarget == null || mon.skillTarget.isEnabled == false)
		{
			mon.action.clearSkillData();
			return;
		}
		
		// 스킬을 쓸 수 있는지 본다.
		// 현재 위치에서 스킬을 쓸 수 없으면 
		// 위치 이동을 해야한다.
		if(mon.skillTargetChecker(mon) == false) mon.skillMove(mon);
		else
		{
			// 스킬 시전 가능한 거리가 됐다.
			// 그럼 몸의 방향을 틀고
			// 스킬을 시전한다.
			_q = Util.getLookRotationQuaternion(mon.skillTarget.cTransformPosition - mon.cTransformPosition);
			mon.tf.rotation = Util.getFixedQuaternionSlerp(mon.tf.rotation, _q, CharacterAction.rotationSpeed * GameManager.globalDeltaTime);
			
			if((mon.action.delay > 0 || Xfloat.greaterThan( Quaternion.Angle(_q, mon.tf.rotation) , 5)) || mon.skillTarget.isEnabled == false)
			{
				mon.setPlayAniRightNow(Monster.NORMAL);
			}
			else
			{
				mon.targetPosition = mon.skillTarget.cTransformPosition;
				mon.targetHeight = mon.skillTarget.hitObject.height;
				mon.targetUniqueId = mon.skillTarget.stat.uniqueId;
				mon.action.state = CharacterAction.STATE_ACTION;
			}				
		}
	}
	
	
}