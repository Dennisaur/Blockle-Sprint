using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//*TODO back button to exit
	}

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
