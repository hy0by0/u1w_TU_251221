using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// シーン移動関数
    /// </summary>
    /// <param name="name">移動先シーン名。デフォルト:サンプルシーン</param>
    public void Load(string name)
    {
        if (name == null)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene(name);
        }

    }

}
