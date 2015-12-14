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
	public Sprite playerLostHeadSprite;
	public Sprite playerLostLeftRotationSprite;
	public Sprite playerLostUpSprite;
	public Sprite playerWonHeadSprite;
	public Sprite playerWonLeftRotationSprite;
	public Sprite playerWonUpSprite;
	public Sprite playerNormalHeadSprite;
	public Text	playerScoreStatsTextTemplate;
	public Text okTextTemplate;

	public Grid Grid
	{
		get { return m_grid; }
	}

	private GameObject[] m_players;
	private Text[] m_playersSelectionDoneText;
	private bool[] m_playersCheckSpace;
	private bool[] m_playersEndDone;
	private Grid m_grid;
	private Anim m_countDownAnim;
	private GameObject m_canvasGob;
	private Text m_timeLeftText;
	private Text m_goText;
	private Text m_debugText;
	private Image m_scoreStatsPopup;
	private List<Text> m_playerScoresTexts;
	private List<int> m_winningPlayerIndices;
	
	// Use this for initialization
	void Start () {
		m_canvasGob = GameObject.Find ("Canvas");
		m_timeLeftText = GameObject.Find ("Time Left Text").GetComponent<Text>();
		m_goText = GameObject.Find ("Go Text").GetComponent<Text>();
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
		m_playersSelectionDoneText = new Text[numPlayers];
		m_playersCheckSpace = new bool[numPlayers];
		m_playersEndDone = new bool[numPlayers];
		m_playerScoresTexts = new List<Text>();
		m_winningPlayerIndices = new List<int>();
		for (int i = 0; i < numPlayers; i++) {
			Debug.Log ("Creating Player " + i);
			m_players[i] = Instantiate(playerTemplate);
			m_players[i].transform.parent = gridWorld.transform;
			
			Text playerOkText = Instantiate(okTextTemplate) as Text;
			playerOkText.transform.SetParent(m_canvasGob.transform, false);
			m_playersSelectionDoneText[i] = playerOkText;
			playerOkText.gameObject.SetActive(false);
			
			Text playerScoreStatsText = Instantiate(playerScoreStatsTextTemplate) as Text;
			playerScoreStatsText.transform.SetParent(m_scoreStatsPopup.transform, false);
			playerScoreStatsText.text = "Player " + i;
			playerScoreStatsText.rectTransform.sizeDelta = new Vector2(m_scoreStatsPopup.rectTransform.rect.width / numPlayers, playerScoreStatsText.rectTransform.rect.height);
			playerScoreStatsText.transform.localPosition = new Vector2(
				(m_scoreStatsPopup.rectTransform.rect.width * i / numPlayers) + m_scoreStatsPopup.rectTransform.rect.x,
				playerScoreStatsText.transform.localPosition.y);
			m_playerScoresTexts.Add(playerScoreStatsText);
		}
		
		// hide stuff
		m_goText.gameObject.SetActive(false);
		m_scoreStatsPopup.gameObject.SetActive(false);
		
		AnimMaster.delay ("gameStartDelay", this.gameObject, G.get ().GAME_RESET_DELAY).onComplete("onGameReset");
	}
	
	// Update is called once per frame
	void Update () {
		AnimMaster.get ().update();
		
		if (m_countDownAnim != null) {
			m_timeLeftText.text = "Time Left: " + m_countDownAnim.getTimeRemainingSeconds().ToString ("F1") + "s";
		}
		
		if (Input.GetButton("Back")) {
			Debug.Log ("Back to menu");
			Application.LoadLevel ("main_scene");
		}
		
//		string debugText = " Debug Text \n";
//		debugText += "\n" + Input.GetAxis ("HorizontalAxis0");
//		debugText += "\n" + Input.GetAxis ("VerticalAxis0");
//		m_debugText.text = debugText;
	}
	
	void onGameReset() {
		Debug.Log ("onGameReset");
		
		m_goText.gameObject.SetActive(false);
		m_scoreStatsPopup.gameObject.SetActive(false);
		
		if (m_grid != null) {
			m_grid.clear();
		}
		
		// Create grid
		m_grid = new Grid(G.get ().GRID_W, G.get ().GRID_H);
		
		// The middle is actually 1 more grid away since the player graphic takes up 2 tiles.
		Grid.Gid playerCenterGid = new Grid.Gid (1, 0);
		// Initialize players
		int numPlayers = m_players.Length;
		for (int i = 0; i < numPlayers; i++) {
			int startingX = ((i + 1) * G.get ().GRID_W / (numPlayers + 1)) / 2 * 2;
			Grid.Gid playerStartGid = new Grid.Gid(startingX, 0);
			m_players[i].GetComponent<Player>().reset(this, i, playerStartGid);
			m_playersSelectionDoneText[i].transform.position = m_players[i].transform.TransformPoint(playerCenterGid.gridWorldPos);
			m_playersEndDone[i] = false;
		}
		onGameStart();
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
			m_playersSelectionDoneText[i].gameObject.SetActive(false);
		}
		
		m_winningPlayerIndices.Clear();
		foreach(GameObject player in m_players) {
			Player playerComp = player.GetComponent<Player>(); 
			if (playerComp.onHasValidMoves()) {
				m_winningPlayerIndices.Add (playerComp.getPlayerIndex());
			}
		}
		if (m_winningPlayerIndices.Count > 1) {
			m_goText.gameObject.SetActive(true);
			AnimMaster.delay ("gameRoundStartDelay", this.gameObject, G.get ().ROUND_START_DELAY).onComplete("onGameRoundStart");
		} else {
			// There might be 0 or 1 winning players at this point.
			// If there are no players with valid moves, there can be a tie with multiple players whichever are still alive.
			if (m_winningPlayerIndices.Count == 0) {
				foreach(GameObject player in m_players) {
					Player playerComp = player.GetComponent<Player>(); 
					if (!playerComp.getIsDead()) {
						m_winningPlayerIndices.Add (playerComp.getPlayerIndex());
					}
				}
			}
			AnimMaster.delay ("gameEndDelay", this.gameObject, G.get ().GAME_END_DELAY).onComplete("onGameEnd");
		}
	}
	
	void onGameRoundStart() {
		Debug.Log ("onGameRoundStart");
		m_goText.gameObject.SetActive(false);
		AnimMaster.clearWithKey("roundCountDown");
		AnimMaster.clearWithKey("gameRoundStartDelay");
		m_countDownAnim = AnimMaster.delay("roundCountDown", this.gameObject, G.get ().COUNTDOWN_SECONDS).onComplete("onGameRoundEnd");
		for (int i = 0; i < m_players.Length; i++) {
			GameObject player = m_players[i];
			// reset selection done bool
			m_playersSelectionDoneText[i].gameObject.SetActive(false);
			player.SendMessage("onGameRoundStart");
		}
	}

	public void onPlayerSelectionDone(int playerIndex) {
		Debug.Log ("onPlayerSelectionDone " + playerIndex);
		m_playersSelectionDoneText[playerIndex].gameObject.SetActive(true);
		
		// if all players are done, proceed to round end.
		foreach (Text done in m_playersSelectionDoneText) {
			if (!done.IsActive()) {
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
		Debug.Log ("onGameEnd");
		for (int i = 0; i < m_players.Length; i++) {
			if (m_winningPlayerIndices.Contains (i)) {
				m_players[i].SendMessage ("onGameWon");
			} else {
				m_players[i].SendMessage ("onGameLost");
			}
		}
	}
	
	void onGameEndAnimDone(int playerIndex) {
		Debug.Log ("onGameEndAnimDone " + playerIndex);
		m_playersEndDone[playerIndex] = true;
		
		// if all players are done, proceed to round end.
		foreach (bool done in m_playersEndDone) {
			if (!done) {
				return;
			}
		}
		onGameShowScores();
	}
	void onGameShowScores() {
		for (int i = 0; i < m_players.Length; i++) {
			Player playerComp = m_players[i].GetComponent<Player>();
			m_playerScoresTexts[i].text =
				"Player " + playerComp.getPlayerIndex() 
				+ "\n\n\n"
				+ "\nSteps: " + playerComp.getNumSteps();
		}
		m_scoreStatsPopup.gameObject.SetActive(true);
	}
	
	public void onScoreStatsDone() {
		m_scoreStatsPopup.gameObject.SetActive(false);
		AnimMaster.delay("gameResetDelay", this.gameObject, G.get ().GAME_RESET_DELAY).onComplete("onGameReset");
	}
}
