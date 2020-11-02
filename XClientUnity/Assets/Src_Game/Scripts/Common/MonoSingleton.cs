using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject(string.Format("__{0}__", typeof(T).Name));
                    m_Instance = obj.AddComponent<T>();
                    var root = GameObject.Find("MonoSingleton");
                    if (root == null)
                    {
                        root = new GameObject("MonoSingleton");
                        DontDestroyOnLoad(root);
                    }
                    obj.transform.SetParent(root.transform);
                }
            }
            return m_Instance;
        }
    }

    public void Init()
    {
    }

    public virtual void Startup()
    {
    }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnInit();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnInit()
    {
    }

    protected virtual void Dispose()
    {
    }
}