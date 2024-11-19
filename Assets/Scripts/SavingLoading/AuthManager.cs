using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Auth;
using System;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using UnityEditor.PackageManager;

public class AuthManager : MonoBehaviour
{
    FirebaseFirestore db;
    FirebaseAuth auth;
    public static Action RegisterAttemptStarted;
    public static Action<string> RegisterAttemptFailed;
    public static Action LoginAttemptStarted;
    public static Action<string> LoginAttemptFailed;
    public static Action OnSuccessfulLogIn;
    public static Action OnSuccessfulCreation;


    public static AuthManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        Firebase.FirebaseApp app;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("*** Fixed FirebaseApp Dependencies ***");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private string logInEmail = "none@none.com";
    public void RegisterPlayerWithUserNameAndPassword(string email, string password, TextMeshProUGUI resultTextfield = null)
    {
        authResult = null;
        errorMessage = "";
        StartCoroutine(WaitForRegister(email,password));
        Debug.Log("Register player: "+email+" pass: "+password);
        //password = "TEST123test";
        auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                //Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                errorMessage = "Canceled";
                
                return;
            }
            if (task.IsFaulted)
            {
                //resultTextfield.text = task.Exception.ToString();
                foreach (var error in task.Exception.Flatten().InnerExceptions)
                    errorMessage = error.Message;
                //Debug.LogWarning("Exception: " + error.Message);
                //Debug.Log(" ");
                //Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("Registration success!");
            authResult = task;
            // Firebase user has been created.
            //Debug.LogFormat("Firebase user created successfully: {0} ({1})",
        });


    }
    Task<AuthResult> authResult = null;
    string errorMessage = "";
    
    public void SignInPlayerWithUserNameAndPassword(string email, string password)
    {
        errorMessage = "";
        Debug.Log("Running SignInPlayerWithUserNameAndPassword");
        //auth.SignOut();
        authResult = null;
        logInEmail = email;
        StartCoroutine(WaitForLogIn());
        auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                //Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                errorMessage = "Canceled";
                return;
            }
            if (task.IsFaulted)
            {
                errorMessage = "error";
                //Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                foreach (Exception innerException in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = innerException as Firebase.FirebaseException;
                    //Debug.LogError("Error code: " + firebaseEx.ErrorCode);
                    //Debug.LogError("Error message: " + innerException.Message);
                    errorMessage = innerException.Message;                    
                }

                return;
            }

            authResult = task;
            Debug.Log("Player successfully Logged in!");
            //AuthResult result = task.Result;
            Debug.Log("Result set");
            //Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        });


    }

    private IEnumerator WaitForRegister(string email, string password)
    {
        Debug.Log("Waiting for auth result to complete");
        RegisterAttemptStarted?.Invoke();
        while ((authResult == null || !authResult.IsCompleted) && errorMessage == "")
            yield return new WaitForSeconds(0.1f);


        if (errorMessage != "" || authResult.IsFaulted || authResult.IsCanceled)
        {
            Debug.Log("Exited Wait For Register With Error message");
            RegisterAttemptFailed?.Invoke(errorMessage);
        }
        else
        {
            //yield return new WaitUntil(() => authResult.IsCompleted);
            Debug.Log("Waited until auth result was complete");

            Debug.Log("Start Player Log in!");
            //OnSuccessfulCreation?.Invoke(result.User.UserId);
            SignInPlayerWithUserNameAndPassword(email, password);
        }
    }

    private IEnumerator WaitForLogIn()
    {
        Debug.Log("WaitForSignIn - Started");
        LoginAttemptStarted?.Invoke();
        while ( (authResult == null || !authResult.IsCompleted) && errorMessage == "")
            yield return new WaitForSeconds(0.1f);

        if(errorMessage != "" || authResult.IsFaulted || authResult.IsCanceled)
        {
            Debug.Log("Exited Wait For Login With Error message");
            LoginAttemptFailed?.Invoke(errorMessage);
        }
        else
        {
            USerInfo.Instance.SetUserInfoFromFirebaseUser(auth.CurrentUser);
            Debug.Log("Display Players name");
            OnSuccessfulLogIn?.Invoke();
        }
    }

    internal void LogOut() => auth?.SignOut();

    /*
    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                photoUrl = user.PhotoUrl ?? "";
            }
        }
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
    */
    /*
    public void WriteLevelData(string name, string level)
    {
        DocumentReference docRef = db.Collection("Levels").Document();
        Dictionary<string, object> dictionaryMessage = new Dictionary<string, object>
        {
                { "Time", FieldValue.ServerTimestamp},
                { "Name", name},
                { "Level", level},
                { "User", User.DisplayName}
        };
        docRef.SetAsync(dictionaryMessage).ContinueWithOnMainThread(task => {
            Debug.Log("Write to Firebase");
            saveUtility.SetSavingInfo("Write to Firebase");
        });
    }

    public void ReadLevelData()
    {
        Debug.Log("Read from Firebase");
        CollectionReference usersRef = db.Collection("Levels");
        Query query = usersRef.OrderBy("Time");

        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Read from Firebase,continue");
            QuerySnapshot snapshot = task.Result;
            loadUtility.UpdateLevelListFirebase(snapshot);
        });
    }
    */


}
