using UnityEngine;

public class Timer : MonoBehaviour
{
    float startTime;
    int lastValue = 0;
    [SerializeField] DigiDisplay display;
    public bool Paused { get; set; }

    public static Timer Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartTimer()
    {
        startTime = Time.time;
        Paused = false;
    }
    public void Pause()
    {
        Paused = true;
    }
    public void ResetCounterAndPause()
    {
        display.ShowValue(0);
        Paused = true;
    }

    private void Update()
    {
        if (Paused) return;

        float timeElapsed = Time.time - startTime;

        int seconds = (int)timeElapsed;

        if (seconds != lastValue)
        {
            lastValue = seconds;
            display.ShowValue(seconds);
        }
    }

}
