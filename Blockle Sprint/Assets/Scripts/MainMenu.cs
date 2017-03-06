using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MainMenu : MonoBehaviour {
	public void StartGame() {
		SceneManager.LoadScene ("Blockle");
	}

	public void Settings() {
		SceneManager.LoadScene ("Settings");
	}

	public void ClearBestTime() {
		PlayerPrefs.DeleteKey("Best Time");
	}

	public void Exit() {
		Application.Quit ();
	}
}
