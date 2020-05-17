using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Quest
{
    [Serializable]
    public class QuestInfo
    {
        public uint ID;
        byte _type;
        public byte type
        {
            set { _type = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_type); }
        }
        uint _sort;
        public uint sort
        {
            set { _sort = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_sort); }
        }
        byte _LimitLevel;
        public byte LimitLevel
        {
            set { _LimitLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_LimitLevel); }
        }
        uint _BeforeQuestId;
        public uint BeforeQuestId
        {
            set { _BeforeQuestId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_BeforeQuestId); }
        }
        uint _NextQuestId;
        public uint NextQuestId
        {
            set { _NextQuestId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextQuestId); }
        }
        uint _icon;
        public uint icon
        {
            set { _icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_icon); }
        }
        byte _QuestType;
        public byte QuestType
        {
            set { _QuestType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_QuestType); }
        }
        byte _StageType;
        public byte StageType
        {
            set { _StageType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_StageType); }
        }
        uint _ParamId;
        public uint ParamId
        {
            set { _ParamId = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ParamId); }
        }
        byte _value;
        public byte value
        {
            set { _value = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_value); }
        }
        uint _QuestTalkSceneID;
        public uint QuestTalkSceneID
        {
            set { _QuestTalkSceneID = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QuestTalkSceneID); }
        }
        public string Title;
        public string LeftDescription;
        public string Description;
        uint _rewardGold;
        public uint rewardGold
        {
            set { _rewardGold = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardGold); }
        }
        uint _rewardExp;
        public uint rewardExp
        {
            set { _rewardExp = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardExp); }
        }
        uint _rewardEnergy;
        public uint rewardEnergy
        {
            set { _rewardEnergy = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardEnergy); }
        }
        uint _rewardItem;
        public uint rewardItem
        {
            set { _rewardItem = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_rewardItem); }
        }
    }
    
    [Serializable]
    public class QuestTalkSceneInfo
    {
        public uint SceneID;
        uint _Sequence;
        public uint Sequence
        {
            set { _Sequence = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Sequence); }
        }
        public string NPCModelLEFT;
        public string NPCNameLEFT;
        public string NPCModelRIGHT;
        public string NPCNameRIGHT;
        byte _TalkPosition;
        public byte TalkPosition
        {
            set { _TalkPosition = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_TalkPosition); }
        }
        public string TalkString;
    }

    [Serializable]
    public class TutorialInfo
    {
        public uint ID;
        uint _Group;
        public uint Group
        {
            set { _Group = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Group); }
        }
        uint _Sort;
        public uint Sort
        {
            set { _Sort = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Sort); }
        }
        byte _OpenLevel;
        public byte OpenLevel
        {
            set { _OpenLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_OpenLevel); }
        }
        uint _NextTutorial;
        public uint NextTutorial
        {
            set { _NextTutorial = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextTutorial); }
        }
        uint _Icon;
        public uint Icon
        {
            set { _Icon = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Icon); }
        }
        uint _QuestTalkSceneID;
        public uint QuestTalkSceneID
        {
            set { _QuestTalkSceneID = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_QuestTalkSceneID); }
        }
        uint _SubTalkSceneID;
        public uint SubTalkSceneID
        {
            set { _SubTalkSceneID = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_SubTalkSceneID); }
        }
        public string Title;
        public string LeftDescription;
        public string Description;
        byte _ProgressType;
        public byte ProgressType
        {
            set { _ProgressType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ProgressType); }
        }
    }

    [Serializable]
    public class SubTalkSceneInfo
    {
        public uint SceneID;
        uint _Sequence;
        public uint Sequence
        {
            set { _Sequence = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Sequence); }
        }
        public string NpcFace;
        byte _Position;
        public byte Position
        {
            set { _Position = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_Position); }
        }
        uint _RemoveType;
        public uint RemoveType
        {
            set { _RemoveType = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RemoveType); }
        }
        uint _RemoveTime;
        public uint RemoveTime
        {
            set { _RemoveTime = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_RemoveTime); }
        }
        public string TalkString;
    }

    [Serializable]
    public class MainTutorialInfo
    {
        public uint ID;
        uint _Group;
        public uint Group
        {
            set { _Group = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Group); }
        }
        uint _Sequence;
        public uint Sequence
        {
            set { _Sequence = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_Sequence); }
        }
        byte _OpenLevel;
        public byte OpenLevel
        {
            set { _OpenLevel = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_OpenLevel); }
        }
        uint _NextTutorial;
        public uint NextTutorial
        {
            set { _NextTutorial = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_NextTutorial); }
        }
        byte _OpenType;
        public byte OpenType
        {
            set { _OpenType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_OpenType); }
        }
        public string OpenMessage;
        byte _ProgressType;
        public byte ProgressType
        {
            set { _ProgressType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_ProgressType); }
        }
        byte _GuideType;
        public byte GuideType
        {
            set { _GuideType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuideType); }
        }
        byte _RemoveType;
        public byte RemoveType
        {
            set { _RemoveType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_RemoveType); }
        }
        byte _GuideCount;
        public byte GuideCount
        {
            set { _GuideCount = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_GuideCount); }
        }
        public string GuideText1;
        public string GuideText2;
        public string GuideText3;
        public string GuideText4;
        public string GuideText5;
    }

    public List<QuestTalkSceneInfo> QuestTalkSceneInfoList = new List<QuestTalkSceneInfo>();
    public Dictionary<uint, QuestInfo> QuestInfoDic = new Dictionary<uint, QuestInfo>();
    public Dictionary<uint, TutorialInfo> TutorialInfoDic = new Dictionary<uint, TutorialInfo>();
    public List<SubTalkSceneInfo> SubTalkSceneInfoList = new List<SubTalkSceneInfo>();
    public Dictionary<uint, MainTutorialInfo> MainTutorialInfoDic = new Dictionary<uint, MainTutorialInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Quest_Quest", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Quest = new JSONObject(strSrc);

            for (int i = 0; i < Quest.list.Count; i++)
            {
                QuestInfo tmpInfo = new QuestInfo();
                tmpInfo.ID = (uint)Quest[i]["ID_ui"].n;
                tmpInfo.type = (byte)Quest[i]["type_b"].n;
                tmpInfo.sort = (uint)Quest[i]["sort_ui"].n;
                tmpInfo.LimitLevel = (byte)Quest[i]["LimitLevel_b"].n;
                tmpInfo.BeforeQuestId = (uint)Quest[i]["BeforeQuestId_ui"].n;
                tmpInfo.NextQuestId = (uint)Quest[i]["NextQuestId_ui"].n;
                tmpInfo.icon = (uint)Quest[i]["icon_ui"].n;
                tmpInfo.QuestType = (byte)Quest[i]["QuestType_b"].n;
                tmpInfo.StageType = (byte)Quest[i]["StageType_b"].n;
                tmpInfo.ParamId = (uint)Quest[i]["ParamId_ui"].n;
                tmpInfo.value = (byte)Quest[i]["value_b"].n;
                tmpInfo.QuestTalkSceneID = (uint)Quest[i]["QuestTalkSceneID_ui"].n;
                tmpInfo.Title = Quest[i]["Title_c"].str;
                tmpInfo.LeftDescription = Quest[i]["LeftDescription_c"].str;
                tmpInfo.Description = Quest[i]["Description_c"].str;
                tmpInfo.rewardGold = (uint)Quest[i]["rewardGold_ui"].n;
                tmpInfo.rewardExp = (uint)Quest[i]["rewardExp_ui"].n;
                tmpInfo.rewardEnergy = (uint)Quest[i]["rewardEnergy_ui"].n;
                tmpInfo.rewardItem = (uint)Quest[i]["rewardItem_ui"].n;

                QuestInfoDic.Add(tmpInfo.ID, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Quest_QuestTalkScene", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject QuestTalkScene = new JSONObject(strSrc);

            for (int i = 0; i < QuestTalkScene.list.Count; i++)
            {
                QuestTalkSceneInfo tmpInfo = new QuestTalkSceneInfo();
                tmpInfo.SceneID = (uint)QuestTalkScene[i]["SceneID_ui"].n;
                tmpInfo.Sequence = (uint)QuestTalkScene[i]["Sequence_ui"].n;
                tmpInfo.NPCModelLEFT = QuestTalkScene[i]["NPCModelLEFT_c"].str;
                tmpInfo.NPCNameLEFT = QuestTalkScene[i]["NPCNameLEFT_c"].str;
                tmpInfo.NPCModelRIGHT = QuestTalkScene[i]["NPCModelRIGHT_c"].str;
                tmpInfo.NPCNameRIGHT = QuestTalkScene[i]["NPCNameRIGHT_c"].str;
                tmpInfo.TalkPosition = (byte)QuestTalkScene[i]["TalkPosition_b"].n;
                tmpInfo.TalkString = QuestTalkScene[i]["TalkString_c"].str;

                QuestTalkSceneInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Quest_Tutorial", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Tutorial = new JSONObject(strSrc);

            for (int i = 0; i < Tutorial.list.Count; i++)
            {
                TutorialInfo tmpInfo = new TutorialInfo();
                tmpInfo.ID = (uint)Tutorial[i]["ID_ui"].n;
                tmpInfo.Group = (uint)Tutorial[i]["Group_ui"].n;
                tmpInfo.Sort = (uint)Tutorial[i]["Sort_ui"].n;
                tmpInfo.OpenLevel = (byte)Tutorial[i]["OpenLevel_b"].n;
                tmpInfo.NextTutorial = (uint)Tutorial[i]["NextTutorial_ui"].n;
                tmpInfo.Icon = (uint)Tutorial[i]["Icon_ui"].n;
                tmpInfo.QuestTalkSceneID = (uint)Tutorial[i]["QuestTalkSceneID_ui"].n;
                tmpInfo.SubTalkSceneID = (uint)Tutorial[i]["SubTalkSceneID_ui"].n;
                tmpInfo.Title = Tutorial[i]["Title_c"].str;
                tmpInfo.LeftDescription = Tutorial[i]["LeftDescription_c"].str;
                tmpInfo.Description = Tutorial[i]["Description_c"].str;
                tmpInfo.ProgressType = (byte)Tutorial[i]["ProgressType_b"].n;

                TutorialInfoDic.Add(tmpInfo.ID, tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Quest_SubTalkScene", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject SubTalkScene = new JSONObject(strSrc);

            for (int i = 0; i < SubTalkScene.list.Count; i++)
            {
                SubTalkSceneInfo tmpInfo = new SubTalkSceneInfo();
                tmpInfo.SceneID = (uint)SubTalkScene[i]["SceneID_ui"].n;
                tmpInfo.Sequence = (uint)SubTalkScene[i]["Sequence_ui"].n;
                tmpInfo.NpcFace = SubTalkScene[i]["NpcFace_c"].str;
                tmpInfo.Position = (byte)SubTalkScene[i]["Position_b"].n;
                tmpInfo.RemoveType = (uint)SubTalkScene[i]["RemoveType_ui"].n;
                tmpInfo.RemoveTime = (uint)SubTalkScene[i]["RemoveTime_ui"].n;
                tmpInfo.TalkString = SubTalkScene[i]["TalkString_c"].str;

                SubTalkSceneInfoList.Add(tmpInfo);
            }
        }

        {
            TextAsset data = Resources.Load("TestJson/Quest_MainTutorial", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject MainTutorial = new JSONObject(strSrc);

            for (int i = 0; i < MainTutorial.list.Count; i++)
            {
                MainTutorialInfo tmpInfo = new MainTutorialInfo();
                tmpInfo.ID = (uint)MainTutorial[i]["ID_ui"].n;
                tmpInfo.Group = (uint)MainTutorial[i]["Group_ui"].n;
                tmpInfo.Sequence = (uint)MainTutorial[i]["Sequence_ui"].n;
                tmpInfo.OpenLevel = (byte)MainTutorial[i]["OpenLevel_b"].n;
                tmpInfo.NextTutorial = (uint)MainTutorial[i]["NextTutorial_ui"].n;
                tmpInfo.OpenType = (byte)MainTutorial[i]["OpenType_b"].n;
                tmpInfo.OpenMessage = MainTutorial[i]["OpenMessage_c"].str;
                tmpInfo.ProgressType = (byte)MainTutorial[i]["ProgressType_b"].n;
                tmpInfo.GuideType = (byte)MainTutorial[i]["GuideType_b"].n;
                tmpInfo.RemoveType = (byte)MainTutorial[i]["RemoveType_b"].n;
                tmpInfo.GuideCount = (byte)MainTutorial[i]["GuideCount_b"].n;
                tmpInfo.GuideText1 = MainTutorial[i]["GuideText1_c"].str;
                tmpInfo.GuideText2 = MainTutorial[i]["GuideText2_c"].str;
                tmpInfo.GuideText3 = MainTutorial[i]["GuideText3_c"].str;
                tmpInfo.GuideText4 = MainTutorial[i]["GuideText4_c"].str;
                tmpInfo.GuideText5 = MainTutorial[i]["GuideText5_c"].str;

                MainTutorialInfoDic.Add(tmpInfo.ID, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/QuestInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, QuestInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/QuestTalkSceneInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, QuestTalkSceneInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/TutorialInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, TutorialInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SubTalkSceneInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SubTalkSceneInfoList);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/MainTutorialInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, MainTutorialInfoDic);
            fs.Close();
        }
    }

    public void DeserializeData()
    {
//        QuestInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, QuestInfo>>("QuestInfo");
//        QuestTalkSceneInfoList = _LowDataMgr.instance.DeserializeData<List<QuestTalkSceneInfo>>("QuestTalkSceneInfo");
//        TutorialInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, TutorialInfo>>("TutorialInfo");
//        SubTalkSceneInfoList = _LowDataMgr.instance.DeserializeData<List<SubTalkSceneInfo>>("SubTalkSceneInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, QuestInfo>>("QuestInfo", (data) => { QuestInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<QuestTalkSceneInfo>>("QuestTalkSceneInfo", (data) => { QuestTalkSceneInfoList = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, TutorialInfo>>("TutorialInfo", (data) => { TutorialInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<List<SubTalkSceneInfo>>("SubTalkSceneInfo", (data) => { SubTalkSceneInfoList = data; });
        _LowDataMgr.instance.DeserializeData<Dictionary<uint, MainTutorialInfo>>("MainTutorialInfo", (data) => { MainTutorialInfoDic = data; });

    }
}
