using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace XDebug
{
    [DisallowMultipleComponent]
    public class XLog : MonoBehaviour
    {
        public class MyInfo
        {
            private float startTime;
            public string apkVersion = "0.1.0";
            public string latestAssetVersion = "0.1.0";
            public string localAssetVersion = "0.1.0";
            public int runMode = 0;
            public int serverType = 0;
            public int sdkPlatformId = 0;
            public string sdkPlatformTag = "sdk";
            public string suid = "0";
            public string sdkPlatformName = "0";
            public string roleName = "roleName";
            public string roleId = "0";
            public int lbsProvinceCode = 0;
            public string lbsCity = "city";
            public string lbsIp = "ip";
            public string deviceName { get; private set; }
            public string deviceId { get; private set; }
            public string os { get; private set; }
            public string osVersion { get; private set; }
            public string cpu { get; private set; }
            public string gpu { get; private set; }
            public int networkType { get; set; }
            public string networkOperator { get; set; }
            public int mapId = 0;

            public int gameTime { get { return (int)(Time.time - startTime) * 1000; } }

            public void Init()
            {
                startTime = Time.time;
                deviceName = SystemInfo.deviceName;
                deviceId = SystemInfo.deviceUniqueIdentifier;
                os = SystemInfo.operatingSystem;
                osVersion = Regex.Match(os, @"((\d+)\.?)+", RegexOptions.ECMAScript).Value;
                os = os.Substring(0, os.IndexOf(osVersion) - 1);
                cpu = SystemInfo.processorType;
                gpu = SystemInfo.graphicsDeviceVersion;
                networkType = GetNetworkType();
            }

            private int GetNetworkType()
            {
                switch (Application.internetReachability)
                {
                    case NetworkReachability.NotReachable: return 0;
                    case NetworkReachability.ReachableViaLocalAreaNetwork: return 1;
                    case NetworkReachability.ReachableViaCarrierDataNetwork: return 2;
                    default: return 1;
                }
            }
        }

        public enum LogLevel
        {
            All,
            Debug,
            Warn,
            Info,
            Proto,
            Vital,
            Error
        }

        [SerializeField] int m_MaxErrorNum = 100;

        private static bool s_Inited;
        private static List<LogLevel> s_LevelList = new List<LogLevel>();
        private static List<string> s_writeList = new List<string>();
        private static List<string> s_ErrorList = new List<string>();
        private static List<string> s_AllLogList = new List<string>();
        private static Dictionary<int, List<string>> s_RoleLogList = new Dictionary<int, List<string>>();

        private StreamWriter m_Writer;
        private string[] m_WriteTemps;
        private static XLog m_Instance = null;

        public static int errorCount = 0;
        public static int exceptionCount = 0;
        public static MyInfo myInfo = new MyInfo();

        public static StringBuilder sb = new StringBuilder();

        public static XLog Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = string.Format("__{0}__", typeof(XLog).Name);
                    m_Instance = obj.AddComponent<XLog>();
                    obj.AddComponent<XLogView>();
                    var root = GameObject.Find("MonoSingleton");
                    if (root == null)
                    {
                        root = new GameObject("MonoSingleton");
                        DontDestroyOnLoad(root);
                    }
                    obj.transform.SetParent(root.transform);
                    return m_Instance;
                }
                else
                {
                    return m_Instance;
                }
            }
        }

        public void Init()
        {
        }

        void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
                GameObject.DontDestroyOnLoad(gameObject);
                myInfo.Init();
                InitLogFile();
                if (s_Inited)
                {
                    Application.logMessageReceived += HandleLog;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (s_writeList.Count > 0)
            {
                if (!s_Inited)
                {
                    s_writeList.Clear();
                }
                else
                {
                    try
                    {
                        m_WriteTemps = s_writeList.ToArray();
                        int count = 0;
                        foreach (var log in m_WriteTemps)
                        {
                            count++;
                            m_Writer.WriteLine(log);
                            s_writeList.Remove(log);
                            if (count > 10) break;
                        }
                        m_Writer.Flush();
                    }
                    catch (Exception e)
                    {
                        s_Inited = false;
                        Application.logMessageReceived -= HandleLog;
                        UnityEngine.Debug.LogError("write to xlog.txt ERROR:" + e.Message);
                    }
                }
            }
        }

        void InitLogFile()
        {

#if UNITY_EDITOR
            AddLevel(LogLevel.All);
#else
			AddLevel(LogLevel.Vital);
			AddLevel(LogLevel.Error);
#endif
            string path = Application.persistentDataPath + "/xlog.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                var msg = string.Format("init log file: %s", path);
                m_Writer = new StreamWriter(path, false, Encoding.UTF8);
                m_Writer.Flush();
                s_Inited = true;
                Vital("init log file: " + path);
            }
            catch (Exception e)
            {
                s_Inited = false;
                Error("init log file ERROR:" + e.Message);
            }
        }

        void HandleLog(string logString, string logTrace, LogType type)
        {
            if (!s_Inited) return;
            if (type == LogType.Log)
            {
                s_writeList.Add(GetLogInfo(logString));
            }
            else if (type == LogType.Error)
            {
                errorCount++;
                s_AllLogList.Add(logString);
                s_writeList.Add(logString);
                if (s_ErrorList.Count > m_MaxErrorNum)
                {
                    s_ErrorList.RemoveAt(1);
                    s_ErrorList.Add(logString);
                }
            }
            else if (type == LogType.Exception)
            {
                exceptionCount++;
                s_AllLogList.Add(logString + "\n" + logTrace);
                s_writeList.Add(logString + "\n" + logTrace);
                if (s_ErrorList.Count > m_MaxErrorNum)
                {
                    s_ErrorList.RemoveAt(1);
                    s_ErrorList.Add(logString + "\n" + logTrace);
                }
            }
        }

        public static void AddLevel(int level)
        {
            if (level < 0) return;
            AddLevel((LogLevel)level);
        }

        public static void ClearLevel()
        {
            s_LevelList.Clear();
        }

        public static void AddLevel(LogLevel level)
        {
            if (!s_LevelList.Contains(level))
            {
                s_LevelList.Add(level);
            }
        }

        public static void Debug(string log)
        {
            if (s_LevelList.Contains(LogLevel.Debug) || s_LevelList.Contains(LogLevel.All))
            {

                UnityEngine.Debug.Log(Helper.GetNowTime() + "[Debug]\t" + log);
            }
        }

        public static void Warn(string log)
        {
            if (s_LevelList.Contains(LogLevel.Warn) || s_LevelList.Contains(LogLevel.All))
            {
                UnityEngine.Debug.Log(Helper.GetNowTime() + "[Warn]\t" + log);
            }
        }

        public static void Info(string log)
        {
            if (s_LevelList.Contains(LogLevel.Info) || s_LevelList.Contains(LogLevel.All))
            {
                log = GetLogInfo(log);
                UnityEngine.Debug.Log(Helper.GetNowTime() + "[Info]\t" + log);
            }
        }

        public static void Proto(string log)
        {
            if (s_LevelList.Contains(LogLevel.Proto) || s_LevelList.Contains(LogLevel.All))
            {
                UnityEngine.Debug.Log(Helper.GetNowTime() + "[Proto]\t" + log);
            }
        }

        public static void Vital(string log)
        {
            if (s_LevelList.Contains(LogLevel.Vital) || s_LevelList.Contains(LogLevel.All))
            {
                UnityEngine.Debug.Log(Helper.GetNowTime() + "[Vital]\t" + log);
            }
        }

        public static void Error(string log)
        {
            if (s_LevelList.Contains(LogLevel.Error) || s_LevelList.Contains(LogLevel.All))
            {
                UnityEngine.Debug.LogError(Helper.GetNowTime() + "[Error]\t" + log);
            }
        }

        public static string GetLogInfo(string log)
        {
            if (!string.IsNullOrEmpty(log) && log.Length > 4096)
            {
                log = log.Substring(0, 4096);
            }
            sb.Length = 0;
            sb.AppendFormat("{0}${1}${2}$", myInfo.latestAssetVersion, myInfo.localAssetVersion, myInfo.apkVersion);
            sb.AppendFormat("{0}${1}$", myInfo.runMode, myInfo.serverType);
            sb.AppendFormat("{0}${1}${2}$", Application.identifier, myInfo.sdkPlatformId, myInfo.sdkPlatformTag);
            sb.AppendFormat("{0}$", myInfo.suid);
            sb.AppendFormat("{0}$", myInfo.sdkPlatformName);
            sb.AppendFormat("{0}${1}$", myInfo.roleName, myInfo.roleId);
            sb.AppendFormat("{0}${1}$", myInfo.lbsProvinceCode, myInfo.gameTime);
            sb.AppendFormat("{0}${1}${2}${3}${4}${5}$", myInfo.deviceName, myInfo.os, myInfo.osVersion, myInfo.deviceId, myInfo.networkType, myInfo.networkOperator);
            sb.AppendFormat("{0}${1}$", myInfo.cpu, myInfo.gpu);
            sb.AppendFormat("{0}${1}$", myInfo.lbsCity, myInfo.lbsIp);
            sb.AppendFormat("{0}${1}$", errorCount, exceptionCount);
            sb.AppendFormat("{0}", myInfo.mapId);
            sb.Append("\n");
            sb.Append(log).Append("\nEND\n");
            return sb.ToString();
        }

        private static IEnumerator UploadLog(string log, string trace)
        {
            if (log == null) log = "";
            else log = log.Trim();
            if (!string.IsNullOrEmpty(log))
                log = string.Format("{0}\n{1}", log, trace);
            var form = new WWWForm();
            form.AddField("log", GetLogInfo(log));
            var web = UnityWebRequest.Post("", form);
            yield return web.SendWebRequest();
            if (web.isNetworkError || web.isHttpError)
            {
                UnityEngine.Debug.Log(web.error);
            }
        }
    }
}