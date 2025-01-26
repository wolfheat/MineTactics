using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using UnityEngine;
public class FirebaseGoogleSignInManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser firebaseUser;

    public static FirebaseGoogleSignInManager Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("FirebaseGoogleSignInManager - Start");
        Debug.Log(" ****** SKIP CheckAndFixDependenciesAsync - FirebaseGoogleSignInManager");

        StartCoroutine(WaitForAuth());

    }

    private IEnumerator WaitForAuth()
    {
        while (AuthManager.Instance == null || !AuthManager.Instance.AuthResolved)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("FirebaseGoogleSignInManager - WaitForAuth");
        }

        // Initialize Firebase Auth
        auth = FirebaseAuth.DefaultInstance;

        Debug.Log("FirebaseGoogleSignInManager - Dependencies");
        Debug.Log(" **  Google - check Firebase Aut ID: "+auth.CurrentUser?.UserId);


        // Check for an already signed-in user
        if (auth.CurrentUser != null)
        {
            firebaseUser = auth.CurrentUser;
            Debug.Log($"    Already signed in with Firebase. User: {firebaseUser.DisplayName}");
            BottomInfoController.Instance?.ShowDebugText($"Already signed in with Firebase. User: {firebaseUser.DisplayName}");
        }
        else
        {
            Debug.Log("No user is signed in.");
            BottomInfoController.Instance?.ShowDebugText("Google - No user is signed in.");
        }
        Debug.Log(" ---- RUN SignInWithGoogle ----");
    }

    public void RequestSignInWithGoogle()
    {
        _ = SignInWithGoogle();
    }

    public async Task SignInWithGoogle()
    {
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += OnLoginCompleted;

        await ServiceManager.GetService<OpenIDConnectService>().OpenLoginPageAsync();
    }
    AuthResult result = null;
    private void LoginComplete(AuthResult result)
    {
        Debug.Log(" - Login Complete -");
        Debug.Log(" ... User Signed in as: " + result.User.DisplayName + " | " + result.User.UserId);
        if (FindFirstObjectByType<AuthManager>() == null) Debug.Log("AuthManger = Null");
        FindFirstObjectByType<AuthManager>().SetCredentialsAndLoadMainGame(result);

        Debug.Log(" ... Calling AutManager OnSuccessfulLogIn");
        // HAve the progress show the result from the login attempt now
        AuthManager.OnSuccessfulLogIn?.Invoke();
    }


    private void GoogleLoginCompleteCreateFirebaseUser()
    {
        Firebase.Auth.Credential credential =
        Firebase.Auth.GoogleAuthProvider.GetCredential("googleIdToken", "googleAccessToken");
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    private void OnLoginCompleted(object sender, EventArgs e)
    {
        Debug.Log(" ---- Login Completed ----");
        string accessToken = ServiceManager.GetService<OpenIDConnectService>().AccessToken;
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(null, accessToken);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Sign-in was canceled");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Sign-in was faulted "+task.Exception);
                return;
            }
            LoginComplete(task.Result);

        });
    }

    /// <summary>
    /// OLD VERSIONS OF THE SIGN IN
    /// </summary>
    /// <param name="task"></param>
    private void GoogleAuthProviderSignInWithFirebase(Task<GoogleSignInUser> task)
    {
        Debug.Log("** Start of new GoogleAuthProviderSignInWithFirebase method");
        string googleIdToken = task.Result.IdToken;
        string googleAccessToken = "";

        Debug.Log("* ID token and ResultToken aquired: "+googleIdToken);

            
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    private void GoogleSignInWithFirebase()
    {
        Debug.Log("** Start of old GoogleSignInWithFirebase method");
        // Replace with your Web Client ID
        string webClientId = "623452355834-4o5t6qte51usb4a1epa4f83i1vnv3afh.apps.googleusercontent.com";

        // Create Google Sign-In configuration
        var configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            UseGameSignIn = false,
            RequestIdToken = true
        };

        GoogleSignIn.Configuration = configuration;
        Debug.Log("Created Google Signin Configuration");

        Debug.Log("GoogleSignIn.DefaultInstance=null? " + GoogleSignIn.DefaultInstance == null);

        /*
        // Trigger Google Sign-In
        try
        {
            //BottomInfoController.Instance?.ShowDebugText("Google - ContinueWith");
            Debug.Log(" *** GoogleSignIn.DefaultInstance = null ? ="+(GoogleSignIn.DefaultInstance == null));

            //GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnGoogleAuthFinished);
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                Debug.Log(" * Google Sign-In complete.");
                if (task.IsFaulted)
                {
                    //BottomInfoController.Instance?.ShowDebugText("Google - IsFaulted");
                    Debug.LogError("Google Sign-In failed: " + task.Exception);
                    return;
                }

                if (task.IsCanceled)
                {
                    //BottomInfoController.Instance?.ShowDebugText("Google - IsCanceled");
                    Debug.Log("Google Sign-In was canceled.");
                    return;
                }

                //BottomInfoController.Instance?.ShowDebugText("Google - Successful sign-in, get the ID Token");
                // Successful sign-in, get the ID Token
                Debug.Log("Google Sign-In successful. Authenticating with Firebase...");
                //Debug.Log("Google Sign-In was canceled.");
                string idToken = task.Result.IdToken;

                // Use ID Token to authenticate with Firebase
                Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(firebaseTask =>
                {
                    if (firebaseTask.IsFaulted)
                    {
                        Debug.LogError("Firebase authentication failed: " + firebaseTask.Exception);
                        return;
                    }

                    firebaseUser = firebaseTask.Result;
                    Debug.Log($"Firebase Google Sign-In successful! User: {firebaseUser.DisplayName}");
                });
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"SignIn Exception: {ex.Message}");
            BottomInfoController.Instance?.ShowDebugText($"SignIn Exception: {ex.Message}");
        }
        */
    }

}
