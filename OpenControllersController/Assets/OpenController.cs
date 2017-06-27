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
public class OpenController : MonoBehaviour
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
	string wantToConnect = "";
	//
	GUIStyle guis = new GUIStyle();

	OSC.NET.OSCTransmitter transmitter;
	//
	byte[] receiveBytes;
	//
	void Start ()
	{
		remote_end = new IPEndPoint (IPAddress.Any, startup_port);
		udp_client = new UdpClient (remote_end);
		udp_client.JoinMulticastGroup (group_address);
		StartCoroutine (MakeConnection ());
		StartGameClient ();
	}
	// ------------------------ CLIENT
	void StartGameClient ()
	{
		// multicast receive setup
	
		// async callback for multicast
		udp_client.BeginReceive (new AsyncCallback (ServerLookup), null);
		//
	}
	//
	IEnumerator MakeConnection ()
	{
		// continues after we get server's address
		while (server_ip == null) {
			Debug.Log ("yield");
			yield return new WaitForSeconds (1);
		}
		while (Network.peerType == NetworkPeerType.Disconnected) {
			Debug.Log ("connecting: " + server_ip + ":" + server_port);
	
			// the Unity3d way to connect to a server
			NetworkConnectionError error;
			error = Network.Connect (server_ip, server_port);
	
			Debug.Log ("status: " + error);
			//BroadcastMessage("Connexion", server_ip);
			yield return new WaitForSeconds (1);
		}
	}
	/*void ServerLookupCo(IAsyncResult ar) {
		StartCoroutine(ServerLookup(ar));
	}*/
	/******* broadcast functions *******/
	void ServerLookup (IAsyncResult ar)
	{
		//Debug.Log("RECEIVE QQCH ?");
		// receivers package and identifies IP
		receiveBytes = udp_client.EndReceive (ar, ref remote_end);
		//Debug.Log(remote_end.ToString());
		//
		temp_server_ip = remote_end.Address.ToString ();
		wantToConnect = Encoding.ASCII.GetString (receiveBytes);
		Debug.Log ("Server: " + server_ip + Encoding.ASCII.GetString (receiveBytes));
		//
		OpenWindow ();
		//AcceptConnexion();
	    
	}
	//
	void OpenWindow ()
	{
		windowIsOpen = true;
	}

	void AcceptConnexion ()
	{
		windowIsOpen = false;
		server_ip = temp_server_ip;
		connexion (server_ip);
	}

	void DenyConnexion ()
	{
		windowIsOpen = false;
		StartGameClient ();
	}

	void OnGUI ()
	{

		GUI.skin.button.fontSize = 50;
		GUI.skin.label.fontSize = 50;
		GUILayout.BeginArea (new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height/2)));
		GUILayout.BeginVertical ();
		GUILayout.Label ("I am a client, the server IP is: " + server_ip);
		//
		if (windowIsOpen) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Accept " + temp_server_ip + " ? ("+wantToConnect+")")) {
				AcceptConnexion ();
			}
			if (GUILayout.Button ("Deny !")) {
				DenyConnexion ();
			}
			GUILayout.EndHorizontal ();
		}
		//
		GUILayout.EndVertical ();
		GUILayout.EndArea ();


		GUI.depth = 10;
		//if (Input.GetMouseButton(0)) {
		GUI.color = new Color (1, 1, 1, 1);
			
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
			
		//}

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
		
		//Application.LoadLevel(1);
	}
}