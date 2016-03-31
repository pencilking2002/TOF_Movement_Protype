using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode
	//public TestCam cam;

	public PhatRobit.SimpleRpgCamera simpleRpgCam;

	// debug
	[HideInInspector]
	public RomanCharState charState;
	//private RomanCameraController camScript;
	private ClimbDetector climbDetector;
	private FollowPlayer follow;
	private Transform currentChar;
	private VineClimbController2 vineClimbCollider;
	private RomanCharController charController;
	public TunnelObserver tunnelObserver;
	public AntiWallSlideController antiWallSlideController;

	private int[] yPos = new int[]
	{
		0, 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360, 390, 420, 450, 480, 520, 550
	};

	private void Start ()
	{
		
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);

//		if (cam == null)
//			cam = Camera.main.GetComponent<TestCam>();
		simpleRpgCam = Camera.main.GetComponent<PhatRobit.SimpleRpgCamera>();

		charState = GameObject.FindObjectOfType<RomanCharState> ();
		//camScript = GameObject.FindObjectOfType<RomanCameraController> ();
		climbDetector = GameObject.FindObjectOfType<ClimbDetector> ();
		follow = GameObject.FindObjectOfType<FollowPlayer>();
		vineClimbCollider = GameObject.FindObjectOfType<VineClimbController2>();
		charController = GameObject.FindObjectOfType<RomanCharController>();
		tunnelObserver = GameObject.FindObjectOfType<TunnelObserver>();
		antiWallSlideController = GameObject.FindObjectOfType<AntiWallSlideController>();

		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TestCam>();

		// Get the character that is selected
		currentChar = charController.transform;
	}

	//#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (GUI.Button(new Rect(0, yPos[4] + 50, 170, 30), "Wall Climb"))
		{
			currentChar.position = GameObject.FindGameObjectWithTag("WallClimbSpot").transform.position;
		}
		if (GUI.Button(new Rect(0, yPos[2] + 50, 170, 30), "Restart"))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, yPos[0], 170, 30), "Squirrel State: " + charState.GetState());
			GUI.Button(new Rect(Screen.width - 150, yPos[1], 170, 30), "climb collider detected " + climbDetector.climbColliderDetected);
			GUI.Button(new Rect(Screen.width - 150, yPos[2], 170, 30), "Detached: " + vineClimbCollider.detached);
			GUI.Button(new Rect(Screen.width - 150, yPos[3], 170, 30), "In Tube: " + tunnelObserver.inTunnel);
			GUI.Button(new Rect(Screen.width - 150, yPos[4], 170, 30), "rawH: " + InputController.rawH);
			GUI.Button(new Rect(Screen.width - 150, yPos[5], 170, 30), "CamPlayer pos: " + simpleRpgCam.GetSignedDirection());
			GUI.Button(new Rect(Screen.width - 150, yPos[6], 170, 30), "Cam auto rot: " + simpleRpgCam.autoRotate);
			GUI.Button(new Rect(Screen.width - 150, yPos[7], 170, 30), "At player pos: " + follow.followAtPlayerPos);
			GUI.Button(new Rect(Screen.width - 150, yPos[8], 170, 30), "Jump released: " + InputController.jumpReleased);
			GUI.Button(new Rect(Screen.width - 150, yPos[9], 170, 30), "collidng with wall " + antiWallSlideController.colliding);


			if (GUI.Button(new Rect(0, yPos[0], 170, 30), "Spawn at Cliff "))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("CliffSpawnSpot").transform.position;
			
			if (GUI.Button(new Rect(0, yPos[1], 170, 30), "Quit"))
				Application.Quit();

	
			
			if (GUI.Button(new Rect(0, yPos[3], 170, 30), "Sprint"))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("SprintSpawnSpot").transform.position;
			

		}
	}
	
	//#endif
	
}
