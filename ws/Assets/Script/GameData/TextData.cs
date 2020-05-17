using System;
using System.Collections.Generic;

sealed public class TextData
{
	private string _kor;
	private string _jpn;
	private string _eng;

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		_kor = (l[k["TEXT"]]).ToString().Replace("\\n","\n");

		_jpn = (l[k["JPN"]]).ToString().Replace("\\n","\n");

		_eng = (l[k["ENG"]]).ToString().Replace("\\n","\n");
	}


	public string text
	{
		get
		{
			if(LocalizeManager.instance != null && LocalizeManager.instance.useLocalize)
			{
				if(LocalizeManager.language == UnityEngine.SystemLanguage.Japanese)
				{
					return _jpn;
				}
				else if(LocalizeManager.language == UnityEngine.SystemLanguage.English)
				{
					return _eng;
				}
			}

			return _kor;
		}
	}
}
