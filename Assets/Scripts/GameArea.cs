using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using WolfheatProductions;
public class GameArea : MonoBehaviour  
{
    [SerializeField] private bool isOnlyView = false;
    [SerializeField] private GameBox unclearedBoxPrefab;
    [SerializeField] private GameBox underlayBoxPrefab;
    [SerializeField] private GameObject boxHolder;
    [SerializeField] private GameObject underLaying;

    Vector3 align = new Vector3(0.5f, -0.5f, 0);
    Vector2 borderAddon = new Vector3(0.3f, 0.81f);

    Vector3 boxScale = new Vector3(0.48f, 0.48f, 1f);

    private int gameWidth = 6;
    private int gameHeight = 6;
    private int mineCountAmount = 0;
    private int totalmines = 0;

    int[,] mines;
    GameBox[,] underlayBoxes = new GameBox[0, 0];
    GameBox[,] overlayBoxes = new GameBox[0, 0];

    private int opened;
    private int totalToOpen;
    private Vector2Int swapBox;
    public bool LevelBusted { get; private set; }
    public int B3V { get; private set; }
    public int Clicks { get; private set; }

    public static GameArea Instance { get; private set; }
    public float GameWidth { get; set; }
    /*
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }*/
    private void Start()
    {
        RestartGame();
        FirestoreManager.LoadComplete += OnLoadLevelComplete;
        FirestoreManager.SubmitLevelAttemptSuccess += OnSubmitLevelSuccess;
    }

    private void OnSubmitLevelSuccess()
    {
        if (isOnlyView)
            return;
        if (USerInfo.Instance.currentType != GameType.Challenge)
            return;
        Debug.Log("Resetting board after sumbitting level success if i Challengemode");
        ResetBoard();
    }

    public void OnLoadLevelComplete(string compressed) => OnLoadLevelComplete(compressed,false);
    public void OnLoadLevelComplete(string compressed, bool editorcreateMode, bool useRotate = true)
    {
        Debug.Log("Recieved compressed level from database: " + compressed);
        string deCompressed = SavingLoadingConverter.UnComressString(compressed);

        (int[,] newMines, int[,] gameLoaded, int newgameWidth, int newgameHeight, int newtotalmines) = SavingLoadingConverter.StringLevelToGameArray(deCompressed,editorcreateMode,useRotate);

        // I dont want the size to change from the loaded level only player can change this in settings
        //USerInfo.Instance.BoardSize = newgameWidth;

        gameHeight = newgameHeight;
        gameWidth = newgameWidth;
        totalmines = newtotalmines;
        mines = newMines;

        Clicks = 0;

        LoadGame(gameLoaded, editorcreateMode);
    }


    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI amtText;
    [SerializeField] private GameObject objects;
    [SerializeField] private GameObject origo;

    [SerializeField] DigiDisplay mineDisplay;
    [SerializeField] DigiDisplay timeDisplay;
    public void RestartGame(bool resetPosition = false)
    {
        if (isOnlyView)
            return;
        SizeGameArea();

        RandomizeMines();

        DrawLevel();
        ResetLevel(resetPosition);
        AlignBoxesAnchor(resetPosition);
        SmileyButton.Instance.ShowNormal();
        Timer.Instance.ResetCounterAndPause();
        USerInfo.Instance.WaitForFirstMove = true;
        USerInfo.Instance.levelID = "RANDOM " + gameWidth + "x" + gameHeight;

        // Exchange this
        levelText.text = USerInfo.Instance.levelID;
        amtText.text = "" + FirestoreManager.Instance.LoadedAmount;

        Clicks = 0;
        LevelBusted = false;
        USerInfo.Instance.currentType = GameType.Normal;

    }

    public void PrintAllMines(string prefix, int[,] array)
    {
        StringBuilder sb = new(prefix+"\n[");
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
                sb.Append((i == 0 ? "[" : "") + array[i, j] + (i != gameWidth - 1 ? "," : "]\n"));
        } 
        sb.Append(']');
        Debug.Log(sb.ToString());
    }
    
    public void PrintFirstRowMines(string prefix, int[,] array)
    {
        StringBuilder sb = new(prefix+" [");
        for (int i = 0; i < gameWidth; i++)
            sb.Append(array[i, 0] + i != gameWidth - 1 ? "," : "");
        sb.Append(']');
        Debug.Log(sb.ToString());
    }

    public void LoadGame(int[,] gameLoaded,bool editorcreateMode = false)
    {
        // Define mines for this load?
        if (!editorcreateMode && isOnlyView)
        {
            Debug.Log("MINI VIEW - View Should does not load the normal Challenge levels!");
            return;
        }
        //Debug.Log("LOADING GAME IN GAMEAREA View: "+isOnlyView);

        USerInfo.Instance.BoardSize = gameLoaded.GetLength(0);
        ResetBoard();
        // Pre open and flag
        PreOpenAndFlag(gameLoaded, editorcreateMode);
        if (isOnlyView)
            return;
        LevelBusted = false;

        LevelCreator.Instance.LoadedGameFinalizing(editorcreateMode);
        // Also Start the timer and reset the smiley
    }

    public void ResetBoard()
    {
        SizeGameArea(false);
        //mines = gameLoaded;
        DefineUnderAndOverBoxes(); // Restes the Boxes
        DetermineNumbersFromNeighbors(); // Sets all numbers
        DrawLevel();
        if (isOnlyView)
            return;
        Timer.Instance.ResetCounterAndPause();
        SmileyButton.Instance.UpdateCollectionSize(FirestoreManager.Instance.ActiveChallengeLevels.Count);
    }

    private void PreOpenAndFlag(int[,] gameLoaded,bool editorcreateMode = false)
    {
        TotalMines();
        // Go through all mines and flagg all un-flagged 
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                //Debug.Log("gameLoaded["+i+","+j+"] = "+ gameLoaded[i, j]);
                if (gameLoaded[i, j] == 2)
                    overlayBoxes[i, j].Mark();
                else if (editorcreateMode && gameLoaded[i, j] == 1) 
                    overlayBoxes[i, j].SetAsHiddenMine();
                else if (gameLoaded[i, j] == 3)
                {
                    overlayBoxes[i, j].RemoveAndSetUnderActive();
                    underlayBoxes[i, j].MakeInteractable();
                    opened++;
                }
            }
        }

        UpdateMineCount();
    }

    public void AlignBoxesAnchor(bool resetCamera = true)
    {
        // Align GameArea
        objects.transform.position = origo.transform.position;
        objects.transform.localScale = Vector3.one * 0.5f;
        if(resetCamera)
            CameraController.Instance.ResetCamera();
        //objects.transform.localScale = Vector3.one * 0.5f;
    }

    public void ResetLevel(bool resetCamera=true)
    {
        //Debug.Log("Resetting under and over boxes in arrays");

        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                overlayBoxes[i, j].Reset();

                // Make underlaying
                underlayBoxes[i, j].SetType(mines[i, j]);
            }
        }
        TotalMines();
        UpdateMineCount();
        if(resetCamera)
            CameraController.Instance.ResetCamera();
    }
    
    public void ResetAllNonMine()
    {
        Debug.Log("Resetting under and over boxes in arrays");

        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                if (mines[i,j]!=-1)
                    overlayBoxes[i, j].Reset();
            }
        }
        TotalMines();
        UpdateMineCount();
    }

    public void TotalMines()
    {
        if (isOnlyView)
            return;
        totalToOpen = gameWidth * gameHeight - totalmines;
        mineCountAmount = totalmines;
        opened = 0;
    }
    public void DecreaseMineCount()
    {
        mineCountAmount--;
        UpdateMineCount();
    }

    public void IncreaseMineCount()
    {
        mineCountAmount++;
        UpdateMineCount();
    }

    public void DrawLevel()
    {
        //Debug.Log("Creating new under and over boxes in arrays ["+gameWidth+","+gameHeight+"] ");
        //Debug.Log("OverlayBoxes size = ["+overlayBoxes.GetLength(0)+","+overlayBoxes.GetLength(1)+"] ");
        //Debug.Log("Underlayboxes size = ["+underlayBoxes.GetLength(0)+","+underlayBoxes.GetLength(1)+"] ");
        //BottomInfoController.Instance.ShowDebugText("DrawLevel "+overlayBoxes.GetLength(0)+"x"+overlayBoxes.GetLength(1));
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                GameBox box = Instantiate(unclearedBoxPrefab, boxHolder.transform);
                box.transform.localPosition = new Vector3(i, -j, 0) + align;
                box.transform.localScale = boxScale;
                box.Pos = new Vector2Int(i, j);
                overlayBoxes[i, j] = box;

                // Make underlaying
                GameBox underlayBox = Instantiate(unclearedBoxPrefab, underLaying.transform);
                underlayBox.SetOrderingLeyer(0);
                underlayBox.transform.localPosition = new Vector3(i, -j, 0) + align;
                underlayBox.transform.localScale = boxScale;
                underlayBox.Pos = new Vector2Int(i, j);
                underlayBoxes[i, j] = underlayBox;
                underlayBox.SetType(mines[i, j]);
                
            }
        }
        Timer.Instance.Pause();
    }

    private void RandomizeMines()
    {
        mineCountAmount = 0;

        // Place total Mines at random positions A (Get all positions and take one at random)
        Debug.Log("***** Randomize Mines "+gameWidth+"*"+gameHeight);
        int[] allPos = Enumerable.Range(0, gameHeight * gameWidth).ToArray();
        
        // Fisher-Yates scramble
        allPos = Converter.FisherYatesScramble(allPos);

        // Set last position as a swapbox cause this is free unless all positons are mines
        int row = allPos[allPos.Length - 1] % gameWidth;
        int col = allPos[allPos.Length - 1] / gameWidth;
        swapBox = new Vector2Int(row, col);
        Debug.Log("Game area size = "+mines.GetLength(0)+" "+mines.GetLength(1));
        Debug.Log("Swap box at = " + ((allPos.Length - 1) % gameWidth) + ","+ ((allPos.Length - 1) / gameWidth));

        //Debug.Log("RandomizeMine is picked from row = "+row+" and col = "+col+" allpos size = "+ allPos.Length+" Total Mines = "+totalmines);
        // allPos stores all positions 0-Size-1 of the game area
        // Takes out a number representating a spot in the grid
        for (int i = 0; i < totalmines; i++)
        {
            row = allPos[i] % gameWidth;
            col = allPos[i] / gameWidth;
            mines[row, col] = -1;
        }
        mineCountAmount = totalmines;
        totalToOpen = gameWidth * gameHeight - totalmines;
        DetermineNumbersFromNeighbors();
        // Set minecount
        UpdateMineCount();
    }

    public void AddClicks()
    {
        Clicks++;
    }
    public void Chord(Vector2Int pos)
    {
        //Debug.Log("Charding levelcreator at "+pos);
        if (Chordable(pos))
        {
            //Debug.Log("Chardable");
            OpenAllNeighbors(pos);
        }
    }

    private bool Chordable(Vector2Int pos)
    {
        // Is chardable if X amount of mines are marked around it
        int number = underlayBoxes[pos.x, pos.y].value;
        return number == MarkedNeighbors(pos.x, pos.y);
    }

    private int MarkedNeighbors(int iCenter, int jCenter)
    {
        int amt = 0;
        // Determine neighbors
        for (int i = iCenter - 1; i <= iCenter + 1; i++)
        {
            for (int j = jCenter - 1; j <= jCenter + 1; j++)
            {
                if (i == iCenter && j == jCenter)
                    continue;
                if (i < 0 || j < 0 || i >= gameWidth || j >= gameHeight)
                    continue;
                if (overlayBoxes[i, j].Marked)
                    amt++;
            }
        }
        return amt;
    }

    private void BoxClickInEditB(Vector2Int pos)
    {
        // Clicking mine in Edit Mode 1 - Toggle
        if (mines[pos.x, pos.y] == -1)
        {
            Debug.Log("* EDIT MODE B - MINE TOGGLE");
            // Handle mined position
            overlayBoxes[pos.x, pos.y].RightClick(true);
        }
        else
        {
            if (overlayBoxes[pos.x, pos.y].gameObject.activeSelf)
            {
                Debug.Log("* EDIT MODE B - NORMAL CLICK OPEN");
                BasicOpeningBox(pos);
            }
            else
            {
                Debug.Log("* EDIT MODE B - CHORD");
                Chord(pos);
            }
        }
    }
    
    private void BoxClickInEditA(Vector2Int pos)
    {
        Debug.Log("Toggle Edit Mode Mine at " + pos);   
        // Clicking mine in Edit Mode 1 - Toggle
        if (mines[pos.x, pos.y] != -1)
        {
            mines[pos.x, pos.y] = -1;
            overlayBoxes[pos.x, pos.y].SetAsHiddenMine();
        }
        else if (overlayBoxes[pos.x, pos.y].Marked)
        {
            mines[pos.x, pos.y] = 0;
            overlayBoxes[pos.x, pos.y].SetAsUnFlagged();
            overlayBoxes[pos.x, pos.y].Marked = false;
        }
        else
        {
            overlayBoxes[pos.x, pos.y].RightClick();
        }
        UpdateMineCount();

    }

    public void OpenBoxCreate(Vector2Int pos)
    {
        // Separate Create Clicks for ease
        //Debug.Log("Open Box "+pos);
        if (USerInfo.Instance.currentType == GameType.Create)
        {
            if (USerInfo.EditMode == 0)
                BoxClickInEditA(pos);
            else
                BoxClickInEditB(pos);
            return;
        }
    }

    public bool OpenBox(Vector2Int pos)
    {
        // Only wait for first click in Normal mode to start timer
        if (USerInfo.Instance.WaitForFirstMove && USerInfo.Instance.currentType == GameType.Normal)
        {
            //Start the timer
            Timer.Instance.StartTimer();
            USerInfo.Instance.WaitForFirstMove = false;

            // If this is a mine swap it and recalculate the level
            if (mines[pos.x, pos.y] == -1)
            {
                SwapAndRecalculateLevel(pos);
            }

        }
        if (Timer.Instance.Paused && USerInfo.Instance.currentType != GameType.Create)
        {
            Debug.Log("Timer Paused skip");
            PanelController.Instance.ShowFadableInfo("Start Challenge on Smiley!");
            return true;
        }

        if(!BasicOpeningBox(pos))
            return false;

        // If last opened is a number check if game is cleared?
        if (opened == totalToOpen && !Timer.Instance.Paused)
        {
            WinLevel();
        }
        return true;
    }

    private bool BasicOpeningBox(Vector2Int pos)
    {
        if (mines[pos.x, pos.y] == -1)
        {
            //Debug.Log("Bust");
            overlayBoxes[pos.x, pos.y].Bust();
            BustLevel();
            return false;
        }
        if (mines[pos.x, pos.y] == 0)
        {
            //Debug.Log("Opening");
            overlayBoxes[pos.x, pos.y].RemoveAndSetUnderActive();
            opened++;
            OpenAllNeighbors(pos);
        }
        else
        {
            //Debug.Log("Number ");
            underlayBoxes[pos.x, pos.y].MakeInteractable();
            overlayBoxes[pos.x, pos.y].RemoveAndSetUnderActive();
            opened++;
        }
        return true;
    }

    private void EditToggleMine(Vector2Int pos)
    {
        throw new NotImplementedException();
    }

    private void WinLevel()
    {
        // Pause the timer
        Timer.Instance.Pause();

        Debug.Log("Win Level " + Timer.TimeElapsed);
        // Go through all mines and flagg all un-flagged 
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // If flagged and wrong change to red flag
                if (overlayBoxes[i, j].Marked && mines[i, j] != -1)
                    overlayBoxes[i, j].ShowWrongFlag();
                //else if (mines[i,j]==-1 && overlayBoxes[i, j].gameObject.activeSelf)
                else if (mines[i, j] == -1)
                    overlayBoxes[i, j].Mark();
            }
        }
        SmileyButton.Instance.ShowWin();

        LevelBusted = false;

        B3V = Calculate3BV();

        // Add Stats
        if(USerInfo.Instance.currentType == GameType.Normal)
            SavingUtility.gameSettingsData.NormalWon++;
        else if(USerInfo.Instance.currentType == GameType.Challenge)
            SavingUtility.gameSettingsData.ChallengeWon++;
        SavingUtility.Instance.SaveAllDataToFile();

        // Open Completion Panel - Pick correct one depending on level type

        PanelController.Instance.ShowLevelComplete();

    }

    private int Calculate3BV()
    {
        // Go through all tiles, when finding an opening not used expand and use surroundings
        int[,] unused = new int[mines.GetLength(0),mines.GetLength(1)];
        int bv3Count = 0;
        for (int i = 0; i < mines.GetLength(0); i++)
        {
            for (int j = 0; j < mines.GetLength(1); j++)
            {
                if (unused[i, j] == 1)
                    continue;
                if (underlayBoxes[i,j].value == 0)
                {
                    // Found an unused opening = GROW
                    unused[i, j] = 1; // use this
                    bv3Count++;
                    Grow(new List<Vector2Int>() { underlayBoxes[i,j].Pos});
                }
            }
        }
        // Now all nonmine unused are clicks
        for (int i = 0; i < mines.GetLength(0); i++)
        {
            for (int j = 0; j < mines.GetLength(1); j++)
            {
                if (unused[i, j] == 0 && mines[i, j] != -1)
                    bv3Count++;
            }
        }
        return bv3Count;

        // Local Grow Method
        void Grow(List<Vector2Int> list)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>(list);
            while (queue.Count > 0)
            {
                Vector2Int pos = queue.Dequeue();  
                foreach (var step in steps)
                {
                    Vector2Int neighbor = pos + step;
                    if(neighbor.x<0|| neighbor.y<0||neighbor.x>=gameWidth||neighbor.y>=gameHeight)
                        continue;
                    if (underlayBoxes[neighbor.x, neighbor.y].value == 0 && unused[neighbor.x,neighbor.y]==0)
                    {
                        unused[neighbor.x, neighbor.y] = 1; // use it
                        queue.Enqueue(neighbor);
                    }
                    else
                    {
                        // Has to be a number
                        unused[neighbor.x, neighbor.y] = 1; // use it
                    }
                }
            }
        }
    }
    private Vector2Int[] steps = new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1) };

    private void OpenAllNeighbors(Vector2Int pos)
    {
        int iCenter = pos.x;
        int jCenter = pos.y;

        for (int i = iCenter - 1; i <= iCenter + 1; i++)
        {
            for (int j = jCenter - 1; j <= jCenter + 1; j++)
            {
                if (i == iCenter && j == jCenter)
                    continue;
                if (i < 0 || j < 0 || i >= gameWidth || j >= gameHeight)
                    continue;
                if (overlayBoxes[i, j].gameObject.activeSelf && !overlayBoxes[i, j].Marked)
                {
                    //Debug.Log("Opening overlayBox ["+i+","+j+"] since it is active");
                    OpenBox(new Vector2Int(i, j));
                }
            }
        }
    }
    private void BustLevel()
    {
        // Pause the timer
        Timer.Instance.Pause();
        LevelBusted = true;

        Debug.Log("Bust Level");
        // Go through all flagged boxes and change wrongly marked to red flags and show all unmarked mines
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // If flagged and wrong change to red flag
                if (overlayBoxes[i, j].Marked && mines[i, j] != -1)
                    overlayBoxes[i, j].ShowWrongFlag();
                else if (mines[i, j] == -1 && overlayBoxes[i, j].gameObject.activeSelf && !overlayBoxes[i, j].Marked && !overlayBoxes[i, j].Busted)
                    overlayBoxes[i, j].ShowMine();
            }
        }
        SmileyButton.Instance.ShowBust();

        // Add Stats
        if (USerInfo.Instance.currentType == GameType.Normal)
            SavingUtility.gameSettingsData.NormalLost++;
        else if (USerInfo.Instance.currentType == GameType.Challenge)
            SavingUtility.gameSettingsData.ChallengeLost++;
        SavingUtility.Instance.SaveAllDataToFile();
    }

    private void SwapAndRecalculateLevel(Vector2Int pos)
    {
        Debug.Log("Mine at first click swap " + pos + " for " + swapBox);
        mines[pos.x, pos.y] = 0;
        mines[swapBox.x, swapBox.y] = -1;

        // Make underlaying
        RecalculateNeighbors(pos);
        RecalculateNeighbors(swapBox);

    }

    private int Neighbors(int iCenter, int jCenter)
    {
        int amt = 0;
        // Determine neighbors
        for (int i = iCenter - 1; i <= iCenter + 1; i++)
        {
            for (int j = jCenter - 1; j <= jCenter + 1; j++)
            {
                if (i == iCenter && j == jCenter)
                    continue;
                if (i < 0 || j < 0 || i >= gameWidth || j >= gameHeight)
                    continue;
                if (mines[i, j] == -1)
                    amt++;
            }
        }
        return amt;
    }

    private void RecalculateNeighbors(Vector2Int pos)
    {
        int iCenter = pos.x;
        int jCenter = pos.y;

        // Determine numbers
        for (int i = iCenter - 1; i <= iCenter + 1; i++)
        {
            for (int j = jCenter - 1; j <= jCenter + 1; j++)
            {
                if (i < 0 || j < 0 || i >= gameWidth || j >= gameHeight)
                    continue;
                if (mines[i, j] != -1)
                    mines[i, j] = Neighbors(i, j);
                underlayBoxes[i, j].SetType(mines[i, j]);
            }
        }
    }

    public void UpdateMineCount()
    {
        if (isOnlyView)
            return;
        if (USerInfo.Instance.currentType == GameType.Create)
        {
            //Show Mines Total
            if(USerInfo.EditMode == 0)
                mineDisplay.ShowValue(TotalAllMines());
            else
                mineDisplay.ShowValue(TotalUnFlagged());
            return;
        }
        //Debug.Log("Showing minecount "+mineCountAmount);
        // Set minecount
        mineDisplay?.ShowValue(mineCountAmount);
    }

    private int TotalAllMines()
    {
        int all = 0;
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {

                if (mines[i,j]==-1)
                    all++;
            }
        }
        return all;
    }
    
    private int TotalUnFlagged()
    {
        int unFlagged = 0;
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {

                if (mines[i,j]==-1 && overlayBoxes[i, j] != null && !overlayBoxes[i, j].Marked)
                    unFlagged++;
            }
        }
        return unFlagged;
    }
    
    private int TotalClickable()
    {
        int clickable = 0;
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {

                if (mines[i,j]!=-1 && overlayBoxes[i, j].gameObject.activeSelf)
                    clickable++;
            }
        }
        return clickable;
    }

    private void DetermineNumbersFromNeighbors()
    {
        // Determine numbers
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                if (mines[i, j] != -1)
                {
                    mines[i, j] = Neighbors(i, j);
                    //Debug.Log("Neighbor for "+i+","+j+" = " + mines[i,j]);
                }
            }
        }
    }

    public void SizeGameArea(bool sizeFromSettings = true, bool resetMines = true)
    {
        // Not sure if it should be here
        LevelBusted = false;
        
        if (sizeFromSettings)
        {
            int boardSize = USerInfo.Instance.BoardSize;
            Debug.Log("**** Loading game of type "+ USerInfo.Instance.BoardType);
            switch (USerInfo.Instance.BoardType)
            {
                case BoardTypes.Slider:
                    // Load info of current size
                    gameWidth = boardSize;
                    gameHeight = boardSize;
                    //float minePercent = (11 + boardSize * 0.5f) / 100;
                    totalmines = (int)(boardSize * boardSize * (11 + boardSize * 0.5f)/100);
                    break;
                case BoardTypes.Beginner:
                    Debug.Log("**** Loading beginner");
                    gameWidth = 8;
                    gameHeight = 8;
                    totalmines = 10;
                    break;
                case BoardTypes.Intermediate:
                    Debug.Log("**** Loading intermediate");
                    gameWidth = 16;
                    gameHeight = 16;
                    totalmines = 40;
                    break;
                case BoardTypes.Expert:
                    Debug.Log("**** Loading expert");
                    gameWidth = USerInfo.Instance.UseRotatedExpert?16:30;
                    gameHeight = USerInfo.Instance.UseRotatedExpert?30:16;
                    totalmines = 99;
                    break;
            }

            Debug.Log("Game Area - Load Game of Size [" + gameWidth + "x" + gameHeight + "]");

            // Use calculation instead of array to acomodate for any size

            // Mine% = 11 + size*0.5 => TotalMines = boardSize*boardSize*11 + size*0.5

            //totalmines = gameSizes[boardSize - 6]; // -6 since the lowest setting a gamearea can be is 6 and the index starsts at 0

            // Set mines array
            if (resetMines)
                mines = new int[gameWidth, gameHeight];
            else
            {
                // Copy last mines to the new array
                int[,] newMines = new int[gameWidth, gameHeight];
                int maxWidth = Math.Min(gameWidth, mines.GetLength(0));
                int maxHeight = Math.Min(gameHeight, mines.GetLength(1));
                for (int j = 0; j < maxHeight; j++)
                {
                    for (int i = 0; i < maxWidth; i++)
                    {
                        newMines[i, j] = mines[i, j];
                    }
                }
                mines = newMines;
            }
        }
        DefineUnderAndOverBoxes();

    }

    private void DefineUnderAndOverBoxes()
    {
        for (int i = 0; i < underlayBoxes.GetLength(0); i++)
        {
            for (int j = 0; j < underlayBoxes.GetLength(1); j++)
            {
                if (underlayBoxes[i, j] != null)
                    Destroy(underlayBoxes[i, j].gameObject);
                if (overlayBoxes[i, j] != null)
                    Destroy(overlayBoxes[i, j].gameObject);
            }
        }

        // Set new boxes arrays size
        underlayBoxes = new GameBox[gameWidth, gameHeight];
        overlayBoxes = new GameBox[gameWidth, gameHeight];

    }

    public void OnCreateBack(int[,] flagged)
    {
        // Flag all Mines
        FlagMinesForEditA(flagged);
    }

    public bool IsMine(Vector2Int pos)
    {
        return mines[pos.x, pos.y] == -1;
    }

    private void FlagAllMines()
    {
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                if (mines[i, j] == -1)
                    overlayBoxes[i, j].Mark();
            }
        }
    }
    
    private void FlagMinesForEditA(int[,] flags)
    {
        int maxHeight = Math.Min(gameHeight, flags.GetLength(1));
        int maxWidht = Math.Min(gameWidth, flags.GetLength(0));


        for (int j = 0; j < maxHeight; j++)
        {
            for (int i = 0; i < maxWidht; i++)
            {
                // Make uncleared
                if (mines[i, j] == -1)
                {
                    if (flags[i,j] == 1)
                        overlayBoxes[i, j].Mark();
                    else
                        overlayBoxes[i, j].SetAsHiddenMine();
                }
            }
        }
    }

    public void OnCreateNext()
    {
        SetMinesFromFlags();

        // TODO Figure out why Numbers are not added correctly

        DetermineNumbersFromNeighbors();

        UpdateNumbers();

        // Set minecount
        mineCountAmount = 0;
        UpdateMineCount();
    }

    private void SetMinesFromFlags()
    {
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                if (overlayBoxes[i, j].Marked)
                {
                    Debug.Log("Mine at " + i + "," + j + " since its marked");
                    mines[i, j] = -1;
                }
            }
        }
    }
    private void UpdateNumbers(bool keepCollidersActive = false)
    {
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                if (mines[i, j] != -1)
                {
                    underlayBoxes[i, j].SetType(mines[i, j],keepCollidersActive);
                }
            }
        }

    }

    public Vector2 BorderAreaRendererWidth() => new Vector2(gameWidth / 2f + borderAddon.x, gameHeight / 2f + borderAddon.y);
    
    public float SmileyColliderWidth() => gameWidth / 2f + borderAddon.x;

    public int[,] GetFlaggedArray()
    {
        int[,] flagged = new int[gameWidth, gameHeight];
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                if (overlayBoxes[i,j].Marked)
                    flagged[i, j] = 1;
            }
        }
        return flagged;
    }

    public bool ReplaceLevelToCollection(int index)
    {
        (int[,] charArray, string pre) = SavingLoadingConverter.LevelTo2DArray(mines, overlayBoxes);
        string compressed = SavingLoadingConverter.ComressToString(charArray, pre);
        bool isInList = FirestoreManager.Instance.LocalCollectionListHasLevel(compressed);
        if(isInList)
            return false;
        bool valid = FirestoreManager.Instance.ReplaceItemInLocalCollection(compressed, index);
        Debug.Log("Saved Level added to Collection");
        return valid;
    }

    public void AddLevelToCollection()
    {
        (int[,] charArray, string pre) = SavingLoadingConverter.LevelTo2DArray(mines, overlayBoxes);
        string compressed = SavingLoadingConverter.ComressToString(charArray, pre);
        FirestoreManager.Instance.AddToLocalCollection(compressed);
        Debug.Log("Saved Level added to Collection");
    }
    
    public string GetCompressedLevel()
    {
        (int[,] charArray, string pre) = SavingLoadingConverter.LevelTo2DArray(mines, overlayBoxes);
        return SavingLoadingConverter.ComressToString(charArray, pre);
    }
    public void SaveLevel(string levelName = "L01")
    {
        (int[,] charArray, string pre) = SavingLoadingConverter.LevelTo2DArray(mines, overlayBoxes);
        string compressed = SavingLoadingConverter.ComressToString(charArray, pre);
        Debug.Log("Saving Level completed, send to firebase firestore");
        FirestoreManager.Instance.Store(compressed);
        Debug.Log("Saved Level sent");
    }

    public bool UnSolved(Vector2Int pos) => overlayBoxes[pos.x, pos.y].UnSolved();

    public bool ValidateLevel()
    {
        return TotalClickable() > 0;
    }

    internal void ShowLevel(LevelData level)
    {
        //Debug.Log("Show Level on Miniview");
        string deCompressed = SavingLoadingConverter.UnComressString(level.Level);
        (int[,] newMines, int[,] gameLoaded, int newgameWidth, int newgameHeight, int newtotalmines) = SavingLoadingConverter.StringLevelToGameArray(deCompressed, true);

        gameHeight = newgameHeight;
        gameWidth = newgameWidth;
        totalmines = newtotalmines;
        mines = newMines;

        LoadGame(gameLoaded, true);

        //TODO FIx scaling of the miniView Here

        //SizeGameArea();
        //DrawLevel();
        //ResetLevel();
        objects.transform.position = origo.transform.position;
        // Set to fit 5x5 as base this means new size is width/5*0.5f?
        objects.transform.localScale = Vector3.one * 0.5f * 5f/gameWidth;

        // Set camera to correct position? Or move the gameArea
        //X minus Y plus
        //transform.localPosition = new Vector3(,,0);

    }

    internal void UpdateTheme()
    {
        UpdateNumbers(true);
                        
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                
                overlayBoxes[i, j].UpdateSprite();
                
            }
        }
    }
}
