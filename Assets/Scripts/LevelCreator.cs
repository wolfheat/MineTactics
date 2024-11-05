using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{

    [SerializeField] private int gameWidth = 10;
    [SerializeField] private int gameHeight = 6;

    [SerializeField] private GameBox mineBoxPrefab;
    [SerializeField] private GameBox unclearedBoxPrefab;
    [SerializeField] private GameBox underlayBoxPrefab;

    [SerializeField] private GameBox[] numberPrefabs;

    [SerializeField] private GameObject boxHolder;
    [SerializeField] private GameObject underLaying;
    [SerializeField] private GameObject borderArea;
    [SerializeField] private SpriteRenderer borderAreaRenderer;
    [SerializeField] private GameObject objects;
    [SerializeField] private GameObject alignPosition;
    Vector3 align = new Vector3(0.5f, -0.5f, 0); 
    //Vector3 align = new Vector3(0.5f, -0.5f, 0);
    Vector2 borderAddon = new Vector3(0.8f, 1.31f);
    Vector3 borderAlign = new Vector3(0.17f, -0.72f, 0);

    [SerializeField] private GameObject smiley;
    [SerializeField] private GameObject mineCount;
    [SerializeField] private GameObject timeCount;
    private int mineCountAmount=0;
    private int totalmines=0;
    [SerializeField] DigiDisplay mineDisplay;
    [SerializeField] DigiDisplay timeDisplay;


    // Should there be one actual board and one visual board? yes? Only mines are necessarey to describe actual board


    public static LevelCreator Instance { get; private set; }
    public bool WaitForFirstMove { get; private set; } = true;

    int[,] mines;
    GameBox[,] underlayBoxes;
    GameBox[,] overlayBoxes;


    void Start()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        SizeGameArea();
        
        CreateLevel();

        DrawLevel();

        // Align GameArea
        objects.transform.position = alignPosition.transform.position; 
        objects.transform.localScale = Vector3.one*0.5f;

        // Place smiley at half width of game area
        mineCount.transform.localPosition = new Vector3(0.5f, smiley.transform.localPosition.y,0);
        smiley.transform.localPosition = new Vector3(borderAreaRenderer.size.x/2, smiley.transform.localPosition.y,0);
        timeCount.transform.localPosition = new Vector3(borderAreaRenderer.size.x - 0.5f, smiley.transform.localPosition.y,0);
    }

    private void SizeGameArea()
    {
        Debug.Log("Create Level");
        mines = new int[gameWidth, gameHeight];
        underlayBoxes = new GameBox[gameWidth, gameHeight];
        overlayBoxes = new GameBox[gameWidth, gameHeight];

        // Set the border to correct size
        borderAreaRenderer.size = new Vector2(gameWidth / 2 + borderAddon.x, gameHeight / 2 + borderAddon.y);
        //borderAreaRenderer.size = new Vector2(gameWidth/2+borderAddon.x, gameHeight/2+borderAddon.y);


    }

    internal void RestartGame()
    {
        RandomizeMines();
        ResetLevel();
        SmileyButton.Instance.ShowNormal();
        Timer.Instance.ResetCounterAndPause();
        WaitForFirstMove = true;
    }

    public bool OpenBox(Vector2Int pos)
    {
        if (WaitForFirstMove)
        {
            //Start the timer
            Timer.Instance.StartTimer();
            WaitForFirstMove = false;
        }
        // If allready open skip
        if (!overlayBoxes[pos.x, pos.y].gameObject.activeSelf)
            return false;

        Debug.Log("Opening pos "+pos);
        if (mines[pos.x, pos.y] == -1)
        {
            Debug.Log("Bust");
            overlayBoxes[pos.x, pos.y].Bust();
            BustLevel();
            return false;
        }
        if(mines[pos.x, pos.y] == 0)
        {
            Debug.Log("Opening");
            overlayBoxes[pos.x, pos.y].RemoveAndSetUnderActive();
            OpenAllNeighbord(pos);
        }
        else
        {
            Debug.Log("Number ");
            underlayBoxes[pos.x, pos.y].MakeInteractable();
            overlayBoxes[pos.x,pos.y].RemoveAndSetUnderActive();

        }
        return true;
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
    }

    private void ResetLevel()
    {
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

    private void DrawLevel()
    {
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                Vector3 pos = boxHolder.transform.position + new Vector3(i, -j, 0);

                // Make uncleared
                GameBox box = Instantiate(unclearedBoxPrefab, pos+align, Quaternion.identity, boxHolder.transform);
                box.Pos = new Vector2Int(i,j);
                overlayBoxes[i, j] = box;

                //Debug.Log("checking value for "+i+","+j+" = " + mines[i,j]);
                // Make underlaying
                GameBox underlayBox = Instantiate(underlayBoxPrefab, pos + align, Quaternion.identity, underLaying.transform);
                underlayBox.Pos = new Vector2Int(i,j);
                underlayBoxes[i, j] = underlayBox;
                underlayBox.SetType(mines[i, j]);
            }
        }
    }

    private void CreateLevel()
    {
        RandomizeMines();
    }

    private void RandomizeMines()
    {
        mineCountAmount = 0;
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                mines[i, j] = Random.Range(-1, 1);
                if (mines[i, j] == -1)
                    mineCountAmount++;
            }
        }
        // Determine numbers
        for (int j = 0; j < gameHeight; j++)
        {
            for (int i = 0; i < gameWidth; i++)
            {
                if (mines[i, j] != -1)
                    mines[i, j] = Neighbors(i, j);
            }
        }
        totalmines = mineCountAmount;
        // Set minecount
        UpdateMineCount();
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
            OpenAllNeighbord(pos);
        }else
            Debug.Log("Not Chardable");
    }

    private void OpenAllNeighbord(Vector2Int pos)
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
                    OpenBox(new Vector2Int(i,j));
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
        mineCountAmount = totalmines;
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
        Debug.Log("Showing minecount "+mineCountAmount);
        // Set minecount
        mineDisplay.ShowValue(mineCountAmount);
    }
}
