using System;
using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SOBuilding m_BuildingData;

    [Header("Status")]
    [SerializeField] private int m_CurrentStepIdx = 0;
    [SerializeField] private int m_CurrentStepItems = 0;
    [SerializeField] private Sprite m_CurrentStepItemSprite;
    [Range(0, 1)]
    [SerializeField] private float m_Progress = 0;

    [Header("Camera Offset")]
    [SerializeField] private Vector3 m_EndLookAtOffset;
    [SerializeField] private Vector3 m_EndCameraOffset;

    [Header("Area")]
    [SerializeField] private ItemIOArea m_InputArea;
    private InventoryExpended inv;

    private Coroutine m_InputCoroutine;
    private readonly WaitForSeconds m_InputDuration = new(0.1f);

    // Events
    public event Action<bool> m_OnInputChanged;
    public event Action m_OnStepChanged;
    public event Action<float> m_OnProgressChanged;
    public event Action m_OnBuildCompleted;
    public event Action<int> m_OnStepCompleted;
    public event Action<Sprite> m_OnSpriteChanged;
    public event Action m_OnCurrentStepItemAdded;

    private SoundManager m_SoundManager;

    private const string m_InputSound = "ITEM_Click_Item_Put";

    void NotifyInput(bool increaseCount = true, bool playSound = true)
    {
        if (playSound)
            m_SoundManager.PlaySound(m_InputSound, transform.position, Quaternion.identity);
        m_OnInputChanged?.Invoke(increaseCount);
    }
    void NotifyBuildComplete() => m_OnBuildCompleted?.Invoke();

    // Property Getters/Setters
    public float Progress => m_Progress;
    public int CurrentStepItems => m_CurrentStepItems;
    public int CurrentRequire = 0;
    public Sprite CurrentItemSprite 
    { 
        get { return m_CurrentStepItemSprite; }
        set
        {
            m_CurrentStepItemSprite = value;
            m_OnSpriteChanged?.Invoke(m_CurrentStepItemSprite);
        }
    }
    public SOBuilding BuildingData => m_BuildingData;

    private void Awake()
    {
        var currentStep = m_BuildingData.Steps[0];
        CurrentItemSprite = currentStep.ItemIcon;
        CurrentRequire = currentStep.RequierAmount;
    }

    private void Start()
    {
        m_SoundManager ??= GameManager.Instance.GetService<SoundManager>();

        m_OnProgressChanged?.Invoke(0f);
    }

    private void OnEnable()
    {
        inv = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryExpended>();

        m_InputArea.m_OnEnterAreaByPlayer += InputItems;
        m_InputArea.m_OnExitAreaByPlayer += ExitArea;

        var cvTransition = Camera.main.GetComponent<CameraViewTransitionBehaviour>();
        cvTransition.MainBuildingTransform = this.transform;
        cvTransition.SetEndLookAtOffset(m_EndLookAtOffset);
        cvTransition.SetEndCameraOffset(m_EndCameraOffset);
    }

    private void OnDisable()
    {
        m_InputArea.m_OnEnterAreaByPlayer -= InputItems;
        m_InputArea.m_OnExitAreaByPlayer -= ExitArea;
    }

    public void InputItems()
    {
        m_InputCoroutine = StartCoroutine(Co_InputItems());
    }

    void ExitArea()
    {
        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    private IEnumerator Co_InputItems()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            if (!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (m_InputArea.IsPlayerEnter)
        {
            var currentStep = m_BuildingData.Steps[m_CurrentStepIdx];

            if (m_CurrentStepIdx >= m_BuildingData.Steps.Count)
            {
                yield return null;
                continue;
            }

            if (inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            if (inv.TryRemoveItemByName(currentStep.StepName))
            {
                m_CurrentStepItems++;
                NotifyInput();
            }

            UpdateProgress();

            if (m_CurrentStepItems >= currentStep.RequierAmount)
            {
                m_CurrentStepIdx++;
                m_CurrentStepItems = 0;

                if(m_CurrentStepIdx >= m_BuildingData.Steps.Count)
                {
                    m_Progress = 1f;
                    m_OnProgressChanged?.Invoke(m_Progress);
                    NotifyBuildComplete();
                    yield break;
                }

                m_OnStepCompleted?.Invoke(m_CurrentStepIdx);
                CurrentItemSprite = m_BuildingData.Steps[m_CurrentStepIdx].ItemIcon;
                NotifyInput(false);

                yield break;
            }

            yield return m_InputDuration;
        }

        m_InputCoroutine = null;
    }

    private void UpdateProgress()
    {
        if (m_BuildingData.Steps.Count == 0) return;

        var totalStep = m_BuildingData.Steps.Count;
        var currentStepProgress = (float)m_CurrentStepItems / m_BuildingData.Steps[m_CurrentStepIdx].RequierAmount;

        m_Progress = (m_CurrentStepIdx + currentStepProgress) / totalStep;
        m_OnProgressChanged?.Invoke(m_Progress);
    }
}
