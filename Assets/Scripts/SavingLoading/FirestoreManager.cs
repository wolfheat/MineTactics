using UnityEngine;

using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;


[FirestoreData]
public class PlayerNameData
{
    [FirestoreProperty]    public string Name { get; set; } // Players Name
}

[FirestoreData]
public class LevelData
{
    [FirestoreProperty]    public string Level { get; set; } // The actual Level string
    [FirestoreProperty]    public string LevelId { get; set; }
    [FirestoreProperty]    public string CreatorId { get; set; }
    [FirestoreProperty]    public string Status { get; set; } // Pending, Approved, Declined
    [FirestoreProperty] public float DifficultyRating { get; set; } = 1000;// 0-3000 ?? 
    [FirestoreProperty] public int Upvotes { get; set; } = 0;
    [FirestoreProperty] public int Downvotes { get; set; } = 0;
    [FirestoreProperty] public int PlayCount { get; set; } = 0;
}

[FirestoreData]
public class LevelDataCollection
{
    // Collect 100 levels in each Collection or any amount?
    [FirestoreProperty] public string[] Level { get; set; } // The actual Level string
    [FirestoreProperty] public string[] LevelId { get; set; }
    [FirestoreProperty] public string[] CreatorId { get; set; }
    [FirestoreProperty] public string[] Status { get; set; } // Pending, Approved, Declined
    [FirestoreProperty] public float[] DifficultyRating { get; set; }
    [FirestoreProperty] public int[] Upvotes { get; set; }
    [FirestoreProperty] public int[] Downvotes { get; set; }
    [FirestoreProperty] public int[] PlayCount { get; set; }
}

public class FirestoreManager : MonoBehaviour
{
    FirebaseFirestore db;
    public static FirestoreManager Instance { get; private set; }
    public static Action<string> LoadComplete;
    public LevelData LevelData { get; private set; }
    public HashSet<string> SentLevels { get; private set; } = new HashSet<string>();
    public List<LevelData> DownloadedLevels { get; private set; } = new List<LevelData>();
    public List<LevelData> LocalCollectionList { get; private set; } = new List<LevelData>();
    public int LoadedAmount => DownloadedLevels.Count;

    public List<LevelData> DownloadedCollection { get; private set; }

    public static Action<string> SubmitLevelAttemptFailed;
    public static Action<string> SubmitNameFailed;
    public static Action SubmitLevelAttemptSuccess;
    public static Action SubmitNameAttemptSuccess;
    public static Action OnSubmitLevelStarted;

    public static Action<bool> OnSuccessfulLoadingOfLevels;
    public static Action<int> OnLevelCollectionLevelsDownloaded;
    public static Action<string> OnLevelCollectionLevelsDownloadedFail;
    public static Action OnLoadLevelStarted;
    public static Action OnLevelCollectionListChange;

    public bool ReplaceLevelInDownloadedCollection(LevelData data)
    {
        for (int i = 0; i < DownloadedCollection.Count; i++)
        {
            if (DownloadedCollection[i].LevelId == data.LevelId)
            {
                DownloadedCollection[i] = data;
                return true;
            }
        }
        return false;
    } 

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

    public void LoadDownloadedLevel()
    {
        if(DownloadedLevels.Count > 0)
        {
            Debug.Log("There is Downloaded levels in the list, Load one from there instead of downloading new");
            LoadALevelFromDownloadedLevelsList();
            return;
        }
        SmileyButton.Instance.ShowNormal();
    }

    public void GetRandomLevel(float playerRating)
    {
        if (DownloadedLevels.Count > 0)
        {
            Debug.Log("There is Downloaded levels in the list, not allowed to download new from database");

            return;
        }
        float minDifficulty = 0;
        float maxDifficulty = playerRating+500;

        string statusToLoad = "Approved";
        // Select Pending or Approved
        if (USerInfo.Instance.UsePending)
        {
            //if (UnityEngine.Random.Range(0, 2) == 1)
                statusToLoad = "Pending";
        }
        Debug.Log("No Downloaded Levels in the list, Loading Level with Status: "+statusToLoad+" PendingToggle is set to "+ USerInfo.Instance.UsePending);

        // Show LoadingPanel here
        OnLoadLevelStarted?.Invoke();

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
                        OnSuccessfulLoadingOfLevels?.Invoke(true);
                        OnLevelCollectionListChange?.Invoke();
                        USerInfo.Instance.Collection = null;
                    }
                    else
                    {
                        Debug.Log("No levels found within the specified range.");
                        OnSuccessfulLoadingOfLevels?.Invoke(false);
                    }
                }
                else
                {
                    Debug.LogError("Error retrieving levels: " + task.Exception);
                }
            });
    }

    public void GetLevelCollection(string id,bool forEditMode = false)
    {
        // Show LoadingPanel here
        if(!forEditMode)
            OnLoadLevelStarted?.Invoke();

        db.Collection("LevelCollections").Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Tried to get snapshot for "+id);
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                DocumentSnapshot document = task.Result;
                if (document.Exists)
                {
                    LevelDataCollection levelCollection = document.ConvertTo<LevelDataCollection>();
                    List<LevelData> levels = ConvertCollectionToLevels(levelCollection);
                    if (forEditMode)
                    {
                        LocalCollectionList = levels;
                        OnLevelCollectionLevelsDownloaded?.Invoke(levels.Count);
                    }
                    DownloadedLevels.AddRange(levels);
                    DownloadedCollection = levels;
                    // Save all Recieved levels into the Downloaded List

                    Debug.Log("Downloaded Levels from the collectiondatabase: " + levels.Count);

                    OnSuccessfulLoadingOfLevels?.Invoke(true);
                    OnLevelCollectionListChange?.Invoke();
                    USerInfo.Instance.Collection = id;
                }
                else
                {
                    if(!forEditMode)
                        OnSuccessfulLoadingOfLevels?.Invoke(false);
                    OnLevelCollectionLevelsDownloadedFail?.Invoke("Could not find a Collection with this name!");    
                }
            }
            else
            {
                Debug.LogError("Error retrieving levels: " + task.Exception);
                OnLevelCollectionLevelsDownloadedFail?.Invoke("Unknown Error Loading the collection");    
            }
        });
    }

    private List<LevelData> ConvertCollectionToLevels(LevelDataCollection levelCollection)
    {
        List<LevelData> ans = new List<LevelData>();

        int length = levelCollection.Level.Length;
        // Separate collection into List
        for (int i = 0; i < length; i++)
        {
            ans.Add(new LevelData() { 
                Level = levelCollection.Level[i],
                LevelId = levelCollection.LevelId[i],
                CreatorId = levelCollection.CreatorId[i],
                Status = levelCollection.Status[i],
                DifficultyRating = levelCollection.DifficultyRating[i],
                Upvotes = levelCollection.Upvotes[i],
                Downvotes = levelCollection.Downvotes[i],
                PlayCount = levelCollection.PlayCount[i]
            });
            Debug.Log("Adding level " + levelCollection.Level[i]);
        }
        return ans;
    }
    
    private LevelDataCollection ConvertLevelsToCollection(List<LevelData> levels)
    {
        LevelDataCollection ans = new();
        int length = levels.Count;

        ans.Level = new string[length];
        ans.LevelId = new string[length];
        ans.CreatorId = new string[length];
        ans.Status = new string[length];
        ans.DifficultyRating = new float[length];
        ans.Upvotes = new int[length];
        ans.Downvotes = new int[length];
        ans.PlayCount = new int[length];

        // Separate collection into List
        for (int i = 0; i < length; i++)
        {
            ans.Level[i] = levels[i].Level;
            ans.LevelId[i] = levels[i].LevelId;
            ans.CreatorId[i] = levels[i].CreatorId;
            ans.Status[i] = levels[i].Status;
            ans.DifficultyRating[i] = levels[i].DifficultyRating;
            ans.Upvotes[i] = levels[i].Upvotes;
            ans.Downvotes[i] = levels[i].Downvotes;
            ans.PlayCount[i] = levels[i].PlayCount;
        }
        return ans;
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
        OnLevelCollectionListChange?.Invoke();
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
                Debug.LogWarning("Error updating level data: " + task.Exception);
        });
    }

    public async Task StoreLevelWithUniqueId(LevelData levelData)
    {
        Debug.Log("Trying to store Level");
        DocumentReference newDocRef = db.Collection("Levels").Document();
        Debug.Log("Trying to store Level "+levelData.Level+" status: "+levelData.Status+" upVotes = "+levelData.Upvotes+" downVotes= "+levelData.Downvotes);
        // Set data to Firestore
        try
        {
            await newDocRef.SetAsync(levelData);

        }
        catch (Exception e)
        {
            Debug.Log("Submitting of the level failed: "+e.Message);
            SubmitLevelAttemptFailed?.Invoke(e.Message);
            return;
        }
        Debug.Log("Level with ID " + newDocRef.Id + " written to Database successfully.");
        SubmitLevelAttemptSuccess?.Invoke();
    }

    public async Task StoreLevelCollectionWithUniqueId(LevelDataCollection levelDataCollection,string id)
    {
        Debug.Log("Trying to store Level Collection");
        DocumentReference newDocRef = db.Collection("LevelCollections").Document(id);
        
        // Set data to Firestore
        try
        {
            await newDocRef.SetAsync(levelDataCollection);
        }
        catch (Exception e)
        {
            Debug.Log("Submitting of the level failed: "+e.Message);
            SubmitLevelAttemptFailed?.Invoke(e.Message);
            return;
        }
        Debug.Log("Submitting of the level success!");
        SubmitLevelAttemptSuccess?.Invoke();
    }

    public async Task StoreUsernameById(string name, string UserID)
    {
        Debug.Log("** STORE USER NAME BY ID STARTED");
        PlayerNameData playerNameData = new PlayerNameData();
        playerNameData.Name = name;

        DocumentReference newDocRef = db.Collection("Players").Document(UserID.ToString());
        // Set data to Firestore
        try
        {
            await newDocRef.SetAsync(playerNameData);
        }
        catch (Exception e)
        {
            Debug.Log("Submitting players name to database failed: "+e.Message);
            SubmitNameFailed?.Invoke(e.Message);
            return;
        }
        SubmitNameAttemptSuccess?.Invoke();
    }

    public void Store(string val)
    {
        LevelData levelData = CreateLevelDataFromName(val);
        if (levelData == null)
            return;

        OnSubmitLevelStarted?.Invoke();
        // Have levelName be random?
        Task task = StoreLevelWithUniqueId(levelData);
    }
    public void ReplaceItemInLocalCollection(string val,int index)
    {
        LevelData levelData = CreateLevelDataFromName(val);
        if (levelData == null)
            return;
        Debug.Log("Replacing index "+index+" in LocalCollectionList "+ LocalCollectionList[index].LevelId + " with new LevelData: "+levelData.LevelId);
        LocalCollectionList[index] = levelData;
        OnLevelCollectionListChange?.Invoke();
    }
    
    public void AddToLocalCollection(string val)
    {
        LevelData levelData = CreateLevelDataFromName(val);
        if (levelData == null)
            return;

        LocalCollectionList.Add(levelData);
        OnLevelCollectionListChange?.Invoke();
    }

    private LevelData CreateLevelDataFromName(string val)
    {
        // Check if Level has been sent to db allready
        if (SentLevels.Contains(val))
        {
            Debug.Log("This Level has allready been sent to the database!");
            PanelController.Instance.ShowInfo("This Level has allready been sent to the database!");
            SubmitLevelAttemptFailed ?.Invoke("Level has allready been Submitted");
            return null;
        }
        // Add this Level to the list of sent levels so it wont be sent again
        SentLevels.Add(val);
        
        DocumentReference newDocRef = db.Collection("Levels").Document(); // Trouble with Id only unique inside each document not both 
        string autoGeneratedId = newDocRef.Id; // This ID is guaranteed to be unique

        // Create Level from this data
        LevelData levelData = new LevelData
        {
            Level = val,
            LevelId = autoGeneratedId, // <- this is overridden in next step to unique value
            PlayCount = 0,
            Status = "Pending",
            Upvotes = 0,
            Downvotes = 0,
            DifficultyRating = 0,
            CreatorId = USerInfo.Instance.userName
        };
        return levelData;
    }

    internal void RegisterUserName(string v,string id)
    {
        // Have levelName be random?
        Task task = StoreUsernameById(v,id);
    }

    internal void StoreLevelCollection(string collectionName = "BasicCollection")
    {
        LevelDataCollection collection = ConvertLevelsToCollection(LocalCollectionList);
        Task task = StoreLevelCollectionWithUniqueId(collection, collectionName);

    }
    
    public void LoadLevelCollection(string collectionName, bool editMode = false)
    {
        GetLevelCollection(collectionName,editMode);
    }
    
    public void LoadLevelCollectionChallengeMode(string name)
    {
        GetLevelCollection(name,false);
    }
    
    public void LoadLevelCollectionPreset(bool editMode = false)
    {
        GetLevelCollection("BasicCollection",editMode);
    }

    internal void ClearLocalCollectionList()
    {
        LocalCollectionList.Clear();
        OnLevelCollectionListChange?.Invoke();
    }
}
