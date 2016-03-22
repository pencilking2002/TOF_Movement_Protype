using UnityEngine;
using System.Collections;

/*---------------------------------------------------\
|	Offsets the UVs of the mesh renderer by			 |
|	specified amount							     |
\---------------------------------------------------*/
public class UVOffsetController : MonoBehaviour {

	public float yScrollSpeed = 0.0f;
	public float xScrollSpeed = 0.0f;

	private float yOffset;
	private float xOffset;

	private MeshRenderer rend;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (rend.isVisible)
		{
			yOffset = Time.time * yScrollSpeed;
			xOffset = Time.time * xScrollSpeed;  
			rend.material.mainTextureOffset = new Vector2 (yOffset % 1, xOffset % 1);
		} 
	}
}
