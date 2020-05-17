using UnityEngine;
using System.Collections;

public class UITabButton : MonoBehaviour {

	public UISprite spFontSprite;
	public UISprite spFrameSprite;

	public BoxCollider collider;

	public string fontOnName, fontOffName;
	public string frameOnName, frameOffName;

	public UIButton btn;


	public GameObject goOnObject;
	public GameObject goOffObject;

	public enum Type
	{
		ChangeSprite, OnOffGameObject
	}

	public Type type = Type.ChangeSprite;


	void Awake()
	{
		if(btn == null)
		{
			btn = GetComponent<UIButton>();
		}
	}

	public bool isEnabled
	{
		set
		{

			//Debug.Log("== " + name + " : " + value);

			if(value)
			{
				if(type == Type.ChangeSprite)
				{
					if(spFontSprite != null && string.IsNullOrEmpty(fontOnName) == false) spFontSprite.spriteName = fontOnName;
					if(spFrameSprite != null && string.IsNullOrEmpty(frameOnName) == false) spFrameSprite.spriteName = frameOnName;
				}
				else
				{
					goOnObject.SetActive(true);
					goOffObject.SetActive(false);
				}


				collider.enabled = false;
			}
			else
			{
				if(type == Type.ChangeSprite)
				{
					if(spFontSprite != null && string.IsNullOrEmpty(fontOffName) == false) spFontSprite.spriteName = fontOffName;
					if(spFrameSprite != null && string.IsNullOrEmpty(frameOffName) == false) spFrameSprite.spriteName = frameOffName;
				}
				else
				{
					goOnObject.SetActive(false);
					goOffObject.SetActive(true);
				}

				collider.enabled = true;
			}
		}
		get
		{
			return !collider.enabled;
		}
	}


}
