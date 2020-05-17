using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AnimationEventExport
{
    public static readonly string AnimationsPath = @"\Resources\Character\Animations";
    static Dictionary<string, uint> ResourceAni = new Dictionary<string, uint>();

    static Resource resourceLowData;
    static SkillTables skillLowData;
    static Item itemLowData;
    static Mob monsterLowData;

    [MenuItem("MagnetGames/AnimationEventExport")]
    static void GenerateAnimationEvent()
    {
        resourceLowData = new Resource();
        skillLowData = new SkillTables();
        itemLowData = new Item();
        monsterLowData = new Mob();

        resourceLowData.LoadLowData();
        skillLowData.LoadLowData();
        itemLowData.LoadLowData();
        monsterLowData.LoadLowData();

        List<AnimationClip> clipList = GetAnimationClipFromFolder("Assets" + AnimationsPath);
        //foreach (AnimationClip clip in clipList)
        //{
        //    CreateAniEventData(clip);
        //}
        ProcessAniRead(clipList);
        
        resourceLowData = null;
        skillLowData = null;
        itemLowData = null;
        monsterLowData = null;
    }

    public static List<AnimationClip> GetAnimationClipFromFolder(string dirPath)
    {
        List<AnimationClip> animList = new List<AnimationClip>();
        string[] clipFilePaths = Directory.GetFiles(dirPath, "*.anim", SearchOption.AllDirectories);
        foreach (string clipPath in clipFilePaths)
        {
            //if(clipPath.Contains("Coin"))
            {
                Object[] assetObjects = AssetDatabase.LoadAllAssetsAtPath(clipPath);
                foreach (Object assetObject in assetObjects)
                {
                    if (assetObject is UnityEngine.AnimationClip)
                        animList.Add(assetObject as AnimationClip);
                }
            }
        }

        return animList;
    }

    static void CreateAniEventData(AnimationClip animClip)
    {
        if (null == animClip)
            Debug.LogWarning("존재하지 않는 AnimationClip 입니다.");

        AnimationEvent[] aniEvents = AnimationUtility.GetAnimationEvents(animClip);

        int count = 0;
        uint iValue = 0;

        if ( ResourceAni.TryGetValue(animClip.name, out iValue) )
        {
            foreach (AnimationEvent aniEvent in aniEvents)
            {
                ++count;
                Debug.Log(iValue + ": event" + count + " time:" + aniEvent.time);
            }
        }
        else
        {
            Debug.LogError("not Found ResourceData");
        }
    }

    static void ResourceAniRead()
    {
        ResourceAni.Clear();

        TextAsset data = Resources.Load("TestJson/Resource_Ani", typeof(TextAsset)) as TextAsset;
        StringReader sr = new StringReader(data.text);
        string strSrc = sr.ReadToEnd();
        JSONObject Ani = new JSONObject(strSrc);

        for (int i = 0; i < Ani.list.Count; i++)
        {
            if (!ResourceAni.ContainsKey(Ani[i]["aniName_c"].str))
                ResourceAni.Add(Ani[i]["aniName_c"].str, (uint)Ani[i]["id_ui"].n);
            else
                Debug.LogError(Ani[i]["aniName_c"].str + " exist");
        }
    }

    struct SkillData
    {
        public uint skillID;
        public uint aniID;
        public string aniName;
    }

    static void ProcessAniRead(List<AnimationClip> aniclips)
    {
        string a_strBuf = "";
        a_strBuf = "idx_ui,casttime_f,hittime_f,mergetime_f,watingtime_f\n";

        List<SkillData> skillDatas = new List<SkillData>();

        //일단 유저꺼만 - 코스튬
        Resource.AniInfo aniInfo = null;

        //var enumerator = itemLowData.CostumeInfoDic.GetEnumerator();
        var enumerator = skillLowData.SkillSetInfoDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            //해당 리소스 데이터를 가져옴
            Resource.UnitInfo data = null;
            if( resourceLowData.UnitInfoDic.TryGetValue(enumerator.Current.Value.AniId, out data) )
            {
                //평타1

                SkillData newData = new SkillData();
                newData.skillID = System.Convert.ToUInt32(enumerator.Current.Value.skill0[0]);
                newData.aniID = data.AniAttack01;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = System.Convert.ToUInt32(enumerator.Current.Value.skill0[1]);
                newData.aniID = data.AniAttack02;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = System.Convert.ToUInt32(enumerator.Current.Value.skill0[2]);
                newData.aniID = data.AniAttack03;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = enumerator.Current.Value.skill1;
                newData.aniID = data.AniSkill01;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = enumerator.Current.Value.skill2;
                newData.aniID = data.AniSkill02;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = enumerator.Current.Value.skill3;
                newData.aniID = data.AniSkill03;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);

                newData = new SkillData();
                newData.skillID = enumerator.Current.Value.skill4;
                newData.aniID = data.AniSkill04;

                if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                {
                    newData.aniName = aniInfo.aniName;
                }

                skillDatas.Add(newData);
            }
        }

        //몬스터스킬
        var enumerator2 = monsterLowData.MobInfoDic.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            //해당 리소스 데이터를 가져옴
            Resource.UnitInfo data = null;
            if (resourceLowData.UnitInfoDic.TryGetValue(enumerator2.Current.Value.AniId, out data))
            {
                //평타1

                if(System.Convert.ToUInt32(enumerator2.Current.Value.skill[0]) != 0)
                {
                    SkillData newData = new SkillData();
                    newData.skillID = System.Convert.ToUInt32(enumerator2.Current.Value.skill[0]);
                    newData.aniID = data.AniAttack01;

                    if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                    {
                        newData.aniName = aniInfo.aniName;
                    }

                    skillDatas.Add(newData);
                }

                if (System.Convert.ToUInt32(enumerator2.Current.Value.skill[1]) != 0)
                {
                    SkillData newData = new SkillData();
                    newData.skillID = System.Convert.ToUInt32(enumerator2.Current.Value.skill[1]);
                    newData.aniID = data.AniAttack02;

                    if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                    {
                        newData.aniName = aniInfo.aniName;
                    }

                    skillDatas.Add(newData);
                }

                if (System.Convert.ToUInt32(enumerator2.Current.Value.skill[2]) != 0)
                {
                    SkillData newData = new SkillData();
                    newData.skillID = System.Convert.ToUInt32(enumerator2.Current.Value.skill[2]);
                    newData.aniID = data.AniAttack03;

                    if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                    {
                        newData.aniName = aniInfo.aniName;
                    }

                    skillDatas.Add(newData);
                }

                if (System.Convert.ToUInt32(enumerator2.Current.Value.skill[3]) != 0)
                {
                    SkillData newData = new SkillData();
                    newData.skillID = System.Convert.ToUInt32(enumerator2.Current.Value.skill[3]);
                    newData.aniID = data.AniAttack03;

                    if (resourceLowData.AniInfoDic.TryGetValue(newData.aniID, out aniInfo))
                    {
                        newData.aniName = aniInfo.aniName;
                    }

                    skillDatas.Add(newData);
                }
            }

        }

        for (int i=0;i<skillDatas.Count;i++)
        {
            for(int j=0;j<aniclips.Count;j++)
            {
                if (skillDatas[i].aniName.Equals(aniclips[j].name) && !skillDatas[i].aniName.Equals (""))
                {
                    //동일할경우
                    AnimationEvent[] aniEvents = AnimationUtility.GetAnimationEvents(aniclips[j]);

                    //int count = 0;
                    //uint iValue = (uint)skillDatas[i].skillID;

                    //노티만 체크하던 버전
                    //foreach (AnimationEvent aniEvent in aniEvents)
                    //{
                    //    if(aniEvent.functionName.Equals("SkillUse") )
                    //    {
                    //        ++count;
                    //        a_strBuf = a_strBuf + iValue.ToString() + "," + count.ToString() + "," + aniEvent.time.ToString() + "\n";
                    //    }                        
                    //}

                    float castTime = 0;
                    float mergeTime = 0;
                    float lastAttackTime = 0;
                    uint iValue = (uint)skillDatas[i].skillID;

                    a_strBuf = a_strBuf + iValue.ToString() + ",";

                    foreach (AnimationEvent aniEvent in aniEvents)
                    {
                        if (aniEvent.functionName.Equals("PrepareEnd"))
                        {
                            castTime = aniEvent.time;
                        }
                        else if (aniEvent.functionName.Equals("MergeTime"))
                        {
                            mergeTime = aniEvent.time;
                        }
                        else if (aniEvent.functionName.Equals("SkillUse"))
                        {
                            lastAttackTime = aniEvent.time;
                        }
                    }

                    a_strBuf = a_strBuf + castTime.ToString() + "," + lastAttackTime.ToString() + "," + mergeTime.ToString() + "," + aniclips[i].length.ToString() + "\n";
                }
            }
        }

        string a_temfname = Application.dataPath;
        a_temfname = a_temfname.Replace('\\', '/');
        string a_RootPath = a_temfname.Remove(a_temfname.LastIndexOf("/") + 1); //프로젝트가 있는 경로...
        if (!System.IO.Directory.Exists(a_RootPath + "/ExportAniEvent"))
        {
            System.IO.Directory.CreateDirectory(a_RootPath + "/ExportAniEvent");
        }

        string a_PathName = a_RootPath + "/ExportAniEvent/aniEvent.csv";
        System.IO.File.WriteAllText(a_PathName, a_strBuf);

        skillDatas.Clear();
    }
}
