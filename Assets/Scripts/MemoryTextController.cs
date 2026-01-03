using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Rendering.PostProcessing;
using UnityEngine;


namespace NovelGame
{
    public class MemoryTextController : MonoBehaviour
    {
        [SerializeField] private NovelManager novelManager;
        [SerializeField] TextMeshProUGUI _mainTextObject;
        [SerializeField] TextMeshProUGUI _nameTextObject;
        public bool isNovelReading = false; //テキスト表示処理を制御するためのフラグ。先走らせないための
        int _displayedSentenceLength;
        int _sentenceLength;
        float _time;
        float _feedTime;


        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("OK文字送りなどの初期設定が実行されました");
            _time = 0f;
            _feedTime = 0.05f;

            //DisplayText();
        }


        // Update is called once per frame
        void Update()
        {
            if (!isNovelReading) return;

            // 文章を１文字ずつ表示する
            _time += Time.deltaTime; //feedTimeごとに文字送りをさせる
            if (_time >= _feedTime)
            {
                _time -= _feedTime;
                if (!CanGoToTheNextLine())
                {
                    //_displayedSentenceLengthの大きさだけテキストが表示されるようにする
                    _displayedSentenceLength++;
                    _mainTextObject.maxVisibleCharacters = _displayedSentenceLength;
                }
            }


            //　クリックされたとき、次の行へ移動
            if (Input.GetMouseButtonUp(0))
            {
                if (CanGoToTheNextLine())
                {
                    GoToTheNextLine();
                    DisplayText();
                }
                else //(まだ表示するテキストが残ってたら)
                {
                    //いきなり全文字数にすることで、クイック表示されるようにする
                    _displayedSentenceLength = _sentenceLength;
                }

            }

        }



        /// <summary>
        /// その行の、すべての文字が表示されていなければ、まだ次の行へ進むことはできない
        /// </summary>
        /// <returns></returns>
        public bool CanGoToTheNextLine()
        {
            string sentence = novelManager.user_script_manager.GetCurrentSentence(); //ここが行早く進んでしまっている。
            string[] words = sentence.Split(',');
            //Debug.Log($"[{words[0]}]");
            string textsentence = words[2];
            _sentenceLength = textsentence.Length;
            //表示する文字数がテキストの文字数を上回ったらTrueを出す。
            return (_displayedSentenceLength > textsentence.Length);
        }


        /// <summary>
        /// 次の行へ移動させる。行番号の更新
        /// </summary>
        public void GoToTheNextLine()
        {
            //この関数が最初に３回も呼ばれてしまっている
            Debug.Log("GoTONEXT");
            //値の初期化をする
            _displayedSentenceLength = 0;
            _time = 0f;
            _mainTextObject.maxVisibleCharacters = 0;

            // 現在の行の値を増加
            novelManager.line_number++;

            //現在の文を定義し、それが命令文であればそれを実行する
            string sentence = novelManager.user_script_manager.GetCurrentSentence(); //ここが行早く進んでしまっている。
            if (novelManager.user_script_manager.IsStatement(sentence))
            {
                //IsStateまでは正常にできているが、こここに来るまでに最初初期化再度されてしまっている
                //Debug.Log("コマンド実行命令が下った！！");
                // 命令文の実行をUserScriptManagerにて実行
                novelManager.user_script_manager.ExecuteStatement(sentence);
            }

        }



        /// <summary>
        /// 該当テキストを表示させる
        /// </summary>
        public void DisplayText()
        {
            //現在の文章をuserScriptManagerのGetCurrentSentenceで引っ張ってくる
            string sentence = novelManager.user_script_manager.GetCurrentSentence();
            //カンマ区切りで名前と本文をわける
            string[] words = sentence.Split(',');

            //1番が名前、2番が本文
            string namesentence = words[1];
            string textsentence = words[2];
            //それぞれテキストオブジェクトに代入する
            _mainTextObject.text = textsentence;
            _nameTextObject.text = namesentence;
        }


        /// <summary>
        /// 初期化の関数。最初に呼び出されるときに実行されるようにする
        /// </summary>
        public void ClearText()
        {
            isNovelReading = false;

            //値の初期化をする
            _displayedSentenceLength = 0;
            _time = 0f;
            _mainTextObject.maxVisibleCharacters = 0;
            //Debug.Log("OKテキストコントローラー初期化");

            // 最初の行のテキストを表示、または命令を実行
            string statement = novelManager.user_script_manager.GetCurrentSentence();
            if (novelManager.user_script_manager.IsStatement(statement)) //その行の文章命令文判定なら
            {
                //Debug.Log("最初は命令だあああああ");
                // 命令文の実行をUserScriptManagerにて実行
                novelManager.user_script_manager.ExecuteStatement(statement);
            }

            // 最初の行のテキストを表示
            DisplayText();
        }


    }

}
