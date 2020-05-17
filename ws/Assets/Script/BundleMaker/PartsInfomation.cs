using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartsInfomation : MonoBehaviour {

	public List<string> bones = new List<string>();
	
	
	public void setInfo(SkinnedMeshRenderer smr)
	{
		foreach(Transform tn in smr.bones)
		{
			bones.Add(tn.name);
		}
	}
}



