using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Plays a 'Beep' in editor only")]
	public class DebugBeep : ActionTask {

		protected override void OnExecute(){
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.Beep();
			#endif
			EndAction();
		}
	}
}