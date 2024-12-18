using UnityEngine;

public class StatsPanel : MonoBehaviour
{

    public static StatsPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void UpdateStats()
    {
        Debug.Log("Updating Stats Panel info");
    }

}
