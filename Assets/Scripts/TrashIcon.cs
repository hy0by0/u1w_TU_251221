using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashIcon : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        IconController icon = eventData.pointerDrag?.GetComponent<IconController>();
        if (icon == null) return;

        //削除するアイコンに応じた処理を呼び出す
        GameManager.Instance.OnIconDroppedToTrash(icon);
    }
}
