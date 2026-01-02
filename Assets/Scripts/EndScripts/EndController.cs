using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class EndController : MonoBehaviour
{
    public bool isTrueEnd = false;
    private bool isGoFinal = false;
    private bool isFinish = false;
    public MonologueController monologue;
    public ChangeScene changeScene;
    public GameObject UIObj;
    public GameObject postEffectObj;

    public Image resultPanel;
    public Image resultIntro;
    public Image resultCat;
    public Image resultCicada;
    public Image resultRibborn;
    public Image resultFight;
    public Image resultPass;
    public Image WhitePanel;


    // Start is called before the first frame update
    void Start()
    {
        isGoFinal = false;
        isFinish = false;
        if (isTrueEnd)
        {
            UIObj.SetActive(false);
            postEffectObj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            changeScene.Load("SampleScene");
        }

        if (isFinish)
        {
            return;
        }

        if (isGoFinal)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Final());
            }
            return;
        }

        if (isTrueEnd)
        {
            if (monologue.isRead)
            {
                DoTrueEnd();
            }
        }
        else
        {
            Debug.Log("ラスト");
            CheckResult();

        }
    }

    public void CheckResult()
    {
        if (WatchMemoryIntro)
        {
            resultIntro.DOColor(new Color(255f, 255, 255), 0f);
            resultIntro.DOFade(1f, 0f);
        }
        if (WatchMemoryCat)
        {
            resultCat.DOColor(new Color(255f, 255, 255), 0f);
            resultCat.DOFade(1f, 0f);
        }
        if (WatchMemoryCicada)
        {
            resultCicada.DOColor(new Color(255f, 255, 255), 0f);
            resultCicada.DOFade(1f, 0f);
        }
        if (WatchMemoryRibborn)
        {
            resultRibborn.DOColor(new Color(255f, 255, 255), 0f);
            resultRibborn.DOFade(1f, 0f);
        }
        if (WatchMemoryFight)
        {
            resultFight.DOColor(new Color(255f, 255, 255), 0f);
            resultFight.DOFade(1f, 0f);
        }
        if (WatchMemoryPass)
        {
            resultPass.DOColor(new Color(255f, 255, 255), 0f);
            resultPass.DOFade(1f, 0f);
        }
        resultPanel.DOFade(1f, 0f);
        //UIObj.SetActive(true);
        isFinish = true;
    }

    private void DoTrueEnd()
    {
        Debug.Log("ラストスチルを出します！！");
        isGoFinal = true;
        resultPanel.DOFade(1f, 1.5f);
    }


    IEnumerator Final()
    {
        Debug.Log("最期の画面！！");
        isFinish = true;
        WhitePanel.DOFade(1f, 2f).SetLoops(2, LoopType.Yoyo);
        yield return new WaitForSeconds(1.5f);
        postEffectObj.SetActive(true);
        UIObj.SetActive(true);
        
    }
}
