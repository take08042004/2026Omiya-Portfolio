using System.Reflection;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class OpeningScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoURL;  //後からURLを指定する

    private HandInput handInput;

    // 「グー」の検知用

    private bool wasFist = false;
    void Start()
    {
        //HandInputの取得
        handInput = FindFirstObjectByType<HandInput>();

        //動画再生パート
        videoPlayer.url = videoURL;

        videoPlayer.Play();
        
    }

    void Update()
    {
        if(handInput == null) return;

        bool isFist = handInput.IsFist();

        if (!wasFist && isFist)
        {
            OnFistDetected();
        }

        wasFist = isFist;
    }

    private void OnFistDetected()
    {
        gameObject.SetActive(false);
        GameManager.Instance.OnOpeningFinished();
    }
}