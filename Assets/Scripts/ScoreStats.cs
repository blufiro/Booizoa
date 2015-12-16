using UnityEngine;
using System.Collections;

public class ScoreStats : MonoBehaviour {

	public GameController gameController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach (int i in G.get().players) {
			if (Input.GetButton("Select" + i)) {
				gameController.onScoreStatsDone();
			}
		}
	}
}