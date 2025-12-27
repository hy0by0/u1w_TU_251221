using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowObject : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform; // 移動したいオブジェクトのRectTransform
    private RectTransform parentRectTransform; // 移動したいオブジェクトの親のRectTransform
    private Vector2 offset;
    [Header("制限したい挙動")]
    public bool canDrag = true;
    public bool canClose = true;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform; //親オブジェクトを取得
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ドラッグ開始時の処理
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        //ドラッグ位置の補正値を取得
        offset = (Vector2)rectTransform.anchoredPosition - GetLocalPosition(eventData.position);

    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        // eventData.positionから、親に従うlocalPositionへの変換を行う
        // オブジェクトの位置をlocalPositionに変更する

        Vector2 localPosition = GetLocalPosition(eventData.position);
        rectTransform.anchoredPosition = localPosition + this.offset;
    }

    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPositionを親の座標系(parentRectTransform)に対応するよう変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

    // 追加
    public void CloseWindow()
    {
        if (!canClose) return;

        this.gameObject.SetActive(false);
    }


}
