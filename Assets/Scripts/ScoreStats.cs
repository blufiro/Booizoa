﻿using UnityEngine;
using System.Collections;

public class ScoreStats : MonoBehaviour {

	public GameController gameController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Space)) {
			gameController.onScoreStatsDone();
		}
	}
}
