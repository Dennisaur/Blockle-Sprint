using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorifyText : MonoBehaviour {
	public string textString;
	private Text textObject;

	private Color[] colors = new Color[] {
		Color.cyan,
		Color.blue,
		new Color (255, 153, 0), // Orange
		Color.yellow,
		Color.green,
		Color.magenta,
		Color.red
	};

	// Use this for initialization
	void Start () {
		textObject = gameObject.GetComponent<Text> ();
		textObject.text = AddColorTags(textString);
	}

	/// <summary>
	/// Adds color tags to each character of the given text
	/// </summary>
	/// <returns>Rich text string of colored text.</returns>
	/// <param name="text">Text.</param>
	string AddColorTags(string text) {
		int count = colors.Length;
		int currentColor = 0;
		string result = "";

		foreach (char c in text) {
			result += "<color=" + ToRGBHex(colors[currentColor]) + ">" + c + "</color>";
			currentColor = (currentColor + 1) % count;
		}

		return result;
	}

	/// <summary>
	/// Converts Color to RGB Hex value.
	/// See http://answers.unity3d.com/questions/1102232/how-to-get-the-color-code-in-rgb-hex-from-rgba-uni.html
	/// </summary>
	/// <returns>RGB hex string.</returns>
	/// <param name="color">Color.</param>
	string ToRGBHex(Color color) {
		return string.Format ("#{0:X2}{1:X2}{2:X2}", ToByte (color.r), ToByte (color.g), ToByte (color.b));
	}
	byte ToByte(float f) {
		f = Mathf.Clamp01 (f);
		return (byte)(f * 255);
	}
}
