using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.VR;

public class Game : MonoBehaviour {

	// OUTSIDE MANAGABLE
	public float spawningInterval;
	public float fireInterval;
	public float imposibleSwapningInterval;
	public float imposibleDuration;
	public int players;

	public Bluetooth bluetooth;
	public Timer timer;
	public int maxTanksOnField;
	public Cannon[] cannons;
	public Tank[] tanksPool;
	public Shell[] shellsPool;
	public Transform[] startPositions;
	public int[] spawningOrder;

	[HideInInspector]
	public GameState state;

	private int currentTankIndex;
	private int currentShellIndex;
	private int currentPositionIndex;
	private float spawnTimeElapsed;
	private float fireTimeElapsed;
	private bool canFire;

	[HideInInspector]
	public List<Tank> activeTanks;



	// Use this for initialization
	void Start () {
//		for (int i = 0; i < cannons.Length; i++) {
//			cannons[i].canvas.GetComponentInChildren<Text>().text = (i+1) + "";
//		}
		activeTanks = new List<Tank>();

		currentShellIndex = 0;
		currentTankIndex = 0;
		currentPositionIndex = 0;
		state = GameState.PrePlay;
		canFire = true;

		VRSettings.enabled = true;
	}

	public void StartGame () {
		players = PlayerPrefs.GetInt("PLAYERS", 2);
		fireInterval = PlayerPrefs.GetFloat("FI"+players.ToString(), 2);
		spawningInterval = PlayerPrefs.GetFloat("GTSR"+players.ToString(), 5);
		imposibleSwapningInterval = PlayerPrefs.GetFloat("GTSR"+players.ToString(), 5);
		imposibleDuration = PlayerPrefs.GetFloat("ID"+players.ToString(), 5);
		timer.totalTime = PlayerPrefs.GetFloat("TIME", 100);
		state = GameState.Playing;
	}

	void Update () {
		for (int i = 1; i <= cannons.Length; i++) {
			if (Input.GetKeyDown(i.ToString())) {
				Fire (i);
			}
		}

		if (Input.GetKey(KeyCode.Space)) {
			if (Input.mousePosition.x > .8f*Screen.width) {
				Camera.main.transform.Rotate (0, 30*Time.deltaTime, 0);
			}
			if (Input.mousePosition.x < .2f*Screen.width) {
				Camera.main.transform.Rotate (0, -30*Time.deltaTime, 0);
			}
		}
		if (Input.GetKey(KeyCode.S)) {
			state = GameState.Playing;
//			spawnTimeElapsed = 0;
			fireTimeElapsed = 0;
			canFire = true;
		}

		if (state == GameState.Playing && spawningInterval < spawnTimeElapsed) {
			SpawnTank ();
		}
		else {
			spawnTimeElapsed += Time.deltaTime;
		}
		if (!canFire) {
			fireTimeElapsed += Time.deltaTime;
		}
		if (state == GameState.Playing && fireTimeElapsed > fireInterval) {
			canFire = true;
			fireTimeElapsed = 0;
			bluetooth.Send (Bluetooth.SendCommand.CAN_ACTIVE);
			print("can fire");
		}
		if (state == GameState.PostPlay) {
			spawningInterval = imposibleSwapningInterval;
		}
	}

	public void Fire (int i) {
		if (!canFire) return;
		canFire = false;
		Tank tank = null;
		foreach (var t in activeTanks) {
			if (t.id == i) {
				tank = t;
			}
		}

		print("fire");
		bluetooth.Send (Bluetooth.SendCommand.MISSILE_LAUNCHED);
		cannons[i-1].Load (shellsPool[currentShellIndex],tank);
		cannons[i-1].Fire ();
		currentShellIndex++;
		if (currentShellIndex >= shellsPool.Length) currentShellIndex = 0;
		//Camera.main.transform.parent = cannons[i].shell.transform;
	}

	void SpawnTank () {
		if (activeTanks.Count > maxTanksOnField) return;
		spawnTimeElapsed = 0;

		int pos = spawningOrder[currentPositionIndex];
		tanksPool[currentTankIndex].ShowAt(startPositions[pos-1]);
		tanksPool[currentTankIndex].id = pos;
		activeTanks.Add (tanksPool[currentTankIndex]);
		currentTankIndex++;
		currentPositionIndex++;
		if (currentTankIndex >= tanksPool.Length) currentTankIndex = 0;
		if (currentPositionIndex >= spawningOrder.Length) currentPositionIndex = 0;

		//print ("active tank: "+pos);
	}

	public void TankHittedBunker () {
		timer.Reset ();
		bluetooth.Send(Bluetooth.SendCommand.TANK_SUCCEEDED);
	}

	public enum GameState {
		PrePlay,
		Playing,
		PostPlay
	}
}
