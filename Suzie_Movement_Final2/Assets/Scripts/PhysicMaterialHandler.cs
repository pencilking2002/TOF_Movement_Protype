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

			{ GameEvent.StopVineClimbing, true },
			{ GameEvent.StopEdgeClimbing, true },
			{ GameEvent.IsIdle, true },

			{ GameEvent.StartVineClimbing, false },
			{ GameEvent.StartEdgeClimbing, false }, 

		});
	}


	private void Update ()
	{
		// Raycast into the ground and make sure its a ground material
//		origin = transform.position;
//		ray = new Ray(origin, Vector3.down);
//		if ((GameManager.Instance.charState.IsIdleOrRunning()) && 
//		    Physics.Raycast (ray, out hit, groundRayLenth) && hit.collider.material != groundMaterial)
//		{
//			hit.collider.material = groundMaterial;
//		}

//		if (GameManager.Instance.charState.IsIdleOrRunningJumping())
//		{
//			origin = transform.position;
//			origin.y = transform.position.y + 1;
//			ray = new Ray(origin, transform.forward);
//
//			if (Physics.Raycast (ray, out hit, groundRayLenth) && hit.collider.material != groundMaterial)
//			{
//				
//			}
//		}
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

	// TODO get rid of land code?
	private void SetPhysicMaterial(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			
			//origin = cCollider.bounds.center - cCollider.bounds.extents;
			origin = transform.position;
			ray = new Ray(origin, Vector3.down);
			
			Debug.DrawLine (origin, origin + new Vector3(0, -groundRayLenth, 0), Color.red);
			//Debug.Break();
			//Debug.LogError("blah");
			if (Physics.Raycast (ray, out hit, groundRayLenth))
			{
				print ("is on ground");
				hit.collider.material = groundMaterial;
				GameManager.Instance.charState.SetState(RomanCharState.State.Idle);
				
			}
		}
		else if (gEvent == GameEvent.WallCollision)
		{
			origin = cCollider.bounds.center;
			Vector3 bottomOrigin = transform.position;

			ray = new Ray(origin, transform.forward);
			rayFromBottom = new Ray(bottomOrigin, transform.forward);
//			
//			Debug.LogError("blah");
			if (Physics.Raycast (ray, out hit, wallRayLength) || Physics.Raycast (rayFromBottom, out hit, wallRayLength))
			{
				Debug.DrawLine (origin, hit.point, Color.green);
				Debug.DrawLine (bottomOrigin, hit.point, Color.green);

				hit.collider.material = wallMaterial;
				//print ("Wall Collision");
				//Debug.Break();
			}


		}
		
	}
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == 9)
			return;

		origin = transform.position;
		origin.y = transform.position.y + 0.5f;
		Vector3 pointOfContact = new Vector3(other.contacts[0].point.x, origin.y, other.contacts[0].point.z);

		//print(Vector3.Dot((pointOfContact - origin).normalized, transform.forward.normalized));

		Debug.DrawLine(origin, pointOfContact, Color.black);
		Debug.Break();	
	}
//	private void SetPhysicMaterial(Collider col, bool ground)
//	{
//		col.material = ground ? groundMaterial : wallMaterial;
//	}
	
}
