using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class Responsible for controlling the character during all jumps
/// including Idle, Running and Sprint Jumping
/// </summary>
public class JumpController : MonoBehaviour {

	/*----------------------------------------------------------|
	| PUBLIC VARIABLES			    	                        |
	|----------------------------------------------------------*/

	// Idle Jumping --------------------------
	[Range(0,100)] 
	public float idleJumpUpForce = 11.0f;				// Maximum Y force to Apply to Rigidbody when idle jumping

	[Range(0,400)]
	public float idleJumpForwardSpeed = 10f;		    // Speed modifier of the character's Z movement wheile jumping

	// Running Jumping --------------------------
	[Range(0,100)] 
	public float runningJumpUpForce = 11.0f;			    // Maximum Y force to Apply to Rigidbody when jumping

	[Range(0,400)] 
	public float runningJumpForwardSpeed = 11.0f;	    // Maximum Y force to Apply to Rigidbody when jumping

	// Sprint Jumping --------------------------
	[Range(0,100)] 
	public float sprintJumpUpForce = 11.0f;				// Y force to Apply to Rigidbody when sprint jumping

	[Range(0,700)]
	public float sprintJumpForwardSpeed = 40f;

	[Range(0,100)]
	public float doubleJumpForce = 10.0f;		

	[Range(0,50)]
	public float allJumpTurnSpeed = 30.56f;			    // Turning speed used for all jumps

	[Header("Jump Limiting")]
	public float maxDownwardForce = -6.0f;
	public float downwardForceSpeed = 5.0f;
	public Transform cColliderFrontTransf;
	public float jumpWallLimitRayLength = 0.3f;

	// PRIVATE vars -----------------------

	private float forwardSpeed; 					// Temp var for forward speed
	private float speed;							// Temp var for locomotion 
	private Vector3 forwardVel;					    // Temp vector for calculating forward velocity while jumping
	private float lerpedJumpForce;
	private bool hasDoubleJumped = false;

	private float downwardForce;					// Used to apply downward force when the player releases the jump button

	// Animator variables
	private int anim_idleJump = Animator.StringToHash("IdleJump");
	private int anim_runningJump = Animator.StringToHash("RunningJump");
	private int anim_sprintJump = Animator.StringToHash("SprintJump");
	private int anim_doubleJump = Animator.StringToHash("DoubleJump");

	private Animator animator;
	private Rigidbody rb;
	private RomanCharController charController;
	private RomanCharState charState;

	// wallJumpLimiting
	private Ray wallJumpRayLeft, wallJumpRayRight;
	private RaycastHit hit;
	private float smoothDownvel;

	private static float lastJumpTime;								//Time stamp of the last time the char jumped
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		charController = GetComponent<RomanCharController>();
		charState = GetComponent<RomanCharState>();

		if (cColliderFrontTransf == null)
			Debug.LogError("collider front point not defined");

//		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {
//			{ GameEvent.StartWallClimbing, false }
//
//		});
	}


	private void OnEnable ()
	{
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
		if (charState.IsInAnyJumpingState())
		{
			LimitJump();
			RotateAndMoveJump();
		}
	}

	/// <summary>
	/// Apply a lerped downward force to the character
	/// when the player releases the jump button
	/// </summary>
	private void LimitJump()
	{
		if (InputController.jumpReleased && !charState.IsAnyDoubleJumping() && rb.velocity.y > 0) 
		{
			//print("Jump released");
			// Old downward force:  -rb.velocity.y/2
			downwardForce = Mathf.Lerp(0, maxDownwardForce, downwardForceSpeed * Time.fixedDeltaTime);
			rb.AddForce (new Vector3 (0, downwardForce, 0), ForceMode.Impulse);
		}

		// limit the zero out the x/z movement if idle jumping and there's no input
		// This is a fix the the PhysicMaterialHandler's handling of changing materials
		if (charState.IsIdleJumping() && charController.speed == 0)
      		rb.velocity = new Vector3(0, rb.velocity.y, 0);
  
		
	}

	private void RotateAndMoveJump()
	{
//		if (AntiWallSlideController.Instance.colliding) 
//		{
//		}
		if (charController.moveDirectionRaw != Vector3.zero)
		{
			rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(charController.moveDirectionRaw), allJumpTurnSpeed * Time.deltaTime));
			forwardVel = transform.forward * forwardSpeed * Mathf.Clamp01(charController.moveDirectionRaw.sqrMagnitude) * Time.deltaTime;

			if (AntiWallSlideController.Instance.onSloap && TimePassedSinceJump (0.5f)) {
				//forwardVel.y = Mathf.SmoothDamp(forwardVel.y, -200, ref smoothDownvel, 0.5f);
				rb.AddForce(0,-500, 0);
			}
			else
				forwardVel.y = rb.velocity.y;

			rb.velocity = forwardVel;
		}
	}

	// Check if character is colliding with wall
	// and that its less than 2 seconds since the jump
	// if so zero out the forward speed
//	private float GetForwardSpeed ()
//	{
//		if (AntiWallSlideController.Instance.colliding && Time.time > lastJumpTime + 1.0f)
//			 return 0;
//		else 
//			return forwardSpeed;	
//	}

	/// <summary>
	/// This method is called jumping animation events
	/// </summary>
	public void JumpUp()
	{
		//print ("runs");
		float force = 0;
		lastJumpTime = Time.time;
		if (charState.IsIdleJumping())
		{
			force = idleJumpUpForce;
			forwardSpeed = idleJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
		else if (charState.IsRunningJumping())
		{
			force = idleJumpUpForce;
			forwardSpeed = runningJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
		else if (charState.IsSprintJumping())
		{
			print("sprint jump");
			force = sprintJumpUpForce;									
			forwardSpeed = sprintJumpForwardSpeed;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}
	
	}

	public void DoubleJumpUp()
	{
		if (!hasDoubleJumped)
		{
			hasDoubleJumped = true;
			float force = doubleJumpForce;

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

		else if (gameEvent == GameEvent.Jump && charState.IsIdleOrMoving()) 
		{				
			EventManager.OnCharEvent(GameEvent.Jump);
			InputController.jumpReleased = false;

			//jumpTime = Time.time;

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
			//print("JumpController:LAND");
			hasDoubleJumped = false;
			animator.SetBool (anim_sprintJump, false);					        // Reset sprint jump trigger, Sometimes it gets stuck
			InputController.jumpReleased = false;
		}
		else if (gEvent == GameEvent.IsIdle)
		{
			InputController.jumpReleased = false;
		}
	}

	public static bool TimePassedSinceJump(float t)
	{
		return Time.time > lastJumpTime + t;
	}

//	private void OnCollisionStay (Collision Collision)
//	{
//		//Debug.DrawRay(cColliderFrontTransf.position, transform.forward * 0.3f, Color.white);
//	}

}
