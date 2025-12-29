using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace NovelGame
{
    /// <summary>
    /// テキストファイルから文章を読み込み、処理の実行を管理するクラス
    /// </summary>
    public class UserScriptManager : MonoBehaviour
    {
        [SerializeField] TextAsset _textFile; // テキストファイルを格納するための変数

        private bool isRunning = false; // 実行状態かどうか

        List<string> _sentences = new List<string>(); // 読み込んだ文章を格納するリスト
        public bool isWaiting = false; // ウェイト中かどうかのフラグ
        
        // スクリプトの初期化時に実行されるメソッド。ここは毎回呼び出さなくともシーンの最初のみに実行すれば良い。
        void Awake()
        {
            //Debug.Log("OK無事にテキストファイルが読み込まれました");
            // テキストファイルから文章を一行ずつ読み込んでリストに格納する。ここで文書を保持する。
            StringReader reader = new StringReader(_textFile.text);
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                _sentences.Add(line);
            }
        }


        /// <summary>
        /// 現在の文章を取得するメソッド
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSentence()
        {
            return _sentences[NovelManager.Instance.line_number];
        }



        /// <summary>
        /// 命令文かどうかを判定するメソッド
        /// </summary>
        /// <param name="sentence">読み込んだ１行の文章</param>
        /// <returns></returns>
        public bool IsStatement(string sentence)
        {
            string[] words = sentence.Split(','); // 文章を単語に分割する
            //Debug.Log($"[{words[0]}]");

            if (words[0] == "com") // A列目がcomの場合、命令とみなす
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 文字列が数値なら数値で、そうでないならデフォルト値に設定するメソッド
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultNum"></param>
        /// <returns></returns>
        private int ConvertToInt(string str, int defaultNum = 0)
        {
            return int.TryParse(str, out int result) ? result : defaultNum;
        }


        /// <summary>
        /// 取得した命令を実行するメソッド
        /// </summary>
        /// <param name="sentence">該当する行の文章</param>
        public void ExecuteStatement(string sentence)
        {
            //Debug.Log("命令するぜえええええええ");
            if (!isRunning) return;

            string[] words = sentence.Split(','); // 文章を単語に分割する。何列目に何の情報を記載するか規定しておく。

            // 単語によって処理を分岐する。追加事項ができたらここを更新すること。
            switch (words[1])
            {
                case "end": // endステートメントの場合

                    //Debug.Log("OK終了コマンドが読み込まれました");
                    StopRun();
                    NovelManager.Instance.End();
                    break;

                case "putImage": // putImageステートメントの場合

                    //Debug.Log("画像表示コマンド発動！！！");
                    int layerOrder = ConvertToInt(words[4], 10000);
                    int img_x = ConvertToInt(words[5]);
                    int img_y = ConvertToInt(words[6]);
                    int scale_percent = ConvertToInt(words[7], 100);
                    NovelManager.Instance.image_manager.PutImage(words[2], words[3], layerOrder, img_x, img_y, scale_percent); // 画像を表示する
                    NovelManager.Instance.main_text_controller.GoToTheNextLine(); //次の行に進む
                    break;

                case "removeImage": // removeImageステートメントの場合

                    NovelManager.Instance.image_manager.RemoveImage(words[2]); // 画像を削除する
                    NovelManager.Instance.main_text_controller.GoToTheNextLine();
                    break;

                default: //そうでない場合
                    break;
            }
        }


        /// <summary>
        /// 処理をストップさせる。実行状態を解除する
        /// </summary>
        public void StopRun()
        {
            isRunning = false;
        }


        /// <summary>
        /// 状態の初期化
        /// </summary>
        public void ResetState()
        {
            //Debug.Log("OK:UserScriptManagerの状態の初期化がされました");
            isRunning = true;
            isWaiting = false;
        }



    }

}

