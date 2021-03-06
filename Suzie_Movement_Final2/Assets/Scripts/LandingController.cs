﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandingController : MonoBehaviour {

	public float rayLength = 0.5f;

	/*----------------------------------------------------------|
	| PRIVATE VARIABLES	             	                        |
	|----------------------------------------------------------*/

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private CapsuleCollider cCollider;

	private Vector3 origin, endPoint;
	private Ray ray;
	private RaycastHit hit;

    // water/land detection
    private Vector3 dOrigin, dDirection;
    public float dRayLength = 1.3f;
    public LayerMask dLayerMask;

	private int anim_land = Animator.StringToHash("Land");
	private int anim_Falling = Animator.StringToHash("Falling");
	private int anim_IdleFalling = Animator.StringToHash("IdleFalling");
	private int anim_SprintJump = Animator.StringToHash("SprintJump");
	private int anim_Idle = Animator.StringToHash("Idle");

	public bool touchingWater = false;

	/*----------------------------------------------------------|
	| UNITY METHODS	      		    	                        |
	|----------------------------------------------------------*/

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cCollider = GetComponent<CapsuleCollider>();
	}

	private void Start ()
	{
		if (GameManager.componentActivatorOn) 
		{
			ComponentActivator.Instance.Register (this, new Dictionary<GameEvent, bool> {
				{ GameEvent.StartClimbing, false },
				{ GameEvent.StopClimbing, true }

			});
		}
	}

    private void FixedUpdate()
    {
        dOrigin = transform.position + new Vector3(0, 1.2f, 0);
        dDirection = Vector3.down * dRayLength;

        Debug.DrawRay(dOrigin, dDirection, Color.black);

        // Detect if character is currently touching water
        touchingWater = Physics.Raycast (dOrigin, Vector3.down, dRayLength, dLayerMask);
   
    }

	/// <summary>
	/// Force animator to trigger the falling animation 
	/// on collision exit from a collider that is below the player
	/// </summary>
	private void OnCollisionExit ()
	{
		if (rb.velocity.y > 0)
			return;

		if (charState.IsIdleOrRunning())
		{
			origin = transform.position + new Vector3(0, 0.2f, 0);
			ray = new Ray(origin, Vector3.down);

			if (!Physics.Raycast (ray, 0.6f))
			{	
				//Debug.DrawRay(ray.origin, ray.direction * 0.6f, Color.blue);
				//Debug.Break();
				animator.SetTrigger (anim_Falling);
			}
		}
		else if (charState.IsSprinting())
		{
			origin = transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.4f;
			ray = new Ray(origin, Vector3.down);

			if (!Physics.Raycast (ray, 0.6f))
			{	
				//Debug.DrawRay(ray.origin, ray.direction * 0.6f, Color.blue);
				//Debug.Break();
				//Force a sprint jump
				EventManager.OnInputEvent(GameEvent.Jump);
			}
		}

	}

	// if the character is sprint falling and colliding with something
	// go into idle falling
	void OnCollisionStay()
	{
		if (charState.IsSprintFalling())
		{
			animator.SetTrigger(anim_IdleFalling);
			RSUtil.OrientCapsuleCollider(cCollider, true);
		}
	}

	/// <summary>
	/// Lands or makes the char go into idle if raycast returns true
	/// </summary>
	/// <param name="animHash">Animation hash.</param>
	/// <param name="gEvent">G event.</param>
	void LandChar(int animHash, GameEvent gEvent)
	{
		origin = new Vector3(cCollider.bounds.center.x, cCollider.bounds.center.y - cCollider.bounds.extents.y + 0.3f, cCollider.bounds.center.z);
		ray = new Ray(origin, Vector3.down);
		Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

		if (Physics.Raycast(ray, rayLength))
		{
			animator.SetTrigger(animHash);
			EventManager.OnCharEvent(gEvent);

			//print ("land char"); 
		}
	}

	/// <summary>
	/// If not on a sloap then land the character
	/// </summary>
	void OnTriggerStay (Collider col)
	{
		//touchingWater = (col.gameObject.layer == 25);

		if (JumpController.TimePassedSinceJump(0.3f))
		{
			if (charState.IsFalling ()) 
			{
				if (col.gameObject.layer == 23) 
				{
					InitWallBounce (col.transform);
					//Debug.Break ();
				} 
				else if (!SloapDetector.Instance.onSloap)
				{
					// If hitting a wall bounce, send event for the jump controller to handle
					LandChar (anim_land, GameEvent.Land);

					if (touchingWater)
						EventManager.OnCharEvent (GameEvent.WaterColliderLanding);
					else
						EventManager.OnCharEvent (GameEvent.ColliderLand);
				}
			} 
			else if (charState.IsLanding ()) 
			{
				if (col.gameObject.layer == 23) 
				{
					InitWallBounce (col.transform);
					//Debug.Break ();
				} 
				else if (!SloapDetector.Instance.onSloap)
				{
					LandChar (anim_Idle, GameEvent.EnterIdle);
				}
			} 
			else if (charState.IsIdle () && col.gameObject.layer == 23) 
			{
				InitWallBounce (col.transform);
			}
		}

	}

	void InitWallBounce(Transform t)
	{
		animator.SetBool ("WallBounce", true);
		print ("RomanCharController: sending jump event");
		charState.SetState (RomanCharState.State.WallBouncing);
		EventManager.OnCharEvent (GameEvent.WallBounce, t);
	}
}
