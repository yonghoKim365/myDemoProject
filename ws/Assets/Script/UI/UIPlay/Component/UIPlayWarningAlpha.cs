using UnityEngine;
using System.Collections;

public class UIPlayWarningAlpha : MonoBehaviour 
{
	public UISprite[] targets;
	float alpha = 0.0f;

	public void start()
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1) return;
		#endif

		gameObject.SetActive(true);

		alpha = 0.0f;
		setAlpha();
		isUp = true;
		StartCoroutine(startTweenAlpha());

		SoundData.playGroanSound( GameManager.me.player.resourceId , GameManager.me.player.playerData.characterId);

	}

	public void stop ()
	{
		gameObject.SetActive(false);
		SoundManager.instance.stopGroanVoice();
	}


	public float speed = 0.05f;

	WaitForSeconds ws005 = new WaitForSeconds(0.05f);
	bool isUp = false;

	IEnumerator startTweenAlpha()
	{
		while(true)
		{
			yield return ws005;

			setAlpha();

			if(isUp)
			{
				alpha += speed;
				if(alpha > 1.0f)
				{
					alpha = 1.0f;
					isUp = false;
				}
			}
			else
			{
				alpha -= speed;
				if(alpha < 0.0f)
				{
					alpha = 0.0f;
					isUp = true;
				}
			}
		}
	}

	Color color = new Color(1,1,1,1);

	void setAlpha()
	{
		for(int i = targets.Length - 1; i >= 0; --i)
		{
			color.a = alpha;
			targets[i].color = color; 
		}
	}

	void OnDisable()
	{
		alpha = 0;
		setAlpha();
	}
}
