using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Resource
{
    [Serializable]
    public class AniInfo
    {
        public uint id;
        public string aniName;
        public string effect;
        byte _cameraShakeSet;
        public byte cameraShakeSet
        {
            set { _cameraShakeSet = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_cameraShakeSet); }
        }
        public bool rootMotion;
        public float killPushOffset;
        byte _childEffect;
        public byte childEffect
        {
            set { _childEffect = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_childEffect); }
        }
        uint _seSkill;
        public uint seSkill
        {
            set { _seSkill = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_seSkill); }
        }
        public JsonCustomData voice;
        ushort _voiceTime;
        public ushort voiceTime
        {
            set { _voiceTime = EncryptHelper.SSecureUSHORT(value); }
            get { return EncryptHelper.GSecureUSHORT(_voiceTime); }
        }
    }
    public Dictionary<uint, AniInfo> AniInfoDic = new Dictionary<uint, AniInfo>();

    [Serializable]
    public class SoundInfo
    {
        public uint id;
        public string soundFile;
    }
    public Dictionary<uint, SoundInfo> SoundInfoDic = new Dictionary<uint, SoundInfo>();

    [Serializable]
    public class UnitInfo
    {
        public uint id;
        byte _unitType;
        public byte unitType
        {
            set { _unitType = EncryptHelper.SSecureBYTE(value); }
            get { return EncryptHelper.GSecureBYTE(_unitType); }
        }
        uint _aniIdle01;
        public uint aniIdle01
        {
            set { _aniIdle01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniIdle01); }
        }
        uint _battleAniIdle01;
        public uint battleAniIdle01
        {
            set { _battleAniIdle01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_battleAniIdle01); }
        }
        uint _aniRun;
        public uint aniRun
        {
            set { _aniRun = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniRun); }
        }
        uint _aniHit01;
        public uint aniHit01
        {
            set { _aniHit01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniHit01); }
        }
        uint _aniDie01;
        public uint aniDie01
        {
            set { _aniDie01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniDie01); }
        }
        uint _aniIntro;
        public uint aniIntro
        {
            set { _aniIntro = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniIntro); }
        }
        uint _aniStand;
        public uint aniStand
        {
            set { _aniStand = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniStand); }
        }
        uint _aniDown;
        public uint aniDown
        {
            set { _aniDown = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniDown); }
        }
        uint _aniSpecial;
        public uint aniSpecial
        {
            set { _aniSpecial = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniSpecial); }
        }
        uint _aniRevival;
        public uint aniRevival
        {
            set { _aniRevival = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniRevival); }
        }
        uint _aniStun;
        public uint aniStun
        {
            set { _aniStun = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniStun); }
        }
        uint _aniVictory;
        public uint aniVictory
        {
            set { _aniVictory = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniVictory); }
        }
        uint _aniFail;
        public uint aniFail
        {
            set { _aniFail = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniFail); }
        }
        uint _aniFailIdle;
        public uint aniFailIdle
        {
            set { _aniFailIdle = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniFailIdle); }
        }
        uint _awakenEffect;
        public uint awakenEffect
        {
            set { _awakenEffect = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_awakenEffect); }
        }
        uint _AniAttack01;
        public uint AniAttack01
        {
            set { _AniAttack01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniAttack01); }
        }
        uint _AniAttack02;
        public uint AniAttack02
        {
            set { _AniAttack02 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniAttack02); }
        }
        uint _AniAttack03;
        public uint AniAttack03
        {
            set { _AniAttack03 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniAttack03); }
        }
        uint _AniAttack04;
        public uint AniAttack04
        {
            set { _AniAttack04 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniAttack04); }
        }
        uint _AniSkill01;
        public uint AniSkill01
        {
            set { _AniSkill01 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill01); }
        }
        uint _AniSkill02;
        public uint AniSkill02
        {
            set { _AniSkill02 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill02); }
        }
        uint _AniSkill03;
        public uint AniSkill03
        {
            set { _AniSkill03 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill03); }
        }
        uint _AniSkill04;
        public uint AniSkill04
        {
            set { _AniSkill04 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill04); }
        }
        uint _AniSkill05;
        public uint AniSkill05
        {
            set { _AniSkill05 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill05); }
        }
        uint _AniSkill06;
        public uint AniSkill06
        {
            set { _AniSkill06 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill06); }
        }
        uint _AniSkill07;
        public uint AniSkill07
        {
            set { _AniSkill07 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill07); }
        }
        uint _AniSkill08;
        public uint AniSkill08
        {
            set { _AniSkill08 = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniSkill08); }
        }
        uint _AniChain;
        public uint AniChain
        {
            set { _AniChain = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_AniChain); }
        }
        uint _aniWalk;
        public uint aniWalk
        {
            set { _aniWalk = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniWalk); }
        }
        uint _ExtraAni;
        public uint ExtraAni
        {
            set { _ExtraAni = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_ExtraAni); }
        }
        uint _aniIntroStart;
        public uint aniIntroStart
        {
            set { _aniIntroStart = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniIntroStart); }
        }
        uint _aniIntroEnd;
        public uint aniIntroEnd
        {
            set { _aniIntroEnd = EncryptHelper.SSecureUINT(value); }
            get { return EncryptHelper.GSecureUINT(_aniIntroEnd); }
        }
    }
    public Dictionary<uint, UnitInfo> UnitInfoDic = new Dictionary<uint, UnitInfo>();

    public void LoadLowData()
    {
        {
            TextAsset data = Resources.Load("TestJson/Resource_Ani", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Ani = new JSONObject(strSrc);

            {
                //Null데이터용 0번 애니메이션을 넣어줘야한다.
                AniInfo tmpInfo = new AniInfo();
                tmpInfo.id = 0;
                tmpInfo.aniName = "none";
                tmpInfo.effect = "none";
                tmpInfo.cameraShakeSet = 0;
                tmpInfo.rootMotion = false;
                tmpInfo.killPushOffset = 0;
                tmpInfo.childEffect = 0;
                tmpInfo.seSkill = 0;
                tmpInfo.voiceTime = 0;

                AniInfoDic.Add(tmpInfo.id, tmpInfo);
            }

            for (int i = 0; i < Ani.list.Count; i++)
            {
                AniInfo tmpInfo = new AniInfo();
                tmpInfo.id = (uint)Ani[i]["id_ui"].n;
                tmpInfo.aniName = Ani[i]["aniName_c"].str;
                tmpInfo.effect = Ani[i]["effect_c"].str;
                tmpInfo.cameraShakeSet = (byte)Ani[i]["cameraShakeSet_b"].n;

                if(Ani[i]["rootMotion_is"].n == 0)
                {
                    tmpInfo.rootMotion = false;
                }
                else
                {
                    tmpInfo.rootMotion = true;
                }
                
                tmpInfo.killPushOffset = (float)Ani[i]["killPushOffset_f"].n;
                tmpInfo.childEffect = (byte)Ani[i]["childEffect_b"].n;
                tmpInfo.seSkill = (uint)Ani[i]["seSkill_ui"].n;
                tmpInfo.voice = new JsonCustomData(Ani[i]["voice_j"].ToString());
                tmpInfo.voiceTime = (ushort)Ani[i]["voiceTime_us"].n;

                AniInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Resource_Sound", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Sound = new JSONObject(strSrc);

            for (int i = 0; i < Sound.list.Count; i++)
            {
                SoundInfo tmpInfo = new SoundInfo();
                tmpInfo.id = (uint)Sound[i]["id_ui"].n;
                tmpInfo.soundFile = Sound[i]["soundFile_c"].str;

                SoundInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
        {
            TextAsset data = Resources.Load("TestJson/Resource_Unit", typeof(TextAsset)) as TextAsset;
            StringReader sr = new StringReader(data.text);
            string strSrc = sr.ReadToEnd();
            JSONObject Unit = new JSONObject(strSrc);

            for (int i = 0; i < Unit.list.Count; i++)
            {
                UnitInfo tmpInfo = new UnitInfo();
                tmpInfo.id = (uint)Unit[i]["id_ui"].n;
                tmpInfo.unitType = (byte)Unit[i]["unitType_b"].n;
                tmpInfo.aniIdle01 = (uint)Unit[i]["aniIdle01_ui"].n;
                tmpInfo.battleAniIdle01 = (uint)Unit[i]["battleAniIdle01_ui"].n;
                tmpInfo.aniRun = (uint)Unit[i]["aniRun_ui"].n;
                tmpInfo.aniHit01 = (uint)Unit[i]["aniHit01_ui"].n;
                tmpInfo.aniDie01 = (uint)Unit[i]["aniDie01_ui"].n;
                tmpInfo.aniIntro = (uint)Unit[i]["aniIntro_ui"].n;
                tmpInfo.aniStand = (uint)Unit[i]["aniStand_ui"].n;
                tmpInfo.aniDown = (uint)Unit[i]["aniDown_ui"].n;
                tmpInfo.aniSpecial = (uint)Unit[i]["aniSpecial_ui"].n;
                tmpInfo.aniRevival = (uint)Unit[i]["aniRevival_ui"].n;
                tmpInfo.aniStun = (uint)Unit[i]["aniStun_ui"].n;
                tmpInfo.aniVictory = (uint)Unit[i]["aniVictory_ui"].n;
                tmpInfo.aniFail = (uint)Unit[i]["aniFail_ui"].n;
                tmpInfo.aniFailIdle = (uint)Unit[i]["aniFailIdle_ui"].n;
                tmpInfo.awakenEffect = (uint)Unit[i]["awakenEffect_ui"].n;
                tmpInfo.AniAttack01 = (uint)Unit[i]["AniAttack01_ui"].n;
                tmpInfo.AniAttack02 = (uint)Unit[i]["AniAttack02_ui"].n;
                tmpInfo.AniAttack03 = (uint)Unit[i]["AniAttack03_ui"].n;
                tmpInfo.AniAttack04 = (uint)Unit[i]["AniAttack04_ui"].n;
                tmpInfo.AniSkill01 = (uint)Unit[i]["AniSkill01_ui"].n;
                tmpInfo.AniSkill02 = (uint)Unit[i]["AniSkill02_ui"].n;
                tmpInfo.AniSkill03 = (uint)Unit[i]["AniSkill03_ui"].n;
                tmpInfo.AniSkill04 = (uint)Unit[i]["AniSkill04_ui"].n;
                tmpInfo.AniSkill05 = (uint)Unit[i]["AniSkill05_ui"].n;
                tmpInfo.AniSkill06 = (uint)Unit[i]["AniSkill06_ui"].n;
                tmpInfo.AniSkill07 = (uint)Unit[i]["AniSkill07_ui"].n;
                tmpInfo.AniSkill08 = (uint)Unit[i]["AniSkill08_ui"].n;
                tmpInfo.AniChain = (uint)Unit[i]["AniChain_ui"].n;
                tmpInfo.aniWalk = (uint)Unit[i]["aniWalk_ui"].n;
                tmpInfo.ExtraAni = (uint)Unit[i]["ExtraAni_ui"].n;

                tmpInfo.aniIntroStart = (uint)Unit[i]["aniIntroStart01_ui"].n;
                tmpInfo.aniIntroEnd = (uint)Unit[i]["aniIntroEnd01_ui"].n;


                UnitInfoDic.Add(tmpInfo.id, tmpInfo);
            }
        }
    }

    public void SerializeData()
    {
        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/AniInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, AniInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/SoundInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SoundInfoDic);
            fs.Close();
        }

        {
            FileStream fs = new FileStream("Assets/Resources/SerializeData/UnitInfo.txt", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, UnitInfoDic);
            fs.Close();
        }

    }

    public void DeserializeData()
    {
//        AniInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, AniInfo>>("AniInfo");
//        SoundInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, SoundInfo>>("SoundInfo");
//        UnitInfoDic = _LowDataMgr.instance.DeserializeData<Dictionary<uint, UnitInfo>>("UnitInfo");

		_LowDataMgr.instance.DeserializeData<Dictionary<uint, AniInfo>>("AniInfo", (data) => { AniInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, SoundInfo>>("SoundInfo", (data) => { SoundInfoDic = data; });
		_LowDataMgr.instance.DeserializeData<Dictionary<uint, UnitInfo>>("UnitInfo", (data) => { UnitInfoDic = data; });

    }
}
