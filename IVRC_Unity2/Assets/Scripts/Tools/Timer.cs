using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float startTime;
    private float stopTime;
    private bool running = false;

    // Starts the timer
    public void StartTimer()
    {
        startTime = UnityEngine.Time.time;
        running = true;
    }

    // Stops the timer and returns the elapsed time
    public float StopTimer()
    {
        if (running)
        {
            stopTime = UnityEngine.Time.time;
            running = false;
            return stopTime - startTime;
        }
        return 0f;
    }

    // Gets the total elapsed time without stopping the timer
    public float GetElapsedTime()
    {
        if (running)
        {
            return UnityEngine.Time.time - startTime;
        }
        return stopTime - startTime;
    }

    // Checks if the timer is currently running
    public bool IsRunning()
    {
        return running;
    }
}
