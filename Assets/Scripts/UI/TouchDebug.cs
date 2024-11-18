using TMPro;
using UnityEngine;

public class TouchDebug : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textField;
	public static TouchDebug Instance { get; private set; }

	private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

    public void ShowText(string t) => textField.text = t;


}
