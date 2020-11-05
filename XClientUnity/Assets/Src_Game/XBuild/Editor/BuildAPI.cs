using UnityEditor;
using XBuild;

public static class BuildAPI
{
    [MenuItem("XBuild/BuildWindow")]
    public static void BuildWindow()
    {
        BuildTool.BuildWindow();
    }

    [MenuItem("XBuild/BuildWindowAB")]
    public static void BuildWindowAB()
    {
        BuildTool.BuildWindowAB();
    }

    [MenuItem("XBuild/BuildOSXAB")]
    public static void BuildOSXAB()
    {
        BuildTool.BuildOSXAB();
    }
}