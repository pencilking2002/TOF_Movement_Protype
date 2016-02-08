using UnityEngine;
using System.Collections;
using System;

public class RSUtil : MonoBehaviour {
	
	public static RSUtil Instance;
	
	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
	}
	
	/// <summary>
	/// Determines whether this instance is ground the specified obj.
	/// </summary>
	/// <returns><c>true</c> if this specifed GO is some ort of ground otherwise, <c>false</c>.</returns>
	/// <param name="obj">Object.</param>
	public static bool IsGround(GameObject obj)
	{
		return obj.layer == 8;
	}

	public void DelayedAction(Action method, float delay)
	{
		StartCoroutine(C(method, delay));
	}
	
	private IEnumerator C (Action method, float delay)
	{
		yield return new WaitForSeconds(delay);
		method ();
	}
	
	/// <summary>
	///Get the Vector that represents the top center point of a collider
	/// </summary>
	/// <returns>The collider top point.</returns>
	/// <param name="collider">Collider.</param>
	static public Vector3 GetColliderTopPoint (Collider collider)
	{
		float height = collider.bounds.extents.y;
		return new Vector3(collider.transform.position.x, collider.transform.position.y + height, collider.transform.position.z);
	}

	/// <summary>
	/// Used to enable MonoBehavior scripts
	/// </summary>
	/// <param name="script">Reference to the script to disable</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	static public void EnableScript (MonoBehaviour script)
	{
		script.enabled = true;	
	}

	/// <summary>
	/// Used to disable MonoBehavior scripts
	/// </summary>
	/// <param name="script">Reference to the script to disable</param>
	/// <param name="enable">If set to <c>true</c> enable.</param>
	static public void DisableScript (MonoBehaviour script)
	{
		script.enabled = false;	
	}
	
	
	/// <summary>
	/// ing Pong a value from min to max
	
	/// </summary>
	/// <returns>The pong.</returns>
	/// <param name="aValue">A value.</param>
	/// <param name="aMin">A minimum.</param>
	/// <param name="aMax">A max.</param>
	static public float PingPong(float val, float aMin, float aMax)
	{
		return Mathf.PingPong(val, aMax-aMin) + aMin;
	}


}
