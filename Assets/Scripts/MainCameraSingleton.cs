using UnityEngine;

public class MainCameraSingleton : MonoBehaviour
{
	public static MainCameraSingleton Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

}
