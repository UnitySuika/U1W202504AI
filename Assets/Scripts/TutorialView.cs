using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour
{
  [SerializeField]
  private Sprite[] pages;

  [SerializeField]
  private Image pageImage;


  public async UniTask PlayTutorial(CancellationToken token)
  {
    CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());

    Color c = pageImage.color;
    c.a = 1f;
    pageImage.color = c;

    for (int i = 0; i < pages.Length; ++i)
    {
      pageImage.sprite = pages[i];
      await UniTask.WaitWhile(() => Input.GetMouseButton(0), cancellationToken: token);
      token.ThrowIfCancellationRequested();
      await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: token);
      token.ThrowIfCancellationRequested();
      AudioManager.Instance.PlaySe("next_page", false);
    }

    await pageImage.DOFade(0f, 0.5f).ToUniTask(cancellationToken: token);
  }
}
