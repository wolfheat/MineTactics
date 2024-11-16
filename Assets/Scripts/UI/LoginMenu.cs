using TMPro;
using UnityEngine;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username_field;
    [SerializeField] TextMeshProUGUI password_field;
    [SerializeField] TextMeshProUGUI errorMessage;

    
    public void OnTryLogin()
    {
        Debug.Log("Player is trying to log in with name: "+username_field.text+" password: "+password_field.text);
    }
    public void OnTryRegister()
    {
        Debug.Log("Player is trying to register with name: "+username_field.text+" password: "+password_field.text);
    }
    public void ShowError()
    {
        errorMessage.text = "Unable to Create User";
    }
    public void CloseMenu() => gameObject.SetActive(false);
}
