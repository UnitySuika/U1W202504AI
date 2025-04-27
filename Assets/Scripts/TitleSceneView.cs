using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneView : MonoBehaviour
{
  [SerializeField]
  private Slider masterVolumeSlider;
  [SerializeField]
  private Slider musicVolumeSlider;
  [SerializeField]
  private Slider seVolumeSlider;

  public void OnStartButton()
  {
    if (TransitionMotionManager.Instance.IsTransitioning) return;
    AudioManager.Instance.StopBgm();
    TransitionMotionManager.Instance.PlayTransitionMotion("Battle", TransitionMotionManager.TransitionMotionTypes.FadeNormal).Forget();
  }

  private void Start()
  {
    AudioManager.Instance.PlayBgm("title");

    SetVolumeSliders();

    masterVolumeSlider.onValueChanged.AddListener(value => OnVolumeSliderMoved("master", value));
    musicVolumeSlider.onValueChanged.AddListener(value => OnVolumeSliderMoved("music", value));
    seVolumeSlider.onValueChanged.AddListener(value => OnVolumeSliderMoved("se", value));
  }

  public void SetVolumeSliders()
  {
    masterVolumeSlider.value = PlayerPrefs.GetFloat("master_volume", 0.5f);
    musicVolumeSlider.value = PlayerPrefs.GetFloat("music_volume", 0.5f);
    seVolumeSlider.value = PlayerPrefs.GetFloat("se_volume", 0.5f);
    AudioManager.Instance.SetVolume();
  }

  public void OnVolumeSliderMoved(string id, float value)
  {
    if (id == "master")
    {
      PlayerPrefs.SetFloat("master_volume", value);
    }
    else if (id == "music")
    {
      PlayerPrefs.SetFloat("music_volume", value);
    }
    else if (id == "se")
    {
      PlayerPrefs.SetFloat("se_volume", value);
    }
    AudioManager.Instance.SetVolume();
  }
}
