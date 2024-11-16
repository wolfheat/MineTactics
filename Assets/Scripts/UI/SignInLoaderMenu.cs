using UnityEngine;

public class SignInLoaderMenu : MonoBehaviour
{
    public static SignInLoaderMenu Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;
    }

    private void OnSuccessfulLogIn(string obj)
    {
        Debug.Log("Signed In success");
        gameObject.SetActive(false);
    }
}
