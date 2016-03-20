using UnityEngine;
using System.Collections;

public class BoxColliderGenerator : MonoBehaviour {

	// Multiplier to control how much inside the this box collider will be offset
	// from its parent
	public Vector3 insideOffsetMultiplier = new Vector3(0.5f, 0.5f, 0.5f);

	// Amount to rotate the box collider to match the orientation fo the parent
	// if the parent's rotation isn't axis aligned
	public Vector3 rotationOffset = new Vector3(0,0,30.0f);

	private BoxCollider boxCollider;
	private MeshCollider meshCollider;
	private Vector3 size;

	private void Awake ()
	{
		if (transform.parent == null)
		{
			Debug.LogError("BoxColliderGenerator ERROR: this GO is not parented");
		}
		else
		{
			MeshCollider meshCollider = GetComponentInParent<MeshCollider>();
			boxCollider = GetComponent<BoxCollider>();

			if (meshCollider == null || boxCollider == null)
			{
				Debug.Log("BoxColliderGenerator ERROR: mesh or box collider not defined");
			}
			else
			{
				
			}
	//		meshCollider = GetComponent<MeshCollider>();
	//		logCollider = gameObject.AddComponent<BoxCollider>();
	//		logCollider.isTrigger = true;
			//insideOffsetMultiplier = -insideOffsetMultiplier;
			size = meshCollider.bounds.size;
			boxCollider.size = new Vector3(size.x * insideOffsetMultiplier.x, size.y * insideOffsetMultiplier.y, size.z * insideOffsetMultiplier.z);

			transform.localPosition = Vector3.zero;
			transform.eulerAngles = rotationOffset;
		}
	}
}
