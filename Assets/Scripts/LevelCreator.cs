using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{

    [SerializeField] private int gameWidth;
    [SerializeField] private int gameHeight;

    [SerializeField] private GameBox mineBoxPrefab;
    [SerializeField] private GameBox unclearedBoxPrefab;
    [SerializeField] private GameBox underlayBoxPrefab;

    [SerializeField] private GameBox[] numberPrefabs;

    [SerializeField] private GameObject boxHolder;
    [SerializeField] private GameObject underLaying;
    [SerializeField] private GameObject borderArea;
    [SerializeField] private GameObject playArea;
    [SerializeField] private SpriteRenderer borderAreaRenderer;
    [SerializeField] private GameObject objects;
    [SerializeField] private GameObject origo;
    [SerializeField] private GameObject alignPosition;
    Vector3 align = new Vector3(0.5f, -0.5f, 0); 
    //Vector3 align = new Vector3(0f, 0f, 0f);
    Vector2 borderAddon = new Vector3(0.3f, 0.81f);
    //Vector2 borderAddon = new Vector3(0.8f, 1.31f);
    Vector3 borderAlign = new Vector3(0.17f, -0.72f, 0);

    Vector3 boxScale = new Vector3(0.48f, 0.48f, 1f);

    //Vector3 boxScale = new Vector3(0.5882f, 0.5882f, 1f);

    [SerializeField] private GameObject smiley;
    [SerializeField] private GameObject mineCount;
    [SerializeField] private GameObject timeCount;
    private int mineCountAmount=0;
    private int totalmines=0;
    [SerializeField] DigiDisplay mineDisplay;
    [SerializeField] DigiDisplay timeDisplay;

    Vector2[] sizePositions = new Vector2[3] { new Vector2(-2.2f,2.94f), new Vector2(-2.2f, 2.94f) , new Vector2(-2.2f, 2.94f) };

    Vector3Int[] gameSizes = new Vector3Int[3] { new Vector3Int(6, 6, 10), new Vector3Int(8, 8, 22), new Vector3Int(12, 12, 30) };

    float[] sizeScales = new float[3] { 0.832f, 0.832f, 0.832f};


    // Should there be one actual board and one visual board? yes? Only mines are necessarey to describe actual board


    public static LevelCreator Instance { get; private set; }
    public bool WaitForFirstMove { get; private set; } = true;
    public bool EditMode { get; set; } = false;

    int[,] mines;
    GameBox[,] underlayBoxes = new GameBox[0,0];
    GameBox[,] overlayBoxes = new GameBox[0, 0];
    private int opened;
    private int totalToOpen;
    private Vector2Int swapBox;

    void Start()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        RestartGame();
        /*
        SizeGameArea();
        CreateLevel();
        DrawLevel();
        AlignBoxesAnchor();
        */
        // Add size change listener
        SettingsPanel.GameSizeChange += OnPlaySizeChange;
        Inputs.Instance.Controls.Main.S.performed += OnRequestSaveLevel;
        Inputs.Instance.Controls.Main.L.performed += OnRequestLoadLevel;
        FirestoreManager.LoadComplete += OnLoadLevelComplete;
    }

    private void AlignBoxesAnchor()
    {
        // Align GameArea
        objects.transform.position = origo.transform.position;
        objects.transform.localScale = Vector3.one * 0.5f;
    }

    private void AlignSmileyAndCounterIcons()
    {

        // Place smiley at half width of game area
        mineCount.transform.localPosition = new Vector3(0.5f, smiley.transform.localPosition.y, 0);
        smiley.transform.localPosition = new Vector3(borderAreaRenderer.size.x / 2, smiley.transform.localPosition.y, 0);
        timeCount.transform.localPosition = new Vector3(borderAreaRenderer.size.x - 0.5f, smiley.transform.localPosition.y, 0);
    }

    
    private void SizeGameArea(bool sizeFromSettings = true)
    {

        if (sizeFromSettings)
        {
            Debug.Log("Load Game Size "+SettingsPanel.activeGameSize);  
            // Load info of current size
            gameWidth = gameSizes[SettingsPanel.activeGameSize].x;
            gameHeight = gameSizes[SettingsPanel.activeGameSize].y;
            totalmines = gameSizes[SettingsPanel.activeGameSize].z;

            // Set mines array
            mines = new int[gameWidth, gameHeight];
        }else
            Debug.Log("Sizing game Area from Loaded File instead of settings");

        DefineUnderAndOverBoxes();

        // Set correct size of the border
        borderAreaRenderer.size = new Vector2(gameWidth / 2f + borderAddon.x, gameHeight / 2f + borderAddon.y);

        // Orthographics reading changes, currently using a fixed size to get correct scaling
        float cameraWidthUnits = 5.6262f;
        float scaleNeeded = cameraWidthUnits / borderAreaRenderer.size.x;

        // This scale is calculated but total width for screen is hardcoded
        playArea.transform.localScale = new Vector3(scaleNeeded, scaleNeeded, 1);

        playArea.transform.position = new Vector3(-borderAreaRenderer.size.x / 2 * playArea.transform.localScale.x, borderAreaRenderer.size.y / 2 * playArea.transform.localScale.y, 0);

        AlignSmileyAndCounterIcons();
    }

    private void DefineUnderAndOverBoxes()
    {
        for (int i = 0; i < underlayBoxes.GetLength(0); i++)
        {
            for (int j = 0; j < underlayBoxes.GetLength(1); j++)
            {
                Destroy(underlayBoxes[i, j].gameObject);
                Destroy(overlayBoxes[i, j].gameObject);
            }
        }

        // Set new boxes arrays size
        underlayBoxes = new GameBox[gameWidth, gameHeight];
        overlayBoxes = new GameBox[gameWidth, gameHeight];

    }

    public void OnRequestSaveLevel(InputAction.CallbackContext context)
    {
        Debug.Log("Saving Level requested");

        (int[,] charArray, string pre) = SavingLoadingConverter.LevelTo2DArray(mines, overlayBoxes);
        string compressed = SavingLoadingConverter.ComressToString(charArray, pre);
        Debug.Log("Saving Level completed, send to firebase firestore");
        FirestoreManager.Instance.Store(compressed);
        Debug.Log("Saved Level sent");

    }
    
    public void OnRequestLoadLevel(InputAction.CallbackContext context)
    {
        Debug.Log("Loading Level requested");
        LoadRandomLevel();
    }
    public void LoadRandomLevel()
    {
        Debug.Log("Loading Level requested");
        FirestoreManager.Instance.Load("ID1");
    }


    public void OnLoadLevelComplete(string compressed)
    {
        Debug.Log("Recieved compressed level from database: "+compressed);
        string deCompressed = SavingLoadingConverter.UnComressString(compressed);
        (int[,] newMines, int[,] gameLoaded,int newgameWidth,int newgameHeight,int newtotalmines) = SavingLoadingConverter.StringLevelToGameArray(deCompressed);
        gameHeight = newgameHeight;
        gameWidth = newgameWidth;
        totalmines = newtotalmines;
        mines = newMines;
        LoadGame(gameLoaded);
    }


    internal void LoadGame(int[,] gameLoaded)
    {
        SizeGameArea(false);
        DetermineNumbersFromNeighbors();
        DrawLevel();
        AlignBoxesAnchor();

        // Pre open and flag
        PreOpenAndFlag(gameLoaded);

        // Smiley set to Normal
        SmileyButton.Instance.ShowNormal();

        // Start game and open all preopened and flag flagged
        Timer.Instance.StartTimer(); // starts the timer

        // Players first click should count in this game mode
        WaitForFirstMove = false;
    }

    private void PreOpenAndFlag(int[,] gameLoaded)
    {
        TotalMines();
        Debug.Log("Open correct Boxes");
        // Go through all mines and flagg all un-flagged 
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                //Debug.Log("gameLoaded["+i+","+j+"] = "+ gameLoaded[i, j]);
                if (gameLoaded[i,j]==2)
                    overlayBoxes[i, j].Mark();
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

    public void OnCreateNext()
    {
        // Step into opening/flagging mode
        // Change button to Submit // Have a reset?
        Debug.Log("Step Into Open / Flag mode");

        // Define Mines from flags
        SetMinesFromFlags(); 
        DetermineNumbersFromNeighbors();
        // Set minecount
        UpdateMineCount();
        // Generate numbers
        Debug.Log("Mines set to flagged positions, Numbers updated");
        EditMode = false;
    }

    private void SetMinesFromFlags()
    {
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                if (overlayBoxes[i, j].Marked)
                    mines[i, j] = -1;
            }
        }
    }

    public void OnToggleCreate()
    {
        Debug.Log("Create Toggle requested");

        // Create empty board
        SizeGameArea(); // Sizes and set empty game

        // Go into Edit mode here - no counter - No normal fail on click
        EditMode = true;

        // Clicks adds mines
        // After clicking Next go to Opening tiles or flag(if mine)
        // After next Add Gold Squares if using them or send the level 
        DrawLevel();
        ResetLevel();
        totalmines = 0;
        mineCountAmount = 0;
        AlignBoxesAnchor();
        SmileyButton.Instance.ShowNormal();
        Timer.Instance.ResetCounterAndPause();
        WaitForFirstMove = false;
    }

    public void OnPlaySizeChange()
    {
        Debug.Log("Play size changed, update game area");
        RestartGame();
    }

    internal void RestartGame()
    {

        SizeGameArea();
        RandomizeMines();
        DrawLevel();
        ResetLevel();
        AlignBoxesAnchor();
        SmileyButton.Instance.ShowNormal();
        Timer.Instance.ResetCounterAndPause();
        WaitForFirstMove = true;
    }

    public bool OpenBox(Vector2Int pos)
    {
        Debug.Log("Open Box "+pos);
        if (EditMode)
        {
            OpenBoxEditMode(pos);
            return true;
        }
        //Debug.Log("Open box "+pos);
        if (WaitForFirstMove)
        {
            //Start the timer
            Timer.Instance.StartTimer();
            WaitForFirstMove = false;

            // If this is a mine swap it and recalculate the level
            if (mines[pos.x, pos.y] == -1)
            {
                SwapAndRecalculateLevel(pos);
            }

        }
        if (Timer.Instance.Paused)
        {
            Debug.Log("Timer Paused skip");
        }
        // If allready open skip
        //if (!overlayBoxes[pos.x, pos.y].Active)
        

        //Debug.Log("Opening pos "+pos+" total opened = "+opened+"/"+totalToOpen);
        if (mines[pos.x, pos.y] == -1)
        {
            //Debug.Log("Bust");
            overlayBoxes[pos.x, pos.y].Bust();
            BustLevel();
            return false;
        }
        if(mines[pos.x, pos.y] == 0)
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
            overlayBoxes[pos.x,pos.y].RemoveAndSetUnderActive();
            opened++;
        }
        // If last opened is a number check if game is cleared?
        if (opened == totalToOpen && !Timer.Instance.Paused)
        {
            WinLevel();                
        }
        return true;
    }

    private void OpenBoxEditMode(Vector2Int pos)
    {
        Debug.Log("Toggle Edit Mode Mine "+pos);    
        
        // Toggle Mine
        mines[pos.x, pos.y] = mines[pos.x, pos.y] == -1?0:-1;
        overlayBoxes[pos.x, pos.y].RightClick();
    }

    private void BustLevel()
    {
        // Pause the timer
        Timer.Instance.Pause();

        Debug.Log("Bust Level");
        // Go through all flagged boxes and change wrongly marked to red flags and show all unmarked mines
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // If flagged and wrong change to red flag
                if (overlayBoxes[i, j].Marked && mines[i, j] != -1)
                    overlayBoxes[i, j].ShowWrongFlag();
                else if (mines[i,j]==-1 && overlayBoxes[i, j].gameObject.activeSelf && !overlayBoxes[i, j].Marked && !overlayBoxes[i, j].Busted)
                    overlayBoxes[i, j].ShowMine();
            }
        }
        SmileyButton.Instance.ShowBust();
    }
    
    private void WinLevel()
    {
        // Pause the timer
        Timer.Instance.Pause();

        Debug.Log("Win Level "+Timer.TimeElapsed);
        // Go through all mines and flagg all un-flagged 
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // If flagged and wrong change to red flag
                if (overlayBoxes[i, j].Marked && mines[i, j] != -1)
                    overlayBoxes[i, j].ShowWrongFlag();
                //else if (mines[i,j]==-1 && overlayBoxes[i, j].gameObject.activeSelf)
                else if (mines[i,j]==-1)
                    overlayBoxes[i, j].Mark();
            }
        }
        SmileyButton.Instance.ShowWin();
    }

    private void ResetLevel()
    {
        Debug.Log("Resetting under and over boxes in arrays");

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
    }

    private void SwapAndRecalculateLevel(Vector2Int pos)
    {
        Debug.Log("Mine at first click swap "+pos+" for "+swapBox);
        mines[pos.x, pos.y] = 0;
        mines[swapBox.x, swapBox.y] = -1;

        // Make underlaying
        RecalculateNeighbors(pos);
        RecalculateNeighbors(swapBox);

    }

    private void DrawLevel()
    {
        Debug.Log("Creating new under and over boxes in arrays");
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                // Make uncleared
                GameBox box = Instantiate(unclearedBoxPrefab, boxHolder.transform);
                box.transform.localPosition = new Vector3(i, -j, 0)+align;
                box.transform.localScale = boxScale;
                box.Pos = new Vector2Int(i,j);
                overlayBoxes[i, j] = box;

                // Make underlaying
                GameBox underlayBox = Instantiate(underlayBoxPrefab, underLaying.transform);
                underlayBox.transform.localPosition = new Vector3(i, -j, 0) + align;
                underlayBox.transform.localScale = boxScale;
                underlayBox.Pos = new Vector2Int(i,j);
                underlayBoxes[i, j] = underlayBox;
                underlayBox.SetType(mines[i, j]);
                if(i==3 && j== 3)
                    Debug.Log("Setting UnderLayBox to "+ mines[i, j]);
            }
        }
        Timer.Instance.Pause();
    }

    private void CreateLevel()
    {
        RandomizeMines();
    }

    private void RandomizeMines()
    {
        mineCountAmount = 0;

        // Place total Mines at random positions A (Get all positions and take one at random)

        int[] allPos = Enumerable.Range(0, gameHeight * gameWidth).ToArray();
        // Fisher-Yates scramble
        allPos = FisherYatesScramble(allPos);
        int row = allPos[allPos.Length - 1] / gameWidth;
        int col = allPos[allPos.Length - 1] % gameWidth;
        swapBox = new Vector2Int(row, col);


        for (int i = 0; i < totalmines; i++)
        {
            row = allPos[i] / gameWidth;
            col = allPos[i] % gameWidth;
            mines[row, col] = -1;
        }
        mineCountAmount = totalmines;
        totalToOpen = gameWidth * gameHeight - totalmines;
        DetermineNumbersFromNeighbors();
        // Set minecount
        UpdateMineCount();
    }

    private void DetermineNumbersFromNeighbors()
    {
        // Determine numbers
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                if (mines[i, j] != -1)
                    mines[i, j] = Neighbors(i, j);
            }
        }
    }

    private int[] FisherYatesScramble(int[] allPos)
    {
        int n = allPos.Length;
        for(int i = 0;i < n; i++)
        {
            int temp = allPos[i];
            // Rendom pos
            int random = Random.Range(0,n);
            allPos[i] = allPos[random];
            allPos[random] = temp;

        }
        return allPos;
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
                underlayBoxes[i,j].SetType(mines[i, j]);
            }
        }
    }
    
    private int Neighbors(int iCenter, int jCenter)
    {
        int amt = 0;
        // Determine neighbors
        for (int i = iCenter-1; i <= iCenter+1; i++)
        {
            for (int j = jCenter-1; j <= jCenter+1; j++)
            {
                if (i == iCenter && j == jCenter)
                    continue;
                if(i<0 || j<0 || i>=gameWidth || j >= gameHeight)
                    continue;
                if (mines[i,j]==-1)
                    amt++;  
            }
        }
        return amt;
    }


    internal void Chord(Vector2Int pos)
    {
        Debug.Log("Charding levelcreator at "+pos);
        if (Chordable(pos))
        {
            Debug.Log("Chardable");
            OpenAllNeighbors(pos);
        }else
            Debug.Log("Not Chardable");
    }

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
                    OpenBox(new Vector2Int(i,j));
                }
            }
        }
    }

    private bool Chordable(Vector2Int pos)
    {
        // Is chardable if X amount of mines are marked around it
        int number = underlayBoxes[pos.x, pos.y].value;
        return number == MarkedNeighbors(pos.x,pos.y);
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
                if (overlayBoxes[i,j].Marked)
                    amt++;
            }
        }
        return amt;
    }

    internal void TotalMines()
    {
        totalToOpen = gameWidth * gameHeight - totalmines;
        mineCountAmount = totalmines;
        opened = 0;
    }
    internal void DecreaseMineCount()
    {
        mineCountAmount--;
        UpdateMineCount();
    }
    
    internal void IncreaseMineCount()
    {
        mineCountAmount++;
        UpdateMineCount();
    }

    internal void UpdateMineCount()
    {
        //Debug.Log("Showing minecount "+mineCountAmount);
        // Set minecount
        mineDisplay.ShowValue(mineCountAmount);
    }
}
