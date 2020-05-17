using System;
using System.Text;
using UnityEngine;
using System.IO;

sealed public class Log
{
	private static StringBuilder str = new StringBuilder();
	public static void log(params object[] data)
	{
		#if UNITY_EDITOR
//		if(GameManager.isDebugBuild)
		{
			int len = str.Length;
			
			str.Remove(0,len);
			
			len = data.Length;		
			
			for(int i = 0; i < len; ++i)
			{
				str.Append(data[i].ToString() + (((i+1)<len)?",":""));
			}
//		UnityEngine.Debug.Log(str.ToString());


			try
			{
				fileLog.Append("           " + str.ToString() + "   playtime: " + GameManager.me.stageManager.playTime + "\r\n");
			}
			catch(Exception e)
			{
			}
		}

		#endif
	}

	private static StringBuilder fileLog = new StringBuilder();

	public static void logError(params object[] data)
	{
#if UNITY_EDITOR
//		if(GameManager.isDebugBuild)
		{
			int len = str.Length;
			
			str.Remove(0,len);
			
			len = data.Length;		
			
			for(int i = 0; i < len; ++i)
			{
				str.Append(data[i].ToString() + (((i+1)<len)?",":""));
			}
			
			//		UnityEngine.Debug.LogError(str.ToString());
			
			try
			{
				fileLog.Append(str.ToString() + "   playtime: " + GameManager.me.stageManager.playTime + "\r\n");
			}
			catch(Exception e)
			{
			}
		}
#endif
	}	

	public static void clearFileLog()
	{
		fileLog.Length = 0;
	}

	public static void saveFileLog()
	{
#if UNITY_EDITOR
//		if(GameManager.isDebugBuild)
		{
			string path = Application.dataPath;
			#if UNITY_EDITOR
			path = path.Replace("Assets","log/");
			#else
			path = AssetBundleManager.getLocalFilePath();
			#endif


			if(Directory.Exists(path) == false) Directory.CreateDirectory(path);
			System.IO.File.WriteAllText(path+ System.DateTime.Now.ToString("dd_hh_mm_ss") + ".txt",  fileLog.ToString());
			fileLog = new StringBuilder();
		}
#endif

	}


	public static void saveLogInEditor(string str, string fileName)
	{
#if UNITY_EDITOR
//		saveFileLog(str, fileName);
#endif
	}


	public static void saveFileLog(string str, string fileName)
	{
		string path = Application.dataPath;
		path = AssetBundleManager.getLocalFilePath();

		fileLog.Append(str);

		if(Directory.Exists(path) == false) Directory.CreateDirectory(path);
		System.IO.File.WriteAllText(path+ fileName + System.DateTime.Now.ToString("dd_hh_mm_ss") + ".txt",  fileLog.ToString());
		fileLog.Length = 0;

	}


}

