using UnityEngine;
using System.Collections;

public class Stabilizer_CS : MonoBehaviour {

	public bool Track_Flag ;

	Transform This_Transform ;
	float Default_Pos ;
	Vector3 Default_Ang ;

	bool Direction ;
	float Increased_Mass ;

	void Start () {
		This_Transform = transform ;
		Default_Pos = This_Transform.localPosition.y ;
		Default_Ang = This_Transform.localEulerAngles ;
		// Set Direction for adjusting the tracks.
		if ( Track_Flag ) {
			if ( Default_Pos > 0 ) {
				Direction = true ; // Left
			} else {
				Direction = false ; // Right
			}
		}
	}

	void Update () {
		// Stabilize position.
		float Temp_X = This_Transform.localPosition.x ;
		float Temp_Z = This_Transform.localPosition.z ;
		This_Transform.localPosition = new Vector3 ( Temp_X , Default_Pos , Temp_Z ) ;
		// Stabilize angle.
		float Temp_Y = This_Transform.localEulerAngles.y ;
		This_Transform.localEulerAngles = new Vector3 ( Default_Ang.x , Temp_Y , Default_Ang.z ) ;
	}

	void Adjust_Mass ( bool Temp_Direction ) {
		if ( Track_Flag && GetComponent<Rigidbody>() && Temp_Direction == Direction ) {
			Increased_Mass += 1.0f ;
			GetComponent<Rigidbody>().mass += 1.0f ;
		}
	}

	void Reset_Mass () {
		if ( Track_Flag && GetComponent<Rigidbody>() ) {
			GetComponent<Rigidbody>().mass -= Increased_Mass ;
			Increased_Mass = 0.0f ;
		}
	}
}
