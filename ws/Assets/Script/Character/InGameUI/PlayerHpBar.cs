using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerHpBar : MonoBehaviour {

	public UISprite spHp;
	public UISprite spDelayHp;
	public UILabel lbHp;

	public Animation ani;

	private static StringBuilder _sb = new StringBuilder();

	public GameObject cachedGameObject;

	void Awake()
	{
		if(cachedGameObject == null) cachedGameObject = gameObject;
	}


	public void updateHP(float hpPer, float hp, float maxHp, bool useAniEffect = true)
	{
		_sb.Length = 0;
		_sb.Append((int)hp);
		_sb.Append("/");
		_sb.Append((int)maxHp);
		lbHp.text = _sb.ToString();

		if(cachedGameObject == null || cachedGameObject.activeInHierarchy == false)
		{
			spHp.fillAmount = hpPer;			
			spDelayHp.fillAmount = hpPer;
			return;
		}



		if(useAniEffect)
		{
			if(hpPer < 0.999f)
			{
				++_playIndex;

				if(GameManager.me.recordMode == GameManager.RecordMode.continueGame)
				{
					return;
				}

				StartCoroutine(hpBarEffect(_playIndex, hpPer));
				StartCoroutine(hpBarEffect2(_playIndex, hpPer));
				ani.Rewind();
				ani.Play();
			}
			else
			{
				_playIndex = 0;
				spHp.fillAmount = hpPer;			
				spDelayHp.fillAmount = hpPer;
			}
		}
		else
		{
			++_playIndex;
			spHp.fillAmount = hpPer;			
			spDelayHp.fillAmount = hpPer;
		}
	}

	private int _playIndex = 0;

	const float ANIMATION_DELAY = 0.02f;

	private static WaitForSeconds _wsDelay = new WaitForSeconds(ANIMATION_DELAY);
	private static WaitForSeconds _wsDelay2 = new WaitForSeconds(0.3f);

	IEnumerator hpBarEffect(int playIndex, float targetValue)
	{
		float hp = spHp.fillAmount;
		float speed = (hp - targetValue) / (ani.clip.length / ANIMATION_DELAY);

		while(true)
		{
			if(playIndex != _playIndex) break;
			yield return _wsDelay;
			hp -= speed;

			speed *= 1.05f;

			if(hp <= targetValue)
			{
				spHp.fillAmount = targetValue;
				break;
			}
			else
			{
				spHp.fillAmount = hp;
			}
		}
	}


	IEnumerator hpBarEffect2(int playIndex, float targetValue)
	{
		yield return _wsDelay2;

		float hp = spDelayHp.fillAmount;
		float speed = (hp - targetValue) / (ani.clip.length / ANIMATION_DELAY);

		while(true)
		{
			if(playIndex != _playIndex) break;
			yield return _wsDelay;

			hp -= speed;

			speed *= 1.1f;

			if(hp <= targetValue)
			{
				spDelayHp.fillAmount = targetValue;
				break;
			}
			else
			{
				spDelayHp.fillAmount = hp;
			}
		}
	}


}
