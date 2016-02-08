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
	ClimbColliderDetected,
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
	FinishVineClimbing
	
}

public class EventManager : MonoBehaviour 
{
	//---------------------------------------------------------------------------------------------------------------------------
	// Character events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void CharEvent(GameEvent gameEvent);
	public static CharEvent onCharEvent;
	
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
	
	public static void OnInputEvent (GameEvent gameEvent)
	{
		if (onInputEvent != null)
			onInputEvent(gameEvent);
	}
	
	public static void OnDetectEvent (GameEvent gameEvent, RaycastHit hit)
	{
		if (onDetectEvent != null)
			onDetectEvent(gameEvent, hit);
	}
}
