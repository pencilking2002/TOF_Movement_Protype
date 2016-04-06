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

	[Range(0,10)]
	public float vMovementSpeed = 6.0f;					// The vertical move speed of the char while climbing
	[Range(0,10)]
	public float hMovementSpeed = 6.0f;					// The speed to move the game object

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

	// Animator hashes
	int anim_hWallClimbSpeed = Animator.StringToHash("hWallClimbSpeed");
	int anim_vWallClimbSpeed = Animator.StringToHash("vWallClimbSpeed");
	int anim_ClimbOverEdge = Animator.StringToHash("ClimbOverEdge");

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
	void FixedUpdate () 
	{
		//Debug.DrawRay(cCollider.bounds.center, transform.forward * 0.5f, Color.blue);
		//Debug.Break ();
		//cast the ray 5 units at the specified direction  	
		if(charState.IsWallClimbing () && 
		   Physics.Raycast(cColliderFrontTransf.position, transform.forward, out hit, 0.7f, layerMask) &&
		   cController.enabled)
		{ 

			// Get the vertical center of the Squirrel
//			Vector3 center = cCollider.bounds.center;
//			center = center + transform.forward * cCollider.radius;

			// Get the hit point of the the climb collider but use the player's Y center
			Vector3 hitPoint = new Vector3(hit.point.x, cColliderFrontTransf.position.y, hit.point.z);
			Debug.DrawLine(cColliderFrontTransf.position, hitPoint, Color.blue);

			// If the character is further away then the threshold
			// Calculate the gravity. Otherwise set it to zero
			gravity = Vector3.Distance(cColliderFrontTransf.position, hitPoint);

			if (gravity < gravityThreshold || (InputController.h == 0 && InputController.v == 0))
				gravity = 0;
			

			// If the current transform's forward z value has passed the threshold test
			// smoothly match the player's forward with the inverse of the normal
			if(oldNormal.z >= transform.forward.z + normalThreshold || oldNormal.z <= transform.forward.z - normalThreshold)
				transform.forward = Vector3.SmoothDamp(transform.forward, -hit.normal, ref normalVel, 0.3f);
			
			//store the current hit.normal inside the oldNormal
			oldNormal = -hit.normal;

			moveDirection = new Vector3(InputController.h * hMovementSpeed, InputController.v * vMovementSpeed, gravity)  * Time.deltaTime;
			moveDirection = transform.TransformDirection(moveDirection);
			//moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
			//transform.position = hit.point + -transform.forward * 0f;

			cController.Move(moveDirection);

			animator.SetFloat(anim_hWallClimbSpeed, InputController.rawH, 0.02f, Time.deltaTime);
			animator.SetFloat(anim_vWallClimbSpeed, InputController.rawV, 0.02f, Time.deltaTime);

			if (cColliderFrontTransf.position.y > RSUtil.GetTopColliderTopYPoint(hit.collider))
			{
				print("above collider");
				EventManager.OnCharEvent(GameEvent.ClimbOverEdge);
			}

		}

	}

//	private void FixedUpdate()
//	{
//		if (rb.velocity.y < 0 && Physics.Raycast()
//		{
//			Game
//		}
//	}

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
			EventManager.OnCharEvent(GameEvent.StartClimbing);
			rb.velocity = Vector3.zero;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			rb.useGravity = false;
			animator.SetTrigger("WallClimb");
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
		initialWallClimbingPos += transform.forward * (dist - 0.83f);

		//Debug.Break ();	
		transform.position = initialWallClimbingPos;
	}

	private void StopClimbing (GameEvent gEvent)
	{
		if (!charState.IsWallClimbing())
			return;

		if (gEvent == GameEvent.StopClimbing )
		{
			climbReleased = true;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			rb.isKinematic = false;
			rb.useGravity = true;
			print("allClimbCOntroller: StopClimbing");
		}
		else if (gEvent == GameEvent.ClimbOverEdge)
		{
			animator.SetTrigger(anim_ClimbOverEdge);
	
			Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z) + transform.forward * 0.5f;
			Vector3 targetPos2 = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z) + transform.forward * 1f;
			LeanTween.move(gameObject, targetPos, 1.2f)

				.setOnComplete(() => {
					LeanTween.move(gameObject, targetPos2, 0.8f)
						.setOnComplete(() => {
							climbReleased = true;
							cController.enabled = false;
							rb.isKinematic = false;
							rb.useGravity = true;
							EventManager.OnCharEvent(GameEvent.FinishClimbOver);
							EventManager.OnCharEvent(GameEvent.IsIdle);
						});
				});

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

