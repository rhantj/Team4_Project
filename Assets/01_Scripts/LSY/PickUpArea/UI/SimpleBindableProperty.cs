using System;
using System.Collections.Generic;

public class SimpleBindableProperty<T>
{
    private T _value;
    private Action<T> _onChange;

    public SimpleBindableProperty(T value = default)
    {
        _value = value;
    }

    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;
            _value = value;
            _onChange?.Invoke(_value);
        }
    }

    public void Subscribe(Action<T> observer)
    {
        _onChange += observer;
        observer?.Invoke(_value);
    }

    public void Unsubscribe(Action<T> observer)
    {
        _onChange -= observer;
    }
}
