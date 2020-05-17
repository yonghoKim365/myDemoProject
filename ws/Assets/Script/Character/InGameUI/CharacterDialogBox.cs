
using UnityEngine;
using System.Collections;

sealed public class CharacterDialogBox : MonoBehaviour 
{
	public UISprite spTooltip;
	public UILabel tfText;

	public float _leftTime = 0.0f;

	public enum Position
	{
		Left, Right
	}

	Vector3 _v;

	public void setData(bool isLeft, string txt, string imgId, float delay)
	{
		gameObject.SetActive(true);

		_leftTime = delay;
		tfText.text = Util.getText(txt);

		spTooltip.spriteName = imgId;

		if(isLeft)
		{
			_v.x = -440; _v.y = 23; _v.z = 0;
			spTooltip.transform.localPosition = _v;
			_v = tfText.transform.localPosition;
			_v.x = -295.3872f;
			tfText.transform.localPosition = _v;
		}
		else
		{
			_v.x = 415; _v.y = 23; _v.z = 0;
			spTooltip.transform.localPosition = _v;
			_v = tfText.transform.localPosition;
			_v.x = -455.3296f;
			tfText.transform.localPosition = _v;
		}
	}

	void LateUpdate()
	{
		_leftTime -= CutSceneManager.cutSceneDeltaTime;
		
		if(_leftTime <= 0.0f)
		{
			gameObject.SetActive(false);
		}
	}
}
