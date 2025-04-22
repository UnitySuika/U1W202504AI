using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Horizontal : TransitionMotionObject
{
  [SerializeField]
  private RectTransform image;

  public override async UniTask PlayTransitionMotion(string nextSceneName, CancellationToken token)
  {
    token = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;
    await image.DOAnchorPosX(0f, 1.5f).WithCancellation(token);
    token.ThrowIfCancellationRequested();
    SceneManager.LoadScene(nextSceneName);
    await image.DOAnchorPosX(1920f, 1.5f).WithCancellation(token);
    token.ThrowIfCancellationRequested();
  }
}
