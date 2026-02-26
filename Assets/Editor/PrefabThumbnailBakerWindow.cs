// Assets/Editor/PrefabThumbnailBakerWindow.cs
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PrefabThumbnailBakerWindow : EditorWindow
{
    [Header("Output")]
    private DefaultAsset outputFolder;
    private int width = 512;
    private int height = 512;

    [Header("Capture")]
    private bool transparentBackground = true;
    private Color backgroundColor = new Color(0, 0, 0, 0);
    private bool useOrthographic = true;
    private float padding = 1.15f;

    [Header("Pose")]
    private Vector3 eulerAngles = new Vector3(20f, -35f, 0f);
    private bool centerToBounds = true;

    [Header("Lighting")]
    private bool addTempLight = true;
    private float lightIntensity = 1.2f;
    private Vector3 lightEuler = new Vector3(45f, 30f, 0f);

    private const int CaptureLayer = 31; // 임시로 31번 레이어 사용 (프로젝트에 따라 바꿔도 됨)

    [MenuItem("Tools/Thumbnail Baker")]
    public static void Open()
    {
        var w = GetWindow<PrefabThumbnailBakerWindow>("Thumbnail Baker");
        w.minSize = new Vector2(420, 420);
    }

    private void OnEnable()
    {
        const string defaultPath = "Assets/04_Images";

        if (outputFolder == null && AssetDatabase.IsValidFolder(defaultPath))
        {
            outputFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(defaultPath);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Thumbnail Baker (Batch PNG)", EditorStyles.boldLabel);
        EditorGUILayout.Space(6);

        outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            new GUIContent("Output Folder (Assets)"),
            outputFolder,
            typeof(DefaultAsset),
            false
        );

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        EditorGUILayout.Space(8);
        transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);
        using (new EditorGUI.DisabledScope(transparentBackground))
        {
            backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        }

        useOrthographic = EditorGUILayout.Toggle("Use Orthographic Camera", useOrthographic);
        padding = EditorGUILayout.Slider("Padding", padding, 1.0f, 2.0f);

        EditorGUILayout.Space(8);
        eulerAngles = EditorGUILayout.Vector3Field("Model Rotation (Euler)", eulerAngles);
        centerToBounds = EditorGUILayout.Toggle("Center Model To Bounds", centerToBounds);

        EditorGUILayout.Space(8);
        addTempLight = EditorGUILayout.Toggle("Add Temp Directional Light", addTempLight);
        if (addTempLight)
        {
            lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0.1f, 3f);
            lightEuler = EditorGUILayout.Vector3Field("Light Rotation", lightEuler);
        }

        EditorGUILayout.Space(14);

        var selectedPrefabs = GetSelectedPrefabAssets();
        EditorGUILayout.LabelField($"Selected Prefabs: {selectedPrefabs.Length}");

        using (new EditorGUI.DisabledScope(selectedPrefabs.Length == 0))
        {
            if (GUILayout.Button("Bake Selected Prefabs", GUILayout.Height(40)))
            {
                BakeSelected(selectedPrefabs);
            }
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.HelpBox(
            "프로젝트 뷰에서 프리팹을 여러 개 선택한 뒤 버튼을 누르면, 선택한 프리팹 이름으로 PNG가 저장됩니다.\n" +
            "Output Folder는 반드시 Assets 하위 폴더여야 합니다.",
            MessageType.Info
        );
    }

    private static GameObject[] GetSelectedPrefabAssets()
    {
        // 프로젝트 뷰에서 선택된 에셋 중 GameObject(프리팹)만 가져오기
        var gos = Selection.GetFiltered<GameObject>(SelectionMode.Assets);
        return gos.Where(go => PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab).ToArray();
    }

    private void BakeSelected(GameObject[] prefabAssets)
    {
        var folderPath = GetAssetsFolderPath(outputFolder);
        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("Output Folder Error", "Output Folder를 Assets 하위 폴더로 지정해줘.", "OK");
            return;
        }

        // 캡처용 임시 씬/스테이지 느낌으로: 현재 씬 오염 최소화
        // (씬에 오브젝트가 생성되긴 하지만, 끝나면 바로 제거)
        var prevScene = EditorSceneManager.GetActiveScene();

        try
        {
            EditorUtility.DisplayProgressBar("Thumbnail Baker", "Preparing capture objects...", 0f);

            // 캡처용 카메라
            var camGO = new GameObject("__ThumbCam");
            camGO.hideFlags = HideFlags.HideAndDontSave;

            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
            cam.orthographic = useOrthographic;
            cam.allowHDR = false;
            cam.allowMSAA = true;
            cam.cullingMask = 1 << CaptureLayer;

            // 임시 라이트
            Light light = null;
            GameObject lightGO = null;
            if (addTempLight)
            {
                lightGO = new GameObject("__ThumbLight");
                lightGO.hideFlags = HideFlags.HideAndDontSave;
                light = lightGO.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = lightIntensity;
                lightGO.transform.rotation = Quaternion.Euler(lightEuler);
            }

            // RenderTexture
            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 4;
            rt.Create();

            cam.targetTexture = rt;

            for (int i = 0; i < prefabAssets.Length; i++)
            {
                float t = (float)i / prefabAssets.Length;
                EditorUtility.DisplayProgressBar("Thumbnail Baker", $"Capturing {prefabAssets[i].name}", t);

                BakeOne(prefabAssets[i], cam, rt, folderPath);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Done", $"PNG 저장 완료!\n폴더: {folderPath}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            // 혹시 남아있을 수 있는 임시 오브젝트 정리
            var leftovers = GameObject.FindObjectsOfType<GameObject>()
                .Where(go => go.name.StartsWith("__Thumb"))
                .ToArray();
            foreach (var go in leftovers)
                DestroyImmediate(go);

            // 씬 상태 복구는 크게 필요 없지만, 안전 차원
            EditorSceneManager.SetActiveScene(prevScene);
        }
    }

    private void BakeOne(GameObject prefabAsset, Camera cam, RenderTexture rt, string folderPath)
    {
        // 프리팹 인스턴스 생성
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
        instance.hideFlags = HideFlags.HideAndDontSave;

        // 레이어 통일 (모델만 렌더)
        SetLayerRecursively(instance, CaptureLayer);

        // 회전 적용
        instance.transform.rotation = Quaternion.Euler(eulerAngles);

        // Bounds 계산
        var renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            DestroyImmediate(instance);
            Debug.LogWarning($"[Thumbnail Baker] Renderer가 없어 스킵: {prefabAsset.name}");
            return;
        }

        var bounds = CalculateBounds(renderers);

        // 중앙 정렬 옵션
        if (centerToBounds)
        {
            // bounds.center가 원점으로 오도록 이동
            var offset = bounds.center;
            instance.transform.position -= offset;

            // 이동 후 bounds 재계산
            bounds = CalculateBounds(instance.GetComponentsInChildren<Renderer>(true));
        }

        // 카메라 자동 프레이밍
        FrameCamera(cam, bounds, padding);

        // 렌더
        var prev = RenderTexture.active;
        RenderTexture.active = rt;

        cam.Render();

        // ReadPixels
        var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply(false, false);

        // PNG 저장 (Assets 폴더)
        var png = tex.EncodeToPNG();
        var filePath = MakeUniquePath(Path.Combine(folderPath, prefabAsset.name + ".png"));
        File.WriteAllBytes(filePath, png);

        // 정리
        RenderTexture.active = prev;
        DestroyImmediate(tex);
        DestroyImmediate(instance);

        // 에셋 임포트 설정 (선택: 그냥 PNG로만 두고 싶으면 아래 블록 삭제 가능)
        var assetPath = ToAssetPath(filePath);
        if (!string.IsNullOrEmpty(assetPath))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite; // UI에 바로 쓰기 편함
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.sRGBTexture = true;
                importer.SaveAndReimport();
            }
        }
    }

    private static void FrameCamera(Camera cam, Bounds b, float padding)
    {
        // 카메라 방향은 고정: forward 기준으로 뒤로 빠짐
        cam.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 필요하면 여기서 카메라 방향 바꿔도 됨
        // 위에서 모델 회전으로 시점 확보하는 방식

        // 중심을 바라보도록
        var center = b.center;

        if (cam.orthographic)
        {
            // OrthoSize 결정: 세로/가로(화면비) 고려
            float sizeY = b.extents.y;
            float sizeX = b.extents.x;

            float orthoSize = Mathf.Max(sizeY, sizeX / cam.aspect) * padding;
            cam.orthographicSize = orthoSize;

            // Ortho는 거리 영향이 적으니 적당히
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 1000f;
            cam.transform.position = center - cam.transform.forward * 10f;
        }
        else
        {
            // Perspective: FOV 기반 거리 계산
            float radius = b.extents.magnitude;
            float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
            float dist = (radius * padding) / Mathf.Sin(fovRad * 0.5f);

            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = dist + radius * 4f;
            cam.transform.position = center - cam.transform.forward * dist;
        }

        cam.transform.LookAt(center);
    }

    private static Bounds CalculateBounds(Renderer[] renderers)
    {
        var b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    private static string GetAssetsFolderPath(DefaultAsset folderAsset)
    {
        if (folderAsset == null) return null;
        var assetPath = AssetDatabase.GetAssetPath(folderAsset);
        if (string.IsNullOrEmpty(assetPath)) return null;
        if (!AssetDatabase.IsValidFolder(assetPath)) return null;

        // 절대경로로 변환
        var full = Path.GetFullPath(assetPath);
        return full;
    }

    private static string ToAssetPath(string fullPath)
    {
        fullPath = fullPath.Replace("\\", "/");
        var projectPath = Path.GetFullPath(".").Replace("\\", "/");
        if (!fullPath.StartsWith(projectPath)) return null;

        var rel = fullPath.Substring(projectPath.Length + 1);
        rel = rel.Replace("\\", "/");
        return rel;
    }

    private static string MakeUniquePath(string fullPath)
    {
        if (!File.Exists(fullPath)) return fullPath;

        var dir = Path.GetDirectoryName(fullPath);
        var name = Path.GetFileNameWithoutExtension(fullPath);
        var ext = Path.GetExtension(fullPath);

        for (int i = 1; i < 9999; i++)
        {
            var p = Path.Combine(dir, $"{name}_{i}{ext}");
            if (!File.Exists(p)) return p;
        }
        return fullPath;
    }
}
