﻿using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;
using System.Linq;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set Property (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	[Description("Set a property on a script")]
	public class SetProperty_Multiplatform : ActionTask {

		[SerializeField]
		protected SerializedMethodInfo method;
		[SerializeField]
		protected BBObjectParameter parameter;

		private MethodInfo targetMethod{
			get {return method != null? method.Get() : null;}
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
				return string.Format("{0}.{1} = {2}", agentInfo, targetMethod.Name, parameter.ToString() );
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

		protected override string OnInit(){
			if (method == null){
				return "No property selected";
			}
			if (targetMethod == null){
				return string.Format("Missing property '{0}'", method.GetMethodString());
			}
			return null;
		}

		protected override void OnExecute(){
			targetMethod.Invoke(agent, new object[]{parameter.value});
			EndAction();
		}

		void SetMethod(MethodInfo method){
			if (method != null){
				this.method = new SerializedMethodInfo(method);
				this.parameter.SetType(method.GetParameters()[0].ParameterType);				
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
						menu = EditorUtils.GetMethodSelectionMenu(comp.GetType(), typeof(void), typeof(object), SetMethod, 1, true, false, menu);
					}
					menu.AddSeparator("/");
				}
				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(Component))){
					menu = EditorUtils.GetMethodSelectionMenu(t, typeof(void), typeof(object), SetMethod, 1, true, false, menu);
				}
				if ( NodeCanvas.Editor.NCPrefs.useBrowser){ menu.ShowAsBrowser("Select Property", this.GetType()); }
				else { menu.ShowAsContext(); }
				Event.current.Use();				
			}

			if (targetMethod != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Type", agentType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
				UnityEditor.EditorGUILayout.LabelField("Set Type", parameter.varType.FriendlyName() );
				GUILayout.EndVertical();
				EditorUtils.BBParameterField("Set Value", parameter);
			}
		}

		#endif
	}
}