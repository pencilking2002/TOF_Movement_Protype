using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class RomanCharController : MonoBehaviour {
	
	/*----------------------------------------------------------|
	| PUBLIC VARIABLES			    	                        |
	|----------------------------------------------------------*/

	// movement

	public float walkToRunDampTime = 1f;				// Damping to control how the speed of the "Locomotion" blend tree
	public float runRotateSpeed = 10f;
	public float stopRunningSmoothingSpeed = 2.5f;		// Speed to control how fast the character slides after they stop running

	[HideInInspector]
	public Vector3 moveDirectionRaw;					// The direction/displacement the character will move in
		
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
	private Quaternion targetRot;				// the target rotation to achieve while in idle or running
	public bool inTube = false;

	// Character rotation -------------
	private Vector3 camForward;
	private Quaternion camRot;
	private float speed;					// Temp var for locomotion 
	private Vector3 idlePos;

	private VineClimbController2 vineClimbCollider;

	// Animator hashes - for optimization
	int anim_Speed = Animator.StringToHash("Speed");
	int anim_Falling = Animator.StringToHash("Falling");
	int anim_sprintModDown = Animator.StringToHash("SprintModDown");

	void Start ()
	{
		RegisterEvents();

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
		if (animator.GetBool(anim_sprintModDown) && speed > 0)
			animator.SetFloat (anim_Speed, speed + 2);
		
		else if (speed != 0)
			animator.SetFloat (anim_Speed, Mathf.Clamp01(speed), walkToRunDampTime, Time.fixedDeltaTime);
		
		else
			animator.SetFloat (anim_Speed, 0);

		// Stop sprinting
		if (charState.IsSprinting() && speed == 0 && !inTube)
		{
			print("Get out of sprint mode");
			animator.SetBool(anim_sprintModDown, false);
		}
	}


	/// <summary>
	/// handle character rotation during running
	/// Also, add some force while while sprinting so that the character goes faster
	/// </summary>
	private void OnAnimatorMove ()
	{
		// If is idle or isrunning and not sprinting)
		if (charState.IsIdleOrRunning())
		{
			if (moveDirectionRaw != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), runRotateSpeed * Time.deltaTime);

				if (charState.IsRunning())
					animator.ApplyBuiltinRootMotion();
			}
			// Character has stopped, do a lerp
			else
			{
				rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, stopRunningSmoothingSpeed * Time.deltaTime);
			}
				
		}   
    }

	/*----------------------------------------------------------|
	| CUSTOM EVENTS						                        |
	|----------------------------------------------------------*/

	private void RegisterEvents ()
	{
		EventManager.onInputEvent += SprintModifierDown;
		EventManager.onInputEvent += SprintModifierUp;
		EventManager.onCharEvent += Enable;
		EventManager.onCharEvent += Sprint;
		EventManager.onCharEvent += EnterIdle;
		EventManager.onCharEvent += ExitIdle;
		EventManager.onCharEvent += CharLanded;
	}

	private void OnDisable () 
	{ 
		EventManager.onInputEvent -= SprintModifierDown;
		EventManager.onInputEvent -= SprintModifierUp;
		EventManager.onCharEvent -= Enable;
		EventManager.onCharEvent -= Sprint;
		EventManager.onCharEvent -= EnterIdle;
		EventManager.onCharEvent -= ExitIdle;
		EventManager.onCharEvent -= CharLanded;
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


	private void SprintModifierDown(GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.SprintModifierDown)
		{
			if (charState.IsIdleOrRunning() || charState.IsJumping())
				animator.SetBool(anim_sprintModDown, true);
		}
	}

	private void SprintModifierUp(GameEvent gEvent)
	{
		if (charState.IsSprinting() && gEvent == GameEvent.SprintModifierUp && !inTube)
			animator.SetBool(anim_sprintModDown, false);	
	}

	// Handle the collider size and position change when starting/stopping sprinting
	private void Sprint (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StartSprinting)
			OrientCapsuleCollider(false);
		
		else if (gEvent == GameEvent.StopSprinting)
			OrientCapsuleCollider(true);
	}

	private void EnterIdle (GameEvent gEvent)
	{
		if (gEvent == GameEvent.IsIdle)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete; 
			rb.velocity = Vector3.zero;
			OrientCapsuleCollider(true);
			idlePos = transform.position;
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
			OrientCapsuleCollider(true);
			ResetYRotation();
		
			if (!charState.IsSprintFalling())
				animator.SetBool (anim_sprintModDown, false);					// Reset sprint modifer trigger, Sometimes it gets stuck
		}
	}
	
	public void ResetYRotation()
	{
		Vector3 newRotation = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
		transform.localEulerAngles = newRotation;
	}
	
	/*----------------------------------------------------------|
	| PUBLIC METHODS						                    |
	|----------------------------------------------------------*/

	/// <summary>
	/// Used in the animator to turn root motion on and off
	/// </summary>
	/// <param name="apply">If set to <c>true</c> apply.</param>
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

	/*----------------------------------------------------------|
	| UNITY EVENTS							                    |
	|----------------------------------------------------------*/	

	// Happens while character stays in a tube
	private void OnTriggerStay(Collider col)
	{
		if (col.CompareTag("Tube") && charState.IsSprinting())
			inTube = true;
	}

	// Tube exit trigger
	private void OnTriggerExit(Collider col)
	{
		if (col.CompareTag("Tube") && charState.IsSprinting())
			inTube = false;
	}


	// Trigger the falling animation when the character "falls" of an edge
	private void OnCollisionExit ()
	{
		Vector3 startPos = transform.position + new Vector3(0, 0.3f, 0);
		if ((charState.IsRunning() || charState.IsIdle()) && rb.velocity.y < 0 && !Physics.Raycast (startPos, Vector3.down, 0.5f))
		{
			animator.SetTrigger (anim_Falling);
		}

	}

	
}
