using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ParticleResizeUtil : MonoBehaviour {

	public float scale = 1.0f;

	public ParticleSystem[] particles;
	public float[] startSize;
	public float[] startSpeed;
	public int particleCount;

	public GameObject root;

	public void init()
	{
		particles = root.GetComponentsInChildren<ParticleSystem>();
		
		if(particles != null)
		{
			particleCount = particles.Length;
			
			startSize = new float[particleCount];
			startSpeed = new float[particleCount];
			
			for(int i = 0; i < particleCount ; ++i)
			{
				startSize[i] = particles[i].startSize;
				startSpeed[i] = particles[i].startSpeed;
			}
		}

	}

	public void resize()
	{
		for(int i = 0; i < particleCount; ++i)
		{
			particles[i].startSize = startSize[i] * scale;
			particles[i].startSpeed = startSpeed[i] * scale;
		}
		
		for(int i = 0; i < particleCount; ++i)
		{
			particles[i].Play(false);
		}
	}
}



public class ParticleResizeUtil2
{

	public static void resize(GameObject source, float scale)
	{
		if(source == null)
		{
			return;
		}

		GameObject root = (GameObject)GameObject.Instantiate(source);

		float[] startSize = null;
		float[] startSpeed = null;
		int particleCount = 0;

		ParticleSystem[] particles = root.GetComponentsInChildren<ParticleSystem>();
		
		if(particles != null)
		{
			particleCount = particles.Length;
			
			startSize = new float[particleCount];
			startSpeed = new float[particleCount];
			
			for(int i = 0; i < particleCount ; ++i)
			{
				startSize[i] = particles[i].startSize;
				startSpeed[i] = particles[i].startSpeed;
			}
		}

		for(int i = 0; i < particleCount; ++i)
		{
			particles[i].startSize = startSize[i] * scale;
			particles[i].startSpeed = startSpeed[i] * scale;
		}
	}


}
