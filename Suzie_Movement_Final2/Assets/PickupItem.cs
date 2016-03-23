using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

	[Range(0, 10)]
	public float scaleSpeed = 0.6f;
	[Range(0, 10)]
	public float moveDamp = 0.5f;

	private Vector3 initialScale;
	private bool isPickedUp;
	private GameObject parentGO;
	private Transform player;
	private Vector3 targetPos;
	private Vector3 moveVel;

	// Use this for initialization
	void Start () 
	{
		initialScale = transform.localScale;
		parentGO = transform.parent.gameObject;
		//transform.localScale = Vector3.zero;
	}

	private void OnTriggerStay (Collider col)
	{
		if (col.gameObject.layer == 8 && !isPickedUp)
		{
			player = col.transform;
			//targetPos = player.position;

			isPickedUp = true;
			LeanTween.scale(parentGO, Vector3.zero, scaleSpeed)
				.setEase(LeanTweenType.easeInBack)
				.setOnUpdate((float pos) => {
					parentGO.transform.position = Vector3.SmoothDamp(parentGO.transform.position, player.position, ref moveVel, moveDamp);	
			})
			.setOnComplete(() => {
				parentGO.SetActive(false);
			});
		}
	}
}
