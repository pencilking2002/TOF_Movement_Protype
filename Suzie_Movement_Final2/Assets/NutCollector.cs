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

	public Vector3 scaleUpVector = new Vector3(1.2f, 1.4f, 1.2f); 
	private Vector3 moveVel;

	private void OnTriggerStay (Collider col)
	{
		if (col.gameObject.layer == 15)
		{
			GameObject pickup = col.gameObject;

			print("nut triggered");
			// Switch pickup to a layer that ignores all player collisions
			pickup.layer = 16;
			float yOffset = 1.3f;

			LeanTween.scale(pickup, scaleUpVector, scaleUpTime)
			.setOnComplete(() => {
				LeanTween.scale(pickup, Vector3.zero, scaleTime)
					.setOnUpdate((float pos) => {
							Vector3 targetPos = pickup.transform.position + Vector3.up; //new Vector3(transform.position.x, pickup.transform.position.y + yOffset, transform.position.z);
							pickup.transform.position = Vector3.SmoothDamp(pickup.transform.position, targetPos, ref moveVel, moveDamp);	
				})
				.setOnComplete(() => {
					pickup.SetActive(false);
				});
			});
		}
	}
}
