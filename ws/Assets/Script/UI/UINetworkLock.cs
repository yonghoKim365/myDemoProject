using UnityEngine;
using System.Collections;

public class UINetworkLock : UIBase 
{

	public static UINetworkLock instance;

	public GameObject main;

	public tk2dSprite spLoadingBar;
	public UISprite spBackground;

	void Awake()
	{
		if(instance==null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
			instance.gameObject.SetActive(false);
			main.gameObject.SetActive(true);
		}
		else
		{
			DestroyImmediate(this.gameObject);
		}
	}

	public void show(float alpha)
	{
		base.show ();
		Color c = spLoadingBar.color;
		c.a = alpha;
		spLoadingBar.color = c;

		c = spBackground.color;
		c.a = alpha;
		spBackground.color = c;
	}


	public override void show ()
	{
		base.show ();
		spLoadingBar.color = Color.white;
		Color c = spBackground.color;
		c.a = 96f/255f;
		spBackground.color = c;
	}

	public override void hide()
	{
		base.hide();
	}


}
