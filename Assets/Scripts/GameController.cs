using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public GameObject world;
	public GameObject gridWorld;
	public GameObject playerTemplate;
	public GameObject playerLeftRotationTemplate;
	public GameObject playerUpTemplate;
	public Text	playerScoreStatsTextTemplate;

	public Grid Grid
	{
		get { return m_grid; }
	}

	private GameObject[] m_players;
	private bool[] m_playersSelectionDone;
	private bool[] m_playersCheckSpace;
	private Grid m_grid;
	private Anim m_countDownAnim;
	private Text m_timeLeftText;
	private Text m_debugText;
	private Image m_scoreStatsPopup;
	private List<Text> m_playerScoresTexts;
	
	// Use this for initialization
	void Start () {
		m_timeLeftText = GameObject.Find ("Time Left Text").GetComponent<Text>();
		m_debugText = GameObject.Find ("Debug Text").GetComponent<Text>();
		m_scoreStatsPopup = GameObject.Find ("Score Stats Popup").GetComponent<Image>();
		
		// Set a default set of players if none are initialized. (title screen likely skipped)
		if (G.get ().players.Count == 0) {
			G.get().players.Add(0);
			G.get().players.Add(1);
		}

		// Create players from template
		int numPlayers = G.get().players.Count;
		m_players = new GameObject[numPlayers];
		m_playersSelectionDone = new bool[numPlayers];
		m_playersCheckSpace = new bool[numPlayers];
		m_playerScoresTexts = new List<Text>();
		for (int i = 0; i < numPlayers; i++) {
			Debug.Log ("Creating Player " + i);
			m_players[i] = Instantiate(playerTemplate);
			m_players[i].transform.parent = gridWorld.transform;
			
			Text playerScoreStatsText = Instantiate(playerScoreStatsTextTemplate) as Text;
			playerScoreStatsText.transform.SetParent(m_scoreStatsPopup.transform, false);
			m_playerScoresTexts.Add(playerScoreStatsText);
			// TODO layout the texts
		}
		
		onGameReset();
	}
	
	// Update is called once per frame
	void Update () {
		AnimMaster.get ().update();
		
		if (m_countDownAnim != null) {
			m_timeLeftText.text = "Time Left: " + m_countDownAnim.getTimeRemainingSeconds().ToString ("F1") + "s";
		}
		
		string debugText = " Debug Text \n";
//		for (int i = 0; i < m_players.Length; i++) {
//			GameObject player = m_players[i];
//			debugText += "Player " + i + " input"+player.GetComponent<Player>().acceptInput;
//		}
//		m_debugText.text = debugText;
		
	}
	
	void onGameReset() {
		Debug.Log ("onGameReset");
		
		m_scoreStatsPopup.gameObject.SetActive(false);
		
		if (m_grid != null) {
			m_grid.clear();
		}
		
		// Create grid
		m_grid = new Grid(G.get ().GRID_W, G.get ().GRID_H);
		
		// Initialize players
		int numPlayers = m_players.Length;
		for (int i = 0; i < numPlayers; i++) {
			int startingX = (i + 1) * G.get ().GRID_W / (numPlayers + 1);
			m_players[i].GetComponent<Player>().reset(this, i, new Grid.Gid(startingX, 0));
		}
		
		AnimMaster.delay ("gameStartDelay", this.gameObject, G.get ().GAME_START_DELAY).onComplete("onGameStart");
	}
	
	void onGameStart() {
		Debug.Log ("onGameStart");
		AnimMaster.delay ("gameRoundStartDelay", this.gameObject, G.get ().ROUND_START_DELAY).onComplete("onGameRoundStart");
	}
	
	void onGameRoundCheckSpace() {
		Debug.Log ("onGameRoundCheckSpace");
		AnimMaster.clearWithKey("gameRoundStartDelay");
		for (int i = 0; i < m_players.Length; i++) {
			m_playersCheckSpace[i] = false;
		}
		
		int numPlayersWithValidMoves = 0;
		foreach(GameObject player in m_players) {
			if (player.GetComponent<Player>().onHasValidMoves()) {
				numPlayersWithValidMoves++;
			}
		}
		if (numPlayersWithValidMoves > 1) {
			AnimMaster.delay ("gameRoundStartDelay", this.gameObject, G.get ().ROUND_START_DELAY).onComplete("onGameRoundStart");
		} else {
			AnimMaster.delay ("gameEndDelay", this.gameObject, G.get ().GAME_END_DELAY).onComplete("onGameEnd");
		}
	}
	
	void onGameRoundStart() {
		Debug.Log ("onGameRoundStart");
		AnimMaster.clearWithKey("roundCountDown");
		AnimMaster.clearWithKey("gameRoundStartDelay");
		m_countDownAnim = AnimMaster.delay("roundCountDown", this.gameObject, G.get ().COUNTDOWN_SECONDS).onComplete("onGameRoundEnd");
		for (int i = 0; i < m_players.Length; i++) {
			GameObject player = m_players[i];
			// reset selection done bool
			m_playersSelectionDone[i] = false;
			player.SendMessage("onGameRoundStart");
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
		
		AnimMaster.delay ("gameRoundStartDelay", this.gameObject, G.get ().ROUND_END_DELAY).onComplete ("onGameRoundCheckSpace");
	}

	void onGameEnd() {
		m_scoreStatsPopup.gameObject.SetActive(true);
	}
	
	public void onScoreStatsDone() {
		m_scoreStatsPopup.gameObject.SetActive(false);
		AnimMaster.delay("gameResetDelay", this.gameObject, G.get ().GAME_RESET_DELAY).onComplete("onGameReset");
	}
}
