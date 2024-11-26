using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject randomButton;
    [SerializeField] GameObject startMenuButton;

    [SerializeField] GameObject normalModeButton;
    [SerializeField] GameObject challengeModeButton;
    
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject nextButton;
    [SerializeField] TextMeshProUGUI modeText;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loginMenu;
    [SerializeField] GameObject registerMenu;

    [SerializeField] GameObject createMainButtons;
    [SerializeField] GameObject submitMenu;

    [SerializeField] LevelCompletionScreen levelCompleteNormal;
    [SerializeField] LevelCompletionScreen levelComplete;

    [SerializeField] LoadingPanel progressPanel;

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
            normalModeButton.gameObject.SetActive(false);
            challengeModeButton.gameObject.SetActive(true);
            LevelCreator.Instance.RestartGame();
        }
        else
        {
            normalModeButton.gameObject.SetActive(true);
            challengeModeButton.gameObject.SetActive(false);
            LevelCreator.Instance.LoadRandomLevel();
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
        LevelCreator.Instance.OnToggleCreate();
    }

    public void Next()
    {
        createMainButtons.SetActive(false);
        submitMenu.SetActive(true);
        LevelCreator.Instance.OnCreateNext();
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
        createPanel.SetActive(false);
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
        normalModeButton.SetActive(Logged ? setActive && !Normal : false);    
        challengeModeButton.SetActive(Logged ? setActive && Normal : false);    

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
}
