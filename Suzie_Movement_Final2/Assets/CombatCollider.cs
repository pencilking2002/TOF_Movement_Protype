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
		
		if (other.gameObject.layer == 24) 
		{
			wall = other.GetComponentInParent<BreakableWall> ();
			wall.Hit ();
			print ("Player Wall health hit: " + wall.health);
		}
	}
}
