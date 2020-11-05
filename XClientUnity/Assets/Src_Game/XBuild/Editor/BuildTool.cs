using System.Net.Mime;
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

namespace XBuild
{
    public static class BuildTool
    {
        private static BuildParams s_Param;

        public static void BuildWindow()
        {
            if (s_Param == null) s_Param = GetBuildParamsByCommandArgs(BuildTarget.StandaloneWindows);
            Debug.LogError(s_Param);
        }

        public static void BuildWindowAB()
        {
            if (s_Param == null) s_Param = GetBuildParamsByCommandArgs(BuildTarget.StandaloneWindows);
            BuildAB(BuildTarget.StandaloneWindows, s_Param);
        }

        public static void BuildOSXAB()
        {
            if (s_Param == null) s_Param = GetBuildParamsByCommandArgs(BuildTarget.StandaloneWindows);
            BuildAB(BuildTarget.StandaloneOSX, s_Param);
        }

        private static void BuildAB(BuildTarget target, BuildParams param)
        {
            var abDir = "Res_AB/" + target;
            var abPath = Application.dataPath + "/../" + abDir;
            var option = BuildAssetBundleOptions.None;
            option |= BuildAssetBundleOptions.ChunkBasedCompression;
            BuildHelper.CreateDir(abPath);
            BuildPipeline.BuildAssetBundles(abDir, option, target);
        }

        private static void BuildPlayer(BuildTarget target, BuildParams param)
        {
            PlayerSettings.productName = param.productName;
            PlayerSettings.applicationIdentifier = param.applicationIdentifier;
            PlayerSettings.bundleVersion = param.apkVersion;
            PlayerSettings.Android.bundleVersionCode = param.bundleVersionCode;
            PlayerSettings.iOS.buildNumber = param.bundleVersionCode.ToString();
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = param.appleDeveloperTeamID;

            BuildPlayerOptions op = new BuildPlayerOptions();
            op.scenes = new[] { param.startScene };
            op.target = target;
            op.options = param.isDebug ? (BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler) :
                BuildOptions.None;

            var result = BuildPipeline.BuildPlayer(op);
            var summary = result.summary;

        }

        private static BuildParams GetBuildParamsByCommandArgs(BuildTarget target)
        {
            BuildParams param;
            var args = Environment.GetCommandLineArgs();
            if (Application.isBatchMode && args != null && args.Length >= 10)
            {
                var strParams = args[9];
                param = BuildParams.Parse(strParams);
            }
            else
            {
                param = new BuildParams();
            }
            return param;
        }
    }
}

