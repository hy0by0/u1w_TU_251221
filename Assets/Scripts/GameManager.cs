using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ゲームのフェーズリスト
    public enum EventState
    {
        //ここの構成変えるかも？
        Off,//ゲーム開始前時。画面クリックまでこの状態。
        Start, //ゲーム開始時演出。クリックされたらここ。
        Memory_Intro, //メモリー最初。約束
        SelfTalk_1, //最初の独白
        WaitDeleteAction_1, //削除タイム１
        FlagCheck_1, //遺品フラグ１-耳
        SelfTalk_2,
        Memory_Cat, //メモリー２つ目ネコ。
        SelfTalk_3,
        SelfTalk_4,
        Memory_Cicada, //メモリー３つ目。セミ。
        SelfTalk_5,
        WaitDeleteAction_2, //削除タイム２
        FlagCheck_2, //遺品フラグ2-目。
        SelfTalk_6,
        Memory_Ribborn, //メモリー４つ目髪型。。
        SelfTalk_7,
        SelfTalk_8,
        Memory_Fight, //メモリー５つ目。ケンカ。
        SelfTalk_9,
        WaitDeleteAction_3, //削除タイム３ラスト
        FlagCheck_3, //遺品フラグ目＋パス？。
        SelfTalk_10,
        Memory_Pass, //遺品フラグ３-日記。
        SelfTalk_11, //必要かは不明。SadEndにまとめられるかも？
        NormalEnd, //ノーマルエンド時。思い出も集めきれず、日記も見なかった場合
        SadEnd, //SADエンド時。懺悔
        TrueEnd //すべての思い出を取得するとTrueエンドへ
    }

    [Header("メインキャラ")]
    [SerializeField] private ImageController imageCont;

    [Header("SoundManager入力")]
    [SerializeField] private SoundManager soundManager;

    [Header("SoundManager入力")]
    public EventState eventState = EventState.Off;

    [Header("必要なオブジェクト入れる")]
    public Image OffImage;
    public GameObject pcSound;
    public MonologueController[] monologue; //モノローグ用
    public ErrorWindowManager errorWinManager; //エラーウィンドウの表示非表示用

    private bool isTrashed = false;

    [SerializeField] private Transform iconParent; //アイコンらの親オブジェクト

    [Header("フラグチェッカー")]
    public bool canWatch = true; //見えるかどうか
    public bool canHear = true; //聞こえるかどうか
    public bool WatchMemoryIntro = false; //思い出Aの既読があるかどうか
    public bool WatchMemoryCat = false;
    public bool WatchMemoryCicada = false;
    public bool WatchMemoryRibborn = false;
    public bool WatchMemoryFight = false;
    public bool WatchMemoryPass = false;

    //アイコンに紐づいたウィンドウオブジェクトとタグのペアリストを取得。オブジェクトの呼び出し用に。
    public List<IconPairData> objectPairs = new List<IconPairData>();

    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        eventState = EventState.Off;
        pcSound.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        isTrashed = false;
        canWatch = true; //見えるかどうか
        canHear = true; //聞こえるかどうか
        WatchMemoryIntro = false;
        WatchMemoryCat = false;
        WatchMemoryCicada = false;
        WatchMemoryRibborn = false;
        WatchMemoryFight = false;
        WatchMemoryPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        //　リトライ用
        if (Input.GetKeyDown(KeyCode.R))
        {
            Load("SampleScene");
        }

        // クリック音用
        if (Input.GetMouseButtonDown(0))
        {
            soundManager.PlaySE("click");
        }

        if (eventState == EventState.Off)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("イベント：スタート状態へ");
                pcSound.SetActive(true); //PC起動音開始
                OffImage.DOFade(0f, 1.5f); //画面蓋をとる。
                AddIcon("memory_intro"); // 最初のメモリーを開始
                soundManager.PlaySound(); //サウンド開始
                eventState = EventState.Memory_Intro; //イベント状態を切り替え
            }
        }
        else if (eventState == EventState.Memory_Intro)
        {
            Debug.Log("イベント：メモリー１つ目中");

            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj_A = null;
            IconController icon_A = null;

            if (WatchMemoryIntro)
            {
                Debug.Log("イベント独白１回目開始！！！");
                monologue[0].ChangeActive(true);
                eventState = EventState.SelfTalk_1;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj_A = iconParent.transform.Find("memory_intro").gameObject; //ここで結局見つけられてない
                icon_A = iconObj_A.gameObject.GetComponent<IconController>();

                if (icon_A.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryIntro = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_1)
        {
            if (monologue[0].isRead)
            {
                Debug.Log("モノローグ１が終了しました！");
                Debug.Log("削除ターン１回目！");
                errorWinManager.PutErrrorWindow();
                eventState = EventState.WaitDeleteAction_1;
            }

        }
        else if (eventState == EventState.WaitDeleteAction_1)
        {
            //Debug.Log("イベント削除１回目！！");
            if (isTrashed)
            {
                Debug.Log("削除１回目が完了！！");
                errorWinManager.Remove();
                isTrashed = false;
                monologue[1].ChangeActive(true);
                eventState = EventState.FlagCheck_1;
            }
        }
        else if (eventState == EventState.FlagCheck_1)
        {
            if (monologue[1].isRead)
            {
                Debug.Log("モノローグ2が終了しました！");
                if (canHear)
                {
                    Debug.Log("思い出ネコモノローグへ。音鳴らす？");
                    monologue[2].ChangeActive(true);
                    eventState = EventState.SelfTalk_2;
                }
                else
                {
                    Debug.Log("モノローグセミ思い出前へ");
                    monologue[4].ChangeActive(true);
                    eventState = EventState.SelfTalk_4;
                }

            }
        }
        else if (eventState == EventState.SelfTalk_2)
        {
            if (monologue[2].isRead)
            {
                Debug.Log("メモリーネコ開始！！");
                //鈴の音入れるならここかも
                AddIcon("memory_cat");
                eventState = EventState.Memory_Cat;
            }

        }
        else if (eventState == EventState.Memory_Cat)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryCat)
            {
                Debug.Log("メモリー猫終了！！");
                Debug.Log("モノローグ４へ");
                monologue[3].ChangeActive(true);
                eventState = EventState.SelfTalk_3;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_cat").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryCat = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_3)
        {
            if (monologue[3].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                monologue[4].ChangeActive(true);
                eventState = EventState.SelfTalk_4;
            }

        }
        else if (eventState == EventState.SelfTalk_4)
        {
            if (monologue[4].isRead)
            {
                Debug.Log("モノローグ4が終了しました！");
                AddIcon("memory_cicada");
                eventState = EventState.Memory_Cicada;
            }

        }
        else if (eventState == EventState.Memory_Cicada)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryCicada)
            {
                Debug.Log("イベント独白１回目開始！！！");
                monologue[5].ChangeActive(true);
                eventState = EventState.SelfTalk_5;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_cicada").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryCicada = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_5)
        {
            if (monologue[5].isRead)
            {
                Debug.Log("モノローグ１が終了しました！");
                errorWinManager.PutErrrorWindow();
                eventState = EventState.WaitDeleteAction_2;
            }

        }
        else if (eventState == EventState.WaitDeleteAction_2)
        {
            Debug.Log("イベント削除２回目！！"); //後で並び替え
            if (isTrashed)
            {
                Debug.Log("削除2回目が完了！！");
                errorWinManager.Remove();
                isTrashed = false;
                monologue[6].ChangeActive(true);
                eventState = EventState.FlagCheck_2;
            }
        }
        else if (eventState == EventState.FlagCheck_2)
        {
            if (monologue[6].isRead)
            {
                Debug.Log("モノローグ2が終了しました！");
                if (canWatch)
                {
                    Debug.Log("思い出リボンへ。音鳴らす？");
                    monologue[7].ChangeActive(true);
                    eventState = EventState.SelfTalk_6;
                }
                else
                {
                    Debug.Log("モノローグセミ思い出前へ");
                    monologue[9].ChangeActive(true);
                    eventState = EventState.SelfTalk_8;
                }

            }
        }
        else if (eventState == EventState.SelfTalk_6)
        {
            if (monologue[7].isRead)
            {
                AddIcon("memory_ribborn");
                eventState = EventState.Memory_Ribborn;
            }

        }
        else if (eventState == EventState.Memory_Ribborn)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryRibborn)
            {
                Debug.Log("イベント独白１回目開始！！！");
                monologue[8].ChangeActive(true);
                eventState = EventState.SelfTalk_7;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_ribborn").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知"); //ここで処理ストップしてしまっている！！
                    imageCont.ChangeSprite("ribborn");
                    WatchMemoryRibborn = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_7)
        {
            if (monologue[8].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                monologue[9].ChangeActive(true);
                eventState = EventState.SelfTalk_8;
            }

        }
        else if (eventState == EventState.SelfTalk_8)
        {
            if (monologue[9].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                AddIcon("memory_fight");
                eventState = EventState.Memory_Fight;

            }
        }
        else if (eventState == EventState.Memory_Fight)
        {
            Debug.Log("イベント：メモリー１つ目中");

            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryFight)
            {
                Debug.Log("イベント独白１回目開始！！！");
                monologue[10].ChangeActive(true);
                eventState = EventState.SelfTalk_9;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_fight").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    WatchMemoryFight = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_9)
        {
            if (monologue[10].isRead)
            {
                Debug.Log("モノローグ１が終了しました！");
                errorWinManager.PutErrrorWindow();
                eventState = EventState.WaitDeleteAction_3;
            }


        }
        else if (eventState == EventState.WaitDeleteAction_3)
        {
            //Debug.Log("イベント削除１回目！！");
            if (isTrashed)
            {
                Debug.Log("削除１回目が完了！！");
                errorWinManager.Remove();
                isTrashed = false;
                monologue[11].ChangeActive(true);
                eventState = EventState.FlagCheck_3;
            }
        }
        else if (eventState == EventState.FlagCheck_3)
        {
            if (monologue[11].isRead)
            {
                Debug.Log("モノローグ2が終了しました！");
                if (canWatch)
                {
                    Debug.Log("思い出リボンへ。音鳴らす？");
                    //鈴の音入れるならここかも
                    monologue[12].ChangeActive(true);
                    eventState = EventState.SelfTalk_10;
                }
                else
                {
                    Debug.Log("ノーマルエンドへ");
                    //monologue[9].ChangeActive(true);
                    eventState = EventState.NormalEnd;//この前にモノローグはさむ？
                }

            }
        }
        else if (eventState == EventState.SelfTalk_10)
        {
            if (monologue[12].isRead)
            {
                AddIcon("memory_pass");
                eventState = EventState.Memory_Pass;
            }

        }
        else if (eventState == EventState.Memory_Pass)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryPass)
            {
                Debug.Log("SADENDへ");
                //monologue[8].ChangeActive(true);
                eventState = EventState.SadEnd;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_pass").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    imageCont.ChangeSprite("ribborn");
                    WatchMemoryPass = true;
                }
            }

        }
        else if (eventState == EventState.NormalEnd)
        {
            Debug.Log("イベント独白１回目！！");
        }
        else if (eventState == EventState.SadEnd)
        {
            Debug.Log("イベント独白１回目！！");
        }
        else if (eventState == EventState.TrueEnd)
        {
            Debug.Log("イベント独白１回目！！");
        }


    }





    /// <summary>
    /// アイコン追加させる関数
    /// </summary>
    /// <param name="iconName">対象アイコン</param>
    public void AddIcon(string iconName)
    {
        GameObject iconObj = null;
        IconController icon = null;

        foreach (var pair in objectPairs)
        {
            //Debug.Log(pair.IconName);
            if (pair.IconName == iconName)
            {
                Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find(pair.IconName).gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();
            }
        }
        if (iconObj == null) return;
        iconObj.gameObject.SetActive(true);
        icon.OpenWindow();
    }



    /// <summary>
    /// 実行する削除処理の管理関数
    /// </summary>
    /// <param name="icon">削除対象アイコン</param>
    public void OnIconDroppedToTrash(IconController icon)
    {
        Debug.Log($"UIアイコンが捨てられた: {icon.name}");

        //string iconName = null;
        //GameObject windowObj = null;

        Debug.Log(icon.name);

        //if (iconName == null) return;

        //対応する削除演出を行う
        PlayDeleteEffect(icon.name);

        //該当のアイコンを消す
        icon.gameObject.SetActive(false);
        icon.windowObj.gameObject.SetActive(false);
        isTrashed = true;
    }


    /// <summary>
    /// 削除したアイコンに応じた削除演出
    /// </summary>
    /// <param name="name"></param>
    private void PlayDeleteEffect(string name)
    {
        if (name == null) return;
        // icon の種類に応じて演出を変える
        if (name == "camera")
        {
            imageCont.ChangeSprite("invisible");
            imageCont.FadaIn("AddBackGround");
            canWatch = false;
        }
        else if (name == "sound")
        {
            soundManager.SetBGMVolume(0.0f);
            canHear = false;
        }
        else if (name == "memory_intro")
        {
            WatchMemoryIntro = false;
        }
        else if (name == "memory_cat")
        {
            WatchMemoryCat = false;
        }
        else if (name == "memory_cicada")
        {
            WatchMemoryCicada = false;
        }
        else if (name == "memory_ribborn")
        {
            WatchMemoryRibborn = false;
        }
        else if (name == "memory_fight")
        {
            WatchMemoryFight = false;
        }
    }


    /// <summary>
    /// シーン移動関数
    /// </summary>
    /// <param name="name"></param>
    public void Load(string name)
    {
        if (name == null)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene(name);
        }
            
    }


}
