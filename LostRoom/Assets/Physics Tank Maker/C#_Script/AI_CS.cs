using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof ( NavMeshAgent ) ) ]

public class AI_CS : MonoBehaviour {

	// Set by user.
	public GameObject WayPoint_Pack ;
	public float WayPoint_Radius = 2.0f ;
	public int Patrol_Type = 0 ; // 0 = Order, 1 = Random.
	public Transform Follow_Target ;

	public float Min_Turn_Angle = 2.0f ;
	public float Pivot_Turn_Angle = 60.0f ;
	public float Slow_Turn_Angle = 10.0f ;
	public float Slow_Turn_Rate = 0.5f ;
	public float Min_Turn_Rate = 0.0f ;
	public float Max_Turn_Rate = 1.0f ;
	public float Min_Speed_Rate = 0.25f ;
	public float Max_Speed_Rate = 1.0f ;
	public float Stuck_Count = 3.0f ;
	public bool Pro_Flag = true ;

	public float Visibility_Radius = 300.0f ;
	public float Approach_Distance = 50.0f ;
	public bool Face_Enemy = false ;
	public float Face_Offest_Angle = 0.0f ;
	public bool Direct_Fire = true ;
	public float Lost_Count = 20.0f ;
	public float Fire_Count = 3.0f ;
	public float Fire_Angle = 2.0f ;
	public int Bullet_Type = 0 ; // 0 = AP.. 1 = HE.
	public GUIText Message_GUIText ;

	// Referred to from "Drive_Control".
	public float Speed_Order ;
	public float Turn_Order ;
	// Referred to from "Cannon_Vertical"
	public Vector3 Target_Pos ;
	public Transform Target_Root_Transform ;
	public bool Detect_Flag = false ;
	public bool Near_Flag = false ;
	public float AI_Upper_Offset ;
	public float AI_Lower_Offset ;
	// Controled by "Cannon_Vertical"
	public bool Fire_Flag = false ;
	// Controled by "AI_Hand", and Referred to from "Drive_Control".
	public bool Brake_Flag = false ;
	// Stored and sent by "Tank_ID_Control".
	public Vector3 [] WayPoints ;
	
	Transform This_Transform ;
	Transform Eye_Transform ;
	NavMeshAgent This_Agent ;
	Vector3 Initial_Position ;
	Vector3 Last_Pos ;
	Vector3 Stored_Pos ;
	string Tank_Name ;
	Color Default_Text_Color ;
	Vector3 Toward_Pos ;
	Transform Follow_MainBody ;
	Tank_ID_Control_CS Tank_ID_Control_Script ;

	Transform Target_Transform ;
	float Target_Distance ;
	int Action_Type ;
	float Lost_Time ;
	int Next_Num = -1 ;
	int Step = 1 ;
	float Search_Count ;
	float SetDestination_Count ;
	bool Toward_Flag = false ;
	bool GoBack_Flag = false ;
	bool Follow_Flag = false ;

	int Ray_LayerMask = ~ ( ( 1 << 10 ) + ( 1 << 2 ) ) ; // Layer 2 = Ignore Ray, Layer 10 = Ignore All.

	void Start () {
		This_Transform = transform ;
		// Find "AI_Eye"
		Eye_Transform = This_Transform.Find ( "AI_Eye" ) ; // Do not rename "AI_Eye".
		if ( !Eye_Transform ) {
			Debug.LogError ( "'AI_Eye' can not be found. (Physics Tank Maker" ) ; 
		}
		// NavMeshAgent settings.
		This_Agent = GetComponent < NavMeshAgent > () ;
		//This_Agent.Stop () ; // This AI requires only path information.
		Initial_Position = This_Transform.localPosition ; // for fixing this object on its default position.
		// Initial settings for storing position.
		Last_Pos = This_Transform.position ;
		Stored_Pos = Last_Pos ;
		// Find "Tank_ID_Control_CS" in the root object.
		Tank_ID_Control_Script = GetComponentInParent < Tank_ID_Control_CS > () ;
		// Text settings.
		Tank_Name = This_Transform.root.name ;
		if ( !Tank_ID_Control_Script.Message_GUIText ) { // Message_GUIText is not stored in "Tank_ID_Control".
			if ( Message_GUIText ) {
				Tank_ID_Control_Script.Message_GUIText = Message_GUIText ; // Store Message_GUIText for respawn.
				Tank_ID_Control_Script.Default_Text_Color = Message_GUIText.color ; // Store the default color for respawn.
				Default_Text_Color = Message_GUIText.color ;
				Text_Update ( "Search" , Default_Text_Color ) ;
			}
		} else { // Message_GUIText is stored in "Tank_ID_Control".
			Message_GUIText = Tank_ID_Control_Script.Message_GUIText ;
			Message_GUIText.color = Tank_ID_Control_Script.Default_Text_Color ;
			Default_Text_Color = Message_GUIText.color ;
			Text_Update ( "Search" , Default_Text_Color ) ;
		}
		// WayPoints settings.
		if ( Tank_ID_Control_Script.WayPoints.Length == 0 ) { // WayPoints are not stored in "Tank_ID_Control".
			if ( WayPoint_Pack && WayPoint_Pack.transform.childCount > 1 ) { // WayPoint_Pack has more than two points.
				WayPoints = new Vector3 [ WayPoint_Pack.transform.childCount ] ;
				for ( int i = 0 ; i < WayPoint_Pack.transform.childCount ; i++ ) {
					WayPoints [ i ] = WayPoint_Pack.transform.GetChild ( i ).position ;
				}
			} else if ( WayPoint_Pack && WayPoint_Pack.transform.childCount == 1 ) { // WayPoint_Pack has only one point.
				WayPoints = new Vector3 [ 1 ] ;
				WayPoints [ 0 ] = WayPoint_Pack.transform.GetChild ( 0 ).position ;
				Toward_Pos = WayPoints [ 0 ] + ( This_Transform.forward * 100.0f ) ; // Set the initial direction.
			} else { // WayPoint_Pack is not assigined, or has no point.
				WayPoints = new Vector3 [ 1 ] ;
				WayPoints [ 0 ] = This_Transform.position ; // Set its initial position.
				Toward_Pos = WayPoints [ 0 ] + ( This_Transform.forward * 100.0f ) ; // Set the initial direction.
			}
			Tank_ID_Control_Script.WayPoints = WayPoints ; // Store the WayPoints in "Tank_ID_Control".
		} else { // WayPoint_Pack is stored in "Tank_ID_Control".
			WayPoints = Tank_ID_Control_Script.WayPoints ;
			if ( WayPoints.Length == 1 ) {
				Toward_Pos = WayPoints [ 0 ] + ( This_Transform.forward * 100.0f ) ;
			}
		}
		// Set the first waypoint.
		Update_Next_WayPoint () ;
		// Follow_Mode settings.
		if ( !Tank_ID_Control_Script.Follow_Target ) { // Follow_Target is not stored in "Tank_ID_Control".
			if ( Follow_Target ) {
				Tank_ID_Control_Script.Follow_Target = Follow_Target ; // Store the Follow_Target in "Tank_ID_Control".
			}
		} else { // Follow_Target is stored in "Tank_ID_Control".
			Follow_Target = Tank_ID_Control_Script.Follow_Target ;
		}
		if ( Follow_Target ) {
			Follow_MainBody = Follow_Target.Find ( "MainBody" ) ; // Find the "MainBody" of Follow_Target.
			if ( Follow_MainBody ) {
				Follow_Flag = true ;
			} else {
				Follow_Flag = false ;
			}
		} else {
			Follow_Flag = false ;
		}
		// Send this script to "Turret_Horizontal", "Cannon_Vertical", "Drive_Control", "Steer_Wheel".
		This_Transform.root.BroadcastMessage ( "Get_AI" , this , SendMessageOptions.DontRequireReceiver ) ;
	}
	
	public void Set_Target ( Transform Temp_Transform , MainBody_Setting_CS Temp_Script ) { // Called from "Game_Controller".
		if ( Target_Transform != Temp_Transform ) {
			Lost_Target () ;
			Target_Transform = Temp_Transform ;
			Target_Root_Transform = Target_Transform.root ;
			if ( Temp_Script ) {
				AI_Upper_Offset = Temp_Script.AI_Upper_Offset ;
				AI_Lower_Offset = Temp_Script.AI_Lower_Offset ;
			}
		}
	}

	void Update () {
		// Fix this position and rotation.
		This_Transform.localPosition = Initial_Position ;
		This_Transform.localRotation = Quaternion.identity ;
		// Search the target.
		if ( Target_Transform ) {
			Target_Pos = Target_Transform.position + ( Target_Transform.up * AI_Upper_Offset ) ;
			Search_Count -= Time.deltaTime ;
			if ( Search_Count < 0.0f ) {
				Search_Count = 1.0f ;
				Search () ;
			}
		}
		// Action
		print (Action_Type);
		Action_Type = 1;
		switch ( Action_Type ) {
		case 0 : // Patrol mode.
			if ( Follow_Flag ) { // Follow mode.
				if ( Follow_MainBody ) { // The target's MainBody to follow exists.
					// Update the destination.
					if ( SetDestination_Count <= 0.0f ) {
						This_Agent.SetDestination ( Follow_MainBody.position ) ;
						SetDestination_Count = 5.0f ;
					} else {
						SetDestination_Count -= Time.deltaTime ;
					}
					Auto_Drive () ;
					break ;
				} else { // The target's MainBody to follow is lost.
					if ( Follow_Target ) {
						Follow_MainBody = Follow_Target.Find ( "MainBody" ) ; // Find the "MainBody" of Follow_Target.
						if ( Follow_MainBody ) {
							break ;
						}
					}
					// Follow_Target is lost, or target's MainBody is not found.
					Follow_Flag = false ;
					Update_Next_WayPoint () ;
					break ;
				}
			}
			if ( WayPoints.Length > 1 ) { // 'WayPoint_Pack' is assigned.
				if ( Vector3.Distance ( This_Transform.position , WayPoints [ Next_Num ] ) < WayPoint_Radius ) { // Arrived at the WayPoint.
					Update_Next_WayPoint () ;
				}
				Auto_Drive () ;
				break ;
			} else { // 'WayPoint_Pack' is not assigned or has only one waypoint.
				if ( Vector3.Distance ( This_Transform.position , WayPoints [ 0 ] ) < WayPoint_Radius ) { // Arrived at the WayPoint (initial position).
					if ( !Toward_Flag ) {
						Toward_Flag = true ;
						This_Agent.SetDestination ( Toward_Pos ) ; // Turn toward the initial direction.
					}
					Auto_Drive () ;
					Speed_Order = 0.0f ; // Stay here. Turn only.
					break ;
				} else {  // Not arrived.
					if ( Toward_Flag ) {
						Toward_Flag = false ;
						This_Agent.SetDestination ( WayPoints [ 0 ] ) ;
					}
					Auto_Drive () ;
					break ;
				}
			}
		case 1 : // Chase mode.
			if ( Target_Transform ) {
				if ( !GoBack_Flag ) { // Not going back now.
					// Update the destination.
					if ( SetDestination_Count <= 0.0f ) {
						This_Agent.SetDestination ( Target_Transform.position ) ;
						SetDestination_Count = 5.0f ;
					} else {
						SetDestination_Count -= Time.deltaTime ;
					}
					// Set 'Near_Flag'.
					if ( Target_Distance > Approach_Distance + 10.0f ) { // Far from Approach_Distance.
						Near_Flag = false ; // Keep approaching.
					} else { // Within Approach_Distance.
						if ( Fire_Flag || Target_Root_Transform.tag == "Finish" ) { // Cannon can aim the target, or the target is dead.
							Near_Flag = true ; // Stop approaching.
						} else { // Cannon can not aim the target yet.
							Near_Flag = false ; // Keep approaching.
						}
					}
					Auto_Drive () ;
					break ;
				} else { // Going back now.
					if ( Fire_Flag ) { // Cannon can aim the target.
						GoBack_Flag = false ; // Stop going back.
						SetDestination_Count = 0.0f ;
						break ;
					} else { // Cannon can not aim the target yet.
						Near_Flag = false ;
						Auto_Drive () ;
						break ;
					}
				}
			} else { // Target_Transform is lost.
				Lost_Target () ;
				break ;
			}
		}
		// "AI_Hand" touches an obstacle.
		if ( Brake_Flag && !GoBack_Flag) { // Brake_Flag is controlled by AI_Hand.
			Speed_Order = 0.0f ; // The tank can only turn.
		}
	}

	void Update_Next_WayPoint () {
		switch ( Patrol_Type ) {
		case 0 :
			Next_Num += Step ;
			if ( Next_Num >= WayPoints.Length ) {
				Next_Num = 0 ;
			} else if ( Next_Num < 0 ) {
				Next_Num = WayPoints.Length - 1 ;
			}
			break ;
		case 1 :
			Next_Num = Random.Range ( 0 , WayPoints.Length ) ;
			break ;
		}
		//This_Agent.SetDestination ( WayPoints [ Next_Num ] ) ;
	}

	void Search () {
		if ( Target_Root_Transform.tag != "Finish" ) {
			Target_Distance = Vector3.Distance ( This_Transform.position , Target_Transform.position ) ;
			if ( Target_Distance < Visibility_Radius ) {
				// Cast Ray from "AI_Eye" to the target.
				Ray Temp_Ray = new Ray ( Eye_Transform.position , Target_Pos - Eye_Transform.position ) ;
				RaycastHit Temp_RaycastHit ;
				if ( Physics.Raycast ( Temp_Ray , out Temp_RaycastHit , Visibility_Radius , Ray_LayerMask ) ) { // Ray hits anything.
					if ( Temp_RaycastHit.transform.root == Target_Root_Transform ) { // Ray hits the target.
						Detect_Flag = true ; // Referred to from "Cannon_Vertical".
						switch ( Action_Type ) {
						case 0 : // Patrol mode.
							Action_Type = 1 ;
							SetDestination_Count = 0.0f ;
							// Send Target_Transform to "Turret_Horizontal".
							This_Transform.parent.BroadcastMessage ( "Set_Lock_On" , Target_Transform , SendMessageOptions.DontRequireReceiver ) ;
							// Update Text.
							Text_Update ( "Attack" , Color.red ) ;
							break ;
						case 1 : // Chase mode.
							Lost_Time = 0.0f ;
							// Update Text.
							Text_Update ( "Attack" , Color.red ) ;
							break ;
						}
						return ; // Ignore the following code.
					}
				}
			}
		}
		// Target is out of range or Ray does not hit anything or Ray hits other object or the target is already dead.
		Detect_Flag = false ; // Referred to from "Cannon_Vertical".
		if ( Action_Type == 1 ) { // Chase mode.
			Lost_Time += Time.deltaTime + 1.0f ;
			// Update Text.
			Text_Update ( "Lost : " + Mathf.CeilToInt ( Lost_Count - Lost_Time ) , Color.magenta ) ;
			// AI has missed the target.
			if ( Lost_Time > Lost_Count ) {
				Lost_Target () ;
			}
		}
	}

	public void Lost_Target () { // Called when AI has missed the target, or called from "Game_Controller" when any tank is ReSpawned.
		if ( Action_Type == 1 ) { // Chase_Mode
			if ( !Follow_Flag ) {
				This_Agent.SetDestination ( WayPoints [ Next_Num ] ) ; // Set destination .
			} else {
				SetDestination_Count = 0.0f ;
			}
		}
		Detect_Flag = false ; // Referred to from "Cannon_Vertical".
		Action_Type = 0 ; // Search mode.
		Lost_Time = 0.0f ;
		Near_Flag = false ;
		Toward_Flag = false ;
		// Send Message to "Turret_Horizontal".
		transform.parent.BroadcastMessage ( "Reset_Lock_On" , SendMessageOptions.DontRequireReceiver ) ;
		// Update Text.
		Text_Update ( "Search" , Default_Text_Color ) ;
	}

	void Auto_Drive () {
		// Get the next corner position.
		Vector3 Next_Corner_Pos ;
		if ( Near_Flag && Face_Enemy ) { // 'Near_Flag' is true only when the cannon can aim the target.
			Next_Corner_Pos = Target_Transform.position ;
		} else if ( This_Agent.path.corners.Length > 1 ) {
			Next_Corner_Pos = This_Agent.path.corners [ 1 ] ;
		} else {
			Next_Corner_Pos = new Vector3();//This_Agent.path.corners [ 0 ] ;
		}
		// Store the last position.
		if ( !GoBack_Flag ) {
			if ( Vector3.Distance ( This_Transform.position , Last_Pos ) > 50.0f ) {
				Stored_Pos = Last_Pos ;
				Last_Pos = This_Transform.position ;
			}
		}
		// Calculate the direction to the next corner.
		float Temp_Angle ;
		Vector3 Local_Pos = This_Transform.InverseTransformPoint ( Next_Corner_Pos ) ;
		Temp_Angle = Vector2.Angle ( Vector2.up , new Vector2 ( Local_Pos.x , Local_Pos.z ) ) ;
		// Move backward.
		if ( GoBack_Flag ) {
			Temp_Angle -= 180.0f ;
		}
		// Left or Right.
		if ( Local_Pos.x < 0.0f ) {
			Temp_Angle = -Temp_Angle ;
		}
		// Face the target.
		if ( Near_Flag && Face_Enemy ) {
			if ( Temp_Angle >= 0.0f ) { // Left
				Temp_Angle -= Face_Offest_Angle ;
			} else { // Right
				Temp_Angle += Face_Offest_Angle ;
			}
		}
		// Calculate Turn rate and Speed rate.
		float Temp_Distance = Vector3.Distance ( This_Transform.position , Next_Corner_Pos ) ; // Distance to the next corner.
		if ( Temp_Angle < -Min_Turn_Angle ) { // Left
			Turn_Order = Mathf.Lerp ( -Min_Turn_Rate , -Max_Turn_Rate , Temp_Angle / -Pivot_Turn_Angle ) ;
			Speed_Order = Max_Speed_Rate + Turn_Order ;
			Speed_Order = Mathf.Clamp ( Speed_Order , 0.0f , Max_Speed_Rate ) ;
			if ( Temp_Angle < -Slow_Turn_Angle ) { // Slow Turn
				Turn_Order *= Slow_Turn_Rate ;
				Speed_Order *= Slow_Turn_Rate ;
			}
			if ( Temp_Distance < 30.0f ) { // Near to the next corner.
				Speed_Order *= Mathf.Lerp ( Min_Speed_Rate , 1.0f , Temp_Distance / 30.0f ) ;
			}
			if ( Near_Flag ) { // Near to the target.
				float Temp_Speed_Order = Mathf.Lerp ( 0.0f , Max_Speed_Rate , ( Target_Distance - Approach_Distance ) / 10.0f ) ;
				if ( Speed_Order > Temp_Speed_Order ) {
					Speed_Order = Temp_Speed_Order ;
				}
			}
		} else if ( Temp_Angle > Min_Turn_Angle ) { // Right
			Turn_Order = Mathf.Lerp ( Min_Turn_Rate , Max_Turn_Rate , Temp_Angle / Pivot_Turn_Angle ) ;
			Speed_Order = Max_Speed_Rate - Turn_Order ;
			Speed_Order = Mathf.Clamp ( Speed_Order , 0.0f , Max_Speed_Rate ) ;
			if ( Temp_Angle > Slow_Turn_Angle ) { // Slow Turn
				Turn_Order *= Slow_Turn_Rate ;
				Speed_Order *= Slow_Turn_Rate ;
			}
			if ( Temp_Distance < 30.0f ) { // Near to the next corner.
				Speed_Order *= Mathf.Lerp ( Min_Speed_Rate , 1.0f , Temp_Distance / 30.0f ) ;
			}
			if ( Near_Flag ) { // Near to the target.
				float Temp_Speed_Order = Mathf.Lerp ( 0.0f , Max_Speed_Rate , ( Target_Distance - Approach_Distance ) / 10.0f ) ;
				if ( Speed_Order > Temp_Speed_Order ) {
					Speed_Order = Temp_Speed_Order ;
				}
			}
		} else { // No Turn
			Turn_Order = 0.0f ;
			if ( Near_Flag ) { // Near to the target.
				Speed_Order = Mathf.Lerp ( 0.0f , Max_Speed_Rate , ( Target_Distance - Approach_Distance ) / 10.0f ) ;
				if ( Mathf.Abs ( Speed_Order ) < 0.01f ) {
					Speed_Order = 0.0f ; // Remove wasted order.
				}
			} else { // Far from the target.
				Speed_Order = Mathf.Lerp ( Min_Speed_Rate , Max_Speed_Rate , Temp_Distance / 50.0f ) ;
			}
		}
		// Move backward when stuck.
		if ( GoBack_Flag ) {
			Speed_Order *= -0.25f ;
		}
	}

	public IEnumerator Escape_Stuck () { // Called from "AI_Hand" when stuck.
		if ( !Follow_Flag ) {
			if ( Action_Type == 0 && !GoBack_Flag ) { // Patrol mode.
				if ( Random.Range ( 0 , 3 ) == 0 ) { // case 1/3 >> Go back to the stored position.
					GoBack_Flag = true ;
					This_Agent.SetDestination ( Stored_Pos ) ;
					yield return new WaitForSeconds ( Stuck_Count * 2.0f ) ;
					GoBack_Flag = false ;
				}
				if ( Patrol_Type == 0 ) { // Order.
					Step *= -1 ; // Switch the direction.
				}
				if ( Action_Type == 0 ) { // Patrol mode.
					Update_Next_WayPoint () ;
				} else { // Chase mode.
					SetDestination_Count = 0.0f ;
				}
			} else if ( Action_Type == 1 && !GoBack_Flag && !Fire_Flag ) { // Chase mode.
				GoBack_Flag = true ; // Go back to the stored position.
				This_Agent.SetDestination ( Stored_Pos ) ;
				yield return new WaitForSeconds ( Stuck_Count * 2.0f ) ;
				GoBack_Flag = false ;
				if ( Action_Type == 0 ) { // Patrol mode.
					This_Agent.SetDestination ( WayPoints [ Next_Num ] ) ;
				} else { // Chase mode.
					SetDestination_Count = 0.0f ;
				}
			}
		}
	}

	void Text_Update ( string Temp_String , Color Temp_Color ) {
		if ( Message_GUIText ) {
			Message_GUIText.text = Tank_Name + " = " + Temp_String ;
			Message_GUIText.color = Temp_Color ;
		}
	}

	public bool RayCast_Check ( Transform Temp_Transform , MainBody_Setting_CS Temp_Script ) {
		Vector3 Temp_Pos = Temp_Transform.position + ( Temp_Transform.up * Temp_Script.AI_Upper_Offset ) ;
		Ray Temp_Ray = new Ray ( Eye_Transform.position , Temp_Pos - Eye_Transform.position ) ;
		RaycastHit Temp_RaycastHit ;
		if ( Physics.Raycast ( Temp_Ray , out Temp_RaycastHit , Visibility_Radius , Ray_LayerMask ) ) { // Ray hits anything.
			if ( Temp_RaycastHit.transform.root == Temp_Transform.root ) { // Ray hits the target.
				return true ;
			} else { // Ray does not hit the target.
				return false ;
			}
		} else { // Ray does not hit anything.
			return false ;
		}
	}

	void MainBody_Linkage () {
		Text_Update ( "Dead." , Color.black ) ; // Update Text.
		Destroy ( this.gameObject ) ; // Bye.
	}

}
