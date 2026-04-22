using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageController : MonoBehaviour
{

    [Header("画面フェード遷移")]
    public Image WhitePanel; //白幕
    public float fadeInBlindTime = 0.5f; //盲目時に背景を遷移させる時間
    public float fadeInWhiteTime = 1f; //白幕フェードインする時間

    [Header("主人公のスプライト差分集")]
    public Sprite normal;
    public Sprite invisible;
    public Sprite ribborn;
    public Sprite ribborn_invisible;
    public Sprite sad;
    public Sprite sad_ribborn;
    public Sprite sad_ribborn_invisible;

    [Header("カメラの風景スプライト集")]
    public Sprite indoor;
    public Sprite outdoor;
    public Sprite cat;
    public Sprite ribborn_camera;
    public Sprite diary;

    public Image Chara;
    public Image CameraImage;
    public Image AddBackGround;


    /// <summary>
    /// メインキャラ画像を変更させる関数
    /// </summary>
    /// <param name="name"></param>
    public void ChangeCharaSprite(string name)
    {
        if (name == "ribborn_invisible")
        {
            Chara.sprite = ribborn_invisible;
        }
        else if (name == "invisible")
        {
            Chara.sprite = invisible;
        }
        else if (name == "ribborn")
        {
            Chara.sprite = ribborn;
        }
        else if (name == "sad")
        {
            Chara.sprite = sad;
        }
        else if (name == "sad_ribborn")
        {
            Chara.sprite = sad_ribborn;
        }
        else if (name == "sad_ribborn_invisible")
        {
            Chara.sprite = sad_ribborn_invisible;
        }
    }


    /// <summary>
    /// カメラウィンドウに表示される画像を変更する関数
    /// </summary>
    /// <param name="name"></param>
    public void ChangeCameraSprite(string name)
    {
        if (name == "indoor")
        {
            CameraImage.sprite = indoor;
        }
        else if (name == "outdoor")
        {
            CameraImage.sprite = outdoor;
        }
        else if (name == "cat")
        {
            CameraImage.sprite = cat;
        }
        else if (name == "ribborn")
        {
            CameraImage.sprite = ribborn_camera;
        }
        else if (name == "diary")
        {
            CameraImage.sprite = diary;
        }
    }


    /// <summary>
    /// 画像のフェードイン処理関数
    /// </summary>
    /// <param name="ImageName"></param>
    public void FadaIn(string ImageName)
    {
        if (ImageName == "AddBackGround")
        {
            AddBackGround.DOFade(0.8f, fadeInBlindTime);
            
        }
        else if (ImageName == "WhitePanel")
        {
            WhitePanel.DOFade(1f, fadeInWhiteTime);
        }
    }


}
