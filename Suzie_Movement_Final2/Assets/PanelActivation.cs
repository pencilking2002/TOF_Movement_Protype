using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PanelActivation : MonoBehaviour {

	public Image panel;

	// Use this for initialization
	void Awake () 
	{
		panel.gameObject.SetActive(true);
	}
}
