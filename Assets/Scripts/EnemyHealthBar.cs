using UnityEngine;
using UnityEngine.UI;

// Creates a small world-space health bar above an enemy.
public class EnemyHealthBar : MonoBehaviour
{
    public float height = 2.4f;
    public Vector2 size = new Vector2(1.4f, 0.14f);

    Image fill;
    Canvas canvas;
    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        CreateBar();
    }

    void LateUpdate()
    {
        if (canvas == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            canvas.transform.rotation = Quaternion.LookRotation(mainCamera.transform.position - canvas.transform.position);
    }

    public void SetHealth(float current, float maximum)
    {
        if (fill != null)
            fill.fillAmount = maximum <= 0f ? 0f : Mathf.Clamp01(current / maximum);
    }

    void CreateBar()
    {
        GameObject canvasObject = new GameObject("EnemyHealthBar");
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = Vector3.up * height;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObject.AddComponent<CanvasScaler>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = size;
        canvasRect.localScale = Vector3.one * 0.01f;

        Image background = CreateImage("Background", canvasObject.transform, Color.black);
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = Vector2.zero;
        background.rectTransform.offsetMax = Vector2.zero;

        fill = CreateImage("Fill", canvasObject.transform, Color.red);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.rectTransform.anchorMin = Vector2.zero;
        fill.rectTransform.anchorMax = Vector2.one;
        fill.rectTransform.offsetMin = Vector2.zero;
        fill.rectTransform.offsetMax = Vector2.zero;
    }

    Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }
}