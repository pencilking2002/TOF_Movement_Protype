﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeClimbController : MonoBehaviour 
{
	public LayerMask layerMask;
	// Gravity pulling the player into the climb collider
	public float gravity = 50.0f;
	public float gravityMultiplier = 20.0f;
	public float gravityThreshold = 0.01f;				// Used to calculate the point at which to stop applying gravity
	public float normalThreshold = 0.009f;				// The threshold, to discard some of the normal value variations
	public float movementSpeed = 6.0f;					// the speed to move the game object

	private RaycastHit hit;						
	private Vector3 oldNormal;							//a class to store the previous normal value

	[Range(-5.0f,5.0f)]
	public float climbSpotYOffset = 1.0f;
	
	[Range(-5.0f,5.0f)]
	public float climbSpotZOffset = 1.0f;
	

	public float rotLerpSpeed = 10.0f;

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

	private Vector3 moveDirection = Vector3.zero;		//the direction to move the character
	private Vector3 normalVel;							// Vel for smooth damping to a different forward/normal rotation

	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		cController = GetComponent<CharacterController>();
		cCollider = GetComponent<CapsuleCollider>();

		if (GameManager.componentActivatorOn) 
		{
			ComponentActivator.Instance.Register (this, new Dictionary<GameEvent, bool> { 

				{ GameEvent.EdgeClimbColliderDetected, true },

				{ GameEvent.StartWallClimbing, false },
				{ GameEvent.StopEdgeClimbing, false },
				{ GameEvent.FinishClimbOver, false },
				{ GameEvent.Land, false }

			});
		}
	}
	
	private void Update ()
	{
		if (!charState.IsEdgeClimbing())
			return;
		
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
			EventManager.OnCharEvent(GameEvent.StopEdgeClimbing);
			StopClimbing(GameEvent.StopEdgeClimbing);
		} 

		moveDirection = new Vector3(InputController.h * movementSpeed, 0, gravity * gravityMultiplier)  * Time.deltaTime;
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection = Vector3.ClampMagnitude(moveDirection, 0.03f);

		animator.SetFloat("HorEdgeClimbDir", InputController.rawH, 0.02f, Time.deltaTime);

		if (cController.enabled)
			cController.Move(moveDirection); //cController.Move(finalDir); 
	
	}
	
	private void InitEdgeClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.EdgeClimbColliderDetected)
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
		if (gEvent == GameEvent.StopEdgeClimbing && charState.IsWallClimbing())
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
