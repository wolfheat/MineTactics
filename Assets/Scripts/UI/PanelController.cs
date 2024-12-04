using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PanelController : MonoBehaviour
{

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
    [SerializeField] ConfirmReplaceScreen confirmReplacePanel;
    [SerializeField] ConfirmDeleteScreen confirmDeletePanel;
    [SerializeField] ConfirmRemoveManyScreen confirmRemoveManyScreen;
    [SerializeField] RemoveAllFromCollectionPanel removeAllFromCollection;
    [SerializeField] ConfirmRemoveManySelectedScreen confirmConfirmRemoveManySelectedScreen;

    [SerializeField] LoadingPanel progressPanel;
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
        startMenu.SetActive(true);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);

        settingsPanel.SetActive(false);
        progressPanel.gameObject.SetActive(false);

        ToggleMenuButtons(false);
    }

    public void CloseMainMenuNoLogIn()
    {
        ToggleMenuButtons(true);
        startMenu.SetActive(false);
    }

    public void ToggleSettings()
    {
        if (!settingsPanel.gameObject.activeSelf)
        {
            OpenSettings();
            return;
        }
        ToggleMenuButtons(true);
        settingsPanel.SetActive(false);
    }

    public void UpdateModeShown()
    {
        switch (USerInfo.Instance.currentType)
        {
            case GameType.Normal:
                modeText.text = "NORMAL";

                break;
            case GameType.Loaded:
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
        USerInfo.Instance.currentType = (GameType)type;
        if (type == 0)
        {
            normalModeButtonPanel.gameObject.SetActive(true);
            challengeModeButtonPanel.gameObject.SetActive(false);
            BackgroundController.Instance.SetColorNormal(); 
            LevelCreator.Instance.RestartGame();
        }
        else
        {
            normalModeButtonPanel.gameObject.SetActive(false);
            challengeModeButtonPanel.gameObject.SetActive(true);
            //LevelCreator.Instance.RestartGame();
            // Set unstarted level here for challengemode where player needs to press smiley to access the gamearea


            BackgroundController.Instance.SetColorTactics(); 
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
        localLevelPanel.gameObject.SetActive(true);
        localLevelPanel.UpdatePanel();
    }
        
    public void LoginConfirmed()
    {
        ToggleMenuButtons(true);
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
    
    public void OpenSettings()
    {
        Debug.Log("Opening Settings");
        ToggleMenuButtons(false);
        settingsPanel.SetActive(true);
        Debug.Log("Settings Instance: "+ SettingsPanel.Instance);
        SettingsPanel.Instance.SetValuesFromLoadedSettings();
    }

    public void OpenCreateMenu()
    {
        ToggleMenuButtons(false);
        createPanel.SetActive(true);
        Clear();
    }

    public void Clear()
    {
        LevelCreator.Instance.OnToggleCreate();
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
        UpdateModeShown();

        BaseMenu();
        ToggleMenuButtons(true);
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
        if (USerInfo.Instance.currentType == GameType.Loaded)
            return;
        ChangeMode((int)GameType.Loaded);
    }

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
        
    }

    internal void ShowLevelComplete()
    {
        if (USerInfo.Instance.currentType == GameType.Normal)
        {
            levelCompleteNormal.gameObject.SetActive(true); 
            levelCompleteNormal.RequestUpdateLevelInfo();
        }
        else
        {
            levelComplete.gameObject.SetActive(true);
            levelComplete.RequestUpdateLevelInfo();

        }

    }

    internal void ShowFadableInfo(string info)
    {
        fadableInfo.gameObject.SetActive(true);
        fadableInfo.ShowInfo(info);
    }

    internal void ShowConfirmReplaceScreen(int index)
    {
        confirmReplacePanel.gameObject.SetActive(true);
        confirmReplacePanel.SetActiveIndex(index);
    }
    internal void ShowDeleteReplaceScreen(int index)
    {
        confirmDeletePanel.gameObject.SetActive(true);
        confirmDeletePanel.SetActiveIndex(index);
    }

    public void ShowRemoveManyScreen()
    {
        removeAllFromCollection.gameObject.SetActive(true);
    }
    public void ShowConfirmRemoveManyScreen(List<LevelData> query)
    {
        confirmRemoveManyScreen.gameObject.SetActive(true);
        confirmRemoveManyScreen.SetActiveQuery(query);
    }

    internal void ShowConfirmRemoveManyConfirmationScreen(int amt)
    {
        confirmConfirmRemoveManySelectedScreen.gameObject.SetActive(true);
        confirmConfirmRemoveManySelectedScreen.SetAmt(amt);
    }
}
