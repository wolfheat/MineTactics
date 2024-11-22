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

    private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void SetUserInfoFromFirebaseUser(Firebase.Auth.FirebaseUser user)
	{
		userName = user.DisplayName;
		email = user.Email;
		uid = user.UserId;
	}


}
