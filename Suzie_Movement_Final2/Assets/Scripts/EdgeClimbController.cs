using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeClimbController : MonoBehaviour 
{
	[Range(-5.0f,5.0f)]
	public float climbSpotYOffset = 1.0f;
	
	[Range(-5.0f,5.0f)]
	public float climbSpotZOffset = 1.0f;
	
	// the speed to move the game object
	public float speed = 6.0f;
	
	// Gravity pulling the player into the climb collider
	public float gravity = 50.0f;
	public float gravityMultiplier = 20.0f;

	// The threshold, to discard some of the normal value variations
	public float threshold = 0.009f;

	//public Transform leftHand;
	private CapsuleCollider cCollider;
	private Animator animator;
	private CharacterController cController;
	private Rigidbody rb;
	private RomanCharState charState;
	
	private Vector3 topPoint;
	private Vector3 pointOfAttachment;

	private Collider parentCol;
	private Vector3 climbPos;
	private int layerMask = 1 << 10;
	
	//the direction to move the character
	private Vector3 moveDirection = Vector3.zero;
	
	//a ray to be cast 
	//private Ray ray;
	//A class that stores ray collision info
	private RaycastHit hit;
	
	//a class to store the previous normal value
	private Vector3 oldNormal;
	//private bool atContactPoint = false;

	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		cController = GetComponent<CharacterController>();
		cCollider = GetComponent<CapsuleCollider>();
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.ClimbColliderDetected, true},

			{ GameEvent.StopEdgeClimbing, false },
			{ GameEvent.FinishClimbOver, false },
			{ GameEvent.Land, false }

		});
	}
	
	private void Update ()
	{
		if (charState.IsEdgeClimbing())
		{
		 
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
				if (gravity > 0.2f)
				{
					gravity = Vector3.Distance(center, hitPoint);
					print (gravity);
				}
				else
				{
					gravity = 0;
				}
//				if (gravity < 1.5f)
//					gravity = 0;

				//print(gravity);
				//Debug.Break();
				//gravity = hitPoint - leftHand.position;
				//Debug.Break();
				
				//Debug.LogError("Pause");

				//if the current transform's forward z value has passed the threshold test
				if(oldNormal.z >= transform.forward.z + threshold || oldNormal.z <= transform.forward.z - threshold)
				{
					//smoothly match the player's forward with the inverse of the normal
					transform.forward = Vector3.Lerp (transform.forward, -hit.normal, 20 * Time.deltaTime);
				}
//				//store the current hit.normal inside the oldNormal
				oldNormal = -hit.normal;


			} 
			else
			{
				StopClimbing(GameEvent.StopEdgeClimbing);
			} 

			moveDirection = new Vector3(InputController.h * speed, 0, gravity * gravityMultiplier)  * Time.deltaTime;
			//moveDirection.z = 0; 
			moveDirection = transform.TransformDirection(moveDirection);
	
			animator.SetFloat("HorEdgeClimbDir", InputController.h);

			if (cController.enabled)
				cController.Move(moveDirection);
		}
	
		
	}
	
	private void InitEdgeClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.ClimbColliderDetected)
		{
			InputController.h = 0;
			cController.enabled = true;
			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			animator.SetTrigger("EdgeClimb");

			// Posiiton and orient the player according to the hit point
			SetPlayerPosition(hit);
			SetPlayerOrientation (hit);
		}
	}
	
	/// <summary>
	/// Get the point of contact, which is a Vector3 
	/// that represents where the character's raycast hit on the edge climb collider
	/// but for the y we use the top point of the collider (as opposed the hit's y)
	/// </summary>
	/// <param name="hit">Hit.</param>
	private void SetPlayerPosition(RaycastHit hit)
	{
		// Get the top most Y coordinate of the collider
		float topPointYPoint = RSUtil.GetTopColliderTopYPoint(hit.collider);

		// Apply an offset to the Y point
		topPointYPoint += climbSpotYOffset;

		// Create a Vector3 that is a mix of the hit point and the collider's top Y coordinate
		topPoint = new Vector3(hit.point.x, topPointYPoint, hit.point.z);

		// Apply a Z offset to the point so we can positon the 
		// character correctly along their local forward axis
		topPoint = topPoint - transform.forward * climbSpotZOffset;

		transform.position = topPoint;
	}
	
	/// <summary>
	/// Set the player's rotation in reference to the point of contact
	/// </summary>
	/// <param name="hit">Hit.</param>
	private void SetPlayerOrientation(RaycastHit hit)
	{
		Debug.DrawRay(hit.point, -hit.normal, Color.white);
		transform.forward = -hit.normal;
		//Debug.Break();
	}
	
	private void StopClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopEdgeClimbing && charState.IsClimbing())
		{
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			print("stop climbing");
		}
	}
	
	// EVENTS -----------------------------------------------------------
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += StopClimbing;
		EventManager.onInputEvent += StopClimbing;
		EventManager.onDetectEvent += InitEdgeClimb;
	}
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= StopClimbing;
		EventManager.onInputEvent -= StopClimbing;
		EventManager.onDetectEvent -= InitEdgeClimb;	
	}
}
