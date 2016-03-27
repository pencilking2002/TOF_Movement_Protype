using UnityEngine;
using System.Collections;
using InControl;

public class InputController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	public static InputController Instance;
	public static float v, h, orbitV, orbitH;

	public static float rawH
	{
		get { return _rawH; }
		set
		{
			_rawH = Mathf.RoundToInt(value);
		}
	}
	public static float rawV
	{
		get { return _rawV; }
		set
		{
			_rawV = Mathf.RoundToInt(value);
		}
	}

	private static int _rawH;
	private static int _rawV;

	public static bool jumpReleased = false;
	private RomanCharState charState;

	[HideInInspector]
	public static bool leftStickPressed = false;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Priate Variables
	//---------------------------------------------------------------------------------------------------------------------------
	[HideInInspector]
	public InputDevice inputDevice;
	//private static float _rawV, _rawH;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
		//inputDevice = InputManager.ActiveDevice;
		charState = GameObject.FindObjectOfType<RomanCharState>();
	}
	
	private void Update ()
	{
		inputDevice = InputManager.ActiveDevice;
		
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");
//		h = inputDevice.LeftStickX.Value;
//		v = inputDevice.LeftStickY.Value;

		// Check if left stick is down
		leftStickPressed = inputDevice.LeftStick.IsPressed ? true : false;
		
		rawH = inputDevice.LeftStickX.RawValue;
		rawV = inputDevice.LeftStickY.RawValue;
		
		orbitH = inputDevice.RightStickX;
		orbitV = inputDevice.RightStickY;

		//----------------------------------------------------------------------------------------------------------------------
		// Player Movement
		//----------------------------------------------------------------------------------------------------------------------

		// Sprinting ------------------------------------------------
		if (inputDevice.LeftTrigger.WasPressed)
		{
			EventManager.OnInputEvent(GameEvent.SprintModifierDown);
			print("Shift down");
		}
		else if (inputDevice.LeftTrigger.WasReleased)
		{
			EventManager.OnInputEvent(GameEvent.SprintModifierUp);
		}

		if (charState.IsClimbing() && inputDevice.LeftStickY.WasPressed)
		{
			print ("edge climbing raw v: " + v);
			
//			if (rawV == 1 || v > 0)
//			{
//				EventManager.OnInputEvent(GameEvent.ClimbOverEdge);
//			}
//			// TODO - doesnt work with controller
//			else if (rawV == -1 || v < 0)
//			{
//	//			print ("InputController: stop climbing " + rawV);
//				EventManager.OnInputEvent(GameEvent.StopEdgeClimbing);
//				EventManager.OnInputEvent(GameEvent.StopWallClimbing);
//				print("InputController: Stop climbing");
//
//			}
//			print ("Event sent: climboveredge");
		}
		
		if (charState.IsClimbing() && inputDevice.Action1.WasPressed/*rawV == -1 || v < 0*/)
		{
			EventManager.OnInputEvent(GameEvent.StopClimbing);
			//print("stop climbing");		
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Jumping
		//----------------------------------------------------------------------------------------------------------------------
		
		// if pressed Y or pressed Space
		if (inputDevice.Action1.WasPressed)
		{
			EventManager.OnInputEvent(GameEvent.Jump);
			//print("InputController: Jump pressed");
		}

		// If the player let go of jump button and the character is still jumping
		else if (inputDevice.Action1.WasReleased && charState.IsInAnyJumpingState())
			jumpReleased = true;


		//----------------------------------------------------------------------------------------------------------------------
		// Recenter Camera
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightBumper.WasReleased)
			EventManager.OnInputEvent(GameEvent.RecenterCam);	
		
		
		//----------------------------------------------------------------------------------------------------------------------
		// Camera Orbiting
		//----------------------------------------------------------------------------------------------------------------------
		
//		if (inputDevice.RightStickX.IsPressed)
//			EventManager.OnInputEvent(GameEvent.OrbitCamera);
//		
//		else if (inputDevice.RightStickX.WasReleased) 
//			EventManager.OnInputEvent(GameEvent.CamBehind);
		


	}
		
	
}
