﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {
	// Default settings
	public static bool defaultShowGhostPiece = true;
	public static int defaultDAS = 12;
	public static int defaultARR = 4;

	public GameObject LandscapeCanvas;
	public GameObject PortraitCanvas;

	private bool isLandscape;
	private SettingsDisplay portraitSettings;
	private SettingsDisplay landscapeSettings;
	private SettingsDisplay activeSettingsCanvas;

	private bool showGhostPiece;
	private int das;
	private int arr;

	// Use this for initialization
	void Start () {
		// Enable screen rotations
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.orientation = ScreenOrientation.AutoRotation;

		// Initialize SettingsDisplay for both orientations
		portraitSettings = PortraitCanvas.GetComponent<SettingsDisplay> ();
		landscapeSettings = LandscapeCanvas.GetComponent<SettingsDisplay> ();

	#if MOBILE_INPUT
		isLandscape = (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight);
		activeSettingsCanvas = isLandscape ? landscapeSettings : portraitSettings;
		if (isLandscape) {
			SwitchToLandscape ();
		} else {
			SwitchToPortrait ();
		}
	#else
		isLandscape = true;
		activeSettingsCanvas = landscapeSettings;
		SwitchToLandscape();
	#endif

		// Get previously loaded tuning settings
		showGhostPiece = PlayerPrefs.GetInt("Show Ghost Piece", defaultShowGhostPiece ? 1 : 0) == 1 ? true : false;
		das = PlayerPrefs.GetInt ("DAS", defaultDAS);
		arr = PlayerPrefs.GetInt ("ARR", defaultARR);

		// Update settings display with loaded tuning settings
		if (activeSettingsCanvas != null)
			activeSettingsCanvas.UpdateSettingsDisplay (showGhostPiece, das, arr);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("escape")) {
			Exit ();
		}

		if (activeSettingsCanvas == null)
			return;
		
	#if MOBILE_INPUT
		// Check for orientation change and update active canvas accordingly
		if (!isLandscape && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)) {
			GetCurrentSettings ();
			SwitchToLandscape ();
			LoadCurrentSettings ();
		} else if (isLandscape && (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)) {
			GetCurrentSettings ();
			SwitchToPortrait ();
			LoadCurrentSettings ();
		}
	#endif
	}

	/// <summary>
	/// Stores the current settings values. 
	/// </summary>
	void GetCurrentSettings() {
		showGhostPiece = activeSettingsCanvas.showGhostPiece;
		das = activeSettingsCanvas.das;
		arr = activeSettingsCanvas.arr;
	}

	/// <summary>
	/// Loads the current settings values.
	/// </summary>
	void LoadCurrentSettings() {
		activeSettingsCanvas.UpdateSettingsDisplay (showGhostPiece, das, arr);
	}

	/// <summary>
	/// Set portrait canvas to inactive and landscape canvas to active.
	/// </summary>
	void SwitchToLandscape() {
		isLandscape = true;
		PortraitCanvas.SetActive (false);
		LandscapeCanvas.SetActive (true);
		activeSettingsCanvas = landscapeSettings;
	}

	/// <summary>
	/// Set landscape canvas to inactive and portrait canvas to active.
	/// </summary>
	void SwitchToPortrait() {
		isLandscape = false;
		LandscapeCanvas.SetActive (false);
		PortraitCanvas.SetActive (true);
		activeSettingsCanvas = portraitSettings;
	}

	/// <summary>
	/// Uses default for current settings.
	/// </summary>
	public void UseDefault() {
		activeSettingsCanvas.UpdateSettingsDisplay (defaultShowGhostPiece, defaultDAS, defaultARR);
	}

	/// <summary>
	/// Save current settings in PlayerPrefs.
	/// </summary>
	void Save() {
		GetCurrentSettings();
		PlayerPrefs.SetInt("Show Ghost Piece", showGhostPiece ? 1 : 0);
		PlayerPrefs.SetInt("DAS", das);
		PlayerPrefs.SetInt("ARR", arr);
	}

	/// <summary>
	/// Save settings and return to main menu screen
	/// </summary>
	public void SaveAndExit() {
		Save ();
		Exit ();
	}

	/// <summary>
	/// Returns to main menu.
	/// </summary>
	public void Exit() {
		SceneManager.LoadScene ("Main Menu");
	}
}
