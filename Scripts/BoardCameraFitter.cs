using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardCameraFitter : MonoBehaviour
{
    private const string RuntimeObjectName = "[BoardCameraFitter]";
    private const string TargetRootName = "Square";
    private const float Padding = 0.4f;
    private const float HorizontalOffset = 0.15f;
    private const float VerticalOffset = 0f;

    private Camera targetCamera;
    private Transform targetRoot;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeFitter()
    {
        if (GameObject.Find(RuntimeObjectName) != null)
        {
            return;
        }

        GameObject runtimeObject = new GameObject(RuntimeObjectName);
        DontDestroyOnLoad(runtimeObject);
        runtimeObject.AddComponent<BoardCameraFitter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ResolveTargets();
        FitNow();
    }

    private void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            FitNow();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolveTargets();
        FitNow();
    }

    private void ResolveTargets()
    {
        targetCamera = Camera.main;

        GameObject targetObject = GameObject.Find(TargetRootName);
        targetRoot = targetObject != null ? targetObject.transform : null;
    }

    private void FitNow()
    {
        if (targetCamera == null || targetRoot == null)
        {
            ResolveTargets();
        }

        if (targetCamera == null || targetRoot == null)
        {
            return;
        }

        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        if (!TryGetTargetBounds(out Bounds bounds))
        {
            return;
        }

        targetCamera.orthographic = true;

        float aspect = Mathf.Max(0.0001f, targetCamera.aspect);

        float requiredHalfHeightByHeight = bounds.extents.y + Padding;
        float requiredHalfHeightByWidth = (bounds.extents.x + Padding) / aspect;

        targetCamera.orthographicSize = Mathf.Max(
            requiredHalfHeightByHeight,
            requiredHalfHeightByWidth
        );

        Vector3 cameraPosition = targetCamera.transform.position;
        cameraPosition.x = bounds.center.x + HorizontalOffset;
        cameraPosition.y = bounds.center.y + VerticalOffset;
        targetCamera.transform.position = cameraPosition;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private bool TryGetTargetBounds(out Bounds bounds)
    {
        if (targetRoot == null)
        {
            bounds = default;
            return false;
        }

        SpriteRenderer boardSpriteRenderer = targetRoot.GetComponent<SpriteRenderer>();

        if (boardSpriteRenderer != null && boardSpriteRenderer.enabled)
        {
            bounds = boardSpriteRenderer.bounds;
            return true;
        }

        Renderer[] renderers = targetRoot.GetComponentsInChildren<Renderer>(true);

        bool hasBounds = false;
        bounds = default;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer current = renderers[i];

            if (current == null || !current.enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = current.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(current.bounds);
            }
        }

        return hasBounds;
    }
}
