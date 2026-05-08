using UnityEngine;

/// <summary>
/// 이 스크립트 역할:
/// - 유닛 머리 위에 2D 체력 바를 표시한다.
/// - 별도의 UI Canvas 없이 SpriteRenderer만 사용한다.
/// - CharacterBase가 체력 변경 시 SetValue()를 호출한다.
/// </summary>
public class HealthBar2D : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.55f, 0f);

    [Header("Size")]
    [SerializeField] private float barWidth = 0.7f;
    [SerializeField] private float barHeight = 0.09f;

    [Header("Sorting")]
    [SerializeField] private int sortingOrder = 100;

    [Header("Colors")]
    [SerializeField] private Color backgroundColor = Color.black;
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color middleHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    private GameObject barRoot;
    private Transform fillTransform;

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer fillRenderer;

    private static Sprite whiteSprite;

    private void Awake()
    {
        CreateBarIfNeeded();
    }

    private void LateUpdate()
    {
        // 체력 바가 항상 유닛을 따라다니게 한다.
        if (barRoot != null)
        {
            barRoot.transform.position = transform.position + worldOffset;
        }
    }

    private void OnEnable()
    {
        if (barRoot != null)
        {
            barRoot.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (barRoot != null)
        {
            barRoot.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (barRoot != null)
        {
            Destroy(barRoot);
        }
    }

    /// <summary>
    /// 현재 체력 비율에 맞게 체력 바 길이와 색을 갱신한다.
    /// </summary>
    public void SetValue(int currentHP, int maxHP)
    {
        CreateBarIfNeeded();

        if (fillTransform == null || fillRenderer == null)
        {
            return;
        }

        float ratio = 0f;

        if (maxHP > 0)
        {
            ratio = Mathf.Clamp01((float)currentHP / maxHP);
        }

        float fillWidth = barWidth * ratio;

        // 체력 바를 왼쪽 기준으로 줄어들게 보이도록 위치를 조정한다.
        fillTransform.localScale = new Vector3(fillWidth, barHeight, 1f);

        float leftEdge = -barWidth * 0.5f;
        fillTransform.localPosition = new Vector3(leftEdge + fillWidth * 0.5f, 0f, 0f);

        // 체력 비율에 따라 색상 변경
        if (ratio > 0.5f)
        {
            fillRenderer.color = highHealthColor;
        }
        else if (ratio > 0.25f)
        {
            fillRenderer.color = middleHealthColor;
        }
        else
        {
            fillRenderer.color = lowHealthColor;
        }
    }

    /// <summary>
    /// SpriteRenderer 기반 체력 바 오브젝트를 만든다.
    /// </summary>
    private void CreateBarIfNeeded()
    {
        if (barRoot != null)
        {
            return;
        }

        barRoot = new GameObject(gameObject.name + "_HealthBar");
        barRoot.transform.position = transform.position + worldOffset;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(barRoot.transform);
        backgroundObject.transform.localPosition = Vector3.zero;
        backgroundObject.transform.localScale = new Vector3(barWidth, barHeight, 1f);

        backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = GetWhiteSprite();
        backgroundRenderer.color = backgroundColor;
        backgroundRenderer.sortingOrder = sortingOrder;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(barRoot.transform);
        fillObject.transform.localPosition = Vector3.zero;
        fillObject.transform.localScale = new Vector3(barWidth, barHeight, 1f);

        fillRenderer = fillObject.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = GetWhiteSprite();
        fillRenderer.color = highHealthColor;
        fillRenderer.sortingOrder = sortingOrder + 1;

        fillTransform = fillObject.transform;
    }

    /// <summary>
    /// 1x1 흰색 Sprite를 코드로 만든다.
    /// 체력 바 배경과 채움 부분에 공통으로 사용한다.
    /// </summary>
    private Sprite GetWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return whiteSprite;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whiteSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        return whiteSprite;
    }
}