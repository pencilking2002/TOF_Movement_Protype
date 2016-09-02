using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NutCollector : MonoBehaviour 
{
	public Text nutCountText;
	public Animator panelAnimator;
	public Text gameOverText;

	public static NutCollector Instance;

	[Range(0,10)]
	public float scaleTime = 2.0f;

	[Range(0,1)]
	public float scaleUpTime = 0.1f;

	[Range(0,10)]
	public float moveDamp = 2.0f;

	public Vector3 scaleUpVector = new Vector3(1.2f, 1.4f, 1.2f); 
	private Vector3 moveVel;


	[HideInInspector]
	public List<GameObject> totalNuts = new List<GameObject>();

	[HideInInspector]
	public List<GameObject> collectedNuts = new List<GameObject>();

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else Destroy(this);

		if (nutCountText == null)
			Debug.Log("Nut count text not defined");

	}

	void Start ()
	{
		GameObject[] Nuts = GameObject.FindGameObjectsWithTag("CrystalNut");

		foreach(GameObject nut in Nuts)
			totalNuts.Add(nut);

		if (nutCountText != null)
			nutCountText.text = collectedNuts.Count + " / " + totalNuts.Count;
	}

	public void RegisterNut (GameObject nut)
	{
		totalNuts.Add(nut);
	}

	private void OnTriggerStay (Collider col)
	{
		if (col.gameObject.layer == 15)
		{
			GameObject pickup = col.gameObject;
			CollectNut(pickup);

			//print("nut triggered");

			// Switch pickup to a layer that ignores all player collisions
			pickup.layer = 16;
			float yOffset = 1.3f;

			LeanTween.scale(pickup, scaleUpVector, scaleUpTime)
			.setOnComplete(() => {
				LeanTween.scale(pickup, Vector3.zero, scaleTime)
					.setOnUpdate((float pos) => {
							Vector3 targetPos = pickup.transform.position + Vector3.up; //new Vector3(transform.position.x, pickup.transform.position.y + yOffset, transform.position.z);
							pickup.transform.position = Vector3.SmoothDamp(pickup.transform.position, targetPos, ref moveVel, moveDamp);	
				})
				.setOnComplete(() => {
					pickup.SetActive(false);
				});
			});
		}
	}

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.G))
			GameOver();
	}
	private void CollectNut(GameObject pickup)
	{
		EventManager.OnCharEvent (GameEvent.CollectNut);

		collectedNuts.Add(pickup);

		if (nutCountText != null)
			nutCountText.text = collectedNuts.Count + " / " + totalNuts.Count;

		if (collectedNuts.Count == totalNuts.Count)
		{
			GameOver();
		}
	}

	private void GameOver()
	{
		panelAnimator.SetTrigger("FadeIn");
		gameOverText.gameObject.SetActive(true);
	}
}
