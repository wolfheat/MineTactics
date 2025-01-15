using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSignInManager : MonoBehaviour
{
    string GoogleWebAPI = "623452355834-4o5t6qte51usb4a1epa4f83i1vnv3afh.apps.googleusercontent.com";
    private GoogleSignInConfiguration googleSignInConfiguration;
    Firebase.DependencyStatus firebaseStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    // Start is called befo;re the first frame update
    void Awake()
    {
        // Configure webAPI with google
        googleSignInConfiguration = new GoogleSignInConfiguration { 
            WebClientId = GoogleWebAPI, 
            RequestIdToken=true
        };
        Debug.Log("GoogleSignInManager Awake");

        BottomInfoController.Instance.ShowDebugText("GoogleSignInManager Awake run");

    }
    private void Start()
    {
        InitFirebase();
        OnSignInRequested();
    }

    private void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        BottomInfoController.Instance.ShowDebugText("InitFirebase");

    }

    // Update is called once per frame
    void OnSignInRequested()
    {
        BottomInfoController.Instance.ShowDebugText("OnSignInRequested");
        
        Debug.Log("GoogleSignInManager OnSignInRequested");
        GoogleSignIn.Configuration = googleSignInConfiguration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthFinished);
    }

    private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
    {

        BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished");
        if (task.IsFaulted)
        {
            Debug.Log("Fault");
            BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Fault");
            return;
        }
        if(task.IsCanceled)
        {
            Debug.Log("Canceled");
            BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Canceled");
            return;
        }
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Fault Credential");
                Debug.Log("Fault Credential");
                return;
            }
            if (task.IsCanceled)
            {
                BottomInfoController.Instance.ShowDebugText("OnGoogleAuthFinished Canceled Credential");
                Debug.Log("Canceled Credential");
                return;
            }
            user = auth.CurrentUser;

        }
        );


    }
}
