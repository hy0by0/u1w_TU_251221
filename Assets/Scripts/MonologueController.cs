using NovelGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonologueController : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI monoText; //扱うテキスト(TMP)オブジェクト
    public bool isNovelReading = false; //テキスト表示処理を制御するためのフラグ。先走らせないための
    public bool isRead = false;
    public string[] monos;
    public int sentense_number;
    int _displayedSentenceLength;
    int _sentenceLength;
    float _time;
    float _feedTime;


    // Start is called before the first frame update
    void Start()
    {
        sentense_number = 0;
        isRead = false;
        if (monos.Length != 0)
        {
            DisplayMono(monos[sentense_number]); //１行目を表示させる
            sentense_number++;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //クリックされたら次の文章を表示
        if (Input.GetMouseButtonUp(0))
        {
            if (sentense_number == monos.Length)
            {
                ChangeActive(false);
            }
            else
            {
                DisplayMono(monos[sentense_number]);
                sentense_number++;
            }

        }
    }


    public void ChangeActive(bool active)
    {
        Debug.Log("モノローグ消えろ！！！");
        if (!active)
        {
            isRead = true;
        }
        this.gameObject.SetActive(active);
    }


    /// <summary>
    /// テキストを表示する関数
    /// </summary>
    /// <param name="sentense"></param>
    public void DisplayMono(string sentense)
    {
        //それぞれテキストオブジェクトに代入する
        monoText.text = sentense;
    }

}
