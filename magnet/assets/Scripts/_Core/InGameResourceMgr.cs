using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameResourceMgr : Immortal<InGameResourceMgr>
{
    static Dictionary<string, Dictionary<string, RMCurve>> RmcurveDic = new Dictionary<string, Dictionary<string, RMCurve>>();
    static Dictionary<string, GameObject> ProjectTileDic = new Dictionary<string, GameObject>();

    public void ClearAssetList()
    {
        var enumerator = RmcurveDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            //enumerator.Current.Value.Clear();

            var enumerator2 = enumerator.Current.Value.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                Resources.UnloadAsset(enumerator2.Current.Value);
            }

            enumerator.Current.Value.Clear();
        }

        RmcurveDic.Clear();

        //var enumerator2 = ProjectTileDic.GetEnumerator();
        //while (enumerator2.MoveNext())
        //{

        //}

        ProjectTileDic.Clear();
    }

    #region RMCurves

    Dictionary<string, List<System.Action>> RMCurvesLoadingDic = new Dictionary<string, List<System.Action>>();
    public void GetRMCurves(string fileName, System.Action<Dictionary<string, RMCurve>> call)
    {
        string[] splitArr = fileName.Split('_');
        if (fileName.Contains("pc_"))
        {
            if (null != splitArr && splitArr.Length > 2)
                fileName = string.Format("{0}_{1}", splitArr[0], splitArr[1]);
        }
        /*
        else if (fileName.Contains("m_b_"))
        {
           if (null != splitArr && splitArr.Length > 4)
              fileName = string.Format("{0}_{1}_{2}_{3}", splitArr[0], splitArr[1], splitArr[2], splitArr[3]);
        }
        else if (fileName.Contains("m_") || fileName.Contains("par_"))
        {
           if (null != splitArr && splitArr.Length > 3)
              fileName = string.Format("{0}_{1}_{2}", splitArr[0], splitArr[1], splitArr[2]);
        }
        */
        if (RmcurveDic.ContainsKey(fileName))
        {
            //< 있으면 바로 보내줌
            call(RmcurveDic[fileName]);
        }
        else
        {
            new Task(LocalLoadRMCurves(fileName, () =>
            {
                GetRMCurves(fileName, call);
            }));
        }
    }

    IEnumerator LocalLoadRMCurves(string anifilename, System.Action call)
    {
        //Debug.LogWarning("<color=green>2JW : AssetbundleLoader LocalLoadRMCurves In</color> - b fileName = " + anifilename);

        //string[] splitArr = anifilename.Split('_');
        //    if (null != splitArr && splitArr.Length > 2)
        //        anifilename = string.Format("{0}_{1}", splitArr[0], splitArr[1]);

        string categoryName = anifilename;
        //Debug.LogWarning("<color=yellow>2JW : AssetbundleLoader LocalLoadRMCurves In</color>- a categoryName = " + categoryName);
#if UNITY_ANDROID
        anifilename += "_android";
#elif UNITY_IPHONE
            anifilename += "_ios";
#endif

        if (RMCurvesLoadingDic.ContainsKey(categoryName))
        {
            RMCurvesLoadingDic[categoryName].Add(call);
            yield return null;
        }
        else
        {
            RMCurvesLoadingDic.Add(categoryName, new List<System.Action>());

            if (!RmcurveDic.ContainsKey(categoryName))
                RmcurveDic.Add(categoryName, new Dictionary<string, RMCurve>());

            RMCurve[] Obj = Resources.LoadAll<RMCurve>(string.Format("Character/AnimationRMs/Data/{0}", categoryName));
            foreach (RMCurve data in Obj)
            {
                if (!RmcurveDic[categoryName].ContainsKey(data.name))
                {
                    //Debug.Log(string.Format("2JW : <color=purple>{0}</color> In Category : {1}", data.name, categoryName));
                    RmcurveDic[categoryName].Add(data.name, data);
                }
            }

            for (int i = 0; i < RMCurvesLoadingDic[categoryName].Count; i++)
                RMCurvesLoadingDic[categoryName][i]();
            RMCurvesLoadingDic[categoryName].Clear();
            RMCurvesLoadingDic.Remove(categoryName);

            if (call != null)
                call();
            //WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + anifilename + ".rmcurve");
            //www.threadPriority = ThreadPriority.High;
            //yield return www;

            //if (www.isDone && null == www.error)
            //{
            //    if (!RmcurveDic.ContainsKey(categoryName))
            //        RmcurveDic.Add(categoryName, new Dictionary<string, RMCurve>());

            //    UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(RMCurve));
            //    foreach (UnityEngine.Object obj in objs)
            //    {
            //        RMCurve tem = (RMCurve)obj;
            //        if (!RmcurveDic[categoryName].ContainsKey(tem.name))
            //            RmcurveDic[categoryName].Add(tem.name, tem);
            //    }

            //    AddBundleDic(false, www.assetBundle, categoryName);

            //    www.Dispose();

            //    yield return null;

            //    for (int i = 0; i < RMCurvesLoadingDic[categoryName].Count; i++)
            //        RMCurvesLoadingDic[categoryName][i]();
            //    RMCurvesLoadingDic[categoryName].Clear();
            //    RMCurvesLoadingDic.Remove(categoryName);

            //    if (call != null)
            //        call();
            //}
            //else
            //{
            //    if (GameDefine.TestMode)
            //        Debug.Log("LocalLoadRMCurves error " + www.error);

            //    www.Dispose();
            //}
        }

    }

    #endregion

    #region ProjectTile

    Dictionary<string, List<System.Action>> ProjectTileLoadingDic = new Dictionary<string, List<System.Action>>();
    public void GetProjectTile(string fileName, System.Action<GameObject> call, bool _Save = false)
    {
        if (ProjectTileDic.ContainsKey(fileName))
        {
            //< 있으면 바로 보내줌
            call(ProjectTileDic[fileName]);
        }
        else
        {

            GameObject temp = G_GameInfo.ProtoTypeProjectileGenerator(fileName);

            if (!ProjectTileDic.ContainsKey(fileName))
            {
                ProjectTileDic.Add(fileName, temp);
            }
            call(ProjectTileDic[fileName]);

            ////< 없다면 아예 그룹자체를 로드후에 실행한다
            //foreach (KeyValuePair<string, List<string>> dic in ProjectTileJsonDic)
            //        {
            //            List<string> list = dic.Value;
            //            for (int i = 0; i < list.Count; i++)
            //            {
            //                if (list[i] == fileName)
            //                {
            //                    new Task(LocalLoadProjectTile(dic.Key, () =>
            //                    {
            //                        GetProjectTile(fileName, call, _Save);
            //                    }, _Save));

            //                    return;
            //                }
            //            }
            //        }
        }
    }

    //static IEnumerator LocalLoadProjectTile(string filename, System.Action call, bool _Save)
    //{
    //    if (ProjectTileLoadingDic.ContainsKey(filename))
    //    {
    //        ProjectTileLoadingDic[filename].Add(call);
    //        yield return null;
    //    }
    //    else
    //    {
    //        ProjectTileLoadingDic.Add(filename, new List<System.Action>());

    //        WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + filename + ".projecttile");

    //        www.threadPriority = ThreadPriority.High;
    //        yield return www;

    //        if (www.isDone && null == www.error)
    //        {
    //            UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(GameObject));
    //            foreach (UnityEngine.Object obj in objs)
    //            {
    //                GameObject tem = (GameObject)obj;

    //                // TODO : 중복체크 없도록 해야함.
    //                if (!ProjectTileDic.ContainsKey(tem.name))
    //                {
    //                    ProjectTileDic.Add(tem.name, tem);
    //                }
    //            }

    //            //< 셰이더 다시 설정
    //            object[] temMaterial = www.assetBundle.LoadAll(typeof(UnityEngine.Material));
    //            foreach (object material in temMaterial)
    //            {
    //                UnityEngine.Material temmat = (UnityEngine.Material)material;
    //                string shaderName = temmat.shader.name;
    //                Shader newShader = Shader.Find(shaderName);
    //                if (newShader != null)
    //                {
    //                    temmat.shader = newShader;
    //                }
    //                else
    //                {
    //                    Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + temmat.name);
    //                }
    //            }

    //            AddBundleDic(_Save, www.assetBundle, filename);

    //            www.Dispose();

    //            yield return null;

    //            for (int i = 0; i < ProjectTileLoadingDic[filename].Count; i++)
    //                ProjectTileLoadingDic[filename][i]();
    //            ProjectTileLoadingDic[filename].Clear();
    //            ProjectTileLoadingDic.Remove(filename);

    //            if (call != null)
    //                call();
    //        }
    //        else
    //        {
    //            if (GameDefine.TestMode)
    //                Debug.Log("LocalLoadProjectTile error " + www.error);

    //            www.Dispose();
    //        }
    //    }

    //}

    //< 이펙트 그룹 안에있는 이펙트명들을 모두 Json파일로 저장처리
    //public static IEnumerator GetProjectTile(List<string> filenames, AssetDownLoad _parent, System.Action call)
    //{
    //    for (int i = 0; i < filenames.Count; i++)
    //    {
    //        WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + filenames[i]);
    //        yield return www;

    //        if (www.isDone && null == www.error)
    //        {
    //            string fileName = filenames[i].Replace(".projecttile", "");
    //            if (!ProjectTileJsonDic.ContainsKey(fileName))
    //                ProjectTileJsonDic.Add(fileName, new List<string>());

    //            UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(GameObject));
    //            foreach (UnityEngine.Object obj in objs)
    //            {
    //                if (!ProjectTileJsonDic[fileName].Contains(obj.name))
    //                    ProjectTileJsonDic[fileName].Add(obj.name);
    //            }

    //            www.assetBundle.Unload(true);
    //            www.Dispose();

    //            yield return null;

    //        }
    //        else
    //        {
    //            if (GameDefine.TestMode)
    //                Debug.Log(www.error);
    //        }

    //        _parent.AssetLoadCount++;
    //    }

    //    call();

    //}

    #endregion
}


//백업
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class InGameResourceMgr
//{
//    static Dictionary<string, Dictionary<string, RMCurve>> RmcurveDic = new Dictionary<string, Dictionary<string, RMCurve>>();
//    static Dictionary<string, GameObject> ProjectTileDic = new Dictionary<string, GameObject>();

//    public static void ClearAssetList()
//    {
//        var enumerator = RmcurveDic.GetEnumerator();
//        while (enumerator.MoveNext())
//        {
//            //enumerator.Current.Value.Clear();

//            var enumerator2 = enumerator.Current.Value.GetEnumerator();

//            while (enumerator2.MoveNext())
//            {
//                Resources.UnloadAsset(enumerator2.Current.Value);
//            }

//            enumerator.Current.Value.Clear();
//        }

//        RmcurveDic.Clear();

//        //var enumerator2 = ProjectTileDic.GetEnumerator();
//        //while (enumerator2.MoveNext())
//        //{

//        //}

//        ProjectTileDic.Clear();
//    }

//    #region RMCurves

//    static Dictionary<string, List<System.Action>> RMCurvesLoadingDic = new Dictionary<string, List<System.Action>>();
//    public static void GetRMCurves(string fileName, System.Action<Dictionary<string, RMCurve>> call)
//    {
//        string[] splitArr = fileName.Split('_');
//        if (fileName.Contains("pc_"))
//        {
//            if (null != splitArr && splitArr.Length > 2)
//                fileName = string.Format("{0}_{1}", splitArr[0], splitArr[1]);
//        }
//        /*
//        else if (fileName.Contains("m_b_"))
//        {
//           if (null != splitArr && splitArr.Length > 4)
//              fileName = string.Format("{0}_{1}_{2}_{3}", splitArr[0], splitArr[1], splitArr[2], splitArr[3]);
//        }
//        else if (fileName.Contains("m_") || fileName.Contains("par_"))
//        {
//           if (null != splitArr && splitArr.Length > 3)
//              fileName = string.Format("{0}_{1}_{2}", splitArr[0], splitArr[1], splitArr[2]);
//        }
//        */
//        if (RmcurveDic.ContainsKey(fileName))
//        {
//            //< 있으면 바로 보내줌
//            call(RmcurveDic[fileName]);
//        }
//        else
//        {
//            new Task(LocalLoadRMCurves(fileName, () =>
//            {
//                GetRMCurves(fileName, call);
//            }));
//        }
//    }

//    static IEnumerator LocalLoadRMCurves(string anifilename, System.Action call)
//    {
//        //Debug.LogWarning("<color=green>2JW : AssetbundleLoader LocalLoadRMCurves In</color> - b fileName = " + anifilename);

//        //string[] splitArr = anifilename.Split('_');
//        //    if (null != splitArr && splitArr.Length > 2)
//        //        anifilename = string.Format("{0}_{1}", splitArr[0], splitArr[1]);

//        string categoryName = anifilename;
//        //Debug.LogWarning("<color=yellow>2JW : AssetbundleLoader LocalLoadRMCurves In</color>- a categoryName = " + categoryName);
//#if UNITY_ANDROID
//    	    anifilename += "_android";
//#elif UNITY_IPHONE
//            anifilename += "_ios";
//#endif

//        if (RMCurvesLoadingDic.ContainsKey(categoryName))
//        {
//            RMCurvesLoadingDic[categoryName].Add(call);
//            yield return null;
//        }
//        else
//        {
//            RMCurvesLoadingDic.Add(categoryName, new List<System.Action>());

//            if (!RmcurveDic.ContainsKey(categoryName))
//                RmcurveDic.Add(categoryName, new Dictionary<string, RMCurve>());

//            RMCurve[] Obj = Resources.LoadAll<RMCurve>(string.Format("Character/AnimationRMs/Data/{0}", categoryName));
//            foreach (RMCurve data in Obj)
//            {
//                if (!RmcurveDic[categoryName].ContainsKey(data.name))
//                {
//                    //Debug.Log(string.Format("2JW : <color=purple>{0}</color> In Category : {1}", data.name, categoryName));
//                    RmcurveDic[categoryName].Add(data.name, data);
//                }
//            }

//            for (int i = 0; i < RMCurvesLoadingDic[categoryName].Count; i++)
//                RMCurvesLoadingDic[categoryName][i]();
//            RMCurvesLoadingDic[categoryName].Clear();
//            RMCurvesLoadingDic.Remove(categoryName);

//            if (call != null)
//                call();
//            //WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + anifilename + ".rmcurve");
//            //www.threadPriority = ThreadPriority.High;
//            //yield return www;

//            //if (www.isDone && null == www.error)
//            //{
//            //    if (!RmcurveDic.ContainsKey(categoryName))
//            //        RmcurveDic.Add(categoryName, new Dictionary<string, RMCurve>());

//            //    UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(RMCurve));
//            //    foreach (UnityEngine.Object obj in objs)
//            //    {
//            //        RMCurve tem = (RMCurve)obj;
//            //        if (!RmcurveDic[categoryName].ContainsKey(tem.name))
//            //            RmcurveDic[categoryName].Add(tem.name, tem);
//            //    }

//            //    AddBundleDic(false, www.assetBundle, categoryName);

//            //    www.Dispose();

//            //    yield return null;

//            //    for (int i = 0; i < RMCurvesLoadingDic[categoryName].Count; i++)
//            //        RMCurvesLoadingDic[categoryName][i]();
//            //    RMCurvesLoadingDic[categoryName].Clear();
//            //    RMCurvesLoadingDic.Remove(categoryName);

//            //    if (call != null)
//            //        call();
//            //}
//            //else
//            //{
//            //    if (GameDefine.TestMode)
//            //        Debug.Log("LocalLoadRMCurves error " + www.error);

//            //    www.Dispose();
//            //}
//        }

//    }

//    #endregion

//    #region ProjectTile

//    static Dictionary<string, List<System.Action>> ProjectTileLoadingDic = new Dictionary<string, List<System.Action>>();
//    public static void GetProjectTile(string fileName, System.Action<GameObject> call, bool _Save = false)
//    {
//        if (ProjectTileDic.ContainsKey(fileName))
//        {
//            //< 있으면 바로 보내줌
//            call(ProjectTileDic[fileName]);
//        }
//        else
//        {

//            GameObject temp = G_GameInfo.ProtoTypeProjectileGenerator(fileName);

//            if (!ProjectTileDic.ContainsKey(fileName))
//            {
//                ProjectTileDic.Add(fileName, temp);
//            }
//            call(ProjectTileDic[fileName]);

//            ////< 없다면 아예 그룹자체를 로드후에 실행한다
//            //foreach (KeyValuePair<string, List<string>> dic in ProjectTileJsonDic)
//            //        {
//            //            List<string> list = dic.Value;
//            //            for (int i = 0; i < list.Count; i++)
//            //            {
//            //                if (list[i] == fileName)
//            //                {
//            //                    new Task(LocalLoadProjectTile(dic.Key, () =>
//            //                    {
//            //                        GetProjectTile(fileName, call, _Save);
//            //                    }, _Save));

//            //                    return;
//            //                }
//            //            }
//            //        }
//        }
//    }

//    //static IEnumerator LocalLoadProjectTile(string filename, System.Action call, bool _Save)
//    //{
//    //    if (ProjectTileLoadingDic.ContainsKey(filename))
//    //    {
//    //        ProjectTileLoadingDic[filename].Add(call);
//    //        yield return null;
//    //    }
//    //    else
//    //    {
//    //        ProjectTileLoadingDic.Add(filename, new List<System.Action>());

//    //        WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + filename + ".projecttile");

//    //        www.threadPriority = ThreadPriority.High;
//    //        yield return www;

//    //        if (www.isDone && null == www.error)
//    //        {
//    //            UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(GameObject));
//    //            foreach (UnityEngine.Object obj in objs)
//    //            {
//    //                GameObject tem = (GameObject)obj;

//    //                // TODO : 중복체크 없도록 해야함.
//    //                if (!ProjectTileDic.ContainsKey(tem.name))
//    //                {
//    //                    ProjectTileDic.Add(tem.name, tem);
//    //                }
//    //            }

//    //            //< 셰이더 다시 설정
//    //            object[] temMaterial = www.assetBundle.LoadAll(typeof(UnityEngine.Material));
//    //            foreach (object material in temMaterial)
//    //            {
//    //                UnityEngine.Material temmat = (UnityEngine.Material)material;
//    //                string shaderName = temmat.shader.name;
//    //                Shader newShader = Shader.Find(shaderName);
//    //                if (newShader != null)
//    //                {
//    //                    temmat.shader = newShader;
//    //                }
//    //                else
//    //                {
//    //                    Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + temmat.name);
//    //                }
//    //            }

//    //            AddBundleDic(_Save, www.assetBundle, filename);

//    //            www.Dispose();

//    //            yield return null;

//    //            for (int i = 0; i < ProjectTileLoadingDic[filename].Count; i++)
//    //                ProjectTileLoadingDic[filename][i]();
//    //            ProjectTileLoadingDic[filename].Clear();
//    //            ProjectTileLoadingDic.Remove(filename);

//    //            if (call != null)
//    //                call();
//    //        }
//    //        else
//    //        {
//    //            if (GameDefine.TestMode)
//    //                Debug.Log("LocalLoadProjectTile error " + www.error);

//    //            www.Dispose();
//    //        }
//    //    }

//    //}

//    //< 이펙트 그룹 안에있는 이펙트명들을 모두 Json파일로 저장처리
//    //public static IEnumerator GetProjectTile(List<string> filenames, AssetDownLoad _parent, System.Action call)
//    //{
//    //    for (int i = 0; i < filenames.Count; i++)
//    //    {
//    //        WWW www = new WWW("file:///" + NativeManager.instance.AppDataPath + "/asset/" + filenames[i]);
//    //        yield return www;

//    //        if (www.isDone && null == www.error)
//    //        {
//    //            string fileName = filenames[i].Replace(".projecttile", "");
//    //            if (!ProjectTileJsonDic.ContainsKey(fileName))
//    //                ProjectTileJsonDic.Add(fileName, new List<string>());

//    //            UnityEngine.Object[] objs = www.assetBundle.LoadAll(typeof(GameObject));
//    //            foreach (UnityEngine.Object obj in objs)
//    //            {
//    //                if (!ProjectTileJsonDic[fileName].Contains(obj.name))
//    //                    ProjectTileJsonDic[fileName].Add(obj.name);
//    //            }

//    //            www.assetBundle.Unload(true);
//    //            www.Dispose();

//    //            yield return null;

//    //        }
//    //        else
//    //        {
//    //            if (GameDefine.TestMode)
//    //                Debug.Log(www.error);
//    //        }

//    //        _parent.AssetLoadCount++;
//    //    }

//    //    call();

//    //}

//    #endregion
//}
