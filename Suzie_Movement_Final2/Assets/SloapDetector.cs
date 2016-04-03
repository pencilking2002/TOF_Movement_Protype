using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SloapDetector : MonoBehaviour {

	public static SloapDetector Instance;
	public LayerMask layerMask;
	public float rayLength = 0.5f;

	[HideInInspector]
	public bool colliding = false;

	[HideInInspector]
	public bool onSloap = false;

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

	/// <summary>
	/// Check if character is on a surfaces that's too steep
	/// Uses the char's forward instead of Vector3.down because that way
	/// we cna ignore steep hills
	/// </summary>
	private void CheckSloap()
	{
		if (TunnelObserver.Instance.inTunnel)
			return;

		if (charState.IsSprinting ())
			offset = 0.4f;
		else
			offset = 0.2f;
		
		origin = transform.position + new Vector3 (0, 0.1f, 0) + transform.forward * offset;
		ray = new Ray (origin, Vector3.down);

		Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

		if (Physics.Raycast (ray, out hit, rayLength, layerMask)) 
		{
			float dot = Vector3.Dot (transform.forward, hit.normal);
			onSloap = (dot < -0.7f);
			return;
		}

		onSloap = false;
	}
}
