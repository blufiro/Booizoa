using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private static Vector3 ROTATION_AXIS = new Vector3(0,0,1);

	public bool acceptInput { get { return m_acceptInput; }}
	private bool m_acceptInput;
	private int m_playerIndex;
	private Dictionary<Direction, KeyCode> m_controls;
	private List<Grid.Gid> m_gidHistory; // may not contain all the gids the player takes up since the sprites take up 2x2 spaces.
	private List<Direction> m_directions;
	private List<GameObject> m_sprites;
	private GameController m_gameController;
	private GameObject m_playerHead;
	private Direction m_chosenDir;
	private Grid.Gid m_chosenGid;

	// Use this for initialization
	void Start () {
		m_playerHead = this.transform.FindChild("Player Sprite Head").gameObject;
	}
	
	// Must be called after Start() so we have a reference to child objects.
	public void reset(GameController gameController, int playerIndex, Grid.Gid startGid) {
		this.m_gameController = gameController;
		this.m_acceptInput = false;
		this.m_playerIndex = playerIndex;
		
		if (m_sprites != null) {
			foreach (GameObject sprite in m_sprites) {
				Destroy(sprite);
			}
		}
		
		this.m_gidHistory = new List<Grid.Gid>();
		this.m_directions = new List<Direction>();
		this.m_sprites = new List<GameObject>();
		
		this.m_directions.Add(Direction.UP);
		addToGrid (startGid, m_playerHead, false /* Don't add this.gameObject to m_sprites as we don't want this to destroy itself. */);
		Debug.Log("player " + m_playerIndex + " start " + startGid.x + "," + startGid.y);
		this.transform.localPosition = startGid.gridWorldPos;
		this.transform.localScale = new Vector2(1, 1);
		
		// reset the head position
		int headSize = m_playerHead.GetComponent<Tile>().tileSize;
		Grid.Gid headGid = new Grid.Gid(0,0);
		int offsetCenter = headSize / 2;
		m_playerHead.transform.localPosition = headGid.getGridWorldPosCenter(offsetCenter);
		m_playerHead.transform.localRotation = Quaternion.identity;
		m_chosenDir = Direction.UP;
		
		// reset head sprite. We shouldn't have any body parts now.
		m_playerHead.GetComponent<SpriteRenderer>().sprite = m_gameController.playerNormalHeadSprite;
				
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
		Debug.Log ("player " + m_playerIndex + " onGameRoundStart");
		m_acceptInput = true;
		// Default the chosen gid, player may overwrite but if there was no input, they would execute this.
		m_chosenGid = getNextGid(m_directions[m_directions.Count - 1]);
	}
	
	public bool onHasValidMoves() {
		bool hasFreeSpace = false;
		foreach (Direction dir in Direction.GetValues(typeof(Direction))) {
			if (m_gameController.Grid.getStatus(getNextGid(dir)) == Grid.GidStatus.FREE) {
				hasFreeSpace = true;
				break;
			}
		}
		return hasFreeSpace;
	}
	
	public int getPlayerIndex() {
		return m_playerIndex;
	}
	
	void chooseDirection(Direction dir) {
		Debug.Log("player " + m_playerIndex + " chose direction " + dir);
		Direction lastDirection = m_directions[m_directions.Count - 1];
		if (m_directions.Count > 0 && isOpposite(lastDirection, dir)) {
			Debug.Log("player " + m_playerIndex + " tried to move backwards " + dir);
			// invalid move to go backwards, do nothing
			return;
		}
		
		Grid.Gid nextGid = getNextGid(dir);
		Grid.Gid[] gids = getGidsForTile(nextGid, m_playerHead.GetComponent<Tile>());
		foreach (Grid.Gid gid in gids) {
			switch (m_gameController.Grid.getStatus(gid)) {
			case Grid.GidStatus.OUT_OF_BOUNDS:
				Debug.Log("player " + m_playerIndex + " tried to move out of bounds " + gid + " " + dir);
				// invalid move to the walls.
				return;
			case Grid.GidStatus.OCCUPIED:
				Debug.Log ("player " + m_playerIndex + " tried to move into another occupied cell.");
				// invalid move into occupied cells
				return;
			}
		}
		
		m_chosenDir = dir;
		m_chosenGid = nextGid;
		
		m_gameController.onPlayerSelectionDone(m_playerIndex);
		m_acceptInput = false;
	}
	
	// Chosen direction is always set, so the player will always execute the previous move even if there was no input.
	void onExecuteChosenDirection() {
		Grid.Gid nextGid = getNextGid(m_chosenDir);
		Grid.Gid[] gids = getGidsForTile(nextGid, m_playerHead.GetComponent<Tile>());
		foreach (Grid.Gid gid in gids) {
			if (m_gameController.Grid.getStatus(gid) != Grid.GidStatus.FREE) {
				Debug.Log ("Not executing chosen direction " + m_chosenDir + " on player " + m_playerIndex);
				return;
			}
		}
		Direction lastDirection = m_directions[m_directions.Count - 1];
		m_directions.Add(m_chosenDir);

		// Add a new body part where the head is
		GameObject nextBodyPart = getNextBodyPart(lastDirection, m_chosenDir, m_playerHead.transform.localPosition);
		addToGrid(m_chosenGid, nextBodyPart, true);
		
		// Move head
		Grid.Gid headGid = Grid.Gid.clone(m_chosenGid);
		// head should be relative to player start since the parent is the same.
		headGid.sub(m_gidHistory[0]);
		int offsetForCenter = nextBodyPart.GetComponent<Tile>().tileSize / 2;
		m_playerHead.transform.localPosition = headGid.getGridWorldPosCenter(offsetForCenter);
		m_playerHead.transform.localRotation = Quaternion.AngleAxis(getAngle(m_chosenDir), ROTATION_AXIS);
		
		m_acceptInput = false;
	}
	
	void onGameWon() {
		swapTextures(
			m_gameController.playerWonHeadSprite,
			m_gameController.playerWonUpSprite,
			m_gameController.playerWonLeftRotationSprite);
	}
	
	void onGameLost() {
		swapTextures(
			m_gameController.playerLostHeadSprite,
			m_gameController.playerLostUpSprite,
			m_gameController.playerLostLeftRotationSprite);
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
	
	private GameObject getNextBodyPart(Direction prevDir, Direction nextDir, Vector3 newPosition) {
		GameObject nextBodyPart;
		switch (prevDir) {
		case Direction.UP:
			switch (nextDir) {
			case Direction.UP:
				nextBodyPart = Instantiate(m_gameController.playerUpTemplate);
				break;
			case Direction.LEFT:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				break;
			case Direction.RIGHT:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.DOWN:
			switch (nextDir) {
			case Direction.DOWN:
				nextBodyPart = Instantiate(m_gameController.playerUpTemplate);
				break;
			case Direction.LEFT:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, -90.0f, Space.Self);
				break;
			case Direction.RIGHT:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 180.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.LEFT:
			switch (nextDir) {
			case Direction.UP:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 180.0f, Space.Self);
				break;
			case Direction.DOWN:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 90.0f, Space.Self);
				break;
			case Direction.LEFT:
				nextBodyPart = Instantiate(m_gameController.playerUpTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		case Direction.RIGHT:
			switch (nextDir) {
			case Direction.UP:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, -90.0f, Space.Self);
				break;
			case Direction.DOWN:
				nextBodyPart = Instantiate(m_gameController.playerLeftRotationTemplate);
				break;
			case Direction.RIGHT:
				nextBodyPart = Instantiate(m_gameController.playerUpTemplate);
				nextBodyPart.transform.Rotate (ROTATION_AXIS, 90.0f, Space.Self);
				break;
			default:
				throw new UnityException("Illegal next dir " + nextDir + " from prev dir " + prevDir);
			}
			break;
		default:
			throw new UnityException("Unknown prev dir " + prevDir);
		}
		nextBodyPart.transform.parent = this.transform;
		nextBodyPart.transform.localScale = m_playerHead.transform.localScale;
		nextBodyPart.transform.localPosition = newPosition;
		return nextBodyPart;
	}
	
	Grid.Gid getNextGid(Direction dir) {
		Grid.Gid prevGid = m_gidHistory[m_gidHistory.Count - 1];
		// TODO should this size be the size of the piece we're going to place?
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
		return nextGid;
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

	private void addToGrid (Grid.Gid nextGid, GameObject nextBodyPart, bool isBody)
	{
		m_gidHistory.Add(nextGid);
		if (isBody) {
			m_sprites.Add(nextBodyPart);
		}
		
		// each sprite can spread over a few grid cells
		Grid.Gid[] gids = getGidsForTile(nextGid, nextBodyPart.GetComponent<Tile>());
		foreach  (Grid.Gid gid in gids) {
			m_gameController.Grid.add(gid, nextBodyPart);
		}
	}
	
	private Grid.Gid[] getGidsForTile(Grid.Gid firstGid, Tile tile) {
		int tileSize = tile.tileSize;
		Grid.Gid[] gids = new Grid.Gid[tileSize * tileSize];
		int gidIndex = 0;
		for (int x = 0; x < tileSize; x++) {
			for (int y = 0; y < tileSize; y++) {
				Grid.Gid gid = Grid.Gid.clone (firstGid);
				gid.plus(x, y);
				gids[gidIndex++] = gid;
			}
		}
		return gids;
	}
	
	private void swapTextures(Sprite head, Sprite up, Sprite leftRotation) {
		float delaySeconds = G.get ().PLAYER_END_ANIM_INIT_DELAY;
		foreach (GameObject bodyPart in m_sprites) {
			GameObject closurebodyPart = bodyPart;
			Tile.BodyPartType bodyPartType = bodyPart.GetComponent<Tile>().bodyPartType;
			switch (bodyPartType) {
			case Tile.BodyPartType.BODYPART_UP:
				AnimMaster.delay ("player"+m_playerIndex+"_SwapSpriteUp", this.gameObject, delaySeconds)
					.onCompleteDelegate(() =>  { closurebodyPart.GetComponent<SpriteRenderer>().sprite = up; });
				break;
			case Tile.BodyPartType.BODYPART_ROTATE:
				AnimMaster.delay ("player"+m_playerIndex+"_SwapSpriteRotation", this.gameObject, delaySeconds)
					.onCompleteDelegate(() =>  { closurebodyPart.GetComponent<SpriteRenderer>().sprite = leftRotation; });
				break;
			default:
				throw new UnityException("Unknown body part type : " + bodyPartType);
			}
			delaySeconds += G.get ().PLAYER_END_ANIM_DELAY;
		}
		AnimMaster.delay ("player"+m_playerIndex+"_SwapSpriteRotation", this.gameObject, delaySeconds)
			.onCompleteDelegate(() =>  { m_playerHead.GetComponent<SpriteRenderer>().sprite = head; });
		delaySeconds += G.get ().PLAYER_END_ANIM_END_DELAY;
		AnimMaster.delay ("player"+m_playerIndex+"_SwapAnimDone", m_gameController.gameObject, delaySeconds)
			.onComplete("onGameEndAnimDone").onCompleteParams(m_playerIndex);
	}
}
