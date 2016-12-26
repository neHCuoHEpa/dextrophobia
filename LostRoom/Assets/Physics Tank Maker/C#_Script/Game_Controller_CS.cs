using UnityEngine;
using System.Collections;

public class Game_Controller_CS : MonoBehaviour {

	public int Max_Friendly_Num = 10 ;
	public int Max_Hostile_Num = 10 ;
	public float Assign_Frequency = 3.0f ;
	
	public bool Easy_Culling_Flag = true ;
	public float Ray_Distance = 1000.0f ;
	public float Ignore_Distance = 50.0f ;

	public bool Carving_Flag = true ;

	public float Time_Scale = 1.0f ;
	public float Gravity = -9.81f ;
	public float Fixed_TimeStep = 0.02f ;

	Transform [] Operable_Tanks ;
	Transform [] Not_Operable_Tanks ;
	Transform [] Friendly_Body_Transforms ;
	Transform [] Hostile_Body_Transforms ;
	MainBody_Setting_CS [] Friendly_Body_Scripts ;
	MainBody_Setting_CS [] Hostile_Body_Scripts ;
	AI_CS [] Friendly_AI_Scripts ;
	AI_CS [] Hostile_AI_Scripts ;

	float Assign_Count ;

	void Awake () {
		Operable_Tanks = new Transform [ Max_Friendly_Num + Max_Hostile_Num + 1 ] ; // Operable_Tanks [ 0 ] is reserved.
		Not_Operable_Tanks = new Transform [ Max_Friendly_Num + Max_Hostile_Num ] ;
		Friendly_Body_Transforms = new Transform [ Max_Friendly_Num ] ;
		Hostile_Body_Transforms = new Transform [ Max_Hostile_Num ] ;
		Friendly_Body_Scripts = new MainBody_Setting_CS [ Max_Friendly_Num ] ;
		Hostile_Body_Scripts = new MainBody_Setting_CS [ Max_Hostile_Num ] ;
		Friendly_AI_Scripts = new AI_CS [ Max_Friendly_Num ] ;
		Hostile_AI_Scripts = new AI_CS [ Max_Hostile_Num ] ;
		this.gameObject.name = "Game_Controller" ; // This name is referred to by "Tank_ID_Control".
		this.tag = "GameController" ;
		// Modify the Physics and Time manager.
		Time.timeScale = Time_Scale ;
		Physics.gravity = new Vector3 ( 0.0f , Gravity , 0.0f ) ;
		Time.fixedDeltaTime = Fixed_TimeStep ;
	}

	public int Receive_ID ( int Temp_ID , Transform Temp_Transform ) { // Called from "Tank_ID_Control" at the opening.
		// In case of "ID = 0" (Not operable).
		if ( Temp_ID == 0 ) {
			for ( int i = 0 ; i < Not_Operable_Tanks.Length ; i++ ) {
				if ( Not_Operable_Tanks [ i ] == null ) {
					Not_Operable_Tanks [ i ] = Temp_Transform ;
					Store_Components ( Temp_Transform ) ;
					return 0 ;
				}
			}
			return 0 ;
		}
		// In case of operable.
		if ( Operable_Tanks [ Temp_ID ] == null ) { // Operable_Tanks[ # ] is empty.
			Operable_Tanks [ Temp_ID ] = Temp_Transform ;
			Store_Components ( Temp_Transform ) ; // Store MainBodies and AI scripts.
			return Temp_ID ;
		} else { // Operable_Tanks[ # ] is not empty.
			for ( int i = 1 ; i < Operable_Tanks.Length ; i++ ) { // Search empty ID number.
				if ( Operable_Tanks [ i ] == null ) {
					Operable_Tanks [ i ] = Temp_Transform ;
					Store_Components ( Temp_Transform ) ; // Store MainBodies and AI scripts.
					return i ;
				}
			}
		}
		return 0 ; // 'Operable_Tanks' is full.
	}

	void Store_Components ( Transform Temp_Transform ) { // Store MainBody's transform and sctipt ,and AI scripts.
		Transform Temp_MainBody = Temp_Transform.Find ( "MainBody" ) ;
		if ( Temp_MainBody ) {
			if ( Temp_Transform.tag == "Player" ) { // Friendly tank.
				for ( int i = 0 ; i < Friendly_Body_Transforms.Length ; i++ ) { // Search empty.
					if ( Friendly_Body_Transforms [ i ] == null ) {
						// Store components.
						Friendly_Body_Transforms [ i ] = Temp_MainBody ;
						MainBody_Setting_CS Temp_Body_Script = Temp_MainBody.GetComponent < MainBody_Setting_CS > () ;
						if ( Temp_Body_Script ) {
							Friendly_Body_Scripts [ i ] = Temp_Body_Script ;
						}
						AI_CS Temp_AI_Script = Temp_MainBody.GetComponentInChildren < AI_CS > () ;
						if ( Temp_AI_Script ) {
							Friendly_AI_Scripts [ i ] = Temp_AI_Script ;
						}
						break ;
					}
				}
			} else { // Hostile tank.
				for ( int i = 0 ; i < Hostile_Body_Transforms.Length ; i++ ) { // Search empty.
					if ( Hostile_Body_Transforms [ i ] == null ) {
						// Store components.
						Hostile_Body_Transforms [ i ] = Temp_MainBody ;
						MainBody_Setting_CS Temp_Body_Script = Temp_MainBody.GetComponent < MainBody_Setting_CS > () ;
						if ( Temp_Body_Script ) {
							Hostile_Body_Scripts [ i ] = Temp_Body_Script ;
						}
						AI_CS Temp_AI_Script = Temp_MainBody.GetComponentInChildren < AI_CS > () ;
						if ( Temp_AI_Script ) {
							Hostile_AI_Scripts [ i ] = Temp_AI_Script ;
						}
						break ;
					}
				}
			}
		} else {
			Debug.LogWarning ( "'Game_Controller' cannot find the MainBody of '" + Temp_Transform.name + "'. (Physics Tank Maker)" ) ;
			Debug.LogWarning ( "Make sure the name of MainBody." ) ;
		}
	}
	
	void Update () {
		if ( Assign_Count > Assign_Frequency ) {
				Assign_Count = 0.0f ;
				Assign_Target () ;
			} else {
				Assign_Count += Time.deltaTime ;
			}
		if ( Input.anyKeyDown ) {
			Key_Check () ;
		}
	}

	void Assign_Target () {
		// Assign the target to friendly AI tanks.
		float Min_Distance = 10000.0f ;
		Transform Temp_Target = null ;
		MainBody_Setting_CS Temp_Target_Script = null ;
		for ( int i = 0 ; i < Friendly_Body_Transforms.Length ; i++ ) {
			if ( Friendly_AI_Scripts [ i ] && !Friendly_AI_Scripts [ i ].Fire_Flag ) {
				for ( int j = 0 ; j < Hostile_Body_Transforms.Length ; j++ ) {
					if ( Hostile_Body_Transforms [ j ] && Hostile_Body_Transforms [ j ].root.tag != "Finish" ) {
						float Temp_Distance = Vector3.Distance ( Friendly_Body_Transforms [ i ].position , Hostile_Body_Transforms [ j ].position ) ;
						if ( Temp_Distance < Friendly_AI_Scripts [ i ].Visibility_Radius && Temp_Distance < Min_Distance ) {
							if ( Friendly_AI_Scripts [ i ].RayCast_Check ( Hostile_Body_Transforms [ j ] , Hostile_Body_Scripts [ j ] ) ) {
								Min_Distance = Temp_Distance ;
								Temp_Target = Hostile_Body_Transforms [ j ] ;
								Temp_Target_Script = Hostile_Body_Scripts [ j ] ;
							}
						}
					}
				}
				if ( Temp_Target ) {
					Friendly_AI_Scripts [ i ].Set_Target ( Temp_Target , Temp_Target_Script ) ;
				}
			}
		}
		// Assign the target to hostile AI tanks.
		Min_Distance = 10000.0f ;
		Temp_Target = null ;
		Temp_Target_Script = null ;
		for ( int i = 0 ; i < Hostile_Body_Transforms.Length ; i++ ) {
			if ( Hostile_AI_Scripts [ i ] && !Hostile_AI_Scripts [ i ].Fire_Flag ) {
				for ( int j = 0 ; j < Friendly_Body_Transforms.Length ; j++ ) {
					if ( Friendly_Body_Transforms [ j ] && Friendly_Body_Transforms [ j ].root.tag != "Finish" ) {
						float Temp_Distance = Vector3.Distance ( Hostile_Body_Transforms [ i ].position , Friendly_Body_Transforms [ j ].position ) ;
						if ( Temp_Distance < Hostile_AI_Scripts [ i ].Visibility_Radius && Temp_Distance < Min_Distance ) {
							if ( Hostile_AI_Scripts [ i ].RayCast_Check ( Friendly_Body_Transforms [ j ] , Friendly_Body_Scripts [ j ] ) ) {
								Min_Distance = Temp_Distance ;
								Temp_Target = Friendly_Body_Transforms [ j ] ;
								Temp_Target_Script = Friendly_Body_Scripts [ j ] ;
							}
						}
					}
				}
				if ( Temp_Target ) {
					// Send message to "AI".
					Hostile_AI_Scripts [ i ].Set_Target ( Temp_Target , Temp_Target_Script ) ;
				}
			}
		}
	}

	void Key_Check () {
		if ( Input.GetKeyDown ( KeyCode.Keypad1 ) || Input.GetKeyDown ( "1" ) ) {
			Check_and_Cast_Current_ID ( 1 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad2 ) || Input.GetKeyDown ( "2" ) ) {
			Check_and_Cast_Current_ID ( 2 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad3 ) || Input.GetKeyDown ( "3" ) ) {
			Check_and_Cast_Current_ID ( 3 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad4 ) || Input.GetKeyDown ( "4" ) ) {
			Check_and_Cast_Current_ID ( 4 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad5 ) || Input.GetKeyDown ( "5" ) ) {
			Check_and_Cast_Current_ID ( 5 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad6 ) || Input.GetKeyDown ( "6" ) ) {
			Check_and_Cast_Current_ID ( 6 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad7 ) || Input.GetKeyDown ( "7" ) ) {
			Check_and_Cast_Current_ID ( 7 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad8 ) || Input.GetKeyDown ( "8" ) ) {
			Check_and_Cast_Current_ID ( 8 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad9 ) || Input.GetKeyDown ( "9" ) ) {
			Check_and_Cast_Current_ID ( 9 ) ;
			return ;
		} else if ( Input.GetKeyDown ( KeyCode.Keypad0 ) || Input.GetKeyDown ( "0" ) ) {
			Check_and_Cast_Current_ID ( 10 ) ;
			return ;
		}
	}

	void Check_and_Cast_Current_ID ( int Temp_ID ) {
		if  ( Operable_Tanks.Length > Temp_ID ) { // To avoid overflowing.
			if ( Operable_Tanks [ Temp_ID ] ) { // This ID is used.
				// Broadcast Current_ID to all tanks.
				for ( int i = 0 ; i < Operable_Tanks.Length ; i++ ) {
					if ( Operable_Tanks [ i ] ) {
						Operable_Tanks [ i ].BroadcastMessage ( "Receive_Current_ID" , Temp_ID , SendMessageOptions.DontRequireReceiver ) ;
					}
				}
			}
		}
	}

	public void ReSpawn_ReSetting () { // Called from "Tank_ID_Control" when ReSpawn.
		Assign_Count = 0.0f ;
		// Store MainBodies and AI scripts again.
		Friendly_Body_Transforms = new Transform [ Max_Friendly_Num ] ;
		Hostile_Body_Transforms = new Transform [ Max_Hostile_Num ] ;
		Friendly_AI_Scripts = new AI_CS [ Max_Friendly_Num ] ;
		Hostile_AI_Scripts = new AI_CS [ Max_Hostile_Num ] ;
		for ( int i = 0 ; i < Operable_Tanks.Length ; i++ ) {
			if ( Operable_Tanks [ i ] ) {
				Store_Components ( Operable_Tanks [ i ] ) ;
			}
		}
		for ( int i = 0 ; i < Not_Operable_Tanks.Length ; i++ ) {
			if ( Not_Operable_Tanks [ i ] ) {
				Store_Components ( Not_Operable_Tanks [ i ] ) ;
			}
		}
		// Send message 'Lost_Target' to AI.
		for ( int i = 0 ; i < Friendly_AI_Scripts.Length ; i++ ) {
			if ( Friendly_AI_Scripts [ i ] ) {
				Friendly_AI_Scripts [ i ].Lost_Target () ;
			}
		}
		for ( int i = 0 ; i < Hostile_AI_Scripts.Length ; i++ ) {
			if ( Hostile_AI_Scripts [ i ] ) {
				Hostile_AI_Scripts [ i ].Lost_Target () ;
			}
		}
	}

}
