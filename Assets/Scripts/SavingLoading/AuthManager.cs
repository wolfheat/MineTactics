using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
public class AuthManager : MonoBehaviour
{
    FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

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
