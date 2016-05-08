
/*-----------------------------------------------------------------------------------\ 
| @class PhysicMaterialHandler													      |
|-------------------------------------------------------------------------------------|
| -- DESCRITION -- 																	  |
  Responsible for changing the PhysicMaterial of the char's capsule collider based    |
| on the state of the character.													  |
|------------------------------------------------------------------------------------ |
| -- WALL COLLISIONS ---															  |
| When the character is running, this script looks for walls in front of the char.	  |
| If a wall is in the way of the char, a frictionless material is used so that the    |
| character doesn't get stuck on the wall but instead "slides" around it			  |										
\------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicMaterialHandler : MonoBehaviour {

	public LayerMask layerMask;
	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;
	public float wallRayLength = 0.5f;

	private CapsuleCollider cCollider;

	// Temp raycasting vars
	private Vector3 origin;
	private Ray ray;
	private RomanCharState charState;

	private RaycastHit hit;
	
	private void Start ()
	{
		cCollider = GetComponent<CapsuleCollider>();
		charState = GetComponent<RomanCharState>();

		if (GameManager.componentActivatorOn) 
		{
			ComponentActivator.Instance.Register (this, new Dictionary<GameEvent, bool> { 

				{ GameEvent.Jump, true },
				{ GameEvent.StopVineClimbing, true },
				{ GameEvent.StopEdgeClimbing, true },
				{ GameEvent.StopWallClimbing, true },
				{ GameEvent.ClimbOverEdge, true },

				{ GameEvent.StartVineClimbing, false },
				{ GameEvent.StartEdgeClimbing, false },
				{ GameEvent.StartWallClimbing, false }, 

			});
		}
	}

	private void FixedUpdate ()
	{
		if (charState.IsRunningOrSprinting ())
		{
			ray = new Ray (transform.position + Vector3.up * 0.1f, transform.forward);
			Debug.DrawRay(ray.origin, ray.direction * wallRayLength, Color.green);

			if (Physics.Raycast (ray, out hit, wallRayLength, layerMask))
				cCollider.sharedMaterial = wallMaterial;
		}

	}
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += SetPhysicMaterial;
	}
	
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= SetPhysicMaterial;
	}
//
//	/// <summary>
//	/// When the character lands, set the mesh underneath to a ground physics material
//	/// </summary>
//	/// <param name="gEvent">G event.</param>
	private void SetPhysicMaterial(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Jump) 
			cCollider.sharedMaterial = wallMaterial;

		else if (gEvent == GameEvent.IsIdle || gEvent == GameEvent.StartRunning)
			cCollider.sharedMaterial = groundMaterial;
	}

}
