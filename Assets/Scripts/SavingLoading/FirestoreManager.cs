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
    public string Collection { get; set; } = "none";
}

[FirestoreData]
public class LevelDataCollection
{
    // Collect 100 levels in each Collection or any amount?
    //[FirestoreProperty] public string[] Level { get; set; } // The actual Level string
    [FirestoreProperty] public string[] Level { get; set; } // The actual Level string
    [FirestoreProperty] public string[] LevelId { get; set; }
    [FirestoreProperty] public string[] CreatorId { get; set; }
    [FirestoreProperty] public string[] Status { get; set; } // Pending, Approved, Declined
    [FirestoreProperty] public float[] DifficultyRating { get; set; }
    [FirestoreProperty] public int[] Upvotes { get; set; }
    [FirestoreProperty] public int[] Downvotes { get; set; }
    [FirestoreProperty] public int[] PlayCount { get; set; }
    public DateTime LastDownload { get; set; }
    public string CollectionName { get; set; } = "";
}

public class FirestoreManager : MonoBehaviour
{
    FirebaseFirestore db;
    public static FirestoreManager Instance { get; private set; }
    public static Action<string> LoadComplete;
    public LevelData LevelData { get; private set; }
    public HashSet<string> SentLevels { get; private set; } = new HashSet<string>();
    public List<LevelData> ActiveChallengeLevels { get; private set; } = new List<LevelData>();
    public List<LevelData> LocalCollectionList { get; private set; } = new List<LevelData>();
    public int LoadedAmount => ActiveChallengeLevels.Count;

    public LevelDataCollection ReuploadCollection { get; private set; }
    public Dictionary<string, LevelDataCollection> LevelDataCollections { get; set; } = new();

    public static Action<string> SubmitLevelAttemptFailed;
    public static Action<string> SubmitNameFailed;
    public static Action SubmitLevelAttemptSuccess;
    public static Action SubmitNameAttemptSuccess;
    public static Action OnSubmitLevelStarted;

    public static Action<int> OnSuccessfulLoadingOfLevels;
    public static Action<int> OnLevelCollectionLevelsDownloaded;
    public static Action<LevelDataCollection,string> OnSuccessfulLoadingOfCollection;
    public static Action<string> OnLevelCollectionLevelsDownloadedFail;
    public static Action<string> OnLevelCollectionLevelsDownloadedFailSendCollection;
    public static Action<string> OnLevelCollectionListItemUpdatedItsCollection;
    public static Action OnLoadLevelStarted;
    public static Action<int> OnLevelCollectionListChange;
    private string latestCollectionName="";

    public bool ReplaceLevelInLevelDataCollections(LevelData data)
    {
        // Get the Colelction
        LevelDataCollection collection = LevelDataCollections[data.Collection];
        if(collection == null) 
            return false;
        
        // Get The Level Index
        int levelIndex = Array.IndexOf(collection.LevelId,data.LevelId);
        if(levelIndex == -1)
            return false;

        // Found the Correct level, now update it
        collection.Upvotes[levelIndex] =data.Upvotes;
        collection.Downvotes[levelIndex] =data.Downvotes;
        collection.PlayCount[levelIndex] =data.PlayCount;
        return true;
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
        Debug.Log("** Initiated FireStoreManager **");
        db = FirebaseFirestore.DefaultInstance;
        //Inputs.Instance.Controls.Main.S.performed += SaveToFile;
    }

    public void LoadDownloadedLevel()
    {
        if(ActiveChallengeLevels.Count > 0)
        {
            Debug.Log("There is Downloaded levels in the list, Load one from there instead of downloading new");
            LoadALevelFromDownloadedLevelsList();
            return;
        }
        else if (LevelData != null) 
        { 
            Debug.Log("** ALL Downloaded Levels have been completed - Was not using a Collection");            
            WasLastInCollectionANDSendUpdateToDatabase(LevelData.Collection);

            // Reload all the active collections
            ReactivateAllActiveCollectionsToChallengeList();
            /*
            if(USerInfo.Instance.ActiveCollections.Count > 0)
            {
                PanelController.Instance.ShowFadableInfo("Reactivating "+USerInfo.Instance.ActiveCollections+" levels!");
                LoadALevelFromDownloadedLevelsList();
            }*/
        }
        SmileyButton.Instance.ShowNormal();
    }

    public void GetRandomLevel(float playerRating)
    {
        if (ActiveChallengeLevels.Count > 0)
        {
            Debug.Log("There is Downloaded levels in the list, not allowed to download new from database");

            return;
        }

        Debug.Log("Get a random Level But none exists - go get one from the open database");
        Debug.Log("Goeas and grabs levels from the database here");
        return;

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
                                ActiveChallengeLevels.Add(level);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Error converting document: " + document.Id + " - " + ex.Message);
                            }
                        }
                        // Save all Recieved levels into the Downloaded List
                        //DownloadedLevels = snapshot.Documents.Select(x=>x.ConvertTo<LevelData>()).ToList();

                        Debug.Log("Downloaded LEvels from the database: "+snapshot.Count);
                        OnSuccessfulLoadingOfLevels?.Invoke(snapshot.Documents.Count());
                        OnLevelCollectionListChange?.Invoke(-1);
                        USerInfo.Instance.Collection = null;
                    }
                    else
                    {
                        Debug.Log("No levels found within the specified range.");
                        OnSuccessfulLoadingOfLevels?.Invoke(0);
                    }
                }
                else
                {
                    Debug.LogError("Error retrieving levels: " + task.Exception);
                }
            });
    }

    public void GetLevelCollectionLatestVersion(string id)
    {
        // Show LoadingPanel here
        db.Collection("LevelCollections").Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Tried to get snapshot for "+id);
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                DocumentSnapshot document = task.Result;
                if (document.Exists)
                {
                    ReuploadCollection = document.ConvertTo<LevelDataCollection>();

                    UpdateAndReupploadCollection(id);
                }
                else
                {
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

    private void UpdateAndReupploadCollection(string collectionName)
    {
        int collectionSize = ReuploadCollection.LevelId.Length;

        LevelDataCollection levelCollection = LevelDataCollections[collectionName];
        for (int i = 0; i < collectionSize; i++)
        {
            ReuploadCollection.PlayCount[i]++;
            ReuploadCollection.Downvotes[i] += levelCollection.Downvotes[i];
            ReuploadCollection.Upvotes[i] += levelCollection.Upvotes[i];
            // Rating etc also?
        }
        // Now send the levelCollection back to db
        ReUpploadLevelCollection(collectionName);
    }

    public void ReUpploadLevelCollection(string collectionName)
    {
        Debug.Log("** ReUpploadLevelCollection");
        Task task = StoreLevelCollectionByName(ReuploadCollection, collectionName);
    }

    public void GetLevelCollection(string id,bool forEditMode = false)
    {
        // Show LoadingPanel here
        //if(!forEditMode)
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
                    levelCollection.LastDownload = System.DateTime.Now;
                    levelCollection.CollectionName = id;
                    OnSuccessfulLoadingOfCollection?.Invoke(levelCollection,id);

                    //
                    List<LevelData> levels = ConvertCollectionToLevels(levelCollection,id);
                    if (forEditMode)
                    {
                        LocalCollectionList.AddRange(levels);
                        OnLevelCollectionLevelsDownloaded?.Invoke(levels.Count);
                    }
                    else
                    {
                        //ActiveChallengeLevels.AddRange(levels);
                        OnSuccessfulLoadingOfLevels?.Invoke(levels.Count);
                        OnLevelCollectionListChange?.Invoke(-1); // Select nothing = -1
                    }
                    latestCollectionName = id;
                    // Save all Recieved levels into the Downloaded List

                    Debug.Log("Downloaded Levels from the collectiondatabase: " + levels.Count);
                    if (forEditMode)
                    {
                        OnLevelCollectionListChange?.Invoke(-1);
                        USerInfo.Instance.Collection = id;
                    }
                }
                else
                {
                    if(!forEditMode)
                        OnSuccessfulLoadingOfLevels?.Invoke(0);
                    OnLevelCollectionLevelsDownloadedFail?.Invoke("Could not find a Collection with this name!");    
                    OnLevelCollectionLevelsDownloadedFailSendCollection?.Invoke(id);    
                }
            }
            else
            {
                Debug.LogError("Error retrieving levels: " + task.Exception);
                OnLevelCollectionLevelsDownloadedFail?.Invoke("Unknown Error Loading the collection");    
            }
        });
    }

    private List<LevelData> ConvertCollectionToLevels(LevelDataCollection levelCollection,string collectionName)
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
                PlayCount = levelCollection.PlayCount[i],
                Collection = collectionName
            });
            //if(length<5)Debug.Log("Adding level " + levelCollection.Level[i]);
        }
        //if(length>0) Debug.Log("Added "+length+" levels.");
        return ans;
    }
    
    private LevelDataCollection ConvertLevelsToCollection(List<LevelData> levels)
    {
        Debug.Log("Converting Levels to LevelCollection");
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
        int loadIndex = USerInfo.Instance.LoadRandom?UnityEngine.Random.Range(0,ActiveChallengeLevels.Count):0;

        LevelData = ActiveChallengeLevels[loadIndex];
        ActiveChallengeLevels.RemoveAt(loadIndex);
        Debug.Log("Loaded the first Downloaded Level into LevelData");
        Debug.Log("Leveldata: "+LevelData.Level);


        // Callback with the retrieved level data
        LoadComplete?.Invoke(LevelData.Level);
        OnLevelCollectionListChange?.Invoke(-1);
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

    public void UpdateLevel(Dictionary<string,object> data,string levelId)
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

    public async Task StoreLevelCollectionByName(LevelDataCollection levelDataCollection,string id)
    {
        Debug.Log("Trying to store Level Collection");
        DocumentReference newDocRef = db.Collection("LevelCollections").Document(id);

        OnSubmitLevelStarted?.Invoke();
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
    public bool ReplaceItemInLocalCollection(string val,int index)
    {
        LevelData levelData = CreateLevelDataFromName(val);
        if (levelData == null)
            return false;
        Debug.Log("Replacing index "+index+" in LocalCollectionList "+ LocalCollectionList[index].LevelId + " with new LevelData: "+levelData.LevelId);
        LocalCollectionList[index] = levelData;
        OnLevelCollectionListChange?.Invoke(index);
        return true;
    }
    
    public void AddToLocalCollection(string val)
    {
        LevelData levelData = CreateLevelDataFromName(val);
        if (levelData == null)
            return;

        LocalCollectionList.Add(levelData);
        OnLevelCollectionListChange?.Invoke(-1);
    }

    private LevelData CreateLevelDataFromName(string val)
    {
        // Check if Level has been sent to db allready
        
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

    public void RegisterUserName(string v,string id)
    {
        // Have levelName be random?
        Task task = StoreUsernameById(v,id);
    }

    public void StoreLevelCollection(string collectionName = "BasicCollection")
    {
        LevelDataCollection collection = ConvertLevelsToCollection(LocalCollectionList);
        Task task = StoreLevelCollectionByName(collection, collectionName);

    }
    
    public void StoreSelectedLevelCollection(string collectionName = "BasicCollection", List<LevelData> selectedListItems = null)
    {
        if (selectedListItems.Count == 0)
            return;

        LevelDataCollection collection = ConvertLevelsToCollection(selectedListItems);
        Task task = StoreLevelCollectionByName(collection, collectionName);
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

    public void ClearLocalCollectionList()
    {
        LocalCollectionList.Clear();
        OnLevelCollectionListChange?.Invoke(-1);
    }

    public void UpdateLevelCollection(string collectionName)
    {
        Debug.Log("Updating Level Collection after player completed it all");
        
        // Redownload the original Collection TODO Remove this, then name of the collection should be grabbed from the collection data
        GetLevelCollectionLatestVersion(collectionName);

        // If Played Last Level reload all?
        if(LoadedAmount == 0 && USerInfo.Instance.ActiveCollections.Count > 0)
        {
            Debug.Log("*** NO LEVEL LOADED *** Reload all if available and set Smiley correctly");
            Debug.Log("Re enable "+USerInfo.Instance.ActiveCollections.Count+" Collections");

            ReactivateAllActiveCollectionsToChallengeList();
        }

    }

    public void RemoveLocalCollectionListQuery(List<LevelData> levelsToRemove)
    {
        List<LevelData> newLocalCollection = LocalCollectionList.Except(levelsToRemove).ToList();
        Debug.Log("New Local List bocomes "+newLocalCollection.Count+" long after deleting "+levelsToRemove.Count);
        LocalCollectionList = newLocalCollection;
    }

    internal void SetLocalCollectionListAsFromCollection(string lastSavedCollectionName, List<int> selectedIndexes)
    {
        foreach (var index in selectedIndexes)
        {
            Debug.Log("Setting index "+index+" to bnelong to collection "+lastSavedCollectionName);
            LocalCollectionList[index].Collection = lastSavedCollectionName;
        }
        Debug.Log("The Collection list is X long "+LocalCollectionList.Count);
    }

    internal void SetLocalCollectionListAsFromCollection(string lastSavedCollectionName)
    {
        foreach (var item in LocalCollectionList) { item.Collection = lastSavedCollectionName; }
    }

    internal void ReactivateAllActiveCollectionsToChallengeList()
    {
        Debug.Log("** Reactivating Collections from local");
        bool deletedUnavailablesFromList = false;
        for (int i = USerInfo.Instance.ActiveCollections.Count - 1; i >= 0; i--) {
            string collectionsName = USerInfo.Instance.ActiveCollections[i];
            
            // Read the collection from file if not already in the Collections dictionary
            LevelDataCollection data = LevelDataCollections.ContainsKey(collectionsName)? LevelDataCollections[collectionsName]:SavingUtility.Instance.LoadCollectionDataFromFile(collectionsName);

            if(data == null)
            {
                deletedUnavailablesFromList = true;
                USerInfo.Instance.ActiveCollections.RemoveAt(i);
            }
            else
                ActivateAllLevelsFromCollection(data,collectionsName);
        }
        if(deletedUnavailablesFromList)
            SavingUtility.Instance.SaveAllDataToFile(); // Forces local Save of settings
        // Send UpdateNotice
        OnLevelCollectionListChange?.Invoke(-1);
    }
    internal void RemoveCollectionFromChallengeList(string collectionName)
    {

        DeactivateAllLevelsFromCollection(collectionName);

        // Add to the List of active collections
        if (USerInfo.Instance.ActiveCollections.Contains(collectionName))
        {
            USerInfo.Instance.ActiveCollections.Remove(collectionName);
            USerInfo.Instance.InactiveCollections.Add(collectionName);
            SavingUtility.Instance.SaveAllDataToFile();
        }
        // Send UpdateNotice
        OnLevelCollectionListChange?.Invoke(-1);
    }
        // Adding a collection}
    internal void AddCollectionToChallengeList(LevelDataCollection collection,string collectionName)
    {
        // Adding a collection to the Active Challenge Levels
        ActivateAllLevelsFromCollection(collection, collectionName);

        // Add to the List of active collections
        if (!USerInfo.Instance.ActiveCollections.Contains(collectionName))
            USerInfo.Instance.ActiveCollections.Add(collectionName);
        if (USerInfo.Instance.InactiveCollections.Contains(collectionName))
            USerInfo.Instance.InactiveCollections.Remove(collectionName);
        SavingUtility.Instance.SaveAllDataToFile();

        LevelDataCollections[collectionName] = collection;
        OnLevelCollectionListChange?.Invoke(-1);
        OnLevelCollectionListItemUpdatedItsCollection?.Invoke(collectionName);// Invoke change of Level amount [-1 = select none]
    }

    private void ActivateAllLevelsFromCollection(LevelDataCollection collection, string collectionName)
    {
        Debug.Log("*** ActivateAllLevelsFromCollection collection name = "+collection.CollectionName);
        // First Remove all Levels that are allready in this Level list?
        ActiveChallengeLevels = ActiveChallengeLevels.Where(level => level.Collection != collectionName).ToList(); // Removes all levels from this collection
        
        // Convert to Levels
        List<LevelData> newLevels = ConvertCollectionToLevels(collection, collectionName);

        // Add Levels to the list
        ActiveChallengeLevels.AddRange(newLevels);

        // Also Add these so the ChallengeButtonList can be updated
        LevelDataCollections[collectionName] = collection;
    }
    private void DeactivateAllLevelsFromCollection(string collectionName)
    {
        // First Remove all Levels that are allready in this Level list?
        ActiveChallengeLevels = ActiveChallengeLevels.Where(level => level.Collection != collectionName).ToList(); // Removes all levels from this collection
    }

    internal bool WasLastInCollectionANDSendUpdateToDatabase(string currentCollection)
    {
        if (ActiveChallengeLevels?.Where(x => x.Collection == currentCollection).Count() == 0)
        {
            Debug.Log("Completed last Level from the Collection " + currentCollection);
            //Send request to download latest version and update it before reuploading
            UpdateLevelCollection(currentCollection);
            return false;
        }
        return true;
    }

    internal bool LocalCollectionListHasLevel(string compressed) => LocalCollectionList.Where(x => x.Level == compressed).Count() > 0;
}
