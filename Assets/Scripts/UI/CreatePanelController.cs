using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanelController : MonoBehaviour
{
    [SerializeField] GameObject[] createPanels;

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;

    public static CreatePanelController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // Initiate the Create With start of creat buttons and 
        Debug.Log("Create Panel Controller Enabled - Enables first Panel");

        //SetCreateButtonsToLevel(1);
        //UpdateCreateLevelSize();
        SetSliderSize();
        //UpdateCreateLevelSize();
    }

    string LastLevel = "";
    public void TestCreatedLevel()
    {
        Debug.Log("Try to play this level!");

        // Get gameLoaded int[,] array and Load it - Have a separate Create Test Mode
        LastLevel = GameAreaMaster.Instance.MainGameArea.GetCompressedLevel();

        Debug.Log("Set mode so Only level can be tested here for level :"+LastLevel);

        // Change Mode?
        PanelController.Instance.ChangeMode(3);


        ResetTest();
        // public void LoadGame(int[,] gameLoaded, bool editorcreateMode = false)
    }
    public void AddToCollectionList()
    {
        Debug.Log("Add this Level to The Collection List");

        // CHeck here for valid Level
        bool isValid = GameAreaMaster.Instance.MainGameArea.ValidateLevel();

        if (isValid)
        {
            GameAreaMaster.Instance.MainGameArea.AddLevelToCollection();
            PanelController.Instance.ShowFadableInfo("Level Added!");
            // Set this new Level as avtive in list
            LocalLevelsPanel.Instance.SelectRecentlyAdded();
            PanelController.Instance.ShowLocalLevelPanel();
        }
        else
            PanelController.Instance.ShowInfo("not able to save level due to no clickable tiles");
    }

    public void ExitTest()
    {
        Debug.Log("Exiting Test mode");
        ButtonController.Instance.ShowButtons(MenuState.CreateB);
        BackgroundController.Instance.SetColorEditModeB();

        USerInfo.EditMode = 1;
        // Reset the CreateB to latest settings
        PanelController.Instance.ChangeMode(2);

        // Load LastLevel
        GameAreaMaster.Instance.MainGameArea.OnLoadLevelComplete(LastLevel, true);
    }
    public void ResetTest()
    {
        GameAreaMaster.Instance.MainGameArea.OnLoadLevelComplete(LastLevel, false,false);
        LevelCreator.Instance.LoadedGameFinalizing(false); // makes it playable
        SmileyButton.Instance.ShowNormal();
    }

    public void Back()
    {
        //SetCreateButtonsToLevel(1);
        SetSliderSize();
        LevelCreator.Instance.OnCreateBack();
        ButtonController.Instance.ShowButtons(MenuState.CreateA);
    }
    public void Next()
    {
        //SetCreateButtonsToLevel(2);
        LevelCreator.Instance.OnCreateNext();
        ButtonController.Instance.ShowButtons(MenuState.CreateB);
    }
    
    public void Cancel()
    {
        gameObject.SetActive(false);
        PanelController.Instance.Cancel();
    }

    public void SetCreateButtonsToLevel(int v)
    {
        for (int i = 0; i < createPanels.Length; i++) 
            createPanels[i].SetActive(i==v?true:false);
        
    }

    public void ConfirmSettings()
    {
        USerInfo.Instance.BoardSize = (int)slider.value;

        // SAVE THIS??? NO

        int[,] flagged = GameAreaMaster.Instance.MainGameArea.GetFlaggedArray();
        
        Debug.Log("** Before OnChangeSizeCreate");   
        // Apply the size settings
        LevelCreator.Instance.OnChangeSizeCreate(flagged);
        Debug.Log("** After OnChangeSizeCreate");


        //LevelCreator.Instance.ApplyFlagged(flagged);
        GameAreaMaster.Instance.MainGameArea.OnCreateBack(flagged);
    }

    public void SetSliderSize() => slider.value = USerInfo.Instance.BoardSize;
    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
    }
}
