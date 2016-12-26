using UnityEngine;
using System.Collections;

public class StartPoint : MonoBehaviour {


	public Transform cannon;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, 2);
		Gizmos.DrawLine(transform.position, cannon.position);
	}
}
