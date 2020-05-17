using UnityEngine;
using System.Collections;

public class RuneStudioEventReceiver : MonoBehaviour 
{
	public AudioSource source;

	void playSound(string fileName)
	{
		source.PlayOneShot ( Resources.Load("sounds/uinocomp/"+fileName) as AudioClip );
	}


	public void playSoundImpact()
	{
		//uirf_impact(수정) : 타이밍 변경 <WMV>
		#if UNITY_EDITOR
		Debug.Log("playSoundImpact");
		#endif

		Invoke("playImpactSound", 0.2f);
	}

	void playImpactSound()
	{
		if(GameManager.me != null)
		{
			SoundData.play("uirf_impact");
		}
		else
		{
			playSound("uirf_impact");
		}
	}


	public void startSoundCardMovement()
	{
		#if UNITY_EDITOR
		Debug.Log("startSoundCardMovement");
		#endif

//		if(GameManager.me != null) GameManager.me.uiManager.goBtnRuneStudioSkip.gameObject.SetActive(false);

		for(int i = 0; i < 5; ++i)
		{
			Invoke("playCardMovementSound", 0.5f + i*0.3f);
		}
	}

	void playCardMovementSound()
	{
		#if UNITY_EDITOR
		Debug.Log("playCardMovementSound");
		#endif

		if(GameManager.me != null)
		{
			SoundData.play("uirf_eat");
		}
		else
		{
			playSound("uirf_eat");
		}
	}


	public void playComposeCardSound()
	{
		//uirf_impact(수정) : 타이밍 변경 <WMV>
		#if UNITY_EDITOR
		Debug.Log("playDelayComposeCardSound");
		#endif
		
		Invoke("playDelayComposeCardSound", 0.58f);
	}
	
	void playDelayComposeCardSound()
	{
		if(GameManager.me != null)
		{
			SoundData.play("uicm_cards");
		}
		else
		{
			playSound("uicm_cards");
		}
	}


	public void onCompleteDCompose()
	{
#if UNITY_EDITOR
		Debug.Log("onCompleteDCompose");
#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showComposeResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}
	}


	public void onCompleteCCompose()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteCCompose");
#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showComposeResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}
		
	}

	public void onCompleteBCompose()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteBCompose");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showComposeResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}
		
	}

	public void onCompleteACompose()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteACompose");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showComposeResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
			SoundData.play("uicm_result2");
		}
		else
		{
			playSound("uicm_result2");
		}		
	}


	public void playSoundCardBreak()
	{
		#if UNITY_EDITOR
		Debug.Log("playSoundCardBreak");
		#endif

		Invoke("playBreakSound",1.0f);
	}

	void playBreakSound()
	{
		// uicp_break : A급이상 연출스타트시 <WMV>
		if(GameManager.me != null)
		{
			SoundData.play("uicp_break");
		}
		else
		{
			playSound("uicp_break");
		}

	}


	public void playSoundComposeASGoldEffectStart()
	{
		#if UNITY_EDITOR
		Debug.Log("playSoundComposeASGoldEffectStart");
		#endif
		
		// uicm_as : A급이상 연출들어갈 때 (뽑기에도 동일하게 적용) <WMV>
		if(GameManager.me != null)
		{
			SoundData.play("uigt_cast3");//uicm_as
		}
		else
		{
			playSound("uigt_cast3");
		}
	}


	public void onStartMakeRunePaperOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartMakeRunePaperOpen");
		#endif

		// uigt_cast1 : 스크롤 펼쳐질 때 <WMV>
		if(GameManager.me != null)
		{
			SoundData.play("uigt_cast1");
		}
		else
		{
			playSound("uigt_cast1");
		}

	}


	public void onStartMakeRuneCast2()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartMakeRuneCast2");
		#endif
		//uigt_cast2 : 번쩍할 때 <WMV>
		Invoke("playCast2", 1.0f);
	}

	void playCast2()
	{
		if(GameManager.me != null)
		{
			SoundData.play("uigt_cast2");
		}
		else
		{
			playSound("uigt_cast2");
		}

	}


	public void onCompleteMakeSOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeBOpen");
		#endif
		if(GameManager.me != null)
		{
			SoundData.play("uicm_result2");
		}
		else
		{
			playSound("uicm_result2");
		}
	}

	public void onCompleteMakeAOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeBOpen");
		#endif
		if(GameManager.me != null)
		{
			SoundData.play("uicm_result2");
		}
		else
		{
			playSound("uicm_result2");
		}

	}

	public void onCompleteMakeBOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeBOpen");
		#endif
		if(GameManager.me != null)
		{
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}

	}

	public void onCompleteMakeCOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeCOpen");
		#endif
		if(GameManager.me != null)
		{
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}
	}

	public void onCompleteMakeDOpen()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeDOpen");
		#endif
		if(GameManager.me != null)
		{
			SoundData.play("uicm_result1");
		}
		else
		{
			playSound("uicm_result1");
		}
	}


	public void onCompleteDMake()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteDMake");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}
	}


	public void onCompleteCMake()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteCMake");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}
	}

	public void onCompleteBMake()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteBMake");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}
	}

	public void onCompleteAMake()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteAMake");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}
	}

	public void onCompleteSMake()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteSMake");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}
	}



	public void onCompleteReinforce()
	{
#if UNITY_EDITOR
		Debug.Log("onCompleteReinforce");
#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showReinforceResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
		}
	}


	public void onCompleteTranscend()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteTranscend");
		#endif
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showTranscendResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
		}
	}





	public void onCompletePreMakeScene()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompletePreMakeScene");
		#endif

		if(GameManager.me != null)
		{
			StartCoroutine(onCompletePreMakeSceneCT());
			//RuneStudioMain.instance.nextMakeResult();
		}
	}

	IEnumerator onCompletePreMakeSceneCT()
	{
		yield return new WaitForSeconds(2f);
		RuneStudioMain.instance.nextMakeResult();
	}



	public void onCompleteShowRareScene()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteShowRareScene");
		#endif


		
		if(GameManager.me != null)
		{
			RuneStudioMain.instance.ShowNextButton();
		}

	}


	public void onCompleteDisplayMultipleCard()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteDisplayMultipleCard");
		#endif

		if(GameManager.me != null)
		{

		}
	}



	public void onCompleteMakeUnit()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteMakeUnit");
		#endif

		if(GameManager.me != null)
		{
			RuneStudioMain.instance.resetPlaySpeed();
		}
	}





	public void onStartEvolveSS()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveSS");
		#endif

		if(GameManager.me != null) SoundData.play("ui_evolve_cast");
		else playSound("ui_evolve_cast");
	}


	public void onStartEvolveS()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveS");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_cast");
		else playSound("ui_evolve_cast");
	}


	public void onStartEvolveA()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveA");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_cast");
		else playSound("ui_evolve_cast");
	}


	public void onStartEvolveB()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveB");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_cast");
		else playSound("ui_evolve_cast");
	}


	public void onStartEvolveC()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveC");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_cast");
		else playSound("ui_evolve_cast");
	}






	public void onStartEvolveShotSS()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveShotSS");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_shot");
		else playSound("ui_evolve_shot");
	}


	public void onStartEvolveShotS()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveShotS");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_shot");
		else playSound("ui_evolve_shot");
	}

	public void onStartEvolveShotA()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveShotA");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_shot");
		else playSound("ui_evolve_shot");
	}

	public void onStartEvolveShotB()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveShotB");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_shot");
		else playSound("ui_evolve_shot");
	}

	public void onStartEvolveShotC()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveShotC");
		#endif
		
		if(GameManager.me != null) SoundData.play("ui_evolve_shot");
		else playSound("ui_evolve_shot");
	}




	public void onStartEvolveResultCardAni()
	{
		#if UNITY_EDITOR
		Debug.Log("onStartEvolveResultCardAni");
		#endif

		if(GameManager.me != null)
		{
			RuneStudioMain.instance.showEvolveResultCard();
			RuneStudioMain.instance.resetPlaySpeed();
			SoundData.play("ui_evolve_complete");
		}
		else
		{
			playSound("ui_evolve_complete");
		}	
	}



	public void onCompleteEvolveResultCardAniSS()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteEvolveResultCardAniSS");
		#endif
		
		if(GameManager.me != null) 
		{
			RuneStudioMain.instance.iTweenOnCompleteEvolve();
		}
	}

	public void onCompleteEvolveResultCardAniS()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteEvolveResultCardAniS");
		#endif
		
		if(GameManager.me != null) 
		{
			RuneStudioMain.instance.iTweenOnCompleteEvolve();
		}
	}

	public void onCompleteEvolveResultCardAniA()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteEvolveResultCardAniA");
		#endif
		
		if(GameManager.me != null) 
		{
			RuneStudioMain.instance.iTweenOnCompleteEvolve();
		}
	}

	public void onCompleteEvolveResultCardAniB()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteEvolveResultCardAniB");
		#endif
		
		if(GameManager.me != null) 
		{
			RuneStudioMain.instance.iTweenOnCompleteEvolve();
		}
	}


	public void onCompleteEvolveResultCardAniC()
	{
		#if UNITY_EDITOR
		Debug.Log("onCompleteEvolveResultCardAniC");
		#endif
		
		if(GameManager.me != null) 
		{
			RuneStudioMain.instance.iTweenOnCompleteEvolve();
		}
	}







	// === 초월 & 제련 사운드 ==== //


	public float transcend3SoundDelay = 1.0f;

	public void onPlayTranscendSound(string soundId)
	{
		#if UNITY_EDITOR
		Debug.Log(soundId);
		#endif

		switch(soundId)
		{
		case "ui_transcend_03_4":
			if(GameManager.me != null) SoundData.play(soundId);
			else playSound(soundId);
			StartCoroutine(playDelaySoundCT(soundId, 0.5f));

			StartCoroutine(playDelaySoundCT("ui_transcend_05_6", transcend3SoundDelay));
			StartCoroutine(playDelaySoundCT("ui_transcend_05_6", transcend3SoundDelay+0.5f));

			break;
		default:
			if(GameManager.me != null) SoundData.play(soundId);
			else playSound(soundId);
			break;
		}
	}







	IEnumerator playDelaySoundCT(string id, float delay)
	{
		yield return new WaitForSeconds(delay);

		if(GameManager.me != null)
		{
			SoundData.play(id);
		}
		else
		{
			playSound(id);
		}
	}




}
