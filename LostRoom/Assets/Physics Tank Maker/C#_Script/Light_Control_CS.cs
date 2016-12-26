using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof ( Light ) ) ]

public class Light_Control_CS : MonoBehaviour {

	int Tank_ID ;
	
	void Awake () {
		GetComponent<Light>().enabled = false ;
	}

	void Set_Tank_ID ( int Temp_Tank_ID ) {
		Tank_ID = Temp_Tank_ID ;
	}
	
	void Receive_Current_ID ( int Temp_Current_ID ) {
		if ( Temp_Current_ID == Tank_ID ) {
			GetComponent<Light>().enabled = true ;
		} else {
			GetComponent<Light>().enabled = false ;
		}
	}
}