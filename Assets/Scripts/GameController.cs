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
	private Anim m_countDownAnim;
	private Text m_timeLeftText;
	private bool[] m_playersSelectionDone;
	
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
		m_playersSelectionDone = new bool[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			Debug.Log ("Creating Player " + i);
			m_players[i] = Instantiate(playerTemplate);
			m_players[i].transform.parent = gridWorld.transform;
		}
		
		m_timeLeftText = GameObject.Find ("Time Left Text").GetComponent<Text>();
		
		onGameReset();
	}
	
	// Update is called once per frame
	void Update () {
		AnimMaster.get ().update();
		
		if (m_countDownAnim != null) {
			m_timeLeftText.text = "Time Left: " + m_countDownAnim.getTimeRemainingSeconds().ToString ("F1") + "s";
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
		
		AnimMaster.delay ("gameStartDelay", this.gameObject, 0.5f).onComplete("onGameStart");
	}
	
	void onGameStart() {
		Debug.Log ("onGameStart");
		onGameRoundStart();
	}
	
	void onGameRoundStart() {
		Debug.Log ("onGameRoundStart");
		AnimMaster.clearWithKey("roundCountDown");
		m_countDownAnim = AnimMaster.delay("roundCountDown", this.gameObject, G.get ().COUNTDOWN_SECONDS).onComplete("onGameRoundEnd");
		for (int i = 0; i < m_players.Length; i++) {
			GameObject player = m_players[i];
			player.SendMessage("onGameRoundStart");
			// reset selection done bool
			m_playersSelectionDone[i] = false;
		}
	}

	public void onPlayerSelectionDone(int playerIndex) {
		Debug.Log ("onPlayerSelectionDone " + playerIndex);
		m_playersSelectionDone[playerIndex] = true;
		
		// if all players are done, proceed to round end.
		foreach (bool done in m_playersSelectionDone) {
			if (!done) {
				return;
			}
		}
		onGameRoundEnd();
	}
	
	void onGameRoundEnd() {
		Debug.Log ("onGameRoundEnd");
		AnimMaster.clearWithKey("roundCountDown");
		m_countDownAnim = null;
		m_timeLeftText.text = "";
		
		foreach(GameObject player in m_players) {
			player.SendMessage("onExecuteChosenDirection");
		}
		
		Debug.Log ("wait for game round start");
		AnimMaster.delay ("gameRoundStartDelay", this.gameObject, 1.0f).onComplete ("onGameRoundStart");
	}

	void onGameWin() {
	}
	
	void onGameLose() {
	}	
}
