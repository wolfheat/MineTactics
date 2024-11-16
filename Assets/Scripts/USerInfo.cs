using UnityEngine;

public class USerInfo : MonoBehaviour
{

	public string userName = "";
	public string email = "";
	public string uid = "00";

	public static USerInfo Instance { get; private set; }

	private void Start()
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
