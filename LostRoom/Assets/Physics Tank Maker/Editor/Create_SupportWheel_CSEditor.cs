using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Create_SupportWheel_CS ) ) ]

public class Create_SupportWheel_CSEditor : Editor {
	
	SerializedProperty Wheel_DistanceProp ;
	SerializedProperty NumProp ;
	SerializedProperty SpacingProp ;
	SerializedProperty Wheel_MassProp ;
	SerializedProperty Wheel_RadiusProp ;
	SerializedProperty Collider_MaterialProp ;
	SerializedProperty Wheel_MeshProp ;
	SerializedProperty Wheel_MaterialProp ;
	SerializedProperty Collider_MeshProp ;
	SerializedProperty Collider_Mesh_SubProp ;
	SerializedProperty ConvexProp ;
	SerializedProperty Drive_WheelProp ;
	SerializedProperty Wheel_ResizeProp ;
	SerializedProperty ScaleDown_SizeProp ;
	SerializedProperty Return_SpeedProp ;
	SerializedProperty Infinite_DurabilityProp ;
	SerializedProperty Wheel_DurabilityProp ;
	SerializedProperty Static_FlagProp ;

	SerializedProperty RealTime_FlagProp ;
	SerializedProperty Parent_TransformProp ;
	
	Transform Parent_Transform ;
	float WheelPos_X ;
	float WheelPos_Y ;
	float WheelPos_Z ;
	int Count = 1 ;
	
	void OnEnable () {
		Wheel_DistanceProp = serializedObject.FindProperty ( "Wheel_Distance" ) ;
		NumProp = serializedObject.FindProperty ( "Num" ) ;
		SpacingProp = serializedObject.FindProperty ( "Spacing" ) ;
		Wheel_MassProp = serializedObject.FindProperty ( "Wheel_Mass" ) ;
		Wheel_RadiusProp = serializedObject.FindProperty ( "Wheel_Radius" ) ;
		Collider_MaterialProp = serializedObject.FindProperty ( "Collider_Material" ) ;
		Wheel_MeshProp = serializedObject.FindProperty ( "Wheel_Mesh" ) ;
		Wheel_MaterialProp = serializedObject.FindProperty ( "Wheel_Material" ) ;
		Collider_MeshProp = serializedObject.FindProperty ( "Collider_Mesh" ) ;
		Collider_Mesh_SubProp = serializedObject.FindProperty ( "Collider_Mesh_Sub" ) ;
		ConvexProp = serializedObject.FindProperty ( "Convex" ) ;
		Drive_WheelProp = serializedObject.FindProperty ( "Drive_Wheel" ) ;
		Wheel_ResizeProp = serializedObject.FindProperty ( "Wheel_Resize" ) ;
		ScaleDown_SizeProp = serializedObject.FindProperty ( "ScaleDown_Size" ) ;
		Return_SpeedProp = serializedObject.FindProperty ( "Return_Speed" ) ;
		Infinite_DurabilityProp = serializedObject.FindProperty ( "Infinite_Durability" ) ;
		Wheel_DurabilityProp = serializedObject.FindProperty ( "Wheel_Durability" ) ;
		Static_FlagProp = serializedObject.FindProperty ( "Static_Flag" ) ;

		RealTime_FlagProp = serializedObject.FindProperty ( "RealTime_Flag" ) ;
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
			if ( GUI.changed && RealTime_FlagProp.boolValue ) {
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

		// for Static Wheel
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Wheel Type", MessageType.None, true ) ;
		Static_FlagProp.boolValue = EditorGUILayout.Toggle ( "Static Wheel" , Static_FlagProp.boolValue ) ;

		// Wheels settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Wheels settings", MessageType.None, true );
		EditorGUILayout.Slider ( Wheel_DistanceProp , 0.1f , 10.0f , "Distance" ) ;
		EditorGUILayout.IntSlider ( NumProp , 0 , 30 , "Number" ) ;
		EditorGUILayout.Slider ( SpacingProp , 0.1f , 10.0f , "Spacing" ) ;
		if ( !Static_FlagProp.boolValue ) { // Physics Wheel.
			EditorGUILayout.Slider ( Wheel_MassProp , 0.1f , 300.0f , "Mass" ) ;
		}
		EditorGUILayout.Space () ;
		GUI.backgroundColor = new Color ( 1.0f , 0.5f , 0.5f , 1.0f ) ;
		EditorGUILayout.Slider ( Wheel_RadiusProp , 0.01f , 10.0f , "SphereCollider Radius" ) ;
		GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
		Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Physic Material" , Collider_MaterialProp.objectReferenceValue , typeof ( PhysicMaterial ) , false ) ;
		EditorGUILayout.Space () ;
		Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Mesh" , Wheel_MeshProp.objectReferenceValue , typeof ( Mesh ) , false ) ;
		Wheel_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Material" , Wheel_MaterialProp.objectReferenceValue , typeof ( Material ) , false ) ;

		// Mesh Collider
		if ( !Static_FlagProp.boolValue ) {
			EditorGUILayout.Space () ; EditorGUILayout.Space () ;
			EditorGUILayout.HelpBox( "'MeshCollider settings for 'Physic Wheel'", MessageType.None, true );
			Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Mesh Collider" , Collider_MeshProp.objectReferenceValue , typeof ( Mesh ) , false ) ;
			Collider_Mesh_SubProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Mesh Sub Collider" , Collider_Mesh_SubProp.objectReferenceValue , typeof ( Mesh ) , false ) ;
			ConvexProp.boolValue = EditorGUILayout.Toggle ( "Convex" , ConvexProp.boolValue ) ;
		}

		// Scripts settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		if ( !Static_FlagProp.boolValue ) {
			EditorGUILayout.HelpBox( "Scripts settings for 'Physics_Wheel'", MessageType.None, true );
			// Drive Wheel
			Drive_WheelProp.boolValue = EditorGUILayout.Toggle ( "Drive Wheel" , Drive_WheelProp.boolValue ) ;
			EditorGUILayout.Space () ;
			// Wheel Resize
			Wheel_ResizeProp.boolValue = EditorGUILayout.Toggle ( "Wheel Resize Script" , Wheel_ResizeProp.boolValue ) ;
			if ( Wheel_ResizeProp.boolValue ) {
				EditorGUILayout.Slider ( ScaleDown_SizeProp , 0.1f , 3.0f , "Scale Size" ) ;
				EditorGUILayout.Slider ( Return_SpeedProp , 0.01f , 0.1f , "Return Speed" ) ;
			}
			EditorGUILayout.Space () ;
			// Durability
			Infinite_DurabilityProp.boolValue = EditorGUILayout.Toggle ( "Infinite Durability" , Infinite_DurabilityProp.boolValue ) ;
			if ( !Infinite_DurabilityProp.boolValue ) {
				EditorGUILayout.Slider ( Wheel_DurabilityProp , 1 , 1000000 , "Wheel Durability" ) ;
			}
		}

		// Update Value
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		RealTime_FlagProp.boolValue = EditorGUILayout.Toggle ( "Real Time Update" , RealTime_FlagProp.boolValue ) ;
		if ( GUILayout.Button ( "Update Value" ) ) {
			if ( RealTime_FlagProp.boolValue == false ) {
				Create () ;
			}
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
		
		// Create Wheel	
		Count = 1 ;
		for ( int i = 0 ;  i  < NumProp.intValue ; i++ ) {
			
			WheelPos_X = 0.0f ;
			WheelPos_Z = -SpacingProp.floatValue * i ;
			
			WheelPos_Y = -Wheel_DistanceProp.floatValue / 2.0f ;
			SetWheelValue ( "R" ) ;
			
			WheelPos_Y = Wheel_DistanceProp.floatValue / 2.0f ;
			SetWheelValue ( "L" ) ;
			
			Count ++ ;
		}
	}
	
	void SetWheelValue ( string Direction ){
		//Create gameobject & Set transform
		GameObject Temp_Object = new GameObject ( "SupportWheel_" + Direction + "_" + Count ) ;
		Temp_Object.transform.parent = Parent_Transform ;
		Temp_Object.transform.localPosition = new Vector3 ( WheelPos_X , WheelPos_Y , WheelPos_Z ) ;
		if ( Direction == "R" ) {
			Temp_Object.transform.localRotation = Quaternion.Euler ( 0.0f , 0.0f , 180.0f ) ;
		} else {
			Temp_Object.transform.localRotation = Quaternion.Euler ( Vector3.zero ) ;
		}
		// Mesh
		Temp_Object.AddComponent < MeshRenderer > () ;
		Temp_Object.GetComponent<Renderer>().material = Wheel_MaterialProp.objectReferenceValue as Material ;
		MeshFilter Temp_MeshFilter ;
		Temp_MeshFilter = Temp_Object.AddComponent < MeshFilter > () ;
		Temp_MeshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh ;
		// SphereCollider
		SphereCollider Temp_SphereCollider ;
		Temp_SphereCollider = Temp_Object.AddComponent < SphereCollider > () ;
		Temp_SphereCollider.radius = Wheel_RadiusProp.floatValue ;
		Temp_SphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial ;
		//
		if ( !Static_FlagProp.boolValue ) { // Physics Wheel.
			// Rigidbody
			Temp_Object.AddComponent < Rigidbody > () ;
			Temp_Object.GetComponent<Rigidbody>().mass = Wheel_MassProp.floatValue ;
			// HingeJoint
			HingeJoint Temp_HingeJoint ;
			Temp_HingeJoint = Temp_Object.AddComponent < HingeJoint > () ;
			Temp_HingeJoint.anchor = Vector3.zero ;
			Temp_HingeJoint.axis = new Vector3 ( 0.0f , 1.0f , 0.0f ) ;
			Temp_HingeJoint.connectedBody = Parent_Transform.parent.gameObject.GetComponent<Rigidbody>() ;
			// MeshCollider
			MeshCollider Temp_MeshCollider ;
			Temp_MeshCollider = Temp_Object.AddComponent < MeshCollider > () ;
			Temp_MeshCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial ;
			Temp_MeshCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh ;
			Temp_MeshCollider.convex = ConvexProp.boolValue ;
			Temp_MeshCollider.enabled = false ;
			// Sub MeshCollider
			if  ( Collider_Mesh_SubProp.objectReferenceValue != null ) {
				MeshCollider Temp_MeshCollider_Sub ;
				Temp_MeshCollider_Sub = Temp_Object.AddComponent < MeshCollider > () ;
				Temp_MeshCollider_Sub.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial ;
				Temp_MeshCollider_Sub.sharedMesh = Collider_Mesh_SubProp.objectReferenceValue as Mesh ;
				Temp_MeshCollider_Sub.convex = ConvexProp.boolValue ;
				Temp_MeshCollider_Sub.enabled = false ;
			}
			// Drive_Wheel_CS
			Drive_Wheel_CS Temp_Drive_Wheel_CS ;
			Temp_Drive_Wheel_CS = Temp_Object.AddComponent < Drive_Wheel_CS > () ;
			Temp_Drive_Wheel_CS.Radius = Wheel_RadiusProp.floatValue ;
			Temp_Drive_Wheel_CS.Drive_Flag = Drive_WheelProp.boolValue ;
			// Wheel_Resize_CS
			if ( Wheel_ResizeProp.boolValue ) {
				Wheel_Resize_CS Temp_Wheel_Resize_CS ;
				Temp_Wheel_Resize_CS = Temp_Object.AddComponent < Wheel_Resize_CS > () ;
				Temp_Wheel_Resize_CS.ScaleDown_Size = ScaleDown_SizeProp.floatValue ;
				Temp_Wheel_Resize_CS.Return_Speed = Return_SpeedProp.floatValue ;
			}
			// Damage_Control_CS
			Damage_Control_CS Temp_Damage_Control_CS ;
			Temp_Damage_Control_CS = Temp_Object.AddComponent < Damage_Control_CS > () ;
			Temp_Damage_Control_CS.Type = 8 ; // 8 = Wheel in "Damage_Control"
			if ( Infinite_DurabilityProp.boolValue ) {
				Temp_Damage_Control_CS.Durability = Mathf.Infinity ;
			} else {
				Temp_Damage_Control_CS.Durability = Wheel_DurabilityProp.floatValue ;
			} 
			if ( Direction == "L" ) {
				Temp_Damage_Control_CS.Direction = 0 ;
			} else {
				Temp_Damage_Control_CS.Direction = 1 ;
			}
			// Stabilizer_CS
			Temp_Object.AddComponent < Stabilizer_CS > () ;
		} else { // Static Wheel
			Temp_Object.AddComponent < Static_Wheel_CS > () ;
		}
		// Set Layer
		Temp_Object.layer = 9 ; // Ignore eachother.
	}
}