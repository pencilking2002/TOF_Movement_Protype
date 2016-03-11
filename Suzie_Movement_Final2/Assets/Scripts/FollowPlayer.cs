﻿using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform player;

	public Vector3 offset;
	public float damping = 10.0f;
	public float climbSpeed = 20.0f;

	[HideInInspector]
	public bool followAtPlayerPos = false;
	
	private Vector3 vel;
	private Vector3 targetPos;
	private Vector3 targetRot;

	private RomanCharState charState;
	private float _damping;
	private float climbSpeedSmoothVel;

	private Animator animator;

	void Start ()
	{

		if (player != null)
		{
			animator = player.GetComponent<Animator>();
			charState = player.GetComponent<RomanCharState>();
		}
		else
		{
			Debug.LogError("FollowPlayer: Player is null");
		}

		
	}
	// Update is called once per frame
	void Update () 
	{

		//targetPos = cam.colliding ? player.position + offset - Vector3.up * 0.5f :
		targetPos = player.position + offset;

		// Don't follow the character's y position if the character is jumping or sprinting
		if (charState.IsInAnyJumpingState() || charState.IsRunningOrSprinting())
		{
			targetPos.y = transform.position.y;
		}
		if (charState.IsClimbing())
		{
			_damping = climbSpeed;
		}
		else
		{
			_damping = damping;
		}

		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, _damping * Time.deltaTime);
//		
		// Check if the follow object has caught up with the player and that the follow object is above the camera
		if (Vector3.Distance(transform.position, targetPos) < 0.05f)
			followAtPlayerPos = true;
		
		else
			followAtPlayerPos = false;
		
		targetRot.y = Mathf.SmoothDampAngle(transform.eulerAngles.y, player.eulerAngles.y, ref climbSpeedSmoothVel, _damping * Time.deltaTime);
		transform.eulerAngles = targetRot;
			
	}
	
	private void OnEnable() 
	{ 
//		EventManager.onCharEvent += AttachFollow;
//		EventManager.onInputEvent += AttachFollow; 
		//EventManager.onInputEvent += Test;
	}

	private void OnDisable() 
	{ 
//		EventManager.onCharEvent -= AttachFollow;
//		EventManager.onInputEvent -= AttachFollow; 
		//EventManager.onInputEvent -= Test;
	}

//	private void Test (GameEvent e)
//	{
//		//if (e == GameEvent.Jump)
//			//print("follow: Test jump");
//	}

	private void AttachFollow (GameEvent gEvent)
	{
//		if (gEvent == GameEvent.AttachFollow || gEvent == GameEvent.StartClimbing || gEvent == GameEvent.StopClimbing || gEvent == GameEvent.Land)
//		{
//			Attach = true;
//			//print(gEvent);
//		}
//		else if (gEvent == GameEvent.DetachFollow )
//		{
//			Attach = false;
//		}
	}

}