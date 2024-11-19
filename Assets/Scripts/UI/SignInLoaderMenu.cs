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

    }
    private void OnEnable()
    {
        AuthManager.OnSuccessfulLogIn += OnSuccessfulLogIn;        
    }
    private void OnDisable()
    {
        AuthManager.OnSuccessfulLogIn -= OnSuccessfulLogIn;
    }

    private void OnSuccessfulLogIn()
    {
        Debug.Log("Signed In success - Close the Loader Menu");
        LevelCreator.Instance.OnPlayerSignedInSuccess();
        gameObject.SetActive(false);
    }
}
