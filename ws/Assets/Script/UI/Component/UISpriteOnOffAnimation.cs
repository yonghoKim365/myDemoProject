using UnityEngine;
using System.Collections;

public class UISpriteOnOffAnimation : MonoBehaviour {

	public float timeOffset = 0.5f;

	public UISprite target;

	private WaitForSeconds _waiter;

	void OnEnable()
	{
		if(target != null)
		{
			_waiter = new WaitForSeconds(0.5f);

			target.enabled = true;

			StartCoroutine(startAni());
		}
	}

	IEnumerator startAni()
	{
		while(true)
		{
			if(target.enabled)
			{
				yield return _waiter;
				yield return _waiter;
				yield return _waiter;
			}
			else
			{
				yield return _waiter;
			}

			target.enabled = !target.enabled;
		}
	}


	void OnDisable()
	{
		if(target != null)
		{
			target.enabled = true;
		}
	}


}
