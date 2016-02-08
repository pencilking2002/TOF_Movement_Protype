using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VineClimbController2 : MonoBehaviour {

	// Vars for measuring vine climbing operations
	public float vineClimbSpeed = 20.0f;
	public float attachForwardSpeed = 40.0f;								// How quickly the character moves towards the vine when they	
	public float maxTimeBeforeCanReattach = 1f;								// The amount of time that has to pass before the character can re-attach on to th vine
	private Vector3 distToVine = Vector3.zero;
	[HideInInspector] public bool detached = false;											// Has the character detached from the vine?

	// Points and objects needed for vine climbing operations
	public Transform vineAttachPoint = null;
	private Transform vineClimbOverPoint = null;							// Empty GO inside char mesh that is used to detecting at which point the char should climb over

	private Vector3 vinePos = Vector3.zero;
	private Transform vine = null;
	private Collider vineCollider = null;
	private float vineTopPointY;					// Top point of current vine collider				
	private float vineBottomPointY;			   	    // Bottom point of current vine collider
	private Animator animator;
	private Rigidbody rb;

	// Hashed animator variables
	private int anim_vineClimbSpeed = Animator.StringToHash("VineClimbSpeed");
	private int anim_vineClimbCurve = Animator.StringToHash("vineClimbCurve");

	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		vineAttachPoint = GameObject.FindGameObjectWithTag("VineAttachPoint").transform;
		vineClimbOverPoint = GameObject.FindGameObjectWithTag("VineClimbOverPoint").transform;

		// Register this script's shutdown and activation events
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.StartVineClimbing, true },
			{ GameEvent.StopVineClimbing, false },
			{ GameEvent.ClimbOverEdge, false },
			{ GameEvent.Land, false },

		});
	}
	
	private void Update ()
	{

		if (GameManager.Instance.charState.IsVineAttaching())
		{
			vinePos = new Vector3(vine.position.x, vineAttachPoint.position.y, vine.position.z);
			distToVine = vinePos - vineAttachPoint.position;
			transform.position = Vector3.Lerp(transform.position, transform.position + distToVine, attachForwardSpeed * Time.deltaTime);

			//Debug.DrawLine(vinePos, vineAttachPoint.position, Color.blue, 1f);
			//Debug.Break();

		}
		else if (GameManager.Instance.charState.IsVineClimbing())
		{

//			if (transform.position.y < vineBottomPoint.y)
//			{
//				print("Squirrel below collider");
//				StopVineClimbing(GameEvent.StopVineClimbing);
//			}


			vinePos = new Vector3(vine.position.x, vineAttachPoint.position.y, vine.position.z);
			distToVine = vinePos - vineAttachPoint.position;
			transform.position = Vector3.Lerp(transform.position, transform.position + distToVine, attachForwardSpeed * Time.deltaTime);

			animator.SetFloat(anim_vineClimbSpeed, InputController.v);

			transform.Translate(new Vector3(0, InputController.v * vineClimbSpeed * animator.GetFloat(anim_vineClimbCurve) * Time.deltaTime, 0));
			if (InputController.rawV != 0)
			{
				rb.centerOfMass = vineAttachPoint.position;
				rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(-vine.transform.forward, Vector3.up), 1.5f * Time.deltaTime));
			}

			if (vineClimbOverPoint.position.y > vineTopPointY)
			{
				print("Squirrel above collider");
				EventManager.OnCharEvent(GameEvent.ClimbOverEdge);
				ResetVineData();
			}
		}
	}

	// Setup the characte for attaching to the vine
	private void OnTriggerEnter (Collider coll)
	{
		//print("blah");
//		print("Component active" + this.enabled);
//		print("collider layer: " + coll.gameObject.layer);
//		print("charState: " + GameManager.Instance.charState.GetState());
//		print("detached: " + detached);

		//Debug.Break();
		if (coll.gameObject.layer == 14 && !GameManager.Instance.charState.IsVineClimbing() && !detached)
		{
			
			EventManager.OnCharEvent(GameEvent.StartVineClimbing);
			GameManager.Instance.charState.SetState(RomanCharState.State.VineAttaching);

			vine = coll.transform.parent.transform;
			vineCollider = coll;

			vineTopPointY = vineCollider.bounds.center.y + vineCollider.bounds.extents.y;
			vineBottomPointY = vineCollider.bounds.center.y - vineCollider.bounds.extents.y;


			//print("vine name: " + vine.name);
			// Publish a an event for StartVineClimbing
			rb.isKinematic = true;
			animator.SetTrigger ("VineAttach");

		}

//		if (coll.CompareTag("VineTopExit"))
//		{
//			print("VineClimbController: ClimbOverEdge event sent");
//			EventManager.OnCharEvent(GameEvent.ClimbOverEdge);
//		}
	}

	private void OnEnable ()
	{
		
		EventManager.onInputEvent += StopVineClimbing;
		//EventManager.onCharEvent += ResetDetached;
	}

	private void OnDisable ()
	{
		EventManager.onInputEvent -= StopVineClimbing;
		//EventManager.onCharEvent -= ResetDetached;
	}


	private void StopVineClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopVineClimbing && GameManager.Instance.charState.IsVineClimbing())
		{
			print("stop vine climbing");
			transform.parent = null;
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");

			ResetVineData();
		}
	}

//	private void ResetDetached(GameEvent gEvent)
//	{
//		if (gEvent == GameEvent.Land)
//		{
//			detached = false;
//		}
//	}

	public void ResetVineData()
	{
		// Reset all vine variables when char climbs over
		vine = null;
		vinePos = Vector3.zero;
		vineCollider = null;
		vineTopPointY = 0;
		vineBottomPointY = 0;
		detached = true;
		print("vine data reset");
}

	void OnDrawGizmos()
	{
		if (vine != null)
		{
			Bounds bounds = vineCollider.bounds;

			//Gizmos.color = Color.black;
			//Gizmos.DrawSphere(new Vector3(vine.position.x, bounds.center.y + bounds.extents.y, vine.position.z), 0.2f);

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(new Vector3(vine.position.x, bounds.center.y + bounds.extents.y, vine.position.z), 0.2f);

			Gizmos.color = Color.black;
			Gizmos.DrawSphere(new Vector3(vine.position.x, bounds.center.y - bounds.extents.y, vine.position.z), 0.2f);
//		
		}
	}


}