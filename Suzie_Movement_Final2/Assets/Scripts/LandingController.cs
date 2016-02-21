using UnityEngine;
using System.Collections;

public class LandingController : MonoBehaviour {

	/*----------------------------------------------------------|
	| PRIVATE VARIABLES	             	                        |
	|----------------------------------------------------------*/

	private RomanCharState charState;
	private RaycastHit hit;
	private Animator animator;
	private Rigidbody rb;

	private int layerMask = 1 << 9;						// Mask out every layer except the ground layer
	private Vector3 origin, endPoint;
	private int anim_land = Animator.StringToHash("Land");
	private int anim_Idle = Animator.StringToHash("Idle");

	/*----------------------------------------------------------|
	| UNITY METHODS	      		    	                        |
	|----------------------------------------------------------*/

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
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
		if (!charState.IsFalling())
			return;

		origin = transform.position + new Vector3(0, 0.2f, 0);
		endPoint = origin;
		endPoint.y -= 0.5f;

		Debug.DrawLine(origin, endPoint, Color.red);

		if (Physics.Linecast(origin, endPoint, out hit, layerMask))
		{
			if (rb.velocity.y <= 0)
			{
				animator.SetTrigger(anim_land);
				EventManager.OnCharEvent(GameEvent.Land);
			}				
		}
	}

	/// <summary>
	/// If char is landing, send trigger the idle animation ( used in sprint landing state)
	/// in the animator. the animator also checks for other variables like speed before going to idle
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.layer != 9)
			return;

		else if (charState.IsLanding())
			animator.SetTrigger(anim_Idle);
		
	}
}
