using System;
using UnityEngine;
using System.Collections;

public class SavingUtility : MonoBehaviour
{

    private const string PlayerDataSaveFile = "/player-data.txt";
    private const string GameSettingsDataSaveFile = "/player-settings.txt";
    public static SavingUtility Instance { get; private set; }

    public static Action LoadingComplete;  

    public static GameSettingsData gameSettingsData;

    private void Start()
    {
        Debug.Log("SavingUtility Started");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;    

        
        StartCoroutine(LoadFromFile());
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
        //SavePlayerDataToFile();
        SaveSettingsDataToFile();
    }    
    public void SaveSettingsDataToFile()
    {
        IDataService dataService = new JsonDataService();
        if (dataService.SaveData(GameSettingsDataSaveFile, gameSettingsData, false))
            Debug.Log("Saved settings data in: " + GameSettingsDataSaveFile);
        else
            Debug.LogError("Could not save file: GameSettingsData");
    }

    public IEnumerator LoadFromFile()
    {
        // Hold the load so Game has time to load
        yield return new WaitForSeconds(0.4f);

        IDataService dataService = new JsonDataService();
        try
        {
            Debug.Log("** Trying To load data from file. **");
            GameSettingsData data = dataService.LoadData<GameSettingsData>(GameSettingsDataSaveFile, false);
            if (data != null)
            {
                Debug.Log("  PlayerGameData loaded - Valid data!");
                gameSettingsData = data;
            }
            else
            {
                gameSettingsData = new GameSettingsData();
            }            
        }
        catch   
        {
            Debug.Log("  Could not load data, set default: ");
            gameSettingsData = new GameSettingsData();
        }
        finally
        { 
            Debug.Log(" -- Loading From File Completed --");
            LoadingComplete?.Invoke();
            //StartCoroutine(KeepTrackOfPlaytime());
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
}
