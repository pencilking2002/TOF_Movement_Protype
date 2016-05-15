using UnityEngine;
using System.Collections;

public class TransparentLogic : MonoBehaviour {

	public bool isTransparent = false;

	private float opacityLevel = 0.5f;
	private MeshRenderer rend;
	private Color oldColor;
	private Color transparentColor;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<MeshRenderer> ();
		oldColor = rend.sharedMaterial.color;
		transparentColor = new Color (oldColor.r, oldColor.g, oldColor.b, opacityLevel);

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!rend.isVisible)
			return;
			
		if (isTransparent) {
			ChangeOpacity (transparentColor);
		} 
		else 
		{
			ChangeOpacity (oldColor);
		}
			
	}

	void ChangeOpacity (Color color)
	{
		rend.sharedMaterial.SetColor ("_Color", color);
	}
}
