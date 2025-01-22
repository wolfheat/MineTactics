using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum LoadingState{LogIn,Register,SubmitLevel,LoadingLevels,LoadingLevelsFailed}
public class ProgressPanel : MonoBehaviour
{
    public static ProgressPanel Instance { get; private set; }
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
        AuthManager.RegisterAttemptFailed += OnRegisterFailed;               
        AuthManager.LoginAttemptFailed += OnLoginFailed;        
        
        FirestoreManager.SubmitLevelAttemptSuccess += OnSubmitLevelSuccess;        
        FirestoreManager.SubmitLevelAttemptFailed += OnSubmitLevelFailed;        

        FirestoreManager.OnSuccessfulLoadingOfLevels += OnSuccessfulLoadLevels;
        FirestoreManager.OnLevelCollectionLevelsDownloaded += OnSuccessfulLoadLevels;

        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;
    }

    private void ShowLoadingCircleAnimation(bool show) => loadingBar.gameObject.SetActive(show);
    private void OnRegisterFailed(string error)
    {
        ShowLoadingCircleAnimation(false);
        Debug.Log("OnRegisterFailed: "+error);
        // Set Name to Regitrating
        headerText.text = "Registrating new Player";
        subText.text = "Failed to register!";
    }
    private void OnLoginFailed(string error)
    {
        ShowLoadingCircleAnimation(false);
        Debug.Log("OnLoginFailed" + error);
        // Set Name to Regitrating
        subText.text = "Failed to log in!";
    }

    public void OnRegisterStarted()
    {
        currentState = LoadingState.Register;
        ShowLoadingCircleAnimation(true);
        Debug.Log("Progress Panel - OnRegisterStarted");
        // Set Name to Regitrating
        headerText.text = "Registrating new Player";
        subText.text = "Trying to Register, please wait!";
    }
    public void OnLoginStarted()
    {
        currentState = LoadingState.LogIn;
        ShowLoadingCircleAnimation(true);
        Debug.Log("Progress Panel - OnLoginStarted");
        // Set Name to Regitrating
        headerText.text = "Logging in";
        subText.text = "Trying to Log in, please wait!";
    }
    public void OnLoadLevelsStarted()
    {
        currentState = LoadingState.LoadingLevels;
        ShowLoadingCircleAnimation(true);
        Debug.Log("Progress Panel - OnLoadLevelsStarted");
        // Set Name to Regitrating
        headerText.text = "Loading new Levels";
        subText.text = "Trying to Load new Levels, please wait!";
    }
    private void OnDisable()
    {

        AuthManager.RegisterAttemptFailed -= OnRegisterFailed;

        AuthManager.LoginAttemptFailed -= OnLoginFailed;

        FirestoreManager.SubmitLevelAttemptSuccess -= OnSubmitLevelSuccess;
        FirestoreManager.SubmitLevelAttemptFailed -= OnSubmitLevelFailed;

        FirestoreManager.OnSuccessfulLoadingOfLevels -= OnSuccessfulLoadLevels;

        AuthManager.OnSuccessfulLogIn -= OnSuccessfulLogIn;
    }

    private void OnSuccessfulLogIn()
    {
        Debug.Log("** Recieve Invoked OnSuccessfulLogIn - in ProgressPanel");

        Debug.Log("Signed In success - Close the Loader Menu");
        LevelCreator.Instance.OnPlayerSignedInSuccess();
        StartCoroutine(DelayedDisable());
    }

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);        
    }

    public void OnSubmitLevelStarted()
    {
        currentState = LoadingState.SubmitLevel;
        ShowLoadingCircleAnimation(true);
        Debug.Log("OnSubmitLevelStarted");
        // Set Name to Regitrating
        headerText.text = "Submitting Levels";
        subText.text = "Trying to Submit the Levels, please wait!";
    }

    public void OnSuccessfulLoadLevels(int amt)
    {
        ShowLoadingCircleAnimation(false);
        
        if(amt == 0)
            subText.text = "No levels could be loaded!";        
        else
            subText.text = amt+" Levels Added Successfully";        

            // Set Name to Regitrating
    }
    
    public void OnSuccessfulLoadLevels(bool success=true)
    {
        ShowLoadingCircleAnimation(false);
        Debug.Log("OnSuccessfulLoadLevels: "+success);
        if (success)
        {
            // Set Name to Regitrating
            subText.text = FirestoreManager.Instance.LoadedAmount +" Levels Loaded Successfully";
        }
        else
        {
            currentState = LoadingState.LoadingLevelsFailed;
            // Set Name to Regitrating
            headerText.text = "Could Not Load any Levels";
            subText.text = "Sorry!";
        }
    }

    public void OnSubmitLevelSuccess()
    {
        ShowLoadingCircleAnimation(false);
        Debug.Log("OnSubmitLevelSuccess");
        // Set Name to Regitrating
        subText.text = "Levels Accepted Thanks!";
    }
    private void OnSubmitLevelFailed(string error)
    {
        ShowLoadingCircleAnimation(false);
        Debug.Log("OnSubmitLevelFailed: " + error);
        headerText.text = "Level Rejected";
        // Set Name to Regitrating
        subText.text = "Could not Submit this Level: "+error;
    }
    public void OnCloseWindow()
    {
        Debug.Log("Closing Loader Window: "+currentState);
        // Load Main Menu if coming from log in or register else just close?
        if(currentState == LoadingState.LogIn || currentState == LoadingState.Register)
            startMenu.gameObject.SetActive(true);
        if (currentState == LoadingState.LoadingLevels)
            // Select a random level from the retrieved documents
            FirestoreManager.Instance.GetRandomLevel(1000);
        else if (currentState == LoadingState.SubmitLevel)
        {
            Debug.Log("Closing Window SubmitLevel mode - do nothing");
            //PanelController.Instance.Back();
        }
        else
            PanelController.Instance.ChangeMode(0);

        this.gameObject.SetActive(false);
    }

}
