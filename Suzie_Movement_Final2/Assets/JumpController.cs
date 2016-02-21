using UnityEngine;
using System.Collections;

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
	[Range(0,100)] 
	public float runningJumpForwardSpeed = 11.0f;	    // Maximum Y force to Apply to Rigidbody when jumping

	// Sprint Jumping --------------------------
	[Range(0,100)] 
	public float sprintJumpUpForce = 11.0f;				// Y force to Apply to Rigidbody when sprint jumping
	[Range(0,400)]
	public float sprintJumpForwardSpeed = 40f;

	[Range(0,100)]
	public float doubleJumpForce = 10.0f;		

	[Range(0,50)]
	public float allJumpTurnSpeed = 30.56f;			    // Turning speed used for all jumps
	


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
		if (InputController.jumpReleased && charState.IsInAnyJumpingState() && !charState.IsAnyDoubleJumping())
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
		if (charController.moveDirectionRaw != Vector3.zero && charState.IsInAnyJumpingState())
		{
			rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(charController.moveDirectionRaw), allJumpTurnSpeed * Time.deltaTime));
			forwardVel = transform.forward * forwardSpeed * Mathf.Clamp01(charController.moveDirectionRaw.sqrMagnitude) * Time.deltaTime;
			forwardVel.y = rb.velocity.y;
			rb.velocity = forwardVel;
		}
	}

	/// <summary>
	/// This method is called jumping animation events
	/// </summary>
	public void JumpUp()
	{
		//print ("runs");
		float force = 0;

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
