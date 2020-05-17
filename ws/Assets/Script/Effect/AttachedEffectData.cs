using System;
using UnityEngine;

sealed public class AttachedEffectData
{
	public string id;
	public int type;
	
	public bool attachToParent = true;
	
	public bool option = true;	
	
	public Vector3 pos = Vector3.zero;
	
	public float timeLimit = 10000.0f;

	public string parentName = null;

	public AttachedEffectData ()
	{
		pos.z = 0.1f;
	}
}

