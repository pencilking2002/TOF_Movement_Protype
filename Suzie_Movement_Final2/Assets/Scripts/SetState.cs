using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SetState : StateMachineBehaviour {
	
	public RomanCharState.State characterState;	// Which state to switch into on enter
	private RomanCharState charState;
	 
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (charState == null)
			charState = GameManager.Instance.charState;

		charState.SetState(characterState);
			
	}
	
}