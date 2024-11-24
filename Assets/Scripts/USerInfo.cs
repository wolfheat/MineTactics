using System;
using UnityEngine;


public enum GameType{Normal,Loaded}
public class USerInfo : MonoBehaviour
{

	public string userName = "";
	public string email = "";
	public string uid = "00";
    internal string levelID;

	public GameType currentType = GameType.Normal;

    public static USerInfo Instance { get; private set; }
	public int BoardSize { get; internal set; } = 6;
	public int Sensitivity { get; internal set; } = 15;
	public float SensitivityMS => Sensitivity / 100f;

	public bool UsePending { get; internal set; } = false;

    public bool IsPlayerLoggedIn { get; internal set; } = false;

    private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		SavingUtility.LoadingComplete += SetDataFromSaveFile;
	}

    private void SetDataFromSaveFile()
    {
		Debug.Log("Setting data from Saved Settings File");
        Sensitivity = SavingUtility.gameSettingsData.TouchSensitivity;
		BoardSize = SavingUtility.gameSettingsData.BoardSize;
		UsePending = SavingUtility.gameSettingsData.UsePending;
    }

    public void SetUserInfoFromFirebaseUser(Firebase.Auth.FirebaseUser user)
	{
        Debug.Log("Setting data from Firebase User File");
        userName = user.DisplayName;
		email = user.Email;
		uid = user.UserId;
		IsPlayerLoggedIn = true;

    }


}
