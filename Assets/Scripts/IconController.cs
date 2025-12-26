using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class IconController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Canvas canvas; // 座標変換用
    private RectTransform rect; // 自分のRectTransform
    private CanvasGroup canvasGroup; // Raycastの制御用
    [SerializeField] private Transform homeParent; // 元の親
    private Vector2 homeAnchorPos; // 元の位置
    private int homeSiblingIndex; // 最初の並び順

    // ドラッグごとの復元用
    private Transform originalParent; // ドラッグ開始時の親
    private Vector2 startAnchorPos; // 元の位置
    private int startSiblingIndex;
    private bool wasInDropZone; // ドロップゾーンに入っていたか

    private bool dropped = false; // ドロップされたか
    public void MarkDropped() => dropped = true;

    private Vector2 pointerOffset; // ドラッグ開始時のマウス位置とカード位置の差分
    private Camera UiCam() => canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;


    [Header("ダブルクリック用変数")]
    private int clickCount;
    private bool flg = false;
    public float DoubleClickIntervalTime = 0.3f;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        if (homeParent == null)
        {
            homeParent = rect.parent;
            homeAnchorPos = rect.anchoredPosition;
            homeSiblingIndex = rect.GetSiblingIndex();
        }
    }


    void Start()
    {
        clickCount = 0;
        flg = false;
    }

    /// <summary>
    /// クリックの感知
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        clickCount++;
        if (clickCount == 1)
        {
            Invoke("OnDoubleClick", DoubleClickIntervalTime); //一定時間後にダブルクリックされたか判定を呼び出す
        }
    }


    /// <summary>
    /// ダブルクリックの判定とダブルクリック時の処理呼び出し関数
    /// </summary>
    private void OnDoubleClick()
    {
        if (clickCount >= 2)
        {
            Debug.Log("ダブルクリックされたぞ！");
            OpenWindow();
        }

        
        clickCount = 0;
    }

    /// <summary>
    /// ダブルクリック時のウィンドウを開く処理実行関数
    /// </summary>
    private void OpenWindow()
    {
        if (flg == false)
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
            flg = true;
        }
        else
        {
            transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0f);
            flg = false;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = rect.parent;
        startAnchorPos = rect.anchoredPosition;
        startSiblingIndex = rect.GetSiblingIndex();
        wasInDropZone = originalParent != null && originalParent.GetComponent<DropZone>() != null;

        var parentRect = rect.parent as RectTransform;
        if (parentRect != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, UiCam(), out var pLocal))
        {
            pointerOffset = rect.anchoredPosition - pLocal;
        }

        // ドラッグ中は一番上に表示
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;

        // 最前面に
        rect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var parentRect = rect.parent as RectTransform;
        if (parentRect != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, UiCam(), out var pLocal))
        {
            rect.anchoredPosition = pLocal + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        if (dropped)
        {
            // DropZoneに配置された場合、アンカーを中央に調整
            if (rect.parent.TryGetComponent<DropZone>(out var dropZone))
            {
                // アンカーを中央に設定
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                // 位置を中央に調整
                rect.anchoredPosition = Vector2.zero;

                rect.localScale = Vector3.one * 0.8f;
            }
        }
        else
        {
            if (wasInDropZone)
            {
                rect.localScale = Vector3.one;

                // アンカーを左上に戻す
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);

                rect.SetParent(homeParent, worldPositionStays: false);
                rect.SetSiblingIndex(Mathf.Clamp(homeSiblingIndex, 0, homeParent.childCount));
                rect.anchoredPosition = homeAnchorPos;
            }
            else
            {
                rect.SetParent(originalParent, worldPositionStays: false);
                rect.SetSiblingIndex(Mathf.Clamp(startSiblingIndex, 0, originalParent.childCount));
                rect.anchoredPosition = startAnchorPos;
            }
        }
        dropped = false;
    }

    // スクリプトから明示的にHomeに戻す時用
    public void ReturnToHome()
    {
        rect.SetParent(homeParent, worldPositionStays: false);
        rect.SetSiblingIndex(Mathf.Clamp(homeSiblingIndex, 0, homeParent.childCount));
        rect.anchoredPosition = homeAnchorPos;
    }
}