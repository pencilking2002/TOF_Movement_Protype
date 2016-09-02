using UnityEngine;
using System.Collections;

public class BugHack : MonoBehaviour {

	private RomanCharController charController;
	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private int anim_wallClimbHash = Animator.StringToHash ("WallClimb");
	// Use this for initialization
	void Start () 
	{
		charState = GetComponent<RomanCharState> ();
		charController = GetComponent<RomanCharController> ();
		animator = GetComponent <Animator> ();
		rb = GetComponent <Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (charState.IsIdleOrRunning() && !charController.enabled) 
		{
			charController.enabled = true;	
		}

		if (charState.IsIdle () && animator.GetBool (anim_wallClimbHash)) 
		{
			animator.SetBool (anim_wallClimbHash, false);
		}

		if (charState.IsFalling () && !rb.useGravity) 
		{
			rb.useGravity = true;
		}
	}
}
