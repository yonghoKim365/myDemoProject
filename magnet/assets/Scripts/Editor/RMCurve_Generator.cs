using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RMCurve_Generator
{
    public static readonly string FileExtension = ".rmcurve";
    public static readonly string RMAnimationsPath = @"\Resources\Character\AnimationRMs";
    public static readonly string RMDataSavedPath = @"\Resources\Character\AnimationRMs\Data";

    static readonly string RMBoneName = "Bip001";

    RMCurve savedCurve;

    [MenuItem( "MagnetGames/RootMotion Curve Generator" )]
    static void GenerateRMsFromAnims()
    {
        Init();

        List<AnimationClip> clipList = GetAnimationClipFromFolder( "Assets" + RMAnimationsPath );
        foreach (AnimationClip clip in clipList)
        {
            CreateRMCurveFromAnimationClip( clip );
        }
    }

    static void Init()
    { 
        string fullPath = Path.GetFullPath( Application.dataPath + RMDataSavedPath );

        if (!Directory.Exists( fullPath ))
        {
            Directory.CreateDirectory( fullPath );
            AssetDatabase.Refresh();
        }
    }

    static RMCurve CreateRMCurveFromAnimationClip(AnimationClip animClip)
    {
        if (null == animClip)
            Debug.LogWarning( "존재하지 않는 AnimationClip 입니다." );

        
        //
        // 해당 AnimationClip에서 존재하는 Curve들을 모두 찾아서, 
        // RMBoneName에 존재하는 Curve만 데이터화 시키도록한다.
        AnimationCurve[] localPosCurves = null;
        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves( animClip );
        foreach (AnimationClipCurveData curve in curveDatas)
        {
            // 위치 변위 값만 저장하기 위해서 조건 검사
            if (curve.path != RMBoneName || !curve.propertyName.Contains( "m_LocalPosition" ))
                continue;

            /* TODO : 추후 최적화 아이디어
             * 
             * 키프레임간 차이가 Epsilon정도로 작게 차이나는 키값에 대해서 제거 작업해서 필요없는 키 값을 없애는 작업
             * 삭제한 키값의 좌우 키들은 Left Tangent, Right Tanget을 Linear나 Constant로 조절할 필요가 있음.
             * 
            */

            if (null == localPosCurves)
                localPosCurves = new AnimationCurve[3];

            switch (curve.propertyName)
            {
                case "m_LocalPosition.x":
                    localPosCurves[0] = curve.curve;// new AnimationCurve( curve.curve.keys );
                    break;
                case "m_LocalPosition.y":
                    localPosCurves[1] = curve.curve;// new AnimationCurve( curve.curve.keys );
                    break;
                case "m_LocalPosition.z":
                    localPosCurves[2] = curve.curve;// new AnimationCurve( curve.curve.keys );
                    break;
            }
        }

        RMCurve createdRM = null;
        if (null != localPosCurves)
        {
            // RMCurve 생성후, Curve존재시 데이터 채우기
            createdRM = ScriptableObject.CreateInstance<RMCurve>();
            string animName = animClip.name;
            createdRM.Name = animName;
            createdRM.Length = animClip.length;            
            CopyCurve( localPosCurves[0], ref createdRM.XCurve );
            CopyCurve( localPosCurves[1], ref createdRM.YCurve );
            CopyCurve( localPosCurves[2], ref createdRM.ZCurve );

            //
            // 그룹별 폴더 생성을 위한 코드
            string[] splitNames = animName.Split( '_' );
            string groupFolderName = "";
		  switch (splitNames[0])
		  {
			 case "m"://몬스터
				if (splitNames[1] == "b")//보스
				{
				    //ex ) m_b_merchant_01
				    groupFolderName = string.Format("{0}_{1}_{2}_{3}", splitNames[0], splitNames[1], splitNames[2], splitNames[3]);
				    break;
				}
				//ex ) m_merchant_01
				groupFolderName = string.Format("{0}_{1}_{2}", splitNames[0], splitNames[1], splitNames[2]);
				break;

			 case "pc"://플레이어
				//ex ) pc_f
				groupFolderName = string.Format("{0}_{1}", splitNames[0], splitNames[1]);
				break;

			 case "par"://파트너
                    //ex ) par_mokkwailan_01
                    //groupFolderName = string.Format("{0}_{1}_{2}", splitNames[0], splitNames[1], splitNames[2]);
                    groupFolderName = string.Format("{0}_{1}", splitNames[0], splitNames[1]);
                    break;
		  }
            //if (splitNames.Length > 3)
            //    groupFolderName = string.Format("{0}_{1}_{0}", splitNames[0], splitNames[1], splitNames[2]);

            if (!string.IsNullOrEmpty(groupFolderName))
            {
                string newFolder = Path.GetFullPath( "Assets" + RMDataSavedPath + Path.DirectorySeparatorChar + groupFolderName );
                if (!Directory.Exists( newFolder ))
                    Directory.CreateDirectory( newFolder );

                groupFolderName += Path.DirectorySeparatorChar;
            }

            // RMCurve데이터 asset으로 저장
            AssetDatabase.CreateAsset( createdRM, "Assets" + RMDataSavedPath + Path.DirectorySeparatorChar + groupFolderName + createdRM.Name + ".asset" );
            AssetDatabase.SaveAssets();
        }

        return createdRM;
    }

    public static void CopyCurve(AnimationCurve src, ref AnimationCurve dest)
    {
        if (null == src)
            return;

        if (null == dest)
            dest = new AnimationCurve();

        foreach (Keyframe kf in src.keys)
            dest.AddKey( new Keyframe(kf.time, kf.value, kf.inTangent, kf.outTangent) );
    }

    /// <summary>
    /// RMCurve 객체를 정해진 경로에 빈 RMCurve Asset파일을 생성해준다.
    /// </summary>
    /// <param name="name">생성할 파일명</param>
    /// <returns>해당 경로에 생성된 asset</returns>
    public static RMCurve CreateEmptyRMCurve(string name)
    {
        RMCurve createdCurve = ScriptableObject.CreateInstance<RMCurve>();
        createdCurve.Name = name;
        createdCurve.Length = 0f;

        AssetDatabase.CreateAsset(createdCurve, "Assets" + RMDataSavedPath + Path.DirectorySeparatorChar + name + ".asset");
        AssetDatabase.SaveAssets();

        return createdCurve;
    }

    /// <summary>
    /// 해당 경로에 존재하는 AnimationClip들을 찾아준다.
    /// </summary>
    public static List<AnimationClip> GetAnimationClipFromFolder(string dirPath)
    {
        List<AnimationClip> animList = new List<AnimationClip>();
        string[] clipFilePaths = Directory.GetFiles( dirPath, "*.anim", SearchOption.AllDirectories );
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
}