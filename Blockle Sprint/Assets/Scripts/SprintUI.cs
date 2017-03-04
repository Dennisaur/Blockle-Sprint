using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintUI : MonoBehaviour {
	public UnityEngine.UI.Text bestTime;
	public UnityEngine.UI.Text timeElapsed;
	public UnityEngine.UI.Text linesLeft;
	public bool isLandscape;

	// Use this for initialization
	void Start () {
		UpdateBestTime(PlayerPrefs.GetFloat ("Best Time", 0));
	}

	// Update is called once per frame
	void Update () {
		if (isLandscape) {
			linesLeft.text = "Lines left: " + TetrisManager.linesRemaining;
		} else {
			linesLeft.text = "Lines\n" + TetrisManager.linesRemaining;
		}
		timeElapsed.text = TimeToString(TetrisManager.timeElapsed);
	}

	public void UpdateBestTime(float time) {
		if (isLandscape) {
			bestTime.text = "Best time:\n" + TimeToString (time);
		} else {
			bestTime.text = "Best time: " + TimeToString (time);
		}
	}

	/// <summary>
	/// Converts time in milliseconds to a readable string
	/// </summary>
	/// <returns>"<HH:>mm:ss.ff" formatted string.</returns>
	/// <param name="time">Time in milliseconds.</param>
	string TimeToString(float time) {
		int hours = Mathf.FloorToInt (time / 3600);
		int minutes = Mathf.FloorToInt (time % 3600 / 60);
		int seconds = Mathf.FloorToInt (time % 60);
		int milliseconds = (int)((time - (seconds) - (minutes * 60) - (hours * 60 * 60)) * 1000);
		System.DateTime dt = new System.DateTime (1, 1, 1, hours, minutes, seconds, milliseconds);
		if (hours > 0) {
			return dt.ToString ("HH:mm:ss.ff");
		} else {
			return dt.ToString ("mm:ss.ff");
		}
	}
}
