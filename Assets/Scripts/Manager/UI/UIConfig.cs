using UnityEngine;

[System.Serializable]
public class UIConfig
{
    public int sortingOrder = 100;
    public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
    public Vector2 anchorMax = new Vector2(0.5f, 0.5f);
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    public Vector2 anchoredPosition = Vector2.zero;
    public Vector2 sizeDelta = new Vector2(300, 150);
}

