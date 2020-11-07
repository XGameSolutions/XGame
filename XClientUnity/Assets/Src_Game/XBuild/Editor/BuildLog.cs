/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace XBuild
{
    public class BuildLog
    {
        private static BuildLog s_Instance;
        private StreamWriter m_Write;
        private bool m_IsInited;

        public static BuildLog Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new BuildLog();
                }
                return s_Instance;
            }
        }

        private BuildLog()
        {
            Application.logMessageReceived += HandleLog;
        }

        public void Init(string fileName)
        {
            if (m_IsInited) return;
            var path = string.Format("{0}/{1}.log", Application.dataPath, fileName);
            try
            {
                if (File.Exists(path)) File.Delete(path);
                m_Write = new StreamWriter(path, false, Encoding.UTF8);
                m_Write.WriteLine(PackLog("init success!"));
                m_Write.Flush();
                m_IsInited = true;
            }
            catch (Exception e)
            {
                m_IsInited = false;
                m_Write = null;
                Debug.LogError("BuildLog ERROR: init file failed: " + e.Message);
            }
        }

        private void HandleLog(string logString, string logStackTrace, LogType logType)
        {
            if (!m_IsInited) return;
            switch (logType)
            {
                case LogType.Log:
                    WriteLogToFile(logString);
                    break;
                case LogType.Error:
                    WriteLogToFile(logString + "\n");
                    break;
                case LogType.Exception:
                    WriteLogToFile(logString + "\n" + logStackTrace + "\n");
                    break;
            }
        }

        private void WriteLogToFile(string log)
        {
            try
            {
                m_Write.WriteLine(log);
                m_Write.Flush();
            }
            catch (Exception e)
            {
                m_IsInited = false;
                Application.logMessageReceived -= HandleLog;
                Debug.LogError("BuildLog ERROR: write log to file failed: " + e.Message);
            }
        }

        public static void Log(string log)
        {
            Debug.Log(PackLog(log, "INFO"));
        }

        public static void LogError(string log)
        {
            Debug.LogError(PackLog(log, "ERROR"));
        }

        public static void LogWarning(string log)
        {
            Debug.LogWarning(PackLog(log, "WARN"));
        }

        public static string GetCurrTime()
        {
            return DateTime.Now.ToString("[MM-dd HH:mm:ss fff]", DateTimeFormatInfo.InvariantInfo);
        }

        private static string PackLog(string log, string type = "")
        {
            if (string.IsNullOrEmpty(type)) return string.Format("{0}\t{1}", GetCurrTime(), log);
            else return string.Format("{0}[{1}]\t{2}", GetCurrTime(), type, log);
        }
    }
}