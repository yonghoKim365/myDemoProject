using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Local
{
    [Serializable]
    public class RandomNameInfo
    {
        public uint Index;
        public string Name1;
        public string Name2;
        public string Name3;
    }
   
    [Serializable]
    public class StageDataInfo
    {
        public uint StringId;
        public string String;
    }
    
    [Serializable]
    public class StringBuffInfo
    {
        public uint StringId;
        public string String;
    }
    
    [Serializable]
    public class StringCommonInfo
    {
        public uint StringId;
        public string String;
    }
    
    [Serializable]
    public class StringItemInfo
    {
        public uint StringId;
        public string String;
    }
    
    [Serializable]
    public class StringSkillInfo
    {
        public uint StringId;
        public string String;
    }

    [Serializable]
    public class StringUnitInfo
    {
        public uint StringId;
        public string String;
    }
    
    [Serializable]
    public class StringTitleInfo
    {
        public uint StringId;
        public string String;
    }
    [Serializable]
    public class StringAchievementInfo
    {
        public uint StringId;
        public string String;
    }

    [Serializable]
    public class ErrorPopupInfo
    {
        public uint Index;
        public string Description;
    }

    [Serializable]
    public class BanInfo
    {
        public uint ID;
        public string word;
    }

	[Serializable]
	public class StringLocalDialogInfo
	{
		public uint StringId;
		public string String;
		public string NPCNameLEFT;
		public string NPCNameRIGHT;
		public byte TalkPosition;
	}
	



    public Dictionary<uint, StringTitleInfo> StringTitleInfoDic = new Dictionary<uint, StringTitleInfo>();
    public Dictionary<uint, RandomNameInfo> RandomNameInfoDic = new Dictionary<uint, RandomNameInfo>();
    public Dictionary<uint, StageDataInfo> StageDataInfoDic = new Dictionary<uint, StageDataInfo>();
    public Dictionary<uint, StringBuffInfo> StringBuffInfoDic = new Dictionary<uint, StringBuffInfo>();
    public Dictionary<uint, StringCommonInfo> StringCommonInfoDic = new Dictionary<uint, StringCommonInfo>();
    public Dictionary<uint, StringItemInfo> StringItemInfoDic = new Dictionary<uint, StringItemInfo>();
    public Dictionary<uint, StringSkillInfo> StringSkillInfoDic = new Dictionary<uint, StringSkillInfo>();
    public Dictionary<uint, StringUnitInfo> StringUnitInfoDic = new Dictionary<uint, StringUnitInfo>();
    public Dictionary<uint, StringAchievementInfo> StringAchievementInfoDic = new Dictionary<uint, StringAchievementInfo>();
    public Dictionary<uint, ErrorPopupInfo> ErrorPopupInfoDic = new Dictionary<uint, ErrorPopupInfo>();
    public List<BanInfo> BanInfoList = new List<BanInfo>();
	public Dictionary<uint, StringLocalDialogInfo> StringLocalDialogInfoDic = new Dictionary<uint, StringLocalDialogInfo>();


    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Local_RandomName", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject RandomName = new JSONObject(strSrc);

            for (int i = 0; i < RandomName.list.Count; i++)
            {
                RandomNameInfo tmpInfo = new RandomNameInfo();
                tmpInfo.Index = (uint)RandomName[i]["Index_ui"].n;
                tmpInfo.Name1 = RandomName[i]["Name1_c"].str;
                tmpInfo.Name2 = RandomName[i]["Name2_c"].str;
                tmpInfo.Name3 = RandomName[i]["Name3_c"].str;

                RandomNameInfoDic.Add(tmpInfo.Index, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StageData", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StageData = new JSONObject(strSrc);

            for (int i = 0; i < StageData.list.Count; i++)
            {
                StageDataInfo tmpInfo = new StageDataInfo();
                tmpInfo.StringId = (uint)StageData[i]["StringId_ui"].n;
                tmpInfo.String = StageData[i]["String_c"].str;

                string kr_c = StageData[i]["String_c"].str;

                if (kr_c.Contains("\\\\n"))
                    kr_c = kr_c.Replace("\\\\n", "\n");
                else if (kr_c.Contains("\\n"))
                    kr_c = kr_c.Replace("\\n", "\n");

                tmpInfo.String = kr_c;


                StageDataInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringBuff", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringBuff = new JSONObject(strSrc);

            for (int i = 0; i < StringBuff.list.Count; i++)
            {
                StringBuffInfo tmpInfo = new StringBuffInfo();
                tmpInfo.StringId = (uint)StringBuff[i]["StringId_ui"].n;
                tmpInfo.String = StringBuff[i]["String_c"].str;

                StringBuffInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringCommon", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringCommon = new JSONObject(strSrc);

            for (int i = 0; i < StringCommon.list.Count; i++)
            {
                StringCommonInfo tmpInfo = new StringCommonInfo();
                tmpInfo.StringId = (uint)StringCommon[i]["StringId_ui"].n;

                /// 스트링에 엔터값이 잘못 들어간다 제거해준다
                string kr_c = StringCommon[i]["String_c"].str;

                if (kr_c.Contains("\\\\n"))
                    kr_c = kr_c.Replace("\\\\n", "\n");
                else if (kr_c.Contains("\\n"))
                    kr_c = kr_c.Replace("\\n", "\n");
                tmpInfo.String = kr_c;
                ////
                //tmpInfo.String = StringCommon[i]["String_c"].str;

                StringCommonInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringItem", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringItem = new JSONObject(strSrc);

            for (int i = 0; i < StringItem.list.Count; i++)
            {
                StringItemInfo tmpInfo = new StringItemInfo();
                tmpInfo.StringId = (uint)StringItem[i]["StringId_ui"].n;
                tmpInfo.String = StringItem[i]["String_c"].str;

                StringItemInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringSkill", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringSkill = new JSONObject(strSrc);

            for (int i = 0; i < StringSkill.list.Count; i++)
            {
                StringSkillInfo tmpInfo = new StringSkillInfo();
                tmpInfo.StringId = (uint)StringSkill[i]["StringId_ui"].n;
                tmpInfo.String = StringSkill[i]["String_c"].str;

                StringSkillInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringUnit", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringUnit = new JSONObject(strSrc);

            for (int i = 0; i < StringUnit.list.Count; i++)
            {
                StringUnitInfo tmpInfo = new StringUnitInfo();
                tmpInfo.StringId = (uint)StringUnit[i]["StringId_ui"].n;
                //tmpInfo.String = StringUnit[i]["String_c"].str;
                /// 스트링에 엔터값이 잘못 들어간다 제거해준다
                string kr_c = StringUnit[i]["String_c"].str;

                if (kr_c.Contains("\\\\n"))
                    kr_c = kr_c.Replace("\\\\n", "\n");
                else if (kr_c.Contains("\\n"))
                    kr_c = kr_c.Replace("\\n", "\n");
                tmpInfo.String = kr_c;

                StringUnitInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        
        {
            TextAsset data = Resources.Load("TestJson/Local_StringTitle", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringTitle = new JSONObject(strSrc);

            for (int i = 0; i < StringTitle.list.Count; i++)
            {
                StringTitleInfo tmpInfo = new StringTitleInfo();
                tmpInfo.StringId = (uint)StringTitle[i]["StringId_ui"].n;
                tmpInfo.String = StringTitle[i]["String_c"].str;

                StringTitleInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_StringAchievement", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject StringAchievement = new JSONObject(strSrc);

            for (int i = 0; i < StringAchievement.list.Count; i++)
            {
                StringAchievementInfo tmpInfo = new StringAchievementInfo();
                tmpInfo.StringId = (uint)StringAchievement[i]["StringId_ui"].n;
                tmpInfo.String = StringAchievement[i]["String_c"].str;

                StringAchievementInfoDic.Add(tmpInfo.StringId, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Local_ErrorPopup", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject ErrorPopup = new JSONObject(strSrc);

            for (int i = 0; i < ErrorPopup.list.Count; i++)
            {
                ErrorPopupInfo tmpInfo = new ErrorPopupInfo();
                tmpInfo.Index = (uint)ErrorPopup[i]["Index_ui"].n;

                string str = ErrorPopup[i]["Description_c"].str;
                if (str.Contains("\\\\n"))
                    str = str.Replace("\\\\n", "\n");
                else if (str.Contains("\\n"))
                    str = str.Replace("\\n", "\n");

                tmpInfo.Description = str;

                if (!ErrorPopupInfoDic.ContainsKey(tmpInfo.Index))
                    ErrorPopupInfoDic.Add(tmpInfo.Index, tmpInfo);
                else
                    Debug.LogError("already added index " + tmpInfo.Index);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Local_Ban", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Ban = new JSONObject(strSrc);

            for (int i = 0; i < Ban.list.Count; i++)
            {
                BanInfo tmpInfo = new BanInfo();
                tmpInfo.ID = (uint)Ban[i]["ID_ui"].n;
                tmpInfo.word = Ban[i]["word_c"].str;

                BanInfoList.Add(tmpInfo);
            }
        }
		{
			TextAsset data = Resources.Load("TestJson/Local_Dialog", typeof(TextAsset)) as TextAsset;
			StringReader sr = new StringReader(data.text);
			string strSrc = sr.ReadToEnd();
			JSONObject StringCommon = new JSONObject(strSrc);
			
			for (int i = 0; i < StringCommon.list.Count; i++)
			{
				StringLocalDialogInfo tmpInfo = new StringLocalDialogInfo();
				tmpInfo.StringId = (uint)StringCommon[i]["StringId_ui"].n;
				tmpInfo.NPCNameLEFT = StringCommon[i]["NPCNameLEFT_c"].str;
				tmpInfo.NPCNameRIGHT = StringCommon[i]["NPCNameRIGHT_c"].str;
				tmpInfo.TalkPosition = (byte)StringCommon[i]["TalkPosition_b"].n;
				tmpInfo.String = StringCommon[i]["String_c"].str;

				//Debug.Log(" string :"+tmpInfo.String+", TalkPosition:"+tmpInfo.TalkPosition);

				StringLocalDialogInfoDic.Add(tmpInfo.StringId, tmpInfo);
			}
		}

    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/RandomNameInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, RandomNameInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StageDataInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StageDataInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringBuffInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringBuffInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringCommonInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringCommonInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringItemInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringItemInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringSkillInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringSkillInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringUnitInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringUnitInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringTitleInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringTitleInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/StringAchievementInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, StringAchievementInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/ErrorPopupInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, ErrorPopupInfoDic);
            fs.Close();
        }
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/BanInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, BanInfoList);
            fs.Close();
        }
		{
			FileStream fs = new FileStream("Assets/Resources/SerializeData/StringLocalDialogInfo.txt", FileMode.Create, FileAccess.Write);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, StringLocalDialogInfoDic);
			fs.Close();
		}
		
	}
	
	public void DeserializeData()
	{

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, RandomNameInfo>>("RandomNameInfo", (data) =>
		{
			RandomNameInfoDic = data;
		});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StageDataInfo>>("StageDataInfo", (data) =>
		{
			StageDataInfoDic = data;
		});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringBuffInfo>> ("StringBuffInfo", (data) =>
		{
			StringBuffInfoDic = data;
		});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringCommonInfo>> ("StringCommonInfo", (data) => {
			StringCommonInfoDic = data;
		});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringItemInfo>>("StringItemInfo", (data) => { StringItemInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringSkillInfo>>("StringSkillInfo", (data) => { StringSkillInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringUnitInfo>>("StringUnitInfo", (data) => { StringUnitInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringTitleInfo>>("StringTitleInfo", (data) => { StringTitleInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringAchievementInfo>>("StringAchievementInfo", (data) => { StringAchievementInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, ErrorPopupInfo>>("ErrorPopupInfo", (data) => { ErrorPopupInfoDic = data;});
		_LowDataMgr.instance.DeserializeData<List<BanInfo>>("BanInfo", (data) => { BanInfoList = data;});
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, StringLocalDialogInfo>> ("StringLocalDialogInfo", (data) => {
			StringLocalDialogInfoDic = data;
		});
    }
}
