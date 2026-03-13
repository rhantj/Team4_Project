using System;
using UnityEngine;

public class TaggedGroupCacheHelper : MonoBehaviour
{
    /// <summary>
    /// Format: void OnChangeTag(string previousTag, string newTag);
    /// </summary>
    private event Action<string, string> OnChangeTag;
    public void RegisterEventOnChangeTag(Action<string, string> callbackOnChangeTag) => OnChangeTag += callbackOnChangeTag;
    public void UnegisterEventOnChangeTag(Action<string, string> callbackOnChangeTag) => OnChangeTag -= callbackOnChangeTag;

    //private void OnEnable()
    //{
    //    OnChangeTag = null;
    //    TryRegisterTaggedGroupCache();
    //}

    //private void OnDisable()
    //{
    //    TryUnregisterTaggedGroupCache();
    //    OnChangeTag = null;
    //}

    private void Start()
    {
        OnChangeTag = null;
        TryRegisterTaggedGroupCache();
    }

    private void OnDestroy()
    {
        TryUnregisterTaggedGroupCache();
        OnChangeTag = null;
    }

    //public void ChangeTag(string newTag)
    //{
    //    string previousTag = tag;
    //    TryUnregisterTaggedGroupCache();
    //    tag = newTag;
    //    TryRegisterTaggedGroupCache();
    //    OnChangeTag?.Invoke(previousTag, newTag);
    //}

    private bool TryRegisterTaggedGroupCache()
    {
        if (!GameManager.Instance.TryGetService(out GameObjectTaggedGroupCacheService service)) return false;
        service.RegisterTaggedGroupCache(this);
        return true;
    }

    private bool TryUnregisterTaggedGroupCache()
    {
        if (!GameManager.Instance.TryGetService(out GameObjectTaggedGroupCacheService service)) return false;
        service.UnregisterTaggedGroupCache(this);
        return true;
    }
}
