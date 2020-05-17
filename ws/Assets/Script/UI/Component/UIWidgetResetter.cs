using UnityEngine;
using System.Collections;

public class UIWidgetResetter : MonoBehaviour 
{
	public Color originalColor;
	public TweenAlpha ta;
	public TweenScale ts;
	public UISprite sprite;

	void Awake()
	{
		originalColor = sprite.color;
		ta = GetComponent<TweenAlpha>();
		ts = GetComponent<TweenScale>();
	}

	void OnEnable()
	{
		transform.localScale = Vector3.one;
		sprite.color = originalColor;
		if(ta != null) ta.ResetToBeginning();
		if(ts != null) ts.ResetToBeginning();
	}

	void OnDisable()
	{
		transform.localScale = Vector3.one;
		sprite.color = originalColor;
		if(ta != null) ta.ResetToBeginning();
		if(ts != null) ts.ResetToBeginning();
	}

}
