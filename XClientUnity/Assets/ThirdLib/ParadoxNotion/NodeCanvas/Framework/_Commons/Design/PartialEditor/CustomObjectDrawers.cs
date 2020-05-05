#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Reflection;

namespace ParadoxNotion.Design{

	///Derive this to create custom attributes to be drawn with an ObjectAttributeDrawer<T>.
	[AttributeUsage(AttributeTargets.Field)]
	abstract public class CustomDrawerAttribute : Attribute{}

	///Do not derive this. Derive from it's generic version only, where T is the type we care for.
	abstract public class CustomDrawer {
		abstract public object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, Attribute attribute, object context);
	}

	///Derive this to create custom drawers for T assignable object types.
	abstract public class ObjectDrawer<T> : CustomDrawer{

		///The instance of the object being drawn/serialized and for which this drawer is for
		public T instance{get; private set;}
		///The reflected FieldInfo representation
		public FieldInfo fieldInfo{get; private set;}
		///The object the instance is drawn/serialized within
		public object context{get; private set;}

		///Begin GUI
		sealed public override object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, Attribute attribute, object context){
			this.fieldInfo = fieldInfo;
			this.context = context;
			return OnGUI(content, (T)instance);
		}

		///Override to implement GUI. Return the modified instance at the end.
		abstract public T OnGUI(GUIContent content, T instance);
	}


	///Derive this to create custom drawers for T ObjectDrawerAttributes.
	abstract public class AttributeDrawer<T> : CustomDrawer where T:CustomDrawerAttribute{

		///The instance of the object being drawn/serialized
		public object instance{get; private set;}
		///The reflection FieldInfo representation
		public FieldInfo fieldInfo{get; private set;}
		///The attribute against this drawer is for.
		public T attribute{get; private set;}
		///The object the instance is drawn/serialized within
		public object context{get; private set;}

		///Begin GUI
		sealed public override object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, Attribute attribute, object context){
			this.fieldInfo = fieldInfo;
			this.context = context;
			this.attribute = (T)attribute;
			return OnGUI(content, instance);
		}

		///Override to implement GUI. Return the modified instance at the end.
		abstract public object OnGUI(GUIContent content, object instance);
	}


	///A stub
	sealed public class NoDrawer : CustomDrawer{
		public override object DrawGUI(GUIContent content, object instance, FieldInfo fieldInfo, Attribute attribute, object context){return instance;}
	}
}

#endif