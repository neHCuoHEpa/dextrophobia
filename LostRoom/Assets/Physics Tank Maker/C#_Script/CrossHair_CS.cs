using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof ( GUITexture ) ) ]

public class CrossHair_CS : MonoBehaviour {

	Camera Gun_Camera ;

	int Tank_ID ;

	void Awake () {
		transform.position = new Vector3 ( 0.5f , 0.5f , 0.0f ) ;
		transform.localScale = new Vector3 ( 0.0f , 0.0f , 1.0f ) ;
		GetComponent<GUITexture>().pixelInset = new Rect ( -Screen.width * 0.5f , -Screen.height * 0.5f , Screen.width , Screen.height ) ;
		gameObject.layer = 8 ;
	}

	void Change_Camera_Mode () { // Called from "Gun_Camera".
		Vector2 View_Size = new Vector2 ( Screen.width * Gun_Camera.rect.width , Screen.height * Gun_Camera.rect.height ) ;
		GetComponent<GUITexture>().pixelInset = new Rect ( -View_Size.x * 0.5f , -View_Size.y * 0.5f , View_Size.x , View_Size.y ) ;
	}

	void Get_Gun_Camera ( Camera Temp_Camera ) {
		Gun_Camera = Temp_Camera ;
	}

	void Set_Tank_ID ( int Temp_Tank_ID ) {
		Tank_ID = Temp_Tank_ID ;
	}
	
	void Receive_Current_ID ( int Temp_Current_ID ) {
		if ( Temp_Current_ID == Tank_ID ) {
			GetComponent<GUITexture>().enabled = true ;
			transform.position = new Vector3 ( 0.5f , 0.5f , 0.0f ) ;
		} else {
			GetComponent<GUITexture>().enabled = false ;
		}
	}
}