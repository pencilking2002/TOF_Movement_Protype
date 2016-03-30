using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour {
	public float maxUpAndDown = 1;               // amount of meters going up and down
	public float speed = 200;            	     // up and down speed
	public float angle = 0;           			 // angle to determin the height by using the sinus
	private float toDegrees = Mathf.PI/180;      // radians to degrees
 	private MeshRenderer rend;

 	private void Awake()
 	{
 		rend = GetComponent<MeshRenderer>();
 		//NutCollector.Instance.RegisterNut(gameObject);
 	}	

	void Update()
	{
		if (rend.isVisible)
		{
			angle += speed * Time.deltaTime;

			if (angle > 360) angle -= 360;
				transform.position = new Vector3(transform.position.x, transform.position.y + maxUpAndDown * Mathf.Sin(angle * toDegrees), transform.position.z);
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


