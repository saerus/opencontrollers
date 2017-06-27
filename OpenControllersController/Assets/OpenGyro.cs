using UnityEngine;
using System.Collections;
//
//[RequireComponent(typeof(_OpenNet))]
//[RequireComponent(typeof(UnityOSCReceiver))]
//
public class OpenGyro : MonoBehaviour {
	public float savedAngle;
	public float calibratedAngle;
	public Quaternion q = Quaternion.identity;
	public Texture fleche;
	int circle;
	bool firstCalibration = false;
	GUIStyle guis = new GUIStyle();
	// Use this for initialization
	public void Start () {
		Input.gyro.enabled = true;
		circle = (int)Screen.width/5;
		StartCoroutine (sendInfos ());
	}
	// Update is called once per frame
	public void Update () {
		q.x = Input.gyro.attitude.x;
		q.y = Input.gyro.attitude.y;
		q.z = Input.gyro.attitude.z;
		q.w = Input.gyro.attitude.w;
		
	}
	public void OnGUI() {
		GUI.skin.button.fontSize = 50;
		GUILayout.BeginArea (new Rect(new Vector2(0, Screen.height/2), new Vector2(Screen.width, Screen.height/2)));
		GUILayout.BeginVertical();
		if (GUILayout.Button("Calibrate")) {
			calibrate();
		}
		if (GUILayout.Button("Quit")) {
			//BroadcastMessage("DeleteConnexion", SendMessageOptions.DontRequireReceiver);
			//Application.LoadLevel(1);
			Application.Quit();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea ();
		//
		Matrix4x4 svgGUI = GUI.matrix;
		Vector2 gp = new Vector2(Screen.width / 2, Screen.height / 2);
		calibratedAngle = q.eulerAngles.z-savedAngle;
		if(firstCalibration) {
			GUIUtility.RotateAroundPivot(calibratedAngle, gp);
		}
		GUI.DrawTexture(new Rect(Screen.width/2-circle/2, Screen.height/2-Screen.width/3-circle, circle, circle), fleche);
		//GUI.DrawTexture(new Rect(Screen.width/2-Screen.width/2, Screen.height/2-Screen.width/2, Screen.width, Screen.width), (Texture)Resources.Load("ordi"));	
		GUI.matrix = svgGUI;
	}
	public virtual void calibrate() {
		// get the yaw to calibrate
		firstCalibration = true;
		savedAngle = q.eulerAngles.z;
		Handheld.Vibrate ();
	}
	IEnumerator sendInfos ()
	{
		while (true) {
			// GYRO
			OSC.NET.OSCMessage messageGyro = new OSC.NET.OSCMessage ("gyro");
			messageGyro.Append (q.x);
			messageGyro.Append (q.y);
			messageGyro.Append (q.z);
			messageGyro.Append (q.w);
			messageGyro.Append (savedAngle);
			BroadcastMessage ("TransmitMessage", messageGyro);
			//
			OSC.NET.OSCMessage messageAccel = new OSC.NET.OSCMessage ("accel");
			messageAccel.Append (Input.acceleration.x);
			messageAccel.Append (Input.acceleration.y);
			messageAccel.Append (Input.acceleration.z);
			messageAccel.Append (Input.gyro.gravity.x);
			messageAccel.Append (Input.gyro.gravity.y);
			messageAccel.Append (Input.gyro.gravity.z);
			BroadcastMessage ("TransmitMessage", messageAccel);
			//
			yield return new WaitForSeconds (0.1f);
		}
	}
}
