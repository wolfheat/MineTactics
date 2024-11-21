using UnityEngine;


public enum GameType{Normal,Loaded}
public class USerInfo : MonoBehaviour
{

	public string userName = "";
	public string email = "";
	public string uid = "00";
	public GameType currentType = GameType.Normal;
    internal string levelID;

    public static USerInfo Instance { get; private set; }

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
