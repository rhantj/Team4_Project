using System;
using UnityEngine;

public interface IServiceConfig
{
    public enum ERequiredStatus
    {
        NotRequired,
        NotRequiredButReconfigureIfAvailable,
        NotRequiredButRestartIfAvailable,
        Required,
        RequiredAndReconfigureAlways,
        RequiredAndRestartAlways,
        Shutdown
    }

    public ERequiredStatus Requirement { get; }

    public Type ServiceType { get; }
}

[Serializable]
public abstract class ServiceConfig<T> : IServiceConfig where T : IService
{
    [field: SerializeField] public IServiceConfig.ERequiredStatus Requirement { get; private set; }

    public Type ServiceType => typeof(T);
}
