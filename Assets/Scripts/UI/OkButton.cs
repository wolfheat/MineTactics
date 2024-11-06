using UnityEngine;

public class OkButton : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;

    public void Click()
    {
        Debug.Log("Clicking Ok in Settings, close panel?");
        settingsPanel.SetActive(false);
    }
}
