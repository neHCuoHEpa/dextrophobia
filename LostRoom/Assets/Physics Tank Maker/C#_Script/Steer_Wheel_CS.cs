using UnityEngine;
using System.Collections;

public class Steer_Wheel_CS : MonoBehaviour {

	public float Max_Angle = 35.0f ;
	public float Rotation_Speed = 45.0f ;
	
	float Vertical ;
	float Horizontal ;
	float Current ;

	bool  Flag = true ;
	int Tank_ID ;
	int Input_Type = 4 ;

	AI_CS AI_Script ;

	void Update () {
		if ( Flag ) {
			switch ( Input_Type ) {
			case 0 :
				KeyBoard_Input () ;
				break ;
			case 1 :
				Stick_Input () ;
				break ;
			case 2 :
				Trigger_Input () ;
				break ;
			case 3 :
				Stick_Trigger_Input () ;
				break ;
			case 4 :
				Mouse_Input () ;
				break ;
			case 5 :
				Mouse_Input () ;
				break ;
			case 10 :
				AI_Input () ;
				break ;
			}
		}
	}
	
	void KeyBoard_Input () {
		if  ( Input.GetKey ( "z" ) == false && Input.GetKey ( "c" ) == false ) {
			Base_Input () ;
		}
	}
	
	void Stick_Input () {
		if  ( Input.GetButton ( "Jump" ) == false ) {
			Base_Input () ;
		}
	}
	
	void Trigger_Input () {
		float L_Temp = Input.GetAxis ( "L_Trigger" ) - Input.GetAxis ( "L_Button" ) ;
		float R_Temp = Input.GetAxis ( "R_Trigger" ) - Input.GetAxis ( "R_Button" ) ;
		if ( L_Temp > 0.0f || R_Temp > 0.0f ) {
			Horizontal = L_Temp - R_Temp ;
			if ( Horizontal > 1.0f ) {
				Horizontal = 1.0f ;
			} else if ( Horizontal < -1.0f ) {
				Horizontal = -1.0f ;
			}
			Steer () ;
		} else if ( L_Temp < 0.0f || R_Temp < 0.0f ) {
			Horizontal = R_Temp - L_Temp ;
			if ( Horizontal > 1.0f ) {
				Horizontal = 1.0f ;
			} else if ( Horizontal < -1.0f ) {
				Horizontal = -1.0f ;
			}
			Steer () ;
		} 
	}
	
	void Stick_Trigger_Input () {
		if  ( Input.GetButton ( "Jump" ) == false ) {
			Horizontal = Input.GetAxis ( "Horizontal" ) ;
			if ( Input.GetAxis ( "R_Trigger" ) != 0.0f || Input.GetAxis ( "L_Trigger" ) != 0.0f || Horizontal != 0.0f ) {
				Steer () ;
			}
		}
	}
	
	void Mouse_Input () {
		float Temp_Horizontal = 0.0f ;
		if ( Input.GetKey ( "e" ) ) {
			Temp_Horizontal += 0.3f ;
		} else if ( Input.GetKey ( "d" ) ) {
			Temp_Horizontal += 1.0f ;
		} else if ( Input.GetKey ( "c" ) ) {
			Temp_Horizontal += 1.0f ;
		}
		if ( Input.GetKey ( "q" ) ) {
			Temp_Horizontal -= 0.3f ;
		} else if ( Input.GetKey ( "a" ) ) {
			Temp_Horizontal -= 1.0f ;
		} else if ( Input.GetKey ( "z" ) ) {
			Temp_Horizontal -= 1.0f ;
		}
		Horizontal = Temp_Horizontal ;
		Steer () ;
	}
	
	void Base_Input () {
		Vertical = Input.GetAxis ( "Vertical" ) ;
		Horizontal = Input.GetAxis ( "Horizontal" ) ;
		if ( Vertical != 0.0f  || Horizontal != 0.0f ) {
			Steer () ;
		}
	}

	void AI_Input () {
		Vertical = AI_Script.Speed_Order ;
		Horizontal = AI_Script.Turn_Order ;
		if ( Vertical != 0.0f  || Horizontal != 0.0f ) {
			Steer () ;
		}
	}

	void Steer () {
		float Target = Max_Angle * Horizontal ;
		if ( Mathf.Abs ( Target - Current ) < 0.5f ) {
			Current = Target ;
		} else {
		if ( Current > Target ) {
			Current -= Rotation_Speed * Time.deltaTime ;
		} else if ( Current < Target ) {
			Current += Rotation_Speed * Time.deltaTime ;
		}
		//if ( Mathf.Abs ( Current ) < 0.1f ) {
		//	Current = 0.0f ;
		//}
		}
		JointSpring Temp_Spring = GetComponent<HingeJoint>().spring ;
		Temp_Spring.targetPosition = Current ;
		GetComponent<HingeJoint>().spring = Temp_Spring ;
	}
	
	public void Set_Value ( float Temp_Angle , float Temp_Speed ) {
		Max_Angle = Temp_Angle ;
		Rotation_Speed = Temp_Speed ;
	}
	
	void Set_Input_Type ( int Temp_Input_Type ) {
		Input_Type = Temp_Input_Type ;
	}
	
	void Set_Tank_ID ( int Temp_Tank_ID ) {
		Tank_ID = Temp_Tank_ID ;
	}

	void Receive_Current_ID ( int Temp_Current_ID ) {
		if ( Temp_Current_ID == Tank_ID ) {
			Flag = true ;
		} else {
			if ( Input_Type == 10 ) {
				Flag = true ;
			} else {
				Flag = false ;
			}
		}
	}

	void Get_AI ( AI_CS Temp_Script ) {
		AI_Script =Temp_Script ;
	}

	void MainBody_Linkage () { // Called from MainBody's "Damage_Control".
		Destroy ( this ) ;
	}
	
}