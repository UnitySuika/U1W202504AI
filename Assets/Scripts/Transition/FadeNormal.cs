using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeNormal : TransitionMotionObject
{
  [SerializeField]
  private Image fadeImage;

  public override async UniTask PlayTransitionMotion(string nextSceneName, CancellationToken token)
  {
    token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    await fadeImage.DOFade(1f, 2f).WithCancellation(token);
    token.ThrowIfCancellationRequested();
    SceneManager.LoadScene(nextSceneName);
    await fadeImage.DOFade(0f, 2f).WithCancellation(token);
    token.ThrowIfCancellationRequested();
  }
}
