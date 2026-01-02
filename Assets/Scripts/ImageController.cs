using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageController : MonoBehaviour
{
    [Header("主人公のスプライト差分集")]
    public Sprite normal;
    public Sprite invisible;
    public Sprite ribborn;
    public Sprite ribborn_invisible;

    [Header("カメラの風景スプライト集")]
    public Sprite indoor;
    public Sprite outdoor;
    public Sprite cat;
    public Sprite ribborn_camera;
    public Sprite diary;

    public Image Chara;
    public Image CameraImage;
    public Image AddBackGround;

    // Start is called before the first frame update
    void Start()
    {
        //image = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    }

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

    public void FadaIn(string ImageName)
    {
        if (ImageName == "AddBackGround")
        {
            AddBackGround.DOFade(0.8f, 0.5f);
        }
    }


}
