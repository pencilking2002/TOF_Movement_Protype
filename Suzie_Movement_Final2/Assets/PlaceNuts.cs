using UnityEngine;
using System.Collections;

public class PlaceNuts : MonoBehaviour {

	[Range(0,20)]
	public int numNuts = 5;
	private LeanTweenPath path;
	private GameObject itemPrefab;
	private LTBezierPath nutPath;
	private Transform[] Nuts;

	// Use this for initialization
	void Start () 
	{
		Nuts = new Transform[numNuts];

		// Get the leantween editor path
		path = GetComponent<LeanTweenPath>();

		// Create a bezier path based on the path
		nutPath = new LTBezierPath(path.vec3);

		// Load up the prefab
		itemPrefab = Resources.Load("CrystalNutPrefab", typeof(GameObject)) as GameObject;

		// Fill up the array with nuts
		for (int i=0; i< Nuts.Length; i++)
			Nuts[i] = (Transform) Instantiate(itemPrefab).transform;

		float pct = 0.0f;
		//float portion = 1.0f/Nuts.Length + 0.1f;
		float portion = 1.0f / Nuts.Length;
		print("portion:" + portion);
		for (int i = 0; i < Nuts.Length; i++)
		{
			//float pct = 0.1f * i * (10 / Nuts.Length);
			print(pct);
			nutPath.place(Nuts[i], pct);
			pct += portion;



		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
