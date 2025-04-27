using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class AudioManager : MonoBehaviour
{
  [Serializable]
  public class AudioClipItem
  {
    public string Id;
    public AudioClip Clip;
  }

  [SerializeField]
  private float _defaultFadeBgmDuration;

  [SerializeField]
  private float _defaultVolumeBgm;

  
  [SerializeField]
  private float _defaultVolumeSe;

  [SerializeField]
  private AudioSource[] _sourceBgms;

  [SerializeField]
  private AudioSource[] _sourceSes;

  [SerializeField]
  private AudioClipItem[] _clipItems;

  public static AudioManager Instance;

  private AudioSource _currentBgmSource;
  private AudioSource _anotherBgmSource;

  private Tween[] _currentBgmTweens;

  private void Awake()
  {
    if (Instance)
    {
      Destroy(gameObject);
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }

  private AudioClip FindClip(string clipId)
  {
    return Array.Find(_clipItems, item => item.Id == clipId).Clip;
  }

  /// <summary>
  /// BGMを再生
  /// </summary>
  /// <param name="clipId">クリップのID</param>
  public void PlayBgm(string clipId)
  {
    PlayBgm(clipId, _defaultFadeBgmDuration, _defaultVolumeBgm);
  }
  
  /// <summary>
  /// BGMを再生
  /// </summary>
  /// <param name="clipId">クリップのID</param>
  /// <param name="fadeBgmDuration">BGMのクロスフェード時間</param>
  /// <param name="volumeBgm">新BGMのボリューム</param>
  public void PlayBgm(string clipId, float fadeBgmDuration, float volumeBgm)
  {
    if (_currentBgmTweens != null)
    {
      foreach (Tween t in _currentBgmTweens) t?.Kill();
      _anotherBgmSource.volume = 0f;
    }
    _currentBgmTweens = new Tween[2];
    
    if (_currentBgmSource != null)
    {
      _currentBgmTweens[0] = _currentBgmSource.DOFade(0f, fadeBgmDuration);

      AudioSource tmp = _anotherBgmSource;
      _anotherBgmSource = _currentBgmSource;
      _currentBgmSource = tmp;
    }
    else
    {
      _currentBgmSource = _sourceBgms[0];
      _anotherBgmSource = _sourceBgms[1];
    }
    
    _currentBgmSource.clip = FindClip(clipId);
    _currentBgmSource.Play();
    
    _currentBgmSource.DOFade(volumeBgm, fadeBgmDuration);
  }

  /// <summary>
  /// BGMを止める
  /// </summary>
  public void StopBgm()
  {
    StopBgm(_defaultFadeBgmDuration);
  }

  /// <summary>
  /// BGMを止める
  /// </summary>
  /// <param name="fadeBgmDuration">消えるまでの時間</param>
  public void StopBgm(float fadeBgmDuration)
  {
    if (_currentBgmSource == null) return;

    if (_currentBgmTweens != null)
    {
      foreach (Tween t in _currentBgmTweens) t?.Kill();
    }
    _currentBgmTweens = new Tween[2];
    
    _currentBgmTweens[0] = _currentBgmSource.DOFade(0f, fadeBgmDuration);
    _currentBgmTweens[1] = _anotherBgmSource.DOFade(0f, fadeBgmDuration);
  }

  /// <summary>
  /// SEを再生
  /// </summary>
  /// <param name="clipId">クリップのID</param>
  /// <param name="isOverride">同じクリップが再生中なら上書き再生するか</param>
  public void PlaySe(string clipId, bool isOverride)
  {
    bool isFull = true;
    int maxTimeIndex = -1;

    for (int i = 0; i < _sourceSes.Length; ++i)
    {
      bool isAudioEmpty = _sourceSes[i].clip == null || !_sourceSes[i].isPlaying;
      bool canOverride = isOverride && _sourceSes[i].isPlaying && _sourceSes[i].clip == FindClip(clipId);
      if (isAudioEmpty || canOverride)
      {
        isFull = false;
        _sourceSes[i].Stop();
        _sourceSes[i].clip = FindClip(clipId);
        _sourceSes[i].volume = _defaultVolumeSe;
        _sourceSes[i].Play();
        // 修正済み
        break;
      }
      else
      {
        if (maxTimeIndex == -1 || _sourceSes[i].time > _sourceSes[maxTimeIndex].time)
        {
          maxTimeIndex = i;
        }
      }
    }
    
    if (isFull)
    {
        _sourceSes[maxTimeIndex].Stop();
        _sourceSes[maxTimeIndex].clip = FindClip(clipId);
        _sourceSes[maxTimeIndex].volume = _defaultVolumeSe;
        _sourceSes[maxTimeIndex].Play();
    }
  }

  public void SetVolume()
  {
    AudioListener.volume = PlayerPrefs.GetFloat("master_volume", 0.5f);
    _defaultVolumeBgm = PlayerPrefs.GetFloat("music_volume", 0.5f);
    foreach (AudioSource source in _sourceBgms)
    {
      source.volume = _defaultVolumeBgm;
    }
    _defaultVolumeSe = PlayerPrefs.GetFloat("se_volume", 0.5f);
    foreach (AudioSource source in _sourceSes)
    {
      source.volume = _defaultVolumeSe;
    }
  }
}
