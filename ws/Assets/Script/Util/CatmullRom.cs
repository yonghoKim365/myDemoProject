/////////////////////////////
// Oh hyun sik
// 2010.04.28
// create bezier curve class by catmull-rom method
/////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRom : MonoBehaviour {
	public static List<Vector3> GetPoints(Vector3 p0,Vector3 p1,Vector3 p2,Vector3 p3,float ad){
		float count = 0f;
		float bx=0f,by=0f,bz=0f;
		
		//return value
		List<Vector3> rv = new List<Vector3>();

		//get point num, point to point;
		float p2pNum = Vector3.Distance(p1,p2);

		for(float i=0f;i<p2pNum;i+=0.1f){
			count = i/p2pNum;
			bx = p0.x*Curve0(count) + p1.x*Curve1(count) + p2.x*Curve2(count) + p3.x*Curve3(count);
			by = p0.y*Curve0(count) + p1.y*Curve1(count) + p2.y*Curve2(count) + p3.y*Curve3(count);
			if(ad<=0){
				rv.Add(new Vector3(bx,by,bz));
			}else{
				if(rv.Count == 0){
					rv.Add(new Vector3(bx,by,bz));
				}else if(Vector3.Distance(rv[rv.Count-1],new Vector3(bx,by,bz)) > ad){
					rv.Add(new Vector3(bx,by,bz));
				}
			}
		}
		return rv;
	}
		
		
	//get all point from ankerpoints
	//ankers : anker points Point class Array
	//ad : point array's distance in return value ( refault 0 is all point
	public static List<Vector3> GetAllPoints(List<Vector3> ankers, float ad){
		
		List<Vector3> rv = new List<Vector3>();
		if(ankers.Count < 4){
			return rv;
		}
		for(int i=0;i<ankers.Count-3;i++){
			if(i==0){
				rv.AddRange(GetPoints(ankers[i],ankers[i],ankers[i+1],ankers[i+2],ad));
			}
			rv.AddRange(GetPoints(ankers[i],ankers[i+1],ankers[i+2],ankers[i+3],ad));
			if(i==ankers.Count-4){
				rv.AddRange(GetPoints(ankers[i+1],ankers[i+2],ankers[i+3],ankers[i+3],ad));
			}
		}
		return rv;
	}
		
	static float Curve0(float t){
		return (-t + 2*t*t - t*t*t)/2;
	}
	static float Curve1(float t){
		return (2- 5*t*t + 3*t*t*t)/2;
	}
	static float Curve2(float t){
		return (t+ 4*t*t - 3*t*t*t)/2;
	}
	static float Curve3(float t){
		return (-t*t + t*t*t)/2;
	}
}