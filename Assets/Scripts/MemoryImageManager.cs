using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelGame
{

    public class MemoryImageManager : MonoBehaviour
    {
        [Header("対応する画像変数")]
        [SerializeField] Sprite _background1;
        [SerializeField] Sprite _charaA_face;
        [SerializeField] Sprite _charaB_face;
        [SerializeField] GameObject _backgroundObject;
        [SerializeField] GameObject _eventObject;
        [SerializeField] GameObject _imagePrefab;

        // テキストファイルから、文字列でSpriteやGameObjectを扱えるようにするための辞書
        Dictionary<string, Sprite> _textToSprite;
        Dictionary<string, GameObject> _textToParentObject;

        // 各画像のGameObjectを管理するためのリスト
        List<(string, GameObject, int)> _textToSpriteObject;


        /// <summary>
        /// テキストと画像(orオブジェクト)の対応の辞書の設定。追加の画像があればここも更新すること
        /// </summary>
        void Awake()
        {
            //Debug.Log("OK画像マネージャーの初期設定が完了しました");
            //テキスト→スプライトの初期化と画像の追加
            _textToSprite = new Dictionary<string, Sprite>();
            _textToSprite.Add("background1", _background1);
            _textToSprite.Add("A_face", _charaA_face);
            _textToSprite.Add("B_face", _charaB_face);

            //テキスト→親オブジェクト(backgroundかeventか)の追加
            _textToParentObject = new Dictionary<string, GameObject>();
            _textToParentObject.Add("backgroundObject", _backgroundObject);
            _textToParentObject.Add("eventObject", _eventObject);

            _textToSpriteObject = new List<(string, GameObject, int)>();
        }




        /// <summary>
        /// レイヤー番号を取得メソッド
        /// </summary>
        /// <param name="Object">対象のオブジェクト</param>
        /// <param name="layerOrder"></param>
        /// <returns></returns>
        int getLayerNum(List<(string, GameObject, int)> Object, int layerOrder)
        {
            int rank = 0;
            foreach (var pair in Object)
            {
                (string name, GameObject obj, int layer) = pair;
                if (layerOrder > layer)
                {
                    rank++;
                }
            }
            return rank;
        }




        /// <summary>
        /// 画像を配置するメソッド
        /// </summary>
        /// <param name="imageName">呼び出す画像名</param>
        /// <param name="parentObjectName">呼び出す親オブジェクト名</param>
        /// <param name="layerOrder">レイヤー番号</param>
        /// <param name="img_x">配置するx座標値</param>
        /// <param name="img_y">配置するy座標値</param>
        /// <param name="scale_percent_percent">画像の大きさ%</param>
        public void PutImage(string imageName, string parentObjectName, int layerOrder = 10000, int img_x = 0, int img_y = 0, int scale_percent = 100)
        {
            //画像を取得
            Sprite image = _textToSprite[imageName];
            GameObject parentObject = _textToParentObject[parentObjectName];

            //既存のオブジェクトがないか調べる
            GameObject existingObject = null;
            //レイヤー番号の既存のオブジェクトを検索する
            foreach (var pair in _textToSpriteObject)
            {
                (string name, GameObject obj, int layer) = pair;
                if (layer == layerOrder)
                {
                    existingObject = obj;
                    break;
                }
            }

            // 既存のオブジェクトがあれば削除する
            if (existingObject != null)
            {
                Destroy(existingObject);
                // リストからも削除する
                _textToSpriteObject.RemoveAll(item => item.Item3 == layerOrder);
            }


            // 新しい画像を生成する
            Quaternion rotation = Quaternion.identity;
            Transform parent = parentObject.transform;

            // 画像のGameObjectを生成
            GameObject item = Instantiate(_imagePrefab, parent);
            item.GetComponent<Image>().sprite = image;
            RectTransform rectTransform = item.GetComponent<RectTransform>();

            // RectTransformを設定
            rectTransform.anchoredPosition = new Vector2(img_x, img_y); // img_xとimg_yを使用してアンカーポイントからの相対位置を設定
            rectTransform.localRotation = rotation; // 回転を設定

            float scale = (float)scale_percent / 100; // 拡大率を決定
            rectTransform.localScale = new Vector3(scale, scale, 1.0f);

            // レイヤー番号を設定し、画像の追加、レイヤー番号通りに画像を配置
            int siblingIndex = getLayerNum(_textToSpriteObject, layerOrder);
            _textToSpriteObject.Add((imageName, item, layerOrder));
            item.transform.SetSiblingIndex(siblingIndex);
        }

        /// <summary>
        /// 画像の削除メソッド
        /// </summary>
        /// <param name="imageName">対象の画像</param>
        public void RemoveImage(string imageName)
        {
            // すべて削除
            if (imageName == "all")
            {
                //Debug.Log("OK画像初期化の実行");
                foreach (var item in _textToSpriteObject)
                {
                    Destroy(item.Item2);
                }

                _textToSpriteObject.Clear(); // 中身を空にする
                return;
            }

            var existingObjects = _textToSpriteObject.FindAll(item => item.Item1 == imageName);
            foreach (var existingObject in existingObjects)
            {
                GameObject obj = existingObject.Item2;
                Destroy(obj);
                _textToSpriteObject.Remove(existingObject);
            }
        }



    }
}
