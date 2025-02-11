using TMPro;
using UnityEditor;
using UnityEngine;


public class StartMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI version;
    void Start()
    {
        this.gameObject.SetActive(true);
        SetVersion();
        
    }

    private void OnDrawGizmos()
    {
        SetVersion();
    }

    public void SetVersion()
    {
#if UNITY_EDITOR
        version.text = PlayerSettings.bundleVersion;
#endif
    }
}
