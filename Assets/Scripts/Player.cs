using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private bool m_acceptInput;
	private int m_playerIndex;
	private Dictionary<Direction, KeyCode> m_controls;
	private List<Grid.Gid> m_gids;
	private List<Direction> m_directions;
	private GameController m_gameController;
	private GameObject m_playerPartTemplate;

	// Called before Start() immediately after Player is created.
	public void init(GameController gameController, int playerIndex, Grid.Gid startGid) {
		this.m_gameController = gameController;
		this.m_acceptInput = false;
		this.m_playerIndex = playerIndex;
		this.m_gids = new List<Grid.Gid>();
		this.m_directions = new List<Direction>();

		this.m_gids.Add(startGid);
		m_gameController.Grid.add(startGid, this.gameObject);
		Debug.Log("player start " + startGid.x + "," + startGid.y +" pos:"+ startGid.worldPos);
		this.transform.localPosition = startGid.worldPos;
		
		
		// for now controls are set up here.
		// TODO move this to be data driven.
		m_controls = new Dictionary<Direction, KeyCode>();
		switch (playerIndex) {
			case 0:
				m_controls.Add(Direction.UP, KeyCode.W);
				m_controls.Add(Direction.LEFT, KeyCode.A);
				m_controls.Add (Direction.DOWN, KeyCode.S);
				m_controls.Add (Direction.RIGHT, KeyCode.D);
				break;
			case 1:
				m_controls.Add(Direction.UP, KeyCode.I);
				m_controls.Add(Direction.LEFT, KeyCode.J);
				m_controls.Add (Direction.DOWN, KeyCode.K);
				m_controls.Add (Direction.RIGHT, KeyCode.L);
				break;
			default: 
				throw new UnityException("player index controls not supported " + playerIndex);
		}
	}

	// Use this for initialization
	void Start () {
		m_playerPartTemplate = GameObject.Find ("Player Part");
	}
	
	// Update is called once per frame
	void Update () {
		if (m_acceptInput) {
			if (Input.GetKeyUp(m_controls[Direction.UP])) {
				chooseDirection(Direction.UP);
			} else if (Input.GetKeyUp(m_controls[Direction.DOWN])) {
				chooseDirection(Direction.DOWN);
			} else if (Input.GetKeyUp(m_controls[Direction.LEFT])) {
				chooseDirection(Direction.LEFT);
			} else if (Input.GetKeyUp(m_controls[Direction.RIGHT])) {
				chooseDirection(Direction.RIGHT);
			}
		}
	}
	
	void onGameRoundStart() {
		m_acceptInput = true;
	}
	
	void chooseDirection(Direction dir) {
		Debug.Log("player " + m_playerIndex + " chose direction " + dir);
		if (m_directions.Count > 0 && isOpposite(m_directions[m_directions.Count - 1], dir)) {
			// invalid move to go backwards, do nothing
			return;
		}
		Grid.Gid prevGid = m_gids[m_gids.Count - 1];
		Grid.Gid newGid;
		switch (dir) {
			case Direction.UP:
				newGid = new Grid.Gid(prevGid.x, prevGid.y - 1);
				break;
			case Direction.DOWN:
				newGid = new Grid.Gid(prevGid.x, prevGid.y + 1);
				break;
			case Direction.LEFT:
				newGid = new Grid.Gid(prevGid.x - 1, prevGid.y);
				break;
			case Direction.RIGHT:
				newGid = new Grid.Gid(prevGid.x + 1, prevGid.y);
				break;
			default:
				throw new UnityException("Unknown dir " + dir);
		}
		if (m_gameController.Grid.outOfBounds(newGid)) {
			// invalid move to the walls.
			return;
		}
		m_gids.Add(newGid);
		GameObject part = Instantiate(m_playerPartTemplate);
		part.transform.parent = this.transform;
		part.transform.localPosition = newGid.worldPos;
		m_gameController.Grid.add(newGid, this.gameObject);
		m_directions.Add(dir);
		
	}
	
	void expand() {
		
	}
	
	private bool isOpposite(Direction prevDir, Direction nextDir) {
		switch (prevDir) {
		case Direction.UP:
			if (nextDir == Direction.DOWN)
				return true;
			break;
		case Direction.DOWN:
			if (nextDir == Direction.UP)
				return true;
			break;
		case Direction.LEFT:
			if (nextDir == Direction.RIGHT)
				return true;
			break;
		case Direction.RIGHT:
			if (nextDir == Direction.LEFT)
				return true;
			break;
		}
		return false;
	}
}
