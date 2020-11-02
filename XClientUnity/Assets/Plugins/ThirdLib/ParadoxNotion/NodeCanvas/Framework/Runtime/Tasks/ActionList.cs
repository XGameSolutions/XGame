#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion.Serialization;

namespace NodeCanvas.Framework{

	///ActionList is an ActionTask itself that holds multilple ActionTasks which can be executed either in parallel or in sequence.
	[DoNotList]
	public class ActionList : ActionTask{

		public enum ActionsExecutionMode
		{
			ActionsRunInSequence,
			ActionsRunInParallel
		}
		
		public ActionsExecutionMode executionMode;
		public List<ActionTask> actions = new List<ActionTask>();

		private int currentActionIndex;
		private readonly List<int> finishedIndeces = new List<int>();

		protected override string info{
			get
			{
				if (actions.Count == 0){
					return "No Actions";
				}

				var finalText= string.Empty;
				for (var i= 0; i < actions.Count; i++){

					var action = actions[i];
					if (action == null){
						continue;
					}

					if (action.isActive){
						var prefix = action.isPaused? "<b>||</b> " : action.isRunning? "► " : "";
						finalText += prefix + action.summaryInfo + (i == actions.Count -1? "" : "\n");
					}
				}

				return finalText;	
			}
		}


		///ActionList overrides to duplicate listed actions correctly
		public override Task Duplicate(ITaskSystem newOwnerSystem){
			var newList = (ActionList)base.Duplicate(newOwnerSystem);
			newList.actions.Clear();
			foreach (var action in actions){
				newList.AddAction( (ActionTask)action.Duplicate(newOwnerSystem) );
			}

			return newList;
		}


		protected override void OnExecute(){
			finishedIndeces.Clear();
			currentActionIndex = 0;
		}

		protected override void OnUpdate(){

			if (actions.Count == 0){
				EndAction();
				return;
			}

			if (executionMode == ActionsExecutionMode.ActionsRunInParallel){

				for (var i = 0; i < actions.Count; i++){
						
					if (finishedIndeces.Contains(i)){
						continue;
					}

					if (!actions[i].isActive){
						finishedIndeces.Add(i);
						continue;
					}

					var status = actions[i].ExecuteAction(agent, blackboard);
					if (status == Status.Failure){
						EndAction(false);
						return;
					}

					if (status == Status.Success){
						finishedIndeces.Add(i);
					}
				}

				if (finishedIndeces.Count == actions.Count){
					EndAction(true);
				}

			} else {

				for (var i = currentActionIndex; i < actions.Count; i++){
				    
				    if ( !actions[i].isActive ){
				    	continue;
				    }

				    var status = actions[i].ExecuteAction(agent, blackboard);
				    if (status == Status.Failure){
				        EndAction(false);
				        return;
				    }

				    if (status == Status.Running){
				        currentActionIndex = i;
				        return;
				    }
				}

				EndAction(true);
			}
		}

		protected override void OnStop(){
			for (var i = 0; i < actions.Count; i++){
				actions[i].EndAction(null);
			}
		}

		protected override void OnPause(){
			for (var i = 0; i < actions.Count; i++){
				actions[i].PauseAction();			
			}
		}

		public override void OnDrawGizmos(){
			for (var i = 0; i < actions.Count; i++){
				actions[i].OnDrawGizmos();			
			}
		}

		public override void OnDrawGizmosSelected(){
			for (var i = 0; i < actions.Count; i++){
				actions[i].OnDrawGizmosSelected();
			}
		}

		public void AddAction(ActionTask action){

			if (action is ActionList){
				Debug.LogWarning("Adding an ActionList within another ActionList is not allowed for clarity");
				return;
			}

			#if UNITY_EDITOR
			if (!Application.isPlaying){
				Undo.RecordObject(ownerSystem.contextObject, "List Add Task");
				currentViewAction = action;
			}
			#endif
			
			actions.Add(action);
			action.SetOwnerSystem(this.ownerSystem);
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected ActionTask currentViewAction;


		protected override void OnTaskInspectorGUI(){
			ShowListGUI();
			ShowNestedActionsGUI();
		}

		void ValidateList(){
			for (var i = 0; i < actions.Count; i++){
				if (actions[i] == null){
					actions.RemoveAt(i);
				}
			}
		}

		//The action list gui
		public void ShowListGUI(){

			if (ownerSystem == null){
				GUILayout.Label("Owner System is null!");
				return;
			}

			EditorUtils.TaskSelectionButton<ActionTask>(ownerSystem, (a)=>{ AddAction(a); });

			ValidateList();

			if (actions.Count == 0){
				EditorGUILayout.HelpBox("No Actions", MessageType.None);
				return;
			}

			if (actions.Count == 1){
				return;
			}

			//show the actions
			EditorUtils.ReorderableList(actions, (i, picked)=>
			{
				var action = actions[i];
				GUI.color = new Color(1, 1, 1, 0.25f);
				EditorGUILayout.BeginHorizontal("box");

				GUI.color = action.isActive? new Color(1,1,1,0.8f) : new Color(1,1,1,0.25f);
				action.isActive = EditorGUILayout.Toggle(action.isActive, GUILayout.Width(18));

				GUI.backgroundColor = action == currentViewAction? Color.grey : Color.white;
				if (GUILayout.Button(EditorUtils.viewIcon, GUILayout.Width(25), GUILayout.Height(18))){
					currentViewAction = action == currentViewAction? null : action;
				}
				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				GUI.backgroundColor = Color.white;

				GUILayout.Label( (action.isPaused? "<b>||</b> " : action.isRunning? "► " : "") + action.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));

				if (!Application.isPlaying && GUILayout.Button("X", GUILayout.Width(20))){
					Undo.RecordObject(ownerSystem.contextObject, "List Remove Task");
					actions.RemoveAt(i);
				}

				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
			});

			executionMode = (ActionsExecutionMode)EditorGUILayout.EnumPopup(executionMode);
		}


		public void ShowNestedActionsGUI(){

			if (actions.Count == 1){
				currentViewAction = actions[0];
			}

			if (currentViewAction != null){
				EditorUtils.Separator();
				Task.ShowTaskInspectorGUI(currentViewAction, (a)=>
				{
					if (a == null){
						var i = actions.IndexOf(currentViewAction);
						actions.RemoveAt(i);
					}
					currentViewAction = (ActionTask)a;
				});
			}
		}



		public void DoSavePreset(){
			#if !UNITY_WEBPLAYER
			var path = EditorUtility.SaveFilePanelInProject ("Save Preset", "", "actionList", "");
            if (!string.IsNullOrEmpty(path)){
                System.IO.File.WriteAllText( path, JSONSerializer.Serialize(typeof(ActionList), this, true) ); //true for pretyJson
                AssetDatabase.Refresh();
            }				
            #else
            Debug.LogWarning("Preset saving is not possible with WebPlayer as active platform");
            #endif
		}

		public void DoLoadPreset(){
			#if !UNITY_WEBPLAYER
            var path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "actionList");
            if (!string.IsNullOrEmpty(path)){
                var json = System.IO.File.ReadAllText(path);
                var list = JSONSerializer.Deserialize<ActionList>(json);
                this.actions = list.actions;
                this.executionMode = list.executionMode;
                this.currentViewAction = null;
                foreach(var a in actions){
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