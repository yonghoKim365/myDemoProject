using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Security.Cryptography;



public class Util 
{

	public static void splitAlphaChannelFromTexture(Texture2D tex, string path)
	{
		if(tex == null) return;
		
		Texture2D alphaTex = new Texture2D(tex.width, tex.height);
		
		for(int x = 0; x < tex.width; ++x)
		{
			for(int y = 0; y < tex.height; ++y)
			{
				Color c = tex.GetPixel(x,y);
				
				if (c.a > 0 & c.a <= 1)
				{
					alphaTex.SetPixel(x, y, new Color(c.a, c.a, c.a, c.a));
				}
				else
				{
					alphaTex.SetPixel(x, y, new Color(0,0,0,0));
				}
			}
		}
		
		string clonePath = path.Substring(0,path.LastIndexOf(".")) + "_ETCCOMP.PNG";
		string alphaPath = path.Substring(0,path.LastIndexOf(".")) + "_ETCCOMP_ALPHA.PNG";
		
		SaveTextureToPNG(tex, clonePath);
		SaveTextureToPNG(alphaTex, alphaPath);
		
		Debug.LogError("Alpha Channel Splitting Complete : " + path);
	}
	
	
	public static void SaveTextureToPNG(Texture2D tex, string path)
	{
		byte[] bytes = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes(path, bytes);
		
		Debug.Log("SaveTextureToPNG : " + path);
		
		bytes = null;
	}


}
