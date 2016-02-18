﻿using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	// PUBLIC vars -----------------------
	[Range(0,100)]
	public float maxJumpForce = 0.8f;					// Maximum Y force to Apply to Rigidbody when jumping

	[Range(0,100)]
	public float maxDoubleJumpForce = 0.8f;		

	[Range(0,100)]
	public float sprintJumpForce = 2.0f;				// Y force to Apply to Rigidbody when sprint jumping
	
	[Range(0,1)]
	public float jumpForceDeclineSpeed = 0.02f;			// How fast the jump force declines when jumping

	[Range(0,50)]
	public float jumpTurnSpeed = 20f;
	
	// Speed modifier of the character's Z movement wheile jumping
	[Range(0,400)]
	public float idleJumpForwardSpeed = 10f;
	
	[Range(0,400)]
	public float runningJumpForwardSpeed = 10f;
	
	[Range(0,400)]
	public float sprintJumpForwardSpeed = 40f;

	// PRIVATE vars -----------------------

	private float jumpTriggerLimitTimer = 5f;
	private float forwardSpeed; 					// Temp var for forward speed
	private float lastJumpTime;
	private float speed;							// Temp var for locomotion 
	private Vector3 forwardVel;					    // Temp vector for calculating forward velocity while jumping
	private float lerpedJumpForce;
	private bool hasDoubleJumped = false;

	// Animator variables
	private int anim_idleJump = Animator.StringToHash("IdleJump");
	private int anim_runningJump = Animator.StringToHash("RunningJump");
	private int anim_sprintJump = Animator.StringToHash("SprintJump");
	private int anim_doubleJump = Animator.StringToHash("DoubleJump");

	private Animator animator;
	private Rigidbody rb;
	private RomanCharController charController;
	private RomanCharState charState;

	// Temp vars

	// Use this for initialization
	void Start () 
	{
		RegisterEvents();

		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		charController = GetComponent<RomanCharController>();
		charState = GetComponent<RomanCharState>();
	}


	private void RegisterEvents ()
	{
		print("registered events");
		EventManager.onInputEvent += Jump;
		EventManager.onCharEvent += CharLanded;
	}

	private void OnDisable () 
	{ 
		//print("char disable");	
		EventManager.onInputEvent -= Jump;
		EventManager.onCharEvent -= CharLanded;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		LimitJump();

		RotateAndMoveJump();
	}

	private void LimitJump()
	{
		// Add a force downwards if the player releases the jump button
		// when the character is jumping up
		if (InputController.jumpReleased && charState.IsIdleOrRunningJumping() /*&& !charState.IsSprintJumping()*/)
		{
			//print ("Jump released. Y velocity: " + rb.velocity.y);
			InputController.jumpReleased = false;

			if (rb.velocity.y > 0)
			{
				rb.AddForce (new Vector3 (0, -rb.velocity.y/2, 0), ForceMode.Impulse);
			}
		}
	}

	private void RotateAndMoveJump()
	{
		if (charController.moveDirectionRaw != Vector3.zero)
		{
			rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(charController.moveDirectionRaw), jumpTurnSpeed * Time.deltaTime));
			forwardVel = transform.forward * forwardSpeed * Mathf.Clamp01(charController.moveDirectionRaw.sqrMagnitude) * Time.deltaTime;
			forwardVel.y = rb.velocity.y;
			rb.velocity = forwardVel;
		}
	}

	public void JumpUp()
	{
		float force = 0;

		if (charState.IsIdleJumping())
		{
			force = maxJumpForce;
			forwardSpeed = idleJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
		else if (charState.IsRunningJumping())
		{
			force = maxJumpForce;
			forwardSpeed = runningJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
		else if (charState.IsSprintJumping())
		{
			force = sprintJumpForce;									
			forwardSpeed = sprintJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
	
	}

	public void DoubleJumpUp()
	{
		if (!hasDoubleJumped)
		{
			hasDoubleJumped = true;
			float force = maxDoubleJumpForce;

			print("Do double jump");
			//rb.velocity = Vector3.zero;
			rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
	}

	private void Jump (GameEvent gameEvent)
	{
		//print (gameEvent);
		if (gameEvent == GameEvent.Jump && charState.IsIdleOrRunningJumping() && !hasDoubleJumped)
		{
			animator.SetTrigger(anim_doubleJump);
		}

		else if (gameEvent == GameEvent.Jump && charState.IsIdleOrRunning()) 
		{				
			EventManager.OnCharEvent(GameEvent.Jump);

			// Change the forward speed based on what kind of jump it is
			if (charState.IsIdle())
			{
				animator.SetTrigger (anim_idleJump);
			}

			else if (charState.IsRunning())
			{
				animator.SetTrigger (anim_runningJump);
			}

			else if (charState.IsSprinting())
			{
				animator.SetBool (anim_sprintJump, true);
				charController.OrientCapsuleCollider(false);
			}
		}
	}

	private void CharLanded (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			hasDoubleJumped = false;
			animator.SetBool (anim_sprintJump, false);					        // Reset sprint jump trigger, Sometimes it gets stuck

		}
	}

}
