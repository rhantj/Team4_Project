using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundManagerConfig : ServiceConfig<SoundManager>
{
    [field: SerializeField] public GameObject SoundPlayer;
    [field: SerializeField] public List<AudioClip> SoundClips = new();
}

public class SoundManager : MonoBehaviour, IService
{
    private GameObject m_SoundPlayer;
    private List<AudioClip> m_SoundClips = new();
    private Dictionary<string, AudioClip> m_ClipDictionary = new();

    private AudioSource m_CurrentAudio;

    private GameObjectPoolingService m_PoolingService;

    public float MasterVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public AudioSource CurrentAudio => m_CurrentAudio;

    public void Configure(IServiceConfig iConfig)
    {
        m_SoundClips.Clear();
        if (iConfig is SoundManagerConfig cfg && cfg != null)
        {
            m_SoundPlayer = cfg.SoundPlayer;
            m_SoundClips = cfg.SoundClips;
        }

        foreach (var c in m_SoundClips)
            m_ClipDictionary[c.name] = c;
    }

    public AudioClip GetClip(string name)
    {
        return m_ClipDictionary.TryGetValue(name, out var clip) ? clip : null;
    }

    public void CheckServicies()
    {
        if (!m_PoolingService)
            m_PoolingService = GameManager.Instance.GetService<GameObjectPoolingService>();
    }

    public void PlaySound(string name, Vector3 pos, Quaternion rot)
    {
        CheckServicies();

        var speaker = m_PoolingService.GetOrCreateGameObject(m_SoundPlayer);
        speaker.transform.SetPositionAndRotation(pos, rot);
        var audioSource = speaker.GetComponent<AudioSource>();

        audioSource.clip = GetClip(name);
        audioSource.volume = 1f;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.Play();

        StartCoroutine(Co_DespawnSoundPlayer(audioSource));
    }

    public void PlayBGM(string name, Vector3 pos, Quaternion rot)
    {
        CheckServicies();

        if (m_CurrentAudio != null)
            m_CurrentAudio.Stop();
        var bgmPlayer = m_PoolingService.GetOrCreateGameObject(m_SoundPlayer);
        bgmPlayer.transform.SetPositionAndRotation(pos, rot);
        var audioSource = bgmPlayer.GetComponent<AudioSource>();
        m_CurrentAudio = audioSource;

        audioSource.volume = 1f;
        audioSource.clip = GetClip(name);
        audioSource.spatialBlend = 0.0f; // 2D sound
        audioSource.loop = true;
        audioSource.Play();
    }

    IEnumerator Co_DespawnSoundPlayer(AudioSource aus)
    {
        while (aus.isPlaying)
            yield return null;

        m_PoolingService.ReturnOrDestroyGameObject(aus.gameObject);
    }
}