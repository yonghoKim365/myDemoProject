using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class Monster : MonoBehaviour//, ITweenableObject
{

	public static IVector3 shootHandVector = IVector3.zero;
	public static IVector3 setShootingHand(Monster mon, float yOffset = 0.0f)
	{
		if(mon.hasShootingPos)
		{
			shootHandVector.x = mon.shootingPos[0];
			shootHandVector.y = mon.shootingPos[1];
			shootHandVector.z = mon.shootingPos[2];
			
			shootHandVector *= mon.monsterData.scale;
			
			shootHandVector = VectorUtil.rotateVector3AroundPivot(shootHandVector,mon.cTransformPosition,mon.tf.rotation);
			// Log.log(mon.name,"mon hand",_v);
		}
		else
		{
			shootHandVector = mon.cTransformPosition;	
			shootHandVector.y += yOffset;
			// Log.log(mon.name,"mon dont have hand:",mon.cTransformPosition);
		}
		
		return shootHandVector;
	}


	public static Vector3 setShootingHandWithCustomValue(Monster mon, float x, float y, float z)
	{
		shootHandVector.x = x;
		shootHandVector.y = y;
		shootHandVector.z = z;
		shootHandVector *= mon.monsterData.scale;
		shootHandVector = VectorUtil.rotateVector3AroundPivot(shootHandVector,mon.cTransformPosition,mon.tf.rotation);

		return shootHandVector;
	}


	public static Vector3 setShootingHandWithCustomValueWithoutMonsterPosition(Monster mon, float x, float y, float z)
	{
		shootHandVector.x = x;
		shootHandVector.y = y;
		shootHandVector.z = z;
		shootHandVector *= mon.monsterData.scale;
		shootHandVector = VectorUtil.rotateVector3AroundPivot(shootHandVector, IVector3.zero ,mon.tf.rotation);
		
		return shootHandVector;
	}



//============= 피격 각도 계산기 ===============//

	struct AngleCalc
	{
		public int getAngle;
		public int angleDiff;
		public int tempDiff;
		
		public AngleCalc(int angle, int diff)
		{
			getAngle = angle;
			angleDiff = 1000;
			tempDiff = 0;
		}
	}
	
	static readonly int[] mAngles =  {180, 110, 126, 154,   240, 224, 207};
	
	static readonly  int[] mUpAngles =  {180, 110, 126, 154};
	static readonly  int[] mDownAngles =  {190, 240, 224, 207};
	
	static readonly  int[] pAngles =  {0,26,70,54, 280,300,320};
	
	static readonly  int[] pUpAngles =  {0,26,70,54};
	static readonly  int[] pDownAngles =  {350,280,300,320};

	int[] _tempAngles;
	int[] _candiAngles = new int[7];	
	
	private static AngleCalc _angleCalc = new AngleCalc(0,0);
	
	private const int ANGLE_UP = 1;
	private const int ANGLE_DOWN = -1;
	private const int ANGLE_ALL = 0;
	private int _angleType = 0;
	private bool _isTargetAtLeft;
	
	public void setTargetAngle(Monster attacker)
	{
		attacker.targetAngle = -1;
		
		int nowAngle;
		int angleLen;
		
		_v = attacker.cTransformPosition;
		_v2 = cTransformPosition;
		
		_isTargetAtLeft = _v2.x < _v.x;
		nowAngle = Util.getAngleBetweenXZ(_v2, _v);
		
		_angleCalc.angleDiff = 1000;
		

		// 나 혹은 상대가 맵의 상위 80% 지점에 걸치면 얘는 아래쪽으로만 공격...
		if(hitObject.distance >= MapManager.top * 0.8f || attacker.hitObject.distance >= MapManager.top * 0.8f )
		{
			_angleType = ANGLE_DOWN;
			_tempAngles = (_isTargetAtLeft)?pDownAngles:mDownAngles;
			angleLen = 4;
		}
		// 나 혹은 상대가 맵의 하위 80% 지점에 걸치면 얘는 위쪽으로만 공격...
		else if(hitObject.z <= MapManager.bottom * 0.8f || attacker.hitObject.z <= MapManager.bottom * 0.8f)
		{
			_angleType = ANGLE_UP;
			_tempAngles = (_isTargetAtLeft)?pUpAngles:mUpAngles;
			angleLen = 4;
		}
		else
		{
			_angleType = ANGLE_ALL;
			_tempAngles = (_isTargetAtLeft)?pAngles:mAngles;
			angleLen = 7;
		}		
		
		
		int count = attackers.Count;
		
		int totalAngleNum = 0;
		
		bool isContinue = false;
		for(i = 0; i < angleLen; ++i)
		{
			for(int j = 0; j < count; ++j)
			{
				if(attackers[j].targetAngle == _tempAngles[i])
				{
					isContinue = true;
					break;	
				}
			}
			
			if(isContinue) { isContinue = false; continue; }
			_candiAngles[totalAngleNum] = _tempAngles[i];
			++totalAngleNum;
		}
		
		if(totalAngleNum > 0)
		{
			for(i = 0; i < totalAngleNum; ++i)
			{
				if(_isTargetAtLeft == false)
				{
					_angleCalc.tempDiff = MathUtil.abs(_candiAngles[i],nowAngle);
				}
				else
				{
					
					int temp1 = _candiAngles[i] - 90;
					int temp2 = nowAngle - 90;
					
					if(temp1 <= 0) temp1 = 360 + temp1;
					if(temp2 <= 0) temp2 = 360 + temp2;
					
					_angleCalc.tempDiff = MathUtil.abs(temp1,temp2);	
				}
				
				
				if(_angleCalc.tempDiff < _angleCalc.angleDiff)
				{
					_angleCalc.getAngle = _candiAngles[i];
					_angleCalc.angleDiff = _angleCalc.tempDiff;
				}
			}
			
			attacker.targetAngle = _angleCalc.getAngle;
		}
		
		
		if(attacker.targetAngle == -1)
		{
			if(_isTargetAtLeft)
			{
				if(_angleType == ANGLE_ALL)
				{
					attacker.targetAngle = GameManager.inGameRandom.Range(20,140);
					if(attacker.targetAngle > 70) attacker.targetAngle += 190;				
				}
				else if(_angleType == ANGLE_UP) attacker.targetAngle = GameManager.inGameRandom.Range(20,80);
				else attacker.targetAngle = GameManager.inGameRandom.Range(285,345);
			}
			else
			{
				if(_angleType == ANGLE_ALL) attacker.targetAngle = GameManager.inGameRandom.Range(110,250);					
				else if(_angleType == ANGLE_UP) attacker.targetAngle = GameManager.inGameRandom.Range(185,250);
				else attacker.targetAngle = GameManager.inGameRandom.Range(175,110);
			}
		}
	}	


// ====== 데미지 =====//

	// 반경을 그리는 충돌 영역 검사.
	public bool hitByDistance(IVector3 centerPoint, IFloat hitRadius)
	{
		// 반경에 damageRange를 더한 이유는 원 끝부분에 캐릭터가 걸쳐있을때도 맞게하기 위함이다.
		// 보면서 조정이 필요할 수도 있다.

#if UNITY_EDITOR
//		Debug.Log("centerPoint : " + centerPoint + " cTransformPosition : " + cTransformPosition + " hitRadius : " + hitRadius );
#endif

		return (VectorUtil.DistanceXZ(centerPoint , cTransformPosition) <= hitRadius + damageRange);// * 0.5f);
	}



	public static int sortByUISize(Monster x, Monster y)
	{
		return y.uiSize.CompareTo(x.uiSize);
	}

}
