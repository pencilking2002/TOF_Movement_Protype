using UnityEngine;
using System.Collections;

public class TOFCamAdjuster : MonoBehaviour {

	private PhatRobit.SimpleRpgCamera rpgCam;

	// Use this for initialization
	void Awake () 
	{
		rpgCam = GetComponent<PhatRobit.SimpleRpgCamera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if (
	}

	void OnEnable()
	{
		EventManager.onCharEvent += TunnelCam;
	}

	void OnDisable()
	{
		EventManager.onCharEvent -= TunnelCam;
	}

	void TunnelCam (GameEvent gEvent)
	{
		if (gEvent == GameEvent.EnterTunnel)
		{
			rpgCam.allowRotation = false;
			rpgCam.stayBehindTarget = true;
			print("enter tunnel");
		}
		else if (gEvent == GameEvent.ExitTunnel)
		{
			rpgCam.allowRotation = true;
			rpgCam.stayBehindTarget = false;

		}
	}
}
