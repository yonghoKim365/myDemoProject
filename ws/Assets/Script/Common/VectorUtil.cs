using System;
using UnityEngine;

sealed public class VectorUtil
{
	public VectorUtil ()
	{
	}


	private static Vector3 _v;


	private const float FLOAT_HELPER = 100.0f;
	private const float FLOAT_HELPER2 = 0.01f;


	public static float Distance(float posX1, float posX2)
	{
		if(posX1 > posX2) return Mathf.FloorToInt(((posX1 * FLOAT_HELPER) - (posX2 * FLOAT_HELPER))*FLOAT_HELPER2);
		else return Mathf.FloorToInt(((posX2 * FLOAT_HELPER) - (posX1 * FLOAT_HELPER))*FLOAT_HELPER2);
	}	

	public static float Distance(float v1x, float v1y, float v2x, float v2y)
	{
		int dx = Mathf.FloorToInt(((v2x * FLOAT_HELPER) - (v1x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2y * FLOAT_HELPER) - (v1y * FLOAT_HELPER))*FLOAT_HELPER2);

		return Mathf.Sqrt( dx * dx + dy * dy );
	}	
	
	
	public static float Distance(Vector2 v1, Vector2 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		
		return Mathf.Sqrt( dx * dx + dy * dy );
	}

	public static float DistanceXZ(Vector3 v1, Vector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);

#if UNITY_EDITOR
		// Log.log("distanceXZ : " + v1, v2, dx, dz, Mathf.Sqrt( dx * dx + dz * dz ));
#endif
		return Mathf.Sqrt( dx * dx + dz * dz );
	}	
	

	
	public static float Distance3D(Vector3 v1, Vector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);

		return Mathf.Sqrt( dx * dx + dy * dy + dz * dz);
	}	
	
	
	
	public static Vector3 rotateVector3AroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return (angle * point) + pivot;
	}	


	public static Vector3 lerp(Vector3 from, Vector3 to, float time)
	{
		return from + (to - from) * time;
	}


	///========== Fixed ===============//



	public static IFloat Distance(IFloat posX1, float posX2)
	{
		if(posX1 > posX2) return Mathf.FloorToInt(((posX1 * FLOAT_HELPER) - (posX2 * FLOAT_HELPER))*FLOAT_HELPER2);
		else return Mathf.FloorToInt(((posX2 * FLOAT_HELPER) - (posX1 * FLOAT_HELPER))*FLOAT_HELPER2);
	}	


	public static IFloat Distance(float posX1, IFloat posX2)
	{
		if(posX1 > posX2) return Mathf.FloorToInt(((posX1 * FLOAT_HELPER) - (posX2 * FLOAT_HELPER))*FLOAT_HELPER2);
		else return Mathf.FloorToInt(((posX2 * FLOAT_HELPER) - (posX1 * FLOAT_HELPER))*FLOAT_HELPER2);
	}	




	public static IFloat Distance(IVector2 v1, Vector2 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		
		return Mathf.Sqrt( dx * dx + dy * dy );
	}
	
	public static IFloat DistanceXZ(IVector3 v1, Vector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);
		
		#if UNITY_EDITOR
		// Log.log("distanceXZ : " + v1, v2, dx, dz, Mathf.Sqrt( dx * dx + dz * dz ));
		#endif
		return Mathf.Sqrt( dx * dx + dz * dz );
	}	

	public static IFloat Distance(Vector2 v1, IVector2 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		
		return Mathf.Sqrt( dx * dx + dy * dy );
	}
	
	public static IFloat DistanceXZ(Vector3 v1, IVector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);
		
		#if UNITY_EDITOR
		// Log.log("distanceXZ : " + v1, v2, dx, dz, Mathf.Sqrt( dx * dx + dz * dz ));
		#endif
		return Mathf.Sqrt( dx * dx + dz * dz );
	}	





	public static IFloat Distance(IFloat posX1, IFloat posX2)
	{
		if(posX1 > posX2) return posX1  - posX2;
		else return posX2  - posX1;
	}	
	
	public static IFloat Distance(IFloat v1x, IFloat v1y, IFloat v2x, IFloat v2y)
	{
		int dx = Mathf.FloorToInt(v2x - v1x);
		int dy = Mathf.FloorToInt(v2y - v1y);
		
		return Mathf.Sqrt( dx * dx + dy * dy );
	}	
	
	
	public static IFloat Distance(IVector3 v1, IVector3 v2)
	{
		int dx = Mathf.FloorToInt(v2.x - v1.x);
		int dy = Mathf.FloorToInt(v2.y - v1.y);
		
		return Mathf.Sqrt( dx * dx + dy * dy );
	}
	
	public static IFloat DistanceXZ(IVector3 v1, IVector3 v2)
	{
		int dx = Mathf.FloorToInt(v2.x - v1.x);
		int dz = Mathf.FloorToInt(v2.z - v1.z);
		
		#if UNITY_EDITOR
		// Log.log("distanceXZ : " + v1, v2, dx, dz, Mathf.Sqrt( dx * dx + dz * dz ));
		#endif
		return Mathf.Sqrt( dx * dx + dz * dz );
	}	
	
	
	
	public static IFloat Distance3D(IVector3 v1, IVector3 v2)
	{
		int dx = Mathf.FloorToInt(v2.x - v1.x);
		int dy = Mathf.FloorToInt(v2.y - v1.y);
		int dz = Mathf.FloorToInt(v2.z - v1.z);
		
		return Mathf.Sqrt( dx * dx + dy * dy + dz * dz);
	}	
	


	public static IFloat Distance3D(IVector3 v1, Vector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);
		
		return Mathf.Sqrt( dx * dx + dy * dy + dz * dz);
	}	


	public static IFloat Distance3D(Vector3 v1, IVector3 v2)
	{
		int dx = Mathf.FloorToInt(((v2.x * FLOAT_HELPER) - (v1.x * FLOAT_HELPER))*FLOAT_HELPER2);
		int dy = Mathf.FloorToInt(((v2.y * FLOAT_HELPER) - (v1.y * FLOAT_HELPER))*FLOAT_HELPER2);
		int dz = Mathf.FloorToInt(((v2.z * FLOAT_HELPER) - (v1.z * FLOAT_HELPER))*FLOAT_HELPER2);
		
		return Mathf.Sqrt( dx * dx + dy * dy + dz * dz);
	}	



	
	public static IVector3 rotateVector3AroundPivot(IVector3 point, IVector3 pivot, Quaternion angle)
	{
		return (angle * point) + pivot;
	}	
	
	
	public static IVector3 lerp(IVector3 from, IVector3 to, float time)
	{
		return from + (to - from) * time;
	}


}

