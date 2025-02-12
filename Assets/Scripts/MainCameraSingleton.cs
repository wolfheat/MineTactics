using UnityEngine;

public class MainCameraSingleton : MonoBehaviour
{
	[SerializeField] private bool Keep = false; 
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
