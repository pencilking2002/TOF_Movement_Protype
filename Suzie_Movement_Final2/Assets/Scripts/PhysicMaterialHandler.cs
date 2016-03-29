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
	public PhysicMaterial runningMaterial;

//	public float groundRayLenth = 0.1f;
	public float wallRayLength = 0.5f;

	private CapsuleCollider cCollider;

	// Temp raycasting vars
	private Vector3 origin;
//	private Vector3 bottomOrigin;
	private Ray ray;
//	private Ray rayFromBottom;
	private RomanCharState charState;

	private RaycastHit hit;
	
	private void Start ()
	{
		cCollider = GetComponent<CapsuleCollider>();
		charState = GetComponent<RomanCharState>();

		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

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

//	private void FixedUpdate ()
//	{
//		if (charState.IsRunning() && AntiWallSlideController.Instance.colliding)
//		{
//			cCollider.sharedMaterial = runningMaterial;
//		}
//	}
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
		if (gEvent == GameEvent.Jump || 
		   (gEvent == GameEvent.StartRunning && !AntiWallSlideController.Instance.colliding) ||
			gEvent == GameEvent.StartSprinting)
			cCollider.sharedMaterial = wallMaterial;

		else if (gEvent == GameEvent.Land || gEvent == GameEvent.IsIdle)
			cCollider.sharedMaterial = groundMaterial;
		

	}

}
