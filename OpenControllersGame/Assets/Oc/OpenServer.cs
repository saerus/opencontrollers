// NEWONE
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using System;
using System.Text;
using System.IO;

//
public class OpenServer : MonoBehaviour
{
	private int server_port = 5000;
	private string temp_server_ip;
	private string server_ip;
	// multicast
	private int startup_port = 5100;
	private IPAddress group_address = IPAddress.Parse ("239.0.0.1");
	private UdpClient udp_client;
	private IPEndPoint remote_end;
	bool windowIsOpen = false;
	public String name = "Marc";
	//

	OSC.NET.OSCTransmitter transmitter;
	//
	byte[] receiveBytes;
	//
	void Start ()
	{
		StartGameServer ();
	
	}
	// ------------------------ SERVER
	void StartGameServer ()
	{
		// the Unity3d way to become a server
		NetworkConnectionError init_status = Network.InitializeServer (10, server_port, false);
		Debug.Log ("status: " + init_status);
		StartCoroutine (StartBroadcast ());
	}
	//
	IEnumerator StartBroadcast ()
	{
		// multicast send setup
		udp_client = new UdpClient ();
		udp_client.JoinMulticastGroup (group_address);
		remote_end = new IPEndPoint (group_address, startup_port);
		// sends multicast
		while (true) {
			// Server send every 1 sec.
			var buffer = Encoding.ASCII.GetBytes (name);
			udp_client.Send (buffer, buffer.Length, remote_end);
			//Debug.Log ("try");
			yield return new WaitForSeconds (1f);
		}
	}

	void OnGUI ()
	{
		GUI.depth = 10;
		//if (Input.GetMouseButton(0)) {
		GUI.color = new Color (1, 1, 1, 1);
		GUI.skin.label.alignment = TextAnchor.UpperRight;
		GUI.Label (new Rect (Screen.width / 2, 10, Screen.width / 2 - 10, Screen.height - 10), "I am a server and i know: " + server_ip);
	}
	//
	void TransmitMessage (OSC.NET.OSCMessage _msg)
	{
		if (transmitter != null) {
			transmitter.Connect ();
			transmitter.Send (_msg);
			transmitter.Close ();
		}
	}
	//
	void connexion (string serverIp)
	{
		transmitter = new OSC.NET.OSCTransmitter (serverIp, 3333);
	}
}