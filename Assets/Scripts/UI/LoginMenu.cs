using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI username_field;
    [SerializeField] TMP_InputField username_field;
    [SerializeField] TMP_InputField email_field;
    [SerializeField] TMP_InputField password_field;
    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] Button submitButton;
    [SerializeField] Button adam;

    private void Start()
    {
    }
    private void OnEnable()
    {
        //Debug.Log("Register for Success Methods from "+this.name);
        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;
        AuthManager.OnSuccessfulCreation += OnSuccessfulCreation;
        AuthManager.RegisterAttemptFailed += OnRegisterFailed;
        AuthManager.LoginAttemptFailed += OnLoginFailed;
        Inputs.Instance.Controls.UI.TAB.performed += OnTAB;
        Inputs.Instance.Controls.UI.Enter.performed += OnEnter;

        EventSystem.current.SetSelectedGameObject(username_field.gameObject);
    }

    private void OnEnter(InputAction.CallbackContext context)
    {
        Debug.Log("ENTER");
        if (EventSystem.current.currentSelectedGameObject.name == "Password")
        {
            Debug.Log("SUMBMIT");
            submitButton.onClick?.Invoke();
        }
    }
    private void OnTAB(InputAction.CallbackContext context)
    {
        Debug.Log("TAB WHEN EventSystem.current.currentSelectedGameObject.name ="+ EventSystem.current.currentSelectedGameObject.name);
        // IF Current Active is email go to Pass, iff pass go to Submit
        if(EventSystem.current.currentSelectedGameObject.name == "Name")
        {
            Debug.Log("Go To Pass");
            EventSystem.current.SetSelectedGameObject(password_field.gameObject);
        }else if(EventSystem.current.currentSelectedGameObject.name == "Password")
        {
            Debug.Log("Go To Submit");
            EventSystem.current.SetSelectedGameObject(submitButton.gameObject);
        }
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
        //Debug.Log("UN-Register for Success Methods from "+this.name);
        AuthManager.OnSuccessfulLogIn -= OnSuccessfulLogIn;
        AuthManager.OnSuccessfulCreation -= OnSuccessfulCreation;
        AuthManager.RegisterAttemptFailed -= OnRegisterFailed;
        AuthManager.LoginAttemptFailed -= OnLoginFailed;
        Inputs.Instance.Controls.UI.TAB.performed -= OnTAB;
        Inputs.Instance.Controls.UI.Enter.performed -= OnEnter;

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

    public void OnTryLoginAdam() => AuthManager.Instance.SignInPlayerEmailAndPassword("adam@adam.com", "adam1234");
    public void OnTryLogin()
    {
        Debug.Log("Name length = "+username_field.text.Length+ "Pass length = " + password_field.text.Length);
        Debug.Log("Player is trying to log in with name: "+username_field.text+" password: "+password_field.text);
        //AuthManager.Instance.SignInPlayerWithUserNameAndPassword(username_field.text,password_field.text);
        errorMessage.text = "Trying to Log In";
        AuthManager.Instance.SignInPlayerEmailAndPassword(email_field.text,password_field.text);
    }
    public void OnTryRegister()
    {
        Debug.Log("Player is trying to register with name: "+username_field.text+" password: "+password_field.text);
        errorMessage.text = "Trying to Register";
        USerInfo.Instance.userName = username_field.text;
        AuthManager.Instance.RegisterPlayerWithEmailAndPassword(username_field.text,email_field.text,password_field.text, errorMessage);
    }
    public void ShowError()
    {
        errorMessage.text = "Unable to Create User";
    }
    public void CloseMenu() => gameObject.SetActive(false);
}
