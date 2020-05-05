using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Utility")]
	[Description("Check if an event is received and it's value is equal to specified value, then return true for one frame")]
	[EventReceiver("OnCustomEvent")]
	public class CheckEventValue<T> : ConditionTask<GraphOwner> {

		[RequiredField]
		public BBParameter<string> eventName;
		[Name("Compare Value To")]
		public BBParameter<T> value;

		protected override string info{ get {return string.Format("Event [{0}].value == {1}", eventName, value);} }
		protected override bool OnCheck(){ return false; }
		public void OnCustomEvent(EventData receivedEvent){
			if (receivedEvent is EventData<T> && isActive && receivedEvent.name.ToUpper() == eventName.value.ToUpper()){
				var receivedValue = ((EventData<T>)receivedEvent).value;
				if (receivedValue != null && receivedValue.Equals(value.value)){
					
					#if UNITY_EDITOR
					if (NodeCanvas.Editor.NCPrefs.logEvents){
						Debug.Log(string.Format("Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
					}
					#endif			
					
					YieldReturn(true);
				}
			}
		}		
	}
}