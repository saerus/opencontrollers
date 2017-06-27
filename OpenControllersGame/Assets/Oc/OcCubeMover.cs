using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OcCubeMover : MonoBehaviour {
	// Relative MOVEMENT
	// GYRO
	Vector3 angularAccelGyro = new Vector3();
	Vector3 angularAccelGyroSaver = new Vector3();
	//
	GameObject thisOne;
	public GameObject face;
	public GameObject camera;
	Quaternion gyroCorrected = Quaternion.identity;
	Vector3[] sixFaces = new Vector3[6];
	float angleMin = 15;
	float camRotation;
	Vector3 camPosition = new Vector3();
	public float force = 1;
	//
	Quaternion currentFace;
	Quaternion oldFace;
	Vector3 faceDown;
	bool inMovement = false;
	float size;
	//
	Vector3 edgeVec;
	Vector3 axeVec;
	int counterRot = 0;
	//
	void Start () {
		thisOne = this.gameObject;
		//biais.eulerAngles = new Vector3(45, 0, 0);
		//
		/*foreach(Quaternion q in sixPos) {
			q = Quaternion.identity;
		}*/
		sixFaces[0] = Vector3.right;
		sixFaces[1] = Vector3.left;
		sixFaces[2] = Vector3.up;
		sixFaces[3] = Vector3.down;
		sixFaces[4] = Vector3.forward;
		sixFaces[5] = Vector3.back;
	}
	void OnGUI() {
		/*if(inMovement == false) {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			Debug.Log("s");
			StartCoroutine(turnTheCube(1, 0));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			StartCoroutine(turnTheCube(-1, 0));
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			StartCoroutine(turnTheCube(0, -1));
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			StartCoroutine(turnTheCube(0, 1));
		}
		}*/
	}
	// Update is called once per frame
	/*void OnCollisionEnter (Collision col) {
		if(col.gameObject.tag.Equals("wall")) {
			//Debug.Log("TOUCHED");
			//StopCoroutine("turnTheCube");
		}
	}*/
	void Update () {
		if(camera != null) {
			Debug.Log(gyroCorrected);
			camRotation=(gyroCorrected.eulerAngles.y/360)*Mathf.PI*2;
			camRotation = -Mathf.PI/2;
			camPosition = thisOne.transform.position;
			camPosition.x+=Mathf.Cos(camRotation)*4;
			camPosition.z+=Mathf.Sin(camRotation)*4;
			camPosition.y+=2;
			camera.transform.position = camPosition;
			camera.transform.LookAt(thisOne.transform.position, Vector3.up);
		}
		//
		//
		if(inMovement == false) {
			float dst = float.MaxValue;
			foreach(Vector3 v in sixFaces) {
				float currentDst = Vector3.Distance(Vector3.down, thisOne.transform.rotation*v);
				//Debug.Log(currentDst);
				if(currentDst < dst) {
					dst = currentDst;
					faceDown = v;
				}
			}
			if(face != null) {
				face.transform.localPosition = faceDown;
			}
			// Strange but works
			if(Mathf.Abs(angularAccelGyro.x) > Mathf.Abs(angularAccelGyro.y)) {
				angularAccelGyro.y = 0;
			}
			if(Mathf.Abs(angularAccelGyro.y) > Mathf.Abs(angularAccelGyro.x)) {
				angularAccelGyro.x = 0;
			}
			//
			angularAccelGyroSaver += angularAccelGyro*force;
			angularAccelGyroSaver.x *= 0.95f;
			angularAccelGyroSaver.z *= 0.95f;
			
			//Debug.Log(angularAccelGyroSaver);
			if(angularAccelGyroSaver.x>=angleMin) {
				LaunchMovement(1, 0);
				//StartCoroutine(turnTheCube(1, 0));
			} else 
			if(angularAccelGyroSaver.x<=-angleMin) {
				LaunchMovement(-1, 0);
				//StartCoroutine(turnTheCube(-1, 0));
			} else 
			if(angularAccelGyroSaver.z>=angleMin) {
				LaunchMovement(0, -1);
				//StartCoroutine(turnTheCube(0, -1));
			} else 
			if(angularAccelGyroSaver.z<=-angleMin) {
				LaunchMovement(0, 1);
				//StartCoroutine(turnTheCube(0, 1));
			}
		}
		//thisOne.transform.rigidbody.AddTorque(angularAccelGyro*5);
		//thisOne.transform.rigidbody.AddTorque(angularAccelGyroSaver*50);
		/*if(angularAccelGyro.x>0) {
			Vector3 edgeVec = new Vector3(0, -0.5f, -0.5f);
			thisOne.transform.RotateAround(edgeVec, Vector3.Cross(edgeVec, Vector3.up), angularAccelGyro.x);
		}
		if(angularAccelGyro.x<0) {
			Vector3 edgeVec = thisOne.transform.position+new Vector3(0, -0.5f, 0.5f);
			thisOne.transform.RotateAround(edgeVec, Vector3.Cross(edgeVec, Vector3.up), -angularAccelGyro.x);
		}*/
		//
		
	}
	IEnumerator StopMoment() {
		inMovement = true;
		yield return new WaitForSeconds(0.4f);
		inMovement = false;
	}
	void LaunchMovement(int _dirX, int _dirZ) {
		//inMovement = true;
		angularAccelGyroSaver = new Vector3();
		StartCoroutine("StopMoment");
		size = this.transform.localScale.x/2;
		edgeVec = new Vector3(size*_dirZ, -size, size*_dirX);
		axeVec = Vector3.Cross(edgeVec, Vector3.down);
		edgeVec += thisOne.transform.localPosition;
		counterRot = 0;
	}
	void FixedUpdate() {
		if(inMovement) {
			if(counterRot<10) {
				thisOne.transform.GetComponent<Rigidbody>().AddTorque(axeVec*100*this.GetComponent<Rigidbody>().mass);
				counterRot++;
			} else {
				//inMovement = false;	
			}
		}
	}
	IEnumerator turnTheCube(int _dirX, int _dirZ) {
		
		size = this.transform.localScale.x/2;
		//thisOne.rigidbody.
		/*angularAccelGyroSaver = new Vector3();
		Vector3 t = new Vector3(_dirX, 0, _dirZ);
		t*=1000;
		
		inMovement = true;
		yield return new WaitForSeconds(0f);
		inMovement = false;*/
		//
		// **
		//
		angularAccelGyroSaver = new Vector3();
		inMovement = true;
		int dirX = _dirX;
		int dirZ = _dirZ;
		//Debug.Log("TURN NOW x:"+dirX+"  /  z:"+dirZ+"                                                                   "+Random.Range(0, 100000));
		//
		Vector3 edgeVec = new Vector3(size*dirZ, -size, size*dirX);
		Vector3 axeVec = Vector3.Cross(edgeVec, Vector3.down);
		edgeVec += thisOne.transform.localPosition;
		//
		int i=0;
		while(i<15) {
			//thisOne.rigidbody.Sleep();
			thisOne.transform.GetComponent<Rigidbody>().AddTorque(axeVec*100);
			//thisOne.transform.RotateAround(edgeVec, axeVec, 4.5f);
			//
			i++;
			yield return new WaitForSeconds(0.01f);
			//thisOne.rigidbody.WakeUp();
		}
		//yield return new WaitForSeconds(0.2f);
		inMovement = false;
		//thisOne.rigidbody.WakeUp();
	}
	public void setAngularAccel(Vector3 _angularAccelGyro) {
		angularAccelGyro = _angularAccelGyro;
	}
	public void setGyroCorrected(Quaternion _gyroCorrected) {
		gyroCorrected = _gyroCorrected;
	}
}
