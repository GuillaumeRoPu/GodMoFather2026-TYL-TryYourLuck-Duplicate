using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Reference<>), true)]
public class ReferenceDrawer : PropertyDrawer
{
    private Type GetVariableType()
    {
        Type type = fieldInfo.FieldType;

        if (type.IsArray)
            type = type.GetElementType();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            type = type.GetGenericArguments()[0];

        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Reference<>))
            {
                Type t = type.GetGenericArguments()[0];
                Type genericBase = typeof(Variable<>).MakeGenericType(t);

                var variableTypes = TypeCache.GetTypesDerivedFrom(typeof(Variable<>));

                foreach (var concreteType in variableTypes)
                {
                    if (!concreteType.IsAbstract && genericBase.IsAssignableFrom(concreteType))
                        return concreteType;
                }

                return genericBase;
            }
            type = type.BaseType;
        }

        return typeof(ScriptableObject);
    }


    private bool IsCollectionType(SerializedProperty constantValueProp)
    {
        return constantValueProp.isArray;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        SerializedProperty useConstantProp = property.FindPropertyRelative("useConstant");
        SerializedProperty constantValueProp = property.FindPropertyRelative("ConstantValue");
        SerializedProperty variableProp = property.FindPropertyRelative("Variable");

        bool isCollection = IsCollectionType(constantValueProp);
        
        var root = new VisualElement();
        
        var topRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };

        var options = new List<string> { "Constant", "Variable" };
        int currentIndex = useConstantProp.boolValue ? 0 : 1;
        var popup = new PopupField<string>(options, currentIndex)
        {
            style =
            {
                width = StyleKeyword.Auto,
            }
        };


        var constantFieldInline = new PropertyField(constantValueProp, "")
        {
            style =
            {
                flexGrow = 1
            }
        };


        var variableField = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexGrow = 1
            }
        };
        
        var objectField = new ObjectField("")
        {
            objectType = GetVariableType(),
            bindingPath = variableProp.propertyPath,
            style =
            {
                flexGrow = 1
            }
        };

        var persistToggle = new Toggle("Persist")
        {
            tooltip = "Si activé, les modifications faites au runtime sont conservées après la sortie du Play Mode.",
            style =
            {
                marginLeft = 10
            }
        };
        
        persistToggle.labelElement.style.minWidth= 0;
        persistToggle.labelElement.style.width= 38;

        void RefreshPersistToggle(UnityEngine.Object assignedVariable)
        {
            bool hasVariable = assignedVariable != null;
            persistToggle.style.display = hasVariable ? DisplayStyle.Flex : DisplayStyle.None;
            if (!hasVariable) return;

            var vso = new SerializedObject(assignedVariable);
            var persistProp = vso.FindProperty("persistRuntimeChanges");
            if (persistProp != null)
                persistToggle.SetValueWithoutNotify(persistProp.boolValue);
        }

        persistToggle.RegisterValueChangedCallback(evt =>
        {
            var assignedVariable = variableProp.objectReferenceValue;
            if (assignedVariable == null) return;

            var vso = new SerializedObject(assignedVariable);
            var persistProp= vso.FindProperty("persistRuntimeChanges");
            if (persistProp == null) 
                return;

            persistProp.boolValue = evt.newValue;
            vso.ApplyModifiedProperties();
        });

        objectField.RegisterValueChangedCallback(evt => RefreshPersistToggle(evt.newValue));
        RefreshPersistToggle(variableProp.objectReferenceValue);

        variableField.Add(objectField);
        variableField.Add(persistToggle);
        
        
        var constantFieldBlock = new PropertyField(constantValueProp)
        {
            style =
            {
                marginLeft = 4
            }
        };

        void UpdateVisibility(bool useConstant)
        {
            if (useConstant)
            {
                if (isCollection)
                {
                    constantFieldInline.style.display= DisplayStyle.None;
                    constantFieldBlock.style.display= DisplayStyle.Flex;
                }
                else
                {
                    constantFieldInline.style.display= DisplayStyle.Flex;
                    constantFieldBlock.style.display= DisplayStyle.None;
                }
                variableField.style.display = DisplayStyle.None;
            }
            else
            {
                constantFieldInline.style.display= DisplayStyle.None;
                constantFieldBlock.style.display= DisplayStyle.None;
                variableField.style.display= DisplayStyle.Flex;
            }
        }

        popup.RegisterValueChangedCallback(evt =>
        {
            bool isConstant = evt.newValue == "Constant";
            useConstantProp.boolValue = isConstant;
            useConstantProp.serializedObject.ApplyModifiedProperties();
            UpdateVisibility(isConstant);
        });

        root.TrackPropertyValue(useConstantProp, prop =>
        {
            UpdateVisibility(prop.boolValue);
            popup.value = prop.boolValue ? options[0] : options[1];
        });

        topRow.Add(popup);
        topRow.Add(constantFieldInline);
        topRow.Add(variableField);

        root.Add(topRow);
        root.Add(constantFieldBlock);    

        UpdateVisibility(useConstantProp.boolValue);

        return root;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        SerializedProperty useConstantProp = property.FindPropertyRelative("useConstant");
        SerializedProperty constantValueProp = property.FindPropertyRelative("ConstantValue");
        SerializedProperty variableProp = property.FindPropertyRelative("Variable");

        float persistToggleWidth = 58;
        float persistGap = 4;

        Rect popupRect = new Rect(position.x, position.y, 85, position.height);
        Rect persistRect = new Rect(position.xMax - persistToggleWidth, position.y, persistToggleWidth, position.height);

        int currentIndex = useConstantProp.boolValue ? 0 : 1;
        string[] options = { "Constant", "Variable" };

        int newIndex = EditorGUI.Popup(popupRect, currentIndex, options);
        useConstantProp.boolValue = (newIndex == 0);

        if (useConstantProp.boolValue)
        {
            Rect valueRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
            EditorGUI.PropertyField(valueRect, constantValueProp, GUIContent.none, true);
        }
        else
        {
            Type varType = GetVariableType();

            UnityEngine.Object currentVariable = variableProp.objectReferenceValue;
            bool hasVariableBefore = currentVariable != null;

            float objectFieldWidth = hasVariableBefore
                ? position.width - 90 - persistGap - persistToggleWidth
                : position.width - 90;

            Rect valueRect = new Rect(position.x + 90, position.y, objectFieldWidth, position.height);

            EditorGUI.BeginChangeCheck();
            UnityEngine.Object newObj = EditorGUI.ObjectField(valueRect, GUIContent.none, currentVariable, varType, false);
            if (EditorGUI.EndChangeCheck())
            {
                variableProp.objectReferenceValue = newObj;
                currentVariable = newObj;
            }

            if (currentVariable != null)
            {
                var vso = new SerializedObject(currentVariable);
                var persistProp = vso.FindProperty("persistRuntimeChanges");

                if (persistProp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    bool newPersist = EditorGUI.ToggleLeft(persistRect, "Persist", persistProp.boolValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        persistProp.boolValue = newPersist;
                        vso.ApplyModifiedProperties();
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty useConstantProp   = property.FindPropertyRelative("useConstant");
        SerializedProperty constantValueProp = property.FindPropertyRelative("ConstantValue");
        
        if (useConstantProp.boolValue && constantValueProp.isArray)
            return EditorGUI.GetPropertyHeight(constantValueProp, true);

        return EditorGUIUtility.singleLineHeight;
    }
}