using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public enum BodyPartType {
		BODYPART_NONE,
		BODYPART_UP,
		BODYPART_ROTATE,
	}
	
	public int tileSize;
	public BodyPartType bodyPartType;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
