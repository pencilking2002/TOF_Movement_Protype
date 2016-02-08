using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*---------------------------------------------------------------------------------------\
	When the player is jumping, this class looks for a climbing collider on layer 10	 |
 	by raycasting in front of the character. A GameEvent.ClimbColliderDetected event     |
 	is emmited when a collider is found													 |
\---------------------------------------------------------------------------------------*/

public class ClimbDetector : MonoBehaviour {
	
	[HideInInspector]
	public bool climbColliderDetected;
	public float rayLength = 2.0f;			// How long the raycast to look for climbable objects should be

	private Ray ray;
	private RaycastHit hit;
	private float cColliderHeight;
	private int layerMask = 1 << 10;
	private Vector3 raycastOffset = new Vector3 (0, 1f, 0);
	private ClimbOverEdge climbOverEdge;
	private CapsuleCollider cCollider;
	private bool detached = false;

	private bool showGizmo = false;
	private Vector3 topOfClimbCollider;

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> {

			{ GameEvent.Jump, true },
			{ GameEvent.Land, false },
			{ GameEvent.StartEdgeClimbing, false },
			{ GameEvent.StartVineClimbing, false }

		});

		climbOverEdge = GetComponent<ClimbOverEdge>();

		if (climbOverEdge == null)
			Debug.LogError("climb over egde not found");

			cCollider =GetComponent<CapsuleCollider>();
	}

	private void Update ()
	{
		if (GameManager.Instance.charState.IsJumping() && !detached)
		{
			Debug.DrawRay(transform.position + raycastOffset,  transform.forward * rayLength, Color.red); 
			
			if (Physics.Raycast (transform.position + raycastOffset, transform.forward, out hit, rayLength, layerMask))
			{
				float climbColTopY = hit.collider.bounds.center.y + hit.collider.bounds.extents.y;
				float charTopY = cCollider.bounds.center.y + cCollider.bounds.extents.y;

				if (charTopY > climbColTopY - 0.2f)
				{
					showGizmo = true;

					topOfClimbCollider = new Vector3(hit.point.x, climbColTopY - 0.2f, hit.point.z);
					//Debug.Break();
					//print("Yooo");
//					Debug.DrawLine(transform.position, hit.point, Color.green);
//					Debug.Break();

					EventManager.OnDetectEvent(GameEvent.ClimbColliderDetected, hit);

					if (hit.collider.CompareTag("NoClimbOver"))
						climbOverEdge.noClimbOver = true;
					else 
						climbOverEdge.noClimbOver = false;
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

	private void Detach (GameEvent gEvent)
	{
		
		if (gEvent == GameEvent.StopEdgeClimbing)
		{
			print("climb detector: stop climbing");
			detached = true;
		}
		else if (gEvent == GameEvent.Land)
		{
			detached = false;
		}
		
	}

	private void OnDrawGizmos()
	{
		if (showGizmo)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(topOfClimbCollider, 0.2f);	
		}
	}
	
}
