using UnityEngine;
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
	private float forwardSpeed; 			// Temp var for forward speed
	private float lastJumpTime;
	private float speed;					// Temp var for locomotion 
	private Vector3 vel;					// Temp vector for calculating forward velocity while jumping
	private float lerpedJumpForce;
	private Vector3 idlePos;
	private bool hasDoubleJumped = false;

	private int anim_idleJump = Animator.StringToHash("IdleJump");
	private int anim_runningJump = Animator.StringToHash("RunningJump");
	private int anim_sprintJump = Animator.StringToHash("SprintJump");
	private int anim_doubleJump = Animator.StringToHash("DoubleJump");

	private Animator animator;
	private Rigidbody rb;
	private RomanCharController charController;
	private RomanCharState charState;

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
//		if (InputController.jumpReleased && charState.IsJumping() /*&& !charState.IsSprintJumping()*/)
//		{
//			InputController.jumpReleased = false;
//
//			if (rb.velocity.y > 0)
//			{
//				rb.AddForce (new Vector3 (0, -5, 0), ForceMode.Impulse);
//			}
//		}
	}

	private void RotateAndMoveJump()
	{
		if (charController.moveDirectionRaw != Vector3.zero)
		{
			rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(charController.moveDirectionRaw), jumpTurnSpeed * Time.deltaTime));
			vel = transform.forward * forwardSpeed * Mathf.Clamp01(charController.moveDirectionRaw.sqrMagnitude) * Time.deltaTime;
			vel.y = rb.velocity.y;
			rb.velocity = vel;
		}
	}

	public void JumpUp()
	{
		float force = maxJumpForce;
		print("called by animation");
		forwardSpeed = idleJumpForwardSpeed;
		charState.SetState(RomanCharState.State.IdleJumping);
		rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
	}

	private void Jump (GameEvent gameEvent)
	{
		print (gameEvent);
		if (gameEvent == GameEvent.Jump && charState.IsJumping() && !hasDoubleJumped)
		{
			hasDoubleJumped = true;
			float force = maxDoubleJumpForce;

			print("Do double jump");
			animator.SetTrigger(anim_doubleJump);
			rb.velocity = Vector3.zero;
			rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
		}

		else if (gameEvent == GameEvent.Jump && charState.IsIdleOrRunning()) 
		{	
			print("do it");
			float force = maxJumpForce;
			
			//EventManager.OnCharEvent(GameEvent.DetachFollow);
			EventManager.OnCharEvent(GameEvent.Jump);
			
			//print (charState.GetState ());
			
			// Change the forward speed based on what kind of jump it is
			if (charState.IsIdle())
			{
				//forwardSpeed = idleJumpForwardSpeed;
				//charState.SetState(RomanCharState.State.IdleJumping);
				animator.SetTrigger (anim_idleJump);
				//rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
			}
			else if (charState.IsJogging())
			{
				forwardSpeed = runningJumpForwardSpeed;
				charState.SetState(RomanCharState.State.RunningJumping);
				animator.SetTrigger (anim_runningJump);
				rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
			}
			else if (charState.IsSprinting())
			{
				// Override Y jump force to be a constant value when sprinting
				force = sprintJumpForce;									
				forwardSpeed = sprintJumpForwardSpeed;
				charState.SetState(RomanCharState.State.SprintJumping);
				animator.SetBool (anim_sprintJump, true);
				
				charController.OrientCapsuleCollider(false);
				rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
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
