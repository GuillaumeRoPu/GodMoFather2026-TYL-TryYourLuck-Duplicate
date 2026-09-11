using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationProxy : MonoBehaviour
{
    [SerializeField, SerializedDictionary("Event name", "UnityEvent")]
    private SerializedDictionary<string, UnityEvent> events;
    
    
    public void InvokeEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName]?.Invoke();
        }
        else
        {
            Debug.LogError("[AnimationProxy] No event named " + eventName, this);
        }
        
    }
}


