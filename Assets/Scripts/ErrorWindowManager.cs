using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ErrorWindowManager : MonoBehaviour
{
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject ErrorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutErrrorWindow()
    {
        Transform parent = parentObject.transform;

        // 新しい画像を生成する
        Quaternion rotation = Quaternion.identity; //後で確認

        // 画像のGameObjectを生成
        GameObject item = Instantiate(ErrorPrefab, parent);
        RectTransform rectTransform = item.GetComponent<RectTransform>();

        //位置をある程度ランダムに
        float img_x = Random.Range(-5.0f, 5.0f);
        float img_y = Random.Range(-5.0f, 5.0f);

        // RectTransformを設定
        rectTransform.anchoredPosition = new Vector2(img_x, img_y); // img_xとimg_yを使用してアンカーポイントからの相対位置を設定
        rectTransform.localRotation = rotation; // 回転を設定

    }

    public void Remove()
    {
        if (transform.GetChild(0).gameObject == null)
        {
            Debug.Log("閉じるエラーウィンドウが存在していません");
            return;
        }

        GameObject child = transform.GetChild(0).gameObject;
        Destroy(child);
            
    }

}
