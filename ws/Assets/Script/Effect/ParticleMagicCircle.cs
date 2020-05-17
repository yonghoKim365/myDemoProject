using UnityEngine;
using System.Collections;

sealed public class ParticleMagicCircle : MonoBehaviour 
{
	public ParticleSystem particle;
	public Transform tf;
	
	public ParticleSystem[] particles;
	public float[] startSize;
	public float[] startSpeed;
	public int particleCount;

	public tk2dSprite[] circleSprite;

	
	Vector3 _v;

	bool _isInit = false;

	public void start(Color color, float scale = 1.0f, bool useColor = true)
	{
		gameObject.SetActive(true);

		int i;

		if(circleSprite != null)
		{
			foreach(tk2dSprite sp in circleSprite)
			{
				sp.color = color;
				_v.x = scale; _v.y = scale; _v.z = scale;
				sp.scale = _v;
				sp.transform.localPosition = Vector3.zero;
			}
		}

		if(_isInit == false)
		{
			//tf = transform;
			//particles = tf.GetComponentsInChildren<ParticleSystem>();
			particleCount = particles.Length;
			startSize = new float[particleCount];
			startSpeed = new float[particleCount];
			for(i = 0; i < particleCount ; ++i)
			{
				startSize[i] = particles[i].startSize;
				startSpeed[i] = particles[i].startSpeed;
			}
			
			_isInit = true;
		}

		if(useColor) particle.startColor = color;

//		_v = tf.localScale;
//		_v.x = scale;
//		_v.y = scale;
//		_v.z = scale;
//		tf.localScale = _v;
		
		for(i = 0; i < particleCount; ++i)
		{
			if(useColor) particles[i].startColor = color;
			particles[i].startSize = startSize[i] * scale;
			particles[i].startSpeed = startSpeed[i] * scale;

			//particles[i].renderer.material.color = color;
		}
		
		particle.Play(false);
		
		for(i = 0; i < particleCount; ++i)
		{
			particles[i].Play(false);
		}
	}		

	public bool isEnabled
	{
		set
		{
			gameObject.SetActive(value);
			if(value == false)
			{
				particle.Stop(true);
				particle.Clear(true);
			}
		}
	}
}
