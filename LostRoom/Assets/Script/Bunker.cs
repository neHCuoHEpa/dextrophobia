using UnityEngine;
using System.Collections;

public class Bunker : MonoBehaviour {

	public Game game;

	void OnCollisionEnter(Collision collision) {
		game.TankHittedBunker ();
		Tank tank = collision.gameObject.GetComponent<Tank>();
		if (tank == null) {
			tank = collision.gameObject.GetComponentInParent<Tank>();
		}
		tank.Hit(collision);
	}
}
