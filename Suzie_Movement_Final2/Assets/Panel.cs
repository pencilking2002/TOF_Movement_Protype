using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Panel : MonoBehaviour {

	public Image panel;

	// Use this for initialization
	void Awake () 
	{
		panel.gameObject.SetActive(true);
	}
		
	// Update is called once per frame
	void Update () {
	
	}
}
