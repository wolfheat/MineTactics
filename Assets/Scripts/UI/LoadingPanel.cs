using TMPro;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public static LoadingPanel Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private GameObject loadingBar;

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
        ShowLoader(true);
        Debug.Log("OnRegisterStarted");
        // Set Name to Regitrating
        headerText.text = "Registrating new Player";
        subText.text = "Trying to Register, please wait!";
    }
    public void OnLoginStarted()
    {
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
}
