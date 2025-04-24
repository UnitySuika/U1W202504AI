using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ClearText : MonoBehaviour
{
  [SerializeField]
  private RectTransform rectTransform;

  private void Start()
  {
    ClearTextAnimation(this.GetCancellationTokenOnDestroy()).Forget();
  }

  private async UniTask ClearTextAnimation(CancellationToken token)
  {
    bool dir = true;
    while (true)
    {
      await rectTransform.transform.DORotate(new Vector3(0, 0, dir ? 360f : -360f), 0.5f)
        .SetRelative()
        .ToUniTask(cancellationToken: token);
      token.ThrowIfCancellationRequested();
      await UniTask.Delay(500, cancellationToken: token);
      token.ThrowIfCancellationRequested();
      dir = !dir;
    }
  }
}
