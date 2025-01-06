using System;
using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;

public class SavingUtility : MonoBehaviour
{

    private const string PlayerDataSaveFile = "player-data";
    private const string GameSettingsDataSaveFile = "player-settings";
    private string PlayerName = "";
    public static SavingUtility Instance { get; private set; }

    public static Action LoadingComplete;  

    public static GameSettingsData gameSettingsData = new();

    private void Start()
    {
        Debug.Log("** SavingUtility Initiated **");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;    

        
        //StartCoroutine(LoadFromFile());
        // Waits for Firebase To log in before laoding a save file
        AuthManager.OnSuccessfulLogIn += LoadFromFile;
    }

    public void ResetSaveFile()
    {
        Debug.Log("Resetting save data, keeps the game settings data");
        gameSettingsData = new GameSettingsData();
        IDataService dataService = new JsonDataService();
        if (dataService.SaveData(PlayerDataSaveFile, gameSettingsData, false))
            Debug.Log("Player save file was reset: "+PlayerDataSaveFile);
        else
            Debug.LogError("Could not reset file.");
            
        LoadingComplete?.Invoke(); // Call this to update all ingame values
    }

    public void SaveAllDataToFile()
    {
        Debug.Log("ID in SaVe: "+USerInfo.Instance.GetInstanceID());
        Debug.Log("USerInfo.Instance.ActiveCollections = " + USerInfo.Instance.ActiveCollections.Count);
        gameSettingsData.ActiveCollections = USerInfo.Instance.ActiveCollections;
        Debug.Log("gameSettingsData.ActiveCollections = "+ gameSettingsData.ActiveCollections.Count);

        Debug.Log("USerInfo.Instance.InactiveCollections = " + USerInfo.Instance.InactiveCollections.Count);
        gameSettingsData.InactiveCollections = USerInfo.Instance.InactiveCollections;
        Debug.Log("gameSettingsData.InactiveCollections = " + gameSettingsData.InactiveCollections.Count);
        gameSettingsData.UseRotatedExpert = USerInfo.Instance.UseRotatedExpert;


        //SavePlayerDataToFile();
        SaveSettingsDataToFile();
    }    

    public void SaveSettingsDataToFile()
    {
        IDataService dataService = new JsonDataService();
        if (dataService.SaveData(GameSettingsDataSaveFile + "-" + PlayerName, gameSettingsData, false))
            Debug.Log("  Saved settings data in: " + GameSettingsDataSaveFile);
        else
            Debug.LogError("  Could not save file: GameSettingsData");
    }

    public void LoadFromFile()
    {
        // Hold the load so Game has time to load
        //yield return new WaitForSeconds(0.4f);
        // Instead wait for Firebase Data to load? Logged in?
        IDataService dataService = new JsonDataService();
        try
        {
            Debug.Log("** Trying To load data from file. ** ");
            if (AuthManager.Instance.Auth.CurrentUser.IsValid())
            {
                PlayerName = AuthManager.Instance.Auth.CurrentUser.DisplayName;
                Debug.Log("Player "+ PlayerName +" Logged in, load correct settings from file");
            }
            GameSettingsData data = dataService.LoadData<GameSettingsData>(GameSettingsDataSaveFile+"-"+PlayerName, false);
            if (data != null)
            {
                Debug.Log("  PlayerGameData loaded - Valid data!");
                Debug.Log("Read data GameSettingsdata 6x6 ="+data.Records);
                gameSettingsData = data;
                Debug.Log("Set GameSettingsdata 6x6 to "+gameSettingsData.Records);
            }
            else
            {
                gameSettingsData = new GameSettingsData() { PlayerName = PlayerName };
            }            
        }
        catch   
        {
            Debug.Log("  Could not load data, set default: ");
            gameSettingsData = new GameSettingsData() { PlayerName = PlayerName };
        }
        finally
        { 
            Debug.Log(" -- Loading From File Completed --");
            Debug.Log("");
            LoadingComplete?.Invoke();
            //StartCoroutine(KeepTrackOfPlaytime());
        }
    }
    
    public void SaveCollectionDataToFile<T>(T Data, string collectionName)
    {
        IDataService dataService = new JsonDataService();
        if (dataService.SaveData(collectionName, Data, false))
            Debug.Log("Saved collection data for "+collectionName+": ");
        else
            Debug.LogError("Could not save file: "+collectionName);
    }

    public LevelDataCollection LoadCollectionDataFromFile(string collectionName)
    {
        // Hold the load so Game has time to load
        //yield return new WaitForSeconds(0.4f);

        IDataService dataService = new JsonDataService();
        try
        {
            Debug.Log("** Trying To load data from file. **");
            LevelDataCollection data = dataService.LoadData<LevelDataCollection>(collectionName, false);

            // FoolProof Fixing Collection Name not set
            if (data.CollectionName == null || data.CollectionName == "")
                data.CollectionName = collectionName;

            return data;
        }
        catch   
        {
            Debug.Log("  Could not load data, set default: ");
            return null;
        }
    }

    private IEnumerator KeepTrackOfPlaytime()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            gameSettingsData.AddPlayTimeMinutes(1);
            //Debug.Log("Tick ONE minute played Total: "+playerGameData.PlayTime);
        }
    }
        
    public void ClearGameData()
    {
        Debug.Log("Clear Game Data");
        IDataService dataService = new JsonDataService();
        dataService.RemoveData(GameSettingsDataSaveFile);
        // After this clear the game data in game
        gameSettingsData = null;
    }

    internal bool FileExists(string collectionToLoad) => JsonDataService.FileExists(collectionToLoad);
    
}
