using UnityEngine;
using UnityEngine.Rendering;

public class MainBuildingProgressVisualizer : MonoBehaviour
{
    [field: SerializeField] public Material MatarialToApply { get; private set; }

    [field: Tooltip("Interpolation rate is calculated with Mathf.Exp(-(Time.deltaTime / smoothTime))")]
    [field: SerializeField] public float SmoothTime { get; private set; }

    private Renderer m_TargetRenderer;
    private MaterialPropertyBlock m_PropertyBlock;

    private Building m_Building;

    private static readonly int MinYShaderPropertyID = Shader.PropertyToID("_MinY");
    private static readonly int MaxYShaderPropertyID = Shader.PropertyToID("_MaxY");
    private static readonly int Progress01ShaderPropertyID = Shader.PropertyToID("_Progress01");

    private void Awake()
    {
        m_TargetRenderer = GetComponent<Renderer>();
        m_PropertyBlock = new MaterialPropertyBlock();

        m_Building = GetComponentInParent<Building>();

        if (null != m_TargetRenderer && null != MatarialToApply) m_TargetRenderer.sharedMaterial = MatarialToApply;
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (null == m_TargetRenderer) return;

        m_TargetRenderer.GetPropertyBlock(m_PropertyBlock);

        Bounds bounds = m_TargetRenderer.bounds;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        float interpolationRate = Mathf.Exp(-(Time.deltaTime / SmoothTime));
        float targetProgress = m_Building.Progress;
        float previousVisualizedProgress = m_PropertyBlock.GetFloat(Progress01ShaderPropertyID);
        float nextVisualizedProgress = Mathf.Lerp(targetProgress, previousVisualizedProgress, interpolationRate);

        m_PropertyBlock.SetFloat(MinYShaderPropertyID, minY);
        m_PropertyBlock.SetFloat(MaxYShaderPropertyID, maxY);
        m_PropertyBlock.SetFloat(Progress01ShaderPropertyID, nextVisualizedProgress);

        m_TargetRenderer.SetPropertyBlock(m_PropertyBlock);
    }
}
