using System;
using UnityEngine;

public class MathUtil
{
	public static IFloat Rad2Deg = IFloat.Create(234684,false);



	public MathUtil ()
	{
	}
	
	public static IFloat deg2Rad(IFloat inputValue)
	{
		return inputValue * IFloat.Create(71487,false) / 1000;;
	}



	public static IFloat Ceil (IFloat d)
	{
		d.Value = ((d.Value >> IFloat.SHIFT_AMOUNT) << IFloat.SHIFT_AMOUNT) + IFloat.OneF.Value;
		return d;
	}
	
	public static IFloat Floor (IFloat d)
	{
		d.Value = ((d.Value >> IFloat.SHIFT_AMOUNT) << IFloat.SHIFT_AMOUNT);
		return d;
	}
	
	public static IFloat Round (IFloat d)
	{
		d.Value = ((d.Value + (IFloat.HalfF.Value) ) >> IFloat.SHIFT_AMOUNT) << IFloat.SHIFT_AMOUNT;
		return d;
	}
	
	public static int CeilToInt (IFloat d)
	{
		return (int)Ceil (d);
	}
	
	public static int FloorToInt (IFloat d)
	{
		return (int)Floor (d);
	}
	
	public static int RoundToInt (IFloat d)
	{
		return (int)Round (d);
	}
	public static short RoundToShort (IFloat d)
	{
		return (short) Round (d);
	}

	public static IFloat Clamp (IFloat value, IFloat min, IFloat max)
	{
		if (value < min)
			value = min;
		else if (value > max)
			value = max;
		return value;
	}
	
	public static long Clamp (long value, long min, long max)
	{
		if (value < min)
			value = min;
		else if (value > max)
			value = max;
		return value;
	}
	
	public static IFloat Clamp01 (IFloat value)
	{
		if (value < 0)
			return 0;
		if (value > 1)
			return 1;
		else
			return value;
	}



	public static IFloat Repeat (IFloat t, IFloat length)
	{
		return t - Floor (t / length) * length;
	}






	public static IFloat pow(IFloat a, int b)
	{
		while(b > 1)
		{
			a *= a;
			--b;
		}

		return a;
	}

	public static IFloat pow(IFloat a, IFloat b)
	{
		while(b > 1)
		{
			a *= a;
			b -= 1;
		}
		
		return a;
	}



	public static IFloat min(IFloat a, int b)
	{
		return (a >= b) ? b : a;
	}


	public static IFloat max(IFloat a, int b)
	{
		return (a > b) ? a : b;
	}



	public static int abs(int a)
	{
		return (a < 0)?-a:a;
	}
	
	public static float abs(float a)
	{
		return (a < 0)?-a:a;
	}

	public static IFloat abs(IFloat a)
	{
		if(a < 0)
		{
			return -a;
		}

		return a;
	}

	public static Xfloat abs(Xfloat a)
	{
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}

	
	public static int abs(int a, int b)
	{
		a -= b;
		return (a < 0)?-a:a;
	}
	
	public static float abs(float a, float b)
	{
		a -= b;
		return (a < 0)?-a:a;
	}	


	public static IFloat abs(IFloat a, int b)
	{
		a -= b;
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}	

	public static IFloat abs(IFloat a, IFloat b)
	{
		a -= b;
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}	

	public static IFloat abs(IFloat a, Xfloat b)
	{
		a -= b;
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}	


	public static IFloat abs(Xfloat a, IFloat b)
	{
		a -= b;
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}	


	public static Xfloat abs(Xfloat a, Xfloat b)
	{
		a -= b;
		if(a < 0)
		{
			return -a;
		}
		
		return a;
	}	

	
	public static float[,] multiflyMatrix3X3(float[,] B, float[,] A)
	{
		int i,j,k;
		float[,] total = new float[3,3];
		
		 for(i=0;i<3;++i)
		 {
		  for(j=0;j<3;++j)
		  {
			 total[i,j]=0;
			   for(k=0;k<3;++k)
			   {
			    total[i,j]+=A[i,k]*B[k,j];
			   }
		  }
		 }
		
		return total;
	}






	/*	
	public static int getAngleBetween(Vector2 v1, Vector2 v2)
	{
		float dx = v1.x - v2.x;
		float dy = v1.y - v2.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	
	
	
	public static int getAngleBetween(Transform t1, Transform t2)
	{
		float dx = t1.transform.position.x - t2.transform.position.x;
		float dy = t1.transform.position.y - t2.transform.position.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}
	
	
	public static int getAngleBetweenXZ(Transform t1, Transform t2)
	{
		float dx = t1.transform.position.x - t2.transform.position.x;
		float dy = t1.transform.position.z - t2.transform.position.z;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	
	*/
	
	public static float getFloatAngleBetweenXY(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.y - t2.y;
		
		float ta = (Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}
	
	
	public static int getAngleBetweenXZ(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.z - t2.z;
		
		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}		



	public static int getFixedAngleBetweenXZ(IVector3 t1, IVector3 t2)
	{
		int ta = (int)(IFloat.Atan2(t1.z - t2.z, t1.x - t2.x) * MathUtil.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}



	
	public static float getFloatAngleBetweenXZ(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.z - t2.z;
		
		float ta = (float)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	
	
	
	
	
	/*
	public static int getAngleBetween(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.y - t2.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}
	*/
	
	
	
	private static Vector3 _v = Vector2.zero;
	public static Vector3 getPositionByAngleAndDistance(int angle, float dist)
	{
		_v.x = dist * GameManager.angleTable[angle].x;
		_v.y = dist * GameManager.angleTable[angle].y;
		_v.z = 0.0f;
		
		return _v;
	}
	
	public static IVector3 getFixedPositionByAngleAndDistance(int angle, IFloat dist)
	{
		IVector3 iv;
		iv.x = dist * GameManager.fixedAngleTable[angle].x;
		iv.y = dist * GameManager.fixedAngleTable[angle].y;
		iv.z = 0;
		
		return iv;
	}
	
	public static Vector3 getPositionByAngleAndDistanceWithoutTable(float angle, float dist)
	{
		_v.x = dist * Mathf.Cos(angle*Mathf.Deg2Rad);
		_v.y = dist * Mathf.Sin(angle*Mathf.Deg2Rad);
		_v.z = 0.0f;
		
		return _v;
	}
	
	
	public static Vector3 getPositionByAngleAndDistanceXZ(int angle, float dist)
	{
		_v.x = dist * GameManager.angleTable[angle].x;
		_v.y = 0.0f;
		_v.z = dist * GameManager.angleTable[angle].y;
		
		return _v;
	}
	
	
	public static IVector3 getFixedPositionByAngleAndDistanceXZ(int angle, IFloat dist)
	{
		IVector3 iv;
		iv.x = dist * GameManager.fixedAngleTable[angle].x;
		iv.y = 0;
		iv.z = dist * GameManager.fixedAngleTable[angle].y;
		
		return iv;
	}
	
	
	public static Vector3 getPositionByAngleAndDistanceXZWithoutTable(float angle, float dist)
	{
		_v.x = dist * Mathf.Cos(angle*Mathf.Deg2Rad);
		_v.z = dist * Mathf.Sin(angle*Mathf.Deg2Rad);
		_v.y = 0.0f;
		
		return _v;
	}










	public static IFloat getAngleDiff(IFloat a1, IFloat a2)
	{
		IFloat d = abs(a1 - a2) % 360;
		
		return d > 180 ? 360 - d : d;
	}



	public static IFloat lerpAngle(IFloat from, IFloat to, IFloat step)
	{
		IFloat num = MathUtil.Repeat (to - from, 360);
		
		if (num > 180)
		{
			num -= 360;
		}
		num = from + num * MathUtil.Clamp01 (step);
		
		if(num < 0)
		{
			while(num < 0)
			{
				num = 360 + num;
			}
		}
		else if(num >= 360)
		{
			while(num >= 360)
			{
				num -= 360;
			}
		}

		return num;

	}





	public Vector3 toEulerAngle(Matrix4x4  m) 
	{
		Vector3 v = new Vector3();
		
		float heading = 0;
		float attitude = 0;
		float bank = 0;
		
		// Assuming the angles are in radians.
		if (m.m10 > 0.998f) { // singularity at north pole
			heading = Mathf.Atan2(m.m02,m.m22);
			attitude = Mathf.PI/2;
			bank = 0;
		}
		else if (m.m10 < -0.998f) { // singularity at south pole
			heading = Mathf.Atan2(m.m02,m.m22);
			attitude = -Mathf.PI/2f;
			bank = 0;
		}
		else
		{
			heading = Mathf.Atan2(-m.m20,m.m00);
			bank = Mathf.Atan2(-m.m12,m.m11);
			attitude = Mathf.Asin(m.m10);
		}
		
		heading *= Mathf.Rad2Deg;
		attitude *= Mathf.Rad2Deg;
		bank *= Mathf.Rad2Deg;
		
		while(heading >= 360) heading -= 360;
		while(heading < 0) heading += 360;
		
		while(bank >= 360) bank -= 360;
		while(bank < 0) bank += 360;
		
		while(attitude >= 360) attitude -= 360;
		while(attitude < 0) attitude += 360;
		
		return new Vector3(bank, heading, attitude);
	}
	
	
	
	
	public static Vector3 toEulerAngle(Quaternion q1) 
	{
		float sqw = q1.w*q1.w;
		float sqx = q1.x*q1.x;
		float sqy = q1.y*q1.y;
		float sqz = q1.z*q1.z;
		float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
		float test = q1.x*q1.y + q1.z*q1.w;
		
		float heading = 0;
		float attitude = 0;
		float bank = 0;
		
		if (test > 0.499f*unit) 
		{ 
			heading = 2 * Mathf.Atan2(q1.x,q1.w);
			attitude = Mathf.PI/2;
			bank = 0;
		}
		else if (test < -0.499f*unit) 
		{ 
			heading = -2 * Mathf.Atan2(q1.x,q1.w);
			attitude = -Mathf.PI/2;
			bank = 0;
		}
		else
		{
			heading = Mathf.Atan2(2*q1.y*q1.w-2*q1.x*q1.z , sqx - sqy - sqz + sqw);
			attitude = Mathf.Asin(2*test/unit);
			bank = Mathf.Atan2(2*q1.x*q1.w-2*q1.y*q1.z , -sqx + sqy - sqz + sqw);
		}
		
		while(heading >= 360) heading -= 360;
		while(heading < 0) heading += 360;
		
		while(bank >= 360) bank -= 360;
		while(bank < 0) bank += 360;
		
		while(attitude >= 360) attitude -= 360;
		while(attitude < 0) attitude += 360;
		
		return new Vector3(bank, heading, attitude);
	}
	
	
	
	public static Quaternion MatrixToRotation(Matrix4x4 m)
	{
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
		q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
		q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
		q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
		
		q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
		q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
		q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
		return q;
	}
	
	
	public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget)
	{
		// http://msdn.microsoft.com/en-us/library/bb205343(v=VS.85).aspx
		
		Vector3 vz = Vector3.Normalize(cameraPosition - cameraTarget);
		Vector3 vx = Vector3.Normalize(Vector3.Cross(Vector3.up, vz));
		Vector3 vy = Vector3.Cross(vz, vx);
		Matrix4x4 result = new Matrix4x4();
		
		result.m00 = vx.x;
		result.m01 = vy.x;
		result.m02 = vz.x;
		
		result.m10 = vx.y;
		result.m11 = vy.y;
		result.m12 = vz.y;
		
		result.m20 = vx.z;
		result.m21 = vy.z;
		result.m22 = vz.z;
		
		result.m30 = -Vector3.Dot(vx, cameraPosition);
		result.m31 = -Vector3.Dot(vy, cameraPosition);
		result.m32 = -Vector3.Dot(vz, cameraPosition);
		
		return result;
	}



	public Matrix4x4 CreateFromQuaternion(Quaternion quaternion)
	{
		Matrix4x4 result = new Matrix4x4();
		
		result.m00 = 1 - 2 * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);
		result.m01 = 2 * (quaternion.x * quaternion.y + quaternion.w * quaternion.z);
		result.m02 = 2 * (quaternion.x * quaternion.z - quaternion.w * quaternion.y);
		result.m11 = 2 * (quaternion.x* quaternion.y - quaternion.w * quaternion.z);
		result.m12 = 1 - 2 * (quaternion.x * quaternion.x + quaternion.z * quaternion.z);
		result.m13 = 2 * (quaternion.y * quaternion.z + quaternion.w * quaternion.x);
		result.m20 = 2 * (quaternion.x * quaternion.z + quaternion.w * quaternion.y);
		result.m21 = 2 * (quaternion.y * quaternion.z - quaternion.w * quaternion.x);
		result.m22 = 1 - 2 * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);
		
		return result;
	}








	public static int getDamage(float inputAtk, float def, float inputMagicAtk = 0.0f, float magicDef = 0.0f, float damagePer = 1.0f, float minimumDamagePer = 1.0f, float damagePer2 = 1.0f, float inputTempDiscountDamageValue = 1.0f)
		// damagePer2는 공격동작중 한번에 여러번 타격판정이 들어가는 경우에 사용한다.
	{
		//#if UNITY_EDITOR
		//		Log.log("atk:",atk,"def:",def,"magicAtk:",magicAtk,"magicDef:",magicDef,"damagePer:",damagePer,"minimumDamagePer:",minimumDamagePer,"damagePer2:",damagePer2);
		//#endif
		IFloat atk = inputAtk;
		IFloat magicAtk = inputMagicAtk;
		IFloat tempDiscountDamageValue = inputTempDiscountDamageValue;
		
		damagePer = (Mathf.Round(damagePer * 100.0f))/100.0f;
		
		//데미지 = 물리공격력 * (1/SQRT(물리방어력)) * A + 마법공격력 * (1/SQRT(마법방어력)) * B
		//* SQRT : 제곱근, A : 데미지보정계수 (5), B : 데미지보정계수 (5)
		
		if(Xfloat.lessEqualThan( def , 0 ) )
		{
			def = 1; 
		}
		
		if(Xfloat.lessEqualThan( magicDef , 0 ) )
		{
			magicDef = 1;
		}
		
		atk = (atk * (1.0f/Mathf.Sqrt(def))) * 5.0f + (magicAtk * (1.0f/Mathf.Sqrt(magicDef)) * 5.0f);
		
		// damagePer // 기본 데미지의 몇 %.
		// minimumDamagePer // 데미지의 최소 비율.
		if(Xfloat.lessThan( damagePer , 1.0f ) )
		{
			atk = atk * minimumDamagePer + ( (atk - (atk * minimumDamagePer)) * damagePer);
		}
		
		// 얘는 한공격 동작에 데미지가 여러번 들어가는 경우 그 횟수로 나누어줘서 밸런스를 맞춰주기 위함이다.
		atk *= damagePer2; 
		
		
		// 이런 코드를 만들고 싶지 않았으나... 공격타입 5번과 15번은 단계마다 특정비율로 데미지를 깍는다. 
		// 거치는 곳이 많기 때문에 어쩔 수 없이 예외를 만들어 데미지를 깍아준다.
		atk *= tempDiscountDamageValue;
		
		int gValue = Mathf.RoundToInt( atk * 100.0f );
		
		if(GameManager.inGameRandom.Range(0,100) < gValue % 100)
		{
			return (gValue/100 + 1);
		}
		else
		{
			return gValue/100;
		}
	}

	/*
	public static int getDamage( bool isDefaultAtk, MonsterPropertyInfo atkPInfo, Monster target, IFloat atk, IFloat def, IFloat magicAtk, IFloat magicDef, IFloat damagePer, IFloat minimumDamagePer, IFloat damagePer2, IFloat tempDiscountDamageValue, out WSDefine.HPDamageType damageType)
	// damagePer2는 공격동작중 한번에 여러번 타격판정이 들어가는 경우에 사용한다.
	{
//#if UNITY_EDITOR
//		Log.log("atk:",atk,"def:",def,"magicAtk:",magicAtk,"magicDef:",magicDef,"damagePer:",damagePer,"minimumDamagePer:",minimumDamagePer,"damagePer2:",damagePer2);
//#endif
//		damagePer = (Mathf.Round(damagePer * 100.0f))/100.0f;

		//데미지 = 물리공격력 * (1/SQRT(물리방어력)) * A + 마법공격력 * (1/SQRT(마법방어력)) * B
		//* SQRT : 제곱근, A : 데미지보정계수 (5), B : 데미지보정계수 (5)

		float propertyDamageRatio = 1;
		IFloat tempDamageRatio = 0;

		if( def <= 0  )
		{
			def = 1; 
		}

		if( magicDef <= 0  )
		{
			magicDef = 1;
		}

		atk = (atk * (1/ IFloat.Sqrt(def))) * 5 + (magicAtk * (1/ IFloat.Sqrt(magicDef)) * 5);
		
		// damagePer // 기본 데미지의 몇 %.
		// minimumDamagePer // 데미지의 최소 비율.
		if( damagePer < 1 )
		{
			atk = atk * minimumDamagePer + ( (atk - (atk * minimumDamagePer)) * damagePer);
		}

		// 얘는 한공격 동작에 데미지가 여러번 들어가는 경우 그 횟수로 나누어줘서 밸런스를 맞춰주기 위함이다.
		atk *= damagePer2; 


		// 이런 코드를 만들고 싶지 않았으나... 공격타입 5번과 15번은 단계마다 특정비율로 데미지를 깍는다. 
		// 거치는 곳이 많기 때문에 어쩔 수 없이 예외를 만들어 데미지를 깍아준다.
		atk *= tempDiscountDamageValue;


		if(target != null)
		{

			int defProperty = RunePropertyData.NONE_INDEX;
			
			// > 유닛 및 히어로 기본 공격일때, 내 공격력과 상대 방어력으로 계산된 데미지 수치에 위 테이블의 보정 수치들을 적용													
			if(target.isPlayer)
			{
				//물리공격일때, 히어로가 장착한 의상 장비의 속성						
				if(atk > magicAtk) // 물리공격.
				{
					defProperty = ((Player)target).playerData.partsBody.parts.property;
				}
				//마법공격일때, 히어로가 장착한 모자 장비의 속성						
				else
				{
					defProperty = ((Player)target).playerData.partsHead.parts.property ;
				}
			}
			else
			{
				defProperty = target.stat.property ;
			}

			if(isDefaultAtk )
			{
				atk *= GameManager.info.propertySetup[atkPInfo.property].data[defProperty ];
			}

			propertyDamageRatio = GameManager.info.propertySetup[atkPInfo.property].data[defProperty ];
			
			if(atkPInfo.logicData != null) // 공격데미지 속성 적용.
			{
				tempDamageRatio = (IFloat.n1 + atkPInfo.logicData.getDamageAtkValue(atkPInfo.property));
				atk *= tempDamageRatio;
//				propertyDamageRatio *= tempDamageRatio;
			}
			
			if(target.stat.propertyData != null) // 피격데미지 속성 적용.
			{
				tempDamageRatio = (IFloat.n1 + target.stat.propertyData.getDamageDefValue(defProperty));
				atk *= tempDamageRatio;
//				propertyDamageRatio *= tempDamageRatio;
			}

			if(target.isPlayerSide == false) // 내가 적에게.
			{
				atk *= StageManager.playerAtkRatio;
			}
			else // 적이 나에게 
			{
				atk *= StageManager.enermyAtkRatio;
			}


			// 데미지 증가 어빌리티 -> 공격자의 어빌리티와 피격자의 속성 검사.
			if(atkPInfo.ability != null)
			{
				atk = atkPInfo.getDamageUpAbility(atk, target.stat.property);
			}

			// 데미지 감소 어빌리티 -> 피격자의 어빌리티와 공격자의 속성 검사.
			if(target.stat.ability != null)
			{
				atk = target.stat.getDamageDownAbility(atk, atkPInfo.property);
			}
		}


		if(propertyDamageRatio > 1.001f)
		{
			damageType = WSDefine.HPDamageType.HighDamage;
		}
		else if(propertyDamageRatio < 0.999f)
		{
			damageType = WSDefine.HPDamageType.LowDamage;
		}
		else
		{
			damageType = WSDefine.HPDamageType.Damage;
		}



		//====================//

		int gValue = MathUtil.RoundToInt( atk * 100 );

		if(GameManager.inGameRandom.Range(0,100) < gValue % 100)
		{
			return (gValue/100 + 1);
		}
		else
		{
			return gValue/100;
		}
	}
	*/
	

	public static int levelDifferencePoint(int level, int[] table)
	{
		if(level <= 2) return table[0];
		else if(level <= 5) return table[1];
		else if(level <= 8) return table[2];
		else if(level <= 12) return table[3];
		else if(level <= 18) return table[4];
		else if(level <= 25) return table[5];
		else return table[6];
	}


	private static System.Random _random;
	public static void setFixedRandom(int seed)
	{
		_random = new System.Random(seed);
	}

	// int random은 max 값을 포함하지 않는다.
	// 고로 편의상 미만이 아닌 이하로 만들기위해 max+1을 했다.
	public static int getFixedRandomNum(int min, int max)
	{
		return _random.Next(min, max+1);
	}

	public static int getGoldToRuby(int gold)
	{
		// 250골드는 1루비.
		// 200골드라서 0.8 루비가 필요해도 구입할때는 무조건 반올림 한다. 1루비로.
		return 1;
	}


}




public class PseudoRandom
{
	private int m_Val = 0;
	public PseudoRandom(){}
	public PseudoRandom(int aSeed)
	{
		m_Val = aSeed;
	}
	private int Next()
	{
		m_Val = m_Val * 0x08088405 + 1;
		return m_Val;
	}
	public int Range(int aMin, int aMax)
	{
		return aMin + Next() % (aMax-aMin);
	}
}






//public static IFloat Sqrt (IFloat ret)
//{
//	if(ret.Value > 0)
//	{
//		if(ret.Value >= int.MaxValue)
//		{
//			long n = (ret.Value >> 1) + 1;
//			long n1 = (n + (ret.Value / n)) >> 1;
//			while (n1 < n) {
//				n = n1;  
//				n1 = (n + (ret.Value / n)) >> 1;  
//			}
//			
//			n = n1;
//			
//			n = n << (HalfShift);
//			ret.Value = n;
//		}
//		else
//		{
//			int v = (int)ret.Value;
//			
//			int n = (v >> 1) + 1;
//			int n1 = (n + (v / n)) >> 1;
//			while (n1 < n) {
//				n = n1;  
//				n1 = (n + (v / n)) >> 1;  
//			}
//			
//			n = n1;
//			
//			n = n << (HalfShift);
//			ret.Value = n;
//		}
//	}
//	
//	return ret;
//}
