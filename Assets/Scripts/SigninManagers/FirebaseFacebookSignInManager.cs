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

        Debug.Log(" ** ** FirebaseFacebookSignInManager Awake ** ** ");   

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if(auth==null)
            Debug.Log(" ** ** FirebaseFacebookSignInManager Auth == null ** ** ");

        // Facebook does not work in Editor?
        if (!FB.IsInitialized)
        {
            Debug.Log(" ** ** FirebaseFacebookSignInManager IsInitialized ** ** ");
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            Debug.Log(" ** ** FirebaseFacebookSignInManager NOT IsInitialized ** ** ");
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void RequestSignInWithFacebook(bool OnlyCredentials = false)
    {
        Debug.Log(" ** ** FirebaseFacebookSignInManager RequestSignInWithFacebook ** ** ");
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
        OnlyGetCredentials = OnlyCredentials;
    }
    private bool OnlyGetCredentials = false;
    public static Action<Firebase.Auth.Credential> OnCredentialsRecieved { get; set; }

    private void AuthCallback(ILoginResult result)
    {
        Debug.Log(" ** ** FirebaseFacebookSignInManager AuthCallback ** ** ");

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
        
        // USe the access token to get the firebase credential
        Debug.Log(" ** ** FirebaseFacebookSignInManager FirebaseFacebookAuth ** ** ");
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

        if (OnlyGetCredentials) {
            OnCredentialsRecieved?.Invoke(credential);
            return;
        }
        // If player is already signed in - link the accounts


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
            Debug.Log("");
            Debug.Log("");
            Debug.Log(" NEW INFO ");

            // At this point the player has successfully signed in into Firebase with the credentials gained from exchanging the token from Facebook

            // Only show the change displayname if this player did not exist before?

            // Trying to use the metadata to decide if this is the first time this user sigs in


            Debug.Log("First login is: "+ user.Metadata.CreationTimestamp+" this log in is: "+ user.Metadata.LastSignInTimestamp);
            if(user.Metadata.LastSignInTimestamp - user.Metadata.CreationTimestamp < 10) {
                // Have him select User name at first login
                Debug.Log("Let User pick new displayname");
                PanelController.Instance.ShowChangeDisplayNamePanel(user);
            }
            else {
                Debug.Log("Do not let User pick new displayname");

                AuthManager.Instance.SetCredentialsAndLoadMainGame(user);
                // If This is not first time this user logs in load game
                FirebaseSignInFacebookCredentials(user);
            }
        });
    }

    private void FirebaseSignInFacebookCredentials(Firebase.Auth.FirebaseUser user)
    {
        Debug.Log("Signing into Firebase with Facebook Credentials: "+user.DisplayName+" "+user.UserId+" "+(user.Email??"null"));
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            Debug.Log(" ** ** FirebaseFacebookSignInManager ActivateApp ** ** ");
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
