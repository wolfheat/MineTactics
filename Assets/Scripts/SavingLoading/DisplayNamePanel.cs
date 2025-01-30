using System;
using System.Collections;
using System.Linq;
using i5.Toolkit.Core.RocketChatClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNamePanel : MonoBehaviour
{

    [SerializeField] TMP_InputField username_field;
    [SerializeField] TextMeshProUGUI errorMessageText;
    [SerializeField] Button submitButton;


    public static DisplayNamePanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("<color=red>Test In Red Color</color>");
    }




    public void InitilizeDisplayNamePanel(Firebase.Auth.FirebaseUser user)
    {
        username_field.text = user.DisplayName;
        errorMessageText.text = "Type your wanted Display Name. This name will be visible to others if you create levels.";
    }
    
    public void InitilizeDisplayNamePanel(string initName)
    {
        username_field.text = initName;
    }

    public void OnCancelUserName()
    {
        // Keep the user name supplied by facebook
        Debug.Log("Keeping default User name");
        GotoLoaderPage();
    }
    public void OnConfirmUserName()
    {
        // Validate name is valid

        // TODO MAke it impossible to add invalid chars into the inputbox (Check how its done in email and username input)

        string currentUserName = username_field.text;
        if (!ValidateUserName(currentUserName))
        {
            errorMessageText.text = "Invalid User Name";
            return;
        }
        Debug.Log("Changed User name to "+currentUserName);
        AuthManager.Instance.UpdateFirebaseUserName(currentUserName);
        GotoLoaderPage();
        
    }

    private void GotoLoaderPage()
    {
        PanelController.Instance.ShowLoaderPanelChangeDisplayName();
        gameObject.SetActive(false);
        StartCoroutine(DelayedStartGame());
    }

    private IEnumerator DelayedStartGame()
    {
        Debug.Log("Delayed Start Game Display Name changed");
        yield return new WaitForSeconds(3f);
        AuthManager.Instance.SetCredentialsAndLoadMainGame(AuthManager.Instance.Auth.CurrentUser);
    }

    private bool ValidateUserName(string currentUserName) => currentUserName.All(c => Char.IsDigit(c) || Char.IsLetter(c));
}
