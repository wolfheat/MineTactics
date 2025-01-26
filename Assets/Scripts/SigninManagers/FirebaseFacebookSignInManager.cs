using System;
using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;
using Firebase.Extensions;

public class FirebaseFacebookSignInManager : MonoBehaviour
{


    public static FirebaseFacebookSignInManager Instance { get; private set; }

    private Firebase.Auth.FirebaseAuth auth;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance; ;

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void RequestSignInWithFacebook()
    {
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);

    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log("PLayer is logged in with ID: "+aToken.UserId);

            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log("Permission:"+perm);
            }
            FirebaseFacebookAuth(aToken.TokenString);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void FirebaseFacebookAuth(string accessToken)
    {
        Firebase.Auth.Credential credential =
    Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
        //auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
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

            Firebase.Auth.FirebaseUser user = task.Result;
            Debug.Log("Facebook - Player Logged in as: "+user.DisplayName+" ID:"+user.UserId);

            AuthManager.Instance.SetCredentialsAndLoadMainGame(user);

        });
    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    private void LogOut()
    {
        // Loggin out player

        auth.SignOut();
        Debug.Log("Player signed out from facebook.");
    }
}
