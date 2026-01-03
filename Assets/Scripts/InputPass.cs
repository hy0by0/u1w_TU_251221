using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputPass : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI passText;
    public bool isPass = false;
    public GameObject keyboardPanel;

    // Start is called before the first frame update
    void Start()
    {
        isPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenKeyboard()
    {
        keyboardPanel.SetActive(true);
    }

    public void NotPass()
    {
        isPass = false;
        passText.text = "*";
        passText.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        keyboardPanel.SetActive(false);
    }

    public void CorrectPass()
    {
        isPass = true;
        passText.text = "*";
        passText.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        keyboardPanel.SetActive(false);
    }

}
