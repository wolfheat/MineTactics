using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Auth;
using System;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using Firebase;
using i5.Toolkit.Core.RocketChatClient;
using System.Net;
public class AuthManager : MonoBehaviour
{
    FirebaseFirestore db;
    FirebaseAuth auth;
    public FirebaseAuth Auth => auth;
    public bool AuthResolved { get; private set; } = false;

    Firebase.FirebaseApp app;
    public static Action RegisterAttemptStarted;
    public static Action<string> RegisterAttemptFailed;
    public static Action LoginAttemptStarted;
    public static Action<string> LoginAttemptFailed;
    public static Action OnSuccessfulLogIn;
    public static Action OnSuccessfulCreation;

    public static Action<string> OnDependenciesSuccess;
    public static Action OnNameChangeSuccess;
    public static Action OnLinkSuccess;
    public static Action<string> OnLinkFail;
    public static Action OnNameChangeFail;
    public static Action<string> OnShowInfo;

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
        Firebase.FirebaseApp.Create();
        BottomInfoController.Instance?.ShowDebugText("AuthManager - Start");
            //LevelCreator.Instance.SetAppRef("Run Fix!");
            OnShowInfo?.Invoke("Init A");
        //LevelCreator.Instance.SetAppRef("Run Fix!!");GoogleSignInManager
        Debug.Log(" ****** CheckAndFixDependenciesAsync - AuthManager "+gameObject.GetInstanceID(),this);

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            OnShowInfo?.Invoke("Init B");
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                AuthResolved = true;
                Debug.Log("*** Fixed FirebaseApp Dependencies ***");
                Debug.Log("");
                Debug.Log("");



                //LevelCreator.Instance.SetAppRef("OK!");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                OnDependenciesSuccess?.Invoke("Success");
            }
            else
            {
                OnDependenciesSuccess?.Invoke("Failed");
                //LevelCreator.Instance.SetAppRef("FAIL!");
                // Firebase Unity SDK is not safe to use here.
            }
        });
            
    }

    internal IEnumerator MainSceneLoaded()
    {
        yield return new WaitForSeconds(0.2f);

        Debug.Log("MainSceneLoaded");
        auth = FirebaseAuth.DefaultInstance;
        if(auth.CurrentUser != null) {
            // The user is already defined when scene is loaded
            // Set this as main player by default and load corresponding local save files
            Debug.Log("SetCredentialsAndLoadMainGame "+auth.CurrentUser?.DisplayName);
            SetCredentialsAndLoadMainGame(auth.CurrentUser);
        }
    }

    private string logInEmail = "none@none.com";
    public void RegisterPlayerWithEmailAndPassword(string userName,string email, string password, TextMeshProUGUI resultTextfield = null)
    {
        authResult = null;
        errorMessage = "";
        StartCoroutine(WaitForRegister(email,password));
        Debug.Log("Register player: "+email+" pass: "+password);
        //password = "TEST123test";
        auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                //Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                errorMessage = "Canceled";
                
                return;
            }
            if (task.IsFaulted)
            {
                //resultTextfield.text = task.Exception.ToString();
                FirebaseException exception = task.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)exception.ErrorCode;
                /*
                switch (error)
                {
                    case AuthError.None:
                        break;
                    case AuthError.Unimplemented:
                        break;
                    case AuthError.Failure:
                        break;
                    case AuthError.InvalidCustomToken:
                        break;
                    case AuthError.CustomTokenMismatch:
                        break;
                    case AuthError.InvalidCredential:
                        break;
                    case AuthError.UserDisabled:
                        break;
                    case AuthError.AccountExistsWithDifferentCredentials:
                        break;
                    case AuthError.OperationNotAllowed:
                        break;
                    case AuthError.EmailAlreadyInUse:
                        break;
                    case AuthError.RequiresRecentLogin:
                        break;
                    case AuthError.CredentialAlreadyInUse:
                        break;
                    case AuthError.InvalidEmail:
                        break;
                    case AuthError.WrongPassword:
                        break;
                    case AuthError.TooManyRequests:
                        break;
                    case AuthError.UserNotFound:
                        break;
                    case AuthError.ProviderAlreadyLinked:
                        break;
                    case AuthError.NoSuchProvider:
                        break;
                    case AuthError.InvalidUserToken:
                        break;
                    case AuthError.UserTokenExpired:
                        break;
                    case AuthError.NetworkRequestFailed:
                        break;
                    case AuthError.InvalidApiKey:
                        break;
                    case AuthError.AppNotAuthorized:
                        break;
                    case AuthError.UserMismatch:
                        break;
                    case AuthError.WeakPassword:
                        break;
                    case AuthError.NoSignedInUser:
                        break;
                    case AuthError.ApiNotAvailable:
                        break;
                    case AuthError.ExpiredActionCode:
                        break;
                    case AuthError.InvalidActionCode:
                        break;
                    case AuthError.InvalidMessagePayload:
                        break;
                    case AuthError.InvalidPhoneNumber:
                        break;
                    case AuthError.MissingPhoneNumber:
                        break;
                    case AuthError.InvalidRecipientEmail:
                        break;
                    case AuthError.InvalidSender:
                        break;
                    case AuthError.InvalidVerificationCode:
                        break;
                    case AuthError.InvalidVerificationId:
                        break;
                    case AuthError.MissingVerificationCode:
                        break;
                    case AuthError.MissingVerificationId:
                        break;
                    case AuthError.MissingEmail:
                        break;
                    case AuthError.MissingPassword:
                        break;
                    case AuthError.QuotaExceeded:
                        break;
                    case AuthError.RetryPhoneAuth:
                        break;
                    case AuthError.SessionExpired:
                        break;
                    case AuthError.AppNotVerified:
                        break;
                    case AuthError.AppVerificationFailed:
                        break;
                    case AuthError.CaptchaCheckFailed:
                        break;
                    case AuthError.InvalidAppCredential:
                        break;
                    case AuthError.MissingAppCredential:
                        break;
                    case AuthError.InvalidClientId:
                        break;
                    case AuthError.InvalidContinueUri:
                        break;
                    case AuthError.MissingContinueUri:
                        break;
                    case AuthError.KeychainError:
                        break;
                    case AuthError.MissingAppToken:
                        break;
                    case AuthError.MissingIosBundleId:
                        break;
                    case AuthError.NotificationNotForwarded:
                        break;
                    case AuthError.UnauthorizedDomain:
                        break;
                    case AuthError.WebContextAlreadyPresented:
                        break;
                    case AuthError.WebContextCancelled:
                        break;
                    case AuthError.DynamicLinkNotActivated:
                        break;
                    case AuthError.Cancelled:
                        break;
                    case AuthError.InvalidProviderId:
                        break;
                    case AuthError.WebpublicError:
                        break;
                    case AuthError.WebStorateUnsupported:
                        break;
                    case AuthError.TenantIdMismatch:
                        break;
                    case AuthError.UnsupportedTenantOperation:
                        break;
                    case AuthError.InvalidLinkDomain:
                        break;
                    case AuthError.RejectedCredential:
                        break;
                    case AuthError.PhoneNumberNotFound:
                        break;
                    case AuthError.InvalidTenantId:
                        break;
                    case AuthError.MissingClientIdentifier:
                        break;
                    case AuthError.MissingMultiFactorSession:
                        break;
                    case AuthError.MissingMultiFactorInfo:
                        break;
                    case AuthError.InvalidMultiFactorSession:
                        break;
                    case AuthError.MultiFactorInfoNotFound:
                        break;
                    case AuthError.AdminRestrictedOperation:
                        break;
                    case AuthError.UnverifiedEmail:
                        break;
                    case AuthError.SecondFactorAlreadyEnrolled:
                        break;
                    case AuthError.MaximumSecondFactorCountExceeded:
                        break;
                    case AuthError.UnsupportedFirstFactor:
                        break;
                    case AuthError.EmailChangeNeedsVerification:
                        break;
                }
                */
                // Maybe just printe the name? 
                errorMessage = error.GetType().Name ?? "??";

                //foreach (var error in task.Exception.Flatten().InnerExceptions)
                //    errorMessage = error.Message;
                //Debug.LogWarning("Exception: " + error.Message);
                //Debug.Log(" ");
                //Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Debug.Log("Registration success!");
            authResult = task;
            // Firebase user has been created.
            //Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            FirebaseUser user = auth.CurrentUser;
            UpdateFirebaseUserName(user,userName);
            
        });


    }

    public void UpdateFirebaseUserName(string userName) => UpdateFirebaseUserName(auth.CurrentUser, userName);

    public void UpdateFirebaseUserName(FirebaseUser user, string userName)
    {
        if (user != null)
        {
            Debug.Log("Updating Player " + user.Email + " with name " + user.DisplayName + " to " + userName);
            UserProfile profile = new UserProfile
            {
                DisplayName = userName
                //PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    OnNameChangeFail.Invoke();
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    OnNameChangeFail?.Invoke();
                    return;
                }

                Debug.Log("User profile updated successfully.");
                OnNameChangeSuccess?.Invoke();
            });
        }
    }

    public void EmailAuth(string email, string password)
    {
        // USe the access token to get the firebase credential
        Debug.Log(" ** ** Email Auth ** ** ");
        Firebase.Auth.Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        LinkAccounts(credential);
    }

    public static Action<Firebase.Auth.Credential> OnCredentialsRecieved { get; set; }


    // USE LATER FOR LINKING ACCOUNTS - Should allow any credential to be linked to the current user
    public void LinkAccounts(Credential credential)
    {
        auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWithOnMainThread(linkTask =>
        {
            if (linkTask.IsCanceled) {
                Debug.LogError("LinkWithCredentialAsync was canceled.");
                return;
            }
            if (linkTask.IsFaulted) {

                OnLinkFail?.Invoke(linkTask.Exception.Message.ToString());
                Debug.LogError("LinkWithCredentialAsync encountered an error: " + linkTask.Exception);
                return;
            }

            if (linkTask.IsCompleted){
                BottomInfoController.Instance.ShowDebugText("Google account linked with Firebase.");
                Debug.Log("Linking of Credentials to Logged in Account completed successfully!");
                OnLinkSuccess?.Invoke();
            }
        });
    }


    Task<AuthResult> authResult = null;
    string errorMessage = "";
    
    public void SignInPlayerEmailAndPassword(string email, string password)
    {
        errorMessage = "";
        Debug.Log("Running SignInPlayerWithUserNameAndPassword");
        //auth.SignOut();
        authResult = null;
        logInEmail = email;
        StartCoroutine(WaitForLogIn());
        auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
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
            Debug.Log("** Player successfully Logged in! **");
            Debug.Log("");
            //AuthResult result = task.Result;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
        });


    }

    private IEnumerator WaitForRegister(string email, string password)
    {
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

            // Register success here so Also send the Username to PLayerList in Database?
            //FirestoreManager.Instance.RegisterUserName(USerInfo.Instance.userName,email);

            Debug.Log("Start Player Log in!");
            //OnSuccessfulCreation?.Invoke(result.User.UserId);
            SignInPlayerEmailAndPassword(email, password);
        }
    }

    private IEnumerator WaitForLogIn()
    {
        while ( (authResult == null || !authResult.IsCompleted) && errorMessage == "")
            yield return new WaitForSeconds(0.1f);

        if(errorMessage != "" || authResult.IsFaulted || authResult.IsCanceled)
        {
            Debug.Log("Exited Wait For Login With Error message");
            LoginAttemptFailed?.Invoke(errorMessage);
        }
        else
        {
            SetCredentialsAndLoadMainGame();
        }
    }

    public void SetCredentialsAndLoadMainGame(FirebaseUser user)
    {
        Debug.Log("-- SetCredentialsAndLoadMainGame (with google auth result) for UID:"+gameObject.GetInstanceID());
        Debug.Log("User anme = "+user?.DisplayName);
        if(USerInfo.Instance == null)
            Debug.Log("Useriunf null");
        Debug.Log("USerInfo.Instance = " + USerInfo.Instance);
        USerInfo.Instance.SetUserInfoFromFirebaseUser(user);

        // Set the playerName in Stats and Save file
        SavingUtility.gameSettingsData.PlayerName = user.DisplayName;
        Debug.Log("** ** Setting Playername in gamesettingsdata to " + SavingUtility.gameSettingsData.PlayerName);

        LevelCreator.Instance.OnPlayerSignedInSuccess();
        OnSuccessfulLogIn?.Invoke();
    }
    public void SetCredentialsAndLoadMainGame(AuthResult result) => SetCredentialsAndLoadMainGame(result.User);

    public void SetCredentialsAndLoadMainGame()
    {
        Debug.Log("-- SetCredentialsAndLoadMainGame");
        USerInfo.Instance.SetUserInfoFromFirebaseUser(auth.CurrentUser);

        // Set the playerName in Stats and Save file
        SavingUtility.gameSettingsData.PlayerName = auth.CurrentUser.DisplayName;
        Debug.Log("** ** Setting Playername in gamesettingsdata to " + SavingUtility.gameSettingsData.PlayerName);


        LevelCreator.Instance.OnPlayerSignedInSuccess();
        OnSuccessfulLogIn?.Invoke();
    }

    public void LogOut() => auth?.SignOut();


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
