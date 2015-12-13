using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private bool m_acceptInput;
	private int m_playerIndex;
	private Dictionary<Direction, KeyCode> m_controls;
	private List<Grid.Gid> m_gidHistory; // may not contain all the gids the player takes up since the sprites take up 2x2 spaces.
	private List<Direction> m_directions;
	private GameController m_gameController;
	private GameObject m_playerHead;

	// Called before Start() immediately after Player is created.
	public void init(GameController gameController, int playerIndex, Grid.Gid startGid) {
		this.m_gameController = gameController;
		this.m_acceptInput = false;
		this.m_playerIndex = playerIndex;
		this.m_gidHistory = new List<Grid.Gid>();
		this.m_directions = new List<Direction>();

		this.m_gidHistory.Add(startGid);
		this.m_directions.Add(Direction.UP);
		m_gameController.Grid.add(startGid, this.gameObject);
		Debug.Log("player start " + startGid.x + "," + startGid.y);
		this.transform.localPosition = startGid.gridWorldPosCenter;
		this.transform.localScale = new Vector2(1, 1);
		
		
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
		m_playerHead = this.transform.FindChild("Player Sprite Head").gameObject;
		
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
		Debug.Log ("player onGameRoundStart");
		m_acceptInput = true;
	}
	
	void chooseDirection(Direction dir) {
		Debug.Log("player " + m_playerIndex + " chose direction " + dir);
		Direction lastDirection = m_directions[m_directions.Count - 1];
		if (m_directions.Count > 0 && isOpposite(lastDirection, dir)) {
			Debug.Log("player " + m_playerIndex + " tried to move backwards " + dir);
			// invalid move to go backwards, do nothing
			return;
		}
		Grid.Gid prevGid = m_gidHistory[m_gidHistory.Count - 1];
		int spriteSize = m_playerHead.GetComponent<Tile>().tileSize;
		Grid.Gid nextGid;
		switch (dir) {
			case Direction.UP:
			nextGid = new Grid.Gid(prevGid.x, prevGid.y + spriteSize);
				break;
			case Direction.DOWN:
			nextGid = new Grid.Gid(prevGid.x, prevGid.y - spriteSize);
				break;
			case Direction.LEFT:
			nextGid = new Grid.Gid(prevGid.x - spriteSize, prevGid.y);
				break;
			case Direction.RIGHT:
			nextGid = new Grid.Gid(prevGid.x + spriteSize, prevGid.y);
				break;
			default:
				throw new UnityException("Unknown dir " + dir);
		}
		if (m_gameController.Grid.outOfBounds(nextGid)) {
			Debug.Log("player " + m_playerIndex + " tried to move out of bounds " + nextGid + " " + dir);
			// invalid move to the walls.
			return;
		}
		if (m_gameController.Grid.exists (nextGid)) {
			Debug.Log ("player " + m_playerIndex + " tried to move into another occupied cell.");
			// invalid move into occupied cells
			return;
		}
		m_directions.Add(dir);
		
		// Add a new body part where the head is
		GameObject nextBodyPart = getNextSprite(lastDirection, dir, m_playerHead.transform.localPosition);
		addToGrid(nextGid, nextBodyPart);
		
		// Move head
		Grid.Gid headGid = Grid.Gid.clone(nextGid);
		headGid.sub(m_gidHistory[0]);
		m_playerHead.transform.localPosition = headGid.gridWorldPosCenter;
		m_playerHead.transform.localRotation = Quaternion.AngleAxis(getAngle(dir), new Vector3(0, 0, 1));
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
	
	private GameObject getNextSprite(Direction prevDir, Direction nextDir, Vector3 newPosition) {
		GameObject nextSprite;
		switch (prevDir) {
		case Direction.UP:
			switch (nextDir) {
			case Direction.UP:
				nextSprite = Instantiate(m_gameController.playerUpTemplate);
				break;
			case Direction.LEFT:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				break;
			case Direction.RIGHT:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.DOWN:
			switch (nextDir) {
			case Direction.DOWN:
				nextSprite = Instantiate(m_gameController.playerUpTemplate);
				break;
			case Direction.LEFT:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), -90.0f, Space.Self);
				break;
			case Direction.RIGHT:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 180.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.LEFT:
			switch (nextDir) {
			case Direction.UP:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 180.0f, Space.Self);
				break;
			case Direction.DOWN:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 90.0f, Space.Self);
				break;
			case Direction.LEFT:
				nextSprite = Instantiate(m_gameController.playerUpTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.RIGHT:
			switch (nextDir) {
			case Direction.UP:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), -90.0f, Space.Self);
				break;
			case Direction.DOWN:
				nextSprite = Instantiate(m_gameController.playerLeftRotationTemplate);
				break;
			case Direction.RIGHT:
				nextSprite = Instantiate(m_gameController.playerUpTemplate);
				nextSprite.transform.Rotate (new Vector3(0, 0, 1), 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		default:
			throw new UnityException("Unknown prev dir " + prevDir);
		}
		nextSprite.transform.parent = this.transform;
		nextSprite.transform.localScale = m_playerHead.transform.localScale;
		nextSprite.transform.localPosition = newPosition;
		return nextSprite;
	}
	
	// assumes the up is 0.
	private float getAngle(Direction dir) {
		switch (dir) {
		case Direction.UP:
			return 0;
		case Direction.DOWN:
			return 180;
		case Direction.LEFT:
			return 90;
		case Direction.RIGHT:
			return -90;
		default:
			throw new UnityException("Unknown dir " + dir);
		}
	}

	private void addToGrid (Grid.Gid nextGid, GameObject nextBodyPart)
	{
		m_gidHistory.Add(nextGid);
		
		// each sprite can spread over a few grid cells
		int tileSize = nextBodyPart.GetComponent<Tile>().tileSize;
		for (int x = 0; x < tileSize; x++) {
			for (int y = 0; y < tileSize; y++) {
				Grid.Gid gid = Grid.Gid.clone (nextGid);
				gid.plus(x, y);
				m_gameController.Grid.add(gid, nextBodyPart);
			}
		}
	}
}
