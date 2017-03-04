using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TetrisManager : MonoBehaviour {
	#region Variables
	public static TetrisManager tm;

	public enum tetros {i, j, l, o, s, t, z};

	// Set up tetromino material dictionaries by inspector using arrays of structs
	public TetroMaterial[] materialMap;
	[System.Serializable]
	public struct TetroMaterial {
		public tetros blockType;
		public Material material;
		public Material ghostMaterial;
	}
	public static Dictionary<tetros, Material> tetroMaterialMap;
	public static Dictionary<tetros, Material> tetroGhostMaterialMap;

	public GameObject placedPieces; // Empty GameObject as parent for placed pieces
	public GameObject tetrominoObject; // Prefab GameObject with Tetromino script

	private int gridWidth = 10;
	private int gridHeight = 22;
	private List<List<GameObject>> tetrisGrid;
	private Stack<int> bag;

	// Current tetromino
	private int currentX;
	private int currentY;
	private GameObject currentBlock;
	private GameObject currentTetromino;

	// Ghost piece
	private bool useGhostPiece = true;
	private GameObject ghostPiece;
	private int ghostY;

	// Next tetromino
	private GameObject nextTetromino;
	public GameObject nextFrameObjectLandscape;
	public GameObject nextFrameObjectPortrait;
	private GameObject activeNextFrameObject;

	// Hold
	private GameObject holdTetromino;
	public GameObject holdFrameObjectLandscape;
	public GameObject holdFrameObjectPortrait;
	private GameObject activeHoldFrameObject;
	private bool usedHold;
	public HoldFrame holdFrame;

	// Menu
	private bool paused;
	private bool gameOver;

	public GameObject inGameMenu;
	public Button resumeButton;
	public Text resumeText;
	public Text newRecordText;
	public GameObject tapToStart;

	// Sprint UI
	public GameObject sprintUILandscape;
	public GameObject sprintUIPortrait;
	private GameObject activeSprintUI;

	public int sprintLines = 40;
	public static int linesRemaining;
	public static float timeElapsed;
	private bool runTimer;

	// Gravity settings
	public float dropTime = 1f;
	public float speedDropTime = 0.1f;
	private float currentDropTime;
	private float currentTime;

	// Game tuning settings
	private bool rightDown = false;
	private bool leftDown = false;
	private int delayedAutoShift = 10;
	private int currentDAS = -1;
	private int autoRepeatRate = 5;
	private int currentARR = -1;
	#endregion

	// Use this for initialization
	void Start () {
		tm = this;

		// Instantiate material maps
		tetroMaterialMap = new Dictionary<tetros, Material> ();
		tetroGhostMaterialMap = new Dictionary<tetros, Material> ();
		foreach (TetroMaterial tetroMaterial in materialMap) {
			tetroMaterialMap.Add (tetroMaterial.blockType, tetroMaterial.material);
			tetroGhostMaterialMap.Add(tetroMaterial.blockType, tetroMaterial.ghostMaterial);
		}

		ToggleOrientation (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight);

		LoadSettings ();
		InitializeGame ();
	}

	// Update is called once per frame
	void Update () {
		//*TODO Back button to pause
		if (Input.GetKeyDown ("escape") && !gameOver) {
			Pause ();
		}

		if (currentTetromino == null || paused || gameOver) {
			return;
		}

		if (runTimer) {
			timeElapsed += Time.deltaTime;
		}

		Hold ();
		UpdateRotation ();
		UpdateX ();
		UpdateY ();

		// Set current tetromino's position
		currentTetromino.transform.position = new Vector3 (currentX, currentY, 0);
	}

	/// <summary>
	/// Refills the bag in a random order of the 7 tetrominoes.
	/// </summary>
	void RefillBag() {
		System.Random random = new System.Random ();

		List<int> tempBag = Enumerable.Range (0, 7).ToList ();
		for (int i = 0; i < tempBag.Count; i++) {
			int iRand = random.Next(i, tempBag.Count);
			int temp = tempBag [i];
			tempBag [i] = tempBag[iRand];
			tempBag [iRand] = temp;
		}
		bag = new Stack<int> (tempBag);

	}

	/// <summary>
	/// Instantiates new next piece.
	/// </summary>
	void NextPiece() {
		nextTetromino = Instantiate (tetrominoObject);
		Tetromino tetromino = nextTetromino.GetComponent<Tetromino> ();
		tetromino.CreateTetrominoBlock ((tetros)bag.Pop());
		nextTetromino.transform.parent = activeNextFrameObject.transform;
		nextTetromino.transform.localPosition = tetromino.offset;
		nextTetromino.transform.localScale = new Vector3 (1,1,1);

		if (bag.Count == 0) {
			RefillBag ();
		}
	}

	/// <summary>
	/// Spawns given piece in the tetris grid and sets it to currentTetromino.
	/// </summary>
	/// <returns><c>true</c>, if piece was spawned successfully, <c>false</c> if spawn was invalid (game over).</returns>
	/// <param name="currentPiece">Current piece to spawn in grid.</param>
	bool SpawnPiece(GameObject newCurrentPiece) {
		Tetromino tetromino = newCurrentPiece.GetComponent<Tetromino> ();
		currentTetromino = newCurrentPiece;
		currentTetromino.transform.parent = null;
		currentTetromino.transform.position = tetromino.spawnPosition; 
		currentTetromino.transform.localScale = new Vector3 (1, 1, 1);
		currentX = (int)tetromino.spawnPosition.x;
		currentY = (int)tetromino.spawnPosition.y;

		// Game over if new piece spawns in an invalid space
		if (!CheckValid (tetromino.blockMatrix)) {
			GameOver ();
			return false;
		}

		// Instantiante new ghost piece
		UpdateGhostPiece ();

		// Reset drop timer when piece is spawned on grid
		currentTime = currentDropTime;
		return true;
	}

	/// <summary>
	/// Checks if current piece is in a valid position.
	/// </summary>
	/// <returns><c>true</c>, if current tetromino is in a valid space, <c>false</c> otherwise.</returns>
	/// <param name="blockMatrix">Matrix of current tetromino rotation.</param>
	bool CheckValid(GameObject[,] blockMatrix) {
		for (int col = 0; col < blockMatrix.GetLength(1); col++) {
			for (int row = 0; row < blockMatrix.GetLength(0); row++) {
				GameObject currBlock = blockMatrix [row, col];
				if (currBlock != null) {
					// Hit bottom
					if (currentY + (blockMatrix.GetLength (0) - 1 - row) < 0) {
						return false;
					}
					// Hit left wall
					if (currentX + col < 0) {
						return false;
					}
					// Hit right wall
					if (currentX + col > gridWidth - 1) {
						return false;
					}
					// Hit a pre-existing block
					if (tetrisGrid [currentY + (blockMatrix.GetLength(0) - 1 - row)] [currentX + col] != null)
						return false;
				}
			}
		}
		return true;
	}

	#region Movement X
	void UpdateX() {
		if (CrossPlatformInputManager.GetButtonDown ("Move Right")) {
			MoveRight ();
			rightDown = true;
			leftDown = false;
			currentDAS = 0;
			currentARR = -1;
		}
		if (CrossPlatformInputManager.GetButtonDown ("Move Left")) {
			MoveLeft ();
			leftDown = true;
			rightDown = false;
			currentDAS = 0;
			currentARR = -1;
		}

		// Reset DAS/ARR on button up
		if (CrossPlatformInputManager.GetButtonUp ("Move Left") && !rightDown) {
			leftDown = false;
			currentDAS = -1;
			currentARR = -1;
		}
		if (CrossPlatformInputManager.GetButtonUp ("Move Right") && !leftDown) {
			rightDown = false;
			currentDAS = -1;
			currentARR = -1;
		}

		// Delayed auto shift
		if (currentDAS >= 0) {
			currentDAS += 1;
			// Start auto repeat when shift delay is reached
			if (currentDAS >= delayedAutoShift) {
				currentARR = 0;
				currentDAS = -1;
			}
		}

		// Auto repeat rate
		if (currentARR >= 0) {
			currentARR += 1;
			// Move left/right at each auto repeat interval
			if (currentARR >= autoRepeatRate) {
				if (leftDown) {
					MoveLeft ();
				} else if (rightDown) {
					MoveRight ();
				}
				currentARR = 0;
			}
		}
	}

	/// <summary>
	/// Moves current piece to the right.
	/// </summary>
	void MoveRight() {
		currentX += 1;
		KickLeft ();
		UpdateGhostPiece ();
	}

	/// <summary>
	/// Moves current piece to the left.
	/// </summary>
	void MoveLeft() {
		currentX -= 1;
		KickRight ();
		UpdateGhostPiece ();
	}
	#endregion

	#region Movement Y
	void UpdateY() {
		if (CrossPlatformInputManager.GetButtonDown ("Hard Drop")) {
			HardDrop();
			return;
		} else if (CrossPlatformInputManager.GetButton ("Soft Drop")) {
			SoftDrop ();
		} else {
			// Resets drop time
			currentDropTime = dropTime;
		}

		// Decrement drop timer
		currentTime -= Time.deltaTime;
		if (currentTime < 0) {
			if (!DropY ()) {
				AddPieceToGrid ();
			}
			currentTime = currentDropTime;
		}
	}

	/// <summary>
	/// Drops the current Y position by 1 unit
	/// </summary>
	/// <returns><c>false</c>, if current piece dropped to an invalid space, <c>true</c> otherwise.</returns>
	bool DropY() {
		currentY -= 1;
		if (!CheckValid(currentTetromino.GetComponent<Tetromino>().blockMatrix)) {
			currentY += 1;
			return false;
		}
		return true;
	}

	/// <summary>
	/// Drops current piece to lowest Y position possible
	/// </summary>
	void HardDrop() {
		if (useGhostPiece) {
			currentY = ghostY;
		} else {
			while (DropY ())
				;
		}

		AddPieceToGrid();
	}

	/// <summary>
	/// Lowers the drop timer during soft drop
	/// </summary>
	void SoftDrop() {
		currentDropTime = speedDropTime;
		if (CrossPlatformInputManager.GetButtonDown ("Soft Drop")) {
			currentTime = currentDropTime;
		}
	}
	#endregion

	/// <summary>
	/// Updates the ghost piece.
	/// </summary>
	void UpdateGhostPiece() {
		if (!useGhostPiece)
			return;

		Destroy (ghostPiece);

		// Instantiante new ghost piece and set to ghost material
		ghostPiece = Instantiate (currentTetromino);
		Material ghostMat = tetroGhostMaterialMap [currentTetromino.GetComponent<Tetromino>().myTetro]; 
		foreach (Renderer child in ghostPiece.GetComponentsInChildren<Renderer>()) {
			child.material = ghostMat;
		}

		// Simulate hard drop to determine ghost piece location
		int temp = currentY;
		while (DropY ())
			;
		ghostY = currentY;
		ghostPiece.transform.position = new Vector3(currentX, ghostY);
		currentY = temp;
	}

	#region Rotation
	void UpdateRotation() {
		if (CrossPlatformInputManager.GetButtonDown ("Rotate Right")) {
			RotateRight ();
		}
		if (CrossPlatformInputManager.GetButtonDown("Rotate Left")) {
			RotateLeft ();
		}
	}

	/// <summary>
	/// Rotates current piece clockwise.
	/// </summary>
	public void RotateRight() {
		Tetromino tetromino = currentTetromino.GetComponent<Tetromino> ();
		tetromino.RotateRight ();
		
		//*TODO Handle unable to rotate
		if (WallKick (tetromino.blockMatrix.GetLength (0)) >= 2 * tetromino.blockMatrix.GetLength (0)) {
			tetromino.RotateLeft ();
		}

		UpdateGhostPiece ();
	}

	/// <summary>
	/// Rotates current piece counter-clockwise.
	/// </summary>
	public void RotateLeft() {
		Tetromino tetromino = currentTetromino.GetComponent<Tetromino> ();
		tetromino.RotateLeft ();
		
		//*TODO Handle unable to rotate
		if (WallKick (tetromino.blockMatrix.GetLength (0)) >= 2 * tetromino.blockMatrix.GetLength (0)) {
			tetromino.RotateRight ();
		}

		UpdateGhostPiece ();
	}
	#endregion

	#region Kick logic
	//*TODO Improve kick logic
	int WallKick(int maxKicks) {
		KickDown (maxKicks);
		int kicks = 0;
		if (KickLeft (maxKicks) == maxKicks) {
			currentX += maxKicks;
			kicks += maxKicks;
		}
		if (KickRight (maxKicks) == maxKicks) {
			currentX -= maxKicks;
			kicks += maxKicks;
		}

		return kicks;
	}

	// Returns number of times kicked
	int KickLeft(int maxKicks = 1) {
		if (maxKicks <= 0) {
			return 0;
		}
		if (!CheckValid (currentTetromino.GetComponent<Tetromino> ().blockMatrix)) {
			currentX -= 1;
			return 1 + KickLeft(--maxKicks);
		}
		return 0;
	}

	// Returns number of times kicked
	int KickRight(int maxKicks = 1) {
		if (maxKicks <= 0) {
			return 0;
		}
		if (!CheckValid (currentTetromino.GetComponent<Tetromino> ().blockMatrix)) {
			currentX += 1;
			return 1 + KickRight(--maxKicks);
		}
		return 0;
	}

	// Kick piece down if rotated beyond top of grid
	int KickDown(int maxKicks = 1) {
		int kickDown = 0;
		GameObject[,] blockMatrix = currentTetromino.GetComponent<Tetromino> ().blockMatrix;
		for (int col = 0; col < blockMatrix.GetLength (1); col++) {
			for (int row = 0; row < blockMatrix.GetLength (0); row++) {
				GameObject currBlock = blockMatrix [row, col];
				if (currBlock != null) {
					if (currentY + (blockMatrix.GetLength (0) - 1 - row) > gridHeight - 1) {
						kickDown += 1;
					}
				}
			}
		}
		currentY -= kickDown;
		return 0;
	}
	#endregion

	#region Placement
	/// <summary>
	/// Adds the piece to grid
	/// </summary>
	void AddPieceToGrid() {
		// Add each individual block to the corresponding position in the tetris grid
		GameObject[,] blockMatrix = currentTetromino.GetComponent<Tetromino> ().blockMatrix;
		for (int row = 0; row < blockMatrix.GetLength(0); row++) {
			for (int col = 0; col < blockMatrix.GetLength(1); col++) {
				GameObject currBlock = blockMatrix [row, col];
				if (currBlock != null) {
					currBlock.transform.parent = placedPieces.transform;
					tetrisGrid [currentY + (blockMatrix.GetLength(0) - 1 - row)] [currentX + col] = currBlock;
				}
			}
		}
		ClearLines (blockMatrix);
		Destroy (currentTetromino);
		// Reset hold
		usedHold = false;
		holdFrame.UpdateHoldFrame (!usedHold);
		// Spawn next piece
		SpawnPiece (nextTetromino);
		NextPiece ();
	}

	/// <summary>
	/// Checks for complete lines with the newly added piece
	/// </summary>
	/// <param name="blockMatrix">Matrix of current tetromino rotation.</param>
	void ClearLines(GameObject[,] blockMatrix) {
		int checkY = Mathf.Clamp (currentY, 0, gridHeight - 1);
		bool completeLine;
		List<int> completedLines = new List<int> ();

		// Check for rows with completed lines
		for (int row = 0; row < blockMatrix.GetLength (0) && checkY + row < gridHeight; row++) {
			completeLine = true;
			// Check each column in row
			for (int col = 0; col < gridWidth && completeLine; col++) {
				if (tetrisGrid [checkY + row] [col] == null) {
					completeLine = false;
				}
			}
			// Remove line if completed
			if (completeLine) {
				completedLines.Add (checkY + row);
			}
		}

		// Iterate through each completed line in a top-down manner to avoid index corruption 
		List<GameObject> emptyRowObject;
		for (int row = completedLines.Count - 1; row >= 0; row--) {
			// Use new list each time to avoid reference issues
			emptyRowObject = new List<GameObject> ();
			for (int j = 0; j < gridWidth; j++) {
				emptyRowObject.Add (null);
			}
			// Destory each block in the completed line
			int line = completedLines [row];
			foreach (GameObject block in tetrisGrid[line]) {
				Destroy (block);
			}
			// Shift grid at the removed line
			tetrisGrid.RemoveAt (line);
			tetrisGrid.Add (emptyRowObject);
		}

		UpdateBlocks ();
		linesRemaining = Mathf.Max (0, linesRemaining - completedLines.Count);

		// Completed sprint
		if (linesRemaining <= 0) {
			runTimer = false;
			GameOver ();
		}
	}

	/// <summary>
	/// Update the position of each block GameObject in the grid
	/// </summary>
	void UpdateBlocks() {
		for (int row = 0; row < gridHeight; row++) {
			for (int col = 0; col < gridWidth; col++) {
				GameObject currBlock = tetrisGrid [row][col];
				if (currBlock != null) {
					currBlock.transform.position = new Vector3 (col, row, 0);
				}
			}
		}
	}
	#endregion

	#region Hold
	void Hold() {
		if (!CrossPlatformInputManager.GetButtonDown ("Hold") || usedHold) {
			return;
		}

		if (holdTetromino == null) {
			// No tetromino held yet
			HoldPiece(currentTetromino);
			SpawnPiece (nextTetromino);
			NextPiece ();
		} else {
			// Swap current with held tetromino
			GameObject temp = currentTetromino;
			SpawnPiece (holdTetromino);
			HoldPiece (temp);
		}
		usedHold = true;
		holdFrame.UpdateHoldFrame (!usedHold);
	}

	/// <summary>
	/// Resets piece rotation and moves it inside the hold frame object
	/// </summary>
	/// <param name="holdPiece">Tetromino to be held.</param>
	void HoldPiece(GameObject holdPiece) {
		Tetromino tetromino = holdPiece.GetComponent<Tetromino> ();
		tetromino.CreateTetromino (); // Used to reset the rotation
		holdTetromino = holdPiece;
		holdTetromino.transform.parent = activeHoldFrameObject.transform;
		holdTetromino.transform.localPosition = tetromino.offset;
		holdTetromino.transform.localScale = new Vector3 (1, 1, 1);
	}
	#endregion

	#region Menu options and game setup
	/// Get settings from PlayerPrefs
	void LoadSettings() {
		useGhostPiece = PlayerPrefs.GetInt ("Show Ghost Piece", Settings.defaultShowGhostPiece ? 1 : 0) == 1 ? true : false;
		delayedAutoShift = PlayerPrefs.GetInt ("DAS", Settings.defaultDAS);
		autoRepeatRate = PlayerPrefs.GetInt ("ARR", Settings.defaultARR);
	}

	/// <summary>
	/// Sets up tetris grid
	/// </summary>
	void InitializeGame() {
		paused = false;
		gameOver = false;

		HideAllMenus ();
		tapToStart.SetActive (true);

		// Instantiate tetris grid as an empty 2d list of GameObjects
		tetrisGrid = new List<List<GameObject>> ();
		List<GameObject> emptyRowObject;
		for (int i = 0; i < gridHeight; i++) {
			emptyRowObject = new List<GameObject> ();
			for (int j = 0; j < gridWidth; j++) {
				emptyRowObject.Add (null);
			}
			tetrisGrid.Add (emptyRowObject);
		}

		linesRemaining = sprintLines;
		timeElapsed = 0;
		runTimer = false;
	}

	/// <summary>
	/// Sets all menu canvases to inactive
	/// </summary>
	void HideAllMenus() {
		tapToStart.SetActive (false);
		inGameMenu.SetActive (false);
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	public void StartGame() {
		tapToStart.SetActive (false);

		currentDropTime = dropTime;
		currentTime = dropTime;

		RefillBag ();
		NextPiece ();
		SpawnPiece (nextTetromino);
		NextPiece ();

		runTimer = true;
	}

	/// <summary>
	/// Stops game and display game over menu screen
	/// </summary>
	public void GameOver() {
		gameOver = true;
		runTimer = false;

		resumeButton.interactable = false;
		resumeText.gameObject.SetActive (false);
		newRecordText.canvasRenderer.SetAlpha (0);

		inGameMenu.SetActive (true);

		// Compare current time with best time in PlayerPrefs
		if (linesRemaining <= 0 && timeElapsed < PlayerPrefs.GetFloat ("Best Time", Mathf.Infinity)) {
			newRecordText.canvasRenderer.SetAlpha (1);
			PlayerPrefs.SetFloat ("Best Time", timeElapsed);
			activeSprintUI.GetComponent<SprintUI> ().UpdateBestTime (timeElapsed);
		}
	}

	/// <summary>
	/// Destroy all tetrominoes and initialize for next game
	/// </summary>
	void Restart() {
		SetTetrominoesActive (true);
		Transform[] pieces = placedPieces.gameObject.GetComponentsInChildren<Transform> (true);
		foreach (Transform piece in pieces) {
			if (piece.gameObject == placedPieces)
				continue;
			Destroy (piece.gameObject);
		}
		Destroy (currentTetromino);
		Destroy (nextTetromino);
		Destroy (holdTetromino);
		Destroy (ghostPiece);

		InitializeGame ();
	}

	/// <summary>
	/// Pause gameplay and bring up in-game menu
	/// </summary>
	void Pause() {
		paused = true;
		SetTetrominoesActive (false);

		resumeButton.interactable = true;
		resumeText.gameObject.SetActive (true);
		newRecordText.canvasRenderer.SetAlpha (0);
		inGameMenu.SetActive (true);

		runTimer = false;
	}

	/// <summary>
	/// Resumes gameplay and hides in-game menus
	/// </summary>
	public void Resume() {
		paused = false;
		SetTetrominoesActive (true);
		HideAllMenus ();

		runTimer = true;
	}

	/// <summary>
	/// Wrapper function to restart the game
	/// </summary>
	public void PlayAgain() {
		Restart ();
	}

	/// <summary>
	/// Return to main menu
	/// </summary>
	public void Exit() {
		SceneManager.LoadScene ("Main Menu");
	}

	/// <summary>
	/// Helper function to set all tetrominoes (current, next, hold, and placed pieces) to given active state
	/// </summary>
	/// <param name="active">If set to <c>true</c>, set tetrominoes to active.</param>
	void SetTetrominoesActive(bool active) {
		placedPieces.SetActive (active);
		currentTetromino.SetActive (active);
		nextTetromino.SetActive (active);
		if (holdTetromino != null) {
			holdTetromino.SetActive (active);
		}
		if (useGhostPiece) {
			ghostPiece.SetActive (active);
		}
	}

	/// <summary>
	/// Toggles the orientation.
	/// </summary>
	/// <param name="isLandscape">If set to <c>true</c>, toggle to landscape view.</param>
	public void ToggleOrientation(bool isLandscape) {
		if (isLandscape) {
			activeNextFrameObject = nextFrameObjectLandscape;
			activeHoldFrameObject = holdFrameObjectLandscape;
			activeSprintUI = sprintUILandscape;
		} else {
			activeNextFrameObject = nextFrameObjectPortrait;
			activeHoldFrameObject = holdFrameObjectPortrait;
			activeSprintUI = sprintUIPortrait;
		}

		if (nextTetromino != null) {
			nextTetromino.transform.parent = nextFrameObjectLandscape.transform;
			nextTetromino.transform.localPosition = nextTetromino.GetComponent<Tetromino>().offset;
			nextTetromino.transform.localScale = new Vector3 (1, 1, 1);
		}
		if (holdTetromino != null) {
			holdTetromino.transform.parent = activeHoldFrameObject.transform;
			holdTetromino.transform.localPosition = holdTetromino.GetComponent<Tetromino>().offset;
			holdTetromino.transform.localScale = new Vector3 (1, 1, 1);
		}
	}

	#endregion
}
