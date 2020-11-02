// using UnityEditor;
// using UnityEngine;

// namespace XDebug
// {
//     [CustomPropertyDrawer(typeof(Module), true)]
//     public class ModuleDrawer : PropertyDrawer
//     {
//         SerializedProperty m_Type;
//         SerializedProperty m_Key;
//         SerializedProperty m_Text;
//         SerializedProperty m_Width;
//         SerializedProperty m_Height;
//         SerializedProperty m_Color;
//         bool m_BarModuleToggle = true;

//         private void InitProperty(SerializedProperty prop)
//         {
//             m_Type = prop.FindPropertyRelative("m_Type");
//             m_Key = prop.FindPropertyRelative("m_Key");
//             m_Text = prop.FindPropertyRelative("m_Text");
//             m_Width = prop.FindPropertyRelative("m_Width");
//             m_Height = prop.FindPropertyRelative("m_Height");
//             m_Color = prop.FindPropertyRelative("m_Color");
//         }

//         public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
//         {
//             InitProperty(prop);
//             Rect drawRect = pos;
//             drawRect.height = EditorGUIUtility.singleLineHeight;

//             EditorGUI.PropertyField(drawRect, m_Type);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             EditorGUI.PropertyField(drawRect, m_Key);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             EditorGUI.PropertyField(drawRect, m_Text);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             EditorGUI.PropertyField(drawRect, m_Width);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             EditorGUI.PropertyField(drawRect, m_Height);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             EditorGUI.PropertyField(drawRect, m_Color);
//             drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//         }

//         public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
//         {
//             return 6 * EditorGUIUtility.singleLineHeight + 5 * EditorGUIUtility.standardVerticalSpacing;
//         }
//     }
// }