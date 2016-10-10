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
	public AudioClip charWaterLand;
	public AudioClip charJump;
	public AudioClip footstepSplash;

	public static SoundManager Instance;
	public float timeLanded;

	public LandingController landingController;

	void Awake()
	{
		if (Instance == null) 
			Instance = this;
		else 
			Destroy (gameObject);
	}

//      void OnGUI()
//      {
//            GUI.Button (new Rect (100, 100, 200, 50), "On land:" + !landingController.touchingWater);
//      }

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
		if (landingController.touchingWater) {
			footstepsAS.volume = Random.Range (0.5f, 0.8f);
			footstepsAS.pitch = Random.Range (1.0f, 1.3f);
			footstepsAS.PlayOneShot (footstepSplash); //print ("Footstep Sound");
		} 
		else 
		{
			footstepsAS.volume = Random.Range (0.5f, 0.8f);
			footstepsAS.pitch = Random.Range (1.0f, 1.3f);
			footstepsAS.PlayOneShot (footStep); //print ("Footstep Sound");
		}
			
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
		if (Time.time > timeLanded + 0.3f) 
		{
			if (e == GameEvent.ColliderLand) 
			{
				landAS.PlayOneShot (charLand);//print ("Land sound");
				timeLanded = Time.time;
			} 
			else if (e == GameEvent.WaterColliderLanding) 
			{
				landAS.PlayOneShot (charWaterLand);
				timeLanded = Time.time;
			}
		}
	}


	void PlayJump(GameEvent e)
	{
		if (e == GameEvent.Jump)
			jumpAS.PlayOneShot (charJump);//print ("Jump Sound"); 
	}



}
