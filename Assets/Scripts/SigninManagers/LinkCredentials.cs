using System;
using TMPro;
using UnityEngine;

public class LinkCredentials : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] GameObject emailInput;
    [SerializeField] TextMeshProUGUI emailAlready;

    private void OnEnable()
    {
        AuthManager.OnCredentialsRecieved += OnCredentialsReceived;
        AuthManager.OnLinkSuccess += OnLinkSuccess;
        AuthManager.OnLinkFail += OnLinkFail;

        status.text = "----";
    }
    private void OnDisable()
    {
        AuthManager.OnCredentialsRecieved -= OnCredentialsReceived;
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

    public void RequestLinkEmailCredentials()
    {
        Debug.Log("Show Email");
        emailInput.SetActive(true);
        emailAlready.text = AuthManager.Instance.Auth.CurrentUser.Email;
    }
    public void LinkEmailCredentials(LoginMenu loginMenu)
    {
        Debug.Log("Requesting to link Email to this user"+AuthManager.Instance.Auth.CurrentUser.DisplayName);

        // Check that player is logged in with password
        if (!PlayerIsLoggedInWithPassword()) {
            Debug.Log("Player is not logged in with password credentials");
            return;
        }

        status.text = "Trying to create @ Link!";

        Debug.Log("USer: "+loginMenu.InputName+" pass: "+loginMenu.InputPass);

        // Get Credentials
        AuthManager.Instance.EmailAuth(AuthManager.Instance.Auth.CurrentUser.Email,loginMenu.InputPass);

    }

    public void OnCredentialsReceived(Firebase.Auth.Credential credentials)
    {
        status.text = "Credentials Received!";
        Debug.Log("Link Credentials");
        AuthManager.Instance.LinkAccounts(credentials);

    }
        
    public void OnLinkSuccess()
    {
        status.text = "Linked Successfully!";
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
