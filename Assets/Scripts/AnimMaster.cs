using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Singleton animation master
 */
public class AnimMaster
{
	public List<Anim> anims;
	
	private static Anim createAnim(GameObject gob, string animKey) {
		Anim anim = new Anim(gob, animKey);
		get().anims.Add(anim);
		return anim;
	}
	
	/// <summary>
	/// Clears all animations with the given key.
	/// </summary>
	/// <param name="animKey">Animation key.</param>
	public static void clearWithKey(string animKey) {
		if (animKey != "") {
			for (int i=get().anims.Count-1; i>=0; i--) {
				Anim anim = get().anims[i];
				if (anim.getKey() == animKey) {
					get().anims.Remove(anim);
				}
			}
		}
	}
	
	/// <summary>
	/// Create animation with a delay.
	/// </summary>
	/// <param name="animKey">Animation key. Animations the same key can be removed with clearWithKey method.</param>
	/// <param name="gob">Game Object to target</param>
	/// <param name="delaySeconds">Delay in seconds.</param>
	public static Anim delay(string animKey, GameObject gob, float delaySeconds) {
		return createAnim(gob, animKey).delay(delaySeconds);
	}
	
	// Update the animations every frame
	public void update() {
		float deltaTime = Time.deltaTime;
		for (int i = anims.Count-1; i >= 0; i--) {
			Anim anim = anims[i];
			anim.elapse(deltaTime);
			if (anim.isOver()) {
				anims.RemoveAt (i);
			}
		}
	}

	private static AnimMaster m_instance = new AnimMaster();
	public static AnimMaster get() {
		return m_instance;
	}

	private AnimMaster ()
	{
		anims = new List<Anim>();
	}
}

