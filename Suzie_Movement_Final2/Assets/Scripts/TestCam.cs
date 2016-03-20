﻿//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class TestCam : MonoBehaviour 
{
	// Movement
	public Vector3 offset = new Vector3(0, 2, 5);
	public float climbXClampThreshold = 0.7f;				// Used to limit the rotation around the Y axis during climbing
	public float moveSpeed = 40.0f;
	public float zoomInSpeed = 6.0f;						// Speed of camera zoom in at the beginning of the scene
	public float zOffsetDamping = 40.0f;

	// Climbing rotation
	public float extraClimbRotation = 10.0f;
	public float minClimbRotSpeed = 0.0f;
	public float maxClimbRotSpeed = 20.0f;
	public float rotateDamping = 3.0f;
	public float transThreshold = 0.02f;

	// Collision
	public float collisionSpeed = 15.0f;				
	public float collisionToMoveSpeedDamping = 40.0f;	    // Amount of damping to apply when SmoothDamping the speed while exiting a collision
	public float collisionRaycastYOffset = 5.0f;			// How far up from should the collision raycast be offset from the player's y position

	public Transform follow, player;						// Transforms that we use to follow and look at. Follow follows the player

	[Header("Tunnel")]
	public Vector3 zTunnelOffset = new Vector3(0, -1.0f, -2.5f);
	public float tunnelDamping = 2.0f;

	private Vector3 tunnelVel;
	private Vector3 startingPos;							// Position of the camera at the start of the game
	private TunnelObserver tunnelObserver;

	//Rotation
	private float xSpeed;
	private float ySpeed;
	private float rotVel;
	private float mouseSpeedDamping = 1.0f;							// How much to damp the mouse movement for rotation
	private float currentMaxY;
	private float currentMinY;
	private float lastYSpeed;
	private float lastXSpeed;

	//climbing rotation
	private float rotateAroundSpeed;
	private float angleDifference;							// How much the camera will rotate on the Y axis when the character is edge sliding
	private Vector3 lastCamPos;

	//private Vector3 vel;
	//private Movement
	private Vector3 targetPos;
	private float zOffsetVel;
	private float _zOffset;	
													// Keep track of original z offset
	// temp climb vars
	private Vector3 rightDir;
	private Vector3 backwardsDir;
	private Vector3 camDir;
	private float dot;
	private Vector3 climbRotVel;

	// Zoom in vars
	private Vector3 zoomVel;

	//Collision
	private float _moveLerpSpeed = 40.0f;					// Original move lerp speed that we will use to set the public lerp speed to
	private Vector3 collisionVel;
	private float moveSmoothVel;
	private Vector3 collideDamp;

	public enum CamState
	{
		ZoomIn,
		Free,
		ClimbingTransition,
		ClimbCam,
		CloseBehindTransition
	}
	
	[HideInInspector]
	public CamState state;

	// Ignore ground/player layers
	private int layerMask = ~((1 << 8) | (1 << 9) | (1 << 14));
	[HideInInspector] 
	public bool colliding = false;

	private Vector3 origin;
	private RaycastHit hit;
	private Vector3 vel;

	private void Awake () {}

	private void Start ()
	{
		// Record the private _moveLerpSpeed so we have the original value
		// We'll use it later as we transition out of a collision to regular mode
		_moveLerpSpeed = moveSpeed;
		_zOffset = offset.z;

		//follow = GameObject.FindGameObjectWithTag("Follow").transform;
		//player = GameObject.FindGameObjectWithTag("Player").transform;

		// Starting camera state and position
		state = CamState.Free;
		startingPos = follow.position + follow.forward * (-offset.z - 2);
		startingPos.y += offset.y + 2;

		transform.position = startingPos;
		tunnelObserver = GameManager.Instance.tunnelObserver;
	}

	private void LateUpdate()
	{
		switch(state)
		{
			case CamState.Free:

				MoveCamera();

				CollideCamera();

				RotateCamera();
					
				break;

			case CamState.ClimbCam:

				ClimbMoveCamera();

				rightDir = follow.right * -offset.z;
				//backwardsDir = follow.forward * -offset.z; // get rid of this?
				camDir = transform.position - follow.position;
				dot = Vector3.Dot(rightDir.normalized, camDir.normalized);
		
//				Debug.DrawRay(follow.position, rightDir, Color.green);
//				Debug.DrawRay(follow.position, camDir, Color.red);
				CollideCamera();

				ClimbRotateCamera();

				break;

			case CamState.ClimbingTransition:

				//ClimbMoveCamera();
				targetPos = follow.position + follow.forward * -offset.z;
				transform.position = Vector3.Lerp(transform.position, targetPos, 8.0f * Time.deltaTime);

				transform.LookAt(follow);

				if (Vector3.Distance(transform.position, targetPos) < transThreshold)
				{
					SetState(CamState.ClimbCam);
				}

				break;

			case CamState.CloseBehindTransition:

				Vector3 direction = (transform.position - follow.position).normalized * 2.5f;
				targetPos = follow.position + direction;
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref tunnelVel, 0.5f);
				transform.LookAt(follow);

				//CollideCameraLog();

				break;

			case CamState.ZoomIn:

				targetPos = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;
				targetPos.y = follow.position.y + 1;
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref zoomVel, 25.0f * Time.deltaTime);

				transform.LookAt(follow);

			if (Vector3.Distance(targetPos, transform.position) < 0.028f)
			{
				SetState(CamState.Free);
				EventManager.OnCharEvent(GameEvent.CamZoomedIn);
			}
				break;

		}
	}

	/// <summary>
	/// When the camera isn't colliding with anything
	/// Use a SmoothDamp to animate the moveLerpSpeed. 
	/// We do this because while colliding camera moves at a slower pace
	/// Because of this when we exit the collision there is a quick jump in speed the camera makes because the lerp speed goes
	/// back to the regular move lerp speed. With this method we can smooth that out.
	/// </summary>
	private void MoveCamera()
	{
		// Make the camera follow the Follow GO like its a string attached to it
		if (!colliding)
		{ 
			moveSpeed = Mathf.SmoothDamp(moveSpeed, _moveLerpSpeed, ref moveSmoothVel, collisionToMoveSpeedDamping * Time.deltaTime);
			offset.z = Mathf.SmoothDamp(offset.z, _zOffset, ref zOffsetVel, zOffsetDamping * Time.deltaTime);
			targetPos = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;
			transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
		}
	}

	private void CollideCamera ()
	{
		origin = player.position + Vector3.up;
		Vector3 direction = new Vector3(transform.position.x, origin.y + collisionRaycastYOffset, transform.position.z) - origin;
		direction = direction.normalized;

		Debug.DrawRay(origin, direction * 5, Color.red);
		//Debug.Break();

		// TODO when camera LERPs up, the raycast doesn't hit anything and that's why its jittery
		if (Physics.Raycast(origin, direction * 5, out hit, 5, layerMask))
		{
			targetPos = new Vector3(hit.point.x, currentMaxY - 0.5f, hit.point.z);
			//targetPos = targetPos + hit.normal * 0.1f;

			transform.position = Vector3.Lerp(transform.position, targetPos, collisionSpeed * Time.deltaTime);
			//transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref collideDamp, 4f);
			moveSpeed = collisionSpeed;
			offset.z = Vector3.Distance(transform.position, follow.position);
			colliding = true;
			//Debug.DrawLine(transform.position, hit.point, Color.black);
		}
		else
		{
			colliding = false;
		}

	}

	/// <summary>
	/// RotatePlayer()
	/// Used to Rotate the camera around the player during LateUpdate.
	/// We limit X axis rotation by making sure the camera is never too far below or above the follow
	/// There is also collision code that has to be taken into account here
	/// </summary>
	private void RotateCamera()
	{
//		if (GameManager.Instance.tunnelObserver.inTunnel)
//		{
			currentMinY = follow.position.y - offset.y;
			currentMaxY = follow.position.y + offset.y;
//			print("cam:in tunnel");
//		}
//		else
//		{
			// Get the min/max positions the camera should not exceed
			//currentMinY = follow.position.y - 0.0f;
			//currentMaxY = follow.position.y + 0.0f;

//		}

		xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, mouseSpeedDamping * Time.deltaTime);
		ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, mouseSpeedDamping * Time.deltaTime);
		

		// limit the mouse's Y posiiton. Make sure to invert the Y
		ySpeed = (transform.position.y <= currentMinY && ySpeed > 0) || (transform.position.y >= currentMaxY && ySpeed < 0) ? 0 : -ySpeed;

		// Handle camera going exceeding min and max positions
		if (transform.position.y <= currentMinY - 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMinY, transform.position.z), 10.0f * Time.deltaTime);
	
		else if (transform.position.y >= currentMaxY + 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMaxY, transform.position.z), 10.0f * Time.deltaTime);

		transform.RotateAround (follow.position, transform.right, Mathf.Lerp(lastYSpeed, ySpeed, 20.0f * Time.deltaTime));
		transform.RotateAround (follow.position, Vector3.up, Mathf.Lerp(lastXSpeed, xSpeed, 20.0f * Time.deltaTime));

		transform.LookAt (follow);
			 				
		lastYSpeed = ySpeed;
		lastXSpeed = xSpeed;
	}

	/// <summary>
	/// The same MoveCamera() but with a higher Y value (so that the camer is more overhead)
	/// </summary>
	private void ClimbMoveCamera()
	{
		if (colliding || InputController.h == 0)
			return;

		targetPos = follow.position + follow.forward * -offset.z;
		transform.position = Vector3.Lerp(transform.position, targetPos, 3.0f * Time.deltaTime);
		
	}


	/// <summary>
	/// RotatePlayer()
	/// This is an altered version of the RotateCamera() method
	/// Like that method, this one allows the player to orbit around the player but has some limitations:
	/// 1) you can't
	/// </summary>
	private void ClimbRotateCamera()
	{
		// Get the min/max positions the camera should not exceed
		currentMinY = follow.position.y - offset.y;
		currentMaxY = follow.position.y + offset.y;

		xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, mouseSpeedDamping * Time.deltaTime);
		ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, mouseSpeedDamping * Time.deltaTime);

		// limit the mouse's Y posiiton. Make sure to invert the Y
		ySpeed = (transform.position.y <= currentMinY && ySpeed > 0) || (transform.position.y >= currentMaxY && ySpeed < 0) ? 0 : -ySpeed;

		// Handle camera going exceeding min and max positions
		if (transform.position.y <= currentMinY - 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMinY, transform.position.z), 10.0f * Time.deltaTime);
	
		else if (transform.position.y >= currentMaxY + 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMaxY, transform.position.z), 10.0f * Time.deltaTime);

		// Limit the Y rotation when climbing
		if (dot > climbXClampThreshold && xSpeed > 0 || dot < -climbXClampThreshold && xSpeed < 0)
			xSpeed = 0;

		// if player moving and climbing, rotate along with
		// them and revoke camera rotation control from the player
//		if (InputController.h != 0)
//		{
////			angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, follow.eulerAngles.y);
////			transform.RotateAround(follow.position, Vector3.up, Mathf.Lerp(0, angleDifference, 3f * Time.deltaTime));
////			transform.RotateAround (follow.position, transform.right, Mathf.Lerp(lastYSpeed, ySpeed, maxClimbRotSpeed * Time.deltaTime));
//		}
//		else
//		{
		if (InputController.h == 0)
		{
			rotateAroundSpeed = Mathf.Lerp(rotateAroundSpeed, maxClimbRotSpeed, 10.0f * Time.deltaTime);
			transform.RotateAround (follow.position, Vector3.up, Mathf.Lerp(lastXSpeed, xSpeed, rotateAroundSpeed * Time.deltaTime));
			transform.RotateAround (follow.position, transform.right, Mathf.Lerp(lastYSpeed, ySpeed, maxClimbRotSpeed * Time.deltaTime));
			lastYSpeed = ySpeed;
			lastXSpeed = xSpeed;
		}
		transform.LookAt (follow);	 				
		
	}

	private void OnEnable ()
	{
		EventManager.onCharEvent += SetCameraMode;
		EventManager.onInputEvent += SetCameraMode;
	}
	
	private void OnDisable()
	{
		EventManager.onCharEvent -= SetCameraMode;
		EventManager.onInputEvent -= SetCameraMode;
	}

	private void SetCameraMode (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StartEdgeClimbing || gEvent == GameEvent.StartVineClimbing)
		{
			SetState(CamState.ClimbingTransition);
		}
		else if (gEvent == GameEvent.StopEdgeClimbing || gEvent == GameEvent.StopVineClimbing)
		{
			SetState(CamState.Free);
		}
		else if (gEvent == GameEvent.EnterTunnel)
		{
			print("cam: enter tunnel");
			SetState(CamState.CloseBehindTransition);
		}
		else if (gEvent == GameEvent.ExitTunnel)
		{
			SetState(CamState.Free);
		}
	}

	private void SetState (CamState s) { state = s; }
	private CamState GetState() { return state; }

	private bool Cam_ClimbState ()
	{
		return state == CamState.ClimbCam;
	}

	private bool Cam_ClimbTransition ()
	{
		return state == CamState.ClimbingTransition;
	}

//	void OnDrawGizmos() 
//	{
//        Gizmos.color = Color.green;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMinY, transform.position.z), 0.2f);
//
//    }

}