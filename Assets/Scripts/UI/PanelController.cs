using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PanelController : MonoBehaviour
{

    [SerializeField] GameObject[] initDisableMenus;
    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject randomButton;
    [SerializeField] GameObject startMenuButton;

    [SerializeField] GameObject normalModeButtonPanel;
    [SerializeField] GameObject challengeModeButtonPanel;
    
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject nextButton;
    [SerializeField] TextMeshProUGUI modeText;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loginMenu;
    [SerializeField] GameObject registerMenu;
    [SerializeField] LocalLevelsPanel localLevelPanel;

    [SerializeField] GameObject createMainButtons;
    [SerializeField] GameObject submitMenu;

    [SerializeField] LevelCompletionScreen levelCompleteNormal;
    [SerializeField] LevelCompletionScreen levelComplete;
    [SerializeField] SelecetByConditionPanel selectByCondition;
    [SerializeField] ConfirmRemoveManySelectedScreen confirmConfirmRemoveManySelectedScreen;

    [SerializeField] ProgressPanel progressPanel;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] FadableInfo fadableInfo;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject createPanel;
    [SerializeField] Toggle toggle;

    public static PanelController Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Initiate correct Menues at start

        InitStartMenu();
    }
    private void OnEnable()
    {
        AuthManager.RegisterAttemptStarted += ShowLoaderPanelRegister;
        AuthManager.LoginAttemptStarted += ShowLoaderPanelLogin;
        AuthManager.OnSuccessfulLogIn += LoginConfirmed;        
        FirestoreManager.OnSubmitLevelStarted += ShowLoaderPanelSubmitLevel;
        FirestoreManager.OnLoadLevelStarted += ShowLoaderPanelReceiveLevel;
    }
    private void OnDisable()
    {
        AuthManager.RegisterAttemptStarted -= ShowLoaderPanelRegister;
        AuthManager.LoginAttemptStarted -= ShowLoaderPanelLogin;
        AuthManager.OnSuccessfulLogIn -= LoginConfirmed;
        FirestoreManager.OnSubmitLevelStarted -= ShowLoaderPanelSubmitLevel;
        FirestoreManager.OnLoadLevelStarted -= ShowLoaderPanelReceiveLevel;
    }
    private void InitStartMenu()
    {
        foreach (var item in initDisableMenus)
        {
            item.gameObject.SetActive(false);
        }
        startMenu.SetActive(true);        
    }

    public void CloseMainMenuNoLogIn()
    {
        //ToggleMenuButtons(true);
        startMenu.SetActive(false);
    }

    public void ToggleSettings()
    {
        //ChangeMode(0);
        ButtonController.Instance.ResetShowButtons();
    }

    public void UpdateModeShown()
    {
        switch (USerInfo.Instance.currentType)
        {
            case GameType.Normal:
                modeText.text = "NORMAL";

                break;
            case GameType.Challenge:
                modeText.text = "CHALLENGE";
                break;
            case GameType.Create:
                modeText.text = "CREATE";
                break;
            default:
                modeText.text = "MODE";
                break;
        }
    }

    public void ChangeMode(int type)
    {
        Debug.Log("CHANGE MODE +"+type);
        bool sameMode = USerInfo.Instance.currentType == (GameType)type;
        USerInfo.Instance.BoardType = BoardTypes.Slider;
        USerInfo.Instance.currentType = (GameType)type;

        if (type == 0)
        {
            ButtonController.Instance.ShowButtons(MenuState.Normal);

            if(sameMode)
                return;
            BackgroundController.Instance.SetColorNormal(); 
            LevelCreator.Instance.RestartGame();
        }
        else if(type == 1)
        {
            ButtonController.Instance.ShowButtons(MenuState.Challenge);
            if (sameMode)
                return;
            // If no levels are loaded reload all from the settings list
            GameAreaMaster.Instance.MainGameArea.ResetBoard();
            BackgroundController.Instance.SetColorTactics(); 
            SmileyButton.Instance.ShowNormal();
        }else if(type ==2)
        {
            if(USerInfo.EditMode == 1)
            {
                ButtonController.Instance.ShowButtons(MenuState.CreateB);
                BackgroundController.Instance.SetColorEditModeB(); 
            }
            else
            {
                ButtonController.Instance.ShowButtons(MenuState.CreateA);
                BackgroundController.Instance.SetColorEditMode(); 
            }

            if (sameMode)
                return;
            SmileyButton.Instance.ShowNormal();
        }else if(type == 3)
        {
            ButtonController.Instance.ShowButtons(MenuState.Test);
            // If no levels are loaded reload all from the settings list
            //GameAreaMaster.Instance.MainGameArea.ResetBoard();
            BackgroundController.Instance.SetColorTest(); 
            SmileyButton.Instance.ShowNormal();
        }
        UpdateModeShown();
    }

    public void ShowLoaderPanelLoadLevels()
    {
        progressPanel.gameObject.SetActive(true);
        progressPanel.OnLoadLevelsStarted();
    }
    
    public void ShowLoaderPanelRegister()
    {
        progressPanel.gameObject.SetActive(true);
        progressPanel.OnRegisterStarted();
    }
    
    public void ShowLoaderPanelLogin()
    {
        progressPanel.gameObject.SetActive(true);
        progressPanel.OnLoginStarted();
    }
    
    public void ShowLoaderPanelReceiveLevel()
    {
        progressPanel.gameObject.SetActive(true);
        progressPanel.OnLoadLevelsStarted();
    }
    
    public void ShowLoaderPanelSubmitLevel()
    {
        progressPanel.gameObject.SetActive(true);
        progressPanel.OnSubmitLevelStarted();
    }
        
    public void ShowInfo(string infoText)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.ShowInfo(infoText);
    }
        
    public void ShowLocalLevelPanel()
    {
        localLevelPanel.ShowPanel();
        //localLevelPanel.UpdatePanel();
    }
        
    public void LoginConfirmed()
    {
        //ToggleMenuButtons(true);
        startMenu.SetActive(false);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
    }

    public void RequestLogOutInitMainMenu()
    {
        Debug.Log("Player requested log out");

        // Reset Player Name
        LevelCreator.Instance.OnPlayerSignedOut();

        // Log out Menu ?? not needed since its sync?
        AuthManager.Instance.LogOut();

        USerInfo.Instance.IsPlayerLoggedIn = false;

        InitStartMenu();
    }
    
    public void OpenStats()
    {        
        Debug.Log("Opening Stats");
        ButtonController.Instance.ShowButtons(MenuState.Stats);
    }
    
    public void OpenSettings()
    {        
        Debug.Log("Opening Settings");
        ButtonController.Instance.ShowButtons(MenuState.Settings);
        //ToggleMenuButtons(false);
        //settingsPanel.SetActive(true);
        Debug.Log("Settings Instance: "+ SettingsPanel.Instance);
        SettingsPanel.Instance.SetValuesFromLoadedSettings();
    }

    public void OpenCreateMenu()
    {
        USerInfo.Instance.BoardType = BoardTypes.Slider;
        ButtonController.Instance.ShowButtons(MenuState.CreateA);
        //ToggleMenuButtons(false);
        createPanel.SetActive(true);
        Clear();
    }

    public void Clear()
    {
        LevelCreator.Instance.OnToggleCreate(true,true);
    }
    public void LoadLevels()
    {
        Debug.Log("Request Load Levels");
        FirestoreManager.Instance.GetRandomLevel(1000f);
    }
    
    public void Back()
    {
        BaseMenu();
        LevelCreator.Instance.OnCreateBack();
    }

    private void BaseMenu()
    {
        createMainButtons.SetActive(true);
        submitMenu.SetActive(false);
    }

    public void Cancel()
    {
        USerInfo.Instance.currentType = GameType.Normal;
        //UpdateModeShown();
        //BaseMenu();
        //ToggleMenuButtons(true);

        ButtonController.Instance.ShowButtons(MenuState.Normal);

        LevelCreator.Instance.CancelEditMode();
        // Change this for going to Nothing loaded?
        LevelCreator.Instance.RestartGame();
    }
    
    public void Submit()
    {
        LevelCreator.Instance.OnSubmitLevel();
    }
    
    public void Random()
    {
        if (USerInfo.Instance.currentType == GameType.Challenge)
            return;
        ChangeMode((int)GameType.Challenge);
    }
    /*
    private void ToggleMenuButtons(bool setActive)
    {
        // Disable all Buttons
        settingsButton.SetActive(setActive);
        bool Logged = USerInfo.Instance.IsPlayerLoggedIn;        
        bool Normal = USerInfo.Instance.currentType == GameType.Normal;        
        //randomButton.SetActive(Logged ? setActive : false);
        createButton.SetActive(Logged ? setActive : false);
        normalModeButtonPanel.SetActive(Logged ? setActive && Normal : false);
        challengeModeButtonPanel.SetActive(Logged ? setActive && !Normal : false);    

        startMenuButton.SetActive(!Logged ? setActive : false);
        
    }*/

    public void ShowLevelComplete()
    {
        if (USerInfo.Instance.currentType == GameType.Normal)
        {
            levelCompleteNormal.gameObject.SetActive(true); 
            levelCompleteNormal.RequestUpdateLevelInfo();
            // Add to record
            bool record = false;
            switch (USerInfo.Instance.BoardType)
            {
                case BoardTypes.Slider:
                    record = SavingUtility.gameSettingsData.AddIfRecord(Timer.TimeElapsed,USerInfo.Instance.BoardSize-5,GameAreaMaster.Instance.MainGameArea.B3V);
                    break;
                case BoardTypes.Beginner:
                    record = SavingUtility.gameSettingsData.AddOriginalRecord(Timer.TimeElapsed,0, GameAreaMaster.Instance.MainGameArea.B3V);
                    break;
                case BoardTypes.Intermediate:
                    record = SavingUtility.gameSettingsData.AddOriginalRecord(Timer.TimeElapsed,1, GameAreaMaster.Instance.MainGameArea.B3V);
                    break;
                case BoardTypes.Expert:
                    record = SavingUtility.gameSettingsData.AddOriginalRecord(Timer.TimeElapsed,2, GameAreaMaster.Instance.MainGameArea.B3V);
                    break;
            }
            if (record)
                SavingUtility.Instance.SaveAllDataToFile();
        }
        else if (USerInfo.Instance.currentType == GameType.Challenge)
        {
            levelComplete.gameObject.SetActive(true);
            levelComplete.RequestUpdateLevelInfo();

            //SavingUtility.gameSettingsData.AddIfRecord(Timer.TimeElapsed,USerInfo.Instance.BoardSize-6);

        }// Else when in Create Test mode do nothing

    }

    public void ShowFadableInfo(string info)
    {
        fadableInfo.gameObject.SetActive(true);
        fadableInfo.ShowInfo(info);
    }

    public void RequestSelectByConditionScreen()
    {
        selectByCondition.gameObject.SetActive(true);
    }

    public void UnSelectAll()
    {
        Debug.Log("Unselect all here");
    }
}
