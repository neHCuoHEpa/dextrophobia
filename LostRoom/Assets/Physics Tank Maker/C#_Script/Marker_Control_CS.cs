using UnityEngine;
using System.Collections;

public class Marker_Control_CS : MonoBehaviour {

	// Set from "Turret_Horizontal".
	public Turret_Horizontal_CS Turret_Horizontal_Script ;
	public bool Main_Flag ;

	public Color Normal_Color = new Vector4 ( 1.0f , 1.0f , 1.0f , 0.5f ) ;
	public Color LockOn_Color = new Vector4 ( 1.0f , 0.0f , 0.0f , 0.5f ) ;

	Vector2 Texture_Size ;
	Vector2 Center_Offset ;
	GUITexture This_GUITexture ;

	void Awake () {
		transform.position = Vector3.zero ;
		transform.localScale = new Vector3 ( 0.0f , 0.0f , 1.0f ) ;
		This_GUITexture = GetComponent<GUITexture>() ;
		Texture_Size = new Vector2 ( This_GUITexture.texture.width , This_GUITexture.texture.height ) ;
		Center_Offset = Texture_Size * 0.5f ;
	}

	void LateUpdate () {
		if ( Turret_Horizontal_Script.Marker_Flag ) {
			if ( Turret_Horizontal_Script.Free_Aim_Flag ) { // Free Aiming.
				This_GUITexture.color = new Vector4 ( 1.0f , 1.0f , 1.0f , 0.5f ) ;
				if ( Main_Flag ) { // Main marker
					This_GUITexture.enabled = true ;
					Track_Cursor () ;
				} else { // Sub Marker
					This_GUITexture.enabled = false ;
				}
			} else { // Lock On.
				This_GUITexture.color = new Vector4 ( 1.0f , 0.0f , 0.0f , 0.5f ) ;
				if ( Camera.allCamerasCount == 1 ) { // There is only one camera window.
					if ( Main_Flag ) { // Main marker
						Move_Marker ( Camera.main ) ;
					} else { // Sub Marker
						This_GUITexture.enabled = false ;
					}
				} else { // There are two camera windows.
					Camera[] Temp_Cameras = Camera.allCameras ;
					if ( Main_Flag ) { // Main marker
						Move_Marker ( Temp_Cameras[ 0 ] ) ;
					} else { // Sub Marker
						Move_Marker ( Temp_Cameras[ 1 ] ) ;
					}
				}
			}
		} else {
			This_GUITexture.enabled = false ;
		}
	}

	void Move_Marker ( Camera Temp_Camera ) {
		Vector3 Temp_Pos = Temp_Camera.WorldToScreenPoint ( Turret_Horizontal_Script.Target_Pos ) ;
		if ( Temp_Pos.z < 0.0f ) {
			This_GUITexture.enabled = false ;
		} else {
			if ( Main_Flag ) { // Main marker
				This_GUITexture.enabled = true ;
				This_GUITexture.pixelInset = new Rect ( Temp_Pos.x - Center_Offset.x , Temp_Pos.y - Center_Offset.y , Texture_Size.x , Texture_Size.y ) ;
			} else { // Sub Marker
				if ( new Rect ( 0 , 0 , Screen.width * Temp_Camera.rect.width , Screen.height * Temp_Camera.rect.height ).Contains ( new Vector2 ( Temp_Pos.x , Temp_Pos.y ) ) ) {
					This_GUITexture.enabled = true ;
					This_GUITexture.pixelInset = new Rect ( Temp_Pos.x - Center_Offset.x , Temp_Pos.y - Center_Offset.y , Texture_Size.x , Texture_Size.y ) ;
				} else {
					This_GUITexture.enabled = false ;
				}
			}
		}
	}

	void Track_Cursor () {
		Vector2 Temp_Pos = Input.mousePosition ;
		This_GUITexture.pixelInset = new Rect ( Temp_Pos.x - Center_Offset.x , Temp_Pos.y - Center_Offset.y , Texture_Size.x , Texture_Size.y ) ;
	}

}
