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
		if (rend.isVisible) 
		{
			if (isTransparent)
				ChangeOpacity (0.5f);
			else
				ChangeOpacity (1.0f);
		}
			
	}

	void ChangeOpacity (float val)
	{
		LeanTween.alpha (gameObject, val, 0.1f);
		//rend.sharedMaterial.SetColor ("_Color", color);
	}
}
