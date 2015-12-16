using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TitleMenuController : MonoBehaviour {

	public Text playersJoinedText;
	List<int> playerIndices;
	public AudioClip sfx_player_join;
	public AudioClip sfx_player_drop;
	public AudioClip sfx_game_start;

	//This function will be called with a delay
	private void DelayedFunction()
	{
		Application.LoadLevel ("game_scene");
	}

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

				Debug.Log ("Start Sound");
				AudioSource audio = GetComponent<AudioSource>();
				audio.clip = sfx_game_start;
				audio.Play();

				Invoke("DelayedFunction", 2.0f);
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
	
			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = sfx_player_join;
			audio.Play();
		}
	}
	
	void onRemovePlayer(int index) {
		if (playerIndices.Contains(index)) {
			playerIndices.Remove(index);
			Debug.Log ("Remove player " + index);
			updateJoinedText();

			AudioSource audio = GetComponent<AudioSource>();
			audio.clip = sfx_player_drop;
			audio.Play();
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