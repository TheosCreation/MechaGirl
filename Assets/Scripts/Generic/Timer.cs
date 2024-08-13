using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public void StopTimer()
    {
        StopAllCoroutines();
    }

    public void SetTimer(float delay, System.Action callback)
    {
        StartCoroutine(TimerCoroutine(delay, callback));
    }

    private IEnumerator TimerCoroutine(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    public void SetTimer<T>(float delay, System.Action<T> callback, T parameter)
    {
        StartCoroutine(TimerCoroutine(delay, callback, parameter));
    }

    private IEnumerator TimerCoroutine<T>(float delay, System.Action<T> callback, T parameter)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke(parameter);
    }
}