using UnityEngine;
using System.Collections;

public class CombatState : StateMachineBehaviour {

	CombatController combatController = null;
	public enum CombatChoice
	{
		Enter,
		Exit,
		Both
	}

	public bool startCombat = true;

	public CombatChoice choice = CombatChoice.Enter;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (choice == CombatChoice.Enter || choice == CombatChoice.Both) 
		{
			if (combatController == null)
				combatController = animator.GetComponent<CombatController> ();

			if (startCombat)
				combatController.state = CombatController.SquirrelCombatState.Punching;
			else
				combatController.state = CombatController.SquirrelCombatState.None;
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if (choice == CombatChoice.Exit || choice == CombatChoice.Both) 
		{
			if (combatController == null)
				combatController = animator.GetComponent<CombatController> ();

			if (!startCombat)
				combatController.state = CombatController.SquirrelCombatState.None;
			else
				combatController.state = CombatController.SquirrelCombatState.Punching;
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
