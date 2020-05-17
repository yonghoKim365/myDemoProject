using System;
using System.Collections.Generic;

sealed public class VersionData : BaseData
{
	public static int[] codePatchVer = new int[0];
	public static int codePathVerNum = 0;

	public string id;
	public string[] versions;
	public int[] codePatch = null;

	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		string t = (string)l[k["VERSION"]];

		string cv = l[k["CODEPATCH"]].ToString();

		if(string.IsNullOrEmpty(cv))
		{
			codePatch = null;
		}
		else
		{
			codePatch = Util.stringToIntArray(cv,',');
		}

		if(string.IsNullOrEmpty(t) == false)
		{
			versions = t.Split('/');
		}
		else
		{
			versions = null;
		}
	}

	public static bool checkCodeVersion(int checkVersion)
	{
		if(codePatchVer == null)
		{
			return false;
		}
		else
		{
			for(int i = 0; i < codePathVerNum; ++i)
			{
				if(codePatchVer[i] == checkVersion)
				{
					return true;
				}
			}
		}

		return false;
	}

	public static bool isCompatibilityVersion(string checkVersion, bool isContinueMode)
	{
#if UNITY_EDITOR
//		return true;
#endif

		string currentVer;

		if(checkVersion == GameManager.info.clientFullVersion) return true;

		if(isContinueMode)
		{
			currentVer = "C"+GameManager.info.clientFullVersion;
		}
		else
		{
			currentVer = GameManager.info.clientFullVersion;
		}

		VersionData vd;

		if(GameManager.info.versionData.TryGetValue(currentVer, out vd))
		{
			if(vd.versions != null)
			{
				foreach(string v in vd.versions)
				{
					if(v == checkVersion) return true;
				}
			}
		}

		return false;
	}


}