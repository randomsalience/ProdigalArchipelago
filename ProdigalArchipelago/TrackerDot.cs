using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProdigalArchipelago;

class TrackerDot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    string Name;
    public List<TrackerLocation> Locations;
    public Func<bool> Logic;

    private GameObject TextBox;

    public static TrackerDot NewDot(string name, int x, int y, bool large, Func<bool> logic, List<TrackerLocation> locations)
    {
        GameObject obj = new($"Dot{name}");
        obj.transform.SetParent(MapTracker.Instance.transform);
        obj.transform.localPosition = new Vector3(x, y, 0);
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sprite = large ? SpriteManager.TrackerDotLargeSprite : SpriteManager.TrackerDotSprite;
        sprite.sortingLayerName = "UI";
        sprite.sortingOrder = 10;
        var collider = obj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(4, 4);
        var dot = obj.AddComponent<TrackerDot>();
        dot.Name = name;
        dot.Locations = locations;
        dot.Logic = logic;

        dot.TextBox = new GameObject($"DotTextBox{name}");
        dot.TextBox.SetActive(false);
        dot.TextBox.transform.SetParent(MapTracker.Canvas.transform);
        var bg = new GameObject($"DotTextBoxBg{name}");
        bg.transform.SetParent(dot.TextBox.transform, false);
        var image = bg.AddComponent<Image>();
        image.color = new Color(0.31f, 0.50f, 0.64f);
        var textObj = new GameObject($"DotTextBoxText{name}");
        textObj.transform.SetParent(dot.TextBox.transform, false);
        var text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = new Color(0.92f, 0.87f, 0.76f);
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        var fitter = textObj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return dot;
    }

    public void SetColor()
    {
        var sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;

        bool all = true;
        bool inLogic = false;
        bool outOfLogic = false;

        foreach (var location in Locations)
        {
            if (!Archipelago.AP.IsLocationRandomized(location.ID) || GameMaster.GM.Save.Data.Chests.Contains(location.ID))
                continue;
            sprite.enabled = true;

            if (Logic() && location.Logic())
                inLogic = true;
            else
                all = false;
            
            if (Logic() && location.KeyLogic is not null && location.KeyLogic())
                outOfLogic = true;
        }

        if (all)
            sprite.color = new Color(0.35f, 0.90f, 0.22f);
        else if (inLogic)
            sprite.color = new Color(0.98f, 0.95f, 0.21f);
        else if (outOfLogic)
            sprite.color = new Color(0.98f, 0.63f, 0.07f);
        else
            sprite.color = new Color(0.93f, 0.22f, 0.30f);
    }

    public void ClearTextBox()
    {
        TextBox.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData _)
    {
        var lines = from location in Locations where location.IsUnchecked() select location.GetText(Logic());
        if (lines.Any())
        {
            Text text = TextBox.transform.GetChild(1).GetComponent<Text>();
            text.text = $"<b>{Name}</b>\n{string.Join('\n', lines)}";
            text.fontSize = (int)(4 * GetScale());
            TextBox.transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta = new Vector2(text.preferredWidth + 4 * GetScale(), text.preferredHeight + 4 * GetScale());
            PositionTextBox();
            TextBox.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData _)
    {
        TextBox.SetActive(false);
    }

    private void PositionTextBox()
    {
        Text text = TextBox.transform.GetChild(1).GetComponent<Text>();

        float scale = GetScale();
        float x = transform.localPosition.x;
        float y = transform.localPosition.y;
        float textX = scale * x;
        float textY = scale * y;

        if (x < 0)
        {
            textX += 0.5f * text.preferredWidth + 4 * scale;
        }
        else
        {
            textX -= 0.5f * text.preferredWidth + 4 * scale;
        }

        if (y < 0)
        {
            textY += 0.5f * text.preferredHeight + 4 * scale;
        }
        else
        {
            textY -= 0.5f * text.preferredHeight + 4 * scale;
        }

        TextBox.transform.localPosition = new Vector3((int)textX, (int)textY, 0);
    }

    private float GetScale()
    {
        return GameMaster.GM.Save.PlayerOptions.Resolution switch
        {
            1 => 4.0f,
            2 => 5.0f,
            3 => 7.5f,
            4 => 10.0f,
            _ => 2.0f,
        };
    }
}