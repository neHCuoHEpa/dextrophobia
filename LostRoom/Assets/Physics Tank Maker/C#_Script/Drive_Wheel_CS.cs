using UnityEngine;
using System.Collections;

public class Drive_Wheel_CS : MonoBehaviour {
	
	public bool Drive_Flag = true ;
	public float Radius = 0.3f ;
	
	public float MaxAngVelocity ;
	public float Turn_Brake_Drag ;

	bool Turn_Brake_Flag ;

	bool Direction ;
	float Speed_Rate ;
	
	Rigidbody MainBody_Rigidbody ;
	Rigidbody This_Rigidbody ;

	bool Flag = true ;
	int Tank_ID ;
	int Input_Type = 4 ;
	
	Drive_Control_CS Control_Script ;
	
	void Start () {
		This_Rigidbody = GetComponent<Rigidbody>() ;
		// Set direction.
		if ( transform.localEulerAngles.z == 0.0f )	{
			Direction = true ; // Left
		} else {
			Direction = false ; // Right
		}
		// Set Turn_Brake_Flag.
		if ( GetComponentInParent < Steer_Wheel_CS > () ) { // Steered Wheel.
			Turn_Brake_Flag = false ;
		} else {
			Turn_Brake_Flag = true ;
		}
	}

	void FixedUpdate () {
		// Parking Brake.
		if ( Control_Script.Parking_Brake ) {
			if ( MainBody_Rigidbody.velocity.magnitude < 1.0f ) {
				This_Rigidbody.angularDrag = Mathf.Infinity ;
				return ;
			}
		}
		// Drive.
		if ( Control_Script.Acceleration_Flag ) { // Acceleration mode.
			Acceleration_Mode () ;
		} else { // Constant mode.
			Constant_Mode () ;
		}
	}

	void Acceleration_Mode () {
		if ( Direction ) { // Left
			// Set Max Angular Velocity.
			This_Rigidbody.maxAngularVelocity = MaxAngVelocity * Control_Script.L_Speed_Rate ;
			if ( Flag ) {
				// Set Angular Drag.
				if ( Turn_Brake_Flag ) {
					This_Rigidbody.angularDrag = Control_Script.L_Brake * Turn_Brake_Drag ;
				} else {
					This_Rigidbody.angularDrag = 0.05f ;
				}
				// Add Torque.
				if ( Drive_Flag ) {
					if ( Control_Script.L_Forward_Flag ) { // Forward.
						This_Rigidbody.AddRelativeTorque ( 0.0f , -Control_Script.Torque , 0.0f ) ;
					} else { // Backward.
						This_Rigidbody.AddRelativeTorque ( 0.0f , Control_Script.Torque , 0.0f ) ;
					}
				}
			}
		} else { // Right
			// Set Max Angular Velocity.
			This_Rigidbody.maxAngularVelocity = MaxAngVelocity * Control_Script.R_Speed_Rate ;
			if ( Flag ) {
				// Set Angular Drag.
				if ( Turn_Brake_Flag ) {
					This_Rigidbody.angularDrag = Control_Script.R_Brake * Turn_Brake_Drag ;
				} else {
					This_Rigidbody.angularDrag = 0.05f ;
				}
				// Add Torque.
				if ( Drive_Flag ) {
					if ( Control_Script.R_Forward_Flag ) { // Forward.
						This_Rigidbody.AddRelativeTorque ( 0.0f , Control_Script.Torque , 0.0f ) ;
					} else { // Backward.
						This_Rigidbody.AddRelativeTorque ( 0.0f , -Control_Script.Torque , 0.0f ) ;
					}
				}
			}
		}
	}

	void Constant_Mode () {
		if ( Direction ) { // Left
			// Set Max Angular Velocity.
			This_Rigidbody.maxAngularVelocity = Mathf.Abs ( MaxAngVelocity * Control_Script.L_Temp ) ;
			if ( Flag && Drive_Flag ) {
				// Set Angular Drag.
				This_Rigidbody.angularDrag = Control_Script.L_Brake * Turn_Brake_Drag ;
				// Add Torque.
				if ( Control_Script.L_Temp > 0 ) {
					This_Rigidbody.AddRelativeTorque ( 0.0f , Control_Script.Torque , 0.0f ) ;
				} else if ( Control_Script.L_Temp < 0) {
					This_Rigidbody.AddRelativeTorque ( 0.0f , -Control_Script.Torque , 0.0f ) ;
				}
			}
		} else { // Right
			// Set Max Angular Velocity.
			This_Rigidbody.maxAngularVelocity = Mathf.Abs ( MaxAngVelocity * Control_Script.R_Temp ) ;
			if ( Flag && Drive_Flag ) {
				// Set Angular Drag.
				This_Rigidbody.angularDrag = Control_Script.R_Brake * Turn_Brake_Drag ;
				if ( Control_Script.R_Temp > 0 ) {
					This_Rigidbody.AddRelativeTorque ( 0.0f , Control_Script.Torque , 0.0f ) ;
				} else if ( Control_Script.R_Temp < 0) {
					This_Rigidbody.AddRelativeTorque ( 0.0f , -Control_Script.Torque , 0.0f ) ;
				}
			}
		}
	}
	
	void Set_Input_Type ( int Temp_Input_Type ) {
		Input_Type = Temp_Input_Type ;
	}
	
	void Get_MainBody_Object ( GameObject Temp_Object ) {
		MainBody_Rigidbody = Temp_Object.GetComponent<Rigidbody>() ;
	}
	
	void Set_Tank_ID ( int Temp_Tank_ID ) {
		Tank_ID = Temp_Tank_ID ;
	}
	
	void Receive_Current_ID ( int Temp_Current_ID ) {
		if ( Temp_Current_ID == Tank_ID ) {
			Flag = true ;
		} else {
			if ( Input_Type == 4 || Input_Type == 10 ) {
				Flag = true ;
			} else {
				Flag = false ;
			}
		}
	}
	
	public void Set_Value ( float Radius_Value , bool Drive_Value ) {
		Radius = Radius_Value ;
		Drive_Flag = Drive_Value ;
	}
	
	void Get_Drive_Control ( Drive_Control_CS Temp_Script ) {
		Control_Script = Temp_Script ;
		MaxAngVelocity = Mathf.Deg2Rad * ( ( Temp_Script.Max_Speed / ( 2.0f * Radius * 3.14f ) ) * 360.0f ) ;
		Turn_Brake_Drag = Temp_Script.Turn_Brake_Drag ;
	}
	
}
