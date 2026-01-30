using System;
using UnityEngine;

[Serializable]
public class PauseServiceConfig : ServiceConfig<PauseService>
{
    //
}

public class PauseService : MonoBehaviour, IService
{

    public void Configure(IServiceConfig iConfig)
    {
        //TimeManagementServiceConfig config = iConfig as TimeManagementServiceConfig;
    }

    public int PauseRequestCount { get; private set; }

    private float m_PreviousTimeScale;

    private void OnEnable()
    {
        PauseRequestCount = 0;
    }

    public void RequestPause()
    {
        if (0 == PauseRequestCount) m_PreviousTimeScale = Time.timeScale;
        PauseRequestCount++;
        Time.timeScale = 0f;
    }

    public void RequestResume()
    {
        if (0 == PauseRequestCount) throw new InvalidOperationException("Requested resume while game is running already.");
        PauseRequestCount--;
        if (0 == PauseRequestCount) Time.timeScale = m_PreviousTimeScale;
    }
}
