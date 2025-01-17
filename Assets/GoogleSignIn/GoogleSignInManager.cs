using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSignInManager : MonoBehaviour
{
    string GoogleWebAPI = "623452355834-4o5t6qte51usb4a1epa4f83i1vnv3afh.apps.googleusercontent.com";
    private GoogleSignInConfiguration googleSignInConfiguration;
    Firebase.DependencyStatus firebaseStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    private bool firebaseresolved = false;

    // Start is called befo;re the first frame update
    void Awake()
    {
        // Configure webAPI with google
        googleSignInConfiguration = new GoogleSignInConfiguration { 
            WebClientId = GoogleWebAPI, 
            RequestIdToken=true
        };
        Debug.Log("Google - Awake");

        BottomInfoController.Instance.ShowDebugText("Google - Awake");
        AuthManager.OnShowInfo?.Invoke("Google - Awake");

    }
    private void Start()
    {
        BottomInfoController.Instance.ShowDebugText("Google - Start"); 
        AuthManager.OnShowInfo?.Invoke("Google - Start");
        Debug.Log("Google - Start");
        InitFirebase();
        //OnSignInRequested();
        StartCoroutine(DelayedSignIn());
    }

    // USE LATER FOR LINKING ACCOUNTS
    private void LinkAccounts(Credential credential)
    {
        user.LinkWithCredentialAsync(credential).ContinueWithOnMainThread(linkTask =>
        {
            if (linkTask.IsCompleted && !linkTask.IsFaulted)
            {
                BottomInfoController.Instance.ShowDebugText("Google account linked with Firebase.");
            }
        });
    }

    private IEnumerator DelayedSignIn()
    {
        while (!firebaseresolved)
        {
            AuthManager.OnShowInfo?.Invoke("Firebase Not resolved wait 1s");
            yield return new WaitForSeconds(1);
        }
        //GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnGoogleAuthFinished);
        //yield return new WaitForSeconds(1);
        OnSignInRequested();
    }

    private void InitFirebase()
    {
        Debug.Log("GoogleSignInManager InitFirebase");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies available.");
                BottomInfoController.Instance.ShowDebugText("GoogleSignInManager - Firebase initialized");
                firebaseresolved = true;
                //OnSignInRequested();
            }
            else
            {
                Debug.LogError($"Firebase dependencies not available: {task.Result}");
                BottomInfoController.Instance.ShowDebugText($"Firebase error: {task.Result}");
            }
        });
    }

    private void SilentSignIn()
    {
        BottomInfoController.Instance.ShowDebugText("Google - SilentSignIn.");
        Debug.Log("Attempting silent sign-in...");
        try
        {
            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnGoogleAuthFinished);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SignInSilently Exception: {ex.Message}");
            BottomInfoController.Instance.ShowDebugText($"SignInSilently Exception: {ex.Message}");
        }
    }

    public void OnSignInRequested()
    {
        Debug.Log("GoogleSignInManager OnSignInRequested");
        BottomInfoController.Instance.ShowDebugText("Google - OnSignInRequested");

        try
        {
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnGoogleAuthFinished);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SignIn Exception: {ex.Message}");
            BottomInfoController.Instance.ShowDebugText($"SignIn Exception: {ex.Message}");
        }
    }

    private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
    {
        BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished status:"+task.Status);
        if (task.IsFaulted)
        {
            Debug.Log("Fault");
            BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Fault "+task.Exception);
            return;
        }
        if(task.IsCanceled)
        {
            Debug.Log("Canceled");
            BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Canceled");
            return;
        }
        BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Successful");
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Fault Credential");
                Debug.Log("Fault Credential");
                return;
            }
            if (task.IsCanceled)
            {
                BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Canceled Credential");
                Debug.Log("Canceled Credential");
                return;
            }
            user = auth.CurrentUser;
            BottomInfoController.Instance.ShowDebugText("SignInWithCredentialAsync Successful - Name:"+user.DisplayName+" ID: "+user.UserId);

        }
        );


    }
}
