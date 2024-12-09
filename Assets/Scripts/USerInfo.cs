using System;
using UnityEngine;


public enum GameType{Normal,Challenge,
    Create
}
public class USerInfo : MonoBehaviour
{

	public string userName = "";
	public string email = "";
	public string uid = "00";
    public string levelID;

	public GameType currentType = GameType.Normal;
	public static int EditMode { get; set; } = 0;

    public static USerInfo Instance { get; private set; }
	public int BoardSize { get; set; } = 6;
	public int Sensitivity { get; set; } = 15;
	public float SensitivityMS => Sensitivity / 100f;

	public bool UsePending { get; set; } = false;

    public bool IsPlayerLoggedIn { get; set; } = false;
    public bool WaitForFirstMove { get; set; }
    public string Collection { get; set; }

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

	public static Action BoardSizeChange;
    private void SetDataFromSaveFile()
    {
		Debug.Log("** Setting data from Saved Settings File");
        Sensitivity = SavingUtility.gameSettingsData.TouchSensitivity;
		BoardSize = SavingUtility.gameSettingsData.BoardSize;
		UsePending = SavingUtility.gameSettingsData.UsePending;
		Debug.Log("** UserInfo set Usepending to "+UsePending);
		BoardSizeChange?.Invoke();
    }

    public void SetUserInfoFromFirebaseUser(Firebase.Auth.FirebaseUser user)
	{
        Debug.Log("Setting data from Firebase User File: "+user.DisplayName);
        userName = user.DisplayName;
		email = user.Email;
		uid = user.UserId;
		IsPlayerLoggedIn = true;
    }


}
