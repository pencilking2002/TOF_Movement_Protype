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

	private Ray rayLeft, rayCenter, rayRight;
	private Vector3 origin, origin2;
	private Vector3 direction, direction2;
	private RomanCharState charState;
	private RaycastHit hit;
	private Ray ray1, ray2;
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
		if (TunnelObserver.Instance.inTunnel || charState.IsClimbing())
			return;

		if (charState.IsSprinting ())
		{
			origin = transform.position + new Vector3 (0, 0.4f, 0) + transform.forward * 0.4f;
			ray1 = new Ray (origin, Vector3.down);
			Debug.DrawRay(ray1.origin, ray1.direction * rayLength, Color.red);

			if (charState.IsFalling() && JumpController.TimePassedSinceJump(2f))
			{
				onSloap = true;
			}

			else if (Physics.Raycast (ray1, out hit, rayLength, layerMask)) 
			{
				float dot = Vector3.Dot (transform.forward, hit.normal);
				onSloap = (dot < -0.7f);
				//print(dot);
				return;
			}
		}
		else
		{
			origin = transform.position + new Vector3 (0, 0.1f, 0) + transform.forward * 0.2f;
			origin2 = transform.position + new Vector3 (0, 0.1f, 0) + transform.forward * 0.25f;
			ray1 = new Ray (origin, Vector3.down);
			ray2 = new Ray (origin2, Vector3.down);
			Debug.DrawRay(ray2.origin, ray2.direction * rayLength, Color.red);

			if (charState.IsFalling() && JumpController.TimePassedSinceJump(2f))
			{
				onSloap = true;
			}

			else if (Physics.Raycast (ray1, out hit, rayLength, layerMask) || Physics.Raycast (ray2, out hit, rayLength, layerMask)) 
			{
				float dot = Vector3.Dot (transform.forward, hit.normal);
				onSloap = (dot < -0.7f);
				//print(dot);
				return;
			}
		}
	
		

		onSloap = false;
	}
}
