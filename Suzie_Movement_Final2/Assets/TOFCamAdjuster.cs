using UnityEngine;
using System.Collections;

public class TOFCamAdjuster : MonoBehaviour {

	private PhatRobit.SimpleRpgCamera rpgCam;
	private Vector3 origTargetOffset;
	private Vector3 tunnelTargetOffset = new Vector3 (0, 0.6f, 3.5f);

	// Use this for initialization
	void Start () 
	{
		rpgCam = GetComponent<PhatRobit.SimpleRpgCamera>();
		origTargetOffset = rpgCam.targetOffset;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if (
	}

//	void OnEnable()
//	{
//		EventManager.onCharEvent += TunnelCam;
//	}
//
//	void OnDisable()
//	{
//		EventManager.onCharEvent -= TunnelCam;
//	}
//
//	void TunnelCam (GameEvent gEvent)
//	{
////		if (gEvent == GameEvent.EnterTunnel)
////		{
////			rpgCam.allowRotation = false;
////			//rpgCam.stayBehindTarget = true;
////			rpgCam.targetOffset = tunnelTargetOffset;
////			print("enter tunnel");
////		}
////		else if (gEvent == GameEvent.ExitTunnel)
////		{
////			rpgCam.allowRotation = true;
////			//rpgCam.stayBehindTarget = false;
////			rpgCam.targetOffset = origTargetOffset;
////
////		}
//	}
}
