using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SprintHint : MonoBehaviour {

	public Image sprintHintUI;
//	public float timeBuffer = 1.0f;
//	public bool entered = false;
//	public enum HintState {
//		Hidden,
//		Fading,
//		Displayed
//	};
//
//	private RaycastHit hit;
//
//	public HintState state = HintState.Displayed;
//	private bool nearCollider = false;

	public Transform hintTransform;
	private float solidDistance = 7.0f;
	private float dist;
	private float distanceStart = 7.0f;
	private float currentAlpha;
	private float alpha = 0.0f;

	void Update ()
	{
		dist = Vector3.Distance (transform.position, hintTransform.position);

		currentAlpha = sprintHintUI.color.a;

		if (dist < distanceStart) 
		{
			if (dist < solidDistance)
				dist = solidDistance;
			
			alpha = Mathf.Lerp (currentAlpha, solidDistance / dist, 4.0f * Time.deltaTime);
			sprintHintUI.color = new Color (1, 1, 1, alpha);
		}
		else 
		{

			alpha = Mathf.Lerp (currentAlpha, 0.0f, 4.0f * Time.deltaTime);
			sprintHintUI.color = new Color (1, 1, 1, alpha);
			
		}
	}
			
}
