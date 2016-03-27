using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallClimbController : MonoBehaviour {
	public Transform cColliderFrontTransf;
	public LayerMask layerMask;
	public float gravity = 50.0f;
	public float gravityMultiplier = 20.0f;
	public float gravityThreshold = 0.01f;				// Used to calculate the point at which to stop applying gravity
	public float normalThreshold = 0.009f;				// The threshold, to discard some of the normal value variations
	public float movementSpeed = 6.0f;					// the speed to move the game object
	public float wallClimbPosOffset = -0.5f;			// Helps with positioning Squirrel closer to climb collider when beginning to wall climb
	public float centerOffset = 0.68f;

	private RaycastHit hit;						
	private Vector3 oldNormal;							//a class to store the previous normal value

	private Rigidbody rb;
	private Animator animator;
	private CharacterController cController;
	private RomanCharState charState;
	private CapsuleCollider cCollider;

	private Vector3 normalVel;							// Vel for smooth damping to a different forward/normal rotation
	private Vector3 moveDirection = Vector3.zero;		//the direction to move the character
	private Vector3 initialWallClimbingPos = Vector3.zero;
	private Vector3 initialClimbCenterPos = Vector3.zero;
	private bool climbReleased = false;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		cController = GetComponent<CharacterController>();
		charState = GetComponent<RomanCharState>();
		cCollider = GetComponent<CapsuleCollider>();

		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.WallClimbColliderDetected, true},

			//{ GameEvent.StopClimbing, false },
			{ GameEvent.FinishClimbOver, false },
			{ GameEvent.Land, false }

		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!charState.IsWallClimbing ())
			return;

		Debug.DrawRay(cCollider.bounds.center,  transform.forward * 0.5f, Color.blue);

		//Debug.Break ();
		//cast the ray 5 units at the specified direction  	
		if(Physics.Raycast(cCollider.bounds.center, transform.forward, out hit, 0.5f, layerMask))
		{ 
			// Get the vertical center of the Squirrel
			Vector3 center = cCollider.bounds.center;
			center = center + transform.forward * cCollider.radius;

			// Get the hit point of the the climb collider but use the player's Y center
			Vector3 hitPoint = new Vector3(hit.point.x, center.y, hit.point.z);
			Debug.DrawLine(center, hitPoint, Color.blue);


			// If the character is further away then the threshold
			// Calculate the gravity. Otherwise set it to zero
			gravity = Vector3.Distance(center, hitPoint);

			// If the character is to close to the climbing mesh or if there is no horizontal input
			// Zero out the gravity
			if (gravity < gravityThreshold || InputController.h == 0)
				gravity = 0;
			
			//if the current transform's forward z value has passed the threshold test
			if(oldNormal.z >= transform.forward.z + normalThreshold || oldNormal.z <= transform.forward.z - normalThreshold)
			{
				//smoothly match the player's forward with the inverse of the normal
				//transform.forward = Vector3.Lerp (transform.forward, -hit.normal, rotLerpSpeed * Time.deltaTime);
				transform.forward = Vector3.SmoothDamp(transform.forward, -hit.normal, ref normalVel, 0.3f);
			}
			//store the current hit.normal inside the oldNormal
			oldNormal = -hit.normal;
		}
		else
		{
			EventManager.OnCharEvent(GameEvent.StopWallClimbing);
			StopClimbing(GameEvent.StopWallClimbing);
		}

		moveDirection = new Vector3(InputController.h * movementSpeed, InputController.v * movementSpeed, gravity * gravityMultiplier)  * Time.deltaTime;
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection = Vector3.ClampMagnitude(moveDirection, 0.028f);

		//animator.SetFloat("HorEdgeClimbDir", InputController.rawH, 0.02f, Time.deltaTime);

		if (cController.enabled)
			cController.Move(moveDirection); //cController.Move(finalDir); 
		
	}

	// EVENTS -----------------------------------------------------------
	private void OnEnable () 
	{ 
		EventManager.onDetectEvent += InitWallClimb;
		EventManager.onCharEvent += StopClimbing;
		EventManager.onInputEvent += StopClimbing;
		EventManager.onCharEvent += Land;
	}
	private void OnDisable () 
	{ 
		
		EventManager.onDetectEvent -= InitWallClimb;
		EventManager.onCharEvent -= StopClimbing;
		EventManager.onInputEvent -= StopClimbing;
		EventManager.onCharEvent -= Land;
	}

	private void InitWallClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.WallClimbColliderDetected && rb.velocity.y < 0 && climbReleased == false)
		{
			//print ("init wall climb");
			InputController.h = 0;
			cController.enabled = true;
			EventManager.OnCharEvent(GameEvent.StartWallClimbing);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			animator.SetTrigger("WallClimb");
			//Debug.Break ();
			// Posiiton and orient the player according to the hit point
			SetPlayerPosition(hit);
			//SetPlayerOrientation (hit);
		}
	}

	/// <summary>
	/// Position the Squirrel closer to the wall collider
	/// </summary>
	/// <param name="hit">Hit.</param>
	private void SetPlayerPosition(RaycastHit hit)
	{

		//DebugSphere (initialClimbCenterPos);

		initialWallClimbingPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
		float dist = Vector3.Distance (hit.point, cColliderFrontTransf.position);
		initialWallClimbingPos += transform.forward * (dist - 0.7f);

		//Debug.Break ();	
		transform.position = initialWallClimbingPos;
	}

	private void StopClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopClimbing && charState.IsWallClimbing())
		{
			climbReleased = true;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			rb.isKinematic = false;
			print("allClimbCOntroller: StopClimbing");
		}
	}

	private void DebugSphere (Vector3 pos)
	{
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere.transform.position = pos;
		sphere.transform.localScale = Vector3.one * 0.2f;
		Debug.Break ();
	}

	/// <summary>
	/// Reeset rclimbReleased when Squirrel lands
	/// </summary>
	/// <param name="gEvent">G event.</param>
	private void Land (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
			climbReleased = false;
	}

//	private void OnDrawGizmos()
//	{
//		if (initialWallClimbingPos != Vector3.zero) 
//		{
//			Gizmos.color = Color.red;
//			Gizmos.DrawSphere (initialClimbCenterPos, 0.5f);
//			Debug.Break ();	
//		}
//	}
}

