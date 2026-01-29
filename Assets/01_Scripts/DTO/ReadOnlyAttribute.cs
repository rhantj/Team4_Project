using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
    public bool IsRuntimeOnlyReadOnly { get; }

    public ReadOnlyAttribute(bool isRuntimeOnlyReadOnly = false)
    {
        IsRuntimeOnlyReadOnly = isRuntimeOnlyReadOnly;
    }
}
