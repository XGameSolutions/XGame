using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Category("Composites")]
	[Description("Executes ONE child based on the provided int or enum and return it's status. If set the Dynamic and 'case' change while a child is running, that child will be interrupted before the new child is executed.")]
	[Icon("IndexSwitcher")]
	[Color("b3ff7f")]
	public class Switch : BTComposite {

		public enum CaseSelectionMode
		{
			IndexBased,
			EnumBased
		}

		public enum OutOfRangeMode
		{
			ReturnFailure,
			LoopIndex
		}

		public bool dynamic;

		[BlackboardOnly]
		public BBObjectParameter enumCase = new BBObjectParameter(typeof(System.Enum));
		public BBParameter<int> intCase;
		public CaseSelectionMode selectionMode = CaseSelectionMode.IndexBased;
		public OutOfRangeMode outOfRangeMode = OutOfRangeMode.LoopIndex;

		private int current;
		private int runningIndex;

		public override string name{
			get{return base.name.ToUpper();}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (outConnections.Count == 0){
				return Status.Failure;
			}

			if (status == Status.Resting || dynamic){

				if (selectionMode == CaseSelectionMode.IndexBased){
					current = intCase.value;
					if (outOfRangeMode == OutOfRangeMode.LoopIndex){
						current = Mathf.Abs(current) % outConnections.Count;
					}

				} else {
					current = (int)enumCase.value;
				}

				if (runningIndex != current){
					outConnections[runningIndex].Reset();
				}

				if (current < 0 || current >= outConnections.Count){
					return Status.Failure;
				}
			}

			status = outConnections[current].Execute(agent, blackboard);

			if (status == Status.Running){
				runningIndex = current;
			}

			return status;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		public override string GetConnectionInfo(int i){
			if (selectionMode == CaseSelectionMode.EnumBased){
				if (enumCase.value == null){
					return "*Null Enum*";
				}
				var enumNames = System.Enum.GetNames(enumCase.value.GetType());
				if (i >= enumNames.Length){
					return "*Never*";
				}
				return enumNames[i];
			}
			return i.ToString();
		}
		
		protected override void OnNodeGUI(){
			if (dynamic){
				GUILayout.Label("<b>DYNAMIC</b>");
			}
			GUILayout.Label( selectionMode == CaseSelectionMode.IndexBased? ("Current = " + intCase.ToString()) : enumCase.ToString() );
		}

		protected override void OnNodeInspectorGUI(){
			dynamic = UnityEditor.EditorGUILayout.Toggle("Dynamic", dynamic);
			selectionMode = (CaseSelectionMode)UnityEditor.EditorGUILayout.EnumPopup("Selection Mode", selectionMode);
			if (selectionMode == CaseSelectionMode.IndexBased)
			{
				intCase = (BBParameter<int>)EditorUtils.BBParameterField("Index", intCase);
				outOfRangeMode = (OutOfRangeMode)UnityEditor.EditorGUILayout.EnumPopup("When Out Of Range", outOfRangeMode);
			}
			else
			{
				enumCase = (BBObjectParameter)EditorUtils.BBParameterField("Enum", enumCase, true);
				if (enumCase.value != null){
					GUILayout.BeginVertical("box");
					foreach (var s in System.Enum.GetNames(enumCase.value.GetType()) ){
						GUILayout.Label(s);
					}
					GUILayout.EndVertical();
				}
			}
		}
		
		#endif
	}
}