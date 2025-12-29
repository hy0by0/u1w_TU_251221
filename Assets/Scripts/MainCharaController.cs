using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCharaController : MonoBehaviour
{
    public Sprite normal;
    public Sprite invisible;
    public Sprite ribborn;
    public Sprite ribborn_invisible;

    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSprite(string name)
    {
        if (name == "ribborn_invisible")
        {
            image.sprite = ribborn_invisible;
        }
        else if (name == "invisible")
        {
            image.sprite = invisible;
        }
        else if (name == "ribborn")
        {
            image.sprite = ribborn;
        }
    }
}
