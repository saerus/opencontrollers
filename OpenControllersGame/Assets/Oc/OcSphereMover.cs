using UnityEngine;
using System.Collections;

public class OcSphereMover : MonoBehaviour {
	// GYRO
	Vector3 angularAccelGyro = new Vector3();
	// ACCEL
	Vector3 relativeAccel = new Vector3();
	public bool lockX = false;
	public bool lockY = false;
	public bool lockZ = false;
	//
	GameObject thisOne;
	public float force = 1;
	public bool isDynamic = true;
	//
	void Start () {
		thisOne = this.gameObject;
	}
	// Update is called once per frame
	void FixedUpdate () {
		//
		if(lockX) {
			angularAccelGyro.x = 0;
		}
		if(lockY) {
			angularAccelGyro.y = 0;
		}
		if(lockZ) {
			angularAccelGyro.z = 0;
		}
		//
		if(isDynamic) {
			thisOne.GetComponent<Rigidbody>().AddTorque(angularAccelGyro*force);
			angularAccelGyro*=0.5f;
		} else {
			thisOne.transform.Rotate(angularAccelGyro*force, Space.World);
		}
		//thisOne.transform.Rotate(angularAccelGyro, Space.World);
		//thisOne.rigidbody.angularVelocity = angularAccelGyro;
		
		//
		//thisOne.transform.position = relativeAccel;
		//thisOne.transform.Translate(-1, 0, 0, Space.World);
	}
	public void setAngularAccel(Vector3 _angularAccelGyro) {
		angularAccelGyro = _angularAccelGyro;
	}
	public void setAccel(Vector3 _relativeAccel) {
		relativeAccel = _relativeAccel;
	}
}
