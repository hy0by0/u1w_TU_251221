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
            // これで、別クラスからGameManagerの変数などを使えるようにする
            Instance = this;

            line_number = 0;
        }
    }
}
