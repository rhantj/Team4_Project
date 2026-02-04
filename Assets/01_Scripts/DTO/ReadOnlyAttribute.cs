using System;
using UnityEngine;

/// <summary>
/// Readonly attribute to prevent changes in the Unity inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
    public bool IsRuntimeOnlyReadOnly { get; }

    public ReadOnlyAttribute(bool isRuntimeOnlyReadOnly = false)
    {
        IsRuntimeOnlyReadOnly = isRuntimeOnlyReadOnly;
    }
}
