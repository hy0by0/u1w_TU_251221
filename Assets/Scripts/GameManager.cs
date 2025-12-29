using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ゲームのフェーズリスト
    public enum EventState
    {
        //ここの構成変えるかも？
        Start, //ゲーム開始時演出
        Memory_Intro, //メモリー最初
        SelfTalk_1, //最初の独白
        WaitDeleteAction_1, //削除タイム１
        FlagCheck_1, //遺品フラグ１-耳
        Memory_2, //メモリー２つ目
        WaitDeleteAction_2, //削除タイム２
        FlagCheck_2, //遺品フラグ2-目
        Memory_3, //メモリー３つ目（パス）
        WaitDeleteAction_3, //削除タイム３ラスト
        FlagCheck_3, //遺品フラグ３-目（パスが描かれている）
        NormalEnd, //ノーマルエンド時
        TrueEnd //すべての思い出を取得するとTrueエンドへ
    }

    [Header("メインキャラ")]
    [SerializeField] private MainCharaController mainChara;

    [SerializeField] private Transform iconParent; //アイコンらの親オブジェクト

    public bool WatchMemoryA = false;
    public bool WatchMemoryB = false;
    public bool WatchMemoryC = false;
    public bool WatchMemoryD = false;

    //アイコンに紐づいたウィンドウオブジェクトとタグのペアリストを取得
    public List<IconPairData> objectPairs = new List<IconPairData>();

    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        WatchMemoryA = false;
        WatchMemoryB = false;
        WatchMemoryC = false;
        WatchMemoryD = false;
    }

    // Update is called once per frame
    void Update()
    {
        //　アイコンの追加テスト処理
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddIcon("memory_ribborn");
        }


        // 思い出シーン見たかどうかフラグ切り替え処理
        GameObject iconObj_A = null;
        IconController icon_A = null;

        if (!WatchMemoryA)
        {
            //Debug.Log("対応オブジェクトリスト発見！！");
            iconObj_A = iconParent.transform.Find("memory_ribborn").gameObject; //ここで結局見つけられてない
            icon_A = iconObj_A.gameObject.GetComponent<IconController>();

            if (icon_A.IsWatched)
            {
                Debug.Log("思い出データ確認フラグを感知");
                mainChara.ChangeSprite("ribborn");
                WatchMemoryA = true;
            }
        }
        
    }

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
    }


    private void PlayDeleteEffect(string name)
    {
        if (name == null) return;
        // icon の種類に応じて演出を変える
        if (name == "camera")
        {
            mainChara.ChangeSprite("invisible");
        }
    }


}
