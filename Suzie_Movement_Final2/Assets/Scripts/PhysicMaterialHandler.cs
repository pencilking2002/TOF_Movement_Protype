using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
	Class responsible for changing the PhysicMaterials of the colliders the player interacts with
	Shoots rays from middle of the bottom and from the bottom of the body. The Rays are cast using 
	the char's forward direction

*/
public class PhysicMaterialHandler : MonoBehaviour {
	
	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;

	public float groundRayLenth = 0.1f;
	public float wallRayLength = 0.5f;

	private CapsuleCollider cCollider;

	// Temp raycasting vars
	private Vector3 origin;
	private Vector3 bottomOrigin;
	private Ray ray;
	private Ray rayFromBottom;

	private RaycastHit hit;
	
	private void Start ()
	{
		cCollider = GetComponent<CapsuleCollider>();

		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.Jump, true },
			{ GameEvent.StopVineClimbing, true },
			{ GameEvent.StopEdgeClimbing, true },
			{ GameEvent.StopWallClimbing, true },


			{ GameEvent.StartVineClimbing, false },
			{ GameEvent.StartEdgeClimbing, false },
			{ GameEvent.StartWallClimbing, false }, 

		});
	}


	private void Update ()
	{
		if (GameManager.Instance.charState.IsInAnyJumpingState())
		{
			// Set the origin of the ray, which will be 
			// coming out of the middle of the character (vertially)
			origin = transform.position;
			origin.y = transform.position.y + transform.lossyScale.y / 2;
			ray = new Ray(origin, transform.forward);

			Debug.DrawRay(ray.origin, ray.direction * wallRayLength, Color.green);

			if (Physics.Raycast (ray, out hit, wallRayLength))
			{
				hit.collider.material = wallMaterial;
				//Debug.Break();
			}
		}
	}

	private void OnEnable () 
	{ 
		EventManager.onCharEvent += SetPhysicMaterial;
		
		//print ("yoo");
	}
	
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= SetPhysicMaterial;
	}

	/// <summary>
	/// When the character lands, set the mesh underneath to a ground physics material
	/// </summary>
	/// <param name="gEvent">G event.</param>
	private void SetPhysicMaterial(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			//origin = cCollider.bounds.center - cCollider.bounds.extents;
			origin = transform.position;
			ray = new Ray(origin, Vector3.down);
			
			Debug.DrawLine (origin, origin + new Vector3(0, -groundRayLenth, 0), Color.red);
			if (Physics.Raycast (ray, out hit, groundRayLenth))
			{
				print ("is on ground");
				hit.collider.material = groundMaterial;
				GameManager.Instance.charState.SetState(RomanCharState.State.Idle);
			}
		}
	}
}
