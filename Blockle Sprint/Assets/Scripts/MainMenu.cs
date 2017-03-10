using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MainMenu : MonoBehaviour {
	public GameObject dialogCanvas;

	/// <summary>
	/// Loads the play screen
	/// </summary>
	public void StartGame() {
		SceneManager.LoadScene ("Blockle");
	}

	/// <summary>
	/// Loads the settings screen.
	/// </summary>
	public void Settings() {
		SceneManager.LoadScene ("Settings");
	}

	/// <summary>
	/// Instantiates a dialog box to confirm clearing best time.
	/// </summary>
	public void PromptClearBestTime() {
		GameObject dialog = Instantiate (dialogCanvas);
		dialog.GetComponent<DialogPrompt> ().Initialize ("Clear best time?", "Clear", "Cancel", ClearBestTime);
	}

	/// <summary>
	/// Clears the best time.
	/// </summary>
	public void ClearBestTime() {
		// Display confirmation prompt
		PlayerPrefs.DeleteKey("Best Time");

		//*TODO Android toast to confirm cleared record
	}

	/// <summary>
	/// Exit the game.
	/// </summary>
	public void Exit() {
		Application.Quit ();
	}
}
