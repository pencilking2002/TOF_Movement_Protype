using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*---------------------------------------------------------------------------------------\
| Climb Detector																		 |
|________________________________________________________________________________________|	
| When the player is jumping, this class looks for a climbing collider on layer 10	     |
| by raycasting in front of the character. A GameEvent.ClimbColliderDetected event       |
| is emmited when a collider is found													 |
\---------------------------------------------------------------------------------------*/

public class ClimbDetector : MonoBehaviour {
	
	[HideInInspector]
	public bool climbColliderDetected;
	public float rayLength = 2.0f;							// How long the raycast to look for climbable objects should be

	private Ray ray;
	private RaycastHit hit;
	private float cColliderHeight;	
	private int layerMask = 1 << 10; 						// Only raycast against edge climb colliders (layer 10)
	private Vector3 raycastOffset = new Vector3 (0, 1f, 0);
	private Vector3 topOfClimbCollider;
	private bool detached = false;
	private bool showGizmo = false;

	private CapsuleCollider cCollider;
	private ClimbOverEdge climbOverEdge;
	private RomanCharState charState;

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {

			{ GameEvent.Jump, true },

			{ GameEvent.StartEdgeClimbing, false },
			{ GameEvent.StartWallClimbing, false },
			{ GameEvent.StartVineClimbing, false }

		});

		cCollider = GetComponent<CapsuleCollider>();
		climbOverEdge = GetComponent<ClimbOverEdge>();
		charState = GetComponent<RomanCharState>();
	}

	private void Update ()
	{
		if (charState.IsIdleOrRunningJumping() && !detached)
		{
			Debug.DrawRay(transform.position + raycastOffset, transform.forward * rayLength, Color.red); 
			
			if (Physics.Raycast (transform.position + raycastOffset, transform.forward, out hit, rayLength, layerMask))
			{
				float climbColTopY = hit.collider.bounds.center.y + hit.collider.bounds.extents.y;
				float charTopY = cCollider.bounds.center.y + cCollider.bounds.extents.y;

				if (charTopY > climbColTopY - 0.2f)
				{
					showGizmo = true;

					topOfClimbCollider = new Vector3(hit.point.x, climbColTopY - 0.2f, hit.point.z);

					EventManager.OnDetectEvent(GameEvent.ClimbColliderDetected, hit);
//					Debug.Break();
//					
//					if (hit.collider.CompareTag("NoClimbOver"))
//						climbOverEdge.noClimbOver = true;
//					else 
//						climbOverEdge.noClimbOver = false;
				}
				else
				{
					showGizmo = false;
				}

			}
		}
	
	}

	private void OnEnable()
	{
		EventManager.onInputEvent += Detach;
		EventManager.onCharEvent += Detach;
	}

	private void OnDisable()
	{
		EventManager.onInputEvent -= Detach;
		EventManager.onCharEvent -= Detach;
	}

	/// <summary>
	/// Used to record when the player has just detached from an edge
	/// We use this in the Update method to check if the player just detached
	/// and if he did then don't try and reatach again
	/// </summary>
	/// <param name="gEvent">G event.</param>
	private void Detach (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopEdgeClimbing)
			detached = true;

		else if (gEvent == GameEvent.Land)
			detached = false;	
	}

	/// <summary>
	/// Used to show a gizmo at the location of the top of the edge collider that was detected
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		if (showGizmo)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(topOfClimbCollider, 0.2f);	
		}
	}
	
}
