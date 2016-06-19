using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour {

	private GameObject combatCollider;

	private Animator animator;

	private int anim_Punch = Animator.StringToHash ("Punch");
	private int anim_ExitPunch = Animator.StringToHash ("ExitPunch");
	//private bool exitPunch = true;
	// Use this for initialization

	public enum SquirrelCombatState
	{
		None,
		Punching
	}

	[HideInInspector]
	public SquirrelCombatState state = SquirrelCombatState.None;

	void Awake () 
	{
		animator = GetComponent<Animator> ();
		combatCollider = GetComponentInChildren<CombatCollider> ().gameObject;
		combatCollider.SetActive (false);
	}

	void Start()
	{
		
	}

	// Update is called once per frame
	void Update () 
	{
			
	}

	void OnEnable()
	{
		EventManager.onInputEvent += Punch;
	}
	void OnDisable()
	{
		EventManager.onInputEvent -= Punch;
	}


	void Punch(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Punch) 
		{
			//print ("Punch!");
			animator.SetTrigger (anim_Punch);
		}
	}

	public void EnableCombatCollider(int enable)
	{
		combatCollider.SetActive (enable == 1);
		//print ("exit punch");
	}

}
