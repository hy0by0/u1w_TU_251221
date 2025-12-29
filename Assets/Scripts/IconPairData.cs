using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class IconPairData //ScriptableObjectへのパラメータのリスト
{
    public string IconName; //識別ネームタグ。これを用いて処理を変える
    public IconController IconObject; //アイコンオブジェクト
    public GameObject WindowObject; //アイコンに対応するウィンドウオブジェクト
}


