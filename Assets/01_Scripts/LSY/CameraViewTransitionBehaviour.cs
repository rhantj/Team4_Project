using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraViewTransitionBehaviour : MonoBehaviour
{
    [Header("Transition Interval")]
    [SerializeField] private float m_TransitionMinimumDistance;
    [SerializeField] private float m_TransitionMaximumDistance;
    [field: ReadOnly][field: SerializeField] public float MixRatio { get; private set; }

    [Header("Offset")]
    [Tooltip("Mixing curve for offset vector.")][SerializeField] private AnimationCurve m_CameraOffsetMixingCurve;
    [Tooltip("Caution: Do not set this vector to the vertical direction.")][SerializeField] private Vector3 m_DefaultCameraOffset;
    [Tooltip("Caution: Do not set this vector to the vertical direction.")][SerializeField] private Vector3 m_TransitionedCameraOffset;

    [Header("Field of View")]
    [Tooltip("Mixing curve for Field of View.")][SerializeField] private AnimationCurve m_CameraFieldOfViewCurve;
    [Range(0, 180)]
    [SerializeField] private float m_DefaultCameraFieldOfView;
    [Range(0, 180)]
    [SerializeField] private float m_TransitionedCameraFieldOfView;

    [Header("Target")]
    [Tooltip("Mixing curve for viewing target point, also known as 'look at' position.")][SerializeField] private AnimationCurve m_CameraLookAtMixingCurve;
    [SerializeField] private Transform m_MainBuildingTransform;
    [SerializeField] private Transform m_PlayerCharacterTransform;

    private Camera m_thisCamera;

    private void Awake()
    {
        m_thisCamera = GetComponent<Camera>();
        if (m_thisCamera == null) Debug.LogError("camera was null");
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

        Vector2 mainBuildingPositionOnXZ = new Vector2(m_MainBuildingTransform.position.x, m_MainBuildingTransform.position.z);
        Vector2 playerCharacterPositionOnXZ = new Vector2(m_PlayerCharacterTransform.position.x, m_PlayerCharacterTransform.position.z);
        float planarDistanceOnXZ = Vector2.Distance(mainBuildingPositionOnXZ, playerCharacterPositionOnXZ);
        float triggerInterval = m_TransitionMinimumDistance - m_TransitionMaximumDistance;
        MixRatio = Mathf.InverseLerp(m_TransitionMinimumDistance, m_TransitionMaximumDistance, planarDistanceOnXZ);

        float cameraFieldOfView = Mathf.Lerp(m_TransitionedCameraFieldOfView, m_DefaultCameraFieldOfView, m_CameraFieldOfViewCurve.Evaluate(MixRatio));

        Vector2 LookAtOnXZ = Vector2.Lerp(mainBuildingPositionOnXZ,
                                      playerCharacterPositionOnXZ,
                                      m_CameraLookAtMixingCurve.Evaluate(MixRatio));
        Vector3 LookAt = new Vector3(LookAtOnXZ.x, 0, LookAtOnXZ.y);

        Vector3 cameraOffset = Vector3.Lerp(m_TransitionedCameraOffset,
                                            m_DefaultCameraOffset,
                                            m_CameraOffsetMixingCurve.Evaluate(MixRatio));

        camera.transform.position = LookAt - cameraOffset;
        camera.transform.rotation = Quaternion.LookRotation(cameraOffset);
        camera.fieldOfView = cameraFieldOfView;
    }
}
