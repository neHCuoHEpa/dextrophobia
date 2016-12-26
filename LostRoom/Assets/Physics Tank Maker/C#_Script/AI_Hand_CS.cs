using UnityEngine;
using System.Collections;

public class AI_Hand_CS : MonoBehaviour {

	public bool Carving_Flag ; // Set from Tank_ID_Control at the opening. ( The value is set in "Game_Controller". )

	float Count ;
	bool Work_Flag = false ;
	bool Touch_Flag = false ;
	AI_CS AI_Script ;
	
	void Start () {
		gameObject.layer = 2 ; // "Ignore Raycast" layer.
		if ( GetComponent<Renderer>() ) {
			GetComponent<Renderer>().enabled = false ;
		}
		if ( GetComponent<Collider>() ) {
			GetComponent<Collider>().isTrigger = true ;
		}
	}

	void Update () {
		if ( Work_Flag ) {
			if ( Touch_Flag ) {
				Count += Time.deltaTime ;
				if ( Count > AI_Script.Stuck_Count ) {
					AI_Script.StartCoroutine ( "Escape_Stuck" ) ;
					Count = 0.0f ;
				}
				return ;
			} else {
				Count -= Time.deltaTime ;
				if ( Count < 0.0f ) {
					AI_Script.Brake_Flag = false ;
					Count = 0.0f ;
					Work_Flag = false ;
				}
			}
		}
	}
	
	void OnTriggerStay ( Collider Temp_Collider ) {
		if ( !Touch_Flag && Temp_Collider.attachedRigidbody ) {
			if ( Carving_Flag ) {
				if ( Temp_Collider.transform.root.tag != "Finish" ) { // for Unity Pro.
					Work_Flag = true ;
					AI_Script.Brake_Flag = true ;
					Touch_Flag = true ;
				}
			} else {
				Work_Flag = true ;
				AI_Script.Brake_Flag = true ;
				Touch_Flag = true ;
			}
		}
	}
	
	void OnTriggerExit () {
		Touch_Flag = false ;
	}

	void Get_AI ( AI_CS Temp_Script ) {
		AI_Script =Temp_Script ;
	}
	
}
