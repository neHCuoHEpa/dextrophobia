All other demos, reads one line per frame. This demo reads all available packets/lines per frame, which gives a higher bit rate.

Note : 'HighRateTerminal.cs' script is already attached to the 'InfoController' gameobject in the scene.

Note : The highest bit rate possible by this plugin is achieved by avoiding using BluetoothDevice.setEndByte(byt) while simply using the method BluetoothDevice.read() which will read every single byte available at the time of calling it. The only problem for you is that the returned bytes from BluetoothDevice.read() are not packets.