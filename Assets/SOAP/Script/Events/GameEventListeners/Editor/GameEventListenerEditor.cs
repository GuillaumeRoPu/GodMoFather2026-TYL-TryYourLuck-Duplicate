// Editor/GameEventListenerDrawer.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(GameEventListenerBase), true)]
public class GameEventListenerEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        SerializedProperty eventProperty = serializedObject.FindProperty("Event");
        SerializedProperty responseProperty = serializedObject.FindProperty("Response");

        Type concreteEventType = ResolveExpectedEventType(target.GetType());

        var eventField = new ObjectField(eventProperty.displayName)
        {
            objectType = concreteEventType,
            bindingPath = eventProperty.propertyPath
        };

        eventField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != null && !concreteEventType.IsAssignableFrom(evt.newValue.GetType()))
            {
                eventField.value = evt.previousValue;
                return;
            }

            eventProperty.objectReferenceValue = evt.newValue as ScriptableObject;
            eventProperty.serializedObject.ApplyModifiedProperties();
        });

        var responseField = new PropertyField(responseProperty);

        container.Add(eventField);
        container.Add(responseField);

        return container;
    }

    private static Type ResolveExpectedEventType(Type listenerConcreteType)
    {
        Type currentType = listenerConcreteType;

        while (currentType != null && currentType != typeof(GameEventListenerBase))  
        {
            if (currentType == typeof(GameEventListener))
                return ResolveConcreteEventType(typeof(GameEvent));

            if (currentType.IsGenericType)
            {
                Type[] typeArguments = currentType.GetGenericArguments();

                Type matchingEventGenericDefinition = typeArguments.Length switch
                {
                    1 => typeof(GameEvent<>),
                    2 => typeof(GameEvent<,>),
                    3 => typeof(GameEvent<,,>),
                    4 => typeof(GameEvent<,,,>),
                    _ => null
                };

                if (matchingEventGenericDefinition == null)
                    break;

                Type constructedEventType = matchingEventGenericDefinition.MakeGenericType(typeArguments);
                return ResolveConcreteEventType(constructedEventType);
            }

            currentType = currentType.BaseType;
        }

        return typeof(GameEventBase);
    }
    
    private static Type ResolveConcreteEventType(Type abstractEventType)
    {
        IList<Type> derivedTypes = TypeCache.GetTypesDerivedFrom(abstractEventType);

        foreach (Type derivedType in derivedTypes)
        {
            if (!derivedType.IsAbstract && abstractEventType.IsAssignableFrom(derivedType))
                return derivedType;
        }

        return abstractEventType;
    }
}
#endif