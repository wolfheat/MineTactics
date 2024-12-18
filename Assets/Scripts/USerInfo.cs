using System;
using System.Collections.Generic;
using UnityEngine;


public enum GameType{Normal,Challenge,Create,Test}
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
	public bool WaitForFirstMove { get; set; } = true;
    public string Collection { get; set; }
	public List<string> ActiveCollections { get; set; } = new();
	public List<string> InactiveCollections { get; set; } = new();
	public bool LoadRandom { get; internal set; } = true;

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
		Debug.Log(" Setting data from Saved Settings File");
        Sensitivity = SavingUtility.gameSettingsData.TouchSensitivity;
		BoardSize = SavingUtility.gameSettingsData.BoardSize;
		UsePending = SavingUtility.gameSettingsData.UsePending;
		ActiveCollections = SavingUtility.gameSettingsData.ActiveCollections;
		InactiveCollections = SavingUtility.gameSettingsData.InactiveCollections;

        BoardSizeChange?.Invoke();
		// Load Local Collections
		LoadLocalCollections();
    }

    private void LoadLocalCollections()
    {
		Debug.Log("LOCALLY - Load Local Collections");
		foreach (var collection in ActiveCollections)
		{
			Debug.Log("Loading Collection "+collection);
        }
		FirestoreManager.Instance.ReactivateAllActiveCollectionsToChallengeList();
    }

    public void SetUserInfoFromFirebaseUser(Firebase.Auth.FirebaseUser user)
	{
        Debug.Log(" Setting data from Firebase User File: "+user.DisplayName);
        userName = user.DisplayName;
		email = user.Email;
		uid = user.UserId;
		IsPlayerLoggedIn = true;
    }


}
