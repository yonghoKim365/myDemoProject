using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
sealed public class AniData
{
	public string id;
	public string ani;
	public string link;
	public int delayLength;
	public float[] delay;

	public AniDataEffect[] effect;
	public int effectNum = 0;

	public AniDataCamEffect[] camEffect;
	public int camEffectNum = 0;

	public AniDataSound[] sound;
	public int soundNum = 0;

	public string[] shootingPoint = null;
	public int shootingPointLength = 0;
	public int shootingHandLength = 0;

	public int[][] shootingPositions;

	public IFloat hitRange = 0.0f;

//	public IFloat aniLength = -1000;

	public AniData ()
	{
	}
	
	
	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		ani = (string)l[k["ANI"]];
		link = (string)l[k["LINK"]];
		link = link.Trim();

//		UnityEngine.Debug.Log(id);

		if(string.IsNullOrEmpty(link)) link = null;
		
		if(ani.Contains("atk") || ani.Contains("sk_"))
		{
			delay = Util.stringToFloatArray((l[k["FRAME"]].ToString()),',');
			delayLength = delay.Length;
			
			for(int i = 0; i < delayLength ; ++i)
			{
				delay[i] = delay[i]/30.0f;
			}				
		}

//		UnityEngine.Debug.Log(id);

		string temp = (string)l[k["EFFECT"]];
		if(temp.Length > 1)
		{
			string[] e = temp.Split('/');
			effectNum = e.Length;
			effect = new AniDataEffect[effectNum];

			for(int i = 0; i < effectNum; ++i)
			{
				effect[i] = new AniDataEffect();

				string[] tte = e[i].Split('@'); 

				if(tte.Length == 2)
				{
					effect[i].parent = tte[1];
					Debug.Log("parent: " + tte[1]);
				}

				string[] te = tte[0].Split(',');

				effect[i].id = te[0];


				effect[i].delay =  ((float)Convert.ToInt32(te[1]))/30.0f;

				effect[i].attachedToCharacter = false;

				switch(te[2])
				{
				case "S":
					effect[i].shotPoint = AniDataEffect.PointType.ShotPoint;

					if(te.Length == 4) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));

					break;
				case "C":
					effect[i].shotPoint = AniDataEffect.PointType.CustomPoint;

					if(te.Length > 5)
					{
						effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					}
					else
					{
						effect[i].x = 0; effect[i].y = 0; effect[i].z = 0;
					}

					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}

					break;
				case "CB":

					effect[i].shotPoint = AniDataEffect.PointType.CustomBullet;

					if(te.Length > 5)
					{
						effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					}
					else
					{
						effect[i].x = 0; effect[i].y = 0; effect[i].z = 0;
					}

					if(te.Length >= 7)
					{
						effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					}

					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));

					}


					break;
				case "T":
					effect[i].shotPoint = AniDataEffect.PointType.Target;

					if(te.Length > 5)
					{
						effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					}
					else
					{
						effect[i].x = 0; effect[i].y = 0; effect[i].z = 0;
					}


					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}

					break;

				case "AS":
					effect[i].shotPoint = AniDataEffect.PointType.AttachedShotPoint;

					if(te.Length > 5)
					{
						effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					}
					else
					{
						effect[i].x = 0; effect[i].y = 0; effect[i].z = 0;
					}


					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;


				case "AT":
					effect[i].shotPoint = AniDataEffect.PointType.AttachedTransform;

					if(te.Length > 5)
					{
						effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
						effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					}
					else
					{
						effect[i].x = 0; effect[i].y = 0; effect[i].z = 0;
					}


					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;



				default:
					effect[i].shotPoint = AniDataEffect.PointType.Character;
					break;
				}

				if(effect[i].timeLimit <= 0.01f) effect[i].timeLimit = 10000;

			}
		}
		else
		{
			effectNum = 0;
		}

	

		temp = (string)l[k["CAM"]];
		if(temp.Length > 1)
		{
			string[] c = temp.Split('/');
			camEffectNum = c.Length;
			camEffect = new AniDataCamEffect[camEffectNum];
			
			for(int i = 0; i < camEffectNum; ++i)
			{
				camEffect[i] = new AniDataCamEffect();
				string[] ce = c[i].Split(',');

				switch(ce[0])
				{
				case "E":
					camEffect[i].type = AniDataCamEffect.Type.Quake;
					break;
				}

				camEffect[i].delay = ((float)Convert.ToInt32(ce[1]))/30.0f;
				camEffect[i].optionValue = 0.0f;//(float)Convert.ToDouble(ce[2]);
				camEffect[i].optionValue2 = 0.0f;//(float)Convert.(ce[3]);

				float.TryParse(ce[2],out camEffect[i].optionValue);
				float.TryParse(ce[3],out camEffect[i].optionValue2);

			}
		}
		else
		{
			camEffectNum = 0;
		}



		string tempS = (string)l[k["SOUND"]];

		if(string.IsNullOrEmpty(tempS) == false)
		{
			string[] ts = tempS.Split('/');

			soundNum = ts.Length;

			sound = new AniDataSound[soundNum];

			for(int i = 0; i < soundNum; ++i)
			{
				sound[i] = new AniDataSound();

				string[] tts = ts[i].Split(',');

				sound[i].id = tts[0];

				float.TryParse(tts[1], out sound[i].delay);

				sound[i].delay /= 30.0f;

			}
		}
		else
		{
			soundNum = 0;
		}



		temp = (string)l[k["SHOT_POS"]];
		temp = temp.Trim();

//		UnityEngine.Debug.Log(id);

		if(string.IsNullOrEmpty(temp) == false)
		{
			string[] tmp = temp.Split('/');
			shootingPointLength = tmp.Length;
			shootingPositions = new int[tmp.Length][];

			for(int i = 0; i < shootingPointLength; ++i)
			{
				shootingPositions[i] = Util.stringToIntArray(tmp[i],',');

				shootingPositions[i][0] = (int)((float)shootingPositions[i][0] * ((float)GameManager.info.modelData[id].scale)/100.0f);
				shootingPositions[i][1] = (int)((float)shootingPositions[i][1] * ((float)GameManager.info.modelData[id].scale)/100.0f);
				shootingPositions[i][2] = (int)((float)shootingPositions[i][2] * ((float)GameManager.info.modelData[id].scale)/100.0f);
			}

			//shootingPoint = temp.Split(',');
			//shootingPointLength = shootingPoint.Length;
		}


		temp = (string)l[k["SHOOTING_POINT"]];
		temp = temp.Trim();
		
		if(string.IsNullOrEmpty(temp) == false)
		{
			shootingPoint = temp.Split(',');
			shootingHandLength = shootingPoint.Length;
		}

		Util.parseObject(l[k["HIT_RANGE"]],out hitRange, true, 0.0f);

#if UNITY_EDITOR
		if(GameManager.info.modelData.ContainsKey(id) == false)
		{
			UnityEngine.Debug.Log(id);
		}
#endif

		hitRange *= ((((float)GameManager.info.modelData[id].scale)/100.0f));

//		Util.parseObject(l[k["LENGTH"]],out aniLength, true, -1000);

	}	
















	public void setData(string[] f)
	{

		string[] d = new string[17];

		for(int i = 0; i < d.Length; ++i)
		{
			d[i] = string.Empty;
		}

		for(int i = 0; i < f.Length && i < d.Length; ++i)
		{
			d[i] = f[i];
		}



		id = d[3];
		ani = d[4];
		link = d[5];
		link = link.Trim();
		
		if(string.IsNullOrEmpty(d[6])) link = null;
		
		if(ani.Contains("atk") || ani.Contains("sk_"))
		{
			delay = Util.stringToFloatArray( (d[7]) ,',');
			delayLength = delay.Length;
			
			for(int i = 0; i < delayLength ; ++i)
			{
				delay[i] = delay[i]/30.0f;
			}				
		}
		
		//		UnityEngine.Debug.Log(id);
		
		string temp = d[8];
		if(temp.Length > 1)
		{
			string[] e = temp.Split('/');
			effectNum = e.Length;
			effect = new AniDataEffect[effectNum];
			
			for(int i = 0; i < effectNum; ++i)
			{
				effect[i] = new AniDataEffect();

				string[] tte = e[i].Split('@'); 
				
				if(tte.Length == 2)
				{
					effect[i].parent = tte[1];
				}
				
				string[] te = tte[0].Split(',');

				effect[i].id = te[0];
				effect[i].delay =  ((float)Convert.ToInt32(te[1]))/30.0f;
				
				effect[i].attachedToCharacter = false;
				
				switch(te[2])
				{
				case "S":
					effect[i].shotPoint = AniDataEffect.PointType.ShotPoint;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					
					break;
				case "C":
					effect[i].shotPoint = AniDataEffect.PointType.CustomPoint;
					effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;
				case "CB":
					effect[i].shotPoint = AniDataEffect.PointType.CustomBullet;
					effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					
					break;
				case "T":
					effect[i].shotPoint = AniDataEffect.PointType.Target;
					effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;
					
				case "AS":
					effect[i].shotPoint = AniDataEffect.PointType.AttachedShotPoint;
					effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;
					
					
				case "AT":
					effect[i].shotPoint = AniDataEffect.PointType.AttachedTransform;
					effect[i].x = ((float)Convert.ToInt32(te[3])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].y = ((float)Convert.ToInt32(te[4])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					effect[i].z = ((float)Convert.ToInt32(te[5])) * ((float)GameManager.info.modelData[id].scale)/100.0f;
					
					if(te.Length >= 7) effect[i].timeLimit = ((float)Convert.ToDouble(te[6]));
					if(te.Length >= 10)
					{
						effect[i].useCustomRotation = true;
						effect[i].rx = ((float)Convert.ToInt32(te[7]));
						effect[i].ry = ((float)Convert.ToInt32(te[8]));
						effect[i].rz = ((float)Convert.ToInt32(te[9]));
					}
					
					break;
					
					
					
				default:
					effect[i].shotPoint = AniDataEffect.PointType.Character;
					break;
				}
				
				if(effect[i].timeLimit <= 0.01f) effect[i].timeLimit = 10000;
				
			}
		}
		else
		{
			effectNum = 0;
		}
		
		
		temp = d[10];
		if(temp.Length > 1)
		{
			string[] c = temp.Split('/');
			camEffectNum = c.Length;
			camEffect = new AniDataCamEffect[camEffectNum];
			
			for(int i = 0; i < camEffectNum; ++i)
			{
				camEffect[i] = new AniDataCamEffect();
				string[] ce = c[i].Split(',');
				
				switch(ce[0])
				{
				case "E":
					camEffect[i].type = AniDataCamEffect.Type.Quake;
					break;
				}
				
				camEffect[i].delay = ((float)Convert.ToInt32(ce[1]))/30.0f;
				camEffect[i].optionValue = 0.0f;//(float)Convert.ToDouble(ce[2]);
				camEffect[i].optionValue2 = 0.0f;//(float)Convert.(ce[3]);
				
				float.TryParse(ce[2],out camEffect[i].optionValue);
				float.TryParse(ce[3],out camEffect[i].optionValue2);
				
			}
		}
		else
		{
			camEffectNum = 0;
		}



		string tempS = d[11];
		
		if(string.IsNullOrEmpty(tempS) == false)
		{
			string[] ts = tempS.Split('/');
			
			soundNum = ts.Length;
			
			sound = new AniDataSound[soundNum];
			
			for(int i = 0; i < soundNum; ++i)
			{
				sound[i] = new AniDataSound();
				
				string[] tts = ts[i].Split(',');
				
				sound[i].id = tts[0];
				
				float.TryParse(tts[1], out sound[i].delay);
				
				sound[i].delay /= 30.0f;
				
			}
		}
		else
		{
			soundNum = 0;
		}




		temp = d[13];
		temp = temp.Trim();
		
		//		UnityEngine.Debug.Log(id);
		
		if(string.IsNullOrEmpty(temp) == false)
		{
			string[] tmp = temp.Split('/');
			shootingPointLength = tmp.Length;
			shootingPositions = new int[tmp.Length][];
			
			for(int i = 0; i < shootingPointLength; ++i)
			{
				shootingPositions[i] = Util.stringToIntArray(tmp[i],',');
				
//				shootingPositions[i][0] = (int)((float)shootingPositions[i][0] * ((float)GameManager.info.modelData[id].scale)/100.0f);
//				shootingPositions[i][1] = (int)((float)shootingPositions[i][1] * ((float)GameManager.info.modelData[id].scale)/100.0f);
//				shootingPositions[i][2] = (int)((float)shootingPositions[i][2] * ((float)GameManager.info.modelData[id].scale)/100.0f);

				shootingPositions[i][0] = Mathf.FloorToInt((float)shootingPositions[i][0] * ((float)GameManager.info.modelData[id].scale)/100.0f);
				shootingPositions[i][1] = Mathf.FloorToInt((float)shootingPositions[i][1] * ((float)GameManager.info.modelData[id].scale)/100.0f);
				shootingPositions[i][2] = Mathf.FloorToInt((float)shootingPositions[i][2] * ((float)GameManager.info.modelData[id].scale)/100.0f);
			}
			
			//shootingPoint = temp.Split(',');
			//shootingPointLength = shootingPoint.Length;
		}
		
		
		temp = d[14];
		temp = temp.Trim();
		
		if(string.IsNullOrEmpty(temp) == false)
		{
			shootingPoint = temp.Split(',');
			shootingHandLength = shootingPoint.Length;
		}

		Util.parseObject(d[15],out hitRange, true, 0.0f);
		
//		UnityEngine.Debug.Log(id);
		hitRange *= ((((float)GameManager.info.modelData[id].scale)/100.0f));


//		Util.parseObject(d[16],out aniLength, true, -1000);

	}	
}



public class AniDataSound
{
	public string id;
	public float delay;
}


[System.Serializable]
public class AniDataEffect
{
	public string id;

	public float delay;

	public float timeLimit = 10000;

	public enum PointType
	{
		Character, ShotPoint, CustomPoint, CustomBullet, Target, AttachedShotPoint, AttachedTransform
	}

	public PointType shotPoint;

	public bool attachedToCharacter;

	public string parent = null;

	public float x,y,z;

	public bool useCustomRotation = false;

	public float rx, ry, rz;

	public string getLogicString(float scale)
	{
#if UNITY_EDITOR
		string str = "E_"+goEffect.name.ToString().ToUpper();
		str += ","+frame+",";

		string pos = "";

		if(MathUtil.RoundToInt(x) != 0 || MathUtil.RoundToInt(y) != 0 || MathUtil.RoundToInt(z) != 0)
		{
			pos = ","+MathUtil.RoundToInt(x / scale).ToString()+","+MathUtil.RoundToInt(y / scale).ToString()+","+MathUtil.RoundToInt(z / scale).ToString();
		}
		else
		{
			pos = ",0,0,0";
		}

		string t = MathUtil.RoundToInt(timeLimit).ToString();

		if(timeLimit > 0 && timeLimit < 100)
		{
			pos += "," + t;
		}
		else
		{
			pos += ",10000";
		}

		string rot = "";

		if(MathUtil.RoundToInt(rx) != 0 || MathUtil.RoundToInt(ry) != 0 || MathUtil.RoundToInt(rz) != 0)
		{
			rot = ","+MathUtil.RoundToInt(rx / scale).ToString()+","+MathUtil.RoundToInt(ry / scale).ToString()+","+MathUtil.RoundToInt(rz / scale).ToString();
		}




		switch(shotPoint)
		{
		case PointType.Character: 
			str += "D"; 
			break;
		case PointType.ShotPoint: 
			str += "S," + t; 
			break;
		case PointType.CustomPoint: 
			str += "C" + pos + rot;
			break;
		case PointType.CustomBullet: 
			str += "CB" + pos + rot;
			break;
		case PointType.Target: 
			str += "T" + pos + rot;
			break;
		case PointType.AttachedShotPoint: 
			str += "AS" + pos + rot;
			break;
		case PointType.AttachedTransform: 
			str += "AT" + pos + rot + "@"+parent;
			break;
		}

		return str;
#endif

		return null;
	}




	public void setLogicString(float scale, string[] logic)
	{
		if(logic == null ) return;

		string[] temp = logic[logic.Length-1].Split('@');
		logic[logic.Length-1] = temp[0];

		scale = scale / 100.0f; //scale = scale * 0.01f;

		#if UNITY_EDITOR

		int.TryParse(logic[1], out frame);
		delay = ((float)frame)/30.0f;

		if(logic.Length >= 6)
		{
			float.TryParse(logic[3], out x);
			float.TryParse(logic[4], out y);
			float.TryParse(logic[5], out z);

			x *= scale;
			y *= scale;
			z *= scale;

			x = MathUtil.RoundToInt(x);
			y = MathUtil.RoundToInt(y);
			z = MathUtil.RoundToInt(z);
		}

		if(logic.Length >= 7)
		{
			float.TryParse(logic[6], out timeLimit);
		}


		if(logic.Length >= 10)
		{
			float.TryParse(logic[7], out rx);
			float.TryParse(logic[8], out ry);
			float.TryParse(logic[9], out rz);
			
			rx *= scale;
			ry *= scale;
			rz *= scale;

			rx = MathUtil.RoundToInt(rx);
			ry = MathUtil.RoundToInt(ry);
			rz = MathUtil.RoundToInt(rz);

			if(MathUtil.RoundToInt(rx) != 0 ||
			   MathUtil.RoundToInt(ry) != 0 ||
			   MathUtil.RoundToInt(rz) != 0)
			{
				useCustomRotation = true;
			}
			else
			{
				useCustomRotation = false;
			}

		}

		switch(logic[2])
		{
		case "D":
			shotPoint = PointType.Character;
			break;
		case "S": 
			shotPoint = PointType.ShotPoint;
			break;
		case "C": 
			shotPoint = PointType.CustomPoint;
			break;
		case "CB":
			shotPoint = PointType.CustomBullet;
			break;
		case "T":
			shotPoint = PointType.Target;
			break;
		case "AS": 
			shotPoint = PointType.AttachedShotPoint;
			break;
		case "AT": 
			shotPoint = PointType.AttachedTransform;
			parent = temp[1];
			break;
		}
		#endif
	}







#if UNITY_EDITOR
	public UnityEngine.GameObject goEffect;

	public enum EffectType
	{
		None, Particle, MeshAnimation, WrongName
	}

	public EffectType checkType = EffectType.None;

	public bool useThis = true;

	public int frame = 1;

	public float timeLimitDefault = 10000;

	public static int sortByFrame(AniDataEffect x, AniDataEffect y)
	{
		return x.frame.CompareTo(y.frame);
	}
#endif





}


public class AniDataCamEffect
{
	public enum Type
	{
		Quake
	}
	public Type type;

	public float delay;

	public float optionValue;

	public float optionValue2;

}


