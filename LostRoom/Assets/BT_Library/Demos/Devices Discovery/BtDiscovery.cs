using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TechTweaking.Bluetooth;

public class BtDiscovery : MonoBehaviour
{
	public Button deviceButton;
	public Text DeviceText;//ScrollTerminalUI is a script used to control the ScrollView text

	Dictionary<string,DeviceData> MacAddressToBluetoothDevice;
	class DeviceData
	{
		public BluetoothDevice device;
		public short RSSI;
		public Button button;
		
		public DeviceData (BluetoothDevice dev, short RSSI)
		{
			this.device = dev;
			this.RSSI = RSSI;
		}
	}




	// Use this for initialization
	void Start ()
	{
		BluetoothAdapter.askEnableBluetooth ();

		MacAddressToBluetoothDevice = new Dictionary<string,DeviceData> ();
		BluetoothAdapter.OnDeviceDiscovered += HandleOnDeviceDiscovered;

	}

	void HandleOnDeviceDiscovered (BluetoothDevice dev, short rssi)
	{
		//Remember : dev != any other devices even if they're targeting the same device(because they're a connection container for the targeted device). 
		//to check if they target the same device check there MAC address
		DeviceData devData;
		if (!MacAddressToBluetoothDevice.ContainsKey (dev.MacAddress)) {
			devData = new DeviceData(dev, rssi);
			MacAddressToBluetoothDevice.Add (dev.MacAddress, devData);
			devData.button = addButton(dev.Name,dev.MacAddress,rssi.ToString());

		} else {
			devData = MacAddressToBluetoothDevice [dev.MacAddress];
			devData.RSSI = rssi;
		}
		devData.button.GetComponentInChildren<Text>().text =  dev.MacAddress + '\n'+ dev.Name   +'\n' + "RSSI : " + rssi ;

	}



	public Button addButton(string name, string MacAdress, string RSSI){
		

		if (MacAddressToBluetoothDevice.Count == 1) {
			deviceButton.interactable = true;
			deviceButton.name = MacAdress;
		} else {
			GameObject newButton = GameObject.Instantiate(deviceButton.gameObject) as GameObject ;
			newButton.transform.SetParent ( deviceButton.transform.parent,false);
			RectTransform rect = newButton.GetComponent<RectTransform>();
			rect.position += Vector3.down*rect.sizeDelta.y;
			deviceButton = newButton.GetComponent<Button>();
			deviceButton.name = MacAdress;

		}

		deviceButton.GetComponentInChildren<Text>().text =  MacAdress + '\n'+ name   +'\n' + "RSSI : " + RSSI ;


		return deviceButton;


	}

	public void OnButtonClicked(){
		string MAC = EventSystem.current.currentSelectedGameObject.name;
		DeviceText.text = MacAddressToBluetoothDevice[MAC].device.Name;

	}

	public void startDiscovery ()
	{
		/*Discovery is about 15 to 25 seconds procedure*/
		//Since we need a refresh button behaviour. We will cancel any ongoind discovery to start again. like refresh.
		BluetoothAdapter.cancelDiscovery();
		BluetoothAdapter.startDiscovery ();


	}

	void OnDestroy ()
	{
		BluetoothAdapter.OnDeviceDiscovered -= HandleOnDeviceDiscovered;
		BluetoothAdapter.cancelDiscovery();//Should be called whenever you done discovery.

	}

}