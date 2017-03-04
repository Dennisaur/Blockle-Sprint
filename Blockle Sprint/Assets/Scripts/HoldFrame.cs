using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldFrame : MonoBehaviour {
	public Material availableMaterial;
	public Material unavailableMaterial;

	public Transform[] holdBorder; // Array of transforms that make up the frame

	// Changes material color of hold frame
	public void UpdateHoldFrame(bool available) {
		foreach (Transform currTransform in holdBorder) {
			currTransform.GetComponent<Renderer> ().material = available ? availableMaterial : unavailableMaterial;
		}
	}
}
