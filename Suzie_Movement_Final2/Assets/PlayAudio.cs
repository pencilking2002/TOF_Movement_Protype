using UnityEngine;
using System.Collections;

public class PlayAudio : MonoBehaviour {

	public AudioSource source;
	public AudioClip[] collectSoundArr = new AudioClip[2];

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayCollectNut()
	{
		
		source.PlayOneShot (collectSoundArr[Random.Range(0, collectSoundArr.Length)]);
	}


}
