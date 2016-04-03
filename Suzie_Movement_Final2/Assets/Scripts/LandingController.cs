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
	private SphereCollider sCollider;

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
//		charController = GetComponent<RomanCharController>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cCollider = GetComponent<CapsuleCollider>();
		sCollider = GetComponent<SphereCollider>();
	}

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {
			{ GameEvent.StartClimbing, false },
			{ GameEvent.StopClimbing, true }

		});
	}

	private void FixedUpdate() 
	{
//		if (charState.IsFalling())
//			LandChar(GameEvent.Land, anim_land);
//		
//		else if (charState.IsLanding())
//			LandChar(GameEvent.EnterIdle, anim_Idle);
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
				if (charState.IsSprinting())
					animator.SetTrigger (anim_SprintJump);
				else
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

	// if the character is sprint falling and they colliding with something
	// go into idle falling
	void OnCollisionStay()
	{
		if (charState.IsSprintFalling())
		{
			print("sprint falling");
			animator.SetTrigger(anim_IdleFalling);
		}
	}
	/// <summary>
	/// If char is landing, send trigger the idle animation ( used in sprint landing state)
	/// in the animator. the animator also checks for other variables like speed before going to idle
	/// </summary>
	/// <param name="other">Other.</param>
//	private void OnCollisionEnter (Collision other)
//	{
//		if (charState.IsLanding())
//		{
//			//Vector3 origin = transform.position + Vector3.up * 0.3f;
////			Debug.DrawRay(origin, (other.contacts[0].point  - origin).normalized  * 5, Color.black);
////			Debug.Break();
//			origin = transform.position + Vector3.up * 0.1f;
//			ray = new Ray(origin, Vector3.down);
//
////			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
////			sphere.transform.position = ray.origin;
////			sphere.transform.localScale = Vector3.one * 0.3f;
//			
//
//			if (Physics.SphereCast(ray, 0.5f, 0.4f))
//			{
//				Debug.DrawRay(ray.origin, ray.direction * 0.2f, Color.blue);
//				animator.SetTrigger(anim_Idle);
//				print("Spherecast landing");
//			}
//		}
		
//	}

//	private void LandChar(GameEvent gEvent, int animHash)
//	{
//		//origin = transform.position + new Vector3(0, 0.2f, 0);
//			origin = new Vector3(cCollider.bounds.center.x, cCollider.bounds.center.y - cCollider.bounds.extents.y + 0.3f, cCollider.bounds.center.z);
//			ray = new Ray(origin, Vector3.down);
//
//			Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
//			//Debug.Break();
//
//			if (JumpController.TimePassedSinceJump(0.2f) && 
//				//Physics.Raycast(ray, out hit, rayLength) &&
//				Physics.SphereCast(ray, 0.5f, rayLength) &&  
//				!AntiWallSlideController.Instance.onSloap)
//			{
//				animator.SetTrigger(animHash);
//				EventManager.OnCharEvent(gEvent);
//			}
//	}

	private void LandChar(int animHash, GameEvent gEvent)
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
