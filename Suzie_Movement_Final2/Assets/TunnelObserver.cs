using UnityEngine;
using System.Collections;

public class TunnelObserver : MonoBehaviour {

	public  static TunnelObserver Instance;

	[HideInInspector]
	public BoxCollider tunnelCollider;

	[HideInInspector]
	public Vector3 entryPoint;

	public bool inTunnel 
	{
		get 
		{
			return InTunnel;
		}
		set 
		{
			if (value)
			{
				InTunnel = true;
				EventManager.OnCharEvent(GameEvent.EnterTunnel);
			}
			else
			{
				InTunnel = false;
				EventManager.OnCharEvent(GameEvent.ExitTunnel);
			}
		}
	}

	private bool InTunnel;

	void Awake ()
	{
		if (Instance == null)
			Instance = this;
		else 
			Destroy(this);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer == 13)
		{
			tunnelCollider = (BoxCollider) col;
			entryPoint = transform.position;
			inTunnel = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.layer == 13)
		{
			tunnelCollider = (BoxCollider) col;
			inTunnel = false;
		}
	}
}
