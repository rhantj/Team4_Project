using System;
using UnityEngine;

[Serializable]
public struct SerializableNullable<T> where T : struct
{
    [SerializeField] private bool m_HasValue;
    [SerializeField] private T m_Value;

    public SerializableNullable(T value)
    {
        m_HasValue = true;
        m_Value = value;
    }

    public readonly bool HasValue => m_HasValue;

    public readonly T Value
    {
        get
        {
            if (!HasValue) throw new InvalidOperationException("A SerializableNullable object does not have a value.");
            return m_Value;
        }
    }

    public override readonly bool Equals(object obj)
    {
        if (!(obj is SerializableNullable<T> other)) return false;
        else return Equals(other);
    }

    public readonly bool Equals(SerializableNullable<T> other)
    {
        if (!m_HasValue && !other.m_HasValue) return true;
        if (m_HasValue != other.m_HasValue) return false;
        return m_Value.Equals(other.m_Value);
    }

    public override readonly int GetHashCode() => HasValue ? Value.GetHashCode() : 0;
    public readonly T GetValueOrDefault() => HasValue ? Value : default;
    public readonly T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;
    public override readonly string ToString() => m_HasValue ? Value.ToString() : string.Empty;

    public static implicit operator SerializableNullable<T>(T value) => new SerializableNullable<T>(value);
    public static implicit operator SerializableNullable<T>(DBNull _) => new SerializableNullable<T>();
    public static implicit operator T(SerializableNullable<T> value) => value.Value;

    public static bool operator ==(SerializableNullable<T> left, SerializableNullable<T> right) => left.Equals(right);
    public static bool operator !=(SerializableNullable<T> left, SerializableNullable<T> right) => !left.Equals(right);
}
