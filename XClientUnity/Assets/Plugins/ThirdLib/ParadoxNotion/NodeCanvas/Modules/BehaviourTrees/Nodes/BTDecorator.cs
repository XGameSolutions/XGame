using ParadoxNotion;
using NodeCanvas.Framework;


namespace NodeCanvas.BehaviourTrees{

	/// <summary>
	/// Base class for BehaviourTree Decorator nodes.
	/// </summary>
	abstract public class BTDecorator : BTNode{

		sealed public override int maxOutConnections{ get{return 1;}}
		sealed public override Alignment2x2 commentsAlignment{ get{return Alignment2x2.Right;}}

		///The decorated connection object
		protected Connection decoratedConnection{
			get
			{
				try { return outConnections[0]; }
				catch {return null;}
			}
		}

		///The decorated node object
		protected Node decoratedNode{
			get
			{
				try {return outConnections[0].targetNode;}
				catch {return null;}
			}
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu){
			menu = base.OnContextMenu(menu);
			menu = ParadoxNotion.Design.EditorUtils.GetTypeSelectionMenu(typeof(BTDecorator), (t)=>{ ReplaceWith(t); }, menu, "Replace");
			return menu;
		}		

		void ReplaceWith(System.Type t){
			var newNode = graph.AddNode(t, this.nodePosition);
			foreach(var c in inConnections.ToArray()){
				c.SetTarget(newNode);
			}
			foreach(var c in outConnections.ToArray()){
				c.SetSource(newNode);
			}
			if (graph.primeNode == this){
				graph.primeNode = newNode;
			}
			graph.RemoveNode(this);
		}		

		#endif

	}
}