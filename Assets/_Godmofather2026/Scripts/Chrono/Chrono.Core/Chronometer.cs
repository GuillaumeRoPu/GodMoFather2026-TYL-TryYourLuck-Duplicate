using System;
using NaughtyAttributes;
using UnityEngine;

public class Chronometer : MonoBehaviour
{
    public event Action OnStarted;
    public event Action<float> OnTick;
    public event Action<float> OnStopped;

    public event Action<float> OnResume;

    [ShowNativeProperty] public float ElapsedTime { get; private set; }
    [ShowNativeProperty] public bool IsRunning { get; private set; }

    public void StartChrono()
    {
        ElapsedTime = 0f;
        IsRunning = true;
        OnStarted?.Invoke();
    }

    public void ResumeChrono()
    {
        IsRunning = true;
        OnResume?.Invoke(ElapsedTime);
    }

    public void StopChrono()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        OnStopped?.Invoke(ElapsedTime);
    }

    public void ResetChrono()
    {
        IsRunning = false;
        ElapsedTime = 0f;
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        ElapsedTime += Time.deltaTime;
        OnTick?.Invoke(ElapsedTime);
    }
}
