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
        Off,//ゲーム開始前時。画面クリックまでこの状態。
        Start, //ゲーム開始時演出。クリックされたらここ。
        Memory_Intro, //メモリー最初-約束
        SelfTalk_Prologue, //モノローグ最初の独白
        WaitDeleteAction_1, //削除タイム1回目
        FlagCheck_1, //遺品フラグ1-耳
        SelfTalk_BeforeCat,  //モノローグネコ思い出前
        Memory_Cat, //メモリー2つ目-ネコ。
        SelfTalk_AfterCat, //モノローグネコ思い出後
        SelfTalk_BeforeCicada, //モノローグセミ思い出前
        Memory_Cicada, //メモリー3つ目-セミ。
        SelfTalk_AfterCicada, //モノローグセミ思い出後
        WaitDeleteAction_2, //削除タイム2回目
        FlagCheck_2, //遺品フラグ2-目。
        SelfTalk_BeforeRibborn, //モノローグリボン思い出前
        Memory_Ribborn, //メモリー4つ目-髪型。。
        SelfTalk_AfterRibborn, //モノローグリボン思い出後
        SelfTalk_BeforeFight, //モノローグケンカ思い出前
        SelfTalk_FindFInalMemory, //モノローグケンカ思い出前にすべてのメモリーが残っていた場合の追加テキスト
        Memory_Fight, //メモリー5つ目-ケンカ。
        SelfTalk_AfterFight, //モノローグケンカ思い出後
        SelfTalk_SuccesPass, //モノローグパス思い出前。パスを入力成功時のテキスト
        WaitDeleteAction_3, //削除タイム3ラスト
        FlagCheck_3, //遺品フラグ目＋パス？。
        SelfTalk_BeforeDiary, //モノローグ日記思い出前
        CheckPassDiary, //日記パス
        SelfTalk_AfterDiary, //日記開いた場合モノローグ
        Memory_Diaray, //メモリー6つ目-日記。
        Memory_Pass, //メモリー7つ目-最期
        NormalEnd, //ノーマルエンド時。思い出も集めきれず、日記も見なかった場合
        SadEnd, //SADエンド時。懺悔
        TrueEnd //すべての思い出を取得するとTrueエンドへ
    }

    [Header("メインキャラ")]
    [SerializeField] private ImageController imageCont; //画像の変更管理用

    [Header("SoundManager入力")]
    [SerializeField] private SoundManager soundManager; //音の管理用

    [Header("SoundManager入力")]
    public EventState eventState = EventState.Off;  //現在のイベント状態を管理する変数。最初は「Off」にしておく。

    [Header("必要なオブジェクト入れる")]
    public Image OffImage; //ゲーム開始前の画面全体を覆う画像。クリックされたらフェードアウトさせる。
    public GameObject pcSound; //PC起動音用オブジェクト
    public PassManager passManager; //日記のパス入力管理用オブジェクト
    public MonologueController[] monologue; //モノローグ用
    public ErrorWindowManager errorWinManager; //エラーウィンドウの表示非表示用
    public GameObject postProcess;  //ポストプロセス管理用オブジェクト

    private bool isTrashed = false; //削除タイム中に削除が完了されたか
    private bool isEnd = false; //エンド迎えるかどうか

    [SerializeField] private Transform iconParent; //アイコンらの親オブジェクト

    [Header("管理するフラグ一覧")]
    public bool canWatch = true; //見えるかどうか
    public bool canHear = true; //聞こえるかどうか
    public static bool isFindDiary = false; //日記を発見したかどうか（後のヒント用に）
    public static bool isAppearFinalMemory = false; //最後のメモリーが出現させられたかどうか（後のヒント用に）
    public static bool WatchMemoryIntro = false; //導入メモリーの既読
    public static bool WatchMemoryCat = false;      //猫メモリーの既読
    public static bool WatchMemoryCicada = false;   //セミメモリーの既読
    public static bool WatchMemoryRibborn = false; //リボンメモリーの既読
    public static bool WatchMemoryFight = false;    //ケンカメモリーの既読
    public static bool WatchMemoryPass = false;     //パスで隠された最後のメモリーの既読
    public static bool WatchMemoryDiary = false;    //日記メモリーの既読

    //アイコンに紐づいたウィンドウオブジェクトとタグのペアリストを取得。オブジェクトの呼び出し用に。
    public List<IconPairData> objectPairs = new List<IconPairData>();

    public static GameManager Instance { get; private set; }    //シングルトン用インスタンス


    private void Awake()
    {
        Instance = this;
        eventState = EventState.Off;
        pcSound.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        isEnd = false;
        isTrashed = false;
        isFindDiary = false;
        isAppearFinalMemory = false;
        canWatch = true; //見えるかどうか
        canHear = true; //聞こえるかどうか
        WatchMemoryIntro = false;
        WatchMemoryCat = false;
        WatchMemoryCicada = false;
        WatchMemoryRibborn = false;
        WatchMemoryFight = false;
        WatchMemoryPass = false;
        passManager.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //　リトライ用
        if (Input.GetKeyDown(KeyCode.R))
        {
            Load("MainScene");
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
                OffImage.DOFade(0f, 1.5f); //画面蓋をとる。
                AddIcon("memory_intro"); // 最初のメモリーを開始
                StartCoroutine(PlaySound());
                imageCont.ChangeCameraSprite("indoor");
                eventState = EventState.Memory_Intro; //イベント状態を切り替え
            }
        }
        else if (eventState == EventState.Memory_Intro)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj_A = null;
            IconController icon_A = null;

            if (WatchMemoryIntro)
            {
                Debug.Log("イベント独白１回目開始！！！");
                imageCont.ChangeCameraSprite("outdoor");
                OpenCameraWindow();
                monologue[0].ChangeActive(true);
                eventState = EventState.SelfTalk_Prologue;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj_A = iconParent.transform.Find("memory_intro").gameObject;
                icon_A = iconObj_A.gameObject.GetComponent<IconController>();

                if (icon_A.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryIntro = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_Prologue)
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
                if (canHear)
                {
                    Debug.Log("思い出ネコモノローグへ。音鳴らす？");
                    imageCont.ChangeCameraSprite("cat");
                    OpenCameraWindow();
                    soundManager.PlaySE("bell");
                    monologue[2].ChangeActive(true);
                    eventState = EventState.SelfTalk_BeforeCat;
                }
                else
                {
                    Debug.Log("モノローグセミ思い出前へ");
                    imageCont.ChangeCameraSprite("indoor");
                    monologue[4].ChangeActive(true);
                    eventState = EventState.SelfTalk_BeforeCicada;
                }

            }
        }
        else if (eventState == EventState.SelfTalk_BeforeCat)
        {
            if (monologue[2].isRead)
            {
                Debug.Log("メモリー猫開始！！");
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
                imageCont.ChangeCameraSprite("outdoor");
                OpenCameraWindow();
                monologue[3].ChangeActive(true);
                eventState = EventState.SelfTalk_AfterCat;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_cat").gameObject;
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryCat = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_AfterCat)
        {
            if (monologue[3].isRead)
            {
                imageCont.ChangeCameraSprite("indoor");
                OpenCameraWindow();
                monologue[4].ChangeActive(true);
                eventState = EventState.SelfTalk_BeforeCicada;
            }

        }
        else if (eventState == EventState.SelfTalk_BeforeCicada)
        {
            if (monologue[4].isRead)
            {
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
                imageCont.ChangeCameraSprite("outdoor");
                OpenCameraWindow();
                monologue[5].ChangeActive(true);
                eventState = EventState.SelfTalk_AfterCicada;
            }
            else
            {
                iconObj = iconParent.transform.Find("memory_cicada").gameObject;
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    //imageCont.ChangeSprite("ribborn");
                    WatchMemoryCicada = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_AfterCicada)
        {
            if (monologue[5].isRead)
            {
                errorWinManager.PutErrrorWindow();
                eventState = EventState.WaitDeleteAction_2;
            }

        }
        else if (eventState == EventState.WaitDeleteAction_2)
        {
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
                    imageCont.ChangeCameraSprite("ribborn");
                    OpenCameraWindow();
                    monologue[7].ChangeActive(true);
                    eventState = EventState.SelfTalk_BeforeRibborn;
                }
                else
                {
                    imageCont.ChangeCameraSprite("indoor");
                    OpenCameraWindow();
                    monologue[9].ChangeActive(true);
                    eventState = EventState.SelfTalk_BeforeFight;
                }

            }
        }
        else if (eventState == EventState.SelfTalk_BeforeRibborn)
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
                imageCont.ChangeCameraSprite("outdoor");
                OpenCameraWindow();
                monologue[8].ChangeActive(true);
                eventState = EventState.SelfTalk_AfterRibborn;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_ribborn").gameObject;
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    imageCont.ChangeCharaSprite("ribborn");
                    WatchMemoryRibborn = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_AfterRibborn)
        {
            if (monologue[8].isRead)
            {
                imageCont.ChangeCameraSprite("indoor");
                OpenCameraWindow();
                monologue[9].ChangeActive(true);
                eventState = EventState.SelfTalk_BeforeFight;
            }

        }
        else if (eventState == EventState.SelfTalk_BeforeFight)
        {
            if (monologue[9].isRead)
            {
                //これまでのメモリーを残していたら、最後のメモリーを開放、それに伴うセリフの追加を行う
                if (WatchMemoryIntro & WatchMemoryCat & WatchMemoryCicada & WatchMemoryRibborn)
                {
                    isAppearFinalMemory = true; //最後のメモリーが出現させられたフラグを立てておく（後のヒント用に）
                    monologue[18].ChangeActive(true);
                    eventState = EventState.SelfTalk_FindFInalMemory;
                }
                else
                {
                    AddIcon("memory_fight");
                    eventState = EventState.Memory_Fight;
                }
                    

            }
        }
        else if (eventState == EventState.SelfTalk_FindFInalMemory)
        {
            if (monologue[18].isRead)
            {
                AddIcon("memory_fight");
                eventState = EventState.Memory_Fight;

            }
        }
        else if (eventState == EventState.Memory_Fight)
        {

            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryFight)
            {
                imageCont.ChangeCameraSprite("indoor");
                OpenCameraWindow();
                if (WatchMemoryIntro & WatchMemoryCat & WatchMemoryCicada & WatchMemoryRibborn & WatchMemoryFight)
                {
                    AddIcon("memory_pass");
                }
                monologue[10].ChangeActive(true);
                eventState = EventState.SelfTalk_AfterFight;
            }
            else
            {
                iconObj = iconParent.transform.Find("memory_fight").gameObject;
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    WatchMemoryFight = true;
                }
            }

        }
        else if (eventState == EventState.SelfTalk_AfterFight)
        {
            if (monologue[10].isRead)
            {
                if (WatchMemoryIntro & WatchMemoryCat & WatchMemoryCicada & WatchMemoryRibborn & WatchMemoryFight)
                {
                    //Debug.Log("モノローグ3が終了しました！");
                    monologue[19].ChangeActive(true);
                    eventState = EventState.SelfTalk_SuccesPass;
                }
                else
                {
                    Debug.Log("モノローグ１が終了しました！");
                    errorWinManager.PutErrrorWindow();
                    eventState = EventState.WaitDeleteAction_3;
                }
                
            }


        }
        else if (eventState == EventState.SelfTalk_SuccesPass)
        {
            if (monologue[19].isRead)
            {
                errorWinManager.PutErrrorWindow();
                eventState = EventState.WaitDeleteAction_3;

            }
        }
        else if (eventState == EventState.WaitDeleteAction_3)
        {
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryPass)
            {
                imageCont.ChangeCameraSprite("outdoor");
                OpenCameraWindow();
                monologue[17].ChangeActive(true);
                eventState = EventState.TrueEnd;
            }
            else
            {
                iconObj = iconParent.transform.Find("memory_pass").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    WatchMemoryPass = true;
                }

                if (isTrashed)
                {
                    errorWinManager.Remove();
                    isTrashed = false;
                    monologue[11].ChangeActive(true);
                    eventState = EventState.FlagCheck_3;
                }
            }
        }
        else if (eventState == EventState.FlagCheck_3)
        {
            if (monologue[11].isRead)
            {
                if (canWatch)
                {
                    Debug.Log("日記へ");
                    imageCont.ChangeCameraSprite("diary");
                    OpenCameraWindow();
                    monologue[12].ChangeActive(true);
                    eventState = EventState.SelfTalk_BeforeDiary;
                }
                else
                {
                    Debug.Log("ノーマルエンドへ");
                    imageCont.ChangeCameraSprite("indoor");
                    OpenCameraWindow();
                    monologue[16].ChangeActive(true);
                    eventState = EventState.NormalEnd;//この前にモノローグはさむ？
                }

            }
        }
        else if (eventState == EventState.SelfTalk_BeforeDiary)
        {
            if (monologue[12].isRead)
            {
                isFindDiary = true; //日記発見フラグを立てる
                passManager.gameObject.SetActive(true);
                eventState = EventState.CheckPassDiary;
            }

        }
        else if (eventState == EventState.CheckPassDiary)
        {
            if (passManager.isOpen)
            {
                monologue[13].ChangeActive(true);
                eventState = EventState.SelfTalk_AfterDiary;
            }
            else if(passManager.isFaild)
            {
                Debug.Log("ノーマルエンドへ");
                imageCont.ChangeCameraSprite("indoor");
                OpenCameraWindow();
                monologue[16].ChangeActive(true);
                eventState = EventState.NormalEnd;//この前にモノローグはさむ？
            }

        }
        else if (eventState == EventState.SelfTalk_AfterDiary)
        {
            if (monologue[13].isRead)
            {
                AddIcon("memory_diary");
                eventState = EventState.Memory_Diaray;
            }

        }
        else if (eventState == EventState.Memory_Diaray)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryDiary)
            {
                Debug.Log("SADENDへ");
                //monologue[8].ChangeActive(true);
                if (WatchMemoryRibborn)
                {
                    imageCont.ChangeCharaSprite("sad_ribborn");
                }
                else
                {
                    imageCont.ChangeCharaSprite("sad");
                }
                monologue[15].ChangeActive(true);
                eventState = EventState.SadEnd;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_diary").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    WatchMemoryDiary = true;
                }
            }

        }
        else if (eventState == EventState.Memory_Pass)
        {
            // 思い出シーン見たかどうかフラグ切り替え処理
            GameObject iconObj = null;
            IconController icon = null;

            if (WatchMemoryPass)
            {
                Debug.Log("TRUEENDへ");
                //monologue[8].ChangeActive(true);
                eventState = EventState.TrueEnd;
            }
            else
            {
                //Debug.Log("対応オブジェクトリスト発見！！");
                iconObj = iconParent.transform.Find("memory_pass").gameObject; //ここで結局見つけられてない
                icon = iconObj.gameObject.GetComponent<IconController>();

                if (icon.IsWatched)
                {
                    Debug.Log("思い出データ確認フラグを感知");
                    WatchMemoryPass = true;
                }
            }

        }
        else if (eventState == EventState.NormalEnd)
        {
            if (monologue[16].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                imageCont.ChangeCameraSprite("indoor");
                //シーン移動
                GoEnd("normal");
            }
        }
        else if (eventState == EventState.SadEnd)
        {
            if (monologue[15].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                imageCont.ChangeCameraSprite("indoor");
                //シーン移動
                GoEnd("sad");
            }
        }
        else if (eventState == EventState.TrueEnd)
        {
            if (monologue[17].isRead)
            {
                Debug.Log("モノローグ3が終了しました！");
                imageCont.ChangeCameraSprite("indoor");
                //シーン移動
                GoEnd("true");
            }
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
                iconObj = iconParent.transform.Find(pair.IconName).gameObject;
                icon = iconObj.gameObject.GetComponent<IconController>();
            }
        }
        if (iconObj == null) return;
        iconObj.gameObject.SetActive(true);
        if (iconName == "memory_pass")
        {
            return;
        }
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
            if (WatchMemoryRibborn)
            {
                imageCont.ChangeCharaSprite("ribborn_invisible");
            }
            else
            {
                imageCont.ChangeCharaSprite("invisible");
            }
            imageCont.FadaIn("AddBackGround");
            canWatch = false;
        }
        if (name == "sound")
        {
            soundManager.SetBGMVolume(0.0f);
            canHear = false;
        }
        if (name == "memory_intro") WatchMemoryIntro = false;
        if (name == "memory_cat") WatchMemoryCat = false;
        if (name == "memory_cicada") WatchMemoryCicada = false;
        if (name == "memory_ribborn") WatchMemoryRibborn = false;
        if (name == "memory_fight") WatchMemoryFight = false;
        if (name == "memory_pass") WatchMemoryPass = false;

        return;
    }


    /// <summary>
    /// 音声開始処理
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaySound()
    {
        //web版では、ゲームの音が反映されるまでに少しラグがあるため、サウンド開始前に少し待機させる
        yield return new WaitForSeconds(1.3f); //ウェブ上で音が反映されるまで待機させる
        pcSound.SetActive(true); //PC起動音開始
        yield return new WaitForSeconds(1.0f); //起動して遅れてメインサウンドも聞こえるようにする
        soundManager.PlaySound(); //メインサウンド開始
    }


    /// <summary>
    /// カメラアプリを開く関数
    /// </summary>
    public void OpenCameraWindow()
    {
        if (canWatch)
        {
            GameObject iconObj = iconParent.transform.Find("camera").gameObject;
            IconController icon = iconObj.gameObject.GetComponent<IconController>();
            icon.OpenWindow();
        }
    }

    /// <summary>
    /// エンド処理関数
    /// </summary>
    /// <param name="name"></param>
    public void GoEnd(string name)
    {
        if (name == null)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if (name  == "true")
        {
            isEnd = true;
            StartCoroutine(GoTrueEnd());
        }
        else
        {
            isEnd = true;
            Load("SadEnd");
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

    /// <summary>
    /// トゥルーエンドシーン遷移処理関数
    /// </summary>
    /// <returns></returns>
    IEnumerator GoTrueEnd()
    {
        //フェードアウト演出
        imageCont.FadaIn("WhitePanel");
        postProcess.SetActive(false);

        //フェードアウト処理終了まで待機後、Trueエンドシーン遷移
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("TrueEnd");
    }


}
