using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    private static LevelTimer _instance;
    public static LevelTimer Instance { get { return _instance; } }

    private bool timerRunning = false;
    public void SetTimerRunning(bool value)
    {
        remainingTime = 60f;
        timerRunning = value;
    }

    private float remainingTime = 60.0f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
        }
    }

    public int GetPoints()
    {
        return Mathf.Clamp((int)(remainingTime / 20) + 1, 0, 3);
    }
}
