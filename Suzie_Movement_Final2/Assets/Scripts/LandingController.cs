using UnityEngine;
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
	private float lastYPos;
	private Ray ray;
	private RaycastHit hit;

	private int anim_land = Animator.StringToHash("Land");
	private int anim_Falling = Animator.StringToHash("Falling");
	private int anim_IdleFalling = Animator.StringToHash("IdleFalling");
	private int anim_SprintJump = Animator.StringToHash("SprintJump");
	private int anim_Idle = Animator.StringToHash("Idle");


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
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {
			{ GameEvent.StartClimbing, false },
			{ GameEvent.StopClimbing, true }

		});
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
				animator.SetTrigger (anim_SprintJump);
			}
		}

	}

	// if the character is sprint falling and colliding with something
	// go into idle falling
	void OnCollisionStay()
	{
		if (charState.IsSprintFalling())
			animator.SetTrigger(anim_IdleFalling);
		
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
		}
	}

	/// <summary>
	/// If not on a sloap then land the character
	/// </summary>
	void OnTriggerStay ()
	{
		if (JumpController.TimePassedSinceJump(0.2f) && !SloapDetector.Instance.onSloap)
		{
			if (charState.IsFalling())
				LandChar(anim_land, GameEvent.Land);

			else if (charState.IsLanding())
				LandChar(anim_Idle, GameEvent.EnterIdle);
		}

	}
}
