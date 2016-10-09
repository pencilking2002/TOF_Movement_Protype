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

}
