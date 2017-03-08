using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {
	public Text countdownText;
	private string startText;

	public int countdownSeconds = 3;
	private float currentTime;
	private bool runCountdown;
	private bool goTextDisplayed;

	// Use this for initialization
	void Start () {
		startText = countdownText.text;
		currentTime = countdownSeconds;
	}
	
	// Update is called once per frame
	void Update () {
		if (!runCountdown) {
			return;
		}

		currentTime -= Time.deltaTime;
		if (currentTime <= 0) {
			if (!goTextDisplayed) {
				// Display "Go" text before game start
				countdownText.text = "Go!";
				goTextDisplayed = true;
				currentTime = 1;
			} else {
				// Start the game
				runCountdown = false;
				ResetCountdown ();
				TetrisManager.tm.StartGame ();
			}
		} else if (!goTextDisplayed) {
			// Update countdown text
			int roundedTime = Mathf.CeilToInt (currentTime);
			countdownText.text = roundedTime + "...";
		}
	}

	/// <summary>
	/// Begins countdown to game start
	/// </summary>
	public void StartCountdown() {
		runCountdown = true;
	}

	/// <summary>
	/// Resets the countdown text and timer.
	/// </summary>
	public void ResetCountdown() {
		currentTime = countdownSeconds;
		countdownText.text = startText;
		goTextDisplayed = false;
	}
}
