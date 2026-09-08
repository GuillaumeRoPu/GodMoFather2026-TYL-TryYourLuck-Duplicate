#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RuntimeSet<>), true)]
public class RuntimeSetEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        SerializedProperty eventProperty = serializedObject.FindProperty("onItemsListChanged");

        Type exactConcreteEventType = ResolveListEventType(target.GetType());

        var eventField = new ObjectField(eventProperty.displayName)
        {
            objectType = exactConcreteEventType,
            bindingPath = eventProperty.propertyPath
        };

        eventField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue != null && !exactConcreteEventType.IsAssignableFrom(evt.newValue.GetType()))
            {
                eventField.SetValueWithoutNotify(evt.previousValue);
                return;
            }

            eventProperty.objectReferenceValue = evt.newValue as ScriptableObject;
            eventProperty.serializedObject.ApplyModifiedProperties();
        });

        container.Add(eventField);
        
        container.Add(new VisualElement
        {
            style =
            {
                height = 4
            }
        });
        
        var itemsSection = new IMGUIContainer(DrawRuntimeItems);
        container.Add(itemsSection);

        return container;
    }

    private void DrawRuntimeItems()
    {
        FieldInfo itemsField = GetItemsField(target.GetType());
        IList items = itemsField?.GetValue(target) as IList;

        int count = items?.Count ?? 0;
        string header = Application.isPlaying
            ? $"Items (runtime) : {count} élément(s)"
            : "Items (runtime) : visible en Play Mode uniquement";

        EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

        if (!Application.isPlaying || items == null || count == 0)
            return;

        EditorGUI.indentLevel++;
        GUI.enabled = false;

        for (int i = 0; i < items.Count; i++)
        {
            object item = items[i];

            if (item is UnityEngine.Object unityObj)
                EditorGUILayout.ObjectField($"[{i}]", unityObj, unityObj.GetType(), true);
            else
                EditorGUILayout.LabelField($"[{i}]", item?.ToString() ?? "null");
        }

        GUI.enabled = true;
        EditorGUI.indentLevel--;
    }

    private static FieldInfo GetItemsField(Type type)
    {
        while (type != null && type != typeof(ScriptableObject))
        {
            FieldInfo field = type.GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field;

            type = type.BaseType;
        }
        return null;
    }

    private static Type ResolveListEventType(Type runtimeSetConcreteType)
    {
        Type currentType = runtimeSetConcreteType;

        while (currentType != null && currentType != typeof(ScriptableObject))
        {
            if (currentType.IsGenericType &&
                currentType.GetGenericTypeDefinition() == typeof(RuntimeSet<>))
            {
                Type itemType = currentType.GetGenericArguments()[0];
                Type listType = typeof(List<>).MakeGenericType(itemType);
                Type abstractType = typeof(GameEvent<>).MakeGenericType(listType);

                return ResolveConcreteEventType(abstractType);
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