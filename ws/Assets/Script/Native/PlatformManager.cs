using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlatformManager : MonoBehaviour 
{
	public enum Platform
	{
		Google, Kakao
	}

	public Platform type = Platform.Google;

	public static PlatformManager instance = null;

	public PandoraManager pandora;

	void Awake () 
	{
		if(instance==null)
		{
			DontDestroyOnLoad(gameObject);	
			instance = this;
		}
		else
		{
			DestroyImmediate(this.gameObject);
		}
		
	}



}
