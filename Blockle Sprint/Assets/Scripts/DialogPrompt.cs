using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPrompt : MonoBehaviour {
	public Text dialogText;
	public Text acceptText;
	public Text cancelText;

	public delegate void VoidDelegate();
	private VoidDelegate acceptDelegate;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			CloseDialog ();
		}
	}

	/// <summary>
	/// Initialize the dialog prompt with the specified dialog, accept and cancel captions, and accept delegate.
	/// </summary>
	/// <param name="dialog">Dialog text.</param>
	/// <param name="accept">Accept caption.</param>
	/// <param name="cancel">Cancel caption.</param>
	/// <param name="acceptDel">Void delegate action to be executed on Accept.</param>
	public void Initialize(string dialog, string accept, string cancel,	VoidDelegate acceptDel) {
		dialogText.text = dialog;
		acceptText.text = accept;
		cancelText.text = cancel;
		acceptDelegate = acceptDel;
	}

	/// <summary>
	/// Cancels and closes the dialog prompt.
	/// </summary>
	public void Cancel() {
		CloseDialog ();
	}

	/// <summary>
	/// Accept this dialog prompt and executes the accept delegate.
	/// </summary>
	public void Accept() {
		acceptDelegate ();
		CloseDialog ();
	}

	/// <summary>
	/// Closes the dialog by destroying the game object.
	/// </summary>
	void CloseDialog() {
		Destroy (gameObject);
	}
}
