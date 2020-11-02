

using UnityEngine;

public class ShaderMgr
{
    private AssetBundle m_ShaderAb;

    public void Init(AssetBundle ab)
    {
        m_ShaderAb = ab;
    }

    public Shader Find(string shaderName, string shaderFileName)
    {
        Shader shader = null;
        if (m_ShaderAb != null)
        {
            shader = m_ShaderAb.LoadAsset<Shader>(shaderFileName);
        }
        if (shader == null)
        {
            shader = Shader.Find(shaderName);
        }
        return shader;
    }
}