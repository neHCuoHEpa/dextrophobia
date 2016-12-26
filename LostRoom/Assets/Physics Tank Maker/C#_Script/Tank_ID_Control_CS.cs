using UnityEngine;
using System.Collections;

public class Tank_ID_Control_CS: MonoBehaviour {

	public int Tank_ID = 1 ;
	public int Relationship ;
	public int Input_Type = 4 ;
	public int Turn_Type = 0 ;
	public bool Auto_ReSpawn_Flag = false ;
	public float ReSpawn_Time = 10.0f ;

	// 'public' is needed for Editor script.
	public bool ReSpawn_Flag ;
	public string Prefab_Path ;
	
	// Stored values set and used by AI.
	public GUIText Message_GUIText ;
	public Color Default_Text_Color ;
	public Vector3 [] WayPoints ;
	public Transform Follow_Target ;
	
	bool Flag = true ; 
	int Current_ID = 1 ;

	Game_Controller_CS Game_Controller_Script ;


	void Awake () {
		Set_Tag () ; // Set Tag.
	}

	void Set_Tag () { // This function is called also from 'ReSpawn ()'.
		if ( Relationship == 0 ) { // 0 = Friendly.
			this.tag = "Player" ;
		} else { // 1 = Hostile.
			this.tag = "Untagged" ;
		}
	}
	
	void Start () {
		// Find Game_Controller.
		GameObject Temp_Object = GameObject.Find ( "Game_Controller" ) ;
		// Send this Transform, and Get Tank_ID.
		if ( Temp_Object ) {
			Game_Controller_Script = Temp_Object.GetComponent < Game_Controller_CS > () ;
			if ( Game_Controller_Script ) {
				Tank_ID = Game_Controller_Script.Receive_ID ( Tank_ID , this.transform ) ;
			}
		}
		// Send settings to all parts.
		Send_Settings () ;
	}

	void Send_Settings () { // This function is called also from 'ReSpawn'.
		// Broadcast Input_Type.
		BroadcastMessage ( "Set_Input_Type" , Input_Type , SendMessageOptions.DontRequireReceiver ) ;
		if ( Input_Type == 0 || Input_Type == 1 || Input_Type == 5 ) {
			BroadcastMessage ( "Set_Turn_Type" , Turn_Type , SendMessageOptions.DontRequireReceiver ) ;
		}
		// Broadcast Tank_ID and Current_ID.
		BroadcastMessage ( "Set_Tank_ID" , Tank_ID , SendMessageOptions.DontRequireReceiver ) ;
		BroadcastMessage ( "Receive_Current_ID" , Current_ID , SendMessageOptions.DontRequireReceiver ) ;
		//
		if ( Game_Controller_Script ) {
			// Culling settings in "MainBody_Setting".
			MainBody_Setting_CS MainBody_Setting_Script = GetComponentInChildren < MainBody_Setting_CS > () ;
			if ( MainBody_Setting_Script ) {
				MainBody_Setting_Script.Easy_Culling_Flag = Game_Controller_Script.Easy_Culling_Flag ;
				MainBody_Setting_Script.Ray_Distance = Game_Controller_Script.Ray_Distance ;
				MainBody_Setting_Script.Ignore_Distance = Game_Controller_Script.Ignore_Distance ;
			}
			// Obstacle Craving setting in "Damage_Control" in MainBody.
			Damage_Control_CS Damage_Control_Script = MainBody_Setting_Script.GetComponent < Damage_Control_CS > () ;
			if ( Damage_Control_Script ) {
				Damage_Control_Script.Carving_Flag = Game_Controller_Script.Carving_Flag ;
			}
			// Obstacle Craving setting in "AI_Hand".
			AI_Hand_CS AI_Hand_Script = GetComponentInChildren < AI_Hand_CS > () ;
			if ( AI_Hand_Script ) {
				AI_Hand_Script.Carving_Flag = Game_Controller_Script.Carving_Flag ;
			}
		}
	}

	void Update () {
		if ( Flag ) {
			if ( Input.GetKeyDown ( KeyCode.Backspace ) ) {
				Application.LoadLevel ( Application.loadedLevelName ) ; // ReLoad this scene.
			} else if ( Input.GetKeyDown ( KeyCode.Return ) ) {
				if ( ReSpawn_Flag ) {
					ReSpawn () ; // ReSpawn.
				}
			}
		}
	}

	void ReSpawn () {
		// Make sure that the prefab exists.
		GameObject Check_Object = Resources.Load ( Prefab_Path ) as GameObject ;
		if ( Check_Object == null ) {
			ReSpawn_Flag = false ;
			return ;
		}
		// This object is continuously used even when a new tank is spawned.
		// Destroy child parts.
		int Temp_Num = transform.childCount ;
		for ( int i = 0 ;  i  < Temp_Num ; i++ ) {
			DestroyImmediate ( transform.GetChild ( 0 ).gameObject ) ;
		}
		if ( transform.childCount == 0 ) { // Destroying succeeded.
			// Set this Tag again.
			Set_Tag () ;
			// Instantiate the new tank with reference to the Prefab_Path from 'Resources' folder.
			GameObject Temp_Object = Instantiate ( Resources.Load ( Prefab_Path ) , transform.position , transform.rotation ) as GameObject ;
			// Change the hierarchy of the new tank.
			Temp_Num = Temp_Object.transform.childCount ;
			for ( int i = 0 ;  i  < Temp_Num ; i++ ) {
				Temp_Object.transform.GetChild ( 0 ).parent = this.transform ; // New child objects are moved under this object as its children.
			}
			// Destroy the top object of the new tank.
			DestroyImmediate ( Temp_Object ) ;
			// Broadcast settings to new children.
			Send_Settings () ;
			// Reset the stored components in the "Game_Controller".
			Game_Controller_Script.ReSpawn_ReSetting () ;
		}
	}

	void MainBody_Linkage () {
		this.tag = "Finish" ;
		if ( Auto_ReSpawn_Flag && ReSpawn_Flag ) {
			StartCoroutine ( "ReSpawn_Timer" ) ;
		}
	}

	IEnumerator ReSpawn_Timer () {
		yield return new WaitForSeconds ( ReSpawn_Time ) ;
		yield return new WaitForEndOfFrame () ;
		ReSpawn () ;
	}

	void Receive_Current_ID ( int Temp_Current_ID ) {
		Current_ID = Temp_Current_ID ;
		if ( Temp_Current_ID == Tank_ID ) {
			Flag = true ;
		} else {
			Flag = false ;
		}
	}

}
