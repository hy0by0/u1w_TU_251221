using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageController : MonoBehaviour
{
    public Sprite normal;
    public Sprite invisible;
    public Sprite ribborn;
    public Sprite ribborn_invisible;

    public Image Chara;
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

    public void ChangeSprite(string name)
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

    public void FadaIn(string ImageName)
    {
        if (ImageName == "AddBackGround")
        {
            AddBackGround.DOFade(0.8f, 0.5f);
        }
    }


}
