using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username_field;
    [SerializeField] TMP_InputField password_field;
    [SerializeField] TextMeshProUGUI errorMessage;

    private void Start()
    {
    }
    private void OnEnable()
    {
        Debug.Log("Register for Success Methods from "+this.name);
        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;
        AuthManager.OnSuccessfulCreation += OnSuccessfulCreation;
        AuthManager.RegisterAttemptFailed += OnRegisterFailed;
        AuthManager.LoginAttemptFailed += OnLoginFailed;
    }

    private void OnLoginFailed(string error)
    {
        errorMessage.text = error;
    }

    private void OnRegisterFailed(string error)
    {
        errorMessage.text = error;
    }

    private void OnDisable()
    {
        Debug.Log("UN-Register for Success Methods from "+this.name);
        AuthManager.OnSuccessfulLogIn -= OnSuccessfulLogIn;
        AuthManager.OnSuccessfulCreation -= OnSuccessfulCreation;
        AuthManager.RegisterAttemptFailed -= OnRegisterFailed;
        AuthManager.LoginAttemptFailed -= OnLoginFailed;

    }

    private void OnSuccessfulCreation()
    {
        Debug.Log("Successfully Created User from "+this.name);
        CloseMenu();        

    }
    
    private void OnSuccessfulLogIn()
    {
        Debug.Log("Successfully Logged in User from " + this.name);
        CloseMenu();
        // PLayer is logged in just close the menues
    }

    public void OnTryLogin()
    {
        Debug.Log("Player is trying to log in with name: "+username_field.text+" password: "+password_field.text);
        //AuthManager.Instance.SignInPlayerWithUserNameAndPassword(username_field.text,password_field.text);
        errorMessage.text = "Trying to Log In";
        AuthManager.Instance.SignInPlayerWithUserNameAndPassword(username_field.text,password_field.text);
    }
    public void OnTryRegister()
    {
        Debug.Log("Player is trying to register with name: "+username_field.text+" password: "+password_field.text);
        errorMessage.text = "Trying to Register";
        AuthManager.Instance.RegisterPlayerWithUserNameAndPassword(username_field.text,password_field.text, errorMessage);
    }
    public void ShowError()
    {
        errorMessage.text = "Unable to Create User";
    }
    public void CloseMenu() => gameObject.SetActive(false);
}
