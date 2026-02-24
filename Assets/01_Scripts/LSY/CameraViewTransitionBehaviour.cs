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

    private Vector3 m_DebugLookAtOnXZ;
    private Vector3 m_DebugLookAtOffset;
    private Vector3 m_DebugLookAt;
    private Vector3 m_DebugCameraOffset;
    private Vector3 m_DebugTargetCameraPosition;
    private Vector3 m_DebugResultCameraPosition;
    Mesh m_DebugLookOffsetGizmoMesh;
    Mesh m_DebugCameraOffsetGizmoMesh;

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

        // 주 건물과 플레이어 사이 distance가 커브 적용 거리 구간 위에서 어느 지점에 존재하는지 계산해 커브값 적용
        Vector2 mainBuildingPositionOnXZ = new Vector2(MainBuildingTransform.position.x, MainBuildingTransform.position.z);
        Vector2 playerCharacterPositionOnXZ = new Vector2(PlayerCharacterTransform.position.x, PlayerCharacterTransform.position.z);
        float planarDistanceOnXZ = Vector2.Distance(mainBuildingPositionOnXZ, playerCharacterPositionOnXZ);
        RawRatio = Mathf.InverseLerp(MinimumDistance, MaximumDistance, planarDistanceOnXZ);
        MixRatio = Mathf.Clamp(m_MixingCurve.Evaluate(RawRatio),
                               Mathf.Min(m_ClampValue1, m_ClampValue2),
                               Mathf.Max(m_ClampValue1, m_ClampValue2));

        // 두 목표 사이 비율에 따라 보간된 지점으로부터 보간된 LookAt 오프셋 적용해 바라보는 지점 계산
        Vector2 lookAtOnXZ = Vector2.Lerp(mainBuildingPositionOnXZ, playerCharacterPositionOnXZ, MixRatio);
        Vector3 lookAtOffset = Vector3.Lerp(m_EndLookAtOffset, m_StartLookAtOffset, MixRatio);
        Vector3 lookAt = new Vector3(lookAtOnXZ.x, 0, lookAtOnXZ.y) + lookAtOffset;

        // 바라보는 지점으로부터 보간된 카메라 오프셋 적용하여 카메라 위치 계산
        Vector3 cameraOffset = Vector3.Lerp(m_EndCameraOffset, m_StartCameraOffset, MixRatio);
        Vector3 targetCameraPosition = lookAt + cameraOffset;
        Quaternion targetCameraRotation = Quaternion.LookRotation(-cameraOffset);

        // 카메라 화각은 tangent를 보간하여 역으로 얻어냄
        float defaultFoVTangent = Mathf.Tan(m_StartFieldOfView * Mathf.Deg2Rad * 0.5f);
        float transitionedFoVTangent = Mathf.Tan(m_EndFieldOfView * Mathf.Deg2Rad * 0.5f);
        float targetFoVTangent = Mathf.Lerp(transitionedFoVTangent, defaultFoVTangent, MixRatio);

        // 카메라 움직임 보간 수치 계산
        float interpolationRate = Mathf.Exp(-(Time.deltaTime / SmoothTime));
#if UNITY_EDITOR
        if (!Application.isPlaying) interpolationRate = 0f;
#endif

        // 카메라가 부드럽게 움직이도록 보간
        Vector3 resultCameraPosition = Vector3.Lerp(targetCameraPosition, m_thisCamera.transform.position, interpolationRate);
        Quaternion resultCameraRotation = Quaternion.Slerp(targetCameraRotation, m_thisCamera.transform.rotation, interpolationRate);
        float currentFoVTangent = Mathf.Tan(m_thisCamera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float resultFoVTangent = Mathf.Lerp(targetFoVTangent, currentFoVTangent, interpolationRate);
        float fieldOfView = Mathf.Atan(resultFoVTangent) * Mathf.Rad2Deg * 2f;

#if UNITY_EDITOR
        m_DebugLookAtOnXZ = new Vector3(lookAtOnXZ.x, 0, lookAtOnXZ.y);
        m_DebugLookAtOffset = lookAtOffset;
        m_DebugLookAt = lookAt;
        m_DebugCameraOffset = cameraOffset;
        m_DebugTargetCameraPosition = targetCameraPosition;
        m_DebugResultCameraPosition = resultCameraPosition;

        if (float.IsNaN(fieldOfView)) Debug.LogError("Calculated Field of View was NaN.");
#endif
        if (float.IsNaN(fieldOfView)) fieldOfView = 60f;

        m_thisCamera.transform.SetPositionAndRotation(resultCameraPosition, resultCameraRotation);
        m_thisCamera.fieldOfView = fieldOfView;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_DebugLookOffsetGizmoMesh = new Mesh();
        m_DebugLookOffsetGizmoMesh.vertices = new Vector3[3]
        {
            Vector3.zero,
            m_StartLookAtOffset,
            m_EndLookAtOffset
        };
        m_DebugLookOffsetGizmoMesh.triangles = new int[6] { 0, 1, 2, 2, 1, 0 };
        m_DebugLookOffsetGizmoMesh.normals = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };

        m_DebugCameraOffsetGizmoMesh = new Mesh();
        m_DebugCameraOffsetGizmoMesh.vertices = new Vector3[3]
        {
            Vector3.zero,
            m_StartCameraOffset,
            m_EndCameraOffset
        };
        m_DebugCameraOffsetGizmoMesh.triangles = new int[6] { 0, 1, 2, 2, 1, 0 };
        m_DebugCameraOffsetGizmoMesh.normals = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };
    }

    private void OnDrawGizmos()
    {
        MoveCamera();

        Color prevColor = Gizmos.color;


        Vector3 mainBuildingPositionOnXZ = new Vector3(MainBuildingTransform.position.x, 0f, MainBuildingTransform.position.z);
        Vector3 playerCharacterPositionOnXZ = new Vector3(PlayerCharacterTransform.position.x, 0f, PlayerCharacterTransform.position.z);

        Gizmos.color = Color.magenta;   Gizmos.DrawLine(m_DebugLookAtOnXZ, mainBuildingPositionOnXZ);
        Gizmos.color = Color.cyan;      Gizmos.DrawLine(m_DebugLookAtOnXZ, playerCharacterPositionOnXZ);

        Gizmos.color = new Color(Color.pink.r, Color.pink.g, Color.pink.b, 0.5f);
        Gizmos.DrawMesh(m_DebugLookOffsetGizmoMesh, m_DebugLookAtOnXZ);

        Gizmos.color = new Color(Color.skyBlue.r, Color.skyBlue.g, Color.skyBlue.b, 0.5f);
        Gizmos.DrawMesh(m_DebugCameraOffsetGizmoMesh, m_DebugLookAt);

        Gizmos.color = Color.pink;
        Gizmos.DrawRay(m_DebugLookAtOnXZ, m_StartLookAtOffset);
        Gizmos.DrawRay(m_DebugLookAtOnXZ, m_EndLookAtOffset);

        Gizmos.color = Color.skyBlue;
        Gizmos.DrawRay(m_DebugLookAt, m_StartCameraOffset);
        Gizmos.DrawRay(m_DebugLookAt, m_EndCameraOffset);

        Gizmos.color = Color.white;     Gizmos.DrawRay(m_DebugTargetCameraPosition, -100f * m_DebugCameraOffset);
        Gizmos.color = Color.red;       Gizmos.DrawLine(m_DebugLookAtOnXZ, m_DebugLookAt);
        Gizmos.color = Color.blue;      Gizmos.DrawLine(m_DebugTargetCameraPosition, m_DebugLookAt);
        Gizmos.color = Color.green;     if (Application.isPlaying) Gizmos.DrawLine(m_DebugResultCameraPosition, m_DebugTargetCameraPosition);

        Gizmos.color = Color.white;     Gizmos.DrawSphere(m_DebugLookAtOnXZ, 0.5f);
        Gizmos.color = Color.red;       Gizmos.DrawSphere(m_DebugLookAt, 0.5f);
        Gizmos.color = Color.blue;      Gizmos.DrawSphere(m_DebugTargetCameraPosition, 0.5f);
        Gizmos.color = Color.green;     if (Application.isPlaying) Gizmos.DrawSphere(m_DebugResultCameraPosition, 0.5f);

        Gizmos.color = prevColor;
    }
#endif
}
