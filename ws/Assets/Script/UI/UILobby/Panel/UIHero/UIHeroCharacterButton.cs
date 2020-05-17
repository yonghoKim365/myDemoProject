using UnityEngine;
using System.Collections;

public class UIHeroCharacterButton : MonoBehaviour 
{

	public void setData()
	{
		
	}
	
	public UISprite spLockImage;
	public UILabel lbName;
	public UIButton button;
	public UISprite spCharacterIcon;
	
	public void show()
	{
		gameObject.SetActive(true);
	}
	
	public void hide()
	{
		gameObject.SetActive(false);
	}
	
	private bool _isLock = false;
	
	public bool isLock
	{
		get
		{
			return _isLock;
		}
		set
		{
			_isLock = value;
			//button.enabled = !value;
			spLockImage.gameObject.SetActive(value);
		}
	}
	
	
	
}
