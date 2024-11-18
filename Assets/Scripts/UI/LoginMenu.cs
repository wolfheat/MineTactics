using System;
using TMPro;
using UnityEngine;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username_field;
    [SerializeField] TextMeshProUGUI password_field;
    [SerializeField] TextMeshProUGUI errorMessage;

    private void Start()
    {
        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;
        AuthManager.OnSuccessfulCreation += OnSuccessfulCreation;
    }

    private void OnSuccessfulCreation(string obj)
    {
        CloseMenu();        
        Debug.Log("Successfully Created User, Log in directly?");

    }
    
    private void OnSuccessfulLogIn(string obj)
    {
        CloseMenu();
        // PLayer is logged in just close the menues
    }

    public void OnTryLogin()
    {
        Debug.Log("Player is trying to log in with name: "+username_field.text+" password: "+password_field.text);
        //AuthManager.Instance.SignInPlayerWithUserNameAndPassword(username_field.text,password_field.text);
        errorMessage.text = "Trying to Log In";
        StartCoroutine(AuthManager.Instance.SignInPlayerWithUserNameAndPasswordTest(username_field.text,password_field.text,errorMessage));
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
