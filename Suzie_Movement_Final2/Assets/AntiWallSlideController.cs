using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntiWallSlideController : MonoBehaviour {

	public static AntiWallSlideController Instance;
	public LayerMask layerMask;
	public float rayLength = 0.5f;

	[HideInInspector]
	public bool colliding = false;

	[HideInInspector]
	public bool onSloap = false;

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
	private RaycastHit hit;
	private Ray ray;
	private float offset;

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


		ComponentActivator.Instance.Register (this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.StartClimbing, false },
			{ GameEvent.StopClimbing, true }

		});
	
	}

	void FixedUpdate ()
	{
		CheckSloap ();
	}

	private void CheckSloap()
	{
		if (charState.IsRunning ())
			offset = 0.2f;
		else if (charState.IsSprinting ())
			offset = 0.4f;
		else
			offset = 0;
		
		origin = transform.position + new Vector3 (0, 0.1f, 0) + transform.forward * offset;
		ray = new Ray (origin, Vector3.down);

		Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

		if (Physics.Raycast (ray, out hit, rayLength, layerMask)) 
		{
			float dot = Vector3.Dot (Vector3.up, hit.normal);
			//print (dot);

			onSloap = (dot < 0.7f);
			return;
		}

		onSloap = false;
	}
}
