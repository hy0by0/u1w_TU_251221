using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;
    public AudioClip ClickSE;
    public AudioClip ErrorSE;

    void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBGMVolume(float volume)
    {
        bgmAudioSource.volume = volume * 0.7f;
    }

    public void PlaySound()
    {
        bgmAudioSource.Play();
    }

    // クリップを受け取ってSEを鳴らす
    public void PlaySE(string seName)
    {
        var se = GetSoundName(seName);
        seAudioSource.PlayOneShot(se);
    }

    public AudioClip GetSoundName(string name)
    {
        AudioClip clip = null;

        if (name == "click")
        {
            clip = ClickSE;
        }
        else if (name == "error")
        {
            clip = ErrorSE;
        }
        return clip;
    }


}
