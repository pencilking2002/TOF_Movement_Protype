using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DentedPixel.LTEditor;

public class ClimbOverEdge : MonoBehaviour {
	
	public float yClimbOverSpeed = 0.15f;
	public float zClimbOverSpeed = 0.15f;

	public bool noClimbOver = false;

	public LeanTweenVisual edgeTween;
	public LeanTweenVisual vineTween;

	public LeanTweenPath edgePath;
	public LeanTweenPath vinePath;
	
	private RomanCharState charState;
	private Animator animator;
	private float tweenDuration;

	private Transform vineMeshBounds;

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();

		// Calculate the climbing tween's duration
		tweenDuration = edgeTween.groupList[0].endTime - edgeTween.groupList[0].startTime;
		
		//print (tweenDuration);
		if (edgeTween == null)
			Debug.LogError("tween not defined");
		
		if (edgePath == null || vinePath == null)
			Debug.LogError("path not defined");
//
//		// Disable the tween and path at beginning
		EnableClimbOverTween(edgePath, edgeTween, false);
		EnableClimbOverTween(vinePath, vineTween, false);
	}

	private void Start ()
	{
		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.StartEdgeClimbing, true },
			{ GameEvent.StartVineClimbing, true },

			{ GameEvent.StopEdgeClimbing, false },
			{ GameEvent.StopVineClimbing, false },
			{ GameEvent.Land, false }

		});
	}
	
	private void Update () 
	{
		if (charState.IsVineClimbing() && vineMeshBounds != null)
		{
//			//Vector3 bottomVinePos = vineMeshBounds.center.y + vineMeshBounds.extents.y;
//			if (transform.position.y < bottomVinePos.y)
//			{
//				print("Squirrel is beneath the vine");
//			}
		}
	}
	
	private void OnEnable()
	{
		EventManager.onInputEvent += ClimbOverEdgeMove;
		EventManager.onCharEvent += ClimbOverEdgeMove;
	}
	
	private void OnDisable()
	{
		EventManager.onInputEvent -= ClimbOverEdgeMove;
		EventManager.onCharEvent -= ClimbOverEdgeMove;
	}
	
	private void ClimbOverEdgeMove(GameEvent gEvent)
	{
		if(charState.IsClimbing() && gEvent == GameEvent.ClimbOverEdge && !noClimbOver)
		{
			LeanTweenPath path = new LeanTweenPath();
			LeanTweenVisual tween = new LeanTweenVisual();
//
			if (charState.IsVineClimbing())
			{
				tween = vineTween;
				path = vinePath;
				print("ClimbOverVine: should start climbing over vine");
			}
			else if(charState.IsEdgeClimbing())
			{
				tween = edgeTween;
				path = edgePath;
			}

			EnableClimbOverTween(path, tween, true);

			path.transform.parent = null;
			animator.SetTrigger("ClimbOverEdge");

			RSUtil.Instance.DelayedAction(() => {
				path.transform.parent = transform;
				path.transform.localPosition = Vector3.zero;
				EnableClimbOverTween(path, tween, false);
				EventManager.OnCharEvent(GameEvent.FinishClimbOver);
				noClimbOver = false;
			}, tweenDuration);

		}
	}

	private void EnableClimbOverTween(LeanTweenPath _path, LeanTweenVisual _tween, bool enable)
	{
		// Enable climbing tween and path
		_tween.enabled = enable;
		_path.gameObject.SetActive(enable);
	}

	// Set the vine so we can measure the 
	// character's position against it
	public void SetVineMeshBounds(Transform _vine)
	{
		//vineMeshBounds = _vine.GetComponent<MeshFilter>().mesh.bounds;
	}

}
