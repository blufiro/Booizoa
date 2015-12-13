using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	
	public GameObject world;
	public GameObject gridWorld;
	public GameObject playerTemplate;
	public GameObject playerLeftRotationTemplate;
	public GameObject playerUpTemplate;

	public Grid Grid
	{
		get { return m_grid; }
	}

	private GameObject[] m_players;
	private Grid m_grid;
	private Anim countDownAnim;
	private Text timeLeftText;
	
	// Use this for initialization
	void Start () {
		// Set a default set of players if none are initialized. (title screen likely skipped)
		if (G.get ().players.Count == 0) {
			G.get().players.Add(0);
			G.get().players.Add(1);
		}

		// Create players from template
		int numPlayers = G.get().players.Count;
		m_players = new GameObject[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			Debug.Log ("Creating Player " + i);
			m_players[i] = Instantiate(playerTemplate);
			m_players[i].transform.parent = gridWorld.transform;
		}
		
		timeLeftText = GameObject.Find ("Time Left Text").GetComponent<Text>();
		
		onGameReset();
	}
	
	// Update is called once per frame
	void Update () {
		AnimMaster.get ().update();
		
		if (countDownAnim != null) {
			timeLeftText.text = "Time Left: " + countDownAnim.getTimeRemainingSeconds().ToString ("F1") + "s";
		}
	}
	
	void onGameReset() {
		Debug.Log ("onGameReset");
		
		// Create grid
		m_grid = new Grid(G.get ().GRID_W, G.get ().GRID_H);
		
		// Initialize players
		int numPlayers = m_players.Length;
		for (int i = 0; i < numPlayers; i++) {
			int startingX = (i + 1) * G.get ().GRID_W / (numPlayers + 1);
			m_players[i].GetComponent<Player>().init(this, i, new Grid.Gid(startingX, 0));
		}
		
		onGameStart ();
	}
	
	void onGameStart() {
		Debug.Log ("onGameStart");
		onGameRoundStart();
	}
	
	void onGameRoundStart() {
		Debug.Log ("onGameRoundStart");
		AnimMaster.clearWithKey("roundCountDown");
		countDownAnim = AnimMaster.delay("roundCountDown", this.gameObject, G.get ().COUNTDOWN_SECONDS).onComplete("onGameRoundEnd");
		foreach (GameObject player in m_players) {
			player.SendMessage("onGameRoundStart");
		}
	}

	void onGameRoundEnd() {
		Debug.Log ("onGameRoundEnd");
		countDownAnim = null;
		timeLeftText.text = "";
	}
			
	void onGameUpdate() {
	}
	
	void onGameWin() {
	}
	
	void onGameLose() {
	}	
}
