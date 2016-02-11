﻿using UnityEngine;
using System.Collections;

public class TestEvent : MonoBehaviour {

	public delegate void ClickEvent();
	public static ClickEvent onClickEvent;

	private void OnEnable ()
	{
		//EventManager.onCharEvent += Test;
		//onClickEvent += Test;
		EventManager.onInputEvent += Test;
	}

	private void OnDisable ()
	{
		//EventManager.onCharEvent -= Test;
		//onClickEvent -= Test;
		EventManager.onInputEvent -= Test;

	}

	private void Update ()
	{
//		if (Input.GetKeyDown(KeyCode.Space))
//		{
//			if (onClickEvent != null)
//				onClickEvent();
//		}
	}

	private void Test (GameEvent gEvent)
	{
		//print(e);
		print("TestEvent: yo");
	}

}
