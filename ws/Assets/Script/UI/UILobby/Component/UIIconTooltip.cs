using UnityEngine;
using System.Collections;

public class UIIconTooltip : MonoBehaviour {

	public UIPanel panel;
	public UILabel lbText;
	public UISprite spBg;

	float _delay = 0.0f;
	public void start(string text, float posX, float posY, float showTime = 2.0f, bool setPanelPosition = false, bool useLabelAutoResize = true, float printOffset = 40.0f)
	{
		panel.cachedGameObject.SetActive(true);
		panel.alpha = 1.0f;
		_delay = showTime;

		Vector3 v;

		if(setPanelPosition)
		{
			v = panel.transform.localPosition;
			v.x = posX ;
			v.y = posY ;
			panel.transform.localPosition = v;

		}
		else
		{
			v = transform.position;
			v.x = posX ;
			v.y = posY - 90;
			transform.position = v;
		}

		lbText.text = text;

		if(useLabelAutoResize) spBg.width = Mathf.RoundToInt(lbText.printedSize.x + printOffset);

		_isMouseDown = false;
	}

	Bounds _b;

	public void start(float showTime = 2.0f)
	{
		panel.cachedGameObject.SetActive(true);
		panel.alpha = 1.0f;
		_delay = showTime;

		_isMouseDown = false;
	}


	public void start(string text, float showTime = 2.0f)
	{
		panel.cachedGameObject.SetActive(true);
		panel.alpha = 1.0f;
		_delay = showTime;
		lbText.text = text;

		_isMouseDown = false;
	}


	void Update()
	{
		if(_isMouseDown == false)
		{
			if(Input.GetMouseButtonDown(0))
			{
				_isMouseDown = true;
			}
		}
		else if(_isMouseDown)
		{
			if(Input.GetMouseButtonUp(0))
			{
				_isMouseDown = false;
				hide();
			}
		}

		if(_delay > 0)
		{
			_delay -= Time.smoothDeltaTime;
			return;
		}

		if(panel.alpha > 0)
		{
			float alpha = panel.alpha;
			alpha -= Time.smoothDeltaTime;
			if(alpha > 0)
			{
				panel.alpha = alpha;
			}
			else
			{
				hide ();
			}
		}
	}


	private bool _isMouseDown = false;

	public void hide()
	{
		panel.cachedGameObject.SetActive(false);
		_isMouseDown = false;
	}


	public bool isPlaying
	{
		get
		{
			if(panel.cachedGameObject.activeSelf)
			{
				hide();
				return true;
			}

			return false;
		}
	}

	void OnDisable()
	{
		hide ();
	}

}
