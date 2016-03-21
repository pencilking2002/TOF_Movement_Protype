using UnityEngine;
using System.Collections;

public class ItemScale : MonoBehaviour {

	private Vector3 initialScale;

	// Use this for initialization
	void Start () 
	{
		initialScale = transform.localScale;
		transform.localScale = Vector3.zero;
	}

	private void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			LeanTween.scale(gameObject, initialScale, 0.3f)
				.setEase(LeanTweenType.easeInBack);
		}
	}
}
