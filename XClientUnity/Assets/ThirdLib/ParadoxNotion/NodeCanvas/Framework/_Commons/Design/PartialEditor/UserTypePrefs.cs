#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

#if UNITY_5_5_OR_NEWER
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
#endif

namespace ParadoxNotion.Design{

	/// <summary>
	/// A collection of user prefered types and associated colors (stored in EditorPrefs)
	/// </summary>
    public static class UserTypePrefs {
		
		public static event Action onPreferredTypesChanged;

		public const string LIST_MENU_STRING = "Collections/List (T)/";
		public const string DICT_MENU_STRING = "Collections/Dictionary (T)/";

		private static List<Type> _preferedTypes;
		private static List<Type> _preferedTypesFiltered;
		private static readonly string typesPrefsKey = string.Format("ParadoxNotion.{0}.PreferedTypes", PlayerSettings.productName);

		private static readonly List<Type> defaultTypesList = new List<Type>
		{
			typeof(object),
			typeof(System.Type),

			//Primitives
			typeof(string),
			typeof(float),
			typeof(int),
			typeof(bool),

			//Unity basics
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Quaternion),
			typeof(Color),
			typeof(LayerMask),
			typeof(AnimationCurve),
			typeof(RaycastHit),
			typeof(RaycastHit2D),

			//Unity functional classes
			typeof(Debug),
			typeof(Application),
			typeof(Mathf),
			typeof(Physics),
			typeof(Physics2D),
			typeof(Input),
			typeof(NavMesh),
			typeof(PlayerPrefs),
			typeof(UnityEngine.Random),
			typeof(Time),

			//Unity Objects
			typeof(UnityEngine.Object),
			typeof(GameObject),
			typeof(Transform),
			typeof(Animation),
			typeof(Animator),
			typeof(Rigidbody),
			typeof(Rigidbody2D),
			typeof(Collider),
			typeof(Collider2D),
			typeof(NavMeshAgent),
			typeof(CharacterController),
			typeof(AudioSource),
			typeof(Camera),
			typeof(Light),

			//UGUI
			typeof(UnityEngine.UI.Button),
			typeof(UnityEngine.UI.Slider),

			//Unity Asset Objects
			typeof(Texture2D),
			typeof(Sprite),
			typeof(AudioClip),
			typeof(AnimationClip)
		};

		//These types will be filtered out when requesting types with 'filterOutFunctionalOnlyTypes' true.
		//The problem with these is that they are not static thus instance can be made, but still, there is no reason to have an instance cause their memebers are static.
		//Hopefully this made sense :)
		public static readonly List<Type> functionalTypesBlacklist = new List<Type>
		{
			typeof(Debug),
			typeof(Application),
			typeof(Mathf),
			typeof(Physics),
			typeof(Physics2D),
			typeof(Input),
			typeof(NavMesh),
			typeof(PlayerPrefs),
			typeof(UnityEngine.Random),
			typeof(Time)
		};


		//The default prefered types list to be shown wherever a type is important
		private static string defaultTypesListString{
			get { return string.Join("|", defaultTypesList.OrderBy(t => t.Name).OrderBy(t => t.Namespace).Select(t => t.FullName).ToArray() ); }
		}

		/// Get the prefered types set by the user.
		public static List<Type> GetPreferedTypesList(bool filterOutFunctionalOnlyTypes = false){ return GetPreferedTypesList(typeof(object), filterOutFunctionalOnlyTypes); }
		public static List<Type> GetPreferedTypesList(Type baseType, bool filterOutFunctionalOnlyTypes = false){

			if (_preferedTypes == null || _preferedTypesFiltered == null){
				LoadTypes();
			}

			if (baseType == typeof(object)){
				return filterOutFunctionalOnlyTypes? _preferedTypesFiltered : _preferedTypes;
			}

			if (filterOutFunctionalOnlyTypes){
				return _preferedTypesFiltered.Where( t => baseType.IsAssignableFrom(t) ).ToList();
			}
			return _preferedTypes.Where( t => baseType.IsAssignableFrom(t) ).ToList();
		}

		[InitializeOnLoadMethod]
		private static void LoadTypes(){
			_preferedTypes = new List<Type>();
			foreach(var s in EditorPrefs.GetString(typesPrefsKey, defaultTypesListString).Split('|')){
				var resolvedType = ReflectionTools.GetType(s, /*fallback?*/ true);
				if (resolvedType != null){
					_preferedTypes.Add( resolvedType );
				}
			}
			//re-write back, so that fallback type resolved are saved
			SetPreferedTypesList(_preferedTypes);
		}

		///Set the prefered types list for the user
		public static void SetPreferedTypesList(List<Type> types){
			var finalTypes = types
			.Where(t => t != null && !t.IsGenericType)
			.OrderBy(t => t.Name)
			.OrderBy(t => t.Namespace)
			.ToList();
			var joined = string.Join("|", finalTypes.Select(t => t.FullName).ToArray() );
			EditorPrefs.SetString(typesPrefsKey, joined);
			_preferedTypes = finalTypes;

			var finalTypesFiltered = finalTypes
			.Where(t => !functionalTypesBlacklist.Contains(t) && !t.IsInterface && !t.IsAbstract )
			.ToList();
			_preferedTypesFiltered = finalTypesFiltered;
			if (onPreferredTypesChanged != null){
				onPreferredTypesChanged();
			}
		}

		///Append a type to the list
		public static void AddType(Type type){
			var current = GetPreferedTypesList(typeof(object));
			if (!current.Contains(type)){
				current.Add(type);
			}
			SetPreferedTypesList(current);
		}

		///Reset the prefered types to the default ones
		public static void ResetTypeConfiguration(){
			SetPreferedTypesList(defaultTypesList);
		}

		//----------------------------------------------------------------------------------------------

		private static readonly Color DEFAULT_TYPE_COLOR = new Color(1,1,1,0.8f);
		//A Type to color lookup
		private static Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>()
		{
			{typeof(Delegate),           new Color(1,0.4f,0.4f)},
			{typeof(bool?),              new Color(1,0.4f,0.4f)},
			{typeof(float?),             new Color(0.6f,0.6f,1)},
			{typeof(int?),               new Color(0.5f,1,0.5f)},
			{typeof(string),             new Color(0.55f,0.55f,0.55f)},
			{typeof(Vector2?),           new Color(1f,0.7f,0.2f)},
			{typeof(Vector3?),           new Color(1f,0.7f,0.2f)},
			{typeof(Vector4?),           new Color(1f,0.7f,0.2f)},
			{typeof(Quaternion?),		 new Color(1f,0.7f,0.2f)},
			{typeof(GameObject),		 new Color(0.537f, 0.415f, 0.541f)},
			{typeof(UnityEngine.Object), Color.grey}
		};

		///Get the color preference for a type
		public static Color GetTypeColor(Type type){
			
			if (!EditorGUIUtility.isProSkin){
				return Color.white;
			}
			
			if (type == null){
				return Color.black;
			}
			
			Color color;
			if (typeColors.TryGetValue(type, out color)){
				return color;
			}

			foreach (var pair in typeColors){
				
				if (pair.Key.IsAssignableFrom(type)){
					return typeColors[type] = pair.Value;
				}

				if ( typeof(IEnumerable).IsAssignableFrom(type) ){
					var elementType = type.GetEnumerableElementType();
					if (elementType != null){
						return typeColors[type] = GetTypeColor(elementType);
					}
				}
			}
			
			return typeColors[type] = DEFAULT_TYPE_COLOR;
		}

		///Get the hex color preference for a type
		public static string GetTypeHexColor(Type type){
			if (!EditorGUIUtility.isProSkin){
				return "#000000";
			}
			return EditorUtils.ColorToHex(GetTypeColor(type));
		}


		//----------------------------------------------------------------------------------------------

		public const string DEFAULT_TYPE_ICON_NAME = "System.Object";

		//A Type.FullName to texture lookup
		private static Dictionary<string, Texture> typeIcons = new Dictionary<string, Texture>(StringComparer.OrdinalIgnoreCase);
		
		///Get icon from type
		public static Texture GetTypeIcon(Type type, bool fallbackToDefault = true){
			if (type == null){ return null; }
			
			Texture texture = null;
			if (typeIcons.TryGetValue(type.FullName, out texture)){
				if (texture.name != DEFAULT_TYPE_ICON_NAME || fallbackToDefault){
					return EditorGUIUtility.isProSkin? texture : null;
				}
				return null;
			}

			if (texture == null){
				if ( type.IsEnumerableCollection() ){
					var elementType = type.GetEnumerableElementType();
					if (elementType != null){
						texture = GetTypeIcon(elementType);
					}
				}
			}

			if (typeof(UnityEngine.Object).IsAssignableFrom(type)){
				texture = AssetPreview.GetMiniTypeThumbnail(type);
				if (texture == null && typeof(MonoBehaviour).IsAssignableFrom(type)){
					texture = EditorUtils.csIcon;
				}
			}

			if (texture == null){
				texture = Resources.Load<Texture>("TypeIcons/" + type.FullName);
			}

			if (texture == null){
				var current = type.BaseType;
				while(current != null){
					texture = Resources.Load<Texture>("TypeIcons/" + current.FullName);
					current = current.BaseType;
					if (texture != null){
						break;
					}
				}
			}

			if (texture == null){
				var iconAtt = type.RTGetAttribute<IconAttribute>(true);
				if (iconAtt != null){
					texture = GetTypeIcon(iconAtt, null);
				}
			}

			if (texture == null){
				texture = Resources.Load<Texture>("TypeIcons/" + DEFAULT_TYPE_ICON_NAME);
			}

			typeIcons[type.FullName] = texture;

			if (texture != null){ //it should not be
				if (texture.name != DEFAULT_TYPE_ICON_NAME || fallbackToDefault){
					return EditorGUIUtility.isProSkin? texture : null;
				}
			}
			return null;			
		}

		///Get icon from [IconAttribute] info
		public static Texture GetTypeIcon(IconAttribute iconAttribute, object instance = null){
			if (iconAttribute == null){ return null; }

			if (instance != null && !string.IsNullOrEmpty(iconAttribute.runtimeIconTypeCallback) ){
				var callbackMethod = instance.GetType().RTGetMethod(iconAttribute.runtimeIconTypeCallback);
				return callbackMethod != null && callbackMethod.ReturnType == typeof(Type)? GetTypeIcon( (Type)callbackMethod.Invoke(instance, null), false) : null;
			}

			Texture texture = null;
			if (typeIcons.TryGetValue(iconAttribute.iconName, out texture)){
				return texture;
			}
			texture = !string.IsNullOrEmpty(iconAttribute.iconName)? Resources.Load<Texture>(iconAttribute.iconName) : null;
			return typeIcons[iconAttribute.iconName] = texture;
		}

	}
}

#endif