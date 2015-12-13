using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TitleMenuController : MonoBehaviour {

	public Text playersJoinedText;
	List<int> playerIndices;

	// Use this for initialization
	void Start () {
		playerIndices = new List<int>();
		playersJoinedText = GameObject.Find("Players Joined").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.F)) {
			onAddPlayer(0);
		}
		if (Input.GetKeyUp(KeyCode.J)) {
			onAddPlayer(1);
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			if (playerIndices.Count > 1) {
				Debug.Log ("Load game");
				G.get ().players = playerIndices;
				Application.LoadLevel ("game_scene");
			}
		}
	}
	
	void onAddPlayer(int index) {
		if (!playerIndices.Contains(index)) {
			playerIndices.Add(index);
			playersJoinedText.text += "\nPlayer " + index;
			Debug.Log ("Add player " + index);
		}
	}
}
