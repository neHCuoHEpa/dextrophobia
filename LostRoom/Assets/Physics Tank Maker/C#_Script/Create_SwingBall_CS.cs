using UnityEngine;
using System.Collections;

[ ExecuteInEditMode ]

public class Create_SwingBall_CS : MonoBehaviour {
	
	public float Distance = 2.7f ;
	public int Num = 3 ;
	public float Spacing = 1.7f ;
	public float Mass = 10.0f ;
	public bool Gravity = false ;
	public float Radius = 0.1f ;
	public float Limit = 0.01f ;
	public float Spring = 500.0f ;

	public Transform Parent_Transform ;
	
	void Start () {
		Parent_Transform = this.transform ;
		if ( Application.isPlaying ) {
			Destroy ( this ) ;
		}
	}
	
	void Update () {
		if ( transform.localEulerAngles.z != 90.0f ) {
			float Temp_X = transform.localEulerAngles.x ;
			float Temp_Y = transform.localEulerAngles.y ;
			transform.localEulerAngles = new Vector3 ( Temp_X , Temp_Y , 90.0f ) ;
		}
	}
	
	void Reset () {
		Start () ;
	}
	
}