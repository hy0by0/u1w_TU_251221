using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag;
        if (icon == null) return;

        if (icon.TryGetComponent<IconController>(out var drag))
        {
            icon.transform.SetParent(transform, worldPositionStays: false);
            (icon.transform as RectTransform).anchoredPosition = Vector2.zero;
            drag.MarkDropped();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { /* ハイライト等 */ }
    public void OnPointerExit(PointerEventData eventData) { /* 解除 */ }
}
