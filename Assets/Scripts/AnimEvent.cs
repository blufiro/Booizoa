using UnityEngine;
using System.Collections;

public class AnimEvent : MonoBehaviour {

	public AudioClip sfx_cat_meow;
	public AudioClip sfx_player_win;
	public AudioClip sfx_player_lose;
	public AudioClip sfx_player_move;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySoundCatOk()
	{
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_cat_meow;
		audio.Play();
	}

	public void PlaySoundCatMove()
	{
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_player_move;
		audio.Play();
	}

	public void PlaySoundCatWin()
	{
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_player_win;
		audio.Play();
	}

	public void PlaySoundCatLose()
	{
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = sfx_player_lose;
		audio.Play();
	}
}
