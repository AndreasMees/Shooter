using UnityEngine;
using UnityEngine.UI;

// Creates a compact bottom-left weapon-slot display at runtime.
public class WeaponHUD : MonoBehaviour
{
    public Color selectedColor = new Color(0.2f, 0.75f, 1f, 1f);
    public Color unselectedColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);

    Image rifleSlot;
    Image pistolSlot;
    Text rifleLabel;
    Text pistolLabel;

    void Awake()
    {
        CreateHud();
    }

    public void SetSelected(int slot)
    {
        if (rifleSlot == null)
            return;

        rifleSlot.color = slot == 1 ? selectedColor : unselectedColor;
        pistolSlot.color = slot == 2 ? selectedColor : unselectedColor;
        rifleLabel.color = slot == 1 ? Color.white : Color.gray;
        pistolLabel.color = slot == 2 ? Color.white : Color.gray;
    }

    void CreateHud()
    {
        GameObject canvasObject = new GameObject("WeaponHUDCanvas");
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("WeaponSlots");
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0f, 0f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = new Vector2(24f, 24f);
        panelRect.sizeDelta = new Vector2(220f, 72f);

        rifleSlot = CreateSlot(panel.transform, "1  RIFLE", new Vector2(0f, 0f), out rifleLabel);
        pistolSlot = CreateSlot(panel.transform, "2  PISTOL", new Vector2(112f, 0f), out pistolLabel);
        SetSelected(2);
    }

    Image CreateSlot(Transform parent, string label, Vector2 position, out Text labelText)
    {
        GameObject slot = new GameObject(label);
        slot.transform.SetParent(parent, false);
        RectTransform rect = slot.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(104f, 64f);

        Image background = slot.AddComponent<Image>();
        background.color = unselectedColor;

        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(slot.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5f, 5f);
        textRect.offsetMax = new Vector2(-5f, -5f);

        labelText = textObject.AddComponent<Text>();
        labelText.text = label;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.gray;
        return background;
    }
}
