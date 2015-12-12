using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	GameObject playerTemplate;
	GameObject[] players;
	
	// Use this for initialization
	void Start () {
		players = new GameObject[G.get().MAX_PLAYERS];
		foreach (int i in G.get ().players) {
			Debug.Log ("Creating Player " + i);
			players[i] = Instantiate(playerTemplate);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void GameReset() {
		
	}
	
	void GameStart() {
	}
	
	void GameJoin() {
		
	}
	
	void GameUpdate() {
	}
	
	void GameWin() {
	}
	
	void GameLose() {
	}
	
	void GameRestart() {
	}
	
}
