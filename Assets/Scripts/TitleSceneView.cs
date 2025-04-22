using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneView : MonoBehaviour
{
  public void OnStartButton()
  {
    if (TransitionMotionManager.Instance.IsTransitioning) return;
    AudioManager.Instance.StopBgm();
    TransitionMotionManager.Instance.PlayTransitionMotion("Battle", TransitionMotionManager.TransitionMotionTypes.FadeNormal).Forget();
  }

  private void Start()
  {
    AudioManager.Instance.PlayBgm("title");
  }
}
