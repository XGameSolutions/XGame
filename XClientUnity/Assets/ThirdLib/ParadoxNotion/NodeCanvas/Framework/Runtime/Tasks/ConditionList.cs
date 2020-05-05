#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.Framework{

	/// ConditionList is a ConditionTask itself that holds many ConditionTasks. It can be set to either require all true or any true.
	[DoNotList]
    public class ConditionList : ConditionTask{

		public enum ConditionsCheckMode
		{
			AllTrueRequired,
			AnyTrueSuffice
		}

		public ConditionsCheckMode checkMode;
		public List<ConditionTask> conditions = new List<ConditionTask>();

		private List<ConditionTask> initialActiveConditions;

		private bool allTrueRequired{
			get {return checkMode == ConditionsCheckMode.AllTrueRequired;}
		}


		protected override string info{
			get
			{
				if (conditions.Count == 0){
					return "No Conditions";
				}
				
				var finalText = "<b>(" + (allTrueRequired? "ALL True" : "ANY True") + ")</b>\n";
				for (var i = 0; i < conditions.Count; i++){

					if (conditions[i] == null){
						continue;
					}

					if (conditions[i].isActive || (initialActiveConditions != null && initialActiveConditions.Contains(conditions[i])) ){
						finalText += conditions[i].summaryInfo + (i == conditions.Count -1? "" : "\n" );
					}
				}
				return finalText;
			}
		}

		///ConditionList overrides to duplicate listed conditions correctly
		public override Task Duplicate(ITaskSystem newOwnerSystem){
			var newList = (ConditionList)base.Duplicate(newOwnerSystem);
			newList.conditions.Clear();
			foreach (var condition in conditions){
				newList.AddCondition( (ConditionTask)condition.Duplicate(newOwnerSystem) );
			}

			return newList;
		}

		//Forward Enable call
		protected override void OnEnable(){
			
			if (initialActiveConditions == null){ //cache the initially active conditions within the list
				initialActiveConditions = conditions.Where(c => c.isActive).ToList();
			}

			for (var i = 0; i < initialActiveConditions.Count; i++){
				initialActiveConditions[i].Enable(agent, blackboard);
			}
		}

		//Forward Disable call
		protected override void OnDisable(){
			for (var i = 0; i < initialActiveConditions.Count; i++){
				initialActiveConditions[i].Disable();
			}
		}

		protected override bool OnCheck(){
			var succeedChecks = 0;
			for (var i = 0; i < conditions.Count; i++){

				if (!conditions[i].isActive){
					succeedChecks ++;
					continue;
				}

				if (conditions[i].CheckCondition(agent, blackboard)){
					if (!allTrueRequired){
						return true;
					}
					succeedChecks ++;

				} else {

					if (allTrueRequired){
						return false;
					}
				}
			}

			return succeedChecks == conditions.Count;
		}

		public override void OnDrawGizmos(){
			foreach (var condition in conditions){
				condition.OnDrawGizmos();
			}
		}

		public override void OnDrawGizmosSelected(){
			foreach (var condition in conditions){
				condition.OnDrawGizmosSelected();
			}
		}

		public void AddCondition(ConditionTask condition){

			if (condition is ConditionList){
				Debug.LogWarning("Adding a ConditionList within another ConditionList is not allowed for clarity");
				return;
			}

			#if UNITY_EDITOR
			if (!Application.isPlaying){
				Undo.RecordObject(ownerSystem.contextObject, "List Add Task");
				currentViewCondition = condition;
			}
			#endif
			
			conditions.Add(condition);
			condition.SetOwnerSystem(this.ownerSystem);
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		private ConditionTask currentViewCondition;

		protected override void OnTaskInspectorGUI(){
			ShowListGUI();
			ShowNestedConditionsGUI();
		}

		void ValidateList(){
			for (var i = 0; i < conditions.Count; i++){
				if (conditions[i] == null){
					conditions.RemoveAt(i);
				}
			}
		}

		public void ShowListGUI(){

			EditorUtils.TaskSelectionButton<ConditionTask>(ownerSystem, (c)=>{ AddCondition(c) ;});

			ValidateList();

			if (conditions.Count == 0){
				EditorGUILayout.HelpBox("No Conditions", MessageType.None);
				return;
			}

			if (conditions.Count == 1){
				return;
			}
			
			EditorUtils.ReorderableList(conditions, (i, picked)=>
			{
				var condition = conditions[i];
				GUI.color = new Color(1, 1, 1, 0.25f);
				GUILayout.BeginHorizontal("box");

				GUI.color = condition.isActive? new Color(1,1,1,0.8f) : new Color(1,1,1,0.25f);

				condition.isActive = EditorGUILayout.Toggle(condition.isActive, GUILayout.Width(18));

				GUI.backgroundColor = condition == currentViewCondition? Color.grey : Color.white;
				if (GUILayout.Button(EditorUtils.viewIcon, GUILayout.Width(25), GUILayout.Height(18))){
					currentViewCondition = condition == currentViewCondition? null : condition;
				}
				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				GUI.backgroundColor = Color.white;
				GUILayout.Label(condition.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
				
				if (!Application.isPlaying && GUILayout.Button("X", GUILayout.MaxWidth(20))){
					Undo.RecordObject(ownerSystem.contextObject, "List Remove Task");
					conditions.RemoveAt(i);
				}

				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				GUILayout.EndHorizontal();
				GUI.color = Color.white;
			});

			checkMode = (ConditionsCheckMode)EditorGUILayout.EnumPopup(checkMode);
		}


		public void ShowNestedConditionsGUI(){

			if (conditions.Count == 1){
				currentViewCondition = conditions[0];
			}

			if (currentViewCondition != null){
				EditorUtils.Separator();
				Task.ShowTaskInspectorGUI(currentViewCondition, (c)=>
				{
					if (c == null){
						var i = conditions.IndexOf(currentViewCondition);
						conditions.RemoveAt(i);
					}
					currentViewCondition = (ConditionTask)c;
				});
			}
		}

		public void DoSavePreset(){
			#if !UNITY_WEBPLAYER
			var path = EditorUtility.SaveFilePanelInProject ("Save Preset", "", "conditionList", "");
            if (!string.IsNullOrEmpty(path)){
                System.IO.File.WriteAllText( path, JSONSerializer.Serialize(typeof(ConditionList), this, true) ); //true for pretyJson
                AssetDatabase.Refresh();
            }
            #else
            Debug.LogWarning("Preset saving is not possible with WebPlayer as active platform");
            #endif
		}

		public void DoLoadPreset(){
			#if !UNITY_WEBPLAYER
            var path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "conditionList");
            if (!string.IsNullOrEmpty(path)){
                var json = System.IO.File.ReadAllText(path);
                var list = JSONSerializer.Deserialize<ConditionList>(json);
                this.conditions = list.conditions;
                this.checkMode = list.checkMode;
                this.currentViewCondition = null;
                foreach(var a in conditions){
                	a.SetOwnerSystem(this.ownerSystem);
                }
            }				
            #else
            Debug.LogWarning("Preset loading is not possible with WebPlayer as active platform");
            #endif
		}


		#endif
	}
}