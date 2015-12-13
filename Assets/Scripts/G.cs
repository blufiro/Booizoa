using System;
using System.Collections.Generic;

public class G
{
	public int SCREEN_W = 640;
	public int SCREEN_H = 360;
	public int GRID_SIZE = 16;
	public int GRID_STEP_MULT = 2;
	public float COUNTDOWN_SECONDS = 30;
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

