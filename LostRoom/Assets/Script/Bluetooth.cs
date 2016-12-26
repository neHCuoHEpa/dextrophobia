using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TechTweaking.Bluetooth;
using UnityEngine.SceneManagement;

public class Bluetooth : MonoBehaviour {

	public Game game;
	public string deviceName;

	public Text log;

	private  BluetoothDevice device;

	public enum SendCommand {
		SUCCESS,
		CAN_ACTIVE,
		TANK_EXPLODED,
		TANK_SUCCEEDED,
		MISSILE_LAUNCHED
	}

	void Start () {

		BluetoothAdapter.enableBluetooth();

		device = new BluetoothDevice();
		device.Name = deviceName;

		device.setEndByte(10);
		device.ReadingCoroutine = ManageConnection;
		if (!device.IsConnected)
			device.connect ();
		GameObject.Find("LogStatus").GetComponent<Image>().color = Color.red;
		Log ("device: " + device.Name);

		DontDestroyOnLoad(this);
	}

	float connectionTryEstimation = 0;
	void Update () {
		if (!device.IsConnected && connectionTryEstimation > 2) {
			device.close();
			Connect ();
			connectionTryEstimation = 0;
		}
		connectionTryEstimation += Time.deltaTime;
	}

	public void Connect () {
		Log ("try to connect");
		device.ReadingCoroutine = ManageConnection;
		device.connect ();
	}

	public void Send (SendCommand command) {
		if (device != null && command != null) {
			Log ("try to send: "+ command.ToString());
			device.send (System.Text.Encoding.ASCII.GetBytes (command.ToString() + "\r\n"));
		}
		else {
			Log ("fail to send: "+ command.ToString());
		}
	}

	public void Log (string l) {
		if (log)
			log.text = "Log: "+ l;
	}

	IEnumerator  ManageConnection (BluetoothDevice device)
	{
		Log("Status : Connected & Can read");

		while (device.IsReading) {
			GameObject.Find("LogStatus").GetComponent<Image>().color = Color.magenta;
			GameObject.Find("LogStatusText").GetComponent<Text>().text = Time.time.ToString();
			if (device.IsDataAvailable) {
				byte [] msg = device.read ();//because we called setEndByte(10)..read will always return a packet excluding the last byte 10.
				GameObject.Find("LogStatus").GetComponent<Image>().color = Color.green;
				if (msg != null && msg.Length > 0) {
					string content = System.Text.ASCIIEncoding.ASCII.GetString (msg);
					Log("MSG : " + content);

					if (content.Contains("START")) {
						game.StartGame();

						Log( "PLAYERS : " + game.players + "\n" +
							"FT : " + game.fireInterval + "\n" + 
							 "ID : " + game.imposibleDuration + "\n" +
							"GTSR : " + game.spawningInterval + "\n" +
							"ITSR : " + game.imposibleSwapningInterval);
					}
//					if (content.Contains("RESTART")) {
//						device.close();
//						Scene scene = SceneManager.GetActiveScene();
//						SceneManager.LoadScene(scene.name);
//					}
//					if (content.Contains("PRESS")){
//						string index = content.Substring(5);
//						game.Fire (int.Parse(index));
//						Log( "FIRE : " + index);
//					}
//					if (content.Contains("PLAYERS")){
//						int value = int.Parse(content.Substring(7));
//						PlayerPrefs.SetInt("PLAYERS", value);
//						Log( "PLAYERS : " + value);
//					}
//					if (content.Contains("FT") || content.Contains("ID")) {
//						string key = content.Substring(0, 3);
//						float value = float.Parse(content.Substring(4));
//						PlayerPrefs.SetFloat(key, value);
//						Log(key + " : " + value);
//					}
//					if (content.Contains("GTSR") || content.Contains("ITSR")) {
//						string key = content.Substring(0, 5);
//						float value = float.Parse(content.Substring(6));
//						PlayerPrefs.SetFloat(key, value);
//						Log(key + " : " + value);
//					}
//					if (content.Contains("TIME")) {
//						int value = int.Parse(content.Substring(5));
//						PlayerPrefs.SetInt("TIME", value);
//					}
				}
			}
			else {
				GameObject.Find("LogStatus").GetComponent<Image>().color = Color.blue;
			}

			yield return null;
		}
		GameObject.Find("LogStatus").GetComponent<Image>().color = Color.red;
		//Log("Status : Done Reading");
	}

	// Test scene UI Send
	public void SendCommadnWithButton(int but) {
		if (but == 0) Send (SendCommand.TANK_EXPLODED);
		if (but == 1) Send (SendCommand.TANK_SUCCEEDED);
		if (but == 2) Send (SendCommand.MISSILE_LAUNCHED);
	}

	public void showDevices ()
	{
		BluetoothAdapter.showDevices ();
	}
	void HandleOnDevicePicked (BluetoothDevice device)//Called when device is Picked by user
	{

		this.device = device;

		device.ReadingCoroutine = ManageConnection;

	}
}
