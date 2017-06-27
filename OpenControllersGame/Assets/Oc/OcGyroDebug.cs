using UnityEngine;
using System.Collections;

public class OcGyroDebug : MonoBehaviour {
	//
	GameObject sphereDebug;
	GameObject XXX;
	GameObject YYY;
	GameObject ZZZ;
	Vector3 left = Vector3.left;
	Vector3 up = Vector3.up;
	Vector3 forward = Vector3.forward;
	Quaternion gyroCorrected = Quaternion.identity;
	//
	GameObject thisOne;
	//
	void Start () {
		sphereDebug = GameObject.Find("SphereDebug");
		XXX = GameObject.Find("XXX");
		YYY = GameObject.Find("YYY");
		ZZZ = GameObject.Find("ZZZ");
		//
		thisOne = this.gameObject;
	}
	//
	void Update () {
		XXX.transform.LookAt(left+thisOne.transform.position);
		YYY.transform.LookAt(up+thisOne.transform.position);
		ZZZ.transform.LookAt(forward+thisOne.transform.position);
		sphereDebug.transform.rotation = gyroCorrected;
	}
	public void setLeft(Vector3 _left) {
		left = _left;
	}
	public void setUp(Vector3 _up) {
		up = _up;
	}
	public void setForward(Vector3 _forward) {
		forward = _forward;
	}
	public void setGyroCorrected(Quaternion _gyroCorrected) {
		gyroCorrected = _gyroCorrected;
	}
}
