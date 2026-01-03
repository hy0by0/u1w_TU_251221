using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperLink : MonoBehaviour
{
    public string url;

    public void clickWeb()
    {
        Application.OpenURL(url);
    }

}
