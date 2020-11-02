﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines{

	public interface IState{

		///The name of the state
		string name{get;}
		
		///The tag of the state
		string tag{get;}
		
		///The elapsed time of the state
		float elapsedTime{get;}
		
		///The FSM this state belongs to
		FSM FSM{get;}

		///An array of the state's transition connections
		FSMConnection[] GetTransitions();

		///Evaluates the state's transitions and returns true if a transition has been performed
		bool CheckTransitions();

		///Marks the state as Finished
		void Finish(bool success);
	}

	/// <summary>
	/// Super base class for FSM state nodes that live within an FSM Graph.
	/// </summary>
	abstract public class FSMState : Node, IState{

		public enum TransitionEvaluationMode
		{
			CheckContinuously,
			CheckAfterStateFinished,
			CheckManually
		}
		
		[SerializeField]
		private TransitionEvaluationMode _transitionEvaluation;

		private float _elapsedTime;
		private bool hasInit;

		public override int maxInConnections{ get{return -1;} }
		public override int maxOutConnections{ get{return -1;} }
		sealed public override System.Type outConnectionType{ get{return typeof(FSMConnection);} }
		public override bool allowAsPrime {get {return true;}}
		sealed public override Alignment2x2 commentsAlignment{ get{return Alignment2x2.Bottom;}}
		sealed public override Alignment2x2 iconAlignment{ get	{return Alignment2x2.Default;}}		

		public TransitionEvaluationMode transitionEvaluation{
			get {return _transitionEvaluation;}
			set {_transitionEvaluation = value;}
		}

		public float elapsedTime{
			get {return _elapsedTime;}
			private set {_elapsedTime = value;}
		}

		///The FSM this state belongs to
		public FSM FSM{
			get{return (FSM)graph;}
		}

		///Returns all transitions of the state
		public FSMConnection[] GetTransitions(){
			return outConnections.Cast<FSMConnection>().ToArray();
		}

		///Declares that the state has finished
		public void Finish(){ Finish(true); }
		public void Finish(bool inSuccess){
			status = inSuccess? Status.Success : Status.Failure;
		}

		sealed public override void OnGraphStarted(){}
		sealed public override void OnGraphStoped(){
			status = Status.Resting;
		}

		sealed public override void OnGraphPaused(){
			if (status == Status.Running){
				OnPause();
			}
		}

		//OnEnter...
		sealed protected override Status OnExecute(Component agent, IBlackboard bb){

			if (!hasInit){
				hasInit = true;
				OnInit();
			}

			if (status == Status.Resting || status == Status.Running){
				status = Status.Running;

				for (int i = 0; i < outConnections.Count; i++){
					if ( ((FSMConnection)outConnections[i]).condition != null){
						((FSMConnection)outConnections[i]).condition.Enable(agent, bb);
					}
				}

				OnEnter();
			}

			return status;
		}

		//OnUpdate...
		public void Update(){

			elapsedTime += Time.deltaTime;

			if (transitionEvaluation == TransitionEvaluationMode.CheckContinuously){
				CheckTransitions();
			} else if (transitionEvaluation == TransitionEvaluationMode.CheckAfterStateFinished && status != Status.Running){
				CheckTransitions();
			}

			if (status == Status.Running){
				OnUpdate();
			}
		}

		///Returns true if a transitions was valid and thus made
		public bool CheckTransitions(){

			for (var i = 0; i < outConnections.Count; i++){
				
				var connection = (FSMConnection)outConnections[i];
				var condition = connection.condition;
				
				if (!connection.isActive){
					continue;
				}

				if ( (condition != null && condition.CheckCondition(graphAgent, graphBlackboard) ) || (condition == null && status != Status.Running) ){
					FSM.EnterState( (FSMState)connection.targetNode );
					connection.status = Status.Success; //editor vis
					return true;
				}

				connection.status = Status.Failure; //editor vis
			}

			return false;
		}

		//OnExit...
		sealed protected override void OnReset(){
			status = Status.Resting;
			elapsedTime = 0;

			for (int i = 0; i < outConnections.Count; i++){
				if ( ((FSMConnection)outConnections[i]).condition != null)
					((FSMConnection)outConnections[i]).condition.Disable();
			}

			OnExit();
		}


		//Converted
		virtual protected void OnInit(){}
		virtual protected void OnEnter(){}
		virtual protected void OnUpdate(){}
		virtual protected void OnExit(){}
		virtual protected void OnPause(){}
		//




		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		private static GUIPort clickedPort{get;set;}
		private static int dragDropMisses;

		class GUIPort{

			public readonly FSMState parent;
			public readonly Vector2 pos;

			public GUIPort(FSMState parent, Vector2 pos){
				this.parent = parent;
				this.pos = pos;
			}
		}

		sealed protected override void DrawNodeConnections(Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){

			var e = Event.current;

			//Receive connections first
			if (clickedPort != null && e.type == EventType.MouseUp && e.button == 0){

				if (nodeRect.Contains(e.mousePosition)){
					foreach(FSMConnection connection in inConnections){
						if (connection.sourceNode == clickedPort.parent){
							Debug.LogWarning("State is already connected to target state. Consider using a 'ConditionList' on the existing transition if you want to check multiple conditions");
							clickedPort = null;
							e.Use();
							return;
						}
					}

					graph.ConnectNodes(clickedPort.parent, this);
					clickedPort = null;
					e.Use();
				
				} else {

					dragDropMisses ++;
					
					if (dragDropMisses == graph.allNodes.Count && clickedPort != null){
						var source = clickedPort.parent;
						var pos = Event.current.mousePosition;
						var menu = new GenericMenu();
						clickedPort = null;

						menu.AddItem(new GUIContent("Add Action State"), false, ()=> {
							var newState = graph.AddNode<ActionState>(pos);
							graph.ConnectNodes(source, newState);
						});

						//PostGUI cause of zoom factors
						Graph.PostGUI += ()=> { menu.ShowAsContext(); };
						Event.current.Use();
						e.Use();
					}
				}
			}



			var portRectLeft = new Rect(0,0,20,20);
			var portRectRight = new Rect(0,0,20,20);
			var portRectBottom = new Rect(0,0,20,20);

			portRectLeft.center = new Vector2( nodeRect.x - 11, nodeRect.center.y );
			portRectRight.center = new Vector2( nodeRect.xMax + 11, nodeRect.center.y );
			portRectBottom.center = new Vector2( nodeRect.center.x, nodeRect.yMax + 11 );

			if (maxOutConnections != 0){
				if (fullDrawPass || drawCanvas.Overlaps(nodeRect)){
					EditorGUIUtility.AddCursorRect(portRectLeft, MouseCursor.ArrowPlus);
					EditorGUIUtility.AddCursorRect(portRectRight, MouseCursor.ArrowPlus);
					EditorGUIUtility.AddCursorRect(portRectBottom, MouseCursor.ArrowPlus);

					GUI.color = new Color(1,1,1,0.3f);
					GUI.Box(portRectLeft, string.Empty, (GUIStyle)"arrowLeft");
					GUI.Box(portRectRight, string.Empty, (GUIStyle)"arrowRight");
					if (maxInConnections == 0){
						GUI.Box(portRectBottom, string.Empty, (GUIStyle)"arrowBottom");
					}
					GUI.color = Color.white;

					if (Graph.allowClick && e.type == EventType.MouseDown && e.button == 0){
						
						if (portRectLeft.Contains(e.mousePosition)){
							clickedPort = new GUIPort(this, portRectLeft.center);
							dragDropMisses = 0;
							e.Use();
						}
						
						if (portRectRight.Contains(e.mousePosition)){
							clickedPort = new GUIPort(this, portRectRight.center);
							dragDropMisses = 0;
							e.Use();
						}

						if (maxInConnections == 0 && portRectBottom.Contains(e.mousePosition)){
							clickedPort = new GUIPort(this, portRectBottom.center);
							dragDropMisses = 0;
							e.Use();
						}
					}
				}
			}

			if (clickedPort != null && clickedPort.parent == this){
				Handles.DrawBezier(clickedPort.pos, e.mousePosition, clickedPort.pos, e.mousePosition, new Color(0.5f,0.5f,0.8f,0.8f), null, 2);
			}

			for (var i = 0; i < outConnections.Count; i++){

				var connection = outConnections[i] as FSMConnection;
				var targetState = connection.targetNode as FSMState;
				if (targetState == null){ //In case of MissingNode type
					continue;
				}
				var targetPos = targetState.GetConnectedInPortPosition(connection);
				var sourcePos = Vector2.zero;

				if (nodeRect.center.x <= targetPos.x){
					sourcePos = portRectRight.center;
				}
				
				if (nodeRect.center.x > targetPos.x){
					sourcePos = portRectLeft.center;
				}

				if (maxInConnections == 0 && nodeRect.center.y < targetPos.y - 50 && Mathf.Abs(nodeRect.center.x - targetPos.x) < 200 ){
					sourcePos = portRectBottom.center;
				}

				var boundRect = RectUtils.GetBoundRect(sourcePos, targetPos);
				if (fullDrawPass || drawCanvas.Overlaps(boundRect)){
					connection.DrawConnectionGUI(sourcePos, targetPos);
				}
			}
		}


		Vector2 GetConnectedInPortPosition(Connection connection){

			var sourcePos = connection.sourceNode.nodeRect.center;
			var thisPos = nodeRect.center;

			var style = 0;

			if (style == 0){
				if (sourcePos.x <= thisPos.x){
					if (sourcePos.y <= thisPos.y){
						return new Vector2(nodeRect.center.x - 15, nodeRect.yMin - (this == graph.primeNode? 20 : 0 ) );
					} else {
						return new Vector2(nodeRect.center.x - 15, nodeRect.yMax + 2);
					}
				}

				if (sourcePos.x > thisPos.x){
					if (sourcePos.y <= thisPos.y){
						return new Vector2(nodeRect.center.x + 15, nodeRect.yMin - (this == graph.primeNode? 20 : 0 ));
					} else {
						return new Vector2(nodeRect.center.x + 15, nodeRect.yMax + 2);
					}
				}
			}

			if (style == 1){
				//Another idea
				if (sourcePos.x <= thisPos.x){
					if (sourcePos.y >= thisPos.y){
						return new Vector2(nodeRect.xMin - 3, nodeRect.yMax - 10);
					} else {
						return new Vector2(nodeRect.xMin - 3, nodeRect.yMin + 10);
					}
				}
				if (sourcePos.x > thisPos.x){
					if (sourcePos.y >= thisPos.y){
						return new Vector2(nodeRect.center.x, nodeRect.yMax + 2);
					} else {
						return new Vector2(nodeRect.center.x, nodeRect.yMin - (this == graph.primeNode? 20 : 0 ));
					}
				}
			}

			if (style >= 2){
				//YET Another idea
				if (sourcePos.x <= thisPos.x){
					if (sourcePos.y >= thisPos.y){
						return new Vector2(nodeRect.xMin - 3, nodeRect.yMax - 10);
					} else {
						return new Vector2(nodeRect.xMin - 3, nodeRect.yMin + 10);
					}
				}
				if (sourcePos.x > thisPos.x){
					if (sourcePos.y >= thisPos.y){
						return new Vector2(nodeRect.xMax + 3, nodeRect.yMax - 10);
					} else {
						return new Vector2(nodeRect.xMax + 3, nodeRect.yMin + 10);
					}
				}
			}

			return thisPos;
		}
		
		// protected override void OnNodeGUI(){ }
		protected override void OnNodeInspectorGUI(){
			ShowBaseFSMInspectorGUI();
			DrawDefaultInspector();
		}

		protected override GenericMenu OnContextMenu(GenericMenu menu){
			
			if (allowAsPrime){
				if (Application.isPlaying){
					menu.AddItem (new GUIContent ("Enter State"), false, ()=>{ FSM.EnterState(this); });
				} else {
					menu.AddDisabledItem(new GUIContent ("Enter State"));
				}
				menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, ()=>{ isBreakpoint = !isBreakpoint; });
			}
			return menu;
		}

		protected void ShowBaseFSMInspectorGUI(){

			EditorUtils.CoolLabel("Transitions");

			if (outConnections.Count == 0){
				GUI.backgroundColor = new Color(1,1,1,0.5f);
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("No Transitions");
				GUILayout.EndHorizontal();
				GUI.backgroundColor = Color.white;
			}

			var onFinishExists = false;
			EditorUtils.ReorderableList(outConnections, (i, picked)=>
			{
				var connection = (FSMConnection)outConnections[i];
				GUI.backgroundColor = new Color(1,1,1,0.5f);
				GUILayout.BeginHorizontal("box");
				if (connection.condition != null){
					GUILayout.Label(connection.condition.summaryInfo, GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
				} else {
					GUILayout.Label("OnFinish" + (onFinishExists? " (exists)" : string.Empty ), GUILayout.MinWidth(0), GUILayout.ExpandWidth(true));
					onFinishExists = true;
				}

				GUILayout.FlexibleSpace();
				GUILayout.Label("--> '" + connection.targetNode.name + "'");
				if (GUILayout.Button("►")){
					Graph.currentSelection = connection;
				}

				GUILayout.EndHorizontal();
				GUI.backgroundColor = Color.white;
			});

			if ( allowAsPrime ){
				transitionEvaluation = (TransitionEvaluationMode)EditorGUILayout.EnumPopup(transitionEvaluation);
			}

			EditorUtils.BoldSeparator();
		}

		#endif
	}
}