using System;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;

public class FirebaseGoogleSignInManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser firebaseUser;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {

            // Initialize Firebase Auth
            auth = FirebaseAuth.DefaultInstance;

            // Check for an already signed-in user
            if (auth.CurrentUser != null)
            {
                firebaseUser = auth.CurrentUser;
                Debug.Log($"    Already signed in with Firebase. User: {firebaseUser.DisplayName}");
                BottomInfoController.Instance.ShowDebugText($"Already signed in with Firebase. User: {firebaseUser.DisplayName}");
            }
            else
            {
                Debug.Log("No user is signed in.");
                BottomInfoController.Instance.ShowDebugText("Google - No user is signed in.");
            }
            SignInWithGoogle();
        });
    }

    public void SignInWithGoogle()
    {
        Debug.Log("Starting Google Sign-In...");
        BottomInfoController.Instance.ShowDebugText("Google - SignInWithGoogle");
        GoogleSignInWithFirebase();
    }

    private void GoogleSignInWithFirebase()
    {
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



        /*


        // Need Google Signin Token here and Access Token to be able to sing into Firebase
        try
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
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
        }*/ 


        // Trigger Google Sign-In
        
        try
        {
            BottomInfoController.Instance.ShowDebugText("Google - ContinueWith");
            _ = FirebaseAuth.DefaultInstance;
            //GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnGoogleAuthFinished);
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    BottomInfoController.Instance.ShowDebugText("Google - IsFaulted");
                    Debug.LogError("Google Sign-In failed: " + task.Exception);
                    return;
                }

                if (task.IsCanceled)
                {
                    BottomInfoController.Instance.ShowDebugText("Google - IsCanceled");
                    Debug.Log("Google Sign-In was canceled.");
                    return;
                }

                BottomInfoController.Instance.ShowDebugText("Google - Successful sign-in, get the ID Token");
                // Successful sign-in, get the ID Token
                Debug.Log("Google Sign-In successful. Authenticating with Firebase...");
                Debug.Log("Google Sign-In was canceled.");
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
            BottomInfoController.Instance.ShowDebugText($"SignIn Exception: {ex.Message}");
        }
        
    }

}
