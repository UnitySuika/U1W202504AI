using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
  [SerializeField]
  private TutorialView tutorialView;

  public void PlayTutorial()
  {
    PlayTutorialTask(this.GetCancellationTokenOnDestroy()).Forget();
  }

  private async UniTask PlayTutorialTask(CancellationToken token)
  {
    tutorialView.gameObject.SetActive(true);
    await tutorialView.PlayTutorial(token);
    token.ThrowIfCancellationRequested();
    tutorialView.gameObject.SetActive(false);
  }
}
