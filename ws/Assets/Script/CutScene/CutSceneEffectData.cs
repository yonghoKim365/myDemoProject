using System;
using UnityEngine;

public class CutSceneEffectData
{
	public EffectData effectData;
	public bool canClone = false;
	public string id;
	
	public GameObject gameObject;
	public ParticleEffect particleEffect;
	
	public GameObject clone()
	{
		return effectData.getEffect(-1000,Vector3.zero);  //GameManager.me.cutSceneManager.addMonsterToStage(isPlayerMon, type, id);
	}
	
}

