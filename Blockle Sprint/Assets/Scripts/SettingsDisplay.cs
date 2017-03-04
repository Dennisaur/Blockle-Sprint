using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class SettingsDisplay : MonoBehaviour {
	public Toggle ghostPieceToggle;
	public bool showGhostPiece;

	public Slider dasSlider;
	public Text dasText;
	public int das;

	public Slider arrSlider;
	public Text arrText;
	public int arr;

	// Use this for initialization
	void Start () {
		// Attach listeners
		ghostPieceToggle.onValueChanged.AddListener(delegate {ToggleGhostPiece();});
		dasSlider.onValueChanged.AddListener(delegate {DasChanged();});
		arrSlider.onValueChanged.AddListener(delegate {ArrChanged();});
	}

	// Callled by onValueChanged listener
	void ToggleGhostPiece() {
		showGhostPiece = ghostPieceToggle.isOn;
	}

	void UpdateGhostPiece() {
		ghostPieceToggle.isOn = showGhostPiece;
	}

	// Callled by onValueChanged listener
	void DasChanged() {
		das = (int)dasSlider.value;
		UpdateDASText ();
	}

	void UpdateDASText() {
		dasText.text = das.ToString ();
	}

	// Callled by onValueChanged listener
	void ArrChanged() {
		arr = (int)arrSlider.value;
		UpdateARRText ();
	}

	void UpdateARRText() {
		arrText.text = arr.ToString ();
	}

	/// <summary>
	/// Updates the settings display.
	/// </summary>
	/// <param name="showGhost">If set to <c>true</c>, show ghost piece.</param>
	/// <param name="dasValue">Delayed Auto Shift value.</param>
	/// <param name="arrValue">Auto Repeat Rate value.</param>
	public void UpdateSettingsDisplay(bool showGhost, int dasValue, int arrValue) {
		showGhostPiece = showGhost;
		ghostPieceToggle.isOn = showGhostPiece;

		das = dasValue;
		dasSlider.value = das;
		UpdateDASText ();

		arr = arrValue;
		arrSlider.value = arr;
		UpdateARRText ();
	}

}
