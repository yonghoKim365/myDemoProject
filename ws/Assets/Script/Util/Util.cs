using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using LitJson;
#endif
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;



public class Util 
{

	public static WaitForSeconds ws1 = new WaitForSeconds(1.0f);
	public static WaitForSeconds ws005 = new WaitForSeconds(0.05f);
	public static WaitForSeconds ws008 = new WaitForSeconds(0.08f);
	public static WaitForSeconds ws01 = new WaitForSeconds(0.1f);
	public static WaitForSeconds ws02 = new WaitForSeconds(0.2f);
	public static WaitForSeconds ws03 = new WaitForSeconds(0.3f);
	public static WaitForSeconds ws05 = new WaitForSeconds(0.5f);

	
	public static StringBuilder stringBuilder = new StringBuilder();

	public static void addToStringBuilder(string str, string seperator = null, bool isFirstTime = false, StringBuilder sb = null)
	{
		if(sb == null) sb = stringBuilder;
		if(isFirstTime) sb.Length = 0;
		sb.Append(str);
		if(seperator != null) sb.Append(seperator);
	}


	public static string secToHourMinuteString(int num, string h =":", string m = ":", bool fixedNumberLength = true)
	{
		int hours, minute, second;
		//시간공식
		hours = num / 3600;//시 공식
		minute = num % 3600 / 60;//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.

		if(hours > 0)
		{
			return hours + h + ((minute<10 && fixedNumberLength)?"0":"") + minute + m ;
		}
		else
		{
			return ((minute<10 && fixedNumberLength)?"0":"") + minute + m ;
		}
	}


	public static string secToHourMinuteSecondString(int num, string h =":", string m = ":", string s ="", bool fixedNumberLength = true)
	{
		int hours, minute, second;
		//시간공식
		hours = num / 3600;//시 공식
		minute = num % 3600 / 60;//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.
		second = num % 3600 % 60;//마지막 남은 시간에서 분을 뺀 나머지 시간을 초로 계산함

		if(hours > 0)
		{
			return hours + h + ((minute<10 && fixedNumberLength)?"0":"") + minute + m + ((second<10 && fixedNumberLength)?"0":"") + second + s;
		}
		else
		{
			return ((minute<10 && fixedNumberLength)?"0":"") + minute + m + ((second<10 && fixedNumberLength)?"0":"") + second + s;
		}
	}

	static System.Text.StringBuilder str = new System.Text.StringBuilder();
	public static string secToDayHourMinuteSecondString(int num, string h =":", string m = ":", string s ="")
	{
		str.Length = 0;

		int days, hours, minute, second;
		//시간공식
		days = num / (3600*24);
		hours = num / 3600 % 24;//시 공식
		minute = num % 3600 / 60;//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.
		second = num % 3600 % 60;//마지막 남은 시간에서 분을 뺀 나머지 시간을 초로 계산함

		if(days > 0)
		{
			str.Append(days);
			str.Append(getUIText("DAY"));
			str.Append(" ");

		}

		if(hours > 0)
		{
			str.Append(hours);
			str.Append(getUIText("TIME"));
			str.Append(" ");
		}

		if(minute > 0)
		{
			str.Append(minute);
			str.Append(getUIText("MINUTE"));
			str.Append(" ");
		}

		if(second > 0)
		{
			str.Append(second);
			str.Append(getUIText("SECOND"));
			str.Append(" ");
		}

		str.Append(Util.getUIText("REMAIN"));

		return str.ToString();
	}



	public static string secToDayHourMinuteSecondString2(int num, string suffix = "남음")
	{
		str.Length = 0;
		
		int days, hours, minute, second;
		//시간공식
		days = num / (3600*24);
		hours = num / 3600 % 24;//시 공식
		minute = num % 3600 / 60;//분을 구하기위해서 입력되고 남은값에서 또 60을 나눈다.
		second = num % 3600 % 60;//마지막 남은 시간에서 분을 뺀 나머지 시간을 초로 계산함
		
		if(days > 0)
		{
			str.Append(days);
			str.Append(getUIText("DAY"));
			str.Append(" ");
			str.Append(suffix);
			return str.ToString();
		}
		
		if(hours > 0)
		{
			str.Append(hours);
			str.Append(getUIText("TIME"));
			str.Append(" ");
			str.Append(suffix);
			return str.ToString();
		}
		
		if(minute > 0)
		{
			str.Append(minute);
			str.Append(getUIText("MINUTE"));
			str.Append(" ");
		}
		
		if(second > 0)
		{
			str.Append(second);
			str.Append(getUIText("SECOND"));
			str.Append(" ");
		}
		
		str.Append(suffix);
		
		return str.ToString();
	}




	public static string stringSubstitute(string source, params string[] values)
	{
		int len = values.Length;
		
		for(int i = 0 ; i < len ; ++i)
		{
			source = source.Replace("{"+i+"}",values[i]);
		}
		
		return source;
	}
	
	
	public static Dictionary<string, int> str2intDic(string str)
	{
		Dictionary<string, int> dic = new Dictionary<string, int>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(',');
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			dic[temp2[0]] = System.Convert.ToInt32(temp2[1]);
		}
		
		temp1 = null;
		
		return dic;
	}
	
	
	
	public static Dictionary<string, int> str2intDic(string str, params char[] seperator)
	{
		Dictionary<string, int> dic = new Dictionary<string, int>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(seperator);
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			dic[temp2[0]] = System.Convert.ToInt32(temp2[1]);
		}
		
		temp1 = null;
		
		return dic;
	}
	
	
	
	
	
	public static Dictionary<string, string> str2strDic(string str)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(',');
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			dic[temp2[0]] = temp2[1];
		}
		
		temp1 = null;
		
		return dic;
	}
	
	
	public static Dictionary<string, string> str2strDic(string str, params char[] seperator)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(seperator);
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			dic[temp2[0]] = temp2[1];
		}
		
		temp1 = null;
		
		return dic;
	}

	
	
	
	
	
	
	public static Dictionary<string, object> str2objDic(string str)
	{
		Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(',');
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			
			if(temp2.Length == 2)
			{
				dic[temp2[0]] = (object)temp2[1];
			}
			else
			{
				dic[temp2[0]] = null;
			}
		}
		
		temp1 = null;
		
		return dic;
	}
	
	
	
	public static Dictionary<string, object> str2objDic(string str, params char[] seperator)
	{
		Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(seperator);
		
		if(str == "" || temp1.Length == 0) return dic;
		
		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			dic[temp2[0]] = (object)temp2[1];
		}
		
		temp1 = null;
		
		return dic;
	}
	
	
	
	public static List<T> arryList2list<T>(ArrayList arrList)
	{
		List<T> l = new List<T>(arrList.Count);
		
		foreach(T instance in arrList)
		{
			l.Add(instance);
		}
		
		return l;
	}
	
	
	
	
	
	
	public static string getText(string textCode, params string[] value)
	{
#if UNITY_EDITOR
//		Debug.Log(textCode);
#endif
		if(GameManager.info.textData.ContainsKey(textCode) == false)
		{
			return string.Empty;
		}

		return stringSubstitute(GameManager.info.textData[textCode].text, value);
	}



	public static string getUIText(string textCode, params string[] value)
	{
		if(GameManager.info != null && GameManager.info.uiTextData != null && GameManager.info.uiTextData.ContainsKey(textCode))
		{
			return stringSubstitute(GameManager.info.uiTextData[textCode].text, value);
		}

		if(HardcodingText.instance != null && HardcodingText.instance.data != null && HardcodingText.instance.data.ContainsKey(textCode))
		{
			return stringSubstitute(HardcodingText.instance.data[textCode].text, value);
		}

		return string.Empty;
	}


	public static string getTutorialText(string id, int step, params string[] value)
	{
		return stringSubstitute(GameManager.info.tutorialData[id][step].text, value);
	}
	
	
	public static int getTimer()
	{
		// 시간 개념을 확인을 해봐야함....
		return (int)Time.realtimeSinceStartup;
	}
	
	
	
	
	
	public static string listStringJoiner(List<string> list, string joinCharacter = ",")
	{
		if(list == null) return "";
		
		string str = "";
		int len = list.Count;
		
		for(int i = 0; i < len; ++i)
		{
			if(i < len-1)
			{
				str += list[i] + joinCharacter;
			}
			else
			{
				str += list[i];
			}
		}
		return str;
	}


	public static T[] setArrayValue<T>(T[] array,params T[] values)
	{
		if(array == null || array.Length < values.Length)
		{
			array = new T[values.Length];
		}

		for(int i = values.Length - 1; i >= 0 ; --i)
		{
			array[i] = values[i];
		}
		
		return array;
	}


	public static List<T> setListValue<T>(List<T> list,params T[] values)
	{
		if(list == null || list.Count < values.Length)
		{
			list = new List<T>(values.Length);
		}
		
		for(int i = 0; i < values.Length ; ++i)
		{
			if(list.Count <= i)
			{
				list.Add(values[i]);
			}
			else
			{
				list[i] = values[i];
			}
		}
		
		return list;
	}

	
	public static T[] suffleArray<T>(T[] inputArray)
	{
		List<T> list = new List<T>(inputArray.Length);
		list.AddRange(inputArray);
		
		T[] outputArray = new T[inputArray.Length];
		
		int index = 0;
		int randomIndex = 0;
		while(list.Count > 0)
		{
			randomIndex = UnityEngine.Random.Range(0,list.Count);
			outputArray[index] = list[randomIndex];
			list.RemoveAt(randomIndex);
			++index;
		}
		
		list = null;
		
		return outputArray;
	}
	

	
	
	
	public static byte[] streamToByteArray(System.IO.Stream stream)
    {
        long originalPosition = 0;

        if(stream.CanSeek)
        {
             originalPosition = stream.Position;
             stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }
            return buffer;
        }
        finally
        {
            if(stream.CanSeek)
            {
                 stream.Position = originalPosition; 
            }
        }
    }
	
	

	
	private static int _tempIndex = 0;
	public static Dictionary<string, string> logicParser(string str, int index, int[] totalPercent)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.Ordinal);
		
		string[] temp1 = str.Split(',');
		
		if(str == "" || temp1.Length == 0) return dic;
		
		totalPercent[index] = 0;

		foreach(string tempCode in temp1)
		{
			string[] temp2 = tempCode.Split(':');
			
			if(temp2.Length == 2)
			{
				string[] temp3 = temp2[0].Split('%'); // %가 있는 녀석들은 중복이 된다는 뜻이다. 근데 키가 I가 여러개라면?
				
				if(temp3.Length == 2) // % 있는 녀석들은 임시로 키값에 유니크한 인덱스를 붙여준다.
				{
					totalPercent[index] += Convert.ToInt32( temp3[0] );	
					dic[temp2[0]+"_"+_tempIndex] = temp2[1];
					++_tempIndex;
				}
				else
				{
					dic[temp2[0]] = temp2[1];
				}
			}
			else dic[temp2[0]] = temp2[0];
		}
		
		if(totalPercent[index] < 100) totalPercent[index] = 100;
		
		temp1 = null;
		
		return dic;
	}
	
/*	
	public static int getAngleBetween(Vector2 v1, Vector2 v2)
	{
		float dx = v1.x - v2.x;
		float dy = v1.y - v2.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	
	
	
	public static int getAngleBetween(Transform t1, Transform t2)
	{
		float dx = t1.transform.position.x - t2.transform.position.x;
		float dy = t1.transform.position.y - t2.transform.position.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}
	
	
	public static int getAngleBetweenXZ(Transform t1, Transform t2)
	{
		float dx = t1.transform.position.x - t2.transform.position.x;
		float dy = t1.transform.position.z - t2.transform.position.z;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	
	*/

	public static float getFloatAngleBetweenXY(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.y - t2.y;
		
		float ta = (Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}


	public static int getAngleBetweenXZ(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.z - t2.z;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}		
	

	public static float getFloatAngleBetweenXZ(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.z - t2.z;
		
		float ta = (float)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
		
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}	



	
	/*
	public static int getAngleBetween(Vector3 t1, Vector3 t2)
	{
		float dx = t1.x - t2.x;
		float dy = t1.y - t2.y;

		int ta = (int)(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg) + 180;
			
		if(ta < 0) ta += 360;
		else if(ta >= 360) ta %= 360;
		
		return ta;
	}
	*/
	
	
	
	private static Vector3 _v = Vector2.zero;
	public static Vector3 getPositionByAngleAndDistance(int angle, float dist)
	{
		_v.x = dist * GameManager.angleTable[angle].x;
		_v.y = dist * GameManager.angleTable[angle].y;
		_v.z = 0.0f;
		
		return _v;
	}

	public static Vector3 getPositionByAngleAndDistanceWithoutTable(float angle, float dist)
	{
		_v.x = dist * Mathf.Cos(angle*Mathf.Deg2Rad);
		_v.y = dist * Mathf.Sin(angle*Mathf.Deg2Rad);
		_v.z = 0.0f;
		
		return _v;
	}

	
	public static Vector3 getPositionByAngleAndDistanceXZ(int angle, float dist)
	{
		_v.x = dist * GameManager.angleTable[angle].x;
		_v.y = 0.0f;
		_v.z = dist * GameManager.angleTable[angle].y;
		
		return _v;
	}


	public static Vector3 getPositionByAngleAndDistanceXZWithoutTable(float angle, float dist)
	{
		_v.x = dist * Mathf.Cos(angle*Mathf.Deg2Rad);
		_v.z = dist * Mathf.Sin(angle*Mathf.Deg2Rad);
		_v.y = 0.0f;
		
		return _v;
	}

	
	
	/*
	public static Vector3 getHitPosition(Transform me, Transform target)
	{
		float dd = VectorUtil.Distance(me.position, target.position);
		int aa = Util.getAngleBetween(me, target);
		return (me.position + Util.getPositionByAngleAndDistance(aa, dd * 0.5f));
	}
	
	public static Vector3 getHitPosition(Vector3 me, Vector3 target)
	{
		float dd = VectorUtil.Distance(me, target);
		int aa = Util.getAngleBetween(me, target);
		return (me + Util.getPositionByAngleAndDistance(aa, dd * 0.5f));
	}	
	*/
	

	public static string GetCommaScore(int score){
		string returnValue = "";
		int leftScore = score;
		while(leftScore>=1000){
			int nowScore = leftScore%1000;
			string nowScoreStr = "";
			if(nowScore<10){
				nowScoreStr = "00"+nowScore;
			}else if(nowScore<100){
				nowScoreStr = "0"+nowScore;
			}else{
				nowScoreStr = ""+nowScore;
			}
			leftScore = leftScore/1000;
			if(returnValue == ""){
				returnValue = nowScoreStr;
			}else{
				returnValue = nowScoreStr+","+returnValue;
			}
		}
		
		if(returnValue == ""){
			returnValue = leftScore.ToString();
		}else{
			returnValue = leftScore.ToString()+","+returnValue;
		}
		
		return returnValue;
	}	
	
	
	
	public static int GetInt(string str){
		try{
			return int.Parse(str);
		}catch{
			return 0;
		}
	}
	
	public const string plus="+";
	public const string multi="*";
	public const string percent="%";
	

	public static string GetShortID(string idStr, int limit, string addStr = "...")
	{
		if(idStr.Length > limit) return idStr.Substring(0,limit) + addStr;
		else return idStr;

		/*
		if(idStr == null) idStr = "";


		byte[] bytes = System.Text.Encoding.UTF8.GetBytes( idStr );

		limit *= 2;

		if(bytes.Length > limit )
		{
			return System.Text.Encoding.UTF8.GetString( bytes, 0, limit ) + addStr;
		}

		return idStr;

		*/


	}

	public static float GetShortFloat(float num, int underPoint = 0)
	{
		if(underPoint == 0)
		{
			num = UnityEngine.Mathf.Round(num);
		}
		else
		{
			num = num * (float)(Math.Pow(  10 ,  underPoint ) );
			num = UnityEngine.Mathf.Round(num);
			num = num /  (float)(Math.Pow(  10 ,  underPoint ) );
		}
		
		return num;
	}
		
	
	
	public static void damageMotion(Monster hitter, Monster target, float motionValue = 2.0f)
	{
		// -- 타격감 용 -- //
		_v = target.cTransformPosition;
		_v.x += motionValue * (((hitter.cTransformPosition.x - target.cTransformPosition.x ) > 0)?-1:1);

		target.setPosition(_v);
	}		
	
	
	
	
	
	public static bool intersectsLine(float x1, float w1, float x2, float w2)
	{
		if(x1 < x2)
		{
			if(x1+w1 >= x2) return true;
			else return false;
		}
		else if(x2+w2 >= x1) return true;
		
		return false;
	}
	
	
	
	public static void fileCopy(string sourcePath, string targetPath, string fileName, bool useMD5Name = false)
	{
        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);

		if(useMD5Name)
		{
			fileName = Util.getMD5HashFromFile(sourceFile) + ".p12";
		}

        string destFile = System.IO.Path.Combine(targetPath, fileName);

        if (!System.IO.Directory.Exists(targetPath))
        {
            System.IO.Directory.CreateDirectory(targetPath);
        }

        // To copy a file to another location and 
        // overwrite the destination file if it already exists.
        System.IO.File.Copy(sourceFile, destFile, true);			
	}



	public static void tryFloatParseToXfloat(string str, out Xfloat xf, float defaultValue)
	{
		float tmp = defaultValue;
		float.TryParse(str, out tmp);
		xf = tmp;
	}


	public static void tryIntParseToXInt(string str, out Xint xi, int defaultValue)
	{
		int tmp = defaultValue;
		int.TryParse(str, out tmp);
		xi = tmp;
	}

	
	public static void parseObject(System.Object obj, out float floatValue, float defaultValue = 0.0f)
	{
		if(obj is long) floatValue = (float)(long)obj;
		else if(obj is int) floatValue = (float)(int)obj;
		else if(obj is double) floatValue = (float)(double)obj;
		else if(obj is float) floatValue = (float)obj;
		else floatValue = defaultValue;
	}


	public static void parseObject(System.Object obj, out float floatValue, bool checkString, float defaultValue = 0.0f)
	{
		if(obj is long) floatValue = (float)(long)obj;
		else if(obj is int) floatValue = (float)(int)obj;
		else if(obj is double) floatValue = (float)(double)obj;
		else if(obj is float) floatValue = (float)obj;
		else if(obj is string && checkString)
		{
			floatValue = defaultValue;
			if(((string)obj).Length > 0) float.TryParse((string)obj, out floatValue);
		}
		else floatValue = defaultValue;
	}



	public static void parseObject(System.Object obj, out Xfloat floatValue, float defaultValue = 0.0f)
	{
		if(obj is long) floatValue = (float)(long)obj;
		else if(obj is int) floatValue = (float)(int)obj;
		else if(obj is double) floatValue = (float)(double)obj;
		else if(obj is float) floatValue = (float)obj;
		else floatValue = defaultValue;
	}
	
	
	public static void parseObject(System.Object obj, out Xfloat floatValue, bool checkString, float defaultValue = 0.0f)
	{
		if(obj is long) floatValue = (float)(long)obj;
		else if(obj is int) floatValue = (float)(int)obj;
		else if(obj is double) floatValue = (float)(double)obj;
		else if(obj is float) floatValue = (float)obj;
		else if(obj is string && checkString)
		{
			float fv = defaultValue;

			if(((string)obj).Length > 0) float.TryParse((string)obj, out fv);

			floatValue = fv;
		}
		else floatValue = defaultValue;
	}



	public static void parseObject(System.Object obj, out IFloat floatValue, bool checkString, float defaultValue = 0.0f)
	{
		if(obj is long) floatValue = (float)(long)obj;
		else if(obj is int) floatValue = (float)(int)obj;
		else if(obj is double) floatValue = (float)(double)obj;
		else if(obj is float) floatValue = (float)obj;
		else if(obj is string && checkString)
		{
			float fv = defaultValue;
			
			if(((string)obj).Length > 0) float.TryParse((string)obj, out fv);
			
			floatValue = fv;
		}
		else floatValue = defaultValue;
	}



	
	public static void parseObject(System.Object obj, out int intValue, int defaultValue = 0)
	{
		if(obj is long) intValue = (int)(long)obj;
		else if(obj is int) intValue = (int)obj;
		else if(obj is double) intValue = (int)(double)obj;
		else if(obj is float) intValue = (int)(float)obj;
		//else if(obj is string)
		//{
		//	intValue = defaultValue;
		//	int.TryParse((string)obj, out intValue);
		//}
		else intValue = defaultValue;
	}


	public static void parseObject(System.Object obj, out int intValue, bool checkString, int defaultValue = 0)
	{
		if(obj is long) intValue = (int)(long)obj;
		else if(obj is int) intValue = (int)obj;
		else if(obj is double) intValue = (int)(double)obj;
		else if(obj is float) intValue = (int)(float)obj;
		else if(obj is string && checkString)
		{
			intValue = defaultValue;
			if(((string)obj).Length > 0) int.TryParse((string)obj, out intValue);
		}
		else intValue = defaultValue;
	}


	public static void parseObject(System.Object obj, out Xint intValue, bool checkString, int defaultValue = 0)
	{
		if(obj is long) intValue = (int)(long)obj;
		else if(obj is int) intValue = (int)obj;
		else if(obj is double) intValue = (int)(double)obj;
		else if(obj is float) intValue = (int)(float)obj;
		else if(obj is string && checkString)
		{
			intValue = defaultValue;
			if(((string)obj).Length > 0) tryIntParseToXInt((string)obj, out intValue, intValue);
		}
		else intValue = defaultValue;
	}


	
	public static int objectToInt(object obj)
	{
		if(obj is long) return (int)(long)obj;
		else if(obj is int) return (int)obj;
		else if(obj is double) return (int)(double)obj;
		else if(obj is float) return (int)(float)obj;
		return -1;
	}
	
	
	public static void stringToIntArray(out int[] ints, string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		ints = new int[l];
		for(int i=0; i <l; ++i)
		{
			int.TryParse(tmp[i], out ints[i]);
		}
		tmp = null;
	}	
	
	public static int[] stringToIntArray(string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		int[] ints = new int[l];
		for(int i=0; i <l; ++i)
		{
			int.TryParse(tmp[i], out ints[i]);
		}
		tmp = null;
		return ints;
	}


	public static Xint[] stringToXIntArray(string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		Xint[] ints = new Xint[l];
		for(int i=0; i <l; ++i)
		{
			tryIntParseToXInt(tmp[i], out ints[i], ints[i]);
		}
		tmp = null;
		return ints;
	}



	public static Vector3 stringToVector3(string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		Vector3 iv = new Vector3();

		float.TryParse(tmp[0], out iv.x);
		float.TryParse(tmp[1], out iv.y);
		float.TryParse(tmp[2], out iv.z);

		tmp = null;
		return iv;
	}

	
	public static void stringToFloatArray(out float[] floats, string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		floats = new float[l];
		for(int i=0; i <l; ++i)
		{
			float.TryParse(tmp[i], out floats[i]);
		}
		tmp = null;
	}	
	
	public static float[] stringToFloatArray(string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		float[] floats = new float[l];
		for(int i=0; i <l; ++i)
		{
			float.TryParse(tmp[i], out floats[i]);
		}
		tmp = null;
		return floats;
	}
	

	public static Xfloat[] stringToXFloatArray(string str, params char[] separator)
	{
		string[] tmp = str.Split(separator);
		int l = tmp.Length;
		Xfloat[] floats = new Xfloat[l];

		float tx = 0;
		for(int i=0; i <l; ++i)
		{
			float.TryParse(tmp[i], out tx);
			floats[i] = tx;
		}
		tmp = null;
		return floats;
	}


	public static void tryFloatParse(string str, out Xfloat xf)
	{
		float fValue = 0;
		float.TryParse(str, out fValue);
		xf = fValue;
	}



	public static string[] combineStringArray(string[] a, string[] b)
	{
		int la = (a == null)?0:a.Length;
		int lb = (b == null)?0:b.Length;
		
		string[] arr = null; 
		
		if(la + lb > 0) arr = new string[la+lb];	
		else return null;
		
		for(int i = 0; i < la; ++i)
		{
			arr[i] = a[i];
		}
		
		for(int i = 0; i < lb; ++i)
		{
			arr[i+la] = b[i];
		}
		
		return arr;
	}
	




	public static int[] intArrayMerger(int[] baseArr, int[] addArr)
	{
		int len = (baseArr == null)?0:baseArr.Length;
		int addLen = (addArr == null)?0:addArr.Length;
		int[] arr = new int[len];
		
		for(int i = 0; i < len; ++i)
		{
			arr[i] = baseArr[i];
			if(i < addLen) arr[i] += addArr[i];
		}
		
		return arr;
	}


	public static Xint[] intArrayMerger(Xint[] baseArr, Xint[] addArr)
	{
		int len = (baseArr == null)?0:baseArr.Length;
		int addLen = (addArr == null)?0:addArr.Length;
		Xint[] arr = new Xint[len];
		
		for(int i = 0; i < len; ++i)
		{
			int v = baseArr[i];
			if(i < addLen)
			{
				v += addArr[i];
			}
			arr[i] = v;
		}
		
		return arr;
	}

	
	public static int[] concatIntArray(int[] a, int[] b)
	{
		int la = (a == null)?0:a.Length;
		int lb = (b == null)?0:b.Length;
		
		int[] arr = null; 
		
		if(la + lb > 0) arr = new int[la+lb];	
		else return null;
		
		for(int i = 0; i < la; ++i)
		{
			arr[i] = a[i];
		}
		
		for(int i = 0; i < lb; ++i)
		{
			arr[i+la] = b[i];
		}
		
		return arr;
	}
	
	public static string[] concatStringArray(string[] a, string[] b)
	{
		int la = (a == null)?0:a.Length;
		int lb = (b == null)?0:b.Length;
		
		string[] arr = null; 
		
		if(la + lb > 0) arr = new string[la+lb];	
		else return null;
		
		for(int i = 0; i < la; ++i)
		{
			arr[i] = a[i];
		}
		
		for(int i = 0; i < lb; ++i)
		{
			arr[i+la] = b[i];
		}
		
		return arr;
	}	
	
	
	public static Vector2 screenPositionWithCamViewRect(Vector2 inputPos)
	{
		float screenWidth = (float)Screen.width;
		float screenHeight = (float)Screen.height;
		
		float camRatioX = GameManager.screenSize.x / screenWidth;
		float camRatioY = GameManager.screenSize.y / screenHeight;

		Rect r = GameManager.me.gameCamera.rect;		
		
		if(r.x > 0)
		{
			camRatioX = GameManager.screenSize.x / (screenWidth * (1.0f - (r.x*2.0f)));
			inputPos.x -= screenWidth * r.x;
			inputPos.x *= camRatioX;
			inputPos.y *= camRatioY;
		}
		else if(r.y > 0)
		{
			camRatioY = GameManager.screenSize.y / (screenHeight * (1.0f - (r.y*2.0f)));
			inputPos.y -= screenHeight * r.y;
			inputPos.y *= camRatioY;
			inputPos.x = inputPos.x * camRatioX;
		}
		else
		{
			inputPos.x *= camRatioX;
			inputPos.y *= camRatioY;
		}
		
		return inputPos;
	}

	static IVector3 _zeroVector = Vector3.zero;
	static Quaternion _zeroQuaternion = new Quaternion(0,0,0,1);
	public static Quaternion getLookRotationQuaternion(IVector3 lookPos)
	{
		if(lookPos.x == _zeroVector.x && lookPos.y == _zeroVector.y && lookPos.z == _zeroVector.z)
		{
			return _zeroQuaternion;
		}
		else
		{
			return Quaternion.LookRotation(lookPos);
		}
//		return Quaternion.LookRotation(lookPos);
	}



	public static string[] CsvParser(string csvText)
	{
		if(csvText.Contains("\t"))
		{
			return csvText.Split('\t');
		}
		else
		{
			List<string> tokens = new List<string>();

			int last = -1;
			int current = 0;
			bool inText = false;
			
			while(current < csvText.Length)
			{
				switch(csvText[current])
				{
				case '"':
					inText = !inText; break;
				case ',':
					if (!inText) 
					{
						tokens.Add(csvText.Substring(last + 1, (current - last)).Trim(' ', ',')); 
						last = current;
					}
					break;
				default:
					break;
				}
				current++;
			}
			
			if (last != csvText.Length - 1) 
			{
				tokens.Add(csvText.Substring(last+1).Trim());
			}

			return tokens.ToArray();
		}
	}



	public static string getPointNumber(float num, int length)
	{
		if(num - (int)num > 0.001f)
		{
			return string.Format("{0:f"+length+"}", num);
		}

		return ((int)num).ToString();
	}



	// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
	public static string ColorToHex(Color color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	
	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color(r / 255.0f,g / 255.0f, b/ 255.0f);
	}


	public static long checkObjectMemorySize(object o)
	{
		long size = 0;
		using (Stream s = new MemoryStream()) {
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(s, o);
			size = s.Length;
		}
		return size;
	}


	public static string Base64Encode(string src, System.Text.Encoding enc) 
	{ 
		byte[] arr = enc.GetBytes(src); 
		return Convert.ToBase64String(arr); 
	}
	
	public static string Base64Decode(string src, System.Text.Encoding enc) 
	{ 
//		Debug.LogError("Base64Decode : " + src);
		byte[] arr = Convert.FromBase64String(src); 
		return enc.GetString(arr); 
	}  


	/*
	private static SHA256 _sha256 = new SHA256Managed();
	public static Int64 GetInt64Hash(string strText)
	{
		//SHA256 provider is not thread safe
		lock (_sha256)
		{
			byte[] hashText = _sha256.ComputeHash(Encoding.Unicode.GetBytes(strText));
			return BitConverter.ToInt64(hashText, 0) ^ BitConverter.ToInt64(hashText, 8) ^ BitConverter.ToInt64(hashText, 24);
		}
	}
	*/


	public static void clearAllPlayerPref()
	{
		int music = PlayerPrefs.GetInt("MUSIC", 0);
		int sfx = PlayerPrefs.GetInt("SFX", 0);

		PlayerPrefs.DeleteAll();

		PlayerPrefs.SetInt("MUSIC", music);
		PlayerPrefs.SetInt("SFX", sfx);
	}




	public static string getMD5HashFromFile(string fileName, int limitSize = AssetBundleManager.MD5_CHECKSIZE, int limitSize2 = AssetBundleManager.CRC_CHECKSIZE)
	{
		using(var md5 = MD5.Create())
		{
			using(var stream = File.OpenRead(fileName))
			{
				string name = "";
				string name2 = "";

				if(limitSize > 0 && stream.Length > limitSize)
				{
					name = BitConverter.ToString(md5.ComputeHash(readBytesBackward(stream, limitSize))).Replace("-",string.Empty);
				}
				else
				{
					name = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-",string.Empty);
				}

				Crc32 crc = new Crc32();

				limitSize2 += fileName.Length;

				if(limitSize2 > 0 && stream.Length > limitSize2)
				{
					crc.Update(readByres(stream, limitSize2));
//					name2 = CustomCrc32.Crc32.Compute( readByres(stream, limitSize2) ).ToString("x2");
				}
				else
				{
					crc.Update(readByres(stream, stream.Length));
//					name2 = CustomCrc32.Crc32.Compute( readByres(stream, stream.Length) ).ToString("x2");
				}

				name2 = crc.Value.ToString("X2");

				if(name2.Length > 4) name2 = name2.Substring(0,4);

				return name + name2;
			}
		}
	}



	public static string getMD5HashFromStream(Stream stream, int limitSize = AssetBundleManager.MD5_CHECKSIZE, int limitSize2 = AssetBundleManager.CRC_CHECKSIZE, int name1Size = 100, int name2Size = 4)
	{
		using(var md5 = MD5.Create())
		{
			//using(var stream = File.OpenRead(fileName))
			{
				string name = "";
				string name2 = "";

				stream.Position = 0;

				if(limitSize > 0 && stream.Length > limitSize)
				{
					name = BitConverter.ToString(md5.ComputeHash(readBytesBackward(stream, limitSize))).Replace("-",string.Empty);
				}
				else
				{
					name = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-",string.Empty);
				}

				stream.Position = 0;

				Crc32 crc = new Crc32();
				
				if(limitSize2 > 0 && stream.Length > limitSize2)
				{
					crc.Update(readByres(stream, limitSize2));
					//					name2 = CustomCrc32.Crc32.Compute( readByres(stream, limitSize2) ).ToString("x2");
				}
				else
				{
					crc.Update(readByres(stream, stream.Length));
					//					name2 = CustomCrc32.Crc32.Compute( readByres(stream, stream.Length) ).ToString("x2");
				}
				
				name2 = crc.Value.ToString("X2");

				if(name.Length > name1Size) name = name.Substring( name.Length - name1Size );

				if(name2.Length > name2Size) name2 = name2.Substring(0,name2Size);

				stream.Position = 0;

				return name + name2;
			}
		}
	}





	public static byte[] readBytesBackward (Stream stream, long size)
	{
		stream.Position = 0;

		byte[] buffer = new byte[32768];
		int read=0;
		
		int chunk;

		if(stream.Length - size >= 0)
		{
			stream.Position = stream.Length - size;
		}

		while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
		{
			read += chunk;
			
			// If we've reached the end of our buffer, check to see if there's
			// any more information
			if (read == buffer.Length)
			{
				int nextByte = stream.ReadByte();
				
				// End of stream? If so, we're done
				if (nextByte==-1)
				{
					return buffer;
				}
				
				// Nope. Resize the buffer, put in the byte we've just
				// read, and continue
				byte[] newBuffer = new byte[buffer.Length*2];
				Array.Copy(buffer, newBuffer, buffer.Length);
				newBuffer[read]=(byte)nextByte;
				buffer = newBuffer;
				++read;
			}
		}
		// Buffer is now too big. Shrink it.
		byte[] ret = new byte[read];
		Array.Copy(buffer, ret, read);
		return ret;
	}



	public static byte[] readByres (Stream stream, long limitSize = -1)
	{

		stream.Position = 0;

		byte[] buffer = new byte[32768];
		int read=0;
		
		int chunk;
		while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
		{
			read += chunk;
			
			// If we've reached the end of our buffer, check to see if there's
			// any more information
			if (read == buffer.Length)
			{
				int nextByte = stream.ReadByte();
				
				// End of stream? If so, we're done
				if (nextByte==-1)
				{
					return buffer;
				}
				
				// Nope. Resize the buffer, put in the byte we've just
				// read, and continue
				byte[] newBuffer = new byte[buffer.Length*2];
				Array.Copy(buffer, newBuffer, buffer.Length);
				newBuffer[read]=(byte)nextByte;
				buffer = newBuffer;
				++read;
			}

			if(limitSize > 0 && read > limitSize) break;
		}
		// Buffer is now too big. Shrink it.
		byte[] ret = new byte[read];
		Array.Copy(buffer, ret, read);
		return ret;
	}







	public static long getFileSize(string fileName)
	{
		long size = 0;

		FileInfo fi = new FileInfo(fileName);
		return fi.Length;
	}




	public static Byte[] enc(string str)
	{
//		Debug.LogError("enc str : " + str);

		string convertStr = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));

//		Debug.LogError("ToBase64String str : " + convertStr);

		using(MemoryStream mStream = new MemoryStream( System.Text.Encoding.UTF8.GetBytes( convertStr ) ))
		{
			Byte[] returnBA = new byte[(int)mStream.Length];
			mStream.Read(returnBA, 0, (int)mStream.Length);
			
			for(int i = 0 ; i < returnBA.Length-21 ; i+=20)
			{
				for(int j=0;j<10;j++)
				{
					returnBA[i+j] ^= returnBA[i+19-j];
					returnBA[i+j] ^= returnBA[i+19-j];
					returnBA[i+j] ^= returnBA[i+19-j];
				}
			}

			return returnBA;
		}
	}
	
	
	public static Byte[] encByte(Byte[] returnBA)
	{
		for(int i = 0 ; i < returnBA.Length-21 ; i+=20)
		{
			for(int j=0;j<10;j++)
			{
				returnBA[i+j] ^= returnBA[i+19-j];
				returnBA[i+j] ^= returnBA[i+19-j];
				returnBA[i+j] ^= returnBA[i+19-j];
			}
		}
		
		return returnBA;
	}
	
	
	public static string dec(Byte[] ba)
	{
		for(int i = 0; i < ba.Length-21;i+=20)
		{
			for(int j = 0; j < 10; ++j)
			{
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
			}
		}
		
		return Util.Base64Decode(System.Text.Encoding.UTF8.GetString(ba), System.Text.Encoding.UTF8);
	}
	
	
	public static Byte[] decByte(Byte[] ba)
	{
		for(int i = 0; i < ba.Length-21;i+=20)
		{
			for(int j = 0; j < 10; ++j)
			{
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
			}
		}
		return ba;
	}

	public static string decByteAndConvertToString(Byte[] ba)
	{
		for(int i = 0; i < ba.Length-21;i+=20)
		{
			for(int j = 0; j < 10; ++j)
			{
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
				ba[i+j] ^= ba[i+19-j];
			}
		}
		return System.Text.Encoding.UTF8.GetString(ba);
	}




	public static void saveZip(string zipPath, string[] entryNames, List<Byte[]> data)//, bool useEncrypt = false)
	{
		try
		{
			using(FileStream writer = File.Create(zipPath))
			{
				ZipOutputStream zos = new ZipOutputStream(writer);
				zos.SetLevel(9);
				
				Crc32 crc = new Crc32();
				
				for(int i = 0; i < entryNames.Length; ++i)
				{
					byte[] buffer = data[i];
					ZipEntry entry = new ZipEntry(entryNames[i]);
					
					entry.DateTime = DateTime.Now;
					
					crc.Reset();
					crc.Update(buffer);
					entry.Crc = crc.Value;
					entry.Size = buffer.Length;
					zos.PutNextEntry(entry);
					zos.Write(buffer, 0, buffer.Length);
					zos.CloseEntry();
				}
				
				zos.IsStreamOwner = false;
				zos.Finish();
				zos.Close();
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);	
		}
	}



	public static void saveZip(string zipPath, string[] files, bool useEncryptFileName = false)
	{
		try
		{

			using(FileStream writer = File.Create(zipPath))
			{
				ZipOutputStream zos = new ZipOutputStream(writer);
				zos.SetLevel(9);
				
				Crc32 crc = new Crc32();
				
				for(int i = 0; i < files.Length; ++i)
				{
					byte[] buffer = File.ReadAllBytes(files[i]);

					ZipEntry entry;

					string entryFileName;

					if(useEncryptFileName == false)
					{
						entryFileName = Path.GetFileName(files[i]);
					}
					else
					{
						string md5Name = Util.getMD5HashFromFile(files[i]) + ".p12";
						entryFileName = Path.GetFileName(md5Name);
					}

					entry = new ZipEntry( entryFileName );

					crc.Reset();
					crc.Update(buffer);
					entry.Crc = crc.Value;
					entry.Size = buffer.Length;
					zos.PutNextEntry(entry);
					zos.Write(buffer, 0, buffer.Length);
					zos.CloseEntry();
				}
				
				zos.IsStreamOwner = false;
				zos.Finish();
				zos.Close();
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.Message);
		}
	}



	public static byte[] saveZipToByteArray(string[] entryNames, List<Byte[]> data, string password = null)//, bool useEncrypt = false)
	{
		using(MemoryStream writer = new MemoryStream())
		{
			ZipOutputStream zos = new ZipOutputStream(writer);
			zos.SetLevel(9);
			
			if(password != null)
			{
				zos.Password = password;
			}
			
			Crc32 crc = new Crc32();
			
			for(int i = 0; i < entryNames.Length; ++i)
			{
				byte[] buffer = data[i];
				ZipEntry entry = new ZipEntry(entryNames[i]);
				
				entry.DateTime = DateTime.Now;
				
				//			crc.Reset();
				//			crc.Update(buffer);
				//			entry.Crc = crc.Value;
				
				
				entry.Size = buffer.Length;
				zos.PutNextEntry(entry);
				zos.Write(buffer, 0, buffer.Length);
				zos.CloseEntry();
			}
			
			zos.IsStreamOwner = false;
			zos.Finish();
			zos.Close();
			
			byte[] returnBytes = writer.ToArray();
			return returnBytes;
		}
	}



	public static void extractZipByteFile(byte[] inputZipByte, string password, string outFolder, bool overwrite = true) 
	{
		ZipFile zf = null;
		try {
			zf = new ZipFile(new MemoryStream(inputZipByte));

			if (!String.IsNullOrEmpty(password)) 
			{
				zf.Password = password;     // AES encrypted entries are handled automatically
			}
			
			foreach (ZipEntry zipEntry in zf) 
			{
				if (!zipEntry.IsFile) 
				{
					continue;           // Ignore directories
				}
				String entryFileName = zipEntry.Name;
				// to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
				// Optionally match entrynames against a selection list here to skip as desired.
				// The unpacked length is available in the zipEntry.Size property.
				
				byte[] buffer = new byte[4096];     // 4K is optimum
				Stream zipStream = zf.GetInputStream(zipEntry);
				
				// Manipulate the output filename here as desired.
				String fullZipToPath = Path.Combine(outFolder, entryFileName);
				string directoryName = Path.GetDirectoryName(fullZipToPath);
				if (directoryName.Length > 0)
				{
#if UNITY_EDITOR
//					Debug.Log(directoryName);
#endif
					if(Directory.Exists(directoryName) == false) Directory.CreateDirectory(directoryName);
				}

				if(File.Exists(fullZipToPath))
				{
					if(overwrite == false)  continue;
					File.Delete(fullZipToPath);
				}
				// Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
				// of the file, but does not waste memory.
				// The "using" will close the stream even if an exception occurs.
				using (FileStream streamWriter = File.Create(fullZipToPath)) {
					StreamUtils.Copy(zipStream, streamWriter, buffer);
				}
			}
		} finally {
			if (zf != null) {
				zf.IsStreamOwner = true; // Makes close also shut the underlying stream
				zf.Close(); // Ensure we release resources
			}
		}
	}






	public static void extractZipFile(string archiveFilenameIn, string password, string outFolder, bool overwrite = true) 
	{
		ZipFile zf = null;
		try 
		{
			FileStream fs = File.OpenRead(archiveFilenameIn);

			zf = new ZipFile(fs);
			if (!String.IsNullOrEmpty(password)) 
			{
				zf.Password = password;     // AES encrypted entries are handled automatically
			}

			foreach (ZipEntry zipEntry in zf) 
			{
				if (!zipEntry.IsFile) 
				{
					continue;           // Ignore directories
				}
				String entryFileName = zipEntry.Name;
				// to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
				// Optionally match entrynames against a selection list here to skip as desired.
				// The unpacked length is available in the zipEntry.Size property.
				
				byte[] buffer = new byte[4096];     // 4K is optimum
				Stream zipStream = zf.GetInputStream(zipEntry);
				
				// Manipulate the output filename here as desired.
				String fullZipToPath = Path.Combine(outFolder, entryFileName);
				string directoryName = Path.GetDirectoryName(fullZipToPath);
				if (directoryName.Length > 0)
					Directory.CreateDirectory(directoryName);

				if(overwrite == false) if(File.Exists(fullZipToPath)) continue;

				// Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
				// of the file, but does not waste memory.
				// The "using" will close the stream even if an exception occurs.
				using (FileStream streamWriter = File.Create(fullZipToPath)) {
					StreamUtils.Copy(zipStream, streamWriter, buffer);
				}
			}
		} 
		finally 
		{
			if (zf != null) 
			{
				zf.IsStreamOwner = true; // Makes close also shut the underlying stream
				zf.Close(); // Ensure we release resources
			}
		}
	}









	public static string getEncryptFileName(string fileName)
	{
		// Use input string to calculate MD5 hash
		MD5 md5 = System.Security.Cryptography.MD5.Create();
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes (fileName);
		byte[] hashBytes  = md5.ComputeHash (inputBytes);
			
		// Convert the byte array to hexadecimal string
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < hashBytes.Length; i++)
		{
			sb.Append (hashBytes[i].ToString ("X2"));
		}

		return sb.ToString();
	}



	public static byte[] mergeByteArray(byte[] first, byte[] second)
	{
		byte[] ret = new byte[first.Length + second.Length];
		Buffer.BlockCopy(first, 0, ret, 0, first.Length);
		Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
		return ret;
	}
	
	public static byte[] mergeByteArray(byte[] first, byte[] second, byte[] third)
	{
		byte[] ret = new byte[first.Length + second.Length + third.Length];
		Buffer.BlockCopy(first, 0, ret, 0, first.Length);
		Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
		Buffer.BlockCopy(third, 0, ret, first.Length + second.Length, third.Length);
		return ret;
	}



	public static string getUnitIdWithoutEXPAndTranscendLevel(string id)
	{
		while(id.Contains("_"))
		{
			id = id.Substring(0,id.LastIndexOf("_"));
		}

		return id;
	}



	public static string getIdWithoutEXP(string id)
	{
		return id.Substring(0,id.LastIndexOf("_"));
	}


	public static int getStringContainCharacterCount(string source, char cha)
	{
		int count = 0;
		foreach (char c in source) 
		{
			if (c == cha) count++;
		}
			
		return count;
	}










	public static string getAesEncryptString(string original, string key, bool addKeyToHeader)
	{
		string result = "";

		using (Aes myAes = Aes.Create())
		{
			byte[] ivArray = myAes.IV;
			byte[] encrypted = EncryptStringToBytes_Aes(original, Encoding.ASCII.GetBytes( key ) , ivArray);
			result = ((addKeyToHeader)?Encoding.ASCII.GetString(ivArray):"")+ Encoding.UTF8.GetString(encrypted);
		}

		return result;
	}



	public static string getAesDecryptString(string original, string key, string iv)
	{
		string result = "";

		byte[] ivArray = Encoding.ASCII.GetBytes( iv );
		result = DecryptStringFromBytes_Aes( Encoding.UTF8.GetBytes(original), Encoding.ASCII.GetBytes(key), ivArray );

		return result;
	}



	static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
	{
		// Check arguments.
		if (plainText == null || plainText.Length <= 0)
			throw new ArgumentNullException("plainText");
		if (Key == null || Key.Length <= 0)
			throw new ArgumentNullException("Key");
		if (IV == null || IV.Length <= 0)
			throw new ArgumentNullException("Key");
		byte[] encrypted;
		// Create an AesCryptoServiceProvider object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			aesAlg.Key = Key;
			aesAlg.IV = IV;
			
			// Create a decrytor to perform the stream transform.
			ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
			
			// Create the streams used for encryption.
			using (MemoryStream msEncrypt = new MemoryStream())
			{
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
					{
						
						//Write all data to the stream.
						swEncrypt.Write(plainText);
					}
					encrypted = msEncrypt.ToArray();
				}
			}
		}
		
		
		// Return the encrypted bytes from the memory stream.
		return encrypted;
		
	}
	
	public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
	{
		// Check arguments.
		if (cipherText == null || cipherText.Length <= 0)
			throw new ArgumentNullException("cipherText");
		if (Key == null || Key.Length <= 0)
			throw new ArgumentNullException("Key");
		if (IV == null || IV.Length <= 0)
			throw new ArgumentNullException("IV");
		
		// Declare the string used to hold
		// the decrypted text.
		string plaintext = null;
		
		// Create an AesCryptoServiceProvider object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			aesAlg.Key = Key;
			aesAlg.IV = IV;
			aesAlg.Mode = CipherMode.CBC;
			aesAlg.Padding = PaddingMode.Zeros;
			
			// Create a decrytor to perform the stream transform.
			ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
			
			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream(cipherText))
			{
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader srDecrypt = new StreamReader(csDecrypt))
					{
						
						// Read the decrypted bytes from the decrypting stream
						// and place them in a string.
						plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
			
		}
		
		return plaintext;
		
	}


	public string descriptionAES128(string key, byte[] iv, byte[] source)
	{
		RijndaelManaged rijndaelCipher = new RijndaelManaged();
		rijndaelCipher.Mode = CipherMode.CBC;
		rijndaelCipher.Padding = PaddingMode.Zeros;
		
		rijndaelCipher.KeySize = 128;
		rijndaelCipher.BlockSize = 128;
		
		rijndaelCipher.Key = Encoding.UTF8.GetBytes(key);
		rijndaelCipher.IV = iv;
		byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(source, 0, source.Length);
		
		return Encoding.UTF8.GetString(plainText);
	}






	public String AESDecrypt128(String Input, String key)
	{
		RijndaelManaged RijndaelCipher = new RijndaelManaged();
		
		byte[] EncryptedData = Convert.FromBase64String(Input);
		byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());
		
		PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
		ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
		MemoryStream memoryStream = new MemoryStream(EncryptedData);
		CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
		
		byte[] PlainText = new byte[EncryptedData.Length];
		
		int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
		
		memoryStream.Close();
		cryptoStream.Close();
		
		string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
		
		return DecryptedData;
	}






	public static string encryptDesData(string str, string key)
	{
		byte[] key2 = Encoding.ASCII.GetBytes(key);
		//byte[] iv2 = Encoding.ASCII.GetBytes( str.Substring(0,8) );
		byte[] data = Encoding.UTF8.GetBytes(str);;

		DES des = DES.Create();

		des.GenerateIV();
		
		des.Key = key2;
		des.Mode = CipherMode.CBC;
		des.Padding = PaddingMode.Zeros;
		
		ICryptoTransform ict = des.CreateEncryptor();
		
		return System.Convert.ToBase64String(Util.mergeByteArray(des.IV, ict.TransformFinalBlock(data, 0, data.Length)));
	}
	
	public static string decryptDesData(string text, string _key)
	{
		byte[] source = Convert.FromBase64String(text);
		
		byte[] iv = new byte[8];
		byte[] body = new byte[source.Length-8];
		
		System.Array.Copy(source,iv,8);
		System.Array.Copy(source,8,body,0,source.Length-8);
		
		byte[] key = Encoding.ASCII.GetBytes(_key);

		DES des = DES.Create();
		des.IV = iv;
		des.Key = key;
		des.Mode = CipherMode.CBC;
		des.Padding = PaddingMode.Zeros;
		ICryptoTransform ict = des.CreateEncryptor();
		// decryption
		ict = des.CreateDecryptor();
		
		string result = Encoding.UTF8.GetString( ict.TransformFinalBlock(body, 0, body.Length) );
		
		return result;
	}













	public static string encryptTripleDesData(string str, string key)
	{
		byte[] key2 = Encoding.ASCII.GetBytes(key);
		//byte[] iv2 = Encoding.ASCII.GetBytes( str.Substring(0,8) );
		byte[] data = Encoding.UTF8.GetBytes(str);;

		TripleDES tdes = TripleDES.Create();

		tdes.GenerateIV();

		//tdes.IV = iv2;

		tdes.Key = key2;
		tdes.Mode = CipherMode.CBC;
		tdes.Padding = PaddingMode.Zeros;

		ICryptoTransform ict = tdes.CreateEncryptor();

		return System.Convert.ToBase64String(Util.mergeByteArray(tdes.IV, ict.TransformFinalBlock(data, 0, data.Length)));
	}
	
	public static string decryptTripleDesData(string text, string _key)
	{
		byte[] source = Convert.FromBase64String(text);

		byte[] iv = new byte[8];
		byte[] body = new byte[source.Length-8];

		System.Array.Copy(source,iv,8);
		System.Array.Copy(source,8,body,0,source.Length-8);

		byte[] key = Encoding.ASCII.GetBytes(_key);
//		byte[] iv = Encoding.ASCII.GetBytes(_iv);
		//byte[] data = Encoding.ASCII.GetBytes(str);

		TripleDES tdes = TripleDES.Create();
		tdes.IV = iv;
		tdes.Key = key;
		tdes.Mode = CipherMode.CBC;
		tdes.Padding = PaddingMode.Zeros;
		ICryptoTransform ict = tdes.CreateEncryptor();
		// decryption
		ict = tdes.CreateDecryptor();

		string result = Encoding.UTF8.GetString( ict.TransformFinalBlock(body, 0, body.Length) );

		return result;
	}



	public static string parseWemeSDKDate(string dateStr)
	{
		if(dateStr != null && dateStr.Length >= 12)
		{
			return dateStr.Substring(0,4) + "." + dateStr.Substring(4,2) + "." + dateStr.Substring(6,2) + " " + dateStr.Substring(8,2) + ":" + dateStr.Substring(10,2);
		}
		
		return "";
	}



	public static string getHangulWithJosa(string txt)
	{
		char[] chars = txt.ToCharArray();
		
		int code = (int)chars[chars.Length-1] - 44032;
		
		// 한글이 아닐때
		if (code < 0 || code > 11171) return txt;
		
		if (code % 28 == 0)
		{
			return txt + "를";
		}
		else
		{
			return txt + "을";
		}
		
		//		// jong : true면 받침있음, false면 받침없음
		//		if (josa == '을' || josa == '를') return (jong?'을':'를');
		//		if (josa == '이' || josa == '가') return (jong?'이':'가');
		//		if (josa == '은' || josa == '는') return (jong?'은':'는');
		//		if (josa == '와' || josa == '과') return (jong?'와':'과');
	}


	public static string getHangulJosa(string txt, string josa1 = "를", string josa2 = "을")
	{
		char[] chars = txt.ToCharArray();
		
		int code = (int)chars[chars.Length-1] - 44032;
		
		// 한글이 아닐때
		if (code < 0 || code > 11171) return string.Empty;
		
		if (code % 28 == 0)
		{
			return josa1;
		}
		else
		{
			return josa2;
		}
		
		//		// jong : true면 받침있음, false면 받침없음
		//		if (josa == '을' || josa == '를') return (jong?'을':'를');
		//		if (josa == '이' || josa == '가') return (jong?'이':'가');
		//		if (josa == '은' || josa == '는') return (jong?'은':'는');
		//		if (josa == '와' || josa == '과') return (jong?'와':'과');
	}



	public static int DateTimeToTimeStamp(DateTime theDate)
	{
		var timeSpan = (theDate - new DateTime(1970, 1, 1, 0, 0, 0));

//		Debug.Log("timeSpan.TotalSeconds : " + timeSpan.TotalSeconds);

		return (int)timeSpan.TotalSeconds;
	}

		
	public static DateTime TimeStampToDateTime(int timestamp)
	{
		System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

		return dateTime.AddSeconds(timestamp);
	}


	//1417337767000


	public static string stringJoin(string seperator, params string[] p)
	{
		stringBuilder.Length = 0;
		int len = p.Length;

		for(int i = 0; i < len ; ++i)
		{
			stringBuilder.Append(p[i]);
			if(i < len -1 )
			{
				stringBuilder.Append(seperator);
			}
		}

		return stringBuilder.ToString();
	}


	public static string stringJoin(string seperator, string[] p, int startIndex, int endIndex)
	{
		stringBuilder.Length = 0;

		for(int i = startIndex; i <= endIndex ; ++i)
		{
			stringBuilder.Append(p[i]);
			if(i < endIndex )
			{
				stringBuilder.Append(seperator);
			}
		}
		
		return stringBuilder.ToString();
	}




	public static string ConvertAosName(string tmpName)
	{
		if(tmpName == null) tmpName = "";
		
		byte[] unicodeNameBytes = Encoding.Unicode.GetBytes(tmpName);
		byte[] utfNameBytes = Encoding.Convert(Encoding.Unicode,Encoding.UTF32,unicodeNameBytes);
		tmpName = Encoding.UTF32.GetString(utfNameBytes);
		
		tmpName = tmpName.Replace("\0","");
		tmpName = tmpName.Trim();				
		
		return tmpName;
	}	






	
	


	public static int getTimeDiffBetweenServerAndClient(int serverTime)
	{
		return MathUtil.abs( Util.DateTimeToTimeStamp(DateTime.UtcNow) , serverTime);
	}


	public static int SizeOf<T>()
	{
		return System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
	}







	public static void openAppOrGoToMarket(string appName)
	{

		if(string.IsNullOrEmpty(appName)) return;

		#if UNITY_ANDROID && !UNITY_EDITOR
		
		try
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject pm = jo.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", appName);
			jo.Call("startActivity", intent);
		}
		catch(Exception e)
		{
			Application.OpenURL("market://details?id="+appName);
			Debug.LogError(e.Message);
		}
		
		#endif
	}





	public static byte[] stringToByteArray(String hex) 
	{ 
		int NumberChars = hex.Length; 
		byte[] bytes = new byte[NumberChars / 2]; 
		
		for (int i = 0; i < NumberChars; i += 2) 
			bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16); 
		
		return bytes; 
	}





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



	private static Quaternion _tq = new Quaternion();
	public static Quaternion getFixedQuaternion(Quaternion q)
	{
		_tq.eulerAngles = new IVector3(q.eulerAngles);
		return _tq;
	}

	private static Quaternion _tq2 = new Quaternion();
	public static Quaternion getFixedQuaternionSlerp(Quaternion from, Quaternion to, IFloat t)
	{
		_tq2.eulerAngles = new IVector3( Quaternion.Slerp(from, to, t).eulerAngles );
		return _tq2;
	}


	public static Color colorLerp(Color start, Color end, float time)
	{
		return new Color(
		Mathf.Lerp(start.r, end.r, time),
		Mathf.Lerp(start.g, end.g, time),
		Mathf.Lerp(start.b, end.b, time)
			);
	}



	public static void resizeEffect(GameObject go, string path, float scaleFactor)
	{
		ParticleSystem ps = go.particleSystem;

		if(ps == null)
		{
			string fuck = path.Substring(path.IndexOf("/")+1);
			Transform tf = go.transform.FindChild(fuck);
			//Transform tf = go.transform.GetChild(0);
			ps = tf.GetComponent<ParticleSystem>();
		}

		if(ps != null)
		{
			ps.transform.localScale = Vector3.one * scaleFactor;
		}

		ParticleSystem[] particles = go.transform.GetComponentsInChildren<ParticleSystem>();

		if(particles != null)
		{
			for(int i = 0; i < particles.Length ; ++i)
			{
				particles[i].startSize = particles[i].startSize * scaleFactor;
				particles[i].startSpeed = particles[i].startSpeed * scaleFactor;
			}
		}
	}


	public static T[] removeElementFromArray<T>(T[] array, int index)
	{
		List<T> temp = new List<T>();

		for(int i = 0; i < array.Length; ++i)
		{
			if(i != index) temp.Add(array[i]);
		}

		return temp.ToArray();
	}



	public static void setTranscendLevel(UILabel lb, int level = -1, GameObject container = null)
	{
		if(lb == null) return;

		if(level > 0)
		{
			lb.enabled = true;
			lb.text = "+[ffbf0d]"+level+"[-]";

			if(container != null) container.SetActive(true);
		}
		else
		{
			lb.enabled = false;

			if(container != null) container.SetActive(false);
		}
	}


}
