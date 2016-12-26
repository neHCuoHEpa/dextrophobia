using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Create_SwingBall_CS ) ) ]

public class Create_SwingBall_CSEditor : Editor {
	
	SerializedProperty DistanceProp ;
	SerializedProperty NumProp ;
	SerializedProperty SpacingProp ;
	SerializedProperty MassProp ;
	SerializedProperty GravityProp ;
	SerializedProperty RadiusProp ;
	SerializedProperty LimitProp ;
	SerializedProperty SpringProp ;

	SerializedProperty Parent_TransformProp ;
	
	Transform Parent_Transform ;
	float Pos_X ;
	float Pos_Y ;
	float Pos_Z ;
	int Count = 1 ;
	
	void OnEnable () {
		DistanceProp = serializedObject.FindProperty ( "Distance" ) ;
		NumProp = serializedObject.FindProperty ( "Num" ) ;
		SpacingProp = serializedObject.FindProperty ( "Spacing" ) ;
		MassProp = serializedObject.FindProperty ( "Mass" ) ;
		GravityProp = serializedObject.FindProperty ( "Gravity" ) ;
		RadiusProp = serializedObject.FindProperty ( "Radius" ) ;
		LimitProp = serializedObject.FindProperty ( "Limit" ) ;
		SpringProp = serializedObject.FindProperty ( "Spring" ) ;

		Parent_TransformProp = serializedObject.FindProperty ( "Parent_Transform" ) ;
		
		Parent_Transform = Parent_TransformProp.objectReferenceValue as Transform ;
	}
	
	public override void OnInspectorGUI () {
		bool Work_Flag ;
		if ( Parent_Transform.parent == null || Parent_Transform.parent.gameObject.GetComponent<Rigidbody>() == null ) {
			Work_Flag = false ;
		} else {
			Work_Flag = true ;
		}
		
		if ( Work_Flag ) {
			Set_Inspector () ;
			if ( GUI.changed ) {
				Create () ;
			}
			if (Event.current.commandName == "UndoRedoPerformed" ) {
				Create () ;
			}
		}
	}
	
	void Set_Inspector () {
		GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
		serializedObject.Update () ;

		// Balls settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Balls settings", MessageType.None, true );
		EditorGUILayout.Slider ( DistanceProp , 0.1f , 10.0f , "Distance" ) ;
		EditorGUILayout.IntSlider ( NumProp , 0 , 30 , "Number" ) ;
		EditorGUILayout.Slider ( SpacingProp , 0.1f , 10.0f , "Spacing" ) ;
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.Slider ( MassProp , 0.1f , 300.0f , "Mass" ) ;
		GravityProp.boolValue = EditorGUILayout.Toggle ( "Use Gravity" , GravityProp.boolValue ) ;
		EditorGUILayout.Space () ;
		EditorGUILayout.Slider ( RadiusProp , 0.01f , 10.0f , "SphereCollider Radius" ) ;
		EditorGUILayout.Space () ;
		EditorGUILayout.Slider ( LimitProp , 0.00f , 1.0f , "Limit Distance" ) ;
		EditorGUILayout.Slider ( SpringProp , 1.0f , 100000.0f , "Spring Force" ) ;

		// Update Value
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		if ( GUILayout.Button ( "Update Value" ) ) {
			Create () ;
		}
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		
		//
		serializedObject.ApplyModifiedProperties ();
	}
	
	
	void Create () {
		// Delete Objects
		int Temp_Num = Parent_Transform.childCount ;
		for ( int i = 0 ;  i  < Temp_Num ; i++ ) {
			DestroyImmediate ( Parent_Transform.GetChild ( 0 ).gameObject ) ;
		}
		
		// Create Ball	
		Count = 1 ;
		for ( int i = 0 ;  i  < NumProp.intValue ; i++ ) {
			Pos_X = 0.0f ;
			Pos_Z = -SpacingProp.floatValue * i ;

			Pos_Y = -DistanceProp.floatValue / 2.0f ;
			SetBallValue ( "R" ) ;
			Pos_Y = DistanceProp.floatValue / 2.0f ;
			SetBallValue ( "L" ) ;
			
			Count ++ ;
		}
	}
	
	void SetBallValue ( string Direction ){
		//Create gameobject & Set transform
		GameObject Temp_Object = new GameObject ( "SwingBall_" + Direction + "_" + Count ) ;
		Temp_Object.transform.parent = Parent_Transform ;
		Temp_Object.transform.localPosition = new Vector3 ( Pos_X , Pos_Y , Pos_Z ) ;
		// SphereCollider
		SphereCollider Temp_SphereCollider ;
		Temp_SphereCollider = Temp_Object.AddComponent < SphereCollider > () ;
		Temp_SphereCollider.radius = RadiusProp.floatValue ;
		// Rigidbody
		Temp_Object.AddComponent < Rigidbody > () ;
		Temp_Object.GetComponent<Rigidbody>().mass = MassProp.floatValue ;
		Temp_Object.GetComponent<Rigidbody>().useGravity = GravityProp.boolValue ;
		// ConfigurableJoint
		ConfigurableJoint Temp_Joint ;
		Temp_Joint = Temp_Object.AddComponent < ConfigurableJoint > () ;
		Temp_Joint.connectedBody = Parent_Transform.parent.gameObject.GetComponent<Rigidbody>() ;
		Temp_Joint.anchor = Vector3.zero ;
		Temp_Joint.axis = Vector3.zero ;
		Temp_Joint.secondaryAxis = Vector3.zero ;
		Temp_Joint.xMotion = ConfigurableJointMotion.Locked ;
		Temp_Joint.yMotion = ConfigurableJointMotion.Limited ;
		Temp_Joint.zMotion = ConfigurableJointMotion.Locked ;
		Temp_Joint.angularXMotion = ConfigurableJointMotion.Locked ;
		Temp_Joint.angularYMotion = ConfigurableJointMotion.Locked ;
		Temp_Joint.angularZMotion = ConfigurableJointMotion.Locked ;
		SoftJointLimit Temp_SoftJointLimit = Temp_Joint.linearLimit ; // Set Linear Limit
		Temp_SoftJointLimit.limit = LimitProp.floatValue ;
//		Temp_SoftJointLimit.spring = SpringProp.floatValue ;
		Temp_Joint.linearLimit = Temp_SoftJointLimit ;
		// Set Layer
		Temp_Object.layer = 10 ; // Ignore All
	}
	
}
