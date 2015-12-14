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
		for (int playerIndex = 0; playerIndex < G.get ().MAX_PLAYERS; playerIndex++) {
			if (Input.GetButton("Select" + playerIndex)) {
				onAddPlayer(playerIndex);
			}
			if (Input.GetButton("Cancel" + playerIndex)) {
				onRemovePlayer(playerIndex);
			}
		}
		if (Input.GetButton("Start")) {
			if (playerIndices.Count > 1) {
				Debug.Log ("Load game");
				G.get ().players = playerIndices;
				Application.LoadLevel ("game_scene");
			}
		}
		if (Input.GetButton("Back")) {
			Application.Quit();
		}
	}
	
	void onAddPlayer(int index) {
		if (!playerIndices.Contains(index)) {
			playerIndices.Add(index);
			Debug.Log ("Add player " + index);
			updateJoinedText();
		}
	}
	
	void onRemovePlayer(int index) {
		if (playerIndices.Contains(index)) {
			playerIndices.Remove(index);
			Debug.Log ("Remove player " + index);
			updateJoinedText();
		}
	}
	
	void updateJoinedText() {
		string text = "Join now!";
		foreach (int playerIndex in playerIndices) {
			text += "\nPlayer " + playerIndex + " joined!";
		}
		playersJoinedText.text = text;
	}
}
