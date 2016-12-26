using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof ( Camera ) ) ]
[ RequireComponent ( typeof ( AudioListener ) ) ]

public class Camera_Distance_CS : MonoBehaviour {
	public float FPV_FOV = 50.0f ;
	public float TPV_FOV = 30.0f ;

	Transform This_Transform ;
	Transform Parent_Transform ;
	
	Camera This_Camera ;
	AudioListener This_AudioListener ;
	bool Camera_Flag = true ;
	bool AudioListener_Flag = true ;
	bool  TPV_Flag = true ;
	
	bool  Flag = true ;
	int Tank_ID ;
	int Input_Type = 4 ;
	
	void Awake () {
		this.tag = "MainCamera" ;
		This_Transform = transform ;
		This_Camera = GetComponent < Camera > () ;
		This_Camera.enabled = true ;
		This_Camera.depth = 0 ;
		This_Camera.cullingMask = ~ ( 1 << 8 ) ; // Ignore CrossHair.
		This_Camera.nearClipPlane = 0.01f ;
		This_Camera.fieldOfView = TPV_FOV ;
		This_AudioListener = GetComponent < AudioListener > () ;
		This_AudioListener.enabled = true ;
		AudioListener.volume = 1.0f ;
	} 
	
	void Start () {
		Parent_Transform = This_Transform.parent ;
		This_Transform.LookAt ( Parent_Transform ) ;
		// Send message to the "Turret_Horizontal".
		transform.root.BroadcastMessage ( "Get_Main_Camera" , This_Camera , SendMessageOptions.DontRequireReceiver ) ;
	}
	
	void Update () {
		if ( Flag ) {
			if ( Camera_Flag ) {
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
					Stick_Input () ;
					break ;
				case 4 :
					Mouse_Input () ;
					break ;
				case 5 :
					Mouse_Input () ;
					break ;
				case 10 :
					Mouse_Input () ;
					break ;
				}
			}
		}
	}
	
	void KeyBoard_Input () {
		if ( Input.GetKey ( "e" ) ) {
			Forward ( 0.5f ) ;
		} else if ( Input.GetKey ( "r" ) ) {
			Backward ( 0.5f ) ;
		}
	}
	
	void Stick_Input () {
		if ( Input.GetButton ( "Fire3" ) ) {
			Forward ( 0.5f ) ;
		} else if ( Input.GetButton ( "Fire1" ) ) {
			Backward ( 0.5f ) ;
		}
	}
	
	void Trigger_Input () {
		if ( Input.GetButton ( "Fire1" ) && Input.GetAxis ( "Vertical" ) > 0 ) {
			Forward ( 0.5f ) ;
		} else if ( Input.GetButton ( "Fire1" ) && Input.GetAxis ( "Vertical" ) < 0 ) {
			Backward ( 0.5f ) ;
		}
	}
	
	void Mouse_Input () {
		if ( Input.GetMouseButton ( 1 ) ) {
			if ( Input.GetAxis ( "Mouse ScrollWheel" ) > 0 ) {
				Forward ( 3.0f ) ;
			} else if ( Input.GetAxis ( "Mouse ScrollWheel" ) < 0 ) {
				Backward ( 3.0f ) ;
			}
		}
	}
	
	void Forward ( float Temp_Speed ) {
		if ( Vector3.Distance ( This_Transform.position , Parent_Transform.position ) > 1.0f ) {
			This_Transform.position += This_Transform.forward * Temp_Speed ;
		} else if ( TPV_Flag ) {
			TPV_Flag = false ;
			Parent_Transform.SendMessage ( "Switch_View" , TPV_Flag , SendMessageOptions.DontRequireReceiver ) ;
			This_Transform.localPosition = Vector3.zero ;
			This_Camera.fieldOfView = FPV_FOV ;
		}
	}
	
	void Backward ( float Temp_Speed ) {
		if ( Vector3.Distance ( This_Transform.position , Parent_Transform.position ) < 50.0f ) {
			This_Transform.position -= This_Transform.forward * Temp_Speed ;
			if ( TPV_Flag == false ) {
				TPV_Flag = true ;
				Parent_Transform.SendMessage ( "Switch_View" , TPV_Flag , SendMessageOptions.DontRequireReceiver ) ;
				This_Transform.position -= This_Transform.forward * 3.0f ;
				This_Camera.fieldOfView = TPV_FOV ;
			}
		}
	}

	void Change_Camera_Mode ( int Mode ) { // Called from "Gun_Camera".
		if ( Mode == 2 ) { // Gun Camera is full screen.
			Camera_Flag = false ;
			AudioListener_Flag = false ;
		} else {
			Camera_Flag = true ;
			AudioListener_Flag = true ;
		}
		if ( Flag ) { // This function is called also when the turret is destroyed.
			This_Camera.enabled = Camera_Flag ;
			This_AudioListener.enabled = AudioListener_Flag ;
		}
	}

	void  Set_Input_Type (  int Temp_Input_Type   ){
		Input_Type = Temp_Input_Type ;
	}
	
	void  Set_Tank_ID (  int Temp_Tank_ID   ){
		Tank_ID = Temp_Tank_ID ;
	}
	
	void  Receive_Current_ID (  int Temp_Current_ID   ){
		if ( Temp_Current_ID == Tank_ID ) {
			Flag = true ;
			This_Camera.enabled = Camera_Flag ;
			This_AudioListener.enabled = AudioListener_Flag ;
		} else {
			Flag = false ;
			This_Camera.enabled = false ;
			This_AudioListener.enabled = false ;
		}
	}
}