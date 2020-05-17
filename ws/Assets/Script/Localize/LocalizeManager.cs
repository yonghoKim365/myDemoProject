using UnityEngine;
using System.Collections;

public class LocalizeManager : MonoBehaviour 
{
	public static SystemLanguage language;

	public static LocalizeManager instance;

	public UIFont[] uifonts;
	public Font[] trueTypeFonts;

	public int KOR_INDEX = 0;
	public int JPN_INDEX = 1;

	public bool useDeviceLanguage = true;

	public delegate void LanguageChanger();
	public LanguageChanger languageDispatcher;

	public bool useLocalize = false;

	void Awake ()
	{
		if(instance == null)
		{
			instance = this;

			language = Application.systemLanguage;

			DontDestroyOnLoad(gameObject);
			return;
		}

		Destroy(gameObject);
	}


	void OnApplicationPause(bool flag){
		
		if(flag == true)
		{

		}
		else
		{
			if(useDeviceLanguage)
			{
				if(language != Application.systemLanguage)
				{
					Debug.LogError("Change!!!");
				}
			}
		}
	}












	public UIFont changeFont(UIFont font)
	{
		if(font == null) return null;
		
		if(uifonts == null) return null;
		
		if(uifonts.Length <= KOR_INDEX || uifonts.Length <= JPN_INDEX) return null;
		
		bool canLocalize = false;
		
		for(int i = uifonts.Length - 1; i >= 0; --i)
		{
			if(font == uifonts[i])
			{
				canLocalize = true;
				break;
			}
		}
		
		if(canLocalize == false) return null;
		
		if(Application.systemLanguage == SystemLanguage.Korean)
		{
			if(uifonts[KOR_INDEX] != font)
			{
				return uifonts[KOR_INDEX];
			}
		}
		else if(Application.systemLanguage == SystemLanguage.Japanese)
		{
			if(uifonts[JPN_INDEX] != font)
			{
				return uifonts[JPN_INDEX];
			}
		}
		
		return null;
	}
	
	
	public Font changeFont(Font font)
	{
		if(font == null) return null;
		
		if(trueTypeFonts == null) return null;
		
		if(trueTypeFonts.Length <= KOR_INDEX || trueTypeFonts.Length <= JPN_INDEX) return null;
		
		bool canLocalize = false;
		
		for(int i = trueTypeFonts.Length - 1; i >= 0; --i)
		{
			if(font == trueTypeFonts[i])
			{
				canLocalize = true;
				break;
			}
		}
		
		if(canLocalize == false) return null;
		
		if(Application.systemLanguage == SystemLanguage.Korean)
		{
			if(trueTypeFonts[KOR_INDEX] != font)
			{
				return trueTypeFonts[KOR_INDEX];
			}
		}
		else if(Application.systemLanguage == SystemLanguage.Japanese)
		{
			if(trueTypeFonts[JPN_INDEX] == font)
			{
				return trueTypeFonts[JPN_INDEX];
			}
		}
		
		return null;
	}


}
