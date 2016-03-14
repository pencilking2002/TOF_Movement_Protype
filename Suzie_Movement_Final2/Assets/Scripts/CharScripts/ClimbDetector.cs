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
	public float edgeRayLength = 0.8f;							// Length of ray to cast to detect edge colliders
	public float wallRayLength = 0.3f;							// Length of ray to cast to detect wall colliders

	private Ray ray;
	private RaycastHit hit;
	private float cColliderHeight;	
	private int layerMask = (1 << 10) | (1 << 11); 						// Only raycast against edge climb colliders (layer 10)
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
			Debug.DrawRay(transform.position + raycastOffset, transform.forward * edgeRayLength, Color.red); 
			
			if (Physics.Raycast (transform.position + raycastOffset, transform.forward, out hit, edgeRayLength, layerMask))
			{
				DetectEdgeClimbing(hit);
				DetectWallClimbing(hit);
//					Debug.Break();
//					
//					if (hit.collider.CompareTag("NoClimbOver"))
//						climbOverEdge.noClimbOver = true;
//					else 
//						climbOverEdge.noClimbOver = false;
				
			}

		}
	
	}

	private void DetectEdgeClimbing(RaycastHit hit)
	{
		if (hit.collider.gameObject.gameObject.layer != 10)
			return;

		float climbColTopY = hit.collider.bounds.center.y + hit.collider.bounds.extents.y;
		float charTopY = cCollider.bounds.center.y + cCollider.bounds.extents.y;

		if (charTopY > climbColTopY - 0.2f)
		{
			showGizmo = true;
			topOfClimbCollider = new Vector3(hit.point.x, climbColTopY - 0.2f, hit.point.z);
			EventManager.OnDetectEvent(GameEvent.EdgeClimbColliderDetected, hit);
		}

		else 
			showGizmo = false;
	}

	private void DetectWallClimbing(RaycastHit hit)
	{
		if (hit.collider.gameObject.gameObject.layer != 11)
			return;

		EventManager.OnDetectEvent(GameEvent.WallClimbColliderDetected, hit);

		//print("Wall collider");
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
