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
	public float rotateDamping = 2.0f;
	public float transThreshold = 0.02f;

	// Collision
	public float collisionSpeed = 15.0f;				
	public float collisionToMoveSpeedDamping = 40.0f;	    // Amount of damping to apply when SmoothDamping the speed while exiting a collision
	public float collisionRaycastYOffset = 5.0f;			// How far up from should the collision raycast be offset from the player's y position
	//public LayerMask collisionLayer;						// the layers the camera will collide against

	public Transform follow, player;						// Transforms that we use to follow and look at. Follow follows the player

	[Header("Tunnel")]
	public Vector3 zTunnelOffset = new Vector3(0, -1.0f, -2.5f);
	public float tunnelDamping = 2.0f;

	private Vector3 tunnelVel;
	private Vector3 startingPos;							// Position of the camera at the start of the game
//	private TunnelObserver tunnelObserver;

	//Rotation
	private float xSpeed;
	private float ySpeed;
	private float rotVel;
	private float autoRotVel;
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
//	private float _moveLerpSpeed = 40.0f;					// Original move lerp speed that we will use to set the public lerp speed to
	private Vector3 collisionVel;
	private Vector3 moveSmoothVel;
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

	public Vector3 targetYPosOffset = new Vector3(0, 3.4f, 0);
	public float distanceFromTarget = -6;
	public float zoomSmooth = 100;
	public float zoomStep = 2;
	public float maxZoom = -2;
	public float minZoom = -15;
	public bool smoothFollow = false;
	public float moveSmoothDamp = 0.05f;
	public float lookSmooth = 100f;

	[HideInInspector]
	public float newDistance = -6; 			// Set by zoom input

	[HideInInspector]
	public float adjustmentDistance = -8;

	// Orbit settings -------------------
	public float xRotation = -20;
	public float yRotation = -180;

	// TODO: reverse the names of max/min
	public float maxXRotation = 25;
	public float minXRotation = -85;
	public float vOrbitSpeed = 150;
	public float hOrbitSpeed = 150;
	private float vOrbitInput;
	private float hOrbitInput;
	private float zoomInput;
	public Quaternion targetRotation;

	[System.Serializable]
   	public class DebugSettings
   	{
   		public bool drawDesiredCollisionLines = true;
   		public bool drawAdjustedCollisionLines = true;
   	}

	public DebugSettings debug = new DebugSettings();
	public CollisionHandler collision = new CollisionHandler();


	private Vector3 targetPos = Vector3.zero;
	private Vector3 destination = Vector3.zero;
	private Vector3 adjustedDestination = Vector3.zero; // for collision
	private Vector3 camVel = Vector3.zero;
	private RomanCharState charState;				
	private float getBehindVel;
	private float angleDelta;
	private float targetRot;
	private int signedDirection;
	private float xRotVel;
	private float yRotVel;
	private Vector3 previousMousePos = Vector3.zero;
	private Vector3 currentMousePos = Vector3.zero;

	private RomanCharController charController;


	private void Awake () {}

	private void Start ()
	{
		charState = GameManager.Instance.charState;
		SetState(CamState.Free);
		SetTarget(follow);
//		tunnelObserver = GameManager.Instance.tunnelObserver;
		MoveToTarget();

		collision.Initialize(Camera.main);
		collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
		//previousMousePos = currentMousePos = Input.mousePosition;
	}

	private void LateUpdate()
	{
		switch(state)
		{
			case CamState.Free:

//				MoveToTarget();
//				LookAtTarget();
				//CollideCamera();

				break;

			case CamState.ClimbCam:

				ClimbMoveCamera();

				rightDir = follow.right * distanceFromTarget;
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
				targetPos = follow.position + follow.forward * distanceFromTarget;
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

				targetPos = follow.position + Vector3.Normalize(follow.position - transform.position) * distanceFromTarget;
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

//	private void FixedUpdate ()
//	{
//		collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
//		collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
//	}

	private void Update ()
	{
		GetInput();
		ZoomInOnTarget();
		MoveToTarget();
		LookAtTarget();
		OrbitTarget();

		collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
		collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

		if (GameManager.Instance.debug)
			{
			for (int i = 0; i < 5; i++)
			{
				if (debug.drawDesiredCollisionLines)
					Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
				
				if (debug.drawAdjustedCollisionLines)
					Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
				
			}
		}

		collision.CheckColliding(targetPos);
		//float pos = collision.GetAdjustedDistanceWithRayFrom(targetPos);
		//adjustmentDistance = Mathf.Lerp(adjustmentDistance, pos, 2.0f * Time.deltaTime);
		adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);
	}

	private void MoveToTarget()
	{
		// Set targetPos to be a bit above the player
//		targetPos = target.position + targetYPosOffset;

		if (charState.IsRunningOrSprinting())
		{
			//Debug.DrawRay(target.position, target.forward * 5.0f, Color.blue);
			//Debug.DrawRay(transform.position, transform.right * 5.0f, Color.red);

			// Check to see if camera is on the left or right of the player
			// -1 = left, 1 = right
			dot = Vector3.Dot(transform.right.normalized, target.forward.normalized);
			signedDirection = dot < 0 ? -1 : 1;

			// used to check to see if camera is behind player
			angleDelta = Mathf.Abs(Vector3.Angle(transform.forward, target.transform.forward) - 180);

			//print(dot);
			//print(angleDelta);
			// if Camera is not directly behind the player && 
			// if player is facing away from the camera &&
			// if they are using not rotating the camera
			// then slowly rotate the camera to be behind the player
			if (Mathf.Abs(dot) > 0.01f && angleDelta > 85 && hOrbitInput == 0)
			{
				targetRot = Mathf.SmoothDamp(targetRot, 60.0f * signedDirection, ref autoRotVel, rotateDamping);
			}

			print(angleDelta);
		
		}

		targetPos = target.position + Vector3.up * targetYPosOffset.y + Vector3.up; //* targetYPosOffset.z /*+ transform.TransformDirection(Vector3.forward)*/;
		destination = Quaternion.Euler(xRotation, yRotation + targetRot, 0) * -Vector3.forward * distanceFromTarget;
		destination += targetPos;

		if (collision.colliding)
		{
			adjustedDestination = Quaternion.Euler(xRotation, yRotation + targetRot, 0) * Vector3.forward * adjustmentDistance;
			adjustedDestination += targetPos;

			//transform.position = Vector3.Lerp(transform.position, adjustedDestination, 10 * Time.deltaTime); //transform.position = adjustedDestination;
			transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref moveSmoothVel, moveSmoothDamp);
		}
		else
		{
			//transform.position = Vector3.Lerp(transform.position, destination, 10 * Time.deltaTime); //transform.position = adjustedDestination;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref moveSmoothVel, moveSmoothDamp);

		}
	
	}

	private void SetTarget(Transform t)
	{
		target = t;
	}

	private void GetInput ()
	{
		vOrbitInput = Mathf.Clamp(InputController.orbitV, -1, 1);
		hOrbitInput = Mathf.Clamp(InputController.orbitH, -1, 1);
		// TODO: Fix this to work with a controller
		zoomInput = Input.GetAxisRaw("Mouse ScrollWheel");
	}

	private void LookAtTarget ()
	{

		transform.LookAt(target);
	}

	private void OrbitTarget()
	{

		if (Input.GetKeyDown(KeyCode.G))
		{
			// TODO: Turn this into a lerp later
			yRotation = 180;
		}
		else
		{
			xRotation += InputController.orbitV * vOrbitSpeed * Time.deltaTime;
			yRotation += InputController.orbitH * hOrbitSpeed * Time.deltaTime;

			//xRotation = Mathf.SmoothDamp(xRotation, xRotation + vOrbitInput * vOrbitSpeed, ref xRotVel, 0.5f);
			//yRotation = Mathf.SmoothDamp(yRotation, yRotation + hOrbitInput * hOrbitSpeed, ref yRotVel, 0.5f);

			//xRotation = Mathf.Lerp(xRotation, xRotation + vOrbitInput * vOrbitSpeed, 5.0f * Time.deltaTime);
			//yRotation = Mathf.Lerp(yRotation, yRotation + hOrbitInput * hOrbitSpeed, 5.0f * Time.deltaTime);

			if (xRotation > maxXRotation)
				xRotation = maxXRotation;
			
			else if (xRotation < minXRotation)
				xRotation = minXRotation;
		}
		

	}

	void ZoomInOnTarget()
	{
		distanceFromTarget += zoomInput * zoomSmooth * Time.deltaTime;

		if (distanceFromTarget > maxZoom)
			distanceFromTarget = maxZoom;

		else if (distanceFromTarget < minZoom)
			distanceFromTarget = minZoom;
	}

	//END  TUT ----------------------


	/// <summary>
	/// The same MoveCamera() but with a higher Y value (so that the camer is more overhead)
	/// </summary>
	private void ClimbMoveCamera()
	{
		if (colliding || InputController.h == 0)
			return;

		targetPos = follow.position + follow.forward * distanceFromTarget;
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
//		else if (gEvent == GameEvent.EnterTunnel)
//		{
//			print("cam: enter tunnel");
//			SetState(CamState.CloseBehindTransition);
//		}
//		else if (gEvent == GameEvent.ExitTunnel)
//		{
//			SetState(CamState.Free);
//		}
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

   [System.Serializable]
   public class CollisionHandler
   {
   		public LayerMask collisionLayer;

   		[HideInInspector]
   		public bool colliding = false;

   		[HideInInspector]
   		public Vector3[] adjustedCameraClipPoints;

   		[HideInInspector]
   		public Vector3[] desiredCameraClipPoints;

   		Camera camera;

		private Vector3 camPos;
		private Vector3 someVel;

   		public void Initialize(Camera cam)
   		{
			
		   	camera = cam;
			camPos = cam.transform.position;
   			adjustedCameraClipPoints = new Vector3[5];
   			desiredCameraClipPoints = new Vector3[5];
   		}

		public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
   		{

			//camPos = Vector3.SmoothDamp(camPos, cameraPosition, ref someVel, 0.2f);   
   			// Clear the contents of intoArray
   			intoArray = new Vector3[5];

   			float z = camera.nearClipPlane;
   			float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
   			float y = x / camera.aspect;

   			//Top left
   			intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; // Added and rotated point relative to camera

   			//Top right
			intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition; 

   			//Bottom left
			intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition; 

   			//Bottom right
			intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition; 

			// Camera's position - subtract the cam's forward to bring the position back a bit
			intoArray[4] = cameraPosition - camera.transform.forward;
   		}

   		// Cast rays from the player to the clip points
   		bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
   		{
   			for(int i = 0; i < clipPoints.Length; i++)
   			{
   				Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
   				float distance = Vector3.Distance(clipPoints[i], fromPosition);
				if (Physics.Raycast(ray, distance, collisionLayer))
   				{
   					return true;
   				}
   			}

			return false;

   		}

   		// Returns the distance the camera needs to be from the target
   		public float GetAdjustedDistanceWithRayFrom(Vector3 from)
   		{
   			float distance = -1;

   			for (int i = 0; i < desiredCameraClipPoints.Length; i++)
   			{
   				// cast the ray from the target to each desired clip point
   				Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
   				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, Vector3.Distance(desiredCameraClipPoints[i],from), collisionLayer))
   				{
   					//print (hit.collider.gameObject.name);
   					//if (hit.collider.gameObject.layer == 15)
   						//return 0;

   					if (distance == -1)				// If the distance hasn't been set yet, set
   						distance = hit.distance;
   					else   							// If it has been set, check if the hti distance is smaller than the previous distance, if so set it
   					{
   						if (hit.distance < distance)
   							distance = hit.distance;
   					}
   				}
   			}

   			// If distance never got set, return 0, else return the distance
   			if (distance == -1)
   				return 0;
   			else
   				return distance;
   		}

   		public void CheckColliding(Vector3 targetPosition)
   		{
   			if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
   				colliding = true;
   			else
   				colliding = false;
   		}
   }

}