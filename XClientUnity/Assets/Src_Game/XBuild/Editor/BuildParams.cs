using System.Text;
using System.Text.RegularExpressions;
/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace XBuild
{
    public class BuildParams
    {
        private static readonly Regex regex = new Regex(@"\{?(\w+[=|:]\w+,)*\w+[=|:]\w+\}?");
        private static StringBuilder sb = new StringBuilder();

        public string apkVersion;
        public string resVersion;
        public string productName;
        public string applicationIdentifier;
        public int bundleVersionCode;
        public string appleDeveloperTeamID;

        public bool isDebug;
        public string startScene;

        public BuildParams()
        {
            productName = "project-name";
            applicationIdentifier = "com.company-name.project-name";
            apkVersion = "0.1.0";
            resVersion = "0.1.0";
            appleDeveloperTeamID = "teamId";

            isDebug = false;
            startScene = "Assets/Scenes/SampleScene.unity";
        }

        public override string ToString()
        {
            sb.Length = 0;
            sb.Append("BuildParams:\n");
            sb.AppendFormat("productName={0}\n", productName);
            sb.AppendFormat("apkVersion={0}\n", apkVersion);
            sb.AppendFormat("resVersion={0}\n", apkVersion);
            sb.AppendFormat("applicationIdentifier={0}\n", applicationIdentifier);
            sb.AppendFormat("bundleVersionCode={0}\n", applicationIdentifier);
            sb.AppendFormat("appleDeveloperTeamID={0}\n", appleDeveloperTeamID);

            sb.AppendFormat("isDebug={0}\n", isDebug);
            sb.AppendFormat("startScene={0}\n", startScene);

            return sb.ToString();
        }

        public static BuildParams Parse(string strParams)
        {
            var param = new BuildParams();
            if (string.IsNullOrEmpty(strParams)) return param;
            if (!regex.IsMatch(strParams)) return param;
            strParams = strParams.Replace('{', ' ');
            strParams = strParams.Replace('}', ' ');
            var alls = strParams.Split(',');
            foreach (var str in alls)
            {
                var subs = str.Split(new char[] { '=', ':' });
                if (subs == null || subs.Length != 2) continue;
                var key = subs[0].Trim();
                var value = subs[1].Trim();
                if (key.Equals("productName")) param.productName = value;
                else if (key.Equals("apkVersion")) param.apkVersion = value;
                else if (key.Equals("resVersion")) param.resVersion = value;
                else if (key.Equals("applicationIdentifier")) param.resVersion = value;
                else if (key.Equals("bundleVersionCode")) param.bundleVersionCode = ParseInt(value);
                else if (key.Equals("isDebug")) param.isDebug = ParseBool(value);
                else if (key.Equals("appleDeveloperTeamID")) param.appleDeveloperTeamID = value;
            }
            return param;
        }

        private static bool ParseBool(string value)
        {
            if (!bool.TryParse(value, out bool flag))
            {
                if (int.TryParse(value, out int test)) flag = test == 1;
            }
            return flag;
        }

        private static int ParseInt(string value)
        {
            if (int.TryParse(value, out int test)) return test;
            else return 0;
        }
    }
}

