using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {

	public GameObject player;
	public Rigidbody2D rb;
	public float jumpForce = 200;

	void Update()
	{
		print (Time.time);
	}

	void FixedUpdate()
	{
		rb.AddForce(Vector2(0, jumpForce);
	}


}
