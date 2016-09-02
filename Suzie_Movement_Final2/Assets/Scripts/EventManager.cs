using UnityEngine;
using System.Collections;

public enum GameEvent
{
	// Player follow object attaching and detaching
	AttachFollow,
	DetachFollow,
	
	//Input ------------
	Jump,
	RecenterCam,
	CamBehind,
	OrbitCamera,
	faceOppositeDirection,

	// Running ------------
	StopRunning,
	StartRunning,
	StopTurnRunning,
	StartTurnRunning,

	// Sprinting ------------
	SprintModifierDown,
	SprintModifierUp,
	StartSprinting,
	StopSprinting,

	// Climbing ------------
	StartEdgeClimbing,
	StopEdgeClimbing,
	StartClimbingOverEdge,
	StartWallClimbing,
	ClimbOverEdge,
	
	Land,
	ResetJumpOff,
	EdgeClimbColliderDetected,
	StopWallClimbing,
	StopClimbing,
	StartClimbing,
	IsIdle,
	ResetCam,
	FinishClimbOver,
	StartVineClimbing,
	ExitIdle,
	StopVineClimbing,
	WallCollision,
	FinishVineClimbing,
	LandedFirstTime,
	CamZoomedIn,
	WallClimbColliderDetected,
	EnterIdle,
	EnterTunnel,
	ExitTunnel,
	WallBounce,
	BounceOffWall,
	Punch,
	WallHit,
	CollectNut,
	Footstep,
	CollderLand
	
}

public class EventManager : MonoBehaviour 
{
	//---------------------------------------------------------------------------------------------------------------------------
	// Character events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void CharEvent(GameEvent gameEvent);
	public static CharEvent onCharEvent;

	// For excepting a vec3 as an arg
	public delegate void CharEvent2(GameEvent gameEvent, Transform t);
	public static CharEvent2 onCharEvent2;

	//---------------------------------------------------------------------------------------------------------------------------
	// Input events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void InputAction(GameEvent gameEvent);
	public static InputAction onInputEvent;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Climb Detection events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void DetectAction(GameEvent gameEvent, RaycastHit hit);
	public static DetectAction onDetectEvent;

	public static void OnCharEvent (GameEvent gameEvent)
	{
		if (onCharEvent != null)
			onCharEvent(gameEvent);
	}

	// Overload of OnCharEvent that can handle a vector3 a sa 2nd arg 
	public static void OnCharEvent (GameEvent gameEvent, Transform t)
	{
		if (onCharEvent2 != null)
			onCharEvent2(gameEvent, t);
	}
	
	public static void OnInputEvent (GameEvent gameEvent)
	{
		if (onInputEvent != null)
			onInputEvent(gameEvent);
		else
			print("onInputEvent is null");
	}
	
	public static void OnDetectEvent (GameEvent gameEvent, RaycastHit hit)
	{
		if (onDetectEvent != null)
			onDetectEvent(gameEvent, hit);
	}
}
