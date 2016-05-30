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
		RunningDoubleJumping,
		IdleDoubleJumping,
		WallBouncing
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private State state = State.IdleFalling;
	public static bool landedFirstTime = false;
	private bool isJumping = false;


	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	

	private void Start() {}

	public void SetState (State _state)
	{
		TriggerEvents(_state);
		state = _state;
	}
	
	public State GetState ()
	{
		return state;
	}
	
	public bool Is (State _state)
	{
		return state == _state;
	}
	
	/// <summary>
	/// This method is called from a StateMachineBehaviour that's attached to the jumping substate machine
	/// We use this so that we don't have to check for numerous amount of jumping states
	/// </summary>
	public void SetIsJumping (bool _isJumping)
	{
		this.isJumping = _isJumping;
	}

	/*------------------------------------------------------------------|
	// 	IDLE STATES											            |
	//-----------------------------------------------------------------*/

	public bool IsIdle ()
	{
		return state == State.Idle;
	}

	public bool IsIdleOrRunning()
	{
		return  state == State.Idle || state == State.Running;
	}

	/*------------------------------------------------------------------|
	// 	RUNNING STATES											        |
	//-----------------------------------------------------------------*/

	public bool IsRunning ()
	{
		return state == State.Running;
	}

	public bool IsSprinting ()
	{
		return state == State.Sprinting;
	}

	public bool IsRunningOrSprinting ()
	{
		return state == State.Running || state == State.Sprinting;
	}

	/// <summary>
	/// Determines whether the character is Idle or running or sprinting
	/// </summary>
	/// <returns><c>true</c> if this instance is idle or moving; otherwise, <c>false</c>.</returns>
	public bool IsIdleOrMoving()
	{
		return state == State.Running || state == State.Idle || state == State.Sprinting;
	}

	/*------------------------------------------------------------------|
	// 	JUMPING STATES											        |
	//-----------------------------------------------------------------*/

	public bool IsInAnyJumpingState()
	{
		return isJumping;
	}

	public bool IsIdleJumping()
	{
		return state == State.IdleJumping || state == State.IdleFalling;
	}

	public bool IsRunningJumping()
	{
		return state == State.RunningJumping || state == State.RunningFalling;
	}

	public bool IsIdleOrRunningJumping()
	{
		return IsIdleJumping() || IsRunningJumping();
	}

	public bool IsSprintJumping ()
	{
		return state == State.SprintJumping || state == State.SprintFalling;
	}

	/*------------------------------------------------------------------|
	| 	FALLING AND LANDING STATES									    |
	|------------------------------------------------------------------*/

	//TODO - IS RUNNING LANDING
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

	public bool IsSprintLanding()
	{
		return state == State.SprintLanding;
	}

	/*------------------------------------------------------------------|
	| 	DOUBLE JUMPING STATES											|
	|------------------------------------------------------------------*/
	
	public bool IsIdleDoubleJumping()
	{
		return state == State.IdleDoubleJumping;
	}

	public bool IsRunningDoubleJumping()
	{
		return state == State.RunningDoubleJumping;
	}

	public bool IsAnyDoubleJumping()
	{
		return state == State.RunningDoubleJumping || state == State.IdleDoubleJumping;
	}

	/*------------------------------------------------------------------|
	| 	CLIMBING STATES									                |
	|------------------------------------------------------------------*/

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
	
	/*------------------------------------------------------------------|
	| 	PRIVATE METHODS          						                |
	|------------------------------------------------------------------*/

	/// <summary>
	/// Triggers events from the EventManager when a state gets set
	/// </summary>
	/// <param name="_state">State.</param>
	private void TriggerEvents(State _state)
	{
		if (state == State.Idle) 
		{
			EventManager.OnCharEvent(GameEvent.ExitIdle);
		}

		// If previous state is sprinting, fire StopSprinting event
		else if (IsSprinting() && _state != State.SprintJumping)
		{
			EventManager.OnCharEvent(GameEvent.StopSprinting);
		}

		else if (_state == State.Idle)
		{
			if (!landedFirstTime)
			{
				landedFirstTime = true;
				EventManager.OnCharEvent(GameEvent.LandedFirstTime);
			}
				
			EventManager.OnCharEvent(GameEvent.IsIdle);
			
		}
		else if (_state == State.Sprinting)
		{
			EventManager.OnCharEvent(GameEvent.StartSprinting);
		}
		else if (_state == State.Running)
		{
			EventManager.OnCharEvent(GameEvent.StartRunning);
		}
	}
	
	
}
