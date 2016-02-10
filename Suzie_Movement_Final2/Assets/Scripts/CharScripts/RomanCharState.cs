using UnityEngine;
using System.Collections;

public class RomanCharState : MonoBehaviour {
	
	//---------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------	
	
	public enum State
	{
		Idle,
		Landing,
		IdleJumping,
		RunningJumping,
		WallClimbing,
		Swimming,
		IdleFalling,
		RunningFalling,
		Running,
		InCombat,
		//InAir,
		Pivoting,
		Sprinting,
		ClimbingOverEdge,
		EdgeClimbing,
		SprintJumping,
		SprintFalling,
		SprintLanding,
		VineClimbing,
		VineAttaching,
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private State state = State.IdleFalling;
	private static bool landedFirstTime = false;
	
//	private Rigidbody rb;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private void Awake ()
	{
//		rb = GetComponent<Rigidbody>();
	}

	public void SetState (State _state)
	{
	
		// If previous state is sprinting, fire StopSprinting event
		if (IsSprinting())
		{
			EventManager.OnCharEvent(GameEvent.StopSprinting);
			print("Stop sprinting");
		}

		else if (_state == State.Idle)
		{
			if (!landedFirstTime)
				landedFirstTime = true;
				
			EventManager.OnCharEvent(GameEvent.IsIdle);
			// TODO - Move to another class
//			rb.collisionDetectionMode = CollisionDetectionMode.Discrete; 
			
		}
		else if (_state == State.Sprinting)
		{
			EventManager.OnCharEvent(GameEvent.StartSprinting);
			print ("Start sprinting");
		}
		else if (_state == State.Running)
		{
			EventManager.OnCharEvent(GameEvent.StartRunning);
			//print ("start running");
		}
		
//		if (_state != State.Idle)
//			rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 	// TODO - Move to another class
			
		state = _state;
		//print("set state: " + _state);
	}
	
	public State GetState ()
	{
		return state;
	}
	
	public bool Is (State _state)
	{
		return state == _state;
	}
	
	
	// Idle ----------------------------------------------------
	
	public bool IsIdle ()
	{
		return state == State.Idle;
	}
	
	public bool IsIdleTurning ()
	{
		return state == State.Idle && InputController.rawH != 0;
	}
	
	
	// Running ----------------------------------------------------
	
	public bool IsJogging ()
	{
		return state == State.Running;
	}
	
	public bool IsSprinting ()
	{
		return state == State.Sprinting;
	}
	
	public bool IsRunning ()
	{
		return state == State.Running || state == State.Sprinting;
	}

	public bool IsIdleOrRunning()
	{
		return  state == State.Idle || state == State.Running || state == State.Sprinting;
	}

	// Jumping ----------------------------------------------------
	
	public bool IsJumping()
	{
		return (state == State.IdleJumping || 
				state == State.RunningJumping || 
				state == State.IdleFalling || 
				state == State.RunningFalling ||
				
				state == State.SprintJumping ||
		        state == State.SprintFalling ||
		        state == State.SprintLanding) 
		        && landedFirstTime;
	}
	
	
	public bool IsIdleJumping()
	{
		return (state == State.IdleJumping || state == State.IdleFalling);
	}

	public bool IsRunningJumping()
	{
		return (state == State.RunningJumping || state == State.RunningFalling);
	}

	public bool IsLanding()
	{
		return state == State.Landing || state == State.SprintLanding;
	}

	public bool IsFalling()
	{
		return state == State.IdleFalling || state == State.RunningFalling || state == State.SprintFalling;
	}

	public bool IsSprintFalling()
	{
		return state == State.SprintFalling;
	}


	public bool IsSprintJumping ()
	{
		return state == State.SprintJumping || state == State.SprintFalling;
	}
	
	// Climbing ----------------------------------------------------

	public bool IsClimbing ()
	{
		return state == State.WallClimbing || state == State.EdgeClimbing || state == State.VineClimbing;
	}

	public bool IsWallClimbing()
	{
		return state == State.WallClimbing;
	}
	public bool IsEdgeClimbing()
	{
		return state == State.EdgeClimbing;
	}

	public bool IsClimbingOverEdge()
	{
		return state == State.ClimbingOverEdge;
	}
	
	public bool IsVineClimbing()
	{
		return state == State.VineClimbing || state == State.VineAttaching;
	}

	public bool IsVineAttaching()
	{
		return state == State.VineAttaching;
	}
	
	// Events ------------------------------------------------------
	
//	private void OnEnable () { EventManager.onCharEvent += Event_SetState; }
//	private void OnDisable () { EventManager.onCharEvent -= Event_SetState; }
//	
	/// <summary>
	/// Set a state through an event
	/// </summary>
	/// <param name="gameEvent">Game event.</param>
//	private void Event_SetState (GameEvent gameEvent)
//	{
//		if (gameEvent == GameEvent.StartEdgeClimbing)
//		{
//			SetState(State.EdgeClimbing);
//		}
//	}
	

	
	
}
