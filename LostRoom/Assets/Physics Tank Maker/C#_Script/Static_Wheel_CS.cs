using UnityEngine;
using System.Collections;

public class Static_Wheel_CS : MonoBehaviour {

	Transform This_Transform ;
	bool Work_Flag = false ;
	bool Direction ; // Left = true.
	float Rate ;
	float Last_Ang ;
	float Pitch ;
	MainBody_Setting_CS MainBody_Script ;
	Static_Track_CS Static_Track_Script ;

	void Awake () {
		This_Transform = transform ;
	}

	void Start () { // Initial settings must be performed in the 'Get_Static_Track'.
	}

	void Get_Static_Track ( Static_Track_CS Temp_Script ) { // Called from "Static_Track".
		Static_Track_Script = Temp_Script ;
		Work_Flag = true ;
		// Set direction.
		if ( This_Transform.localPosition.y > 0.0f ) {
			Direction = true ; // Left
		} else {
			Direction = false ; // Right
		}
		// Set Rate.
		float This_Radius = This_Transform.GetComponent < SphereCollider > ().radius ;
		if ( Direction ) { // Left
			Rate = Static_Track_Script.Reference_Radius_L / This_Radius ;
		} else { // Right
			Rate = Static_Track_Script.Reference_Radius_R / This_Radius ;
		}
	}
	
	void Update () {
		if ( Work_Flag ) {
			if ( MainBody_Script.Visible_Flag ) { // MainBody is visible by any camera.
				if ( Direction ) { // Left
					Pitch = -Static_Track_Script.Diff_Ang_L * Rate ;
				} else { // Right
					Pitch = -Static_Track_Script.Diff_Ang_R * Rate ;
				}
				This_Transform.localEulerAngles = new Vector3 ( This_Transform.localEulerAngles.x , This_Transform.localEulerAngles.y + Pitch , This_Transform.localEulerAngles.z ) ;
			}
		}
	}

	void Get_MainBody_Object ( GameObject Temp_Object ) {
		MainBody_Script = Temp_Object.GetComponent < MainBody_Setting_CS > () ;
	}
	
}
