using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Tank_ID_Control_CS ) ) ]

public class Tank_ID_Control_CSEditor : Editor {
	
	SerializedProperty Tank_IDProp ;
	SerializedProperty RelationshipProp ;
	SerializedProperty Input_TypeProp ;
	SerializedProperty Turn_TypeProp ;
	SerializedProperty Auto_ReSpawn_FlagProp ;
	SerializedProperty ReSpawn_TimeProp ;

	SerializedProperty ReSpawn_FlagProp ;
	SerializedProperty Prefab_PathProp ;

	string[] ID_Names = { "Not Operable" , "[ 1 ]" , "[ 2 ]" , "[ 3 ]" , "[ 4 ]" , "[ 5 ]" , "[ 6 ]" , "[ 7 ]" , "[ 8 ]" , "[ 9 ]" , "[10]" } ;
	string[] Relationship_Names = { "Friendly" , "Hostile" } ;
	string[] Input_Type_Names = { "Keyboard ( Keyboard only )" , "GamePad ( Stick operation )" , "GamePad ( Trigger operation )" ,  "GamePad ( Stick+Trigger operation )" , "Mouse + Keyboard ( Default )" , "Mouse + Keyboard ( Easy )" , "" , "" , "" , "" , "AI" } ;
	string[] Turn_Type_Names = { "Easy Turn (Pivot Turn)" , "Classic Turn (only Brake-Turn)" } ;

	void OnEnable () {
		Tank_IDProp = serializedObject.FindProperty ( "Tank_ID" ) ;
		RelationshipProp = serializedObject.FindProperty ( "Relationship" ) ;
		Input_TypeProp = serializedObject.FindProperty ( "Input_Type" ) ;
		Turn_TypeProp = serializedObject.FindProperty ( "Turn_Type" ) ;
		Auto_ReSpawn_FlagProp = serializedObject.FindProperty ( "Auto_ReSpawn_Flag" ) ;
		ReSpawn_TimeProp = serializedObject.FindProperty ( "ReSpawn_Time" ) ;

		ReSpawn_FlagProp = serializedObject.FindProperty ( "ReSpawn_Flag" ) ;
		Prefab_PathProp = serializedObject.FindProperty ( "Prefab_Path" ) ;
	}
	
	public override void OnInspectorGUI () {
		if ( EditorApplication.isPlaying == false ) {
			GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
			serializedObject.Update () ;

			// Set Prefab_Path and ReSpawn_Flag.
			Get_Prefab_Path () ;

			EditorGUILayout.Space () ; EditorGUILayout.Space () ;	
			EditorGUILayout.HelpBox( "Set the ID number for this tank.", MessageType.None, true );
			Tank_IDProp.intValue = EditorGUILayout.Popup ( "Tank ID" , Tank_IDProp.intValue , ID_Names ) ;
			EditorGUILayout.Space () ;

			RelationshipProp.intValue = EditorGUILayout.Popup ( "Relationship" , RelationshipProp.intValue , Relationship_Names ) ;
			EditorGUILayout.Space () ;

			EditorGUILayout.HelpBox( "Choose 'AI' when you use AI tank.", MessageType.Warning, true );
			Input_TypeProp.intValue = EditorGUILayout.Popup ( "Input Device Type" , Input_TypeProp.intValue , Input_Type_Names ) ;
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			
			if ( Input_TypeProp.intValue < 4 ) {
				EditorGUILayout.HelpBox( "Have you finished setting up 'Input Manager' ?", MessageType.Warning , true );
				EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			}

			if ( Input_TypeProp.intValue == 0 || Input_TypeProp.intValue == 1 || Input_TypeProp.intValue == 5 ) {
				EditorGUILayout.HelpBox( "Set the type of turning.", MessageType.None, true );
				Turn_TypeProp.intValue = EditorGUILayout.Popup ( "Turn Type" , Turn_TypeProp.intValue , Turn_Type_Names ) ;
				EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			}

			if ( Input_TypeProp.intValue == 10 ) { // in case of AI.
				Auto_ReSpawn_FlagProp.boolValue = EditorGUILayout.Toggle ( "Auto ReSpawn" , Auto_ReSpawn_FlagProp.boolValue ) ;
				if ( Auto_ReSpawn_FlagProp.boolValue ) {
					EditorGUILayout.Slider ( ReSpawn_TimeProp , 0.0f , 100.0f , "ReSpawn Time" ) ;
				}
			}

			EditorGUILayout.Space () ; EditorGUILayout.Space () ;	
			if ( ReSpawn_FlagProp.boolValue ) {
				EditorGUILayout.HelpBox( "Path of the prefab in 'Resources' folder." , MessageType.None , true ) ;
				EditorGUILayout.HelpBox( Prefab_PathProp.stringValue , MessageType.None, true ) ;
			} else {
				EditorGUILayout.HelpBox( "Tank prefab must be placed under 'Resources' folder." , MessageType.Error , true ) ;
			}

			EditorGUILayout.Space () ; EditorGUILayout.Space () ;

			serializedObject.ApplyModifiedProperties () ;
		}
	}

	void Get_Prefab_Path () {
		Object Temp_Object = PrefabUtility.GetPrefabParent ( Selection.activeGameObject ) ;
		if ( Temp_Object ) {
			string Temp_Path = AssetDatabase.GetAssetPath ( Temp_Object ) ;
			if ( !string.IsNullOrEmpty ( Temp_Path ) ) {
				int Index = Temp_Path.IndexOf ( "Resources/" ) ;
				if ( Index < 0 ) { // This prefab is not placed in 'Resources' folder.
					ReSpawn_FlagProp.boolValue = false ;
					Prefab_PathProp.stringValue = null ;
				} else {
					Temp_Path = Temp_Path.Substring ( Index + 10 ) ;
					Index = Temp_Path.IndexOf ( ".prefab" ) ;
					if ( Index < 0 ) {
						ReSpawn_FlagProp.boolValue = false ;
						Prefab_PathProp.stringValue = null ;
					} else {
						ReSpawn_FlagProp.boolValue = true ;
						Temp_Path = Temp_Path.Substring ( 0 , Temp_Path.Length - 7 ) ;
						Prefab_PathProp.stringValue = Temp_Path ;
					}
				}
			}
		}
	}
}
