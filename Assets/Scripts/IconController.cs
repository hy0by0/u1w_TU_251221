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
    [Header("アイコンに対応するウィンドウオブジェクトを入力")]
    public GameObject windowObj;

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

    // クリックとドラッグとの競合回避用
    public bool isDragging = false; //ドラッグ中かどうか（クリックと競合させないため）
    private Vector2 pointerDownPos;
    [Header("ドラッグ開始の移動閾値")]
    [SerializeField] private float dragThreshold = 10f;

    private Vector2 pointerOffset; // ドラッグ開始時のマウス位置とアイコン位置の差分
    private Camera UiCam() => canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;


    [Header("ダブルクリック用変数")]
    private int clickCount;
    private bool flg = false; //不要かも
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
        isDragging = false;
        flg = false;
    }

    /// <summary>
    /// クリックの感知
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        pointerDownPos = eventData.position;
        if (isDragging) return; //ドラッグ中ならこの処理は無視

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
        if (!windowObj.activeInHierarchy)
        {
            windowObj.gameObject.SetActive(true);
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
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
        if (!isDragging)
        {
            // 閾値を超えたマウス移動があれば、ドラッグ扱いにする
            if (Vector2.Distance(pointerDownPos, eventData.position) > dragThreshold)
            {
                isDragging = true;
            }
            else
            {
                return; // まだクリック扱い
            }
        }

        //以下からドラッグ中の処理
        var parentRect = rect.parent as RectTransform;
        if (parentRect != null &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, UiCam(), out var pLocal))
        {
            rect.anchoredPosition = pLocal + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
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