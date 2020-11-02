

using System;
using UnityEngine;
using UnityEngine.UI;

namespace XDebug
{
    [Serializable]
    public class Icon: IEquatable<Icon>
    {
        [SerializeField] private float m_Width;
        [SerializeField] private float m_Height;
        [SerializeField] private Color m_Color;

        public Button button { get; set; }
        public float width { get { return m_Width; } set { m_Width = value; } }
        public float height { get { return m_Height; } set { m_Height = value; } }
        public Color color { get { return m_Color; } set { m_Color = value; } }

        public static Icon defaultIcon
        {
            get
            {
                return new Icon()
                {
                    m_Width = 50,
                    m_Height = 50,
                    m_Color = Color.white
                };
            }
        }

        public void Copy(Icon icon)
        {
            m_Width = icon.width;
            m_Height = icon.height;
            m_Color = icon.color;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else if (obj is Icon)
            {
                return Equals((Icon)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Icon other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return width == other.width &&
                height == other.height &&
                color == other.color;
        }

        public static bool operator ==(Icon left, Icon right)
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

        public static bool operator !=(Icon left, Icon right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}