using UnityEngine;
using System.Collections;

public class NutManager : MonoBehaviour {

	public Transform player;
	private Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate ()
	{
		//transform.position = player.position;
		rb.MovePosition(player.position);
	}	

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == 15)
		{
			//col.gameObject.SetActive(true);
			//print("nut activated");
			col.gameObject.GetComponent<Float>().Activate(true);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.layer == 15)
		{
			//col.gameObject.SetActive(false);
			//print("nut deactivated");
			col.gameObject.GetComponent<Float>().Activate(false);

		}
	}	
}
