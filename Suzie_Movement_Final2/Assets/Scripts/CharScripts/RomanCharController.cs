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

	[HideInInspector]
	public float speed;					// Temp var for locomotion 
		
	/*----------------------------------------------------------|
	| PRIVATE VARIABLES			    	                        |
	|----------------------------------------------------------*/

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private Transform cam;
	private CharacterController cController;
	private CapsuleCollider cCollider;
	[HideInInspector]
	public TunnelObserver tunnelObserver;
	public CombatController combatController;	
	//public bool inTube = false;

	// Character rotation -------------
	private Vector3 camForward;
	private VineClimbController2 vineClimbCollider;

	// Animator hashes - for optimization
	int anim_Speed = Animator.StringToHash("Speed");
	int anim_Falling = Animator.StringToHash("Falling");
	int anim_sprintModDown = Animator.StringToHash("SprintModDown");
	int anim_WallBounce = Animator.StringToHash("WallBounce");


	void Start ()
	{
		if (GameManager.componentActivatorOn) 
		{
			ComponentActivator.Instance.Register (this, new Dictionary<GameEvent, bool> { 

				//{ GameEvent.Land, true },
				{ GameEvent.ClimbOverEdge, true },
				{ GameEvent.StopClimbing, true },
				{ GameEvent.FinishClimbOver, true },
				//{ GameEvent.Land, true },

				//{ GameEvent.LandedFirstTime, false },
				{ GameEvent.StartVineClimbing, false }, 
				{ GameEvent.StartEdgeClimbing, false },
				{ GameEvent.StartWallClimbing, false }

			});
		}

		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		cController = GetComponent<CharacterController>();
		cCollider = GetComponent<CapsuleCollider>();
		vineClimbCollider = GetComponent<VineClimbController2>();
		tunnelObserver = GameManager.Instance.tunnelObserver;
		combatController = GetComponent<CombatController> ();
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
		{
			animator.SetFloat (anim_Speed, speed + 2);
			//print("should sprint");
		}
		else if (speed != 0)
		{
			animator.SetFloat (anim_Speed, Mathf.Clamp01(speed), walkToRunDampTime, Time.fixedDeltaTime);
		}
		else
		{
			animator.SetFloat (anim_Speed, 0);
		}

		// Stop sprinting
		if (charState.IsSprinting() && speed == 0 && !tunnelObserver.inTunnel)
		{
			print("Get out of sprint mode");
			animator.SetBool(anim_sprintModDown, false);
		}

		//print (speed);
	}


	/// <summary>
	/// handle character rotation during running
	/// Also, add some force while while sprinting so that the character goes faster
	/// </summary>
	private void OnAnimatorMove ()
	{
		// If is idle or is running and not sprinting)
		if (charState.IsIdleOrMoving ()) {
			//print ("is moving"); 
			/*if (combatController.state == CombatController.SquirrelCombatState.Punching || charState.IsIdle()) 
			{
				animator.ApplyBuiltinRootMotion ();
				//print ("In combat state"); 
			}
			else*/
			if (moveDirectionRaw != Vector3.zero) {
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (moveDirectionRaw), runRotateSpeed * Time.deltaTime);

				if (!charState.IsIdle () && !SloapDetector.Instance.onSloap)
					animator.ApplyBuiltinRootMotion ();
				//else if (charState.IsIdle())
				//transform.rotation = Quaternion.LookRotation(new Vector3(cam.forward.x, 0, cam.forward.z));
				//print("In OnAnimatormMove");
			}
			// Character has stopped, do a lerp
			else {
				rb.velocity = Vector3.Lerp (rb.velocity, Vector3.zero, stopRunningSmoothingSpeed * Time.deltaTime);
			}
				
		} 
//		else 
//		{
//			print ("squirrel not moving. state: " + charState.GetState ()); 	
//		}
    }

	/*----------------------------------------------------------|
	| CUSTOM EVENTS						                        |
	|----------------------------------------------------------*/

	private void OnEnable ()
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
			print("mod down");
			//if (charState.IsIdleOrRunning() || charState.IsIdleOrRunningJumping())
			//{
				animator.SetBool(anim_sprintModDown, true);

			//}
		}
	}

	private void SprintModifierUp(GameEvent gEvent)
	{
		if (/*(charState.IsSprinting() || charState.IsSprintJumping()) && */gEvent == GameEvent.SprintModifierUp && !tunnelObserver.inTunnel)
		{
			animator.SetBool(anim_sprintModDown, false);
			print("sprint mod UP");
		}	
	}

	// Handle the collider size and position change when starting/stopping sprinting
	private void Sprint (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StartSprinting)
			RSUtil.OrientCapsuleCollider(cCollider, false);
		
		else if (gEvent == GameEvent.StopSprinting)
			RSUtil.OrientCapsuleCollider(cCollider, true);
	}

	private void EnterIdle (GameEvent gEvent)
	{		

		if (gEvent == GameEvent.IsIdle)
		{
			rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rb.velocity = Vector3.zero;

			//transform.up = Vector3.up;
			RSUtil.OrientCapsuleCollider(cCollider, true);
		}
	}
	
	private void ExitIdle (GameEvent gEvent)
	{
		if (gEvent == GameEvent.ExitIdle)
		{
			//print ("exit idle");
			//rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
		}
	}
	
	private void CharLanded (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			RSUtil.OrientCapsuleCollider(cCollider, true);
			ResetYRotation();
		
			if (!charState.IsFalling())
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

	/*----------------------------------------------------------|
	| UNITY EVENTS							                    |
	|----------------------------------------------------------*/	

//	private void OnTriggerEnter(Collider col)
//	{
//		if (col.gameObject.layer == 13)
//			EventManager.OnCharEvent(GameEvent.EnterTunnel);
//	}
//
//	// Happens while character stays in a tube
//	private void OnTriggerStay(Collider col)
//	{
//		if (col.gameObject.layer == 13)
//			inTube = true;
//	}
//
//	// Tube exit trigger
//	private void OnTriggerExit(Collider col)
//	{
//		if (col.gameObject.layer == 13)
//		{
//			inTube = false;
//			EventManager.OnCharEvent(GameEvent.ExitTunnel);
//		}
//	}


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
