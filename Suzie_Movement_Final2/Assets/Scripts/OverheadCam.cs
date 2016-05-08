using UnityEngine;
using System.Collections;

public class OverheadCam : MonoBehaviour {

	//Vector3 offset;
	public float distanceAway = 2.0f;
	public bool smooth = true;
	public float smoothDamp = 2.0f;
	public float rotSpeed = 20.0f;

	Transform Squirrel;
	RomanCharController charController;
	Vector3 constantDistance;
	float camYPos;
	Vector3 smoothVel;

	// Use this for initialization
	void Start () 
	{
		transform.LookAt (Squirrel);
		charController = GameManager.Instance.charController;
		Squirrel = charController.transform;

		constantDistance = (transform.position - Squirrel.position ).normalized;

		camYPos = transform.position.y;

		PositionCamera ();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		PositionCamera ();
	}

	void PositionCamera()
	{
		if (smooth) 
		{
			Vector3 targetPos = Squirrel.position + constantDistance * distanceAway;
			//targetPos.y = camYPos;
			//transform.position = targetPos;
			transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref smoothVel, smoothDamp);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Squirrel.position - transform.position), rotSpeed * Time.deltaTime);
			//transform.LookAt (Squirrel);
		} 
		else 
		{
			Vector3 targetPos = Squirrel.position + constantDistance * distanceAway;
			//targetPos.y = camYPos;
			transform.position = targetPos;
			transform.LookAt (Squirrel);
		}
	}
}
