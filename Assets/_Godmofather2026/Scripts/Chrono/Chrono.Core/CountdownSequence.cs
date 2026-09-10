using System;
using System.Collections;
using UnityEngine;

public class CountdownSequence : MonoBehaviour
{
    public event Action<int> OnCountdownTick;
    public event Action OnCountdownFinished;

    public void PlayCountdown(int secondsCount)
    {
        StartCoroutine(RunCountdown(secondsCount));
    }

    private IEnumerator RunCountdown(int secondsCount)
    {
        for (int remainingSeconds = secondsCount; remainingSeconds > 0; remainingSeconds--)
        {
            OnCountdownTick?.Invoke(remainingSeconds);
            yield return new WaitForSeconds(1f);
        }
        OnCountdownFinished?.Invoke();
    }
}
