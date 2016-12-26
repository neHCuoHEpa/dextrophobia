using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Game_Controller_CS ) ) ]

public class Game_Controller_CSEditor : Editor {

	SerializedProperty Max_Friendly_NumProp ;
	SerializedProperty Max_Hostile_NumProp ;
	SerializedProperty Assign_FrequencyProp ;

	SerializedProperty Easy_Culling_FlagProp ;
	SerializedProperty Ray_DistanceProp ;
	SerializedProperty Ignore_DistanceProp ;

	SerializedProperty Carving_FlagProp ;

	SerializedProperty Time_ScaleProp ;
	SerializedProperty GravityProp ;
	SerializedProperty Fixed_TimeStepProp ;

	void OnEnable () {
		Max_Friendly_NumProp = serializedObject.FindProperty ( "Max_Friendly_Num" ) ;
		Max_Hostile_NumProp = serializedObject.FindProperty ( "Max_Hostile_Num" ) ;
		Assign_FrequencyProp = serializedObject.FindProperty ( "Assign_Frequency" ) ;

		Easy_Culling_FlagProp = serializedObject.FindProperty ( "Easy_Culling_Flag" ) ;
		Ray_DistanceProp = serializedObject.FindProperty ( "Ray_Distance" ) ;
		Ignore_DistanceProp = serializedObject.FindProperty ( "Ignore_Distance" ) ;

		Carving_FlagProp = serializedObject.FindProperty ( "Carving_Flag" ) ;

		Time_ScaleProp = serializedObject.FindProperty ( "Time_Scale" ) ;
		GravityProp = serializedObject.FindProperty ( "Gravity" ) ;
		Fixed_TimeStepProp = serializedObject.FindProperty ( "Fixed_TimeStep" ) ;
	}
	
	public override void OnInspectorGUI () {
		if ( EditorApplication.isPlaying == false ) {
			GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
			serializedObject.Update () ;

			// Scene settings.
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "Scene settings", MessageType.None, true );
			EditorGUILayout.IntSlider ( Max_Friendly_NumProp , 5 , 50 , "Max Friendly tanks" ) ;
			EditorGUILayout.IntSlider ( Max_Hostile_NumProp , 5 , 50 , "Max Hostile tanks" ) ;
			EditorGUILayout.Slider ( Assign_FrequencyProp , 0.1f , 10.0f , "Assign Target Interval" ) ;

			// Culling settings.
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "Culling settings for Static_Track", MessageType.None, true );
			Easy_Culling_FlagProp.boolValue = EditorGUILayout.Toggle ( "Easy Culling" , Easy_Culling_FlagProp.boolValue ) ;
			if ( Easy_Culling_FlagProp.boolValue ) {
				EditorGUILayout.Slider ( Ray_DistanceProp , 500.0f , 5000.0f , "Ray Distance" ) ;
				EditorGUILayout.Slider ( Ignore_DistanceProp , 0.0f , 100.0f , "Ignore Distance" ) ;
			}

			// NavMesh Obstacle setting.
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "NavMesh Obstacle settings for AI ( only for 'Unity Pro' )", MessageType.None, true );
			Carving_FlagProp.boolValue = EditorGUILayout.Toggle ( "Carving" , Carving_FlagProp.boolValue ) ;

			// Time Manager and Gravity settings.
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "Time Manager and Gravity settings", MessageType.None, true );
			EditorGUILayout.Slider ( Time_ScaleProp , 0.1f , 5.0f , "Time Scale" ) ;
			EditorGUILayout.Slider ( GravityProp , -9.81f , -1.0f , "Gravity" ) ;
			EditorGUILayout.Slider ( Fixed_TimeStepProp , 0.01f , 0.03f , "Fixed TimeStep" ) ;

			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		
			serializedObject.ApplyModifiedProperties ();
		}
	}
	
}