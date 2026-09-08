using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Reference<T>
{
    public bool useConstant;
    public T ConstantValue;
    public Variable<T> Variable;
    
    public T Value
    {
        get => useConstant ? ConstantValue : Variable.Value;
        set
        {
            if (useConstant)
                ConstantValue = value;
            else
                Variable.Value = value;
        } 
    }
}

[System.Serializable]
public class FloatReference : Reference<float>{}
[System.Serializable]
public class IntReference :  Reference<int>{}
[System.Serializable]
public class BoolReference :  Reference<bool>{}
[System.Serializable]
public class StringReference :  Reference<string>{}
[System.Serializable]
public class Vector2Reference :  Reference<Vector2>{}
[System.Serializable]
public class Vector3Reference : Reference<Vector3>{}
