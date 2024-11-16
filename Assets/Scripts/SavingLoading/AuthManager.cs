using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Auth;
using System;

public class AuthManager : MonoBehaviour
{
    FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
    FirebaseAuth auth = FirebaseAuth.DefaultInstance;
    public static Action<string> OnSuccessfulLogIn;

    private void Start()
    {
        Firebase.FirebaseApp app;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void CreatePlayerWithUserNameAndPassword(string email, string password)
    {

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            //Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            OnSuccessfulLogIn?.Invoke(result.User.UserId);
        });


    }
    
    public void SignInPlayerWithUserNameAndPassword(string email, string password)
    {

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            LevelCreator.Instance.OnPlayerSignedInSuccess(result.User.DisplayName);
        });


    }

    /*
    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                photoUrl = user.PhotoUrl ?? "";
            }
        }
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
    */
    /*
    public void WriteLevelData(string name, string level)
    {
        DocumentReference docRef = db.Collection("Levels").Document();
        Dictionary<string, object> dictionaryMessage = new Dictionary<string, object>
        {
                { "Time", FieldValue.ServerTimestamp},
                { "Name", name},
                { "Level", level},
                { "User", User.DisplayName}
        };
        docRef.SetAsync(dictionaryMessage).ContinueWithOnMainThread(task => {
            Debug.Log("Write to Firebase");
            saveUtility.SetSavingInfo("Write to Firebase");
        });
    }

    public void ReadLevelData()
    {
        Debug.Log("Read from Firebase");
        CollectionReference usersRef = db.Collection("Levels");
        Query query = usersRef.OrderBy("Time");

        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Read from Firebase,continue");
            QuerySnapshot snapshot = task.Result;
            loadUtility.UpdateLevelListFirebase(snapshot);
        });
    }
    */


}
