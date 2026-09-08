// GameEventListenerBase.cs
using UnityEngine;
using UnityEngine.Events;

public abstract class GameEventListenerBase : MonoBehaviour{}
public abstract class GameEventListener : GameEventListenerBase
{
    [SerializeField] private GameEvent Event;
    public UnityEvent Response;

    private void OnEnable()  => Event?.RegisterListener(this);
    private void OnDisable() => Event?.UnregisterListener(this);
    public void OnEventRaised() => Response?.Invoke();
}

public abstract class GameEventListener<T0> : GameEventListenerBase
{
    [SerializeField] private GameEvent<T0> Event;
    public UnityEvent<T0> Response;

    private void OnEnable()  => Event?.RegisterListener(this);
    private void OnDisable() => Event?.UnregisterListener(this);
    public void OnEventRaised(T0 arg0) => Response?.Invoke(arg0);
}

public abstract class GameEventListener<T0, T1> : GameEventListenerBase
{
    [SerializeField] private GameEvent<T0, T1> Event;
    public UnityEvent<T0, T1> Response;

    private void OnEnable()  => Event?.RegisterListener(this);
    private void OnDisable() => Event?.UnregisterListener(this);
    public void OnEventRaised(T0 arg0, T1 arg1) => Response?.Invoke(arg0, arg1);
}

public abstract class GameEventListener<T0, T1, T2> : GameEventListenerBase
{
    [SerializeField] private GameEvent<T0, T1, T2> Event;
    public UnityEvent<T0, T1, T2> Response;

    private void OnEnable()  => Event?.RegisterListener(this);
    private void OnDisable() => Event?.UnregisterListener(this);
    public void OnEventRaised(T0 arg0, T1 arg1, T2 arg2) => Response?.Invoke(arg0, arg1, arg2);
}

public abstract class GameEventListener<T0, T1, T2, T3> : GameEventListenerBase
{
    [SerializeField] private GameEvent<T0, T1, T2, T3> Event;
    public UnityEvent<T0, T1, T2, T3> Response;

    private void OnEnable()  => Event?.RegisterListener(this);
    private void OnDisable() => Event?.UnregisterListener(this);
    public void OnEventRaised(T0 arg0, T1 arg1, T2 arg2, T3 arg3) => Response?.Invoke(arg0, arg1, arg2, arg3);
}

