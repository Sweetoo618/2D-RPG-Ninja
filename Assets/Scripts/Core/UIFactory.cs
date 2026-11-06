using TMPro;
using UnityEngine;

/// <summary>
/// Shared factory for building simple UI text/elements, removing duplicated
/// layout code across HUD, shop and dialogue views.
/// </summary>
public static class UIFactory
{
    public static TMP_Text CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, float fontSize)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)obj.transform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }
}