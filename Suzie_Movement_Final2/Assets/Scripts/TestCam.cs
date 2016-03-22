//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class TestCam : MonoBehaviour 
{
	// Movement
	//public Vector3 offset = new Vector3(0, 2, 5);
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

	// TUT ----------------------------

	//movement -------------------
	private Transform target;
	private RomanCharState charState;

	public Vector3 targetYPosOffset = new Vector3(0, 3.4f, 0);
	public float distanceFromtarget = -8;
	public float lookSmooth = 100f;
	public float zoomSmooth = 100;

	// Zooming -------------------
	public float maxZoom = -2;
	public float minZoom = -15;

	// Orbit settings -------------------
	public float xRotation = -20;
	public float yRotation = -180;

	// TODO: reverse the names of max/min
	public float maxXRotation = 25;
	public float minXRotation = -85;
	public float vOrbitSmooth = 150;
	public float hOrbitSmooth = 150;
	private float vOrbitInput;
	private float hOrbitInput;
	private float zoomInput;

	public Quaternion targetRotation;

	private Vector3 targetPos = Vector3.zero;
	private Vector3 destination = Vector3.zero;
	private float getBehindVel;
	private float angleDelta;
	private float targetRot;
	private int signedDirection;
	private float xRotVel;
	private float yRotVel;

	private RomanCharController charController;
	
	private void Awake () {}

	private void Start ()
	{
		charState = GameManager.Instance.charState;
		SetState(CamState.Free);
		SetTarget(follow);
		tunnelObserver = GameManager.Instance.tunnelObserver;
		MoveToTarget();
	}

	private void LateUpdate()
	{
		switch(state)
		{
			case CamState.Free:

				MoveToTarget();
				LookAtTarget();
				//CollideCamera();

				break;

			case CamState.ClimbCam:

				ClimbMoveCamera();

				rightDir = follow.right * distanceFromtarget;
				//backwardsDir = follow.forward * -offset.z; // get rid of this?
				camDir = transform.position - follow.position;
				dot = Vector3.Dot(rightDir.normalized, camDir.normalized);
		
//				Debug.DrawRay(follow.position, rightDir, Color.green);
//				Debug.DrawRay(follow.position, camDir, Color.red);
				//CollideCamera();

				ClimbRotateCamera();

				break;

			case CamState.ClimbingTransition:

				//ClimbMoveCamera();
				targetPos = follow.position + follow.forward * distanceFromtarget;
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

				targetPos = follow.position + Vector3.Normalize(follow.position - transform.position) * distanceFromtarget;
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
	// TUT ----------------------

	private void Update ()
	{
		GetInput();
		OrbitTarget();
		ZoomInOnTarget();
	}

	private void SetTarget(Transform t)
	{
		target = t;
	}

	private void GetInput ()
	{
		vOrbitInput = Mathf.Clamp(InputController.orbitV, -1, 1);
		hOrbitInput = Mathf.Clamp(InputController.orbitH, -1, 1);
		zoomInput = Input.GetAxisRaw("Mouse ScrollWheel");
	}

	private void MoveToTarget()
	{
		// Set targetPos to be a bit above the player
		targetPos = target.position + targetYPosOffset;

		if (charState.IsRunningOrSprinting())
		{
			Debug.DrawRay(target.position, target.forward * 5.0f, Color.blue);
			Debug.DrawRay(transform.position, transform.right * 5.0f, Color.red);

			dot = Vector3.Dot(transform.right.normalized, target.forward.normalized);
			// -1 = left, 1 = right
			signedDirection = dot < 0 ? -1 : 1;

			// used to check to see if camera is behind player
			angleDelta = Mathf.Abs(Vector3.Angle(transform.forward, target.transform.forward) - 180);

			print(hOrbitInput);

			if (Mathf.Abs(dot) > 0.01f && angleDelta > 60 && hOrbitInput == 0)
			{
				//targetRot += 30.0f * signedDirection * Time.deltaTime;
				targetRot = Mathf.SmoothDamp(targetRot, targetRot + 60.0f * signedDirection, ref rotVel, 2.0f); 
				print("sweet spot");
			}
		
		}
		
		//Set destination to equal a rotation (based on input) and multiply that to go behind the target's forward
		destination = Quaternion.Euler(xRotation, yRotation + targetRot, 0) * -Vector3.forward * distanceFromtarget;

		// Add the targetPos to the destination to place the camera a bit above the target
		destination += targetPos;

		// Finall set the position
		transform.position = destination;

		targetPos = transform.position + (target.position - transform.position).normalized * distanceFromtarget;
		transform.position = targetPos;


//		targetPos = target.position + (target.position - transform.position).normalized * distanceFromtarget;
//		targetPos.y = target.position.y + targetYPosOffset.y + yDifference;
//		transform.position = targetPos;
//		transform.eulerAngles = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), target.up) * targetPos;

		//transform.RotateAround(target.position, transform.up, hOrbitInput * hOrbitSmooth * Time.deltaTime);
		//transform.RotateAround(target.position, transform.right, -vOrbitInput * vOrbitSmooth * Time.deltaTime);
		//transform.RotateAround(target.position, target.up, yRotation - transform.eulerAngles.y);
		//transform.RotateAround(target.position, transform.right, -xRotation - transform.eulerAngles.x);

	
		// Subtract the current position (after its been rotated) from the previous targetPos.y
		// and add that to the y difference. We add instead of setting it so that we can maintain the offset
		// as opposed to constantly resetting it
//		yDifference += transform.position.y - targetPos.y;

//		transform.position = target.position + targetYPosOffset;
//		transform.eulerAngles += target.eulerAngles * Input.GetAxis("Mouse X") * hOrbitSmooth * Time.deltaTime;
//		print(hOrbitInput);
//		transform.position += transform.forward * distanceFromtarget;


	
	}


	private void LookAtTarget ()
	{
//		targetRotation = Quaternion.LookRotation(targetPos - transform.position);
//		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSmooth * Time.deltaTime);
		transform.LookAt(target);
	}

	private void OrbitTarget()
	{
//		if (charState.IsRunningOrSprinting() && hOrbitInput == 0)
//		{
//			yRotation = Mathf.SmoothDamp(yRotation, 180, ref getBehindVel, 30.0f);
//			//yRotation = 180;
//		}
//		else 

		if (Input.GetKeyDown(KeyCode.G))
		{
			// TODO: Turn this into a lerp later
			yRotation = 180;
		}
		else
		{
			//xRotation += vOrbitInput * vOrbitSmooth * Time.deltaTime;
			//yRotation += hOrbitInput * hOrbitSmooth * Time.deltaTime;
			xRotation = Mathf.SmoothDamp(xRotation, xRotation + vOrbitInput * vOrbitSmooth, ref xRotVel, 0.5f);
			yRotation = Mathf.SmoothDamp(yRotation, yRotation + hOrbitInput * hOrbitSmooth, ref yRotVel, 0.5f);

			if (xRotation > maxXRotation)
				xRotation = maxXRotation;
			
			else if (xRotation < minXRotation)
				xRotation = minXRotation;
		}
		

	}

	void ZoomInOnTarget()
	{
		distanceFromtarget += zoomInput * zoomSmooth * Time.deltaTime;

		if (distanceFromtarget > maxZoom)
			distanceFromtarget = maxZoom;

		else if (distanceFromtarget < minZoom)
			distanceFromtarget = minZoom;
	}

	//END  TUT ----------------------


	/// <summary>
	/// The same MoveCamera() but with a higher Y value (so that the camer is more overhead)
	/// </summary>
	private void ClimbMoveCamera()
	{
		if (colliding || InputController.h == 0)
			return;

		targetPos = follow.position + follow.forward * distanceFromtarget;
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
		currentMinY = follow.position.y - targetYPosOffset.y;
		currentMaxY = follow.position.y + targetYPosOffset.y;

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

	void OnDrawGizmos() 
	{
        Gizmos.color = Color.green;
		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMinY, transform.position.z), 0.2f);

   }

}