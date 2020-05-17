using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class LobbyPositionData
{
	public Vector3 posVector = new Vector3();

	public int rMin, rMax, rMin2, rMax2;

	public LobbyPositionData ()
	{
	}
	
	public void setData(List<object> list, Dictionary<string, int> k)
	{
		Util.parseObject(list[k["X"]], out posVector.x,true, 0);
		Util.parseObject(list[k["Y"]], out posVector.y,true, 0);
		Util.parseObject(list[k["Z"]], out posVector.z,true, 0);

		Util.parseObject(list[k["R_MIN"]], out rMin, true, 0);
		Util.parseObject(list[k["R_MAX"]], out rMax, true, 0);

		Util.parseObject(list[k["R_MIN2"]], out rMin2, true, -1000);
		Util.parseObject(list[k["R_MAX2"]], out rMax2, true, -1000);
	}	

	public void setPosition(Transform tf)
	{
		tf.position = posVector;

		Quaternion q = tf.localRotation;
		float r = 0;

		if(rMin2 > -1)
		{
			r = UnityEngine.Random.Range(rMin,rMax+1);
			float r2 = UnityEngine.Random.Range(rMin2,rMax2+1);

			if(UnityEngine.Random.Range(0,10) < 5) r = r2;
		}
		else
		{
			r = UnityEngine.Random.Range(rMin,rMax+1);
		}

		Vector3 v = q.eulerAngles;
		v.y = r;
		q.eulerAngles = v;

		tf.localRotation = q;
	}

}

