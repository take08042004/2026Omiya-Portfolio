using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource seSource;

    [Header("BGM Clips")]
    public AudioClip bgmNormal;
    public AudioClip bgmBattle;
    public AudioClip bgmWinFirst;
    public AudioClip bgmLoseFirst;
    public AudioClip bgmResult;

    [Header("SE Clips")]
    public AudioClip seRollLoop;
    public AudioClip seRollDecide;
    public AudioClip seButtonClick;
    public AudioClip seGuard;
    public List<AudioClip> seAttackList = new List<AudioClip>(); // 攻撃音7種をインスペクターで登録

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- BGM制御 ---
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource?.Stop();

    // --- SE制御 ---
    public void PlaySE(AudioClip clip)
    {
        if (seSource == null || clip == null) return;
        seSource.PlayOneShot(clip);
    }

    // ロール音などのループSE用
    public void PlayLoopSE(AudioClip clip)
    {
        if (seSource == null || clip == null) return;
        seSource.clip = clip;
        seSource.loop = true;
        seSource.Play();
    }

    public void StopSE()
    {
        if (seSource == null) return;
        seSource.Stop();
        seSource.loop = false;
    }
}