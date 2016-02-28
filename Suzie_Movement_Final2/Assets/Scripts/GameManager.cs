using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode
	public Text debugText;
	// debug
	[HideInInspector]
	public RomanCharState charState;
	//private RomanCameraController camScript;
	private ClimbDetector climbDetector;
	private FollowPlayer follow;
	private VineClimbController2 vineClimbCollider;
	private RomanCharController charController;
	private TestCam cam;

	private void Start ()
	{
//		EventManager.onCharEvent = null;
//		EventManager.onInputEvent = null;
//		EventManager.onDetectEvent = null;
		
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
		cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TestCam>();
	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, 30, 170, 30), "Squirrel State: " + charState.GetState());
			GUI.Button(new Rect(Screen.width - 150, 60, 170, 30), "climb collider detected " + climbDetector.climbColliderDetected);
			
			if (GUI.Button(new Rect(Screen.width - 150, 90, 170, 30), "Spawn at Cliff "))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("CliffSpawnSpot").transform.position;
			

			//GUI.Button(new Rect(Screen.width - 150, 120, 170, 30), "CamState: " + camScript.state);

			if (GUI.Button(new Rect(Screen.width - 150, 120, 170, 30), "Quit"))
				Application.Quit();
			

			if (GUI.Button(new Rect(Screen.width - 150, 150, 170, 30), "Restart"))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			
			
			GUI.Button(new Rect(Screen.width - 150, 180, 170, 30), "At player pos: " + follow.followAtPlayerPos);
			
			if (GUI.Button(new Rect(Screen.width - 150, 210, 170, 30), "Sprint"))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("SprintSpawnSpot").transform.position;
			

			if (GUI.Button(new Rect(Screen.width - 150, 240, 170, 30), "Climb"))
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("StartClimbSpot").transform.position;
			

			GUI.Button(new Rect(Screen.width - 150, 270, 170, 30), "Detached: " + vineClimbCollider.detached);
			GUI.Button(new Rect(Screen.width - 150, 300, 170, 30), "In Tube: " + charController.inTube);

			GUI.Button(new Rect(Screen.width - 150, 330, 170, 30), "raw H: " + InputController.rawH);
			//GUI.Button(new Rect(Screen.width - 150, 360, 170, 30), "left stick pressed: " + InputController.leftStickPressed);
			GUI.Button(new Rect(Screen.width - 150, 390, 170, 30), "CamState: " + cam.state);
			GUI.Button(new Rect(Screen.width - 150, 420, 170, 30), "Cam colliding: " + cam.colliding);


		}
	}
	
	#endif
	
}
