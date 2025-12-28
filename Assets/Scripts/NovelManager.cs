using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelGame
{
    public class NovelManager : MonoBehaviour
    {
        // 別クラスからGameManagerの変数を使えるようにする
        public static NovelManager Instance { get; private set; }

        //下流のスクリプト
        public UserScriptManager user_script_manager;
        public MemoryTextController main_text_controller;
        public MemoryImageManager image_manager;

        //　ユーザスクリプトの現在の行の値。クリックで１ずつ増える
        [System.NonSerialized] public int line_number;


        /// <summary>
        /// 初期化・最初のセットアップ用関数
        /// </summary>
        private void Awake()
        {
            Debug.Log("OKノベルパートの初期化・セットアップが実行されました");
            // これで、別クラスからGameManagerの変数などを使えるようにする
            Instance = this;
        }


        /// <summary>
        /// ノベルパートを呼び出すための関数。ここですべての初期化も行う。
        /// </summary>
        public void StartNovel()
        {
            //ここの関数が呼び出されていないために初期化がうまくできていない
            Debug.Log("OKノベルパート呼び出し関数！");
            // 進行状態の初期化
            line_number = 0;

            // UserScriptManagerの初期化呼び出し
            user_script_manager.ResetState();

            // 表示状態(MemoryTextController、MemoryImageManager)の初期化
            image_manager.RemoveImage("all");
            main_text_controller.ClearText();

            main_text_controller.isNovelReading = true;

            // 最初の行を処理の実行開始！
            main_text_controller.GoToTheNextLine();
        }


        public void End()
        {
            Debug.Log("OK終了処理実行");
            image_manager.RemoveImage("all"); // すべての画像を削除する
            main_text_controller.ClearText();
            user_script_manager.StopRun();
            main_text_controller.isNovelReading = false;
            //非アクティブする前に上の３つがずっと処理し続けてしまっている
            this.gameObject.SetActive(false);
        }
    }
}
