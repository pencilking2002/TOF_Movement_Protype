using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandingController : MonoBehaviour {

	/*----------------------------------------------------------|
	| PRIVATE VARIABLES	             	                        |
	|----------------------------------------------------------*/

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private RomanCharController charController;

	private RaycastHit hit;
	private Vector3 origin, endPoint;
	//private int layerMask = 1 << 9;						// Mask out every layer except the ground layer

	private int anim_land = Animator.StringToHash("Land");
	private int anim_Idle = Animator.StringToHash("Idle");

	/*----------------------------------------------------------|
	| UNITY METHODS	      		    	                        |
	|----------------------------------------------------------*/

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		charController = GetComponent<RomanCharController>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {

			{ GameEvent.StartEdgeClimbing, false },
			{ GameEvent.StartWallClimbing, false },
			{ GameEvent.StartVineClimbing, false },

			{ GameEvent.StopEdgeClimbing, true },
			{ GameEvent.StopWallClimbing, true },
			{ GameEvent.StopVineClimbing, true }

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
		if (charState.IsFalling())
		{
			origin = transform.position + new Vector3(0, 0.2f, 0);
			endPoint = origin;
			endPoint.y -= 0.5f;

			Debug.DrawLine(origin, endPoint, Color.red);

			if (Physics.Linecast(origin, endPoint, out hit))
			{
				if (rb.velocity.y <= 0)
				{
					animator.SetTrigger(anim_land);
					EventManager.OnCharEvent(GameEvent.Land);
				}				
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
