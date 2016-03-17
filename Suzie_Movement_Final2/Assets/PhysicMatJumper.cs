using UnityEngine;
using System.Collections;

public class PhysicMatJumper : MonoBehaviour {

	[HideInInspector]
	public bool colliding = false;

	private Rigidbody rb;

	private void Start ()
	{
		rb = GetComponentInParent<Rigidbody>();

		if (!rb) 
			Debug.LogError("PhysicMatjumper Error: Rigidbody was not assigned");
	}

	private void OnTriggerStay()
	{
		if (rb.velocity != Vector3.zero)
			colliding = true;
	}

	private void OnTriggerExit ()
	{
		colliding = false;
	}

}
