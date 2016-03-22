using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour {

	public Transform t;

	// Use this for initialization
	void Start () 
	{}
	#if UNITY_EDITOR

	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(t);
	}

	#endif
}
