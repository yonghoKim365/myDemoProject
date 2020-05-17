using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for Des
/// </summary>
public class Des 
{
    string key;
	protected Des()
	{
	}


    public Des(string _key)
    {
        this.key = _key;
    }

    public string Encrypt(string _Input)
    {
        RijndaelManaged aes = new RijndaelManaged();
		aes.KeySize = 256;
		aes.BlockSize = 128;
		aes.Mode = CipherMode.CBC;
		aes.Key = Encoding.UTF8.GetBytes(key);
		
		aes.IV = new byte[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0} ;
		var encrypt = aes.CreateEncryptor(aes.Key , aes.IV);
		
		byte[] xBuff = null;
		
		using (var ms = new MemoryStream())
		{
			using(var cs = new CryptoStream(ms , encrypt , CryptoStreamMode.Write))
			{
				byte[] xXml = Encoding.UTF8.GetBytes(_Input);
				cs.Write(xXml , 0, xXml.Length);
			}
			xBuff = ms.ToArray();
		}
		
		string Output = Convert.ToBase64String(xBuff);
		return Output;
    }

    public string Decrypt(string _Input)
    {
        RijndaelManaged aes = new RijndaelManaged();
		aes.KeySize = 256;
		aes.BlockSize = 128;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		
		aes.Key = Encoding.UTF8.GetBytes(key);
		aes.IV = new byte[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0} ;
		
		var decrypt = aes.CreateDecryptor();
		byte[] xBuff = null;
		
		using(var ms = new MemoryStream())
		{
			using (var cs = new CryptoStream(ms , decrypt , CryptoStreamMode.Write))
			{
				byte[] xXml = Convert.FromBase64String(_Input);
				cs.Write(xXml , 0, xXml.Length);
			}
			
			xBuff = ms.ToArray();
		}
		string Output = Encoding.UTF8.GetString(xBuff);
		return Output;
    }
}
