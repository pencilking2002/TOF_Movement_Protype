using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class RomanCharController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------

	public float idleRotateSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
	public float speedDampTime = 0.05f;
	public float walkToRunDampTime = 1f;

	public float maxRunningRotation = 20f;
	public float runRotateSpeed = 10f;
	public float sprintForce = 80.0f;
	public float runForce = 50.0f;
	public float stopRunningSmoothingSpeed = 2.5f;

	[Header("JUMPING")]
	[Range(0,100)]
	public float maxJumpForce = 0.8f;					// Maximum Y force to Apply to Rigidbody when jumping
	
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
		
	//---------------------------------------------------------------------------------------------------------------------------
	//	Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private Transform cam;
	private CharacterController cController;
	private CapsuleCollider cCollider;
		
	private float dir;							// The  result of the cross product of the player's rotation and the camera's rotation
	
	private Vector3 moveDirectionRaw;			// The direction/displacement the character will move in
	private Quaternion targetRot;				// the target rotation to achieve while in idle or running
	public bool inTube = false;

	// Character rotation -------------
	private Vector3 camForward;
	private Quaternion camRot;
	
	// jumping ----------------------
	private float forwardSpeed; 			// Temp var for forward speed
	//private bool holdShift = false;
	private float speed;					// Temp var for locomotion 
	private Vector3 vel;					// Temp vector for calculating forward velocity while jumping

	private Vector3 idlePos;

	private VineClimbController2 vineClimbCollider;

	// Animator hashes - for optimization
	int anim_Speed = Animator.StringToHash("Speed");
	int anim_Falling = Animator.StringToHash("Falling");
//	int anim_Land = Animator.StringToHash("Land");
	int anim_sprintModDown = Animator.StringToHash("SprintModDown");
	int anim_idleJump = Animator.StringToHash("IdleJump");
	int anim_runningJump = Animator.StringToHash("RunningJump");
	int anim_sprintJump = Animator.StringToHash("SprintJump");
	
	void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.Land, true },
			{ GameEvent.StopVineClimbing, true },
			{ GameEvent.StopEdgeClimbing, true },
			{ GameEvent.FinishClimbOver, true },

			{ GameEvent.StartVineClimbing, false }, 
			{ GameEvent.StartEdgeClimbing, false },

		});

		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		cController = GetComponent<CharacterController>();
		cCollider = GetComponent<CapsuleCollider>();
		vineClimbCollider = GetComponent<VineClimbController2>();
	}


	/// <summary>
	/// Physics Update
	/// </summary>
	private void FixedUpdate ()
	{
		// Get a move direction based on the user's input
		moveDirectionRaw = new Vector3(InputController.rawH, 0, InputController.rawV);

		// Convert the move direction to be reative to the camera
		moveDirectionRaw = Quaternion.LookRotation(new Vector3(cam.forward.x, 0, cam.forward.z)) * moveDirectionRaw;

		// Base the animation speed on the length of the move direction's vector
		speed = Mathf.Clamp01(moveDirectionRaw.sqrMagnitude);


		// set the character's movement if the move stick is pressed
		if (InputController.leftStickPressed)
			animator.SetFloat (anim_Speed, Mathf.Clamp01(speed), walkToRunDampTime, Time.fixedDeltaTime);
		
		else
			animator.SetFloat (anim_Speed, 0);

	}

	/// <summary>
	/// handle character rotation during running
	/// Also, add some force while while sprinting so that the character goes faster
	/// </summary>
	private void OnAnimatorMove ()
	{
		if ((charState.IsIdleOrRunning()))
		{
			if (moveDirectionRaw != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), runRotateSpeed * Time.fixedDeltaTime);

				animator.ApplyBuiltinRootMotion();
			}
			else
			{
				rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, stopRunningSmoothingSpeed * Time.deltaTime);
			}
				
		}   
    }



	/// <summary>
	/// - Get a vector of the camera's forward direction
	/// - Create a rotation based on the forward direction of the camera
	/// - Move moveDirectionRaw be in reference to the camera rotation so that if we go straight for example
	/// the character will also go straight
	/// </summary>
	private void TurnCharToCamera()
	{
//		camForward = new Vector3(cam.forward.x, 0, cam.forward.z);
//		camRot = Quaternion.LookRotation(camForward);
//		moveDirectionRaw = camRot * moveDirectionRaw;
		moveDirectionRaw = Quaternion.LookRotation(new Vector3(cam.forward.x, 0, cam.forward.z)) * moveDirectionRaw;
	
	}
	
	/// <summary>
	/// Get the character to perform the landing animation when he/she hits the floor
	/// the check against IsLanding() is important so that this event only fires once, 
	/// as opposed to firing multiple times while the character is landing
	/// </summary>
	/// <param name="coll">Coll.</param>
//	private void OnCollisionStay (Collision coll)
//	{
//		if (charState.IsFalling() && !charState.IsLanding() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f )
//		{
//			animator.SetTrigger("Land");
//			EventManager.OnCharEvent(GameEvent.AttachFollow);
//			EventManager.OnCharEvent(GameEvent.Land);			
//		}
//	}

//	private void OnCollisionEnter (Collision coll)
//	{
//		if (Vector3.Dot(coll.contacts[0].normal, -transform.forward) > 0.5f)
//		{
//			EventManager.OnCharEvent(GameEvent.WallCollision);
//			Debug.DrawLine(transform.position, coll.contacts[0].normal, Color.red);
//		}
//	}
//
	/// <summary>
	/// Force animator to trigger the falling animation 
	/// on collision exit from a collider that is below the player
	/// </summary>
	private void OnCollisionExit ()
	{
		Vector3 startPos = transform.position + new Vector3(0, 0.3f, 0);
		if ((charState.IsRunning() || charState.IsIdle()) && rb.velocity.y < 0 && !Physics.Raycast (startPos, Vector3.down, 0.5f))
		{
			animator.SetTrigger (anim_Falling);
		}

	}


	// Events --------------------------------------------------------------------------------------------------------------------------------
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		print("char enable");
		EventManager.onInputEvent += PerformJump;
//		EventManager.onInputEvent += SprintModifierDown;
//		EventManager.onInputEvent += SprintModifierUp;
//
//		EventManager.onCharEvent += Enable;
//		//EventManager.onCharEvent += Disable;
//		EventManager.onCharEvent += Sprint;
//		EventManager.onCharEvent += CharIdle;
//		EventManager.onCharEvent += ExitIdle;
//		EventManager.onCharEvent += CharLanded;
		
	}
	private void OnDisable () 
	{ 
		print("char disable");	
		EventManager.onInputEvent -= PerformJump;
//		EventManager.onInputEvent -= SprintModifierDown;
//		EventManager.onInputEvent -= SprintModifierUp;
//
//		EventManager.onCharEvent -= Enable;
//		//EventManager.onCharEvent -= Disable;
//		EventManager.onCharEvent -= Sprint;
//		EventManager.onCharEvent -= CharIdle;
//		EventManager.onCharEvent -= ExitIdle;
//		EventManager.onCharEvent -= CharLanded;
		
	}
	
	private void Enable (GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Land || gameEvent == GameEvent.IsIdle)
		{
			cController.enabled = false;
			rb.isKinematic = false;
			vineClimbCollider.detached = false;
		}
	}

	// Trigger the jump animation and disable root motion
	public void PerformJump (GameEvent gameEvent)
	{
		print("jump");
//		if (gameEvent == GameEvent.Jump && (charState.IsIdle() || charState.IsRunning())) 
//		{	
//			
//			float force = maxJumpForce;
//			
//			EventManager.OnCharEvent(GameEvent.DetachFollow);
//			EventManager.OnCharEvent(GameEvent.Jump);
//			
//			//print (charState.GetState ());
//			
//			// Change the forward speed based on what kind of jump it is
//			if (charState.IsIdle())
//			{
//				forwardSpeed = idleJumpForwardSpeed;
//				charState.SetState(RomanCharState.State.IdleJumping);
//				animator.SetTrigger (anim_idleJump);
//				rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
//			}
//			else if (charState.IsJogging())
//			{
//				forwardSpeed = runningJumpForwardSpeed;
//				charState.SetState(RomanCharState.State.RunningJumping);
//				animator.SetTrigger (anim_runningJump);
//				rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
//			}
//			else if (charState.IsSprinting())
//			{
//				// Override Y jump force to be a constant value when sprinting
//				force = sprintJumpForce;									
//				forwardSpeed = sprintJumpForwardSpeed;
//				charState.SetState(RomanCharState.State.SprintJumping);
//				animator.SetBool (anim_sprintJump, true);
//				
//				OrientCapsuleCollider(false);
//				rb.AddForce (new Vector3 (0, force, 0), ForceMode.Impulse);
//			}
	

//		}
	}

	private void SprintModifierDown(GameEvent gameEvent)
	{
		if (charState.IsIdle() || charState.IsRunning() || charState.IsJumping())
		{
			if (gameEvent == GameEvent.SprintModifierDown)
			{
				//holdShift = true;
				animator.SetBool(anim_sprintModDown, true);
			}
		}
	}

	private void SprintModifierUp(GameEvent gEvent)
	{
		if (charState.IsSprinting() && gEvent == GameEvent.SprintModifierUp && !inTube)
		{
			animator.SetBool(anim_sprintModDown, false);	
		}
	}
	// Handle the collider size and position change when starting/stopping sprinting
	private void Sprint (GameEvent gEvent)
	{
		if(gEvent == GameEvent.StartSprinting)
		{
			OrientCapsuleCollider(false);
		}
		
		else if(gEvent == GameEvent.StopSprinting)
		{	
			OrientCapsuleCollider(true);
	
		}
	}

	private void CharIdle (GameEvent gEvent)
	{
		if (gEvent == GameEvent.IsIdle)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete; 
			rb.velocity = Vector3.zero;
			OrientCapsuleCollider(true);
			idlePos = transform.position;

			//SetPhysicMaterial(true);
			//print ("idle");
		}
	}
	
	private void ExitIdle (GameEvent gEvent)
	{
		if (gEvent == GameEvent.ExitIdle)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
		}
	}
	
	private void CharLanded (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			//print ("CHAR LANDING");
			OrientCapsuleCollider(true);
			ResetYRotation();

			animator.SetBool (anim_sprintJump, false);					        // Reset sprint jump trigger, Sometimes it gets stuck

			if (!charState.IsSprintFalling())
				animator.SetBool (anim_sprintModDown, false);					// Reset sprint modifer trigger, Sometimes it gets stuck

		}
	}
	
	private void ResetYRotation()
	{
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	// Enable Root motion
	public void ApplyRootMotion (bool apply)
	{
		animator.applyRootMotion = apply;
	}
	
	/// <summary>
	/// Orient the capsule collider based on what the character is doing
	/// So when the character is sprinting or sprint jumping, make the collider
	/// Horizontal
	/// </summary>
	public void OrientCapsuleCollider (bool upright)
	{
		if (upright)
		{
			cCollider.direction = 1;
			cCollider.center = new Vector3(cCollider.center.x, 0.47f, cCollider.center.z);
			
		}
		else
		{
			// Adjust the collider during sprinting
			cCollider.direction = 2;
			cCollider.center = new Vector3(cCollider.center.x, 0.3f, cCollider.center.z);
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Tube") && charState.IsSprinting())
		{
			print("enter tube");
			inTube = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Tube") && charState.IsSprinting())
		{
			print("exit tube");

			inTube = false;
		}
	}

	
}
