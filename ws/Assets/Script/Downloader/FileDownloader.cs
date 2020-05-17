using System;
using System.Collections;
using UnityEngine;
using System.IO;

sealed public class FileDownloader : MonoBehaviour
{
	
	void Start()
	{
		//loadFile();
	}
	
	WWW www;
	public void loadFile()
	{
		www = new WWW("http://linktomorrow.iptime.org/common/ek/e.apk");
		
		StartCoroutine(downloadProgress(www));
	}
	
	void Update()
	{
		if(www != null && www.isDone == false)
		{
			//Log.log("www progress: " + www.progress);
		}
	}
	
	private IEnumerator downloadProgress(WWW www)
	{
//		Log.log("www? ");
//		Log.log("www progress: " + www.progress);
		
		yield return www;
		
		if(www.error == null)
		{
			//www.bytes
			
			//[C#] Create new file and open it for read and write, if the file exists overwrite it.
			//FileStream fileStream = new FileStream(@"c:\file.txt", FileMode.Create);

			//[C#] Create new file and open it for read and write, if the file exists throw exception.
			//FileStream fileStream = new FileStream(@"c:\file.txt", FileMode.CreateNew);
			//
			
			FileStream fs = new FileStream("e.apk", FileMode.Create);
			
			fs.Seek(0, SeekOrigin.Begin);
			fs.Write(www.bytes, 0, www.bytes.Length);
			fs.Close();
			
//			Log.log("DONE!");
		}
		else
		{
		}
	}
	
}
