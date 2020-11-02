﻿using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;
using System.Linq;


namespace NodeCanvas.Tasks.Actions{

	[Name("Get Property (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	[Description("Get a property of a script and save it to the blackboard")]
	public class GetProperty_Multiplatform : ActionTask {

		[SerializeField]
		protected SerializedMethodInfo method;
		[SerializeField] [BlackboardOnly]
		protected BBObjectParameter returnValue;

		private MethodInfo targetMethod{
			get {return method != null? method.Get() : null; }
		}

		public override System.Type agentType{
			get {return targetMethod != null? targetMethod.RTReflectedType() : typeof(Transform);}
		}

		protected override string info{
			get
			{
				if (method == null)
					return "No Property Selected";
				if (targetMethod == null)
					return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString() );
				return string.Format("{0} = {1}.{2}", returnValue.ToString(), agentInfo, targetMethod.Name);
			}
		}

		public override void OnValidate(ITaskSystem ownerSystem){
			if (method != null && method.HasChanged()){	
				SetMethod(method.Get());
			}
			if (method != null && method.Get() == null){
				Error( string.Format("Missing Property '{0}'", method.GetMethodString()) );
			}
		}

		//store the method info on init for performance
		protected override string OnInit(){
			if (method == null){
				return "No Property selected";
			}
			if (targetMethod == null){
				return string.Format("Missing Property '{0}'", method.GetMethodString());
			}
			return null;
		}

		//do it by invoking method
		protected override void OnExecute(){
			returnValue.value = targetMethod.Invoke(agent, null);
			EndAction();
		}

		void SetMethod(MethodInfo method){
			if (method != null){
				this.method = new SerializedMethodInfo(method);
				this.returnValue.SetType(method.ReturnType);			
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnTaskInspectorGUI(){

			if (!Application.isPlaying && GUILayout.Button("Select Property")){
				var menu = new UnityEditor.GenericMenu();
				if (agent != null){
					foreach(var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInInspector) ){
						menu = EditorUtils.GetMethodSelectionMenu(comp.GetType(), typeof(object), typeof(object), SetMethod, 0, true, true, menu);
					}
					menu.AddSeparator("/");
				}
				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(Component))){
					menu = EditorUtils.GetMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 0, true, true, menu);
				}

				if ( NodeCanvas.Editor.NCPrefs.useBrowser){ menu.ShowAsBrowser("Select Property", this.GetType()); }
				else { menu.ShowAsContext(); }
				Event.current.Use();
			}


			if (targetMethod != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Type", agentType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
				UnityEditor.EditorGUILayout.LabelField("Property Type", targetMethod.ReturnType.FriendlyName() );
				GUILayout.EndVertical();
				EditorUtils.BBParameterField("Save As", returnValue, true);
			}
		}

		#endif
	}
}
