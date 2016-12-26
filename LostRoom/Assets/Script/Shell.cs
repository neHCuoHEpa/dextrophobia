using UnityEngine;
using System.Collections;

public class Shell : MonoBehaviour {


	public Game game;
	public Rigidbody rigid;
	public GameObject missExplosionPrefab;
	public GameObject hitExplosionPrefab;

	public Tank target;

	void Start () {
	}
	
	float liveTime = 0;
	void Update () {
		if (rigid.velocity.magnitude > 0) 
			transform.rotation = Quaternion.LookRotation (rigid.velocity);
		liveTime += Time.deltaTime;
	}

	void FixedUpdate () {
		if (liveTime > 1 && target != null) {
			rigid.velocity = (target.transform.position - transform.position).normalized * rigid.velocity.magnitude;
		}
	}

	void OnCollisionEnter(Collision collision) {
		
		if (collision.collider.gameObject.layer == 8) {
			GameObject explosion = Instantiate<GameObject> (missExplosionPrefab);//, collision.contacts[0].point, Quaternion.identity);
			explosion.transform.position = collision.contacts[0].point;
			Destroy(explosion, 5);
		}
		if (collision.collider.gameObject.layer == 11) {
			target.Hit (collision);
			game.bluetooth.Send(Bluetooth.SendCommand.TANK_EXPLODED);
		}
		this.gameObject.SetActive (false);
	}
}
