using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisOrientationManager : MonoBehaviour {
	private bool isLandscape;

	public GameObject landscapeParent;
	public GameObject portraitParent;

	// Use this for initialization
	void Start () {
	#if MOBILE_INPUT
		isLandscape = (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight);
		if (!isLandscape) {
			SwitchToPortrait ();
		} else {
			SwitchToLandscape ();
		}
	#else
		isLandscape = true;
		SwitchToLandscape();
	#endif
	}
	
	// Update is called once per frame
	void Update () {
	#if MOBILE_INPUT
		// Check for orientation change and update active canvas accordingly
		if (!isLandscape && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)) {
			SwitchToLandscape ();
		} else if (isLandscape && (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)) {
			SwitchToPortrait ();
		}
	#endif
	}

	/// <summary>
	/// Set portrait canvas to inactive and landscape canvas to active.
	/// </summary>
	void SwitchToLandscape() {
		isLandscape = true;
		Camera.main.transform.position = new Vector3 (4.5f, 11.5f, -24f);
		landscapeParent.SetActive (true);
		portraitParent.SetActive (false);
		TetrisManager.tm.ToggleOrientation (true);
	}

	/// <summary>
	/// Set landscape canvas to inactive and portrait canvas to active.
	/// </summary>
	void SwitchToPortrait() {
		isLandscape = false;
		Camera.main.transform.position = new Vector3 (4.5f, 7f, -31f);
		landscapeParent.SetActive (false);
		portraitParent.SetActive (true);
		TetrisManager.tm.ToggleOrientation (false);
	}
}
