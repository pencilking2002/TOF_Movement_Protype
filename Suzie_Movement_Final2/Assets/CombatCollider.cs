using UnityEngine;
using System.Collections;

public class CombatCollider : MonoBehaviour {

	public float forceAmount;
	public Transform character;

	private BreakableWall wall;
	private Vector3 force;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (wall == null)
			wall = other.GetComponentInParent<BreakableWall> ();
		
		if (other.gameObject.layer == 24) 
		{
			//if (wall.health != 0) 
			//{
				EventManager.OnCharEvent (GameEvent.WallHit);
				print ("Player Wall health hit: " + wall.health);
				//force = character.forward * forceAmount;
				//other.GetComponent<Rigidbody> ().AddForce (force, ForceMode.Impulse);
			//} 
			//else if ()
			//{
				//EventManager.OnCharEvent (GameEvent.WallHit);
				//force = character.forward * forceAmount;

			//}

		}
	}
}
