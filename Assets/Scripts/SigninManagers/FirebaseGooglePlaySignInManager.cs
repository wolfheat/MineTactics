using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using GooglePlayGames;
using UnityEngine;

public class FirebaseGooglePlaySignInManager : MonoBehaviour
{
    private string GooglePlayToken;
    private string GooglePlayError;

    public static FirebaseGooglePlaySignInManager Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;    

    }
    public void Authenticate()
    {

        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Sucess Google Play Games");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authcode: " + code);
                    GooglePlayToken = code;
                });
            }
            else
            {
                GooglePlayError = "Failed to retrieve Google PLay Games Access Code.";
                Debug.LogError("Google Play Error = " + GooglePlayError);
                ProgressPanel.Instance.OnGooglePlayGamesError();
            }
        });
    }

    
}
