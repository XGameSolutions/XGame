/******************************************/
/*                                        */
/*     Copyright (c) 2020 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System.Text;
using System.Text.RegularExpressions;

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
        public string apkFileName;
        public string companyName;

        public bool isDebug;
        public string startScene;

        public BuildParams()
        {
            productName = BuildConfig.productName;
            apkFileName = BuildConfig.apkFileName;
            applicationIdentifier = BuildConfig.applicationIdentifier;
            apkVersion = "0.1.0";
            resVersion = "0.1.0";
            appleDeveloperTeamID = "teamId";
            companyName = BuildConfig.companyName;

            isDebug = false;
            startScene = BuildConfig.startScenePath;
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
            sb.AppendFormat("apkFileName={0}\n", apkFileName);
            sb.AppendFormat("companyName={0}\n", companyName);

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
                else if (key.Equals("apkFileName")) param.apkFileName = value;
                else if (key.Equals("companyName")) param.companyName = value;
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

