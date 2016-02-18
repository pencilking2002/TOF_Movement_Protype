using UnityEngine;
using System.Collections;

/// <summary>
/// This script is used for keeping track of weather the character is doing any kind of jumping
/// </summary>
public class SetJumpStateOnExit : StateMachineBehaviour 
{
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
//	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
//	{	
//		//Debug.Log("runs");
//		charState.SetIsJumping(isJumpingOnEnter);
//	}

//	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		charState.SetIsJumping(isJumping);
	}
}
