using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cannon : MonoBehaviour {

	public Game game;
	public Bluetooth bluetooth;
	public Shell shellPrefab;

	public Shell shell;

	public Transform spawnPoint;
	public Transform directionPoint;

	private Vector3 direction;

	public Canvas canvas;

	void Start () {
		direction = directionPoint.position - spawnPoint.position;

		if (!Application.isEditor) {
			canvas.gameObject.SetActive (false);
		}
		else {
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void Load (Shell shell,Tank tank) {
		this.shell = shell;

		shell.rigid.velocity = Vector3.zero;
		shell.transform.position = spawnPoint.position;
		shell.transform.rotation = Quaternion.LookRotation (direction);
		shell.gameObject.SetActive (true);
		shell.target = tank;
	}

	public void Fire () {
		shell.rigid.AddForce (direction.normalized*5000);
	}
}
