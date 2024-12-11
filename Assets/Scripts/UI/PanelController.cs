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
        startMenu.SetActive(true);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
        localLevelPanel.HidePanel();
        settingsPanel.SetActive(false);
        progressPanel.gameObject.SetActive(false);

        //ToggleMenuButtons(false);
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
        USerInfo.Instance.currentType = (GameType)type;
        if (type == 0)
        {
            ButtonController.Instance.ShowButtons(MenuState.Normal);

            if(sameMode)
                return;
            BackgroundController.Instance.SetColorNormal(); 
            LevelCreator.Instance.RestartGame();
        }
        else
        {
            ButtonController.Instance.ShowButtons(MenuState.Challenge);
            if (sameMode)
                return;
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
        if (USerInfo.Instance.currentType == GameType.Create)
            return;

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
        }
        else
        {
            levelComplete.gameObject.SetActive(true);
            levelComplete.RequestUpdateLevelInfo();

        }

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
