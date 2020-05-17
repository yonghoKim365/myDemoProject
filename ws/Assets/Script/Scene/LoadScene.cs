using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour 
{
	public static LoadScene instance;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			gameObject.transform.parent = null;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void OnDestroy()
	{
		instance = null;
	}

}
