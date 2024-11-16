﻿using UnityEngine;

using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;


[FirestoreData]
public class LevelData
{
    [FirestoreProperty]    public string Level { get; set; } // The actual Level string
    [FirestoreProperty]    public string LevelId { get; set; }
    [FirestoreProperty]    public string CreatorId { get; set; }
    [FirestoreProperty]    public string Status { get; set; } // Pending, Approved, Declined
    [FirestoreProperty]    public float DifficultyRating { get; set; } // 0-3000 ?? 
    [FirestoreProperty]    public int Upvotes { get; set; }
    [FirestoreProperty]    public int Downvotes { get; set; }
    [FirestoreProperty]    public int PlayCount { get; set; }
}

public class FirestoreManager : MonoBehaviour
{
    FirebaseFirestore db;
    public static FirestoreManager Instance { get; private set; }
    public static Action<string> LoadComplete;
    public LevelData LevelData { get; private set; }
    public HashSet<string> SentLevels { get; private set; } = new HashSet<string>();
    public List<LevelData> DownloadedLevels { get; private set; } = new List<LevelData>();
    public int LoadedAmount => DownloadedLevels.Count;

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

    public void GetRandomLevel(float playerRating)
    {
        if(DownloadedLevels.Count > 0)
        {
            Debug.Log("There is Downloaded levels in the list, Load one from there instead of downloading new");
            LoadALevelFromDownloadedLevelsList();
            return;
        }

        float minDifficulty = 0;
        float maxDifficulty = playerRating+500;

        string statusToLoad = "Approved";
        // Select Pending or Approved
        if (PanelController.UsePending)
        {
            //if (UnityEngine.Random.Range(0, 2) == 1)
                statusToLoad = "Pending";
        }
        Debug.Log("No Downloaded Levels in the list, Loading Level with Status: "+statusToLoad+" PendingToggle is set to "+PanelController.UsePending);

        CollectionReference levelsRef = db.Collection("Levels");
        levelsRef
            .WhereEqualTo("Status", statusToLoad)
            .WhereGreaterThanOrEqualTo("DifficultyRating", minDifficulty)
            .WhereLessThanOrEqualTo("DifficultyRating", maxDifficulty)
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    QuerySnapshot snapshot = task.Result;
                    if (snapshot.Count > 0)
                    {
                        foreach (var document in snapshot.Documents)
                        {
                            try
                            {
                                LevelData level = document.ConvertTo<LevelData>();
                                Debug.Log("Read a level from db added to the list: "+level.Level);
                                DownloadedLevels.Add(level);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Error converting document: " + document.Id + " - " + ex.Message);
                            }
                        }
                        // Save all Recieved levels into the Downloaded List
                        //DownloadedLevels = snapshot.Documents.Select(x=>x.ConvertTo<LevelData>()).ToList();

                        Debug.Log("Downloaded LEvels from the database: "+snapshot.Count);
                        // Select a random level from the retrieved documents
                        LoadALevelFromDownloadedLevelsList();                        
                    }
                    else
                    {
                        Debug.Log("No levels found within the specified range.");
                    }
                }
                else
                {
                    Debug.LogError("Error retrieving levels: " + task.Exception);
                }
            });
    }

    private void LoadALevelFromDownloadedLevelsList()
    {
        // Load one level
        LevelData = DownloadedLevels[0];
        DownloadedLevels.RemoveAt(0);
        Debug.Log("Loaded the first Downloaded Level into LevelData");
        Debug.Log("Leveldata: "+LevelData.Level);


        // Callback with the retrieved level data
        LoadComplete?.Invoke(LevelData.Level);
    }

    public void UpdateLevelData(string levelId, float newDifficultyRating)
    {
        DocumentReference levelRef = db.Collection("levels").Document(levelId);

        levelRef.UpdateAsync("difficulty_rating", newDifficultyRating).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Level data updated successfully.");
            }
            else
            {
                Debug.LogError("Error updating level data: " + task.Exception);
            }
        });
    }

    public void UpdatePlayerRating(string playerId, float newRating)
    {
        DocumentReference playerRef = db.Collection("players").Document(playerId);

        playerRef.UpdateAsync("current_rating", newRating).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Player rating updated successfully.");
            }
            else
            {
                Debug.LogError("Error updating player rating: " + task.Exception);
            }
        });
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

    internal void UpdateLevel(Dictionary<string,object> data,string levelId)
    {
        Debug.Log("Requesting to Update Level "+levelId+" with "+data.Count+" values");
        foreach (var item in data)
        {
            Debug.Log(item + ": "+data);
        }
        DocumentReference playerRef = db.Collection("Levels").Document(levelId);

        playerRef.UpdateAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
                Debug.Log("Level updated successfully.");
            else
                Debug.LogError("Error updating level data: " + task.Exception);
        });
    }

    public async Task StoreLevelWithUniqueId(LevelData levelData)
    {
        DocumentReference newDocRef = db.Collection("Levels").Document();
        string autoGeneratedId = newDocRef.Id; // This ID is guaranteed to be unique
        levelData.LevelId = autoGeneratedId;
        // Set data to Firestore
        await newDocRef.SetAsync(levelData);
        Debug.Log("Level with ID " + autoGeneratedId + " written to Database successfully.");
    }
    public void Store(string val, string levelName)
    {
        // Check if Level has been sent to db allready
        if (SentLevels.Contains(val))
        {
            Debug.Log("This Level has allready been sent to the database!");
            return;
        }
        // Add this Level to the list of sent levels so it wont be sent again
        SentLevels.Add(val);

        // Create Level from this data
        LevelData levelData = new LevelData {
            Level = val,
            LevelId = levelName,
            PlayCount = 0,
            Status = "Approved",
            DifficultyRating = 0,
            CreatorId = USerInfo.Instance.uid
        };

        // Have levelName be random?
        Task task = StoreLevelWithUniqueId(levelData);
    }

}
