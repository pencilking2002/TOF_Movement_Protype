// Class to setup the follow object and the camera.
// By default they are parented to the character for easy positioning in the scene
// Whwn the game starts his class unparents them
using UnityEngine;
using System.Collections;

public class CharacterSetup : MonoBehaviour {

	public Transform cam;
	public Transform follow;

	void Awake()
	{
		cam.SetParent(null);
		follow.SetParent(null);
	}

	// Use this for initialization
	void Start () {
	
	}
}
