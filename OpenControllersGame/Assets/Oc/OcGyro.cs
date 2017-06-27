using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//
[RequireComponent(typeof(UnityOSCReceiver))]
//
public class OcGyro : MonoBehaviour  {
	// GYRO
	public Quaternion gyro = Quaternion.identity;
	Quaternion gyroCorrected = Quaternion.identity;
	Quaternion gyroCorrector = Quaternion.identity;
	Quaternion oldGyro = Quaternion.identity;
	Quaternion correction = Quaternion.identity;
	float angleCalibration = 0;
	float tempAngleCalibration = 0;
	Vector3 angularAccelGyro = new Vector3();
	bool firstReceive = true;
	Vector3 left = Vector3.left;
	Vector3 up = Vector3.up;
	Vector3 forward = Vector3.forward;
	// ACCEL
	Vector3 accel = new Vector3();
	Vector3 relativeAccel = new Vector3();
	Vector3 gravity = new Vector3();
	public Vector3 excitation;
	Vector3 excitationSmooth;
	bool LevelBegan = true;
	//
	void Start() {
		StartCoroutine("checkMoves");
		//StartCoroutine("checkShakes");
	}
	public void Begin() {
//		Debug.Log("LETS GO");
		LevelBegan = true;
	}
	public void Stop() {
//		Debug.Log("LETS GO");
		LevelBegan = false;
	}
	//
	void FixedUpdate() {
		if(!firstReceive) {
			// CALIBRATION CHANGE
			if(tempAngleCalibration != angleCalibration) {
				angleCalibration = tempAngleCalibration;
				correction.eulerAngles = new Vector3(90, angleCalibration+180, 0);
				gyroCorrected = correction*gyro;
				oldGyro = gyroCorrected;
			}
			gyroCorrected = correction*gyro;

			//correction = new Quaternion(Mathf.PI/2, 0, angleCalibration, 0);
			//Debug.Log(oldGyro+"  "+gyroCorrected);
			oldGyro = Quaternion.Lerp(oldGyro, gyroCorrected, 0.5f);
			//
			//Quaternion diff = gyroCorrected*Quaternion.Inverse(oldGyro);
			Quaternion diffCorrected = gyroCorrected*Quaternion.Inverse(oldGyro);
			//
			
			//
			left = diffCorrected*left;
			up = diffCorrected*up;
			forward = diffCorrected*forward;
			left = Vector3.Lerp(left, Vector3.left, 0.2f);
			up = Vector3.Lerp(up, Vector3.up, 0.2f);
			forward = Vector3.Lerp(forward, Vector3.forward, 0.2f);
			//
			angularAccelGyro.x = -forward.y;
			angularAccelGyro.y = forward.x;
			angularAccelGyro.z = -left.y;
			angularAccelGyro*=1;
			//
			if(LevelBegan) {
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Gyro")) {
					// ABSOLUTE GYROSCOPE ROTATION (gyroscope)
					g.BroadcastMessage("setGyro", gyro, SendMessageOptions.DontRequireReceiver);
					// ABSOLUTE GYROSCOPE R	OTATION CORRECTED (with screen orientation - gyroscope)
					g.BroadcastMessage("setGyroCorrected", gyroCorrected, SendMessageOptions.DontRequireReceiver);
					// ----------
					// RELATIVE ACCELLERATION OF GYRO CORRECTED (with screen orientation - gyroscope)
					g.BroadcastMessage("setAngularAccel", angularAccelGyro, SendMessageOptions.DontRequireReceiver);
					// RELATIVE ACCELLERATION (accelerometer)
					g.BroadcastMessage("setAccel", relativeAccel, SendMessageOptions.DontRequireReceiver);
				}
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Debug")) {
					g.BroadcastMessage("setGyroCorrected", gyroCorrected, SendMessageOptions.DontRequireReceiver);
					g.BroadcastMessage("setLeft", left, SendMessageOptions.DontRequireReceiver);
					g.BroadcastMessage("setUp", up, SendMessageOptions.DontRequireReceiver);
					g.BroadcastMessage("setForward", forward, SendMessageOptions.DontRequireReceiver);
				}
			}
			/*foreach(GameObject g in GameObject.FindGameObjectsWithTag("Level")) {
				// ABSOLUTE GYROSCOPE ROTATION (gyroscope)
				g.BroadcastMessage("setGyro", gyro, SendMessageOptions.DontRequireReceiver);
				// ABSOLUTE GYROSCOPE ROTATION CORRECTED (with screen orientation - gyroscope)
				g.BroadcastMessage("setGyroCorrected", gyroCorrected, SendMessageOptions.DontRequireReceiver);
				// ----------
				// RELATIVE ACCELLERATION OF GYRO CORRECTED (with screen orientation - gyroscope)
				g.BroadcastMessage("setAngularAccel", angularAccelGyro, SendMessageOptions.DontRequireReceiver);
				// RELATIVE ACCELLERATION (accelerometer)
				g.BroadcastMessage("setAccel", relativeAccel, SendMessageOptions.DontRequireReceiver);
			}*/
		}
	}
	//
	public void OSCMessageReceived(OSC.NET.OSCMessage message){
		string address = message.Address;
		ArrayList args = message.Values;
		//Debug.Log ("receive something: ");
		//Debug.Log("s");
		//
		if(address.Equals("gyro")) {
			gyro.x = (float)args[0];
			gyro.y = (float)args[1];
			gyro.z = (float)args[2];
			gyro.w = (float)args[3];
			tempAngleCalibration = (float)args[4];
			//
			if(firstReceive) {
				gyroCorrected = correction*gyro;
				oldGyro = gyro;
				firstReceive = false;
			}
		}
		if(address.Equals("accel")) {
			accel.x = (float)args[0];
			accel.y = (float)args[1];
			accel.z = (float)args[2];
			gravity.x = (float)args[3];
			gravity.y = (float)args[4];
			gravity.z = (float)args[5];
			//
			relativeAccel = accel-gravity;
		}
	}
	IEnumerator checkMoves() {
		while(true) {
			excitation = relativeAccel;
			Debug.Log (excitation.magnitude);
			if(angularAccelGyro.magnitude > 0.5) {
				//Debug.Log("***** "+excitation.magnitude+"          "+angularAccelGyro.magnitude);
				excitation *= 0f;
				excitationSmooth *=0f;

			}
			//Debug.Log(moyenne);
			if(LevelBegan) {
				foreach(GameObject g in GameObject.FindGameObjectsWithTag("Shake")) {
					g.BroadcastMessage("ShakeExcitation", excitation.magnitude, SendMessageOptions.DontRequireReceiver);
				}
			}
			/*foreach(GameObject g in GameObject.FindGameObjectsWithTag("Level")) {
				g.BroadcastMessage("ShakeExcitation", excitation.magnitude, SendMessageOptions.DontRequireReceiver);
			}*/
			yield return new WaitForSeconds(0.1f);
		}
	}
}
