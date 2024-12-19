using System;
using System.Collections.Generic;


[Serializable]
public class SoundSettings
{
    public bool GlobalMaster{ get; set; } = true;
    public bool UseMaster { get; set; } = true;
    public bool UseMusic { get; set; } = true;
    public float MasterVolume { get; set; } = 0.5f;
    public float MusicVolume { get; set; } = 0.2f;
    public bool UseSFX { get; set; } = true;
    public float SFXVolume { get; set; } = 0.4f;
}


[Serializable]
public class GameSettingsData
{
    // General Game Settings
    public int UsageTime { get; set; }
    public float PlayTime { get; set; }
    public int TouchSensitivity { get; set; } = 15;
    public int BoardSize { get; set; } = 6;
    public bool UsePending { get; set; } = true;
    public int NormalWon { get; set; } = 0;
    public int NormalLost { get; set; } = 0;    
    public int ChallengeWon { get; set; } = 0;
    public int ChallengeLost { get; set; } = 0;
    public float Rating{ get; set; } = 999;
    public string PlayerName { get; set; } = "None";
    public DateTime Registration { get; set; } = DateTime.UtcNow;
    
    public List<string> ActiveCollections { get; set; } = new();
    public List<string> InactiveCollections { get; set; } = new();
    public List<float> Records { get; internal set; } = new();
    public List<float> OriginalRecords { get; internal set; } = new();

    public void AddOriginalRecord(float completionTime, int index)
    {
        if (OriginalRecords == null || OriginalRecords.Count == 0)
            OriginalRecords = new List<float> { 0, 0, 0};
if (OriginalRecords[index]==0 || completionTime < OriginalRecords[index])
            OriginalRecords[index] = completionTime;

    }
    public bool AddIfRecord(float completionTime, int index)
    {
        if (Records == null || Records.Count == 0)
            Records = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        if (Records[index]==0 || completionTime < Records[index])
        {
            Records[index] = completionTime;
            return true;
        }
        return false;
    }
    public void AddUsageTimeMinutes(int v)
    {
        UsageTime += v;
    }
    public void AddPlayTimeMinutes(float time)
    {
        PlayTime += time;
    }

    [Serializable]
    public class LocalCollectionsData
    {
        // Downloaded Collections
        public bool UsePending { get; set; } = true;

    }
}
