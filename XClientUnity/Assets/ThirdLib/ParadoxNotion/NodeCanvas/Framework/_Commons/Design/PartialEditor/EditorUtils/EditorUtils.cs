#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ParadoxNotion;
using UnityObject = UnityEngine.Object;

namespace ParadoxNotion.Design{

	///Have some commonly stuff used across most inspectors and helper functions. Keep outside of Editor folder since many runtime classes use this in #if UNITY_EDITOR
	///This is a partial class. Different implementation provide different tools, so that everything is referenced from within one class.
	[InitializeOnLoad]
	public static partial class EditorUtils{

		readonly public static Texture2D playIcon     = EditorGUIUtility.FindTexture("d_PlayButton");
		readonly public static Texture2D pauseIcon    = EditorGUIUtility.FindTexture("d_PauseButton");
		readonly public static Texture2D stepIcon     = EditorGUIUtility.FindTexture("d_StepButton");
		readonly public static Texture2D viewIcon     = EditorGUIUtility.FindTexture("d_ViewToolOrbit On");
		readonly public static Texture2D csIcon       = EditorGUIUtility.FindTexture("cs Script Icon");
		readonly public static Texture2D jsIcon       = EditorGUIUtility.FindTexture("Js Script Icon");
		readonly public static Texture2D tagIcon      = EditorGUIUtility.FindTexture("d_FilterByLabel");
		readonly public static Texture2D searchIcon   = EditorGUIUtility.FindTexture("Search Icon");
		readonly public static Texture2D infoIcon     = EditorGUIUtility.FindTexture("d_console.infoIcon.sml");
		readonly public static Texture2D warningIcon  = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
		readonly public static Texture2D errorIcon    = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
		readonly public static Texture2D redCircle    = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
		readonly public static Texture2D folderIcon   = EditorGUIUtility.FindTexture("Folder Icon");
		readonly public static Texture2D favoriteIcon = EditorGUIUtility.FindTexture("Favorite Icon");

		readonly private static Color _lightOrange = new Color(1, 0.9f, 0.4f);
		readonly private static Color _lightBlue   = new Color(0.8f,0.8f,1);
		readonly private static Color _lightRed    = new Color(1,0.5f,0.5f, 0.8f);
		public static Color lightOrange{ get {return EditorGUIUtility.isProSkin? _lightOrange : Color.white;} }
		public static Color lightBlue{ get {return EditorGUIUtility.isProSkin? _lightBlue : Color.white;} }
		public static Color lightRed{ get {return EditorGUIUtility.isProSkin? _lightRed : Color.white;} }


		//For gathering script/type meta-information
		public class ScriptInfo{
			public Type originalType;
			public string originalName;
			public string originalCategory;
			
			public Type type;
			public string name;
			public string category;

			public int priority;

			public string description;
			public Texture icon;

			public ScriptInfo(){}
			public ScriptInfo(Type type, string name, string category, int priority = 0){
				this.type = type;
				this.name = name;
				this.category = category;
				this.priority = priority;
				if (type != null){
					var iconAtt = type.RTGetAttribute<IconAttribute>(true);
					icon = iconAtt != null? UserTypePrefs.GetTypeIcon(iconAtt) : null;
					var descAtt = type.RTGetAttribute<DescriptionAttribute>(true);
					description = descAtt != null? descAtt.description : description;
				}
			}
		}

		[InitializeOnLoadMethod]
		static void Init(){
			UserTypePrefs.onPreferredTypesChanged += ()=>{cachedInfos = null; };
		}

		///Get a list of ScriptInfos of the baseType excluding: the base type, abstract classes, Obsolete classes and those with the DoNotList attribute, categorized as a list of ScriptInfo
		private static Dictionary<Type, List<ScriptInfo>> cachedInfos;
		public static List<ScriptInfo> GetScriptInfosOfType(Type baseType){

			if (cachedInfos == null){ cachedInfos = new Dictionary<Type, List<ScriptInfo>>(); }

			List<ScriptInfo> infosResult;
			if (cachedInfos.TryGetValue(baseType, out infosResult)){
				return infosResult.ToList();
			}

			infosResult = new List<ScriptInfo>();

			var subTypes = GetAssemblyTypes(baseType);
			if (baseType.IsGenericTypeDefinition){
				subTypes = new List<Type>{ baseType };
			}

			foreach (var subType in subTypes){

				if ( subType.IsDefined(typeof(DoNotListAttribute), true) || subType.IsDefined(typeof(ObsoleteAttribute), true) ){
					continue;
				}
				
				if (subType.IsAbstract){
					continue;
				}

				var isGeneric = subType.IsGenericTypeDefinition && subType.GetGenericArguments().Length == 1;
				var scriptName = subType.FriendlyName().SplitCamelCase();
				var scriptCategory = string.Empty;
				var scriptPriority = 0;

				var nameAttribute = subType.RTGetAttribute<NameAttribute>(true);
				if (nameAttribute != null){
					scriptPriority = nameAttribute.priority;
					scriptName = nameAttribute.name;
					if (isGeneric && !scriptName.EndsWith("<T>")){
						scriptName += " (T)";
					}
				}

				var categoryAttribute = subType.RTGetAttribute<CategoryAttribute>(true);
				if (categoryAttribute != null){
					scriptCategory = categoryAttribute.category;
				}

				var info = new ScriptInfo(subType, scriptName, scriptCategory, scriptPriority);
				info.originalType = subType;
				info.originalName = scriptName;
				info.originalCategory = scriptCategory;

				//add the generic types based on constrains and prefered types list
				if (isGeneric){
					var exposeAsBaseDefinition = subType.RTIsDefined<ExposeAsDefinition>(true);
					if (!exposeAsBaseDefinition){
						var typesToWrap = UserTypePrefs.GetPreferedTypesList(true);
						foreach (var t in typesToWrap){
							infosResult.Add( info.MakeGenericInfo(t, string.Format("/{0}/{1}", info.name, t.NamespaceToPath())) );
							infosResult.Add( info.MakeGenericInfo(typeof(List<>).MakeGenericType(t), string.Format("/{0}/{1}{2}", info.name, UserTypePrefs.LIST_MENU_STRING, t.NamespaceToPath()), -1 ) );
							infosResult.Add( info.MakeGenericInfo(typeof(Dictionary<,>).MakeGenericType(typeof(string), t), string.Format("/{0}/{1}{2}", info.name, UserTypePrefs.DICT_MENU_STRING, t.NamespaceToPath()), -2 ) );
						}
						continue;
					}
				}

				infosResult.Add(info);
			}

			infosResult = infosResult
			.Where(s => s != null)
			.OrderBy(s => s.GetBaseInfo().name)
			.OrderBy(s => s.GetBaseInfo().priority * -1)
			.OrderBy(s => s.GetBaseInfo().category)
			.ToList();
			cachedInfos[baseType] = infosResult;
			return infosResult;
		}

		///Returns the base "definition" indo from which the provided info was made
		public static ScriptInfo GetBaseInfo(this ScriptInfo info){
			var result = new ScriptInfo(info.originalType, info.originalName, info.originalCategory, info.priority);
			result.originalType = info.originalType;
			result.originalName = info.originalName;
			result.originalCategory = info.originalCategory;
			return result;
		}

		///Makes and returns a closed generic ScriptInfo for targetType out of an existing ScriptInfo
		public static ScriptInfo MakeGenericInfo(this ScriptInfo info, Type targetType, string subCategory = null, int priorityShift = 0){
			if (info == null || !info.originalType.IsGenericTypeDefinition){
				return null;
			}

			info = info.GetBaseInfo();
			var genericArg = info.originalType.GetGenericArguments().First();
			if (targetType.IsAllowedByGenericArgument(genericArg)){
				var genericType = info.originalType.MakeGenericType(targetType);
				var genericCategory = info.category + subCategory;
				var genericName = info.name.Replace("(T)", string.Format("({0})", targetType.FriendlyName() ) );
				var newInfo = new ScriptInfo(genericType, genericName, genericCategory, info.priority + priorityShift);
				newInfo.originalType = info.originalType;
				newInfo.originalName = info.originalName;
				newInfo.originalCategory = info.originalCategory;
				return newInfo;
			}
			return null;
		}

        //...
        public static string NamespaceToPath(this Type type){
			if (type == null){ return string.Empty; }
			return string.IsNullOrEmpty(type.Namespace)? "No Namespace" : type.Namespace.Split('.').First();
		}


		//Get all base derived types in the current loaded assemplies, excluding the base type itself
		private static Dictionary<Type, List<Type>> cachedSubTypes = new Dictionary<Type, List<Type>>();
		public static List<Type> GetAssemblyTypes(Type baseType){

			List<Type> subTypes;
			if (cachedSubTypes.TryGetValue(baseType, out subTypes)){
				return subTypes;
			}

			subTypes = new List<Type>();

			foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
			    try
			    {
			        foreach (var t in asm.GetExportedTypes().Where(t => t.IsSubclassOf(baseType))) {
			            if ( t.IsVisible && !t.IsDefined(typeof(System.ObsoleteAttribute), true)){
							subTypes.Add(t);
						}
			        }
			    }
			    catch
			    {
			        Debug.Log(asm.FullName + " will be excluded");
			        continue;
			    }
			}				
			
			subTypes = subTypes.OrderBy(t => t.FriendlyName()).OrderBy(t => t.Namespace).ToList();
			return cachedSubTypes[baseType] = subTypes;
		}


		//Gets the first type found by providing just the name of the type. Rarely used (currently for upgrading ScriptControl tasks)
		public static Type GetType(string name, Type fallback){
			foreach (var t in GetAssemblyTypes(typeof(object))){
				if (t.Name == name){
					return t;
				}
			}
			return fallback;
		}

		///Opens the MonoScript of a type if existant
		public static bool OpenScriptOfType(Type type){
			foreach (var path in AssetDatabase.GetAllAssetPaths()){
				if (path.EndsWith(type.Name + ".cs") || path.EndsWith(type.Name + ".js")){
					var script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
					if (type == script.GetClass()){
						AssetDatabase.OpenAsset(script);
						return true;
					}
				}
			}

			Debug.Log(string.Format("Can't open script of type '{0}', cause a script with the same name does not exist", type.FriendlyName() ));
			return false;
		}


		//Get all scene names (added in build settings)
		public static List<string> GetSceneNames(){
			var allSceneNames = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes){
				if (scene.enabled){
					var name = scene.path.Substring(scene.path.LastIndexOf("/") + 1);
					name = name.Substring(0,name.Length-6);
					allSceneNames.Add(name);
				}
			}

			return allSceneNames;
		}

	}
}

#endif