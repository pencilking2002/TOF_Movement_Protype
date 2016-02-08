using UnityEngine;
using System.Collections;

public class LandingController : MonoBehaviour {

	private RomanCharState charState;
	private RaycastHit hit;
	private Animator animator;

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
	}

	private void Update() {

		Debug.DrawLine(transform.position + new Vector3(0,0.2f,0), transform.position - new Vector3(0,0.2f + 0.1f,0), Color.red);
		//TODO look for ground collision when the char is falling
		if (charState.IsFalling() && Physics.Linecast(transform.position + new Vector3(0,0.2f,0), transform.position - new Vector3(0,0.2f + 0.1f,0), out hit))
		{
			//print(hit.collider.gameObject.name);
			//Debug.DrawLine(transform.position + new Vector3(0,0.2f,0), transform.position - new Vector3(0,0.2f + 0.1f,0), Color.red);

//			//Debug.Break();	 
			animator.SetTrigger("Land");
			print("Falling");
			//EventManager.OnCharEvent(GameEvent.AttachFollow);
			EventManager.OnCharEvent(GameEvent.Land);				
		}
	}
}
