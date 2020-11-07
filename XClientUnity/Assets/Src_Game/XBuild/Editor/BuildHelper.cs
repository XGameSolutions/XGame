/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace XBuild
{
    public static class BuildHelper
    {
        public static void CreateDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void ClearDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
            CreateDir(path);
        }

        public static bool CopyDir(string sourPath, string destPath, bool createDestDirIfNotExist = true)
        {
            var sourDir = new DirectoryInfo(sourPath);
            var destDir = new DirectoryInfo(destPath);
            if (!sourDir.Exists)
            {
                BuildLog.LogError("BuildHelper.CopyDir ERROR: sourPath not exist:" + sourPath);
                return false;
            }
            if (!destDir.Exists)
            {
                if (createDestDirIfNotExist) destDir.Create();
                else
                {
                    BuildLog.LogError("BuildHelper.CopyDir ERROR: destPath not exist:" + sourPath);
                    return false;
                }
            }
            foreach (var file in sourDir.GetFiles())
            {
                CopyFile(file.FullName, destPath);
            }
            foreach (var dir in sourDir.GetDirectories())
            {
                CopyDir(dir.FullName, destDir.FullName + "/" + dir.Name, createDestDirIfNotExist);
            }
            return true;
        }

        public static bool CopyFile(string sourFilePath, string destDir, bool checkExists = true)
        {
            if (checkExists)
            {
                if (!File.Exists(sourFilePath)) return false;
                if (!Directory.Exists(destDir)) return false;
            }
            string fileName = Path.GetFileName(sourFilePath);
            File.Copy(sourFilePath, destDir + "/" + fileName, true);
            return true;
        }

        public static string GetABDir(BuildTarget target)
        {
            return BuildConfig.abDir + target;
        }

        public static string GetABDirPath(BuildTarget target)
        {
            return Application.dataPath + "/../" + GetABDir(target);
        }

        public static BuildParams GetBuildParamsFromCommandLine(BuildTarget target)
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

