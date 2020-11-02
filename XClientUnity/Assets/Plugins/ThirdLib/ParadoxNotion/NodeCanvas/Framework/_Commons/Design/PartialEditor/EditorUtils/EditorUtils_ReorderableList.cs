#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace ParadoxNotion.Design{

	partial class EditorUtils {
		
		public struct ReorderableListOptions{
			public bool allowRemove;
			public bool allowAdd;
		}

		public delegate void ReorderableListCallback(int index, bool isReordering);
		private static readonly Dictionary<IList, int> pickedListIndex = new Dictionary<IList, int>();

		/// A simple reorderable list. Pass the list and a function to call for GUI. The callback comes with the current iterated element index in the list
		public static void ReorderableList(IList list, ReorderableListOptions options, ReorderableListCallback GUICallback, UnityObject unityObject = null){
			Internal_ReorderableList(list, options, GUICallback, unityObject);
		}

		/// A simple reorderable list. Pass the list and a function to call for GUI. The callback comes with the current iterated element index in the list
		public static void ReorderableList(IList list, ReorderableListCallback GUICallback, UnityObject unityObject = null){
			Internal_ReorderableList(list, default(ReorderableListOptions), GUICallback, unityObject);
		}

		/// A simple reorderable list. Pass the list and a function to call for GUI. The callback comes with the current iterated element index in the list
		static void Internal_ReorderableList(IList list, ReorderableListOptions options, ReorderableListCallback GUICallback, UnityObject unityObject){

			if (list == null || list.Count == 0){
				return;
			}

			if (!pickedListIndex.ContainsKey(list)){
				pickedListIndex[list] = -1;
			}

			var e = Event.current;
			var pickedIndex = pickedListIndex[list];
			var handleStyle = new GUIStyle("label");
			handleStyle.alignment = TextAnchor.MiddleCenter;
			for (var i = 0; i < list.Count; i++){
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(16);
				GUILayout.BeginVertical();
				GUICallback(i, pickedIndex == i);
				GUILayout.EndVertical();
				var lastRect = GUILayoutUtility.GetLastRect();
				var pickRect = Rect.MinMaxRect(lastRect.xMin - 16, lastRect.yMin, lastRect.xMin, lastRect.yMax);
				GUI.color = new Color(1,1,1,0.5f);
				GUI.Label(pickRect, "☰", handleStyle);
				GUI.color = Color.white;
				if (options.allowRemove){
					GUILayout.Space(16);
					var removeRect = Rect.MinMaxRect(lastRect.xMax, lastRect.yMin, lastRect.xMax + 16, lastRect.yMax);
					if (GUI.Button(removeRect, "X")){
						if (unityObject != null){ Undo.RecordObject(unityObject, "Remove Item"); }
						list.RemoveAt(i);
						GUI.changed = true;
						if (unityObject != null){ EditorUtility.SetDirty(unityObject); }
					}
				}
				GUILayout.EndHorizontal();

				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;

				EditorGUIUtility.AddCursorRect(pickRect, MouseCursor.MoveArrow);
				var boundRect = RectUtils.GetBoundRect(lastRect, pickRect);

				if (pickRect.Contains(e.mousePosition) && e.type == EventType.MouseDown){
					pickedListIndex[list] = i;
				}

				if (pickedIndex == i){
					GUI.Box(boundRect, string.Empty);
				}

				if (pickedIndex != -1 && pickedIndex != i && boundRect.Contains(e.mousePosition) ){

					var markRect = new Rect(boundRect.x, boundRect.y - 2, boundRect.width, 2);
					if (pickedIndex < i){
						markRect.y = boundRect.yMax - 2;
					}

					GUI.Box(markRect, string.Empty);
					if (e.type == EventType.MouseUp){
						if (unityObject != null){ Undo.RecordObject(unityObject, "Reorder Item"); }
						var obj = list[pickedIndex];
						list.RemoveAt(pickedIndex);
						list.Insert(i, obj);
						pickedListIndex[list] = -1;
						GUI.changed = true;
						if (unityObject != null){ EditorUtility.SetDirty(unityObject); }
					}
				}
			}

			if (e.rawType == EventType.MouseUp){
				pickedListIndex[list] = -1;
			}

		}
	}
}

#endif