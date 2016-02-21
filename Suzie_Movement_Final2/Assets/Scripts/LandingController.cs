using UnityEngine;
using System.Collections;

public class LandingController : MonoBehaviour {

	private RomanCharState charState;
	private RaycastHit hit;
	private Animator animator;
	private Rigidbody rb;

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}

	private void Update() {
		Vector3 origin = transform.position + new Vector3(0,0.2f,0);
		Vector3 endPoint = transform.position - new Vector3(0,0.2f + 0.1f,0);

		Debug.DrawLine(origin, endPoint, Color.red);

		//TODO look for ground collision when the char is falling
		if (charState.IsFalling() && Physics.Linecast(origin, endPoint, out hit))
		{
			if (rb.velocity.y <= 0)
			{
				animator.SetTrigger("Land");
				EventManager.OnCharEvent(GameEvent.Land);
			}				
		}
	}

	private void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.layer != 9)
			return;

	    // If char is landing, send trigger the idle animation ( used in sprint landing state)
	    // in the animator. the animator also checks for other variables like speed before going to idle
		if (charState.IsLanding())
		{
			//print("force idle");
			animator.SetTrigger("Idle");
		}
	}
}
