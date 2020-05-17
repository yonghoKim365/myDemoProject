using UnityEngine;
using System.Collections;

public class ParticleQueueResetter : MonoBehaviour {

	public int renderQueue = 3005;

	public Renderer renderer;

	public Material mat;


	void Awake ()
	{
		setQueue();
	}

	void OnEnable ()
	{
		setQueue();
//		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
//		
//		if(ps != null)
//		{
//			Debug.LogError(ps.Length);
//			
//			foreach(ParticleSystem p in ps)
//			{
//				p.renderer.material.renderQueue = renderQueue;
//			}
//		}
	}


	public void setQueue()
	{

	}




	/*
	public void setQueue()
	{
		//ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		Renderer[] ps = GetComponentsInChildren<Renderer>();
		
		if(ps != null)
		{
			Debug.LogError(ps.Length);
			
			foreach(Renderer p in ps)
			{
				p.material.renderQueue = renderQueue;
			}
			
			//			foreach(ParticleSystem p in ps)
			//			{
			//				p.renderer.material.renderQueue = renderQueue;
			//			}
		}

	}
	*/


}
