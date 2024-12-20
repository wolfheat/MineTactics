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

    public List<string> CollectionNames { get; set; } = new();

public List<string> ActiveCollections { get; set; } = new();
    public List<string> InactiveCollections { get; set; } = new();
    public List<float> Records { get; internal set; } = new();
    public List<float> Records3BV { get; internal set; } = new();
    public List<float> OriginalRecords { get; internal set; } = new();
    public List<float> OriginalRecords3BV { get; internal set; } = new();

    public bool AddOriginalRecord(float completionTime, int index, int b3v)
    {
        if (OriginalRecords == null || OriginalRecords.Count == 0)
            OriginalRecords = new List<float> { 0, 0, 0};        
        if (OriginalRecords3BV == null || OriginalRecords3BV.Count == 0)
            OriginalRecords3BV = new List<float> { 0, 0, 0};

        float B3Vs = b3v / completionTime;
        bool newRecord = false;
        if (OriginalRecords[index] == 0 || completionTime < OriginalRecords[index])
        {
            OriginalRecords[index] = completionTime;
            newRecord = true;
        }
        if (OriginalRecords3BV[index] == 0 || B3Vs > OriginalRecords3BV[index])
        {
            OriginalRecords3BV[index] = B3Vs;
            newRecord = true;
        }

        return newRecord;

    }
    public bool AddIfRecord(float completionTime, int index,int b3v)
    {
        float B3Vs = b3v / completionTime;
        if (Records == null || Records.Count == 0)
            Records    = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        if (Records3BV == null || Records3BV.Count == 0)
            Records3BV = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        bool newRecord = false;
        if (Records[index]==0 || completionTime < Records[index])
        {
            Records[index] = completionTime;
            newRecord = true;
        }
        if (Records3BV[index]==0 || B3Vs > Records3BV[index])
        {
            Records3BV[index] = B3Vs;
            newRecord = true;
        }

        return newRecord;
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
