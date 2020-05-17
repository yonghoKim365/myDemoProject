using UnityEngine;
using System.Collections;

public class ParticleCullingUtil : MonoBehaviour {


	void Awake()
	{
		gameObject.GetComponent<ParticleSystemRenderer>().mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
	}


	void OnBecameVisible()
	{
		Debug.Log("OnBecameVisible : " + renderer);
	}

	void OnBecameInvisible()
	{
		Debug.Log("OnBecameInvisible : " + renderer);
	}


}
