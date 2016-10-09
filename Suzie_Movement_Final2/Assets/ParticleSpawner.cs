using UnityEngine;
using System.Collections;

public class ParticleSpawner : MonoBehaviour {

    public GameObject smokePuff;
    public GameObject splash;
    public GameObject nutCollect;
    public GameObject stepPuff;
    public GameObject stepSplash;

    private GameObject smokeInstance;
    private GameObject nutCollectInstance;
    private GameObject stepPuffInstance;
    private GameObject stepSplashInstance;

    private LandingController landController;
    private NutCollector nutCollector;

	// Use this for initialization
	void Start () 
    {
        landController = GetComponentInParent<LandingController>();
        nutCollector = GetComponentInParent<NutCollector>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
     
//    void OnGUI()
//    {
//        if (GUI.Button (new Rect (100, 100, 100, 50), "Smoke puff"))
//        {
//            PlaySmokePuff();
//        }
//    }
    void OnEnable()
    {
        EventManager.onCharEvent += PlaySmokePuff;
        EventManager.onCharEvent += PlayNutCollectEffect;
        EventManager.onCharEvent += PlayStepEffect;

    }


    void OnDisable()
    {
        EventManager.onCharEvent -= PlaySmokePuff;
        EventManager.onCharEvent -= PlayNutCollectEffect;
        EventManager.onCharEvent -= PlayStepEffect;
    }

    private void PlaySmokePuff(GameEvent e)
    {
        if (e == GameEvent.Land)
        {
            if (landController.touchingWater)
            {
                smokeInstance = CFX_SpawnSystem.GetNextObject(splash);
            }
            else
            {
                smokeInstance = CFX_SpawnSystem.GetNextObject(smokePuff);
            }
            
               
            smokeInstance.transform.position = transform.parent.transform.position;
        }
    }

    private void PlayNutCollectEffect(GameEvent e)
    {
        if (e == GameEvent.CollectNut)
        {
            print ("last nut collected pos:");
            print(nutCollector.lastCollectedNutPos); 
            nutCollectInstance = CFX_SpawnSystem.GetNextObject(nutCollect);
            nutCollectInstance.transform.position = nutCollector.lastCollectedNutPos;
        }

    }

    private void PlayStepEffect(GameEvent e)
    {
        if (e == GameEvent.Footstep)
        { 
            if (!landController.touchingWater)
            {
                print ("step puff");
                stepPuffInstance = CFX_SpawnSystem.GetNextObject(stepPuff);
                //stepPuffInstance.transform.parent = null;
                stepPuffInstance.transform.position = transform.parent.transform.position + transform.parent.transform.forward * 0.5f;
                //stepPuffInstance.transform.position = transform.parent.transform.position + transform.parent.transform.forward * -0.2f;
            }
            else
            {
                stepSplashInstance = CFX_SpawnSystem.GetNextObject(stepSplash);
                //stepSplashInstance.transform.parent = null;
                stepSplashInstance.transform.position = transform.parent.transform.position  + transform.parent.transform.forward * 0.5f;
            }
        }
    }
}
