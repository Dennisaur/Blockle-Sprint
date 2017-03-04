using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {
	public GameObject block;

	public int[,] arrayInts;			// Initial rotation matrix
	public GameObject[,] blockMatrix;   // Current rotation matrix of GameObject
	public Vector3 spawnPosition;		// Spawn position in tetris grid
	public Vector3 offset;				// Offset position in hold/next frame

	public TetrisManager.tetros myTetro;

	/// <summary>
	/// Creates the tetromino block.
	/// </summary>
	/// <param name="tetroPiece">Tetromino type.</param>
	public void CreateTetrominoBlock(TetrisManager.tetros tetroType) {
		myTetro = tetroType;

		switch (tetroType) {
		case TetrisManager.tetros.i:
			arrayInts = new int[4, 4] {
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 1 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			};
			blockMatrix = new GameObject[4, 4];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1, 0);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.j:
			arrayInts = new int[3,3] {
				{1, 0, 0},
				{1, 1, 1},
				{0, 0, 0}};
			blockMatrix = new GameObject[3, 3];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1.5f, 0.5f);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.l:
			arrayInts = new int[3,3] {
				{0, 0, 1},
				{1, 1, 1},
				{0, 0, 0}};
			blockMatrix = new GameObject[3, 3];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1.5f, 0.5f);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.o:
			arrayInts = new int[2,2] {
				{1, 1},
				{1, 1}};
			blockMatrix = new GameObject[2, 2];
			spawnPosition = new Vector3 (3, 20, 0);
			offset = new Vector3 (2f, 1.5f);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.s:
			arrayInts = new int[3,3] {
				{0, 1, 1},
				{1, 1, 0},
				{0, 0, 0}};
			blockMatrix = new GameObject[3, 3];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1.5f, 0.5f);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.t:
			arrayInts = new int[3,3] {
				{0, 1, 0},
				{1, 1, 1},
				{0, 0, 0}};
			blockMatrix = new GameObject[3, 3];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1.5f, 0.5f);

			CreateTetromino ();
			break;
		case TetrisManager.tetros.z:
			arrayInts = new int[3,3] {
				{1, 1, 0},
				{0, 1, 1},
				{0, 0, 0}};
			blockMatrix = new GameObject[3, 3];
			spawnPosition = new Vector3 (3, 19, 0);
			offset = new Vector3 (1.5f, 0.5f);

			CreateTetromino ();
			break;
		}
	}

	/// <summary>
	/// Creates the tetromino using the default rotation provided by arrayInts.
	/// </summary>
	public void CreateTetromino() {
		for (int row = 0; row < arrayInts.GetLength (0); row++) {
			for (int col = 0; col < arrayInts.GetLength (1); col++) {
				Destroy (blockMatrix [row, col]);
				if (arrayInts [row, col] == 1) {
					GameObject currBlock = Instantiate (block, transform);
					currBlock.transform.localPosition = new Vector3(col, arrayInts.GetLength(0) - 1 - row);
					currBlock.GetComponent<Renderer> ().material = TetrisManager.tetroMaterialMap[myTetro];
					blockMatrix [row, col] = currBlock;
				}
			}
		}
	}

	/// <summary>
	/// Rotates the tetromino clockwise.
	/// </summary>
	public void RotateRight() {
		blockMatrix = RotateMatrixCW (blockMatrix, blockMatrix.GetLength(0));
	}

	/// <summary>
	/// Rotate an n*n matrix clockwise.
	/// </summary>
	/// <returns>The rotated matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">n.</param>
	GameObject[,] RotateMatrixCW(GameObject[,] matrix, int n) {
		GameObject[,] returnMatrix = new GameObject[n, n];
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				returnMatrix [i, j] = matrix [n - j - 1, i];
				if (returnMatrix [i, j] != null)
					returnMatrix [i, j].transform.localPosition = new Vector3(j, n - i - 1);
			}
		}
		return returnMatrix;
	}

	/// <summary>
	/// Rotates the tetromino counter-clockwise.
	/// </summary>
	public void RotateLeft() {
		blockMatrix = RotateMatrixCCW (blockMatrix, blockMatrix.GetLength(0));
	}

	/// <summary>
	/// Rotate an n*n matrix counter-clockwise.
	/// </summary>
	/// <returns>The rotated matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">n.</param>
	GameObject[,] RotateMatrixCCW(GameObject[,] matrix, int n) {
		GameObject[,] returnMatrix = new GameObject[n, n];
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				returnMatrix [i, j] = matrix [j, n - i - 1];
				if (returnMatrix [i, j] != null)
					returnMatrix [i, j].transform.localPosition = new Vector3(j, n - i - 1);
			}
		}
		return returnMatrix;
	}
}
