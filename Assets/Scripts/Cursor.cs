using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform; //親オブジェクトを取得
        offset = new Vector2(13f, -17.5f); //実際のマウスとの位置ずれの補完
}

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = GetLocalPosition(Input.mousePosition) + this.offset;
    }

    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;

        // screenPositionを親の座標系(parentRectTransform)に対応するよう変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPosition, Camera.main, out result);

        return result;
    }

}
