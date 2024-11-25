using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingleton : MonoBehaviour
{
	public static EventSystemSingleton Instance { get; private set; }
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
