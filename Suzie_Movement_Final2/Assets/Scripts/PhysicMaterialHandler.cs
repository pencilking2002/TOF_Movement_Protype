using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
	Class responsible for changing the PhysicMaterials of the colliders the player interacts with
	Shoots rays from middle of the bottom and from the bottom of the body. The Rays are cast using 
	the char's forward direction

*/
public class PhysicMaterialHandler : MonoBehaviour {
	public PhysicMatJumper jumperCollider;

	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;

//	public float groundRayLenth = 0.1f;
	public float wallRayLength = 0.5f;

	private CapsuleCollider cCollider;

	// Temp raycasting vars
	private Vector3 origin;
//	private Vector3 bottomOrigin;
	private Ray ray;
//	private Ray rayFromBottom;

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

	private void OnEnable () 
	{ 
		EventManager.onCharEvent += SetPhysicMaterial;
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
		if (gEvent == GameEvent.Jump)
		{
			origin = transform.position;
			origin.y = transform.position.y + transform.lossyScale.y / 2;
			ray = new Ray(origin, transform.forward);
			if (Physics.Raycast (ray, out hit, wallRayLength))
			{
				cCollider.sharedMaterial = wallMaterial;
			}
			else
			{
				RSUtil.Instance.DelayedAction(() => {
					cCollider.sharedMaterial = wallMaterial;
				}, 0.2f);
			}
		}	
		else if (gEvent == GameEvent.Land)
		{
			cCollider.sharedMaterial = groundMaterial;

		}

	}
}
