using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandingController : MonoBehaviour {

	/*----------------------------------------------------------|
	| PRIVATE VARIABLES	             	                        |
	|----------------------------------------------------------*/

	public float lineLength = 0.2f;

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
//	private RomanCharController charController;

//	private RaycastHit hit;
	private Vector3 origin, endPoint;
	//private int layerMask = 1 << 9;						// Mask out every layer except the ground layer

	private int anim_land = Animator.StringToHash("Land");
	private int anim_Falling = Animator.StringToHash("Falling");
	private int anim_SprintJump = Animator.StringToHash("SprintJump");
//	private int anim_Idle = Animator.StringToHash("Idle");


	private float lastYPos;
	private Ray ray;
	private RaycastHit hit;

	/*----------------------------------------------------------|
	| UNITY METHODS	      		    	                        |
	|----------------------------------------------------------*/

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
//		charController = GetComponent<RomanCharController>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {

//			{ GameEvent.StartEdgeClimbing, false },
//			{ GameEvent.StartWallClimbing, false },
//			{ GameEvent.StartVineClimbing, false },

			{ GameEvent.StopClimbing, true }

		});
	}

	private void Update() 
	{
		LandCharacter();

	}

	/*----------------------------------------------------------|
	| CUSTOM METHODS	      		    	                    |
	|----------------------------------------------------------*/

	/// <summary>
	/// When the character is falling, linecast downward to look for the ground
	/// If the ground is found then perform a landing animation
	/// </summary>
	private void LandCharacter ()
	{
		if (charState.IsFalling() || charState.IsLanding())
		{
			origin = transform.position + new Vector3(0, 0.2f, 0);
//			endPoint = origin;
//			endPoint.y -= lineLength;
			ray = new Ray(origin, Vector3.down);

			Debug.DrawRay(ray.origin, ray.direction * lineLength, Color.red);
			//Debug.Break();
			//if (Physics.Linecast(origin, endPoint))
			if (Physics.Raycast(ray, out hit, lineLength))
			{
				//print(rb.GetRelativePointVelocity(hit.point));
				//if (rb.velocity.y <= 0)
				//{
					//animator.SetTrigger(anim_land);
					// TODO: Not checking the Y velocity fixes the prolongued langing bug but also makes
					// brings another bug where the character immidiately goes into landing animation when 
					// they jump

				//float (Vector3.Dot(Vector3.down, 

				if (charState.IsFalling() && JumpController.TimePassedSinceJump(0.2f) && !AntiWallSlideController.Instance.onSloap)
					{
						//print("landing triggered");
						animator.SetTrigger(anim_land);
						EventManager.OnCharEvent(GameEvent.Land);
					}
//					else if (charState.IsLanding())
//					{
//						animator.SetTrigger(anim_Idle);
//						print("Triggered idle animation from Landing Controller");
//					}
				//}
								
			}
		}

}

//	void OnEnable ()
//	{
//		EventManager.onCharEvent += RecordJumpTime;
//	}
//	void OnDisable()
//	{
//		EventManager.onCharEvent -= RecordJumpTime;
//	}
//
//	void RecordJumpTime(GameEvent gEvent)
//	{
//		if (gEvent == GameEvent.Jump)
//		{
//			jumpTime = Time.time;
//		}
//	}

	/// <summary>
	/// Force animator to trigger the falling animation 
	/// on collision exit from a collider that is below the player
	/// </summary>
	private void OnCollisionExit ()
	{
		if (charState.IsIdleOrMoving() && rb.velocity.y < 0)
		{
			origin = transform.position + new Vector3(0, 0.2f, 0);
			ray = new Ray(origin, Vector3.down);

			if (!Physics.Raycast (ray, 0.6f))
			{	
				Debug.DrawRay(ray.origin, ray.direction * 0.6f, Color.blue);
				//Debug.Break();
				if (charState.IsSprinting())
					animator.SetTrigger (anim_SprintJump);
				else
					animator.SetTrigger (anim_Falling);
			}
		}

	}

	/// <summary>
	/// If char is landing, send trigger the idle animation ( used in sprint landing state)
	/// in the animator. the animator also checks for other variables like speed before going to idle
	/// </summary>
	/// <param name="other">Other.</param>
//	private void OnCollisionEnter (Collision other)
//	{
//		if (other.gameObject.layer == 9 && charState.IsLanding() && charController.speed < 0.1f)
//		{
//			animator.SetTrigger(anim_Idle);
//		}
//		
//	}
}
