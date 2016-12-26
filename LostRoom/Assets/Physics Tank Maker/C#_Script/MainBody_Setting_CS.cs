using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof ( MeshFilter ) ) ]
[ RequireComponent ( typeof ( MeshRenderer ) ) ]
[ RequireComponent ( typeof ( Rigidbody ) ) ]

public class MainBody_Setting_CS : MonoBehaviour {

	public float Body_Mass = 2000.0f ;
	public Mesh Body_Mesh ;

	public int Materials_Num = 1 ;
	public Material[] Materials ;
	public Material Body_Material ;

	public Mesh Collider_Mesh ;
	public bool Convex = true ;
	public Mesh Sub_Collider_Mesh ;
	public bool Sub_Convex = true ;
	public bool Infinite_Durability = false ;
	public float Durability = 150000.0f ;
	public int Turret_Number = 1 ;

	public int SIC = 14 ;
	public bool Soft_Landing_Flag ;
	public float Landing_Drag = 20.0f ;
	public float Landing_Time = 1.5f ;

	public float AI_Upper_Offset = 1.5f ;
	public float AI_Lower_Offset = 0.3f ;
	// Set from Tank_ID_Control at the opening. ( The values are set in "Game_Controller". )
	public bool Easy_Culling_Flag = true ;
	public float Ray_Distance = 1000.0f ;
	public float Ignore_Distance = 50.0f ;

	// Referred to Static_Track.
	public bool Visible_Flag ;
	
	Transform This_Transform ;
	Transform This_Root_Transform ;
	bool Work_Flag ;
	int Ray_LayerMask = ~ ( ( 1 << 10 ) + ( 1 << 2 ) ) ; // Layer 2 = Ignore Ray, Layer 10 = Ignore All.

	void Start () {
		This_Transform = transform ;
		This_Root_Transform = transform.root ;
		// Layer Collision Settings.
		// Layer8 >> for CrossHair Image.
		// Layer9 >> for wheels.
		// Layer10 >> for Suspensions and Track Reinforce.
		// Layer11 >> for MainBody.
		for ( int i =0 ; i <= 11 ; i ++ ) {
			Physics.IgnoreLayerCollision ( 9 , i , false ) ;
			Physics.IgnoreLayerCollision ( 11 , i , false ) ;
		}
		Physics.IgnoreLayerCollision ( 9 , 9 , true ) ; // Wheels ignore each other.
		Physics.IgnoreLayerCollision ( 9 , 11 , true ) ; // Wheels ignore MainBody.
		for ( int i =0 ; i <= 11 ; i ++ ) {
			Physics.IgnoreLayerCollision ( 10 , i , true ) ; // Ignore all.
		}
		// Solver Iteration Count setting.
		GetComponent<Rigidbody>().solverIterations = SIC ;
		// Send Message to "Drive_Wheel", "Static_Wheel", "Static_Track", "Damage_Control".
		BroadcastMessage ( "Get_MainBody_Object" , this.gameObject , SendMessageOptions.DontRequireReceiver ) ;
		// Soft Landing.
		if ( Soft_Landing_Flag ) {
			StartCoroutine ( "Soft_Landing" ) ;
		}
	}

	IEnumerator Soft_Landing () {
		float Default_Drag= GetComponent<Rigidbody>().drag ;
		GetComponent<Rigidbody>().drag = Landing_Drag ;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation ;
		yield return new WaitForSeconds ( Landing_Time ) ;
		GetComponent<Rigidbody>().drag = Default_Drag ;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None ;
	}

	void OnBecameVisible () {
		if ( Easy_Culling_Flag ) {
			Work_Flag = true ;
		} else {
			Work_Flag = false ;
			Visible_Flag = true ;
		}
	}
	
	void OnBecameInvisible () {
		Work_Flag = false ;
		Visible_Flag = false ;
	}

	void Update () {
		if ( Work_Flag ) {
			int Visible_Count = 0 ;
			Camera [] Current_Cameras = Camera.allCameras ;
			for ( int i = 0 ; i < Camera.allCamerasCount ; i ++ ) {
				Vector3 Temp_Pos = Current_Cameras [ i ].transform.position ;
				Ray Temp_Ray = new Ray ( Temp_Pos , ( This_Transform.position + ( This_Transform.up * AI_Upper_Offset ) ) - Temp_Pos ) ;
				RaycastHit Temp_RaycastHit ;
				if ( Physics.Raycast ( Temp_Ray , out Temp_RaycastHit , Ray_Distance , Ray_LayerMask ) ) { // Ray hits anything.
					if ( Temp_RaycastHit.transform.root == This_Root_Transform ) {
						Visible_Count += 1 ;
					} else if ( Temp_RaycastHit.distance < Ignore_Distance ) {
						Visible_Count += 1 ;
					}
				}
			}
			if ( Visible_Count > 0 ) {
				Visible_Flag = true ;
			} else {
				Visible_Flag = false ;
			}
		}
	}
	
}