using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	/// <summary>
	/// Base class for BehaviourTree Composite nodes.
	/// </summary>
    abstract public class BTComposite : BTNode {

		sealed public override int maxOutConnections{ get {return -1;}}
		sealed public override Alignment2x2 commentsAlignment{ get{return Alignment2x2.Right;}}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu){
			menu = base.OnContextMenu(menu);
			menu = EditorUtils.GetTypeSelectionMenu(typeof(BTComposite), (t)=>{ ReplaceWith(t); }, menu, "Replace");
			menu.AddItem (new GUIContent ("Convert to SubTree"), false, ()=> { ConvertToSubTree(this); });
            if (outConnections.Count > 0){
				menu.AddItem(new GUIContent("Duplicate Branch"), false, ()=> { DuplicateBranch(this, graph); });
				menu.AddItem (new GUIContent ("Delete Branch"), false, ()=> { DeleteBranch(this); } );
			}
			return menu;
		}		

		//Replace the node with another composite
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

		///Create a new SubTree out of the branch of the provided root node
		static void ConvertToSubTree(BTNode root){

			if (!UnityEditor.EditorUtility.DisplayDialog("Convert to SubTree", "This will create a new SubTree out of this branch.\nThe SubTree can NOT be unpacked later on.\nAre you sure?", "Yes", "No!")){
				return;
			}

			var newBT = EditorUtils.CreateAsset<BehaviourTree>(true);
			if (newBT == null){
				return;
			}

			var subTreeNode = root.graph.AddNode<SubTree>(root.nodePosition);
			subTreeNode.subTree = newBT;

			for (var i = 0; i < root.inConnections.Count; i++){
				root.inConnections[i].SetTarget(subTreeNode);
			}

			root.inConnections.Clear();

			newBT.primeNode = DuplicateBranch(root, newBT);
			DeleteBranch(root);

			UnityEditor.AssetDatabase.SaveAssets();
		}

		///Delete the whole branch of provided root node along with the root node
		static void DeleteBranch(BTNode root){
			var graph = root.graph;
			foreach ( var node in root.GetAllChildNodesRecursively(true).ToArray() ){
				graph.RemoveNode(node);
			}
		}

		//Duplicate a node along with all children hierarchy
		static Node DuplicateBranch(BTNode root, Graph targetGraph){
			
			if (targetGraph == null){
				return null;
			}

			var newNode = root.Duplicate(targetGraph);
			var dupConnections = new List<Connection>();
			for (var i = 0; i < root.outConnections.Count; i++){
				dupConnections.Add( root.outConnections[i].Duplicate(newNode, DuplicateBranch( (BTNode)root.outConnections[i].targetNode, targetGraph) ));
			}
			newNode.outConnections.Clear();
			foreach (var c in dupConnections){
				newNode.outConnections.Add(c);
			}
			return newNode;
		}

		#endif
	}
}