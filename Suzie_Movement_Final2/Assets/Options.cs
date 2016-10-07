using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour {
	public Image fadeInPanel;
	private FadeInPanel panelScript;

	private Button button;
	private Image image;
	private RectTransform rt;

	private Color transpColor = new Color (1,1,1,0.5f);
	private Color fullColor = new Color (1,1,1,1);
	private Color darkColor = new Color(0, 0, 0, 0.5f);

	private bool optionsOpen = false;

	// Use this for initialization
	void Start () 
	{
		button = GetComponent <Button> ();
		image = GetComponent <Image> ();
		rt = GetComponent <RectTransform> ();
		panelScript = fadeInPanel.GetComponent <FadeInPanel> ();


		if (button == null || image == null || rt == null || panelScript == null)
			Debug.LogError ("Cant find button");


	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			MouseClick ();
		}
	}

//	void OnGUI()
//	{
//		GUI.Button (new Rect (100, 100, 100, 50), "Options Open:" + optionsOpen);
//	}
//
	public void MouseOver()
	{
		image.color = fullColor;
		LeanTween.scale (rt, Vector3.one * 1.2f, 0.2f);
		float offset = getRotationAmount (rt, -90);
		LeanTween.rotate (rt, offset, 0.2f);
	}

	public void MouseExit()
	{
		image.color = transpColor;
		LeanTween.scale (rt, Vector3.one, 0.2f);

		float offset = getRotationAmount (rt, 0);
		LeanTween.rotate (rt, offset, 0.2f);
	}

	public void MouseClick()
	{
		image.color = darkColor;
		LeanTween.scale (rt, Vector3.one * 1.3f, 0.2f);
		ToggleOptions ();
	}

	public void MouseUp()
	{
		image.color = transpColor;
		LeanTween.scale (rt, Vector3.one * 1.2f, 0.2f);
	}

	private void ToggleOptions()
	{
		if (!optionsOpen) 
		{
			optionsOpen = true;
			panelScript.FadeIn ();

		} 
		else
		{
			optionsOpen = false;
			panelScript.FadeOut ();
		}
	}
		
	private float getRotationAmount(RectTransform rt, float target)
	{
		float current = rt.eulerAngles.z;
		float offset = target - current;
		return offset;
	}

//	public void PauseGame()
//	{
//		Time.timeScale = 0.0f;
//	}
//
//	public void ResumeGame()
//	{
//		Time.timeScale = 1.0f;
//	}


}
