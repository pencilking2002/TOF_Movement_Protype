using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour {

	public float maxUpAndDown = 1;               // amount of meters going up and down
	public float speed = 200;            	     // up and down speed
	private float angle = 0;           			 // angle to determin the height by using the sinus
	private float addRandomSpeed;

	private float toDegrees = Mathf.PI/180;      // radians to degrees
 	private MeshRenderer rend;

 	private bool initialized = false;
 	private Vector3 startingPos;

 
 	IEnumerator Start()
 	{
		addRandomSpeed = Random.Range(-10, 10);
 		speed += addRandomSpeed;
		rend = GetComponent<MeshRenderer>();
		startingPos = transform.position;

 		yield return new WaitForSeconds(Random.Range(0.0f, 4.0f));
		initialized = true;
 	}	

	void Update()
	{
		if (initialized && rend.isVisible)
		{
			angle += speed * Time.deltaTime;
			transform.position = new Vector3(startingPos.x, transform.position.y + maxUpAndDown * Mathf.Sin(angle * toDegrees), startingPos.z);

		}
	}

	public void Activate (bool activate)
	{
		if (activate)
		{
			rend.enabled = true;
			this.enabled = true;
		}
		else
		{
			rend.enabled = false;
			this.enabled = false;
		}

	}

}


