using UnityEngine;
using System.Collections;

public class BreakableWall : MonoBehaviour {

	public int health = 3;
	public int forceAmount = 100;
	private Vector3 force;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnEnable()
	{
		EventManager.onCharEvent += Hit;
	}

	public void OnDisable()
	{
		EventManager.onCharEvent -= Hit;
	}

	void Hit(GameEvent gEvent)
	{
		if (gEvent == GameEvent.WallHit) 
		{
			if (health > 1) 
			{
				print ("wall health: " + health); 
				health--;
			}
			else
			{
				print ("wall crumble"); 
				foreach (Transform brick in transform) 
				{
					health--;
					Rigidbody rb = brick.GetComponent <Rigidbody> ();
					rb.isKinematic = false;
				}

				foreach (Transform brick in transform) 
				{
					Rigidbody rb = brick.GetComponent <Rigidbody> ();
					rb.AddExplosionForce (forceAmount, brick.position, 1.0f);
				}
			

			}
		}
	}
}
