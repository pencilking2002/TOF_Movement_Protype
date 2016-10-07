using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInPanel : MonoBehaviour {

	public Button restartBtn, quitBtn;
	private RectTransform restartRT, quitRT;

	//private Animator anim;
	RectTransform rt;
	Image panel;


	void Awake()
	{
		rt = GetComponent <RectTransform> ();
		panel = GetComponent <Image> ();

		restartRT = restartBtn.GetComponent<RectTransform> ();
		quitRT = quitBtn.GetComponent<RectTransform> ();


		restartRT.gameObject.SetActive (false);
		quitRT.gameObject.SetActive (false);

		//anim = GetComponent<Animator> ();
		//anim.SetTrigger ("FadeIn");
		panel.color = new Color(0,0,0,1);
		LeanTween.alpha (rt, 0.0f, 1.5f);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void FadeIn()
	{
		
		//anim.SetTrigger ("DisplayOptions");
		LeanTween.alpha(rt, 0.6f, 0.1f).setOnComplete(() => { Time.timeScale = 0; });

		restartBtn.gameObject.SetActive (true);
		quitBtn.gameObject.SetActive (true);
		LeanTween.alpha (restartRT, 1.0f, 0.1f);
		LeanTween.alpha (quitRT, 1.0f, 0.1f);


	}

	public void FadeOut()
	{
		//Time.timeScale = 1;
		LeanTween.alpha (rt, 0.0f, 0.1f);
		LeanTween.alpha (restartRT, 0, 0.1f).setOnComplete (() => { restartBtn.gameObject.SetActive(false); });
		LeanTween.alpha (quitRT, 0, 0.1f).setOnComplete (() => { quitBtn.gameObject.SetActive(false); });

	}

	public void PauseGame()
	{
		//Time.timeScale = 0.0f;
	}

	public void ResumeGame()
	{
		//Time.timeScale = 1.0f;
	}


	public void RestartClick()
	{
		Time.timeScale = 1;
		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}

	public void QuitClick()
	{
		Time.timeScale = 1;
		Resources.UnloadUnusedAssets ();
		System.GC.Collect();

		EventManager.onCharEvent = null;
		EventManager.onInputEvent = null;
		EventManager.onDetectEvent = null;
		EventManager.onCharEvent2 = null;

		Destroy (Camera.main.gameObject);
		Destroy (GameObject.FindGameObjectWithTag ("EventManager"));
		//TOFCamAdjuster = null;
		Application.Quit ();
	}

}
