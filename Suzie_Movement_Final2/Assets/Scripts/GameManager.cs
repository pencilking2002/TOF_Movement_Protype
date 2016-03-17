using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode
	public TestCam cam;

	// debug
	[HideInInspector]
	public RomanCharState charState;
	//private RomanCameraController camScript;
	private ClimbDetector climbDetector;
	private FollowPlayer follow;
	private Transform currentChar;
	private VineClimbController2 vineClimbCollider;
	private RomanCharController charController;

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
		
		charState = GameObject.FindObjectOfType<RomanCharState> ();
		//camScript = GameObject.FindObjectOfType<RomanCameraController> ();
		climbDetector = GameObject.FindObjectOfType<ClimbDetector> ();
		follow = GameObject.FindObjectOfType<FollowPlayer>();
		vineClimbCollider = GameObject.FindObjectOfType<VineClimbController2>();
		charController = GameObject.FindObjectOfType<RomanCharController>();
		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TestCam>();

		// Get the character that is selected
		currentChar = charController.transform;
	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, yPos[0], 170, 30), "Squirrel State: " + charState.GetState());
			GUI.Button(new Rect(Screen.width - 150, yPos[1], 170, 30), "climb collider detected " + climbDetector.climbColliderDetected);
			GUI.Button(new Rect(Screen.width - 150, yPos[2], 170, 30), "Detached: " + vineClimbCollider.detached);
			GUI.Button(new Rect(Screen.width - 150, yPos[3], 170, 30), "In Tube: " + charController.inTube);
			GUI.Button(new Rect(Screen.width - 150, yPos[4], 170, 30), "H: " + InputController.h);
			GUI.Button(new Rect(Screen.width - 150, yPos[5], 170, 30), "CamState: " + cam.state);
			GUI.Button(new Rect(Screen.width - 150, yPos[6], 170, 30), "Cam colliding: " + cam.colliding);
			GUI.Button(new Rect(Screen.width - 150, yPos[7], 170, 30), "At player pos: " + follow.followAtPlayerPos);
			GUI.Button(new Rect(Screen.width - 150, yPos[8], 170, 30), "Jump released: " + InputController.jumpReleased);
			//GUI.Button(new Rect(Screen.width - 150, yPos[9], 170, 30), "PMJ colliding " + pmj.colliding);


			if (GUI.Button(new Rect(0, yPos[0], 170, 30), "Spawn at Cliff "))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("CliffSpawnSpot").transform.position;
			
			if (GUI.Button(new Rect(0, yPos[1], 170, 30), "Quit"))
				Application.Quit();

			if (GUI.Button(new Rect(0, yPos[2], 170, 30), "Restart"))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			
			if (GUI.Button(new Rect(0, yPos[3], 170, 30), "Sprint"))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("SprintSpawnSpot").transform.position;
			
			if (GUI.Button(new Rect(0, yPos[4], 170, 30), "Edge Climb"))
			{
				currentChar.position = GameObject.FindGameObjectWithTag("EdgeClimbSpot").transform.position;
			}
		}
	}
	
	#endif
	
}
