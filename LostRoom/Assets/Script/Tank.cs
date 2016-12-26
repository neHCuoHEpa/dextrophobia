using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

	[HideInInspector]
	public int id;

	public Game game;
	public GameObject bunker;
	public GameObject hitExplosionPrefab;
	public Rigidbody rigid;
	public float speed;
	//private Drive_Control_CS drive;
	private float y;

	[HideInInspector]
	public bool isHitted = false;

	void Start () {
		y = transform.position.y;
		//drive = this.GetComponent<Drive_Control_CS>();

	}

	void Update () {
		transform.rotation = Quaternion.LookRotation ((bunker.transform.position - this.transform.position));
		rigid.velocity = transform.forward*speed;
	}




	public void Hit (Collision coll) {
		GameObject explosion = Instantiate<GameObject> (hitExplosionPrefab);
		explosion.transform.position = coll.contacts[0].point;
		Destroy(explosion, 1);
		isHitted = true;
		//drive.Speed_Step = 0;
		game.activeTanks.Remove(this);
		Invoke ("Hide", 1);
	}
	private void Hide () {
		gameObject.SetActive(false);
	}

	public void ShowAt (Transform point) {
		isHitted = false;
		gameObject.SetActive(true);
		Vector3 newPosition = point.position;
		newPosition.y = y;
		transform.position = newPosition;
		transform.rotation = point.rotation;

		//drive.Speed_Step = speed;
	}
}
