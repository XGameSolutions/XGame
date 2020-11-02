
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XDebug
{


    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(XLog))]
    public class XLogView : MonoBehaviour
    {
        [SerializeField] private Icon m_Icon = Icon.defaultIcon;
        [SerializeField] private Location m_Location = Location.defaultRight;
        [SerializeField] private Modules m_Modules = Modules.defaultModules;

        private Font m_Font;
        private RectTransform m_RectTransform;
        private Location m_CheckLocation = Location.defaultRight;
        private Icon m_CheckIcon = Icon.defaultIcon;

        private GameObject m_ModulesObj;
        private bool m_ModulesActive = false;
        private Dictionary<string, Text> m_ModulesText = new Dictionary<string, Text>();

        private void Awake()
        {
            m_Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_RectTransform = gameObject.GetComponent<RectTransform>();
            m_RectTransform.anchorMin = Vector2.zero;
            m_RectTransform.anchorMax = Vector2.one;
            m_RectTransform.offsetMin = Vector2.zero;
            m_RectTransform.offsetMax = Vector2.zero;
            m_CheckIcon.Copy(m_Icon);
            InitIcon();
            InitModules();
        }

        private void Update()
        {
            CheckLocaltion();
            CheckIcon();
        }

        private void InitIcon()
        {
            TextAnchor anchor = TextAnchor.MiddleCenter;
            Vector2 size = new Vector2(m_Icon.width, m_Icon.height);
            m_Icon.button = Helper.AddButtonObject("icon", transform, m_Font, 30, Color.black, anchor,
                m_Location.anchorMin, m_Location.anchorMax, m_Location.pivot, size);
            m_Icon.button.GetComponent<Image>().color = m_Icon.color;
            m_Icon.button.GetComponentInChildren<Text>().text = "X";
            UpdateIconPosition();
            Helper.AddEventListener(m_Icon.button.gameObject, EventTriggerType.PointerDown, (data) =>
            {
                m_ModulesActive = !m_ModulesActive;
                m_ModulesObj.SetActive(m_ModulesActive);
                UpdateIconPosition();
            });
        }

        private void UpdateIconPosition()
        {
            Vector2 anchoredPos = Vector2.zero;
            if (m_ModulesActive && m_Modules.orient == Orient.Horizonal)
            {
                switch (m_Location.align)
                {
                    case Location.Align.TopCenter:
                    case Location.Align.Center:
                    case Location.Align.BottomCenter:
                        anchoredPos = new Vector2(-m_Modules.GetModulesWidth() / 2 - m_Icon.width / 2 - m_Modules.gap, 0);
                        Debug.LogError(anchoredPos);
                        break;

                }
            }
            m_Icon.button.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        }

        private void InitModules()
        {
            Helper.HideAllObject(transform, "modules");
            m_ModulesObj = Helper.AddObject("modules", transform, m_Location.anchorMin, m_Location.anchorMax,
                m_Location.pivot, new Vector2(50, 50));
            var moduleSize = new Vector2(m_Modules.width, m_Modules.height);
            var anchor = TextAnchor.MiddleCenter;
            m_ModulesObj.GetComponent<RectTransform>().anchoredPosition = m_Modules.GetModulesPosition(m_Location, m_Icon);
            m_ModulesObj.SetActive(m_ModulesActive);
            for (int i = 0; i < m_Modules.moduleList.Count; i++)
            {
                var module = m_Modules.moduleList[i];
                module.button = Helper.AddButtonObject("module_btn_" + i, m_ModulesObj.transform, m_Font, 14, Color.black,
                    anchor, m_Location.anchorMin, m_Location.anchorMax, m_Location.pivot, moduleSize);
                module.button.transform.localPosition = m_Modules.GetModulePosition(m_Location, i);
                module.button.GetComponent<Image>().color = module.color;
                module.button.GetComponentInChildren<Text>().text = module.name;

                module.panel = Helper.AddObject("module_" + i, transform, m_Location.anchorMin,
                    m_Location.anchorMax, m_Location.pivot, new Vector2(100, 100));
                module.panel.transform.localPosition =new Vector2(m_Icon.button.transform.position.x + i*(m_Modules.width+m_Modules.gap),-m_Icon.height); 
                Helper.GetOrAddComponent<Image>(module.panel);
                module.text = Helper.AddTextObject("Text", module.panel.transform, m_Font, Color.black, TextAnchor.UpperLeft, 
                    m_Location.anchorMin, m_Location.anchorMax, m_Location.pivot, new Vector2(0, 0), 20);
            //module.button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * (m_Modules.height + m_Modules.gap));
        }
    }

    private void CheckLocaltion()
    {
        if (m_CheckLocation != m_Location)
        {
            m_CheckLocation.Copy(m_Location);
            m_Location.OnChanged();
            InitIcon();
            InitModules();
        }
    }
    private void CheckIcon()
    {
        if (m_CheckIcon != m_Icon)
        {
            m_CheckIcon.Copy(m_Icon);
            InitIcon();
        }
    }
}
}
