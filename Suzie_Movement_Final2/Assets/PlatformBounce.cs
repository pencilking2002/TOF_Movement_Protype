using UnityEngine;
using System.Collections;

public class PlatformBounce : MonoBehaviour {	

	Rigidbody rb;

	void Awake()
	{
		//rb = GetComponent<Rigidbody>();
	}

	void OnCollisionEnter()
	{
		//rb.isKinematic = false;
	}
}
