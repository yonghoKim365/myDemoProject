using UnityEngine;
using System.Collections;
using System;

public class UIBattleStartAnimation : MonoBehaviour 
{
	public ParticleSystem psEffect;
	public Animation ani;

	public void onStartParticleEffect()
	{
		psEffect.playOnAwake = false;
		psEffect.gameObject.SetActive(true);
		psEffect.Play();
	}

	public void play()
	{
		this.gameObject.SetActive(true);
		ani.Play();
	}

	public void onCompleteEffect()
	{
		psEffect.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

}
