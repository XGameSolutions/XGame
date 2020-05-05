/*
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor{

	public class SceneGraphsEditorInspector : EditorWindow {

		private Dictionary<Type, List<GraphOwner>> graphOwners = new Dictionary<Type, List<GraphOwner>>();

		[MenuItem("Window/NodeCanvas/Inspect Scene GraphOwners")]
		public static void Open(){
			var win = GetWindow<SceneGraphsEditorInspector>();
			win.Show();
		}

		void OnEnable(){
	        #if UNITY_5_1 || UNITY_5_2 || UNITY_5_3
	        titleContent = new GUIContent("NC Graphs");
	        #else
	        title = "NC Graphs";
	        #endif

			Fetch();
			EditorApplication.playmodeStateChanged += () => { Fetch(); };
		}

		void Fetch(){
			graphOwners.Clear();
			foreach (var owner in FindObjectsOfType<GraphOwner>().OrderBy(o => o.GetType().Name) ){
				if (!graphOwners.ContainsKey(owner.GetType()))
					graphOwners[owner.GetType()] = new List<GraphOwner>();
				graphOwners[owner.GetType()].Add( owner );
			}
		}

		void OnGUI(){

			if (GUILayout.Button("Refresh")){
				Fetch();
			}

			foreach (var pair in graphOwners){

				GUILayout.BeginVertical("box");
				GUILayout.Label(string.Format("<size=18><b>{0}s</b></size>", pair.Key.Name) );
				EditorGUI.indentLevel ++;

				foreach (var owner in pair.Value){

					GUILayout.BeginVertical("box");
					if (GUILayout.Button(string.Format("► {0} ({1})", owner.name, owner.graph != null? owner.graph.name : "null"), (GUIStyle)"label" )){
						GraphEditor.OpenWindow(owner);
					}
					GUILayout.EndVertical();
				}
				
				EditorGUI.indentLevel --;
				GUILayout.EndVertical();
				GUILayout.Space(5);
			}
		}
	}
}
*/