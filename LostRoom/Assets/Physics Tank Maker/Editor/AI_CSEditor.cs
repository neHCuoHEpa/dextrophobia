using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( AI_CS ) ) ]

public class AI_CSEditor : Editor {
	
	SerializedProperty WayPoint_PackProp ;
	SerializedProperty WayPoint_RadiusProp ;
	SerializedProperty Patrol_TypeProp ;
	SerializedProperty Follow_TargetProp ;

	SerializedProperty Min_Turn_AngleProp ;
	SerializedProperty Pivot_Turn_AngleProp ;
	SerializedProperty Slow_Turn_AngleProp ;
	SerializedProperty Slow_Turn_RateProp ;
	SerializedProperty Min_Turn_RateProp ;
	SerializedProperty Max_Turn_RateProp ;
	SerializedProperty Min_Speed_RateProp ;
	SerializedProperty Max_Speed_RateProp ;
	SerializedProperty Stuck_CountProp ;

	SerializedProperty Visibility_RadiusProp ;
	SerializedProperty Approach_DistanceProp ;
	SerializedProperty Face_EnemyProp ;
	SerializedProperty Face_Offest_AngleProp ;
	SerializedProperty Direct_FireProp ;
	SerializedProperty Lost_CountProp ;
	SerializedProperty Fire_CountProp ;
	SerializedProperty Fire_AngleProp ;
	SerializedProperty Bullet_TypeProp ;

	SerializedProperty Message_GUITextProp ;

	string[] Patrol_Type_Names = { "Order" , "Random" } ;
	string[] Bullet_Type_Names = { "AP" , "HE" } ;

	void OnEnable () {
		WayPoint_PackProp = serializedObject.FindProperty ( "WayPoint_Pack" ) ;
		WayPoint_RadiusProp = serializedObject.FindProperty ( "WayPoint_Radius" ) ;
		Patrol_TypeProp = serializedObject.FindProperty ( "Patrol_Type" ) ;
		Follow_TargetProp = serializedObject.FindProperty ( "Follow_Target" ) ;

		Min_Turn_AngleProp = serializedObject.FindProperty ( "Min_Turn_Angle" ) ;
		Pivot_Turn_AngleProp = serializedObject.FindProperty ( "Pivot_Turn_Angle" ) ;
		Slow_Turn_AngleProp = serializedObject.FindProperty ( "Slow_Turn_Angle" ) ;
		Slow_Turn_RateProp = serializedObject.FindProperty ( "Slow_Turn_Rate" ) ;

		Min_Turn_RateProp = serializedObject.FindProperty ( "Min_Turn_Rate" ) ;
		Max_Turn_RateProp = serializedObject.FindProperty ( "Max_Turn_Rate" ) ;
		Min_Speed_RateProp = serializedObject.FindProperty ( "Min_Speed_Rate" ) ;
		Max_Speed_RateProp = serializedObject.FindProperty ( "Max_Speed_Rate" ) ;
		Stuck_CountProp = serializedObject.FindProperty ( "Stuck_Count" ) ;

		Visibility_RadiusProp = serializedObject.FindProperty ( "Visibility_Radius" ) ;
		Approach_DistanceProp = serializedObject.FindProperty ( "Approach_Distance" ) ;
		Face_EnemyProp = serializedObject.FindProperty ( "Face_Enemy" ) ;
		Face_Offest_AngleProp = serializedObject.FindProperty ( "Face_Offest_Angle" ) ;
		Direct_FireProp = serializedObject.FindProperty ( "Direct_Fire" ) ;
		Lost_CountProp = serializedObject.FindProperty ( "Lost_Count" ) ;
		Fire_CountProp = serializedObject.FindProperty ( "Fire_Count" ) ;
		Fire_AngleProp = serializedObject.FindProperty ( "Fire_Angle" ) ;
		Bullet_TypeProp = serializedObject.FindProperty ( "Bullet_Type" ) ;

		Message_GUITextProp = serializedObject.FindProperty ( "Message_GUIText" ) ;
	}
	
	public override void OnInspectorGUI () {
		GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
		serializedObject.Update () ;
		if ( EditorApplication.isPlaying == false ) {
			EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "Patrol Settings", MessageType.None, true );
			WayPoint_PackProp.objectReferenceValue = EditorGUILayout.ObjectField ( "WayPoint Pack" , WayPoint_PackProp.objectReferenceValue , typeof ( GameObject ) , true ) ;
			EditorGUILayout.Slider ( WayPoint_RadiusProp , 0.0f , 1000.0f , "WayPoint Radius" ) ;
			Patrol_TypeProp.intValue = EditorGUILayout.Popup ( "Patrol Type" , Patrol_TypeProp.intValue , Patrol_Type_Names ) ;
			EditorGUILayout.Space () ;
			Follow_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Follow Target" , Follow_TargetProp.objectReferenceValue , typeof ( Transform ) , true ) ;
		}
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Drive Settings", MessageType.None, true );
		EditorGUILayout.Slider ( Min_Turn_AngleProp , 0.0f , 10.0f , "Min Turn Angle" ) ;
		EditorGUILayout.Slider ( Pivot_Turn_AngleProp , 0.0f , 360.0f , "Pivot Turn Angle" ) ;
		EditorGUILayout.Slider ( Slow_Turn_AngleProp , 0.0f , 360.0f , "Slow Turn Angle" ) ;
		EditorGUILayout.Slider ( Slow_Turn_RateProp , 0.0f , 1.0f , "Slow Turn Rate" ) ;
		EditorGUILayout.Space () ;
		EditorGUILayout.Slider ( Min_Turn_RateProp , 0.0f , 1.0f , "Min Turn Rate" ) ;
		EditorGUILayout.Slider ( Max_Turn_RateProp , 0.0f , 1.0f , "Max Turn Rate" ) ;
		EditorGUILayout.Slider ( Min_Speed_RateProp , 0.0f , 1.0f , "Min Speed Rate" ) ;
		EditorGUILayout.Slider ( Max_Speed_RateProp , 0.0f , 1.0f , "Max Speed Rate" ) ;
		EditorGUILayout.Slider ( Stuck_CountProp , 1.0f , 10.0f , "Stuck Count" ) ;

		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Combat Settings", MessageType.None, true );
		EditorGUILayout.Slider ( Visibility_RadiusProp , 0.1f , 10000.0f , "Visibility Radius" ) ;
		EditorGUILayout.Slider ( Approach_DistanceProp , 1.0f , 10000.0f , "Approach Distance" ) ;
		if ( Approach_DistanceProp.floatValue == 10000.0f ) {
			Approach_DistanceProp.floatValue = Mathf.Infinity ;
		}
		Face_EnemyProp.boolValue = EditorGUILayout.Toggle ( "Face the Enemy" , Face_EnemyProp.boolValue ) ;
		if ( Face_EnemyProp.boolValue ) {
			EditorGUILayout.Slider ( Face_Offest_AngleProp , 0.0f , 90.0f , "Face Offest Angle" ) ;
		}
		Direct_FireProp.boolValue = EditorGUILayout.Toggle ( "Direct Fire" , Direct_FireProp.boolValue ) ;
		EditorGUILayout.Slider ( Lost_CountProp , 0.0f , 100.0f , "Lost Count" ) ;
		EditorGUILayout.Slider ( Fire_AngleProp , 0.0f , 45.0f , "Fire Angle" ) ;
		EditorGUILayout.Slider ( Fire_CountProp , 0.0f , 10.0f , "Fire Count" ) ;
		Bullet_TypeProp.intValue = EditorGUILayout.Popup ( "Bullet Type" , Bullet_TypeProp.intValue , Bullet_Type_Names ) ;
		EditorGUILayout.Space () ;

		EditorGUILayout.HelpBox( "GUI Text Settings", MessageType.None, true );
		Message_GUITextProp.objectReferenceValue = EditorGUILayout.ObjectField ( "GUI Text" , Message_GUITextProp.objectReferenceValue , typeof ( GUIText ) , true ) ;

		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		serializedObject.ApplyModifiedProperties ();
	}
	
}