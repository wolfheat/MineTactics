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
    public List<string> ActiveCollections { get; set; } = new();

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
