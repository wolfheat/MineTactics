using System;
using TMPro;
using UnityEngine;

public class LinkCredentials : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI status;
    private void OnEnable()
    {
        FirebaseFacebookSignInManager.OnCredentialsRecieved += OnCredentialsReceived;
        AuthManager.OnLinkSuccess += OnLinkSuccess;
        AuthManager.OnLinkFail += OnLinkFail;

        status.text = "----";
    }
    private void OnDisable()
    {
        FirebaseFacebookSignInManager.OnCredentialsRecieved -= OnCredentialsReceived;
        AuthManager.OnLinkSuccess -= OnLinkSuccess;
        AuthManager.OnLinkFail -= OnLinkFail;
    }

    public void LinkFacebookCredentials()
    {
        Debug.Log("Requesting to link Facebook Account to this User "+AuthManager.Instance.Auth.CurrentUser.DisplayName);

        // Check that player is logged in with password
        if (!PlayerIsLoggedInWithPassword()) {
            Debug.Log("Player is not logged in with password credentials");
            return;
        }

        status.text = "Trying to create Link!";

        // Get Credentials
        FirebaseFacebookSignInManager.Instance.RequestSignInWithFacebook(true);

        // Do the Link

    }

    public void OnCredentialsReceived(Firebase.Auth.Credential credentials)
    {
        status.text = "Facebook Credentials Received!";
        Debug.Log("Link Credentials - Facebook Auth recievied");
        AuthManager.Instance.LinkAccounts(credentials);

    }
        
    public void OnLinkSuccess()
    {
        status.text = "Facebook Linked Successfully!";
        Debug.Log("Link Success");        
    }    
    
    public void OnLinkFail(string message)
    {
        status.text = message;
        Debug.Log("Link Failed: "+message);        
    }

    private bool PlayerIsLoggedInWithPassword()
    {
        return true;
    }
}
