using System;
using System.Collections.Generic;

public class G
{
	public int MAX_PLAYERS = 4;
	public int SCREEN_W = 640;
	public int SCREEN_H = 360;
	public int GRID_SIZE = 16;
	public int GRID_STEP_MULT = 2;
	public float GAME_START_DELAY = 0.5f;
	public float GAME_END_DELAY = 0.2f;
	public float GAME_RESET_DELAY = 0.5f;
	public float ROUND_START_DELAY = 0.5f;
	public float ROUND_END_DELAY = 0.5f;
	public float COUNTDOWN_SECONDS = 3.0f;
	public float PLAYER_END_ANIM_INIT_DELAY = 0.2f;
	public float PLAYER_END_ANIM_DELAY = 0.05f;
	public float PLAYER_END_ANIM_END_DELAY = 0.5f;
	public float PLAYER_MOVE_ANIM_DURATION = 1.0f;
	public float AXIS_THRESHOLD = 0.9f;
	public int GRID_W { get { return SCREEN_W / GRID_SIZE; } }
	public int GRID_H { get { return SCREEN_H / GRID_SIZE; } }
	
	// For passing the players that joined between the title screen and the game screen
	public List<int> players = new List<int>();

	public static G get() {
		if (instance == null) { instance = new G(); }
		return instance;
	}
	
	private static G instance = null;
	private G () {}
}

