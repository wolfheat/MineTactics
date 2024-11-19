using TMPro;
using UnityEngine;


public enum LoadingState{LogIn,Register,SubmitLevel}

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private StartMenu startMenu;

    private LoadingState currentState=LoadingState.LogIn;
    private void Start()
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
        AuthManager.RegisterAttemptStarted += OnRegisterStarted;        
        AuthManager.RegisterAttemptFailed += OnRegisterFailed;       
        
        AuthManager.LoginAttemptStarted += OnLoginStarted;        
        AuthManager.LoginAttemptFailed += OnLoginFailed;        
        
        FirestoreManager.SubmitLevelAttemptSuccess += OnSubmitLevelSuccess;        
        FirestoreManager.SubmitLevelAttemptFailed += OnSubmitLevelFailed;        

        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;        
    }

    private void ShowLoader(bool show) => loadingBar.gameObject.SetActive(show);
    private void OnRegisterFailed(string error)
    {
        ShowLoader(false);
        Debug.Log("OnRegisterFailed: "+error);
        // Set Name to Regitrating
        subText.text = "Failed to log in!";
    }
    private void OnLoginFailed(string error)
    {
        ShowLoader(false);
        Debug.Log("OnLoginFailed" + error);
        // Set Name to Regitrating
        subText.text = "Failed to log in!";
    }
    public void OnRegisterStarted()
    {
        currentState = LoadingState.Register;
        ShowLoader(true);
        Debug.Log("OnRegisterStarted");
        // Set Name to Regitrating
        headerText.text = "Registrating new Player";
        subText.text = "Trying to Register, please wait!";
    }
    public void OnLoginStarted()
    {
        currentState = LoadingState.LogIn;
        ShowLoader(true);
        Debug.Log("OnLoginStarted");
        // Set Name to Regitrating
        headerText.text = "Logging in";
        subText.text = "Trying to Log in, please wait!";
    }

    private void OnDisable()
    {

        AuthManager.RegisterAttemptStarted -= OnRegisterStarted;
        AuthManager.RegisterAttemptFailed -= OnRegisterFailed;

        AuthManager.LoginAttemptStarted -= OnLoginStarted;
        AuthManager.LoginAttemptFailed -= OnLoginFailed;

        AuthManager.OnSuccessfulLogIn -= OnSuccessfulLogIn;
    }

    private void OnSuccessfulLogIn()
    {
        Debug.Log("Signed In success - Close the Loader Menu");
        LevelCreator.Instance.OnPlayerSignedInSuccess();
        gameObject.SetActive(false);
    }

    internal void OnSubmitLevelStarted()
    {
        currentState = LoadingState.SubmitLevel;
        ShowLoader(true);
        Debug.Log("OnSubmitLevelStarted");
        // Set Name to Regitrating
        headerText.text = "Submitting Level";
        subText.text = "Trying to Submit the Level, please wait!";
    }
    internal void OnSubmitLevelSuccess()
    {
        ShowLoader(false);
        Debug.Log("OnSubmitLevelSuccess");
        // Set Name to Regitrating
        headerText.text = "Level Accepted";
        subText.text = "Thanks!";
    }
    private void OnSubmitLevelFailed(string error)
    {
        ShowLoader(false);
        Debug.Log("OnSubmitLevelFailed: " + error);
        headerText.text = "Level Rejected";
        // Set Name to Regitrating
        subText.text = "Could not Submit this Level: "+error;
    }
    public void OnCloseWindow()
    {
        Debug.Log("Closing Loader Window: ");
        // Load Main Menu if coming from log in or register else just close?
        if(currentState == LoadingState.LogIn || currentState == LoadingState.Register)
            startMenu.gameObject.SetActive(true);

        this.gameObject.SetActive(false);
    }
}
