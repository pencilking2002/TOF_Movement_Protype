using UnityEngine;
using System.Collections;

public class MakeTransparent : MonoBehaviour {
	[HideInInspector]
	public Transform Squirrel;
	public LayerMask layerMask;
	private RaycastHit hit;
	private TransparentLogic opacityScript;

	// Use this for initialization
	void Start () 
	{
		Squirrel = GameManager.Instance.charController.transform;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 squirrelPos = new Vector3 (Squirrel.position.x, Squirrel.position.y + 0.5f, Squirrel.position.z);
		Vector3 dir = squirrelPos - transform.position;
		Debug.DrawRay (transform.position, dir, Color.red);

		if (Physics.Raycast (transform.position, dir, out hit, dir.magnitude, layerMask)) 
		{
			if (opacityScript == null) 
			{
				// Get the script component from the mesh
				opacityScript = hit.collider.GetComponent <TransparentLogic> ();
				// Set the isTransparent flag on the object
				if (opacityScript != null) 
				{
					opacityScript.isTransparent = true;
					print ("make transparent"); 
				}
			}
		} 
		// If raycast is not hitting any objects than set any of them that are currently transparent
		// back to normal
		else if (opacityScript != null) 
		{
			opacityScript.isTransparent = false;
			opacityScript = null;
		}
	}


}
