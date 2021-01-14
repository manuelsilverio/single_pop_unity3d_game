using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public int rotationSpeedMin;
	public int rotationSpeedMax;
	private int rotationSpeed;
	public bool canRotate;
	public float rotationDirection;
	public GameObject BallCenter;

	// Use this for initialization
	void Start () {
		resetRotationSpeed ();
	}
	
	// Update is called once per frame
	void Update () {
		if (canRotate) {
			transform.Rotate(0, 0, rotationSpeed * rotationDirection *Time.deltaTime);
		}
	}

	public void resetRotationSpeed(){
		rotationSpeed = (int) Random.Range (rotationSpeedMin, rotationSpeedMax);
		Debug.Log ("rotation speed is: " + rotationSpeed);
	}

	public void resetRotation(){
		Quaternion target = Quaternion.Euler(0, 0, 0);
		transform.rotation = target;
	}
}
