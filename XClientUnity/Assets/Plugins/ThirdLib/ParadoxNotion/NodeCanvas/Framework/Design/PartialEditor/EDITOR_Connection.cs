#if UNITY_EDITOR

using System.Collections;
using NodeCanvas.Editor;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Framework{

	partial class Connection{

		protected enum TipConnectionStyle
		{
			None,
			Circle,
			Arrow
		}

		protected enum BlinkPacketDirection
		{
			None,
			Forward,
			Backward
		}

		[SerializeField]
		private bool _infoCollapsed;
		
		const float RELINK_DISTANCE_SNAP      = 20f;
		const float STATUS_BLINK_DURATION     = 0.2f;
		const float STATUS_BLINK_SIZE_ADD     = 2f;
		const float STATUS_BLINK_SPEED        = 0.8f;
		const float STATUS_BLINKPACKET_SIZE   = 8f;
		const float STATUS_BLINK_PACKET_COUNT = 5f;

		private Rect centerInfoRect     = new Rect(0,0,50,10);
		private Status lastStatus       = Status.Resting;
		private Color connectionColor   = Node.statusColors[Status.Resting];
		private float lineSize          = 3;
		private bool isBlinking         = false;
		private Vector3 lineFromTangent = Vector3.zero;
		private Vector3 lineToTangent   = Vector3.zero;
		private bool isRelinking        = false;
		private Vector3 relinkClickPos;
		private Rect startPortRect;
		private Rect endPortRect;
		private float hor;
		
		private float blinkCompletion;		
		private float blinkTraversalTimer;

		private bool infoExpanded{
			get {return !_infoCollapsed;}
			set {_infoCollapsed = !value;}
		}


		virtual protected Color defaultColor{
			get {return Node.statusColors[Status.Resting];}
		}

		virtual protected float defaultSize{
			get {return 3f;}
		}

		virtual protected TipConnectionStyle tipConnectionStyle{
			get {return TipConnectionStyle.Circle;}
		}

		virtual protected BlinkPacketDirection blinkPacketDirection{
			get {return BlinkPacketDirection.Forward;}
		}

		virtual protected bool canRelink{
			get {return true;}
		}

		//Draw connection from-to
		public void DrawConnectionGUI(Vector3 lineFrom, Vector3 lineTo){
			
			var mlt = 0f;
			if (NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Curved){ mlt = 0.8f; }
			if (NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Stepped){ mlt = 1f; }
			var tangentX = Mathf.Abs(lineFrom.x - lineTo.x) * mlt;
			var tangentY = Mathf.Abs(lineFrom.y - lineTo.y) * mlt;

			GUI.color = connectionColor;

			startPortRect = new Rect(0,0,12,12);
			startPortRect.center = lineFrom;

			endPortRect = new Rect(0, 0, 16, 16);
			endPortRect.center = lineTo;

			hor = 0;

			if (lineFrom.x <= sourceNode.nodeRect.x){
				lineFromTangent = new Vector3(-tangentX, 0, 0);
				hor--;
			}

			if (lineFrom.x >= sourceNode.nodeRect.xMax){
				lineFromTangent = new Vector3(tangentX, 0, 0);
				hor++;
			}

			if (lineFrom.y <= sourceNode.nodeRect.y){
				lineFromTangent = new Vector3(0, -tangentY, 0);
			}

			if (lineFrom.y >= sourceNode.nodeRect.yMax){
				lineFromTangent = new Vector3(0, tangentY, 0);
			}


			if (lineTo.x <= targetNode.nodeRect.x){
				lineToTangent = new Vector3(-tangentX, 0, 0);
				hor--;
				if (tipConnectionStyle == TipConnectionStyle.Arrow){
					GUI.Box(endPortRect, string.Empty, (GUIStyle)"arrowRight");
				}
			}

			if (lineTo.x >= targetNode.nodeRect.xMax){
				lineToTangent = new Vector3(tangentX, 0, 0);
				hor++;
				if (tipConnectionStyle == TipConnectionStyle.Arrow){
					GUI.Box(endPortRect, string.Empty, (GUIStyle)"arrowLeft");
				}
			}

			if (lineTo.y <= targetNode.nodeRect.y){
				lineToTangent = new Vector3(0, -tangentY, 0);
				if (tipConnectionStyle == TipConnectionStyle.Arrow){
					GUI.Box(endPortRect, string.Empty, (GUIStyle)"arrowBottom");
				}
			}

			if (lineTo.y >= targetNode.nodeRect.yMax){
				lineToTangent = new Vector3(0, tangentY, 0);
				if (tipConnectionStyle == TipConnectionStyle.Arrow){
					GUI.Box(endPortRect, string.Empty, (GUIStyle)"arrowTop");
				}
			}

			if (tipConnectionStyle == TipConnectionStyle.Circle){
				GUI.Box(endPortRect, string.Empty, (GUIStyle)"circle");
			}

			hor = hor == 0? 0.5f : 1;

			GUI.color = Color.white;
			///


			///...
			if (Application.isPlaying && isActive){
				UpdateBlinkStatus(lineFrom, lineTo);
			}

			//...
			HandleEvents(lineFrom, lineTo);
			if (!isRelinking || Vector3.Distance(relinkClickPos, Event.current.mousePosition) < RELINK_DISTANCE_SNAP ){
				DrawConnection(lineFrom, lineTo);
				DrawInfoRect(lineFrom, lineTo);
			}
		}

		//The actual connection graphic
		void DrawConnection(Vector3 lineFrom, Vector3 lineTo){
			
			connectionColor = isActive? connectionColor : new Color(0.3f, 0.3f, 0.3f);
			if (!Application.isPlaying){
				connectionColor = isActive? defaultColor : new Color(0.3f, 0.3f, 0.3f);
				var highlight = Graph.currentSelection == this || Graph.currentSelection == sourceNode || Graph.currentSelection == targetNode;
				// if (Vector2.Distance(Event.current.mousePosition, lineFrom) < 10 || Vector2.Distance(Event.current.mousePosition, lineTo) < 10 ){
				// 	highlight = true;
				// }
				connectionColor.a = highlight? 1 : connectionColor.a;
				lineSize = highlight? defaultSize + 2 : defaultSize;
			}

			Handles.color = connectionColor;
			if (NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Curved){
				var shadow = new Vector3(3.5f,3.5f,0);
				Handles.DrawBezier(lineFrom, lineTo + shadow, lineFrom + shadow + lineFromTangent + shadow, lineTo + shadow + lineToTangent, new Color(0,0,0,0.1f), null, lineSize + 10f);
				Handles.DrawBezier(lineFrom, lineTo, lineFrom + lineFromTangent, lineTo + lineToTangent, connectionColor, null, lineSize);
			} else if (NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Stepped){
				var shadow = new Vector3(1,1,0);
				Handles.DrawPolyLine(lineFrom, lineFrom + lineFromTangent * hor, lineTo + lineToTangent * hor, lineTo);
				Handles.DrawPolyLine(lineFrom + shadow, (lineFrom + lineFromTangent * hor) + shadow, (lineTo + lineToTangent * hor) + shadow, lineTo + shadow);
			} else {
				Handles.DrawBezier(lineFrom, lineTo, lineFrom, lineTo, connectionColor, null, lineSize);
			}
			Handles.color = Color.white;
		}

		//Information showing in the middle
		void DrawInfoRect(Vector3 lineFrom, Vector3 lineTo){

			centerInfoRect.center = GetPosAlongConnectionCurve(lineFrom, lineTo, 0.5f);
			var isExpanded = infoExpanded || Graph.currentSelection == this || Graph.currentSelection == sourceNode;
			var alpha = isExpanded? 0.8f : 0.25f;
			var info = GetConnectionInfo();
			var extraInfo = sourceNode.GetConnectionInfo(sourceNode.outConnections.IndexOf(this) );
			if (!string.IsNullOrEmpty(info) || !string.IsNullOrEmpty(extraInfo)){
				
				if (!string.IsNullOrEmpty(extraInfo) && !string.IsNullOrEmpty(info)){
					extraInfo = "\n" + extraInfo;
				}

				var textToShow = string.Format("<size=9>{0}{1}</size>", info, extraInfo);
				if (!isExpanded){
					textToShow = "<size=9>...</size>";
				}
				var finalSize = GUI.skin.GetStyle("Box").CalcSize(new GUIContent(textToShow));

				centerInfoRect.width = finalSize.x;
				centerInfoRect.height = finalSize.y;

				GUI.color = new Color(1f,1f,1f,alpha);
				GUI.Box(centerInfoRect, textToShow);
				GUI.color = Color.white;

			} else {
			
				centerInfoRect.width = 0;
				centerInfoRect.height = 0;
			}
		}

		///Get position on curve from, to, by t
		public Vector2 GetPosAlongConnectionCurve(Vector3 from, Vector3 to, float t){
			float u = 1.0f - t;
		    float tt = t * t;
		    float uu = u * u;
		    float uuu = uu * u;
		    float ttt = tt * t;
		    Vector3 result = uuu * from;
		    result += 3 * uu * t * (from + lineFromTangent);
		    result += 3 * u * tt * (to + lineToTangent);
		    result += ttt * to;
		    return result;
		}

		///Is target position along from, to curve
		bool IsPositionAlongConnection(Vector2 lineFrom, Vector2 lineTo, Vector2 targetPosition){
			if ( ParadoxNotion.RectUtils.GetBoundRect(lineFrom, lineTo).Contains(targetPosition) ){
				var CLICK_CHECK_RES = 50f;
				var CLICK_CHECK_DISTANCE = 10f;
				for (var i = 0f; i <= CLICK_CHECK_RES; i++){
					var checkPos = GetPosAlongConnectionCurve(lineFrom, lineTo, i/CLICK_CHECK_RES );
					if ( Vector2.Distance( targetPosition, checkPos ) < CLICK_CHECK_DISTANCE ){
						return true;
					}
				}
			}
			return false;
		}


		//The connection's inspector
		public void ShowConnectionInspectorGUI(){

			UndoManager.CheckUndo(graph, "Connection Inspector");

			GUILayout.BeginHorizontal();
			GUI.color = new Color(1,1,1,0.5f);

			if (GUILayout.Button("◄", GUILayout.Height(14), GUILayout.Width(20))){
				Graph.currentSelection = sourceNode;
			}

			if (GUILayout.Button("►", GUILayout.Height(14), GUILayout.Width(20))){
				Graph.currentSelection = targetNode;
			}

			isActive = EditorGUILayout.ToggleLeft("ACTIVE", isActive, GUILayout.Width(150));

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("X", GUILayout.Height(14), GUILayout.Width(20))){
				Graph.PostGUI += delegate { graph.RemoveConnection(this); };
				return;
			}

			GUI.color = Color.white;
			GUILayout.EndHorizontal();

			EditorUtils.BoldSeparator();
			OnConnectionInspectorGUI();
			sourceNode.OnConnectionInspectorGUI(sourceNode.outConnections.IndexOf(this));

			UndoManager.CheckDirty(graph);
		}

		//The information to show in the middle area of the connection
		virtual protected string GetConnectionInfo(){ return null; }
		//Editor.Override to show controls in the editor panel when connection is selected
		virtual protected void OnConnectionInspectorGUI(){}


		///Handle UI events
		void HandleEvents(Vector2 lineFrom, Vector2 lineTo){

			var e = Event.current;

			//On click select this connection
			if ( Graph.allowClick && e.type == EventType.MouseDown && e.button == 0 ){
				if ( IsPositionAlongConnection(lineFrom, lineTo, e.mousePosition) || centerInfoRect.Contains(e.mousePosition) || startPortRect.Contains(e.mousePosition) || endPortRect.Contains(e.mousePosition) ){
					if (canRelink){
						isRelinking = true;
						relinkClickPos = e.mousePosition;
					}
					Graph.currentSelection = this;
					e.Use();
					return;
				}
			}

			if (canRelink && isRelinking){
				if (Vector3.Distance(relinkClickPos, Event.current.mousePosition) > RELINK_DISTANCE_SNAP){
					Handles.DrawBezier(startPortRect.center, e.mousePosition, startPortRect.center, e.mousePosition, defaultColor, null, defaultSize);
					if (e.rawType == EventType.MouseUp && e.button == 0){
						foreach(var node in graph.allNodes){
							if (node != targetNode && node != sourceNode && node.nodeRect.Contains(e.mousePosition) && node.IsNewConnectionAllowed() ){
								SetTarget(node);
								break;
							}
						}
						isRelinking = false;
						e.Use();
					}
				} else {
					if (e.rawType == EventType.MouseUp && e.button == 0){					
						isRelinking = false;
					}
				}
			}

			if (Graph.allowClick && e.type == EventType.MouseDown && e.button == 1 && centerInfoRect.Contains(e.mousePosition)){
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent(infoExpanded? "Collapse Info" : "Expand Info"), false, ()=> { infoExpanded = !infoExpanded; });
				menu.AddItem(new GUIContent(isActive? "Disable" : "Enable"), false, ()=> { isActive = !isActive; });
				
				var assignable = this as ITaskAssignable;
				if (assignable != null){
					
					if (assignable.task != null){
						menu.AddItem(new GUIContent("Copy Assigned Condition"), false, ()=> { Task.copiedTask = assignable.task; });
					} else {
						menu.AddDisabledItem(new GUIContent("Copy Assigned Condition"));
					}

					if (Task.copiedTask != null){
						menu.AddItem(new GUIContent(string.Format("Paste Assigned Condition ({0})", Task.copiedTask.name)), false, ()=>
						{
							if (assignable.task == Task.copiedTask){
								return;
							}

							if (assignable.task != null){
								if (!EditorUtility.DisplayDialog("Paste Condition", string.Format("Connection already has a Condition assigned '{0}'. Replace assigned condition with pasted condition '{1}'?", assignable.task.name, Task.copiedTask.name), "YES", "NO"))
									return;								
							}

							try {assignable.task = Task.copiedTask.Duplicate(graph);}
							catch {Debug.LogWarning("Can't paste Condition here. Incombatible Types");}
						});

					} else {
						menu.AddDisabledItem(new GUIContent("Paste Assigned Condition"));
					}

				}

				menu.AddSeparator("/");
				menu.AddItem(new GUIContent("Delete"), false, ()=> { graph.RemoveConnection(this); });

				Graph.PostGUI += ()=> { menu.ShowAsContext(); };
				e.Use();
			}
		}

		//Blink the connection color based on status.
		void UpdateBlinkStatus(Vector2 lineFrom, Vector2 lineTo){

			if (blinkPacketDirection != BlinkPacketDirection.None){
				if (status == Status.Running || isBlinking){
					for (var i = 0f; i < STATUS_BLINK_PACKET_COUNT; i++){
						var traverse = blinkTraversalTimer + (i/STATUS_BLINK_PACKET_COUNT);
						var norm = Mathf.Repeat(traverse, 1f);
						if (blinkPacketDirection == BlinkPacketDirection.Backward){
							norm = Mathf.Lerp(1, 0, norm);
						}
						norm = Mathf.Clamp01(norm);
						var invSize = Mathf.InverseLerp(0.2f, 0.8f, Mathf.PingPong(norm, 0.5f) );
						var pos = GetPosAlongConnectionCurve(lineFrom, lineTo, norm);
						var size = STATUS_BLINKPACKET_SIZE;
						size *= Mathf.Lerp(1, 1.5f, invSize);
						var color = connectionColor;
						color.a = Mathf.Lerp(0, 2, Mathf.PingPong(norm, 0.5f));
						if (status != Status.Running){
							var mlt = Mathf.Lerp(1, 0, blinkCompletion * 2f);
							size *= mlt;
						}
						color.a *= blinkCompletion;

						var rect = new Rect(0, 0, size, size);
						rect.center = pos;
						GUI.color = color;
						GUI.Box(rect, "", (GUIStyle)"circle");
						GUI.color = Color.white;
					}
				}
			}

			if (status == lastStatus || isBlinking){
				return;
			}
			
			lastStatus = status;
			if (status == Status.Resting){
				connectionColor = defaultColor;
				blinkTraversalTimer = 0;
				return;
			}

			if (status == Status.Running){
				blinkTraversalTimer = 0;
			}

			MonoManager.current.StartCoroutine(Internal_UpdateBlinkStatus());
		}

		//Simple tween to enhance the GUI line for debugging.
		IEnumerator Internal_UpdateBlinkStatus(){

			isBlinking = true;
			SetColorFromStatus();
			
			var timer = 0f;
			while(timer < STATUS_BLINK_DURATION){
				timer += Time.deltaTime;
				if (blinkPacketDirection != BlinkPacketDirection.None){
					blinkTraversalTimer += Time.deltaTime * STATUS_BLINK_SPEED;
				}
				blinkCompletion = timer/STATUS_BLINK_DURATION;
				lineSize = Mathf.Lerp(defaultSize + STATUS_BLINK_SIZE_ADD, defaultSize, blinkCompletion);
				yield return null;
			}

			if (blinkPacketDirection != BlinkPacketDirection.None){
				while(status == Status.Running && connectionColor == Node.statusColors[Status.Running]){
					if (graph.isRunning){
						blinkTraversalTimer += Time.deltaTime * STATUS_BLINK_SPEED;
					}
					yield return null;
				}
			}

			SetColorFromStatus();
			isBlinking = false;
		}


		//set the connection color from it's current status.
		void SetColorFromStatus(){
			if (status == Status.Resting){
				connectionColor = defaultColor;
			}

			connectionColor = Node.statusColors[status];
		}
	}
}

#endif