using UnityEngine;
using System.Collections;

public class CystalTest : MonoBehaviour {

	private MeshRenderer rend;
	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<MeshRenderer>();
		rend.sharedMaterial.SetInt("_ZWrite", 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
