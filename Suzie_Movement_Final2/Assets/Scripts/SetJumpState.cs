using UnityEngine;
using System.Collections;

/// <summary>
/// This script is used for keeping track of weather the character is doing any kind of jumping
/// </summary>
public class SetJumpState : StateMachineBehaviour 
{
	public bool onExit;
	public bool onEnter;

	public bool isJumping;

	private RomanCharState _charState;
	private RomanCharState charState
	{
		get 
		{
			if (_charState == null)
				_charState = GameManager.Instance.charState;

			return _charState;
		}
	}

	// OnStateEnter is called before OnStateEnter is called on any state inside this state machine
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{	
		if (onEnter)
			charState.SetIsJumping(isJumping);
	}

//	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (onExit)
			charState.SetIsJumping(isJumping);
	}
}
