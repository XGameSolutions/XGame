/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using System.IO;

namespace XBuild
{
    public static class BuildTool
    {
        private static BuildParams s_Param;

        public static void BuildWindow()
        {
            var target = BuildTarget.StandaloneWindows;
            if (s_Param == null) s_Param = BuildHelper.GetBuildParamsFromCommandLine(target);
            Debug.LogError(s_Param);
        }

        public static void BuildWindowAB()
        {
            var target = BuildTarget.StandaloneWindows;
            if (s_Param == null) s_Param = BuildHelper.GetBuildParamsFromCommandLine(target);
            BuildAB(target, s_Param);
        }

        public static void BuildOSXAB()
        {
            var target = BuildTarget.StandaloneOSX;
            if (s_Param == null) s_Param = BuildHelper.GetBuildParamsFromCommandLine(target);
            BuildAB(target, s_Param);
        }

        public static void BuildIOSXcode()
        {
            var target = BuildTarget.iOS;
            if (s_Param == null) s_Param = BuildHelper.GetBuildParamsFromCommandLine(target);
            BuildAB(target, s_Param);
            BuildPackage(target, s_Param);
        }

        public static void BuildIOSAB()
        {
            var target = BuildTarget.iOS;
            if (s_Param == null) s_Param = BuildHelper.GetBuildParamsFromCommandLine(target);
            BuildAB(target, s_Param);
        }

        public static void BuildAB(BuildTarget target, BuildParams param)
        {
            var abDir = BuildHelper.GetABDir(target);
            var abPath = BuildHelper.GetABDirPath(target);
            var option = BuildAssetBundleOptions.None;
            option |= BuildAssetBundleOptions.ChunkBasedCompression;
            BuildHelper.CreateDir(abPath);
            BuildPipeline.BuildAssetBundles(abDir, option, target);
            BuildLog.Log("BuildAB DONE!");
        }

        private static void BuildPackage(BuildTarget target, BuildParams param)
        {
            EditorSceneManager.OpenScene(param.startScene);

            var init = GameObject.Find(BuildConfig.startSceneInitObjectPath);
            if (init != null)
            {
                //TODO: init params
                //var startScript = init.gameObject.GetComponent<GameStart>();
                //starScript.
                EditorSceneManager.SaveOpenScenes();
            }
            var flag = BuildPlayer(target, param);
            if (flag)
            {
                //TODO: save pkg to dir
            }
        }

        private static bool BuildPlayer(BuildTarget target, BuildParams param)
        {
            var sourAbPath = BuildHelper.GetABDirPath(target);
            var destAbPath = Application.streamingAssetsPath;
            if (!Directory.Exists(sourAbPath))
            {
                BuildLog.LogError("ab path not exists:" + sourAbPath);
                return false;
            }
            BuildHelper.ClearDir(destAbPath);
            BuildHelper.CopyDir(sourAbPath, destAbPath);

            PlayerSettings.productName = param.productName;
            PlayerSettings.companyName = param.companyName;
            PlayerSettings.applicationIdentifier = param.applicationIdentifier;
            PlayerSettings.bundleVersion = param.apkVersion;
            PlayerSettings.Android.bundleVersionCode = param.bundleVersionCode;
            PlayerSettings.iOS.buildNumber = param.bundleVersionCode.ToString();
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = param.appleDeveloperTeamID;

            BuildPlayerOptions op = new BuildPlayerOptions();
            op.scenes = new[] { param.startScene };
            op.locationPathName = param.apkFileName;
            op.target = target;
            op.options = param.isDebug ?
                (BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler) :
                BuildOptions.None;

            var result = BuildPipeline.BuildPlayer(op);
            var summary = result.summary;
            if (summary.result == BuildResult.Succeeded)
            {

            }
            if (summary.result == BuildResult.Failed)
            {
                return false;
            }
            return true;
        }
    }
}

