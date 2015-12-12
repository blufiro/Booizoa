using System;
using System.Collections.Generic;

public class G
{
	public int GRID_W = 80;
	public int GRID_H = 45;
	public int MAX_PLAYERS = 4;

	public List<int> players;

	public static G get() {
		if (instance == null) { instance = new G(); }
		return instance;
	}
	
	private static G instance = null;
	private G () {}
}

