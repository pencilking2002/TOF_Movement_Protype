using UnityEngine;
using System.Collections;

public class FootstepSound : MonoBehaviour {

	public void PlayFootstep()
	{
		SoundManager.Instance.PlayFootstep ();
	}
}
