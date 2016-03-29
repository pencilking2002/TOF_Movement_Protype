using UnityEngine;
using System.Collections;

public class AntiWallSlideController : MonoBehaviour {

	public static AntiWallSlideController Instance;

	[HideInInspector]
	public bool colliding = false;
	public Transform cColliderFrontTransf;

	[Range(0,2)]
	public float jumpWallLimitRayLength = 0.3f;

	[Range(0,2)]
	public float runWallLimitRayLength = 0.2f;

	public float sideRayAngle = 0.2f;

	private Ray rayLeft, rayCenter, rayRight;
	private Vector3 origin;
	private Vector3 direction;
	private RomanCharState charState;

	void Awake ()
	{
		if (Instance == null) 
			Instance = this; 
		else 
			Destroy(this);
	}

	void Start ()
	{
		if (cColliderFrontTransf == null)
			Debug.LogWarning("cColliderFrontTransf not defined");

		charState = GetComponent<RomanCharState>();
	
	}

	void FixedUpdate ()
	{
		if (charState.IsRunning())
			colliding = CastRunRays();
		else if (charState.IsInAnyJumpingState())
			colliding = CastJumpRays();
	}

	private bool CastRunRays()
	{
		// If jumping against a surface and pressing forward, eliminate forward velocity
		origin = cColliderFrontTransf.position - transform.forward * 0.2f;
		direction = transform.forward * jumpWallLimitRayLength;
		rayCenter = new Ray(origin, direction);

		Debug.DrawRay(rayCenter.origin, rayCenter.direction * jumpWallLimitRayLength, Color.black);

		return Physics.Raycast(rayCenter, jumpWallLimitRayLength);
		
	}

	private bool CastJumpRays ()
	{
		// If jumping against a surface and pressing forward, eliminate forward velocity
		origin = cColliderFrontTransf.position - transform.forward * 0.2f;
		direction = transform.forward * jumpWallLimitRayLength;

		rayLeft = new Ray(origin, direction - transform.right * sideRayAngle);
		rayCenter = new Ray(origin, direction);
		rayRight = new Ray(origin, direction + transform.right * sideRayAngle);

		Debug.DrawRay(rayLeft.origin, rayLeft.direction *jumpWallLimitRayLength, Color.black);
		Debug.DrawRay(rayCenter.origin, rayCenter.direction * jumpWallLimitRayLength, Color.black);
		Debug.DrawRay(rayRight.origin, rayRight.direction * jumpWallLimitRayLength, Color.black);

		return	Physics.Raycast(rayLeft, jumpWallLimitRayLength) || 
				Physics.Raycast(rayCenter, jumpWallLimitRayLength) ||
				Physics.Raycast(rayRight, jumpWallLimitRayLength);
	}
}
