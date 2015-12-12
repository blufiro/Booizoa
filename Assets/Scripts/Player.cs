using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	List<Vector2> cellPositions;

	// Use this for initialization
	void Start () {
		cellPositions = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void ChooseDirection(Direction dir) {
		switch (dir) {
			case Direction.UP:
				cellPositions.Add(new Vector2(0, -1));
				break;
			case Direction.DOWN:
				cellPositions.Add(new Vector2(0, 1));
				break;
			case Direction.LEFT:
				cellPositions.Add(new Vector2(-1, 0));
				break;
			case Direction.RIGHT:
				cellPositions.Add(new Vector2(1, 0));
				break;
		}
	}
	
	void Expand() {
		
	}
}
