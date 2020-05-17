using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public static class BuildScript
{
    static void BuildAndroid()
    {
        Console.Out.WriteLine("[LOG] Android Build Start");

        DateTime startTime = DateTime.Now;

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        List<string> enableScenePathList = new List<string>();

        foreach( EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(false == scene.enabled) { continue;  }
            enableScenePathList.Add(scene.path);
            Console.Out.WriteLine("[LOG] buildScene: " + scene);
        }

        string binaryPath = UnityEngine.Application.dataPath + "/../Builds/wongfeihung_"+ startTime.Month.ToString() + startTime.Day.ToString()+".apk";

        if(false == File.Exists(binaryPath))
        {
            FileInfo fileInfo = new FileInfo(binaryPath);
            fileInfo.Directory.CreateSubdirectory(fileInfo.DirectoryName);
        }

        BuildTarget buildtarget = BuildTarget.Android;
        BuildOptions buildOption = BuildOptions.None;

        string result = BuildPipeline.BuildPlayer(enableScenePathList.ToArray(), binaryPath, buildtarget, buildOption);

        DateTime endTime = DateTime.Now;

        Console.Out.WriteLine("[LOG] BuildResult: " + result + " BuildTime: " + endTime.Subtract(startTime).TotalSeconds);

    }
}

