using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PassManager : MonoBehaviour
{
    public bool isTruePass = false; //最期のメモリー用か、日記用かを区別
    public bool isOpen = false;
    public bool isFaild = false; //一度失敗したら。
    public WindowObject windowObj; //True時のウィンドウ
    [SerializeField] TextMeshProUGUI anounceText;
    public InputPass[] textBox;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isFaild = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JudgePass()
    {
        bool checkPass = true;
        foreach (var item in textBox)
        {
            if (!item.isPass)
            {
                checkPass = false;
            }
        }

        if (isTruePass) //最期メモリーパス
        {
            if (checkPass)
            {
                //オープンウィンドウ
                windowObj.OpenWindow();
                Close();
            }
            else
            {
                //普通に閉じる
                anounceText.color = new Color(255f, 0.0f, 0.0f, 1.0f);
            }
        }
        else //日記パス
        {
            if (checkPass)
            {
                isFaild = false;
                isOpen = true;
                //シンプルに切り替え
                Close();
            }
            else
            {
                isOpen = false;
                isFaild = true;
                Close();
            }
        }

    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }


}
