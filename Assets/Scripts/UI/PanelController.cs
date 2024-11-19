using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject randomButton;
    [SerializeField] GameObject startMenuButton;
    
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject nextButton;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loginMenu;
    [SerializeField] GameObject registerMenu;
    [SerializeField] GameObject submitMenu;

    [SerializeField] GameObject signInLoaderPanel;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject createPanel;
    [SerializeField] Toggle toggle;
    public static bool UsePending { get; private set; }

    private void Start()
    {
        // Initiate correct Menues at start

        InitStartMenu();
    }
    private void OnEnable()
    {
        AuthManager.LogInAttemptStarted += ShowLoaderPanel;
        AuthManager.OnSuccessfulLogIn += JustLoggedInMenus;        
    }
    private void OnDisable()
    {
        AuthManager.LogInAttemptStarted -= ShowLoaderPanel;
        AuthManager.OnSuccessfulLogIn -= JustLoggedInMenus;
    }
    private void InitStartMenu()
    {
        ToggleMenuButtons(false);
        startMenu.SetActive(true);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void CloseMainMenuNoLogIn()
    {
        startMenuButton.SetActive(true);
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

    public void ShowLoaderPanel()
    {
        // Close login and register panels
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
        signInLoaderPanel.gameObject.SetActive(true);
    }
    public void JustLoggedInMenus()
    {
        ToggleMenuButtons(true);
        startMenu.SetActive(false);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
    }
    public void RequestLogOut()
    {
        Debug.Log("Player requested log out");

        // Log out Menu ?? not needed since its sync?
        AuthManager.Instance.LogOut();
        InitStartMenu();
    }
    
    public void UpdateLoadPending()
    {
        UsePending = toggle.isOn;
    }

    public void OpenSettings()
    {
        Debug.Log("Opening Settings");
        ToggleMenuButtons(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCreateMenu()
    {
        ToggleMenuButtons(false);
        createPanel.SetActive(true);
        LevelCreator.Instance.OnToggleCreate();
    }

    public void Next()
    {
        nextButton.gameObject.SetActive(false);
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
        nextButton.gameObject.SetActive(true);
        submitMenu.SetActive(false);
    }

    public void Cancel()
    {
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
        LevelCreator.Instance.LoadRandomLevel();
    }

    private void ToggleMenuButtons(bool setActive)
    {
        // Disable all Buttons
        settingsButton.SetActive(setActive);
        createButton.SetActive(setActive);
        randomButton.SetActive(setActive);
        startMenuButton.SetActive(false);
    }

}
