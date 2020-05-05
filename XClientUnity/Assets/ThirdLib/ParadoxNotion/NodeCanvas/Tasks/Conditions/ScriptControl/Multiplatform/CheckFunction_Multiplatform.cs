using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Name("Check Function (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	[Description("Call a function on a component and return whether or not the return value is equal to the check value")]
	public class CheckFunction_Multiplatform : ConditionTask {

		[SerializeField]
		protected SerializedMethodInfo method;
		[SerializeField]
		protected List<BBObjectParameter> parameters = new List<BBObjectParameter>();
		[SerializeField] [BlackboardOnly]
		protected BBObjectParameter checkValue;
		[SerializeField]
		protected CompareMethod comparison;

		private object[] args;

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
					return "No Method Selected";
				if (targetMethod == null)
					return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString() );

				var paramInfo = "";
				for (var i = 0; i < parameters.Count; i++)
					paramInfo += (i != 0? ", " : "") + parameters[i].ToString();
				return string.Format("{0}.{1}({2}){3}", agentInfo, targetMethod.Name, paramInfo, OperationTools.GetCompareString(comparison) + checkValue);
			}
		}

		public override void OnValidate(ITaskSystem ownerSystem){
			if (method != null && method.HasChanged()){	
				SetMethod(method.Get());
			}
			if (method != null && method.Get() == null){
				Error( string.Format("Missing Method '{0}'", method.GetMethodString()) );
			}
		}

		//store the method info on agent set for performance
		protected override string OnInit(){
			if (targetMethod == null){
				return "CheckFunction Error";
			}

			if (args == null){
				args = new object[parameters.Count];
			}

			return null;
		}

		//do it by invoking method
		protected override bool OnCheck(){

			for (var i = 0; i < parameters.Count; i++){
				args[i] = parameters[i].value;
			}

			if (checkValue.varType == typeof(float))
				return OperationTools.Compare( (float)targetMethod.Invoke(agent, args), (float)checkValue.value, comparison, 0.05f );
			if (checkValue.varType == typeof(int))
				return OperationTools.Compare( (int)targetMethod.Invoke(agent, args), (int)checkValue.value, comparison );
			return object.Equals(targetMethod.Invoke(agent, args), checkValue.value);
		}


		void SetMethod(MethodInfo method){
			if (method != null){
				this.method = new SerializedMethodInfo(method);
				this.parameters.Clear();
				foreach(var p in method.GetParameters()){
					var newParam = new BBObjectParameter(p.ParameterType){bb = blackboard};
					if (p.IsOptional){
						newParam.value = p.DefaultValue;
					}
					parameters.Add(newParam);
				}

				this.checkValue = new BBObjectParameter(method.ReturnType){bb = blackboard};
				comparison = CompareMethod.EqualTo;			
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnTaskInspectorGUI(){

			if (!Application.isPlaying && GUILayout.Button("Select Method")){
				
				var menu = new UnityEditor.GenericMenu();
				if (agent != null){
					foreach(var comp in agent.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ){
						menu = EditorUtils.GetMethodSelectionMenu(comp.GetType(), typeof(object), typeof(object), SetMethod, 10, false, true, menu);
					}
					menu.AddSeparator("/");
				}

				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(Component))){
					menu = EditorUtils.GetMethodSelectionMenu(t, typeof(object), typeof(object), SetMethod, 10, false, true, menu);
				}
				menu.ShowAsContext();
				Event.current.Use();
			}

			if (targetMethod != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Type", agentType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Method", targetMethod.Name);
				GUILayout.EndVertical();

				var paramNames = targetMethod.GetParameters().Select(p => p.Name.SplitCamelCase() ).ToArray();
				for (var i = 0; i < paramNames.Length; i++){
					EditorUtils.BBParameterField(paramNames[i], parameters[i]);
				}

				GUI.enabled = checkValue.varType == typeof(float) || checkValue.varType == typeof(int);
				comparison = (CompareMethod)UnityEditor.EditorGUILayout.EnumPopup("Comparison", comparison);
				GUI.enabled = true;				
				EditorUtils.BBParameterField("Check Value", checkValue);
			}
		}

		#endif
	}
}