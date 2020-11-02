using System.IO;
using UnityEngine;
using XLua;
using XDebug;

public class GameStart : MonoSingleton<GameStart>
{
    public int pkgType = 0;
    public bool IsAbRes = false;
    public bool IsAbLua = false;
    public bool IsAbCfg = false;

    protected override void OnInit()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IsAbRes = true;
            IsAbLua = true;
            IsAbCfg = true;
        }
        XLog.Instance.Init();
        ABMgr.Instance.Init();
        XLuaMgr.Instance.Init();
        BTDebug.Init(XLuaMgr.Instance.GetLuaEnv());
    }

    public override void Startup()
    {
        XLuaMgr.Instance.Startup();
    }

    private void OnDestroy()
    {
    }
}
