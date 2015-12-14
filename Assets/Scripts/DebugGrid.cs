using UnityEngine;
using System.Collections;

public class DebugGrid : MonoBehaviour {

	public float offsetX = 0;
	public float offsetY = 0;
	public float width = G.get ().GRID_SIZE;
	public float height = G.get ().GRID_SIZE;
	public Texture occupiedTexture;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos()
	{
		Vector3 pos = Camera.current.transform.position;

		int halfScreenH = G.get ().SCREEN_H / 2;
		int index = 0;
		for (float y = pos.y - halfScreenH; y < pos.y + halfScreenH; y+= height)
		{
			Gizmos.color = (index % 10 == 0) ? Color.cyan : (index % 5 == 0) ? Color.white : Color.gray;
			Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/height) * height + offsetY, 0.0f),
			                new Vector3(1000000.0f, Mathf.Floor(y/height) * height + offsetY, 0.0f));
			index++;
		}

		int halfScreenW = G.get ().SCREEN_W / 2;
		index = 0;
		for (float x = pos.x - halfScreenW; x < pos.x + halfScreenW; x+= width)
		{
			Gizmos.color = (index % 10 == 0) ? Color.cyan : (index % 5 == 0) ? Color.white : Color.gray;
			Gizmos.DrawLine(new Vector3(Mathf.Floor(x/width) * width + offsetX, -1000000.0f, 0.0f),
			                new Vector3(Mathf.Floor(x/width) * width + offsetX, 1000000.0f, 0.0f));
			index++;
		}
		
		Grid grid = gameObject.GetComponent<GameController>().Grid;
		if (grid != null) {
			int gy = 0;
			for (float y = pos.y - halfScreenH; y < pos.y + halfScreenH; y+= height)
			{
				int gx = 0;
				for (float x = pos.x - halfScreenW; x < pos.x + halfScreenW; x+= width)
				{
					Grid.Gid gid = new Grid.Gid(gx, gy);
					if (!grid.outOfBounds(gid) && grid.exists(gid)) {
						Debug.Log ("grid occ:" + gx +"," + gy);
						Gizmos.DrawGUITexture(new Rect(x + offsetX, y + offsetY, width, height), occupiedTexture);
					}
					gx++;
				}
				gy++;
			}
		}
				
	}
}
