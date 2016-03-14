using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallClimbController : MonoBehaviour {

	private Rigidbody rb;
	private Animator animator;
	private CharacterController cController;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		cController = GetComponent<CharacterController>();

		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.WallClimbColliderDetected, true},

			{ GameEvent.StopWallClimbing, false },
			{ GameEvent.FinishClimbOver, false },
			{ GameEvent.Land, false }

		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// EVENTS -----------------------------------------------------------
	private void OnEnable () 
	{ 
		EventManager.onDetectEvent += InitWallClimb;
		EventManager.onCharEvent += StopClimbing;
		EventManager.onInputEvent += StopClimbing;
	}
	private void OnDisable () 
	{ 
		
		EventManager.onDetectEvent -= InitWallClimb;
		EventManager.onCharEvent -= StopClimbing;
		EventManager.onInputEvent -= StopClimbing;	
	}

	private void InitWallClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.WallClimbColliderDetected && rb.velocity.y < 0)
		{
			InputController.h = 0;
			cController.enabled = true;
			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			animator.SetTrigger("WallClimb");

			// Posiiton and orient the player according to the hit point
			//SetPlayerPosition(hit);
			//SetPlayerOrientation (hit);
		}
	}

	private void StopClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopWallClimbing && GameManager.Instance.charState.IsClimbing())
		{
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			print("stop wall climbing");
		}
	}
}

