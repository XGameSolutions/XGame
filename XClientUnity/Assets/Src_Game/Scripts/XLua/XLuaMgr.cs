


using System;
using System.IO;
using UnityEngine;
using XLua;

public class XLuaMgr : MonoSingleton<XLuaMgr>
{
    private LuaEnv m_LuaEnv;
    private Action<float, float> m_LuaUpdate;
    private Action<float, float> m_LuaLateUpdate;
    private Action<float> m_LuaFixedUpdate;

    protected override void OnInit()
    {
        InitLuaEnv();
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            var luaPath = Application.dataPath + "/../../XClientLua/?.lua";
            DoString(string.Format("package.path='{0}'", luaPath));
        }
        LoadLuaScript("startup");
    }

    private void Update()
    {
        if (m_LuaEnv != null)
        {
            m_LuaEnv.Tick();
        }
        if (m_LuaUpdate != null)
        {
            try
            {
                m_LuaUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("LuaUpdate ERROR:{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }

    private void LateUpdate()
    {
        if (m_LuaLateUpdate != null)
        {
            try
            {
                m_LuaLateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("LuaLateUpdate ERROR:{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_LuaFixedUpdate != null)
        {
            try
            {
                m_LuaFixedUpdate(Time.fixedDeltaTime);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("LuaFixedUpdate ERROR:{0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }

    private void InitLuaEnv()
    {
        m_LuaEnv = new LuaEnv();
        m_LuaEnv.AddLoader(CustomLoader);
    }

    public override void Startup()
    {
        m_LuaUpdate = m_LuaEnv.Global.Get<Action<float, float>>("update");
        m_LuaLateUpdate = m_LuaEnv.Global.Get<Action<float, float>>("lateUpdate");
        m_LuaFixedUpdate = m_LuaEnv.Global.Get<Action<float>>("fixedUpdate");
    }

    public LuaEnv GetLuaEnv()
    {
        return m_LuaEnv;
    }

    public void DoString(string luacode)
    {
        if (m_LuaEnv == null) return;
        try
        {
            m_LuaEnv.DoString(luacode);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("XLua DoString ERROR:{0}\n{1}", e.Message, e.StackTrace));
        }
    }

    public void LoadLuaScript(string scriptName)
    {
        DoString(string.Format("require ('{0}')", scriptName));
    }

    public void ReloadLuaScript(string scriptName)
    {
        DoString(string.Format("package.loaded['{0}']=nil", scriptName));
        LoadLuaScript(scriptName);
    }

    private byte[] CustomLoader(ref string filename)
    {
        return ABMgr.LoadLuaFile(filename);
    }

    private void OnDestroy()
    {
        m_LuaUpdate = null;
        m_LuaLateUpdate = null;
        m_LuaFixedUpdate = null;
    }
}
