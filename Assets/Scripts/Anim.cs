using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Anim
{
	public delegate void CompleteDelegate();

	private float m_timeElapsedSeconds = 0.0f;
	private string m_onComplete = null;
	private object m_onCompleteParams = null;
	private CompleteDelegate m_onCompleteDelegate;
	private float m_delaySeconds = 0.0f;
	private GameObject m_target = null;
	private string m_key = "";
	
	public Anim delay(float delaySeconds) {
		this.m_delaySeconds = delaySeconds;
		return this;
	}
	
	public Anim onComplete(string onComplete) {
		this.m_onComplete = onComplete;
		return this;
	}
	public Anim onCompleteParams(object onCompleteParams) {
		this.m_onCompleteParams = onCompleteParams;
		return this;
	}
	public Anim onCompleteDelegate(CompleteDelegate onComplete) {
		this.m_onCompleteDelegate += onComplete;
		return this;
	}

	/// <summary>
	/// Elapse the animation time and play it. Do not call this manually.
	/// </summary>
	public void elapse(float elapsedSeconds) {
		m_timeElapsedSeconds += elapsedSeconds;
		if (isOver()) {
			if (m_onComplete != null && m_target != null) {
				m_target.SendMessage (m_onComplete, m_onCompleteParams, SendMessageOptions.RequireReceiver);
			}
			if (m_onCompleteDelegate != null) {
				m_onCompleteDelegate();
			}
		}
	}
	
	public bool isOver() {
		return m_timeElapsedSeconds >= m_delaySeconds;
	}
	
	public float getTimeElapsedSeconds() {
		return m_timeElapsedSeconds;
	}
	
	public float getTimeRemainingSeconds() {
		return m_delaySeconds - m_timeElapsedSeconds;
	}

	public GameObject getTarget() {
		return m_target;
	}
	
	public string getKey() {
		return m_key;
	}

	/// <summary>
	/// Creates a new Anim object. Do not call this manually, use AnimMaster instead.
	/// </summary>
	public Anim (GameObject target, string key)
	{
		this.m_target = target;
		this.m_key = key;
	}
}
