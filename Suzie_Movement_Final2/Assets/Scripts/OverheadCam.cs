using UnityEngine;
using System.Collections;

public class OverheadCam : MonoBehaviour {
	public Transform Squirrel;

	public Vector3 offfset = new Vector3(0, 8.0f, 7.0f);				// Offset of the cam
	public float turnSpeed = 4.0f;
	public float xSpeed = 20.0f;
	public float damping = 0.02f;
	public bool canRotate = true;

	private Vector3 vel;
	private Quaternion lastRot;
	private float x;


	// Use this for initialization
	void Start () 
	{
		//transform.rotation =  Quaternion.Euler(0, transform.eulerAngles.y, 0);
		RotateCam ();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		RotateCam ();
	}

	private void RotateCam()
	{

		// Increment the x-axis input
		x = InputController.orbitH * xSpeed;
		//x = Mathf.Atan ();
		//Rotate the camera to those angles 
		Quaternion rotation = Quaternion.Euler(new Vector3(0, x + transform.eulerAngles.y, 0));

		transform.rotation = rotation;

		//Move the camera to look at the target
		Vector3 position = rotation * new Vector3(0.0f, offfset.y, -offfset.z) + Squirrel.position;
		transform.position = Vector3.SmoothDamp (transform.position, position, ref vel, damping);
		transform.LookAt (Squirrel);
	}
		
}
