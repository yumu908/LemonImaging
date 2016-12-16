using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MoviePlayer : MonoBehaviour {

    //电影纹理  
    public MovieTexture movTexture;
    public AudioClip movAudio;

    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite audioPlaySprite;
    public Sprite audioPauseSprite;
    public Image image;
    private bool isPlay;
    private bool audioIsPlay;
    public Image audioImage;

    private AudioSource audioSource;

    void Start()
    {
        //设置当前对象的主纹理为电影纹理  
        GetComponent<Renderer>().material.mainTexture = movTexture;
        movTexture.loop = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = movAudio;
        audioSource.loop = true;
        isPlay = false;
        audioIsPlay = false;
        image.sprite = playSprite;
        audioImage.sprite = audioPlaySprite;
    }

    public void Play(bool flag)
    {
        isPlay = flag;
        //Debug.LogWarning("Play : " + isPlay);
        if (isPlay)
        {
            image.sprite = pauseSprite;
            if (!movTexture.isPlaying)
            {
                movTexture.Play();
                audioSource.Play();
            }
        }
        else
        {
            image.sprite = playSprite;

            //暂停播放  
            movTexture.Pause();
            audioSource.Pause();
            image.sprite = playSprite;
        }
    }


    public void PlayAudio(bool flag)
    {
        audioIsPlay = flag;
        if (audioIsPlay)
        {
            audioImage.sprite = audioPauseSprite;
            audioSource.mute = false;
        }
        else
        {
            audioImage.sprite = audioPlaySprite;
            audioSource.mute = true;
        }
    }

    public void Stop()
    {
        //停止播放  
        movTexture.Stop();
        audioSource.Stop();

        isPlay = false;
        image.sprite = playSprite;
    }
}
