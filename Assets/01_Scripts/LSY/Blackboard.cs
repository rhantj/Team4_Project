using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Blackboard : ISerializationCallbackReceiver,
                          IEnumerable<KeyValuePair<string, Blackboard.IEntry>>,
                          IEnumerable<string>,
                          IEnumerable<Blackboard.IEntry>
{
    public interface IEntry
    {
        Type EntryType { get; }
    }

    [Serializable]
    public class Entry<T> : IEntry
    {
        [field: SerializeField] public T Value { get; set; }
        public T ReadOnlyValue => Value;

        public Entry(T initValue = default)
        {
            Value = initValue;
        }

        public Type EntryType => typeof(T);
    }

    [Serializable]
    public readonly struct KeyEntryPair
    {
        [field: ReadOnly][field: SerializeField] public string Key { get; }
        [field: ReadOnly][field: SerializeReference] public IEntry Entry { get; }

        public KeyEntryPair(string key, IEntry entry)
        {
            Key = key;
            Entry = entry;
        }

        public KeyEntryPair(KeyValuePair<string, IEntry> pair)
        {
            Key = pair.Key;
            Entry = pair.Value;
        }

        public static implicit operator KeyValuePair<string, IEntry>(KeyEntryPair pair) => new KeyValuePair<string, IEntry>(pair.Key, pair.Entry);
        public static implicit operator KeyEntryPair(KeyValuePair<string, IEntry> pair) => new KeyEntryPair(pair);
    }

    private readonly Dictionary<string, IEntry> m_Entries = new Dictionary<string, IEntry>();
    [ReadOnly][SerializeField] public readonly List<KeyEntryPair> m_SerializedEntries;

    public void Set<T>(string key, T value)
    {
        if (m_Entries.TryGetValue(key, out IEntry entry) && entry is Entry<T> entryT) entryT.Value = value;
        else m_Entries[key] = new Entry<T>(value);
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        if (m_Entries.TryGetValue(key, out IEntry entry) && entry is Entry<T> entryT)
        {
            value = entryT.Value;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public void Remove(string key)
    {
        m_Entries.Remove(key);
    }

    public void OnBeforeSerialize()
    {
        m_SerializedEntries.Clear();
        foreach (KeyValuePair<string, IEntry> kv in m_Entries) m_SerializedEntries.Add(new KeyEntryPair(kv.Key, kv.Value));
    }

    public void OnAfterDeserialize()
    {
        m_Entries.Clear();
        foreach (KeyEntryPair ke in m_SerializedEntries) m_Entries[ke.Key] = ke.Entry;
        m_SerializedEntries.Clear();
    }

    public IEnumerator<KeyValuePair<string, IEntry>> GetEnumerator()
    {
        return m_Entries.GetEnumerator();
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return m_Entries.Keys.GetEnumerator();
    }

    IEnumerator<IEntry> IEnumerable<IEntry>.GetEnumerator()
    {
        return m_Entries.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Entries.GetEnumerator();
    }
}
