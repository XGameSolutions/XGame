
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XDebug
{
    [Serializable]
    public enum Orient
    {
        Horizonal,
        Vertical
    }

    [Serializable]
    public class Module
    {
        [Serializable]
        public enum Type
        {
            Text,
            Chart
        }
        [SerializeField] private Type m_Type;
        [SerializeField] private string m_Key;
        [SerializeField] private string m_Name;
        [SerializeField] private Color m_Color;

        public Type type { get { return m_Type; } set { m_Type = value; } }
        public string key { get { return m_Key; } set { m_Key = value; } }
        public string name { get { return m_Name; } set { m_Name = value; } }
        public Color color { get { return m_Color; } set { m_Color = value; } }

        public GameObject panel { get; set; }
        public Button button { get; set; }
        public Text text { get; set; }

        public static List<Module> defaultModules
        {
            get
            {
                return new List<Module>(){
                    new Module(){
                        m_Type = Type.Text,
                        m_Key = "sys",
                        m_Name = "sys",
                        m_Color = Color.white
                    },
                    new Module(){
                        m_Type = Type.Text,
                        m_Key = "my",
                        m_Name = "my",
                        m_Color = Color.white
                    },
                    new Module(){
                        m_Type = Type.Text,
                        m_Key = "other",
                        m_Name = "other",
                        m_Color = Color.white
                    }
                };
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else if (obj is Module)
            {
                return Equals((Module)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Module other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return type == other.type &&
                key == other.key &&
                name == other.name &&
                color == other.color;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    [Serializable]
    public class Modules : IEquatable<Modules>
    {
        [SerializeField] private Orient m_Orient;
        [SerializeField] private float m_Width;
        [SerializeField] private float m_Height;
        [SerializeField] private float m_Gap;
        [SerializeField] List<Module> m_ModuleList;

        public Orient orient { get { return m_Orient; } set { m_Orient = value; } }
        public float gap { get { return m_Gap; } set { m_Gap = value; } }
        public float width { get { return m_Width; } set { m_Width = value; } }
        public float height { get { return m_Height; } set { m_Height = value; } }
        public List<Module> moduleList { get { return m_ModuleList; } set { m_ModuleList = value; } }

        public static Modules defaultModules
        {
            get
            {
                return new Modules()
                {
                    m_Width = 50,
                    m_Height = 50,
                    m_Gap = 5,
                    m_ModuleList = Module.defaultModules
                };
            }
        }

        public float GetModulesWidth()
        {
            int size = m_ModuleList.Count;
            return m_Width * size + m_Gap * (size - 1);
        }

        public Vector2 GetModulesPosition(Location location, Icon icon)
        {
            switch (m_Orient)
            {
                case Orient.Vertical:
                    switch (location.align)
                    {
                        case Location.Align.TopLeft:
                        case Location.Align.TopRight:
                        case Location.Align.TopCenter:
                            return new Vector2(0, -icon.height - m_Gap);
                        case Location.Align.CenterRight:
                        case Location.Align.Center:
                        case Location.Align.CenterLeft:
                            return new Vector2(0, icon.height + m_Gap + icon.height / 2 + m_Gap);
                        case Location.Align.BottomLeft:
                        case Location.Align.BottomRight:
                        case Location.Align.BottomCenter:
                            return new Vector2(0, icon.height + m_Gap);
                    }
                    return Vector2.zero;

                case Orient.Horizonal:
                    switch (location.align)
                    {
                        case Location.Align.TopLeft:
                        case Location.Align.CenterLeft:
                        case Location.Align.BottomLeft:
                            return new Vector2(icon.width + m_Gap, 0);
                        case Location.Align.TopRight:
                        case Location.Align.CenterRight:
                        case Location.Align.BottomRight:
                            return new Vector2(-icon.width - m_Gap, 0);
                        case Location.Align.Center:
                        case Location.Align.TopCenter:
                            return new Vector2(0, 0);
                        case Location.Align.BottomCenter:
                            return new Vector2(0, 0);
                    }
                    return Vector2.zero;
            }
            return Vector2.zero;
        }

        public Vector2 GetModulePosition(Location location, int index)
        {
            int size = m_ModuleList.Count;
            switch (m_Orient)
            {
                case Orient.Vertical:
                    switch (location.align)
                    {
                        case Location.Align.TopCenter:
                        case Location.Align.TopLeft:
                        case Location.Align.TopRight:
                            return new Vector2(0, -index * (m_Height + m_Gap));

                        case Location.Align.Center:
                        case Location.Align.CenterLeft:
                        case Location.Align.CenterRight:
                            float totalHeight = size * m_Height + (size - 1) * m_Gap;
                            float startY = totalHeight / 2;
                            return new Vector2(0, startY - index * (m_Height + m_Gap));

                        case Location.Align.BottomCenter:
                        case Location.Align.BottomLeft:
                        case Location.Align.BottomRight:
                            return new Vector2(0, (size - index - 1) * (m_Height + m_Gap));
                    }
                    return Vector2.zero;

                case Orient.Horizonal:
                    switch (location.align)
                    {
                        case Location.Align.TopLeft:
                        case Location.Align.CenterLeft:
                        case Location.Align.BottomLeft:
                            return new Vector2(index * (m_Width + m_Gap), 0);

                        case Location.Align.TopCenter:
                        case Location.Align.Center:
                        case Location.Align.BottomCenter:
                            float totalWidth = size * m_Width + (size - 1) * m_Gap;
                            float startX = totalWidth / 2;
                            return new Vector2(-startX + m_Width / 2 + index * (m_Width + m_Gap), 0);
                        case Location.Align.TopRight:
                        case Location.Align.CenterRight:
                        case Location.Align.BottomRight:
                            return new Vector2(-(size - index - 1) * (m_Width + m_Gap), 0);
                    }
                    return Vector2.zero;
            }
            return Vector2.zero;
        }

        public void Copy(Modules modules)
        {
            orient = modules.orient;
            width = modules.width;
            height = modules.height;
            gap = modules.gap;
            m_ModuleList.Clear();
            foreach (var d in modules.moduleList) m_ModuleList.Add(d);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else if (obj is Location)
            {
                return Equals((Location)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Modules other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return orient == other.orient &&
                width == other.width &&
                height == other.height &&
                gap == other.gap &&
                Helper.IsValueEqualsList<Module>(moduleList, other.moduleList);
        }

        public static bool operator ==(Modules left, Modules right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }
            else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }
            return Equals(left, right);
        }

        public static bool operator !=(Modules left, Modules right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}