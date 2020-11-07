/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using UnityEditor;
using XBuild;

public static class BuildAPI
{
    [MenuItem("XBuild/BuildWindow")]
    public static void BuildWindow()
    {
        BuildLog.Instance.Init("BuildAPI.BuildWindow");
        BuildTool.BuildWindow();
    }

    [MenuItem("XBuild/BuildWindowAB")]
    public static void BuildWindowAB()
    {
        BuildLog.Instance.Init("BuildAPI.BuildWindowAB");
        BuildTool.BuildWindowAB();
    }

    [MenuItem("XBuild/BuildOSXAB")]
    public static void BuildOSXAB()
    {
        BuildLog.Instance.Init("BuildAPI.BuildOSXAB");
        BuildTool.BuildOSXAB();
    }

    [MenuItem("XBuild/BuildIOSXcode")]
    public static void BuildIOSXcode()
    {
        BuildLog.Instance.Init("BuildAPI.BuildIOSXcode");
        BuildTool.BuildIOSXcode();
    }

    [MenuItem("XBuild/BuildIOSAB")]
    public static void BuildIOSAB()
    {
        BuildLog.Instance.Init("BuildAPI.BuildIOSAB");
        BuildTool.BuildIOSAB();
    }
}