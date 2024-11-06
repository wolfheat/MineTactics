using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;


    private void Start()
    {
        // Want to use the UI verison here?
        //Inputs.Instance.Controls.UI.Click

    }

    public void Click()
    {
        Debug.Log("Clicking Settings, toggle?");
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
