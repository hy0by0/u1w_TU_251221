using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCSound : MonoBehaviour
{
    private AudioSource audio;
    public GameObject PCsound2;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.Play();

        StartCoroutine(ChangeClip());
    }

    public delegate void functionType();
    private IEnumerator ChangeClip()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!audio.isPlaying)
            {
                PCsound2.SetActive(true);
                this.gameObject.SetActive(false);
                break;
            }
        }
    }
}
