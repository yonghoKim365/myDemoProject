using UnityEngine;
using System.Collections;

public class TutorialEndCircleEffect : MonoBehaviour 
{

	private float _delay = 3.0f;
	public void show()
	{
		gameObject.SetActive(true);

		_delay = 3.0f;

		StartCoroutine(autoHide());
	}

	public void hide()
	{
		gameObject.SetActive(false);
	}


	IEnumerator autoHide()
	{
		yield return new WaitForSeconds(3.0f);
		hide ();
	}


	void Update()
	{
		if(_delay > 0)
		{
			_delay -= Time.smoothDeltaTime;
			return;
		}

		hide();


	}

}
