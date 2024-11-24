using System;


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

    internal void AddUsageTimeMinutes(int v)
    {
        UsageTime += v;
    }
    internal void AddPlayTimeMinutes(float time)
    {
        PlayTime += time;
    }



    //public SoundSettings soundSettings = new SoundSettings();
    /*
     * // General Settings - methods
    public void SetSoundSettings(float master, float music, float SFX,bool setFromFile=false)
    {
        soundSettings.MasterVolume = master;
        soundSettings.MusicVolume = music;
        soundSettings.SFXVolume = SFX;
        if(!setFromFile)
            GameSettingsUpdated?.Invoke();
    }*/
}
