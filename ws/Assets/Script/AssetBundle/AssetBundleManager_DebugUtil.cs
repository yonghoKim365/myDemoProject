using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

public partial class AssetBundleManager : MonoBehaviour 
{


	//==== 임시 ====================================//
	public string[] deleteTempData()
	{
		try
		{

			string[] f = Directory.GetFiles(getLocalFilePath());

			List<string> dn = new List<string>();

			for(int i = 0; i < f.Length; ++i)
			{
				if(Path.GetExtension(f[i]).ToLower() == "txt" || Path.GetExtension(f[i]).ToLower() == ".txt")
				{
					File.Delete(f[i]);
					dn.Add(  Path.GetFileName(f[i]) );
				}
			}

			if(dn.Count > 0)
			{
				return dn.ToArray();
			}
		}
		catch(System.Exception e)
		{

		}

		return null;
	}


	public void startDownloadTempDataFile(params string[] files)
	{
		UINetworkLock.instance.show();
		
		for(int i = files.Length - 1; i >= 0; --i)
		{
			_files.Push(files[i]);
		}
		
		StartCoroutine(startDownLoadTempData());
	}
	
	Stack<string> _files = new Stack<string>();
	
	private IEnumerator startDownLoadTempData()
	{
		bool isSuccess = false;
		string msg = "";
		
		yield return null;
		
		while(Caching.ready == false)
		{
			yield return null;
		}
		
		while(_files.Count > 0)
		{
			string n = _files.Pop();
			
			WWW www = new WWW(resourcePath + "data/" + n + ".txt");
			yield return www;
			
			if(www.error != null)
			{
				
			}
			else if( www.isDone)
			{
				if(n.Contains("cs/"))
				{
					n = n.Replace("cs/","");
				}
				
				File.WriteAllText(getLocalFilePath() + n + ".txt", www.text );
				
				isSuccess = true;
				msg += n + ",";
			}
			
			www.Dispose();
			www = null;
		}
		
		UINetworkLock.instance.hide();
		
		if(isSuccess)
		{
			UISystemPopup.open( UISystemPopup.PopupType.YesNo, msg, NetworkManager.RestartApplication);
		}
	}
	
	
	public static string getLocalFilePath()
	{
		string localFileName = "";
		
		#if UNITY_EDITOR 
		localFileName = Application.dataPath + "/../Cache/";
		#elif UNITY_ANDROID
		localFileName =  Application.persistentDataPath + "/";
		#elif UNITY_IPHONE
		localFileName =  Application.temporaryCachePath + "/";
		#endif		
		
		return localFileName;
	}	
	

	
	private void saveDataToLocal(string assetName, byte[] data)
	{
		FileStream file = File.Open(ResourceManager.getLocalFilePath(assetName,""), FileMode.Create);
		var binary= new BinaryWriter(file);
		binary.Write(data);
		file.Close();	
	}
	
	//==== 임시 ====================================//


}