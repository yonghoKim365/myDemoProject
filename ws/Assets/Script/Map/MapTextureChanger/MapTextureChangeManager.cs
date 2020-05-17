using UnityEngine;
using System.Collections;

public class MapTextureChangeManager : MonoBehaviour 
{
	public MapTextureChangeObject[] targetObjects;

	public ParticleSystem[] particles;

	// 효과를 유지할 시간.
	public float[] effectTime;
	public float[] secondEffectTime;
	public float[] thirdEffectTime;

	// 간격.
	public float[] duration = new float[]{3,5,4,2,5};

	// 파티클이 뿌려진후 매터리얼이 바뀔 시간.
	public float effectDelay = 0.0f;

	void Awake()
	{
		for(int i = particles.Length - 1; i >= 0; --i)
		{
			particles[i].playOnAwake = false;
			particles[i].Stop();
		}
	}



	public void start()
	{
		if(targetObjects == null) return;
		StartCoroutine(startEffect());
	}

	int _effectIndex = 0;
	private float getEffectTime()
	{
		float value = 0.0f;
		if(_effectIndex >= effectTime.Length) _effectIndex = 0;
		value = effectTime[_effectIndex];
		++_effectIndex;
		return value;
	}


	int _effectIndex2 = 0;
	private float getEffectTime2()
	{
		float value = 0.0f;
		if(_effectIndex2 >= secondEffectTime.Length) _effectIndex2 = 0;
		value = secondEffectTime[_effectIndex2];
		++_effectIndex2;
		return value;
	}


	int _effectIndex3 = 0;
	private float getEffectTime3()
	{
		float value = 0.0f;
		if(_effectIndex3 >= thirdEffectTime.Length) _effectIndex3 = 0;
		value = thirdEffectTime[_effectIndex3];
		++_effectIndex3;
		return value;
	}


	int _durationIndex = 0;
	private float getDuration()
	{
		float value = 0.0f;
		if(_durationIndex >= duration.Length) _durationIndex = 0;
		value = duration[_durationIndex];
		++_durationIndex;
		return value;
	}




	IEnumerator startEffect()
	{
		while(true)
		{
			playParticle();

			if(GameManager.me != null)
			{
				if(GameManager.me.uiManager.currentUI != UIManager.Status.UI_MENU )
				{
					SoundData.play("btl_thunder");
				}
			}

			yield return new WaitForSeconds(effectDelay);
			play();
			yield return new WaitForSeconds(getEffectTime());
			reset();

			float e2 = getEffectTime2();
			if(e2 > 0)
			{
				yield return new WaitForSeconds(e2);
				play();

				yield return new WaitForSeconds(getEffectTime3());
				reset();
			}

			yield return new WaitForSeconds(getDuration());
		}
	}

	public void playParticle()
	{
		for(int i = particles.Length - 1; i >= 0; --i)
		{
			particles[i].Play();
		}
	}


	public void play()
	{
		for(int i = targetObjects.Length - 1 ; i >= 0; --i)
		{
			targetObjects[i].setEffectTexture();
		}
	}


	public void reset()
	{
		if(targetObjects == null) return;
		for(int i = targetObjects.Length - 1 ; i >= 0; --i)
		{
			targetObjects[i].setOriginalTexture();
		}
	}


	void OnEnable()
	{
		if(GameManager.me != null && PerformanceManager.isLowPc == false)
		{
			start();
		}
	}


	void OnDisable()
	{
		reset();
	}


	void OnDestroy()
	{

	}


}
