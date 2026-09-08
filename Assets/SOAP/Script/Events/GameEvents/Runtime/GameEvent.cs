using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public abstract class GameEventBase : ScriptableObject { }


public abstract class GameEvent : GameEventBase
{
    private List<GameEventListener> registeredListeners = new List<GameEventListener>();

    [Button]
    public void Raise()
    {
        for (int i = registeredListeners.Count - 1; i >= 0; i--)
            registeredListeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener) =>
        registeredListeners.Add(listener);

    public void UnregisterListener(GameEventListener listener) =>
        registeredListeners.Remove(listener);
}


public abstract class GameEvent<T0> : GameEventBase
{
    private List<GameEventListener<T0>> registeredListeners = new List<GameEventListener<T0>>();

    protected abstract T0 GetArg0();

    [Button]
    private void Raise() => Raise(GetArg0());

    public void Raise(T0 arg0)
    {
        for (int i = registeredListeners.Count - 1; i >= 0; i--)
            registeredListeners[i].OnEventRaised(arg0);
    }

    public void RegisterListener(GameEventListener<T0> listener) =>
        registeredListeners.Add(listener);

    public void UnregisterListener(GameEventListener<T0> listener) =>
        registeredListeners.Remove(listener);
}


public abstract class GameEvent<T0, T1> : GameEventBase
{
    private List<GameEventListener<T0, T1>> registeredListeners = new List<GameEventListener<T0, T1>>();

    protected abstract T0 GetArg0();
    protected abstract T1 GetArg1();

    [Button]
    private void Raise() => Raise(GetArg0(), GetArg1());

    public void Raise(T0 arg0, T1 arg1)
    {
        for (int i = registeredListeners.Count - 1; i >= 0; i--)
            registeredListeners[i].OnEventRaised(arg0, arg1);
    }

    public void RegisterListener(GameEventListener<T0, T1> listener) =>
        registeredListeners.Add(listener);

    public void UnregisterListener(GameEventListener<T0, T1> listener) =>
        registeredListeners.Remove(listener);
}


public abstract class GameEvent<T0, T1, T2> : GameEventBase
{
    private List<GameEventListener<T0, T1, T2>> registeredListeners = new List<GameEventListener<T0, T1, T2>>();

    protected abstract T0 GetArg0();
    protected abstract T1 GetArg1();
    protected abstract T2 GetArg2();

    [Button]
    private void Raise() => Raise(GetArg0(), GetArg1(), GetArg2());

    public void Raise(T0 arg0, T1 arg1, T2 arg2)
    {
        for (int i = registeredListeners.Count - 1; i >= 0; i--)
            registeredListeners[i].OnEventRaised(arg0, arg1, arg2);
    }

    public void RegisterListener(GameEventListener<T0, T1, T2> listener) =>
        registeredListeners.Add(listener);

    public void UnregisterListener(GameEventListener<T0, T1, T2> listener) =>
        registeredListeners.Remove(listener);
}


public abstract class GameEvent<T0, T1, T2, T3> : GameEventBase
{
    private List<GameEventListener<T0, T1, T2, T3>> registeredListeners = new List<GameEventListener<T0, T1, T2, T3>>();

    protected abstract T0 GetArg0();
    protected abstract T1 GetArg1();
    protected abstract T2 GetArg2();
    protected abstract T3 GetArg3();

    [Button]
    private void Raise() => Raise(GetArg0(), GetArg1(), GetArg2(), GetArg3());

    public void Raise(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        for (int i = registeredListeners.Count - 1; i >= 0; i--)
            registeredListeners[i].OnEventRaised(arg0, arg1, arg2, arg3);
    }

    public void RegisterListener(GameEventListener<T0, T1, T2, T3> listener) =>
        registeredListeners.Add(listener);

    public void UnregisterListener(GameEventListener<T0, T1, T2, T3> listener) =>
        registeredListeners.Remove(listener);
}