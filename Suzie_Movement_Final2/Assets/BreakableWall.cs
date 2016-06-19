using UnityEngine;
using System.Collections;

public class BreakableWall : MonoBehaviour {

	public int health = 3;
	public int forceAmount = 100;
	private Vector3 force;

	//private Transform[] = 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Hit()
	{
		health--;

		if (health > 1) 
		{
			print ("wall health: " + health); 
			GameObject b = transform.GetChild (0).gameObject;
			Color color = b.GetComponent<MeshRenderer> ().material.color;


			foreach (Transform brick in transform)
			{	
				GameObject theBrick = brick.gameObject;
				LeanTween.color(theBrick, Color.red, 0.1f)
					.setOnComplete (() => {
						LeanTween.color(theBrick, color, 0.1f);
					});
			}

		}
		else
		{
			print ("wall crumble"); 
			force = GameManager.Instance.charController.transform.forward * forceAmount;
			foreach (Transform brick in transform) 
			{
				Rigidbody rb = brick.GetComponent <Rigidbody> ();
				rb.isKinematic = false;
				//rb.AddExplosionForce (forceAmount, brick.position, 1.0f, 5.0f);
				rb.AddForceAtPosition (force, brick.position);
			}

//				foreach (Transform brick in transform) 
//				{
//					Rigidbody rb = brick.GetComponent <Rigidbody> ();
//					//rb.AddExplosionForce (forceAmount, brick.position, 1.0f, 5.0f);
//				}
		

		}

	}
}
