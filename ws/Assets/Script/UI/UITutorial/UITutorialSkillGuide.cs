using UnityEngine;
using System.Collections;

public class UITutorialSkillGuide : MonoBehaviour {

	public UISprite spHero, spSkill, spProgress;

	public Transform tfHand;

	public ParticleSystem psCharging;

	public void start()
	{
		gameObject.SetActive(true);
		init();
		StartCoroutine(play());
		StartCoroutine(changeSkillSprite());
	}


//	void OnEnable()
//	{
//		start();
//	}


	void init()
	{
		spHero.spriteName = "img_cha_ani0";
		spSkill.spriteName = "img_skill_ani0";
		spSkill.cachedTransform.transform.localPosition = new Vector3(-54.0f, -119, -220.0f); // to 214
		tfHand.transform.localPosition = new Vector3(158.0f, -273.0f, -220.0f);
		spSkill.enabled = false;
		spProgress.fillAmount = 0;
		tfHand.gameObject.SetActive(true);
		psCharging.Play();
	}

	IEnumerator play()
	{
		int i = 0;

		while(true)
		{
			init();

			float value = 0;
			while(true)
			{
				yield return new WaitForSeconds(0.05f);
				value += 0.05f;
				if(value >= 0.99f)
				{
					value = 1.0f;
					spProgress.fillAmount = 1.0f;
					break;
				}
				else
				{
					spProgress.fillAmount = value / 1.0f;
				}
			}

			yield return new WaitForSeconds(0.2f);
			tfHand.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.2f);
			spProgress.fillAmount = 0;
			yield return new WaitForSeconds(0.3f);

			spHero.spriteName = "img_cha_ani1";
			yield return new WaitForSeconds(0.08f);
			spHero.spriteName = "img_cha_ani2";

			yield return new WaitForSeconds(0.4f);
			spSkill.enabled = true;
			TweenPosition.Begin(spSkill.gameObject, 1.0f, new Vector3(214,-119,220));
			yield return new WaitForSeconds(2.0f);
		}
	}

	IEnumerator changeSkillSprite()
	{
		int index = 0;
		while(true)
		{
			yield return new WaitForSeconds(0.1f);

			{
				if(index > 2) index = 0;

				switch(index)
				{
				case 0:
					spSkill.spriteName = "img_skill_ani0";
					break;
				case 1:
					spSkill.spriteName = "img_skill_ani1";
					break;
				case 2:
					spSkill.spriteName = "img_skill_ani2";
					break;
				}

				++index;
			}
		}
	}


}
