using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InGameMenu : MonoBehaviour {
	private enum MenuSelections	{resume, playAgain, exit};

	public GameObject resume;
	public GameObject playAgain;
	public GameObject exit;

	private GameObject[] selections;
	private int currentSelected = 0;

	// Use this for initialization
	void Start () {
		selections = new GameObject[3] {resume, playAgain, exit};
	}
	
	// Update is called once per frame
	void Update () {
		if (CrossPlatformInputManager.GetButtonDown ("Vertical")) {
			UpdateSelected (-1 * Input.GetAxisRaw ("Vertical"));
		}

		if (CrossPlatformInputManager.GetButtonDown ("Submit")) {
			Select ();
		}
	}

	void UpdateSelected(float direction = 0) {
		if (!gameObject.activeInHierarchy)
			return;
		
		if (selections == null) {
			Start ();
		}
		currentSelected = (currentSelected + selections.Length + (int)direction) % selections.Length;

		// If selection inactive, move to next selection option
		if (!selections [currentSelected].activeSelf) {
			currentSelected = (currentSelected + selections.Length + (int)direction) % selections.Length;
		}

		for (int i = 0; i < selections.Length; i++) {
			if (i == currentSelected) {
				selections [i].transform.localScale = new Vector3 (1.1f, 1.1f);
				selections [i].gameObject.GetComponent<UnityEngine.UI.Text> ().color = Color.cyan;
				//selections [i].gameObject.GetComponent<Rotator>().enabled = true;
			} else {
				selections [i].transform.localScale = new Vector3 (1, 1);
				selections [i].gameObject.GetComponent<UnityEngine.UI.Text> ().color = Color.white;
				//selections[i].GetComponent<Rotator>().enabled = false;
				selections [i].transform.rotation = new Quaternion ();
			}
		}
	}

	void Select() {
		switch ((MenuSelections)currentSelected) {
		case MenuSelections.resume:
			TetrisManager.tm.Resume ();
			break;
		case MenuSelections.playAgain:
			TetrisManager.tm.PlayAgain ();
			break;
		case MenuSelections.exit:
			TetrisManager.tm.Exit ();
			break;
		}
	}

	void ResetRotation() {
		for (int i = 0; i < selections.Length; i++) {
			selections [i].transform.localScale = new Vector3 (1, 1);
			selections [i].gameObject.GetComponent<UnityEngine.UI.Text> ().color = Color.white;
			//selections [i].GetComponent<Rotator> ().enabled = false;
			selections [i].transform.rotation = new Quaternion ();
		}
	}

	public void Pause() {
		gameObject.SetActive (true);
		resume.SetActive (true);
		currentSelected = 0;
		UpdateSelected ();
	}

	public void GameOver() {
		gameObject.SetActive (true);
		resume.SetActive (false);
		currentSelected = 1;
		UpdateSelected ();
	}

}
