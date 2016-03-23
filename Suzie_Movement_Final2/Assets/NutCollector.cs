using UnityEngine;
using System.Collections;

public class NutCollector : MonoBehaviour 
{
	[Range(0,10)]
	public float scaleTime = 2.0f;

	[Range(0,1)]
	public float scaleUpTime = 0.1f;

	[Range(0,10)]
	public float moveDamp = 2.0f;

	public Vector3 scaleUpVector = new Vector3(1.4f, 1.4f, 1.4f); 
	private Vector3 moveVel;

	private void OnTriggerStay (Collider col)
	{
		if (col.gameObject.layer == 15)
		{
			GameObject pickup = col.gameObject;

			print("nut triggered");
			// Switch pickup to a layer that ignores all player collisions
			pickup.layer = 16;

			LeanTween.scale(pickup, scaleUpVector,scaleUpTime)
			.setOnComplete(() => {
				LeanTween.scale(pickup, Vector3.zero, scaleTime)
					.setEase(LeanTweenType.easeInBack)
					.setOnUpdate((float pos) => {
						pickup.transform.position = Vector3.SmoothDamp(pickup.transform.position, transform.position, ref moveVel, moveDamp);	
				})
				.setOnComplete(() => {
					pickup.SetActive(false);
				});
			});
		}
	}
}
