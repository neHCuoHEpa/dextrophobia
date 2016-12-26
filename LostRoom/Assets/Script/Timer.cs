using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public float totalTime;
	public Image timer;
	public Game game;

	[HideInInspector]
	public float time;

	void Start () {
		Reset ();
	}
	

	void Update () {
		if (game.state == Game.GameState.Playing) {
			if (time < totalTime) {
				timer.transform.localScale = new Vector2((totalTime-time)/totalTime, 1);
			}
			else {
				time = 0;
				timer.transform.parent.gameObject.SetActive(false);
				game.state = Game.GameState.PostPlay;
				game.bluetooth.Send(Bluetooth.SendCommand.SUCCESS);
			}
			time += Time.deltaTime;
		}
		if (game.state == Game.GameState.Playing) {
			time += Time.deltaTime;
			if (time > game.imposibleDuration) {

			}
		}
	}

	public void Reset () {
		time = 0;
	}
}
