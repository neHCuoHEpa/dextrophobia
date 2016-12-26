using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Drive_Control_CS ) ) ]

public class Drive_Control_CSEditor : Editor {
	
	SerializedProperty TorqueProp ;
	SerializedProperty Max_SpeedProp ;
	SerializedProperty Turn_Brake_DragProp ;

	SerializedProperty Acceleration_FlagProp ;
	SerializedProperty Acceleration_TimeProp ;
	SerializedProperty Deceleration_TimeProp ;
	SerializedProperty Lost_Drag_RateProp ;
	SerializedProperty Lost_Speed_RateProp ;

	SerializedProperty Torque_LimitterProp ;
	SerializedProperty Max_Slope_AngleProp ;
	
	void OnEnable () {
		TorqueProp = serializedObject.FindProperty ( "Torque" ) ;
		Max_SpeedProp = serializedObject.FindProperty ( "Max_Speed" ) ;
		Turn_Brake_DragProp = serializedObject.FindProperty ( "Turn_Brake_Drag" ) ;

		Acceleration_FlagProp = serializedObject.FindProperty ( "Acceleration_Flag" ) ;
		Acceleration_TimeProp = serializedObject.FindProperty ( "Acceleration_Time" ) ;
		Deceleration_TimeProp = serializedObject.FindProperty ( "Deceleration_Time" ) ;
		Lost_Drag_RateProp = serializedObject.FindProperty ( "Lost_Drag_Rate" ) ;
		Lost_Speed_RateProp = serializedObject.FindProperty ( "Lost_Speed_Rate" ) ;

		Torque_LimitterProp = serializedObject.FindProperty ( "Torque_Limitter" ) ;
		Max_Slope_AngleProp = serializedObject.FindProperty ( "Max_Slope_Angle" ) ;
	}
	
	public override void OnInspectorGUI () {
		GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
		serializedObject.Update () ;
		if ( EditorApplication.isPlaying == false ) {
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "Driving Wheels Settings", MessageType.None, true );
			EditorGUILayout.Slider ( TorqueProp , 0.0f , 100000.0f , "Torque" ) ;
			EditorGUILayout.Slider ( Max_SpeedProp , 0.0f , 30.0f , "Maximum Speed" ) ;
			EditorGUILayout.Slider ( Turn_Brake_DragProp , 0.0f , 1000.0f , "Turn Brake Drag" ) ;
			
			EditorGUILayout.Space () ;
			Acceleration_FlagProp.boolValue = EditorGUILayout.Toggle ( "Acceleration" , Acceleration_FlagProp.boolValue ) ;
			if ( Acceleration_FlagProp.boolValue ) {
				EditorGUILayout.Slider ( Acceleration_TimeProp , 0.1f , 30.0f , "Acceleration Time" ) ;
				EditorGUILayout.Slider ( Deceleration_TimeProp , 0.1f , 30.0f , "Deceleration Time" ) ;
				EditorGUILayout.Slider ( Lost_Drag_RateProp , 0.0f , 1.0f , "Lost Brake Drag Rate" ) ;
				EditorGUILayout.Slider ( Lost_Speed_RateProp , 0.0f , 1.0f , "Lost Speed Rate" ) ;
			}
			
			Torque_LimitterProp.boolValue = EditorGUILayout.Toggle ( "Torque Limitter" , Torque_LimitterProp.boolValue ) ;
			if ( Torque_LimitterProp.boolValue ) {
				EditorGUILayout.Slider ( Max_Slope_AngleProp , 0.0f , 90.0f , "Max Slope Angle" ) ;
			}
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		}
		serializedObject.ApplyModifiedProperties ();
	}
	
}