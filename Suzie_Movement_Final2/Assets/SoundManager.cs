using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	[Header("Audio Sources")]
	public AudioSource soundTrackAS;
	public AudioSource footstepsAS;
	public AudioSource nutCollectAS;
	public AudioSource landAS;
	public AudioSource jumpAS;

	[Header("Audio Clips")]
	public AudioClip footStep;
	public AudioClip[] nutCollect;
	public AudioClip charLand;
	public AudioClip charJump;

	public static SoundManager Instance;

	void Awake()
	{
		if (Instance == null) 
			Instance = this;
		else 
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnEnable()
	{
		EventManager.onCharEvent += PlayLand;
		//EventManager.onCharEvent += PlayFootstep;
		EventManager.onCharEvent += PlayJump;
		EventManager.onCharEvent += PlayNutCollect;
	}


	void OnDisable()
	{
		EventManager.onCharEvent -= PlayLand;
		//EventManager.onCharEvent -= PlayFootstep;
		EventManager.onCharEvent -= PlayJump;
		EventManager.onCharEvent -= PlayNutCollect;
	}

	public void PlayFootstep()
	{
		//if (e == GameEvent.Footstep)
		footstepsAS.volume = Random.Range (0.5f, 0.8f);
		footstepsAS.pitch = Random.Range (1.0f, 1.3f);
		footstepsAS.PlayOneShot (footStep); //print ("Footstep Sound"); 
	}

	void PlayNutCollect(GameEvent e)
	{
		if (e == GameEvent.CollectNut)
		{
			nutCollectAS.pitch = Random.Range (1.0f, 1.3f);
			nutCollectAS.PlayOneShot (nutCollect[Random.Range(0, nutCollect.Length)]); //print ("Nut Collect sound"); 
		}
	}

	void PlayLand(GameEvent e)
	{
		if (e == GameEvent.CollderLand)
		{
			landAS.PlayOneShot (charLand);//print ("Land sound");
		}
	}



	void PlayJump(GameEvent e)
	{
		if (e == GameEvent.Jump)
			jumpAS.PlayOneShot (charJump);//print ("Jump Sound"); 
	}



}
