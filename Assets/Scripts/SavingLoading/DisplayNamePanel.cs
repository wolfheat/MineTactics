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
    private string startName = "";

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

    private void OnEnable()
    {
        startName = SavingUtility.gameSettingsData.PlayerName.ToString();
    }


    public void InitilizeDisplayNamePanel(Firebase.Auth.FirebaseUser user)
    {
        username_field.text = user.DisplayName;
        errorMessageText.text = "Pick your Display Name. Your name will be visible to others if you create levels.";
    }
    
    public void InitilizeDisplayNamePanel(string initName)
    {
        username_field.text = initName;
    }

    public void OnCancelUserName()
    {
        // Keep the user name supplied by facebook
        Debug.Log("Keeping default User name");
        gameObject.SetActive(false);
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
        if(startName != currentUserName) {
            AuthManager.Instance.UpdateFirebaseUserName(currentUserName);
            PanelController.Instance.ShowLoaderPanelChangeDisplayName();
            SavingUtility.Instance.UpdatePlayerNameAndSaveAllDataToFile(currentUserName);
            Debug.Log("Changed User name to "+currentUserName);
        }
        gameObject.SetActive(false);
    }

    private IEnumerator DelayedStartGame()
    {
        Debug.Log("Delayed Start Game Display Name changed");
        yield return new WaitForSeconds(0.4f);
    }

    private bool ValidateUserName(string currentUserName) => currentUserName.All(c => Char.IsDigit(c) || Char.IsLetter(c));
}
