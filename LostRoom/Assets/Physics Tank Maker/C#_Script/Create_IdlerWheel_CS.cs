﻿using UnityEngine;
using System.Collections;

[ ExecuteInEditMode ]

public class Create_IdlerWheel_CS : MonoBehaviour {

	public bool Arm_Flag = false ;
	public float Arm_Distance = 2.11f ;
	public float Arm_Length = 0.25f ;
	public float Arm_Angle = 110.0f ;
	public Mesh Arm_L_Mesh ;
	public Mesh Arm_R_Mesh ;
	public Material Arm_L_Material ;
	public Material Arm_R_Material ;
	
	public float Wheel_Distance = 2.7f ;
	public float Wheel_Mass = 30.0f ;
	public float Wheel_Radius = 0.35f ;
	public PhysicMaterial Collider_Material ;
	public Mesh Wheel_Mesh ;
	public Material Wheel_Material ;
	public Mesh Collider_Mesh ;
	public Mesh Collider_Mesh_Sub ;
	public bool Convex = false ;
	public bool Drive_Wheel = true ;
	public bool Wheel_Resize = true ;
	public float ScaleDown_Size = 0.7f ;
	public float Return_Speed = 0.05f ;
	public bool Infinite_Durability = false ;
	public float Wheel_Durability = 55000.0f ;
	public bool Static_Flag = false ;

	public Transform Parent_Transform ;
	
	void Start () {
		Parent_Transform = this.transform ;
	}
	
	void Update () {
		if ( transform.localEulerAngles.z != 90.0f ) {
			float Temp_X = transform.localEulerAngles.x ;
			float Temp_Y = transform.localEulerAngles.y ;
			transform.localEulerAngles = new Vector3 ( Temp_X , Temp_Y , 90.0f ) ;
		}
		if ( Application.isPlaying ) {
			Destroy ( this ) ;
		}
	}
	
	void Reset () {
		Start () ;
	}

}