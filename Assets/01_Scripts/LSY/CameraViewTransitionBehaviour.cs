using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraViewTransitionBehaviour : MonoBehaviour
{
    [Header("Common")]
    [field: SerializeField] public Transform PlayerCharacterTransform { get; set; }
    [field: SerializeField] public Transform MainBuildingTransform { get; set; }
    [field: SerializeField] public float MinimumDistance { get; set; }
    [field: SerializeField] public float MaximumDistance { get; set; }
    [field: Tooltip("Interpolation rate is calculated with Mathf.Exp(-(Time.deltaTime / smoothTime))")]
    [field: SerializeField] public float SmoothTime { get; set; }

    [Header("Mix")]
    [Tooltip("Mixing curve where an evaluated value would be clamped in [0, 1].")]
    [SerializeField] private AnimationCurve m_MixingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Range(0, 1)][SerializeField] private float m_ClampValue1 = 0f;
    [Range(0, 1)][SerializeField] private float m_ClampValue2 = 1f;
    [Tooltip("Raw distance ratio on the interval.")]
    [field: ReadOnly][field: SerializeField] public float RawRatio { get; private set; }
    [Tooltip("Mix ratio converted from distance ratio by applying the mixing curve.")]
    [field: ReadOnly][field: SerializeField] public float MixRatio { get; private set; }

    [Header("LookAt")]
    [Tooltip("Caution: Do not set this vector to the vertical direction!")][SerializeField] private Vector3 m_StartLookAtOffset;
    [Tooltip("Caution: Do not set this vector to the vertical direction!")][SerializeField] private Vector3 m_EndLookAtOffset;

    [Header("Camera Offset")]
    [Tooltip("Caution: Do not set this vector to the vertical direction!")][SerializeField] private Vector3 m_StartCameraOffset = new Vector3(0f, 0f, -10f);
    [Tooltip("Caution: Do not set this vector to the vertical direction!")][SerializeField] private Vector3 m_EndCameraOffset = new Vector3(0f, 0f, -10f);

    [Header("Field of View")]
    [Range(0, 180)][SerializeField] private float m_StartFieldOfView = 60f;
    [Range(0, 180)][SerializeField] private float m_EndFieldOfView = 60f;

    private Camera m_thisCamera;

    private void Awake()
    {
        m_thisCamera = GetComponent<Camera>();
        if (m_thisCamera == null) Debug.LogError("The camera was null.");
    }

    private void OnEnable()
    {
        // temporarily invoke stage initialization method from here
        OnStageInitialize();
    }

    private void OnDisable()
    {
        // temporarily invoke stage finalization method from here
        OnStageFinalize();
    }

    private void OnStageInitialize()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnStageFinalize()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (m_thisCamera != camera) return;
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (null == m_thisCamera) return;
        if (null == PlayerCharacterTransform) return;
        if (null == MainBuildingTransform) return;

        Vector2 mainBuildingPositionOnXZ = new Vector2(MainBuildingTransform.position.x, MainBuildingTransform.position.z);
        Vector2 playerCharacterPositionOnXZ = new Vector2(PlayerCharacterTransform.position.x, PlayerCharacterTransform.position.z);
        float planarDistanceOnXZ = Vector2.Distance(mainBuildingPositionOnXZ, playerCharacterPositionOnXZ);

        RawRatio = Mathf.InverseLerp(MinimumDistance, MaximumDistance, planarDistanceOnXZ);
        MixRatio = Mathf.Clamp(m_MixingCurve.Evaluate(RawRatio), Mathf.Min(m_ClampValue1, m_ClampValue2), Mathf.Max(m_ClampValue1, m_ClampValue2));

        Vector2 lookAtOnXZ = Vector2.Lerp(mainBuildingPositionOnXZ, playerCharacterPositionOnXZ, MixRatio);

        Vector3 lookAtOffset = Vector3.Lerp(m_EndLookAtOffset, m_StartLookAtOffset, MixRatio);
        Vector3 lookAt = new Vector3(lookAtOnXZ.x, 0, lookAtOnXZ.y) + lookAtOffset;
        Vector3 cameraOffset = Vector3.Lerp(m_EndCameraOffset, m_StartCameraOffset, MixRatio);
        Vector3 resultPosition = lookAt + cameraOffset;
        Quaternion resultRotation = Quaternion.LookRotation(-cameraOffset);

        float defaultTangent = Mathf.Tan(m_StartFieldOfView * Mathf.Deg2Rad * 0.5f);
        float transitionedTangent = Mathf.Tan(m_EndFieldOfView * Mathf.Deg2Rad * 0.5f);
        float tangent = Mathf.Lerp(transitionedTangent, defaultTangent, MixRatio);

        float interpolationRate = Mathf.Exp(-(Time.deltaTime / SmoothTime));

#if UNITY_EDITOR
        if (!Application.isPlaying) interpolationRate = 0f;
#endif

        resultPosition = Vector3.Lerp(resultPosition, m_thisCamera.transform.position, interpolationRate);
        resultRotation = Quaternion.Slerp(resultRotation, m_thisCamera.transform.rotation, interpolationRate);
        float currentTangent = Mathf.Tan(m_thisCamera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float resultTangent = Mathf.Lerp(tangent, currentTangent, interpolationRate);
        float fieldOfView = Mathf.Atan(resultTangent) * Mathf.Rad2Deg * 2f;

#if UNITY_EDITOR
        if (float.IsNaN(fieldOfView)) Debug.LogError("Calculated Field of View was NaN.");
#endif
        if (float.IsNaN(fieldOfView)) fieldOfView = 60f;

        m_thisCamera.transform.SetPositionAndRotation(resultPosition, resultRotation);
        m_thisCamera.fieldOfView = fieldOfView;
    }
}
