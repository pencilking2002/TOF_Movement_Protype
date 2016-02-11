using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform player;

	public Vector3 offset;
	public float damping = 10.0f;
	public float climbSpeed = 20.0f;

	public bool Attach = true;
	
	[HideInInspector]
	public bool followAtPlayerPos = false;
	
	private Vector3 vel;
	private Vector3 rotVel;
	private Vector3 targetPos;
	private Vector3 targetRot;



	private RomanCharState charState;
	private float _damping;
	private float climbSpeedSmoothVel;

	private TestCam cam;

	void Awake () 
	{
	}

	void Start ()
	{
		charState = GameObject.FindObjectOfType<RomanCharState>();

		if(player == null)
			player = GameObject.FindGameObjectWithTag("Player").transform;

		cam = GameObject.FindObjectOfType<TestCam>();
	}
	// Update is called once per frame
	void Update () 
	{

		//targetPos = cam.colliding ? player.position + offset - Vector3.up * 0.5f :
		targetPos = player.position + offset;

		// If the follow is not supposed to be attached to player
		// retain existing y position (don't bounce)
		if (charState.IsJumping() || charState.IsLanding())
		{
			targetPos.y = transform.position.y;
			//print("blah");
		}
		if (charState.IsClimbing())
		{
			_damping = climbSpeed;
		}
		else
		{
			_damping = damping;
		}
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, _damping * Time.deltaTime);
//		
		// Check if the follow object has caught up with the player and that the follow object is above the camera
		if (Vector3.Distance(transform.position, targetPos) < 0.05f)
			followAtPlayerPos = true;
		
		else
			followAtPlayerPos = false;
		
		targetRot.y = Mathf.SmoothDampAngle(transform.eulerAngles.y, player.eulerAngles.y, ref climbSpeedSmoothVel, _damping * Time.deltaTime);
		transform.eulerAngles = targetRot;
			
	}
	
	private void OnEnable() 
	{ 
//		EventManager.onCharEvent += AttachFollow;
//		EventManager.onInputEvent += AttachFollow; 
		EventManager.onInputEvent += Test;
	}

	private void OnDisable() 
	{ 
//		EventManager.onCharEvent -= AttachFollow;
//		EventManager.onInputEvent -= AttachFollow; 
		EventManager.onInputEvent -= Test;
	}

	private void Test (GameEvent e)
	{
		//if (e == GameEvent.Jump)
			print("follow: Test jump");
	}

	private void AttachFollow (GameEvent gEvent)
	{
//		if (gEvent == GameEvent.AttachFollow || gEvent == GameEvent.StartClimbing || gEvent == GameEvent.StopClimbing || gEvent == GameEvent.Land)
//		{
//			Attach = true;
//			//print(gEvent);
//		}
//		else if (gEvent == GameEvent.DetachFollow )
//		{
//			Attach = false;
//		}
	}

}
