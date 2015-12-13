using UnityEngine;

/// <summary>
/// Class to store all the game objects in the game grid.
/// </summary>
public class Grid
{
	// TODO maybe use a GameState class instead?
	private GameObject[,] m_gridObjects;
	// for convenience out of bounds checking
	private int m_width;
	private int m_height;
	
	public Grid (int w, int h)
	{
		m_width = w;
		m_height = h;
		m_gridObjects = new GameObject[w, h];
		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				m_gridObjects[x, y] = null;
			}
		}
	}
	
	public void add(Gid gid, GameObject gob) {
		m_gridObjects[gid.x, gid.y] = gob;
	}
	
	public GameObject get(Gid gid) {
		return m_gridObjects[gid.x, gid.y];
	}
	
	public bool exists(Gid gid) {
		return (m_gridObjects[gid.x, gid.y] != null);
	}
	
	public bool outOfBounds(Gid gid) {
		return (gid.x < 0 
			|| gid.y < 0
			|| gid.x >= m_width
			|| gid.y >= m_height);
	}
	
	public class Gid {
		private int m_x;
		private int m_y;
		
		public int x {
			get { return m_x; }
		}
		
		public int y {
			get { return m_y; }
		}
		
		public Vector2 gridWorldPos {
			get { return new Vector2(m_x, m_y); }
		}

		public Gid(int gridX, int gridY) {
			this.m_x = gridX;
			this.m_y = gridY;
		}
		
		public void sub(Gid otherGid) {
			this.m_x -= otherGid.x;
			this.m_y -= otherGid.y;
		}

		public void plus (int x, int y)
		{
			this.m_x += x;
			this.m_y += y;
		}
		
		public static Gid clone(Gid gid)
		{
			return new Gid(gid.x, gid.y);
		}

		public Vector2 getGridWorldPosCenter(int tileSize) {
			return new Vector2(m_x + tileSize, m_y + tileSize);
		}

		public override string ToString ()
		{
			return string.Format ("[Gid: x={0}, y={1}]", x, y);
		}
//		public static Gid fromScreen(Vector2 position) {
//			return new Gid(
//				(int) (position.x / G.get ().GRID_SIZE),
//				(int) (position.y / G.get ().GRID_SIZE));
//		}
	}
}
