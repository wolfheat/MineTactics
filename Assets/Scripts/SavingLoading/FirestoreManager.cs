using UnityEngine;

using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System;

[FirestoreData]
struct MyData
{
    // get a string, to answer the question in the post
    [FirestoreProperty]
    public string StringValue { get; set; }

    // get an int to demonstrate that it's not string specific
    [FirestoreProperty]
    public int IntValue { get; set; }
}

public class FirestoreManager : MonoBehaviour
{
    FirebaseFirestore db;
    public static FirestoreManager Instance { get; private set; }
    public static Action<string> LoadComplete;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Init FireStoreManager");
        db = FirebaseFirestore.DefaultInstance;
        //Inputs.Instance.Controls.Main.S.performed += SaveToFile;
    }

    public void Load(string id)
    {
        Debug.Log("Firestore Manager Loading Level "+id);
        // The string in read from database
        db.Collection("Levels").Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Could not Load Results from database");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Loading Recieved Result");
                if(!task.Result.Exists)
                {
                    Debug.Log("A Level with ID: "+id+" Does not exist in the Database. Loading Aborted");
                    return;
                }

                Dictionary<string, object> dict = task.Result.ToDictionary();
                string level = dict["val"].ToString();
                //var snapshot = task.Result[0];
                // Do something with snapshot...
                Debug.Log("Send Load COmplete Action Event");
                LoadComplete?.Invoke(level);
            }
        });
    }
    
    public void Store(string val, string levelName)
    {
        // The string in val is written to the database
        //DocumentReference docRef = db.Collection("Levels").Document("ID1");
        //docRef.SetAsync(new Dictionary<string, string> {{ "val", val }}).ContinueWithOnMainThread(task =>
        db.Collection("Levels").Document(levelName).SetAsync(new Dictionary<string, string> {{ "val", val }}).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Updated Level info");

        });

    }

}
