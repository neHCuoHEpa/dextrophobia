﻿using UnityEngine;
using System.Collections;
using UnityEditor ;

[ CustomEditor ( typeof ( Turret_Base_CS ) ) ]

public class Turret_Base_CSEditor : Editor {

	SerializedProperty Part_MeshProp ;
	SerializedProperty Collider_MeshProp ;
	SerializedProperty Sub_Collider_MeshProp ;

	SerializedProperty Materials_NumProp ;
	SerializedProperty MaterialsProp ;
	SerializedProperty Part_MaterialProp ;

	SerializedProperty Offset_XProp ;
	SerializedProperty Offset_YProp ;
	SerializedProperty Offset_ZProp ;

	SerializedProperty Coming_OffProp ;
	SerializedProperty Turret_MassProp ;
	SerializedProperty Infinite_DurabilityProp ;
	SerializedProperty DurabilityProp ;
	SerializedProperty Sub_DurabilityProp ;
	SerializedProperty Trouble_TimeProp ;
	SerializedProperty Damage_Effect_ObjectProp ;
	SerializedProperty Trouble_Effect_ObjectProp ;
	SerializedProperty Parent_TransformProp ;

	Transform Parent_Transform ;
	
	void  OnEnable (){
		Part_MeshProp = serializedObject.FindProperty ( "Part_Mesh" ) ;
		Collider_MeshProp = serializedObject.FindProperty ( "Collider_Mesh" ) ;
		Sub_Collider_MeshProp = serializedObject.FindProperty ( "Sub_Collider_Mesh" ) ;

		Materials_NumProp = serializedObject.FindProperty ( "Materials_Num" ) ;
		MaterialsProp = serializedObject.FindProperty ( "Materials" ) ;
		Part_MaterialProp = serializedObject.FindProperty ( "Part_Material" ) ;

		Offset_XProp = serializedObject.FindProperty ( "Offset_X" ) ;
		Offset_YProp = serializedObject.FindProperty ( "Offset_Y" ) ;
		Offset_ZProp = serializedObject.FindProperty ( "Offset_Z" ) ;

		Coming_OffProp = serializedObject.FindProperty ( "Coming_Off" ) ;
		Turret_MassProp = serializedObject.FindProperty ( "Turret_Mass" ) ;
		Infinite_DurabilityProp = serializedObject.FindProperty ( "Infinite_Durability" ) ;
		DurabilityProp = serializedObject.FindProperty ( "Durability" ) ;
		Sub_DurabilityProp = serializedObject.FindProperty ( "Sub_Durability" ) ;
		Trouble_TimeProp = serializedObject.FindProperty ( "Trouble_Time" ) ;
		Damage_Effect_ObjectProp = serializedObject.FindProperty ( "Damage_Effect_Object" ) ;
		Trouble_Effect_ObjectProp = serializedObject.FindProperty ( "Trouble_Effect_Object" ) ;
		Parent_TransformProp = serializedObject.FindProperty ( "Parent_Transform" ) ;
		
		Parent_Transform = Parent_TransformProp.objectReferenceValue as Transform ;
	}
	
	public override void OnInspectorGUI () {
		Set_Inspector () ;
		if ( GUI.changed ) {
			Create () ;
		}
		if ( Event.current.commandName == "UndoRedoPerformed" ) {
			Create () ;
		}
		if ( Parent_Transform.hasChanged ) {
			Create () ;
			Parent_Transform.hasChanged = false ;
		}
	}
	
	void Set_Inspector () {
		GUI.backgroundColor = new Color ( 1.0f , 1.0f , 0.5f , 1.0f ) ;
		serializedObject.Update () ;
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Fold out above 'Transform' window when you move this object.", MessageType.Warning, true );

		// Mesh settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Mesh settings", MessageType.None, true );
		Part_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Mesh" , Part_MeshProp.objectReferenceValue , typeof ( Mesh ) , false ) ;
		EditorGUILayout.Space () ;
		EditorGUILayout.IntSlider ( Materials_NumProp , 1 , 10 , "Number of Materials" ) ;
		MaterialsProp.arraySize = Materials_NumProp.intValue ;
		if ( Materials_NumProp.intValue == 1 && Part_MaterialProp.objectReferenceValue != null ) {
			if ( MaterialsProp.GetArrayElementAtIndex ( 0 ).objectReferenceValue == null ) {
				MaterialsProp.GetArrayElementAtIndex ( 0 ).objectReferenceValue = Part_MaterialProp.objectReferenceValue ;
			}
			Part_MaterialProp.objectReferenceValue = null ;
		}
		for ( int i = 0 ; i < Materials_NumProp.intValue ; i++ ) {
			MaterialsProp.GetArrayElementAtIndex ( i ).objectReferenceValue = EditorGUILayout.ObjectField ( "Material" , MaterialsProp.GetArrayElementAtIndex ( i ).objectReferenceValue , typeof ( Material ) , false ) ;
		}

		// Position settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Position settings", MessageType.None, true );
		EditorGUILayout.Slider ( Offset_XProp , -5.0f , 5.0f , "Offset X" ) ;
		EditorGUILayout.Slider ( Offset_YProp , -5.0f , 5.0f , "Offset Y" ) ;
		EditorGUILayout.Slider ( Offset_ZProp , -10.0f , 10.0f , "Offset Z" ) ;

		// Collider settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox ( "Collider settings", MessageType.None , true ) ;
		EditorGUILayout.HelpBox ( "Convex option is enabled forcibly. (for adapting to Unity5)", MessageType.Warning , true ) ;
		Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ( "MeshCollider" , Collider_MeshProp.objectReferenceValue , typeof ( Mesh ) , false ) ;
		Sub_Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Sub MeshCollider" , Sub_Collider_MeshProp.objectReferenceValue , typeof ( Mesh ) , false ) ;

		// Damage settings
		EditorGUILayout.Space () ; EditorGUILayout.Space () ;
		EditorGUILayout.HelpBox( "Damage settings", MessageType.None, true ) ;
		//  Coming off
		Coming_OffProp.boolValue = EditorGUILayout.Toggle ( "Coming off" , Coming_OffProp.boolValue ) ;
		// Mass
		if ( Coming_OffProp.boolValue ) {
			EditorGUILayout.Slider ( Turret_MassProp , 1.0f , 10000.0f , "Turret Mass" ) ;
		}
		EditorGUILayout.Space () ;
		// Durability
		Infinite_DurabilityProp.boolValue = EditorGUILayout.Toggle ( "Infinite Durability" , Infinite_DurabilityProp.boolValue ) ;
		if ( !Infinite_DurabilityProp.boolValue ) {
			EditorGUILayout.Slider ( DurabilityProp , 1.0f , 1000000.0f , "Durability" ) ;
		}
		EditorGUILayout.Slider ( Sub_DurabilityProp , 1.0f , 1000000.0f , "Sub Durability" ) ;
		// Effect
		EditorGUILayout.Slider ( Trouble_TimeProp , 0.0f , 120.0f , "Trouble Time" ) ;
		Trouble_Effect_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Trouble Effect Prefab" , Trouble_Effect_ObjectProp.objectReferenceValue , typeof ( GameObject ) , true ) ;
		Damage_Effect_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ( "Damage Effect Prefab" , Damage_Effect_ObjectProp.objectReferenceValue , typeof ( GameObject ) , true ) ;

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
		// Check for part's children. 
		GameObject Temp_Object ;
		int Child_Count = 0 ;
		if ( Parent_Transform.Find ( "Turret" ) ) {
			Temp_Object = Parent_Transform.Find ( "Turret" ).gameObject ;
			Child_Count = Temp_Object.transform.childCount ;
		} else {
			Temp_Object = null ;
		}
		// Change children's hierarchy, and delete old part. 
		GameObject[] Children= new GameObject[ Child_Count ] ;
		if ( Child_Count > 0 ) {
			for ( int i = 0 ;  i < Child_Count ; i++ ) {
				Children[ i ] = Temp_Object.transform.GetChild ( 0 ).gameObject ;
				Children[ i ].transform.parent = Parent_Transform ;
			}
		}
		if ( Temp_Object ) {
			DestroyImmediate ( Temp_Object ) ;
		}
		// Create new Gameobject & Set Transform.
		GameObject Part_Object = new GameObject ( "Turret" ) ;
		Part_Object.transform.parent = Parent_Transform ;
		Part_Object.transform.localPosition = -Parent_Transform.localPosition + new Vector3 ( Offset_XProp.floatValue , Offset_YProp.floatValue , Offset_ZProp.floatValue ) ;
		Part_Object.transform.localRotation = Quaternion.identity ;
		// Add components
		Part_Object.AddComponent < MeshRenderer > () ;
		Material [] Temp_Materials = new Material [ Materials_NumProp.intValue ] ;
		for ( int i = 0 ; i < Temp_Materials.Length ; i++ ) {
			Temp_Materials [ i ] = MaterialsProp.GetArrayElementAtIndex ( i ).objectReferenceValue as Material ;
		}
		Part_Object.GetComponent<Renderer>().materials = Temp_Materials ;
		MeshFilter Temp_MeshFilter = Part_Object.AddComponent < MeshFilter > () ;
		Temp_MeshFilter.mesh = Part_MeshProp.objectReferenceValue as Mesh ;
		if ( Collider_MeshProp.objectReferenceValue ) {
			MeshCollider Temp_MeshCollider = Part_Object.AddComponent < MeshCollider > () ;
			Temp_MeshCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh ;
			Temp_MeshCollider.convex = true ;
		}
		if ( Sub_Collider_MeshProp.objectReferenceValue ) {
			MeshCollider Temp_MeshCollider = Part_Object.AddComponent < MeshCollider > () ;
			Temp_MeshCollider.sharedMesh = Sub_Collider_MeshProp.objectReferenceValue as Mesh ;
			Temp_MeshCollider.convex = true ;
		}
		// Add script
		Damage_Control_CS Temp_Damage_Control_CS = Part_Object.AddComponent < Damage_Control_CS > () ;
		Temp_Damage_Control_CS.Type = 2 ;  // 2 = Turret in Damage_Control_CS
		Temp_Damage_Control_CS.Coming_Off = Coming_OffProp.boolValue ;
		if ( Coming_OffProp.boolValue ) {
			Temp_Damage_Control_CS.Mass = Turret_MassProp.floatValue ;
		}
		if ( Infinite_DurabilityProp.boolValue ) {
			Temp_Damage_Control_CS.Durability = Mathf.Infinity ;
		} else {
			Temp_Damage_Control_CS.Durability = DurabilityProp.floatValue ;
		} 
		Temp_Damage_Control_CS.Sub_Durability = Sub_DurabilityProp.floatValue ;
		Temp_Damage_Control_CS.Trouble_Time = Trouble_TimeProp.floatValue ;
		Temp_Damage_Control_CS.Damage_Effect_Object = Damage_Effect_ObjectProp.objectReferenceValue as GameObject ;
		Temp_Damage_Control_CS.Trouble_Effect_Object = Trouble_Effect_ObjectProp.objectReferenceValue as GameObject ;
		// Set Layer
		Part_Object.layer = 0 ;
		// Return its children.
		if ( Child_Count > 0 ) {
			for ( int i = 0 ;  i < Child_Count ; i++ ) {
				Children[ i ].transform.parent = Part_Object.transform ;
			}
		}
	}
}
