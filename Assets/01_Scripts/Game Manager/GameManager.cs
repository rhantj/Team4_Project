using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: ReadOnly(true)][field: SerializeReference] public List<IServiceConfig> InitialConfigs { get; set; } = new List<IServiceConfig>();
    private readonly Dictionary<Type, IService> m_RegesteredServices = new Dictionary<Type, IService>();

    private void Awake()
    {
        if (null != Instance && this != Instance)
        {
            ForwardConfig(InitialConfigs);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ForwardConfig(InitialConfigs);
        DontDestroyOnLoad(gameObject);
    }

    private static IService InitializeService(IServiceConfig config)
    {
        IService service = Instance.gameObject.AddComponent(config.ServiceType) as IService;
        service?.Configure(config);
        return service;
    }

    private static void TerminateService(IService service)
    {
        if (service is MonoBehaviour) Destroy(service as MonoBehaviour);
    }

    private static void ForwardConfig(List<IServiceConfig> Configs)
    {
        foreach (IServiceConfig config in Configs)
        {
            Type t = config.ServiceType;
            Instance.m_RegesteredServices.TryGetValue(t, out IService service);

            switch (config.Requirement)
            {
                case IServiceConfig.ERequiredStatus.NotRequiredButReconfigureIfAvailable:
                    service?.Configure(config);
                    break;

                case IServiceConfig.ERequiredStatus.NotRequiredButRestartIfAvailable:
                    if (null != service)
                    {
                        TerminateService(service);
                        service = InitializeService(config);
                        Instance.m_RegesteredServices[t] = service;
                    }
                    break;

                case IServiceConfig.ERequiredStatus.Required:
                    if (null == service)
                    {
                        service = InitializeService(config);
                        Instance.m_RegesteredServices.Add(t, service);
                    }
                    break;

                case IServiceConfig.ERequiredStatus.RequiredAndReconfigureAlways:
                    if (null == service)
                    {
                        service = InitializeService(config);
                        Instance.m_RegesteredServices.Add(t, service);
                    }
                    else
                    {
                        service.Configure(config);
                    }
                    break;

                case IServiceConfig.ERequiredStatus.RequiredAndRestartAlways:
                    if (null == service)
                    {
                        service = InitializeService(config);
                        Instance.m_RegesteredServices.Add(t, service);
                    }
                    else
                    {
                        TerminateService(service);
                        service = InitializeService(config);
                        Instance.m_RegesteredServices[t] = service;
                    }
                    break;

                case IServiceConfig.ERequiredStatus.Shutdown:
                    TerminateService(service);
                    Instance.m_RegesteredServices.Remove(t);
                    break;
            }
        }
    }

    public T GetService<T>() where T : MonoBehaviour, IService
    {
        if (!m_RegesteredServices.TryGetValue(typeof(T), out IService iService)) iService = null;
        return iService as T;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Dictionary<Type, bool> configFoundFlags = TypeCache.GetTypesDerivedFrom<IServiceConfig>()
                                                           .Where(t => !t.IsAbstract)
                                                           .ToDictionary(t => t, t => false);

        InitialConfigs.RemoveAll(config => null == config);

        foreach (IServiceConfig config in InitialConfigs)
        {
            Type configType = config.GetType();
            if (configFoundFlags.ContainsKey(configType)) configFoundFlags[configType] = true;
        }

        foreach (KeyValuePair<Type, bool> kv in configFoundFlags)
        {
            if (null == kv.Key || kv.Value) continue;
            if (typeof(IServiceConfig).IsAssignableFrom(kv.Key))
            {
                IServiceConfig config = Activator.CreateInstance(kv.Key) as IServiceConfig;
                if (null != config) InitialConfigs.Add(config);
            }
        }

        InitialConfigs.Sort((left, right) => left.ServiceType.FullName.CompareTo(right.ServiceType.FullName));
    }
#endif
}